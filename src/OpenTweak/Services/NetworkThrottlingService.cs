using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Disables Network Throttling Index - PLACEBO tweak for educational purposes.
/// </summary>
public class NetworkThrottlingService : TweakServiceBase
{
    private const string MultimediaPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";

    private readonly List<RegistryBackup> _backups = new();

    public override TweakInfo Info => new()
    {
        Id = "network-throttling",
        Name = "Net Throttle 🎭",
        Description = "Disables Network Throttling Index (Placebo)",
        Category = "Placebo",
        TechnicalDetails = """
            This is a PLACEBO tweak commonly sold in "FPS boosters" and "ping reducers".

            What sellers claim:
            - "Removes network throttling"
            - "Improves ping in games"
            - "More bandwidth for gaming"

            What it actually does:
            - Changes NetworkThrottlingIndex from 10 to 0xFFFFFFFF
            - This setting ONLY affects multimedia streaming (like WMP)
            - It has ZERO effect on games, web browsers, or general network traffic
            - Games use UDP which bypasses this entirely

            Reality check:
            - This setting is for Windows Media Foundation apps
            - It doesn't affect TCP/IP gaming traffic
            - UDP-based games (most online games) ignore this completely
            - Your ping is determined by your ISP and server location, not this

            We include this to expose a common "FPS booster" scam.
            """,
        WhatItDoes = """
            Registry: HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile
            - NetworkThrottlingIndex = 0xFFFFFFFF (disabled)

            Default value is 10, which limits network throughput during
            multimedia playback to prevent audio glitches.
            This has nothing to do with games.
            """,
        Effectiveness = EffectivenessRating.Placebo,
        RecommendedByDefault = false,
        RequiresRestart = false
    };

    public override Task<bool> IsAppliedAsync()
    {
        var value = RegistryHelper.GetValue(MultimediaPath, "NetworkThrottlingIndex", RegistryHive.LocalMachine);

        // Check if set to 0xFFFFFFFF (disabled)
        bool isDisabled = value is int index && index == unchecked((int)0xFFFFFFFF);

        return Task.FromResult(isDisabled);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            _backups.Clear();

            var backup = RegistryHelper.BackupValue(MultimediaPath, "NetworkThrottlingIndex", RegistryHive.LocalMachine);
            if (backup != null) _backups.Add(backup);

            // Set to 0xFFFFFFFF (disabled)
            if (RegistryHelper.SetValue(
                MultimediaPath,
                "NetworkThrottlingIndex",
                unchecked((int)0xFFFFFFFF),
                RegistryValueKind.DWord,
                RegistryHive.LocalMachine))
            {
                Log("Set NetworkThrottlingIndex = 0xFFFFFFFF");
                return Task.FromResult(TweakResult.Ok(
                    "Network Throttling disabled - Note: This is placebo for games"));
            }

            return Task.FromResult(TweakResult.Fail(
                "Could not modify Network Throttling",
                "Requires Administrator rights (HKLM)"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to modify Network Throttling", ex.Message));
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
                    // Set back to Windows default (10)
                    RegistryHelper.SetValue(
                        MultimediaPath,
                        "NetworkThrottlingIndex",
                        10,
                        RegistryValueKind.DWord,
                        RegistryHive.LocalMachine);
                    Log("Reset NetworkThrottlingIndex to default (10)");
                }
                else
                {
                    RegistryHelper.RestoreBackup(backup);
                    Log($"Restored NetworkThrottlingIndex");
                }
            }

            return Task.FromResult(TweakResult.Ok("Restored Network Throttling setting"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to restore Network Throttling", ex.Message));
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var isApplied = await IsAppliedAsync();
        return isApplied ? "Disabled (Placebo)" : "Default";
    }
}
