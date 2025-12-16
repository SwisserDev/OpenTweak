using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Disables Windows Power Throttling to prevent CPU performance reduction during gaming.
/// </summary>
public class PowerThrottlingService : TweakServiceBase
{
    private const string PowerThrottlingPath = @"SYSTEM\CurrentControlSet\Control\Power\PowerThrottling";

    private readonly List<RegistryBackup> _backups = new();

    public override TweakInfo Info => new()
    {
        Id = "power-throttling",
        Name = "Power Throttling Disable",
        Description = "Prevents Windows from throttling CPU performance",
        Category = "Power",
        TechnicalDetails = """
            Disables Windows Power Throttling feature which reduces CPU
            performance to save power.

            What it actually does:
            - Prevents Windows from reducing CPU clock speeds
            - Stops background app throttling
            - Ensures maximum CPU performance during gaming

            Reality check:
            - Very effective on laptops with aggressive power management
            - Less impact on desktops with high-performance power plans
            - Will increase power consumption and heat
            - Combined with Ultimate Performance plan for best results
            - Can prevent GPU bottlenecking caused by throttled CPU
            """,
        WhatItDoes = """
            Registry: HKLM\SYSTEM\CurrentControlSet\Control\Power\PowerThrottling
            - PowerThrottlingOff = 1 (disabled)

            0 = Power throttling enabled (default)
            1 = Power throttling disabled
            """,
        Effectiveness = EffectivenessRating.Effective,
        RecommendedByDefault = true,
        RequiresRestart = false
    };

    public override Task<bool> IsAppliedAsync()
    {
        var value = RegistryHelper.GetValue(PowerThrottlingPath, "PowerThrottlingOff", RegistryHive.LocalMachine);

        // Throttling is disabled if PowerThrottlingOff = 1
        return Task.FromResult(value is int v && v == 1);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            _backups.Clear();

            var backup = RegistryHelper.BackupValue(PowerThrottlingPath, "PowerThrottlingOff", RegistryHive.LocalMachine);
            if (backup != null) _backups.Add(backup);

            if (RegistryHelper.SetValue(PowerThrottlingPath, "PowerThrottlingOff", 1,
                RegistryValueKind.DWord, RegistryHive.LocalMachine))
            {
                Log("Set PowerThrottlingOff = 1");
                return Task.FromResult(TweakResult.Ok("Power throttling disabled"));
            }

            return Task.FromResult(TweakResult.Fail(
                "Could not disable power throttling",
                "Registry access may be restricted."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to disable power throttling", ex.Message));
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

            // If no backup, enable throttling
            if (_backups.Count == 0)
            {
                RegistryHelper.SetValue(PowerThrottlingPath, "PowerThrottlingOff", 0,
                    RegistryValueKind.DWord, RegistryHive.LocalMachine);
            }

            return Task.FromResult(TweakResult.Ok("Power throttling re-enabled"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to restore power throttling", ex.Message));
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var isApplied = await IsAppliedAsync();
        return isApplied ? "Disabled (Full Power)" : "Enabled (Power Saving)";
    }
}
