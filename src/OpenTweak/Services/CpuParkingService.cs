using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Disables CPU core parking to keep all cores active.
/// </summary>
public class CpuParkingService : TweakServiceBase
{
    // Power Settings GUIDs
    private const string ProcessorPowerSubgroup = "54533251-82be-4824-96c1-47b60b740d00";
    private const string CoreParkingMinCores = "0cc5b647-c1df-4637-891a-dec35c318583";

    private readonly List<RegistryBackup> _backups = new();

    public override TweakInfo Info => new()
    {
        Id = "cpu-parking",
        Name = "CPU Unparking",
        Description = "Prevents Windows from parking CPU cores",
        Category = "CPU",
        TechnicalDetails = """
            Core Parking is a Windows feature that puts idle CPU cores into a
            low-power state. Disabling it keeps all cores active.

            Reality check:
            - Modern Windows (10/11) already unparks cores when needed
            - Gaming workloads typically use multiple cores anyway
            - May slightly increase idle power consumption
            - Placebo for most gaming scenarios

            The "Core 0 bottleneck" myth: GTA V does have single-threaded
            bottlenecks, but unparking doesn't help with that - the render
            thread is already running on an active core.
            """,
        WhatItDoes = """
            Registry: HKLM\SYSTEM\CurrentControlSet\Control\Power\PowerSettings\
                      {54533251-82be-4824-96c1-47b60b740d00}\
                      {0cc5b647-c1df-4637-891a-dec35c318583}
            Sets ValueMin and ValueMax to 100 (all cores active)
            """,
        Effectiveness = EffectivenessRating.Placebo,
        RecommendedByDefault = false,
        RequiresRestart = false
    };

    public override Task<bool> IsAppliedAsync()
    {
        var keyPath = $@"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\{ProcessorPowerSubgroup}\{CoreParkingMinCores}";

        var valueMin = RegistryHelper.GetValue(keyPath, "ValueMin");
        var valueMax = RegistryHelper.GetValue(keyPath, "ValueMax");

        // Check if both are set to 100 (all cores)
        bool isApplied = (valueMin is int min && min == 100) &&
                         (valueMax is int max && max == 100);

        return Task.FromResult(isApplied);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            var keyPath = $@"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\{ProcessorPowerSubgroup}\{CoreParkingMinCores}";

            // Backup current values
            _backups.Clear();
            var backupMin = RegistryHelper.BackupValue(keyPath, "ValueMin");
            var backupMax = RegistryHelper.BackupValue(keyPath, "ValueMax");
            if (backupMin != null) _backups.Add(backupMin);
            if (backupMax != null) _backups.Add(backupMax);

            Log($"Backed up original values: Min={backupMin?.OriginalValue}, Max={backupMax?.OriginalValue}");

            // Set to 100% (all cores active)
            var successMin = RegistryHelper.SetValue(keyPath, "ValueMin", 100, RegistryValueKind.DWord);
            var successMax = RegistryHelper.SetValue(keyPath, "ValueMax", 100, RegistryValueKind.DWord);

            if (!successMin || !successMax)
            {
                return Task.FromResult(TweakResult.Fail(
                    "Could not write registry values",
                    "Make sure you're running as Administrator"));
            }

            Log("Set CPU core parking minimum to 100%");

            // Also apply via powercfg for immediate effect
            _ = ProcessHelper.RunCommandAsync("powercfg",
                $"/setacvalueindex scheme_current {ProcessorPowerSubgroup} {CoreParkingMinCores} 100");
            _ = ProcessHelper.RunCommandAsync("powercfg", "/setactive scheme_current");

            return Task.FromResult(TweakResult.Ok("CPU core parking disabled - all cores will stay active"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to disable CPU parking", ex.Message));
        }
    }

    public override Task<TweakResult> RevertAsync()
    {
        try
        {
            foreach (var backup in _backups)
            {
                RegistryHelper.RestoreBackup(backup);
                Log($"Restored {backup.ValueName} to {backup.OriginalValue}");
            }

            // Apply default via powercfg
            _ = ProcessHelper.RunCommandAsync("powercfg",
                $"/setacvalueindex scheme_current {ProcessorPowerSubgroup} {CoreParkingMinCores} 0");
            _ = ProcessHelper.RunCommandAsync("powercfg", "/setactive scheme_current");

            return Task.FromResult(TweakResult.Ok("CPU parking settings restored"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to restore CPU parking", ex.Message));
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var isApplied = await IsAppliedAsync();
        return isApplied ? "All cores active" : "Default (parking enabled)";
    }
}
