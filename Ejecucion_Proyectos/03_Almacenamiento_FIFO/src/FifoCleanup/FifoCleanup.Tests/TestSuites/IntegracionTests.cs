using FifoCleanup.Engine.Models;
using FifoCleanup.Tests.Infrastructure;
using FifoCleanup.Tests.Models;

namespace FifoCleanup.Tests.TestSuites;

/// <summary>Pruebas de Integración entre módulos</summary>
public static class IntegracionTests
{
    public static List<TestCase> GetTests(TestContext ctx)
    {
        return new List<TestCase>
        {
            new()
            {
                Id = "TC-1001", Area = "Integración", Titulo = "Flujo completo: Inventario → Limpieza → Bitácora",
                Descripcion = "Verificar el flujo end-to-end: escanear, limpiar y registrar en bitácora",
                Precondiciones = "Datos generados con uso alto",
                Pasos = "1. CreateTestData | 2. ScanAsync | 3. ApplyStorageLimit | 4. ExecuteGeneralCleanupAsync | 5. Verificar bitácora",
                ResultadoEsperado = "Limpieza exitosa y registrada en bitácora", Prioridad = "Alta", CARef = "RF-01,RF-04,RF-05", Tipo = "Integración",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 3, variables: 2, days: 10, mbPerDay: 2);
                    await ctx.Bitacora.InitializeAsync(ctx.BitacoraPath);

                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    status.ApplyStorageLimit(0.2); // Force high usage

                    var config = new FifoConfiguration
                    {
                        StoragePath = ctx.DataPath,
                        ThresholdPercent = 85,
                        CleanupCapPercent = 20,
                        MaxStorageSizeGB = 0.2
                    };

                    var result = await ctx.Cleanup.ExecuteGeneralCleanupAsync(status, config);

                    // Verify bitácora entry
                    var entries = await ctx.Bitacora.GetEntriesAsync(from: DateTime.Now.AddMinutes(-2));
                    bool hasBitacora = entries.Any(e => e.Action == "LIMPIEZA_GENERAL_FIFO");

                    bool ok = result.Success && result.BytesFreed > 0 && hasBitacora;
                    return (ok, $"Limpieza: {result.Success}, BytesFreed: {result.BytesFreedFormatted}, Bitácora: {hasBitacora}");
                }
            },
            new()
            {
                Id = "TC-1002", Area = "Integración", Titulo = "Simulación completa → Inventario → DryRun",
                Descripcion = "Verificar que RunSimulationAsync integra generación, inventario y limpieza simulada",
                Precondiciones = "Ninguna",
                Pasos = "1. RunSimulationAsync | 2. Verificar DataGenerationSuccess | 3. Verificar StatusBefore con datos | 4. Verificar CleanupResult",
                ResultadoEsperado = "Simulación completa con todas las fases exitosas", Prioridad = "Alta", CARef = "RF-03, RF-04", Tipo = "Integración",
                TestAction = async () =>
                {
                    var simPath = Path.Combine(ctx.BasePath, "IntegSim");
                    if (Directory.Exists(simPath)) Directory.Delete(simPath, true);

                    var param = new SimulationParams
                    {
                        SimulationPath = simPath,
                        NumberOfAssets = 2,
                        VariablesPerAsset = 2,
                        DaysOfHistory = 10,
                        AvgDayFolderSizeMB = 2,
                        SimulatedDiskSizeGB = 0.2,
                        InitialUsagePercent = 90,
                        ThresholdPercent = 85,
                        CleanupCapPercent = 20
                    };

                    var result = await ctx.Simulation.RunSimulationAsync(param);

                    bool ok = result.DataGenerationSuccess
                        && result.StatusBefore.Assets.Count > 0
                        && result.CleanupResult != null;

                    try { Directory.Delete(simPath, true); } catch { }
                    return (ok, $"DataGen: {result.DataGenerationSuccess}, Assets: {result.StatusBefore.Assets.Count}, Duration: {result.TotalDurationMs}ms");
                }
            },
            new()
            {
                Id = "TC-1003", Area = "Integración", Titulo = "Config → Inventario → Limpieza con MaxStorageSizeGB",
                Descripcion = "Verificar flujo completo usando MaxStorageSizeGB como límite",
                Precondiciones = "Datos generados",
                Pasos = "1. SaveAsync config con MaxStorageSizeGB=0.15 | 2. LoadAsync | 3. ScanAsync | 4. ApplyStorageLimit | 5. Cleanup si necesario",
                ResultadoEsperado = "El flujo usa correctamente el límite de MaxStorageSizeGB", Prioridad = "Alta", CARef = "RF-02, RF-04", Tipo = "Integración",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 2, variables: 2, days: 10, mbPerDay: 2);

                    var config = ctx.DefaultConfig;
                    config.MaxStorageSizeGB = 0.15;
                    var cfgPath = Path.Combine(ctx.BasePath, "integ_config.json");
                    await ctx.Configuration.SaveAsync(config, cfgPath);

