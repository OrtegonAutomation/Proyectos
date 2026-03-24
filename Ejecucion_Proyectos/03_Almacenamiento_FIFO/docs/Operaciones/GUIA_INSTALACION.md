# Guía de Instalación y Despliegue — FifoCleanup v1.1

**Proyecto:** Gestión de Almacenamiento FIFO  
**Cliente:** ODL Instrumentación y Control  
**Autor:** IDC Ingeniería  
**Fecha:** Marzo 2026  
**Servidor destino:** SRVODLRTDNMICA

---

## 1. Requisitos previos

### Hardware (mínimo)

| Componente | Requisito mínimo | Servidor SRVODLRTDNMICA |
|------------|------------------|------------------------|
| CPU | Dual core x64 | Intel Xeon E5-2695 v4 (18C/36T) ✅ |
| RAM | 4 GB (2 GB disponibles) | 16 GB (≈8 GB libres) ✅ |
| Disco | 100 MB para la aplicación | Disponible ✅ |
| OS | Windows 10/11 o Server 2016+ | Windows Server ✅ |

### Software

| Software | Requerido | Descarga |
|----------|-----------|----------|
| **.NET 8.0 Desktop Runtime** (x64) | ✅ Obligatorio | https://dotnet.microsoft.com/download/dotnet/8.0 |
| Visual C++ Redistributable (x64) | Recomendado (para LiveCharts/SkiaSharp) | https://aka.ms/vs/17/release/vc_redist.x64.exe |

### Permisos

| Permiso | Necesario para |
|---------|---------------|
| Lectura/escritura en carpeta de datos (ej: `D:\MonitoringData`) | Inventario y limpieza FIFO |
| Lectura/escritura en carpeta de instalación | Configuración y bitácora |
| Permisos de eliminación en subcarpetas de datos | Limpieza FIFO efectiva |

---

## 2. Instalación de .NET 8.0 Runtime

### Opción A: Instalación online

1. Abrir navegador en el servidor
2. Ir a https://dotnet.microsoft.com/download/dotnet/8.0
3. En la sección **".NET Desktop Runtime 8.0.x"**, descargar el instalador **Windows x64**
4. Ejecutar el instalador → Siguiente → Instalar → Cerrar
5. Verificar:
   ```powershell
   dotnet --list-runtimes
   # Buscar: Microsoft.WindowsDesktop.App 8.0.x
   ```

### Opción B: Instalación offline

Si el servidor no tiene acceso a internet:
1. Descargar `.NET 8.0 Desktop Runtime (x64)` en otra máquina
2. Copiar el instalador al servidor vía USB o red
3. Ejecutar el instalador en el servidor

---

## 3. Instalación de FifoCleanup

> **Recomendado v1.1:** usar paquete `FIFO_Release_YYYYMMDD` y scripts de instalación (`02_Scripts`).

### 3.1 Desde archivos publicados (producción)

1. Obtener la carpeta `publish/` con los archivos compilados de FifoCleanup
2. Copiar la carpeta completa al servidor:
   ```
   C:\FifoCleanup\
   ├── FifoCleanup.exe
   ├── FifoCleanup.dll
   ├── FifoCleanup.Engine.dll
   ├── [dependencias *.dll]
   └── FifoCleanup.runtimeconfig.json
   ```
3. Verificar que todos los archivos se copiaron correctamente

### 3.1.1 Instalación con scripts (recomendado)

```powershell
$pkg = "C:\Deploy\FIFO_Release_20260316"
powershell -ExecutionPolicy Bypass -File "$pkg\02_Scripts\01_Instalar_FIFO.ps1" -SourceReleasePath $pkg -InstallPath "C:\Apps\FIFO"
powershell -ExecutionPolicy Bypass -File "$pkg\02_Scripts\02_SmokeTest_FIFO.ps1" -InstallPath "C:\Apps\FIFO" -WaitSeconds 12
```

**Sin internet:**
- No afecta instalación local.
- Para alertas por correo, usar relay SMTP interno o mantener `enableEmailAlerts=false` (valor recomendado actual).

### 3.2 Desde código fuente (desarrollo)

1. Clonar/copiar el repositorio al servidor o máquina de desarrollo
2. Instalar .NET 8.0 SDK (no solo Runtime)
3. Compilar y publicar:
   ```powershell
   cd src\FifoCleanup
   dotnet restore
   dotnet publish FifoCleanup.UI -c Release -o C:\FifoCleanup
   ```
