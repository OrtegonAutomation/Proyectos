# Guía de Troubleshooting — FifoCleanup v1.0

**Proyecto:** Gestión de Almacenamiento FIFO  
**Cliente:** ODL Instrumentación y Control  
**Autor:** IDC Ingeniería  
**Fecha:** Marzo 2026

---

## Índice de Problemas

| # | Problema | Severidad | Ref. |
|---|---------|-----------|------|
| TS-01 | [La aplicación no inicia](#ts-01-la-aplicación-no-inicia) | Alta |
| TS-02 | [Error "Ruta de almacenamiento no existe"](#ts-02-error-ruta-de-almacenamiento-no-existe) | Alta |
| TS-03 | [El inventario no detecta Assets](#ts-03-el-inventario-no-detecta-assets) | Media |
| TS-04 | [El inventario tarda demasiado (>90s)](#ts-04-el-inventario-tarda-demasiado) | Media |
| TS-05 | [RF-07 no ejecuta limpieza](#ts-05-rf-07-no-ejecuta-limpieza) | Alta |
| TS-06 | [RF-08 no detecta archivos nuevos](#ts-06-rf-08-no-detecta-archivos-nuevos) | Alta |
| TS-07 | [RF-08 error al iniciar en nueva máquina](#ts-07-rf-08-error-al-iniciar-en-nueva-máquina) | Alta |
| TS-08 | [La limpieza no libera suficiente espacio](#ts-08-la-limpieza-no-libera-suficiente-espacio) | Media |
| TS-09 | [Error de permisos al eliminar carpetas](#ts-09-error-de-permisos-al-eliminar-carpetas) | Alta |
| TS-10 | [Bitácora no se crea o está vacía](#ts-10-bitácora-no-se-crea-o-está-vacía) | Media |
| TS-11 | [Archivo de configuración corrupto](#ts-11-archivo-de-configuración-corrupto) | Media |
| TS-12 | [Uso de CPU excesivo durante limpieza](#ts-12-uso-de-cpu-excesivo-durante-limpieza) | Media |
| TS-13 | [La simulación falla o no genera datos](#ts-13-la-simulación-falla-o-no-genera-datos) | Baja |
| TS-14 | [Gráficas del Dashboard no se muestran](#ts-14-gráficas-del-dashboard-no-se-muestran) | Baja |
| TS-15 | [Error "Validación de configuración falló"](#ts-15-error-validación-de-configuración-falló) | Media |

---

## TS-01: La aplicación no inicia

**Síntomas:** Doble clic en `FifoCleanup.exe` no hace nada; la aplicación se cierra inmediatamente; error de .NET.

**Causas posibles:**

| # | Causa | Solución |
|---|-------|----------|
| 1 | .NET 8.0 Desktop Runtime no instalado | Instalar desde https://dotnet.microsoft.com/download/dotnet/8.0 (Desktop Runtime, no solo Runtime) |
| 2 | Arquitectura incorrecta | Verificar que se instaló x64 (no x86) para servidores 64-bit |
| 3 | Archivo de config corrupto | Renombrar `fifo_config.json` → `fifo_config.json.bak` y reiniciar |
| 4 | DLLs faltantes | Verificar que todos los archivos de `publish/` están presentes |
| 5 | Permisos insuficientes | Ejecutar como administrador (clic derecho → Ejecutar como administrador) |

**Diagnóstico:**
```powershell
# Verificar .NET Runtime instalado
dotnet --list-runtimes

# Buscar: Microsoft.WindowsDesktop.App 8.0.x
# Si no aparece, instalar .NET 8.0 Desktop Runtime
```

---

## TS-02: Error "Ruta de almacenamiento no existe"

**Síntomas:** Mensaje de error al escanear, al activar RF-07/RF-08, o al abrir la aplicación.

**Causas posibles:**

| # | Causa | Solución |
|---|-------|----------|
| 1 | Disco no montado | Verificar que la unidad esté disponible en "Este equipo" |
| 2 | Ruta no existe | Crear la carpeta o corregir la ruta en Configuración |
| 3 | Letra de unidad cambió | Actualizar la ruta en Configuración → Guardar |
| 4 | Red no disponible | Si la ruta es de red (\\server\share), verificar conectividad |

**Verificación:**
```powershell
# Verificar que la ruta existe y es accesible
Test-Path "D:\MonitoringData"

# Verificar permisos
Get-Acl "D:\MonitoringData" | Format-List
```

---

## TS-03: El inventario no detecta Assets

**Síntomas:** Escaneo completa pero muestra 0 Assets o Assets incompletos.

**Causas posibles:**

| # | Causa | Solución |
|---|-------|----------|
| 1 | Ruta apunta al lugar incorrecto | La ruta debe apuntar a la carpeta que contiene las carpetas de Assets (ej: `D:\MonitoringData`, NO `D:\MonitoringData\Asset001`) |
| 2 | Estructura de carpetas no estándar | Verificar: `Asset/Variable/E|F/YYYY/MM/DD` |
| 3 | Carpetas E y F no existen | Solo se cuentan datos en subcarpetas E y F. Las carpetas de tendencias no se incluyen |
| 4 | Sin datos recientes | Si todas las carpetas de día son > 7 días, el crecimiento diario será 0 |

**Verificación:**
```powershell
# Listar Assets en la ruta
Get-ChildItem "D:\MonitoringData" -Directory

# Verificar estructura de un Asset
Get-ChildItem "D:\MonitoringData\Asset001" -Recurse -Depth 3 -Directory
```

---

## TS-04: El inventario tarda demasiado

**Síntomas:** Escaneo > 90 segundos, barra de progreso avanza muy lento.

**Causas posibles:**

| # | Causa | Solución |
|---|-------|----------|
| 1 | Disco bajo carga de I/O | Esperar a que el software de monitoreo termine escrituras intensivas |
| 2 | Millones de archivos pequeños | Normal para datasets grandes. El sistema escala linealmente |
| 3 | Disco fragmentado | Ejecutar defragmentación en horario de mantenimiento |
| 4 | Antivirus escaneando | Agregar la ruta de datos a exclusiones del antivirus |

**Mitigación:** El sistema usa `EnumerateFiles` (lazy evaluation) en lugar de `GetFiles` para minimizar uso de memoria.

---

## TS-05: RF-07 no ejecuta limpieza

**Síntomas:** RF-07 activo pero nunca limpia; la bitácora siempre muestra `RF07_EVALUACION_SKIP`.

**Causas posibles:**

| # | Causa | Solución |
|---|-------|----------|
| 1 | Proyección es segura | Si el uso proyectado no supera el umbral, RF-07 correctamente salta. Verificar que el umbral sea apropiado |
| 2 | Umbral muy alto | Si el umbral es 95% y el disco está al 90%, RF-07 no actuará hasta que esté al 95% |
| 3 | Sin datos de crecimiento | Si no hay datos de los últimos 7 días, la proyección es 0. Verificar que hay carpetas de día recientes |
| 4 | Frecuencia muy larga | Si la frecuencia es 24h, RF-07 solo evalúa 1 vez al día |

**Diagnóstico:**
1. Ir a Bitácora → filtrar por tipo `CleanupScheduled`
2. Buscar entradas `RF07_EVALUACION_SKIP` — el campo `Details` muestra la proyección y el umbral
3. Si la proyección siempre es < umbral, considerar reducir el umbral

---

## TS-06: RF-08 no detecta archivos nuevos

**Síntomas:** RF-08 activo pero el contador de "Archivos detectados" no incrementa.

**Causas posibles:**

| # | Causa | Solución |
|---|-------|----------|
| 1 | Archivos se crean fuera de la estructura esperada | RF-08 solo cuenta archivos en `Asset/Variable/E|F/YYYY/MM/DD` |
| 2 | `FileSystemWatcher` buffer overflow | Si se crean > miles de archivos/segundo, el buffer de 16 KB se llena. No es pérdida de datos, solo retraso |
| 3 | Ruta incorrecta | Verificar que RF-08 monitorea la ruta correcta (ver bitácora → `RF08_INICIADO`) |
| 4 | Permisos insuficientes | FileSystemWatcher requiere permisos de lectura en la carpeta |

---

## TS-07: RF-08 error al iniciar en nueva máquina

**Síntomas:** Al activar RF-08, error: "El directorio no existe" o `DirectoryNotFoundException`.

**Causa:** La ruta de almacenamiento en el archivo de configuración no existe en la nueva máquina.

**Solución detallada:** (Documentada en [SOLUCION_ERROR_RF08.md](../../src/FifoCleanup/docs/SOLUCION_ERROR_RF08.md))

1. Ir a pestaña **Configuración**
2. Verificar la **Ruta de almacenamiento**
3. Si la ruta no existe, actualizar a la ruta correcta en el nuevo servidor
4. Si la carpeta no existe pero debería, crearla:
   ```powershell
   New-Item -ItemType Directory -Path "D:\MonitoringData" -Force
   ```
5. Guardar configuración
6. Intentar activar RF-08 nuevamente

**Validaciones integradas:**
- `ConfigurationService.Validate()` verifica que la ruta exista
- `PreventiveMonitorService.StartAsync()` valida el directorio antes de crear el `FileSystemWatcher`
- `ExecutionViewModel` muestra error claro si el directorio no es accesible

---

## TS-08: La limpieza no libera suficiente espacio

**Síntomas:** Limpieza completa pero el uso del disco sigue alto.

**Causas posibles:**

| # | Causa | Solución |
|---|-------|----------|
| 1 | Cap de limpieza muy bajo | Aumentar `CleanupCapPercent` temporalmente (Configuración → 30-50%) |
| 2 | Datos fuera de la estructura FIFO | Si hay archivos grandes fuera de `Asset/Variable/E|F/`, FIFO no los toca. Limpieza manual necesaria |
| 3 | Datos muy recientes | FIFO solo elimina los más antiguos. Si los datos son de los últimos días, puede que no haya suficiente para eliminar |
| 4 | Muchas variables pequeñas | Si cada carpeta de día es pequeña (< 1 MB), se necesitan eliminar muchas para liberar espacio |

**Diagnóstico:**
```powershell
# Verificar qué ocupa espacio fuera de la estructura FIFO
Get-ChildItem "D:\MonitoringData" -Recurse | 
  Sort-Object Length -Descending | 
  Select-Object -First 20 FullName, @{N='SizeMB';E={$_.Length/1MB}}
```

---

## TS-09: Error de permisos al eliminar carpetas

**Síntomas:** Limpieza reporta errores `UnauthorizedAccessException` o `IOException`.

**Causas posibles:**

| # | Causa | Solución |
|---|-------|----------|
| 1 | Archivos en uso | El software de monitoreo tiene archivos abiertos. FIFO salta estos y continúa |
| 2 | Permisos NTFS | La cuenta de usuario no tiene permiso de eliminación. Otorgar Full Control sobre la carpeta de datos |
| 3 | Archivos de solo lectura | Algunos archivos están marcados como read-only |

**Solución:**
```powershell
# Verificar y otorgar permisos
$acl = Get-Acl "D:\MonitoringData"
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule(
    "$env:USERNAME", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
$acl.SetAccessRule($rule)
Set-Acl "D:\MonitoringData" $acl
```

⚠️ **Nota:** La limpieza FIFO es tolerante a errores: si no puede eliminar un archivo, salta al siguiente y registra el error en bitácora.

---

## TS-10: Bitácora no se crea o está vacía

**Síntomas:** Carpeta de bitácora vacía o inexistente; pestaña Bitácora muestra 0 entradas.

**Causas posibles:**

| # | Causa | Solución |
|---|-------|----------|
| 1 | Ruta de bitácora no creada | La ruta se crea al iniciar. Si no se creó, verificar permisos de escritura |
| 2 | Disco lleno | Si no hay espacio ni para la bitácora, el log falla silenciosamente |
| 3 | Retención muy agresiva | Si `BitacoraRetentionDays` es 1, los archivos se eliminan al día siguiente |

**Verificación:**
```powershell
# Verificar archivos de bitácora
Get-ChildItem "bitacora" -Filter "bitacora_*.csv"

# Verificar contenido del más reciente
Get-Content "bitacora\bitacora_$(Get-Date -Format 'yyyyMMdd').csv" -First 5
```

---

## TS-11: Archivo de configuración corrupto

**Síntomas:** Error al cargar configuración; valores incorrectos; la aplicación usa valores por defecto inesperados.

**Solución:**

1. Localizar `fifo_config.json` (ruta en la carpeta de la aplicación o donde se configuró)
2. Intentar abrirlo con un editor de texto
3. Si el JSON está malformado:
   - Renombrar a `fifo_config.json.corrupted`
   - Reiniciar la aplicación — creará uno nuevo con valores por defecto
   - Reconfigurar los parámetros

**Alternativa — restaurar manualmente:**
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

---

## TS-12: Uso de CPU excesivo durante limpieza

**Síntomas:** CPU > 25% durante limpieza; el software de monitoreo se vuelve lento.

**Causas posibles:**

| # | Causa | Solución |
|---|-------|----------|
| 1 | Throttle desactivado | Verificar `DeleteThrottleMs` > 0 (default: 50 ms) |
| 2 | Hilos de prioridad normal | Verificar `UseLowPriorityThreads` = true |
| 3 | Muchos Assets concurrentes | Reducir `MaxConcurrentAssets` a 1 |
| 4 | Antivirus escaneando archivos eliminados | Agregar exclusión en antivirus |

**Ajuste recomendado para servidor bajo carga:**
- `DeleteThrottleMs`: 100–200 ms
- `UseLowPriorityThreads`: true
- `MaxConcurrentAssets`: 1
- `EventBatchIntervalSeconds`: 30

---

## TS-13: La simulación falla o no genera datos

**Síntomas:** Error durante generación de datos sintéticos; simulación no completa.

**Causas posibles:**

| # | Causa | Solución |
|---|-------|----------|
| 1 | Sin espacio para datos sintéticos | La simulación necesita espacio temporal. Verificar espacio libre |
| 2 | Ruta de simulación no accesible | Verificar que la carpeta temporal existe y tiene permisos de escritura |
| 3 | Simulación cancelada | Si fue cancelada, los datos parciales persisten. Usar "Limpiar datos de prueba" |

---

## TS-14: Gráficas del Dashboard no se muestran

**Síntomas:** Área de gráficas vacía o con error de rendering.

**Causa probable:** LiveCharts requiere SkiaSharp, que necesita Visual C++ Redistributable.

**Solución:**
1. Instalar Visual C++ Redistributable (x64) desde https://aka.ms/vs/17/release/vc_redist.x64.exe
2. Reiniciar la aplicación

---

## TS-15: Error "Validación de configuración falló"

**Síntomas:** Al guardar configuración, se muestra lista de errores.

**Rangos válidos de cada parámetro:**

| Parámetro | Rango válido | Default |
|-----------|-------------|---------|
| ThresholdPercent | 50–95 | 85 |
| CleanupCapPercent | 5–50 | 20 |
| ScheduledFrequencyHours | 1–24 | 6 |
| ScheduledHour | 0–23 | 2 |
| PreventiveThresholdDays | 1–10 | 3 |
| MaxConcurrentAssets | 1–10 | 2 |
| MaxDaysToDeletePerAsset | 1–30 | 5 |
| BitacoraRetentionDays | 1–365 | 90 |
| StoragePath | Ruta existente | — |

Corregir el valor fuera de rango y guardar nuevamente.

---

## Registro de errores conocidos

| ID | Versión | Error | Estado | Solución |
|----|---------|-------|--------|----------|
| BUG-001 | 0.9 | RF-08 falla al iniciar si la ruta no existe | Resuelto v1.0 | 3 niveles de validación agregados |
| BUG-002 | 0.9 | Escaneo incluye carpetas de tendencias en el conteo | Resuelto v1.0 | Solo cuenta E y F |

