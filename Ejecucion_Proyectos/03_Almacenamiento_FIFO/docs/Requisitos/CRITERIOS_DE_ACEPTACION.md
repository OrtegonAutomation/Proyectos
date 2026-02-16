# Criterios de Aceptación - FIFO

**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Cliente:** ODL Instrumentación y Control  
**Duración:** 1 mes  
**Versión:** 0.2 (Primera Edición)  
**Estado:** Borrador actualizado con arquitectura WPF/C++

---

## 1. Introducción

Los criterios de aceptación definen exactamente qué debe cumplir el sistema para considerarse "terminado" y aceptable por el cliente. Cada criterio es mensurable y verificable.

---

## 2. Criterios de Aceptación - Alcance del Proyecto

### CA-01: Inventario de Almacenamiento

**Especificación:**  
El sistema debe inventariar el almacenamiento del servidor con precisión total.

**Criterios de Aceptación:**
- [ ] **CA-01-01:** Escaneo completa todas las rutas especificadas en < 60 segundos
- [ ] **CA-01-02:** Reporte de inventario incluye: ruta, tamaño en GB, cantidad de archivos, fecha última modificación
- [ ] **CA-01-03:** Precisión de tamaños: diferencia < 0.1% vs. herramienta de referencia (ej: Windows Explorer)
- [ ] **CA-01-04:** Identifica 100% de archivos > 1 MB
- [ ] **CA-01-05:** Detecta cambios de tamaño desde último inventario (crecimiento/decrecimiento)
- [ ] **CA-01-06:** Muestra tasa de crecimiento en GB/día con mínimo 7 días de histórico
- [ ] **CA-01-07:** Exporta inventario en CSV con especificación de codificación (UTF-8)
- [ ] **CA-01-08:** Maneja rutas inaccesibles con mensaje claro sin fallar operación total

**Validación:**
```
Comando: fifo-inventory
Entrada: (configuración cargada)
Salida esperada: 
  Total: 245.3 GB de 500 GB (49%)
  Crecimiento (últimos 7 días): +2.1 GB (0.3 GB/día)
  Rutas escaneadas: 8
  Archivos encontrados: 245,312
  Tiempo de escaneo: 47 segundos
```

---

### CA-02: Definición de Política de Retención

**Especificación:**  
El sistema debe leer y aplicar política de retención correctamente.

**Criterios de Aceptación:**
- [ ] **CA-02-01:** Archivo de configuración en formato texto (YAML, INI o similar) legible
- [ ] **CA-02-02:** Umbrales definibles: crítico 50-95%, recuperación 20-70%
- [ ] **CA-02-03:** Lista blanca soporta mínimo 20 rutas sin duplicados
- [ ] **CA-02-04:** Patrones de exclusión soportan wildcards (* y ?)
- [ ] **CA-02-05:** Criterio FIFO por fecha creación O por fecha modificación
- [ ] **CA-02-06:** Cambios a política requieren confirmación antes de aplicar
- [ ] **CA-02-07:** Versionado automático: política_v1.ini → política_v2.ini
- [ ] **CA-02-08:** Rollback a versión anterior en < 10 segundos
- [ ] **CA-02-09:** Validación de sintaxis: rechaza configuración malformada
- [ ] **CA-02-10:** Archivo de configuración inmutable después de aplicar (permisos)

**Validación:**
```
Archivo: policy.ini
[Thresholds]
critical_percent = 85
recovery_percent = 70

[Retention]
fifo_criteria = creation_date

[Whitelist]
path_1 = C:\Monitored\Vibration
path_2 = C:\Monitored\Process

[Exclusions]
pattern_1 = *.log
pattern_2 = *.tmp
```

---

### CA-03: Motor FIFO - Modo Simulación

**Especificación:**  
Modo simulación debe predecir exactamente qué se eliminaría sin ejecutar eliminación real.

**Criterios de Aceptación:**
- [ ] **CA-03-01:** Modo simulación NO elimina ningún archivo (verificable con hash de archivos)
- [ ] **CA-03-02:** Listado de archivos a eliminar detallado: ruta, nombre, tamaño, fecha creación
- [ ] **CA-03-03:** Espacio a liberar calculado con precisión 100% vs. modo producción
- [ ] **CA-03-04:** Orden de eliminación respeta FIFO: más antiguos primero
- [ ] **CA-03-05:** Respeta 100% de rutas en lista blanca: cero falsos positivos
- [ ] **CA-03-06:** Respeta 100% de patrones de exclusión: cero falsos positivos
- [ ] **CA-03-07:** Ejecución < 60 segundos incluso con 100K archivos
- [ ] **CA-03-08:** Simulación ejecutable múltiples veces con resultado idéntico
- [ ] **CA-03-09:** Salida exportable a CSV con todas las columnas
- [ ] **CA-03-10:** Resume al final: cantidad archivos a eliminar, espacio total, rutas afectadas

