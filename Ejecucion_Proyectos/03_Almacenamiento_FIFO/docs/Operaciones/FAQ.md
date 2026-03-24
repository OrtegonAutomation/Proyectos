# FAQ — Preguntas Frecuentes — FifoCleanup v1.0

**Proyecto:** Gestión de Almacenamiento FIFO  
**Cliente:** ODL Instrumentación y Control  
**Autor:** IDC Ingeniería  
**Fecha:** Marzo 2026

---

## General

### ¿Qué hace FifoCleanup?

FifoCleanup gestiona automáticamente el espacio en disco del servidor de monitoreo. Cuando el almacenamiento se acerca a su límite, elimina los datos más antiguos primero (política FIFO — First In, First Out). Esto garantiza que el software de monitoreo nunca se quede sin espacio.

### ¿Puedo perder datos importantes?

La política FIFO **siempre elimina los datos más antiguos primero**. Los datos más recientes se preservan. Además:
- Existe un **cap de limpieza** que limita cuánto se puede eliminar por ejecución (default: 20%)
- Siempre puede ejecutar una **vista previa (dry run)** antes de la limpieza real
- Toda eliminación queda registrada en la **bitácora de auditoría**

### ¿La aplicación necesita estar abierta todo el tiempo?

Sí, para que los servicios automáticos (RF-07 y RF-08) funcionen, la aplicación debe estar ejecutándose. Si la aplicación se cierra, los servicios se detienen automáticamente y se registra en la bitácora.

### ¿Se puede ejecutar como servicio de Windows?

En la versión actual (v1.0), FifoCleanup es una aplicación de escritorio WPF. La ejecución como servicio de Windows está planeada para v2.0.

### ¿Qué pasa si se reinicia el servidor?

La aplicación necesita iniciarse manualmente después de un reinicio. Se recomienda agregar un acceso directo a la carpeta de Inicio de Windows para que se ejecute automáticamente.

---

## Configuración

### ¿Cuál es el umbral recomendado?

Para un servidor de monitoreo típico, **85%** es un buen umbral. Esto deja 15% de margen para que el sistema operativo y otros procesos funcionen correctamente.

| Escenario | Umbral recomendado |
|-----------|-------------------|
| Servidor dedicado solo a monitoreo | 85–90% |
| Servidor compartido con otros servicios | 75–80% |
| Disco con alto crecimiento diario | 70–80% |

### ¿Qué significa "Cap de limpieza"?

Es el **máximo porcentaje de datos monitoreados** que se puede eliminar en una sola ejecución. Actúa como protección contra borrado excesivo accidental.

- **20% (default):** Conservador, elimina máximo 1/5 de los datos por ejecución
- **30%:** Moderado
- **50%:** Agresivo, solo para emergencias

### ¿Qué es "Espacio máximo (MaxStorageSizeGB)"?

Si es > 0, los umbrales se calculan contra este valor en lugar del tamaño real del disco.

**Ejemplo:** Si el disco es de 2 TB pero solo quieres asignar 500 GB al monitoreo:
- Configurar `MaxStorageSizeGB = 500`
- El umbral del 85% se calculará sobre 500 GB (= 425 GB), no sobre 2 TB

Si es 0 (default), se usa el tamaño real del disco.

### ¿Puedo cambiar la configuración mientras RF-07/RF-08 están activos?

Sí. Los cambios se guardan en el archivo JSON y los servicios los utilizan en la siguiente evaluación. No es necesario reiniciarlos.

### ¿Dónde se guarda la configuración?

En un archivo JSON llamado `fifo_config.json`, ubicado en la carpeta de la aplicación (o en la ruta configurada en `ConfigFilePath`).

---

## Inventario y Dashboard

### ¿Qué tipos de carpetas se cuentan?

Solo las carpetas **E** (eventos) y **F** (frecuencia) se incluyen en el inventario y son candidatas para limpieza FIFO. Las carpetas de tendencias (numeradas como años, ej: `2026/`) dentro de las variables se ignoran.

### ¿Cómo se calcula la tasa de crecimiento?

Se analiza el tamaño de las carpetas de día de los **últimos 7 días** y se calcula el promedio diario. Esta tasa se usa para proyectar cuándo el disco alcanzará el umbral.

### ¿Qué significan los colores del semáforo?

| Color | Uso del disco | Acción |
|-------|--------------|--------|
| 🟢 Verde | < 70% | Ninguna, operación normal |
| 🟡 Amarillo | 70% – 85% | Monitorear, verificar que RF-07/RF-08 estén activos |
| 🔴 Rojo | > 85% | Limpieza automática activada o necesaria |

### ¿El inventario afecta al software de monitoreo?

No significativamente. El inventario solo lee el sistema de archivos (no modifica ni bloquea archivos). Usa `EnumerateFiles` con lazy evaluation para minimizar uso de memoria. El impacto en I/O es comparable a un `dir /s` en la consola.

