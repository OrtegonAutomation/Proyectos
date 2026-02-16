using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FifoCleanup.Engine.Models;

namespace FifoCleanup.UI.ViewModels;

/// <summary>
/// ViewModel principal de la aplicación.
/// Gestiona el estado compartido entre pestañas y la barra de estado.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private string _statusMessage = "Listo";
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private double _busyProgress;
    [ObservableProperty] private string _busyMessage = "";
    [ObservableProperty] private FifoConfiguration _configuration = new();
    [ObservableProperty] private StorageStatus? _currentStatus;
    [ObservableProperty] private bool _isScheduledRunning;
    [ObservableProperty] private bool _isPreventiveRunning;
    [ObservableProperty] private string _configFilePath = "fifo_config.json";

    // Sub-ViewModels
    public DashboardViewModel Dashboard { get; }
    public ConfigurationViewModel Configuration_VM { get; }
    public SimulationViewModel Simulation { get; }
    public ExecutionViewModel Execution { get; }
    public BitacoraViewModel Bitacora { get; }

    public MainViewModel()
    {
        Dashboard = new DashboardViewModel(this);
        Configuration_VM = new ConfigurationViewModel(this);
        Simulation = new SimulationViewModel(this);
        Execution = new ExecutionViewModel(this);
        Bitacora = new BitacoraViewModel(this);
    }

    /// <summary>Cargar configuración al iniciar</summary>
    public async Task InitializeAsync()
    {
        try
        {
            Configuration = await App.ConfigService.LoadAsync(ConfigFilePath);
            StatusMessage = $"Configuración cargada desde {ConfigFilePath}";

            if (!string.IsNullOrEmpty(Configuration.BitacoraPath))
                await App.BitacoraService.InitializeAsync(Configuration.BitacoraPath);

            await App.BitacoraService.LogAsync(new BitacoraEntry
            {
                EventType = BitacoraEventType.SystemStart,
                Action = "APLICACION_INICIADA",
                Source = "UI"
            });
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error al cargar configuración: {ex.Message}";
            Configuration = App.ConfigService.GetDefault();
        }
    }

    /// <summary>Actualizar estado de storage (usado por Dashboard y otros)</summary>
    [RelayCommand]
    public async Task RefreshStorageAsync()
    {
        if (string.IsNullOrEmpty(Configuration.StoragePath))
        {
            StatusMessage = "Configure la ruta de almacenamiento primero.";
            return;
        }

        IsBusy = true;
        BusyMessage = "Escaneando almacenamiento...";

        try
        {
            var progress = new Progress<(string message, double percent)>(p =>
            {
                BusyMessage = p.message;
                BusyProgress = p.percent;
            });

            CurrentStatus = await App.InventoryService.ScanAsync(Configuration.StoragePath, default, progress);
            StatusMessage = $"Escaneo completado en {CurrentStatus.ScanDurationMs}ms. " +
                          $"Uso: {CurrentStatus.UsagePercent:F1}%";

            Dashboard.UpdateFromStatus(CurrentStatus);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error en escaneo: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            BusyMessage = "";
            BusyProgress = 0;
        }
    }
}