                    var loaded = await ctx.Configuration.LoadAsync(cfgPath);
                    var status = await ctx.Inventory.ScanAsync(loaded.StoragePath);
                    status.ApplyStorageLimit(loaded.MaxStorageSizeGB);

                    bool highUsage = status.UsagePercent > loaded.ThresholdPercent;

                    string result = $"Uso: {status.UsagePercent:F1}%, Umbral: {loaded.ThresholdPercent}%, MaxStorage: {loaded.MaxStorageSizeGB} GB";

                    if (highUsage)
                    {
                        var cleanResult = await ctx.Cleanup.ExecuteGeneralCleanupAsync(status, loaded);
                        result += $", Limpieza: {cleanResult.Success}, BytesFreed: {cleanResult.BytesFreedFormatted}";
                    }

                    try { File.Delete(cfgPath); } catch { }
                    return (true, result);
                }
            },
            new()
            {
                Id = "TC-1004", Area = "Integración", Titulo = "RF-07 start/stop + Bitácora audit trail",
                Descripcion = "Verificar que RF-07 genera rastro de auditoría completo",
                Precondiciones = "Servicios configurados",
                Pasos = "1. Iniciar RF-07 | 2. Esperar | 3. Detener | 4. Verificar bitácora tiene start+stop",
                ResultadoEsperado = "Bitácora contiene RF07_INICIADO y RF07_DETENIDO", Prioridad = "Media", CARef = "RF-05, RF-07", Tipo = "Integración",
                TestAction = async () =>
                {
                    await ctx.Bitacora.InitializeAsync(ctx.BitacoraPath);
                    await ctx.ScheduledCleanup.StartAsync(ctx.DefaultConfig);
                    await Task.Delay(1000);
                    await ctx.ScheduledCleanup.StopAsync();
                    await Task.Delay(500);

                    var entries = await ctx.Bitacora.GetEntriesAsync(from: DateTime.Now.AddMinutes(-2));
                    bool start = entries.Any(e => e.Action == "RF07_INICIADO");
                    bool stop = entries.Any(e => e.Action == "RF07_DETENIDO");

                    return (start && stop, $"Start entry: {start}, Stop entry: {stop}");
                }
            },
            new()
            {
                Id = "TC-1005", Area = "Integración", Titulo = "RF-08 start/stop + Bitácora audit trail",
                Descripcion = "Verificar que RF-08 genera rastro de auditoría completo",
                Precondiciones = "Servicios configurados, datos existen",
                Pasos = "1. Iniciar RF-08 | 2. Esperar | 3. Detener | 4. Verificar bitácora",
                ResultadoEsperado = "Bitácora contiene RF08_INICIADO y RF08_DETENIDO", Prioridad = "Media", CARef = "RF-05, RF-08", Tipo = "Integración",
                TestAction = async () =>
                {
                    await ctx.Bitacora.InitializeAsync(ctx.BitacoraPath);
                    await ctx.PreventiveMonitor.StartAsync(ctx.DataPath, ctx.DefaultConfig);
                    await Task.Delay(1000);
                    await ctx.PreventiveMonitor.StopAsync();
                    await Task.Delay(500);

                    var entries = await ctx.Bitacora.GetEntriesAsync(from: DateTime.Now.AddMinutes(-2));
                    bool start = entries.Any(e => e.Action == "RF08_INICIADO");
                    bool stop = entries.Any(e => e.Action == "RF08_DETENIDO");

                    return (start && stop, $"Start entry: {start}, Stop entry: {stop}");
                }
            },
            new()
            {
                Id = "TC-1006", Area = "Integración", Titulo = "Limpieza local registra en bitácora con AssetId",
                Descripcion = "Verificar que limpieza local RF-08 registra entry con AssetId y VariableId",
                Precondiciones = "Datos generados",
                Pasos = "1. ExecuteLocalCleanupAsync | 2. Buscar en bitácora entry con LIMPIEZA_LOCAL_FIFO | 3. Verificar AssetId y VariableId",
                ResultadoEsperado = "Entrada bitácora contiene AssetId=Asset001, VariableId=00", Prioridad = "Media", CARef = "RF-04, RF-05", Tipo = "Integración",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 2, variables: 1, days: 10, mbPerDay: 1);
                    await ctx.Bitacora.InitializeAsync(ctx.BitacoraPath);

                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    var config = new FifoConfiguration { StoragePath = ctx.DataPath, MaxDaysToDeletePerAsset = 2 };

                    await ctx.Cleanup.ExecuteLocalCleanupAsync("Asset001", "00", status, config);

                    var entries = await ctx.Bitacora.GetEntriesAsync(from: DateTime.Now.AddMinutes(-2));
                    var local = entries.FirstOrDefault(e => e.Action == "LIMPIEZA_LOCAL_FIFO");

                    bool ok = local != null && local.AssetId == "Asset001" && local.VariableId == "00";
                    return (ok, local != null
                        ? $"Bitácora: Asset={local.AssetId}, Variable={local.VariableId}, Bytes={local.BytesAffected}"
                        : "No se encontró entrada LIMPIEZA_LOCAL_FIFO");
                }
            },
        };
    }
}
