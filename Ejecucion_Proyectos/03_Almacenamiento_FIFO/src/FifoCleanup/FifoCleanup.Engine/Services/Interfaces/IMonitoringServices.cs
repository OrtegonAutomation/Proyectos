using FifoCleanup.Engine.Models;

namespace FifoCleanup.Engine.Services.Interfaces;

/// <summary>
/// RF-03: Servicio de simulación.
/// Genera datos sintéticos y ejecuta limpieza FIFO simulada.
/// </summary>
public interface ISimulationService
{
    /// <summary>Generar estructura de datos sintéticos para pruebas</summary>
    Task<bool> GenerateSyntheticDataAsync(SimulationParams parameters,
        CancellationToken ct = default,
        IProgress<(string message, double percent)>? progress = null);

    /// <summary>Ejecutar simulación completa (generar datos + ejecutar limpieza)</summary>
    Task<SimulationResult> RunSimulationAsync(SimulationParams parameters,
        CancellationToken ct = default,
        IProgress<(string message, double percent)>? progress = null);

    /// <summary>Limpiar datos sintéticos generados</summary>
    Task CleanupSyntheticDataAsync(string simulationPath);

    /// <summary>Iniciar simulación continua de ingreso de datos</summary>
    Task StartContinuousSimulationAsync(ContinuousSimulationParams parameters, CancellationToken ct = default);

    /// <summary>Detener simulación continua</summary>
    Task StopContinuousSimulationAsync();

    /// <summary>Si la simulación continua está activa</summary>
    bool IsContinuousSimulationRunning { get; }

    /// <summary>Total de MB generados en simulación continua</summary>
    long ContinuousDataGeneratedMB { get; }

    /// <summary>Evento cuando se generan nuevos datos en simulación continua</summary>
    event EventHandler<(string assetId, string variableId, long byteGenerated)>? OnContinuousDataGenerated;
}

/// <summary>
/// RF-07: Servicio de ejecución programada.
/// Evalúa periódicamente si se requiere limpieza basado en proyección histórica.
/// </summary>
public interface IScheduledCleanupService
{
    /// <summary>Iniciar el servicio de monitoreo programado</summary>
    Task StartAsync(FifoConfiguration config, CancellationToken ct = default);

    /// <summary>Detener el servicio</summary>
    Task StopAsync();

    /// <summary>Si el servicio está corriendo</summary>
    bool IsRunning { get; }

    /// <summary>Próxima ejecución programada</summary>
    DateTime? NextScheduledRun { get; }

    /// <summary>Evento cuando se ejecuta una evaluación</summary>
    event EventHandler<CleanupResult>? OnCleanupExecuted;

    /// <summary>Evento cuando se decide no limpiar (proyección segura)</summary>
    event EventHandler<string>? OnCleanupSkipped;
}

/// <summary>
/// RF-08: Servicio de monitoreo preventivo en tiempo real.
/// Usa FileSystemWatcher para detectar adiciones y evaluar limpieza local.
/// </summary>
public interface IPreventiveMonitorService
{
    /// <summary>Iniciar monitoreo de la ruta de almacenamiento</summary>
    Task StartAsync(string storagePath, FifoConfiguration config, CancellationToken ct = default);

    /// <summary>Detener monitoreo</summary>
    Task StopAsync();

    /// <summary>Si el servicio está corriendo</summary>
    bool IsRunning { get; }

    /// <summary>Archivos detectados desde el inicio</summary>
    long FilesDetected { get; }

    /// <summary>Limpiezas preventivas ejecutadas</summary>
    int PreventiveCleanups { get; }

    /// <summary>Evento cuando se detecta un archivo nuevo</summary>
    event EventHandler<FileDetectedEventArgs>? OnFileDetected;

    /// <summary>Evento cuando se ejecuta limpieza preventiva</summary>
    event EventHandler<CleanupResult>? OnPreventiveCleanup;
}

/// <summary>
/// Args para el evento de detección de archivo.
/// </summary>
public class FileDetectedEventArgs : EventArgs
{
    public string FilePath { get; set; } = string.Empty;
    public string AssetId { get; set; } = string.Empty;
    public string VariableId { get; set; } = string.Empty;
    public string FolderType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime DetectedAt { get; set; } = DateTime.Now;
}
