using FifoCleanup.Tests.Infrastructure;
using FifoCleanup.Tests.Models;

namespace FifoCleanup.Tests.TestSuites;

/// <summary>RF-01/RF-02: Pruebas de Inventario y Caracterización</summary>
public static class InventarioTests
{
    public static List<TestCase> GetTests(TestContext ctx)
    {
        return new List<TestCase>
        {
            new()
            {
                Id = "TC-0101", Area = "Inventario", Titulo = "Escaneo básico de Assets",
                Descripcion = "Verificar que el servicio de inventario detecta todos los Assets en la ruta configurada",
                Precondiciones = "Existen al menos 3 Assets con datos en D:\\FifoTestBed\\Data",
                Pasos = "1. Crear 3 Assets con datos | 2. Ejecutar ScanAsync | 3. Verificar conteo de Assets",
                ResultadoEsperado = "Se detectan exactamente 3 Assets", Prioridad = "Alta", CARef = "RF-01", Tipo = "Funcional",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 3, variables: 2, days: 5, mbPerDay: 1);
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    bool ok = status.Assets.Count == 3;
                    return (ok, $"Assets detectados: {status.Assets.Count} (esperado: 3)");
                }
            },
            new()
            {
                Id = "TC-0102", Area = "Inventario", Titulo = "Detección de Variables E y F",
                Descripcion = "Verificar que el inventario detecta subcarpetas E y F dentro de cada variable",
                Precondiciones = "Existen Assets con carpetas E y F",
                Pasos = "1. Crear datos con E y F | 2. Ejecutar ScanAsync | 3. Verificar HasEData y HasFData",
                ResultadoEsperado = "Todas las variables reportan HasEData=true y HasFData=true", Prioridad = "Alta", CARef = "RF-01", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    var allVars = status.Assets.SelectMany(a => a.Variables).ToList();
                    bool allE = allVars.All(v => v.HasEData);
                    bool allF = allVars.All(v => v.HasFData);
                    return (allE && allF, $"Variables: {allVars.Count}, ConE: {allVars.Count(v => v.HasEData)}, ConF: {allVars.Count(v => v.HasFData)}");
                }
            },
            new()
            {
                Id = "TC-0103", Area = "Inventario", Titulo = "Cálculo de tamaño por Asset",
                Descripcion = "Verificar que TotalSizeBytes refleja el tamaño real en disco de cada Asset",
                Precondiciones = "Datos de prueba generados",
                Pasos = "1. ScanAsync | 2. Comparar TotalSizeBytes de cada Asset con tamaño real en disco",
                ResultadoEsperado = "Diferencia < 1% entre tamaño reportado y tamaño real", Prioridad = "Alta", CARef = "RF-01", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    foreach (var asset in status.Assets)
                    {
                        long realSize = ctx.GetDirectorySize(asset.FullPath);
                        double diff = Math.Abs(asset.TotalSizeBytes - realSize) / (double)Math.Max(realSize, 1) * 100;
                        if (diff > 5)
                            return (false, $"Asset {asset.AssetId}: reportado={asset.TotalSizeBytes}, real={realSize}, diff={diff:F1}%");
                    }
                    return (true, $"Todos los Assets con tamaños correctos. Total monitoreado: {status.MonitoredDataBytes / (1024.0 * 1024):F1} MB");
                }
            },
            new()
            {
                Id = "TC-0104", Area = "Inventario", Titulo = "Detección de carpetas de día YYYY/MM/DD",
                Descripcion = "Verificar que la estructura de carpetas de día se parsea correctamente",
                Precondiciones = "Datos con 5 días de historia",
                Pasos = "1. ScanAsync | 2. Verificar DayFolders.Count por variable | 3. Verificar fechas",
                ResultadoEsperado = "Cada variable tiene 5 DayFolders con fechas válidas", Prioridad = "Alta", CARef = "RF-01", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    var firstVar = status.Assets.First().Variables.First();
                    int expected = 5 * 2; // 5 days * 2 folder types (E, F)
                    bool ok = firstVar.DayFolders.Count == expected;
                    var dates = firstVar.DayFolders.Select(d => d.Date).Distinct().OrderBy(d => d).ToList();
                    return (ok, $"DayFolders: {firstVar.DayFolders.Count} (esperado: {expected}), Fechas únicas: {dates.Count}");
                }
            },
            new()
            {
                Id = "TC-0105", Area = "Inventario", Titulo = "Cálculo de crecimiento diario promedio",
                Descripcion = "Verificar que AverageDailyGrowthBytes se calcula correctamente",
                Precondiciones = "Datos con al menos 5 días de historia",
                Pasos = "1. ScanAsync | 2. Verificar AverageDailyGrowthBytes > 0",
                ResultadoEsperado = "AverageDailyGrowthBytes > 0 para Assets con datos recientes", Prioridad = "Alta", CARef = "RF-02", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    bool anyGrowth = status.Assets.Any(a => a.AverageDailyGrowthBytes > 0);
                    double globalGrowth = status.AverageDailyGrowthBytes;
                    return (anyGrowth && globalGrowth > 0, $"Crecimiento diario global: {globalGrowth / (1024 * 1024):F2} MB/día");
                }
            },
            new()
            {
                Id = "TC-0106", Area = "Inventario", Titulo = "GetDriveInfo reporta disco D:\\",
                Descripcion = "Verificar que GetDriveInfo retorna información correcta del disco D:\\",
                Precondiciones = "Disco D:\\ existe y es accesible",
                Pasos = "1. Llamar GetDriveInfo('D:\\') | 2. Verificar TotalSpaceBytes > 0 y FreeSpaceBytes > 0",
                ResultadoEsperado = "TotalSpaceBytes y FreeSpaceBytes son valores positivos válidos", Prioridad = "Media", CARef = "RF-01", Tipo = "Funcional",
                TestAction = async () =>
                {
                    await Task.CompletedTask;
                    var info = ctx.Inventory.GetDriveInfo(@"D:\");
                    bool ok = info.TotalSpaceBytes > 0 && info.FreeSpaceBytes > 0;
                    double totalGB = info.TotalSpaceBytes / (1024.0 * 1024 * 1024);
                    double freeGB = info.FreeSpaceBytes / (1024.0 * 1024 * 1024);
                    return (ok, $"D:\\ Total: {totalGB:F2} GB, Libre: {freeGB:F2} GB, Uso: {info.UsagePercent:F1}%");
                }
            },
            new()
            {
                Id = "TC-0107", Area = "Inventario", Titulo = "Proporción de llenado (FillProportion)",
                Descripcion = "Verificar que FillProportion suma 100% entre todos los Assets",
                Precondiciones = "Múltiples Assets con datos",
                Pasos = "1. ScanAsync | 2. Sumar FillProportion de todos los Assets | 3. Verificar ~100%",
                ResultadoEsperado = "Suma de FillProportion ≈ 100%", Prioridad = "Media", CARef = "RF-01", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    double totalProp = status.Assets.Sum(a => a.FillProportion);
                    bool ok = Math.Abs(totalProp - 100) < 1;
                    return (ok, $"Suma FillProportion: {totalProp:F2}% (esperado: ~100%)");
                }
            },
            new()
            {
                Id = "TC-0108", Area = "Inventario", Titulo = "Escaneo de ruta vacía",
                Descripcion = "Verificar que ScanAsync maneja correctamente una ruta sin datos",
                Precondiciones = "Ruta existe pero está vacía",
                Pasos = "1. Crear ruta vacía | 2. ScanAsync | 3. Verificar Assets.Count == 0",
                ResultadoEsperado = "Retorna StorageStatus con Assets vacío, sin errores", Prioridad = "Media", CARef = "RF-01", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var emptyPath = Path.Combine(ctx.BasePath, "EmptyDir");
                    Directory.CreateDirectory(emptyPath);
                    var status = await ctx.Inventory.ScanAsync(emptyPath);
                    bool ok = status.Assets.Count == 0 && status.MonitoredDataBytes == 0;
                    try { Directory.Delete(emptyPath, true); } catch { }
                    return (ok, $"Assets: {status.Assets.Count}, MonitoredBytes: {status.MonitoredDataBytes}");
                }
            },
            new()
            {
                Id = "TC-0109", Area = "Inventario", Titulo = "Escaneo de ruta inexistente",
                Descripcion = "Verificar que ScanAsync no lanza excepción con ruta inexistente",
                Precondiciones = "Ruta no existe",
                Pasos = "1. ScanAsync con ruta inexistente | 2. Verificar que retorna sin error",
                ResultadoEsperado = "Retorna StorageStatus vacío sin lanzar excepción", Prioridad = "Media", CARef = "RF-01", Tipo = "Funcional",
                TestAction = async () =>
                {
                    try
                    {
                        var status = await ctx.Inventory.ScanAsync(@"D:\RutaQueNoExiste_Test");
                        return (status.Assets.Count == 0, "Manejó ruta inexistente correctamente");
                    }
                    catch (Exception ex)
                    {
                        return (false, $"Lanzó excepción: {ex.Message}");
                    }
                }
            },
            new()
            {
                Id = "TC-0110", Area = "Inventario", Titulo = "Rendimiento escaneo < 60s",
                Descripcion = "Verificar que el escaneo completa en menos de 60 segundos (RNF)",
                Precondiciones = "Datos de prueba generados",
                Pasos = "1. ScanAsync | 2. Verificar ScanDurationMs < 60000",
                ResultadoEsperado = "ScanDurationMs < 60000 ms", Prioridad = "Alta", CARef = "RNF-01", Tipo = "Rendimiento",
                TestAction = async () =>
                {
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    bool ok = status.ScanDurationMs < 60000;
                    return (ok, $"Duración del escaneo: {status.ScanDurationMs} ms");
                }
            },
        };
    }
}
