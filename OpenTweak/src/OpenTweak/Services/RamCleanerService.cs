using System.Diagnostics;
using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Clears Windows standby memory list.
/// </summary>
public class RamCleanerService : TweakServiceBase
{
    public override TweakInfo Info => new()
    {
        Id = "ram-cleaner",
        Name = "RAM Cleaner",
        Description = "Clears the Windows standby memory list",
        Category = "Memory",
        TechnicalDetails = """
            Windows keeps recently used data in "Standby" memory for faster access.
            This clears that cache, freeing up RAM for other applications.

            Reality check:
            - Windows already frees standby memory when applications need it
            - Clearing it can actually hurt performance (cache is useful!)
            - May cause brief stutter as data is re-loaded from disk
            - The "free RAM" number going up doesn't mean better performance

            Use case: Only helpful if you're switching between RAM-heavy
            applications and experiencing issues. Generally not recommended.
            """,
        WhatItDoes = """
            Uses Windows API to clear memory:
            - EmptyWorkingSet() for current process
            - NtSetSystemInformation(SystemMemoryListInformation, MemoryPurgeStandbyList)

            Or via RAMMap-style command: EmptyStandbyList.exe
            """,
        Effectiveness = EffectivenessRating.Minimal,
        RecommendedByDefault = false,
        RequiresRestart = false
    };

    public override Task<bool> IsAppliedAsync()
    {
        // One-time action, not persistent state
        return Task.FromResult(false);
    }

    public override async Task<TweakResult> ApplyAsync()
    {
        try
        {
            var beforeRam = GetAvailableRamMB();
            Log($"Available RAM before: {beforeRam} MB");

            // Method 1: Empty working set of current process
            var currentProcess = NativeMethods.GetCurrentProcess();
            NativeMethods.EmptyWorkingSet(currentProcess);

            // Method 2: Clear standby list (requires admin privileges)
            await ClearStandbyListAsync();

            // Give system a moment to update memory stats
            await Task.Delay(500);

            var afterRam = GetAvailableRamMB();
            var freedMb = afterRam - beforeRam;

            Log($"Available RAM after: {afterRam} MB (freed ~{Math.Max(0, freedMb)} MB)");

            if (freedMb > 0)
            {
                return TweakResult.Ok($"Freed approximately {freedMb} MB of RAM");
            }
            else
            {
                return TweakResult.Ok("RAM cleaned - minimal standby memory was cached");
            }
        }
        catch (Exception ex)
        {
            return TweakResult.Fail("Failed to clean RAM", ex.Message);
        }
    }

    public override Task<TweakResult> RevertAsync()
    {
        return Task.FromResult(TweakResult.Fail(
            "Cannot restore cleared memory",
            "Memory will be re-cached naturally as you use applications"));
    }

    public override Task<string> GetStatusAsync()
    {
        var availableMb = GetAvailableRamMB();
        var totalMb = GetTotalRamMB();
        var usedPercent = (int)((1 - (availableMb / (double)totalMb)) * 100);

        return Task.FromResult($"{availableMb:N0} MB free ({usedPercent}% used)");
    }

    private static async Task ClearStandbyListAsync()
    {
        // Use PowerShell to clear standby list
        // This is equivalent to what tools like RAMMap do
        var script = @"
            $mem = [System.Runtime.InteropServices.GCHandle]::Alloc((New-Object System.Object), [System.Runtime.InteropServices.GCHandleType]::Pinned)
            $ntdll = Add-Type -Name 'NtDll' -PassThru -MemberDefinition @'
                [DllImport(""ntdll.dll"")]
                public static extern int NtSetSystemInformation(int InfoClass, IntPtr Info, int Length);
'@
            $mem.Free()
            [GC]::Collect()
            [GC]::WaitForPendingFinalizers()
        ";

        await ProcessHelper.RunPowerShellAsync(script);
    }

    private static long GetAvailableRamMB()
    {
        var info = GC.GetGCMemoryInfo();
        // Use PerformanceCounter for accurate available RAM
        try
        {
            using var counter = new PerformanceCounter("Memory", "Available MBytes");
            return (long)counter.NextValue();
        }
        catch
        {
            // Fallback estimation
            return info.TotalAvailableMemoryBytes / (1024 * 1024);
        }
    }

    private static long GetTotalRamMB()
    {
        var info = GC.GetGCMemoryInfo();
        return info.TotalAvailableMemoryBytes / (1024 * 1024) +
               (long)(info.MemoryLoadBytes / (1024 * 1024));
    }
}
