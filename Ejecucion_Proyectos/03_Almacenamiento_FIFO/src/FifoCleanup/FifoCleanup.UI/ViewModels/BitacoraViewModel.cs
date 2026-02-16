using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FifoCleanup.Engine.Models;

namespace FifoCleanup.UI.ViewModels;

/// <summary>
/// ViewModel para la pestaña Bitácora.
/// Muestra el historial de operaciones con filtros y exportación.
/// </summary>
public partial class BitacoraViewModel : ObservableObject
{
    private readonly MainViewModel _main;

    [ObservableProperty] private DateTime _filterFrom = DateTime.Now.AddDays(-7);
    [ObservableProperty] private DateTime _filterTo = DateTime.Now;
    [ObservableProperty] private string? _filterAssetId;
    [ObservableProperty] private BitacoraEventType? _filterType;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _entryCount = "0 registros";

    public ObservableCollection<BitacoraEntry> Entries { get; } = new();

    // Tipos disponibles para filtro (para ComboBox)
    public List<string> EventTypes { get; } = new()
    {
        "Todos",
        "Inventario",
        "Limpieza Manual",
        "Limpieza Programada",
        "Limpieza Preventiva",
        "Simulación",
        "Configuración",
        "Alarma",
        "Error",
        "Inicio del Sistema",
        "Archivo Detectado"
    };

    [ObservableProperty] private string _selectedEventType = "Todos";

    public BitacoraViewModel(MainViewModel main)
    {
        _main = main;
    }

    [RelayCommand]
    private async Task LoadEntriesAsync()
    {
        IsLoading = true;
        try
        {
            BitacoraEventType? typeFilter = SelectedEventType switch
            {
                "Inventario" => BitacoraEventType.Inventory,
                "Limpieza Manual" => BitacoraEventType.CleanupManual,
                "Limpieza Programada" => BitacoraEventType.CleanupScheduled,
                "Limpieza Preventiva" => BitacoraEventType.CleanupPreventive,
                "Simulación" => BitacoraEventType.Simulation,
                "Configuración" => BitacoraEventType.Configuration,
                "Alarma" => BitacoraEventType.Alarm,
                "Error" => BitacoraEventType.Error,
                "Inicio del Sistema" => BitacoraEventType.SystemStart,
                "Archivo Detectado" => BitacoraEventType.FileDetected,
                _ => null
            };

            var entries = await App.BitacoraService.GetEntriesAsync(
                FilterFrom, FilterTo, typeFilter, FilterAssetId, 1000);

            Entries.Clear();
            foreach (var entry in entries)
                Entries.Add(entry);

            EntryCount = $"{entries.Count} registros";
        }
        catch (Exception ex)
        {
            _main.StatusMessage = $"Error cargando bitácora: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        try
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                DefaultExt = ".csv",
                FileName = $"bitacora_export_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (dialog.ShowDialog() == true)
            {
                await App.BitacoraService.ExportToCsvAsync(dialog.FileName, FilterFrom, FilterTo);
                _main.StatusMessage = $"Bitácora exportada a {dialog.FileName}";
            }
        }
        catch (Exception ex)
        {
            _main.StatusMessage = $"Error exportando: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task MaintenanceAsync()
    {
        try
        {
            await App.BitacoraService.MaintenanceAsync(
                _main.Configuration.BitacoraRetentionDays,
                _main.Configuration.BitacoraMaxSizeMB);
            _main.StatusMessage = "Mantenimiento de bitácora completado.";
            await LoadEntriesAsync();
        }
        catch (Exception ex)
        {
            _main.StatusMessage = $"Error en mantenimiento: {ex.Message}";
        }
    }
}
