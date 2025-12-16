using Microsoft.Win32;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Optimizes Windows Multimedia Class Scheduler Service (MMCSS) settings for gaming.
/// </summary>
public class GamePriorityService : TweakServiceBase
{
    private const string GamesTaskPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games";

    private readonly List<RegistryBackup> _backups = new();

    public override TweakInfo Info => new()
    {
        Id = "game-priority",
        Name = "Game Priority Boost",
        Description = "Increases CPU/GPU priority for games",
        Category = "System",
        TechnicalDetails = """
            Optimizes the Windows Multimedia Class Scheduler Service (MMCSS)
            settings specifically for the "Games" task category.

            What it actually does:
            - Sets GPU Priority to maximum (8)
            - Sets CPU Priority to highest gaming level (6)
            - Enables "High" scheduling category
            - Maximizes SFIO (Scheduled File I/O) priority

            Reality check:
            - Works with games that register with MMCSS
            - Not all games use MMCSS scheduling
            - Effect varies depending on system load
            - Combined with other tweaks for best results
            """,
        WhatItDoes = """
            Registry: HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games
            - GPU Priority = 8 (maximum)
            - Priority = 6 (highest for games)
            - Scheduling Category = High
            - SFIO Priority = High
            """,
        Effectiveness = EffectivenessRating.Minimal,
        RecommendedByDefault = true,
        RequiresRestart = false
    };

    public override Task<bool> IsAppliedAsync()
    {
        var gpuPriority = RegistryHelper.GetValue(GamesTaskPath, "GPU Priority", RegistryHive.LocalMachine);
        var priority = RegistryHelper.GetValue(GamesTaskPath, "Priority", RegistryHive.LocalMachine);

        // Applied if GPU Priority = 8 and Priority = 6
        bool isOptimized = (gpuPriority is int gpu && gpu == 8) &&
                           (priority is int prio && prio == 6);

        return Task.FromResult(isOptimized);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            _backups.Clear();

            var settings = new (string name, object value, RegistryValueKind kind)[]
            {
                ("GPU Priority", 8, RegistryValueKind.DWord),
                ("Priority", 6, RegistryValueKind.DWord),
                ("Scheduling Category", "High", RegistryValueKind.String),
                ("SFIO Priority", "High", RegistryValueKind.String),
            };

            int applied = 0;
            foreach (var (name, value, kind) in settings)
            {
                var backup = RegistryHelper.BackupValue(GamesTaskPath, name, RegistryHive.LocalMachine);
                if (backup != null) _backups.Add(backup);

                if (RegistryHelper.SetValue(GamesTaskPath, name, value, kind, RegistryHive.LocalMachine))
                {
                    Log($"Set {name} = {value}");
                    applied++;
                }
            }

            if (applied == 0)
            {
                return Task.FromResult(TweakResult.Fail(
                    "Could not set game priority",
                    "Registry access may be restricted."));
            }

            return Task.FromResult(TweakResult.Ok($"Game priority optimized ({applied} settings)"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to optimize game priority", ex.Message));
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
                RegistryHelper.SetValue(GamesTaskPath, "GPU Priority", 2, RegistryValueKind.DWord, RegistryHive.LocalMachine);
                RegistryHelper.SetValue(GamesTaskPath, "Priority", 2, RegistryValueKind.DWord, RegistryHive.LocalMachine);
                RegistryHelper.SetValue(GamesTaskPath, "Scheduling Category", "Medium", RegistryValueKind.String, RegistryHive.LocalMachine);
                RegistryHelper.SetValue(GamesTaskPath, "SFIO Priority", "Normal", RegistryValueKind.String, RegistryHive.LocalMachine);
                restored = 4;
            }

            return Task.FromResult(TweakResult.Ok($"Restored {restored} game priority setting(s)"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to restore game priority settings", ex.Message));
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var isApplied = await IsAppliedAsync();
        return isApplied ? "High Priority" : "Default";
    }
}