**Validación:**
```
Comando: fifo-simulate --policy policy.ini
Salida:
---
Simulación FIFO - Modo Seco
Fecha: 2026-02-16 10:30:45
Política: policy_v2.ini

ARCHIVOS A ELIMINAR:
1. C:\Data\Old\2023-01-15_vibration.csv | 125 MB | 2023-01-15
2. C:\Data\Old\2023-01-16_vibration.csv | 128 MB | 2023-01-16
... (250 más)

RESUMEN:
Total archivos a eliminar: 252
Espacio a liberar: 32.5 GB
Espacio libre resultado: 65.2 GB (13%)
Rutas afectadas: 3
Archivos protegidos por whitelist: 0
Tiempo simulación: 48 segundos
---
```

---

### CA-04: Motor FIFO - Modo Producción

**Especificación:**  
Eliminación real de archivos con máxima seguridad y trazabilidad.

**Criterios de Aceptación:**
- [ ] **CA-04-01:** Eliminación procede por lotes máximo 100 archivos
- [ ] **CA-04-02:** Requiere confirmación explícita antes de eliminar
- [ ] **CA-04-03:** Validación pre-eliminación: permisos, integridad lista blanca
- [ ] **CA-04-04:** Validación post-lote: espacio libre reportado correctamente
- [ ] **CA-04-05:** Aborta si espacio libre cae por debajo de umbral de seguridad (ej: 5%)
- [ ] **CA-04-06:** 100% de archivos eliminados registrados en bitácora
- [ ] **CA-04-07:** Cero eliminaciones de archivos en lista blanca (verificado)
- [ ] **CA-04-08:** Cero eliminaciones de archivos coincidentes patrones de exclusión
- [ ] **CA-04-09:** Si falla a mitad: pausa, genera reporte, espera intervención
- [ ] **CA-04-10:** Tiempo de eliminación: máximo 10 segundos/lote de 100 archivos

**Validación:**
```
Comando: fifo-cleanup --policy policy.ini --batch-size 100

¿Ejecutar eliminación con estos parámetros? (s/n): s
[Iniciando eliminación...]
[Lote 1/3] Eliminando 100 archivos... 8 segundos. OK
[Lote 2/3] Eliminando 100 archivos... 9 segundos. OK
[Lote 3/3] Eliminando 52 archivos... 5 segundos. OK
[Validando espacio...]

RESULTADO: EXITOSO
Archivos eliminados: 252
Espacio liberado: 32.5 GB
Espacio libre ahora: 65.2 GB (13%)
Tiempo total: 47 segundos
Registrado en bitácora: 252 entradas

[Fin de operación: 2026-02-16 10:45:22]
```

---

## 3. Criterios de Aceptación - Auditoría y Trazabilidad

### CA-05: Bitácora Completa

**Especificación:**  
Sistema mantiene registro auditable de todas las operaciones.

**Criterios de Aceptación:**
- [ ] **CA-05-01:** Cada operación registrada con timestamp exacto (hora:minuto:segundo)
- [ ] **CA-05-02:** Bitácora incluye: operación, resultado (OK/ERROR), usuario/sistema que la ejecutó, detalles
- [ ] **CA-05-03:** Archivo de bitácora inmutable después de cierre de sesión
- [ ] **CA-05-04:** Bitácora retiene mínimo 90 días de histórico
- [ ] **CA-05-05:** Rotación automática de bitácora cuando alcanza 100 MB
- [ ] **CA-05-06:** Bitácora archivada es comprimida (ZIP) para ahorro de espacio
- [ ] **CA-05-07:** Cada eliminación de archivo registra: ruta, nombre, tamaño, usuario, razón
- [ ] **CA-05-08:** Consulta de bitácora por período (fecha inicio - fecha fin)
- [ ] **CA-05-09:** Exportación a CSV con delimitador coma y escape de caracteres especiales
- [ ] **CA-05-10:** Hash o firma digital de bitácora para detectar modificaciones no autorizadas

