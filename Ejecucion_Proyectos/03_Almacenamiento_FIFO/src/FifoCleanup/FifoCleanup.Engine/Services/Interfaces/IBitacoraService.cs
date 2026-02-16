using FifoCleanup.Engine.Models;

namespace FifoCleanup.Engine.Services.Interfaces;

/// <summary>
/// RF-05: Servicio de bitácora/auditoría.
/// Registra todas las operaciones en archivos CSV inmutables.
/// </summary>
public interface IBitacoraService
{
    /// <summary>Registrar una nueva entrada en la bitácora</summary>
    Task LogAsync(BitacoraEntry entry);

    /// <summary>Obtener entradas filtradas por rango de fechas</summary>
    Task<List<BitacoraEntry>> GetEntriesAsync(DateTime? from = null, DateTime? to = null,
        BitacoraEventType? type = null, string? assetId = null, int? limit = null);

    /// <summary>Exportar bitácora a CSV</summary>
    Task<string> ExportToCsvAsync(string outputPath, DateTime? from = null, DateTime? to = null);

    /// <summary>Ejecutar mantenimiento: rotación de archivos y limpieza de entradas antiguas</summary>
    Task MaintenanceAsync(int retentionDays, int maxFileSizeMB);

    /// <summary>Inicializar directorio de bitácora</summary>
    Task InitializeAsync(string bitacoraPath);
}
