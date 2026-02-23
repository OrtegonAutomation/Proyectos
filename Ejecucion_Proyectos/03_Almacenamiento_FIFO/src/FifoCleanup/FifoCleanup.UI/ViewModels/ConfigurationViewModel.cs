using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FifoCleanup.Engine.Models;

namespace FifoCleanup.UI.ViewModels;

/// <summary>
/// ViewModel para la pestaña Configuración.
/// Permite editar y guardar la configuración FIFO con validación en vivo.
/// </summary>
public partial class ConfigurationViewModel : ObservableObject
{
    private readonly MainViewModel _main;

    [ObservableProperty] private string _storagePath = "";
    [ObservableProperty] private double _maxStorageSize = 0;
    [ObservableProperty] private double _thresholdPercent = 85;
    [ObservableProperty] private double _cleanupCapPercent = 20;
    [ObservableProperty] private int _scheduledFrequencyHours = 24;
    [ObservableProperty] private int _scheduledHour = 2;
    [ObservableProperty] private int _preventiveThresholdDays = 3;
    [ObservableProperty] private bool _enableScheduledCleanup = true;
    [ObservableProperty] private bool _enablePreventiveCleanup = true;
    [ObservableProperty] private int _maxConcurrentAssets = 5;
    [ObservableProperty] private int _maxDaysToDeletePerAsset = 10;
    [ObservableProperty] private string _bitacoraPath = "bitacora";
    [ObservableProperty] private int _bitacoraRetentionDays = 90;
    [ObservableProperty] private string _validationMessage = "";
    [ObservableProperty] private bool _hasChanges;
    [ObservableProperty] private bool _isValid = true;

    public ConfigurationViewModel(MainViewModel main)
    {
        _main = main;
        LoadFromConfig(main.Configuration);
    }

    public void LoadFromConfig(FifoConfiguration config)
    {
        StoragePath = config.StoragePath;
        MaxStorageSize = config.MaxStorageSizeGB;
        ThresholdPercent = config.ThresholdPercent;
        CleanupCapPercent = config.CleanupCapPercent;
        ScheduledFrequencyHours = config.ScheduledFrequencyHours;
        ScheduledHour = config.ScheduledHour;
        PreventiveThresholdDays = config.PreventiveThresholdDays;
        EnableScheduledCleanup = config.EnableScheduledCleanup;
        EnablePreventiveCleanup = config.EnablePreventiveCleanup;
        MaxConcurrentAssets = config.MaxConcurrentAssets;
        MaxDaysToDeletePerAsset = config.MaxDaysToDeletePerAsset;
        BitacoraPath = config.BitacoraPath;
        BitacoraRetentionDays = config.BitacoraRetentionDays;
        HasChanges = false;
        Validate();
    }

    partial void OnStoragePathChanged(string value) { HasChanges = true; Validate(); }
    partial void OnMaxStorageSizeChanged(double value) { HasChanges = true; Validate(); }
    partial void OnThresholdPercentChanged(double value) { HasChanges = true; Validate(); }
    partial void OnCleanupCapPercentChanged(double value) { HasChanges = true; Validate(); }
    partial void OnScheduledFrequencyHoursChanged(int value) { HasChanges = true; Validate(); }
    partial void OnScheduledHourChanged(int value) { HasChanges = true; Validate(); }
    partial void OnPreventiveThresholdDaysChanged(int value) { HasChanges = true; Validate(); }
    partial void OnMaxConcurrentAssetsChanged(int value) { HasChanges = true; Validate(); }
    partial void OnMaxDaysToDeletePerAssetChanged(int value) { HasChanges = true; Validate(); }

    private void Validate()
    {
        var config = ToConfig();
        var errors = App.ConfigService.Validate(config);
        IsValid = errors.Count == 0;
        ValidationMessage = errors.Count > 0
            ? string.Join("\n", errors)
            : "✓ Configuración válida";
    }

    private FifoConfiguration ToConfig() => new()
    {
        StoragePath = StoragePath,
        MaxStorageSizeGB = MaxStorageSize,
        ThresholdPercent = ThresholdPercent,
        CleanupCapPercent = CleanupCapPercent,
        ScheduledFrequencyHours = ScheduledFrequencyHours,
        ScheduledHour = ScheduledHour,
        PreventiveThresholdDays = PreventiveThresholdDays,
        EnableScheduledCleanup = EnableScheduledCleanup,
        EnablePreventiveCleanup = EnablePreventiveCleanup,
        MaxConcurrentAssets = MaxConcurrentAssets,
        MaxDaysToDeletePerAsset = MaxDaysToDeletePerAsset,
        BitacoraPath = BitacoraPath,
        BitacoraRetentionDays = BitacoraRetentionDays,
        ConfigFilePath = _main.ConfigFilePath
    };

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!IsValid) return;

        var config = ToConfig();
        await App.ConfigService.SaveAsync(config, _main.ConfigFilePath);
        _main.Configuration = config;
        HasChanges = false;
        _main.StatusMessage = "Configuración guardada exitosamente.";

        await App.BitacoraService.LogAsync(new BitacoraEntry
        {
            EventType = BitacoraEventType.Configuration,
            Action = "CONFIGURACION_GUARDADA",
            Details = $"Umbral: {config.ThresholdPercent}%, Cap: {config.CleanupCapPercent}%, Ruta: {config.StoragePath}",
            Source = "UI"
        });
    }

    [RelayCommand]
    private void Cancel()
    {
        LoadFromConfig(_main.Configuration);
    }

    [RelayCommand]
    private void ResetDefaults()
    {
        LoadFromConfig(App.ConfigService.GetDefault());
        HasChanges = true;
    }

    [RelayCommand]
    private void BrowseStoragePath()
    {
        var dialog = new Microsoft.Win32.OpenFolderDialog
        {
            Title = "Seleccionar carpeta de almacenamiento",
            InitialDirectory = string.IsNullOrEmpty(StoragePath) ? @"C:\" : StoragePath
        };

        if (dialog.ShowDialog() == true)
        {
            StoragePath = dialog.FolderName;
        }
    }
}
