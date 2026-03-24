# ADR-0003: Persistencia en Google Sheets

**Estado:** Implementada
**Fecha:** Marzo 2026
**Autor:** IDC Ingeniería
**Proyecto:** Agente OCR Operativo para Digitalización de Registros

---

## Contexto

Se necesita una capa de persistencia para los registros digitalizados que sea:
- Accesible para los stakeholders sin software adicional
- Consultable para analítica posterior (Power BI, Looker Studio, etc.)
- Auditada (trazabilidad a imagen origen, estado OCR, respuesta cruda)
- Operable sin infraestructura de base de datos dedicada en el horizonte del proyecto (1 mes)

## Decisión

**Se usa Google Sheets (hoja `Registros_OCR`) como base de datos operativa.**

- Spreadsheet ID: `1YsDdmcT9m6TJEiz6W17XiLIMAuRicgYPML2bQl5RNYY`
- Operación: append por ítem detectado (una fila del formato = una fila en Sheets)
- 39 columnas (A–AM) con campos de trazabilidad, datos del documento y métricas de calidad OCR

## Alternativas Consideradas

### Alternativa 1: PostgreSQL / base de datos relacional
- **Pros:** Escalable, consultas SQL potentes, ACID
- **Contras:** Requiere servidor, costo de infraestructura, acceso técnico para stakeholders operativos
- **Considerada para v2+** cuando el volumen lo justifique

### Alternativa 2: Airtable
- **Pros:** Interfaz amigable, API nativa con n8n
- **Contras:** Costo para volúmenes medios, dependencia de plataforma externa
- **Descartada por:** Costo y duplicación con herramientas ya disponibles

### Alternativa 3: Archivo JSON / CSV local en servidor n8n
- **Pros:** Sin costo externo
- **Contras:** Sin acceso colaborativo, sin consulta conversacional integrada, sin respaldo cloud
- **Descartada por:** No cumple criterio de accesibilidad

## Consecuencias

**Positivas:**
- Todos los stakeholders acceden con su cuenta Google sin software adicional
- La Tool de Google Sheets permite consultas conversacionales directas desde Gemini
- Exportable a herramientas de BI estándar
- Respaldo automático en Google Drive

**Negativas / Limitaciones:**
- Si la hoja supera ~50,000 filas con frecuencia alta de acceso, la Tool de consulta puede necesitar paginación
- No hay transacciones: en caso de error parcial, algunas filas pueden quedar escritas y otras no
- La columna `Respuesta_Cruda` puede hacer crecer el tamaño de la hoja rápidamente
