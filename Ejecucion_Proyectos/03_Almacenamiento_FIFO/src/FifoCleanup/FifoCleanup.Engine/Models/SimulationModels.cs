namespace FifoCleanup.Engine.Models;

/// <summary>
/// Parámetros para la simulación de datos sintéticos y limpieza.
/// </summary>
public class SimulationParams
{
    /// <summary>Ruta temporal donde se generarán los datos sintéticos</summary>
    public string SimulationPath { get; set; } = string.Empty;

    /// <summary>Número de Assets a simular</summary>
    public int NumberOfAssets { get; set; } = 5;

    /// <summary>Número de variables por Asset</summary>
    public int VariablesPerAsset { get; set; } = 3;

    /// <summary>Días de datos históricos a generar</summary>
    public int DaysOfHistory { get; set; } = 30;

    /// <summary>Tamaño promedio por carpeta de día en MB</summary>
    public double AvgDayFolderSizeMB { get; set; } = 50;

    /// <summary>Variación de tamaño (+/- porcentaje)</summary>
    public double SizeVariationPercent { get; set; } = 30;

    /// <summary>Si se generan datos en carpeta E</summary>
    public bool GenerateEData { get; set; } = true;

    /// <summary>Si se generan datos en carpeta F</summary>
    public bool GenerateFData { get; set; } = true;

    /// <summary>Tamaño total del disco simulado en GB</summary>
    public double SimulatedDiskSizeGB { get; set; } = 100;

    /// <summary>Porcentaje de uso inicial del disco simulado</summary>
    public double InitialUsagePercent { get; set; } = 80;

    /// <summary>Umbral para la simulación (%)</summary>
    public double ThresholdPercent { get; set; } = 85;

    /// <summary>Cap de limpieza para la simulación (%)</summary>
    public double CleanupCapPercent { get; set; } = 20;
}

/// <summary>
/// Parámetros para simulación continua de ingreso de datos.
/// Permite simular el crecimiento realista del almacenamiento para probar RF-07 y RF-08.
/// </summary>
public class ContinuousSimulationParams
{
    /// <summary>Ruta base donde se generarán los datos (debe existir)</summary>
    public string SimulationPath { get; set; } = string.Empty;

    /// <summary>Cantidad de MB a generar por intervalo</summary>
    public double DataRateMBPerInterval { get; set; } = 100;

    /// <summary>Intervalo de generación en segundos</summary>
    public int IntervalSeconds { get; set; } = 60;

    /// <summary>Assets a usar para generación continua (si vacío, usa todos los existentes)</summary>
    public List<string> TargetAssets { get; set; } = new();

    /// <summary>Si se generan datos en carpeta E</summary>
    public bool GenerateEData { get; set; } = true;

    /// <summary>Si se generan datos en carpeta F</summary>
    public bool GenerateFData { get; set; } = true;

    /// <summary>Variación aleatoria del tamaño (+/- porcentaje)</summary>
    public double SizeVariationPercent { get; set; } = 20;
}

/// <summary>
/// Resultado de una simulación completa.
/// </summary>
public class SimulationResult
{
    /// <summary>Parámetros usados</summary>
    public SimulationParams Parameters { get; set; } = new();

    /// <summary>Estado del storage antes de simulación</summary>
    public StorageStatus StatusBefore { get; set; } = new();

    /// <summary>Estado del storage después de simulación</summary>
    public StorageStatus StatusAfter { get; set; } = new();

    /// <summary>Resultado de la limpieza simulada</summary>
    public CleanupResult CleanupResult { get; set; } = new();

    /// <summary>Si se generaron datos sintéticos exitosamente</summary>
    public bool DataGenerationSuccess { get; set; }

    /// <summary>Mensajes de log de la simulación</summary>
    public List<string> LogMessages { get; set; } = new();

    /// <summary>Duración total de la simulación en ms</summary>
    public long TotalDurationMs { get; set; }
}
