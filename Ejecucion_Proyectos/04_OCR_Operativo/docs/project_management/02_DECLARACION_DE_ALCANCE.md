# Declaración de Alcance — OCR Operativo

**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Código:** OCR-Operativo-v2
**Versión:** 1.0
**Fecha:** Marzo 2026

---

## 1. Descripción del producto

Bot de Telegram conectado a un workflow n8n que:
1. Recibe fotos de registros operativos físicos
2. Extrae los datos estructurados via Gemini Vision (OCR)
3. Persiste las filas en Google Sheets con trazabilidad completa
4. Responde preguntas conversacionales sobre los datos históricos

---

## 2. En alcance

| Elemento | Descripción |
|----------|-------------|
| Formato objetivo | Registro de Fallas Centrifugas (un solo formato en v1.0) |
| Interfaz de usuario | Bot de Telegram con menú, modo OCR y modo consulta |
| OCR | Extracción estructurada de cabecera + N filas por imagen |
| Validación de calidad | Marcado automático `Procesado` / `Requiere Revision` por fila |
| Trazabilidad | File_ID_Origen, Chat_ID, Timestamp, Respuesta_Cruda en cada fila |
| Persistencia | Google Sheets `Registros_OCR`, 39 columnas (A–AM) |
| Consulta conversacional | Respuestas en lenguaje natural con evidencia real de la hoja |
| Documentación | Arquitectura, especificación técnica, ADRs, runbook, manual de captura |
| Pruebas | Plan de pruebas funcionales + UAT con Pichichí |

---

## 3. Fuera de alcance

| Elemento | Justificación |
|----------|---------------|
| Soporte de múltiples formatos | Horizonte de 1 mes, control de alcance y calidad |
| Integraciones con sistemas corporativos (ERP, CMMS) | Requiere proyecto separado |
| Automatización de decisiones operativas | Sin procedimiento de aprobación y control de riesgo definido |
| Infraestructura OT propia del cliente | Fuera del control de IDC |
| Validación automática de calidad de imagen antes de OCR | Planeado para v2+ |
| Tablero automático de KPIs de calidad de captura | Planeado para v2+ |
| Flujo de revisión humana con notificación activa | Planeado para v2+ |

---

## 4. Entregables y criterios de aceptación

| Entregable | Criterio de aceptación |
|-----------|----------------------|
| Workflow n8n activo | Bot responde en Telegram, OCR guarda en Sheets, consultas responden con evidencia |
| Base de datos `Registros_OCR` | 39 columnas correctas, trazabilidad completa, sin pérdida de datos |
| Manual de captura | Operador puede tomar fotos válidas sin instrucción adicional |
| Runbook | Equipo puede operar, monitorear y resolver incidentes básicos sin soporte de IDC |
| Plan y resultados de pruebas | ≥ 95% exactitud en dataset controlado, ≥ 98% `Procesado` en operación estable |

---

## 5. Supuestos

- El formato objetivo es estable durante el primer mes o cualquier cambio se gestiona por control de cambios
- Las fotografías cumplen los criterios mínimos del manual de captura
- Existe acceso a Google Sheets autorizado para el workflow
- El cliente (Pichichí) dispone de al menos 2 semanas de operación real para la validación de estabilidad

---

## 6. Restricciones

- El primer mes se limita a un solo formato
- Cumplimiento de políticas de seguridad de la información del cliente para imágenes y almacenamiento
- Dependencia de disponibilidad de Gemini API, Telegram y Google Sheets (servicios cloud externos)
- El flujo de revisión humana de excepciones depende de la disponibilidad del usuario revisor

---

## 7. Dependencias

| Dependencia | Tipo | Responsable |
|------------|------|-------------|
| Acceso a Gemini API con créditos activos | Técnica | IDC |
| Bot de Telegram creado y token disponible | Técnica | IDC |
| Google Sheets con permisos de escritura para la cuenta de servicio | Técnica | IDC |
| Muestra de imágenes reales del formato para pruebas | Datos | Pichichí |
| Disponibilidad de operadores para UAT | Personas | Pichichí |
