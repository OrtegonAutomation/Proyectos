# Cronograma de Trabajo — OCR Operativo

**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Versión:** 1.0
**Fecha:** Marzo 2026
**Horizonte:** 1 mes

---

## 1. Cronograma de alto nivel

| Semana | Hito | Entregables |
|--------|------|-------------|
| **Semana 1** | Definición del formato y muestra inicial | Diccionario de campos, reglas de validación, dataset inicial de imágenes |
| **Semana 2** | Pipeline OCR funcional | Workflow n8n con OCR básico, Sheets configurada, pruebas con dataset controlado |
| **Semana 3** | Validación, consulta y persistencia completa | Flujo completo OCR + consulta conversacional, trazabilidad, plan de pruebas ejecutado |
| **Semana 4** | Despliegue, UAT y cierre | Pruebas con operadores reales, manual de captura, runbook, reporte de cierre |

---

## 2. Detalle por semana

### Semana 1 — Definición y muestra

| Actividad | Responsable | Estado |
|-----------|------------|--------|
| Levantamiento del formato objetivo (campos, tipos, reglas) | IDC | Completado |
| Definición de estructura de Sheets (39 columnas A–AM) | IDC | Completado |
| Recolección de muestra inicial de imágenes reales | Pichichí / IDC | Completado |
| Configuración de credenciales (Telegram, Gemini, Sheets) | IDC | Completado |

### Semana 2 — Pipeline OCR

| Actividad | Responsable | Estado |
|-----------|------------|--------|
| Desarrollo del workflow n8n v1 (solo OCR) | IDC | Completado |
| Diseño y prueba del prompt Gemini Vision | IDC | Completado |
| Configuración de `FormatearDatos` y `GuardarEnSheets` | IDC | Completado |
| Pruebas con dataset controlado (T-OCR-01 a T-OCR-05) | IDC | Completado |

### Semana 3 — Consulta y trazabilidad

| Actividad | Responsable | Estado |
|-----------|------------|--------|
| Desarrollo de rama consulta conversacional | IDC | Completado |
| Gestión de estado por chatId (`ResolverConversacion`) | IDC | Completado |
| Pruebas funcionales completas (T-QUERY, T-UX, T-TRACE) | IDC | En progreso |
| Documentación técnica (ARQUITECTURA, ESPECIFICACION, ADRs) | IDC | Completado |

### Semana 4 — Despliegue y cierre

| Actividad | Responsable | Estado |
|-----------|------------|--------|
| UAT con operadores de Pichichí | Pichichí + IDC | Pendiente |
| Ajustes post-UAT | IDC | Pendiente |
| Manual de captura fotográfica finalizado | IDC | Completado |
| Runbook operativo finalizado | IDC | Completado |
| Reporte de cierre con desempeño y plan de escalamiento | IDC | Pendiente |

---

## 3. Indicadores de avance

| Indicador | Objetivo | Estado |
|-----------|---------|--------|
| % hitos semana 1 completados | 100% | Completado |
| % hitos semana 2 completados | 100% | Completado |
| % hitos semana 3 completados | 100% | En progreso |
| % hitos semana 4 completados | 100% | Pendiente |
| Casos de prueba ejecutados / total | 18/18 | 0/18 — pendiente ejecución formal |
