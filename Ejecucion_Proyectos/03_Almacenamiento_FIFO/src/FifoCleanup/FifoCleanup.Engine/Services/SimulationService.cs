using System.Diagnostics;
using System.Linq;
using FifoCleanup.Engine.Models;
using FifoCleanup.Engine.Services.Interfaces;

namespace FifoCleanup.Engine.Services;

/// <summary>
/// RF-03: Simulación con datos sintéticos.
/// Genera estructura realista de carpetas Asset/Variable/E/YYYY/MM/DD
/// y ejecuta algoritmo FIFO para validar antes de producción.
/// </summary>
public class SimulationService : ISimulationService
{
    private readonly IInventoryService _inventory;
    private readonly ICleanupService _cleanup;
    private readonly Random _random = new();

    // Simulación continua
    private CancellationTokenSource? _continuousCts;
    private Task? _continuousTask;
    private ContinuousSimulationParams _continuousParams = new();

    public bool IsContinuousSimulationRunning { get; private set; }
    public long ContinuousDataGeneratedMB { get; private set; }
    public event EventHandler<(string assetId, string variableId, long byteGenerated)>? OnContinuousDataGenerated;

    public SimulationService(IInventoryService inventory, ICleanupService cleanup)
    {
        _inventory = inventory;
        _cleanup = cleanup;
    }

