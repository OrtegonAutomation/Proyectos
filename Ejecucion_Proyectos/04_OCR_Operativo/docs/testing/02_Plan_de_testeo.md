# Plan de Pruebas — OCR Operativo

**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Cliente:** IDC Ingeniería / Confiabilidad Ingenio Pichichí
**Empresa:** IDC Ingeniería
**Versión:** 1.0
**Fecha:** Marzo 2026

---

## 1. Introducción

### 1.1 Propósito

Definir la estrategia, alcance y recursos para validar que el OCR Operativo cumple los criterios de aceptación (CA-01 a CA-07) del proyecto.

### 1.2 Alcance

- Pruebas de extracción OCR con imágenes reales y controladas
- Pruebas de persistencia y trazabilidad en Google Sheets
- Pruebas del flujo conversacional (consultas en lenguaje natural)
- Pruebas de comportamiento ante imágenes de baja calidad o formatos inesperados
- Pruebas de experiencia de usuario (flujo del bot, mensajes, botones)

### 1.3 Fuera de Alcance

- Pruebas de carga multi-usuario concurrente
- Pruebas de otros formatos de documento (fuera del alcance v1.0)
- Pruebas de penetración o seguridad de infraestructura

---

## 2. Estrategia de Pruebas

### 2.1 Niveles de Prueba

```
┌─────────────────────────────────────────────────────┐
│   NIVEL 3: PRUEBAS DE ACEPTACIÓN (UAT)              │ ← Pichichí ejecuta
│   Imágenes reales, operadores reales, condiciones   │
│   de campo                                          │
├─────────────────────────────────────────────────────┤
│   NIVEL 2: PRUEBAS FUNCIONALES                      │ ← IDC ejecuta
│   Casos de uso completos, datos controlados         │
├─────────────────────────────────────────────────────┤
│   NIVEL 1: PRUEBAS DE COMPONENTE                    │ ← IDC ejecuta
│   Nodos individuales, transformaciones de datos     │
└─────────────────────────────────────────────────────┘
```

### 2.2 Tipos de Prueba

| Tipo | Objetivo | Responsable |
|------|----------|-------------|
| **OCR funcional** | Validar extracción correcta de campos | IDC |
| **Trazabilidad** | Validar columnas de auditoría en Sheets | IDC |
| **Robustez** | Validar comportamiento ante imágenes degradadas | IDC |
| **Consulta conversacional** | Validar respuestas con evidencia real | IDC |
| **Experiencia de usuario** | Validar flujo del bot, mensajes, botones | IDC + Pichichí |
| **UAT** | Validar con operadores reales en condiciones de campo | Pichichí |

---

## 3. Casos de Prueba

### 3.1 OCR — Extracción

| ID | Caso | Entrada | Resultado esperado | CA |
|----|------|---------|-------------------|-----|
| T-OCR-01 | Foto nítida con 5 registros | Imagen estándar | 5 filas en Sheets, todos los campos mapeados | CA-02 |
| T-OCR-02 | Foto nítida con 1 registro | Imagen con 1 fila | 1 fila en Sheets | CA-02-01 |
| T-OCR-03 | Página con múltiples equipos marcados | Imagen con C1, C3, BBA MIEL 1 marcados | Columnas S, U, Y = `true`; resto = `false` | CA-02-04 |
| T-OCR-04 | Imagen de baja calidad (borrosa) | Foto movida | `Estado_OCR = Requiere Revision`, no error del workflow | CA-03-03 |
| T-OCR-05 | Imagen con campos vacíos | Fila parcialmente diligenciada | Campos vacíos = `N/A`, no falla | CA-02-03 |
| T-OCR-06 | Campos de cabecera presentes | Foto con encabezado legible | Fecha_Emision, Ubicacion, Planta mapeados en todas las filas | CA-02-02 |

### 3.2 Trazabilidad

| ID | Caso | Verificación | CA |
|----|------|--------------|----|
| T-TRACE-01 | Cada fila tiene File_ID_Origen | Columna E no nula | CA-01-01 |
| T-TRACE-02 | Timestamp correcto | Columna A en ISO 8601, fecha del procesamiento | CA-01-03 |
| T-TRACE-03 | Respuesta_Cruda conservada | Columna AM contiene JSON crudo de Gemini | CA-01-04 |

### 3.3 Consulta Conversacional

| ID | Caso | Pregunta | Resultado esperado | CA |
|----|------|----------|-------------------|-----|
| T-QUERY-01 | Conteo simple | "¿Cuántas fallas hay registradas?" | Número correcto con evidencia de Sheets | CA-05-01 |
| T-QUERY-02 | Filtro por equipo | "¿Cuántas fallas afectaron a C1?" | Conteo filtrado por columna Equipo_C1 | CA-05-02 |
| T-QUERY-03 | Pregunta de seguimiento | "¿Y en C2?" (tras pregunta anterior) | Respuesta con historial de contexto | CA-05-03 |
| T-QUERY-04 | Pregunta sin datos | "¿Cuántas fallas en diciembre?" (sin datos de dic) | El modelo indica que no hay datos, no inventa | CA-05-01 |
| T-QUERY-05 | Latencia de consulta | Cualquier pregunta | Respuesta < 30 segundos | CA-05-04 |

### 3.4 Experiencia de Usuario

| ID | Caso | Acción | Resultado esperado | CA |
|----|------|--------|-------------------|-----|
| T-UX-01 | Menú inicial | Iniciar chat con bot | Menú con botones visible | CA-06-01 |
| T-UX-02 | Confirmación de foto | Enviar foto en modo OCR | Bot responde antes de procesar | CA-06-02 |
| T-UX-03 | Volver al menú | Presionar botón en cualquier modo | Menú disponible, historial limpio | CA-06-04 |
| T-UX-04 | Resumen de cargue | Tras cargue exitoso | Mensaje en lenguaje operativo con N registros | CA-06-05 |

---

## 4. Dataset de Pruebas

| ID | Imagen | Descripción | Uso |
|----|--------|-------------|-----|
| IMG-01 | `doc20943920260126103509.pdf` (convertido) | Formato real con registros de referencia | T-OCR-01, T-OCR-06 |
| IMG-02 | Foto de campo tomada con móvil | Condiciones reales, iluminación variable | UAT |
| IMG-03 | Captura degradada intencionalmente | Baja calidad para prueba de robustez | T-OCR-04 |
| IMG-04 | Foto con 1 sola fila diligenciada | Caso mínimo | T-OCR-02 |
| IMG-05 | Foto con múltiples equipos marcados | Prueba de checkboxes | T-OCR-03 |

---

## 5. Criterios de Éxito / Fallo

| Criterio | Umbral de aceptación |
|---------|---------------------|
| Exactitud de extracción (campos correctos / campos totales auditados) | ≥ 95% en dataset de pruebas |
| % registros con Estado_OCR = Procesado | ≥ 98% en operación estable (2 semanas) |
| Trazabilidad completa (File_ID_Origen no nulo) | 100% |
| Latencia captura → Sheets | < 60 minutos |
| Latencia respuesta consulta | < 30 segundos |
| Flujo del bot sin errores en casos de uso principales | 100% |

---

## 6. Cronograma de pruebas

| Semana | Actividades |
|--------|-------------|
| Semana 3 | Pruebas de componente (T-OCR, T-TRACE) con dataset controlado |
| Semana 3 | Pruebas funcionales completas (T-QUERY, T-UX) |
| Semana 4 | UAT con operadores de Pichichí en condiciones de campo |
| Semana 4 | Reporte de resultados y plan de mejora si aplica |
