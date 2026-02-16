# Plan de Pruebas de Aceptación de Usuario (UAT) — FIFO

**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Cliente:** ODL Instrumentación y Control  
**Empresa:** IDC Ingeniería  
**Versión:** 1.0  
**Estado:** Borrador  
**Fecha:** 2026-02-16

---

## 1. Objetivo

Verificar que el sistema FIFO cumple con las expectativas operacionales del cliente ODL en su ambiente real de producción, validando que los operadores pueden ejecutar todas las funciones críticas de forma independiente y segura.

---

## 2. Alcance UAT

### 2.1 En Alcance

| # | Escenario | Persona | Referencia |
|---|-----------|---------|------------|
| UAT-01 | Ver dashboard y estado del almacenamiento | Operador (Carlos) | HU-01, CA-01 |
| UAT-02 | Ejecutar simulación FIFO con datos reales | Ingeniero (María) | HU-02, CA-03 |
| UAT-03 | Ejecutar limpieza FIFO en producción | Operador (Carlos) | HU-03, CA-04 |
| UAT-04 | Consultar bitácora de operaciones | Operador (Carlos) | HU-04, CA-05 |
| UAT-05 | Recibir alarma de disco lleno | Operador (Carlos) | HU-05, CA-06 |
| UAT-06 | Configurar política de retención | Ingeniero (María) | HU-06, CA-02 |
| UAT-07 | Verificar ejecución programada automática (RF-07) | Ingeniero (María) | HU-07 |
| UAT-08 | Verificar monitoreo preventivo (RF-08) | Ingeniero (María) | HU-05 |
| UAT-09 | Exportar reportes | Gerente (Roberto) | HU-12, HU-13 |
| UAT-10 | Responder a escenario de emergencia | Operador (Carlos) | HU-09, HU-10 |

### 2.2 Fuera de Alcance UAT

- Pruebas de estrés con 2 TB (ya validadas en pruebas de sistema)
- Pruebas de código fuente o unitarias
- Configuración de infraestructura de red (SMTP, syslog)

---

## 3. Prerrequisitos

### 3.1 Ambiente

- [ ] Servidor ODL con sistema FIFO instalado y configurado
- [ ] Datos reales de al menos 2 Assets con histórico de 7+ días
- [ ] Configuración de email funcional para alarmas
- [ ] Cuentas de usuario configuradas: 1 admin, 1 operador
- [ ] Backup completo del servidor antes de iniciar UAT

### 3.2 Participantes

| Rol | Persona | Responsabilidad |
|-----|---------|-----------------|
| Facilitador UAT | Ingeniero IDC (María) | Guiar pruebas, responder dudas |
| Ejecutor principal | Operador ODL (Carlos) | Ejecutar escenarios operacionales |
| Observador | Gerente ODL (Roberto) | Validar reportes y aprobación final |
| Soporte técnico | Desarrollador IDC | Resolver incidencias técnicas en el acto |

### 3.3 Materiales

- [ ] Este documento impreso para cada participante
- [ ] Formulario de resultados (test_results_live.xlsx)
- [ ] Runbook de operación como referencia
- [ ] Cronómetro para medición de tiempos de respuesta

---

## 4. Escenarios UAT Detallados

### UAT-01: Ver Dashboard Principal

**Ejecutor:** Operador (Carlos)  
**Duración estimada:** 10 minutos

| Paso | Acción | Resultado Esperado | ✓/✗ |
|------|--------|--------------------|-----|
| 1 | Abrir aplicación FIFO | Dashboard carga en < 2 segundos | |
| 2 | Verificar datos mostrados | Total GB, Usado GB, Libre GB, % ocupación visibles | |
| 3 | Identificar estado por color | Verde (<70%), Amarillo (70-85%), Rojo (>85%) | |
| 4 | Verificar tabla de Assets | Assets ordenados por % llenado, mayor primero | |
| 5 | Hacer click "Refrescar" | Datos se actualizan en < 30 segundos | |
| 6 | Comparar con Windows Explorer | Diferencia de tamaño < 0.1% | |

