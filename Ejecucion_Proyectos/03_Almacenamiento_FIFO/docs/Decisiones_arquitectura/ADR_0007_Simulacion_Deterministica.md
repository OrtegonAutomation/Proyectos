# ADR-0007: Modo Simulación Determinístico con Datos Sintéticos

**Estado:** Aceptada  
**Fecha:** 2026-02-16  
**Autor:** IDC Ingeniería  
**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Cliente:** ODL Instrumentación y Control

---

## Contexto

Antes de ejecutar una limpieza FIFO en producción, operadores e ingenieros necesitan:

1. **Validar comportamiento:** Confirmar que la política eliminará los archivos correctos (CA-03)
2. **Capacitar operadores:** Permitir práctica sin riesgo de eliminar datos reales (HU-15)
3. **Ajustar parámetros:** Experimentar con distintos umbrales y ver resultados antes de aplicar (HU-02)
4. **Auditoría previa:** Generar reporte de "qué se eliminaría" para aprobación antes de ejecutar

El modo simulación debe ser 100% seguro: cero archivos eliminados, resultado idéntico al modo producción, y ejecutable múltiples veces con resultado idéntico (CA-03-08).

## Decisión

**Se implementan dos modos de simulación complementarios:**

### Modo 1: Simulación sobre Datos Reales (Dry-Run)
- Ejecuta el motor FIFO completo sobre el sistema de archivos real
- Calcula exactamente qué archivos se eliminarían
- **NO ejecuta la operación de eliminación** (`DeleteFile` nunca se invoca)
- Genera reporte idéntico al que produciría el modo producción
- Verificable: hash de archivos antes y después confirma cero modificaciones (CA-03-01)

### Modo 2: Simulación con Datos Sintéticos
- Genera estructura de archivos virtual en memoria (sin escribir a disco)
- Permite configurar: cantidad de Assets, tamaño por día, tasa de crecimiento, período
- Ejecuta motor FIFO sobre estructura virtual
- Útil para experimentar con parámetros sin acceso a datos reales
- Determinístico: mismos parámetros → mismo resultado siempre

### Interfaz en UI (Pestaña Simulación)

```
┌─────────────────────────────────────────────┐
│ SIMULACIÓN FIFO                             │
├─────────────────────────────────────────────┤
│ Modo: ○ Datos Reales  ● Datos Sintéticos   │
│                                             │
│ [Configuración Sintética]                   │
│ Ruta simulada: C:\Test\SimulatedAssets      │
│ Período: 30 días                            │
│ Tamaño diario/Asset: 2 GB                  │
│ Tasa crecimiento: 5%                        │
│ Limit: 500 GB                               │
│ Threshold: 85%                              │
│ Cap: 20%                                    │
│                                             │
│ [Generar Datos] [Ejecutar Simulación]       │
│                                             │
│ ─── RESULTADOS ───                          │
│ Sección 1: Assets ordenados por llenado     │
│ Sección 2: Archivos a eliminar (FIFO)       │
│ Sección 3: Resumen y estadísticas           │
│                                             │
│ [Exportar CSV]                              │
└─────────────────────────────────────────────┘
```

## Alternativas Consideradas

### Alternativa 1: Solo dry-run (sin datos sintéticos)
- **Pros:** Más simple, un solo modo, resultado 100% real
- **Contras:** Requiere acceso al servidor real para cualquier prueba; no permite experimentación libre; no útil en desarrollo/capacitación sin datos de producción

### Alternativa 2: Copiar datos reales a carpeta temporal
- **Pros:** Datos realistas, simulación completa
- **Contras:** Requiere espacio duplicado en disco (posiblemente no disponible); tiempo de copia significativo; puede interferir con monitoreo

### Alternativa 3: Simulación basada en snapshots de metadata
- **Pros:** Ligero, no requiere datos reales
- **Contras:** Complejidad de serialización/deserialización de metadata; snapshot puede desactualizarse; formato propietario

## Justificación

1. **Seguridad absoluta:** Dry-run usa el mismo motor pero con `DeleteFile` deshabilitado — imposible eliminar accidentalmente
2. **Experimentación libre:** Datos sintéticos permiten probar escenarios extremos (disco 95% lleno, 1M archivos) sin riesgo
3. **Determinismo:** Simulación sintética con misma semilla produce resultado idéntico, esencial para testing y demos (CA-03-08)
4. **Capacitación:** Operadores practican en modo sintético antes de tocar producción (HU-15)
5. **Aprobación previa:** Gerente puede revisar reporte de dry-run y aprobar antes de ejecución real
6. **Rendimiento:** Simulación sintética en memoria es más rápida que acceso a disco (< 30s para 100 GB simulados)

## Consecuencias

### Positivas
- Cero riesgo de pérdida de datos en simulación
- Dos modos cubren todos los casos de uso (validación real + experimentación)
- Exportación CSV permite revisión offline por auditores
- Modo sintético permite demos y capacitación sin servidor real

### Negativas
- Dos modos de simulación implican testing de ambos caminos
- Datos sintéticos pueden no reflejar distribución real de archivos
- Dry-run sobre 2 TB toma el mismo tiempo que el inventario real (~90s)
- Resultado de simulación puede cambiar si otros procesos modifican archivos entre simulación y ejecución

### Mitigaciones
- Pre-ejecución: comparar snapshot de simulación vs. estado actual antes de eliminar
- Datos sintéticos incluyen distribuciones configurables (uniforme, exponencial) para realismo

---

**Revisores:** [Pendiente]  
**Aprobado por:** [Pendiente]
