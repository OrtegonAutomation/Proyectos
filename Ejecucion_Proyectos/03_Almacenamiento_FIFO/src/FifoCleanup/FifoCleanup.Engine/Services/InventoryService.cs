using System.Diagnostics;
using System.Globalization;
using FifoCleanup.Engine.Models;
using FifoCleanup.Engine.Services.Interfaces;

namespace FifoCleanup.Engine.Services;

/// <summary>
/// RF-01: Inventario y caracterización del almacenamiento.
/// Escanea la estructura Asset/[ID]/[E|F]/[YYYY]/[MM]/[DD].
/// </summary>
public class InventoryService : IInventoryService
{
    /// <summary>Tipos de carpeta que contienen datos críticos (crecen rápidamente)</summary>
    private static readonly string[] CriticalFolderTypes = { "E", "F" };

    public StorageStatus GetDriveInfo(string path)
    {
        var root = Path.GetPathRoot(path) ?? path;
        var driveInfo = new DriveInfo(root);

        return new StorageStatus
        {
            DrivePath = root,
            TotalSpaceBytes = driveInfo.TotalSize,
            UsedSpaceBytes = driveInfo.TotalSize - driveInfo.AvailableFreeSpace,
            FreeSpaceBytes = driveInfo.AvailableFreeSpace,
            LastScanTime = DateTime.Now
        };
    }

    public async Task<StorageStatus> ScanAsync(string storagePath, CancellationToken ct = default,
        IProgress<(string message, double percent)>? progress = null)
    {
        var sw = Stopwatch.StartNew();
        var status = GetDriveInfo(storagePath);

        if (!Directory.Exists(storagePath))
        {
            status.ScanDurationMs = sw.ElapsedMilliseconds;
            return status;
        }

        // Obtener carpetas de Assets (primer nivel bajo storagePath)
        var assetDirs = Directory.GetDirectories(storagePath);
        var assets = new List<AssetInfo>();
        int processed = 0;

        foreach (var assetDir in assetDirs)
        {
            ct.ThrowIfCancellationRequested();

            var assetName = Path.GetFileName(assetDir);
            progress?.Report(($"Escaneando Asset: {assetName}", (double)processed / assetDirs.Length * 100));

            var asset = await ScanAssetAsync(assetDir, ct);
            if (asset != null)
                assets.Add(asset);

            processed++;
        }

        // Calcular proporciones de llenado
        long totalMonitored = assets.Sum(a => a.TotalSizeBytes);
        foreach (var asset in assets)
        {
            asset.FillProportion = totalMonitored > 0
                ? (double)asset.TotalSizeBytes / totalMonitored * 100.0
                : 0;
        }

        status.Assets = assets;
        status.MonitoredDataBytes = totalMonitored;
        status.LastScanTime = DateTime.Now;
        status.ScanDurationMs = sw.ElapsedMilliseconds;

        // Calcular crecimiento diario promedio global
        var allDayFolders = assets.SelectMany(a => a.Variables.SelectMany(v => v.DayFolders)).ToList();
        status.AverageDailyGrowthBytes = CalculateAverageDailyGrowth(allDayFolders);

        // Estimar días hasta el umbral
        if (status.AverageDailyGrowthBytes > 0)
        {
            double bytesUntilThreshold = status.TotalSpaceBytes * 0.85 - status.UsedSpaceBytes;
            if (bytesUntilThreshold > 0)
                status.EstimatedDaysToThreshold = bytesUntilThreshold / status.AverageDailyGrowthBytes;
            else
                status.EstimatedDaysToThreshold = 0;
        }

        progress?.Report(("Escaneo completado", 100));
        sw.Stop();
        status.ScanDurationMs = sw.ElapsedMilliseconds;

        return status;
    }

