using System.Diagnostics;
using FifoCleanup.Engine.Models;
using FifoCleanup.Engine.Services.Interfaces;

namespace FifoCleanup.Engine.Services;

/// <summary>
/// RF-04: Servicio de limpieza FIFO.
/// Elimina carpetas de día más antiguas, respetando orden cronológico y proporcionalidad.
/// </summary>
public class CleanupService : ICleanupService
{
    private readonly IBitacoraService _bitacora;

    public CleanupService(IBitacoraService bitacora)
    {
        _bitacora = bitacora;
    }

    public async Task<CleanupResult> ExecuteGeneralCleanupAsync(
        StorageStatus status,
        FifoConfiguration config,
        CancellationToken ct = default,
        IProgress<(string message, double percent)>? progress = null,
        bool dryRun = false)
    {
        var sw = Stopwatch.StartNew();
        var result = new CleanupResult
        {
            Type = dryRun ? CleanupType.Simulation : CleanupType.Scheduled,
            StartTime = DateTime.Now,
            UsagePercentBefore = status.UsagePercent
        };

        try
        {
            // Calcular cuánto espacio necesitamos liberar
            double targetUsage = config.ThresholdPercent - 5; // Bajar 5% por debajo del umbral
            long bytesToFree = status.UsedSpaceBytes - (long)(status.TotalSpaceBytes * targetUsage / 100.0);

            if (bytesToFree <= 0)
            {
                result.Success = true;
                result.Message = "No se requiere limpieza. El almacenamiento está dentro del rango seguro.";
                result.EndTime = DateTime.Now;
                return result;
            }

            // Aplicar cap de limpieza
            long maxBytesToFree = (long)(status.MonitoredDataBytes * config.CleanupCapPercent / 100.0);
            bytesToFree = Math.Min(bytesToFree, maxBytesToFree);

            progress?.Report(($"Bytes a liberar: {FormatSize(bytesToFree)}", 5));

            // Obtener todas las carpetas de día en orden FIFO
            var fifoQueue = GetFifoOrder(status.Assets);

            if (fifoQueue.Count == 0)
            {
                result.Success = false;
                result.Message = "No se encontraron carpetas de día para limpiar.";
                result.EndTime = DateTime.Now;
                return result;
            }

            long totalFreed = 0;
            int foldersDeleted = 0;
            int filesDeleted = 0;

            foreach (var dayFolder in fifoQueue)
            {
                ct.ThrowIfCancellationRequested();

                if (totalFreed >= bytesToFree)
                    break;

                double percentComplete = 5 + (double)foldersDeleted / fifoQueue.Count * 90;
                progress?.Report(($"Eliminando: {dayFolder.AssetId}/{dayFolder.VariableId}/{dayFolder.FolderType}/{dayFolder.Date:yyyy/MM/dd}",
                    percentComplete));

                if (!dryRun)
                {
                    var (deletedFiles, deletedSize) = await DeleteDayFolderAsync(dayFolder.FullPath);
                    filesDeleted += deletedFiles;
                    totalFreed += deletedSize;
                }
                else
                {
                    totalFreed += dayFolder.SizeBytes;
                }

                foldersDeleted++;

                result.AssetDetails.Add(new AssetCleanupDetail
                {
                    AssetId = dayFolder.AssetId,
                    VariableId = dayFolder.VariableId,
                    BytesFreed = dayFolder.SizeBytes,
                    FoldersDeleted = 1,
                    OldestDeletedDate = dayFolder.Date,
                    NewestDeletedDate = dayFolder.Date,
                    DeletedPaths = new List<string> { dayFolder.FullPath }
                });
            }

            result.BytesFreed = totalFreed;
            result.FoldersDeleted = foldersDeleted;
            result.FilesDeleted = filesDeleted;
            result.Success = true;
            result.Message = $"Limpieza completada. Se liberaron {FormatSize(totalFreed)} eliminando {foldersDeleted} carpetas.";

            // Recalcular uso
            result.UsagePercentAfter = status.TotalSpaceBytes > 0
                ? (double)(status.UsedSpaceBytes - totalFreed) / status.TotalSpaceBytes * 100.0
                : 0;

            if (!dryRun)
            {
                await _bitacora.LogAsync(new BitacoraEntry
                {
                    EventType = BitacoraEventType.CleanupScheduled,
                    Action = "LIMPIEZA_GENERAL_FIFO",
                    Details = result.Message,
                    BytesAffected = totalFreed,
                    Result = "OK"
                });
            }
        }
        catch (OperationCanceledException)
        {
            result.WasCancelled = true;
            result.Message = "Limpieza cancelada por el usuario.";
            result.Success = false;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = $"Error durante la limpieza: {ex.Message}";
            result.Errors.Add(ex.ToString());
        }

        result.EndTime = DateTime.Now;
        progress?.Report(("Proceso finalizado", 100));
        return result;
    }

