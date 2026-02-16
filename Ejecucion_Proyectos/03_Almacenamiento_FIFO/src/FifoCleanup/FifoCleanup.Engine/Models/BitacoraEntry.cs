namespace FifoCleanup.Engine.Models;

/// <summary>
/// Entrada de bitácora/auditoría.
/// Se persiste en CSV con formato: Timestamp,Type,Action,AssetId,VariableId,Details,BytesAffected,Result
/// </summary>
public class BitacoraEntry
{
    /// <summary>Timestamp de la entrada</summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>Tipo de evento</summary>
    public BitacoraEventType EventType { get; set; }

    /// <summary>Acción realizada</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>Asset afectado (vacío si es global)</summary>
    public string AssetId { get; set; } = string.Empty;

    /// <summary>Variable afectada (vacío si aplica a todo el asset)</summary>
    public string VariableId { get; set; } = string.Empty;

    /// <summary>Detalles adicionales</summary>
    public string Details { get; set; } = string.Empty;

    /// <summary>Bytes afectados (liberados o procesados)</summary>
    public long BytesAffected { get; set; }

    /// <summary>Resultado: OK, ERROR, CANCELLED, SKIPPED</summary>
    public string Result { get; set; } = "OK";

    /// <summary>Usuario o proceso que generó la entrada</summary>
    public string Source { get; set; } = "SYSTEM";

    /// <summary>Línea CSV formateada para persistencia</summary>
    public string ToCsvLine()
    {
        return $"\"{Timestamp:yyyy-MM-dd HH:mm:ss}\",\"{EventType}\",\"{EscapeCsv(Action)}\",\"{EscapeCsv(AssetId)}\",\"{EscapeCsv(VariableId)}\",\"{EscapeCsv(Details)}\",{BytesAffected},\"{EscapeCsv(Result)}\",\"{EscapeCsv(Source)}\"";
    }

    /// <summary>Header CSV</summary>
    public static string CsvHeader => "\"Timestamp\",\"EventType\",\"Action\",\"AssetId\",\"VariableId\",\"Details\",\"BytesAffected\",\"Result\",\"Source\"";

    /// <summary>Parsear una línea CSV a BitacoraEntry</summary>
    public static BitacoraEntry? FromCsvLine(string line)
    {
        try
        {
            var fields = ParseCsvLine(line);
            if (fields.Count < 9) return null;

            return new BitacoraEntry
            {
                Timestamp = DateTime.Parse(fields[0]),
                EventType = Enum.Parse<BitacoraEventType>(fields[1]),
                Action = fields[2],
                AssetId = fields[3],
                VariableId = fields[4],
                Details = fields[5],
                BytesAffected = long.TryParse(fields[6], out var b) ? b : 0,
                Result = fields[7],
                Source = fields[8]
            };
        }
        catch
        {
            return null;
        }
    }

    private static string EscapeCsv(string value) => value.Replace("\"", "\"\"");

    private static List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        bool inQuotes = false;
        var current = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    current.Append(c);
                }
            }
            else
            {
                if (c == '"')
                {
                    inQuotes = true;
                }
                else if (c == ',')
                {
                    fields.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
        }
        fields.Add(current.ToString());
        return fields;
    }
}

public enum BitacoraEventType
{
    Information,
    Inventory,
    CleanupManual,
    CleanupScheduled,
    CleanupPreventive,
    Simulation,
    Configuration,
    Alarm,
    Error,
    SystemStart,
    SystemStop,
    FileDetected
}
