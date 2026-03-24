# Especificación Técnica — FifoCleanup v1.0

**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Cliente:** ODL Instrumentación y Control  
**Autor:** IDC Ingeniería  
**Fecha:** Marzo 2026

---

## 1. Parámetros de configuración

Todos los parámetros se almacenan en un archivo JSON (`fifo_config.json`) con naming policy `camelCase`.

### 1.1 Tabla completa de parámetros

| Parámetro | Tipo | Rango | Default | Descripción |
|-----------|------|-------|---------|-------------|
| `storagePath` | string | Ruta existente | `""` | Ruta raíz de los Assets de monitoreo (ej: `D:\MonitoringData`) |
| `maxStorageSizeGB` | double | 0+ | `0` | Espacio máximo asignado en GB. Si 0, usa el disco real |
| `thresholdPercent` | double | 50–95 | `85.0` | Porcentaje de uso que dispara la limpieza |
| `cleanupCapPercent` | double | 5–50 | `20.0` | Máximo % de datos monitoreados a eliminar por ejecución |
| `scheduledFrequencyHours` | int | 1–24 | `6` | Frecuencia de evaluación RF-07 en horas |
| `scheduledHour` | int | 0–23 | `2` | Hora preferida para la primera ejecución RF-07 (formato 24h) |
| `preventiveThresholdDays` | int | 1–10 | `3` | Días de proyección RF-08: si se llena en < N días, limpia |
| `enableScheduledCleanup` | bool | — | `true` | Habilitar servicio RF-07 |
| `enablePreventiveCleanup` | bool | — | `true` | Habilitar servicio RF-08 |
| `maxConcurrentAssets` | int | 1–10 | `2` | Máximo de Assets procesados simultáneamente |
| `maxDaysToDeletePerAsset` | int | 1–30 | `5` | Máximo días a eliminar por Asset en limpieza local RF-08 |
| `configFilePath` | string | — | `"fifo_config.json"` | Ruta del propio archivo de configuración |
| `bitacoraPath` | string | — | `"bitacora"` | Ruta del directorio de bitácora |
| `bitacoraRetentionDays` | int | 1–365 | `90` | Días de retención de archivos de bitácora |
| `bitacoraMaxSizeMB` | int | — | `100` | Tamaño máximo por archivo de bitácora antes de rotación (MB) |
| `eventBatchIntervalSeconds` | int | 5–60 | `10` | Intervalo de agrupación de eventos RF-08 en segundos |
| `useLowPriorityThreads` | bool | — | `true` | Ejecutar hilos de limpieza con prioridad `BelowNormal` |
| `deleteThrottleMs` | int | 0+ | `50` | Pausa entre eliminaciones consecutivas en milisegundos |

### 1.2 Ejemplo de archivo de configuración

```json
{
  "storagePath": "D:\\MonitoringData",
  "maxStorageSizeGB": 0,
  "thresholdPercent": 85,
  "cleanupCapPercent": 20,
  "scheduledFrequencyHours": 6,
  "scheduledHour": 2,
  "preventiveThresholdDays": 3,
  "enableScheduledCleanup": true,
  "enablePreventiveCleanup": true,
  "maxConcurrentAssets": 2,
  "maxDaysToDeletePerAsset": 5,
  "configFilePath": "fifo_config.json",
  "bitacoraPath": "bitacora",
  "bitacoraRetentionDays": 90,
  "bitacoraMaxSizeMB": 100,
  "eventBatchIntervalSeconds": 10,
  "useLowPriorityThreads": true,
  "deleteThrottleMs": 50
}
```

### 1.3 Validación

`ConfigurationService.Validate()` retorna una lista de strings con los errores detectados:

| Regla | Mensaje de error |
|-------|-----------------|
| `StoragePath` vacío | "La ruta de almacenamiento es requerida." |
| `StoragePath` no existe | "La ruta '{path}' no existe." |
| `ThresholdPercent` fuera de 50–95 | "El umbral debe estar entre 50% y 95%." |
| `CleanupCapPercent` fuera de 5–50 | "El cap de limpieza debe estar entre 5% y 50%." |
| `ScheduledFrequencyHours` fuera de 1–24 | "La frecuencia programada debe estar entre 1 y 24 horas." |
| `ScheduledHour` fuera de 0–23 | "La hora programada debe estar entre 0 y 23." |
| `PreventiveThresholdDays` fuera de 1–10 | "El umbral preventivo debe estar entre 1 y 10 días." |
| `MaxConcurrentAssets` fuera de 1–10 | "El máximo de Assets concurrentes debe estar entre 1 y 10." |
| `MaxDaysToDeletePerAsset` fuera de 1–30 | "El máximo de días a eliminar por Asset debe estar entre 1 y 30." |
| `BitacoraRetentionDays` fuera de 1–365 | "La retención de bitácora debe estar entre 1 y 365 días." |

---

## 2. Estructura de datos del sistema de archivos

### 2.1 Estructura esperada

```
{StoragePath}/
├── {AssetId}/                     ← Equipo monitoreado
│   ├── {VariableId}/              ← Variable numérica (00, 01, 02...)
│   │   ├── E/                     ← Datos de eventos
│   │   │   └── {YYYY}/           
│   │   │       └── {MM}/         
│   │   │           └── {DD}/      ← Carpeta de día (unidad atómica FIFO)
│   │   │               ├── data_000.bin
│   │   │               └── data_001.bin
│   │   ├── F/                     ← Datos de frecuencia
│   │   │   └── {YYYY}/{MM}/{DD}/
│   │   └── {YYYY}/               ← Tendencias (NO se tocan)
│   └── {VariableId}/
└── {AssetId}/
```

### 2.2 Reglas de parsing

- **AssetId:** Nombre de la carpeta de primer nivel bajo `StoragePath`
- **VariableId:** Nombre de la carpeta de segundo nivel (numéricas)
- **FolderType:** Solo `E` y `F` son considerados datos críticos
- **Año:** 4 dígitos, rango 2000–2100
- **Mes:** 2 dígitos, rango 01–12
- **Día:** 2 dígitos, rango 01–31 (se valida con `DateTime` constructor)
- **Fechas inválidas** (ej: 30 de febrero) se ignoran silenciosamente
- **Carpetas de tendencias** (ej: `2026/` directamente bajo la variable) se ignoran

### 2.3 Cálculo de tamaños

- El tamaño de una `DayFolder` = suma de todos los archivos recursivamente, usando `DirectoryInfo.EnumerateFiles("*", SearchOption.AllDirectories)`
- Archivos inaccesibles (`UnauthorizedAccessException`, `IOException`) se saltan sin error
- El tamaño de un `Variable` = suma de `DayFolders` en E + F
- El tamaño de un `Asset` = suma de todas las `Variables`
- `MonitoredDataBytes` = suma de todos los `Assets`
- `FillProportion` = `Asset.TotalSize / MonitoredDataBytes * 100`

---

## 3. Interfaces de servicio

### 3.1 IInventoryService

```csharp
Task<StorageStatus> ScanAsync(
    string storagePath,
    CancellationToken ct = default,
    IProgress<(string message, double percent)>? progress = null);

Task<AssetInfo?> ScanAssetAsync(string assetPath, CancellationToken ct = default);

StorageStatus GetDriveInfo(string path);

double CalculateAverageDailyGrowth(List<DayFolderInfo> dayFolders, int lookbackDays = 7);
```

### 3.2 IConfigurationService

```csharp
Task<FifoConfiguration> LoadAsync(string path);
Task SaveAsync(FifoConfiguration config, string path);
List<string> Validate(FifoConfiguration config);
FifoConfiguration GetDefault();
```

### 3.3 ICleanupService