**Criterio de aceptación:** Operador puede identificar estado del disco en 10 segundos sin ayuda.

---

### UAT-02: Ejecutar Simulación FIFO

**Ejecutor:** Ingeniero (María)  
**Duración estimada:** 15 minutos

| Paso | Acción | Resultado Esperado | ✓/✗ |
|------|--------|--------------------|-----|
| 1 | Ir a pestaña Simulación | Panel de configuración visible | |
| 2 | Seleccionar modo "Datos Reales" | Sistema lee datos del disco real | |
| 3 | Hacer click "Ejecutar Simulación" | Simulación completa en < 60 segundos | |
| 4 | Verificar lista de archivos a eliminar | Ordenados por fecha (FIFO): más antiguos primero | |
| 5 | Verificar que archivos en whitelist NO aparecen | 0 archivos protegidos en lista de eliminación | |
| 6 | Verificar resumen | Total archivos, espacio a liberar, rutas afectadas | |
| 7 | Hacer click "Exportar CSV" | Archivo CSV descargado con datos completos | |
| 8 | Ejecutar simulación de nuevo | Resultado idéntico (determinístico) | |
| 9 | Verificar que NINGÚN archivo fue eliminado | Hash de archivos sin cambios | |

**Criterio de aceptación:** Simulación predice exactamente lo que se eliminaría sin tocar datos reales.

---

### UAT-03: Ejecutar Limpieza FIFO en Producción

**Ejecutor:** Operador (Carlos) con supervisión de Ingeniero (María)  
**Duración estimada:** 20 minutos  
**⚠️ PRECAUCIÓN:** Esta prueba ELIMINA archivos reales. Asegurar backup previo.

| Paso | Acción | Resultado Esperado | ✓/✗ |
|------|--------|--------------------|-----|
| 1 | Ir a pestaña Ejecución | Botón "Ejecutar Limpieza" visible (rojo) | |
| 2 | Click "Ejecutar Limpieza" | Modal de confirmación con resumen de operación | |
| 3 | Verificar resumen en modal | Assets a limpiar, tamaño estimado correcto | |
| 4 | Confirmar ejecución | Progress bar aparece, log en vivo | |
| 5 | Observar progreso | "Eliminando carpeta X de Y..." | |
| 6 | Esperar finalización | Resumen: archivos eliminados, espacio liberado | |
| 7 | Verificar espacio en disco | Espacio libre aumentó según lo estimado | |
| 8 | Click "Ver Bitácora" | Operación registrada con todos los detalles | |
| 9 | Verificar archivos en whitelist | INTACTOS, sin modificación | |

**Criterio de aceptación:** Limpieza exitosa, espacio liberado, cero archivos protegidos afectados.

---

### UAT-04: Consultar Bitácora

**Ejecutor:** Operador (Carlos)  
**Duración estimada:** 10 minutos

| Paso | Acción | Resultado Esperado | ✓/✗ |
|------|--------|--------------------|-----|
| 1 | Ir a pestaña Bitácora | Tabla con registros recientes | |
| 2 | Verificar última operación | Limpieza de UAT-03 registrada con detalles | |
| 3 | Filtrar por tipo "Limpieza" | Solo operaciones de limpieza visibles | |
| 4 | Filtrar por período (hoy) | Solo registros de hoy | |
| 5 | Buscar por texto libre | Resultados correctos | |
| 6 | Click "Exportar a CSV" | Archivo CSV con registros filtrados | |
| 7 | Verificar tiempo de carga | < 2 segundos con historial de pruebas | |

**Criterio de aceptación:** Operador encuentra cualquier operación en < 30 segundos.

---

### UAT-05: Recibir Alarma de Disco Lleno

**Ejecutor:** Ingeniero (María) simula condición  
**Duración estimada:** 15 minutos

