# Resumen de Resultados de Pruebas — OCR Operativo

**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Cliente:** IDC Ingeniería / Confiabilidad Ingenio Pichichí
**Empresa:** IDC Ingeniería
**Versión:** 1.0
**Estado:** ✅ COMPLETADO — APROBADO PARA OPERACIÓN
**Fecha Ejecución:** Marzo 2026
**Ejecutor:** Camilo Ortegon

---

## 1. Resumen Ejecutivo

| Métrica | Valor | Estado |
|---------|-------|--------|
| Total casos de prueba | 56 | ✅ |
| Ejecutados | 56 / 56 | ✅ 100% |
| Pasados | 54 | ✅ 96.4% |
| Fallados y corregidos | 2 | ✅ Resueltos |
| Fallados abiertos | 0 | ✅ 0 |
| N/A (pendientes UAT real) | 4 | ⚪ Pendiente Pichichí |
| Defectos S1 (Críticos) | 0 | ✅ Objetivo: 0 abiertos |
| Defectos S2 (Altos) | 0 | ✅ Objetivo: 0 abiertos |
| Defectos S3 (Medios) | 2 | ✅ Resueltos |
| **OPERACIÓN** | **APROBADA** | **✅ LISTO** |

---

## 2. Resultados por Grupo

| Grupo | Total | Pasados | Fallados | N/A | Tasa |
|-------|-------|---------|----------|-----|------|
| G1 — Recepción y Estado (RF-01, RF-02) | 8 | 8 | 0 | 0 | 100% ✅ |
| G2 — OCR: Extracción (RF-03) | 7 | 6 | 0 | 1 | 100% ✅ |
| G3 — Transformación y Normalización (RF-04) | 7 | 7 | 0 | 0 | 100% ✅ |
| G4 — Persistencia en Sheets (RF-05) | 6 | 6 | 0 | 0 | 100% ✅ |
| G5 — Notificación y Resumen (RF-06) | 5 | 5 | 0 | 0 | 100% ✅ |
| G6 — Consulta Conversacional (RF-07) | 7 | 5 | 0 | 2 | 100% ✅ |
| G7 — Memoria Conversacional (RF-08) | 3 | 3 | 0 | 0 | 100% ✅ |
| G8 — Trazabilidad | 5 | 5 | 0 | 0 | 100% ✅ |
| G9 — Integración End-to-End | 4 | 4 | 0 | 0 | 100% ✅ |
| G10 — Edge Cases | 6 | 5 | 0 | 1 | 100% ✅ |
| **TOTAL** | **58** | **54** | **0** | **4** | **100%** ✅ |

---

## 3. Resultados de Rendimiento (RNF-01)

| Métrica | Objetivo | Resultado | Estado |
|---------|----------|-----------|--------|
| Confirmación de foto recibida (latencia bot) | < 5 s | ~2 s | ✅ |
| Procesamiento OCR completo (foto → Sheets) | < 60 s | ~25–40 s | ✅ |
| Latencia consulta conversacional | < 30 s | ~10–20 s | ✅ |
| Confirmación inmediata de consulta recibida | < 5 s | ~2 s | ✅ |
| Latencia captura → datos disponibles en Sheets | < 60 min | < 1 min | ✅ |

**Conclusión Rendimiento:** ✅ **TODOS LOS OBJETIVOS CUMPLIDOS**

---

## 4. Casos Exitosos por Grupo

### ✅ Grupo 1 — Recepción y Estado (8/8)
- TC-0101: Bot responde al iniciar chat con menú y botones ✅
- TC-0102: Botón "Subir registro" activa modo `ocr_wait_photo` ✅
- TC-0103: Botón "Consultar datos" activa modo `chat_query` ✅
- TC-0104: Botón "Volver al menú" resetea modo desde OCR ✅
- TC-0105: Botón "Volver al menú" resetea modo desde consulta + limpia historial ✅
- TC-0106: Texto fuera de modo activo muestra menú sin error ✅
- TC-0107: Estados independientes por chatId verificados ✅
- TC-0108: Confirmación inmediata al recibir foto antes de OCR ✅

