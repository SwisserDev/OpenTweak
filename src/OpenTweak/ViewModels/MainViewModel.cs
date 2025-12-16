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
    private bool _showLog;
    private TweakItemViewModel? _selectedTweak;

    public MainViewModel()
    {
        // Initialize all tweak services (4x5 grid = 20 tweaks)
        Tweaks = new ObservableCollection<TweakItemViewModel>
        {
            // Row 1: Power & Performance
            new(new PowerPlanService()),
            new(new PowerThrottlingService()),
            new(new TimerResolutionService()),
            new(new VbsDisableService()),

            // Row 2: GPU & Graphics
            new(new GpuPriorityService()),
            new(new HagsService()),
            new(new GameDvrService()),
            new(new WindowedOptimizationsService()),

            // Row 3: System & CPU
            new(new SystemResponsivenessService()),
            new(new GamePriorityService()),
            new(new CpuParkingService()),
            new(new PriorityBoostService()),

            // Row 4: Input & Windows
            new(new MouseAccelerationService()),
            new(new FsoDisableService()),
            new(new StartupDelayService()),
            new(new VisualEffectsService()),

            // Row 5: Network & Cleanup
            new(new NetworkTweakService()),
            new(new NetworkThrottlingService()),
            new(new DebloaterService()),
            new(new ServiceManagerService()),
        };

        // Utility tools (one-time actions)
        CacheCleanerTweak = new TweakItemViewModel(new CacheCleanerService());
        ShaderCleanerTweak = new TweakItemViewModel(new ShaderCacheCleanerService());
        RamCleanerTweak = new TweakItemViewModel(new RamCleanerService());

        // Commands
        OptimizeAllCommand = new AsyncRelayCommand(OptimizeAllAsync, () => !IsOptimizing);
        RevertAllCommand = new AsyncRelayCommand(RevertAllAsync, () => !IsOptimizing);
        CleanCacheCommand = new AsyncRelayCommand(CleanCacheAsync, () => !IsOptimizing);
        CleanShadersCommand = new AsyncRelayCommand(CleanShadersAsync, () => !IsOptimizing);
        CleanRamCommand = new AsyncRelayCommand(CleanRamAsync, () => !IsOptimizing);
        ToggleLogCommand = new RelayCommand(() => ShowLog = !ShowLog);
        ExportLogCommand = new RelayCommand(ExportLog);
        OpenGitHubCommand = new RelayCommand(OpenGitHub);
        SelectTweakCommand = new RelayCommand<TweakItemViewModel>(SelectTweak);

        // Auto-select first tweak
        SelectedTweak = Tweaks.FirstOrDefault();

        // Initialize log entries collection
        LogEntries = new ObservableCollection<LogEntryViewModel>();

        // Subscribe to log events
        TweakLogger.Instance.OnLog += entry =>
        {
            // Ensure we're on UI thread
            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                LogEntries.Add(new LogEntryViewModel(entry));

                // Keep max 50 entries in UI
                while (LogEntries.Count > 50)
                    LogEntries.RemoveAt(0);
            });
        };

        // Check admin rights
        IsAdmin = ProcessHelper.IsAdministrator();
        if (!IsAdmin)
        {
            TweakLogger.Instance.LogWarning("system", "System", "Not running as Administrator. Some tweaks may fail.");
        }
    }

    public ObservableCollection<TweakItemViewModel> Tweaks { get; }
    public TweakItemViewModel CacheCleanerTweak { get; }
    public TweakItemViewModel ShaderCleanerTweak { get; }
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

    public ObservableCollection<LogEntryViewModel> LogEntries { get; private set; }

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
    public ICommand CleanShadersCommand { get; }
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
            ShowLog = true;
            TweakLogger.Instance.Clear();
            LogEntries.Clear();

            TweakLogger.Instance.LogInfo("system", "Optimization", "Starting optimization...");

            var enabledTweaks = Tweaks.Where(t => t.IsEnabled).ToList();
            int success = 0;
            int failed = 0;

            foreach (var tweak in enabledTweaks)
            {
                TweakLogger.Instance.LogProgress(tweak.Info.Id, tweak.Info.Name, "Applying...");
                var result = await tweak.ApplyAsync();

                if (result.Success)
                {
                    TweakLogger.Instance.LogSuccess(tweak.Info.Id, tweak.Info.Name, result.Message);
                    success++;
                }
                else
                {
                    var errorMsg = result.ErrorDetails != null
                        ? $"{result.Message} - {result.ErrorDetails}"
                        : result.Message;
                    TweakLogger.Instance.LogError(tweak.Info.Id, tweak.Info.Name, errorMsg);
                    failed++;
                }
            }

            TweakLogger.Instance.LogInfo("system", "Optimization",
                $"Complete: {success} applied, {failed} failed");
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

            TweakLogger.Instance.LogInfo("system", "Revert", "Reverting all tweaks...");

            foreach (var tweak in Tweaks.Where(t => t.IsApplied))
            {
                TweakLogger.Instance.LogProgress(tweak.Info.Id, tweak.Info.Name, "Reverting...");
                var result = await tweak.RevertAsync();

                if (result.Success)
                    TweakLogger.Instance.LogSuccess(tweak.Info.Id, tweak.Info.Name, result.Message);
                else
                    TweakLogger.Instance.LogError(tweak.Info.Id, tweak.Info.Name, result.Message);
            }

            TweakLogger.Instance.LogInfo("system", "Revert", "Revert complete");
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

            TweakLogger.Instance.LogProgress("cache", "Cache Cleaner", "Cleaning FiveM cache...");
            var result = await CacheCleanerTweak.ApplyAsync();

            if (result.Success)
                TweakLogger.Instance.LogSuccess("cache", "Cache Cleaner", result.Message);
            else
                TweakLogger.Instance.LogError("cache", "Cache Cleaner", result.Message);
        }
        finally
        {
            IsOptimizing = false;
        }
    }

    private async Task CleanShadersAsync()
    {
        try
        {
            IsOptimizing = true;
            ShowLog = true;

            TweakLogger.Instance.LogProgress("shaders", "Shader Cleaner", "Cleaning shader caches...");
            var result = await ShaderCleanerTweak.ApplyAsync();

            if (result.Success)
                TweakLogger.Instance.LogSuccess("shaders", "Shader Cleaner", result.Message);
            else
                TweakLogger.Instance.LogError("shaders", "Shader Cleaner", result.Message);
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

            TweakLogger.Instance.LogProgress("ram", "RAM Cleaner", "Cleaning memory...");
            var result = await RamCleanerTweak.ApplyAsync();

            if (result.Success)
                TweakLogger.Instance.LogSuccess("ram", "RAM Cleaner", result.Message);
            else
                TweakLogger.Instance.LogError("ram", "RAM Cleaner", result.Message);
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

        File.WriteAllText(path, TweakLogger.Instance.Export());
        TweakLogger.Instance.LogSuccess("system", "Export", $"Log exported to Desktop");
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
