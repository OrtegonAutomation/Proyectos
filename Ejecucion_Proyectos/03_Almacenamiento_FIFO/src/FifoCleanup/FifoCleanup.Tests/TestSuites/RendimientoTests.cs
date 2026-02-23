using FifoCleanup.Engine.Models;
using FifoCleanup.Tests.Infrastructure;
using FifoCleanup.Tests.Models;
using System.Diagnostics;

namespace FifoCleanup.Tests.TestSuites;

/// <summary>Pruebas de Rendimiento, Confiabilidad y StorageStatus</summary>
public static class RendimientoTests
{
    public static List<TestCase> GetTests(TestContext ctx)
    {
        return new List<TestCase>
        {
            new()
            {
                Id = "TC-0901", Area = "Rendimiento", Titulo = "Escaneo completa en < 60 segundos",
                Descripcion = "Verificar que ScanAsync completa dentro del RNF de 60 segundos",
                Precondiciones = "Datos de prueba generados",
                Pasos = "1. ScanAsync | 2. Medir tiempo | 3. Verificar < 60s",
                ResultadoEsperado = "ScanDurationMs < 60000", Prioridad = "Alta", CARef = "RNF-01", Tipo = "Rendimiento",
                TestAction = async () =>
                {
                    var sw = Stopwatch.StartNew();
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    sw.Stop();
                    return (sw.ElapsedMilliseconds < 60000, $"Duración: {sw.ElapsedMilliseconds} ms, Assets: {status.Assets.Count}");
                }
            },
            new()
            {
                Id = "TC-0902", Area = "Rendimiento", Titulo = "GenerateSyntheticData con archivos reales (64KB buffer)",
                Descripcion = "Verificar que la generación de datos usa buffers de 64KB eficientemente",
                Precondiciones = "Ninguna",
                Pasos = "1. Generar 50 MB de datos | 2. Medir velocidad | 3. Verificar > 10 MB/s",
                ResultadoEsperado = "Velocidad de escritura > 10 MB/s", Prioridad = "Media", CARef = "RNF-02", Tipo = "Rendimiento",
                TestAction = async () =>
                {
                    var simPath = Path.Combine(ctx.BasePath, "PerfTest");
                    if (Directory.Exists(simPath)) Directory.Delete(simPath, true);

                    var param = new SimulationParams
                    {
                        SimulationPath = simPath,
                        NumberOfAssets = 1,
                        VariablesPerAsset = 1,
                        DaysOfHistory = 5,
                        AvgDayFolderSizeMB = 10,
                        SimulatedDiskSizeGB = 1,
                        InitialUsagePercent = 100
                    };

                    var sw = Stopwatch.StartNew();
                    await ctx.Simulation.GenerateSyntheticDataAsync(param);
                    sw.Stop();

                    long size = ctx.GetDirectorySize(simPath);
                    double mbPerSec = (size / (1024.0 * 1024)) / (sw.ElapsedMilliseconds / 1000.0);

                    try { Directory.Delete(simPath, true); } catch { }
                    return (mbPerSec > 10, $"Velocidad: {mbPerSec:F1} MB/s, Generados: {size / (1024.0 * 1024):F1} MB en {sw.ElapsedMilliseconds} ms");
                }
            },
            new()
            {
                Id = "TC-0903", Area = "Rendimiento", Titulo = "Limpieza general procesa rápidamente",
                Descripcion = "Verificar que la limpieza general completa en tiempo razonable",
                Precondiciones = "Datos generados",
                Pasos = "1. ScanAsync | 2. Medir ExecuteGeneralCleanupAsync | 3. Verificar < 30s",
                ResultadoEsperado = "DurationMs < 30000", Prioridad = "Media", CARef = "RNF-01", Tipo = "Rendimiento",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 3, variables: 2, days: 10, mbPerDay: 1);
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    status.ApplyStorageLimit(0.1);

                    var config = new FifoConfiguration { StoragePath = ctx.DataPath, ThresholdPercent = 85, CleanupCapPercent = 20, MaxStorageSizeGB = 0.1 };

                    var sw = Stopwatch.StartNew();
                    var result = await ctx.Cleanup.ExecuteGeneralCleanupAsync(status, config);
                    sw.Stop();

                    return (sw.ElapsedMilliseconds < 30000, $"Limpieza en {sw.ElapsedMilliseconds} ms, BytesFreed: {result.BytesFreedFormatted}");
                }
            },
            new()
            {
                Id = "TC-0904", Area = "StorageStatus", Titulo = "ApplyStorageLimit con valor 0 (disco real)",
                Descripcion = "Verificar que ApplyStorageLimit(0) no modifica TotalSpaceBytes",
                Precondiciones = "Datos escaneados",
                Pasos = "1. ScanAsync | 2. Guardar TotalSpaceBytes | 3. ApplyStorageLimit(0) | 4. Verificar sin cambio",
                ResultadoEsperado = "TotalSpaceBytes permanece igual", Prioridad = "Alta", CARef = "RF-07, RF-08", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    long original = status.TotalSpaceBytes;
                    status.ApplyStorageLimit(0);
                    bool unchanged = status.TotalSpaceBytes == original;
                    return (unchanged, $"Original: {original}, Después de ApplyStorageLimit(0): {status.TotalSpaceBytes}");
                }
            },
            new()
            {
                Id = "TC-0905", Area = "StorageStatus", Titulo = "ApplyStorageLimit con valor positivo",
                Descripcion = "Verificar que ApplyStorageLimit(2) establece TotalSpaceBytes = 2 GB",
                Precondiciones = "Datos escaneados",
                Pasos = "1. ScanAsync | 2. ApplyStorageLimit(2) | 3. Verificar TotalSpaceBytes ≈ 2 GB",
                ResultadoEsperado = "TotalSpaceBytes = 2 * 1024^3", Prioridad = "Alta", CARef = "RF-07, RF-08", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    status.ApplyStorageLimit(2.0);
                    long expected = (long)(2.0 * 1024 * 1024 * 1024);
                    bool ok = status.TotalSpaceBytes == expected;
                    return (ok, $"TotalSpaceBytes: {status.TotalSpaceBytes}, Esperado: {expected}");
                }
            },
            new()
            {
                Id = "TC-0906", Area = "StorageStatus", Titulo = "UsagePercent calcula correctamente",
                Descripcion = "Verificar que UsagePercent = UsedSpaceBytes / TotalSpaceBytes * 100",
                Precondiciones = "Datos escaneados",
                Pasos = "1. ScanAsync | 2. Verificar cálculo manual vs propiedad",
                ResultadoEsperado = "Diferencia < 0.1%", Prioridad = "Media", CARef = "RF-01", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    double calculated = status.TotalSpaceBytes > 0
                        ? (double)status.UsedSpaceBytes / status.TotalSpaceBytes * 100.0 : 0;
                    double diff = Math.Abs(calculated - status.UsagePercent);
                    return (diff < 0.1, $"Calculado: {calculated:F4}%, Propiedad: {status.UsagePercent:F4}%, Diff: {diff:F6}%");
                }
            },
            new()
            {
                Id = "TC-0907", Area = "StorageStatus", Titulo = "StorageLevel correcto según UsagePercent",
                Descripcion = "Verificar que Level cambia según los rangos: Green <70%, Yellow 70-85%, Red >85%",
                Precondiciones = "Ninguna",
                Pasos = "1. Crear StorageStatus con diferentes usos | 2. Verificar Level",
                ResultadoEsperado = "Green/Yellow/Red según rangos definidos", Prioridad = "Media", CARef = "RF-01", Tipo = "Funcional",
                TestAction = async () =>
                {
                    await Task.CompletedTask;
                    var s1 = new StorageStatus { TotalSpaceBytes = 1000, UsedSpaceBytes = 500 }; // 50%
                    var s2 = new StorageStatus { TotalSpaceBytes = 1000, UsedSpaceBytes = 750 }; // 75%
                    var s3 = new StorageStatus { TotalSpaceBytes = 1000, UsedSpaceBytes = 900 }; // 90%

                    bool ok = s1.Level == StorageLevel.Green
                        && s2.Level == StorageLevel.Yellow
                        && s3.Level == StorageLevel.Red;
                    return (ok, $"50%={s1.Level}(Green), 75%={s2.Level}(Yellow), 90%={s3.Level}(Red)");
                }
            },
            new()
            {
                Id = "TC-0908", Area = "StorageStatus", Titulo = "FreeSpaceBytes = Total - Used tras ApplyStorageLimit",
                Descripcion = "Verificar que FreeSpaceBytes se recalcula correctamente",
                Precondiciones = "Datos escaneados",
                Pasos = "1. ScanAsync | 2. ApplyStorageLimit(1) | 3. Verificar Free = Total - Used",
                ResultadoEsperado = "FreeSpaceBytes = TotalSpaceBytes - UsedSpaceBytes", Prioridad = "Media", CARef = "RF-07, RF-08", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    status.ApplyStorageLimit(1.0);
                    long expectedFree = Math.Max(0, status.TotalSpaceBytes - status.UsedSpaceBytes);
                    bool ok = status.FreeSpaceBytes == expectedFree;
                    return (ok, $"Free: {status.FreeSpaceBytes}, Expected: {expectedFree}, Total: {status.TotalSpaceBytes}, Used: {status.UsedSpaceBytes}");
                }
            },
        };
    }
}
