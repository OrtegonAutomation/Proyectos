# ADR-0002: Motor OCR — Google Gemini Vision

**Estado:** Implementada
**Fecha:** Marzo 2026
**Autor:** IDC Ingeniería
**Proyecto:** Agente OCR Operativo para Digitalización de Registros

---

## Contexto

El formato de registro de fallas centrifugas es una tabla física con:
- Cabecera de documento (3 campos)
- Grilla de N filas con ~15 campos por fila
- Checkboxes para equipos afectados (10 opciones)
- Campos numéricos con posibles errores de lectura (ej: `22` vs `2.2`)
- Texto manuscrito en observaciones y firmas

Se evaluó qué motor OCR usar para extraer esta información estructurada desde fotos de campo.

## Decisión

**Se usa Google Gemini Vision (`gemini-2.5-flash`) como motor OCR.**

El prompt exige JSON estricto con esquema conocido (`documento` + `items[]`). Gemini interpreta el layout visual de la tabla directamente, detecta checkboxes marcados y genera la estructura requerida sin preprocesamiento de imagen.

## Alternativas Consideradas

### Alternativa 1: Google Vision API (dedicada, solo OCR)
- **Pros:** API especializada, alta precisión en texto impreso
- **Contras:** Devuelve texto crudo sin estructura; requiere lógica adicional para parsear la tabla y asignar valores a campos; no detecta checkboxes semánticamente
- **Descartada por:** Complejidad de post-procesamiento para tablas con checkboxes

### Alternativa 2: Azure Document Intelligence (Form Recognizer)
- **Pros:** Especializado en formularios estructurados, entrenamiento por plantilla
- **Contras:** Requiere entrenamiento del modelo con muestras etiquetadas, costo y tiempo adicional, dependencia de Azure
- **Descartada por:** Tiempo de implementación y costo

### Alternativa 3: Tesseract + Python
- **Pros:** Open source, sin costo de API
- **Contras:** Requiere servidor Python, baja precisión en fotos de campo con iluminación variable, no interpreta checkboxes
- **Descartada por:** Calidad insuficiente para fotos de campo

## Consecuencias

**Positivas:**
- Gemini interpreta semánticamente el layout sin entrenamiento previo
- Un mismo modelo sirve para OCR y para la rama de consulta conversacional
- Respuesta directamente en JSON reduce código de transformación
- Adaptable a nuevos formatos solo cambiando el prompt

**Negativas / Limitaciones:**
- Si Gemini devuelve estructura inesperada, `FormatearDatos` marca `Requiere Revision` en lugar de perder datos
- Calidad de la extracción depende de la calidad fotográfica (iluminación, encuadre, nitidez)
- Ver `Manual de Captura` para criterios mínimos de calidad de imagen