    public async Task<bool> GenerateSyntheticDataAsync(
        SimulationParams parameters,
        CancellationToken ct = default,
        IProgress<(string message, double percent)>? progress = null)
    {
        try
        {
            if (Directory.Exists(parameters.SimulationPath))
            {
                if (Directory.EnumerateFileSystemEntries(parameters.SimulationPath).Any())
                {
                    progress?.Report(("Datos de simulación existentes detectados, reutilizando...", 0));
                    return true;
                }

                Directory.Delete(parameters.SimulationPath, true);
            }

            Directory.CreateDirectory(parameters.SimulationPath);

            int typeCount = (parameters.GenerateEData && parameters.GenerateFData) ? 2
                          : (parameters.GenerateEData || parameters.GenerateFData) ? 1 : 0;
            int totalOperations = parameters.NumberOfAssets * parameters.VariablesPerAsset * parameters.DaysOfHistory * typeCount;
            int completed = 0;
            long totalBytesWritten = 0;

            // Límite real: no generar más datos que el uso objetivo del disco simulado
            long targetUsageBytes = (long)(parameters.SimulatedDiskSizeGB * 1024 * 1024 * 1024 * parameters.InitialUsagePercent / 100.0);
            bool capacityReached = false;

            for (int a = 1; a <= parameters.NumberOfAssets && !capacityReached; a++)
            {
                ct.ThrowIfCancellationRequested();
                var assetName = $"Asset{a:D3}";
                var assetPath = Path.Combine(parameters.SimulationPath, assetName);

                for (int v = 0; v < parameters.VariablesPerAsset && !capacityReached; v++)
                {
                    ct.ThrowIfCancellationRequested();
                    var varName = v.ToString("D2");
                    var varPath = Path.Combine(assetPath, varName);

                    // Generar carpetas de tendencias (se ignoran en FIFO)
                    var trendPath = Path.Combine(varPath, DateTime.Now.Year.ToString());
                    Directory.CreateDirectory(trendPath);
                    await CreateRealFileAsync(Path.Combine(trendPath, "trend.dat"), 1024, ct);

                    // Generar datos E y F
                    string[] types = { };
                    if (parameters.GenerateEData && parameters.GenerateFData)
                        types = new[] { "E", "F" };
                    else if (parameters.GenerateEData)
                        types = new[] { "E" };
                    else if (parameters.GenerateFData)
                        types = new[] { "F" };

                    foreach (var folderType in types)
                    {
                        for (int d = 0; d < parameters.DaysOfHistory; d++)
                        {
                            ct.ThrowIfCancellationRequested();

                            // Verificar si ya alcanzamos el uso objetivo
                            if (totalBytesWritten >= targetUsageBytes)
                            {
                                capacityReached = true;
                                break;
                            }

                            var date = DateTime.Now.AddDays(-d);
                            var dayPath = Path.Combine(varPath, folderType,
                                date.Year.ToString("D4"),
                                date.Month.ToString("D2"),
                                date.Day.ToString("D2"));

                            Directory.CreateDirectory(dayPath);

                            // Generar archivos con tamaño real y variación
                            double variation = 1 + (_random.NextDouble() * 2 - 1) * parameters.SizeVariationPercent / 100;
                            long targetSize = (long)(parameters.AvgDayFolderSizeMB * 1024 * 1024 * variation);

                            // Recortar si excedería el objetivo
                            long bytesRemaining = targetUsageBytes - totalBytesWritten;
                            if (targetSize > bytesRemaining)
                                targetSize = bytesRemaining;

                            int fileCount = _random.Next(3, 10);
                            long sizePerFile = targetSize / fileCount;

                            for (int f = 0; f < fileCount; f++)
                            {
                                var fileName = $"data_{f:D3}.bin";
                                await CreateRealFileAsync(Path.Combine(dayPath, fileName), sizePerFile, ct);
                            }

                            totalBytesWritten += targetSize;
                            completed++;
                            double percent = (double)totalBytesWritten / targetUsageBytes * 100;
                            double totalGB = totalBytesWritten / (1024.0 * 1024 * 1024);
                            double targetGB = targetUsageBytes / (1024.0 * 1024 * 1024);
                            progress?.Report(($"Generando: {assetName}/{varName}/{folderType}/{date:yyyy/MM/dd} ({totalGB:F2}/{targetGB:F2} GB)", Math.Min(percent, 100)));
                        }

                        if (capacityReached) break;
                    }
                }
            }

            double finalGB = totalBytesWritten / (1024.0 * 1024 * 1024);
            double diskGB = parameters.SimulatedDiskSizeGB;
            double usagePct = (double)totalBytesWritten / (parameters.SimulatedDiskSizeGB * 1024 * 1024 * 1024) * 100;
            string stopReason = capacityReached 
                ? $"Uso objetivo alcanzado ({parameters.InitialUsagePercent}%)" 
                : "Todos los Assets/Variables/Días generados";
            progress?.Report(($"Generación completada: {finalGB:F2} GB / {diskGB:F1} GB ({usagePct:F1}%) — {stopReason}", 100));
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<SimulationResult> RunSimulationAsync(
        SimulationParams parameters,
        CancellationToken ct = default,
        IProgress<(string message, double percent)>? progress = null)
    {
        var sw = Stopwatch.StartNew();
        var result = new SimulationResult { Parameters = parameters };

        // 1. Generar datos sintéticos
        progress?.Report(("Fase 1: Generando datos sintéticos...", 0));
        result.DataGenerationSuccess = await GenerateSyntheticDataAsync(parameters, ct,
            new Progress<(string msg, double pct)>(p =>
                progress?.Report((p.msg, p.pct * 0.4)))); // 0-40%

        if (!result.DataGenerationSuccess)
        {
            result.LogMessages.Add("ERROR: Falló la generación de datos sintéticos.");
            result.TotalDurationMs = sw.ElapsedMilliseconds;
            return result;
        }

        // 2. Escanear inventario de datos generados (métricas reales)
        progress?.Report(("Fase 2: Escaneando inventario real...", 40));
        result.StatusBefore = await _inventory.ScanAsync(parameters.SimulationPath, ct);

        // Disco virtual: SimulatedDiskSizeGB define el tamaño total del disco simulado
        result.StatusBefore.TotalSpaceBytes = (long)(parameters.SimulatedDiskSizeGB * 1024 * 1024 * 1024);

        // Uso real: los bytes reales que ocupan los datos generados en disco
        result.StatusBefore.UsedSpaceBytes = result.StatusBefore.MonitoredDataBytes;
        result.StatusBefore.FreeSpaceBytes = result.StatusBefore.TotalSpaceBytes - result.StatusBefore.UsedSpaceBytes;

        double realDataGB = result.StatusBefore.MonitoredDataBytes / (1024.0 * 1024 * 1024);
        result.LogMessages.Add($"Inventario: {result.StatusBefore.Assets.Count} Assets, " +
            $"{result.StatusBefore.Assets.Sum(a => a.TotalDayFolders)} carpetas de día.");
        result.LogMessages.Add($"Datos reales en disco: {realDataGB:F2} GB");
        result.LogMessages.Add($"Uso real: {result.StatusBefore.UsagePercent:F1}% de {parameters.SimulatedDiskSizeGB} GB");

        // 3. Ejecutar limpieza simulada (dry run)
        progress?.Report(("Fase 3: Ejecutando limpieza FIFO simulada...", 60));

        var simConfig = new FifoConfiguration
        {
            StoragePath = parameters.SimulationPath,
            ThresholdPercent = parameters.ThresholdPercent,
            CleanupCapPercent = parameters.CleanupCapPercent
        };

        result.CleanupResult = await _cleanup.ExecuteGeneralCleanupAsync(
            result.StatusBefore, simConfig, ct,
            new Progress<(string msg, double pct)>(p =>
                progress?.Report((p.msg, 60 + p.pct * 0.3))), // 60-90%
            dryRun: true);

        result.LogMessages.Add($"Resultado: {result.CleanupResult.Message}");
        result.LogMessages.Add($"Se liberarían: {result.CleanupResult.BytesFreedFormatted}");
        result.LogMessages.Add($"Carpetas afectadas: {result.CleanupResult.FoldersDeleted}");

        // 4. Estado después de simulación
        result.StatusAfter = new StorageStatus
        {
            TotalSpaceBytes = result.StatusBefore.TotalSpaceBytes,
            UsedSpaceBytes = result.StatusBefore.UsedSpaceBytes - result.CleanupResult.BytesFreed,
            FreeSpaceBytes = result.StatusBefore.FreeSpaceBytes + result.CleanupResult.BytesFreed,
            LastScanTime = DateTime.Now
        };

        progress?.Report(("Simulación completada", 100));
        result.TotalDurationMs = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task CleanupSyntheticDataAsync(string simulationPath)
    {
        if (Directory.Exists(simulationPath))
        {
            await Task.Run(() => Directory.Delete(simulationPath, true));
        }
    }

    public async Task StartContinuousSimulationAsync(ContinuousSimulationParams parameters, CancellationToken ct = default)
    {
        if (IsContinuousSimulationRunning)
            throw new InvalidOperationException("La simulación continua ya está en ejecución.");

        if (!Directory.Exists(parameters.SimulationPath))
            throw new DirectoryNotFoundException($"La ruta de simulación no existe: {parameters.SimulationPath}");

        _continuousParams = parameters;
        _continuousCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        IsContinuousSimulationRunning = true;
        ContinuousDataGeneratedMB = 0;

        // Iniciar tarea de generación continua
        _continuousTask = Task.Run(() => ContinuousGenerationLoop(_continuousCts.Token), _continuousCts.Token);

        await Task.CompletedTask;
    }

    public async Task StopContinuousSimulationAsync()
    {
        if (!IsContinuousSimulationRunning) return;

        _continuousCts?.Cancel();
        IsContinuousSimulationRunning = false;

        if (_continuousTask != null)
        {
            try { await _continuousTask; }
            catch (OperationCanceledException) { }
        }
    }

    private async Task ContinuousGenerationLoop(CancellationToken ct)
    {
        // Obtener lista de Assets y Variables existentes en la ruta
        var assets = GetExistingAssets(_continuousParams.SimulationPath);

        if (assets.Count == 0)
        {
            // Si no hay assets, no podemos generar datos
            IsContinuousSimulationRunning = false;
            return;
        }

        // Filtrar por TargetAssets si se especificaron
        if (_continuousParams.TargetAssets.Any())
        {
            assets = assets.Where(a => _continuousParams.TargetAssets.Contains(a.assetId)).ToList();
        }

        while (!ct.IsCancellationRequested)
        {
            try
            {
                // Esperar el intervalo configurado
                await Task.Delay(TimeSpan.FromSeconds(_continuousParams.IntervalSeconds), ct);

                // Calcular cuántos MB generar este ciclo
                double variation = 1 + (_random.NextDouble() * 2 - 1) * _continuousParams.SizeVariationPercent / 100;
                long targetBytes = (long)(_continuousParams.DataRateMBPerInterval * 1024 * 1024 * variation);

                // Distribuir los bytes entre los assets/variables
                long bytesPerAsset = targetBytes / assets.Count;

                foreach (var (assetId, variables) in assets)
                {
                    if (ct.IsCancellationRequested) break;

                    foreach (var variableId in variables)
                    {
                        if (ct.IsCancellationRequested) break;

                        // Determinar tipos de carpeta a generar
                        List<string> types = new();
                        if (_continuousParams.GenerateEData && _continuousParams.GenerateFData)
                            types = new[] { "E", "F" }.ToList();
                        else if (_continuousParams.GenerateEData)
                            types = new[] { "E" }.ToList();
                        else if (_continuousParams.GenerateFData)
                            types = new[] { "F" }.ToList();

                        long bytesPerVariable = bytesPerAsset / variables.Count;
                        long bytesPerType = bytesPerVariable / types.Count;

                        foreach (var folderType in types)
                        {
                            if (ct.IsCancellationRequested) break;

                            // Generar archivos en la carpeta del día actual
                            var now = DateTime.Now;
                            var dayPath = Path.Combine(_continuousParams.SimulationPath, assetId, variableId, folderType,
                                now.Year.ToString("D4"), now.Month.ToString("D2"), now.Day.ToString("D2"));

                            Directory.CreateDirectory(dayPath);

                            // Generar varios archivos reales que sumen el tamaño objetivo
                            int fileCount = _random.Next(2, 6);
                            long sizePerFile = bytesPerType / fileCount;

                            long bytesWrittenThisType = 0;
                            for (int f = 0; f < fileCount; f++)
                            {
                                if (ct.IsCancellationRequested) break;
                                var fileName = $"data_{now:HHmmss}_{now.Millisecond:D3}_{f:D3}.bin";
                                var filePath = Path.Combine(dayPath, fileName);
                                await CreateRealFileAsync(filePath, sizePerFile, ct);
                                bytesWrittenThisType += sizePerFile;
                            }

                            // Notificar que se generaron datos reales
                            OnContinuousDataGenerated?.Invoke(this, (assetId, variableId, bytesWrittenThisType));
                        }
                    }
                }

                ContinuousDataGeneratedMB += (long)_continuousParams.DataRateMBPerInterval;
            }
            catch (OperationCanceledException) { break; }
            catch
            {
                // Continuar generando incluso si hay errores
            }
        }
    }

    /// <summary>Obtiene la lista de Assets y Variables existentes en la ruta.</summary>
    private List<(string assetId, List<string> variables)> GetExistingAssets(string path)
    {
        var assets = new List<(string assetId, List<string> variables)>();

        try
        {
            var assetDirs = Directory.GetDirectories(path)
                .Where(d => !Path.GetFileName(d).Equals("sim_alloc.bin", StringComparison.OrdinalIgnoreCase));

            foreach (var assetDir in assetDirs)
            {
                var assetId = Path.GetFileName(assetDir);
                var variables = Directory.GetDirectories(assetDir)
                    .Select(v => Path.GetFileName(v))
                    .Where(v => int.TryParse(v, out _)) // Solo variables numéricas
                    .ToList();

                if (variables.Any())
                {
                    assets.Add((assetId, variables));
                }
            }
        }
        catch { }

        return assets;
    }

    /// <summary>Crea un archivo con tamaño REAL en disco.
    /// Usa buffer de 64 KB para escritura eficiente. Sin límite de tamaño.</summary>
    private async Task CreateRealFileAsync(string path, long sizeBytes, CancellationToken ct = default)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        const int bufferSize = 64 * 1024;
        var buffer = new byte[bufferSize];
        _random.NextBytes(buffer);

        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize);
        long written = 0;
        while (written < sizeBytes)
        {
            ct.ThrowIfCancellationRequested();
            int toWrite = (int)Math.Min(bufferSize, sizeBytes - written);
            await fs.WriteAsync(buffer.AsMemory(0, toWrite), ct);
            written += toWrite;
        }
    }
}
