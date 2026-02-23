using FifoCleanup.Engine.Models;
using FifoCleanup.Tests.Infrastructure;
using FifoCleanup.Tests.Models;

namespace FifoCleanup.Tests.TestSuites;

/// <summary>Pruebas de Casos Borde y Robustez</summary>
public static class EdgeCaseTests
{
    public static List<TestCase> GetTests(TestContext ctx)
    {
        return new List<TestCase>
        {
            new()
            {
                Id = "TC-1101", Area = "Edge Cases", Titulo = "Limpieza con 0 carpetas de día",
                Descripcion = "Verificar que ExecuteGeneralCleanupAsync maneja correctamente cuando no hay carpetas de día",
                Precondiciones = "Ruta vacía con estructura de Asset pero sin días",
                Pasos = "1. Crear Asset sin carpetas de día | 2. ScanAsync | 3. ExecuteGeneralCleanupAsync",
                ResultadoEsperado = "No lanza excepción, BytesFreed = 0", Prioridad = "Media", CARef = "RF-04", Tipo = "Edge Case",
                TestAction = async () =>
                {
                    var emptyAssetPath = Path.Combine(ctx.DataPath, "AssetEmpty", "00", "E");
                    Directory.CreateDirectory(emptyAssetPath);

                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    status.ApplyStorageLimit(0.01); // Very small limit

                    var config = new FifoConfiguration { StoragePath = ctx.DataPath, ThresholdPercent = 50, CleanupCapPercent = 50, MaxStorageSizeGB = 0.01 };

                    try
                    {
                        var result = await ctx.Cleanup.ExecuteGeneralCleanupAsync(status, config);
                        return (true, $"Success: {result.Success}, BytesFreed: {result.BytesFreed}, Msg: {result.Message}");
                    }
                    catch (Exception ex)
                    {
                        return (false, $"Excepción: {ex.Message}");
                    }
                }
            },
            new()
            {
                Id = "TC-1102", Area = "Edge Cases", Titulo = "Asset con un solo día de datos",
                Descripcion = "Verificar limpieza cuando Asset tiene exactamente 1 día",
                Precondiciones = "Asset con 1 día de datos",
                Pasos = "1. Crear Asset con 1 día | 2. ScanAsync | 3. ExecuteLocalCleanupAsync | 4. Verificar eliminación",
                ResultadoEsperado = "Se elimina el único día disponible", Prioridad = "Media", CARef = "RF-04", Tipo = "Edge Case",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 1, variables: 1, days: 1, mbPerDay: 1);

                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    var config = new FifoConfiguration { StoragePath = ctx.DataPath, MaxDaysToDeletePerAsset = 5 };

                    var result = await ctx.Cleanup.ExecuteLocalCleanupAsync("Asset001", "00", status, config);
                    return (result.Success, $"Success: {result.Success}, FoldersDeleted: {result.FoldersDeleted}, BytesFreed: {result.BytesFreedFormatted}");
                }
            },
            new()
            {
                Id = "TC-1103", Area = "Edge Cases", Titulo = "MaxStorageSizeGB negativo tratado como 0",
                Descripcion = "Verificar que un valor negativo de MaxStorageSizeGB no altera el StorageStatus",
                Precondiciones = "Datos escaneados",
                Pasos = "1. ScanAsync | 2. ApplyStorageLimit(-5) | 3. Verificar TotalSpaceBytes sin cambio",
                ResultadoEsperado = "TotalSpaceBytes permanece igual al disco real", Prioridad = "Media", CARef = "RF-07, RF-08", Tipo = "Edge Case",
                TestAction = async () =>
                {
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    long original = status.TotalSpaceBytes;
                    status.ApplyStorageLimit(-5);
                    bool ok = status.TotalSpaceBytes == original;
                    return (ok, $"Original: {original}, Después de -5: {status.TotalSpaceBytes}");
                }
            },
            new()
            {
                Id = "TC-1104", Area = "Edge Cases", Titulo = "Cancelación de limpieza general",
                Descripcion = "Verificar que la limpieza se cancela limpiamente con CancellationToken",
                Precondiciones = "Datos generados",
                Pasos = "1. Crear muchos datos | 2. Iniciar limpieza | 3. Cancelar después de 100ms | 4. Verificar WasCancelled",
                ResultadoEsperado = "WasCancelled=true, no se eliminó todo", Prioridad = "Media", CARef = "RF-04", Tipo = "Edge Case",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 5, variables: 3, days: 15, mbPerDay: 1);

                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    status.ApplyStorageLimit(0.05);

                    var config = new FifoConfiguration { StoragePath = ctx.DataPath, ThresholdPercent = 50, CleanupCapPercent = 50, MaxStorageSizeGB = 0.05 };

                    var cts = new CancellationTokenSource();
                    cts.CancelAfter(100);

