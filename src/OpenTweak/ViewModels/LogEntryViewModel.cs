using System.Windows.Media;
using OpenTweak.Models;

namespace OpenTweak.ViewModels;

/// <summary>
/// ViewModel wrapper for LogEntry with UI-specific properties.
/// </summary>
public class LogEntryViewModel : ViewModelBase
{
    private static readonly Dictionary<LogLevel, (string Icon, Color Color)> LevelStyles = new()
    {
        { LogLevel.Success, ("✓", Color.FromRgb(34, 197, 94)) },    // Green
        { LogLevel.Error, ("✗", Color.FromRgb(239, 68, 68)) },      // Red
        { LogLevel.Warning, ("⚠", Color.FromRgb(234, 179, 8)) },    // Yellow
        { LogLevel.Info, ("ℹ", Color.FromRgb(59, 130, 246)) },      // Blue
        { LogLevel.Progress, ("◌", Color.FromRgb(139, 92, 246)) },  // Purple
    };

    public LogEntry Entry { get; }

    public LogEntryViewModel(LogEntry entry)
    {
        Entry = entry;
    }

    public string TweakName => Entry.TweakName;
    public string Message => Entry.Message;
    public string FormattedTime => Entry.Timestamp.ToString("HH:mm:ss");

    public string IconText => LevelStyles.TryGetValue(Entry.Level, out var style) ? style.Icon : "•";

    public SolidColorBrush IconBrush
    {
        get
        {
            if (LevelStyles.TryGetValue(Entry.Level, out var style))
                return new SolidColorBrush(style.Color);
            return new SolidColorBrush(Colors.Gray);
        }
    }

    public SolidColorBrush BackgroundBrush
    {
        get
        {
            if (LevelStyles.TryGetValue(Entry.Level, out var style))
            {
                var color = style.Color;
                return new SolidColorBrush(Color.FromArgb(25, color.R, color.G, color.B));
            }
            return new SolidColorBrush(Color.FromArgb(25, 128, 128, 128));
        }
    }

    public SolidColorBrush BorderBrush
    {
        get
        {
            if (LevelStyles.TryGetValue(Entry.Level, out var style))
            {
                var color = style.Color;
                return new SolidColorBrush(Color.FromArgb(60, color.R, color.G, color.B));
            }
            return new SolidColorBrush(Color.FromArgb(60, 128, 128, 128));
        }
    }
}
