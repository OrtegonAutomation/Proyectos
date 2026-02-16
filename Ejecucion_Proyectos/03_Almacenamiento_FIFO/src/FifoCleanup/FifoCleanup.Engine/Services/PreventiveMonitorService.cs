using FifoCleanup.Engine.Models;
using FifoCleanup.Engine.Services.Interfaces;

namespace FifoCleanup.Engine.Services;

/// <summary>
/// RF-08: Servicio de monitoreo preventivo en tiempo real.
/// Usa FileSystemWatcher para detectar adiciones de archivos.
/// 
/// Algoritmo:
/// 1. Monitorear storagePath con FileSystemWatcher (Created events)
/// 2. Al detectar archivo nuevo, parsear ruta → Asset/Variable/E|F/YYYY/MM/DD
/// 3. Calcular velocidad inmediata de crecimiento del Asset
/// 4. Proyectar: si llenado < PreventiveThresholdDays → limpieza LOCAL
/// 5. Limpieza local: solo eliminar de la misma ruta Asset/Variable/E|F (más antiguo)
/// 6. Máximo MaxDaysToDeletePerAsset días por Asset
/// </summary>
public class PreventiveMonitorService : IPreventiveMonitorService
{
    private readonly IInventoryService _inventory;
    private readonly ICleanupService _cleanup;
    private readonly IBitacoraService _bitacora;

    private FileSystemWatcher? _watcher;
    private CancellationTokenSource? _cts;
    // Cola para batch de eventos y reducir llamadas a ScanAsync
    private readonly System.Collections.Concurrent.ConcurrentQueue<string> _eventQueue =
        new System.Collections.Concurrent.ConcurrentQueue<string>();
    private Task? _processorTask;
    private FifoConfiguration _config = new();
    private string _storagePath = string.Empty;

    // Control de ráfagas: no evaluar más de 1 vez por minuto por Asset
    private readonly Dictionary<string, DateTime> _lastEvaluation = new();
    private readonly SemaphoreSlim _evaluationLock = new(1, 1);

    public bool IsRunning { get; private set; }
    public long FilesDetected { get; private set; }
    public int PreventiveCleanups { get; private set; }

    public event EventHandler<FileDetectedEventArgs>? OnFileDetected;
    public event EventHandler<CleanupResult>? OnPreventiveCleanup;

    public PreventiveMonitorService(
        IInventoryService inventory,
        ICleanupService cleanup,
        IBitacoraService bitacora)
    {
        _inventory = inventory;
        _cleanup = cleanup;
        _bitacora = bitacora;
    }

    /// <summary>
    /// Enqueue a path for processing from external tests or UI (non-blocking).
    /// </summary>
    public void EnqueuePathForProcessing(string path)
    {
        _eventQueue.Enqueue(path);
    }

    /// <summary>
    /// Force evaluation for an asset/variable pair (useful for tests).
    /// </summary>
    public async Task ForceEvaluateAsync(string assetId, string variableId)
    {
        await EvaluatePreventiveCleanupAsync(assetId, variableId);
    }

