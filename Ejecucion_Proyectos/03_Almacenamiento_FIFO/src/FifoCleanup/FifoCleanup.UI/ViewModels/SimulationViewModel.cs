using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FifoCleanup.Engine.Models;

namespace FifoCleanup.UI.ViewModels;

/// <summary>
/// ViewModel para la pestaña Simulación.
/// Genera datos sintéticos y ejecuta algoritmo FIFO en modo preview.
/// </summary>
public partial class SimulationViewModel : ObservableObject
{
    private readonly MainViewModel _main;
    private CancellationTokenSource? _cts;

    // Parámetros de simulación
    [ObservableProperty] private int _numberOfAssets = 5;
    [ObservableProperty] private int _variablesPerAsset = 3;
    [ObservableProperty] private int _daysOfHistory = 30;
    [ObservableProperty] private double _avgDayFolderSizeMB = 50;
    [ObservableProperty] private double _simulatedDiskSizeGB = 100;
    [ObservableProperty] private double _initialUsagePercent = 80;
    [ObservableProperty] private double _thresholdPercent = 85;
    [ObservableProperty] private double _cleanupCapPercent = 20;

    // Parámetros de simulación continua
    [ObservableProperty] private double _dataRateMBPerInterval = 100;
    [ObservableProperty] private int _intervalSeconds = 60;
    [ObservableProperty] private bool _isContinuousRunning;
    [ObservableProperty] private string _continuousDataGenerated = "0 MB";

    // Estado de la simulación
    [ObservableProperty] private bool _isRunning;
    [ObservableProperty] private double _progress;
    [ObservableProperty] private string _progressMessage = "";
    [ObservableProperty] private string _simulationPath = "";

    // Resultados
    [ObservableProperty] private bool _hasResults;
    [ObservableProperty] private string _resultSummary = "";
    [ObservableProperty] private string _usageBefore = "--";
    [ObservableProperty] private string _usageAfter = "--";
    [ObservableProperty] private string _bytesFreed = "--";
    [ObservableProperty] private string _foldersDeleted = "--";
    [ObservableProperty] private string _duration = "--";

    public ObservableCollection<string> LogMessages { get; } = new();

    public SimulationViewModel(MainViewModel main)
    {
        _main = main;
        SimulationPath = Path.Combine(Path.GetTempPath(), "FifoSimulation");
    }

    /// <summary>
    /// Comando de prueba para forzar evaluación RF-08 usando un archivo generado en la simulación.
    /// Útil para verificar que el servicio preventivo procesa eventos y ejecuta limpieza local.
    /// </summary>
    [RelayCommand]
    private async Task TestPreventiveAsync()
    {
        if (!Directory.Exists(SimulationPath))
        {
            _main.StatusMessage = "No hay datos de simulación para probar RF-08.";
            return;
        }

        var file = Directory.EnumerateFiles(SimulationPath, "data_*.bin", SearchOption.AllDirectories).FirstOrDefault();
        if (file == null)
        {
            _main.StatusMessage = "No se encontraron archivos de datos para probar RF-08.";
            return;
        }

        // Intentar encolar y forzar evaluación en el servicio concreto (si está expuesto)
        if (App.PreventiveService is FifoCleanup.Engine.Services.PreventiveMonitorService svc)
        {
            svc.EnqueuePathForProcessing(file);

            // Extraer asset/variable desde la ruta relativa
            try
            {
                var rel = Path.GetRelativePath(SimulationPath, file);
                var parts = rel.Split(Path.DirectorySeparatorChar);
                if (parts.Length >= 2)
                {
                    await svc.ForceEvaluateAsync(parts[0], parts[1]);
                    _main.StatusMessage = "Prueba RF-08 ejecutada (evaluación forzada).";
                    return;
                }
            }
            catch { }

            _main.StatusMessage = "Prueba RF-08 encolada.";
            return;
        }

        _main.StatusMessage = "Servicio preventivo no disponible para prueba.";
    }

