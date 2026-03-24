# Manual de Operación — FifoCleanup v1.0

**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Cliente:** ODL Instrumentación y Control  
**Autor:** IDC Ingeniería  
**Fecha:** Marzo 2026  
**Versión del documento:** 1.0

---

## Tabla de Contenido

1. [Introducción](#1-introducción)
2. [Descripción general del sistema](#2-descripción-general-del-sistema)
3. [Setup inicial](#3-setup-inicial)
4. [Interfaz de usuario — Las 6 pestañas](#4-interfaz-de-usuario--las-6-pestañas)
5. [Operación diaria](#5-operación-diaria)
6. [Limpieza FIFO — Modos de operación](#6-limpieza-fifo--modos-de-operación)
7. [Bitácora de auditoría](#7-bitácora-de-auditoría)
8. [Respuesta a alarmas y situaciones críticas](#8-respuesta-a-alarmas-y-situaciones-críticas)
9. [Mantenimiento periódico](#9-mantenimiento-periódico)
10. [Glosario](#10-glosario)

---

## 1. Introducción

FifoCleanup es una aplicación de escritorio que gestiona automáticamente el espacio en disco del servidor de monitoreo SRVODLRTDNMICA. Cuando el almacenamiento se acerca a su límite, el sistema elimina automáticamente los datos más antiguos (política FIFO) para garantizar que el software de monitoreo (Aspen Mtell / ODL) nunca se quede sin espacio.

### ¿Para quién es este manual?

- **Operadores** que supervisan el servidor diariamente
- **Ingenieros** que configuran políticas de retención
- **Administradores** que instalan y mantienen el sistema

### Convenciones

| Símbolo | Significado |
|---------|-------------|
| 🟢 | Operación normal, no requiere acción |
| 🟡 | Atención requerida, monitorear |
| 🔴 | Acción inmediata requerida |
| ⚠️ | Advertencia importante |
| 💡 | Consejo o recomendación |

---

## 2. Descripción general del sistema

### Arquitectura

FifoCleanup es una aplicación 100% C# / .NET 8.0 con tres componentes:

| Componente | Función |
|------------|---------|
| **FifoCleanup.Engine** | Motor de lógica: inventario, limpieza FIFO, simulación, servicios automáticos |
| **FifoCleanup.UI** | Interfaz gráfica WPF con 6 pestañas |
| **Bitácora CSV** | Registro inmutable de todas las operaciones |

### Estructura de datos monitoreada

El sistema espera la siguiente estructura de carpetas en el disco de monitoreo:

```
D:\MonitoringData\
├── Asset001\
│   ├── 00\             ← Variable 0
│   │   ├── E\          ← Datos tipo E (eventos)
│   │   │   └── 2026\01\15\   ← Carpeta por día
│   │   └── F\          ← Datos tipo F (frecuencia)
│   │       └── 2026\01\15\
│   ├── 01\             ← Variable 1
│   └── 02\             ← Variable 2
├── Asset002\
└── Asset003\
```

- **Asset:** Equipo monitoreado (ej: bomba, motor, compresor)
- **Variable:** Punto de medición dentro del equipo (numeradas 00, 01, 02...)
- **E / F:** Tipo de datos (Eventos / Frecuencia)
- **YYYY/MM/DD:** Carpeta por día con los archivos de datos

### Semáforo de estado

| Color | Uso del disco | Significado |
|-------|--------------|-------------|
| 🟢 Verde | < 70% | Normal, sin acción necesaria |
| 🟡 Amarillo | 70% – 85% | Monitorear, puede requerir limpieza pronto |
| 🔴 Rojo | > 85% | Crítico, limpieza automática activada o necesaria |

---

## 3. Setup inicial

### Paso 1: Primer inicio

Al ejecutar `FifoCleanup.exe` por primera vez:

1. La aplicación se abre en la pestaña **Dashboard** (vacía)
2. Ir a la pestaña **Configuración**

### Paso 2: Configurar ruta de almacenamiento

1. En el campo **Ruta de almacenamiento**, escribir o seleccionar la carpeta raíz donde están los Assets
   - Ejemplo: `D:\MonitoringData`
2. Si la ruta no existe, se mostrará un error — verificar que la ruta sea correcta y accesible

### Paso 3: Configurar umbrales

| Parámetro | Valor recomendado | Rango |
|-----------|-------------------|-------|
| Umbral de limpieza (%) | 85 | 50–95 |
| Cap de limpieza (%) | 20 | 5–50 |
| Espacio máximo (GB) | 0 (usa disco real) | 0+ |

- **Umbral:** Cuando el uso supere este porcentaje, se activa la limpieza
- **Cap:** Máximo porcentaje de datos monitoreados a eliminar por ejecución (protección contra borrado excesivo)
- **Espacio máximo:** Si > 0, los umbrales se calculan contra este valor en lugar del disco completo

### Paso 4: Configurar servicios automáticos

| Parámetro | Valor recomendado | Rango |
|-----------|-------------------|-------|
| Frecuencia RF-07 (horas) | 6 | 1–24 |
| Hora preferida RF-07 | 2 (2:00 AM) | 0–23 |
| Umbral preventivo RF-08 (días) | 3 | 1–10 |
| Habilitar RF-07 | ✅ Sí | — |
| Habilitar RF-08 | ✅ Sí | — |

### Paso 5: Guardar y verificar

1. Clic en **Guardar configuración**
2. Ir a la pestaña **Dashboard**
3. Clic en **Escanear** para ejecutar el primer inventario
4. Verificar que se muestren los Assets y el uso del disco

### Paso 6: Prueba con simulación

Antes de activar la limpieza en producción:

1. Ir a la pestaña **Simulación**
2. Configurar parámetros de simulación (Assets, Variables, días de historia)
3. Clic en **Ejecutar simulación**
4. Revisar el reporte: qué se eliminaría, cuánto espacio se liberaría
5. Si el resultado es satisfactorio, proceder con la activación

---

## 4. Interfaz de usuario — Las 6 pestañas

### 4.1 Dashboard

**Propósito:** Vista general del estado del almacenamiento en tiempo real.

**Elementos:**
- Indicador de uso con semáforo (verde/amarillo/rojo)
- Gráfica de uso del disco (LiveCharts)
- Lista de Assets detectados con tamaños y tasas de crecimiento
- Proyección de días hasta alcanzar el umbral
- Botón **Escanear** para actualizar el inventario

**Operación:**
- Revisar el Dashboard al inicio del turno
- Si el semáforo está en 🔴, verificar que RF-07/RF-08 estén activos

### 4.2 Configuración

**Propósito:** Gestionar la política de retención y parámetros del sistema.

**Elementos:**
- Formulario con todos los parámetros configurables
- Validación en tiempo real (rangos, rutas existentes)
- Botones **Guardar** y **Restaurar valores por defecto**

**Operación:**
- Modificar parámetros solo cuando sea necesario
- Siempre ejecutar una simulación después de cambiar umbrales

### 4.3 Simulación

**Propósito:** Probar la política FIFO sin afectar datos reales.

**Elementos:**
- Configuración de datos sintéticos (cantidad de Assets, Variables, días)
- Botones: **Generar datos**, **Ejecutar simulación**, **Limpiar datos de prueba**
- Reporte detallado: estado antes/después, bytes a liberar, carpetas afectadas
- Simulación continua para probar RF-07/RF-08 en tiempo real

**Operación:**
- Usar antes de cambiar políticas en producción
- La simulación NO toca datos reales

### 4.4 Ejecución

**Propósito:** Ejecutar y monitorear limpieza FIFO en producción.

**Elementos:**
- Botón **Ejecutar limpieza manual** (con confirmación)
- Vista previa (dry run) antes de ejecutar
- Estado de RF-07 (programada): activo/inactivo, próxima ejecución
- Estado de RF-08 (preventiva): activo/inactivo, archivos detectados, limpiezas ejecutadas
- Botones para activar/desactivar RF-07 y RF-08
- Barra de progreso durante la limpieza
- Botón **Cancelar** durante la ejecución

**Operación:**
- Normalmente RF-07 y RF-08 se encargan automáticamente
- La limpieza manual es para situaciones de emergencia

### 4.5 Bitácora

**Propósito:** Consultar el registro de auditoría de todas las operaciones.

**Elementos:**
- Tabla con todas las entradas: timestamp, tipo, acción, asset, variable, detalles, bytes, resultado
- Filtros por: rango de fechas, tipo de evento, asset
- Botón **Exportar a CSV**
- Ordenamiento por columnas

**Operación:**
- Revisar después de eventos de limpieza
- Exportar para auditorías o informes

### 4.6 Reportes (Dashboard extendido)

**Propósito:** Gráficas y métricas avanzadas del almacenamiento.

**Elementos:**
- Gráficas de tendencia de uso
- Distribución por Asset
- Historial de limpiezas

---

## 5. Operación diaria

### Rutina recomendada

| Momento | Acción | Tiempo |
|---------|--------|--------|
| Inicio de turno | Abrir FifoCleanup → Dashboard. Verificar semáforo 🟢 | 1 min |
| Inicio de turno | Verificar que RF-07 y RF-08 estén activos (pestaña Ejecución) | 30 seg |
| Mediodía | Revisar Dashboard. Si 🟡, monitorear tendencia | 1 min |
| Fin de turno | Revisar Bitácora. Verificar que no haya errores | 2 min |
| **Semanal** | Revisar historial de limpiezas en Bitácora | 5 min |
| **Mensual** | Verificar parámetros de configuración siguen siendo apropiados | 10 min |

### ¿Qué hacer si...?

| Situación | Acción |
|-----------|--------|
| Semáforo 🟢 | Nada. Operación normal. |
| Semáforo 🟡 | Verificar que RF-07/RF-08 estén activos. Monitorear tendencia. |
| Semáforo 🔴 | Si RF-07/RF-08 están activos, esperar a la próxima ejecución. Si están desactivados, ejecutar limpieza manual. |
| RF-07/RF-08 detenidos | Ir a Ejecución → activarlos. Si no inician, verificar configuración (ruta válida). |
| Error en Bitácora | Anotar el error. Si es recurrente, contactar soporte técnico IDC. |

---

## 6. Limpieza FIFO — Modos de operación

### 6.1 Limpieza manual

1. Pestaña **Ejecución** → clic en **Vista previa** (dry run)
2. Revisar qué carpetas se eliminarían y cuánto espacio se liberaría
3. Si es correcto, clic en **Ejecutar limpieza**
4. Confirmar en el diálogo de confirmación
5. Esperar a que termine (barra de progreso)
6. Revisar resultado en la Bitácora

### 6.2 RF-07 — Limpieza programada

**Funcionamiento:**
1. Se ejecuta cada N horas (configurable, default: 6h)
2. Realiza un inventario completo
3. Calcula la proyección de crecimiento (promedio de últimos 7 días)
4. Si proyecta que el uso superará el umbral antes de la siguiente ejecución → ejecuta limpieza general
5. Si la proyección es segura → registra "SKIP" en bitácora y espera

**Algoritmo de limpieza general:**
- Ordena TODAS las carpetas de día de TODOS los Assets por fecha (más antiguas primero = FIFO)
- Elimina carpetas hasta liberar suficiente espacio (baja 5% debajo del umbral)
- Respeta el cap de limpieza (no elimina más del % configurado por ejecución)
- Aplica throttling entre eliminaciones para no saturar el disco

### 6.3 RF-08 — Limpieza preventiva

**Funcionamiento:**
1. `FileSystemWatcher` monitorea la carpeta de datos en tiempo real
2. Al detectar archivos nuevos, identifica el Asset/Variable afectado
3. Agrupa eventos en lotes (cada 10 segundos, configurable)
4. Evalúa si el crecimiento del Asset amenaza con llenar el disco en < N días (configurable)
5. Si hay riesgo → ejecuta limpieza LOCAL (solo del Asset/Variable afectado)
6. Máximo de N días de datos eliminados por Asset (configurable)

**Diferencia con RF-07:**
- RF-07 = Global + proyección histórica + periódica
- RF-08 = Local + detección en tiempo real + reactiva

**Protección contra ejecución simultánea:**
- RF-07 y RF-08 nunca se ejecutan al mismo tiempo
- Si RF-07 está limpiando, RF-08 espera
- Cada uno registra su tipo en la bitácora

---

## 7. Bitácora de auditoría

### Formato

Los archivos de bitácora se almacenan en CSV con el siguiente formato:

```
"Timestamp","EventType","Action","AssetId","VariableId","Details","BytesAffected","Result","Source"
```

### Tipos de evento

| Tipo | Descripción |
|------|-------------|
| `Information` | Información general del sistema |
| `Inventory` | Operación de inventario |
| `CleanupManual` | Limpieza ejecutada manualmente |
| `CleanupScheduled` | Limpieza ejecutada por RF-07 |
| `CleanupPreventive` | Limpieza ejecutada por RF-08 |
| `Simulation` | Operación de simulación |
| `Configuration` | Cambio de configuración |
| `Alarm` | Alarma del sistema |
| `Error` | Error del sistema |
| `SystemStart` | Inicio de servicio RF-07/RF-08 |
| `SystemStop` | Detención de servicio RF-07/RF-08 |
| `FileDetected` | Archivo nuevo detectado por RF-08 |

### Ubicación y rotación

- **Ruta:** `{BitacoraPath}/bitacora_YYYYMMDD.csv` (un archivo por día)
- **Rotación:** Si el archivo supera 100 MB, se crea uno nuevo con marca de tiempo
- **Retención:** Los archivos más antiguos que N días (configurable, default: 90) se eliminan automáticamente

### Exportación

Desde la pestaña **Bitácora**:
1. Seleccionar rango de fechas (opcional)
2. Clic en **Exportar a CSV**
3. Seleccionar ubicación de destino

---

## 8. Respuesta a alarmas y situaciones críticas

### Disco > 95% de uso

1. Verificar Dashboard → semáforo 🔴
2. Verificar que RF-07 y RF-08 estén activos
3. Si están activos, esperar la próxima ejecución (máximo `ScheduledFrequencyHours` horas)
4. Si no hay mejora, ejecutar limpieza manual
5. Si la limpieza manual no libera suficiente espacio, considerar:
   - Aumentar el cap de limpieza temporalmente
   - Reducir el umbral para que limpie más agresivamente
   - Contactar soporte IDC

### Error "Ruta de almacenamiento no existe"

Ocurre cuando la ruta configurada no es accesible:
1. Verificar que el disco/partición esté montado
2. Verificar que la ruta en Configuración sea correcta
3. Verificar permisos de acceso
4. Si la ruta cambió, actualizar en Configuración → Guardar

### RF-07/RF-08 se detienen inesperadamente

1. Revisar la Bitácora en busca de entradas tipo `Error`
2. Causas comunes:
   - Ruta de almacenamiento no accesible
   - Permisos insuficientes
   - Disco lleno (no hay espacio ni para la bitácora)
3. Corregir la causa y reactivar desde pestaña Ejecución

### La aplicación no inicia

1. Verificar que .NET 8.0 Desktop Runtime esté instalado
2. Verificar que el archivo `fifo_config.json` no esté corrupto (renombrarlo si es necesario — se creará uno nuevo con valores por defecto)
3. Verificar los logs de Windows Event Viewer

---

## 9. Mantenimiento periódico

| Frecuencia | Tarea | Procedimiento |
|------------|-------|---------------|
| **Semanal** | Revisar bitácora | Pestaña Bitácora → filtrar última semana → verificar que no hay errores recurrentes |
| **Mensual** | Verificar configuración | Pestaña Configuración → verificar que los umbrales siguen siendo apropiados para el volumen de datos actual |
| **Trimestral** | Exportar bitácora | Pestaña Bitácora → Exportar CSV del trimestre → archivar |
| **Trimestral** | Actualizar .NET Runtime | Verificar si hay actualizaciones de seguridad de .NET 8 |
| **Anual** | Revisión de política | Evaluar si los umbrales y caps necesitan ajuste basado en tendencias de crecimiento del año |

---

## 10. Glosario

| Término | Definición |
|---------|------------|
| **Asset** | Equipo monitoreado (bomba, motor, compresor). Cada uno tiene su carpeta con subcarpetas de variables. |
| **Variable** | Punto de medición dentro de un Asset. Numeradas 00, 01, 02... |
| **Carpeta E** | Datos de eventos del Asset/Variable |
| **Carpeta F** | Datos de frecuencia del Asset/Variable |
| **Carpeta de día** | Carpeta con formato `YYYY/MM/DD` que contiene los datos de un día específico |
| **FIFO** | First In, First Out. Política que elimina los datos más antiguos primero. |
| **Umbral** | Porcentaje de uso del disco que dispara la limpieza automática |
| **Cap** | Límite máximo de datos que se pueden eliminar en una sola ejecución |
| **RF-07** | Servicio de limpieza programada (evaluación periódica global) |
| **RF-08** | Servicio de limpieza preventiva (detección en tiempo real, limpieza local) |
| **Bitácora** | Registro de auditoría inmutable de todas las operaciones del sistema |
| **Dry run** | Ejecución simulada que muestra qué se eliminaría sin borrar nada |
| **Throttling** | Pausa deliberada entre operaciones de eliminación para no saturar el disco |
| **StorageLevel** | Indicador semáforo: Green (<70%), Yellow (70-85%), Red (>85%) |