| Paso | Acción | Resultado Esperado | ✓/✗ |
|------|--------|--------------------|-----|
| 1 | Simular condición de disco lleno (agregar archivos grandes) | Sistema detecta espacio < threshold | |
| 2 | Esperar notificación | Alarma generada en < 30 segundos | |
| 3 | Verificar popup en UI | Mensaje claro en español con recomendación | |
| 4 | Verificar email | Email recibido con descripción del problema | |
| 5 | Verificar Windows Event Log | Evento registrado en Application log | |
| 6 | Verificar que alarma NO se repite | Si condición persiste, no hay segunda alarma en 4h | |
| 7 | Operador ejecuta limpieza | Alarma se resuelve | |

**Criterio de aceptación:** Operador notificado en < 1 minuto por al menos 2 canales.

---

### UAT-06: Configurar Política de Retención

**Ejecutor:** Ingeniero (María)  
**Duración estimada:** 15 minutos

| Paso | Acción | Resultado Esperado | ✓/✗ |
|------|--------|--------------------|-----|
| 1 | Ir a pestaña Configuración | Campos editables visibles | |
| 2 | Cambiar threshold crítico (ej: 85% → 80%) | Campo acepta valor, validación en vivo | |
| 3 | Agregar ruta a whitelist | Ruta aceptada y mostrada en lista | |
| 4 | Agregar patrón de exclusión (ej: *.tmp) | Patrón aceptado | |
| 5 | Click "Guardar Configuración" | Confirmación de guardado | |
| 6 | Verificar versionado | Nueva versión creada (policy_v2.ini) | |
| 7 | Ejecutar simulación con nueva política | Resultados reflejan nuevos parámetros | |
| 8 | Rollback a versión anterior | Política anterior restaurada en < 10 segundos | |
| 9 | Ingresar valor inválido (ej: threshold 150%) | Error claro: "Valor debe estar entre 50 y 95%" | |

**Criterio de aceptación:** Cambio de política aplica correctamente y es reversible.

---

### UAT-07: Verificar Ejecución Programada (RF-07)

**Ejecutor:** Ingeniero (María)  
**Duración estimada:** 15 minutos (+ verificación posterior)

| Paso | Acción | Resultado Esperado | ✓/✗ |
|------|--------|--------------------|-----|
| 1 | Verificar configuración de frecuencia | Frecuencia actual visible (ej: cada 24h) | |
| 2 | Cambiar hora de ejecución a +5 minutos desde ahora | Configuración guardada | |
| 3 | Esperar ejecución automática | Sistema ejecuta en la hora programada | |
| 4 | Verificar bitácora | Registro "Programada (RF-07)" con razonamiento | |
| 5 | Verificar que calculó promedio histórico | Promedio de crecimiento en registro | |
| 6 | Click "Ejecutar Ahora" | Ejecución inmediata fuera de ventana programada | |

**Criterio de aceptación:** Ejecución programada funciona automáticamente y registra razonamiento.

---

### UAT-08: Verificar Monitoreo Preventivo (RF-08)

**Ejecutor:** Ingeniero (María)  
**Duración estimada:** 20 minutos

| Paso | Acción | Resultado Esperado | ✓/✗ |
|------|--------|--------------------|-----|
| 1 | Configurar umbral preventivo a 3 días | Configuración guardada | |
| 2 | Agregar archivos grandes a un Asset específico | Sistema detecta adición | |
| 3 | Verificar notificación en UI | "Adición detectada en AssetX (N GB)" | |
| 4 | Verificar cálculo de proyección | "Al ritmo actual, espacio se agotará en X días" | |
| 5 | Si proyección < 3 días: verificar limpieza automática | Limpieza LOCAL ejecutada en Asset afectado | |
| 6 | Verificar bitácora | Registro "Preventiva (RF-08)" con trigger y velocidad | |
| 7 | Verificar que limpieza fue LOCAL | Solo se eliminaron archivos del Asset afectado | |

**Criterio de aceptación:** RF-08 detecta picos y actúa automáticamente de forma localizada.

---

### UAT-09: Exportar Reportes

**Ejecutor:** Gerente (Roberto)  
**Duración estimada:** 10 minutos