**Validación:**
```
Comando: fifo-log --from 2026-02-01 --to 2026-02-16
[16/02/2026 10:45:22] OPERACIÓN: CLEANUP | RESULTADO: OK | USUARIO: SYSTEM | POLITICA: policy_v2.ini
  - Archivos eliminados: 252
  - Espacio liberado: 32.5 GB
  - Detalles: [archivo1, archivo2, ...252 registros]
[16/02/2026 10:30:45] OPERACIÓN: SIMULATE | RESULTADO: OK | USUARIO: maria.ingenieria | POLITICA: policy_v2.ini
  - Archivos a eliminar (predicción): 252
  - Espacio a liberar (predicción): 32.5 GB
...
Total registros mostrados: 47
```

---

### CA-06: Alarmas y Notificaciones

**Especificación:**  
Sistema genera alarmas automáticas para eventos críticos.

**Criterios de Aceptación:**
- [ ] **CA-06-01:** Alarma crítica cuando espacio < umbral (ej: 15%)
- [ ] **CA-06-02:** Alarma cuando ejecución falla
- [ ] **CA-06-03:** Alarma cuando crecimiento anormal detectado (> 2x promedio)
- [ ] **CA-06-04:** Notificación generada dentro de 30 segundos del evento
- [ ] **CA-06-05:** Notificación incluye: descripción problema, valor actual, valor esperado
- [ ] **CA-06-06:** Soporta múltiples canales: syslog, email, evento Windows
- [ ] **CA-06-07:** Mensaje de alarma en español claro y profesional
- [ ] **CA-06-08:** Historial de alarmas disponible para auditoría
- [ ] **CA-06-09:** Alarma no se repite si condición persiste (coalescing)
- [ ] **CA-06-10:** Permite "silenciar" alarma por X horas para mantenimiento

**Validación:**
```
[CRÍTICA] Espacio disco bajo - 12 GB libres (2%)
Recomendación: Ejecutar fifo-cleanup inmediatamente
Timestamp: 2026-02-16 14:23:45
Contacto escalación: +57 2 2316359

Notificaciones enviadas:
✓ Email a operaciones@odl.com (14:23:50)
✓ Syslog registrado (14:23:45)
✓ Evento Windows generado (14:23:46)
```

---

## 4. Criterios de Aceptación - Rendimiento y Confiabilidad

### CA-07: Desempeño

**Especificación:**  
Sistema opera dentro de límites de desempeño definidos.

**Criterios de Aceptación:**
- [ ] **CA-07-01:** Inventario 500 GB en < 60 segundos
- [ ] **CA-07-02:** Inventario 2 TB en < 90 segundos
- [ ] **CA-07-03:** Simulación 100K archivos en < 60 segundos
- [ ] **CA-07-04:** Eliminación 100 archivos en < 10 segundos
- [ ] **CA-07-05:** Generación de alarma en < 30 segundos de evento
- [ ] **CA-07-06:** Ejecución automática no consume > 25% CPU
- [ ] **CA-07-07:** Ejecución no requiere > 500 MB RAM
- [ ] **CA-07-08:** Consulta de bitácora 1000 registros en < 5 segundos
- [ ] **CA-07-09:** Exportación CSV de 1000 registros en < 10 segundos
- [ ] **CA-07-10:** Rollback de eliminación en < 2 minutos

**Validación (Stress Test):**
```
TEST: Desempeño inventario
Escenario: Servidor con 2 TB de datos, 5M archivos
Resultado:
  ✓ Inventario completado en 82 segundos
  ✓ CPU máximo: 18%
  ✓ RAM pico: 320 MB
  ✓ Precisión: 100%
```

---

### CA-08: Disponibilidad y Confiabilidad

**Especificación:**  
Sistema mantiene disponibilidad y evita pérdida de datos.

**Criterios de Aceptación:**
- [ ] **CA-08-01:** Cero corrupción de datos en 90 días de operación
- [ ] **CA-08-02:** Cero eliminaciones accidentales de archivos críticos
- [ ] **CA-08-03:** Disponibilidad ≥ 99.5% (máximo 3.6 horas downtime/mes)
- [ ] **CA-08-04:** MTTR (tiempo medio de recuperación) < 30 minutos
- [ ] **CA-08-05:** Rollback disponible para últimas 24 horas de operación
- [ ] **CA-08-06:** Recuperación automática de fallos transitorios
- [ ] **CA-08-07:** Lista blanca protege 100% de archivos críticos
- [ ] **CA-08-08:** Backup de configuración y bitácora automático diario
- [ ] **CA-08-09:** Validación post-operación confirma integridad
- [ ] **CA-08-10:** Cero operaciones "silenciosas" (todas registradas)

