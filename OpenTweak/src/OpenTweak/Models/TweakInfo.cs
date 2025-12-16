namespace OpenTweak.Models;

/// <summary>
/// Represents a single optimization tweak with its metadata.
/// </summary>
public class TweakInfo
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Category { get; init; }

    /// <summary>
    /// Detailed technical explanation shown in info popup.
    /// </summary>
    public required string TechnicalDetails { get; init; }

    /// <summary>
    /// The actual registry key, command, or code that gets executed.
    /// </summary>
    public required string WhatItDoes { get; init; }

    /// <summary>
    /// Honest effectiveness rating.
    /// </summary>
    public required EffectivenessRating Effectiveness { get; init; }

    /// <summary>
    /// Whether this tweak is recommended by default.
    /// </summary>
    public bool RecommendedByDefault { get; init; } = true;

    /// <summary>
    /// Whether the tweak requires a system restart to take effect.
    /// </summary>
    public bool RequiresRestart { get; init; } = false;
}

public enum EffectivenessRating
{
    /// <summary>
    /// Actually helps in most cases.
    /// </summary>
    Effective,

    /// <summary>
    /// Might help in specific scenarios.
    /// </summary>
    Minimal,

    /// <summary>
    /// Probably doesn't do anything meaningful.
    /// </summary>
    Placebo
}

/// <summary>
/// Result of applying or reverting a tweak.
/// </summary>
public class TweakResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? ErrorDetails { get; init; }

    public static TweakResult Ok(string message = "Applied successfully") =>
        new() { Success = true, Message = message };

    public static TweakResult Fail(string message, string? details = null) =>
        new() { Success = false, Message = message, ErrorDetails = details };
}

/// <summary>
/// Stores original values for restoration.
/// </summary>
public class TweakBackup
{
    public required string TweakId { get; init; }
    public required DateTime BackupTime { get; init; }
    public required Dictionary<string, object?> OriginalValues { get; init; }
}
