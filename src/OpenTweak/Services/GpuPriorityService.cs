using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Sets GPU preference to High Performance for GTA V and FiveM.
/// </summary>
public class GpuPriorityService : TweakServiceBase
{
    private const string GpuPreferencePath = @"Software\Microsoft\DirectX\UserGpuPreferences";
    private static readonly string[] TargetExecutables = { "GTA5.exe", "FiveM.exe", "FiveM_GTAProcess.exe" };

    private readonly List<RegistryBackup> _backups = new();

    public override TweakInfo Info => new()
    {
        Id = "gpu-priority",
        Name = "GPU High Performance",
        Description = "Forces dedicated GPU for GTA V and FiveM",
        Category = "GPU",
        TechnicalDetails = """
            Tells Windows to always use the high-performance GPU for GTA V.
            This is relevant for laptops with integrated + dedicated graphics.

            Reality check:
            - Desktop PCs with one GPU: No effect
            - Laptops: Can help if Windows is using integrated graphics by mistake
            - Most users already have this set correctly
            - GTA V typically requests the dedicated GPU automatically

            This setting mirrors what you can set in Windows Settings →
            Graphics Settings → App preference.
            """,
        WhatItDoes = """
            Registry: HKCU\Software\Microsoft\DirectX\UserGpuPreferences
            - "GTA5.exe" = "GpuPreference=2;"
            - "FiveM.exe" = "GpuPreference=2;"
            - "FiveM_GTAProcess.exe" = "GpuPreference=2;"

            GpuPreference values:
            0 = Let Windows decide
            1 = Power saving (integrated)
            2 = High performance (dedicated)
            """,
        Effectiveness = EffectivenessRating.Minimal,
        RecommendedByDefault = true,
        RequiresRestart = false
    };

    public override Task<bool> IsAppliedAsync()
    {
        foreach (var exe in TargetExecutables)
        {
            var value = RegistryHelper.GetValue(GpuPreferencePath, exe, RegistryHive.CurrentUser);
            if (value is string pref && pref.Contains("GpuPreference=2"))
                return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            _backups.Clear();
            int applied = 0;

            foreach (var exe in TargetExecutables)
            {
                // Backup existing
                var backup = RegistryHelper.BackupValue(GpuPreferencePath, exe, RegistryHive.CurrentUser);
                if (backup != null) _backups.Add(backup);

                // Set high performance
                var success = RegistryHelper.SetValue(
                    GpuPreferencePath,
                    exe,
                    "GpuPreference=2;",
                    RegistryValueKind.String,
                    RegistryHive.CurrentUser);

                if (success)
                {
                    Log($"Set GPU preference for {exe}");
                    applied++;
                }
            }

            if (applied == 0)
            {
                return Task.FromResult(TweakResult.Fail(
                    "Could not set GPU preferences",
                    "Registry access may be restricted"));
            }

            return Task.FromResult(TweakResult.Ok(
                $"GPU set to High Performance for {applied} executable(s)"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to set GPU preference", ex.Message));
        }
    }

    public override Task<TweakResult> RevertAsync()
    {
        try
        {
            int restored = 0;
            foreach (var backup in _backups)
            {
                // If it didn't exist before, delete the value
                if (!backup.Existed)
                {
                    RegistryHelper.DeleteValue(backup.KeyPath, backup.ValueName, backup.Hive);
                    Log($"Removed GPU preference for {backup.ValueName}");
                }
                else
                {
                    RegistryHelper.RestoreBackup(backup);
                    Log($"Restored GPU preference for {backup.ValueName}");
                }
                restored++;
            }

            return Task.FromResult(TweakResult.Ok($"Restored {restored} GPU setting(s)"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to restore GPU settings", ex.Message));
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var isApplied = await IsAppliedAsync();
        return isApplied ? "High Performance" : "Default";
    }
}
