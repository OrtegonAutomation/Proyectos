# Casos de Prueba — OCR Operativo

**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Versión:** 1.0
**Fecha:** Marzo 2026

---

## Grupo 1 — Recepción y Estado Conversacional (RF-01, RF-02)

| ID | Nombre | Precondición | Pasos | Resultado Esperado | RF | CA |
|----|--------|-------------|-------|-------------------|----|----|
| TC-0101 | Bot responde al iniciar chat | Workflow activo | Enviar `/start` al bot | Menú principal con botones "Subir registro", "Consultar datos" | RF-09 | CA-06-01 |
| TC-0102 | Botón "Subir registro" activa modo OCR | Menú visible | Presionar "Subir registro" | Bot responde con instrucción de foto; modo pasa a `ocr_wait_photo` | RF-02, RF-09 | CA-06-01 |
| TC-0103 | Botón "Consultar datos" activa modo consulta | Menú visible | Presionar "Consultar datos" | Bot muestra ejemplos de preguntas; modo pasa a `chat_query` | RF-02, RF-09 | CA-06-01 |
| TC-0104 | Botón "Volver al menú" resetea modo | Modo `ocr_wait_photo` | Presionar "Volver al menú" | Modo vuelve a `menu`; menú principal visible | RF-02, RF-09 | CA-06-04 |
| TC-0105 | Botón "Volver al menú" desde modo consulta | Modo `chat_query` | Presionar "Volver al menú" | Modo vuelve a `menu`; historial limpiado | RF-02, RF-08 | CA-06-04 |
| TC-0106 | Texto recibido fuera de modo activo | Modo `menu` | Enviar texto libre | Bot muestra menú principal sin error | RF-02 | CA-06-01 |
| TC-0107 | Estados independientes por chatId | Dos usuarios activos | Usuario A en OCR, Usuario B consulta | Cada usuario recibe respuesta de su modo sin interferencia | RF-02 | CA-02-01 |
| TC-0108 | Confirmación inmediata al recibir foto | Modo `ocr_wait_photo` | Enviar foto | Bot responde "Procesando..." antes de completar OCR | RF-01, RF-09 | CA-06-02 |

---

## Grupo 2 — OCR: Extracción de Datos (RF-03)

| ID | Nombre | Precondición | Pasos | Resultado Esperado | RF | CA |
|----|--------|-------------|-------|-------------------|----|----|
| TC-0201 | Foto con 5 filas diligenciadas produce 5 ítems | Modo OCR activo | Enviar `IMG-01` (5 filas) | Gemini devuelve JSON con 5 ítems en `items[]` | RF-03 | CA-02-01 |
| TC-0202 | Foto con 1 sola fila produce 1 ítem | Modo OCR activo | Enviar `IMG-04` (1 fila) | Gemini devuelve JSON con 1 ítem | RF-03 | CA-02-01 |
| TC-0203 | Cabecera extraída correctamente | Modo OCR activo | Enviar foto con encabezado legible | `documento.fecha_emision`, `ubicacion`, `planta` populados | RF-03 | CA-02-02 |
| TC-0204 | Checkboxes de equipos mapeados como booleanos | Modo OCR activo | Enviar `IMG-05` (C1, C3, BBA MIEL 1 marcados) | `equipos.C1=true`, `equipos.C3=true`, `equipos.BBA_MIEL_1=true`; resto `false` | RF-03 | CA-02-04 |
| TC-0205 | Campo vacío devuelve N/A | Modo OCR activo | Enviar foto con observaciones en blanco | `observaciones = "N/A"` | RF-03 | CA-02-03 |
| TC-0206 | Foto ilegible no rompe el flujo | Modo OCR activo | Enviar `IMG-03` (foto borrosa) | Workflow continúa; filas marcadas `Requiere Revision` | RF-03 | CA-03-03 |
| TC-0207 | Hora con período AM/PM extraída | Modo OCR activo | Enviar foto con hora "2:30 PM" | `hora_inicio="2:30"`, `periodo_inicio="PM"` | RF-03 | CA-02-06 |

---

## Grupo 3 — Transformación y Normalización (RF-04)

