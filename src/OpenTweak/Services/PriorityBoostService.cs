using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Modifies Win32PrioritySeparation - PLACEBO tweak for educational purposes.
/// </summary>
public class PriorityBoostService : TweakServiceBase
{
    private const string PriorityControlPath = @"SYSTEM\CurrentControlSet\Control\PriorityControl";

    // "Gaming optimized" value commonly advertised: 0x26 (38 decimal)
    // Means: Short quantum, variable, foreground boost 2:1
    private const int OptimizedValue = 0x26;

    private readonly List<RegistryBackup> _backups = new();

    public override TweakInfo Info => new()
    {
        Id = "priority-boost",
        Name = "Priority Boost 🎭",
        Description = "Modifies Win32PrioritySeparation (Placebo)",
        Category = "Placebo",
        TechnicalDetails = """
            This is a PLACEBO tweak commonly sold in "FPS boosters".

            What sellers claim:
            - "Optimizes CPU scheduling for gaming"
            - "Gives more CPU time to foreground apps"
            - "Reduces background process interference"

            What it actually does:
            - Changes how Windows schedules foreground vs background threads
            - Common "gaming" value is 0x26 or 0x2A
            - Windows default (0x02) is already optimized for desktop use

            Reality check:
            - Modern Windows already prioritizes foreground apps heavily
            - Benchmarks show NO measurable FPS difference
            - Can actually cause problems with background tasks
            - The values circulating online are cargo cult "optimizations"
            - Different "guides" recommend different values - because none matter

            Various values you'll see recommended:
            - 0x26 = "Gaming optimized" (no evidence)
            - 0x2A = "Server-style" (makes no sense for desktop)
            - 0x28 = "Streaming optimized" (also baseless)

            We include this to show another "FPS booster" snake oil tweak.
            """,
        WhatItDoes = """
            Registry: HKLM\SYSTEM\CurrentControlSet\Control\PriorityControl
            - Win32PrioritySeparation = 0x26

            This DWORD controls:
            - Bits 0-1: Priority separation (0-3)
            - Bits 2-3: Quantum length (short/long)
            - Bits 4-5: Fixed vs variable quantums

            Default is 0x02 which is perfectly fine for gaming.
            """,
        Effectiveness = EffectivenessRating.Placebo,
        RecommendedByDefault = false,
        RequiresRestart = true
    };

    public override Task<bool> IsAppliedAsync()
    {
        var value = RegistryHelper.GetValue(PriorityControlPath, "Win32PrioritySeparation", RegistryHive.LocalMachine);

        // Check if set to "optimized" value
        bool isApplied = value is int priority && priority == OptimizedValue;

        return Task.FromResult(isApplied);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            _backups.Clear();

            var backup = RegistryHelper.BackupValue(PriorityControlPath, "Win32PrioritySeparation", RegistryHive.LocalMachine);
            if (backup != null) _backups.Add(backup);

            if (RegistryHelper.SetValue(
                PriorityControlPath,
                "Win32PrioritySeparation",
                OptimizedValue,
                RegistryValueKind.DWord,
                RegistryHive.LocalMachine))
            {
                Log($"Set Win32PrioritySeparation = 0x{OptimizedValue:X2}");
                return Task.FromResult(TweakResult.Ok(
                    "Priority Boost applied - Note: This is placebo. Restart required."));
            }

            return Task.FromResult(TweakResult.Fail(
                "Could not modify Priority settings",
                "Requires Administrator rights (HKLM)"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to modify Priority settings", ex.Message));
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
                    // Set back to Windows default (0x02)
                    RegistryHelper.SetValue(
                        PriorityControlPath,
                        "Win32PrioritySeparation",
                        0x02,
                        RegistryValueKind.DWord,
                        RegistryHive.LocalMachine);
                    Log("Reset Win32PrioritySeparation to default (0x02)");
                }
                else
                {
                    RegistryHelper.RestoreBackup(backup);
                    Log("Restored Win32PrioritySeparation");
                }
            }

            return Task.FromResult(TweakResult.Ok("Restored Priority setting. Restart required."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to restore Priority setting", ex.Message));
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var isApplied = await IsAppliedAsync();
        if (isApplied)
            return $"0x{OptimizedValue:X2} (Placebo)";

        var value = RegistryHelper.GetValue(PriorityControlPath, "Win32PrioritySeparation", RegistryHive.LocalMachine);
        if (value is int priority)
            return $"0x{priority:X2} (Default)";

        return "Unknown";
    }
}
