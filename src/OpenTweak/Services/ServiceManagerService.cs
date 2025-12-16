using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;
using System.ServiceProcess;

namespace OpenTweak.Services;

/// <summary>
/// Disables unnecessary Windows services to free up system resources.
/// </summary>
public class ServiceManagerService : TweakServiceBase
{
    // Services safe to disable for gaming (these have minimal impact on gaming PCs)
    private static readonly string[] ServicesToDisable =
    {
        "PrintSpooler",      // Print Spooler - only needed for printers
        "Fax",               // Fax service
        "WSearch",           // Windows Search Indexer
        "MapsBroker",        // Downloaded Maps Manager
        "WalletService",     // Wallet Service
        "PhoneSvc",          // Phone Service
        "TabletInputService" // Touch Keyboard Service
    };

    private readonly Dictionary<string, ServiceStartMode> _originalStartModes = new();

    public override TweakInfo Info => new()
    {
        Id = "service-manager",
        Name = "Disable Services",
        Description = "Disables unnecessary Windows services",
        Category = "Windows",
        TechnicalDetails = """
            Disables Windows services that are typically not needed for gaming.

            Services disabled:
            - PrintSpooler (Print Spooler) - unless you use a printer
            - Fax - unless you send faxes
            - WSearch (Windows Search) - uses disk/CPU for indexing
            - MapsBroker (Maps) - downloads map data
            - WalletService - Windows Wallet
            - PhoneSvc - Your Phone companion
            - TabletInputService - Touch keyboard

            Reality check:
            - Frees up some RAM and CPU cycles
            - Effect is minimal on modern systems
            - May break functionality you need
            - Can always re-enable if needed
            """,
        WhatItDoes = """
            Sets the following services to Disabled startup type:
            - PrintSpooler, Fax, WSearch, MapsBroker
            - WalletService, PhoneSvc, TabletInputService

            Original startup types are backed up for restoration.
            """,
        Effectiveness = EffectivenessRating.Minimal,
        RecommendedByDefault = false,
        RequiresRestart = false
    };

    public override Task<bool> IsAppliedAsync()
    {
        try
        {
            int disabledCount = 0;
            foreach (var serviceName in ServicesToDisable)
            {
                try
                {
                    using var sc = new ServiceController(serviceName);
                    if (sc.StartType == ServiceStartMode.Disabled)
                        disabledCount++;
                }
                catch
                {
                    // Service doesn't exist, count as "disabled"
                    disabledCount++;
                }
            }

            // Consider applied if at least half are disabled
            return Task.FromResult(disabledCount >= ServicesToDisable.Length / 2);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            _originalStartModes.Clear();
            int disabled = 0;
            int skipped = 0;

            foreach (var serviceName in ServicesToDisable)
            {
                try
                {
                    // Backup original start mode via registry (more reliable)
                    var regPath = $@"SYSTEM\CurrentControlSet\Services\{serviceName}";
                    var startValue = RegistryHelper.GetValue(regPath, "Start", RegistryHive.LocalMachine);

                    if (startValue is int originalStart)
                    {
                        _originalStartModes[serviceName] = (ServiceStartMode)originalStart;

                        // Set to disabled (4)
                        if (RegistryHelper.SetValue(regPath, "Start", 4, RegistryValueKind.DWord, RegistryHive.LocalMachine))
                        {
                            Log($"Disabled service: {serviceName}");
                            disabled++;

                            // Try to stop the service if running
                            try
                            {
                                using var sc = new ServiceController(serviceName);
                                if (sc.Status == ServiceControllerStatus.Running)
                                {
                                    sc.Stop();
                                    Log($"Stopped service: {serviceName}");
                                }
                            }
                            catch
                            {
                                // Service may not be stoppable
                            }
                        }
                    }
                    else
                    {
                        skipped++;
                    }
                }
                catch
                {
                    skipped++;
                }
            }

            if (disabled == 0)
            {
                return Task.FromResult(TweakResult.Fail(
                    "Could not disable any services",
                    "Services may not exist or access is restricted."));
            }

            return Task.FromResult(TweakResult.Ok($"Disabled {disabled} services, skipped {skipped}"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to disable services", ex.Message));
        }
    }

    public override Task<TweakResult> RevertAsync()
    {
        try
        {
            int restored = 0;

            foreach (var (serviceName, originalMode) in _originalStartModes)
            {
                try
                {
                    var regPath = $@"SYSTEM\CurrentControlSet\Services\{serviceName}";
                    if (RegistryHelper.SetValue(regPath, "Start", (int)originalMode, RegistryValueKind.DWord, RegistryHive.LocalMachine))
                    {
                        Log($"Restored service: {serviceName} to {originalMode}");
                        restored++;
                    }
                }
                catch
                {
                    // Service restoration failed
                }
            }

            _originalStartModes.Clear();
            return Task.FromResult(TweakResult.Ok($"Restored {restored} service(s)"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to restore services", ex.Message));
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var isApplied = await IsAppliedAsync();
        return isApplied ? "Optimized" : "Default";
    }
}