---

## Limpieza FIFO

### ¿Cómo decide qué eliminar?

1. Ordena **todas** las carpetas de día de **todos** los Assets por fecha, de más antigua a más reciente
2. Elimina empezando por la más antigua hasta liberar suficiente espacio
3. Se detiene cuando el uso baja 5% debajo del umbral o cuando alcanza el cap

### ¿Puedo proteger ciertos Assets de la limpieza?

En la versión actual (v1.0), todos los Assets dentro de la ruta de almacenamiento son candidatos para limpieza. La protección por lista blanca está planeada para v2.0.

### ¿Qué pasa si la limpieza se interrumpe?

Si cancela la limpieza o la aplicación se cierra inesperadamente:
- Los datos ya eliminados NO se recuperan
- Los datos no procesados siguen intactos
- La bitácora registra la cancelación
- Puede ejecutar otra limpieza para continuar

### ¿Cuál es la diferencia entre RF-07 y RF-08?

| Aspecto | RF-07 (Programada) | RF-08 (Preventiva) |
|---------|-------------------|-------------------|
| Tipo | Evaluación periódica | Monitoreo en tiempo real |
| Alcance | Global (todos los Assets) | Local (Asset/Variable específico) |
| Trigger | Timer (cada N horas) | FileSystemWatcher (archivo nuevo) |
| Proyección | Basada en promedio de 7 días | Basada en crecimiento inmediato del Asset |
| Limpieza | General FIFO (más antiguo de todos) | Local FIFO (más antiguo del Asset afectado) |

### ¿RF-07 y RF-08 pueden ejecutarse al mismo tiempo?

No. Existe protección contra ejecución simultánea. Si RF-07 está limpiando, RF-08 espera, y viceversa. Cada servicio registra su tipo en la bitácora.

### ¿Qué es el throttling?

Es una pausa deliberada (default: 50 ms) entre eliminaciones de carpetas consecutivas. Reduce los picos de I/O en el disco para que el software de monitoreo no se vea afectado durante la limpieza.

---

## Bitácora

### ¿La bitácora se puede editar?

No. Los archivos de bitácora son **append-only** (solo se agregan líneas, nunca se modifican las existentes). Esto garantiza la integridad de la auditoría.

### ¿Cuánto espacio ocupa la bitácora?

Cada entrada ocupa aproximadamente 200 bytes. Un día típico con operaciones normales genera < 1 MB. La rotación automática crea un archivo nuevo si supera 100 MB. Los archivos antiguos (> 90 días por defecto) se eliminan automáticamente.

### ¿Puedo abrir la bitácora en Excel?

Sí. Los archivos `bitacora_YYYYMMDD.csv` se pueden abrir directamente en Excel. El delimitador es coma y los campos están entre comillas dobles. Codificación: UTF-8.

---

## Simulación

### ¿La simulación afecta mis datos reales?

**No.** La simulación genera datos sintéticos en una carpeta temporal separada. Nunca lee, modifica ni elimina datos reales de producción.

### ¿Cuánto espacio necesita la simulación?

Depende de los parámetros configurados. Por ejemplo, con los valores por defecto (5 Assets × 3 Variables × 30 días × 50 MB/día), necesita aproximadamente 45 GB. Puede reducir los parámetros para simulaciones más ligeras.

### ¿Qué es la "simulación continua"?

Es un modo que simula el crecimiento de datos en tiempo real. Genera archivos periódicamente (como lo haría el software de monitoreo) para probar que RF-07 y RF-08 reaccionan correctamente. Útil para pruebas de integración.

---

## Rendimiento

### ¿Cuánto CPU/RAM usa la aplicación?

| Operación | CPU típico | RAM típica |
|-----------|-----------|-----------|
| Idle (esperando) | < 1% | 80–120 MB |
| Inventario | 5–15% | 150–200 MB |
| Limpieza | 3–10% | 120–180 MB |
| Simulación (generación) | 10–20% | 150–250 MB |

Con `UseLowPriorityThreads = true`, la limpieza cede CPU al software de monitoreo.

### ¿El sistema funciona con discos de 2 TB?

Sí. Ha sido diseñado para escalar hasta 2 TB / 5 millones de archivos. El inventario completa en < 90 segundos incluso con ese volumen (en el hardware del servidor SRVODLRTDNMICA).

---

## Actualización y soporte

### ¿Cómo actualizo a una nueva versión?

1. Apagar la aplicación (detener RF-07/RF-08 primero)
2. Respaldar `fifo_config.json` y la carpeta `bitacora/`
3. Copiar los nuevos archivos de la aplicación
4. Reiniciar la aplicación
5. Verificar que la configuración y bitácora se conservaron

### ¿Dónde reporto un problema?

Contactar soporte técnico IDC. Ver detalles en [CONTACTOS_SLA.md](CONTACTOS_SLA.md).

