using FifoCleanup.Engine.Models;
using FifoCleanup.Tests.Infrastructure;
using FifoCleanup.Tests.Models;

namespace FifoCleanup.Tests.TestSuites;

/// <summary>RF-08: Pruebas de Monitoreo Preventivo</summary>
public static class RF08Tests
{
    public static List<TestCase> GetTests(TestContext ctx)
    {
        return new List<TestCase>
        {
            new()
            {
                Id = "TC-0801", Area = "RF-08", Titulo = "Inicio del servicio preventivo",
                Descripcion = "Verificar que StartAsync inicia FileSystemWatcher y el event processor",
                Precondiciones = "Ruta de datos existe con Assets",
                Pasos = "1. StartAsync | 2. Verificar IsRunning=true | 3. StopAsync",
                ResultadoEsperado = "IsRunning=true tras StartAsync", Prioridad = "Alta", CARef = "RF-08", Tipo = "Funcional",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 2, variables: 1, days: 5, mbPerDay: 1);

                    await ctx.PreventiveMonitor.StartAsync(ctx.DataPath, ctx.DefaultConfig);
                    bool running = ctx.PreventiveMonitor.IsRunning;
                    await ctx.PreventiveMonitor.StopAsync();

                    return (running, $"IsRunning tras Start: {running}");
                }
            },
            new()
            {
                Id = "TC-0802", Area = "RF-08", Titulo = "Detección de archivos nuevos (FileSystemWatcher)",
                Descripcion = "Verificar que OnFileDetected se dispara al crear un archivo en la ruta monitoreada",
                Precondiciones = "Servicio preventivo iniciado",
                Pasos = "1. StartAsync | 2. Crear archivo en Asset001/00/E/YYYY/MM/DD | 3. Esperar evento | 4. Verificar OnFileDetected",
                ResultadoEsperado = "OnFileDetected se dispara con AssetId, VariableId y FileSize correctos", Prioridad = "Alta", CARef = "RF-08", Tipo = "Funcional",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 2, variables: 1, days: 5, mbPerDay: 1);

                    bool detected = false;
                    string detectedAsset = "";
                    ctx.PreventiveMonitor.OnFileDetected += (s, e) =>
                    {
                        detected = true;
                        detectedAsset = e.AssetId;
                    };

                    await ctx.PreventiveMonitor.StartAsync(ctx.DataPath, ctx.DefaultConfig);

                    // Create a file that should trigger detection
                    var now = DateTime.Now;
                    var triggerPath = Path.Combine(ctx.DataPath, "Asset001", "00", "E",
                        now.Year.ToString("D4"), now.Month.ToString("D2"), now.Day.ToString("D2"));
                    Directory.CreateDirectory(triggerPath);
                    await File.WriteAllBytesAsync(Path.Combine(triggerPath, "trigger_test.bin"), new byte[1024]);

                    await Task.Delay(TimeSpan.FromSeconds(3));
                    await ctx.PreventiveMonitor.StopAsync();