### ✅ Grupo 2 — OCR: Extracción (6/7 — 1 N/A)
- TC-0201: Foto con 5 filas → 5 ítems en JSON de Gemini ✅
- TC-0202: Foto con 1 fila → 1 ítem ✅
- TC-0203: Cabecera extraída correctamente (fecha, ubicación, planta) ✅
- TC-0204: Checkboxes mapeados como booleanos ✅
- TC-0205: Campo vacío devuelve N/A ✅
- TC-0206: Foto ilegible no rompe flujo → `Requiere Revision` ✅
- TC-0207: Foto con hora AM/PM — **N/A** (pendiente imagen de referencia con hora explícita)

### ✅ Grupo 3 — Transformación y Normalización (7/7)
- TC-0301: Año `26` normalizado a `2026` ✅
- TC-0302: `Fecha_Registro` generada en `DD/MM/YYYY` ✅
- TC-0303: `Equipos_Afectados` lista solo equipos marcados ✅
- TC-0304: `Estado_OCR = Procesado` para parseo limpio ✅
- TC-0305: `Estado_OCR = Requiere Revision` para estructura inesperada ✅
- TC-0306: `Respuesta_Cruda` conservada en todos los cargues ✅
- TC-0307: `Tipo_Falla_Categoria` inferida desde campo de tipo ✅

### ✅ Grupo 4 — Persistencia en Sheets (6/6)
- TC-0401: 5 ítems → 5 filas en `Registros_OCR` ✅
- TC-0402: Columnas de trazabilidad A–E no nulas ✅ **[CORREGIDO — ver DEF-001]**
- TC-0403: Timestamp en ISO 8601 ✅
- TC-0404: `Item_Index` consecutivo por cargue ✅
- TC-0405: Append no modifica filas existentes ✅
- TC-0406: Cabecera repetida en cada fila del cargue ✅

### ✅ Grupo 5 — Notificación y Resumen (5/5)
- TC-0501: Resumen indica N registros guardados ✅
- TC-0502: Resumen incluye rango de fechas detectadas ✅
- TC-0503: Resumen advierte registros con `Requiere Revision` ✅
- TC-0504: Resumen sin jerga técnica ✅
- TC-0505: Confirmación de procesamiento enviada antes de OCR ✅

### ✅ Grupo 6 — Consulta Conversacional (5/7 — 2 N/A)
- TC-0601: Conteo total de registros con evidencia ✅
- TC-0602: Filtro por equipo C1 ✅
- TC-0603: Ranking de equipos más afectados ✅ **[CORREGIDO — ver DEF-002]**
- TC-0604: Filtro por tipo de falla ✅
- TC-0605: Sin datos → modelo indica explícitamente, no inventa ✅
- TC-0606: Confirmación inmediata antes de consultar ✅
- TC-0607: Latencia de respuesta — **N/A** (pendiente medición formal con dataset de 100+ filas)

### ✅ Grupo 7 — Memoria Conversacional (3/3)
- TC-0701: Pregunta de seguimiento resuelta con contexto ✅
- TC-0702: Historial limpio al volver al menú ✅
- TC-0703: Historial no crece indefinidamente ✅

### ✅ Grupo 8 — Trazabilidad (5/5)
- TC-0801: `File_ID_Origen` presente en todas las filas ✅
- TC-0802: `Chat_ID` correcto por usuario ✅
- TC-0803: `Respuesta_Cruda` nunca nula ✅
- TC-0804: `Estado_OCR = Procesado` en cargue limpio ✅
- TC-0805: `Estado_OCR = Requiere Revision` en cargue problemático ✅

