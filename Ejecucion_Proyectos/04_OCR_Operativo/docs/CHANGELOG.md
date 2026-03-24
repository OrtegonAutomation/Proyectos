# Changelog — OCR Operativo

---

## v2.0 — Marzo 2026

### Agregado
- Workflow n8n dual: rama OCR + rama consulta conversacional en un solo bot de Telegram
- Gestión de estado conversacional por `chatId` via `getWorkflowStaticData`
- Nodo `ResolverConversacion` como controlador central de modos
- Rama OCR: `Analyze an image` (Gemini Vision) → `FormatearDatos` → `GuardarEnSheets` → `NotificarExito`
- Rama Consulta: `ProcesarConsulta` → `Message a model` (Gemini + Sheets Tool) → `ResponderConsulta`
- 39 columnas en `Registros_OCR` con trazabilidad completa (A–AM)
- Columna `Estado_OCR` con valores `Procesado` / `Requiere Revision`
- Columna `Respuesta_Cruda` para auditoría y depuración
- Memoria conversacional corta para preguntas de seguimiento
- Documentación técnica completa: ARQUITECTURA.md, ESPECIFICACION_TECNICA.md, 4 ADRs

### Cambios respecto a v1
- Se agrega rama de consulta conversacional (v1 solo tenía OCR)
- Se unifica en un bot con gestión de estado (v1 era solo subida de fotos)
- Modelo actualizado a `gemini-2.5-flash`

---

## v1.0 — Febrero 2026

- Primera versión del workflow OCR
- Solo rama de cargue de fotos
- Estructura básica de Sheets
