using FifoCleanup.Engine.Models;
using FifoCleanup.Engine.Services.Interfaces;

namespace FifoCleanup.Engine.Services;

/// <summary>
/// RF-05: Bitácora de auditoría.
/// Registra eventos en archivos CSV con rotación automática.
/// Los archivos son append-only (inmutables una vez escritos).
/// </summary>
public class BitacoraService : IBitacoraService
{
    private string _basePath = "bitacora";
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    public async Task InitializeAsync(string bitacoraPath)
    {
        _basePath = bitacoraPath;
        if (!Directory.Exists(_basePath))
            Directory.CreateDirectory(_basePath);

        // Crear archivo del día si no existe
        var filePath = GetCurrentFilePath();
        if (!File.Exists(filePath))
        {
            await File.WriteAllTextAsync(filePath, BitacoraEntry.CsvHeader + Environment.NewLine);
        }
    }

    public async Task LogAsync(BitacoraEntry entry)
    {
        await _writeLock.WaitAsync();
        try
        {
            var filePath = GetCurrentFilePath();

            // Verificar rotación por tamaño
            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > 100 * 1024 * 1024) // 100 MB
                {
                    filePath = GetRotatedFilePath();
                    await File.WriteAllTextAsync(filePath, BitacoraEntry.CsvHeader + Environment.NewLine);
                }
            }
            else
            {
                await File.WriteAllTextAsync(filePath, BitacoraEntry.CsvHeader + Environment.NewLine);
            }

            await File.AppendAllTextAsync(filePath, entry.ToCsvLine() + Environment.NewLine);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    public async Task<List<BitacoraEntry>> GetEntriesAsync(
        DateTime? from = null, DateTime? to = null,
        BitacoraEventType? type = null, string? assetId = null, int? limit = null)
    {
        var entries = new List<BitacoraEntry>();

        if (!Directory.Exists(_basePath))
            return entries;

        var csvFiles = Directory.GetFiles(_basePath, "bitacora_*.csv")
            .OrderByDescending(f => f);

        foreach (var file in csvFiles)
        {
            var lines = await File.ReadAllLinesAsync(file);

            foreach (var line in lines.Skip(1)) // Skip header
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var entry = BitacoraEntry.FromCsvLine(line);
                if (entry == null) continue;

                // Filtros
                if (from.HasValue && entry.Timestamp < from.Value) continue;
                if (to.HasValue && entry.Timestamp > to.Value) continue;
                if (type.HasValue && entry.EventType != type.Value) continue;
                if (!string.IsNullOrEmpty(assetId) && entry.AssetId != assetId) continue;

                entries.Add(entry);

                if (limit.HasValue && entries.Count >= limit.Value)
                    return entries;
            }
        }

        return entries.OrderByDescending(e => e.Timestamp).ToList();
    }

    public async Task<string> ExportToCsvAsync(string outputPath, DateTime? from = null, DateTime? to = null)
    {
        var entries = await GetEntriesAsync(from, to);

        var lines = new List<string> { BitacoraEntry.CsvHeader };
        lines.AddRange(entries.Select(e => e.ToCsvLine()));

        await File.WriteAllLinesAsync(outputPath, lines);
        return outputPath;
    }

    public async Task MaintenanceAsync(int retentionDays, int maxFileSizeMB)
    {
        if (!Directory.Exists(_basePath))
            return;

        var cutoffDate = DateTime.Now.AddDays(-retentionDays);
        var files = Directory.GetFiles(_basePath, "bitacora_*.csv");

        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);
            if (fileInfo.CreationTime < cutoffDate)
            {
                try { File.Delete(file); }
                catch { }
            }
        }

        await Task.CompletedTask;
    }

    private string GetCurrentFilePath()
    {
        return Path.Combine(_basePath, $"bitacora_{DateTime.Now:yyyyMMdd}.csv");
    }

    private string GetRotatedFilePath()
    {
        return Path.Combine(_basePath, $"bitacora_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
    }
}
