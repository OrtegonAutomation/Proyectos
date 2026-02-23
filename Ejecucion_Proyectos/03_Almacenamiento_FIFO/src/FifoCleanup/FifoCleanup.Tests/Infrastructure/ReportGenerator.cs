using System.Text;
using FifoCleanup.Tests.Models;

namespace FifoCleanup.Tests.Infrastructure;

public static class ReportGenerator
{
    public static async Task GenerateCsvAsync(List<TestCase> tests, string outputPath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("ID\tArea\tTitulo\tDescripcion\tPrecondiciones\tPasos\tResultado Esperado\tPrioridad\tCA Ref\tTipo\tEstado\tResultado\tFecha Ejecucion\tEjecutor\tObservaciones");

        foreach (var tc in tests)
        {
            sb.AppendLine(string.Join("\t",
                Escape(tc.Id),
                Escape(tc.Area),
                Escape(tc.Titulo),
                Escape(tc.Descripcion),
                Escape(tc.Precondiciones),
                Escape(tc.Pasos),
                Escape(tc.ResultadoEsperado),
                Escape(tc.Prioridad),
                Escape(tc.CARef),
                Escape(tc.Tipo),
                Escape(tc.Estado),
                Escape(tc.Resultado),
                Escape(tc.FechaEjecucion),
                Escape(tc.Ejecutor),
                Escape(tc.Observaciones)));
        }

        var dir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        await File.WriteAllTextAsync(outputPath, sb.ToString(), Encoding.UTF8);
    }

    public static void PrintSummary(List<TestCase> tests)
    {
        var total = tests.Count;
        var passed = tests.Count(t => t.Resultado == "PASÓ");
        var failed = tests.Count(t => t.Resultado == "FALLÓ");
        var na = tests.Count(t => t.Resultado == "N/A");
        var errors = tests.Count(t => t.Estado == "Error");

        Console.WriteLine();
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║       RESUMEN DE EJECUCIÓN DE PRUEBAS   ║");
        Console.WriteLine("╠══════════════════════════════════════════╣");
        Console.WriteLine($"║  Total:       {total,4}                       ║");
        Console.WriteLine($"║  Pasaron:     {passed,4}  ✅                    ║");
        Console.WriteLine($"║  Fallaron:    {failed,4}  ❌                    ║");
        Console.WriteLine($"║  N/A:         {na,4}  ⚪                    ║");
        Console.WriteLine($"║  Errores:     {errors,4}  ⚠️                    ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");
        Console.WriteLine();

        var byArea = tests.GroupBy(t => t.Area).OrderBy(g => g.Key);
        Console.WriteLine("Detalle por Área:");
        Console.WriteLine("─────────────────────────────────────────────────");
        foreach (var area in byArea)
        {
            var ap = area.Count(t => t.Resultado == "PASÓ");
            var af = area.Count(t => t.Resultado == "FALLÓ");
            var an = area.Count(t => t.Resultado == "N/A");
            Console.WriteLine($"  {area.Key,-30} | ✅{ap} ❌{af} ⚪{an}");
        }
        Console.WriteLine("─────────────────────────────────────────────────");
    }

    private static string Escape(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Replace("\t", " ").Replace("\n", " | ").Replace("\r", "");
    }
}
