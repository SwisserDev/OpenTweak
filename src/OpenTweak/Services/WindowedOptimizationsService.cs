using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Enables Windows 11 optimizations for windowed games.
/// </summary>
public class WindowedOptimizationsService : TweakServiceBase
{
    private const string DirectXPath = @"Software\Microsoft\DirectX\UserGpuPreferences";
    private const string GraphicsSettingsPath = @"Software\Microsoft\DirectX\GraphicsSettings";

    private readonly List<RegistryBackup> _backups = new();

    public override TweakInfo Info => new()
    {
        Id = "windowed-optimizations",
        Name = "Windowed Game Optimizations",
        Description = "Enables Windows 11 latency optimizations for windowed games",
        Category = "Windows",
        TechnicalDetails = """
            Enables Windows 11's "Optimizations for windowed games" feature
            which improves latency in borderless windowed mode.

            What it actually does:
            - Enables SwapEffectUpgrade for DirectX 10/11 games
            - Reduces input latency in borderless windowed mode
            - Makes windowed mode closer to fullscreen performance

            Reality check:
            - Windows 11 only (no effect on Windows 10)
            - Works best with DirectX 10/11 games
            - DirectX 12/Vulkan games handle this themselves
            - Microsoft claims "significant" latency improvements
            - May cause issues with some older games
            """,
        WhatItDoes = """
            Registry: HKCU\Software\Microsoft\DirectX\UserGpuPreferences
            - DirectXUserGlobalSettings = SwapEffectUpgradeEnable=1;

            Also available in Windows Settings:
            System > Display > Graphics > Change default graphics settings
            """,
        Effectiveness = EffectivenessRating.Effective,
        RecommendedByDefault = true,
        RequiresRestart = false
    };

    public override Task<bool> IsAppliedAsync()
    {
        var value = RegistryHelper.GetValue(DirectXPath, "DirectXUserGlobalSettings", RegistryHive.CurrentUser);

        // Applied if contains SwapEffectUpgradeEnable=1
        if (value is string settings)
        {
            return Task.FromResult(settings.Contains("SwapEffectUpgradeEnable=1"));
        }

        return Task.FromResult(false);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            _backups.Clear();

            var backup = RegistryHelper.BackupValue(DirectXPath, "DirectXUserGlobalSettings", RegistryHive.CurrentUser);
            if (backup != null) _backups.Add(backup);

            // Get current settings and append or modify
            var currentValue = RegistryHelper.GetValue(DirectXPath, "DirectXUserGlobalSettings", RegistryHive.CurrentUser) as string ?? "";

            string newValue;
            if (currentValue.Contains("SwapEffectUpgradeEnable="))
            {
                // Replace existing value
                newValue = System.Text.RegularExpressions.Regex.Replace(
                    currentValue,
                    @"SwapEffectUpgradeEnable=\d",
                    "SwapEffectUpgradeEnable=1");
            }
            else if (string.IsNullOrEmpty(currentValue))
            {
                newValue = "SwapEffectUpgradeEnable=1;";
            }
            else
            {
                // Append to existing settings
                newValue = currentValue.TrimEnd(';') + ";SwapEffectUpgradeEnable=1;";
            }

            if (RegistryHelper.SetValue(DirectXPath, "DirectXUserGlobalSettings", newValue,
                RegistryValueKind.String, RegistryHive.CurrentUser))
            {
                Log($"Set DirectXUserGlobalSettings = {newValue}");
                return Task.FromResult(TweakResult.Ok("Windowed game optimizations enabled"));
            }

            return Task.FromResult(TweakResult.Fail(
                "Could not enable windowed optimizations",
                "Registry access may be restricted or Windows version doesn't support this feature."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to enable windowed optimizations", ex.Message));
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
                    Log("Removed DirectXUserGlobalSettings");
                }
                else
                {
                    RegistryHelper.RestoreBackup(backup);
                    Log($"Restored DirectXUserGlobalSettings");
                }
            }

            // If no backup, try to remove the setting
            if (_backups.Count == 0)
            {
                var currentValue = RegistryHelper.GetValue(DirectXPath, "DirectXUserGlobalSettings", RegistryHive.CurrentUser) as string ?? "";
                if (currentValue.Contains("SwapEffectUpgradeEnable="))
                {
                    var newValue = System.Text.RegularExpressions.Regex.Replace(
                        currentValue,
                        @"SwapEffectUpgradeEnable=\d;?",
                        "").Trim(';');

                    if (string.IsNullOrEmpty(newValue))
                    {
                        RegistryHelper.DeleteValue(DirectXPath, "DirectXUserGlobalSettings", RegistryHive.CurrentUser);
                    }
                    else
                    {
                        RegistryHelper.SetValue(DirectXPath, "DirectXUserGlobalSettings", newValue,
                            RegistryValueKind.String, RegistryHive.CurrentUser);
                    }
                }
            }

            return Task.FromResult(TweakResult.Ok("Windowed optimizations disabled"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to disable windowed optimizations", ex.Message));
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var isApplied = await IsAppliedAsync();
        return isApplied ? "Enabled" : "Disabled";
    }
}