**Validación:**
```
PRUEBA: Confiabilidad en 90 días
Período: 16/02/2026 - 16/05/2026
Resultados:
  ✓ Uptime: 99.7% (2.2 horas downtime)
  ✓ Fallos corregidos automáticamente: 3 (reconexión red)
  ✓ Fallos requirieron intervención: 0
  ✓ Datos perdidos: 0
  ✓ Falsos positivos: 0
```

---

## 5. Criterios de Aceptación - Seguridad

### CA-09: Control de Acceso y Auditoría

**Especificación:**  
Solo usuarios autorizados pueden ejecutar operaciones críticas.

**Criterios de Aceptación:**
- [ ] **CA-09-01:** Ejecución de limpieza (producción) requiere permisos administrativos
- [ ] **CA-09-02:** Cambio de política requiere confirmación y puede ser bloqueado por administrador
- [ ] **CA-09-03:** Bitácora solo legible para roles autorizados
- [ ] **CA-09-04:** Cada acción crítica registra: usuario, IP (si aplica), timestamp
- [ ] **CA-09-05:** Sistema integra con Active Directory si está disponible
- [ ] **CA-09-06:** Intentos de acceso no autorizado generan alarma
- [ ] **CA-09-07:** Archivos de configuración tienen permisos de lectura restringida
- [ ] **CA-09-08:** Bitácora no puede ser eliminada ni truncada por usuarios normales
- [ ] **CA-09-09:** Rollback requiere aprobación explícita de autoridad responsable
- [ ] **CA-09-10:** Auditoría de cambios de configuración: quién, qué, cuándo

**Validación:**
```
TEST: Control de acceso
- Usuario sin permisos ejecuta fifo-cleanup → RECHAZADO
- Usuario admin cambia policy.ini → Registrado en bitácora
- Intento de borrar bitácora → Rechazado con alarma de seguridad
```

---

## 6. Criterios de Aceptación - Usabilidad

### CA-10: Facilidad de Uso

**Especificación:**  
Sistema es fácil de usar para operadores sin entrenamiento extensivo.

**Criterios de Aceptación:**
- [ ] **CA-10-01:** Comandos tienen nombres intuitivos (fifo-check, fifo-cleanup, etc)
- [ ] **CA-10-02:** Ayuda disponible: `comando --help` en español
- [ ] **CA-10-03:** Mensajes de error describen problema y sugieren acción
- [ ] **CA-10-04:** Valores por defecto son seguros (no destructivos)
- [ ] **CA-10-05:** Confirmaciones obligan a respuesta explícita (no Y/N automático)
- [ ] **CA-10-06:** Salida en español, legible, sin jerga técnica innecesaria
- [ ] **CA-10-07:** Soporte para parámetros comunes: --help, --version, --verbose
- [ ] **CA-10-08:** Documentación con ejemplos reales de ejecución
- [ ] **CA-10-09:** Runbook con procedimientos paso a paso
- [ ] **CA-10-10:** FAQ respondiendo preguntas operacionales comunes

**Validación:**
```
$ fifo-check --help
Uso: fifo-check [opciones]
Descripción: Verifica estado actual del almacenamiento

Opciones:
  --detailed    Muestra desglose por ruta
  --export-csv  Exporta a archivo CSV
  --help        Muestra esta ayuda

Ejemplo: fifo-check --detailed --export-csv inventario.csv
```

---

## 7. Criterios de Aceptación - Documentación

### CA-11: Documentación Completa

**Especificación:**  
Sistema acompañado de documentación detallada en español.

**Criterios de Aceptación:**
- [ ] **CA-11-01:** README.md con descripción, instalación, inicio rápido
- [ ] **CA-11-02:** Manual de operación: setup inicial, operación diaria, respuesta a alarmas
- [ ] **CA-11-03:** Runbook: procedimientos probados para escenarios comunes
- [ ] **CA-11-04:** Troubleshooting: problemas comunes y soluciones
- [ ] **CA-11-05:** FAQ: preguntas frecuentes respondidas
- [ ] **CA-11-06:** Arquitectura: diagrama y descripción de componentes
- [ ] **CA-11-07:** Especificación técnica: parámetros, formato configuración, interfaces
- [ ] **CA-11-08:** Changelog: versiones, cambios, fechas
- [ ] **CA-11-09:** Contactos de soporte con niveles de severidad y SLA
- [ ] **CA-11-10:** Documentación entregada en PDF y Web (si es posible)

