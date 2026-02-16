using FifoCleanup.Engine.Models;

namespace FifoCleanup.Engine.Services.Interfaces;

/// <summary>
/// RF-01: Servicio de inventario de almacenamiento.
/// Escanea la estructura Asset/[ID]/[E|F]/[YYYY]/[MM]/[DD] y calcula métricas.
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// Escanear ruta de almacenamiento y generar inventario completo.
    /// Debe completar en menos de 60s para 5TB (RNF).
    /// </summary>
    Task<StorageStatus> ScanAsync(string storagePath, CancellationToken ct = default,
        IProgress<(string message, double percent)>? progress = null);

    /// <summary>Obtener información de un Asset específico</summary>
    Task<AssetInfo?> ScanAssetAsync(string assetPath, CancellationToken ct = default);

    /// <summary>Obtener estado del disco (total, libre, usado)</summary>
    StorageStatus GetDriveInfo(string path);

    /// <summary>
    /// Calcular tasa de crecimiento diario promedio basado en historial.
    /// Analiza los últimos N días de datos para proyectar crecimiento.
    /// </summary>
    double CalculateAverageDailyGrowth(List<DayFolderInfo> dayFolders, int lookbackDays = 7);
}
