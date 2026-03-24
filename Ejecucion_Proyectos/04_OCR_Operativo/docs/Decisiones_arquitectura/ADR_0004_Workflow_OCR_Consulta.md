# ADR-0004: Workflow Dual — OCR + Consulta Conversacional en un Bot

**Estado:** Implementada
**Fecha:** Marzo 2026
**Autor:** IDC Ingeniería
**Proyecto:** Agente OCR Operativo para Digitalización de Registros

---

## Contexto

El proyecto requiere dos capacidades operativas:
1. **Cargue OCR:** operadores suben fotos de registros para digitalizar
2. **Consulta:** stakeholders hacen preguntas sobre los datos históricos

Se evaluó si implementarlos como bots separados o en un solo workflow.

## Decisión

**Se implementan ambas capacidades en un único bot de Telegram con un único workflow n8n.**

El nodo `ResolverConversacion` actúa como controlador central de estado por `chatId`, detectando si el usuario está en modo `ocr_wait_photo` o `chat_query` y enrutando al `RouterMenu` correctamente. El cambio de modo se realiza con botones explícitos del bot.

## Alternativas Consideradas

### Alternativa 1: Dos bots separados (un bot para OCR, uno para consulta)
- **Pros:** Separación de preocupaciones clara, sin riesgo de interferencia de modos
- **Contras:** El usuario necesita dos contactos distintos, fragmenta la experiencia, doble mantenimiento de credenciales y flujos
- **Descartada por:** Experiencia de usuario fragmentada

### Alternativa 2: Un bot sin gestión de estado (solo comandos)
- **Pros:** Más simple de implementar
- **Contras:** Sin contexto conversacional, el usuario debe recordar comandos, experiencia menos natural para consultas en lenguaje libre
- **Descartada por:** No permite consulta conversacional fluida

## Consecuencias

**Positivas:**
- Un solo punto de contacto para el operador (un bot, una conversación)
- La misma sesión puede cargar y luego consultar sin cambiar de aplicación
- Estado de modo visible para el usuario via mensajes del bot
- Historial corto de conversación permite preguntas de seguimiento naturales

**Negativas / Limitaciones:**
- `getWorkflowStaticData('global')` puede comportarse diferente en pruebas manuales vs. ejecución real
- Si el usuario abandona el flujo a mitad, puede quedar en modo `ocr_wait_photo` hasta que cambie activamente de modo
- La limpieza de contexto al cambiar de modo evita arrastre entre OCR y consulta, pero borra el historial de preguntas anteriores
