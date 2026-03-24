using FifoCleanup.Engine.Models;
using FifoCleanup.Engine.Services.Interfaces;

namespace FifoCleanup.Engine.Services;

/// <summary>
/// RF-07: Servicio de ejecución programada.
/// Evalúa periódicamente si se proyecta llenado de almacenamiento
/// y ejecuta limpieza general FIFO preventiva.
/// 
/// Algoritmo:
/// 1. Ejecutar inventario del storage
/// 2. Calcular promedio diario de crecimiento (7 días)
/// 3. Proyectar: usage_projected = current + avg_daily * hours_until_next_run
/// 4. Si usage_projected > threshold → ejecutar limpieza general
/// 5. Si usage_projected <= threshold → skip (log "seguro")
/// </summary>
public class ScheduledCleanupService : IScheduledCleanupService
{
    private readonly IInventoryService _inventory;
    private readonly ICleanupService _cleanup;
    private readonly IBitacoraService _bitacora;
    private CancellationTokenSource? _cts;
    private Task? _runningTask;
    private FifoConfiguration _config = new();
    private DateTime _lastCriticalAlertAt = DateTime.MinValue;
    private DateTime _lastErrorAlertAt = DateTime.MinValue;
    private DateTime _startedAt = DateTime.MinValue;
    private bool _startupWindowLogged;

    public bool IsRunning { get; private set; }
    public DateTime? NextScheduledRun { get; private set; }

    public event EventHandler<CleanupResult>? OnCleanupExecuted;
    public event EventHandler<string>? OnCleanupSkipped;

    public ScheduledCleanupService(
        IInventoryService inventory,
        ICleanupService cleanup,
        IBitacoraService bitacora)
    {
        _inventory = inventory;
        _cleanup = cleanup;
        _bitacora = bitacora;
    }

    public Task StartAsync(FifoConfiguration config, CancellationToken ct = default)
    {
        if (IsRunning) return Task.CompletedTask;

        _config = config;
        _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        IsRunning = true;
        _startedAt = DateTime.Now;
        _startupWindowLogged = false;

        _runningTask = Task.Run(() => RunLoopAsync(_cts.Token), _cts.Token);

        _ = _bitacora.LogAsync(new BitacoraEntry
        {
            EventType = BitacoraEventType.SystemStart,
            Action = "RF07_INICIADO",
            Details = $"Frecuencia: cada {config.ScheduledFrequencyHours}h, hora preferida: {config.ScheduledHour}:00"
        });

        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        if (!IsRunning) return;

        _cts?.Cancel();
        IsRunning = false;
        NextScheduledRun = null;

        if (_runningTask != null)
        {
            try { await _runningTask; }
            catch (OperationCanceledException) { }
        }

        await _bitacora.LogAsync(new BitacoraEntry
        {
            EventType = BitacoraEventType.SystemStop,
            Action = "RF07_DETENIDO"
        });
    }

