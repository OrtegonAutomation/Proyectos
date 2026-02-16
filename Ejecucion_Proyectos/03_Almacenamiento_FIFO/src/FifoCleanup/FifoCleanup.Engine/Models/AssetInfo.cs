namespace FifoCleanup.Engine.Models;

/// <summary>
/// Información de un Asset (equipo) monitoreado.
/// Estructura esperada: Asset/[ID]/E/[YYYY]/[MM]/[DD]
///                       Asset/[ID]/F/[YYYY]/[MM]/[DD]
///                       Asset/[ID]/[0-N]/[YYYY] (tendencias, no se tocan)
/// </summary>
public class AssetInfo
{
    /// <summary>Identificador del asset (nombre de carpeta)</summary>
    public string AssetId { get; set; } = string.Empty;

    /// <summary>Ruta completa de la carpeta del asset</summary>
    public string FullPath { get; set; } = string.Empty;

    /// <summary>Variables encontradas dentro del asset (subcarpetas)</summary>
    public List<VariableInfo> Variables { get; set; } = new();

    /// <summary>Tamaño total del asset en bytes (solo carpetas E y F)</summary>
    public long TotalSizeBytes { get; set; }

    /// <summary>Número total de carpetas de día encontradas</summary>
    public int TotalDayFolders { get; set; }

    /// <summary>Fecha del dato más antiguo</summary>
    public DateTime? OldestDate { get; set; }

    /// <summary>Fecha del dato más reciente</summary>
    public DateTime? NewestDate { get; set; }

    /// <summary>Proporción de llenado respecto al total del storage (%)</summary>
    public double FillProportion { get; set; }

    /// <summary>Tasa de crecimiento diario promedio en bytes (últimos 7 días)</summary>
    public double AverageDailyGrowthBytes { get; set; }

    /// <summary>Tamaño formateado para UI</summary>
    public string TotalSizeFormatted => FormatSize(TotalSizeBytes);

    /// <summary>Número de variables con datos en E</summary>
    public int VariablesWithE => Variables.Count(v => v.HasEData);

    /// <summary>Número de variables con datos en F</summary>
    public int VariablesWithF => Variables.Count(v => v.HasFData);

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
/// Información de una variable dentro de un Asset.
/// Las variables se identifican con índices numéricos (0, 1, 2, ..., N).
/// </summary>
public class VariableInfo
{
    /// <summary>Índice/ID de la variable (nombre de subcarpeta)</summary>
    public string VariableId { get; set; } = string.Empty;

    /// <summary>Ruta completa</summary>
    public string FullPath { get; set; } = string.Empty;

    /// <summary>Si tiene subcarpeta E con datos</summary>
    public bool HasEData { get; set; }

    /// <summary>Si tiene subcarpeta F con datos</summary>
    public bool HasFData { get; set; }

    /// <summary>Tamaño de datos E en bytes</summary>
    public long ESizeBytes { get; set; }

    /// <summary>Tamaño de datos F en bytes</summary>
    public long FSizeBytes { get; set; }

    /// <summary>Total de tamaño E + F</summary>
    public long TotalCriticalSizeBytes => ESizeBytes + FSizeBytes;

    /// <summary>Carpetas de día encontradas en E y F</summary>
    public List<DayFolderInfo> DayFolders { get; set; } = new();
}

/// <summary>
/// Información de una carpeta de día individual.
/// Ruta: Asset/[ID]/[E|F]/[YYYY]/[MM]/[DD]
/// </summary>
public class DayFolderInfo
{
    /// <summary>Ruta completa de la carpeta de día</summary>
    public string FullPath { get; set; } = string.Empty;

    /// <summary>Tipo de carpeta: E o F</summary>
    public string FolderType { get; set; } = string.Empty;

    /// <summary>Fecha que representa esta carpeta</summary>
    public DateTime Date { get; set; }

    /// <summary>Tamaño en bytes</summary>
    public long SizeBytes { get; set; }

    /// <summary>Asset al que pertenece</summary>
    public string AssetId { get; set; } = string.Empty;

    /// <summary>Variable a la que pertenece</summary>
    public string VariableId { get; set; } = string.Empty;
}
