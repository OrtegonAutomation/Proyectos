# Casos de Uso — OCR Operativo

**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Versión:** 1.0
**Fecha:** Marzo 2026

---

## CU-01: Subir foto de registro para digitalizar

**Actor principal:** Operador de campo
**Precondición:** El bot está activo. El usuario tiene una foto del formato de fallas centrifugas.

**Flujo normal:**
1. El usuario abre el bot y presiona "Subir registro"
2. El bot confirma que está esperando la foto y describe calidad esperada
3. El usuario envía la foto
4. El bot confirma que recibió la foto y está procesando
5. El workflow ejecuta OCR con Gemini Vision
6. Los datos extraídos se guardan en `Registros_OCR` (N filas)
7. El bot envía resumen: N registros guardados, rango de fechas, alertas de revisión si aplica

**Flujo alternativo:**
- Si Gemini devuelve estructura inesperada: filas marcadas `Requiere Revision`, operador notificado
- Si la foto no tiene registros reconocibles: el bot notifica con mensaje de error descriptivo

---

## CU-02: Consultar datos históricos en lenguaje natural

**Actor principal:** Ingeniero de confiabilidad / operador
**Precondición:** Existen datos en `Registros_OCR`. El usuario está en modo consulta.

**Flujo normal:**
1. El usuario presiona "Consultar datos"
2. El bot activa modo consulta y muestra ejemplos de preguntas
3. El usuario escribe su pregunta
4. El bot confirma que está buscando con evidencia real
5. Gemini consulta la hoja via Tool y genera respuesta
6. El bot responde con evidencia de los datos reales

**Flujo alternativo:**
- Si no hay datos que respondan la pregunta: Gemini lo indica explícitamente
- Preguntas de seguimiento usan historial corto de la sesión

---

## CU-03: Volver al menú / cambiar de modo

**Actor principal:** Cualquier usuario del bot
**Precondición:** El usuario está en modo OCR o consulta.

**Flujo normal:**
1. El usuario presiona "Volver al menú"
2. El historial de conversación se limpia
3. El modo se resetea a `menu`
4. El bot muestra el menú principal

---

## CU-04: Revisar registros con baja confianza

**Actor principal:** Supervisor / ingeniero de calidad
**Precondición:** Hay filas con `Estado_OCR = Requiere Revision` en `Registros_OCR`.

**Flujo normal:**
1. El supervisor accede directamente a Google Sheets
2. Filtra columna `Estado_OCR = Requiere Revision`
3. Revisa `Respuesta_Cruda` para entender qué extrajo Gemini
4. Corrige los valores directamente en la hoja
5. Actualiza `Estado_OCR` a `Revisado` manualmente

**Nota:** Este flujo ocurre fuera del bot, directamente en Sheets.
