# Guía de Configuración — FifoCleanup v1.1

**Proyecto:** Gestión de Almacenamiento FIFO  
**Cliente:** ODL Instrumentación y Control  
**Autor:** IDC Ingeniería  
**Fecha:** Marzo 2026

---

## 1. Ubicación del archivo de configuración

La configuración se almacena en formato JSON en:

```
{carpeta_aplicación}\fifo_config.json
```

El archivo usa naming policy `camelCase` y es legible con cualquier editor de texto.

---

## 2. Parámetros detallados

### 2.1 Almacenamiento

#### `storagePath` — Ruta de almacenamiento

- **Tipo:** string
- **Default:** `""` (vacío)
- **Obligatorio:** Sí
- **Descripción:** Ruta raíz donde se encuentran los Assets de monitoreo.
- **Ejemplo:** `"D:\\MonitoringData"`
- **Validación:** Debe existir como directorio accesible.
- **Nota:** Usa `\\` como separador en JSON (doble backslash) o `/` (forward slash).

#### `maxStorageSizeGB` — Espacio máximo asignado

- **Tipo:** double
- **Default:** `0`
- **Rango:** 0 o más
- **Descripción:** Si es mayor que 0, los umbrales y porcentajes se calculan contra este valor en lugar del tamaño real del disco. Útil cuando el disco es compartido y solo se quiere asignar una porción al monitoreo.
- **Ejemplo:** Si el disco es de 2 TB pero solo se asignan 500 GB: configurar `500`.
- **Si es 0:** Se usa el tamaño real del disco/partición.

**Cómo funciona internamente:**
```
Si maxStorageSizeGB > 0:
  TotalSpaceBytes = maxStorageSizeGB × 1024³
  UsedSpaceBytes = solo bytes de datos en E/ y F/ (MonitoredDataBytes)
  FreeSpaceBytes = TotalSpaceBytes - UsedSpaceBytes
```

---

### 2.2 Umbrales de limpieza

#### `thresholdPercent` — Umbral de activación

- **Tipo:** double
- **Default:** `85.0`
- **Rango:** 50–95
- **Descripción:** Porcentaje de uso del disco que activa la limpieza automática. Cuando el uso actual o proyectado supera este valor, RF-07/RF-08 ejecutan limpieza.

**Recomendaciones por escenario:**

| Escenario | Umbral recomendado |
|-----------|-------------------|
| Servidor dedicado solo a monitoreo | 85–90% |
| Servidor compartido con otros servicios | 75–80% |
| Disco con alto crecimiento diario (>5 GB/día) | 70–80% |
| Disco con bajo crecimiento (<1 GB/día) | 85–90% |

#### `cleanupCapPercent` — Cap de limpieza

- **Tipo:** double
- **Default:** `20.0`
- **Rango:** 5–50
- **Descripción:** Máximo porcentaje de datos monitoreados que se puede eliminar en una sola ejecución de limpieza. Actúa como protección contra borrado excesivo.

**Ejemplo:** Si hay 400 GB de datos monitoreados y el cap es 20%, la limpieza eliminará máximo 80 GB por ejecución.

| Valor | Comportamiento |
|-------|---------------|
| 5–10% | Muy conservador. Múltiples ejecuciones necesarias para liberar mucho espacio |
| 20% | Equilibrado (recomendado) |
| 30–50% | Agresivo. Solo para emergencias de disco lleno |

---

### 2.3 Servicio RF-07 (Limpieza programada)

#### `scheduledFrequencyHours` — Frecuencia de evaluación

- **Tipo:** int
- **Default:** `6`
- **Rango:** 1–24
- **Descripción:** Cada cuántas horas RF-07 evalúa si necesita ejecutar limpieza. El servicio hace inventario, calcula proyección y decide si limpia o salta.

| Valor | Comportamiento |
|-------|---------------|
| 1–2h | Evaluación frecuente. Más carga en el servidor pero detección rápida |
| 6h (default) | Equilibrado |
| 12–24h | Evaluación infrecuente. Menor carga pero puede reaccionar tarde |

#### `scheduledHour` — Hora preferida