                    var result = await ctx.Cleanup.ExecuteGeneralCleanupAsync(status, config, cts.Token);
                    return (result.WasCancelled, $"WasCancelled: {result.WasCancelled}, BytesFreed: {result.BytesFreedFormatted}");
                }
            },
            new()
            {
                Id = "TC-1105", Area = "Edge Cases", Titulo = "Limpieza local con Asset inexistente",
                Descripcion = "Verificar que limpieza local con AssetId inexistente no lanza excepción",
                Precondiciones = "Datos escaneados",
                Pasos = "1. ScanAsync | 2. ExecuteLocalCleanupAsync con 'AssetXXX' | 3. Verificar Success=false",
                ResultadoEsperado = "Success=false con mensaje de Asset no encontrado", Prioridad = "Media", CARef = "RF-04", Tipo = "Edge Case",
                TestAction = async () =>
                {
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    var config = new FifoConfiguration { StoragePath = ctx.DataPath, MaxDaysToDeletePerAsset = 3 };

                    var result = await ctx.Cleanup.ExecuteLocalCleanupAsync("AssetXXX", "99", status, config);
                    bool notFound = !result.Success && result.Message.Contains("no encontr", StringComparison.OrdinalIgnoreCase);
                    return (notFound, $"Success: {result.Success}, Msg: {result.Message}");
                }
            },
            new()
            {
                Id = "TC-1106", Area = "Edge Cases", Titulo = "Bitácora con caracteres especiales en detalles",
                Descripcion = "Verificar que la bitácora maneja correctamente caracteres Unicode y especiales",
                Precondiciones = "Bitácora inicializada",
                Pasos = "1. LogAsync con Details conteniendo emojis y acentos | 2. Recuperar | 3. Verificar integridad",
                ResultadoEsperado = "Caracteres especiales se preservan", Prioridad = "Baja", CARef = "RF-05", Tipo = "Edge Case",
                TestAction = async () =>
                {
                    await ctx.Bitacora.InitializeAsync(ctx.BitacoraPath);
                    var entry = new BitacoraEntry
                    {
                        EventType = BitacoraEventType.Information,
                        Action = "UNICODE_TEST",
                        Details = "Prueba con acentos: áéíóú ñ, emojis: 🔥💾✅"
                    };
                    await ctx.Bitacora.LogAsync(entry);

                    var entries = await ctx.Bitacora.GetEntriesAsync();
                    var found = entries.FirstOrDefault(e => e.Action == "UNICODE_TEST");
                    bool ok = found != null && found.Details.Contains("áéíóú") && found.Details.Contains("ñ");
                    return (ok, found != null ? $"Recuperado: {found.Details}" : "No encontrada");
                }
            },
            new()
            {
                Id = "TC-1107", Area = "Edge Cases", Titulo = "Escaneo con Asset que solo tiene tendencias",
                Descripcion = "Verificar que Assets con solo carpetas de tendencias (no E/F) se reportan vacíos",
                Precondiciones = "Asset con solo subcarpeta de tendencias (YYYY)",
                Pasos = "1. Crear Asset con solo carpeta 2026 | 2. ScanAsync | 3. Verificar que no tiene Variables",
                ResultadoEsperado = "El Asset no genera Variables en el inventario", Prioridad = "Baja", CARef = "RF-01", Tipo = "Edge Case",
                TestAction = async () =>
                {
                    var trendAsset = Path.Combine(ctx.DataPath, "AssetTrendOnly", "00", "2026");
                    Directory.CreateDirectory(trendAsset);
                    await File.WriteAllBytesAsync(Path.Combine(trendAsset, "trend.dat"), new byte[128]);

                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    var asset = status.Assets.FirstOrDefault(a => a.AssetId == "AssetTrendOnly");

                    bool noVars = asset == null || asset.Variables.Count == 0 || asset.TotalSizeBytes == 0;
                    return (noVars, asset != null
                        ? $"Variables: {asset.Variables.Count}, Size: {asset.TotalSizeBytes}"
                        : "Asset no encontrado en inventario (correcto si no tiene E/F)");
                }
            },
            new()
            {
                Id = "TC-1108", Area = "Edge Cases", Titulo = "Múltiples limpiezas consecutivas",
                Descripcion = "Verificar que ejecutar limpieza múltiples veces seguidas no causa errores",
                Precondiciones = "Datos generados",
                Pasos = "1. Ejecutar limpieza 3 veces seguidas | 2. Verificar que no hay errores",
                ResultadoEsperado = "Todas las ejecuciones completan sin excepción", Prioridad = "Media", CARef = "RF-04", Tipo = "Edge Case",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 3, variables: 2, days: 10, mbPerDay: 1);

                    int successCount = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                        status.ApplyStorageLimit(0.1);
                        var config = new FifoConfiguration { StoragePath = ctx.DataPath, ThresholdPercent = 80, CleanupCapPercent = 15, MaxStorageSizeGB = 0.1 };
                        var result = await ctx.Cleanup.ExecuteGeneralCleanupAsync(status, config);
                        if (result.Success) successCount++;
                    }

                    return (successCount == 3, $"Limpiezas exitosas: {successCount}/3");
                }
            },
        };
    }
}
