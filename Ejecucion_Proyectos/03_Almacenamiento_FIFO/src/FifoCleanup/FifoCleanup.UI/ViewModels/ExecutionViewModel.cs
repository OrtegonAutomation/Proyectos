using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FifoCleanup.Engine.Models;

namespace FifoCleanup.UI.ViewModels;

/// <summary>
/// ViewModel para la pestaña Ejecución.
/// Controla la ejecución de limpieza FIFO en producción y los servicios RF-07/RF-08.
/// </summary>
public partial class ExecutionViewModel : ObservableObject
{
    private readonly MainViewModel _main;
    private CancellationTokenSource? _cts;

    [ObservableProperty] private bool _isExecuting;
    [ObservableProperty] private double _progress;
    [ObservableProperty] private string _progressMessage = "";

    // Estado RF-07 y RF-08
    [ObservableProperty] private bool _isScheduledRunning;
    [ObservableProperty] private bool _isPreventiveRunning;
    [ObservableProperty] private string _scheduledNextRun = "N/A";
    [ObservableProperty] private string _preventiveFilesDetected = "0";
    [ObservableProperty] private string _preventiveCleanups = "0";

    // Resultado de última ejecución
    [ObservableProperty] private bool _hasResult;
    [ObservableProperty] private string _resultMessage = "";
    [ObservableProperty] private string _bytesFreed = "--";
    [ObservableProperty] private string _foldersDeleted = "--";

    public ObservableCollection<string> ExecutionLog { get; } = new();

    public ExecutionViewModel(MainViewModel main)
    {
        _main = main;
    }

    /// <summary>Ejecutar limpieza FIFO manual</summary>
    [RelayCommand]
    private async Task ExecuteCleanupAsync()
    {
        if (_main.CurrentStatus == null)
        {
            _main.StatusMessage = "Primero escanee el almacenamiento desde el Dashboard.";
            return;
        }

        IsExecuting = true;
        HasResult = false;
        ExecutionLog.Clear();
        _cts = new CancellationTokenSource();

        try
        {
            ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] Iniciando limpieza FIFO manual...");

            var progress = new Progress<(string message, double percent)>(p =>
            {
                ProgressMessage = p.message;
                Progress = p.percent;
                ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] {p.message}");
            });

            var result = await App.CleanupService.ExecuteGeneralCleanupAsync(
                _main.CurrentStatus, _main.Configuration, _cts.Token, progress);

            HasResult = true;
            ResultMessage = result.Message;
            BytesFreed = result.BytesFreedFormatted;
            FoldersDeleted = result.FoldersDeleted.ToString();

            ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] {result.Message}");
            _main.StatusMessage = result.Message;

            // Refrescar dashboard
            await _main.RefreshStorageAsync();
        }
        catch (OperationCanceledException)
        {
            ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] ⚠ Limpieza cancelada.");
            _main.StatusMessage = "Limpieza cancelada.";
        }
        catch (Exception ex)
        {
            ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] ✗ Error: {ex.Message}");
            _main.StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsExecuting = false;
        }
    }

    [RelayCommand]
    private void CancelExecution()
    {
        _cts?.Cancel();
    }

    /// <summary>Preview de limpieza (dry run)</summary>
    [RelayCommand]
    private async Task PreviewCleanupAsync()
    {
        if (_main.CurrentStatus == null)
        {
            _main.StatusMessage = "Primero escanee el almacenamiento.";
            return;
        }

        IsExecuting = true;
        ExecutionLog.Clear();

        try
        {
            ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] Calculando preview de limpieza...");

            var result = await App.CleanupService.PreviewCleanupAsync(_main.CurrentStatus, _main.Configuration);

            HasResult = true;
            ResultMessage = $"PREVIEW: {result.Message}";
            BytesFreed = result.BytesFreedFormatted;
            FoldersDeleted = result.FoldersDeleted.ToString();

            ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] Se eliminarían {result.FoldersDeleted} carpetas ({result.BytesFreedFormatted})");

            foreach (var detail in result.AssetDetails.GroupBy(d => d.AssetId))
            {
                ExecutionLog.Add($"  Asset {detail.Key}: {detail.Sum(d => d.FoldersDeleted)} carpetas");
            }
        }
        catch (Exception ex)
        {
            ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] Error: {ex.Message}");
        }
        finally
        {
            IsExecuting = false;
        }
    }

    /// <summary>Iniciar/detener RF-07 (limpieza programada)</summary>
    [RelayCommand]
    private async Task ToggleScheduledAsync()
    {
        if (IsScheduledRunning)
        {
            await App.ScheduledService.StopAsync();
            IsScheduledRunning = false;
            ScheduledNextRun = "N/A";
            _main.IsScheduledRunning = false;
            ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] RF-07 detenido.");
        }
        else
        {
            App.ScheduledService.OnCleanupExecuted += (_, r) =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] RF-07 ejecutó limpieza: {r.Message}");
                });
            };
            App.ScheduledService.OnCleanupSkipped += (_, msg) =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] RF-07 skip: {msg}");
                    if (App.ScheduledService.NextScheduledRun.HasValue)
                        ScheduledNextRun = App.ScheduledService.NextScheduledRun.Value.ToString("dd/MM/yyyy HH:mm");
                });
            };

            await App.ScheduledService.StartAsync(_main.Configuration);
            IsScheduledRunning = true;
            _main.IsScheduledRunning = true;

            if (App.ScheduledService.NextScheduledRun.HasValue)
                ScheduledNextRun = App.ScheduledService.NextScheduledRun.Value.ToString("dd/MM/yyyy HH:mm");

            ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] RF-07 iniciado. Próxima ejecución: {ScheduledNextRun}");
        }
    }

    /// <summary>Iniciar/detener RF-08 (monitoreo preventivo)</summary>
    [RelayCommand]
    private async Task TogglePreventiveAsync()
    {
        if (IsPreventiveRunning)
        {
            await App.PreventiveService.StopAsync();
            IsPreventiveRunning = false;
            _main.IsPreventiveRunning = false;
            ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] RF-08 detenido.");
        }
        else
        {
            try
            {
                if (string.IsNullOrEmpty(_main.Configuration.StoragePath))
                {
                    _main.StatusMessage = "Configure la ruta de almacenamiento primero.";
                    return;
                }

                App.PreventiveService.OnFileDetected += (_, args) =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        PreventiveFilesDetected = App.PreventiveService.FilesDetected.ToString();
                    });
                };
                App.PreventiveService.OnPreventiveCleanup += (_, r) =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        PreventiveCleanups = App.PreventiveService.PreventiveCleanups.ToString();
                        ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] RF-08 limpieza preventiva: {r.Message}");
                    });
                };

                await App.PreventiveService.StartAsync(_main.Configuration.StoragePath, _main.Configuration);
                IsPreventiveRunning = true;
                _main.IsPreventiveRunning = true;
                ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] RF-08 iniciado. Monitoreando: {_main.Configuration.StoragePath}");
            }
            catch (Exception ex)
            {
                ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] ✗ Error al iniciar RF-08: {ex.Message}");
                _main.StatusMessage = $"Error al iniciar RF-08: {ex.Message}";
            }
        }
    }
}
