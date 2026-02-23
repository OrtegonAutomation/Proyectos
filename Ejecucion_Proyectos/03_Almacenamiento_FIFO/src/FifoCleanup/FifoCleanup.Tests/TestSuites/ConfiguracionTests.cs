using FifoCleanup.Engine.Models;
using FifoCleanup.Tests.Infrastructure;
using FifoCleanup.Tests.Models;

namespace FifoCleanup.Tests.TestSuites;

/// <summary>RF-02: Pruebas de Configuración y Política</summary>
public static class ConfiguracionTests
{
    public static List<TestCase> GetTests(TestContext ctx)
    {
        return new List<TestCase>
        {
            new()
            {
                Id = "TC-0201", Area = "Configuración", Titulo = "Guardar y cargar configuración JSON",
                Descripcion = "Verificar que la configuración se persiste y recupera correctamente en JSON",
                Precondiciones = "Ruta de configuración accesible",
                Pasos = "1. Crear config | 2. SaveAsync | 3. LoadAsync | 4. Comparar valores",
                ResultadoEsperado = "Todos los campos se preservan tras guardar/cargar", Prioridad = "Alta", CARef = "RF-02", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var config = new FifoConfiguration
                    {
                        StoragePath = ctx.DataPath,
                        MaxStorageSizeGB = 5.5,
                        ThresholdPercent = 87,
                        CleanupCapPercent = 25,
                        ScheduledFrequencyHours = 12,
                        ScheduledHour = 3,
                        PreventiveThresholdDays = 5,
                        MaxConcurrentAssets = 7,
                        MaxDaysToDeletePerAsset = 15,
                        BitacoraPath = ctx.BitacoraPath,
                        BitacoraRetentionDays = 180
                    };

                    var path = Path.Combine(ctx.BasePath, "test_config.json");
                    await ctx.Configuration.SaveAsync(config, path);
                    var loaded = await ctx.Configuration.LoadAsync(path);

                    bool ok = loaded.StoragePath == config.StoragePath
                        && Math.Abs(loaded.MaxStorageSizeGB - config.MaxStorageSizeGB) < 0.01
                        && Math.Abs(loaded.ThresholdPercent - config.ThresholdPercent) < 0.01
                        && loaded.CleanupCapPercent == config.CleanupCapPercent
                        && loaded.ScheduledFrequencyHours == config.ScheduledFrequencyHours
                        && loaded.PreventiveThresholdDays == config.PreventiveThresholdDays
                        && loaded.MaxConcurrentAssets == config.MaxConcurrentAssets
                        && loaded.MaxDaysToDeletePerAsset == config.MaxDaysToDeletePerAsset;

                    try { File.Delete(path); } catch { }
                    return (ok, ok ? "Todos los campos coinciden" : "Hay campos que no coinciden");
                }
            },
            new()
            {
                Id = "TC-0202", Area = "Configuración", Titulo = "Validación de rango ThresholdPercent (50-95)",
                Descripcion = "Verificar que la validación rechaza valores fuera del rango 50-95%",
                Precondiciones = "Ninguna",
                Pasos = "1. Crear config con ThresholdPercent=30 | 2. Validate | 3. Repetir con 96 | 4. Repetir con 85",
                ResultadoEsperado = "Valores 30 y 96 generan error; 85 es válido", Prioridad = "Alta", CARef = "RF-02", Tipo = "Funcional",
                TestAction = async () =>
                {
                    await Task.CompletedTask;
                    var config = ctx.Configuration.GetDefault();
                    config.StoragePath = ctx.DataPath;

                    config.ThresholdPercent = 30;
                    var err1 = ctx.Configuration.Validate(config);
                    bool reject30 = err1.Any(e => e.Contains("umbral", StringComparison.OrdinalIgnoreCase));

                    config.ThresholdPercent = 96;
                    var err2 = ctx.Configuration.Validate(config);
                    bool reject96 = err2.Any(e => e.Contains("umbral", StringComparison.OrdinalIgnoreCase));

                    config.ThresholdPercent = 85;
                    var err3 = ctx.Configuration.Validate(config);
                    bool accept85 = !err3.Any(e => e.Contains("umbral", StringComparison.OrdinalIgnoreCase));

                    bool ok = reject30 && reject96 && accept85;
                    return (ok, $"Rechaza 30%: {reject30}, Rechaza 96%: {reject96}, Acepta 85%: {accept85}");
                }
            },
            new()
            {
                Id = "TC-0203", Area = "Configuración", Titulo = "Validación CleanupCapPercent (5-50)",
                Descripcion = "Verificar validación de rango para el cap de limpieza",
                Precondiciones = "Ninguna",
                Pasos = "1. Validate con CleanupCapPercent=3 | 2. Validate con 51 | 3. Validate con 20",
                ResultadoEsperado = "3 y 51 generan error; 20 es válido", Prioridad = "Alta", CARef = "RF-02", Tipo = "Funcional",
                TestAction = async () =>
                {
                    await Task.CompletedTask;
                    var config = ctx.Configuration.GetDefault();
                    config.StoragePath = ctx.DataPath;

                    config.CleanupCapPercent = 3;
                    bool reject3 = ctx.Configuration.Validate(config).Any(e => e.Contains("cap", StringComparison.OrdinalIgnoreCase));

                    config.CleanupCapPercent = 51;
                    bool reject51 = ctx.Configuration.Validate(config).Any(e => e.Contains("cap", StringComparison.OrdinalIgnoreCase));

                    config.CleanupCapPercent = 20;
                    bool accept20 = !ctx.Configuration.Validate(config).Any(e => e.Contains("cap", StringComparison.OrdinalIgnoreCase));

                    bool ok = reject3 && reject51 && accept20;
                    return (ok, $"Rechaza 3%: {reject3}, Rechaza 51%: {reject51}, Acepta 20%: {accept20}");
                }
            },
            new()
            {
                Id = "TC-0204", Area = "Configuración", Titulo = "Validación ScheduledFrequencyHours (1-24)",
                Descripcion = "Verificar validación de rango para la frecuencia de RF-07",
                Precondiciones = "Ninguna",
                Pasos = "1. Validate con 0h | 2. Validate con 25h | 3. Validate con 12h",
                ResultadoEsperado = "0 y 25 generan error; 12 es válido", Prioridad = "Media", CARef = "RF-02", Tipo = "Funcional",
                TestAction = async () =>
                {
                    await Task.CompletedTask;
                    var config = ctx.Configuration.GetDefault();
                    config.StoragePath = ctx.DataPath;

                    config.ScheduledFrequencyHours = 0;
                    bool reject0 = ctx.Configuration.Validate(config).Any(e => e.Contains("frecuencia", StringComparison.OrdinalIgnoreCase));

                    config.ScheduledFrequencyHours = 25;
                    bool reject25 = ctx.Configuration.Validate(config).Any(e => e.Contains("frecuencia", StringComparison.OrdinalIgnoreCase));

                    config.ScheduledFrequencyHours = 12;
                    bool accept12 = !ctx.Configuration.Validate(config).Any(e => e.Contains("frecuencia", StringComparison.OrdinalIgnoreCase));

                    bool ok = reject0 && reject25 && accept12;
                    return (ok, $"Rechaza 0: {reject0}, Rechaza 25: {reject25}, Acepta 12: {accept12}");
                }
            },
            new()
            {
                Id = "TC-0205", Area = "Configuración", Titulo = "Validación PreventiveThresholdDays (1-10)",
                Descripcion = "Verificar validación del umbral preventivo RF-08",
                Precondiciones = "Ninguna",
                Pasos = "1. Validate con 0 | 2. Validate con 11 | 3. Validate con 3",
                ResultadoEsperado = "0 y 11 generan error; 3 es válido", Prioridad = "Media", CARef = "RF-02", Tipo = "Funcional",
                TestAction = async () =>
                {
                    await Task.CompletedTask;
                    var config = ctx.Configuration.GetDefault();
                    config.StoragePath = ctx.DataPath;

                    config.PreventiveThresholdDays = 0;
                    bool reject0 = ctx.Configuration.Validate(config).Any(e => e.Contains("preventivo", StringComparison.OrdinalIgnoreCase));

                    config.PreventiveThresholdDays = 11;
                    bool reject11 = ctx.Configuration.Validate(config).Any(e => e.Contains("preventivo", StringComparison.OrdinalIgnoreCase));

                    config.PreventiveThresholdDays = 3;
                    bool accept3 = !ctx.Configuration.Validate(config).Any(e => e.Contains("preventivo", StringComparison.OrdinalIgnoreCase));

                    bool ok = reject0 && reject11 && accept3;
                    return (ok, $"Rechaza 0: {reject0}, Rechaza 11: {reject11}, Acepta 3: {accept3}");
                }
            },
            new()
            {
                Id = "TC-0206", Area = "Configuración", Titulo = "Valores por defecto correctos",
                Descripcion = "Verificar que GetDefault() retorna valores dentro de rangos válidos",
                Precondiciones = "Ninguna",
                Pasos = "1. GetDefault() | 2. Verificar cada campo contra rangos",
                ResultadoEsperado = "Todos los valores por defecto están dentro de rangos válidos", Prioridad = "Media", CARef = "RF-02", Tipo = "Funcional",
                TestAction = async () =>
                {
                    await Task.CompletedTask;
                    var def = ctx.Configuration.GetDefault();
                    bool ok = def.ThresholdPercent >= 50 && def.ThresholdPercent <= 95
                        && def.CleanupCapPercent >= 5 && def.CleanupCapPercent <= 50
                        && def.ScheduledFrequencyHours >= 1 && def.ScheduledFrequencyHours <= 24
                        && def.PreventiveThresholdDays >= 1 && def.PreventiveThresholdDays <= 10
                        && def.MaxConcurrentAssets >= 1 && def.MaxConcurrentAssets <= 10;
                    return (ok, $"Threshold: {def.ThresholdPercent}%, Cap: {def.CleanupCapPercent}%, Freq: {def.ScheduledFrequencyHours}h, PrevDays: {def.PreventiveThresholdDays}");
                }
            },
            new()
            {
                Id = "TC-0207", Area = "Configuración", Titulo = "Cargar config inexistente retorna defaults",
                Descripcion = "Verificar que LoadAsync con archivo inexistente retorna configuración por defecto",
                Precondiciones = "Archivo no existe",
                Pasos = "1. LoadAsync con ruta inexistente | 2. Verificar retorna defaults",
                ResultadoEsperado = "Retorna FifoConfiguration con valores por defecto", Prioridad = "Media", CARef = "RF-02", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var loaded = await ctx.Configuration.LoadAsync(@"D:\FifoTestBed\noexiste_config.json");
                    var def = ctx.Configuration.GetDefault();
                    bool ok = Math.Abs(loaded.ThresholdPercent - def.ThresholdPercent) < 0.01;
                    return (ok, $"Loaded threshold: {loaded.ThresholdPercent}%, Default: {def.ThresholdPercent}%");
                }
            },
            new()
            {
                Id = "TC-0208", Area = "Configuración", Titulo = "MaxStorageSizeGB se persiste correctamente",
                Descripcion = "Verificar que el campo MaxStorageSizeGB se guarda y carga en JSON",
                Precondiciones = "Ninguna",
                Pasos = "1. Config con MaxStorageSizeGB=5 | 2. SaveAsync | 3. LoadAsync | 4. Verificar valor",
                ResultadoEsperado = "MaxStorageSizeGB = 5 tras cargar", Prioridad = "Alta", CARef = "RF-02", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var config = ctx.Configuration.GetDefault();
                    config.StoragePath = ctx.DataPath;
                    config.MaxStorageSizeGB = 5.0;

                    var path = Path.Combine(ctx.BasePath, "test_maxstorage.json");
                    await ctx.Configuration.SaveAsync(config, path);
                    var loaded = await ctx.Configuration.LoadAsync(path);

                    bool ok = Math.Abs(loaded.MaxStorageSizeGB - 5.0) < 0.01;
                    try { File.Delete(path); } catch { }
                    return (ok, $"MaxStorageSizeGB guardado: 5.0, cargado: {loaded.MaxStorageSizeGB}");
                }
            },
        };
    }
}
