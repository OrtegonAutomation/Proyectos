# Historias de Usuario - FIFO

**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Cliente:** ODL Instrumentación y Control  
**Duración:** 1 mes  
**Versión:** 0.2 (Primera Edición)  
**Estado:** Borrador actualizado con arquitectura WPF/C++

---

## 1. Introducción a Historias de Usuario

Las historias de usuario describen funcionalidades desde la perspectiva del usuario final. Cada historia incluye:
- **Como** [rol de usuario]
- **Quiero** [capacidad o funcionalidad]
- **Para** [beneficio o valor]

---

## 2. Personas de Usuario

### Persona 1: Operador de Infraestructura ODL
**Nombre:** Carlos (Operador)  
**Rol:** Responsable de disponibilidad del servidor de monitoreo  
**Experiencia:** 5 años en infraestructura, básico en scripts  
**Necesidades:** Operación confiable, alertas claras, acciones rápidas

### Persona 2: Ingeniero de Confiabilidad IDC
**Nombre:** María (Ingeniero)  
**Rol:** Responsable de diseño e implementación FIFO  
**Experiencia:** 8 años, experta en scripting y monitoreo  
**Necesidades:** Flexibilidad, trazabilidad, reporte detallado

### Persona 3: Gerente de Operaciones ODL
**Nombre:** Roberto (Gerente)  
**Rol:** Autoridad responsable de políticas operacionales  
**Experiencia:** 10 años, visión estratégica  
**Necesidades:** Reportes ejecutivos, métricas, cumplimiento

---

## 3. Historias de Usuario - Operación Básica

### HU-01: Verificar Estado del Almacenamiento

**Como** operador de infraestructura (Carlos)  
**Quiero** ver en la pantalla principal estado actual: espacio usado, libre, % ocupación  
**Para** saber en 10 segundos si hay riesgo inmediato de disco lleno

**Criterios de aceptación:**
- [ ] Pantalla principal carga en < 2 segundos
- [ ] Dashboard muestra: Total GB, Usado GB, Libre GB, % ocupación
- [ ] Gráfica visual de espacio (pastel o barra) actualizada en tiempo real
- [ ] Tabla de Assets ordenados por % de llenado (mayor primero)
- [ ] Indicadores de color: verde (<70%), amarillo (70-85%), rojo (>85%)
- [ ] Botón "Refrescar" para forzar re-cálculo de inventario
- [ ] Tiempo de refresco < 30 segundos

**Notas:**
- Sin necesidad de abrir archivos o ejecutar comandos
- UI intuitiva para operador sin entrenamiento técnico

---

### HU-02: Simular Limpieza FIFO con Parámetros Personalizados

**Como** ingeniero María o técnico de pruebas  
**Quiero** cambiar parámetros en UI (ruta, limit, threshold, cap) y ver simulación con datos sintéticos  
**Para** validar comportamiento sin afectar datos reales

**Criterios de aceptación:**
- [ ] Panel de "Configuración de Simulación" con campos editables:
  * Ruta de simulación (ej: C:\Test\SimulatedAssets)
  * Período a simular (días)
  * Tamaño diario por Asset (GB)
  * Tasa de crecimiento diario (%)
  * Limit de almacenamiento (GB)
  * Threshold crítico (%)
  * Cap de limpieza (%)
- [ ] Botón "Generar Datos Sintéticos" crea estructura realista
- [ ] Botón "Ejecutar Simulación" muestra:
  * Assets ordenados por proporción de llenado
  * Preview de qué Assets necesitan limpieza
  * Preview de qué días serían eliminados (FIFO)
  * Estimación: espacio liberado, espacio final
- [ ] Datos sintéticos NO se eliminan (solo simulación en pantalla)
- [ ] Ejecución < 30 segundos incluso con 100GB simulado
- [ ] Exportar resultados a CSV

**Notas:**
- Interfaz amigable: sin necesidad de línea de comandos
- Cambios se reflejan inmediatamente en preview

---

### HU-03: Ejecutar Limpieza FIFO en Producción desde UI

**Como** operador (Carlos) o ingeniero (María)  
**Quiero** ver botón "Ejecutar Limpieza" en UI, confirmar, y ver progreso en vivo  
**Para** ejecutar limpieza real con confirmación visual

**Criterios de aceptación:**
- [ ] Panel "Ejecución" con botones claramente identificados:
  * Botón "Ejecutar Limpieza" (color rojo para criticidad)
  * Botón "Pausar" (disponible durante ejecución)
  * Botón "Cancelar" (disponible durante ejecución)
