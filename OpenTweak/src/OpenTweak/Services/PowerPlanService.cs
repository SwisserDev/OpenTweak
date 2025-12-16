using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Activates the hidden "Ultimate Performance" power plan in Windows.
/// </summary>
public class PowerPlanService : TweakServiceBase
{
    // Ultimate Performance GUID (hidden by default in Windows 10/11)
    private const string UltimatePerformanceGuid = "e9a42b02-d5df-448d-aa00-03f14749eb61";
    private const string HighPerformanceGuid = "8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c";

    private string? _previousPlanGuid;

    public override TweakInfo Info => new()
    {
        Id = "power-plan",
        Name = "Ultimate Performance",
        Description = "Activates the hidden Ultimate Performance power plan",
        Category = "Power",
        TechnicalDetails = """
            Windows has a hidden "Ultimate Performance" power plan that disables
            power saving features. It's primarily designed for workstations.

            What it changes:
            - Disables hard disk sleep
            - Disables USB selective suspend
            - Keeps CPU at higher clock speeds
            - Disables display dimming
            """,
        WhatItDoes = $"powercfg -duplicatescheme {UltimatePerformanceGuid}",
        Effectiveness = EffectivenessRating.Minimal,
        RecommendedByDefault = true,
        RequiresRestart = false
    };

    public override async Task<bool> IsAppliedAsync()
    {
        var (success, output, _) = await ProcessHelper.RunCommandAsync(
            "powercfg", "/getactivescheme");

        if (!success) return false;

        // Check if Ultimate Performance or a custom copy is active
        return output.Contains(UltimatePerformanceGuid, StringComparison.OrdinalIgnoreCase) ||
               output.Contains("Ultimate Performance", StringComparison.OrdinalIgnoreCase);
    }

    public override async Task<TweakResult> ApplyAsync()
    {
        try
        {
            Log("Checking current power plan...");

            // Get current active plan for backup
            var (success, currentOutput, _) = await ProcessHelper.RunCommandAsync(
                "powercfg", "/getactivescheme");

            if (success && currentOutput.Contains("GUID:"))
            {
                var guidStart = currentOutput.IndexOf("GUID:") + 6;
                var guidEnd = currentOutput.IndexOf(" ", guidStart);
                if (guidEnd == -1) guidEnd = currentOutput.IndexOf(")", guidStart);
                if (guidEnd > guidStart)
                {
                    _previousPlanGuid = currentOutput.Substring(guidStart, guidEnd - guidStart).Trim();
                    Log($"Backed up current plan: {_previousPlanGuid}");
                }
            }

            // Try to duplicate Ultimate Performance (might already exist)
            Log("Creating Ultimate Performance plan...");
            var (dupSuccess, dupOutput, dupError) = await ProcessHelper.RunCommandAsync(
                "powercfg", $"-duplicatescheme {UltimatePerformanceGuid}");

            string planGuid = UltimatePerformanceGuid;

            // Extract the new GUID if duplicate was created
            if (dupSuccess && dupOutput.Contains("GUID:"))
            {
                var start = dupOutput.IndexOf("GUID:") + 6;
                var end = dupOutput.IndexOf(" ", start);
                if (end == -1) end = dupOutput.Length;
                planGuid = dupOutput.Substring(start, end - start).Trim();
            }

            // Set as active
            Log($"Activating power plan {planGuid}...");
            var (setSuccess, _, setError) = await ProcessHelper.RunCommandAsync(
                "powercfg", $"/setactive {planGuid}");

            if (!setSuccess)
            {
                // Fallback to High Performance if Ultimate fails
                Log("Ultimate Performance failed, trying High Performance...");
                var (fallbackSuccess, _, _) = await ProcessHelper.RunCommandAsync(
                    "powercfg", $"/setactive {HighPerformanceGuid}");

                if (!fallbackSuccess)
                    return TweakResult.Fail("Could not activate performance power plan", setError);

                return TweakResult.Ok("Activated High Performance plan (Ultimate not available)");
            }

            return TweakResult.Ok("Ultimate Performance power plan activated");
        }
        catch (Exception ex)
        {
            return TweakResult.Fail("Failed to change power plan", ex.Message);
        }
    }

    public override async Task<TweakResult> RevertAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(_previousPlanGuid))
            {
                // Default to Balanced if we don't have a backup
                _previousPlanGuid = "381b4222-f694-41f0-9685-ff5bb260df2e";
            }

            Log($"Restoring power plan to {_previousPlanGuid}...");

            var (success, _, error) = await ProcessHelper.RunCommandAsync(
                "powercfg", $"/setactive {_previousPlanGuid}");

            if (!success)
                return TweakResult.Fail("Could not restore power plan", error);

            return TweakResult.Ok("Power plan restored");
        }
        catch (Exception ex)
        {
            return TweakResult.Fail("Failed to restore power plan", ex.Message);
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        var (success, output, _) = await ProcessHelper.RunCommandAsync(
            "powercfg", "/getactivescheme");

        if (!success) return "Unknown";

        if (output.Contains("Ultimate", StringComparison.OrdinalIgnoreCase))
            return "Ultimate Performance";
        if (output.Contains("High performance", StringComparison.OrdinalIgnoreCase))
            return "High Performance";
        if (output.Contains("Balanced", StringComparison.OrdinalIgnoreCase))
            return "Balanced";

        return "Custom";
    }
}
