using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FifoCleanup.Engine.Models;

namespace FifoCleanup.UI.ViewModels;

/// <summary>
/// ViewModel para la pestaña Dashboard.
/// Muestra estado general del almacenamiento, métricas clave y tabla de Assets.
/// </summary>
public partial class DashboardViewModel : ObservableObject
{
    private readonly MainViewModel _main;

    [ObservableProperty] private double _usagePercent;
    [ObservableProperty] private string _usageText = "-- %";
    [ObservableProperty] private string _totalSpace = "--";
    [ObservableProperty] private string _usedSpace = "--";
    [ObservableProperty] private string _freeSpace = "--";
    [ObservableProperty] private string _monitoredData = "--";
    [ObservableProperty] private string _assetsCount = "0";
    [ObservableProperty] private string _dayFoldersCount = "0";
    [ObservableProperty] private string _estimatedDays = "N/A";
    [ObservableProperty] private string _dailyGrowth = "--";
    [ObservableProperty] private string _lastScan = "Nunca";
    [ObservableProperty] private StorageLevel _storageLevel = StorageLevel.Green;
    [ObservableProperty] private string _scheduledStatus = "Detenido";
    [ObservableProperty] private string _preventiveStatus = "Detenido";

    public ObservableCollection<AssetInfo> Assets { get; } = new();

    public DashboardViewModel(MainViewModel main)
    {
        _main = main;
    }

    public void UpdateFromStatus(StorageStatus status)
    {
        UsagePercent = status.UsagePercent;
        UsageText = $"{status.UsagePercent:F1}%";
        TotalSpace = FormatSize(status.TotalSpaceBytes);
        UsedSpace = FormatSize(status.UsedSpaceBytes);
        FreeSpace = FormatSize(status.FreeSpaceBytes);
        MonitoredData = FormatSize(status.MonitoredDataBytes);
        AssetsCount = status.Assets.Count.ToString();
        DayFoldersCount = status.Assets.Sum(a => a.TotalDayFolders).ToString();
        LastScan = status.LastScanTime.ToString("dd/MM/yyyy HH:mm:ss");
        StorageLevel = status.Level;
        DailyGrowth = FormatSize((long)status.AverageDailyGrowthBytes) + "/día";

        EstimatedDays = status.EstimatedDaysToThreshold.HasValue
            ? $"{status.EstimatedDaysToThreshold.Value:F0} días"
            : "N/A";

        ScheduledStatus = App.ScheduledService.IsRunning ? "Activo ✓" : "Detenido";
        PreventiveStatus = App.PreventiveService.IsRunning ? "Activo ✓" : "Detenido";

        Assets.Clear();
        foreach (var asset in status.Assets.OrderByDescending(a => a.FillProportion))
            Assets.Add(asset);
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await _main.RefreshStorageAsync();
    }

    private static string FormatSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }
}
