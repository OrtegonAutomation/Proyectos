using FifoCleanup.Tests.Infrastructure;
using FifoCleanup.Tests.Models;

namespace FifoCleanup.Tests.TestSuites;

/// <summary>Features no implementados o solo de UI: Alarmas, Seguridad, Usabilidad</summary>
public static class PendingFeatureTests
{
    public static List<TestCase> GetTests(TestContext ctx)
    {
        return new List<TestCase>
        {
            // ═══════ ALARMAS (RF-06) ═══════
            new()
            {
                Id = "TC-1201", Area = "Alarmas", Titulo = "Alarma al superar umbral de uso",
                Descripcion = "Verificar que se genera alarma cuando UsagePercent > ThresholdPercent",
                Precondiciones = "RF-06 Alarmas implementado", Pasos = "N/A", ResultadoEsperado = "Alarma disparada",
                Prioridad = "Media", CARef = "RF-06", Tipo = "Funcional"
            },
            new()
            {
                Id = "TC-1202", Area = "Alarmas", Titulo = "Notificación por email/syslog",
                Descripcion = "Verificar que la alarma se envía por canal configurado",
                Precondiciones = "RF-06 canales implementados", Pasos = "N/A", ResultadoEsperado = "Notificación enviada",
                Prioridad = "Baja", CARef = "RF-06", Tipo = "Funcional"
            },

            // ═══════ SEGURIDAD (RF-09) ═══════
            new()
            {
                Id = "TC-1301", Area = "Seguridad", Titulo = "Permisos de escritura en ruta de datos",
                Descripcion = "Verificar que la aplicación tiene permisos de escritura en D:\\FifoTestBed",
                Precondiciones = "D:\\FifoTestBed existe", Pasos = "1. Intentar crear archivo", ResultadoEsperado = "Archivo creado exitosamente",
                Prioridad = "Alta", CARef = "RF-09", Tipo = "Seguridad",
                TestAction = async () =>
                {
                    var testFile = Path.Combine(ctx.BasePath, "perm_test.tmp");
                    try
                    {
                        await File.WriteAllBytesAsync(testFile, new byte[64]);
                        File.Delete(testFile);
                        return (true, "Permisos de escritura confirmados en D:\\FifoTestBed");
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        return (false, $"Sin permisos: {ex.Message}");
                    }
                }
            },
            new()
            {
                Id = "TC-1302", Area = "Seguridad", Titulo = "Bitácora inmutable (append-only)",
                Descripcion = "Verificar que la bitácora solo añade líneas, no modifica existentes",
                Precondiciones = "Bitácora con entradas",
                Pasos = "1. Leer contenido | 2. LogAsync nueva entry | 3. Verificar que contenido anterior intacto",
                ResultadoEsperado = "Las líneas anteriores no fueron modificadas", Prioridad = "Alta", CARef = "RF-05, RF-09", Tipo = "Seguridad",
                TestAction = async () =>
                {
                    var testBitPath = Path.Combine(ctx.BasePath, "BitacoraImmutable");
                    if (Directory.Exists(testBitPath)) Directory.Delete(testBitPath, true);
                    await ctx.Bitacora.InitializeAsync(testBitPath);

                    await ctx.Bitacora.LogAsync(new Engine.Models.BitacoraEntry
                    {
                        EventType = Engine.Models.BitacoraEventType.Information,
                        Action = "IMMUTABLE_LINE_1"
                    });

                    var files = Directory.GetFiles(testBitPath, "bitacora_*.csv");
                    string contentBefore = files.Length > 0 ? await File.ReadAllTextAsync(files[0]) : "";

                    await ctx.Bitacora.LogAsync(new Engine.Models.BitacoraEntry
                    {
                        EventType = Engine.Models.BitacoraEventType.Information,
                        Action = "IMMUTABLE_LINE_2"
                    });

                    string contentAfter = files.Length > 0 ? await File.ReadAllTextAsync(files[0]) : "";

                    bool preserved = contentAfter.StartsWith(contentBefore);
                    bool hasNew = contentAfter.Contains("IMMUTABLE_LINE_2");

                    return (preserved && hasNew, $"Contenido anterior preservado: {preserved}, Nueva línea añadida: {hasNew}");
                }
            },
            new()
            {
                Id = "TC-1303", Area = "Seguridad", Titulo = "RBAC roles de usuario",
                Descripcion = "Verificar control de acceso basado en roles",
                Precondiciones = "Feature RBAC implementado", Pasos = "N/A", ResultadoEsperado = "Roles aplicados",
                Prioridad = "Baja", CARef = "RF-09", Tipo = "Seguridad"
            },

            // ═══════ USABILIDAD (UI) ═══════
            new()
            {
                Id = "TC-1401", Area = "Usabilidad", Titulo = "Interfaz responde durante escaneo",
                Descripcion = "Verificar que la UI no se congela durante operaciones largas",
                Precondiciones = "Aplicación WPF ejecutándose", Pasos = "Prueba manual: iniciar escaneo y verificar que la UI responde",
                ResultadoEsperado = "UI responsiva durante escaneo", Prioridad = "Media", CARef = "RNF-02", Tipo = "Usabilidad"
            },
            new()
            {
                Id = "TC-1402", Area = "Usabilidad", Titulo = "Mensajes de validación claros en configuración",
                Descripcion = "Verificar que los errores de validación se muestran en texto legible",
                Precondiciones = "Aplicación WPF ejecutándose", Pasos = "Prueba manual: ingresar valores inválidos",
                ResultadoEsperado = "Mensajes claros y específicos para cada campo", Prioridad = "Baja", CARef = "RNF-03", Tipo = "Usabilidad"
            },
            new()
            {
                Id = "TC-1403", Area = "Usabilidad", Titulo = "Barra de progreso durante generación de datos",
                Descripcion = "Verificar que la barra de progreso refleja el avance real de la generación",
                Precondiciones = "Aplicación WPF ejecutándose", Pasos = "Prueba manual: iniciar simulación y observar progreso",
                ResultadoEsperado = "Progreso avanza de 0% a 100% proporcionalmente", Prioridad = "Baja", CARef = "RNF-02", Tipo = "Usabilidad"
            },
            new()
            {
                Id = "TC-1404", Area = "Usabilidad", Titulo = "Botón Guardar deshabilitado con config inválida",
                Descripcion = "Verificar que el botón Guardar se deshabilita cuando hay errores de validación",
                Precondiciones = "Aplicación WPF ejecutándose", Pasos = "Prueba manual: ingresar valor inválido y verificar botón",
                ResultadoEsperado = "Botón Guardar deshabilitado", Prioridad = "Baja", CARef = "RNF-03", Tipo = "Usabilidad"
            },
        };
    }
}
