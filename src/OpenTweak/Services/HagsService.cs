using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Enables Hardware Accelerated GPU Scheduling (HAGS).
/// Reduces CPU load by letting the GPU manage its own memory scheduling.
/// </summary>
public class HagsService : TweakServiceBase
{
    private const string GraphicsDriversPath = @"SYSTEM\CurrentControlSet\Control\GraphicsDrivers";

    private readonly List<RegistryBackup> _backups = new();

    public override TweakInfo Info => new()
    {
        Id = "hags",
        Name = "GPU Scheduling (HAGS)",
        Description = "Enables Hardware Accelerated GPU Scheduling",
        Category = "GPU",
        TechnicalDetails = """
            Enables Hardware Accelerated GPU Scheduling (HAGS), which lets
            the GPU manage its own VRAM scheduling instead of the CPU.

            What it actually does:
            - Offloads GPU scheduling from CPU to GPU's dedicated processor
            - Can reduce input latency in some scenarios
            - Slightly reduces CPU overhead during GPU-intensive tasks

            Reality check:
            - Mixed results - some games benefit, others don't
            - May cause issues with OBS and screen recording
            - Requires WDDM 2.7+ compatible GPU (GTX 1000 series or newer)
            - Windows 10 2004+ or Windows 11 required
            - Test per-game to see if it helps
            """,
        WhatItDoes = """
            Registry: HKLM\SYSTEM\CurrentControlSet\Control\GraphicsDrivers
            - HwSchMode = 2 (enabled)

            Values:
            - 1 = Disabled
            - 2 = Enabled
            """,
        Effectiveness = EffectivenessRating.Minimal,
        RecommendedByDefault = false,
        RequiresRestart = true
    };

    public override Task<bool> IsAppliedAsync()
    {
        var hwSchMode = RegistryHelper.GetValue(GraphicsDriversPath, "HwSchMode", RegistryHive.LocalMachine);

        // HAGS is enabled if HwSchMode = 2
        return Task.FromResult(hwSchMode is int mode && mode == 2);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            _backups.Clear();

            var backup = RegistryHelper.BackupValue(GraphicsDriversPath, "HwSchMode", RegistryHive.LocalMachine);
            if (backup != null) _backups.Add(backup);

            if (RegistryHelper.SetValue(GraphicsDriversPath, "HwSchMode", 2,
                RegistryValueKind.DWord, RegistryHive.LocalMachine))
            {
                Log("Set HwSchMode = 2 (HAGS enabled)");
                return Task.FromResult(TweakResult.Ok("HAGS enabled. Restart required."));
            }

            return Task.FromResult(TweakResult.Fail(
                "Could not enable HAGS",
                "Registry access may be restricted or GPU doesn't support HAGS."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to enable HAGS", ex.Message));
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

            // If no backup, set to disabled
            if (_backups.Count == 0)
            {
                RegistryHelper.SetValue(GraphicsDriversPath, "HwSchMode", 1,
                    RegistryValueKind.DWord, RegistryHive.LocalMachine);
            }

            return Task.FromResult(TweakResult.Ok("HAGS disabled. Restart required."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to disable HAGS", ex.Message));
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var isApplied = await IsAppliedAsync();
        return isApplied ? "Enabled" : "Disabled";
    }
}
