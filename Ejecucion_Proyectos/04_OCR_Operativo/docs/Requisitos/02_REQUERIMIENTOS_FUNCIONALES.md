# Requerimientos Funcionales — OCR Operativo

**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Versión:** 1.0
**Fecha:** Marzo 2026

---

## RF-01: Recepción de imágenes vía Telegram

**Descripción:** El sistema debe recibir fotografías de formatos operativos a través del bot de Telegram, descargar los binarios y ponerlos disponibles para el procesamiento OCR.

**Detalles:**
- El trigger escucha eventos tipo `message` y `callback_query`
- Las fotos se descargan como binarios directamente desde la API de Telegram
- El sistema acepta fotos de cualquier tamaño soportado por Telegram
- El sistema no almacena las fotos en disco; las procesa en memoria

**Criterios de aceptación:**
- [ ] El bot recibe fotos enviadas desde Telegram (móvil y escritorio)
- [ ] Las fotos llegan al nodo OCR en formato binario procesable por Gemini
- [ ] Si el usuario envía un archivo que no es foto, el bot responde con mensaje de error claro

---

## RF-02: Gestión de estado conversacional por chatId

**Descripción:** El sistema debe mantener el modo de cada conversación de forma independiente, distinguiendo entre modo menú, modo OCR y modo consulta.

**Detalles:**
- Estado persistido: `chatModes[chatId]` con valores `menu` / `ocr_wait_photo` / `chat_query`
- Implementado via `getWorkflowStaticData('global')` en n8n
- El cambio de modo se activa mediante botones del bot o al completar una acción
- Al cambiar de modo, el historial de conversación se limpia

**Criterios de aceptación:**
- [ ] Un usuario en modo `ocr_wait_photo` recibe procesamiento OCR al enviar foto
- [ ] Un usuario en modo `chat_query` recibe respuesta conversacional al escribir texto
- [ ] El botón "Volver al menú" resetea el modo a `menu` independientemente del estado actual
- [ ] Múltiples usuarios pueden usar el bot simultáneamente sin interferencia de estados

---

## RF-03: Extracción OCR estructurada con Gemini Vision

**Descripción:** El sistema debe extraer todos los campos del formato de fallas centrifugas desde una imagen y devolver un JSON estructurado con cabecera + ítems.

**Detalles:**
- Motor: Google Gemini Vision (`gemini-2.5-flash`)
- El prompt exige JSON estricto con esquema `{ documento: {...}, items: [...] }`
- Una imagen puede producir múltiples ítems (una fila del formato = un ítem)
- Si Gemini no puede leer un campo, devuelve `N/A`

**Campos extraídos de la cabecera:** `fecha_emision`, `ubicacion`, `planta`
**Campos extraídos por ítem:** `fecha_dia`, `fecha_mes`, `fecha_anio`, `hora_inicio`, `periodo_inicio`, `hora_fin`, `periodo_fin`, `tiempo_falla_min`, `equipos` (10 booleanos), `tipo_falla_mecanica`, `tipo_falla_electrica`, `tipo_falla_instrumentacion`, `tipo_falla_otros`, `observaciones`, `firma_hallazgo`

**Criterios de aceptación:**
- [ ] Una foto con N filas diligenciadas produce exactamente N ítems en el JSON
- [ ] Los checkboxes de equipos (C1–GUSANILLO) se mapean como booleanos correctamente
- [ ] Campos no legibles devuelven `N/A`, no error del workflow
- [ ] La extracción de fecha produce los tres subcampos (día, mes, año) separados

---

## RF-04: Transformación y normalización de datos OCR

**Descripción:** El sistema debe convertir la salida cruda de Gemini en filas operativas normalizadas, listas para insertar en Google Sheets.

**Detalles:**
- Implementado en el nodo `FormatearDatos`
- Normalización de año: `22` → `2022`, `26` → `2026` para años de 2 dígitos
- Corrección de códigos de falla: `22` → `2.2` cuando corresponde
- Unificación de equipos: combina detección por checkboxes y por lista textual
- Generación de `Fecha_Registro` normalizada: `DD/MM/YYYY`
- Generación de `Equipos_Afectados`: lista consolidada de equipos marcados
- Inferencia de `Tipo_Falla_Categoria` y `Tipo_Falla_Codigo` desde los campos de tipo

**Criterios de aceptación:**
- [ ] `Fecha_Registro` siempre tiene formato `DD/MM/YYYY`
- [ ] `Equipos_Afectados` lista solo los equipos con valor `true`
- [ ] `Estado_OCR = Procesado` cuando el parseo es limpio
- [ ] `Estado_OCR = Requiere Revision` cuando la estructura de Gemini es inesperada o hay campos críticos faltantes
- [ ] `Respuesta_Cruda` siempre se conserva, independientemente del estado OCR

