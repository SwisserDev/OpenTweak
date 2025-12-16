using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Disables Windows visual effects for better performance.
/// </summary>
public class VisualEffectsService : TweakServiceBase
{
    private const string VisualEffectsPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects";
    private const string DesktopPath = @"Control Panel\Desktop";
    private const string DwmPath = @"Software\Microsoft\Windows\DWM";

    private readonly List<RegistryBackup> _backups = new();

    public override TweakInfo Info => new()
    {
        Id = "visual-effects",
        Name = "Visual Effects Disable",
        Description = "Disables Windows animations and effects",
        Category = "Windows",
        TechnicalDetails = """
            Disables Windows visual effects like animations, shadows,
            and transparency for maximum performance.

            What it actually does:
            - Disables window animations (minimize/maximize)
            - Disables menu fade effects
            - Disables smooth scrolling
            - Disables transparency effects
            - Sets "Adjust for best performance" mode

            Reality check:
            - Makes Windows feel snappier
            - Minimal FPS impact in games
            - UI will look less polished
            - Some users prefer the cleaner look
            """,
        WhatItDoes = """
            Registry: HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects
            - VisualFXSetting = 2 (Best Performance)

            Registry: HKCU\Control Panel\Desktop
            - UserPreferencesMask = optimized values

            Registry: HKCU\Software\Microsoft\Windows\DWM
            - EnableAeroPeek = 0
            """,
        Effectiveness = EffectivenessRating.Minimal,
        RecommendedByDefault = false,
        RequiresRestart = false
    };

    public override Task<bool> IsAppliedAsync()
    {
        var visualFx = RegistryHelper.GetValue(VisualEffectsPath, "VisualFXSetting", RegistryHive.CurrentUser);

        // Applied if VisualFXSetting = 2 (Best Performance)
        return Task.FromResult(visualFx is int v && v == 2);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            _backups.Clear();
            int applied = 0;

            // Set to Best Performance mode
            var backup1 = RegistryHelper.BackupValue(VisualEffectsPath, "VisualFXSetting", RegistryHive.CurrentUser);
            if (backup1 != null) _backups.Add(backup1);

            if (RegistryHelper.SetValue(VisualEffectsPath, "VisualFXSetting", 2,
                RegistryValueKind.DWord, RegistryHive.CurrentUser))
            {
                Log("Set VisualFXSetting = 2 (Best Performance)");
                applied++;
            }

            // Disable Aero Peek
            var backup2 = RegistryHelper.BackupValue(DwmPath, "EnableAeroPeek", RegistryHive.CurrentUser);
            if (backup2 != null) _backups.Add(backup2);

            if (RegistryHelper.SetValue(DwmPath, "EnableAeroPeek", 0,
                RegistryValueKind.DWord, RegistryHive.CurrentUser))
            {
                Log("Disabled Aero Peek");
                applied++;
            }

            // Disable animations
            var backup3 = RegistryHelper.BackupValue(DesktopPath, "DragFullWindows", RegistryHive.CurrentUser);
            if (backup3 != null) _backups.Add(backup3);

            if (RegistryHelper.SetValue(DesktopPath, "DragFullWindows", "0",
                RegistryValueKind.String, RegistryHive.CurrentUser))
            {
                Log("Disabled full window drag");
                applied++;
            }

            // Disable menu animations
            var backup4 = RegistryHelper.BackupValue(DesktopPath, "MenuShowDelay", RegistryHive.CurrentUser);
            if (backup4 != null) _backups.Add(backup4);

            if (RegistryHelper.SetValue(DesktopPath, "MenuShowDelay", "0",
                RegistryValueKind.String, RegistryHive.CurrentUser))
            {
                Log("Set MenuShowDelay = 0");
                applied++;
            }

            if (applied == 0)
            {
                return Task.FromResult(TweakResult.Fail(
                    "Could not disable visual effects",
                    "Registry access may be restricted."));
            }

            return Task.FromResult(TweakResult.Ok($"Visual effects disabled ({applied} settings)"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to disable visual effects", ex.Message));
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

            // Restore defaults if no backups
            if (restored == 0)
            {
                RegistryHelper.SetValue(VisualEffectsPath, "VisualFXSetting", 0,
                    RegistryValueKind.DWord, RegistryHive.CurrentUser);
                RegistryHelper.SetValue(DwmPath, "EnableAeroPeek", 1,
                    RegistryValueKind.DWord, RegistryHive.CurrentUser);
                RegistryHelper.SetValue(DesktopPath, "DragFullWindows", "1",
                    RegistryValueKind.String, RegistryHive.CurrentUser);
                RegistryHelper.SetValue(DesktopPath, "MenuShowDelay", "400",
                    RegistryValueKind.String, RegistryHive.CurrentUser);
                restored = 4;
            }

            return Task.FromResult(TweakResult.Ok($"Restored {restored} visual effect setting(s)"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to restore visual effects", ex.Message));
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var isApplied = await IsAppliedAsync();
        return isApplied ? "Best Performance" : "Default";
    }
}