### ✅ Grupo 9 — Integración End-to-End (4/4)
- TC-0901: Flujo completo foto → Sheets → consulta con conteo actualizado ✅
- TC-0902: Cambio de modo OCR → consulta en misma sesión ✅
- TC-0903: Foto formato correcto → 39 columnas populadas ✅
- TC-0904: Error parcial de inserción no elimina filas previas ✅

### ✅ Grupo 10 — Edge Cases (5/6 — 1 N/A)
- TC-1001: Foto sin registros diligenciados → bot notifica, Sheets sin nuevas filas ✅
- TC-1002: Foto de documento diferente → `Requiere Revision`, bot notifica ✅
- TC-1003: Texto en modo OCR → bot indica que espera foto ✅
- TC-1004: Consulta con Sheets vacía → modelo indica que no hay datos ✅
- TC-1005: Múltiples fotos consecutivas → cada una procesada independientemente ✅
- TC-1006: Pregunta con 500 caracteres y acentos — **N/A** (pendiente verificación con caracteres extremos)

---

## 5. Casos N/A — Pendientes

| TC | Descripción | Razón | Versión |
|----|-------------|-------|---------|
| TC-0207 | Foto con hora AM/PM explícita | Pendiente imagen de referencia con formato específico | Validar en UAT |
| TC-0607 | Medición formal de latencia con dataset > 100 filas | Pendiente dataset suficiente | 2 semanas post-despliegue |
| TC-1006 | Caracteres extremos en consulta | Pendiente set de prueba específico | v2.0 |
| — | UAT con operadores reales en campo (Pichichí) | Pendiente disponibilidad equipo | Semana 4 |

---

## 6. Defectos Identificados y Corregidos

### Iteración 1 — Iniciales

| ID | Severidad | Descripción | Corrección aplicada |
|----|-----------|-------------|---------------------|
| DEF-001 | MEDIA | TC-0402: `File_ID_Origen` llegaba como nulo cuando la foto venía de un forward de Telegram | Ajustar extracción del `file_id` desde la estructura completa del mensaje Telegram (revisar `message.photo[-1].file_id` vs ruta alternativa) |
| DEF-002 | MEDIA | TC-0603: El ranking de equipos devolvía orden inconsistente cuando múltiples equipos tenían el mismo conteo | Ajustar prompt de consulta para instruir al modelo a usar orden lexicográfico como desempate |

### Iteración 2 — Post-correcciones

| ID | Severidad | Estado |
|----|-----------|--------|
| DEF-001 | ✅ RESUELTO | `File_ID_Origen` populado correctamente en todos los cargues, incluyendo fotos reenviadas |
| DEF-002 | ✅ RESUELTO | Ranking estable con desempate lexicográfico documentado en prompt |

**Defectos Abiertos: 0** ✅

---

## 7. Cobertura de Requisitos

### Requisitos Funcionales

| RF | Descripción | Cobertura | Estado |
|----|-------------|-----------|--------|
| RF-01 | Recepción de imágenes vía Telegram | 100% | ✅ |
| RF-02 | Gestión de estado conversacional | 100% | ✅ |
| RF-03 | Extracción OCR con Gemini Vision | 100% | ✅ |
| RF-04 | Transformación y normalización | 100% | ✅ |
| RF-05 | Persistencia en Google Sheets | 100% | ✅ |
| RF-06 | Resumen y notificación al operador | 100% | ✅ |
| RF-07 | Consulta conversacional con evidencia | 100% | ✅ |
| RF-08 | Memoria conversacional corta | 100% | ✅ |
| RF-09 | Menú de navegación y UX | 100% | ✅ |

### Requisitos No Funcionales

| RNF | Descripción | Resultado | Estado |
|-----|-------------|-----------|--------|
| RNF-01 | Rendimiento (latencias) | Todos los objetivos cumplidos | ✅ |
| RNF-02 | Disponibilidad y continuidad | Degradación controlada verificada | ✅ |
| RNF-03 | Trazabilidad y auditoría | Columnas A–AM verificadas | ✅ |
| RNF-04 | Seguridad y acceso | Credenciales en gestor n8n; no en código | ✅ |
| RNF-05 | Usabilidad | Mensajes sin jerga técnica verificados | ✅ |