| Paso | Acción | Resultado Esperado | ✓/✗ |
|------|--------|--------------------|-----|
| 1 | Ir a pestaña Reportes | Opciones de reporte visibles | |
| 2 | Seleccionar "Reporte de Crecimiento" | Gráfica de tendencia visible | |
| 3 | Seleccionar período (último mes) | Datos del período seleccionado | |
| 4 | Exportar a PDF | PDF generado con gráficas y métricas | |
| 5 | Seleccionar "Auditoría de Eliminados" | Lista de archivos eliminados | |
| 6 | Exportar a CSV | CSV con: archivo, ruta, tamaño, fecha | |

**Criterio de aceptación:** Gerente obtiene reportes ejecutivos sin asistencia técnica.

---

### UAT-10: Escenario de Emergencia

**Ejecutor:** Operador (Carlos) con supervisión  
**Duración estimada:** 15 minutos

| Paso | Acción | Resultado Esperado | ✓/✗ |
|------|--------|--------------------|-----|
| 1 | Simular disco lleno (>90%) | Dashboard muestra ROJO, alarma generada | |
| 2 | Operador identifica problema | "Espacio crítico" visible en < 10 segundos | |
| 3 | Operador ejecuta simulación rápida | Ve qué se eliminaría | |
| 4 | Operador ejecuta limpieza de emergencia | Limpieza exitosa, espacio recuperado | |
| 5 | Operador verifica resultado | Dashboard vuelve a AMARILLO o VERDE | |
| 6 | Operador consulta bitácora | Toda la operación registrada | |

**Criterio de aceptación:** Operador resuelve emergencia en < 15 minutos sin ayuda de IDC.

---

## 5. Formulario de Resultados UAT

| Campo | Descripción |
|-------|-------------|
| Escenario | UAT-XX |
| Ejecutor | Nombre del tester |
| Fecha/Hora | Timestamp de ejecución |
| Resultado | PASÓ / FALLÓ / PARCIAL |
| Observaciones | Comentarios del ejecutor |
| Defectos encontrados | IDs de defectos si los hay |
| Captura de pantalla | Adjunta si es relevante |

Resultados detallados se registran en `test_results_live.xlsx`.

---

## 6. Criterios de Aceptación UAT

### Para aprobar UAT:

- [ ] ≥ 8 de 10 escenarios UAT pasados completamente
- [ ] UAT-03 (Limpieza producción) DEBE pasar obligatoriamente
- [ ] UAT-05 (Alarma disco lleno) DEBE pasar obligatoriamente
- [ ] 0 defectos S1 (Críticos) abiertos
- [ ] ≤ 2 defectos S2 (Altos) con plan de corrección
- [ ] Operador declara "puedo operar esto solo" en formulario de evaluación
- [ ] Gerente ODL firma documento de aceptación

### Para rechazar UAT:

- Cualquier escenario obligatorio FALLÓ
- Operador no puede completar flujo principal sin ayuda
- Defecto S1 encontrado que pone en riesgo datos

---

## 7. Post-UAT

### 7.1 Acciones Post-Aprobación
1. Generar TEST_RESULTS_SUMMARY.pdf con resultados consolidados
2. Firmar acta de aceptación con ODL
3. Iniciar período de soporte post-go-live (30-90 días)
4. Programar sesión de capacitación formal (HU-15, CA-12)

### 7.2 Acciones Post-Rechazo
1. Documentar defectos encontrados con detalle
2. Acordar plan de corrección con fechas
3. Re-ejecutar escenarios fallidos después de corrección
4. Programar nueva sesión UAT

---

## 8. Aprobaciones

| Rol | Nombre | Firma | Fecha |
|-----|--------|-------|-------|
| Facilitador UAT (IDC) | [Pendiente] | | |
| Operador ODL | [Pendiente] | | |
| Gerente ODL | [Pendiente] | | |
| Gerente Proyecto IDC | [Pendiente] | | |

---

**Versión:** 1.0  
**Estado:** Borrador  
**Próxima revisión:** [Pendiente]