- [ ] Al hacer click "Ejecutar": modal de confirmación
  * Resume Assets a limpiar y tamaño estimado
  * Solicita confirmación explícita: "¿Ejecutar eliminación de X Assets?"
- [ ] Durante ejecución:
  * Progress bar por Asset
  * Log en vivo de carpetas eliminadas
  * Contador: "Eliminando carpeta 5 de 12..."
- [ ] Post-ejecución:
  * Resumen: carpetas eliminadas, espacio liberado, Assets procesados
  * Botón "Ver Bitácora" para detalles

**Notas:**
- UI impide cierres accidentales durante ejecución
- Botón Cancelar siempre funcional

---

### HU-04: Ver Bitácora de Operaciones en Tabla

**Como** operador o ingeniero  
**Quiero** ver tabla de operaciones recientes en UI con filtros  
**Para** auditar y verificar que el sistema está funcionando correctamente

**Criterios de aceptación:**
- [ ] Panel "Bitácora" con tabla mostrando:
  * Fecha y hora
  * Tipo de operación (Inventario, Simulación, Limpieza)
  * Asset afectado
  * Resultado (OK, ERROR)
  * Espacio liberado (si aplica)
  * Usuario/Sistema que ejecutó
- [ ] Filtros: por período, por tipo, por resultado, por Asset
- [ ] Búsqueda por texto libre
- [ ] Botón "Exportar a CSV"
- [ ] Tabla carga en < 2 segundos incluso con 10K registros
- [ ] Registros más recientes arriba

**Notas:**
- Tabla debe mostrar información de ejecuciones fallidas también
- Datos sensibles ofuscados si es necesario

---

---

### HU-05: Limpieza Preventiva Automática por Adición de Datos

**Como** operador (Carlos) o sistema automático  
**Quiero** que el sistema detecte cuando se añaden datos y automáticamente evalúe si necesita limpiar  
**Para** evitar sorpresas de disco lleno sin intervención manual

**Criterios de aceptación:**
- [ ] Sistema monitorea en tiempo real carpetas Asset/*/E y Asset/*/F
- [ ] Cuando se detecta adición de archivo:
  * UI muestra notificación: "Adición detectada en Asset1/02/E (45 GB)"
  * Sistema calcula velocidad de crecimiento
  * Sistema proyecta: "Al ritmo actual, espacio se agotará en 2.5 días"
- [ ] Si proyección < 3 días (configurable): 
  * Sistema ejecuta limpieza preventiva automáticamente
  * Enfocarse en la MISMA carpeta: Asset1/02/E
  * Empezar por día más antiguo: Asset1/02/E/2026/02/01
  * Eliminar hasta liberar espacio suficiente
- [ ] Bitácora registra:
  * Adición detectada: timestamp, Asset, tamaño, ubicación
  * Proyección: días hasta threshold
  * Acción tomada: limpieza preventiva SÍ/NO
  * Si limpieza: carpetas eliminadas, espacio liberado
- [ ] UI muestra historial de adiciones detectadas en tabla
- [ ] Botón "Ver Proyecciones" muestra gráfica de predicción

**Notas:**
- Eficiencia: limpiar en la ruta donde ocurrió adición evita escaneo completo
- Predictivo: adelantarse a problema en lugar de reaccionar
- Automático: sin necesidad de intervención operador

**Como** operador (Carlos)  
**Quiero** recibir notificación automática cuando espacio libre cae por debajo de umbral  
**Para** actuar rápidamente antes de que se detengarel monitoreo

**Criterios de aceptación:**
- [ ] Alarma generada cuando espacio < umbral crítico (ej: 15%)
- [ ] Notificación por canal configurado (email, syslog, evento Windows)
- [ ] Alarma incluye: espacio disponible, % utilizado, recomendación de acción
- [ ] Notificación dentro de 1 minuto del evento
- [ ] Permite silenciar falsas alarmas por período configurable
- [ ] Historial de alarmas guardado para auditoría

**Notas:**
- Debe escalarse si no se atiende en X horas
- Incluir información de contacto de escalación

---

---

### HU-06B: Configurar Umbral de Limpieza Preventiva (RF-08)

**Como** ingeniero (María)  
**Quiero** configurar en UI cuántos días de anticipación debe ejecutarse limpieza preventiva  
**Para** ajustar agresividad de limpieza según comportamiento del crecimiento inmediato