**Validación:**
```
Documentación entregada:
✓ README.md (500+ palabras)
✓ Manual_Operacion.pdf (20+ páginas)
✓ Runbook.pdf (15+ páginas)
✓ Troubleshooting.pdf (10+ páginas)
✓ FAQ.pdf (5+ páginas)
✓ Arquitectura.pdf (5+ páginas)
```

---

## 8. Criterios de Aceptación - Capacitación

### CA-12: Capacitación del Cliente

**Especificación:**  
Personal de ODL capacitado para operar sistema de forma independiente.

**Criterios de Aceptación:**
- [ ] **CA-12-01:** Sesión de capacitación presencial de 2-3 horas
- [ ] **CA-12-02:** Capacitación incluye demostración vivo con datos reales
- [ ] **CA-12-03:** Operador puede ejecutar: verificación, simulación, limpieza, consulta bitácora
- [ ] **CA-12-04:** Operador entiende cómo responder a alarmas comunes
- [ ] **CA-12-05:** Operador puede cambiar configuración bajo supervisión
- [ ] **CA-12-06:** Sesión de Q&A grabada para referencia futura
- [ ] **CA-12-07:** Material de capacitación entregado en PDF
- [ ] **CA-12-08:** Formulario de evaluación post-capacitación completado
- [ ] **CA-12-09:** Plan de soporte post-go-live definido
- [ ] **CA-12-10:** Contacto técnico de IDC designado para escalaciones

**Validación:**
```
Sesión de capacitación realizada:
Fecha: 16/02/2026
Duración: 2.5 horas
Asistentes: 4 (Carlos, Pedro, Ana, Jorge)
Temas cubiertos:
  ✓ Operación diaria (15 min)
  ✓ Manejo de alarmas (20 min)
  ✓ Simulación vs producción (15 min)
  ✓ Demo en vivo (45 min)
  ✓ Q&A (25 min)
Evaluación: Todos responden correctamente preguntas operacionales
```

---

## 9. Matriz de Rastreabilidad de Requerimientos

| Requerimiento | CA# | Validable | Crítica |
|---------------|-----|-----------|---------|
| RF-01 (Inventario) | CA-01 | Sí | Alta |
| RF-02 (Política) | CA-02 | Sí | Alta |
| RF-03 (Simulación) | CA-03 | Sí | Alta |
| RF-04 (Producción) | CA-04 | Sí | Crítica |
| RF-05 (Bitácora) | CA-05 | Sí | Alta |
| RF-06 (Alarmas) | CA-06 | Sí | Alta |
| RNF-01 (Desempeño) | CA-07 | Sí | Media |
| RNF-04 (Confiabilidad) | CA-08 | Sí | Alta |
| RNF-07 (Seguridad) | CA-09 | Sí | Alta |
| UX General | CA-10 | Sí | Media |
| Documentación | CA-11 | Sí | Media |
| Capacitación | CA-12 | Sí | Media |

---

## 10. Proceso de Validación

### Fase 1: Validación Unitaria (Semana 1-2)
- Cada componente probado independientemente
- CA-01 a CA-05 validadas en ambiente de desarrollo

### Fase 2: Validación de Integración (Semana 3)
- Componentes integrados
- CA-06 a CA-09 validadas
- Pruebas en ambiente similar a producción

### Fase 3: Validación de Aceptación (Semana 4)
- Prueba en servidor ODL real
- CA-07, CA-08, CA-10, CA-11 validadas
- Capacitación con CA-12

### Fase 4: Validación en Operación (30-90 días)
- Monitoreo post-go-live
- Verificación de disponibilidad (CA-08)
- Auditoría de operaciones (CA-05)

---

## 11. Notas y Observaciones

- NA

---

## 12. Estado del Documento

- **Versión:** 0.1 (Borrador)
- **Autor:** IDC Ingeniería
- **Fecha:** [A completar]
- **Pendiente:** Validación con ODL, ajuste de métricas, firma de aceptación

