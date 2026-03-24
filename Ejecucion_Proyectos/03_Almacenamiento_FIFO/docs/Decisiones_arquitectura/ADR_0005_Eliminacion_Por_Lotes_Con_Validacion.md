# ADR-0005: Eliminación por Lotes con Validación Pre/Post y Abort Seguro

**Estado:** Aceptada  
**Fecha:** 2026-02-16  
**Autor:** IDC Ingeniería  
**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Cliente:** ODL Instrumentación y Control

---

## Contexto

El motor FIFO debe eliminar archivos reales del servidor de monitoreo. Esta es la operación más crítica y riesgosa del sistema:

- Un error puede eliminar archivos de monitoreo activo, causando pérdida de datos irreversible
- El servidor debe seguir operando durante la limpieza (no puede detenerse el monitoreo)
- Se requiere trazabilidad completa de cada archivo eliminado (CA-04-06)
- Cero eliminaciones de archivos protegidos (CA-04-07, CA-04-08)
- Capacidad de pausar y cancelar durante ejecución (HU-03)

## Decisión

**Se implementa eliminación por lotes de máximo 100 archivos con validación pre-lote, validación post-lote y mecanismo de abort seguro.**

### Flujo de Eliminación

```
┌──────────────┐
│ PRE-VALIDACIÓN│ ← Verificar permisos, lista blanca, exclusiones
│  GLOBAL       │ ← Confirmar espacio actual, política vigente
└──────┬───────┘
       │ OK
┌──────▼───────┐
│ CONFIRMACIÓN │ ← Modal en UI: "¿Eliminar X archivos? (Y/N)"
│  EXPLÍCITA   │ ← Solo respuesta explícita acepta (CA-04-02)
└──────┬───────┘
       │ Confirmado
┌──────▼───────┐
│ LOTE 1/N     │ ← Máximo 100 archivos
│ Pre-check    │ ← ¿Archivo en whitelist? ¿Coincide exclusión?
│ Eliminar     │ ← DeleteFile + registro en bitácora
│ Post-check   │ ← ¿Espacio libre correcto? ¿Errores?
└──────┬───────┘
       │ OK → Siguiente lote
       │ ERROR → PAUSA + Reporte + Espera intervención
┌──────▼───────┐
│ LOTE 2/N     │
│ ...          │
└──────┬───────┘
       │
┌──────▼───────┐
│ VALIDACIÓN   │ ← Espacio libre final vs. esperado
│  FINAL       │ ← Conteo archivos eliminados vs. planeados
│              │ ← Registro completo en bitácora
└──────────────┘
```

### Reglas de Seguridad

1. **Pre-check por archivo:** Antes de eliminar cada archivo, verificar que NO está en lista blanca ni coincide con patrón de exclusión
2. **Post-check por lote:** Después de cada lote, verificar espacio libre y comparar con estimación
3. **Abort si espacio < 5%:** Si espacio libre cae por debajo del umbral de seguridad, abortar inmediatamente (CA-04-05)
4. **Abort si error:** Si un archivo no puede eliminarse (permiso denegado, en uso), pausar y reportar (CA-04-09)
5. **Confirmación no automática:** La confirmación debe ser respuesta explícita del operador; no hay timeout que auto-acepte (CA-10-05)

### Tamaño de Lote: 100 archivos

- Máximo 100 archivos por lote (CA-04-01)
- Máximo 10 segundos por lote (CA-04-10)
- Progress bar actualizada después de cada lote
- Botón "Pausar" y "Cancelar" evaluados entre lotes

## Alternativas Consideradas

### Alternativa 1: Eliminación archivo por archivo
- **Pros:** Máximo control, abort instantáneo
- **Contras:** Rendimiento inaceptable para miles de archivos; overhead de validación post-operación multiplicado; UI se actualiza demasiado frecuentemente

### Alternativa 2: Eliminación masiva sin lotes (todos de una vez)
- **Pros:** Máximo rendimiento, una sola operación
- **Contras:** No permite pausar/cancelar; un error a mitad puede dejar estado inconsistente; sin progress feedback; no permite abort seguro

### Alternativa 3: Mover a papelera de reciclaje
- **Pros:** Recuperación fácil, operación reversible nativa
- **Contras:** Papelera de reciclaje consume espacio en el mismo disco (no libera espacio inmediatamente); no es programable de forma confiable; puede llenar disco aún más

### Alternativa 4: Mover a carpeta temporal antes de eliminar
- **Pros:** Permite rollback por período; verificación antes de eliminación definitiva
- **Contras:** Requiere espacio adicional temporal (puede no haber espacio); duplica archivos brevemente; complejidad de gestión de carpeta temporal; si disco está al 85%, mover 15% de datos requiere espacio que no existe

## Justificación

1. **Granularidad controlada:** 100 archivos es suficiente para rendimiento (< 10s/lote) pero lo bastante pequeño para abort entre lotes
2. **Validación redundante:** Pre-check + post-check elimina riesgo de borrar archivos protegidos incluso si la lista en memoria está desactualizada
3. **Feedback continuo:** Operador ve progreso real y puede intervenir en cualquier momento (HU-03)
4. **Fail-safe:** Ante cualquier duda o error, el sistema PAUSA en lugar de continuar (principio de mínimo daño)
5. **Trazabilidad total:** Cada archivo eliminado se registra con ruta, tamaño, fecha y lote al que pertenece (CA-04-06)

## Consecuencias

### Positivas
- Máxima seguridad: validación doble antes y después de cada lote
- Operador mantiene control total con pausa/cancelar
- Trazabilidad archivo por archivo en bitácora
- Abort seguro: cualquier anomalía detiene el proceso
- Rendimiento aceptable: < 10s por lote de 100

### Negativas
- Overhead de validación (~5-10% tiempo adicional vs. eliminación directa)
- Complejidad de implementación del estado de la máquina de lotes
- Si hay miles de archivos, la operación total puede tomar varios minutos
- Mutex requerido: solo una operación de eliminación a la vez

### Riesgos y Mitigaciones

| Riesgo | Mitigación |
|--------|------------|
| Archivo bloqueado por otro proceso | Reintentar 3 veces con backoff; si falla, skip + registrar + continuar con siguiente |
| Espacio libre no coincide post-lote | Alertar pero no abortar (otro proceso pudo escribir) |
| UI se desconecta durante ejecución | Motor C++ continúa; bitácora registra; UI reconecta y muestra estado |
| Operador cierra ventana accidentalmente | Modal previene cierre durante ejecución activa (HU-03) |
