# ADR-0001: Arquitectura n8n + Gemini + Telegram

**Estado:** Implementada
**Fecha:** Marzo 2026
**Autor:** IDC Ingeniería
**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Cliente:** IDC Ingeniería / Confiabilidad Ingenio Pichichí

---

## Contexto

Se requiere un agente para digitalizar registros operativos fotografiados (formatos físicos con tablas), transformándolos en datos estructurados consultables. Los operadores son usuarios de campo sin experiencia técnica. Se necesita:

- Captura simple de imágenes desde el campo (móvil)
- Procesamiento OCR con extracción estructurada (múltiples filas por imagen)
- Persistencia trazable en base de datos consultable
- Consulta conversacional en lenguaje natural sobre los datos históricos
- Despliegue en 1 mes sin infraestructura OT compleja

## Decisión

**Se adopta stack: n8n (orquestación) + Google Gemini API (OCR + chat) + Telegram (interfaz) + Google Sheets (persistencia).**

- **n8n:** Orquesta el flujo completo sin necesidad de código de servidor propio. Gestiona estado conversacional, triggers de Telegram, llamadas a Gemini y escritura en Sheets.
- **Telegram Bot:** Interfaz de captura accesible desde cualquier móvil, sin instalar apps adicionales. Manejo nativo de fotos con descarga binaria.
- **Google Gemini:** Motor OCR y chat en un solo modelo. Permite extraer datos estructurados desde imágenes y responder consultas conversacionales sobre datos reales.
- **Google Sheets:** Base de datos operativa ligera, sin infraestructura adicional, accesible para analítica posterior con herramientas de BI.

## Alternativas Consideradas

### Alternativa 1: Aplicación web + backend Python + PostgreSQL
- **Pros:** Stack robusto, escalable, control total de datos
- **Contras:** Requiere infraestructura de servidor, tiempo de desarrollo mayor a 1 mes, curva de adopción alta para operadores de campo
- **Descartada por:** Horizonte de 1 mes y perfil de usuarios finales

### Alternativa 2: Google Apps Script + Google Vision API
- **Pros:** Nativo en ecosistema Google, sin costo de n8n
- **Contras:** Sin interfaz conversacional, limitado para multi-formato futuro, difícil de mantener y extender
- **Descartada por:** Limitaciones de UX y extensibilidad

### Alternativa 3: Power Automate + Azure AI
- **Pros:** Integración nativa con ecosistema Microsoft
- **Contras:** Costo licencias, complejidad de configuración, dependencia de tenant corporativo
- **Descartada por:** Acceso y costo

## Consecuencias

**Positivas:**
- Despliegue en días, no semanas
- Interfaz familiar para operadores (Telegram)
- Un solo modelo (Gemini) para OCR y chat reduce complejidad
- Google Sheets como "base de datos" accesible a todos los stakeholders sin software adicional

**Negativas / Limitaciones:**
- `getWorkflowStaticData()` puede no persistir correctamente en pruebas manuales de n8n
- Si la hoja crece significativamente, la consulta conversacional puede requerir filtros adicionales
- Dependencia de servicios cloud externos (Gemini, Telegram, Sheets)
