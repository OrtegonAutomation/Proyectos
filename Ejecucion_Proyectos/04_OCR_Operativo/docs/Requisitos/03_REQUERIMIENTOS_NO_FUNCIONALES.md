# Requerimientos No Funcionales — OCR Operativo

**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Versión:** 1.0
**Fecha:** Marzo 2026

---

## RNF-01: Rendimiento

| Métrica | Objetivo | Justificación |
|---------|---------|---------------|
| Latencia bot → confirmación de foto | < 5 segundos | El operador debe saber que su foto fue recibida antes de que empiece a dudar |
| Latencia captura → datos en Sheets | < 60 minutos | Criterio de aceptación del proyecto (CA-04) |
| Tiempo de procesamiento OCR completo | < 60 segundos | Incluye llamada a Gemini, transformación e inserción en Sheets |
| Latencia respuesta a consulta conversacional | < 30 segundos | Incluye llamada a Gemini + Tool de Sheets |
| Latencia confirmación de consulta en bot | < 5 segundos | Igual que para foto: el usuario no debe esperar en silencio |

---

## RNF-02: Disponibilidad y continuidad

| Requisito | Detalle |
|-----------|---------|
| Disponibilidad del workflow | El workflow debe estar activo 24/7 en n8n |
| Degradación controlada | Si Gemini devuelve estructura inesperada, el flujo no falla: marca `Requiere Revision` y notifica al operador |
| Sin pérdida de datos por error parcial | Si una fila falla al insertar, las filas ya guardadas no se pierden |
| Recuperación de modo de chat | Si el usuario abandona el flujo, el botón "Volver al menú" restaura el estado |

---

## RNF-03: Trazabilidad y auditoría

| Requisito | Detalle |
|-----------|---------|
| Trazabilidad completa | Cada fila en Sheets tiene `File_ID_Origen`, `Chat_ID`, `Message_ID`, `Update_ID`, `Timestamp` |
| Auditoría OCR | `Respuesta_Cruda` conserva la respuesta completa de Gemini en cada fila |
| Estado de calidad por fila | `Estado_OCR` indica `Procesado` o `Requiere Revision` para cada registro |
| Inmutabilidad de registros | Las filas en Sheets son append-only; no se modifican automáticamente |

---

## RNF-04: Seguridad y acceso

| Requisito | Detalle |
|-----------|---------|
| Gestión de credenciales | Todas las credenciales (Telegram, Gemini, Sheets) se gestionan en el gestor de credenciales de n8n; no se almacenan en el código |
| Acceso a Sheets | La hoja `Registros_OCR` tiene permisos de escritura solo para la cuenta de servicio del workflow |
| Datos de imagen | Las imágenes se transmiten directamente a Gemini API; no se almacenan en n8n ni en disco local |
| Datos sensibles en `Respuesta_Cruda` | El acceso a la hoja Sheets debe ser controlado; `Respuesta_Cruda` puede contener texto del documento |

---

## RNF-05: Usabilidad

| Requisito | Detalle |
|-----------|---------|
| Sin instalación adicional | El operador solo necesita Telegram (aplicación ya instalada) |
| Curva de aprendizaje | Un operador sin instrucción técnica puede completar un cargue exitoso leyendo solo los mensajes del bot |
| Idioma | Todos los mensajes del bot en español |
| Sin jerga técnica | Los mensajes no usan términos como "workflow", "parseo", "JSON", "append" ni "API" |
| Accesibilidad del manual | El manual de captura fotográfica es comprensible con imagen de referencia en < 5 minutos de lectura |

---

## RNF-06: Mantenibilidad y extensibilidad

| Requisito | Detalle |
|-----------|---------|
| Formato de documento configurable | Agregar soporte a un nuevo formato requiere solo cambiar el prompt de `Analyze an image` y las transformaciones de `FormatearDatos`; no requiere cambiar la arquitectura |
| Versión del workflow documentada | Cada cambio significativo se registra en `CHANGELOG.md` y en el Control de Cambios |
| Nodos documentados | Los sticky notes del workflow en n8n documentan la lógica de cada sección |
| Limpieza de nodos inactivos | Los nodos heredados (`LeerDatosSheetsConsulta`, `RouterConsultaIA`) deben eliminarse en la próxima versión |

---

## RNF-07: Dependencias externas (riesgos operativos)

| Dependencia | Riesgo | Mitigación |
|------------|--------|-----------|
| Gemini API | Cuota agotada / cambio de modelo | Monitorear uso de API; documentar versión del modelo en ADR-0002 |
| Telegram | Cambio de política / caída del servicio | Sin mitigación directa; es una dependencia crítica de la interfaz |
| Google Sheets | Cuota de lectura/escritura | Para volúmenes altos, considerar migración a BD en v2+ (ADR-0003) |
| n8n | Downtime del servidor n8n | Monitorear actividad del workflow; configurar alertas de inactividad |
