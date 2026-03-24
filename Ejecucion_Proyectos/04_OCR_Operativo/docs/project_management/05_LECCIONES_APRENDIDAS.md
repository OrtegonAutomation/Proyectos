# Lecciones Aprendidas — OCR Operativo

**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Versión:** 1.0
**Fecha:** Marzo 2026

---

## 1. Lecciones de diseño y arquitectura

| ID | Lección | Categoría | Aplicar en |
|----|---------|-----------|------------|
| LA-01 | Gemini Vision extrae semánticamente el layout de tablas sin entrenamiento previo; el prompt con esquema JSON estricto es suficiente para formatos físicos simples | Arquitectura / OCR | Nuevos formatos en v2+ |
| LA-02 | Unificar OCR y consulta en un solo bot mejora la experiencia del operador, pero requiere gestión de estado explícita por chatId para evitar interferencia entre modos | Arquitectura / UX | Proyectos con múltiples flujos en un bot |
| LA-03 | `getWorkflowStaticData('global')` en n8n tiene comportamiento diferente entre pruebas manuales y ejecución real del workflow activo; las pruebas de estado deben hacerse con el workflow publicado | Técnica / Testing | Cualquier workflow n8n con estado persistente |
| LA-04 | Conservar `Respuesta_Cruda` en Sheets desde el inicio es esencial para depurar errores de extracción; sin ella es imposible entender qué vio Gemini en la imagen | Calidad de datos | Todos los proyectos OCR |

---

## 2. Lecciones de operación y calidad de datos

| ID | Lección | Categoría | Aplicar en |
|----|---------|-----------|------------|
| LA-05 | La calidad fotográfica es el factor más determinante de la exactitud OCR; el manual de captura debe entregarse y validarse antes de iniciar operación, no al final | Operación | Todos los proyectos OCR de campo |
| LA-06 | Marcar `Requiere Revision` en lugar de fallar el workflow ante parseos inesperados garantiza que nunca se pierda trazabilidad; es mejor tener un registro imperfecto que ninguno | Calidad / Robustez | Pipelines OCR en producción |
| LA-07 | La columna `Estado_OCR` debe ser visible y filtrable desde el primer día para que el equipo pueda gestionar la revisión sin instrucción técnica adicional | Operación | Bases de datos operativas en Sheets |

---

## 3. Lecciones de gestión del proyecto

| ID | Lección | Categoría | Aplicar en |
|----|---------|-----------|------------|
| LA-08 | El horizonte de 1 mes es viable para un formato único con stack ligero (n8n + Gemini + Sheets); ampliar a múltiples formatos requiere al menos un mes adicional por formato | Planificación | Estimación de proyectos OCR |
| LA-09 | La restricción de alcance a un solo formato en el primer mes fue clave para entregar con calidad; ampliar el alcance sin ajustar el horizonte habría comprometido la trazabilidad | Alcance | Propuestas de proyectos similares |
| LA-10 | Documentar los ADRs durante la implementación (no al final) facilita las decisiones de diseño y deja trazabilidad de por qué se descartaron alternativas | Gobernanza | Todos los proyectos de analítica |

---

## 4. Pendientes de registrar

*Esta sección se completa al finalizar la fase de UAT y las primeras 2 semanas de operación estable.*

| Tema pendiente | Fecha esperada |
|---------------|---------------|
| Resultados de exactitud en UAT real con Pichichí | Semana 4 |
| Comportamiento del estado conversacional en producción sostenida | 2 semanas post-despliegue |
| % real de `Requiere Revision` en operación de campo | 2 semanas post-despliegue |
