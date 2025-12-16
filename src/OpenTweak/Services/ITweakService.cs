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
/// Logger to track all tweak operations with visual status levels.
/// </summary>
public class TweakLogger
{
    public static TweakLogger Instance { get; } = new();

    private readonly List<LogEntry> _entries = new();
    private const int MaxEntries = 100;

    public IReadOnlyList<LogEntry> Entries => _entries.AsReadOnly();

    public event Action<LogEntry>? OnLog;

    /// <summary>
    /// Log with full control over all parameters.
    /// </summary>
    public void Log(string tweakId, string tweakName, string message, LogLevel level = LogLevel.Info)
    {
        var entry = new LogEntry
        {
            Timestamp = DateTime.Now,
            TweakId = tweakId,
            TweakName = tweakName,
            Message = message,
            Level = level
        };

        _entries.Add(entry);

        // Keep list bounded
        while (_entries.Count > MaxEntries)
            _entries.RemoveAt(0);

        OnLog?.Invoke(entry);
    }

    /// <summary>
    /// Legacy log method for backward compatibility.
    /// </summary>
    public void Log(string tweakId, string message)
    {
        Log(tweakId, tweakId, message, LogLevel.Info);
    }

    public void LogSuccess(string tweakId, string tweakName, string message)
        => Log(tweakId, tweakName, message, LogLevel.Success);

    public void LogError(string tweakId, string tweakName, string message)
        => Log(tweakId, tweakName, message, LogLevel.Error);

    public void LogWarning(string tweakId, string tweakName, string message)
        => Log(tweakId, tweakName, message, LogLevel.Warning);

    public void LogProgress(string tweakId, string tweakName, string message)
        => Log(tweakId, tweakName, message, LogLevel.Progress);

    public void LogInfo(string tweakId, string tweakName, string message)
        => Log(tweakId, tweakName, message, LogLevel.Info);

    public void Clear() => _entries.Clear();

    public string Export() => string.Join(Environment.NewLine,
        _entries.Select(e => $"[{e.Timestamp:HH:mm:ss}] [{e.Level}] [{e.TweakId}] {e.Message}"));
}
