using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Adjusts Windows system responsiveness to prioritize foreground applications.
/// </summary>
public class SystemResponsivenessService : TweakServiceBase
{
    private const string MultimediaPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";

    private readonly List<RegistryBackup> _backups = new();

    public override TweakInfo Info => new()
    {
        Id = "system-responsiveness",
        Name = "System Responsiveness",
        Description = "Reduces background task CPU reservation",
        Category = "System",
        TechnicalDetails = """
            Adjusts the SystemResponsiveness value which controls how much
            CPU time is reserved for background system tasks.

            What it actually does:
            - Default value is 20 (20% reserved for background)
            - Setting to 10 reduces background reservation
            - Allows foreground apps (games) to use more CPU

            Reality check:
            - May provide slight responsiveness improvement
            - Effect is minimal on modern multi-core CPUs
            - Could affect background downloads/updates
            - Part of the Multimedia Class Scheduler Service (MMCSS)
            """,
        WhatItDoes = """
            Registry: HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile
            - SystemResponsiveness = 10 (default: 20)

            Lower values = more resources for foreground apps
            """,
        Effectiveness = EffectivenessRating.Minimal,
        RecommendedByDefault = true,
        RequiresRestart = false
    };

    public override Task<bool> IsAppliedAsync()
    {
        var value = RegistryHelper.GetValue(MultimediaPath, "SystemResponsiveness", RegistryHive.LocalMachine);

        // Applied if value is 10 or lower
        return Task.FromResult(value is int v && v <= 10);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            _backups.Clear();

            var backup = RegistryHelper.BackupValue(MultimediaPath, "SystemResponsiveness", RegistryHive.LocalMachine);
            if (backup != null) _backups.Add(backup);

            if (RegistryHelper.SetValue(MultimediaPath, "SystemResponsiveness", 10,
                RegistryValueKind.DWord, RegistryHive.LocalMachine))
            {
                Log("Set SystemResponsiveness = 10");
                return Task.FromResult(TweakResult.Ok("System responsiveness optimized"));
            }

            return Task.FromResult(TweakResult.Fail(
                "Could not set system responsiveness",
                "Registry access may be restricted."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to optimize system responsiveness", ex.Message));
        }
    }

    public override Task<TweakResult> RevertAsync()
    {
        try
        {
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
            }

            // If no backup, restore default value
            if (_backups.Count == 0)
            {
                RegistryHelper.SetValue(MultimediaPath, "SystemResponsiveness", 20,
                    RegistryValueKind.DWord, RegistryHive.LocalMachine);
            }

            return Task.FromResult(TweakResult.Ok("System responsiveness restored to default"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to restore system responsiveness", ex.Message));
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var value = RegistryHelper.GetValue(MultimediaPath, "SystemResponsiveness", RegistryHive.LocalMachine);
        if (value is int v)
            return $"{v}% reserved";
        return "Default";
    }
}