- **Tipo:** int
- **Default:** `2` (2:00 AM)
- **Rango:** 0–23
- **Descripción:** Hora del día para la primera ejecución. Las siguientes se calculan sumando `scheduledFrequencyHours`. Se recomienda horario nocturno cuando el servidor tiene menor carga.

#### `enableScheduledCleanup` — Habilitar RF-07

- **Tipo:** bool
- **Default:** `true`
- **Descripción:** Si es `false`, RF-07 no estará disponible para activarse.

---

### 2.4 Servicio RF-08 (Monitoreo preventivo)

#### `preventiveThresholdDays` — Días de proyección

- **Tipo:** int
- **Default:** `3`
- **Rango:** 1–10
- **Descripción:** Si se proyecta que el disco se llenará en menos de N días (basado en la tasa de crecimiento del Asset), RF-08 ejecuta limpieza local.

| Valor | Comportamiento |
|-------|---------------|
| 1 día | Muy reactivo. Solo limpia cuando hay peligro inminente |
| 3 días (default) | Preventivo equilibrado |
| 7–10 días | Muy preventivo. Limpia con mucha anticipación |

#### `enablePreventiveCleanup` — Habilitar RF-08

- **Tipo:** bool
- **Default:** `true`
- **Descripción:** Si es `false`, RF-08 no estará disponible.

#### `eventBatchIntervalSeconds` — Intervalo de agrupación

- **Tipo:** int
- **Default:** `10`
- **Rango:** 5–60
- **Descripción:** Cada cuántos segundos RF-08 procesa los eventos acumulados del FileSystemWatcher. Valores más altos reducen evaluaciones pero aumentan latencia de reacción.

---

### 2.5 Limpieza

#### `maxConcurrentAssets` — Assets simultáneos

- **Tipo:** int
- **Default:** `2`
- **Rango:** 1–10
- **Descripción:** Máximo de Assets que se procesan en paralelo durante la limpieza. En un servidor compartido con 50% de carga, se recomienda 1–2 para no competir con el software de monitoreo.

#### `maxDaysToDeletePerAsset` — Días a eliminar por Asset (RF-08)

- **Tipo:** int
- **Default:** `5`
- **Rango:** 1–30
- **Descripción:** En limpieza local (RF-08), máximo número de carpetas de día (más antiguas) a eliminar por Asset/Variable afectado.

---

### 2.6 Rendimiento

#### `useLowPriorityThreads` — Hilos de baja prioridad

- **Tipo:** bool
- **Default:** `true`
- **Descripción:** Si es `true`, los hilos de limpieza y monitoreo se ejecutan con prioridad `BelowNormal`. Esto cede CPU al software de monitoreo durante la limpieza.
- **Recomendación:** Siempre `true` en servidor de producción.

#### `deleteThrottleMs` — Pausa entre eliminaciones

- **Tipo:** int
- **Default:** `50`
- **Rango:** 0+
- **Descripción:** Milisegundos de pausa entre eliminaciones de carpetas consecutivas. Reduce picos de I/O.

| Valor | Comportamiento |
|-------|---------------|
| 0 | Sin pausa. Máxima velocidad de limpieza, máximo impacto en I/O |
| 50 (default) | Pausa mínima. Buen balance velocidad/impacto |
| 100–200 | Limpieza más suave. Recomendado si el disco es HDD compartido |
| 500+ | Limpieza muy lenta. Solo si hay problemas de rendimiento graves |

---

### 2.7 Bitácora

#### `bitacoraPath` — Ruta de bitácora

- **Tipo:** string
- **Default:** `"bitacora"`
- **Descripción:** Ruta del directorio donde se almacenan los archivos CSV de bitácora. Relativa a la carpeta de la aplicación.

#### `bitacoraRetentionDays` — Retención

- **Tipo:** int
- **Default:** `90`
- **Rango:** 1–365
- **Descripción:** Días de retención de archivos de bitácora. Los archivos más antiguos se eliminan automáticamente.

#### `bitacoraMaxSizeMB` — Tamaño máximo por archivo

- **Tipo:** int
- **Default:** `100`
- **Descripción:** Cuando un archivo de bitácora supera este tamaño, se crea uno nuevo con marca de tiempo adicional.

---

### 2.8 Alertas por email (v1.1)

