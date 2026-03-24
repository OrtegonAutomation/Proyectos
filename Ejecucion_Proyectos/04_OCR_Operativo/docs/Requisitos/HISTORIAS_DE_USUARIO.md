# Historias de Usuario — OCR Operativo

**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Cliente:** IDC Ingeniería / Confiabilidad Ingenio Pichichí
**Versión:** 1.0
**Fecha:** Marzo 2026

---

## 1. Personas de Usuario

### Persona 1: Operador de Campo — Pichichí
**Nombre:** Andrés (Operador)
**Rol:** Técnico de turno, diligencia el formato de fallas centrifugas
**Experiencia:** 3 años en planta, usuario de WhatsApp/Telegram, sin experiencia técnica
**Necesidades:** Proceso simple, respuesta rápida, saber si el cargue quedó bien

### Persona 2: Ingeniero de Confiabilidad — IDC / Pichichí
**Nombre:** María (Ingeniero)
**Rol:** Analiza los datos operativos, valida registros y saca conclusiones
**Experiencia:** 6 años, usa Excel y Power BI, cómoda con Google Sheets
**Necesidades:** Datos confiables, trazabilidad a imagen origen, capacidad de consulta

### Persona 3: Supervisor de Operaciones — Pichichí
**Nombre:** Roberto (Supervisor)
**Rol:** Revisa tendencias, toma decisiones sobre equipos
**Experiencia:** 10 años en planta, visión operativa
**Necesidades:** Respuestas rápidas a preguntas sobre equipos, sin abrir herramientas complejas

---

## 2. Historias de Usuario — Cargue OCR

### HU-01: Subir foto de registro desde campo

**Como** operador de campo (Andrés)
**Quiero** enviar una foto del formato de fallas al bot de Telegram
**Para** digitalizar el registro sin tener que transcribir los datos manualmente

**Criterios de aceptación:**
- [ ] El bot tiene un botón claro "Subir registro" en el menú
- [ ] El bot explica qué foto espera y con qué calidad antes de recibirla
- [ ] Al recibir la foto, el bot confirma recepción inmediatamente (no silencio)
- [ ] El cargue procesa y guarda los datos en < 60 segundos
- [ ] El bot responde con resumen en lenguaje operativo: "Se guardaron N registros"
- [ ] Si hay registros con baja confianza, el bot lo indica en el resumen
- [ ] El operador puede subir otra foto sin necesidad de reiniciar el chat

**Notas:**
- Andrés usa Telegram desde el móvil en el área de producción
- La foto puede tomarse con iluminación variable

---

### HU-02: Recibir confirmación clara del cargue

**Como** operador de campo (Andrés)
**Quiero** saber exactamente cuántos registros se guardaron y si hubo problemas
**Para** no quedar con dudas de si el cargue quedó completo

**Criterios de aceptación:**
- [ ] El resumen incluye: número de filas guardadas, rango de fechas detectadas
- [ ] Si hay filas con `Requiere Revision`, el resumen lo indica con el conteo
- [ ] El mensaje de confirmación es en lenguaje operativo, sin jerga técnica
- [ ] El operador sabe qué hacer si hay registros para revisión
- [ ] El bot mantiene el botón "Volver al menú" siempre visible tras el cargue

---

### HU-03: Tomar una foto que el OCR pueda leer bien

**Como** operador de campo (Andrés)
**Quiero** saber qué calidad debe tener la foto antes de enviarla
**Para** evitar que el sistema rechace o malinterprete mis registros

**Criterios de aceptación:**
- [ ] El bot describe la calidad esperada al activar el modo cargue
- [ ] Existe un manual de captura disponible (fuera del bot) con ejemplos visuales
- [ ] El criterio mínimo se puede explicar en 2-3 oraciones simples
- [ ] Si la foto no es procesable, el bot notifica con un mensaje claro (no error técnico)

---

## 3. Historias de Usuario — Consulta Conversacional

### HU-04: Consultar datos en lenguaje natural

**Como** ingeniero de confiabilidad (María)
**Quiero** hacer preguntas sobre los registros en texto libre
**Para** obtener respuestas rápidas sin abrir ni filtrar Google Sheets manualmente

**Criterios de aceptación:**
- [ ] El bot tiene un botón "Consultar datos" en el menú
- [ ] El bot activa el modo consulta y muestra ejemplos de preguntas útiles
- [ ] Las respuestas se basan en datos reales de la hoja (no en memoria del modelo)
- [ ] El bot responde preguntas de conteo, ranking y filtro por equipo o fecha
- [ ] La respuesta llega en < 30 segundos
- [ ] El bot mantiene el botón "Volver al menú" visible durante el modo consulta

---

### HU-05: Hacer preguntas de seguimiento en la misma conversación

