using OpenTweak.Helpers;
using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Disables unnecessary Windows services and telemetry.
/// </summary>
public class DebloaterService : TweakServiceBase
{
    private readonly List<(string ServiceName, string DisplayName)> _targetServices = new()
    {
        ("DiagTrack", "Connected User Experiences and Telemetry"),
        ("SysMain", "Superfetch/SysMain"),
        ("WSearch", "Windows Search Indexer")
    };

    private readonly Dictionary<string, string> _previousStates = new();

    public override TweakInfo Info => new()
    {
        Id = "debloater",
        Name = "Windows Debloater",
        Description = "Disables telemetry and non-essential services",
        Category = "System",
        TechnicalDetails = """
            Disables these Windows services:

            • DiagTrack (Telemetry) - Sends usage data to Microsoft
              Disabling: Reduces background network/disk activity

            • SysMain (Superfetch) - Preloads frequently used apps
              Disabling: Can reduce disk activity on HDDs
              Note: Actually useful on SSDs, disabling may hurt performance

            • WSearch (Windows Search) - Indexes files for search
              Disabling: Reduces background CPU/disk usage
              Note: Windows search will be slower/unavailable

            Reality check:
            - These services use minimal resources on modern PCs
            - Disabling may break some Windows features
            - Impact on gaming: negligible in most cases
            """,
        WhatItDoes = """
            For each service:
            sc config ServiceName start= disabled
            sc stop ServiceName
            """,
        Effectiveness = EffectivenessRating.Minimal,
        RecommendedByDefault = false,
        RequiresRestart = false
    };

    public override async Task<bool> IsAppliedAsync()
    {
        foreach (var (serviceName, _) in _targetServices)
        {
            var (success, output, _) = await ProcessHelper.RunCommandAsync(
                "sc", $"qc {serviceName}");

            if (success && output.Contains("DISABLED"))
                return true;
        }

        return false;
    }

    public override async Task<TweakResult> ApplyAsync()
    {
        try
        {
            _previousStates.Clear();
            int disabled = 0;
            var errors = new List<string>();

            foreach (var (serviceName, displayName) in _targetServices)
            {
                Log($"Processing {displayName}...");

                // Get current state for backup
                var (querySuccess, queryOutput, _) = await ProcessHelper.RunCommandAsync(
                    "sc", $"qc {serviceName}");

                if (querySuccess)
                {
                    if (queryOutput.Contains("AUTO_START"))
                        _previousStates[serviceName] = "auto";
                    else if (queryOutput.Contains("DEMAND_START"))
                        _previousStates[serviceName] = "demand";
                    else if (queryOutput.Contains("DISABLED"))
                    {
                        Log($"{displayName} already disabled");
                        continue;
                    }
                }

                // Disable the service
                var (disableSuccess, _, disableError) = await ProcessHelper.RunCommandAsync(
                    "sc", $"config {serviceName} start= disabled");

                if (!disableSuccess)
                {
                    errors.Add($"{displayName}: {disableError}");
                    continue;
                }

                // Stop the service
                await ProcessHelper.RunCommandAsync("sc", $"stop {serviceName}");

                Log($"Disabled {displayName}");
                disabled++;
            }

            if (disabled == 0 && errors.Count > 0)
            {
                return TweakResult.Fail(
                    "Could not disable any services",
                    string.Join("; ", errors));
            }

            var message = $"Disabled {disabled} service(s)";
            if (errors.Count > 0)
                message += $" ({errors.Count} failed)";

            return TweakResult.Ok(message);
        }
        catch (Exception ex)
        {
            return TweakResult.Fail("Failed to disable services", ex.Message);
        }
    }

    public override async Task<TweakResult> RevertAsync()
    {
        try
        {
            int restored = 0;

            foreach (var (serviceName, displayName) in _targetServices)
            {
                var startType = _previousStates.TryGetValue(serviceName, out var type)
                    ? type
                    : "auto"; // Default to auto if we don't have backup

                var (success, _, _) = await ProcessHelper.RunCommandAsync(
                    "sc", $"config {serviceName} start= {startType}");

                if (success)
                {
                    // Start the service
                    await ProcessHelper.RunCommandAsync("sc", $"start {serviceName}");
                    Log($"Restored {displayName}");
                    restored++;
                }
            }

            return TweakResult.Ok($"Restored {restored} service(s)");
        }
        catch (Exception ex)
        {
            return TweakResult.Fail("Failed to restore services", ex.Message);
        }
    }

    public override async Task<string> GetStatusAsync()
    {
        int disabled = 0;

        foreach (var (serviceName, _) in _targetServices)
        {
            var (success, output, _) = await ProcessHelper.RunCommandAsync(
                "sc", $"qc {serviceName}");

            if (success && output.Contains("DISABLED"))
                disabled++;
        }

        return disabled > 0 ? $"{disabled} service(s) disabled" : "All services running";
    }
}
