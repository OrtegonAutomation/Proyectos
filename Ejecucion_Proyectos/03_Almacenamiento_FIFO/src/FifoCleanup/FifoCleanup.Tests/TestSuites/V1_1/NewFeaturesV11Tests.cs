using System.Reflection;
using FifoCleanup.Engine.Models;
using FifoCleanup.Engine.Services;
using FifoCleanup.Tests.Infrastructure;
using FifoCleanup.Tests.Models;

namespace FifoCleanup.Tests.TestSuites.V1_1;

/// <summary>v1.1: pruebas enfocadas solo en funcionalidades nuevas.</summary>
public static class NewFeaturesV11Tests
{
    public static List<TestCase> GetTests(TestContext ctx)
    {
        return new List<TestCase>
        {
            new()
            {
                Id = "TC-11-01",
                Area = "v1.1-Reinicio",
                Titulo = "Reactivación automática de RF-07 y RF-08 tras reinicio simulado",
                Descripcion = "Simula stop/start de servicios para validar que ambos vuelven a estado running.",
                Precondiciones = "Configuración válida y datos base creados.",
                Pasos = "1) Start RF07/RF08 2) Stop ambos 3) Start ambos 4) Validar IsRunning=true",
                ResultadoEsperado = "RF-07 y RF-08 quedan activos tras reinicio simulado.",
                Prioridad = "Alta",
                CARef = "v1.1-restart",
                Tipo = "Funcional",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 1, variables: 1, days: 3, mbPerDay: 1);

                    var config = CreateMonitoringOnlyConfig(ctx);

                    await ctx.ScheduledCleanup.StartAsync(config);
                    await ctx.PreventiveMonitor.StartAsync(ctx.DataPath, config);

                    await ctx.ScheduledCleanup.StopAsync();
                    await ctx.PreventiveMonitor.StopAsync();

                    await ctx.ScheduledCleanup.StartAsync(config);
                    await ctx.PreventiveMonitor.StartAsync(ctx.DataPath, config);

                    bool ok = ctx.ScheduledCleanup.IsRunning && ctx.PreventiveMonitor.IsRunning;

                    await ctx.ScheduledCleanup.StopAsync();
                    await ctx.PreventiveMonitor.StopAsync();

                    return (ok, $"RF07={ctx.ScheduledCleanup.IsRunning}, RF08={ctx.PreventiveMonitor.IsRunning} (verificado antes de stop final)");
                }
            },
            new()
            {
                Id = "TC-11-02",
                Area = "v1.1-Monitoreo",
                Titulo = "RF-08 monitorea en tiempo real sin ejecutar eliminación",
                Descripcion = "Valida detección de archivos en tiempo real y que no haya limpieza en modo monitoreo.",
                Precondiciones = "RF-08 activo y umbral alto para evitar limpieza.",
                Pasos = "1) Start RF08 2) Crear archivo trigger 3) Verificar OnFileDetected 4) PreventiveCleanups=0",
                ResultadoEsperado = "Detecta eventos en tiempo real sin limpiar.",
                Prioridad = "Alta",
                CARef = "v1.1-rf08-monitor-only",
                Tipo = "Funcional",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 1, variables: 1, days: 2, mbPerDay: 1);

                    var config = CreateMonitoringOnlyConfig(ctx);
                    bool detected = false;

                    ctx.PreventiveMonitor.OnFileDetected += (_, _) => detected = true;
                    await ctx.PreventiveMonitor.StartAsync(ctx.DataPath, config);

                    var now = DateTime.Now;
                    var triggerPath = Path.Combine(ctx.DataPath, "Asset001", "00", "E",
                        now.Year.ToString("D4"), now.Month.ToString("D2"), now.Day.ToString("D2"));
                    Directory.CreateDirectory(triggerPath);
                    await File.WriteAllBytesAsync(Path.Combine(triggerPath, "v11_trigger.bin"), new byte[1024]);

                    await Task.Delay(TimeSpan.FromSeconds(3));
                    int cleanups = ctx.PreventiveMonitor.PreventiveCleanups;

                    await ctx.PreventiveMonitor.StopAsync();