**Como** ingeniero (María)
**Quiero** continuar una conversación sin repetir el contexto en cada pregunta
**Para** explorar los datos de forma natural ("¿y cuántos en C2?", "¿de ese equipo?")

**Criterios de aceptación:**
- [ ] El bot recuerda el contexto de las últimas preguntas de la sesión
- [ ] Las preguntas de seguimiento se resuelven correctamente sin repetir contexto
- [ ] Al presionar "Volver al menú", el historial se limpia limpiamente
- [ ] El historial no crece indefinidamente (se recorta para mantener rendimiento)

---

### HU-06: Consultar frecuencia de fallas por equipo

**Como** supervisor (Roberto)
**Quiero** preguntar cuántas veces ha fallado un equipo específico
**Para** decidir si priorizo una intervención de mantenimiento

**Criterios de aceptación:**
- [ ] La pregunta "¿Cuántas fallas tiene el equipo C1?" produce un conteo con evidencia
- [ ] La respuesta puede filtrarse por período si la pregunta lo incluye
- [ ] La respuesta indica el equipo exacto consultado y la fuente (Sheets)
- [ ] Si no hay datos del equipo, el bot lo indica explícitamente

---

## 4. Historias de Usuario — Calidad y Revisión

### HU-07: Identificar registros que necesitan revisión manual

**Como** ingeniero (María)
**Quiero** saber cuáles registros quedaron con baja confianza tras el OCR
**Para** revisarlos en Google Sheets y corregirlos antes de usarlos en análisis

**Criterios de aceptación:**
- [ ] Cada fila en Sheets tiene columna `Estado_OCR` con valor `Procesado` o `Requiere Revision`
- [ ] La columna es filtrable directamente en Sheets
- [ ] La columna `Respuesta_Cruda` contiene el JSON de Gemini para entender qué extrajo
- [ ] El resumen del bot informa cuántas filas del cargue tienen `Requiere Revision`

---

### HU-08: Auditar el origen de un registro

**Como** ingeniero (María)
**Quiero** saber qué foto generó un registro específico en Sheets
**Para** verificar la lectura en caso de duda o inconsistencia

**Criterios de aceptación:**
- [ ] Cada fila tiene `File_ID_Origen` con el file_id de Telegram de la foto
- [ ] Cada fila tiene `Chat_ID`, `Message_ID` y `Update_ID` para rastrear el mensaje
- [ ] Cada fila tiene `Timestamp` con la fecha y hora exacta del procesamiento
- [ ] La columna `Respuesta_Cruda` permite ver exactamente qué leyó Gemini

---

## 5. Historias de Usuario — Operación y Mantenimiento

### HU-09: Volver al menú desde cualquier estado

**Como** cualquier usuario del bot
**Quiero** poder salir del modo actual y volver al menú principal en cualquier momento
**Para** no quedar atrapado en un modo si me equivoqué o quiero cambiar de actividad

**Criterios de aceptación:**
- [ ] El botón "Volver al menú" está disponible en todos los modos
- [ ] Al presionarlo, el modo se resetea y el menú aparece inmediatamente
- [ ] El historial de consulta se limpia al cambiar de modo
- [ ] El bot no queda en estado inconsistente si el usuario abandona un flujo a mitad

---

### HU-10: Operar el bot sin instrucción técnica

**Como** nuevo operador
**Quiero** entender cómo usar el bot solo leyendo sus mensajes
**Para** no depender de soporte de IDC para el uso diario

**Criterios de aceptación:**
- [ ] El menú principal es autoexplicativo con botones claros
- [ ] Los mensajes del bot no usan términos técnicos (no "workflow", "parseo", "JSON")
- [ ] Ante cualquier acción, el bot confirma qué recibió y qué hará
- [ ] El manual de captura es suficiente para que un nuevo operador tome fotos válidas

---

## 6. Matriz de Prioridad

| ID | Historia | Prioridad | Complejidad | Sprint |
|----|----------|-----------|------------|--------|
| HU-01 | Subir foto de registro | Alta | Alta | 1 |
| HU-02 | Confirmación de cargue | Alta | Baja | 1 |
| HU-03 | Calidad de foto | Alta | Baja | 1 |
| HU-04 | Consulta en lenguaje natural | Alta | Alta | 2 |
| HU-05 | Preguntas de seguimiento | Media | Media | 2 |
| HU-06 | Consulta por equipo | Alta | Baja | 2 |
| HU-07 | Identificar revisión manual | Alta | Baja | 1 |
| HU-08 | Auditar origen de registro | Media | Baja | 1 |
| HU-09 | Volver al menú | Alta | Baja | 1 |
| HU-10 | Operar sin instrucción técnica | Alta | Media | 2 |