```csharp
Task<CleanupResult> ExecuteGeneralCleanupAsync(
    StorageStatus status,
    FifoConfiguration config,
    CancellationToken ct = default,
    IProgress<(string message, double percent)>? progress = null,
    bool dryRun = false);

Task<CleanupResult> ExecuteLocalCleanupAsync(
    string assetId,
    string variableId,
    StorageStatus status,
    FifoConfiguration config,
    CancellationToken ct = default);

Task<CleanupResult> PreviewCleanupAsync(
    StorageStatus status,
    FifoConfiguration config,
    CancellationToken ct = default);

List<DayFolderInfo> GetFifoOrder(List<AssetInfo> assets);
```

### 3.4 IBitacoraService

```csharp
Task InitializeAsync(string bitacoraPath);
Task LogAsync(BitacoraEntry entry);

Task<List<BitacoraEntry>> GetEntriesAsync(
    DateTime? from = null,
    DateTime? to = null,
    BitacoraEventType? type = null,
    string? assetId = null,
    int? limit = null);

Task<string> ExportToCsvAsync(string outputPath, DateTime? from = null, DateTime? to = null);
Task MaintenanceAsync(int retentionDays, int maxFileSizeMB);
```

### 3.5 ISimulationService

```csharp
Task<bool> GenerateSyntheticDataAsync(
    SimulationParams parameters,
    CancellationToken ct = default,
    IProgress<(string message, double percent)>? progress = null);

Task<SimulationResult> RunSimulationAsync(
    SimulationParams parameters,
    CancellationToken ct = default,
    IProgress<(string message, double percent)>? progress = null);

Task CleanupSyntheticDataAsync(string simulationPath);
Task StartContinuousSimulationAsync(ContinuousSimulationParams parameters, CancellationToken ct = default);
Task StopContinuousSimulationAsync();

bool IsContinuousSimulationRunning { get; }
long ContinuousDataGeneratedMB { get; }
event EventHandler<(string assetId, string variableId, long byteGenerated)>? OnContinuousDataGenerated;
```

### 3.6 IScheduledCleanupService

```csharp
Task StartAsync(FifoConfiguration config, CancellationToken ct = default);
Task StopAsync();
bool IsRunning { get; }
DateTime? NextScheduledRun { get; }
event EventHandler<CleanupResult>? OnCleanupExecuted;
event EventHandler<string>? OnCleanupSkipped;
```

### 3.7 IPreventiveMonitorService

```csharp
Task StartAsync(string storagePath, FifoConfiguration config, CancellationToken ct = default);
Task StopAsync();
bool IsRunning { get; }
long FilesDetected { get; }
int PreventiveCleanups { get; }
event EventHandler<FileDetectedEventArgs>? OnFileDetected;
event EventHandler<CleanupResult>? OnPreventiveCleanup;
```

---

## 4. Formato de bitácora CSV

### 4.1 Header

```
"Timestamp","EventType","Action","AssetId","VariableId","Details","BytesAffected","Result","Source"
```

### 4.2 Campos

| Campo | Tipo | Formato | Ejemplo |
|-------|------|---------|---------|
| Timestamp | DateTime | `yyyy-MM-dd HH:mm:ss` | `2026-02-23 14:30:45` |
| EventType | Enum | String | `CleanupScheduled` |
| Action | String | Libre | `LIMPIEZA_GENERAL_FIFO` |
| AssetId | String | ID del asset | `Asset001` (vacío si global) |
| VariableId | String | ID de variable | `02` (vacío si aplica a todo el asset) |
| Details | String | Descripción | `Se liberaron 15.3 GB eliminando 45 carpetas` |
| BytesAffected | long | Entero | `16424673280` |
| Result | String | Resultado | `OK`, `ERROR`, `CANCELLED`, `SKIPPED` |
| Source | String | Origen | `SYSTEM`, `USER`, nombre de servicio |

### 4.3 Tipos de evento (BitacoraEventType)

