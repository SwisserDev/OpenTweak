using OpenTweak.Models;

namespace OpenTweak.Services;

/// <summary>
/// Interface for all tweak services. Each service handles one type of optimization.
/// </summary>
public interface ITweakService
{
    /// <summary>
    /// Metadata about this tweak.
    /// </summary>
    TweakInfo Info { get; }

    /// <summary>
    /// Check if the tweak is currently applied.
    /// </summary>
    Task<bool> IsAppliedAsync();

    /// <summary>
    /// Apply the tweak. Returns backup data for restoration.
    /// </summary>
    Task<TweakResult> ApplyAsync();

    /// <summary>
    /// Revert the tweak to original state.
    /// </summary>
    Task<TweakResult> RevertAsync();

    /// <summary>
    /// Get current status for display.
    /// </summary>
    Task<string> GetStatusAsync();
}

/// <summary>
/// Base class with common functionality for tweak services.
/// </summary>
public abstract class TweakServiceBase : ITweakService
{
    public abstract TweakInfo Info { get; }

    public abstract Task<bool> IsAppliedAsync();
    public abstract Task<TweakResult> ApplyAsync();
    public abstract Task<TweakResult> RevertAsync();

    public virtual async Task<string> GetStatusAsync()
    {
        var isApplied = await IsAppliedAsync();
        return isApplied ? "Active" : "Not Applied";
    }

    /// <summary>
    /// Log action for transparency.
    /// </summary>
    protected void Log(string message)
    {
        TweakLogger.Instance.Log(Info.Id, message);
    }
}

/// <summary>
/// Simple logger to track all tweak operations for transparency.
/// </summary>
public class TweakLogger
{
    public static TweakLogger Instance { get; } = new();

    private readonly List<LogEntry> _entries = new();

    public IReadOnlyList<LogEntry> Entries => _entries.AsReadOnly();

    public event Action<LogEntry>? OnLog;

    public void Log(string tweakId, string message)
    {
        var entry = new LogEntry
        {
            Timestamp = DateTime.Now,
            TweakId = tweakId,
            Message = message
        };
        _entries.Add(entry);
        OnLog?.Invoke(entry);
    }

    public void Clear() => _entries.Clear();

    public string Export() => string.Join(Environment.NewLine,
        _entries.Select(e => $"[{e.Timestamp:HH:mm:ss}] [{e.TweakId}] {e.Message}"));
}

public class LogEntry
{
    public DateTime Timestamp { get; init; }
    public required string TweakId { get; init; }
    public required string Message { get; init; }
}
