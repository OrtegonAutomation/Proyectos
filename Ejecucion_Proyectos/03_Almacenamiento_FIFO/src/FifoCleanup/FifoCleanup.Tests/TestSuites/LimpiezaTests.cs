using FifoCleanup.Engine.Models;
using FifoCleanup.Tests.Infrastructure;
using FifoCleanup.Tests.Models;

namespace FifoCleanup.Tests.TestSuites;

/// <summary>RF-04: Pruebas de Limpieza FIFO</summary>
public static class LimpiezaTests
{
    public static List<TestCase> GetTests(TestContext ctx)
    {
        return new List<TestCase>
        {
            new()
            {
                Id = "TC-0401", Area = "Limpieza", Titulo = "Limpieza general libera espacio",
                Descripcion = "Verificar que ExecuteGeneralCleanupAsync elimina carpetas y libera bytes",
                Precondiciones = "Datos generados con uso > umbral",
                Pasos = "1. Generar datos | 2. ScanAsync | 3. ApplyStorageLimit | 4. ExecuteGeneralCleanupAsync | 5. Verificar BytesFreed > 0",
                ResultadoEsperado = "BytesFreed > 0 y FoldersDeleted > 0", Prioridad = "Alta", CARef = "RF-04", Tipo = "Funcional",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 3, variables: 2, days: 15, mbPerDay: 2);

                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    // Forzar que el uso sea alto para el límite configurado
                    status.ApplyStorageLimit(0.3); // 300 MB limit, datos serán ~360 MB

                    var config = new FifoConfiguration
                    {
                        StoragePath = ctx.DataPath,
                        ThresholdPercent = 85,
                        CleanupCapPercent = 30,
                        MaxStorageSizeGB = 0.3
                    };

                    var result = await ctx.Cleanup.ExecuteGeneralCleanupAsync(status, config);
                    return (result.Success && result.BytesFreed > 0, $"Success: {result.Success}, BytesFreed: {result.BytesFreedFormatted}, Folders: {result.FoldersDeleted}");
                }
            },
            new()
            {
                Id = "TC-0402", Area = "Limpieza", Titulo = "Orden FIFO: más antiguo primero",
                Descripcion = "Verificar que las carpetas se eliminan de más antigua a más reciente",
                Precondiciones = "Datos con múltiples días",
                Pasos = "1. Generar 15 días de datos | 2. Ejecutar limpieza | 3. Verificar que los datos más antiguos desaparecieron",
                ResultadoEsperado = "Las carpetas más antiguas fueron eliminadas primero", Prioridad = "Alta", CARef = "RF-04", Tipo = "Funcional",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    // 2 assets * 1 var * 15 days * 2 MB * 2 (E+F) = ~120 MB
                    await ctx.CreateTestDataAsync(assets: 2, variables: 1, days: 15, mbPerDay: 2);

                    var statusBefore = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    // Limit of 0.08 GB = ~80 MB → usage ~120/80 = 150% → well above threshold
                    statusBefore.ApplyStorageLimit(0.08);

                    var config = new FifoConfiguration
                    {
                        StoragePath = ctx.DataPath,
                        ThresholdPercent = 80,
                        CleanupCapPercent = 40,
                        MaxStorageSizeGB = 0.08
                    };

                    var oldest = statusBefore.Assets.SelectMany(a => a.Variables.SelectMany(v => v.DayFolders))
                        .OrderBy(d => d.Date).First();

                    var result = await ctx.Cleanup.ExecuteGeneralCleanupAsync(statusBefore, config);

                    bool oldestRemoved = !Directory.Exists(oldest.FullPath);
                    return (oldestRemoved, $"Carpeta más antigua ({oldest.Date:yyyy-MM-dd}) eliminada: {oldestRemoved}, BytesFreed: {result.BytesFreedFormatted}, Uso: {statusBefore.UsagePercent:F0}%");
                }
            },
            new()
            {
                Id = "TC-0403", Area = "Limpieza", Titulo = "Cap de limpieza respetado",
                Descripcion = "Verificar que no se elimina más del CleanupCapPercent de los datos",
                Precondiciones = "Datos generados",
                Pasos = "1. Generar datos | 2. Ejecutar limpieza con cap=10% | 3. Verificar BytesFreed ≤ 10% del total monitoreado",
                ResultadoEsperado = "BytesFreed ≤ MonitoredDataBytes * 10%", Prioridad = "Alta", CARef = "RF-04", Tipo = "Funcional",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 3, variables: 2, days: 15, mbPerDay: 2);

                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    status.ApplyStorageLimit(0.2); // Forces high usage

                    var config = new FifoConfiguration
                    {
                        StoragePath = ctx.DataPath,
                        ThresholdPercent = 50,
                        CleanupCapPercent = 10,
                        MaxStorageSizeGB = 0.2
                    };

                    long maxAllowed = (long)(status.MonitoredDataBytes * 0.10);
                    var result = await ctx.Cleanup.ExecuteGeneralCleanupAsync(status, config);

                    bool withinCap = result.BytesFreed <= maxAllowed * 1.05; // 5% tolerance
                    return (withinCap, $"BytesFreed: {result.BytesFreedFormatted}, Cap ({maxAllowed / (1024.0 * 1024):F1} MB), Respetado: {withinCap}");
                }
            },
            new()
            {
                Id = "TC-0404", Area = "Limpieza", Titulo = "Limpieza local RF-08 solo elimina del Asset/Variable indicado",
                Descripcion = "Verificar que ExecuteLocalCleanupAsync no toca otros Assets",
                Precondiciones = "Múltiples Assets con datos",
                Pasos = "1. Generar datos para 3 Assets | 2. Ejecutar limpieza local en Asset001/00 | 3. Verificar Asset002 y Asset003 intactos",
                ResultadoEsperado = "Solo se eliminan carpetas de Asset001/00", Prioridad = "Alta", CARef = "RF-04, RF-08", Tipo = "Funcional",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 3, variables: 2, days: 10, mbPerDay: 1);

                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    var asset2Before = status.Assets.FirstOrDefault(a => a.AssetId == "Asset002")?.TotalSizeBytes ?? 0;
                    var asset3Before = status.Assets.FirstOrDefault(a => a.AssetId == "Asset003")?.TotalSizeBytes ?? 0;

                    var config = new FifoConfiguration
                    {
                        StoragePath = ctx.DataPath,
                        MaxDaysToDeletePerAsset = 3
                    };

                    var result = await ctx.Cleanup.ExecuteLocalCleanupAsync("Asset001", "00", status, config);

                    var statusAfter = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    var asset2After = statusAfter.Assets.FirstOrDefault(a => a.AssetId == "Asset002")?.TotalSizeBytes ?? 0;
                    var asset3After = statusAfter.Assets.FirstOrDefault(a => a.AssetId == "Asset003")?.TotalSizeBytes ?? 0;

                    bool otherIntact = asset2After == asset2Before && asset3After == asset3Before;
                    return (result.Success && otherIntact, $"Local cleanup ok: {result.Success}, BytesFreed: {result.BytesFreedFormatted}, Otros intactos: {otherIntact}");
                }
            },
            new()
            {
                Id = "TC-0405", Area = "Limpieza", Titulo = "MaxDaysToDeletePerAsset respetado en limpieza local",
                Descripcion = "Verificar que la limpieza local no elimina más días del configurado",
                Precondiciones = "Asset con 10 días de datos",
                Pasos = "1. Generar datos con 10 días | 2. Ejecutar local cleanup con MaxDays=3 | 3. Verificar FoldersDeleted ≤ 3",
                ResultadoEsperado = "FoldersDeleted ≤ 3 (o equivalente a 3 days * folder types)", Prioridad = "Alta", CARef = "RF-04, RF-08", Tipo = "Funcional",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 1, variables: 1, days: 10, mbPerDay: 1);

                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    var config = new FifoConfiguration
                    {
                        StoragePath = ctx.DataPath,
                        MaxDaysToDeletePerAsset = 3
                    };

                    var result = await ctx.Cleanup.ExecuteLocalCleanupAsync("Asset001", "00", status, config);
                    // MaxDaysToDeletePerAsset applies to DayFolders (which include both E and F entries)
                    bool ok = result.FoldersDeleted <= 3;
                    return (ok, $"FoldersDeleted: {result.FoldersDeleted}, MaxDays config: 3");
                }
            },
            new()
            {
                Id = "TC-0406", Area = "Limpieza", Titulo = "Preview no elimina datos reales",
                Descripcion = "Verificar que PreviewCleanupAsync calcula sin borrar",
                Precondiciones = "Datos generados",
                Pasos = "1. ScanAsync | 2. PreviewCleanupAsync | 3. Verificar datos intactos en disco",
                ResultadoEsperado = "BytesFreed > 0 en preview pero tamaño en disco sin cambios", Prioridad = "Media", CARef = "RF-04", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    status.ApplyStorageLimit(0.1);
                    long sizeBefore = ctx.GetDirectorySize(ctx.DataPath);

                    var config = new FifoConfiguration { StoragePath = ctx.DataPath, ThresholdPercent = 85, CleanupCapPercent = 20, MaxStorageSizeGB = 0.1 };
                    var preview = await ctx.Cleanup.PreviewCleanupAsync(status, config);
                    long sizeAfter = ctx.GetDirectorySize(ctx.DataPath);

                    bool dataIntact = sizeAfter == sizeBefore;
                    return (dataIntact, $"Preview BytesFreed: {preview.BytesFreedFormatted}, DiskBefore: {sizeBefore}, DiskAfter: {sizeAfter}");
                }
            },
            new()
            {
                Id = "TC-0407", Area = "Limpieza", Titulo = "GetFifoOrder retorna orden cronológico",
                Descripcion = "Verificar que GetFifoOrder ordena de más antiguo a más reciente",
                Precondiciones = "Datos con múltiples días",
                Pasos = "1. ScanAsync | 2. GetFifoOrder | 3. Verificar Date[i] ≤ Date[i+1]",
                ResultadoEsperado = "La lista está ordenada cronológicamente", Prioridad = "Alta", CARef = "RF-04", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    var fifo = ctx.Cleanup.GetFifoOrder(status.Assets);

                    bool ordered = true;
                    for (int i = 1; i < fifo.Count; i++)
                    {
                        if (fifo[i].Date < fifo[i - 1].Date)
                        { ordered = false; break; }
                    }

                    return (ordered && fifo.Count > 0, $"FifoOrder items: {fifo.Count}, Ordenado: {ordered}");
                }
            },
            new()
            {
                Id = "TC-0408", Area = "Limpieza", Titulo = "No limpia si uso bajo umbral",
                Descripcion = "Verificar que ExecuteGeneralCleanupAsync no elimina nada si uso < umbral",
                Precondiciones = "Datos generados con uso bajo",
                Pasos = "1. ScanAsync | 2. ApplyStorageLimit con mucho espacio | 3. Ejecutar limpieza | 4. Verificar BytesFreed = 0",
                ResultadoEsperado = "BytesFreed = 0, mensaje indicando que no se requiere limpieza", Prioridad = "Media", CARef = "RF-04", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    status.ApplyStorageLimit(100); // 100 GB limit, usage will be very low

                    var config = new FifoConfiguration { StoragePath = ctx.DataPath, ThresholdPercent = 85, CleanupCapPercent = 20, MaxStorageSizeGB = 100 };
                    var result = await ctx.Cleanup.ExecuteGeneralCleanupAsync(status, config);

                    bool noCleanup = result.BytesFreed == 0;
                    return (noCleanup, $"BytesFreed: {result.BytesFreed}, Msg: {result.Message}");
                }
            },
        };
    }
}