                    bool ok = detected && cleanups == 0;
                    return (ok, $"Detectado={detected}, PreventiveCleanups={cleanups}");
                }
            },
            new()
            {
                Id = "TC-11-03",
                Area = "v1.1-Monitoreo",
                Titulo = "RF-07 reactivado entra en ventana de gracia sin limpieza inmediata",
                Descripcion = "Ejecuta evaluación RF-07 por reflexión y valida modo monitoreo-only al arranque.",
                Precondiciones = "Config con startupMonitoringGraceMinutes > 0.",
                Pasos = "1) Start RF07 2) Invocar EvaluateAndCleanupAsync 3) Verificar no cleanup y bitácora de gracia",
                ResultadoEsperado = "No cleanup inmediato y bitácora RF07_STARTUP_MONITORING_ONLY.",
                Prioridad = "Alta",
                CARef = "v1.1-rf07-monitor-only",
                Tipo = "Funcional",
                TestAction = async () =>
                {
                    ctx.CleanTestData();
                    await ctx.CreateTestDataAsync(assets: 1, variables: 1, days: 3, mbPerDay: 1);

                    var config = CreateMonitoringOnlyConfig(ctx);
                    bool cleanupExecuted = false;
                    ctx.ScheduledCleanup.OnCleanupExecuted += (_, _) => cleanupExecuted = true;

                    await ctx.ScheduledCleanup.StartAsync(config);
                    await ctx.Bitacora.InitializeAsync(ctx.BitacoraPath);

                    var method = ctx.ScheduledCleanup.GetType().GetMethod("EvaluateAndCleanupAsync", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (method == null)
                    {
                        await ctx.ScheduledCleanup.StopAsync();
                        return (false, "No se encontró método interno EvaluateAndCleanupAsync.");
                    }

                    var task = method.Invoke(ctx.ScheduledCleanup, new object[] { CancellationToken.None }) as Task;
                    if (task != null)
                        await task;

                    var entries = await ctx.Bitacora.GetEntriesAsync(from: DateTime.Now.AddMinutes(-2));
                    bool hasStartupGrace = entries.Any(e => e.Action == "RF07_STARTUP_MONITORING_ONLY");

                    await ctx.ScheduledCleanup.StopAsync();

                    return (!cleanupExecuted && hasStartupGrace, $"OnCleanupExecuted={cleanupExecuted}, StartupGraceLog={hasStartupGrace}");
                }
            },
            new()
            {
                Id = "TC-11-04",
                Area = "v1.1-Alertas",
                Titulo = "Alertas email no rompen ejecución en entorno sin internet",
                Descripcion = "Valida que helper de correo falle controladamente con SMTP inválido.",
                Precondiciones = "Servidor sin salida a internet o host SMTP inválido.",
                Pasos = "1) Configurar SMTP inválido 2) TrySendCriticalAlertAsync 3) Validar false sin excepción",
                ResultadoEsperado = "Retorna false sin afectar el proceso.",
                Prioridad = "Alta",
                CARef = "v1.1-email-resilience",
                Tipo = "No Funcional",
                TestAction = async () =>
                {
                    var cfg = CreateMonitoringOnlyConfig(ctx);
                    cfg.EnableEmailAlerts = true;
                    cfg.SmtpHost = "smtp.inexistente.local";
                    cfg.SmtpFrom = "noreply@odl.local";
                    cfg.SmtpUser = "noreply@odl.local";
                    cfg.SmtpPassword = "dummy";
                    cfg.AlertEmailTo = "camilo.ortegonc@outlook.com";

                    var sent = await EmailAlertHelper.TrySendCriticalAlertAsync(cfg, "test", "test");
                    return (!sent, "Envío falló controladamente (esperado en entorno sin internet/relay).");
                }
            }
        };
    }

    private static FifoConfiguration CreateMonitoringOnlyConfig(TestContext ctx) => new()
    {
        StoragePath = ctx.DataPath,
        MaxStorageSizeGB = 10,
        ThresholdPercent = 95,
        CleanupCapPercent = 5,
        ScheduledFrequencyHours = 1,
        ScheduledHour = DateTime.Now.Hour,
        PreventiveThresholdDays = 1,
        EnableScheduledCleanup = true,
        EnablePreventiveCleanup = true,
        MaxConcurrentAssets = 2,
        MaxDaysToDeletePerAsset = 1,
        ConfigFilePath = ctx.ConfigPath,
        BitacoraPath = ctx.BitacoraPath,
        BitacoraRetentionDays = 30,
        BitacoraMaxSizeMB = 50,
        EventBatchIntervalSeconds = 5,
        UseLowPriorityThreads = true,
        DeleteThrottleMs = 10,
        EnableEmailAlerts = false,
        StartupMonitoringGraceMinutes = 10
    };
}
