# Runbook Operativo — OCR Operativo

**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Autor:** IDC Ingeniería
**Fecha:** Marzo 2026

---

## 1. Operación rutinaria

### 1.1 Verificar que el workflow está activo

1. Abrir n8n y verificar que el workflow `OCR Operativo v2` está en estado **Active**
2. Enviar `/start` al bot de Telegram — debe responder con el menú
3. Si no responde en 30 segundos, revisar sección de troubleshooting

### 1.2 Cargar un registro

1. Abrir Telegram y buscar el bot
2. Presionar **"Subir registro"**
3. El bot confirma que está esperando la foto
4. Enviar la foto del formato. Criterios de calidad:
   - Formato completo visible en la foto
   - Sin partes cortadas del encabezado ni de la grilla
   - Iluminación uniforme, sin sombras sobre campos
   - Enfoque nítido en el texto
5. El bot confirma procesamiento ("Procesando tu registro...")
6. En < 60 segundos recibirás el resumen con N registros guardados

### 1.3 Consultar datos

1. Presionar **"Consultar datos"**
2. El bot activa el modo consulta y muestra ejemplos
3. Escribir la pregunta en lenguaje natural
4. El bot responde con evidencia de la hoja
5. Para preguntas de seguimiento, escribir directamente (el bot recuerda el contexto)
6. Para salir del modo consulta, presionar **"Volver al menú"**

---

## 2. Revisión de registros con baja confianza

1. Abrir Google Sheets — `Registros_OCR`
2. Filtrar columna `AL` (`Estado_OCR`) = `Requiere Revision`
3. Para cada fila:
   - Revisar columna `AM` (`Respuesta_Cruda`) para ver qué extrajo Gemini
   - Contrastar con la imagen original si está disponible
   - Corregir los campos en la hoja directamente
   - Cambiar `Estado_OCR` a `Revisado` manualmente
4. Registrar el % de registros corregidos como indicador de calidad

---

## 3. Verificación de salud del sistema

| Verificación | Frecuencia | Cómo |
|-------------|-----------|-------|
| Workflow activo en n8n | Diaria | Menú responde a `/start` |
| Últimas filas en Sheets tienen Timestamp reciente | Semanal | Ordenar por col A DESC |
| % `Requiere Revision` en semana | Semanal | Filtro col AL |
| Credenciales no vencidas (Gemini API, Sheets) | Mensual | Revisar en n8n → Credentials |

---

## 4. Troubleshooting

### El bot no responde

1. Verificar estado del workflow en n8n (debe estar **Active**)
2. Verificar que el Telegram Webhook está configurado correctamente
3. Revisar logs de ejecución de n8n para el último trigger
4. Verificar que el token de Telegram no ha expirado

### El bot responde pero no guarda en Sheets

1. Revisar logs del nodo `GuardarEnSheets` en n8n
2. Verificar que la credencial de Google Sheets tiene permisos de escritura
3. Verificar que el Spreadsheet ID y la pestaña `Registros_OCR` existen
4. Verificar que la fila 1 tiene exactamente los encabezados definidos (A–AM)

### Gemini devuelve estructura inesperada

1. El workflow marca las filas como `Requiere Revision` (no rompe el flujo)
2. Revisar `Respuesta_Cruda` en Sheets para entender qué devolvió el modelo
3. Si es recurrente, revisar si el formato del documento cambió
4. Contactar a IDC Ingeniería para actualizar el prompt si es necesario

### El estado del chat queda atascado (usuario en modo OCR sin poder salir)

1. El usuario debe presionar **"Volver al menú"**
2. Si el botón no aparece, escribir cualquier texto — el bot debe mostrar el menú
3. Si persiste, el operador de n8n puede limpiar el `WorkflowStaticData` para el chatId afectado

---

## 5. Control de cambios del formato

Si el formato físico del documento cambia (nuevos campos, nueva estructura de tabla):

1. Notificar a IDC Ingeniería **antes** de comenzar a usar el nuevo formato
2. IDC actualizará el prompt del nodo `Analyze an image` y las transformaciones de `FormatearDatos`
3. Se realizarán pruebas con el nuevo formato antes de despliegue
4. Se actualizará la versión del workflow y este runbook

**No usar el bot con formatos modificados sin aprobación previa — los datos pueden guardarse incorrectamente.**

---

## 6. Contactos

| Rol | Contacto | Canal |
|-----|---------|-------|
| Soporte técnico (IDC) | Ingeniero de Analítica Predictiva | WhatsApp / correo |
| Administrador del bot | IDC Ingeniería | Según acuerdo |
| Acceso a Google Sheets | Administrador de cuenta Google IDC | Según acuerdo |