| ID | Nombre | Precondición | Pasos | Resultado Esperado | RF | CA |
|----|--------|-------------|-------|-------------------|----|----|
| TC-0301 | Año de 2 dígitos normalizado | JSON Gemini con `fecha_anio="26"` | Ejecutar `FormatearDatos` | `Fecha_Anio = "2026"` | RF-04 | CA-02-05 |
| TC-0302 | Fecha_Registro generada en DD/MM/YYYY | JSON con `dia=15`, `mes=3`, `anio=2026` | Ejecutar `FormatearDatos` | `Fecha_Registro = "15/03/2026"` | RF-04 | CA-02-05 |
| TC-0303 | Equipos_Afectados lista equipos marcados | JSON con C1=true, C3=true, resto false | Ejecutar `FormatearDatos` | `Equipos_Afectados = "C1, C3"` | RF-04 | CA-02-06 |
| TC-0304 | Estado_OCR = Procesado para parseo limpio | JSON completo y válido de Gemini | Ejecutar `FormatearDatos` | `Estado_OCR = "Procesado"` | RF-04 | CA-03-01 |
| TC-0305 | Estado_OCR = Requiere Revision para estructura inesperada | JSON con estructura inválida | Ejecutar `FormatearDatos` | `Estado_OCR = "Requiere Revision"` | RF-04 | CA-03-01 |
| TC-0306 | Respuesta_Cruda siempre conservada | Cualquier respuesta de Gemini | Ejecutar `FormatearDatos` | Columna `Respuesta_Cruda` no nula en todas las filas | RF-04 | CA-01-04 |
| TC-0307 | Tipo_Falla_Categoria inferida | JSON con tipo_falla_mecanica="3" | Ejecutar `FormatearDatos` | `Tipo_Falla_Categoria = "Mecanica"`, `Tipo_Falla_Codigo = "3"` | RF-04 | CA-02-06 |

---

## Grupo 4 — Persistencia en Google Sheets (RF-05)

| ID | Nombre | Precondición | Pasos | Resultado Esperado | RF | CA |
|----|--------|-------------|-------|-------------------|----|----|
| TC-0401 | N ítems generan N filas en Sheets | 5 ítems normalizados | Ejecutar `GuardarEnSheets` | 5 filas nuevas en `Registros_OCR` | RF-05 | CA-02-01 |
| TC-0402 | Columnas de trazabilidad no nulas | Cargue exitoso | Revisar cols A–E en Sheets | `Timestamp`, `Chat_ID`, `Update_ID`, `Message_ID`, `File_ID_Origen` populados | RF-05 | CA-01-01 |
| TC-0403 | Timestamp en ISO 8601 | Cargue exitoso | Revisar col A en Sheets | Valor con formato `YYYY-MM-DDTHH:mm:ssZ` | RF-05 | CA-01-03 |
| TC-0404 | Item_Index consecutivo por cargue | Foto con 3 ítems | Ejecutar cargue | `Item_Index` = 1, 2, 3 en las filas del cargue | RF-05 | CA-02-01 |
| TC-0405 | Append no modifica filas existentes | Sheets con datos previos | Ejecutar nuevo cargue | Filas anteriores intactas; nuevas filas al final | RF-05 | CA-02-01 |
| TC-0406 | Cabecera repetida en cada fila | Foto con 3 ítems, misma cabecera | Revisar Sheets | `Fecha_Emision`, `Ubicacion`, `Planta` iguales en las 3 filas del cargue | RF-05 | CA-02-02 |

---

## Grupo 5 — Notificación y Resumen (RF-06)

| ID | Nombre | Precondición | Pasos | Resultado Esperado | RF | CA |
|----|--------|-------------|-------|-------------------|----|----|
| TC-0501 | Resumen indica N registros guardados | Cargue de 5 ítems exitoso | Completar flujo OCR | Mensaje del bot: "Se guardaron 5 registros" (o equivalente) | RF-06 | CA-06-05 |
| TC-0502 | Resumen indica rango de fechas | Cargue con fechas variadas | Completar flujo OCR | Resumen incluye rango de fechas detectadas en el cargue | RF-06 | CA-06-05 |
| TC-0503 | Resumen advierte registros para revisión | Cargue con 2 ítems `Requiere Revision` | Completar flujo OCR | Resumen indica "2 registros requieren revisión manual" | RF-06 | CA-03-02 |
| TC-0504 | Resumen no usa jerga técnica | Cargue exitoso | Leer mensaje de confirmación | Mensaje comprensible sin términos "parseo", "JSON", "append" | RF-06 | CA-06-05 |
| TC-0505 | Confirmación de procesamiento antes de OCR | Foto recibida | Enviar foto en modo OCR | Bot responde antes de completar el OCR con mensaje de espera | RF-06 | CA-06-02 |

---

## Grupo 6 — Consulta Conversacional (RF-07)

| ID | Nombre | Precondición | Pasos | Resultado Esperado | RF | CA |
|----|--------|-------------|-------|-------------------|----|----|
| TC-0601 | Conteo total de registros | Sheets con datos | Preguntar: "¿Cuántos registros hay?" | Número correcto basado en Sheets | RF-07 | CA-05-01 |
| TC-0602 | Filtro por equipo | Sheets con datos variados | Preguntar: "¿Cuántas fallas tiene C1?" | Conteo correcto filtrado por columna `Equipo_C1 = true` | RF-07 | CA-05-02 |
| TC-0603 | Ranking de equipos más afectados | Sheets con datos | Preguntar: "¿Qué equipo ha fallado más?" | Ranking ordenado con evidencia de la hoja | RF-07 | CA-05-02 |
| TC-0604 | Filtro por tipo de falla | Sheets con datos | Preguntar: "¿Cuántas fallas mecánicas hay?" | Conteo filtrado por `Tipo_Falla_Categoria` | RF-07 | CA-05-02 |
| TC-0605 | Sin datos disponibles para la pregunta | Sheets vacía o sin datos del período | Preguntar por período sin datos | El modelo indica explícitamente que no hay datos, no inventa | RF-07 | CA-05-01 |
| TC-0606 | Confirmación inmediata antes de consultar | Modo `chat_query` activo | Enviar pregunta | Bot responde "Consultando..." antes de invocar Gemini | RF-07 | CA-06-03 |
| TC-0607 | Latencia de respuesta | Sheets con datos normales | Enviar pregunta | Respuesta completa en < 30 segundos | RF-07 | CA-05-04 |