---

## RF-05: Persistencia en Google Sheets

**Descripción:** El sistema debe insertar las filas procesadas en la hoja `Registros_OCR` del spreadsheet de destino, una fila por ítem del documento.

**Detalles:**
- Operación: `append` (nunca sobreescribe filas existentes)
- Spreadsheet ID: `1YsDdmcT9m6TJEiz6W17XiLIMAuRicgYPML2bQl5RNYY`
- Pestaña: `Registros_OCR`
- 39 columnas (A–AM) según especificación técnica
- Los campos de trazabilidad (`Timestamp`, `Chat_ID`, `File_ID_Origen`, etc.) se completan en cada fila

**Criterios de aceptación:**
- [ ] Cada ítem del documento genera exactamente una fila en Sheets
- [ ] Los campos de trazabilidad (cols A–E) nunca son nulos
- [ ] Las filas se insertan en orden de `Item_Index` (col I)
- [ ] Un error en la inserción de una fila no elimina las filas ya insertadas del mismo cargue
- [ ] La operación de append no modifica filas existentes

---

## RF-06: Resumen de cargue y notificación al operador

**Descripción:** Tras completar el procesamiento OCR, el sistema debe enviar al operador un resumen comprensible del resultado.

**Detalles:**
- Implementado en los nodos `ResumenCarga` y `NotificarExito`
- El resumen incluye: número de filas guardadas, rango de fechas detectadas, número de filas con `Requiere Revision`
- Lenguaje operativo, sin términos técnicos
- El mensaje siempre incluye el botón "Volver al menú"

**Criterios de aceptación:**
- [ ] El operador recibe un resumen después de cada cargue exitoso
- [ ] El resumen indica cuántas filas se guardaron
- [ ] El resumen advierte si hay filas para revisión manual
- [ ] El resumen usa lenguaje comprensible para operadores sin experiencia técnica

---

## RF-07: Consulta conversacional con evidencia de Sheets

**Descripción:** El sistema debe responder preguntas en lenguaje natural sobre los datos históricos, usando la hoja `Registros_OCR` como fuente de verdad (no la memoria del modelo).

**Detalles:**
- Motor: Google Gemini Chat (`gemini-2.5-flash`)
- El modelo tiene acceso a la Tool de Google Sheets para consultar datos reales
- El nodo `ProcesarConsulta` construye el prompt con esquema de columnas + historial corto
- El modelo instruido para usar la Tool cuando la pregunta implique datos reales

**Criterios de aceptación:**
- [ ] Las respuestas se basan en datos reales de la hoja, no en inferencias del modelo
- [ ] El sistema responde preguntas de conteo, ranking y filtro por equipo, fecha y tipo de falla
- [ ] Si no hay datos que respondan la pregunta, el modelo lo indica explícitamente
- [ ] Las preguntas de seguimiento dentro de la misma sesión mantienen contexto

---

## RF-08: Memoria conversacional corta

**Descripción:** El sistema debe mantener un historial corto de la conversación por sesión para soportar preguntas de seguimiento naturales.

**Detalles:**
- Almacenado en `chatHistories[chatId]` via `getWorkflowStaticData`
- Se actualiza tras cada turno de consulta (pregunta + respuesta)
- Se recorta automáticamente para no crecer indefinidamente
- Se limpia al cambiar de modo o volver al menú

**Criterios de aceptación:**
- [ ] Una pregunta de seguimiento ("¿y en C2?") se responde correctamente usando el contexto previo
- [ ] El historial se limpia al presionar "Volver al menú"
- [ ] El tamaño del historial no crece sin límite entre sesiones

---

## RF-09: Menú de navegación y UX del bot

**Descripción:** El sistema debe proveer una interfaz clara con botones para navegar entre modos, confirmando siempre al usuario en qué estado está y qué se espera de él.

**Detalles:**
- Botones disponibles: "Subir registro", "Consultar datos", "Volver al menú"
- Mensajes de confirmación inmediata antes de operaciones lentas (OCR, consulta)
- Mensajes en español, lenguaje operativo, sin jerga técnica

**Criterios de aceptación:**
- [ ] El menú principal es accesible con `/start` o con el botón "Volver al menú"
- [ ] El bot confirma recepción de foto antes de iniciar OCR
- [ ] El bot confirma recepción de pregunta antes de iniciar consulta
- [ ] El botón "Volver al menú" funciona en cualquier estado del chat
- [ ] Los mensajes del bot no contienen términos técnicos del sistema
