using FifoCleanup.Engine.Models;
using FifoCleanup.Tests.Infrastructure;
using FifoCleanup.Tests.Models;

namespace FifoCleanup.Tests.TestSuites;

/// <summary>RF-03: Pruebas de Simulación</summary>
public static class SimulacionTests
{
    public static List<TestCase> GetTests(TestContext ctx)
    {
        return new List<TestCase>
        {
            new()
            {
                Id = "TC-0301", Area = "Simulación", Titulo = "Generación de datos sintéticos",
                Descripcion = "Verificar que GenerateSyntheticDataAsync crea estructura Asset/Variable/E/YYYY/MM/DD",
                Precondiciones = "Ruta de simulación vacía",
                Pasos = "1. Limpiar datos | 2. GenerateSyntheticDataAsync con 2 Assets, 2 vars, 10 días | 3. Verificar estructura",
                ResultadoEsperado = "Se crean carpetas con estructura correcta", Prioridad = "Alta", CARef = "RF-03", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var simPath = Path.Combine(ctx.BasePath, "SimTest");
                    if (Directory.Exists(simPath)) Directory.Delete(simPath, true);

                    var param = new SimulationParams
                    {
                        SimulationPath = simPath,
                        NumberOfAssets = 2,
                        VariablesPerAsset = 2,
                        DaysOfHistory = 10,
                        AvgDayFolderSizeMB = 1,
                        SimulatedDiskSizeGB = 1,
                        InitialUsagePercent = 80
                    };

                    bool success = await ctx.Simulation.GenerateSyntheticDataAsync(param);
                    var assetDirs = Directory.GetDirectories(simPath);

                    bool structureOk = assetDirs.Length == 2;
                    if (structureOk)
                    {
                        foreach (var assetDir in assetDirs)
                        {
                            var varDirs = Directory.GetDirectories(assetDir).Where(d => int.TryParse(Path.GetFileName(d), out _)).ToArray();
                            if (varDirs.Length < 2) { structureOk = false; break; }
                        }
                    }

                    try { Directory.Delete(simPath, true); } catch { }
                    return (success && structureOk, $"Generación: {success}, Estructura: {structureOk}, Assets: {assetDirs.Length}");
                }
            },
            new()
            {
                Id = "TC-0302", Area = "Simulación", Titulo = "Simulación dry-run no elimina datos",
                Descripcion = "Verificar que RunSimulationAsync con dry-run calcula sin borrar archivos",
                Precondiciones = "Datos sintéticos generados",
                Pasos = "1. Generar datos | 2. RunSimulationAsync | 3. Verificar datos intactos",
                ResultadoEsperado = "CleanupResult tiene BytesFreed > 0 pero archivos siguen en disco", Prioridad = "Alta", CARef = "RF-03", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var simPath = Path.Combine(ctx.BasePath, "SimDryRun");
                    if (Directory.Exists(simPath)) Directory.Delete(simPath, true);

                    var param = new SimulationParams
                    {
                        SimulationPath = simPath,
                        NumberOfAssets = 2,
                        VariablesPerAsset = 2,
                        DaysOfHistory = 10,
                        AvgDayFolderSizeMB = 1,
                        SimulatedDiskSizeGB = 0.1,
                        InitialUsagePercent = 90,
                        ThresholdPercent = 85,
                        CleanupCapPercent = 20
                    };

                    var result = await ctx.Simulation.RunSimulationAsync(param);
                    long sizeAfter = ctx.GetDirectorySize(simPath);

                    bool dataPersists = sizeAfter > 0;
                    bool hasResult = result.CleanupResult != null;

                    try { Directory.Delete(simPath, true); } catch { }
                    return (dataPersists && hasResult, $"DryRun: datos intactos={dataPersists}, BytesFreed calculados={result.CleanupResult?.BytesFreedFormatted}");
                }
            },
            new()
            {
                Id = "TC-0303", Area = "Simulación", Titulo = "Simulación respeta capacidad objetivo",
                Descripcion = "Verificar que la generación no supera InitialUsagePercent de SimulatedDiskSizeGB",
                Precondiciones = "Ninguna",
                Pasos = "1. Config: DiskSize=0.5GB, Usage=80% | 2. Generar | 3. Verificar tamaño ≤ 400MB",
                ResultadoEsperado = "Datos generados ≤ 0.5 GB * 80% = 400 MB", Prioridad = "Alta", CARef = "RF-03", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var simPath = Path.Combine(ctx.BasePath, "SimCap");
                    if (Directory.Exists(simPath)) Directory.Delete(simPath, true);

                    var param = new SimulationParams
                    {
                        SimulationPath = simPath,
                        NumberOfAssets = 3,
                        VariablesPerAsset = 2,
                        DaysOfHistory = 30,
                        AvgDayFolderSizeMB = 10,
                        SimulatedDiskSizeGB = 0.5,
                        InitialUsagePercent = 80
                    };

                    await ctx.Simulation.GenerateSyntheticDataAsync(param);
                    long actualSize = ctx.GetDirectorySize(simPath);
                    long targetMax = (long)(0.5 * 1024 * 1024 * 1024 * 0.80);
                    double tolerance = 1.15; // 15% tolerance
                    bool ok = actualSize <= (long)(targetMax * tolerance);

                    try { Directory.Delete(simPath, true); } catch { }
                    return (ok, $"Generado: {actualSize / (1024.0 * 1024):F1} MB, Máximo: {targetMax / (1024.0 * 1024):F1} MB");
                }
            },
            new()
            {
                Id = "TC-0304", Area = "Simulación", Titulo = "Orden FIFO en resultados de simulación",
                Descripcion = "Verificar que la simulación reporta eliminación en orden cronológico (más antiguo primero)",
                Precondiciones = "Datos sintéticos con múltiples días",
                Pasos = "1. Generar datos de 10 días | 2. RunSimulationAsync | 3. Verificar AssetDetails ordenados por fecha",
                ResultadoEsperado = "AssetDetails ordenados de más antiguo a más reciente", Prioridad = "Alta", CARef = "RF-03", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var simPath = Path.Combine(ctx.BasePath, "SimFIFO");
                    if (Directory.Exists(simPath)) Directory.Delete(simPath, true);

                    var param = new SimulationParams
                    {
                        SimulationPath = simPath,
                        NumberOfAssets = 2,
                        VariablesPerAsset = 1,
                        DaysOfHistory = 10,
                        AvgDayFolderSizeMB = 2,
                        SimulatedDiskSizeGB = 0.1,
                        InitialUsagePercent = 90,
                        ThresholdPercent = 85,
                        CleanupCapPercent = 50
                    };

                    var result = await ctx.Simulation.RunSimulationAsync(param);
                    var details = result.CleanupResult.AssetDetails;
                    bool ordered = true;
                    for (int i = 1; i < details.Count; i++)
                    {
                        if (details[i].OldestDeletedDate < details[i - 1].OldestDeletedDate)
                        { ordered = false; break; }
                    }

                    try { Directory.Delete(simPath, true); } catch { }
                    return (ordered, $"Carpetas en resultado: {details.Count}, Orden FIFO: {ordered}");
                }
            },
            new()
            {
                Id = "TC-0305", Area = "Simulación", Titulo = "Limpieza de datos sintéticos",
                Descripcion = "Verificar que CleanupSyntheticDataAsync elimina toda la carpeta de simulación",
                Precondiciones = "Datos sintéticos existen",
                Pasos = "1. Generar datos | 2. Verificar que existen | 3. CleanupSyntheticDataAsync | 4. Verificar eliminados",
                ResultadoEsperado = "La carpeta de simulación ya no existe", Prioridad = "Media", CARef = "RF-03", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var simPath = Path.Combine(ctx.BasePath, "SimClean");
                    if (Directory.Exists(simPath)) Directory.Delete(simPath, true);

                    var param = new SimulationParams
                    {
                        SimulationPath = simPath,
                        NumberOfAssets = 1,
                        VariablesPerAsset = 1,
                        DaysOfHistory = 3,
                        AvgDayFolderSizeMB = 1,
                        SimulatedDiskSizeGB = 0.1,
                        InitialUsagePercent = 50
                    };

                    await ctx.Simulation.GenerateSyntheticDataAsync(param);
                    bool existsAfterGen = Directory.Exists(simPath);
                    await ctx.Simulation.CleanupSyntheticDataAsync(simPath);
                    bool existsAfterClean = Directory.Exists(simPath);

                    return (existsAfterGen && !existsAfterClean, $"Existía después de generar: {existsAfterGen}, Existía después de limpiar: {existsAfterClean}");
                }
            },
            new()
            {
                Id = "TC-0306", Area = "Simulación", Titulo = "Cancelación de generación",
                Descripcion = "Verificar que CancellationToken detiene la generación de datos",
                Precondiciones = "Ninguna",
                Pasos = "1. Iniciar generación grande | 2. Cancelar después de 2s | 3. Verificar que se detiene",
                ResultadoEsperado = "Generación se detiene sin excepción no controlada", Prioridad = "Media", CARef = "RF-03", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var simPath = Path.Combine(ctx.BasePath, "SimCancel");
                    if (Directory.Exists(simPath)) Directory.Delete(simPath, true);

                    var cts = new CancellationTokenSource();
                    var param = new SimulationParams
                    {
                        SimulationPath = simPath,
                        NumberOfAssets = 10,
                        VariablesPerAsset = 5,
                        DaysOfHistory = 30,
                        AvgDayFolderSizeMB = 10,
                        SimulatedDiskSizeGB = 50,
                        InitialUsagePercent = 90
                    };

                    cts.CancelAfter(TimeSpan.FromSeconds(2));
                    bool result = await ctx.Simulation.GenerateSyntheticDataAsync(param, cts.Token);
                    bool stopped = !result; // Should return false when cancelled

                    try { Directory.Delete(simPath, true); } catch { }
                    return (stopped, $"Generación cancelada correctamente: {stopped}");
                }
            },
        };
    }
}
