namespace OpenTweak.Models;

/// <summary>
/// Severity level for log entries.
/// </summary>
public enum LogLevel
{
    Info,
    Success,
    Warning,
    Error,
    Progress
}

/// <summary>
/// Represents a single log entry with metadata for display.
/// </summary>
public class LogEntry
{
    public required DateTime Timestamp { get; init; }
    public required string TweakId { get; init; }
    public required string TweakName { get; init; }
    public required string Message { get; init; }
    public LogLevel Level { get; init; } = LogLevel.Info;
}
