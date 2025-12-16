using OpenTweak.Models;
using System.IO;

namespace OpenTweak.Services;

/// <summary>
/// Cleans GPU shader caches (NVIDIA, AMD, DirectX).
/// Helps fix stuttering after driver or Windows updates.
/// </summary>
public class ShaderCacheCleanerService : TweakServiceBase
{
    private long _lastCleanedBytes;

    // Shader cache locations relative to %localappdata%
    private static readonly (string Name, string RelativePath)[] ShaderCaches =
    {
        ("NVIDIA DX", @"NVIDIA\DXCache"),
        ("NVIDIA GL", @"NVIDIA\GLCache"),
        ("AMD", @"AMD\DxCache"),
        ("DirectX", @"D3DSCache"),
        ("Windows Shader", @"Microsoft\DirectX Shader Cache"),
    };

    public override TweakInfo Info => new()
    {
        Id = "shader-cache-cleaner",
        Name = "Shader Cache Cleaner",
        Description = "Deletes GPU shader caches (NVIDIA, AMD, DirectX)",
        Category = "Cleanup",
        TechnicalDetails = """
            Clears compiled shader caches from your GPU driver and DirectX.

            When to use:
            - After GPU driver updates (shaders need recompilation)
            - After Windows updates
            - When experiencing stuttering in GTA V / FiveM
            - When textures appear corrupted

            Safe to do - shaders are automatically recompiled on next game launch.
            First launch after cleaning may have brief stutters as shaders rebuild.
            """,
        WhatItDoes = """
            Deletes contents of:
            %localappdata%\NVIDIA\DXCache\     (NVIDIA DirectX shaders)
            %localappdata%\NVIDIA\GLCache\     (NVIDIA OpenGL shaders)
            %localappdata%\AMD\DxCache\        (AMD shaders)
            %localappdata%\D3DSCache\          (DirectX general cache)
            %localappdata%\Microsoft\DirectX Shader Cache\
            """,
        Effectiveness = EffectivenessRating.Effective,
        RecommendedByDefault = false,
        RequiresRestart = false
    };

    private static string LocalAppData =>
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public override Task<bool> IsAppliedAsync()
    {
        // One-time action, not a persistent state
        return Task.FromResult(false);
    }

    public override async Task<TweakResult> ApplyAsync()
    {
        try
        {
            long totalBytes = 0;
            int filesDeleted = 0;
            int cachesFound = 0;
            var errors = new List<string>();

            foreach (var (name, relativePath) in ShaderCaches)
            {
                var fullPath = Path.Combine(LocalAppData, relativePath);

                if (!Directory.Exists(fullPath))
                {
                    Log($"{name} cache not found, skipping");
                    continue;
                }

                cachesFound++;
                Log($"Cleaning {name} cache...");

                try
                {
                    var (bytes, files) = await DeleteDirectoryContentsAsync(fullPath);
                    totalBytes += bytes;
                    filesDeleted += files;
                    Log($"Deleted {files} files ({FormatBytes(bytes)}) from {name}");
                }
                catch (Exception ex)
                {
                    errors.Add($"{name}: {ex.Message}");
                }
            }

            _lastCleanedBytes = totalBytes;

            if (cachesFound == 0)
            {
                return TweakResult.Fail(
                    "No shader caches found",
                    "This is normal if you don't have NVIDIA or AMD graphics");
            }

            if (errors.Count > 0 && filesDeleted == 0)
            {
                return TweakResult.Fail(
                    "Could not clean shader caches",
                    string.Join(", ", errors));
            }

            var message = $"Cleaned {FormatBytes(totalBytes)} ({filesDeleted} files)";
            if (errors.Count > 0)
            {
                message += $" - some errors: {string.Join(", ", errors)}";
            }

            return TweakResult.Ok(message);
        }
        catch (Exception ex)
        {
            return TweakResult.Fail("Failed to clean shader caches", ex.Message);
        }
    }

    public override Task<TweakResult> RevertAsync()
    {
        return Task.FromResult(TweakResult.Fail(
            "Cannot restore deleted files",
            "Shaders will be recompiled automatically on next game launch"));
    }

    public override Task<string> GetStatusAsync()
    {
        long totalSize = 0;
        int cachesFound = 0;

        foreach (var (_, relativePath) in ShaderCaches)
        {
            var fullPath = Path.Combine(LocalAppData, relativePath);
            if (Directory.Exists(fullPath))
            {
                cachesFound++;
                totalSize += GetDirectorySize(fullPath);
            }
        }

        if (cachesFound == 0)
            return Task.FromResult("No caches found");

        return Task.FromResult($"{FormatBytes(totalSize)} in {cachesFound} cache(s)");
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
                    // Skip locked files (in use by GPU driver)
                }
            }

            // Clean up empty directories
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
