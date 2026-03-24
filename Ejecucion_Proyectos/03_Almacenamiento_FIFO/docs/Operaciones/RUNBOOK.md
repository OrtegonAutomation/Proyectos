# Runbook — FifoCleanup v1.1

**Proyecto:** Gestión de Almacenamiento FIFO  
**Cliente:** ODL Instrumentación y Control  
**Autor:** IDC Ingeniería  
**Fecha:** Marzo 2026

---

## Propósito

Este runbook contiene procedimientos paso a paso para escenarios operacionales comunes. Cada procedimiento ha sido probado en el servidor SRVODLRTDNMICA.

---

## Índice de Procedimientos

| # | Procedimiento | Tiempo estimado |
|---|--------------|-----------------|
| RB-01 | [Inicio del sistema](#rb-01-inicio-del-sistema) | 2 min |
| RB-02 | [Apagado seguro](#rb-02-apagado-seguro) | 1 min |
| RB-03 | [Inventario inicial](#rb-03-inventario-inicial) | 3 min |
| RB-04 | [Ejecutar simulación de prueba](#rb-04-ejecutar-simulación-de-prueba) | 10 min |
| RB-05 | [Limpieza manual de emergencia](#rb-05-limpieza-manual-de-emergencia) | 5 min |
| RB-06 | [Activar servicios automáticos (RF-07 y RF-08)](#rb-06-activar-servicios-automáticos) | 1 min |
| RB-07 | [Cambiar umbral de limpieza](#rb-07-cambiar-umbral-de-limpieza) | 5 min |
| RB-08 | [Exportar bitácora para auditoría](#rb-08-exportar-bitácora-para-auditoría) | 2 min |
| RB-09 | [Restaurar configuración por defecto](#rb-09-restaurar-configuración-por-defecto) | 2 min |
| RB-10 | [Migrar a un nuevo servidor](#rb-10-migrar-a-un-nuevo-servidor) | 30 min |
| RB-11 | [Respuesta a disco lleno (>95%)](#rb-11-respuesta-a-disco-lleno) | 5 min |
| RB-12 | [Verificar integridad de bitácora](#rb-12-verificar-integridad-de-bitácora) | 3 min |
| RB-13 | [Actualizar la aplicación](#rb-13-actualizar-la-aplicación) | 10 min |

---

## RB-01: Inicio del sistema

**Cuándo:** Después de reinicio del servidor o primera vez.

**Procedimiento:**

1. Verificar si la tarea `FIFO_AutoStart` ya inició la aplicación tras el reboot.
2. Si no inició, ejecutar manualmente `FifoCleanup.exe` desde la carpeta de instalación.
3. Verificar que la ventana principal se abra con 6 pestañas.
4. Confirmar que RF-07 y RF-08 se reactivaron automáticamente (si están habilitados en configuración).
5. Si es primera vez, seguir procedimiento [RB-03](#rb-03-inventario-inicial).

**Verificación:** La aplicación muestra el Dashboard sin errores y RF-07/RF-08 en estado activo cuando aplica.

---

## RB-02: Apagado seguro

**Cuándo:** Antes de reiniciar el servidor o detener la aplicación.

**Procedimiento:**

1. Ir a pestaña **Ejecución**
2. Si RF-07 está activo → clic en **Detener RF-07** → esperar confirmación "RF07_DETENIDO"
3. Si RF-08 está activo → clic en **Detener RF-08** → esperar confirmación "RF08_DETENIDO"
4. Verificar que no haya limpieza en progreso (barra de progreso vacía)
5. Cerrar la aplicación con la X o `Alt+F4`

**Verificación:** La bitácora muestra entradas `SystemStop` para RF-07 y RF-08.

⚠️ **Nunca cerrar la aplicación durante una limpieza activa.** Esperar a que termine o cancelarla primero.

---

## RB-03: Inventario inicial

**Cuándo:** Primera vez o cuando se cambia la ruta de almacenamiento.

**Procedimiento:**

1. Abrir FifoCleanup
2. Ir a pestaña **Configuración**
3. Verificar que **Ruta de almacenamiento** apunte a la carpeta correcta (ej: `D:\MonitoringData`)
4. Clic en **Guardar**
5. Ir a pestaña **Dashboard**
6. Clic en **Escanear**
7. Esperar a que complete (barra de progreso → "Escaneo completado")
8. Verificar:
   - Número de Assets detectados coincide con lo esperado
   - Tamaños reportados son razonables
   - Tasa de crecimiento diario aparece (si hay datos de > 1 día)

**Resultado esperado:**
```
Total: XXX.XX GB de YYY GB (ZZ%)
Assets escaneados: N
Tiempo de escaneo: < 60 segundos
```

⚠️ Si el escaneo tarda más de 90 segundos, verificar que el disco no esté bajo carga de I/O por otro proceso.

---

## RB-04: Ejecutar simulación de prueba

**Cuándo:** Antes de activar limpieza en producción o después de cambiar parámetros.

**Procedimiento:**

1. Ir a pestaña **Simulación**
2. Configurar parámetros:
   - **Número de Assets:** 3–5
   - **Variables por Asset:** 2–3
   - **Días de historia:** 30
   - **Tamaño promedio por carpeta:** 50 MB
   - **Disco simulado (GB):** 100
   - **Uso inicial (%):** 80
   - **Umbral (%):** 85
   - **Cap (%):** 20
3. Clic en **Ejecutar simulación**
4. Esperar las 3 fases:
   - Fase 1: Generación de datos sintéticos
   - Fase 2: Inventario
   - Fase 3: Limpieza FIFO simulada (dry run)
5. Revisar resultados:
   - ¿Cuántas carpetas se eliminarían?
   - ¿Cuánto espacio se liberaría?
   - ¿El uso bajaría al nivel deseado?
6. Cuando termine, clic en **Limpiar datos de prueba** para liberar el espacio temporal

**Resultado esperado:**
```
Datos reales en disco: XX.XX GB
Uso real: 80.0% de 100 GB
Se liberarían: XX.XX GB
Carpetas afectadas: N
```

---

## RB-05: Limpieza manual de emergencia

**Cuándo:** Disco > 90% y RF-07/RF-08 no están respondiendo.

**Procedimiento:**

1. Ir a pestaña **Ejecución**
2. Clic en **Vista previa** (dry run)
3. Esperar a que calcule qué se eliminaría
4. Revisar el listado:
   - ¿Las carpetas son las más antiguas? (FIFO correcto)
   - ¿El espacio a liberar es suficiente?
5. Si es correcto → clic en **Ejecutar limpieza**
6. **CONFIRMAR** en el diálogo de confirmación
7. Monitorear la barra de progreso
8. Al terminar, verificar en Dashboard que el uso bajó

⚠️ **La limpieza elimina datos permanentemente.** Siempre hacer vista previa antes.

**Si necesita cancelar:**
- Clic en **Cancelar** durante la ejecución
- La limpieza se detendrá de forma segura al completar el lote actual
- Los datos ya eliminados NO se recuperan

---

## RB-06: Activar servicios automáticos

**Cuándo:** Después de la configuración inicial o después de un reinicio.

**Procedimiento:**

1. Ir a pestaña **Ejecución**.
2. Verificar estado inicial de RF-07 y RF-08:
   - En v1.1 deben reactivarse automáticamente al iniciar la app (si están habilitados).
3. Si alguno no está activo:
   - Clic en **Activar RF-07** y/o **Activar RF-08**.
4. Verificar en la Bitácora que aparezcan entradas `RF07_INICIADO` y `RF08_INICIADO`.
5. Para operación "solo monitoreo" posterior a reinicio:
   - Mantener umbrales conservadores y validar que no se estén ejecutando limpiezas no deseadas.

**Resultado:** Ambos servicios aparecen como "Activo" con indicadores verdes y monitoreo en tiempo real disponible.

---

## RB-07: Cambiar umbral de limpieza

**Cuándo:** Cuando las condiciones del almacenamiento cambian (más Assets, mayor crecimiento).

**Procedimiento:**

1. Ir a pestaña **Configuración**
2. Modificar el **Umbral de limpieza** (%)
   - Aumentar si la limpieza se ejecuta muy frecuentemente
   - Disminuir si el disco se llena antes de que actúe
3. Clic en **Guardar**
4. **IMPORTANTE:** Ir a pestaña **Simulación** → ejecutar una simulación con los nuevos parámetros
5. Si la simulación da resultados esperados, los servicios RF-07/RF-08 usarán el nuevo umbral automáticamente

---

## RB-08: Exportar bitácora para auditoría

**Cuándo:** Auditorías periódicas o solicitud de reportes.

**Procedimiento:**

1. Ir a pestaña **Bitácora**
2. Seleccionar filtros:
   - **Fecha desde:** inicio del período
   - **Fecha hasta:** fin del período
3. Clic en **Exportar a CSV**
4. Seleccionar destino (ej: `\\SharedDrive\Auditorias\bitacora_2026_Q1.csv`)
5. Verificar que el archivo exportado se abre correctamente en Excel

**Formato del CSV exportado:**
```
"Timestamp","EventType","Action","AssetId","VariableId","Details","BytesAffected","Result","Source"
```

---

## RB-09: Restaurar configuración por defecto

**Cuándo:** Si la configuración actual causa problemas.

**Procedimiento:**

1. Ir a pestaña **Configuración**
2. Clic en **Restaurar valores por defecto**
3. Verificar los valores restaurados:
   - Umbral: 85%, Cap: 20%, Frecuencia RF-07: 6h, etc.
4. **ACTUALIZAR** la ruta de almacenamiento (no se restaura automáticamente)
5. Clic en **Guardar**

**Alternativa manual:** Si la aplicación no abre:
1. Navegar al archivo `fifo_config.json`
2. Renombrarlo a `fifo_config.json.bak`
3. Reiniciar la aplicación — creará uno nuevo con valores por defecto

---

## RB-10: Migrar a un nuevo servidor

**Cuándo:** Cambio de hardware o migración de servidor.

**Procedimiento:**

1. En el servidor **antiguo**:
   - Apagar la aplicación con [RB-02](#rb-02-apagado-seguro)
   - Copiar carpeta de aplicación (ej: `C:\FifoCleanup\`)
   - Copiar archivo de configuración `fifo_config.json`
   - Copiar carpeta de bitácora (ej: `bitacora/`)

2. En el servidor **nuevo**:
   - Instalar .NET 8.0 Desktop Runtime
   - Pegar carpeta de aplicación
   - Pegar `fifo_config.json` y carpeta de bitácora
   - Ejecutar `FifoCleanup.exe`
   - Ir a **Configuración** → actualizar **Ruta de almacenamiento** si cambió
   - Guardar → Escanear desde Dashboard
   - Verificar que detecta los Assets correctamente
   - Activar RF-07 y RF-08 [RB-06](#rb-06-activar-servicios-automáticos)

⚠️ Si la ruta de almacenamiento no existe en el nuevo servidor, la aplicación mostrará error. Verificar que el disco/carpeta esté montado antes de iniciar.

---

## RB-11: Respuesta a disco lleno

**Cuándo:** Uso > 95%, situación de emergencia.

**Procedimiento:**

1. **Verificar estado actual:**
   - Dashboard → uso actual del disco
   - ¿RF-07/RF-08 están activos?

2. **Si RF-07/RF-08 están activos pero no están limpiando:**
   - Verificar Bitácora → ¿hay entradas recientes de error?
   - Posible causa: cap de limpieza muy bajo → aumentar temporalmente a 40-50%
   - Guardar configuración → esperar siguiente ciclo RF-07

3. **Si RF-07/RF-08 están desactivados:**
   - Ejecutar limpieza manual [RB-05](#rb-05-limpieza-manual-de-emergencia)
   - Después de liberar espacio, activar servicios [RB-06](#rb-06-activar-servicios-automáticos)

4. **Si la limpieza no libera suficiente:**
   - Aumentar cap de limpieza temporalmente (Configuración → 40-50%)
   - Ejecutar otra limpieza manual
   - **RECORDAR** restaurar el cap a su valor normal (20%) después

5. **Último recurso:**
   - Contactar soporte IDC
   - Evaluar si hay datos fuera de la estructura Asset/Variable que necesitan limpieza manual

---

## RB-12: Verificar integridad de bitácora

**Cuándo:** Antes de auditorías o si se sospecha corrupción.

**Procedimiento:**

1. Navegar a la carpeta de bitácora (por defecto: `bitacora/`)
2. Verificar que existen archivos `bitacora_YYYYMMDD.csv`
3. Abrir un archivo reciente con un editor de texto
4. Verificar:
   - Primera línea es el header: `"Timestamp","EventType","Action",...`
   - Las líneas siguientes tienen 9 campos separados por comas
   - Los timestamps son cronológicos
   - No hay líneas truncadas o caracteres ilegibles
5. Si hay corrupción:
   - El archivo afectado puede renombrarse con extensión `.corrupted`
   - La aplicación creará uno nuevo automáticamente

---

## RB-13: Actualizar la aplicación

**Cuándo:** Nueva versión disponible de FifoCleanup.

**Procedimiento:**

1. Apagar la aplicación con [RB-02](#rb-02-apagado-seguro)
2. **Respaldar** antes de actualizar:
   - `fifo_config.json`
   - Carpeta `bitacora/`
3. Copiar los nuevos archivos de la aplicación sobre los existentes
4. Iniciar la aplicación [RB-01](#rb-01-inicio-del-sistema)
5. Verificar que la versión es correcta
6. Ejecutar un inventario [RB-03](#rb-03-inventario-inicial)
7. Activar servicios [RB-06](#rb-06-activar-servicios-automáticos)
8. Verificar en Bitácora que no hay errores

