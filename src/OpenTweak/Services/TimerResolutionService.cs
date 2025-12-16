using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Sets Windows timer resolution to 0.5ms for more precise timing.
/// Note: This only stays active while the application is running.
/// </summary>
public class TimerResolutionService : TweakServiceBase
{
    private bool _isApplied;
    private int _originalResolution;

    // 0.5ms in 100-nanosecond intervals
    private const int TargetResolution = 5000;

    public override TweakInfo Info => new()
    {
        Id = "timer-resolution",
        Name = "Timer Resolution (0.5ms)",
        Description = "Sets system timer to 0.5ms for more precise timing",
        Category = "System",
        TechnicalDetails = """
            Windows default timer resolution is ~15.6ms. Setting it to 0.5ms can
            improve input responsiveness in games.

            Reality check:
            - Only active while OpenTweak is running
            - Slightly increases CPU power usage
            - Most modern games already request high timer resolution
            - Actual impact on gameplay is minimal for most users

            This is one of the few tweaks that can have a measurable effect.
            """,
        WhatItDoes = "NtSetTimerResolution(5000, true, out _) // 0.5ms",
        Effectiveness = EffectivenessRating.Effective,
        RecommendedByDefault = true,
        RequiresRestart = false
    };

    public override Task<bool> IsAppliedAsync()
    {
        return Task.FromResult(_isApplied);
    }

    public override Task<TweakResult> ApplyAsync()
    {
        try
        {
            // Query current resolution
            NativeMethods.NtQueryTimerResolution(
                out int minRes, out int maxRes, out int currentRes);

            _originalResolution = currentRes;
            Log($"Current timer resolution: {currentRes / 10000.0}ms (min: {maxRes / 10000.0}ms, max: {minRes / 10000.0}ms)");

            // Set new resolution
            int status = NativeMethods.NtSetTimerResolution(TargetResolution, true, out int actualRes);

            if (status != 0)
            {
                return Task.FromResult(TweakResult.Fail(
                    "Failed to set timer resolution",
                    $"NtSetTimerResolution returned status {status}"));
            }

            _isApplied = true;
            Log($"Timer resolution set to {actualRes / 10000.0}ms");

            return Task.FromResult(TweakResult.Ok(
                $"Timer resolution set to {actualRes / 10000.0}ms"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to set timer resolution", ex.Message));
        }
    }

    public override Task<TweakResult> RevertAsync()
    {
        try
        {
            // Disable our timer resolution request
            int status = NativeMethods.NtSetTimerResolution(TargetResolution, false, out _);

            if (status != 0)
            {
                return Task.FromResult(TweakResult.Fail(
                    "Failed to restore timer resolution",
                    $"NtSetTimerResolution returned status {status}"));
            }

            _isApplied = false;
            Log("Timer resolution restored to system default");

            return Task.FromResult(TweakResult.Ok("Timer resolution restored"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(TweakResult.Fail("Failed to restore timer resolution", ex.Message));
        }
    }

    public override Task<string> GetStatusAsync()
    {
        try
        {
            NativeMethods.NtQueryTimerResolution(out _, out _, out int currentRes);
            return Task.FromResult($"{currentRes / 10000.0:F2}ms");
        }
        catch
        {
            return Task.FromResult("Unknown");
        }
    }
}
