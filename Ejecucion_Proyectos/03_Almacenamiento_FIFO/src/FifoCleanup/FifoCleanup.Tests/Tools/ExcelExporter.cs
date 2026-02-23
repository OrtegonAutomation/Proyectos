using System.Text;
using ClosedXML.Excel;

namespace FifoCleanup.Tests.Tools;

/// <summary>
/// Utility to export test results to Excel format
/// </summary>
public static class ExcelExporter
{
    public static void ExportToExcel(string tsvPath, string excelPath)
    {
        var lines = File.ReadAllLines(tsvPath, Encoding.UTF8);
        if (lines.Length < 2)
        {
            Console.WriteLine("No hay datos para exportar.");
            return;
        }

        var headers = lines[0].Split('\t');
        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Casos de Prueba");

        // Write headers
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
        }

        // Write data rows
        for (int row = 1; row < lines.Length; row++)
        {
            var values = lines[row].Split('\t');
            for (int col = 0; col < values.Length && col < headers.Length; col++)
            {
                worksheet.Cell(row + 1, col + 1).Value = values[col];
                
                // Color-code results
                if (headers[col] == "Resultado")
                {
                    if (values[col] == "PASÓ")
                        worksheet.Cell(row + 1, col + 1).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    else if (values[col] == "FALLÓ")
                        worksheet.Cell(row + 1, col + 1).Style.Fill.BackgroundColor = XLColor.LightCoral;
                    else if (values[col] == "N/A")
                        worksheet.Cell(row + 1, col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                }
            }
        }

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        // Save
        workbook.SaveAs(excelPath);
        Console.WriteLine($"✅ Excel generado: {excelPath}");
    }
}