| Valor | Código | Descripción |
|-------|--------|-------------|
| 0 | `Information` | Información general |
| 1 | `Inventory` | Operación de inventario |
| 2 | `CleanupManual` | Limpieza manual |
| 3 | `CleanupScheduled` | Limpieza RF-07 |
| 4 | `CleanupPreventive` | Limpieza RF-08 |
| 5 | `Simulation` | Operación de simulación |
| 6 | `Configuration` | Cambio de configuración |
| 7 | `Alarm` | Alarma (futuro) |
| 8 | `Error` | Error del sistema |
| 9 | `SystemStart` | Inicio de servicio |
| 10 | `SystemStop` | Detención de servicio |
| 11 | `FileDetected` | Archivo detectado por RF-08 |

### 4.4 Rotación y retención

- **Archivo diario:** `bitacora_YYYYMMDD.csv`
- **Rotación por tamaño:** Si supera `BitacoraMaxSizeMB` (100 MB), se crea `bitacora_YYYYMMDD_HHmmss.csv`
- **Retención:** Archivos con `CreationTime` > `BitacoraRetentionDays` (90 días) se eliminan
- **Thread safety:** `SemaphoreSlim(1, 1)` para escrituras concurrentes

---

## 5. Algoritmos principales

### 5.1 Orden FIFO

```
GetFifoOrder(assets) =
  assets
    .SelectMany(asset → asset.Variables.SelectMany(var → var.DayFolders))
    .OrderBy(folder → folder.Date)        ← Más antiguo primero
    .ThenBy(folder → folder.AssetId)       ← Desempate por Asset
    .ThenBy(folder → folder.VariableId)    ← Desempate por Variable
```

### 5.2 Cálculo de bytes a liberar

```
targetUsage = ThresholdPercent - 5%        ← Bajar 5% debajo del umbral
bytesToFree = UsedSpaceBytes - (TotalSpaceBytes × targetUsage / 100)
maxBytesToFree = MonitoredDataBytes × CleanupCapPercent / 100
bytesToFree = min(bytesToFree, maxBytesToFree)
```

### 5.3 Proyección RF-07

```
projectedGrowth = AverageDailyGrowthBytes × (ScheduledFrequencyHours / 24)
projectedUsageBytes = UsedSpaceBytes + projectedGrowth
projectedPercent = projectedUsageBytes / TotalSpaceBytes × 100

Si projectedPercent > ThresholdPercent → EJECUTAR limpieza
Si no → SKIP (log y esperar)
```

### 5.4 Evaluación RF-08

```
Si UsagePercent < ThresholdPercent × 0.90 → no evaluar (margen 10%)
dailyGrowth = Asset.AverageDailyGrowthBytes
bytesRemaining = TotalSpaceBytes × (ThresholdPercent / 100) - UsedSpaceBytes
daysUntilFull = bytesRemaining / dailyGrowth

Si daysUntilFull < PreventiveThresholdDays → EJECUTAR limpieza local
```

### 5.5 Semáforo StorageLevel

```
UsagePercent < 70%  → Green
70% ≤ UsagePercent < 85%  → Yellow
UsagePercent ≥ 85%  → Red
```

### 5.6 ApplyStorageLimit

```
Si MaxStorageSizeGB > 0:
  TotalSpaceBytes = MaxStorageSizeGB × 1024³
  UsedSpaceBytes = MonitoredDataBytes     ← Solo datos FIFO
  FreeSpaceBytes = max(0, TotalSpaceBytes - UsedSpaceBytes)
```

---

## 6. Crecimiento diario promedio

```csharp
CalculateAverageDailyGrowth(dayFolders, lookbackDays = 7):
  1. Filtrar carpetas con fecha ≥ (hoy - 7 días)
  2. Agrupar por fecha (día)
  3. Sumar tamaños por día
  4. Promediar los totales diarios
  → Retorna bytes/día promedio
```

