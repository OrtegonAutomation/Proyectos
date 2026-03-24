# Arquitectura del Sistema — FifoCleanup v1.0

**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Cliente:** ODL Instrumentación y Control  
**Autor:** IDC Ingeniería  
**Fecha:** Marzo 2026  

---

## 1. Vista General

FifoCleanup es una aplicación de escritorio **100% C# / .NET 8.0** que gestiona automáticamente el almacenamiento del servidor de monitoreo industrial. Utiliza arquitectura MVVM para la interfaz WPF y servicios desacoplados para la lógica de negocio.

### Diagrama de alto nivel

```
┌─────────────────────────────────────────────────────────────────┐
│                    FifoCleanup.UI (WPF)                         │
│  ┌──────────┬──────────┬──────────┬──────────┬──────────┐      │
│  │Dashboard │  Config  │Simulación│Ejecución │ Bitácora │      │
│  │ViewModel │ViewModel│ViewModel │ViewModel │ViewModel │      │
│  └─────┬────┴────┬─────┴────┬─────┴────┬─────┴────┬─────┘      │
│        │         │          │          │          │              │
│        └─────────┴──────────┴──────────┴──────────┘              │
│                           │                                      │
│                    App.xaml.cs                                    │
│              (Service Registration)                              │
└───────────────────────┬──────────────────────────────────────────┘
                        │ (Referencias de proyecto)
┌───────────────────────┴──────────────────────────────────────────┐
│                  FifoCleanup.Engine (.NET 8.0)                   │
│                                                                   │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │                         Services                             │ │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────────┐  │ │
│  │  │ Inventory    │  │ Configuration│  │   Cleanup        │  │ │
│  │  │ Service      │  │ Service      │  │   Service        │  │ │
│  │  │ (RF-01)      │  │ (RF-02)      │  │   (RF-04)        │  │ │
│  │  └──────────────┘  └──────────────┘  └──────────────────┘  │ │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────────┐  │ │
│  │  │ Simulation   │  │ Scheduled    │  │  Preventive      │  │ │
│  │  │ Service      │  │ Cleanup Svc  │  │  Monitor Svc     │  │ │
│  │  │ (RF-03)      │  │ (RF-07)      │  │  (RF-08)         │  │ │
│  │  └──────────────┘  └──────────────┘  └──────────────────┘  │ │
│  │  ┌──────────────┐                                           │ │
│  │  │ Bitacora     │                                           │ │
│  │  │ Service      │                                           │ │
│  │  │ (RF-05)      │                                           │ │
│  │  └──────────────┘                                           │ │
│  └─────────────────────────────────────────────────────────────┘ │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │                        Models                                │ │
│  │  FifoConfiguration │ AssetInfo │ StorageStatus              │ │
│  │  BitacoraEntry     │ SimulationModels │ CleanupResult       │ │
│  └─────────────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────────┘
                        │
                        ▼
┌──────────────────────────────────────────────────────────────────┐
│                     Sistema de Archivos                          │
│                                                                   │
│  D:\MonitoringData\                                              │
│  ├── Asset001\00\{E,F}\YYYY\MM\DD\*.bin                         │
│  ├── Asset002\01\{E,F}\YYYY\MM\DD\*.bin                         │
│  └── ...                                                         │
│                                                                   │
│  fifo_config.json          (Configuración)                       │
│  bitacora\bitacora_*.csv   (Auditoría)                           │
└──────────────────────────────────────────────────────────────────┘
```

---

## 2. Componentes

### 2.1 FifoCleanup.Engine (Class Library)

Librería sin dependencias de UI que contiene toda la lógica de negocio.

**Dependencias:** `System.Text.Json 8.0.5`

#### Models

| Clase | Responsabilidad |
|-------|----------------|
| `FifoConfiguration` | Configuración del sistema (17 propiedades). Persistido como JSON. Calibrado para Xeon E5-2695 v4. |
| `AssetInfo` | Información de un equipo monitoreado: ID, ruta, variables, tamaños, fechas, crecimiento. |
| `VariableInfo` | Punto de medición dentro de un Asset. Datos E y F. |
| `DayFolderInfo` | Carpeta de un día específico (`YYYY/MM/DD`). Unidad atómica de eliminación. |
| `StorageStatus` | Estado del disco: total/usado/libre, nivel semáforo, Assets, proyección. |
| `CleanupResult` | Resultado de limpieza: bytes liberados, carpetas/archivos eliminados, antes/después. |
| `BitacoraEntry` | Entrada de auditoría con serialización CSV. 12 tipos de evento. |
| `SimulationParams` | Parámetros para generación de datos sintéticos. |
| `ContinuousSimulationParams` | Parámetros para simulación de crecimiento en tiempo real. |
| `SimulationResult` | Resultado completo de simulación: antes/después/limpieza. |

#### Services