#### `enableEmailAlerts` — Habilitar alertas por correo

- **Tipo:** bool
- **Default:** `false`
- **Descripción:** Activa envío de correos para eventos críticos (errores de RF-07/RF-08 y umbral crítico).
- **Nota para ODL:** Si el servidor no tiene internet, mantener `false` o usar relay SMTP interno.

**Estado recomendado para despliegue actual (servidor sin internet):**

```json
{
  "enableEmailAlerts": false
}
```

#### `alertEmailTo` — Destinatario de alertas

- **Tipo:** string
- **Default:** `"camilo.ortegonc@outlook.com"`
- **Descripción:** Correo destino para notificaciones críticas.

#### `smtpHost`, `smtpPort`, `smtpUseSsl`, `smtpUser`, `smtpPassword`, `smtpFrom`

- **Descripción:** Parámetros de conexión SMTP.
- **Validación:** Si `enableEmailAlerts=true`, se valida host, puerto y remitente.

#### `startupMonitoringGraceMinutes` — Ventana de gracia post-reinicio

- **Tipo:** int
- **Default:** `10`
- **Rango:** 0–120
- **Descripción:** Minutos iniciales donde RF-07/RF-08 quedan activos en monitoreo pero sin limpieza automática inmediata. Diseñado para reinicios de servidor.
- **Recomendación ODL:** mantener `10` en producción.

**Ejemplo con relay interno (servidor sin internet):**

```json
{
  "enableEmailAlerts": true,
  "alertEmailTo": "camilo.ortegonc@outlook.com",
  "smtpHost": "smtp.interno.odl.local",
  "smtpPort": 25,
  "smtpUseSsl": false,
  "smtpUser": "",
  "smtpPassword": "",
  "smtpFrom": "fifo.alertas@odl.local"
}
```

---

## 3. Perfiles de configuración recomendados

### Perfil: Servidor dedicado

```json
{
  "thresholdPercent": 90,
  "cleanupCapPercent": 25,
  "scheduledFrequencyHours": 12,
  "preventiveThresholdDays": 2,
  "maxConcurrentAssets": 4,
  "deleteThrottleMs": 20,
  "useLowPriorityThreads": false
}
```

### Perfil: Servidor compartido (recomendado para SRVODLRTDNMICA)

```json
{
  "thresholdPercent": 85,
  "cleanupCapPercent": 20,
  "scheduledFrequencyHours": 6,
  "preventiveThresholdDays": 3,
  "maxConcurrentAssets": 2,
  "deleteThrottleMs": 50,
  "useLowPriorityThreads": true
}
```

### Perfil: Servidor bajo carga extrema

```json
{
  "thresholdPercent": 80,
  "cleanupCapPercent": 15,
  "scheduledFrequencyHours": 4,
  "preventiveThresholdDays": 5,
  "maxConcurrentAssets": 1,
  "deleteThrottleMs": 200,
  "useLowPriorityThreads": true,
  "eventBatchIntervalSeconds": 30
}
```

---

## 4. Edición manual del archivo

Si la aplicación no abre o necesita preconfigurar:

1. Abrir `fifo_config.json` con Notepad, VS Code, o cualquier editor de texto
2. Modificar los valores respetando la sintaxis JSON
3. Guardar el archivo (codificación UTF-8)
4. Iniciar o reiniciar la aplicación

**Errores comunes en edición manual:**
- Olvidar comillas en strings: `"storagePath": D:\Data` → debe ser `"storagePath": "D:\\Data"`
- Usar backslash simple: `"D:\Data"` → debe ser `"D:\\Data"` (doble backslash en JSON)
- Coma extra al final: `"deleteThrottleMs": 50,}` → eliminar la coma antes de `}`
- Valor fuera de rango: la aplicación mostrará error de validación al cargar

---

## 5. Notas operativas v1.1

- Tras reinicio del servidor, la aplicación puede iniciar automáticamente vía tarea `FIFO_AutoStart`.
- Al iniciar la app, `RF-07` y `RF-08` se reactivan según `enableScheduledCleanup` y `enablePreventiveCleanup`.
- Para operar en modo "solo monitoreo" (sin borrado): configurar umbrales conservadores y validar con la suite `v1.1`.


