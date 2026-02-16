using FifoCleanup.Engine.Models;

namespace FifoCleanup.Engine.Services.Interfaces;

/// <summary>
/// RF-04: Servicio de limpieza FIFO.
/// Elimina carpetas de día más antiguas respetando orden FIFO y proporcionalidad.
/// </summary>
public interface ICleanupService
{
    /// <summary>
    /// Ejecutar limpieza FIFO general (RF-07 style).
    /// Elimina proporcionalmente de todos los Assets hasta liberar lo necesario.
    /// </summary>
    /// <param name="status">Estado actual del storage</param>
    /// <param name="config">Configuración activa</param>
    /// <param name="ct">Token de cancelación</param>
    /// <param name="progress">Reporte de progreso</param>
    /// <param name="dryRun">Si true, solo calcula sin eliminar</param>
    Task<CleanupResult> ExecuteGeneralCleanupAsync(
        StorageStatus status,
        FifoConfiguration config,
        CancellationToken ct = default,
        IProgress<(string message, double percent)>? progress = null,
        bool dryRun = false);

    /// <summary>
    /// Ejecutar limpieza FIFO local (RF-08 style).
    /// Solo limpia el Asset/Variable donde se detectó la adición.
    /// </summary>
    /// <param name="assetId">Asset donde se detectó crecimiento</param>
    /// <param name="variableId">Variable específica</param>
    /// <param name="config">Configuración activa</param>
    /// <param name="ct">Token de cancelación</param>
    Task<CleanupResult> ExecuteLocalCleanupAsync(
        string assetId,
        string variableId,
        StorageStatus status,
        FifoConfiguration config,
        CancellationToken ct = default);

    /// <summary>
    /// Calcular qué se eliminaría sin ejecutar la eliminación.
    /// Para preview en UI antes de confirmar.
    /// </summary>
    Task<CleanupResult> PreviewCleanupAsync(
        StorageStatus status,
        FifoConfiguration config,
        CancellationToken ct = default);

    /// <summary>Obtener las carpetas de día ordenadas FIFO (más antiguas primero)</summary>
    List<DayFolderInfo> GetFifoOrder(List<AssetInfo> assets);
}
