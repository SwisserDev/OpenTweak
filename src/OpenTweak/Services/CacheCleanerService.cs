using OpenTweak.Models;
using System.IO;

namespace OpenTweak.Services;

/// <summary>
/// Cleans FiveM cache, crash dumps, and log files.
/// </summary>
public class CacheCleanerService : TweakServiceBase
{
    private long _lastCleanedBytes;

    public override TweakInfo Info => new()
    {
        Id = "cache-cleaner",
        Name = "FiveM Cache Cleaner",
        Description = "Deletes FiveM cache, crash dumps, and log files",
        Category = "Cleanup",
        TechnicalDetails = """
            Clears the following FiveM directories:
            - cache/ (temporary files, will be rebuilt on next launch)
            - crashes/ (crash dump files)
            - logs/ (log files)

            This is actually useful:
            - Can fix texture/asset issues caused by corrupted cache
            - Frees up disk space
            - Safe to do - FiveM rebuilds cache automatically

            Note: First launch after cleaning may take longer as cache rebuilds.
            """,
        WhatItDoes = """
            Deletes contents of:
            %localappdata%\FiveM\FiveM.app\cache\
            %localappdata%\FiveM\FiveM.app\crashes\
            %localappdata%\FiveM\FiveM.app\logs\
            """,
        Effectiveness = EffectivenessRating.Effective,
        RecommendedByDefault = false, // On-demand action, not persistent
        RequiresRestart = false
    };

    private static string FiveMPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FiveM", "FiveM.app");

    private static readonly string[] CacheFolders = { "cache", "crashes", "logs" };

    public override Task<bool> IsAppliedAsync()
    {
        // This is a one-time action, not a persistent state
        return Task.FromResult(false);
    }

    public override async Task<TweakResult> ApplyAsync()
    {
        try
        {
            if (!Directory.Exists(FiveMPath))
            {
                return TweakResult.Fail("FiveM not found",
                    $"Expected path: {FiveMPath}");
            }

            long totalBytes = 0;
            int filesDeleted = 0;
            var errors = new List<string>();

            foreach (var folder in CacheFolders)
            {
                var path = Path.Combine(FiveMPath, folder);
                if (!Directory.Exists(path))
                {
                    Log($"Folder not found: {folder}");
                    continue;
                }

                Log($"Cleaning {folder}...");

                try
                {
                    var (bytes, files) = await DeleteDirectoryContentsAsync(path);
                    totalBytes += bytes;
                    filesDeleted += files;
                    Log($"Deleted {files} files ({FormatBytes(bytes)}) from {folder}");
                }
                catch (Exception ex)
                {
                    errors.Add($"{folder}: {ex.Message}");
                }
            }

            _lastCleanedBytes = totalBytes;

            if (errors.Count > 0)
            {
                return TweakResult.Ok(
                    $"Cleaned {FormatBytes(totalBytes)} ({filesDeleted} files) - some errors occurred: {string.Join(", ", errors)}");
            }

            return TweakResult.Ok($"Cleaned {FormatBytes(totalBytes)} ({filesDeleted} files)");
        }
        catch (Exception ex)
        {
            return TweakResult.Fail("Failed to clean cache", ex.Message);
        }
    }

    public override Task<TweakResult> RevertAsync()
    {
        // Cannot restore deleted files
        return Task.FromResult(TweakResult.Fail(
            "Cannot restore deleted files",
            "FiveM will rebuild the cache automatically on next launch"));
    }

    public override Task<string> GetStatusAsync()
    {
        if (!Directory.Exists(FiveMPath))
            return Task.FromResult("FiveM not installed");

        long totalSize = 0;
        foreach (var folder in CacheFolders)
        {
            var path = Path.Combine(FiveMPath, folder);
            if (Directory.Exists(path))
            {
                totalSize += GetDirectorySize(path);
            }
        }

        return Task.FromResult($"{FormatBytes(totalSize)} cached");
    }

    private static async Task<(long bytes, int files)> DeleteDirectoryContentsAsync(string path)
    {
        long totalBytes = 0;
        int fileCount = 0;

        await Task.Run(() =>
        {
            var di = new DirectoryInfo(path);

            foreach (var file in di.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                try
                {
                    totalBytes += file.Length;
                    file.Delete();
                    fileCount++;
                }
                catch
                {
                    // Skip locked files
                }
            }

            foreach (var dir in di.EnumerateDirectories("*", SearchOption.AllDirectories)
                         .OrderByDescending(d => d.FullName.Length))
            {
                try
                {
                    if (!dir.EnumerateFileSystemInfos().Any())
                        dir.Delete();
                }
                catch
                {
                    // Skip locked directories
                }
            }
        });

        return (totalBytes, fileCount);
    }

    private static long GetDirectorySize(string path)
    {
        try
        {
            return new DirectoryInfo(path)
                .EnumerateFiles("*", SearchOption.AllDirectories)
                .Sum(f => f.Length);
        }
        catch
        {
            return 0;
        }
    }

    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }
}
