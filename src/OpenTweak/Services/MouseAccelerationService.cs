using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Disables Windows mouse acceleration (MarkC fix style).
/// </summary>
public class MouseAccelerationService : TweakServiceBase
{
    private const string MousePath = @"Control Panel\Mouse";

    private readonly List<RegistryBackup> _backups = new();

    // Linear curve values (no acceleration) - standard MarkC fix
    private static readonly byte[] LinearSmoothMouseXCurve = new byte[]
    {
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0xC0, 0xCC, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x80, 0x99, 0x19, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x40, 0x66, 0x26, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x33, 0x33, 0x00, 0x00, 0x00, 0x00, 0x00
    };

    private static readonly byte[] LinearSmoothMouseYCurve = new byte[]
    {
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x38, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x70, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0xA8, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0xE0, 0x00, 0x00, 0x00, 0x00, 0x00
    };

    public override TweakInfo Info => new()
    {
        Id = "mouse-acceleration",
        Name = "Mouse Acceleration Fix",
        Description = "Removes Windows mouse acceleration for 1:1 input",
        Category = "Input",
        TechnicalDetails = """
            Disables Windows "Enhance Pointer Precision" and sets linear mouse curves.
            This is the famous "MarkC Mouse Fix" that FPS players use.

            What it actually does:
            - Sets MouseSpeed, MouseThreshold1/2 to 0
            - Applies linear SmoothMouseXCurve/YCurve values
            - Results in true 1:1 mouse movement

            Reality check:
            - ACTUALLY EFFECTIVE for FPS games!
            - Essential for consistent muscle memory in aiming
            - Most competitive FPS players disable acceleration
            - This is one of the few tweaks that genuinely helps
            """,
        WhatItDoes = """
            Registry: HKCU\Control Panel\Mouse
            - MouseSpeed = "0"
            - MouseThreshold1 = "0"
            - MouseThreshold2 = "0"
            - SmoothMouseXCurve = [linear values]
            - SmoothMouseYCurve = [linear values]
            """,
        Effectiveness = EffectivenessRating.Effective,
        RecommendedByDefault = true,
        RequiresRestart = true // Requires logoff/logon
    };

    public override Task<bool> IsAppliedAsync()
    {
        var mouseSpeed = RegistryHelper.GetValue(MousePath, "MouseSpeed", RegistryHive.CurrentUser);
        var threshold1 = RegistryHelper.GetValue(MousePath, "MouseThreshold1", RegistryHive.CurrentUser);
        var threshold2 = RegistryHelper.GetValue(MousePath, "MouseThreshold2", RegistryHive.CurrentUser);

        // Check if all acceleration values are 0
        bool isDisabled = mouseSpeed is string ms && ms == "0" &&
                          threshold1 is string t1 && t1 == "0" &&
                          threshold2 is string t2 && t2 == "0";

        return Task.FromResult(isDisabled);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            _backups.Clear();

            // String values
            var stringSettings = new (string name, string value)[]
            {
                ("MouseSpeed", "0"),
                ("MouseThreshold1", "0"),
                ("MouseThreshold2", "0"),
            };

            int applied = 0;

            foreach (var (name, value) in stringSettings)
            {
                var backup = RegistryHelper.BackupValue(MousePath, name, RegistryHive.CurrentUser);
                if (backup != null) _backups.Add(backup);

                if (RegistryHelper.SetValue(MousePath, name, value, RegistryValueKind.String, RegistryHive.CurrentUser))
                {
                    Log($"Set {name} = {value}");
                    applied++;
                }
            }

            // Binary curve values
            var curveBackupX = RegistryHelper.BackupValue(MousePath, "SmoothMouseXCurve", RegistryHive.CurrentUser);
            if (curveBackupX != null) _backups.Add(curveBackupX);

            var curveBackupY = RegistryHelper.BackupValue(MousePath, "SmoothMouseYCurve", RegistryHive.CurrentUser);
            if (curveBackupY != null) _backups.Add(curveBackupY);

            if (RegistryHelper.SetValue(MousePath, "SmoothMouseXCurve", LinearSmoothMouseXCurve, RegistryValueKind.Binary, RegistryHive.CurrentUser))
            {
                Log("Set linear SmoothMouseXCurve");
                applied++;
            }

            if (RegistryHelper.SetValue(MousePath, "SmoothMouseYCurve", LinearSmoothMouseYCurve, RegistryValueKind.Binary, RegistryHive.CurrentUser))
            {
                Log("Set linear SmoothMouseYCurve");
                applied++;
            }

            if (applied == 0)
            {
                return Task.FromResult(TweakResult.Fail(
                    "Could not disable mouse acceleration",
                    "Registry access may be restricted"));
            }

            return Task.FromResult(TweakResult.Ok(
                $"Mouse acceleration disabled ({applied} settings). Log off to apply."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to disable mouse acceleration", ex.Message));
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

            return Task.FromResult(TweakResult.Ok($"Restored {restored} mouse setting(s). Log off to apply."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to restore mouse settings", ex.Message));
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var isApplied = await IsAppliedAsync();
        return isApplied ? "1:1 (No Accel)" : "Default (Accel On)";
    }
}
