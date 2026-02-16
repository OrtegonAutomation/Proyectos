using System.Text.Json;
using FifoCleanup.Engine.Models;
using FifoCleanup.Engine.Services.Interfaces;

namespace FifoCleanup.Engine.Services;

public class ConfigurationService : IConfigurationService
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<FifoConfiguration> LoadAsync(string path)
    {
        if (!File.Exists(path))
            return GetDefault();

        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<FifoConfiguration>(json, _jsonOptions)
               ?? GetDefault();
    }

    public async Task SaveAsync(FifoConfiguration config, string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(config, _jsonOptions);
        await File.WriteAllTextAsync(path, json);
    }

    public List<string> Validate(FifoConfiguration config)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(config.StoragePath))
            errors.Add("La ruta de almacenamiento es requerida.");
        else if (!Directory.Exists(config.StoragePath))
            errors.Add($"La ruta '{config.StoragePath}' no existe.");

        if (config.ThresholdPercent < 50 || config.ThresholdPercent > 95)
            errors.Add("El umbral debe estar entre 50% y 95%.");

        if (config.CleanupCapPercent < 5 || config.CleanupCapPercent > 50)
            errors.Add("El cap de limpieza debe estar entre 5% y 50%.");

        if (config.ScheduledFrequencyHours < 1 || config.ScheduledFrequencyHours > 24)
            errors.Add("La frecuencia programada debe estar entre 1 y 24 horas.");

        if (config.ScheduledHour < 0 || config.ScheduledHour > 23)
            errors.Add("La hora programada debe estar entre 0 y 23.");

        if (config.PreventiveThresholdDays < 1 || config.PreventiveThresholdDays > 10)
            errors.Add("El umbral preventivo debe estar entre 1 y 10 días.");

        if (config.MaxConcurrentAssets < 1 || config.MaxConcurrentAssets > 10)
            errors.Add("El máximo de Assets concurrentes debe estar entre 1 y 10.");

        if (config.MaxDaysToDeletePerAsset < 1 || config.MaxDaysToDeletePerAsset > 30)
            errors.Add("El máximo de días a eliminar por Asset debe estar entre 1 y 30.");

        if (config.BitacoraRetentionDays < 1 || config.BitacoraRetentionDays > 365)
            errors.Add("La retención de bitácora debe estar entre 1 y 365 días.");

        return errors;
    }

    public FifoConfiguration GetDefault() => new()
    {
        StoragePath = @"D:\MonitoringData",
        ThresholdPercent = 85.0,
        CleanupCapPercent = 20.0,
        ScheduledFrequencyHours = 24,
        ScheduledHour = 2,
        PreventiveThresholdDays = 3,
        EnableScheduledCleanup = true,
        EnablePreventiveCleanup = true,
        MaxConcurrentAssets = 5,
        MaxDaysToDeletePerAsset = 10,
        ConfigFilePath = "fifo_config.json",
        BitacoraPath = "bitacora",
        BitacoraRetentionDays = 90,
        BitacoraMaxSizeMB = 100
    };
}