**Criterios de aceptación:**
- [ ] Campo en panel de configuración: "Días antes de limpiar (Preventiva)" (default: 3)
- [ ] Rango configurable: 1-10 días
- [ ] Botón "Guardar Configuración"
- [ ] Valores guardados en archivo JSON
- [ ] Cambios aplican inmediatamente a monitoreo en tiempo real
- [ ] Tooltip explica: "Si se detecta que en X días se alcanza límite, limpiar AHORA"
- [ ] INDEPENDIENTE de configuración de RF-07 (HU-07)

**Notas:**
- Bajo umbral (1 día) = limpieza más agresiva ante picos
- Alto umbral (10 días) = tolera mejor variaciones normales
- DIFERENCIA con HU-07:
  * HU-07 configura ejecución programada (RF-07)
  * HU-06B configura limpieza preventiva por evento (RF-08)
  * Ambas coexisten independientemente

**Como** ingeniero (María)  
**Quiero** configurar que el sistema se ejecute diariamente y que chequee el promedio histórico  
**Para** garantizar que no se llene el disco entre ejecuciones

**Criterios de aceptación:**
- [ ] Panel "Configuración Automática" con:
  * Campo: "Frecuencia de ejecución" (opciones: diaria, cada X horas)
  * Campo: "Hora de ejecución" (ej: 02:00 AM)
  * Checkbox: "Usar proyección histórica" (default: activado)
  * Campo: "Período histórico" (días a considerar para promedio, default: 7)
- [ ] En cada ejecución automática:
  * Calcular promedio de crecimiento: (GB últimos 7 días) / 7
  * Proyectar: promedio × días_hasta_próxima_ejecución
  * Si proyección > threshold crítico:
    → Ejecutar limpieza general (SIN enfoque local)
    → Limpiar proporcionalmente todos Assets
  * Si proyección <= threshold:
    → Registrar "próxima ejecución segura", NO limpiar
- [ ] Bitácora muestra:
  * Tipo: "Programada (RF-07)"
  * Promedio calculado, proyección, decisión
  * Si limpió: Assets procesados, espacio liberado
- [ ] Cambios aplican inmediatamente
- [ ] Botón "Ejecutar Ahora" para pruebas

**Notas:**
- DIFERENCIA con HU-05 (Limpieza Preventiva):
  * HU-07 es programada, HU-05 es por evento
  * HU-07 limpia global, HU-05 limpia local
  * HU-07 ve promedio histórico, HU-05 ve velocidad inmediata
  * AMBAS coexisten, NO se reemplazan

**Como** ingeniero (María) o gerente (Roberto)  
**Quiero** crear una política que especifique qué archivos se deben retener y cuáles se pueden eliminar  
**Para** asegurar que no se pierdan datos críticos mientras se libera espacio de datos antiguos

**Criterios de aceptación:**
- [ ] Archivo de configuración texto editable
- [ ] Define umbral crítico (% disco utilizado)
- [ ] Define umbral de recuperación (histéresis)
- [ ] Define lista blanca de rutas que NUNCA se tocan
- [ ] Define patrones de exclusión de archivos
- [ ] Define criterio FIFO (por fecha creación o modificación)
- [ ] Versionado: cada cambio es una nueva versión
- [ ] Comentarios en español explican cada sección

**Notas:**
- Cambios deben requerir aprobación formal antes de aplicar
- Debe permitir rollback a versión anterior

---

### HU-07: Configurar Frecuencia de Ejecución Automática

**Como** ingeniero (María)  
**Quiero** programar que la limpieza FIFO se ejecute automáticamente cada día (o cada X horas)  
**Para** no depender de intervención manual y tener protección continua

**Criterios de aceptación:**
- [ ] Configuración en archivo o interfaz
- [ ] Define frecuencia: cada X horas, diaria, semanal
- [ ] Define ventana de tiempo (ej: entre 2 AM y 4 AM)
- [ ] Permite pausar temporalmente la ejecución automática
- [ ] Cada ejecución queda registrada en bitácora
- [ ] Permite override manual si se necesita ejecutar fuera de ventana

**Notas:**
- Integración con Windows Task Scheduler
- Considerar cargas de trabajo: preferir horarios nocturnos

---

### HU-08: Configurar Canales de Notificación

**Como** operador o gerente  
**Quiero** especificar dónde debo recibir notificaciones (email, SMS, log del servidor, etc.)  
**Para** asegurar que me entero de problemas críticos inmediatamente

