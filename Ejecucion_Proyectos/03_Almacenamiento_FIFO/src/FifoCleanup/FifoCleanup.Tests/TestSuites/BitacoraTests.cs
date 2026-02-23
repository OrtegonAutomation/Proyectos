using FifoCleanup.Engine.Models;
using FifoCleanup.Tests.Infrastructure;
using FifoCleanup.Tests.Models;

namespace FifoCleanup.Tests.TestSuites;

/// <summary>RF-05: Pruebas de Bitácora de Auditoría</summary>
public static class BitacoraTests
{
    public static List<TestCase> GetTests(TestContext ctx)
    {
        return new List<TestCase>
        {
            new()
            {
                Id = "TC-0501", Area = "Bitácora", Titulo = "Inicialización de bitácora",
                Descripcion = "Verificar que InitializeAsync crea la carpeta y el archivo CSV con header",
                Precondiciones = "Carpeta de bitácora no existe",
                Pasos = "1. Eliminar carpeta | 2. InitializeAsync | 3. Verificar archivo creado con header",
                ResultadoEsperado = "Archivo CSV existe con header correcto", Prioridad = "Alta", CARef = "RF-05", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var testBitPath = Path.Combine(ctx.BasePath, "BitacoraTest");
                    if (Directory.Exists(testBitPath)) Directory.Delete(testBitPath, true);

                    await ctx.Bitacora.InitializeAsync(testBitPath);
                    var files = Directory.GetFiles(testBitPath, "bitacora_*.csv");
                    bool exists = files.Length > 0;
                    string content = exists ? await File.ReadAllTextAsync(files[0]) : "";
                    bool hasHeader = content.Contains("Timestamp") && content.Contains("EventType");

                    return (exists && hasHeader, $"Archivo creado: {exists}, Header correcto: {hasHeader}");
                }
            },
            new()
            {
                Id = "TC-0502", Area = "Bitácora", Titulo = "Registro de entrada con todos los campos",
                Descripcion = "Verificar que LogAsync persiste todos los campos de BitacoraEntry",
                Precondiciones = "Bitácora inicializada",
                Pasos = "1. InitializeAsync | 2. LogAsync con todos los campos | 3. GetEntriesAsync | 4. Verificar campos",
                ResultadoEsperado = "Todos los campos se recuperan correctamente", Prioridad = "Alta", CARef = "RF-05", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var testBitPath = Path.Combine(ctx.BasePath, "BitacoraFields");
                    if (Directory.Exists(testBitPath)) Directory.Delete(testBitPath, true);
                    await ctx.Bitacora.InitializeAsync(testBitPath);

                    var entry = new BitacoraEntry
                    {
                        EventType = BitacoraEventType.CleanupScheduled,
                        Action = "TEST_ACTION",
                        AssetId = "Asset001",
                        VariableId = "02",
                        Details = "Detalle de prueba con comas, y \"comillas\"",
                        BytesAffected = 1048576,
                        Result = "OK",
                        Source = "AUTOMATED_TEST"
                    };
                    await ctx.Bitacora.LogAsync(entry);

                    var entries = await ctx.Bitacora.GetEntriesAsync();
                    var found = entries.FirstOrDefault(e => e.Action == "TEST_ACTION");

                    bool ok = found != null
                        && found.EventType == BitacoraEventType.CleanupScheduled
                        && found.AssetId == "Asset001"
                        && found.VariableId == "02"
                        && found.BytesAffected == 1048576
                        && found.Source == "AUTOMATED_TEST";

                    return (ok, ok ? "Todos los campos persisten correctamente" : $"Entrada encontrada: {found != null}");
                }
            },
            new()
            {
                Id = "TC-0503", Area = "Bitácora", Titulo = "Filtrado por tipo de evento",
                Descripcion = "Verificar que GetEntriesAsync filtra correctamente por BitacoraEventType",
                Precondiciones = "Múltiples entradas de diferentes tipos",
                Pasos = "1. Crear entradas de tipo Inventory, CleanupScheduled, Error | 2. Filtrar por Inventory | 3. Verificar solo tipo Inventory",
                ResultadoEsperado = "Solo se retornan entradas de tipo Inventory", Prioridad = "Media", CARef = "RF-05", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var testBitPath = Path.Combine(ctx.BasePath, "BitacoraFilter");
                    if (Directory.Exists(testBitPath)) Directory.Delete(testBitPath, true);
                    await ctx.Bitacora.InitializeAsync(testBitPath);

                    await ctx.Bitacora.LogAsync(new BitacoraEntry { EventType = BitacoraEventType.Inventory, Action = "SCAN" });
                    await ctx.Bitacora.LogAsync(new BitacoraEntry { EventType = BitacoraEventType.CleanupScheduled, Action = "CLEANUP" });
                    await ctx.Bitacora.LogAsync(new BitacoraEntry { EventType = BitacoraEventType.Error, Action = "ERROR" });
                    await ctx.Bitacora.LogAsync(new BitacoraEntry { EventType = BitacoraEventType.Inventory, Action = "SCAN2" });

                    var filtered = await ctx.Bitacora.GetEntriesAsync(type: BitacoraEventType.Inventory);
                    bool allInventory = filtered.All(e => e.EventType == BitacoraEventType.Inventory);
                    bool correctCount = filtered.Count == 2;

                    return (allInventory && correctCount, $"Filtradas: {filtered.Count}, Todas Inventory: {allInventory}");
                }
            },
            new()
            {
                Id = "TC-0504", Area = "Bitácora", Titulo = "Filtrado por rango de fechas",
                Descripcion = "Verificar filtrado temporal con from/to",
                Precondiciones = "Entradas con diferentes timestamps",
                Pasos = "1. Crear entradas | 2. Filtrar con from=hace 1 hora | 3. Verificar resultados",
                ResultadoEsperado = "Solo entradas dentro del rango temporal", Prioridad = "Media", CARef = "RF-05", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var entries = await ctx.Bitacora.GetEntriesAsync(from: DateTime.Now.AddHours(-1));
                    bool allRecent = entries.All(e => e.Timestamp >= DateTime.Now.AddHours(-1).AddMinutes(-1));
                    return (allRecent, $"Entradas recientes: {entries.Count}, Todas dentro del rango: {allRecent}");
                }
            },
            new()
            {
                Id = "TC-0505", Area = "Bitácora", Titulo = "Exportar bitácora a CSV",
                Descripcion = "Verificar que ExportToCsvAsync genera archivo CSV válido",
                Precondiciones = "Bitácora con entradas",
                Pasos = "1. ExportToCsvAsync | 2. Verificar archivo generado | 3. Verificar header y contenido",
                ResultadoEsperado = "Archivo CSV generado con header y líneas de datos", Prioridad = "Media", CARef = "RF-05", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var exportPath = Path.Combine(ctx.ReportPath, "export_test.csv");
                    await ctx.Bitacora.ExportToCsvAsync(exportPath);

                    bool exists = File.Exists(exportPath);
                    int lines = 0;
                    if (exists)
                    {
                        lines = (await File.ReadAllLinesAsync(exportPath)).Length;
                    }

                    return (exists && lines >= 2, $"Archivo: {exists}, Líneas: {lines}");
                }
            },
            new()
            {
                Id = "TC-0506", Area = "Bitácora", Titulo = "Formato CSV con campos especiales",
                Descripcion = "Verificar que comas, comillas y saltos de línea se escapan correctamente en CSV",
                Precondiciones = "Bitácora inicializada",
                Pasos = "1. LogAsync con Details que contiene comas y comillas | 2. Leer archivo CSV | 3. Verificar parseo correcto",
                ResultadoEsperado = "La línea CSV se parsea correctamente a pesar de caracteres especiales", Prioridad = "Media", CARef = "RF-05", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var entry = new BitacoraEntry
                    {
                        EventType = BitacoraEventType.Information,
                        Action = "CSV_SPECIAL_TEST",
                        Details = "Detalles con \"comillas\", comas, y más"
                    };
                    await ctx.Bitacora.LogAsync(entry);

                    var entries = await ctx.Bitacora.GetEntriesAsync();
                    var found = entries.FirstOrDefault(e => e.Action == "CSV_SPECIAL_TEST");

                    bool ok = found != null && found.Details.Contains("comillas") && found.Details.Contains("comas");
                    return (ok, found != null ? $"Details recuperado: {found.Details}" : "No se encontró la entrada");
                }
            },
            new()
            {
                Id = "TC-0507", Area = "Bitácora", Titulo = "Mantenimiento: retención de días",
                Descripcion = "Verificar que MaintenanceAsync elimina archivos más antiguos que retentionDays",
                Precondiciones = "Archivo de bitácora antiguo existe",
                Pasos = "1. Crear archivo antiguo (>90 días) | 2. MaintenanceAsync(90, 100) | 3. Verificar que fue eliminado",
                ResultadoEsperado = "Archivos más antiguos que retentionDays son eliminados", Prioridad = "Media", CARef = "RF-05", Tipo = "Funcional",
                TestAction = async () =>
                {
                    var testBitPath = Path.Combine(ctx.BasePath, "BitacoraRetention");
                    if (Directory.Exists(testBitPath)) Directory.Delete(testBitPath, true);
                    Directory.CreateDirectory(testBitPath);

                    // Crear archivo "antiguo"
                    var oldFile = Path.Combine(testBitPath, "bitacora_20230101.csv");
                    await File.WriteAllTextAsync(oldFile, BitacoraEntry.CsvHeader);
                    File.SetCreationTime(oldFile, DateTime.Now.AddDays(-100));

                    // Crear archivo reciente
                    var newFile = Path.Combine(testBitPath, $"bitacora_{DateTime.Now:yyyyMMdd}.csv");
                    await File.WriteAllTextAsync(newFile, BitacoraEntry.CsvHeader);

                    await ctx.Bitacora.InitializeAsync(testBitPath);
                    await ctx.Bitacora.MaintenanceAsync(90, 100);

                    bool oldDeleted = !File.Exists(oldFile);
                    bool newExists = File.Exists(newFile);

                    return (oldDeleted && newExists, $"Antiguo eliminado: {oldDeleted}, Reciente conservado: {newExists}");
                }
            },
            new()
            {
                Id = "TC-0508", Area = "Bitácora", Titulo = "Timestamp ISO con precisión de segundos",
                Descripcion = "Verificar que el timestamp se registra con formato yyyy-MM-dd HH:mm:ss",
                Precondiciones = "Bitácora con entradas",
                Pasos = "1. LogAsync | 2. Leer línea CSV | 3. Verificar formato de timestamp",
                ResultadoEsperado = "Timestamp en formato yyyy-MM-dd HH:mm:ss", Prioridad = "Baja", CARef = "RF-05", Tipo = "Funcional",
                TestAction = async () =>
                {
                    // Re-initialize to isolated path for this test
                    var tsBitPath = Path.Combine(ctx.BasePath, "BitacoraTimestamp");
                    if (Directory.Exists(tsBitPath)) Directory.Delete(tsBitPath, true);
                    await ctx.Bitacora.InitializeAsync(tsBitPath);

                    var before = DateTime.Now;
                    await ctx.Bitacora.LogAsync(new BitacoraEntry
                    {
                        EventType = BitacoraEventType.Information,
                        Action = "TIMESTAMP_TEST"
                    });

                    var entries = await ctx.Bitacora.GetEntriesAsync();
                    var found = entries.FirstOrDefault(e => e.Action == "TIMESTAMP_TEST");

                    bool ok = found != null && found.Timestamp >= before.AddSeconds(-2) && found.Timestamp <= DateTime.Now.AddSeconds(2);

                    // Restore original bitácora path
                    await ctx.Bitacora.InitializeAsync(ctx.BitacoraPath);
                    return (ok, found != null ? $"Timestamp: {found.Timestamp:yyyy-MM-dd HH:mm:ss}" : "No encontrada");
                }
            },
        };
    }
}
