using FifoCleanup.Tests.Infrastructure;
using FifoCleanup.Tests.Models;
using FifoCleanup.Tests.TestSuites;
using FifoCleanup.Tests.TestSuites.V1_1;

namespace FifoCleanup.Tests;

class Program
{
    static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        const string ejecutor = "Camilo Ortegon";
        const string basePath = @"D:\FifoTestBed";
        var version = GetVersion(args);
        var reportFolder = version == "1.1"
            ? Path.Combine(basePath, "Reportes", "v1.1")
            : Path.Combine(basePath, "Reportes");
        var reportPath = Path.Combine(reportFolder, $"TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.tsv");

        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║   FIFO CLEANUP - SUITE DE PRUEBAS AUTOMATIZADAS     ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════╣");
        Console.WriteLine($"║  Ejecutor:   {ejecutor,-40}║");
        Console.WriteLine($"║  Disco:      D:\\                                     ║");
        Console.WriteLine($"║  Base Path:  {basePath,-40}║");
        Console.WriteLine($"║  Fecha:      {DateTime.Now:yyyy-MM-dd HH:mm:ss,-40}║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        Console.WriteLine();

        // Verificar disco D:\
        if (!Directory.Exists(@"D:\"))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: El disco D:\\ no está disponible.");
            Console.ResetColor();
            return 1;
        }

        using var ctx = new TestContext(basePath);

        // Initialize bitácora
        await ctx.Bitacora.InitializeAsync(ctx.BitacoraPath);

        // Collect all test cases
        var allTests = new List<TestCase>();

        Console.WriteLine("Registrando suites de prueba...");
        if (version == "1.1")
        {
            allTests.AddRange(NewFeaturesV11Tests.GetTests(ctx));
            Console.WriteLine($"  ✓ v1.1 Nuevas funcionalidades: {allTests.Count} tests");
        }
        else
        {
            allTests.AddRange(InventarioTests.GetTests(ctx));
            Console.WriteLine($"  ✓ Inventario: {allTests.Count} tests");

            int prev = allTests.Count;
            allTests.AddRange(ConfiguracionTests.GetTests(ctx));
            Console.WriteLine($"  ✓ Configuración: {allTests.Count - prev} tests");

            prev = allTests.Count;
            allTests.AddRange(SimulacionTests.GetTests(ctx));
            Console.WriteLine($"  ✓ Simulación: {allTests.Count - prev} tests");

            prev = allTests.Count;
            allTests.AddRange(LimpiezaTests.GetTests(ctx));
            Console.WriteLine($"  ✓ Limpieza: {allTests.Count - prev} tests");

            prev = allTests.Count;
            allTests.AddRange(BitacoraTests.GetTests(ctx));
            Console.WriteLine($"  ✓ Bitácora: {allTests.Count - prev} tests");

            prev = allTests.Count;
            allTests.AddRange(RF07Tests.GetTests(ctx));
            Console.WriteLine($"  ✓ RF-07: {allTests.Count - prev} tests");

            prev = allTests.Count;
            allTests.AddRange(RF08Tests.GetTests(ctx));
            Console.WriteLine($"  ✓ RF-08: {allTests.Count - prev} tests");

            prev = allTests.Count;
            allTests.AddRange(RendimientoTests.GetTests(ctx));
            Console.WriteLine($"  ✓ Rendimiento/StorageStatus: {allTests.Count - prev} tests");

            prev = allTests.Count;
            allTests.AddRange(IntegracionTests.GetTests(ctx));
            Console.WriteLine($"  ✓ Integración: {allTests.Count - prev} tests");

            prev = allTests.Count;
            allTests.AddRange(EdgeCaseTests.GetTests(ctx));
            Console.WriteLine($"  ✓ Edge Cases: {allTests.Count - prev} tests");

            prev = allTests.Count;
            allTests.AddRange(PendingFeatureTests.GetTests(ctx));
            Console.WriteLine($"  ✓ Pendientes (Alarmas/Seguridad/Usabilidad): {allTests.Count - prev} tests");
        }

        Console.WriteLine($"\nTotal de pruebas registradas: {allTests.Count}");
        Console.WriteLine("─────────────────────────────────────────────────");
        Console.WriteLine("Iniciando ejecución...\n");

        // Execute all tests
        int executed = 0;
        foreach (var test in allTests)
        {
            executed++;
            Console.Write($"  [{executed}/{allTests.Count}] {test.Id} - {test.Titulo}... ");

            try
            {
                await test.ExecuteAsync(ejecutor);
            }
            catch (Exception ex)
            {
                test.Estado = "Error";
                test.Resultado = "FALLÓ";
                test.Observaciones = $"Error no capturado: {ex.Message}";
                test.FechaEjecucion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                test.Ejecutor = ejecutor;
            }

            // Color-coded output
            switch (test.Resultado)
            {
                case "PASÓ":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✅ PASÓ");
                    break;
                case "FALLÓ":
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ FALLÓ - {test.Observaciones}");
                    break;
                case "N/A":
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("⚪ N/A");
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"⚠️ {test.Estado}");
                    break;
            }
            Console.ResetColor();
        }

        // Generate report
        Console.WriteLine("\n─────────────────────────────────────────────────");
        Console.WriteLine("Generando reporte TSV...");
        await ReportGenerator.GenerateCsvAsync(allTests, reportPath);
        Console.WriteLine($"📄 Reporte TSV guardado en: {reportPath}");

        // Generate Excel report
        Console.WriteLine("Generando reporte Excel...");
        var excelPath = version == "1.1"
            ? @"C:\Users\IDC INGENIERIA\OneDrive\IDC\Proyectos\Ejecucion_Proyectos\03_Almacenamiento_FIFO\docs\testing\v1.1\01_Casos_Test_v1.1.xlsx"
            : @"C:\Users\IDC INGENIERIA\OneDrive\IDC\Proyectos\Ejecucion_Proyectos\03_Almacenamiento_FIFO\docs\testing\01_Casos_Test.xlsx";
        try
        {
            var excelDir = Path.GetDirectoryName(excelPath);
            if (!string.IsNullOrWhiteSpace(excelDir))
                Directory.CreateDirectory(excelDir);

            FifoCleanup.Tests.Tools.ExcelExporter.ExportToExcel(reportPath, excelPath);
            Console.WriteLine($"📊 Excel actualizado: {excelPath}");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️ No se pudo actualizar Excel: {ex.Message}");
            Console.WriteLine("   (Verifique que el archivo no esté abierto en Excel)");
            Console.ResetColor();
        }

        // Print summary
        ReportGenerator.PrintSummary(allTests);

        // ═══════ FIX FAILURES ═══════
        var failures = allTests.Where(t => t.Resultado == "FALLÓ").ToList();
        if (failures.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n⚠️  PRUEBAS FALLIDAS:");
            foreach (var f in failures)
            {
                Console.WriteLine($"  {f.Id} | {f.Area} | {f.Titulo}");
                Console.WriteLine($"      → {f.Observaciones}");
            }
            Console.ResetColor();
        }

        return failures.Count;
    }

    private static string GetVersion(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if ((args[i] == "--version" || args[i] == "-v") && i + 1 < args.Length)
                return args[i + 1].Trim();
        }

        return "full";
    }
}
