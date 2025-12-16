using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using OpenTweak.Helpers;
using OpenTweak.Models;
using OpenTweak.Services;

namespace OpenTweak.ViewModels;

/// <summary>
/// Main ViewModel for the application window.
/// </summary>
public class MainViewModel : ViewModelBase
{
    private bool _isOptimizing;
    private string _logText = string.Empty;
    private bool _showLog;
    private TweakItemViewModel? _selectedTweak;

    public MainViewModel()
    {
        // Initialize all tweak services (3x4 grid = 11 tweaks + 1 empty slot)
        Tweaks = new ObservableCollection<TweakItemViewModel>
        {
            // Row 1: Core tweaks
            new(new PowerPlanService()),
            new(new TimerResolutionService()),
            new(new GpuPriorityService()),

            // Row 2: Core tweaks
            new(new CpuParkingService()),
            new(new NetworkTweakService()),
            new(new DebloaterService()),

            // Row 3: New useful + first placebo
            new(new GameDvrService()),
            new(new MouseAccelerationService()),
            new(new FsoDisableService()),

            // Row 4: Placebo tweaks (educational)
            new(new NetworkThrottlingService()),
            new(new PriorityBoostService()),
        };

        // Utility tools (one-time actions)
        CacheCleanerTweak = new TweakItemViewModel(new CacheCleanerService());
        RamCleanerTweak = new TweakItemViewModel(new RamCleanerService());

        // Commands
        OptimizeAllCommand = new AsyncRelayCommand(OptimizeAllAsync, () => !IsOptimizing);
        RevertAllCommand = new AsyncRelayCommand(RevertAllAsync, () => !IsOptimizing);
        CleanCacheCommand = new AsyncRelayCommand(CleanCacheAsync, () => !IsOptimizing);
        CleanRamCommand = new AsyncRelayCommand(CleanRamAsync, () => !IsOptimizing);
        ToggleLogCommand = new RelayCommand(() => ShowLog = !ShowLog);
        ExportLogCommand = new RelayCommand(ExportLog);
        OpenGitHubCommand = new RelayCommand(OpenGitHub);
        SelectTweakCommand = new RelayCommand<TweakItemViewModel>(SelectTweak);

        // Auto-select first tweak
        SelectedTweak = Tweaks.FirstOrDefault();

        // Subscribe to log events
        TweakLogger.Instance.OnLog += entry =>
        {
            LogText += $"[{entry.Timestamp:HH:mm:ss}] [{entry.TweakId}] {entry.Message}\n";
        };

        // Check admin rights
        IsAdmin = ProcessHelper.IsAdministrator();
        if (!IsAdmin)
        {
            LogText = "WARNING: Not running as Administrator. Some tweaks may fail.\n\n";
        }
    }

    public ObservableCollection<TweakItemViewModel> Tweaks { get; }
    public TweakItemViewModel CacheCleanerTweak { get; }
    public TweakItemViewModel RamCleanerTweak { get; }

    public bool IsAdmin { get; }

    public bool IsOptimizing
    {
        get => _isOptimizing;
        private set
        {
            if (SetProperty(ref _isOptimizing, value))
                OnPropertyChanged(nameof(OptimizeButtonText));
        }
    }

    public string OptimizeButtonText => IsOptimizing ? "OPTIMIZING..." : "OPTIMIZE";

    public string LogText
    {
        get => _logText;
        private set => SetProperty(ref _logText, value);
    }

    public bool ShowLog
    {
        get => _showLog;
        set => SetProperty(ref _showLog, value);
    }

    public TweakItemViewModel? SelectedTweak
    {
        get => _selectedTweak;
        set
        {
            // Deselect previous
            if (_selectedTweak != null)
                _selectedTweak.IsSelected = false;

            if (SetProperty(ref _selectedTweak, value) && value != null)
                value.IsSelected = true;
        }
    }

    public string StatusSummary
    {
        get
        {
            var applied = Tweaks.Count(t => t.IsApplied);
            return applied > 0 ? $"{applied} tweak(s) active" : "No tweaks applied";
        }
    }

    public ICommand OptimizeAllCommand { get; }
    public ICommand RevertAllCommand { get; }
    public ICommand CleanCacheCommand { get; }
    public ICommand CleanRamCommand { get; }
    public ICommand ToggleLogCommand { get; }
    public ICommand ExportLogCommand { get; }
    public ICommand OpenGitHubCommand { get; }
    public ICommand SelectTweakCommand { get; }

    private async Task OptimizeAllAsync()
    {
        try
        {
            IsOptimizing = true;
            ShowLog = true; // Auto-show log for feedback
            TweakLogger.Instance.Clear();
            LogText = "=== Starting Optimization ===\n\n";

            var enabledTweaks = Tweaks.Where(t => t.IsEnabled).ToList();
            int success = 0;
            int failed = 0;

            foreach (var tweak in enabledTweaks)
            {
                LogText += $"Applying: {tweak.Info.Name}...\n";
                var result = await tweak.ApplyAsync();

                if (result.Success)
                {
                    LogText += $"  ✓ {result.Message}\n";
                    success++;
                }
                else
                {
                    LogText += $"  ✗ {result.Message}\n";
                    if (result.ErrorDetails != null)
                        LogText += $"    Details: {result.ErrorDetails}\n";
                    failed++;
                }
            }

            LogText += $"\n=== Done: {success} applied, {failed} failed ===\n";
        }
        finally
        {
            IsOptimizing = false;
        }
    }

    private async Task RevertAllAsync()
    {
        try
        {
            IsOptimizing = true;
            ShowLog = true;
            LogText = "=== Reverting All Tweaks ===\n\n";

            foreach (var tweak in Tweaks.Where(t => t.IsApplied))
            {
                LogText += $"Reverting: {tweak.Info.Name}...\n";
                var result = await tweak.RevertAsync();
                LogText += result.Success
                    ? $"  ✓ {result.Message}\n"
                    : $"  ✗ {result.Message}\n";
            }

            LogText += "\n=== Revert Complete ===\n";
        }
        finally
        {
            IsOptimizing = false;
        }
    }

    private async Task CleanCacheAsync()
    {
        try
        {
            IsOptimizing = true;
            ShowLog = true;
            LogText += "\n--- Cleaning FiveM Cache ---\n";
            var result = await CacheCleanerTweak.ApplyAsync();
            LogText += result.Success
                ? $"✓ {result.Message}\n"
                : $"✗ {result.Message}\n";
        }
        finally
        {
            IsOptimizing = false;
        }
    }

    private async Task CleanRamAsync()
    {
        try
        {
            IsOptimizing = true;
            ShowLog = true;
            LogText += "\n--- Cleaning RAM ---\n";
            var result = await RamCleanerTweak.ApplyAsync();
            LogText += result.Success
                ? $"✓ {result.Message}\n"
                : $"✗ {result.Message}\n";
        }
        finally
        {
            IsOptimizing = false;
        }
    }

    private void ExportLog()
    {
        var path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            $"OpenTweak_Log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");

        File.WriteAllText(path, LogText);
        LogText += $"\n✓ Log exported to: {path}\n";
    }

    private void OpenGitHub()
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "https://github.com/your-repo/opentweak",
            UseShellExecute = true
        });
    }

    private void SelectTweak(TweakItemViewModel? tweak)
    {
        if (tweak != null)
            SelectedTweak = tweak;
    }

    private void RefreshStatusSummary()
    {
        OnPropertyChanged(nameof(StatusSummary));
    }
}
