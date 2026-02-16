# ADR-0002: Estrategia FIFO Dual — Ejecución Programada (RF-07) + Preventiva (RF-08)

**Estado:** Aceptada  
**Fecha:** 2026-02-16  
**Autor:** IDC Ingeniería  
**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Cliente:** ODL Instrumentación y Control

---

## Contexto

El servidor de monitoreo de ODL recibe datos continuamente de múltiples Assets (vibración, proceso, etc.). El espacio en disco puede agotarse por:

1. **Crecimiento gradual:** Acumulación constante de datos día a día
2. **Picos repentinos:** Adición masiva de datos en un Asset específico (ej: descarga de históricos, cambio de frecuencia de muestreo)

Un único mecanismo de limpieza no puede cubrir ambos escenarios eficientemente.

## Decisión

**Se implementan dos mecanismos de limpieza FIFO independientes y complementarios que coexisten sin interferencia.**

### RF-07 — Ejecución Programada (Global)
- **Frecuencia:** Configurable, default cada 24 horas (2:00 AM)
- **Análisis:** Promedio histórico de crecimiento (últimos 7 días)
- **Alcance:** Limpieza GENERAL — todos los Assets proporcionalmente
- **Lógica:** Si proyección histórica indica que próximas 24h superan threshold → ejecutar limpieza global; si no → skip con registro en bitácora
- **Analogía:** "Limpieza de rutina programada"

### RF-08 — Monitoreo Preventivo (Local)
- **Frecuencia:** 24/7 continuo, por evento
- **Análisis:** Velocidad inmediata de adición detectada en tiempo real
- **Alcance:** Limpieza LOCAL — solo el Asset donde ocurrió el pico
- **Lógica:** Si velocidad inmediata proyecta que en < 3 días (configurable) se alcanza límite → limpiar FIFO empezando por día más antiguo del mismo Asset
- **Analogía:** "Extintor de emergencia ante pico inesperado"

### Coexistencia
- Si ambas intentan ejecutarse simultáneamente, una espera a la otra (mutex)
- Cada una registra su tipo en bitácora: "Programada (RF-07)" o "Preventiva (RF-08)"
- Configuraciones independientes: cambiar umbral de RF-08 no afecta RF-07

## Alternativas Consideradas

### Alternativa 1: Solo limpieza programada (sin RF-08)
- **Pros:** Más simple, un solo mecanismo
- **Contras:** Vulnerable a picos repentinos entre ejecuciones programadas; si se añaden 50 GB a las 3 PM y la limpieza es a las 2 AM, el disco puede llenarse 11 horas antes

### Alternativa 2: Solo monitoreo continuo (sin RF-07)
- **Pros:** Respuesta inmediata a cualquier cambio
- **Contras:** Alto consumo de recursos por escaneo continuo; no optimiza para crecimiento gradual predecible; puede generar muchas limpiezas pequeñas en lugar de una consolidada

### Alternativa 3: Limpieza reactiva (solo al cruzar threshold)
- **Pros:** Mínima intervención, máximo uso del disco
- **Contras:** Riesgo de que al cruzar threshold ya sea demasiado tarde si la limpieza toma varios minutos; el servidor de monitoreo puede detenerse por disco lleno antes de completar la limpieza

### Alternativa 4: Un solo motor con dos modos
- **Pros:** Código compartido
- **Contras:** Mezcla lógica de decisión global (histórica) con local (inmediata); más difícil de configurar y debuggear; un bug afecta ambos mecanismos

## Justificación

1. **Cobertura completa:** RF-07 cubre crecimiento predecible; RF-08 cubre picos impredecibles. Juntos garantizan que el disco NUNCA se llene por sorpresa
2. **Optimización de recursos:** RF-07 consolida limpieza en horario nocturno (bajo impacto); RF-08 solo actúa cuando es necesario (mínima frecuencia)
3. **Granularidad:** RF-08 limpia solo donde es necesario (Asset específico), evitando escaneo completo; RF-07 balancea todos los Assets proporcionalmente
4. **Auditabilidad:** Cada mecanismo registra su razonamiento en bitácora, facilitando auditoría y troubleshooting (CA-05)
5. **Configurabilidad independiente:** Operador puede ajustar agresividad de cada mecanismo sin afectar al otro (HU-06B, HU-07)

## Consecuencias

### Positivas
- Protección 24/7 contra disco lleno desde dos ángulos complementarios
- Limpieza preventiva localizada reduce I/O innecesario
- Bitácora diferenciada facilita análisis post-incidente
- Configuración independiente permite ajuste fino por escenario

### Negativas
- Mayor complejidad de implementación (dos subsistemas)
- Requiere mutex/sincronización cuando ambos intentan ejecutar simultáneamente
- Dos conjuntos de parámetros a documentar y capacitar
- Testing debe cubrir interacción entre ambos mecanismos

### Riesgos
- Si RF-08 limpia datos que RF-07 iba a limpiar, RF-07 podría hacer skip innecesariamente (mitigado: RF-07 recalcula antes de ejecutar)
- Si watchers de filesystem fallan, RF-08 queda ciego (mitigado: fallback a polling periódico)

## Parámetros Configurables

| Parámetro | RF-07 | RF-08 |
|-----------|-------|-------|
| Frecuencia | 1-24 horas (default: 24) | Continuo (por evento) |
| Análisis | Promedio últimos N días (default: 7) | Velocidad inmediata |
| Umbral acción | Proyección > threshold | Proyección < N días (default: 3) |
| Alcance | Global (todos Assets) | Local (Asset afectado) |
| Horario | Configurable (default: 2 AM) | 24/7 |

## Diagrama de Decisión

```
┌─────────────────────────────┐
│    DATOS LLEGAN AL DISCO    │
└──────────────┬──────────────┘
               │
       ┌───────┴───────┐
       │               │
  ┌────▼────┐    ┌─────▼─────┐
  │ RF-08   │    │  RF-07    │
  │ Detecta │    │  Timer    │
  │ adición │    │  2:00 AM  │
  └────┬────┘    └─────┬─────┘
       │               │
  ┌────▼────┐    ┌─────▼──────┐
  │ Vel.    │    │ Promedio   │
  │ inmed.  │    │ 7 días     │
  │ > riesgo│    │ > threshold│
  │ 3 días? │    │ 24h?       │
  └────┬────┘    └─────┬──────┘
   Sí  │  No      Sí   │  No
   │   │          │     │
   ▼   ▼          ▼     ▼
 LIMPIAR SKIP   LIMPIAR SKIP
 LOCAL          GLOBAL
   │              │
   ▼              ▼
 BITÁCORA       BITÁCORA
 "Preventiva"   "Programada"
```

---

**Revisores:** [Pendiente]  
**Aprobado por:** [Pendiente]
