using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Disables Fullscreen Optimizations (FSO) - PLACEBO tweak for educational purposes.
/// </summary>
public class FsoDisableService : TweakServiceBase
{
    private const string GameConfigStorePath = @"System\GameConfigStore";

    private readonly List<RegistryBackup> _backups = new();

    public override TweakInfo Info => new()
    {
        Id = "fso-disable",
        Name = "FSO Disable 🎭",
        Description = "Disables Fullscreen Optimizations (Placebo)",
        Category = "Placebo",
        TechnicalDetails = """
            This is a PLACEBO tweak commonly sold in "FPS boosters".

            What sellers claim:
            - "Reduces input lag"
            - "Improves FPS"
            - "Disables Windows interfering with fullscreen"

            What it actually does:
            - Tells Windows to use "legacy" fullscreen mode
            - In Windows 10/11, FSO is actually beneficial for most games
            - Disabling it can INCREASE input lag in some cases
            - Modern games are designed with FSO in mind

            Reality check:
            - This was useful in Windows 7/8 era
            - Windows 10/11 FSO is highly optimized
            - Benchmark tests show no consistent improvement
            - You're paying for a registry change that does nothing helpful

            We include this tweak to show what "FPS boosters" actually sell.
            """,
        WhatItDoes = """
            Registry: HKCU\System\GameConfigStore
            - GameDVR_FSEBehavior = 2
            - GameDVR_DXGIHonorFSEWindowsCompatible = 1

            These settings tell Windows to use "classic" fullscreen
            instead of the modern optimized borderless mode.
            """,
        Effectiveness = EffectivenessRating.Placebo,
        RecommendedByDefault = false,
        RequiresRestart = false
    };

    public override Task<bool> IsAppliedAsync()
    {
        var fseBehavior = RegistryHelper.GetValue(GameConfigStorePath, "GameDVR_FSEBehavior", RegistryHive.CurrentUser);
        var honorFse = RegistryHelper.GetValue(GameConfigStorePath, "GameDVR_DXGIHonorFSEWindowsCompatible", RegistryHive.CurrentUser);

        bool isDisabled = (fseBehavior is int fse && fse == 2) ||
                          (honorFse is int hfse && hfse == 1);

        return Task.FromResult(isDisabled);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            _backups.Clear();

            var settings = new (string name, int value)[]
            {
                ("GameDVR_FSEBehavior", 2),
                ("GameDVR_DXGIHonorFSEWindowsCompatible", 1),
            };

            int applied = 0;
            foreach (var (name, value) in settings)
            {
                var backup = RegistryHelper.BackupValue(GameConfigStorePath, name, RegistryHive.CurrentUser);
                if (backup != null) _backups.Add(backup);

                if (RegistryHelper.SetValue(GameConfigStorePath, name, value, RegistryValueKind.DWord, RegistryHive.CurrentUser))
                {
                    Log($"Set {name} = {value}");
                    applied++;
                }
            }

            if (applied == 0)
            {
                return Task.FromResult(TweakResult.Fail(
                    "Could not disable FSO",
                    "Registry access may be restricted"));
            }

            return Task.FromResult(TweakResult.Ok(
                $"FSO disabled ({applied} settings) - Note: This is likely placebo"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to disable FSO", ex.Message));
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

            return Task.FromResult(TweakResult.Ok($"Restored {restored} FSO setting(s)"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to restore FSO settings", ex.Message));
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var isApplied = await IsAppliedAsync();
        return isApplied ? "Disabled (Placebo)" : "Enabled (Default)";
    }
}
