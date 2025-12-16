using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Removes the Windows startup delay for faster boot times.
/// </summary>
public class StartupDelayService : TweakServiceBase
{
    private const string SerializePath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Serialize";

    private readonly List<RegistryBackup> _backups = new();

    public override TweakInfo Info => new()
    {
        Id = "startup-delay",
        Name = "Startup Delay Remove",
        Description = "Removes Windows startup delay for faster boot",
        Category = "Windows",
        TechnicalDetails = """
            Removes the built-in Windows startup delay that waits before
            launching startup programs.

            What it actually does:
            - Sets StartupDelayInMSec to 0 (default is ~10 seconds)
            - Startup programs launch immediately after login
            - Desktop becomes usable faster

            Reality check:
            - Actually makes boot feel faster
            - May cause brief slowdown right after login
            - Startup programs all compete for resources at once
            - Modern SSDs minimize any negative impact
            """,
        WhatItDoes = """
            Registry: HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Serialize
            - StartupDelayInMSec = 0

            Creates the Serialize key if it doesn't exist.
            Default Windows behavior is ~10 second delay.
            """,
        Effectiveness = EffectivenessRating.Effective,
        RecommendedByDefault = true,
        RequiresRestart = false
    };

    public override Task<bool> IsAppliedAsync()
    {
        var value = RegistryHelper.GetValue(SerializePath, "StartupDelayInMSec", RegistryHive.CurrentUser);

        // Applied if StartupDelayInMSec = 0
        return Task.FromResult(value is int delay && delay == 0);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            _backups.Clear();

            var backup = RegistryHelper.BackupValue(SerializePath, "StartupDelayInMSec", RegistryHive.CurrentUser);
            if (backup != null) _backups.Add(backup);

            if (RegistryHelper.SetValue(SerializePath, "StartupDelayInMSec", 0,
                RegistryValueKind.DWord, RegistryHive.CurrentUser))
            {
                Log("Set StartupDelayInMSec = 0");
                return Task.FromResult(TweakResult.Ok("Startup delay removed"));
            }

            return Task.FromResult(TweakResult.Fail(
                "Could not remove startup delay",
                "Registry access may be restricted."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to remove startup delay", ex.Message));
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
                    // Delete the value and potentially the key
                    RegistryHelper.DeleteValue(backup.KeyPath, backup.ValueName, backup.Hive);
                    Log("Removed StartupDelayInMSec (restoring default behavior)");
                }
                else
                {
                    RegistryHelper.RestoreBackup(backup);
                    Log($"Restored StartupDelayInMSec to {backup.OriginalValue}");
                }
            }

            // If no backup, just delete the value
            if (_backups.Count == 0)
            {
                RegistryHelper.DeleteValue(SerializePath, "StartupDelayInMSec", RegistryHive.CurrentUser);
            }

            return Task.FromResult(TweakResult.Ok("Startup delay restored"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to restore startup delay", ex.Message));
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var isApplied = await IsAppliedAsync();
        return isApplied ? "No Delay" : "Default (~10s)";
    }
}