| Servicio | Interfaz | RF | Descripción |
|----------|----------|-----|------------|
| `InventoryService` | `IInventoryService` | RF-01 | Escanea `Asset/Variable/E|F/YYYY/MM/DD`, calcula tamaños y crecimiento |
| `ConfigurationService` | `IConfigurationService` | RF-02 | Carga/guarda JSON, validación de rangos, valores por defecto |
| `SimulationService` | `ISimulationService` | RF-03 | Genera datos sintéticos en disco, ejecuta simulación con dry run |
| `CleanupService` | `ICleanupService` | RF-04 | Limpieza FIFO general y local, preview, throttling de I/O |
| `BitacoraService` | `IBitacoraService` | RF-05 | Auditoría CSV append-only, rotación 100 MB, filtrado, exportación |
| `ScheduledCleanupService` | `IScheduledCleanupService` | RF-07 | Timer loop: inventario → proyección → limpieza general si threshold |
| `PreventiveMonitorService` | `IPreventiveMonitorService` | RF-08 | FileSystemWatcher → queuing → evaluación → limpieza local |

### 2.2 FifoCleanup.UI (WPF Application)

**Framework:** .NET 8.0-windows con WPF  
**Patrón:** MVVM (CommunityToolkit.Mvvm 8.3.2)  
**Gráficas:** LiveChartsCore.SkiaSharpView.WPF 2.0.0-rc3.3

#### ViewModels

| ViewModel | Pestaña | Responsabilidad |
|-----------|---------|----------------|
| `MainViewModel` | — | Navegación raíz entre pestañas |
| `DashboardViewModel` | Dashboard | Escaneo, visualización de estado, gráficas |
| `ConfigurationViewModel` | Configuración | Formulario de configuración, validación, guardado |
| `SimulationViewModel` | Simulación | Control de simulación puntual y continua |
| `ExecutionViewModel` | Ejecución | Control de limpieza manual, RF-07, RF-08 |
| `BitacoraViewModel` | Bitácora | Consulta, filtrado y exportación de auditoría |

#### Inyección de dependencias

La aplicación usa **"poor-man's DI"** (instanciación directa en `App.xaml.cs`) para mantener simplicidad en un entorno de recursos limitados:

```csharp
public static IConfigurationService ConfigService { get; } = new ConfigurationService();
public static IBitacoraService BitacoraService { get; } = new BitacoraService();
public static IInventoryService InventoryService { get; } = new InventoryService();
public static ICleanupService CleanupService { get; private set; } = null!;
// ... inicializados en OnStartup con dependencias
```

#### Manejo de excepciones global

Tres niveles de captura para evitar cierres inesperados:
1. `DispatcherUnhandledException` — Errores en hilo de UI
2. `AppDomain.UnhandledException` — Errores en hilos de dominio
3. `TaskScheduler.UnobservedTaskException` — Errores en tareas async no observadas

Todos registran el error en la bitácora y permiten que la aplicación continúe.

### 2.3 FifoCleanup.Tests (Console App)

Framework de pruebas **propio** (no xUnit/NUnit) implementado como aplicación de consola.

| Componente | Función |
|------------|---------|
| `TestCase` | Modelo con nombre, categoría, acción async, resultado |
| `TestContext` | Ambiente compartido de prueba (paths, servicios) |
| `ReportGenerator` | Genera reportes TSV |
| `ExcelExporter` | Genera reportes Excel (ClosedXML) |
| 11 TestSuites | 89 tests cubriendo RF-01 a RF-08 + edge cases |

---

## 3. Flujos de datos principales

### 3.1 Inventario (RF-01)

```
Usuario → Dashboard → "Escanear"
  → InventoryService.ScanAsync(storagePath)
    → Enumera Assets (nivel 1)
      → Para cada Asset: enumera Variables
        → Para cada Variable: busca E/ y F/
          → Para cada E|F: escanea YYYY/MM/DD
            → Calcula tamaño por DayFolder
    → Calcula proporciones, crecimiento, proyección
  → StorageStatus ← retornado al ViewModel
  → Dashboard actualiza gráficas y semáforo
```

### 3.2 Limpieza General FIFO (RF-04 / RF-07)

```
Trigger: Manual ó ScheduledCleanupService timer
  → CleanupService.ExecuteGeneralCleanupAsync(status, config)
    → Calcula bytes a liberar (umbral - 5%)
    → Aplica cap de limpieza
    → GetFifoOrder(): ordena TODAS las DayFolders por fecha ASC
    → Para cada DayFolder (más antigua primero):
      → DeleteDayFolderAsync(path)
        → Elimina archivos
        → Elimina carpetas vacías
        → Throttle (50ms pause)
      → Suma bytes liberados
      → Si bytesFreed >= bytesToFree → STOP
    → BitacoraService.LogAsync(resultado)
  → CleanupResult ← retornado
```

### 3.3 Monitoreo Preventivo (RF-08)

```
FileSystemWatcher detecta archivo nuevo
  → OnFileCreated callback
    → ParseFilePath → (AssetId, VariableId, FolderType)
    → Enqueue en ConcurrentQueue
  → EventProcessorLoop (cada 10s):
    → Dequeue y agrupar por Asset/Variable
    → Para cada grupo:
      → EvaluatePreventiveCleanupAsync()
        → Throttle: max 1 evaluación/minuto por Asset
        → InventoryService.ScanAsync()
        → Si uso > 90% del umbral:
          → Calcular proyección de llenado
          → Si daysUntilFull < PreventiveThresholdDays:
            → CleanupService.ExecuteLocalCleanupAsync()
              → Elimina N días más antiguos del Asset/Variable afectado
```

