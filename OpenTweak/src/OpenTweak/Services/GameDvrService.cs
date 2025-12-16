using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Disables Xbox Game DVR and Game Bar recording features.
/// </summary>
public class GameDvrService : TweakServiceBase
{
    private const string GameConfigStorePath = @"System\GameConfigStore";
    private const string GameDvrPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR";

    private readonly List<RegistryBackup> _backups = new();

    public override TweakInfo Info => new()
    {
        Id = "game-dvr",
        Name = "Game DVR Disable",
        Description = "Disables Xbox Game Bar recording overlay",
        Category = "Windows",
        TechnicalDetails = """
            Disables Xbox Game Bar and Game DVR background recording.

            What it actually does:
            - Stops background video recording
            - Removes the Game Bar overlay (Win+G)
            - Saves ~1-2% CPU when recording would be active

            Reality check:
            - If you don't use Game Bar recording, this is harmless
            - Modern PCs barely notice the overhead
            - Some games may actually benefit from Game Mode features
            - You can still use other recording software (OBS, etc.)
            """,
        WhatItDoes = """
            Registry: HKCU\System\GameConfigStore
            - GameDVR_Enabled = 0
            - GameDVR_FSEBehaviorMode = 2
            - GameDVR_HonorUserFSEBehaviorMode = 1

            Registry: HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR
            - AppCaptureEnabled = 0
            """,
        Effectiveness = EffectivenessRating.Minimal,
        RecommendedByDefault = false,
        RequiresRestart = false
    };

    public override Task<bool> IsAppliedAsync()
    {
        var gameDvrEnabled = RegistryHelper.GetValue(GameConfigStorePath, "GameDVR_Enabled", RegistryHive.CurrentUser);
        var appCaptureEnabled = RegistryHelper.GetValue(GameDvrPath, "AppCaptureEnabled", RegistryHive.CurrentUser);

        // Disabled if GameDVR_Enabled = 0 or AppCaptureEnabled = 0
        bool isDisabled = (gameDvrEnabled is int gdv && gdv == 0) ||
                          (appCaptureEnabled is int ace && ace == 0);

        return Task.FromResult(isDisabled);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            _backups.Clear();

            // Backup and set GameConfigStore values
            var settings = new (string path, string name, object value, RegistryValueKind kind)[]
            {
                (GameConfigStorePath, "GameDVR_Enabled", 0, RegistryValueKind.DWord),
                (GameConfigStorePath, "GameDVR_FSEBehaviorMode", 2, RegistryValueKind.DWord),
                (GameConfigStorePath, "GameDVR_HonorUserFSEBehaviorMode", 1, RegistryValueKind.DWord),
                (GameDvrPath, "AppCaptureEnabled", 0, RegistryValueKind.DWord),
            };

            int applied = 0;
            foreach (var (path, name, value, kind) in settings)
            {
                var backup = RegistryHelper.BackupValue(path, name, RegistryHive.CurrentUser);
                if (backup != null) _backups.Add(backup);

                if (RegistryHelper.SetValue(path, name, value, kind, RegistryHive.CurrentUser))
                {
                    Log($"Set {name} = {value}");
                    applied++;
                }
            }

            if (applied == 0)
            {
                return Task.FromResult(TweakResult.Fail(
                    "Could not disable Game DVR",
                    "Registry access may be restricted"));
            }

            return Task.FromResult(TweakResult.Ok(
                $"Game DVR disabled ({applied} settings changed)"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to disable Game DVR", ex.Message));
        }
    }

    public override Task<TweakResult> RevertAsync()
    {
        try
        {
            int restored = 0;
            foreach (var backup in _backups)
            {
                if (!backup.Existed)
                {
                    RegistryHelper.DeleteValue(backup.KeyPath, backup.ValueName, backup.Hive);
                }
                else
                {
                    RegistryHelper.RestoreBackup(backup);
                }
                Log($"Restored {backup.ValueName}");
                restored++;
            }

            return Task.FromResult(TweakResult.Ok($"Restored {restored} Game DVR setting(s)"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to restore Game DVR settings", ex.Message));
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var isApplied = await IsAppliedAsync();
        return isApplied ? "Disabled" : "Enabled";
    }
}
