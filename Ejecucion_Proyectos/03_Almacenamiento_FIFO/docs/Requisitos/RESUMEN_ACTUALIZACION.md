# Casos de Uso - Actualización a Versión 0.3 (UI-Centric)

**Fecha:** 11/02/2026  
**Documento:** `CASOS_DE_USO.md`  
**Cambio Principal:** Reemplazo de comandos CLI → Interfaz WPF  

---

## Resumen de Cambios

### ✅ CU-01: Ver Dashboard Principal
- **Antes:** Comando `fifo-check` → Salida en pantalla
- **Ahora:** Pestaña Dashboard en UI → Gráficas, tablas, indicadores visuales
- **Cambio:** Visualización interactiva con código de colores (Verde/Amarillo/Rojo)

### ✅ CU-02: Configurar Parámetros
- **Antes:** Editar archivos JSON o comando `fifo-config`
- **Ahora:** Pestaña Configuración → Campos editables con validación en vivo
- **Cambio:** Interfaz amigable, tooltips explicativos, botón "Guardar" condicional

### ✅ CU-03: Simular Limpieza con Datos Sintéticos
- **Antes:** Comando `fifo-simulate --policy policy.ini` → CSV
- **Ahora:** Pestaña Simulación → Preview en 3 secciones, exportar directo
- **Cambio:** Generación de datos sintéticos realistas, 30s max, ejecución determinística

### ✅ CU-04: Ejecutar Limpieza FIFO en Producción
- **Antes:** Comando `fifo-cleanup` → Confirmación por texto
- **Ahora:** Pestaña Ejecución → Modal confirmación, progreso en vivo, log vivo
- **Cambio:** Botones Pausar/Cancelar, progress bar, resumen final, link "Ver Bitácora"

### ✅ CU-05: Ver y Filtrar Bitácora (REDISEÑO COMPLETO)
- **Antes:** Comando `fifo-log --from X --to Y` → Tabla en terminal
- **Ahora:** Pestaña Bitácora → Tabla filtrable, búsqueda, paginación, export CSV
- **Cambio:** Interfaz Excel-like, filtros dinámicos, detalles en modal

### ✅ CU-06: Responder a Alarma de Disco Lleno
- **Antes:** Email + ejecución manual de limpieza
- **Ahora:** Popup en UI + Email → Dashboard muestra "CRÍTICO" → Botón "Limpieza Emergencia"
- **Cambio:** Escalación automática, evaluación de RF-07 próxima, sugerencias intelligentes

### ✅ CU-07: Monitoreo Preventivo Automático (RF-08)
- **Antes:** No documentado explícitamente en CU
- **Ahora:** Proceso automático 24/7 en background, detalles en bitácora
- **Cambio:** Execución inteligente basada en velocidad inmediata, limpieza LOCAL

### ✅ CU-08: Ejecución Programada Automática (RF-07)
- **Antes:** Tarea de Windows Task Scheduler sin validación de seguridad
- **Ahora:** Algoritmo de proyección histórica, decisión inteligente (ejecutar o skip)
- **Cambio:** Limpieza GENERAL solo si necesario, bitácora con razonamiento completo

### ✅ CU-09: Exportar Reportes
- **Antes:** Comando `fifo-report --type executive --period monthly`
- **Ahora:** Pestaña Reportes → Dropdown tipos, date range, formato, generar PDF
- **Cambio:** Generador visual con gráficas, métricas ejecutivas, firma digital

### ✅ CU-10: Escalar Problema a Soporte Técnico
- **Antes:** Comando `fifo-support create` → Recopilación manual
- **Ahora:** Dialog "Reportar Problema" en UI → Recopilación automática, envío email
- **Cambio:** Número de ticket único, SLA automático, escalación por severidad

---

## Estadísticas del Documento

| Métrica | Antes | Ahora | Cambio |
|---------|-------|-------|--------|
| Líneas | 677 | 872 | +195 (+28.8%) |
| Tamaño KB | 35.2 | 45.7 | +10.5 (+29.8%) |
| Enfoque | CLI-based | WPF/UI-based | 100% cambio |
| CU con UI | 0 | 10 | Todos |

---

## Arquitectura UI: 6 Pestañas Principales

