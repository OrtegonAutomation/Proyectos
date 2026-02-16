namespace FifoCleanup.Engine.Models;

/// <summary>
/// Configuración principal del sistema FIFO.
/// Se persiste como JSON en la ruta seleccionada por el usuario.
/// </summary>
public class FifoConfiguration
{
    /// <summary>Ruta raíz donde se encuentran los Assets (ej: D:\MonitoringData)</summary>
    public string StoragePath { get; set; } = string.Empty;

    /// <summary>Umbral de almacenamiento (%) a partir del cual se activa limpieza. Rango: 50-95</summary>
    public double ThresholdPercent { get; set; } = 85.0;

    /// <summary>Porcentaje máximo a eliminar por ejecución. Rango: 5-50</summary>
    public double CleanupCapPercent { get; set; } = 20.0;

    /// <summary>Frecuencia de ejecución RF-07 en horas. Rango: 1-24</summary>
    public int ScheduledFrequencyHours { get; set; } = 24;

    /// <summary>Hora preferida para ejecución RF-07 (formato 24h). Default: 2 AM</summary>
    public int ScheduledHour { get; set; } = 2;

    /// <summary>Umbral de proyección RF-08 en días. Si se proyecta llenado en menos días, limpia. Rango: 1-10</summary>
    public int PreventiveThresholdDays { get; set; } = 3;

    /// <summary>Habilitar RF-07 (limpieza programada)</summary>
    public bool EnableScheduledCleanup { get; set; } = true;

    /// <summary>Habilitar RF-08 (limpieza preventiva por File System Watcher)</summary>
    public bool EnablePreventiveCleanup { get; set; } = true;

    /// <summary>Máximo de Assets a procesar simultáneamente en limpieza</summary>
    public int MaxConcurrentAssets { get; set; } = 5;

    /// <summary>Días máximos a eliminar por Asset en limpieza local (RF-08)</summary>
    public int MaxDaysToDeletePerAsset { get; set; } = 10;

    /// <summary>Ruta del archivo de configuración</summary>
    public string ConfigFilePath { get; set; } = "fifo_config.json";

    /// <summary>Ruta de la bitácora CSV</summary>
    public string BitacoraPath { get; set; } = "bitacora";

    /// <summary>Retención de bitácora en días</summary>
    public int BitacoraRetentionDays { get; set; } = 90;

    /// <summary>Tamaño máximo de archivo de bitácora en MB antes de rotación</summary>
    public int BitacoraMaxSizeMB { get; set; } = 100;
}
