using FifoCleanup.Engine.Models;
using FifoCleanup.Engine.Services;
using FifoCleanup.Engine.Services.Interfaces;

namespace FifoCleanup.Tests.Infrastructure;

/// <summary>
/// Contexto compartido para todas las pruebas.
/// Instancia los servicios reales del Engine contra D:\FifoTestBed.
/// </summary>
public class TestContext : IDisposable
{
    public string BasePath { get; }
    public string DataPath { get; }
    public string BitacoraPath { get; }
    public string ConfigPath { get; }
    public string ReportPath { get; }

    public IInventoryService Inventory { get; }
    public IBitacoraService Bitacora { get; }
    public ICleanupService Cleanup { get; }
    public ISimulationService Simulation { get; }
    public IConfigurationService Configuration { get; }
    public IScheduledCleanupService ScheduledCleanup { get; }
    public PreventiveMonitorService PreventiveMonitor { get; }

    public FifoConfiguration DefaultConfig { get; set; }

    public TestContext(string basePath = @"D:\FifoTestBed")
    {
        BasePath = basePath;
        DataPath = Path.Combine(basePath, "Data");
        BitacoraPath = Path.Combine(basePath, "Bitacora");
        ConfigPath = Path.Combine(basePath, "config.json");
        ReportPath = Path.Combine(basePath, "Reportes");

        Directory.CreateDirectory(BasePath);
        Directory.CreateDirectory(DataPath);
        Directory.CreateDirectory(BitacoraPath);
        Directory.CreateDirectory(ReportPath);

        // Instanciar servicios reales
        Inventory = new InventoryService();
        Bitacora = new BitacoraService();
        Configuration = new ConfigurationService();
        Cleanup = new CleanupService(Bitacora);
        Simulation = new SimulationService(Inventory, Cleanup);
        ScheduledCleanup = new ScheduledCleanupService(Inventory, Cleanup, Bitacora);
        PreventiveMonitor = new PreventiveMonitorService(Inventory, Cleanup, Bitacora);

        DefaultConfig = new FifoConfiguration
        {
            StoragePath = DataPath,
            MaxStorageSizeGB = 2,
            ThresholdPercent = 85,
            CleanupCapPercent = 20,
            ScheduledFrequencyHours = 1,
            ScheduledHour = 2,
            PreventiveThresholdDays = 3,
            EnableScheduledCleanup = true,
            EnablePreventiveCleanup = true,
            MaxConcurrentAssets = 5,
            MaxDaysToDeletePerAsset = 10,
            ConfigFilePath = ConfigPath,
            BitacoraPath = BitacoraPath,
            BitacoraRetentionDays = 90,
            BitacoraMaxSizeMB = 100
        };
    }

    /// <summary>Crea estructura de datos de prueba con Assets/Variables/E/YYYY/MM/DD</summary>
    public async Task CreateTestDataAsync(int assets = 3, int variables = 2, int days = 15, long mbPerDay = 5)
    {
        Console.WriteLine($"  Generando datos de prueba: {assets} Assets, {variables} vars, {days} días, {mbPerDay} MB/día...");
        for (int a = 1; a <= assets; a++)
        {
            for (int v = 0; v < variables; v++)
            {
                foreach (var folderType in new[] { "E", "F" })
                {
                    for (int d = 0; d < days; d++)
                    {
                        var date = DateTime.Now.AddDays(-d);
                        var dayPath = Path.Combine(DataPath, $"Asset{a:D3}", v.ToString("D2"), folderType,
                            date.Year.ToString("D4"), date.Month.ToString("D2"), date.Day.ToString("D2"));
                        Directory.CreateDirectory(dayPath);

                        var filePath = Path.Combine(dayPath, "data_001.bin");
                        await CreateTestFileAsync(filePath, mbPerDay * 1024 * 1024);
                    }
                }
            }
        }
        Console.WriteLine("  Datos de prueba generados.");
    }

    /// <summary>Crea un archivo de prueba de tamaño específico</summary>
    public async Task CreateTestFileAsync(string path, long sizeBytes)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        const int bufferSize = 64 * 1024;
        var buffer = new byte[bufferSize];
        Random.Shared.NextBytes(buffer);

        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize);
        long written = 0;
        while (written < sizeBytes)
        {
            int toWrite = (int)Math.Min(bufferSize, sizeBytes - written);
            await fs.WriteAsync(buffer.AsMemory(0, toWrite));
            written += toWrite;
        }
    }

    /// <summary>Limpia toda la carpeta de datos de prueba</summary>
    public void CleanTestData()
    {
        if (Directory.Exists(DataPath))
        {
            try { Directory.Delete(DataPath, true); } catch { }
            Directory.CreateDirectory(DataPath);
        }
    }

    /// <summary>Obtiene tamaño total de un directorio</summary>
    public long GetDirectorySize(string path)
    {
        if (!Directory.Exists(path)) return 0;
        return new DirectoryInfo(path)
            .EnumerateFiles("*", SearchOption.AllDirectories)
            .Sum(f => f.Length);
    }

    public void Dispose()
    {
        // No borramos datos al finalizar para inspección manual
        GC.SuppressFinalize(this);
    }
}
