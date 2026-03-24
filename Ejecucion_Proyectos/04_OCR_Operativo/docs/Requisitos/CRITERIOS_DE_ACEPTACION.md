# Criterios de Aceptación — OCR Operativo

**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Cliente:** IDC Ingeniería / Confiabilidad Ingenio Pichichí
**Horizonte:** 1 mes
**Versión:** 1.0
**Fecha:** Marzo 2026

---

## 1. Introducción

Los criterios de aceptación definen exactamente qué debe cumplir el sistema para considerarse terminado y aceptable. Cada criterio es mensurable y verificable.

---

## 2. Criterios de Aceptación

### CA-01: Trazabilidad a imagen origen

**Especificación:**
Cada registro digitalizado debe poder rastrearse hasta la imagen que lo originó.

**Criterios:**
- [ ] **CA-01-01:** Cada fila en `Registros_OCR` tiene `File_ID_Origen` no nulo
- [ ] **CA-01-02:** `Chat_ID`, `Update_ID` y `Message_ID` permiten identificar unívocamente el mensaje de origen
- [ ] **CA-01-03:** `Timestamp` registra la fecha/hora del procesamiento en ISO 8601
- [ ] **CA-01-04:** `Respuesta_Cruda` contiene la respuesta completa de Gemini para auditoría

---

### CA-02: Extracción estructurada correcta

**Especificación:**
El workflow debe extraer correctamente los campos del formato de fallas centrifugas.

**Criterios:**
- [ ] **CA-02-01:** Una foto con N registros genera exactamente N filas en Sheets
- [ ] **CA-02-02:** Campos de cabecera (Fecha_Emision, Ubicacion, Planta) se repiten en cada fila
- [ ] **CA-02-03:** Campos no legibles devuelven `N/A`, no error del workflow
- [ ] **CA-02-04:** Checkboxes de equipos se mapean correctamente a columnas booleanas (C1–GUSANILLO)
- [ ] **CA-02-05:** `Fecha_Registro` está normalizada en formato `DD/MM/YYYY`
- [ ] **CA-02-06:** `Equipos_Afectados` consolida correctamente la lista de equipos marcados

---

### CA-03: Estado de calidad OCR

**Especificación:**
El sistema debe indicar explícitamente la confianza de cada registro.

**Criterios:**
- [ ] **CA-03-01:** Cada fila tiene `Estado_OCR` con valor `Procesado` o `Requiere Revision`
- [ ] **CA-03-02:** El operador recibe notificación cuando hay filas con `Requiere Revision` en el resumen del cargue
- [ ] **CA-03-03:** En operación estable, ≥ 98% de registros tienen estado `Procesado` sin intervención manual
- [ ] **CA-03-04:** % de campos `N/A` en muestra auditada se registra como indicador de seguimiento

---

### CA-04: Latencia de procesamiento

**Especificación:**
El tiempo entre captura de foto y disponibilidad en Sheets no debe superar 1 hora.

**Criterios:**
- [ ] **CA-04-01:** Latencia promedio captura → Sheets < 60 minutos
- [ ] **CA-04-02:** El operador recibe confirmación de cargue exitoso (o error) en el bot
- [ ] **CA-04-03:** Tiempo de respuesta del bot ante foto < 60 segundos en condiciones normales

---

### CA-05: Consulta conversacional con evidencia

**Especificación:**
Las respuestas a preguntas sobre datos deben estar basadas en la hoja real, no en memoria del modelo.

**Criterios:**
- [ ] **CA-05-01:** El modelo usa la Tool de Sheets para preguntas sobre datos (no responde de memoria)
- [ ] **CA-05-02:** Las respuestas pueden incluir conteos, rankings y filtros por equipo/fecha/tipo de falla
- [ ] **CA-05-03:** El historial de conversación permite preguntas de seguimiento ("¿y en ese equipo?")
- [ ] **CA-05-04:** Tiempo de respuesta a consulta < 30 segundos en condiciones normales

---

### CA-06: Experiencia de usuario del bot

**Especificación:**
El bot debe ser operable por un técnico de campo sin instrucción adicional.

**Criterios:**
- [ ] **CA-06-01:** El menú principal es accesible con un botón y tiene instrucciones claras
- [ ] **CA-06-02:** El bot confirma recepción de foto antes de iniciar OCR (no silencio)
- [ ] **CA-06-03:** El bot confirma recepción de pregunta antes de consultar (no silencio)
- [ ] **CA-06-04:** El botón "Volver al menú" funciona en cualquier estado del chat
- [ ] **CA-06-05:** El resumen de cargue comunica cuántos registros se guardaron en lenguaje operativo

---

### CA-07: Operación estable del pipeline

**Especificación:**
El workflow debe operar con auditoría y evidencia de ejecución de manera sostenida.

**Criterios:**
- [ ] **CA-07-01:** Efectividad de digitalización ≥ 98% sostenida durante 2 semanas
- [ ] **CA-07-02:** Sin pérdida de datos por error silencioso (siempre hay registro en Sheets o `Respuesta_Cruda`)
- [ ] **CA-07-03:** El workflow no cae por imágenes de baja calidad (marca `Requiere Revision` sin romper el flujo)
- [ ] **CA-07-04:** El workflow no cae por respuestas inesperadas de Gemini

---

## 3. Criterios fuera de alcance (v1.0)

- Soporte de múltiples formatos distintos
- Validación automática de calidad de imagen antes del OCR
- Flujo de revisión humana con notificación activa al revisor
- Tablero automático de KPIs de calidad de captura