    public async Task StartAsync(string storagePath, FifoConfiguration config, CancellationToken ct = default)
    {
        if (IsRunning) return;

        try
        {
            _storagePath = storagePath;
            _config = config;
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            _watcher = new FileSystemWatcher(storagePath)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size,
                EnableRaisingEvents = true
            };

            _watcher.Created += OnFileCreated;
            IsRunning = true;

            // Iniciar procesador de eventos en background que agrupa eventos cada 5s
            _processorTask = Task.Run(() => EventProcessorLoop(_cts?.Token ?? CancellationToken.None), _cts?.Token ?? CancellationToken.None);

            // Registrar inicio del servicio y del procesador de eventos
            await _bitacora.LogAsync(new BitacoraEntry
            {
                EventType = BitacoraEventType.SystemStart,
                Action = "RF08_INICIADO",
                Details = $"Monitoreando: {storagePath}, Umbral preventivo: {config.PreventiveThresholdDays} días"
            });

            await _bitacora.LogAsync(new BitacoraEntry
            {
                EventType = BitacoraEventType.Information,
                Action = "RF08_PROCESSOR_STARTED",
                Details = "Event processor task iniciado",
                Result = "OK"
            });
        }
        catch (Exception ex)
        {
            IsRunning = false;
            await _bitacora.LogAsync(new BitacoraEntry
            {
                EventType = BitacoraEventType.Error,
                Action = "RF08_STARTUP_ERROR",
                Details = $"Error al iniciar monitoreo preventivo: {ex.Message}",
                Result = "ERROR"
            });
            throw;
        }
    }

    public async Task StopAsync()
    {
        if (!IsRunning) return;

        if (_watcher != null)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Created -= OnFileCreated;
            _watcher.Dispose();
            _watcher = null;
        }

        _cts?.Cancel();
        IsRunning = false;

        if (_processorTask != null)
        {
            try { await _processorTask; }
            catch (OperationCanceledException) { }
        }

        await _bitacora.LogAsync(new BitacoraEntry
        {
            EventType = BitacoraEventType.SystemStop,
            Action = "RF08_DETENIDO"
        });
    }

    private async void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        try
        {
            FilesDetected++;

            // Parsear la ruta para identificar Asset/Variable/Tipo
            var parsed = ParseFilePath(e.FullPath);
            if (parsed == null) return; // No es una ruta de datos críticos

            var args = new FileDetectedEventArgs
            {
                FilePath = e.FullPath,
                AssetId = parsed.Value.assetId,
                VariableId = parsed.Value.variableId,
                FolderType = parsed.Value.folderType,
                FileSize = GetFileSize(e.FullPath),
                DetectedAt = DateTime.Now
            };

            OnFileDetected?.Invoke(this, args);

            // Agregar a cola para procesar en batch y evitar hacer ScanAsync por cada archivo
            _eventQueue.Enqueue(e.FullPath);
        }
        catch (Exception ex)
        {
            await _bitacora.LogAsync(new BitacoraEntry
            {
                EventType = BitacoraEventType.Error,
                Action = "RF08_ERROR",
                Details = ex.Message,
                Result = "ERROR"
            });
        }
    }

    private async Task EvaluatePreventiveCleanupAsync(string assetId, string variableId)
    {
        var key = $"{assetId}/{variableId}";

        await _evaluationLock.WaitAsync();
        try
        {
            // Throttle: no evaluar más de 1 vez por minuto por Asset/Variable
            if (_lastEvaluation.TryGetValue(key, out var lastTime))
            {
                if (DateTime.Now - lastTime < TimeSpan.FromMinutes(1))
                    return;
            }
            _lastEvaluation[key] = DateTime.Now;
        }
        finally
        {
            _evaluationLock.Release();
        }

        if (_cts?.Token.IsCancellationRequested == true) return;

        // Escanear estado actual del storage
        var status = await _inventory.ScanAsync(_storagePath, _cts?.Token ?? CancellationToken.None);

        // Verificar si el uso actual ya supera el umbral
        if (status.UsagePercent < _config.ThresholdPercent * 0.9) // 90% del umbral como margen
            return;

        // Proyectar crecimiento inmediato
        var asset = status.Assets.FirstOrDefault(a => a.AssetId == assetId);
        if (asset == null) return;

        double dailyGrowth = asset.AverageDailyGrowthBytes;
        if (dailyGrowth <= 0) return;

        double bytesRemaining = status.TotalSpaceBytes * (_config.ThresholdPercent / 100.0) - status.UsedSpaceBytes;
        double daysUntilFull = bytesRemaining / dailyGrowth;

        if (daysUntilFull < _config.PreventiveThresholdDays)
        {
            // Ejecutar limpieza local
            var result = await _cleanup.ExecuteLocalCleanupAsync(
                assetId, variableId, status, _config, _cts?.Token ?? CancellationToken.None);

            if (result.Success)
            {
                PreventiveCleanups++;
                OnPreventiveCleanup?.Invoke(this, result);
            }
        }
    }

    /// <summary>
    /// Parsea una ruta de archivo para extraer Asset, Variable y tipo de carpeta.
    /// Ejemplo: D:\Data\Asset001\02\E\2026\02\17\data.bin → (Asset001, 02, E)
    /// </summary>
    private (string assetId, string variableId, string folderType)? ParseFilePath(string filePath)
    {
        try
        {
            var relativePath = Path.GetRelativePath(_storagePath, filePath);
            var parts = relativePath.Split(Path.DirectorySeparatorChar);

            // Esperamos al menos: Asset/Variable/E o F/YYYY/MM/DD/file
            if (parts.Length < 4) return null;

            var assetId = parts[0];
            var variableId = parts[1];
            var folderType = parts[2];

            if (folderType != "E" && folderType != "F")
                return null;

            return (assetId, variableId, folderType);
        }
        catch
        {
            return null;
        }
    }

    private static long GetFileSize(string path)
    {
        try
        {
            return new FileInfo(path).Length;
        }
        catch
        {
            return 0;
        }
    }

    // Procesador de eventos que agrupa eventos y ejecuta evaluación por Asset/Variable cada intervalo
    private async Task EventProcessorLoop(CancellationToken ct)
    {
        var grouped = new Dictionary<string, DateTime>();

        while (!ct.IsCancellationRequested)
        {
            try
            {
                // Esperar un intervalo breve para agrupar eventos
                await Task.Delay(TimeSpan.FromSeconds(5), ct);

                // Vaciar cola
                while (_eventQueue.TryDequeue(out var path))
                {
                    var parsed = ParseFilePath(path);
                    if (parsed == null) continue;
                    var key = $"{parsed.Value.assetId}/{parsed.Value.variableId}";
                    grouped[key] = DateTime.Now; // marcar último evento
                }

                // Procesar cada key (evaluación throttling interna evita ejecuciones muy seguidas)
                foreach (var kv in grouped.ToList())
                {
                    if (ct.IsCancellationRequested) break;
                    var parts = kv.Key.Split('/');
                    if (parts.Length != 2) continue;
                    await EvaluatePreventiveCleanupAsync(parts[0], parts[1]);
                }

                grouped.Clear();
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                await _bitacora.LogAsync(new BitacoraEntry
                {
                    EventType = BitacoraEventType.Error,
                    Action = "RF08_PROCESSOR_ERROR",
                    Details = ex.Message,
                    Result = "ERROR"
                });
            }
        }
    }
}
