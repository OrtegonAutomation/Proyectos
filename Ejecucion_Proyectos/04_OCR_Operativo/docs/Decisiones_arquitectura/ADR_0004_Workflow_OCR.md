# ADR 0004: Flujo Principal de OCR Operativo (n8n + Gemini)

## Estado
Aceptado - Implementado.

## Contexto
El flujo original de procesamiento y validación de formato físico ("04_OCR_Operativo") requiere un orquestador para recibir imágenes desde Telegram, procesarlas usando IA y archivarlas dinámicamente en Google Sheets, proporcionando inmediatamente *feedback* al operario. Se presentaron retos significativos al pasar datos binarios a lo largo de nodos divergentes.

## Decisión
Se ha construido y estabilizado un flujo de trabajo en n8n mediante las siguientes 3 fases operativas:

### 1. Fase de Recepción
- **TelegramTrigger:** Escucha pasivamente la cuenta del Bot.
- **IsPhoto (`If`):** Actúa como compuerta para ignorar mensajes de texto planos. Redirige únicamente los "updates" que contienen la propiedad JSON `message.photo`.
- **NotifyProcessing:** Retorna un mensaje síncrono ("Procesando imagen con IA...") notificando al empleado que su documento entró en cola. 

### 2. Fase de Inteligencia Artificial (Procesamiento)
- **Wait For Notify (`Code`, reemplazando `Merge`):** Para garantizar el orden e impedir que la carga de IA se dispare antes de la notificación de "Procesando", se introdujo un nodo código puro que pausa el flujo. Adicionalmente, este nodo extrae exclusivamente el índice `[0]` de los *data items* de `IsPhoto` para evitar un recorrido de arreglo múltiple ocasionado por Telegram (quien envía *n* resoluciones de foto por subida).
- **GeminiOCR:** Invoca la API de Google Gemini enviando los binarios bajo el formato *multipart/form-data* junto al prompt rector.
- **FormatSheets (`Code`):** Módulo sanitizador de JSON. Intercepta la respuesta bruta de Gemini, elimina cualquier envoltura "Markdown" (como bloques \`\`\`json \`\`\`) y asigna variables unificadas (`Código`, `Versión`, `Vigencia`) o *fallbacks* como `"No detectado"`.

### 3. Fase de Persistencia y Feedback
- **Google Sheets:** Escribe en la hoja conectada el archivo de revisión con la *Fecha*, *ID de Telegram*, y los campos limpios.
- **NeedsReview (`If`):** Examina semánticamente los datos. Si cualquier campo viene nulo o con el *fallback* de error ("No detectado"), enruta la salida por validación negativa.
- **Notificaciones Finales:**
  - **NotifySuccess (`Telegram`):** Formatea exitosamente el mensaje de guardado con Checklists verdes y *MarkdownV2*.
  - **NotifyReview (`Telegram`):** Retorna el mensaje de alerta al empleado solicitando validación manual si Gemini no logró interpretar el 100% del formulario, adjuntando puntualmente los valores incompletos.

## Consecuencias Positivas
- El operador del frente de obra recibe confirmación directa en *menos de un segundo* ("Procesando...").
- Disminución absoluta de dobles envíos gracias a la restricción del nodo de espera en array de índice 0.
- Desacoplamiento de las lógicas de limpieza de AI y lógicas de escritura en Sheets (lo que previene que fallos de cuota detengan la notificación de error).

## Consideraciones Futuras
- Si la complejidad de los formularios aumenta (ej. tablas dinámicas o cuadrículas), se deberá migrar de extracciones JSON manuales hacia agentes estructurados utilizando Output Parsers integrados de n8n Langchain.