### 3.4 Persistencia de configuración

```
ConfigurationService
  → LoadAsync(path): File.ReadAllText → JsonSerializer.Deserialize<FifoConfiguration>
  → SaveAsync(config, path): JsonSerializer.Serialize → File.WriteAllText
  → Validate(config): Lista de errores con rangos específicos
  → GetDefault(): Valores calibrados para SRVODLRTDNMICA
```

---

## 4. Decisiones de arquitectura

Las decisiones arquitectónicas están documentadas en los ADRs:

| ADR | Decisión | Estado |
|-----|----------|--------|
| [ADR-0001](Decisiones_arquitectura/ADR_0001_Arquitectura_WPF_CPP.md) | Arquitectura 100% C#/.NET 8.0 con WPF (actualizado desde diseño inicial WPF+C++) | Implementada |
| [ADR-0002](Decisiones_arquitectura/ADR_0002_Estrategia_FIFO_Dual_RF07_RF08.md) | Estrategia FIFO dual: RF-07 (programada global) + RF-08 (preventiva local) | Implementada |
| [ADR-0003](Decisiones_arquitectura/ADR_0003_Formato_Configuracion_INI.md) | Formato JSON unificado para configuración (actualizado desde diseño INI+JSON) | Implementada |
| [ADR-0004](Decisiones_arquitectura/ADR_0004_Bitacora_Auditoria_Inmutable.md) | Bitácora CSV append-only con rotación 100 MB | Implementada |
| [ADR-0005](Decisiones_arquitectura/ADR_0005_Eliminacion_Por_Lotes_Con_Validacion.md) | Eliminación por lotes con throttling y cap | Implementada |
| [ADR-0006](Decisiones_arquitectura/ADR_0006_Sistema_Alarmas_Multicanal.md) | Sistema de alarmas multicanal | Diferida a v2.0 |
| [ADR-0007](Decisiones_arquitectura/ADR_0007_Simulacion_Deterministica.md) | Simulación determinística con datos sintéticos | Implementada |

### Cambios respecto al diseño inicial

1. **ADR-0001:** El diseño original contemplaba un motor C++ nativo para I/O de alto rendimiento. Durante la implementación se determinó que .NET 8.0 con `EnumerateFiles` y threading manejado cumple los requisitos de rendimiento (< 60s para 500 GB) sin la complejidad de comunicación interprocesos. Se adoptó **100% C#/.NET 8.0**.

2. **ADR-0003:** El diseño original contemplaba INI para políticas de operador y JSON para UI. Se simplificó a **JSON unificado** (`fifo_config.json`) porque: (a) la interfaz WPF hace innecesaria la edición manual de archivos, (b) JSON tiene tipado fuerte nativo, (c) un solo parser `System.Text.Json` simplifica el mantenimiento.

---

## 5. Servidor destino

| Característica | Valor |
|---------------|-------|
| Servidor | SRVODLRTDNMICA |
| CPU | Intel Xeon E5-2695 v4 @ 2.10 GHz (18C/36T) |
| RAM | 16 GB (≈50% ocupado por monitoreo → ≈8 GB libres) |
| OS | Windows Server |
| Framework | .NET 8.0 Runtime |

### Ajustes de rendimiento para servidor compartido

- `UseLowPriorityThreads = true` — Hilos de limpieza con prioridad `BelowNormal`
- `MaxConcurrentAssets = 2` — Limita paralelismo para no competir
- `DeleteThrottleMs = 50` — Pausa entre eliminaciones para reducir picos de I/O
- `EventBatchIntervalSeconds = 10` — Agrupa eventos RF-08 para reducir evaluaciones

---

## 6. Dependencias externas

| Paquete | Versión | Proyecto | Propósito |
|---------|---------|----------|-----------|
| System.Text.Json | 8.0.5 | Engine | Serialización de configuración |
| CommunityToolkit.Mvvm | 8.3.2 | UI | MVVM source generators ([ObservableProperty], [RelayCommand]) |
| LiveChartsCore.SkiaSharpView.WPF | 2.0.0-rc3.3 | UI | Gráficas del Dashboard |
| ClosedXML | — | Tests | Exportación de reportes a Excel |

---

## 7. Funcionalidades pendientes (v2.0)

| Funcionalidad | RF | Estado |
|--------------|-----|--------|
| Sistema de alarmas (email, syslog, Event Log) | RF-06 | Planeado |
| Control de acceso basado en roles (RBAC) | RF-09 | Planeado |
| Lista blanca de rutas protegidas | RF-02 ext | Planeado |
| Ejecución como servicio de Windows | — | Planeado |
| Patrones de exclusión con wildcards | RF-02 ext | Planeado |
| Versionado automático de configuración | RF-02 ext | Planeado |