---

## Grupo 7 — Memoria Conversacional (RF-08)

| ID | Nombre | Precondición | Pasos | Resultado Esperado | RF | CA |
|----|--------|-------------|-------|-------------------|----|----|
| TC-0701 | Pregunta de seguimiento resuelta con contexto | Sesión activa con pregunta previa | Preguntar "¿y en C2?" tras consulta de C1 | Respuesta correcta sobre C2 usando contexto de la pregunta anterior | RF-08 | CA-05-03 |
| TC-0702 | Historial se limpia al volver al menú | Sesión con historial activo | Presionar "Volver al menú" | Nueva sesión de consulta no tiene contexto previo | RF-08 | CA-05-03 |
| TC-0703 | Historial no crece indefinidamente | 20 turnos de conversación | Continuar haciendo preguntas | El historial se mantiene recortado; el workflow no falla por tamaño de estado | RF-08 | CA-05-03 |

---

## Grupo 8 — Trazabilidad (RF-05, CA-01)

| ID | Nombre | Precondición | Pasos | Resultado Esperado | RF | CA |
|----|--------|-------------|-------|-------------------|----|----|
| TC-0801 | File_ID_Origen presente en todas las filas | Cargue exitoso | Revisar col E en Sheets | Valor no nulo y distinto de "N/A" en cada fila | RF-05 | CA-01-01 |
| TC-0802 | Chat_ID correcto por usuario | Cargue desde chatId conocido | Revisar col B en Sheets | Chat_ID coincide con el del chat que envió la foto | RF-05 | CA-01-02 |
| TC-0803 | Respuesta_Cruda nunca nula | Cargues variados (exitosos y con revisión) | Revisar col AM en Sheets | Columna AM siempre con contenido | RF-04, RF-05 | CA-01-04 |
| TC-0804 | Estado_OCR = Procesado en cargue limpio | Foto clara de formato correcto | Completar cargue | Col AL = "Procesado" en todas las filas del cargue | RF-04 | CA-03-01 |
| TC-0805 | Estado_OCR = Requiere Revision en cargue problemático | Foto borrosa o formato alterado | Completar cargue | Col AL = "Requiere Revision" en filas afectadas | RF-04 | CA-03-01 |

---

## Grupo 9 — Integración End-to-End

| ID | Nombre | Pasos | Resultado Esperado | CA |
|----|--------|-------|-------------------|-----|
| TC-0901 | Flujo completo: foto → Sheets → consulta | Subir foto con 3 ítems; consultar conteo | 3 filas guardadas; consulta devuelve conteo actualizado con las nuevas filas | CA-02, CA-05 |
| TC-0902 | Cambio de modo OCR → consulta en misma sesión | Subir foto; volver al menú; consultar | Ambos flujos ejecutados correctamente sin interferencia | CA-06 |
| TC-0903 | Foto con formato correcto → todas las columnas populadas | Enviar `IMG-01` completo | Las 39 columnas tienen valores (incluyendo N/A para campos no aplica) | CA-02 |
| TC-0904 | Cargue con error parcial no pierde filas previas | Forzar error en inserción de la 3ª fila | Filas 1 y 2 permanecen en Sheets | CA-07-02 |

---

## Grupo 10 — Edge Cases

| ID | Nombre | Pasos | Resultado Esperado | CA |
|----|--------|-------|-------------------|-----|
| TC-1001 | Foto sin ningún registro diligenciado | Enviar foto de formato vacío | Bot notifica que no se detectaron registros; Sheets sin nuevas filas | CA-07-03 |
| TC-1002 | Foto con texto no relacionado al formato | Enviar foto de un documento diferente | `Estado_OCR = Requiere Revision`; bot notifica | CA-07-03 |
| TC-1003 | Texto en modo OCR (sin foto) | Enviar texto mientras espera foto | Bot indica que espera una foto; modo se mantiene | CA-06 |
| TC-1004 | Pregunta sin datos en Sheets | Consultar con Sheets vacía | El modelo indica que no hay datos; no devuelve error técnico | CA-05-01 |
| TC-1005 | Múltiples fotos enviadas consecutivamente | Enviar 3 fotos en modo OCR | Cada foto se procesa como cargue independiente | CA-02-01 |
| TC-1006 | Pregunta muy larga o con caracteres especiales | Enviar texto con 500 caracteres y acentos | El workflow procesa sin error | CA-07-04 |