**Criterios de aceptación:**
- [ ] Soporta mínimo 3 canales: email, syslog, evento Windows
- [ ] Configuración por tipo de alarma (crítica, advertencia, informativa)
- [ ] Permite múltiples destinatarios por canal
- [ ] Valida configuración (ej: email válido, servidor SMTP accesible)
- [ ] Permite envío de prueba antes de validar

**Notas:**
- Considerar escalación: si email no se atiende, intentar alternativa
- Logs de notificación para auditar qué se notificó y cuándo

---

## 5. Historias de Usuario - Mantenimiento y Troubleshooting

### HU-09: Ejecutar Rollback de Limpieza

**Como** operador (Carlos)  
**Quiero** restaurar archivos que fueron eliminados por error en la última limpieza  
**Para** recuperarme de un incidente sin pérdida de datos

**Criterios de aceptación:**
- [ ] Comando: `fifo-rollback`
- [ ] Restaura archivos de las últimas 24 horas (configurable)
- [ ] Requiere confirmación explícita
- [ ] Verifica integridad de archivos restaurados
- [ ] Registra rollback en bitácora con razón y autorización
- [ ] Notifica cuando rollback finaliza
- [ ] Valida espacio disponible antes de restaurar

**Notas:**
- Rollback más viejo de X horas requiere recuperación de backup
- Documento de autorización de rollback necesario

---

### HU-10: Investigar Fallo de Limpieza

**Como** ingeniero (María)  
**Quiero** entender por qué falló una ejecución de limpieza  
**Para** corregir el problema y evitar que vuelva a ocurrir

**Criterios de aceptación:**
- [ ] Logs detallados de ejecución fallida
- [ ] Identifica exactamente dónde falló (escaneo, cálculo, eliminación)
- [ ] Incluye stacktrace si es error de programa
- [ ] Sugiere posibles causas (permisos, disco full, ruta inaccesible)
- [ ] Incluye datos contextuales (espacio disco, cantidad archivos)
- [ ] Permite descarga de logs completos en ZIP

**Notas:**
- Información sensible (rutas reales, usuarios) puede necesitarse ofuscar
- Considerarredondeo de números para privacidad

---

### HU-11: Cambiar Política de Retención sin Downtime

**Como** ingeniero (María)  
**Quiero** actualizar la política de retención sin detener la operación del sistema  
**Para** adaptar la política sin afectar monitoreo

**Criterios de aceptación:**
- [ ] Nueva política entra en vigor en siguiente ejecución programada
- [ ] Versión anterior está disponible para rollback rápido
- [ ] Sistema valida nueva política antes de aplicar
- [ ] Comparación clara: política vieja vs nueva (qué cambió)
- [ ] Requiere aprobación antes de aplicar (si es crítico)
- [ ] Ejecuta simulación con nueva política antes de producción

**Notas:**
- Cambios de política deben estar auditados con quién, cuándo, por qué
- Período de transición: 1 ejecución con política vieja, después la nueva

---

## 6. Historias de Usuario - Reportes y Análisis

### HU-12: Generar Reporte de Crecimiento de Almacenamiento

**Como** gerente (Roberto) o ingeniero (María)  
**Quiero** ver tendencia de crecimiento del almacenamiento en las últimas semanas  
**Para** proyectar si la política actual es suficiente o si necesita ajustes

**Criterios de aceptación:**
- [ ] Reporte muestra: tamaño diario/semanal, tasa de crecimiento, proyección
- [ ] Gráfica de tendencia (línea, barras)
- [ ] Desglose por ruta (cuál crece más rápido)
- [ ] Identifica rutas con crecimiento anormal
- [ ] Exportable a Excel o PDF
- [ ] En español con unidades claras (GB, % por día)

**Notas:**
- Útil para forecasting: "¿cuándo necesitaremos más espacio?"
- Comparación mes-a-mes para identificar cambios estacionales

---

### HU-13: Auditoría de Archivos Eliminados

**Como** gerente (Roberto) o auditor interno  
**Quiero** generar reporte de todos los archivos eliminados en un período (ej: último mes)  
**Para** cumplir requisitos de auditoría y tener trazabilidad completa

**Criterios de aceptación:**
- [ ] Reporte con: archivo, ruta, tamaño, fecha creación, fecha eliminación
- [ ] Filtrable por período, ruta, usuario que ejecutó limpieza
- [ ] Firma digital o hash del reporte para integridad
- [ ] Exportable a formatos estándar (CSV, Excel, PDF)
- [ ] Disponible para período mínimo de 3 meses
- [ ] Inmutable (no puede ser modificado posterior a generación)