    public async Task<AssetInfo?> ScanAssetAsync(string assetPath, CancellationToken ct = default)
    {
        if (!Directory.Exists(assetPath))
            return null;

        var asset = new AssetInfo
        {
            AssetId = Path.GetFileName(assetPath),
            FullPath = assetPath
        };

        // Buscar subcarpetas (variables) dentro del Asset
        var variableDirs = Directory.GetDirectories(assetPath);

        foreach (var varDir in variableDirs)
        {
            ct.ThrowIfCancellationRequested();
            var varName = Path.GetFileName(varDir);

            var variable = new VariableInfo
            {
                VariableId = varName,
                FullPath = varDir
            };

            // Buscar carpetas E y F dentro de la variable
            foreach (var folderType in CriticalFolderTypes)
            {
                var criticalPath = Path.Combine(varDir, folderType);
                if (!Directory.Exists(criticalPath))
                    continue;

                if (folderType == "E") variable.HasEData = true;
                if (folderType == "F") variable.HasFData = true;

                // Escanear estructura YYYY/MM/DD
                var dayFolders = await ScanDayFoldersAsync(criticalPath, folderType, asset.AssetId, varName, ct);
                variable.DayFolders.AddRange(dayFolders);

                long folderSize = dayFolders.Sum(d => d.SizeBytes);
                if (folderType == "E") variable.ESizeBytes = folderSize;
                if (folderType == "F") variable.FSizeBytes = folderSize;
            }

            if (variable.HasEData || variable.HasFData)
                asset.Variables.Add(variable);
        }

        // Calcular métricas del asset
        asset.TotalSizeBytes = asset.Variables.Sum(v => v.TotalCriticalSizeBytes);
        asset.TotalDayFolders = asset.Variables.Sum(v => v.DayFolders.Count);

        var allDays = asset.Variables.SelectMany(v => v.DayFolders).ToList();
        if (allDays.Count > 0)
        {
            asset.OldestDate = allDays.Min(d => d.Date);
            asset.NewestDate = allDays.Max(d => d.Date);
            asset.AverageDailyGrowthBytes = CalculateAverageDailyGrowth(allDays);
        }

        return asset;
    }

    /// <summary>
    /// Escanea la estructura YYYY/MM/DD dentro de una carpeta E o F.
    /// </summary>
    private async Task<List<DayFolderInfo>> ScanDayFoldersAsync(
        string criticalPath, string folderType, string assetId, string variableId,
        CancellationToken ct)
    {
        var dayFolders = new List<DayFolderInfo>();

        // Estructura: criticalPath/YYYY/MM/DD
        foreach (var yearDir in Directory.GetDirectories(criticalPath))
        {
            ct.ThrowIfCancellationRequested();
            var yearName = Path.GetFileName(yearDir);
            if (!int.TryParse(yearName, out int year) || year < 2000 || year > 2100)
                continue;

            foreach (var monthDir in Directory.GetDirectories(yearDir))
            {
                ct.ThrowIfCancellationRequested();
                var monthName = Path.GetFileName(monthDir);
                if (!int.TryParse(monthName, out int month) || month < 1 || month > 12)
                    continue;

                foreach (var dayDir in Directory.GetDirectories(monthDir))
                {
                    ct.ThrowIfCancellationRequested();
                    var dayName = Path.GetFileName(dayDir);
                    if (!int.TryParse(dayName, out int day) || day < 1 || day > 31)
                        continue;

                    try
                    {
                        var date = new DateTime(year, month, day);
                        long size = await Task.Run(() => CalculateDirectorySize(dayDir), ct);

                        dayFolders.Add(new DayFolderInfo
                        {
                            FullPath = dayDir,
                            FolderType = folderType,
                            Date = date,
                            SizeBytes = size,
                            AssetId = assetId,
                            VariableId = variableId
                        });
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // Fecha inválida (ej: 30 de febrero), ignorar
                    }
                }
            }
        }

        return dayFolders;
    }

    public double CalculateAverageDailyGrowth(List<DayFolderInfo> dayFolders, int lookbackDays = 7)
    {
        if (dayFolders.Count == 0) return 0;

        var cutoff = DateTime.Now.AddDays(-lookbackDays);
        var recentFolders = dayFolders.Where(d => d.Date >= cutoff).ToList();

        if (recentFolders.Count == 0) return 0;

        // Agrupar por día y sumar tamaños
        var dailySizes = recentFolders
            .GroupBy(d => d.Date.Date)
            .Select(g => new { Date = g.Key, TotalSize = g.Sum(d => d.SizeBytes) })
            .OrderBy(x => x.Date)
            .ToList();

        if (dailySizes.Count <= 1) return dailySizes.Sum(d => d.TotalSize);

        return dailySizes.Average(d => d.TotalSize);
    }

    /// <summary>
    /// Calcula el tamaño total de un directorio recursivamente.
    /// Optimizado para rendimiento en terminales low-power.
    /// </summary>
    private static long CalculateDirectorySize(string path)
    {
        long size = 0;
        try
        {
            var dirInfo = new DirectoryInfo(path);
            foreach (var file in dirInfo.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                try { size += file.Length; }
                catch (UnauthorizedAccessException) { }
                catch (IOException) { }
            }
        }
        catch (UnauthorizedAccessException) { }
        catch (IOException) { }
        return size;
    }
}
