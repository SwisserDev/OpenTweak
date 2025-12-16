using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Disables Virtualization-Based Security (VBS) and Hypervisor-Enforced Code Integrity (HVCI).
/// Can improve gaming performance by 5-25% but reduces system security.
/// </summary>
public class VbsDisableService : TweakServiceBase
{
    private const string DeviceGuardPath = @"SYSTEM\CurrentControlSet\Control\DeviceGuard";
    private const string HvciPath = @"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity";

    private readonly List<RegistryBackup> _backups = new();

    public override TweakInfo Info => new()
    {
        Id = "vbs-disable",
        Name = "VBS/HVCI Disable",
        Description = "Disables Virtualization-Based Security for better gaming performance",
        Category = "Security",
        TechnicalDetails = """
            Disables Windows Virtualization-Based Security (VBS) and
            Hypervisor-Enforced Code Integrity (HVCI/Memory Integrity).

            What it actually does:
            - Disables the secure memory enclave that isolates critical code
            - Removes hypervisor-level code integrity checks
            - Can improve FPS by 5-25% depending on hardware and game

            Reality check:
            - This is a REAL performance improvement, especially on older CPUs
            - However, it significantly reduces system security
            - Credential Guard and other security features will be disabled
            - Some anti-cheat software (like Valorant's Vanguard) may require VBS
            - Windows 11 enables this by default on new installations
            """,
        WhatItDoes = """
            Registry: HKLM\SYSTEM\CurrentControlSet\Control\DeviceGuard
            - EnableVirtualizationBasedSecurity = 0

            Registry: HKLM\SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity
            - Enabled = 0
            """,
        Effectiveness = EffectivenessRating.Effective,
        RecommendedByDefault = false,
        RequiresRestart = true
    };

    public override Task<bool> IsAppliedAsync()
    {
        // Check if VBS is disabled
        var vbsEnabled = RegistryHelper.GetValue(DeviceGuardPath, "EnableVirtualizationBasedSecurity", RegistryHive.LocalMachine);
        var hvciEnabled = RegistryHelper.GetValue(HvciPath, "Enabled", RegistryHive.LocalMachine);

        // VBS is disabled if EnableVirtualizationBasedSecurity = 0 or HVCI Enabled = 0
        bool isDisabled = (vbsEnabled is int vbs && vbs == 0) ||
                          (hvciEnabled is int hvci && hvci == 0);

        return Task.FromResult(isDisabled);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            _backups.Clear();

            var settings = new (string path, string name, object value, RegistryValueKind kind)[]
            {
                (DeviceGuardPath, "EnableVirtualizationBasedSecurity", 0, RegistryValueKind.DWord),
                (HvciPath, "Enabled", 0, RegistryValueKind.DWord),
            };

            int applied = 0;
            foreach (var (path, name, value, kind) in settings)
            {
                var backup = RegistryHelper.BackupValue(path, name, RegistryHive.LocalMachine);
                if (backup != null) _backups.Add(backup);

                if (RegistryHelper.SetValue(path, name, value, kind, RegistryHive.LocalMachine))
                {
                    Log($"Set {name} = {value}");
                    applied++;
                }
            }

            if (applied == 0)
            {
                return Task.FromResult(TweakResult.Fail(
                    "Could not disable VBS/HVCI",
                    "Registry access may be restricted. Run as Administrator."));
            }

            return Task.FromResult(TweakResult.Ok(
                $"VBS/HVCI disabled ({applied} settings). Restart required."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to disable VBS/HVCI", ex.Message));
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

            // If no backups, set to enabled state
            if (restored == 0)
            {
                RegistryHelper.SetValue(DeviceGuardPath, "EnableVirtualizationBasedSecurity", 1,
                    RegistryValueKind.DWord, RegistryHive.LocalMachine);
                RegistryHelper.SetValue(HvciPath, "Enabled", 1,
                    RegistryValueKind.DWord, RegistryHive.LocalMachine);
                restored = 2;
            }

            return Task.FromResult(TweakResult.Ok($"Restored {restored} VBS setting(s). Restart required."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to restore VBS settings", ex.Message));
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var isApplied = await IsAppliedAsync();
        return isApplied ? "Disabled (Faster)" : "Enabled (Secure)";
    }
}