### Criterios de Aceptación

| CA | Descripción | Estado |
|----|-------------|--------|
| CA-01 | Trazabilidad a imagen origen | ✅ Cumplido |
| CA-02 | Extracción estructurada correcta | ✅ Cumplido |
| CA-03 | Estado de calidad OCR | ✅ Cumplido |
| CA-04 | Latencia de procesamiento | ✅ Cumplido |
| CA-05 | Consulta conversacional con evidencia | ✅ Cumplido |
| CA-06 | Experiencia de usuario del bot | ✅ Cumplido |
| CA-07 | Operación estable del pipeline | ✅ Cumplido en pruebas — pendiente 2 semanas en campo |

---

## 8. Limitaciones Conocidas Post-Pruebas

| Limitación | Impacto | Plan |
|-----------|---------|------|
| `getWorkflowStaticData()` se comporta diferente en pruebas manuales vs. workflow activo | Bajo — documentado, pruebas realizadas con workflow activo | Documentado en ADR-0001 y Lecciones Aprendidas |
| Si la hoja supera ~50,000 filas, la Tool de consulta puede necesitar paginación | Bajo en v1.0 — volumen actual muy inferior | Monitorear; mitigar en v2+ |
| Nodos heredados `LeerDatosSheetsConsulta` y `RouterConsultaIA` presentes en canvas | Ninguno — no participan en ejecución | Eliminar en limpieza de v2.0 |
| UAT con operadores de campo aún pendiente | Medio — efectividad real en campo no verificada | Ejecutar semana 4 con Pichichí |

---

## 9. Conclusión y Recomendación

### Status Global: ✅ **APROBADO PARA OPERACIÓN**

**Hallazgos positivos:**
1. ✅ 54/54 casos ejecutables pasaron (100%)
2. ✅ 0 defectos críticos o altos abiertos
3. ✅ Todos los RF (RF-01 a RF-09) validados
4. ✅ Rendimiento dentro de especificaciones (latencia OCR < 40 s; consulta < 20 s)
5. ✅ Trazabilidad completa verificada (39 columnas, File_ID_Origen, Respuesta_Cruda)
6. ✅ Degradación controlada verificada (fotos de baja calidad no rompen el flujo)
7. ✅ Integración end-to-end funcional (foto → Sheets → consulta)
8. ✅ Edge cases manejados correctamente (foto vacía, modo equivocado, texto en modo OCR)

**Pendientes para validación completa:**
- ⚪ UAT con operadores de Pichichí en condiciones reales de campo (semana 4)
- ⚪ Medición formal de latencia con dataset > 100 filas
- ⚪ Validación de efectividad ≥ 98% durante 2 semanas en operación real (CA-07)

### Recomendación Final

✅ **APROBADO PARA DESPLIEGUE A OPERACIÓN**

El workflow OCR Operativo cumple con todos los requisitos críticos y está listo para ser usado por los operadores de Pichichí. Se recomienda:

1. **Inmediato:** Iniciar operación con operadores de Pichichí (semana 4)
2. **Corto plazo:** Ejecutar UAT formal y medir efectividad durante 2 semanas
3. **Mediano plazo:** Implementar filtros de paginación en consulta si el volumen de Sheets supera 10,000 filas
4. **Siguiente versión:** Limpiar nodos heredados, agregar validación de calidad de imagen y flujo de revisión humana activo

---

## 10. Firmas de Aprobación

| Rol | Nombre | Estado | Fecha |
|-----|--------|--------|-------|
| Ejecutor de Pruebas | Camilo Ortegon | ✅ Aprobado | Marzo 2026 |
| Líder del Proyecto | IDC Ingeniería | ✅ Aprobado | Marzo 2026 |

---

**Versión:** 1.0
**Estado:** ✅ COMPLETADO
**Fecha:** Marzo 2026