**Notas:**
- Crítico para cumplimiento de políticas de retención
- Puede ser requerido en auditorías externas

---

## 7. Historias de Usuario - Capacitación y Documentación

### HU-14: Acceder a Guía de Operación

**Como** nuevo operador  
**Quiero** leer un manual paso a paso de cómo operar el sistema FIFO  
**Para** aprender sin depender de persona de IDC

**Criterios de aceptación:**
- [ ] Runbook disponible en español
- [ ] Capítulos: instalación, operación diaria, respuesta a alarmas, troubleshooting
- [ ] Ejemplos con salida real del sistema
- [ ] FAQ con preguntas comunes
- [ ] Contactos de escalación claros
- [ ] Documentación en formato PDF y web (si es posible)

**Notas:**
- Incluir qué hacer si "se llena el disco" (paso a paso)
- Incluir ejemplos de mensajes de error y qué significan

---

### HU-15: Recibir Capacitación Inicial

**Como** operador (Carlos)  
**Quiero** sesión de capacitación vivo de 2-3 horas sobre el sistema FIFO  
**Para** entender cómo funciona, qué puede salir mal, y cómo responder

**Criterios de aceptación:**
- [ ] Sesión vivo de mínimo 2 horas con Q&A
- [ ] Demostración en vivo: simulación, ejecución, consulta de bitácora
- [ ] Casos de uso: disco lleno, rollback, cambio de política
- [ ] Documentación de sesión (video si es posible)
- [ ] Formulario de evaluación post-capacitación
- [ ] Agenda comunicada con 1 semana de anticipación

**Notas:**
- Preferiblemente en español
- Incluir ejercicios prácticos si es posible

---

## 8. Historias de Usuario - Soporte Técnico

### HU-16: Escalar Problema a IDC

**Como** operador (Carlos)  
**Quiero** contactar a IDC cuando necesito ayuda y saber cómo se va a proceder  
**Para** obtener resolución rápida sin improvisar

**Criterios de aceptación:**
- [ ] Proceso de escalación claro y documentado
- [ ] Contactos de soporte con niveles de severidad (crítica, alta, normal)
- [ ] SLA definido: crítica < 2 horas, alta < 8 horas, normal < 24 horas
- [ ] Número único de ticket para seguimiento
- [ ] Confirmación de recepción en < 1 hora
- [ ] Actualizaciones semanales si no se resuelve

**Notas:**
- Incluir información a proporcionar en escalación (logs, contexto)
- Considerar contrato de soporte con IDC

---

## 9. Matriz de Prioridad

| ID | Historia | Prioridad | Complejidad | Sprint |
|----|---------:|:---------:|:-----------:|:------:|
| HU-01 | Verificar estado | Alta | Baja | 1 |
| HU-02 | Simular limpieza | Alta | Media | 1 |
| HU-03 | Ejecutar limpieza | Alta | Alta | 1 |
| HU-04 | Ver bitácora | Alta | Baja | 1 |
| HU-05 | Alarma disco lleno | Alta | Media | 2 |
| HU-06 | Política de retención | Alta | Media | 1 |
| HU-07 | Frecuencia automática | Media | Baja | 2 |
| HU-08 | Canales notificación | Media | Media | 2 |
| HU-09 | Rollback | Media | Alta | 2 |
| HU-10 | Investigar fallo | Media | Media | 2 |
| HU-11 | Cambiar política | Baja | Media | 3 |
| HU-12 | Reporte crecimiento | Baja | Media | 3 |
| HU-13 | Auditoría | Media | Baja | 2 |
| HU-14 | Guía operación | Alta | Baja | 3 |
| HU-15 | Capacitación | Media | Media | 4 |
| HU-16 | Escalación | Media | Baja | 1 |

---

## 10. Notas y Observaciones

- **[POR VALIDAR]** Prioridades y agrupamiento en sprints con ODL
- **[POR VALIDAR]** Historias adicionales específicas de excepciones operacionales
- **[POR VALIDAR]** Historias de integración con sistemas de monitoreo existentes
- **[POR VALIDAR]** Criterios de aceptación específicos de desempeño (umbrales exactos)

---

## 11. Estado del Documento

- **Versión:** 0.1 (Borrador)
- **Autor:** IDC Ingeniería
- **Fecha:** [A completar]
- **Pendiente:** Refinamiento con producto owner y validación de prioridades con ODL