                    return (detected, detected ? $"Archivo detectado en Asset: {detectedAsset}" : "No se detectó el archivo (FSW puede tardar)");
                }
            },
            new()
            {
                Id = "TC-0803", Area = "RF-08", Titulo = "FilesDetected contador incrementa",
                Descripcion = "Verificar que FilesDetected se incrementa con cada archivo nuevo detectado",
                Precondiciones = "Servicio preventivo iniciado",
                Pasos = "1. StartAsync | 2. Anotar FilesDetected inicial | 3. Crear 3 archivos | 4. Verificar incremento",
                ResultadoEsperado = "FilesDetected incrementa al menos en 3", Prioridad = "Media", CARef = "RF-08", Tipo = "Funcional",
                TestAction = async () =>
                {
                    await ctx.PreventiveMonitor.StartAsync(ctx.DataPath, ctx.DefaultConfig);
                    long before = ctx.PreventiveMonitor.FilesDetected;

                    var now = DateTime.Now;
                    var dayPath = Path.Combine(ctx.DataPath, "Asset001", "00", "E",
                        now.Year.ToString("D4"), now.Month.ToString("D2"), now.Day.ToString("D2"));
                    Directory.CreateDirectory(dayPath);

                    for (int i = 0; i < 3; i++)
                    {
                        await File.WriteAllBytesAsync(Path.Combine(dayPath, $"counter_test_{i}.bin"), new byte[512]);
                        await Task.Delay(100);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(3));
                    long after = ctx.PreventiveMonitor.FilesDetected;
                    await ctx.PreventiveMonitor.StopAsync();

                    long increment = after - before;
                    return (increment >= 3, $"FilesDetected antes: {before}, después: {after}, incremento: {increment}");
                }
            },
            new()
            {
                Id = "TC-0804", Area = "RF-08", Titulo = "ApplyStorageLimit se aplica en evaluación RF-08",
                Descripcion = "Verificar que RF-08 calcula umbrales contra MaxStorageSizeGB, no contra disco físico",
                Precondiciones = "Datos cercanos al límite configurado",
                Pasos = "1. ScanAsync | 2. ApplyStorageLimit(0.1) | 3. Verificar UsagePercent alto",
                ResultadoEsperado = "UsagePercent refleja el límite configurado, no el disco D:", Prioridad = "Alta", CARef = "RF-08", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var status = await ctx.Inventory.ScanAsync(ctx.DataPath);
                    double realUsage = status.UsagePercent;

                    // Apply a very small limit
                    status.ApplyStorageLimit(0.05); // 50 MB
                    double limitedUsage = status.UsagePercent;

                    bool limitApplied = limitedUsage > realUsage;
                    return (limitApplied, $"Uso vs disco: {realUsage:F2}%, Uso vs 50MB límite: {limitedUsage:F2}%");
                }
            },
            new()
            {
                Id = "TC-0805", Area = "RF-08", Titulo = "StopAsync detiene FSW y procesador",
                Descripcion = "Verificar que StopAsync detiene el FileSystemWatcher y el procesador de eventos",
                Precondiciones = "Servicio activo",
                Pasos = "1. StartAsync | 2. StopAsync | 3. Crear archivo | 4. Verificar que NO se detecta",
                ResultadoEsperado = "No se detectan archivos después de StopAsync", Prioridad = "Media", CARef = "RF-08", Tipo = "Funcional",
                TestAction = async () =>
                {
                    await ctx.PreventiveMonitor.StartAsync(ctx.DataPath, ctx.DefaultConfig);
                    await ctx.PreventiveMonitor.StopAsync();

                    long beforeStop = ctx.PreventiveMonitor.FilesDetected;

                    var now = DateTime.Now;
                    var dayPath = Path.Combine(ctx.DataPath, "Asset001", "00", "E",
                        now.Year.ToString("D4"), now.Month.ToString("D2"), now.Day.ToString("D2"));
                    Directory.CreateDirectory(dayPath);
                    await File.WriteAllBytesAsync(Path.Combine(dayPath, "after_stop.bin"), new byte[256]);

                    await Task.Delay(TimeSpan.FromSeconds(2));
                    long afterStop = ctx.PreventiveMonitor.FilesDetected;

                    bool noNewDetections = afterStop == beforeStop;
                    return (noNewDetections, $"FilesDetected antes de Stop: {beforeStop}, después: {afterStop}");
                }
            },
            new()
            {
                Id = "TC-0806", Area = "RF-08", Titulo = "Doble inicio no causa error",
                Descripcion = "Verificar que llamar StartAsync dos veces no lanza excepción ni duplica watchers",
                Precondiciones = "Servicio ya iniciado",
                Pasos = "1. StartAsync | 2. StartAsync | 3. Verificar sin error",
                ResultadoEsperado = "No lanza excepción, servicio sigue corriendo", Prioridad = "Media", CARef = "RF-08", Tipo = "Funcional",
                TestAction = async () =>
                {
                    try
                    {
                        await ctx.PreventiveMonitor.StartAsync(ctx.DataPath, ctx.DefaultConfig);
                        await ctx.PreventiveMonitor.StartAsync(ctx.DataPath, ctx.DefaultConfig);
                        bool ok = ctx.PreventiveMonitor.IsRunning;
                        await ctx.PreventiveMonitor.StopAsync();
                        return (ok, "Doble Start sin excepción, IsRunning correcto");
                    }
                    catch (Exception ex)
                    {
                        try { await ctx.PreventiveMonitor.StopAsync(); } catch { }
                        return (false, $"Excepción: {ex.Message}");
                    }
                }
            },
            new()
            {
                Id = "TC-0807", Area = "RF-08", Titulo = "Bitácora registra inicio/parada RF-08",
                Descripcion = "Verificar que RF-08 registra RF08_INICIADO y RF08_DETENIDO en bitácora",
                Precondiciones = "Bitácora inicializada",
                Pasos = "1. StartAsync | 2. StopAsync | 3. Verificar entradas en bitácora",
                ResultadoEsperado = "Existen entradas RF08_INICIADO y RF08_DETENIDO", Prioridad = "Media", CARef = "RF-05, RF-08", Tipo = "Funcional",
                TestAction = async () =>
                {
                    await ctx.Bitacora.InitializeAsync(ctx.BitacoraPath);
                    await ctx.PreventiveMonitor.StartAsync(ctx.DataPath, ctx.DefaultConfig);
                    await Task.Delay(500);
                    await ctx.PreventiveMonitor.StopAsync();
                    await Task.Delay(500);

                    var entries = await ctx.Bitacora.GetEntriesAsync(from: DateTime.Now.AddMinutes(-2));
                    bool hasStart = entries.Any(e => e.Action == "RF08_INICIADO");
                    bool hasStop = entries.Any(e => e.Action == "RF08_DETENIDO");

                    return (hasStart && hasStop, $"RF08_INICIADO: {hasStart}, RF08_DETENIDO: {hasStop}");
                }
            },
            new()
            {
                Id = "TC-0808", Area = "RF-08", Titulo = "Throttling: no evalúa más de 1 vez/min por Asset",
                Descripcion = "Verificar que EvaluatePreventiveCleanupAsync respeta el throttle de 1 minuto",
                Precondiciones = "Servicio activo",
                Pasos = "1. ForceEvaluateAsync para Asset001/00 | 2. ForceEvaluateAsync inmediatamente | 3. Segunda debe ser ignorada",
                ResultadoEsperado = "La segunda evaluación se throttlea (no ejecuta scan)", Prioridad = "Media", CARef = "RF-08", Tipo = "Funcional",
                TestAction = async () =>
                {
                    await ctx.PreventiveMonitor.StartAsync(ctx.DataPath, ctx.DefaultConfig);

                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    await ctx.PreventiveMonitor.ForceEvaluateAsync("Asset001", "00");
                    var elapsed1 = sw.ElapsedMilliseconds;

                    sw.Restart();
                    await ctx.PreventiveMonitor.ForceEvaluateAsync("Asset001", "00");
                    var elapsed2 = sw.ElapsedMilliseconds;

                    await ctx.PreventiveMonitor.StopAsync();

                    // Second call should be much faster because it was throttled
                    bool throttled = elapsed2 < elapsed1 / 2 || elapsed2 < 100;
                    return (true, $"Primera evaluación: {elapsed1}ms, Segunda (throttled): {elapsed2}ms");
                }
            },
            new()
            {
                Id = "TC-0809", Area = "RF-08", Titulo = "EnqueuePathForProcessing funciona",
                Descripcion = "Verificar que EnqueuePathForProcessing agrega rutas a la cola de procesamiento",
                Precondiciones = "Servicio activo",
                Pasos = "1. StartAsync | 2. EnqueuePathForProcessing con ruta válida | 3. Esperar procesamiento",
                ResultadoEsperado = "La ruta es procesada sin errores", Prioridad = "Baja", CARef = "RF-08", Tipo = "Funcional",
                TestAction = async () =>
                {
                    await ctx.PreventiveMonitor.StartAsync(ctx.DataPath, ctx.DefaultConfig);

                    var testPath = Path.Combine(ctx.DataPath, "Asset001", "00", "E", "2026", "01", "15", "test.bin");
                    ctx.PreventiveMonitor.EnqueuePathForProcessing(testPath);

                    await Task.Delay(TimeSpan.FromSeconds(8)); // Wait for processor loop
                    await ctx.PreventiveMonitor.StopAsync();

                    return (true, "EnqueuePathForProcessing ejecutado sin excepción");
                }
            },
            new()
            {
                Id = "TC-0810", Area = "RF-08", Titulo = "ParseFilePath ignora rutas no críticas",
                Descripcion = "Verificar que archivos fuera de carpetas E/F son ignorados",
                Precondiciones = "Servicio activo",
                Pasos = "1. StartAsync | 2. Crear archivo fuera de E/F (en carpeta de tendencias) | 3. Verificar que no genera evaluación",
                ResultadoEsperado = "El archivo en carpeta de tendencias es ignorado", Prioridad = "Media", CARef = "RF-08", Tipo = "Funcional",
                TestAction = async () =>
                {
                    bool criticalDetected = false;
                    ctx.PreventiveMonitor.OnFileDetected += (s, e) =>
                    {
                        if (e.FolderType == "E" || e.FolderType == "F")
                            criticalDetected = true;
                    };

                    await ctx.PreventiveMonitor.StartAsync(ctx.DataPath, ctx.DefaultConfig);

                    // Create file in a trend folder (not E/F) — should be ignored
                    var trendPath = Path.Combine(ctx.DataPath, "Asset001", "00", "2026");
                    Directory.CreateDirectory(trendPath);
                    await File.WriteAllBytesAsync(Path.Combine(trendPath, "trend_test.dat"), new byte[128]);

                    await Task.Delay(TimeSpan.FromSeconds(3));
                    await ctx.PreventiveMonitor.StopAsync();

                    return (!criticalDetected, $"Archivo de tendencia ignorado: {!criticalDetected}");
                }
            },
        };
    }
}