    [RelayCommand]
    private async Task RunSimulationAsync()
    {
        IsRunning = true;
        // Si ya existen datos sintéticos en la ruta, reutilizarlos y mantener resultados/logs previos
        bool reuseExisting = Directory.Exists(SimulationPath) &&
                             Directory.EnumerateFileSystemEntries(SimulationPath).Any();

        if (!reuseExisting)
        {
            HasResults = false;
            LogMessages.Clear();
        }
        _cts = new CancellationTokenSource();

        try
        {
            var parameters = new SimulationParams
            {
                SimulationPath = SimulationPath,
                NumberOfAssets = NumberOfAssets,
                VariablesPerAsset = VariablesPerAsset,
                DaysOfHistory = DaysOfHistory,
                AvgDayFolderSizeMB = AvgDayFolderSizeMB,
                SimulatedDiskSizeGB = SimulatedDiskSizeGB,
                InitialUsagePercent = InitialUsagePercent,
                ThresholdPercent = ThresholdPercent,
                CleanupCapPercent = CleanupCapPercent,
                GenerateEData = true,
                GenerateFData = true
            };

            var progress = new Progress<(string message, double percent)>(p =>
            {
                ProgressMessage = p.message;
                Progress = p.percent;
                LogMessages.Add($"[{DateTime.Now:HH:mm:ss}] {p.message}");
            });

            var result = await App.SimulationService.RunSimulationAsync(parameters, _cts.Token, progress);

            if (reuseExisting)
            {
                LogMessages.Add("✓ Reutilizando datos de simulación existentes.");
            }

            // Mostrar resultados
            HasResults = true;
            UsageBefore = $"{result.StatusBefore.UsagePercent:F1}%";
            UsageAfter = $"{result.StatusAfter.UsagePercent:F1}%";
            BytesFreed = result.CleanupResult.BytesFreedFormatted;
            FoldersDeleted = result.CleanupResult.FoldersDeleted.ToString();
            Duration = $"{result.TotalDurationMs}ms";

            ResultSummary = result.CleanupResult.Success
                ? $"✓ Simulación exitosa. Se liberarían {result.CleanupResult.BytesFreedFormatted} " +
                  $"({result.CleanupResult.FoldersDeleted} carpetas)."
                : $"✗ {result.CleanupResult.Message}";

            foreach (var msg in result.LogMessages)
                LogMessages.Add(msg);

            _main.StatusMessage = "Simulación completada.";
        }
        catch (OperationCanceledException)
        {
            _main.StatusMessage = "Simulación cancelada.";
            LogMessages.Add("⚠ Simulación cancelada por el usuario.");
        }
        catch (Exception ex)
        {
            _main.StatusMessage = $"Error en simulación: {ex.Message}";
            LogMessages.Add($"✗ Error: {ex.Message}");
        }
        finally
        {
            IsRunning = false;
        }
    }

    [RelayCommand]
    private void CancelSimulation()
    {
        _cts?.Cancel();
    }

    [RelayCommand]
    private async Task CleanupSimulationDataAsync()
    {
        try
        {
            await App.SimulationService.CleanupSyntheticDataAsync(SimulationPath);
            _main.StatusMessage = "Datos de simulación eliminados.";
            LogMessages.Add("✓ Datos sintéticos eliminados.");
        }
        catch (Exception ex)
        {
            _main.StatusMessage = $"Error al limpiar datos de simulación: {ex.Message}";
        }
    }

    /// <summary>Iniciar/Detener simulación continua de ingreso de datos</summary>
    [RelayCommand]
    private async Task ToggleContinuousSimulationAsync()
    {
        if (IsContinuousRunning)
        {
            // Detener simulación continua
            await App.SimulationService.StopContinuousSimulationAsync();
            IsContinuousRunning = false;
            _main.StatusMessage = "Simulación continua detenida.";
            LogMessages.Add($"[{DateTime.Now:HH:mm:ss}] ⏸ Simulación continua detenida. Total generado: {ContinuousDataGenerated}");
        }
        else
        {
            // Iniciar simulación continua
            if (!Directory.Exists(SimulationPath))
            {
                _main.StatusMessage = "Primero ejecute una simulación inicial para crear la estructura de datos.";
                return;
            }

            try
            {
                var parameters = new ContinuousSimulationParams
                {
                    SimulationPath = SimulationPath,
                    DataRateMBPerInterval = DataRateMBPerInterval,
                    IntervalSeconds = IntervalSeconds,
                    GenerateEData = true,
                    GenerateFData = true,
                    SizeVariationPercent = 20
                };

                // Suscribirse al evento de generación de datos
                App.SimulationService.OnContinuousDataGenerated += OnContinuousDataGenerated;

                await App.SimulationService.StartContinuousSimulationAsync(parameters);
                IsContinuousRunning = true;
                _main.StatusMessage = $"Simulación continua iniciada: {DataRateMBPerInterval} MB cada {IntervalSeconds}s";
                LogMessages.Add($"[{DateTime.Now:HH:mm:ss}] ▶ Simulación continua iniciada: {DataRateMBPerInterval} MB cada {IntervalSeconds}s");
            }
            catch (Exception ex)
            {
                _main.StatusMessage = $"Error al iniciar simulación continua: {ex.Message}";
                LogMessages.Add($"[{DateTime.Now:HH:mm:ss}] ✗ Error: {ex.Message}");
            }
        }
    }

    private void OnContinuousDataGenerated(object? sender, (string assetId, string variableId, long byteGenerated) args)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            long totalMB = App.SimulationService.ContinuousDataGeneratedMB;
            ContinuousDataGenerated = totalMB >= 1024 
                ? $"{totalMB / 1024.0:F2} GB" 
                : $"{totalMB} MB";

            LogMessages.Add($"[{DateTime.Now:HH:mm:ss}] 📁 Datos generados: {args.assetId}/{args.variableId} ({args.byteGenerated / (1024 * 1024)} MB)");
        });
    }
}

