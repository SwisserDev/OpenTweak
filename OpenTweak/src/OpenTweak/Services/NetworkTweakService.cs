using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Disables Nagle's Algorithm and optimizes TCP settings.
/// </summary>
public class NetworkTweakService : TweakServiceBase
{
    private const string TcpipInterfacesPath = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces";
    private const string TcpipParametersPath = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters";

    private readonly List<RegistryBackup> _backups = new();

    public override TweakInfo Info => new()
    {
        Id = "network-tcp",
        Name = "TCP Optimizer",
        Description = "Disables Nagle's Algorithm for faster packet transmission",
        Category = "Network",
        TechnicalDetails = """
            Nagle's Algorithm buffers small TCP packets to reduce network overhead.
            Disabling it sends packets immediately.

            Reality check:
            - FiveM/GTA V uses UDP for game data, not TCP
            - This only affects TCP connections (web requests, downloads)
            - Modern games and applications often disable Nagle themselves
            - Impact on actual gameplay: essentially zero

            This is the most oversold "optimization" in gaming circles.
            It sounds technical but doesn't help with game netcode.
            """,
        WhatItDoes = """
            For each network interface:
            Registry: HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\{GUID}
            - TcpAckFrequency = 1 (ACK every packet)
            - TCPNoDelay = 1 (Disable Nagle's Algorithm)
            """,
        Effectiveness = EffectivenessRating.Placebo,
        RecommendedByDefault = false,
        RequiresRestart = false
    };

    public override Task<bool> IsAppliedAsync()
    {
        var interfaces = RegistryHelper.GetSubKeyNames(TcpipInterfacesPath);

        foreach (var iface in interfaces)
        {
            var keyPath = $@"{TcpipInterfacesPath}\{iface}";
            var tcpNoDelay = RegistryHelper.GetValue(keyPath, "TcpNoDelay");

            if (tcpNoDelay is int value && value == 1)
                return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            _backups.Clear();
            var interfaces = RegistryHelper.GetSubKeyNames(TcpipInterfacesPath);
            int modified = 0;

            Log($"Found {interfaces.Length} network interfaces");

            foreach (var iface in interfaces)
            {
                var keyPath = $@"{TcpipInterfacesPath}\{iface}";

                // Skip interfaces without IP addresses (virtual/unused)
                var ipAddress = RegistryHelper.GetValue(keyPath, "IPAddress");
                var dhcpIp = RegistryHelper.GetValue(keyPath, "DhcpIPAddress");

                if (ipAddress == null && dhcpIp == null)
                    continue;

                // Backup existing values
                var backupAck = RegistryHelper.BackupValue(keyPath, "TcpAckFrequency");
                var backupDelay = RegistryHelper.BackupValue(keyPath, "TcpNoDelay");
                if (backupAck != null) _backups.Add(backupAck);
                if (backupDelay != null) _backups.Add(backupDelay);

                // Apply optimizations
                RegistryHelper.SetValue(keyPath, "TcpAckFrequency", 1, RegistryValueKind.DWord);
                RegistryHelper.SetValue(keyPath, "TcpNoDelay", 1, RegistryValueKind.DWord);

                Log($"Applied TCP optimizations to interface {iface.Substring(0, 8)}...");
                modified++;
            }

            // Global TCP settings
            var backupGlobalAuto = RegistryHelper.BackupValue(TcpipParametersPath, "TCPNoDelay");
            if (backupGlobalAuto != null) _backups.Add(backupGlobalAuto);

            RegistryHelper.SetValue(TcpipParametersPath, "TCPNoDelay", 1, RegistryValueKind.DWord);

            if (modified == 0)
            {
                return Task.FromResult(TweakResult.Ok(
                    "No active network interfaces found - settings applied globally"));
            }

            return Task.FromResult(TweakResult.Ok(
                $"TCP optimizations applied to {modified} interface(s)"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to apply TCP optimizations", ex.Message));
        }
    }

    public override Task<TweakResult> RevertAsync()
    {
        try
        {
            int restored = 0;
            foreach (var backup in _backups)
            {
                if (RegistryHelper.RestoreBackup(backup))
                    restored++;
            }

            Log($"Restored {restored} registry values");
            return Task.FromResult(TweakResult.Ok($"Restored {restored} TCP settings"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to restore TCP settings", ex.Message));
        }
    }
}