    private async Task RunLoopAsync(CancellationToken ct)
    {
        // Reducir prioridad del hilo para no competir con el software de monitoreo
        if (_config.UseLowPriorityThreads)
        {
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
        }

        while (!ct.IsCancellationRequested)
        {
            try
            {
                // Calcular próxima ejecución
                NextScheduledRun = CalculateNextRun();

                var delay = NextScheduledRun.Value - DateTime.Now;
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, ct);
                }

                ct.ThrowIfCancellationRequested();

                // Ejecutar evaluación
                await EvaluateAndCleanupAsync(ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                await _bitacora.LogAsync(new BitacoraEntry
                {
                    EventType = BitacoraEventType.Error,
                    Action = "RF07_ERROR",
                    Details = ex.Message,
                    Result = "ERROR"
                });

                if (ShouldSendAlert(ref _lastErrorAlertAt, TimeSpan.FromMinutes(10)))
                {
                    await EmailAlertHelper.TrySendCriticalAlertAsync(
                        _config,
                        "[FIFO][CRITICO] RF-07 detenido por error",
                        $"RF-07 reportó error en servidor.\n\nDetalle: {ex.Message}\nFecha: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\nStoragePath: {_config.StoragePath}");
                }

                // Esperar 5 minutos antes de reintentar
                try { await Task.Delay(TimeSpan.FromMinutes(5), ct); }
                catch (OperationCanceledException) { break; }
            }
        }
    }

    private async Task EvaluateAndCleanupAsync(CancellationToken ct)
    {
        // 1. Inventario
        var status = await _inventory.ScanAsync(_config.StoragePath, ct);

        // Aplicar límite de espacio configurado
        status.ApplyStorageLimit(_config.MaxStorageSizeGB);

        // 2. Proyección
        double hoursUntilNext = _config.ScheduledFrequencyHours;
        double projectedGrowth = status.AverageDailyGrowthBytes * (hoursUntilNext / 24.0);
        double projectedUsageBytes = status.UsedSpaceBytes + projectedGrowth;
        double projectedPercent = status.TotalSpaceBytes > 0
            ? projectedUsageBytes / status.TotalSpaceBytes * 100.0
            : 0;

        // 3. Decidir
        if (IsInStartupMonitoringWindow())
        {
            if (!_startupWindowLogged)
            {
                _startupWindowLogged = true;
                var msg = $"RF-07 en ventana de gracia post-inicio ({_config.StartupMonitoringGraceMinutes} min). Monitoreo activo sin limpieza automática inmediata.";
                await _bitacora.LogAsync(new BitacoraEntry
                {
                    EventType = BitacoraEventType.CleanupScheduled,
                    Action = "RF07_STARTUP_MONITORING_ONLY",
                    Details = msg,
                    Result = "SKIPPED"
                });
                OnCleanupSkipped?.Invoke(this, msg);
            }

            return;
        }

        if (projectedPercent > _config.ThresholdPercent || status.UsagePercent > _config.ThresholdPercent)
        {
            if (ShouldSendAlert(ref _lastCriticalAlertAt, TimeSpan.FromMinutes(30)))
            {
                await EmailAlertHelper.TrySendCriticalAlertAsync(
                    _config,
                    "[FIFO][CRITICO] Umbral de almacenamiento sobrepasado",
                    $"Uso actual: {status.UsagePercent:F1}%\n" +
                    $"Uso proyectado: {projectedPercent:F1}%\n" +
                    $"Umbral: {_config.ThresholdPercent:F1}%\n" +
                    $"Fecha: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                    $"Servidor/StoragePath: {_config.StoragePath}");
            }

            // Ejecutar limpieza general
            var result = await _cleanup.ExecuteGeneralCleanupAsync(status, _config, ct);
            OnCleanupExecuted?.Invoke(this, result);
        }
        else
        {
            var msg = $"Proyección segura: {projectedPercent:F1}% (umbral: {_config.ThresholdPercent}%). " +
                      $"Crecimiento diario promedio: {FormatSize((long)status.AverageDailyGrowthBytes)}/día.";

            await _bitacora.LogAsync(new BitacoraEntry
            {
                EventType = BitacoraEventType.CleanupScheduled,
                Action = "RF07_EVALUACION_SKIP",
                Details = msg,
                Result = "SKIPPED"
            });

            OnCleanupSkipped?.Invoke(this, msg);
        }
    }

    private DateTime CalculateNextRun()
    {
        var now = DateTime.Now;
        var nextRun = new DateTime(now.Year, now.Month, now.Day, _config.ScheduledHour, 0, 0);

        if (nextRun <= now)
            nextRun = nextRun.AddHours(_config.ScheduledFrequencyHours);

        while (nextRun <= now)
            nextRun = nextRun.AddHours(_config.ScheduledFrequencyHours);

        return nextRun;
    }

    private static string FormatSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }

    private static bool ShouldSendAlert(ref DateTime lastSent, TimeSpan cooldown)
    {
        if (DateTime.Now - lastSent < cooldown)
            return false;

        lastSent = DateTime.Now;
        return true;
    }

    private bool IsInStartupMonitoringWindow()
    {
        if (_config.StartupMonitoringGraceMinutes <= 0)
            return false;

        return DateTime.Now - _startedAt < TimeSpan.FromMinutes(_config.StartupMonitoringGraceMinutes);
    }
}