4. La carpeta `C:\FifoCleanup\` contendrá todos los archivos necesarios

---

## 4. Configuración inicial del servidor

### 4.1 Verificar acceso a la carpeta de datos

```powershell
# Verificar que la carpeta de datos existe
Test-Path "D:\MonitoringData"

# Verificar que hay Assets
Get-ChildItem "D:\MonitoringData" -Directory | Select-Object Name

# Verificar permisos
$acl = Get-Acl "D:\MonitoringData"
$acl.Access | Format-Table IdentityReference, FileSystemRights, AccessControlType
```

### 4.2 Primer inicio

1. Navegar a `C:\FifoCleanup\`
2. Ejecutar `FifoCleanup.exe`
3. La aplicación abre con el Dashboard vacío
4. Ir a **Configuración**:
   - **Ruta de almacenamiento:** `D:\MonitoringData` (o la ruta real)
   - Verificar que la validación pasa (sin errores)
5. Clic en **Guardar**
6. Ir a **Dashboard** → **Escanear**
7. Verificar que detecta los Assets correctamente

### 4.3 Crear archivo de configuración manualmente (opcional)

Si necesita preconfigurar antes del primer inicio:

```powershell
# Crear fifo_config.json
@'
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
'@ | Set-Content "C:\FifoCleanup\fifo_config.json" -Encoding UTF8
```

---

## 5. Inicio automático con Windows

Para que FifoCleanup inicie automáticamente al reiniciar el servidor:

### Opción A: Carpeta de Inicio (usuario actual)

```powershell
# Crear acceso directo en carpeta de Inicio
$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut("$env:APPDATA\Microsoft\Windows\Start Menu\Programs\Startup\FifoCleanup.lnk")
$shortcut.TargetPath = "C:\FifoCleanup\FifoCleanup.exe"
$shortcut.WorkingDirectory = "C:\FifoCleanup"
$shortcut.Save()
```

### Opción B: Tarea programada (más robusto)

```powershell
# Crear tarea programada que inicie al arranque
$action = New-ScheduledTaskAction -Execute "C:\FifoCleanup\FifoCleanup.exe" -WorkingDirectory "C:\FifoCleanup"
$trigger = New-ScheduledTaskTrigger -AtStartup
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries
Register-ScheduledTask -TaskName "FifoCleanup" -Action $action -Trigger $trigger -Settings $settings -Description "Gestión automática de almacenamiento FIFO"
```

En la instalación con script, esta tarea se registra como `FIFO_AutoStart`.

---

## 6. Verificación post-instalación

### Checklist

| # | Verificación | Comando/Acción | Resultado esperado |
|---|-------------|---------------|-------------------|
| 1 | .NET 8 instalado | `dotnet --list-runtimes` | Muestra `Microsoft.WindowsDesktop.App 8.0.x` |
| 2 | App se ejecuta | Doble clic `FifoCleanup.exe` | Ventana principal con 6 pestañas |
| 3 | Ruta configurada | Configuración → Ruta | Sin errores de validación |
| 4 | Inventario funciona | Dashboard → Escanear | Detecta Assets y muestra tamaños |
| 5 | Bitácora se crea | Verificar carpeta `bitacora/` | Archivo `bitacora_YYYYMMDD.csv` creado |
| 6 | RF-07 inicia | Ejecución → Activar RF-07 | Muestra "Próxima ejecución" |
| 7 | RF-08 inicia | Ejecución → Activar RF-08 | Muestra "Monitoreando..." |
| 8 | Simulación funciona | Simulación → Ejecutar | Completa sin errores |

---

## 7. Desinstalación

1. Cerrar la aplicación (detener RF-07/RF-08 primero)
2. Eliminar tarea programada (si se creó):
   ```powershell
   Unregister-ScheduledTask -TaskName "FifoCleanup" -Confirm:$false
   ```
3. Eliminar carpeta de instalación:
   ```powershell
   Remove-Item "C:\FifoCleanup" -Recurse -Force
   ```
4. (Opcional) Eliminar archivos de bitácora si ya no se necesitan

> ⚠️ La desinstalación NO afecta los datos de monitoreo en `D:\MonitoringData`.

---

## 8. Actualización

Ver procedimiento [RB-13 en el Runbook](RUNBOOK.md#rb-13-actualizar-la-aplicación).