```
┌──────────────────────────────────────────────┐
│  APLICACIÓN WPF FIFO - Interfaz Principal    │
├──────────────────────────────────────────────┤
│                                              │
│ 📊 Dashboard    📋 Configuración              │
│ 🧪 Simulación   ▶️ Ejecución                 │
│ 📝 Bitácora      📈 Reportes                 │
│                                              │
│ [Botón Reportar Problema]                    │
│                                              │
├──────────────────────────────────────────────┤
│ BACKGROUND AUTOMÁTICO:                       │
│ • RF-07 (Programada): Diaria a 2 AM         │
│ • RF-08 (Preventiva): 24/7 continuo         │
│ • Alarmas: Email + Popup simultáneamente    │
│                                              │
└──────────────────────────────────────────────┘
```

---

## Indicadores Visuales: Código Semáforo

| % Ocupación | Color | Estado | Acción |
|------------|-------|--------|--------|
| < 70% | 🟢 Verde | Normal | Monitoreo continuo |
| 70-85% | 🟡 Amarillo | Atención | Preparar limpieza |
| > 85% | 🔴 Rojo | Crítico | Ejecutar limpieza AHORA |

---

## Validación en Vivo (Campos Configurables)

- **Threshold:** 50-95% (default: 85%)
- **Cap (limpieza máx):** 5-50% (default: 20%)
- **Frecuencia RF-07:** 1-24 horas (default: 24 = diaria)
- **Umbral RF-08:** 1-10 días (default: 3 días)
- **Ruta Base:** Debe existir y ser accesible
- **Límite GB:** Capacidad total en GB

---

## Coexistencia RF-07 + RF-08 Explicada

### RF-07 (Ejecución Programada)
- **Cuándo:** Cada 24 horas (ej: 2 AM)
- **Cómo:** Promedio histórico de 7 días
- **Qué:** Limpieza GENERAL (todos los Assets)
- **Decisión:** "¿Próximas 24h seguras?" → Sí: skip, No: ejecutar
- **Documento:** Bitácora con proyección y razonamiento

### RF-08 (Monitoreo Preventivo)
- **Cuándo:** 24/7 continuo
- **Cómo:** Velocidad inmediata de adición
- **Qué:** Limpieza LOCAL (solo Asset donde ocurrió pico)
- **Decisión:** "¿Próximos 3 días en riesgo?" → Sí: ejecutar, No: skip
- **Documento:** Bitácora con trigger y velocidad

### Coexistencia
- ✅ **No interfieren:** Contextos diferentes (histórico vs inmediato)
- ✅ **Se complementan:** Protección 24/7
- ✅ **Se colan:** Si ambas actúan simultáneamente, una espera la otra
- ✅ **Resultado:** Disco NUNCA se llena por sorpresa

---

## Tiempos de Respuesta Especificados

| Operación | Máximo | Realidad Esperada |
|-----------|--------|------------------|
| Dashboard carga | 2s | 1-2s (caché) |
| Refrescar inventario | 30s | 5-15s (paralelo) |
| Simulación | 30s | 15-25s (100GB) |
| Limpieza ejecución | Sin límite | 2-10 min (datos reales) |
| Bitácora búsqueda | 2s | < 1s (indexed) |
| Reportes generación | Sin límite | 5-15s (PDF) |

---

## Próximos Pasos de Implementación

1. **Diseño Visual:** Mockups de cada pestaña (Figma/Adobe)
2. **Especificación Técnica:** Protocolo C++ ↔ WPF (JSON messages)
3. **Plan de Desarrollo:** Fases, sprints, dependencias
4. **Plan de Testing:** Casos de prueba basados en CA
5. **Manual de Usuario:** Guías paso a paso en español
6. **Documentación Técnica:** API, eventos, manejo de errores

---

## Validaciones Requeridas Antes de Desarrollo

- [ ] Flujos UI validados con diseñador UX
- [ ] Modales y confirmaciones revisadas con usuarios finales
- [ ] Tiempos de respuesta validados en ambiente de prueba
- [ ] Accesibilidad WCAG AA verificada
- [ ] Traducción de etiquetas al español completa
- [ ] Mensajes de error claros y accionables
- [ ] Manejo de excepciones documentado
- [ ] Recuperación de fallos (rollback, recuperación) testeado

---

## Conclusión

Los **10 Casos de Uso** ahora están completamente alineados con la **Interfaz WPF**, proporcionando experiencia usuario clara y consistente. Cada caso especifica:

✅ Interacciones precisas en UI  
✅ Flujos alternativos con manejo de errores  
✅ Validaciones y restricciones  
✅ Indicadores visuales y feedback  
✅ Procesos automáticos en background  
✅ Integración con bitácora para auditoría  

**El sistema FIFO está completamente especificado y listo para implementación.**

---

**Versión:** 0.3 (UI-Centric)  
**Estado:** ✅ Completado  
**Siguiente:** Validación con stakeholders