    public async Task<CleanupResult> ExecuteLocalCleanupAsync(
        string assetId, string variableId,
        StorageStatus status, FifoConfiguration config,
        CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var result = new CleanupResult
        {
            Type = CleanupType.Preventive,
            StartTime = DateTime.Now,
            UsagePercentBefore = status.UsagePercent
        };

        try
        {
            // Encontrar el asset y variable específicos
            var asset = status.Assets.FirstOrDefault(a => a.AssetId == assetId);
            if (asset == null)
            {
                result.Success = false;
                result.Message = $"Asset '{assetId}' no encontrado.";
                result.EndTime = DateTime.Now;
                return result;
            }

            var variable = asset.Variables.FirstOrDefault(v => v.VariableId == variableId);
            if (variable == null)
            {
                result.Success = false;
                result.Message = $"Variable '{variableId}' no encontrada en Asset '{assetId}'.";
                result.EndTime = DateTime.Now;
                return result;
            }

            // Obtener carpetas de día del asset/variable en orden FIFO
            var dayFolders = variable.DayFolders
                .OrderBy(d => d.Date)
                .Take(config.MaxDaysToDeletePerAsset)
                .ToList();

            if (dayFolders.Count == 0)
            {
                result.Success = true;
                result.Message = "No hay carpetas de día para limpiar en este Asset/Variable.";
                result.EndTime = DateTime.Now;
                return result;
            }

            long totalFreed = 0;
            int foldersDeleted = 0;
            int filesDeleted = 0;

            foreach (var dayFolder in dayFolders)
            {
                ct.ThrowIfCancellationRequested();

                var (deleted, size) = await DeleteDayFolderAsync(dayFolder.FullPath);
                filesDeleted += deleted;
                totalFreed += size;
                foldersDeleted++;

                result.AssetDetails.Add(new AssetCleanupDetail
                {
                    AssetId = assetId,
                    VariableId = variableId,
                    BytesFreed = size,
                    FoldersDeleted = 1,
                    OldestDeletedDate = dayFolder.Date,
                    NewestDeletedDate = dayFolder.Date,
                    DeletedPaths = new List<string> { dayFolder.FullPath }
                });
            }

            result.BytesFreed = totalFreed;
            result.FoldersDeleted = foldersDeleted;
            result.FilesDeleted = filesDeleted;
            result.Success = true;
            result.Message = $"Limpieza local en {assetId}/{variableId}: liberados {FormatSize(totalFreed)}.";

            result.UsagePercentAfter = status.TotalSpaceBytes > 0
                ? (double)(status.UsedSpaceBytes - totalFreed) / status.TotalSpaceBytes * 100.0
                : 0;

            await _bitacora.LogAsync(new BitacoraEntry
            {
                EventType = BitacoraEventType.CleanupPreventive,
                Action = "LIMPIEZA_LOCAL_FIFO",
                AssetId = assetId,
                VariableId = variableId,
                Details = result.Message,
                BytesAffected = totalFreed,
                Result = "OK"
            });
        }
        catch (OperationCanceledException)
        {
            result.WasCancelled = true;
            result.Success = false;
            result.Message = "Limpieza cancelada.";
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = $"Error: {ex.Message}";
            result.Errors.Add(ex.ToString());
        }

        result.EndTime = DateTime.Now;
        return result;
    }

    public async Task<CleanupResult> PreviewCleanupAsync(
        StorageStatus status, FifoConfiguration config, CancellationToken ct = default)
    {
        return await ExecuteGeneralCleanupAsync(status, config, ct, dryRun: true);
    }

    public List<DayFolderInfo> GetFifoOrder(List<AssetInfo> assets)
    {
        return assets
            .SelectMany(a => a.Variables.SelectMany(v => v.DayFolders))
            .OrderBy(d => d.Date)
            .ThenBy(d => d.AssetId)
            .ThenBy(d => d.VariableId)
            .ToList();
    }

    /// <summary>
    /// Elimina una carpeta de día y retorna (archivos eliminados, bytes liberados).
    /// </summary>
    private static async Task<(int filesDeleted, long bytesFreed)> DeleteDayFolderAsync(string path)
    {
        if (!Directory.Exists(path))
            return (0, 0);

        int files = 0;
        long bytes = 0;

        await Task.Run(() =>
        {
            var dirInfo = new DirectoryInfo(path);
            foreach (var file in dirInfo.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                try
                {
                    bytes += file.Length;
                    file.Delete();
                    files++;
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }

            // Eliminar carpetas vacías de abajo hacia arriba
            foreach (var dir in dirInfo.EnumerateDirectories("*", SearchOption.AllDirectories)
                .OrderByDescending(d => d.FullName.Length))
            {
                try { dir.Delete(false); }
                catch { }
            }

            try { dirInfo.Delete(false); }
            catch { }
        });

        return (files, bytes);
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
