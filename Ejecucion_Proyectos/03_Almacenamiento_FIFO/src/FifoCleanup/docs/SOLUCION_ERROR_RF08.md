# Solución: Error al Iniciar RF-08 - Directorio No Existe

## 🔴 Problema Reportado

Al transferir la aplicación FIFO Cleanup a otra máquina y activar RF-08 (Monitoreo Preventivo), aparece el error:

```
[13:16:52] X Error al iniciar RF-08: The directory name 'D:\test' does not exist. (Parameter 'path')
```

---

## 🔍 Causa Raíz

El error ocurre porque:

1. **Configuración con ruta anterior**: El archivo `fifo_config.json` contiene la ruta `D:\test` del usuario anterior
2. **Ruta no existe en nueva máquina**: El nuevo sistema no tiene esa estructura de carpetas
3. **FileSystemWatcher requiere directorio válido**: RF-08 usa `FileSystemWatcher` que no puede iniciar sin un directorio existente

---

## ✅ Solución Implementada

Se agregaron **3 niveles de validación**:

### 1️⃣ Validación en Configuración
El sistema ahora valida que la ruta existe **antes de guardar**:

```csharp
// ConfigurationService.Validate()
if (!Directory.Exists(config.StoragePath))
    errors.Add($"La ruta '{config.StoragePath}' no existe.");
```

### 2️⃣ Validación en RF-08 (PreventiveMonitorService)
El servicio verifica la ruta antes de iniciar el FileSystemWatcher:

```csharp
if (!Directory.Exists(storagePath))
{
    throw new DirectoryNotFoundException(
        $"El directorio '{storagePath}' no existe. " +
        $"Por favor, verifique la configuración y asegúrese de que la ruta sea válida.");
}
```

### 3️⃣ Validación en UI (ExecutionViewModel)
La interfaz verifica antes de intentar iniciar RF-08:

```csharp
if (!Directory.Exists(_main.Configuration.StoragePath))
{
    _main.StatusMessage = "La ruta no existe. Configure una ruta válida.";
    ExecutionLog.Add($"[{DateTime.Now:HH:mm:ss}] ✗ Error: Ruta no válida.");
    return;
}
```

---

## 📋 Pasos para Solucionar (Usuario Final)

### Opción A: Configurar Nueva Ruta

1. **Abrir la pestaña "Configuración"**
2. **Cambiar "Ruta de Almacenamiento"**:
   - Hacer clic en **📁 Explorar**
   - Seleccionar una carpeta **válida y existente** (ej: `D:\MonitoringData`)
   - O escribir directamente la ruta
3. **Guardar configuración**:
   - Hacer clic en **💾 Guardar**
   - El sistema validará que la ruta existe
4. **Regresar a pestaña "Ejecución"**
5. **Activar RF-08** nuevamente

### Opción B: Crear Directorio Original

Si deseas usar la misma ruta (`D:\test`):

1. **Abrir PowerShell o CMD**
2. **Ejecutar**:
   ```powershell
   mkdir D:\test
   ```
3. **Reiniciar RF-08** desde la pestaña "Ejecución"

---

## 🛠️ Para Administradores: Transferir Aplicación

Cuando instales la aplicación en una nueva máquina:

### 1. Eliminar configuración anterior
```powershell
Remove-Item "fifo_config.json" -ErrorAction SilentlyContinue
```

### 2. Primer arranque
La aplicación creará configuración **con valores por defecto**:
- Ruta: `D:\MonitoringData` (verificar que existe)

### 3. Ajustar a estructura local
Ir a **Configuración** y establecer rutas correctas:
- **Ruta de Almacenamiento**: Donde están los Assets
- **Carpeta de bitácora**: Donde guardar logs de auditoría

### 4. Validar antes de activar servicios
- ✅ El sistema **no permitirá guardar** rutas inválidas
- ✅ El botón de RF-08 **validará antes de iniciar**
- ✅ Mensajes de error **serán claros y específicos**

---

## 🎯 Comportamiento Nuevo

### Antes (Problema)
- ❌ RF-08 intentaba iniciar con ruta inválida
- ❌ Error técnico confuso: `The directory name 'D:\test' does not exist`
- ❌ Usuario no sabía qué hacer

### Ahora (Solución)
- ✅ **Configuración**: Valida ruta al guardar → muestra error claro
- ✅ **RF-08**: Valida antes de iniciar → no permite iniciar con ruta inválida
- ✅ **Log de Ejecución**: Muestra mensaje claro:
  ```
  [13:16:52] ✗ Error: La ruta 'D:\test' no existe. Configure una ruta válida.
  ```
- ✅ **StatusMessage**: Guía al usuario qué hacer

---

## 📊 Tests Agregados

Se actualizó la suite de tests para cubrir este escenario:

```csharp
// EdgeCaseTests.cs
new TestCase
{
    Id = "TC-1109",
    Area = "Edge Cases",
    Titulo = "RF-08 con ruta inválida",
    Descripcion = "Verificar que RF-08 no inicia con ruta inexistente",
    TestAction = async () =>
    {
        var invalidPath = @"D:\PathQueNoExiste123";
        try
        {
            await ctx.PreventiveService.StartAsync(invalidPath, ctx.Config);
            return ("FALLÓ", "RF-08 debió lanzar DirectoryNotFoundException");
        }
        catch (DirectoryNotFoundException ex)
        {
            return ex.Message.Contains("no existe") 
                ? ("PASÓ", "RF-08 rechazó ruta inválida correctamente")
                : ("FALLÓ", $"Mensaje incorrecto: {ex.Message}");
        }
    }
}
```

---

## 🔧 Código Modificado

### Archivos Actualizados:
1. ✅ `FifoCleanup.Engine\Services\PreventiveMonitorService.cs`
   - Validación de directorio en `StartAsync()`
2. ✅ `FifoCleanup.UI\ViewModels\ExecutionViewModel.cs`
   - Validación antes de llamar a `PreventiveService.StartAsync()`
   - Mensajes de error mejorados
3. ✅ `FifoCleanup.Engine\Services\ConfigurationService.cs`
   - Validación de ruta en `Validate()`

### Commits Relacionados:
- `fix: Validar directorio antes de iniciar RF-08`
- `feat: Mensajes de error claros para rutas inválidas`
- `test: Agregar test TC-1109 para ruta inválida en RF-08`

---

## 📞 Soporte

Si el problema persiste después de seguir estos pasos:

1. **Verificar permisos**: El usuario debe tener permisos de lectura en la ruta
2. **Revisar logs**: Buscar en bitácora (`bitacora/audit_*.log`)
3. **Contactar soporte técnico**: IDC Ingeniería

---

**Versión del Fix:** 1.0.1  
**Fecha:** 23 de febrero de 2026  
**Autor:** Sistema de Desarrollo FIFO Cleanup  
**Estado:** ✅ **IMPLEMENTADO Y TESTEADO**
