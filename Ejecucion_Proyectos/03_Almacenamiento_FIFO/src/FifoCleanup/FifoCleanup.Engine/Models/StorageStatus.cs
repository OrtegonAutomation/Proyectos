namespace FifoCleanup.Engine.Models;

/// <summary>
/// Información del almacenamiento general del disco/partición.
/// </summary>
public class StorageStatus
{
    /// <summary>Letra o ruta de la unidad</summary>
    public string DrivePath { get; set; } = string.Empty;

    /// <summary>Espacio total en bytes</summary>
    public long TotalSpaceBytes { get; set; }

    /// <summary>Espacio usado en bytes</summary>
    public long UsedSpaceBytes { get; set; }

    /// <summary>Espacio libre en bytes</summary>
    public long FreeSpaceBytes { get; set; }

    /// <summary>Porcentaje de uso actual</summary>
    public double UsagePercent => TotalSpaceBytes > 0
        ? (double)UsedSpaceBytes / TotalSpaceBytes * 100.0
        : 0;

    /// <summary>Espacio total de las carpetas E/F monitoreadas en bytes</summary>
    public long MonitoredDataBytes { get; set; }

    /// <summary>Assets escaneados</summary>
    public List<AssetInfo> Assets { get; set; } = new();

    /// <summary>Fecha/hora del último escaneo</summary>
    public DateTime LastScanTime { get; set; }

    /// <summary>Duración del escaneo en ms</summary>
    public long ScanDurationMs { get; set; }

    /// <summary>Indicador de nivel: Green (<70%), Yellow (70-85%), Red (>85%)</summary>
    public StorageLevel Level => UsagePercent switch
    {
        < 70 => StorageLevel.Green,
        < 85 => StorageLevel.Yellow,
        _ => StorageLevel.Red
    };

    /// <summary>Promedio diario de crecimiento (últimos 7 días) en bytes</summary>
    public double AverageDailyGrowthBytes { get; set; }

    /// <summary>Días estimados hasta llenar el umbral configurado</summary>
    public double? EstimatedDaysToThreshold { get; set; }
}

public enum StorageLevel
{
    Green,
    Yellow,
    Red
}

/// <summary>
/// Resultado de una operación de limpieza FIFO.
/// </summary>
public class CleanupResult
{
    /// <summary>Tipo de limpieza: Scheduled (RF-07), Preventive (RF-08), Manual, Simulation</summary>
    public CleanupType Type { get; set; }

    /// <summary>Si la operación fue exitosa</summary>
    public bool Success { get; set; }

    /// <summary>Mensaje descriptivo del resultado</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Fecha/hora de inicio</summary>
    public DateTime StartTime { get; set; }

    /// <summary>Fecha/hora de fin</summary>
    public DateTime EndTime { get; set; }

    /// <summary>Duración en milisegundos</summary>
    public long DurationMs => (long)(EndTime - StartTime).TotalMilliseconds;

    /// <summary>Bytes liberados</summary>
    public long BytesFreed { get; set; }

    /// <summary>Carpetas eliminadas</summary>
    public int FoldersDeleted { get; set; }

    /// <summary>Archivos eliminados</summary>
    public int FilesDeleted { get; set; }

    /// <summary>Porcentaje de uso antes de limpieza</summary>
    public double UsagePercentBefore { get; set; }

    /// <summary>Porcentaje de uso después de limpieza</summary>
    public double UsagePercentAfter { get; set; }

    /// <summary>Assets afectados</summary>
    public List<AssetCleanupDetail> AssetDetails { get; set; } = new();

    /// <summary>Errores encontrados durante la limpieza</summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>Si la operación fue cancelada por el usuario</summary>
    public bool WasCancelled { get; set; }

    /// <summary>Bytes formateados liberados</summary>
    public string BytesFreedFormatted => FormatSize(BytesFreed);

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

/// <summary>
/// Detalle de limpieza por Asset individual.
/// </summary>
public class AssetCleanupDetail
{
    public string AssetId { get; set; } = string.Empty;
    public string VariableId { get; set; } = string.Empty;
    public long BytesFreed { get; set; }
    public int FoldersDeleted { get; set; }
    public DateTime? OldestDeletedDate { get; set; }
    public DateTime? NewestDeletedDate { get; set; }
    public List<string> DeletedPaths { get; set; } = new();
}

public enum CleanupType
{
    Manual,
    Scheduled,   // RF-07
    Preventive,  // RF-08
    Simulation
}
