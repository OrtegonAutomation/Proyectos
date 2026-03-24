# Control de Cambios — OCR Operativo

**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Versión:** 1.0
**Fecha inicio:** Enero 2026

---

## Registro de cambios

| ID | Fecha | Solicitante | Descripción del cambio | Impacto | Estado | Aprobado por |
|----|-------|------------|----------------------|---------|--------|-------------|
| CC-001 | Mar 2026 | IDC | Adición de rama consulta conversacional al workflow (v1 solo tenía OCR) | Alcance ampliado, mismo horizonte | Aprobado | IDC |
| CC-002 | Mar 2026 | IDC | Actualización de modelo a `gemini-2.5-flash` (vs versión anterior) | Mejora de capacidad OCR y consulta | Aprobado | IDC |
| CC-003 | Mar 2026 | IDC | Ampliación de estructura Sheets de columnas básicas a 39 columnas (A–AM) con trazabilidad completa | Mejora de calidad de datos | Aprobado | IDC |

---

## Instrucciones para registrar nuevos cambios

1. Asignar ID correlativo (`CC-00N`)
2. Registrar fecha, solicitante y descripción clara del cambio
3. Evaluar impacto: alcance / cronograma / costo / calidad
4. Obtener aprobación antes de implementar
5. Actualizar CHANGELOG.md y documentación afectada

---

## Cambios críticos que requieren control obligatorio

- Cualquier modificación al formato físico del documento (nuevos campos, cambio de estructura)
- Cambio del spreadsheet de destino o de la pestaña `Registros_OCR`
- Cambio del modelo de Gemini (puede afectar calidad de extracción)
- Adición de nuevos formatos de documento al alcance
- Cambio del bot de Telegram (token, nombre, permisos)
