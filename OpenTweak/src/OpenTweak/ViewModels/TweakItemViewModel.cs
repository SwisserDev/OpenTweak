using System.Windows.Input;
using OpenTweak.Models;
using OpenTweak.Services;

namespace OpenTweak.ViewModels;

/// <summary>
/// ViewModel for a single tweak item in the UI.
/// </summary>
public class TweakItemViewModel : ViewModelBase
{
    private readonly ITweakService _service;
    private bool _isEnabled;
    private bool _isApplied;
    private bool _isLoading;
    private bool _isSelected;
    private string _status = "Checking...";
    private bool _showInfo;

    public TweakItemViewModel(ITweakService service)
    {
        _service = service;
        _isEnabled = service.Info.RecommendedByDefault;

        ApplyCommand = new AsyncRelayCommand(ApplyAsync, () => !IsLoading);
        RevertCommand = new AsyncRelayCommand(RevertAsync, () => !IsLoading && IsApplied);
        ToggleInfoCommand = new RelayCommand(() => ShowInfo = !ShowInfo);

        // Load initial status
        _ = RefreshStatusAsync();
    }

    public TweakInfo Info => _service.Info;

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    public bool IsApplied
    {
        get => _isApplied;
        private set
        {
            if (SetProperty(ref _isApplied, value))
                OnPropertyChanged(nameof(StatusColor));
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public string Status
    {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    public bool ShowInfo
    {
        get => _showInfo;
        set => SetProperty(ref _showInfo, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public string EffectivenessText => Info.Effectiveness switch
    {
        EffectivenessRating.Effective => "Effective",
        EffectivenessRating.Minimal => "Minimal Impact",
        EffectivenessRating.Placebo => "Likely Placebo",
        _ => "Unknown"
    };

    public string StatusColor => IsApplied ? "#22c55e" : "#666666";

    public ICommand ApplyCommand { get; }
    public ICommand RevertCommand { get; }
    public ICommand ToggleInfoCommand { get; }

    public async Task RefreshStatusAsync()
    {
        try
        {
            IsLoading = true;
            IsApplied = await _service.IsAppliedAsync();
            Status = await _service.GetStatusAsync();
        }
        catch (Exception ex)
        {
            Status = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task<TweakResult> ApplyAsync()
    {
        try
        {
            IsLoading = true;
            var result = await _service.ApplyAsync();
            await RefreshStatusAsync();
            return result;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task<TweakResult> RevertAsync()
    {
        try
        {
            IsLoading = true;
            var result = await _service.RevertAsync();
            await RefreshStatusAsync();
            return result;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
