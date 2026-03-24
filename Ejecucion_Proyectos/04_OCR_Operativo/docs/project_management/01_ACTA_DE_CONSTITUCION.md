# Acta de Constitución del Proyecto — OCR Operativo

**Código:** IC-PR-18 (Línea 3)
**Versión:** 1.0
**Fecha:** Enero 2026
**Estado:** Activo

---

## 1. Identificación del proyecto

| Campo | Valor |
|-------|-------|
| **Nombre** | Agente OCR Operativo para Digitalización de Registros |
| **Código** | OCR-Operativo-v2 |
| **Sponsor** | IDC Ingeniería de Confiabilidad |
| **Primer usuario final** | Confiabilidad Ingenio Pichichí |
| **Horizonte** | 1 mes |
| **Prioridad** | 2 (dentro del portafolio IC-PR-18) |

---

## 2. Propósito

Digitalizar de forma trazable registros operativos capturados en fotografías, transformándolos en datos estructurados en una base de datos consultable. En el primer mes se soporta un solo formato, con control de calidad por campo y flujo de revisión para garantizar confiabilidad del dato.

---

## 3. Caso de negocio

Cuando la información operativa queda en imágenes, se pierde oportunidad analítica, se incrementa el tiempo de disponibilidad del dato y se elevan los errores por transcripción. Esto limita la correlación entre condición y proceso y reduce la capacidad de cerrar casos con evidencia.

**Valor esperado:** Convertir el registro a datos estructurados con trazabilidad a la fuente, validaciones por rangos y consistencia, y una ruta explícita de revisión para campos con baja confianza.

---

## 4. Objetivos

1. Construir un conjunto de imágenes representativas para calibración y verificación del desempeño del OCR
2. Implementar extracción por plantilla y validación por campo con puntaje de confianza y registro de calidad
3. Implementar revisión humana para excepciones con bitácora de correcciones y trazabilidad a la imagen
4. Publicar los registros en base de datos con consultas y exportación para analítica de operación y confiabilidad

---

## 5. Alcance

**En alcance:**
- Un solo formato operativo durante el primer mes, con control de cambios del formato
- OCR, extracción por campo, validaciones, control de calidad y trazabilidad a imagen fuente
- Base de datos estructurada y consultas para análisis y auditoría
- Flujo de revisión humana para campos con baja confianza y bitácora de correcciones

**Fuera de alcance:**
- Soporte de varios formatos diferentes en el primer mes
- Integraciones complejas con sistemas corporativos
- Automatización de decisiones operativas sin procedimiento de aprobación y control de riesgo

---

## 6. Entregables

| Entregable | Descripción |
|-----------|-------------|
| Workflow n8n productivo | Bot de Telegram con OCR + consulta conversacional |
| Base de datos operativa | Google Sheets `Registros_OCR` con 39 columnas y trazabilidad completa |
| Documentación técnica | ARQUITECTURA.md, ESPECIFICACION_TECNICA.md, ADRs (4) |
| Manual de captura | Criterios mínimos de calidad fotográfica |
| Runbook operativo | Procedimientos de operación, mantenimiento y troubleshooting |
| Plan y reporte de pruebas | Validación funcional y UAT |

---

## 7. Criterios de aceptación (resumen)

- Trazabilidad completa de cada registro a la imagen fuente
- % de campos con baja confianza > 5% activa retroalimentación y ajuste
- Latencia captura → Sheets < 1 hora
- Efectividad de digitalización ≥ 98% sostenida en 2 semanas

---

## 8. Partes interesadas

| Parte | Rol |
|-------|-----|
| IDC Ingeniería | Diseño del pipeline, validación, documentación |
| Confiabilidad Pichichí | Primer usuario final, retroalimentación con formato real |
| Operación (cliente final) | Define formato objetivo, valida reglas de negocio |

---

## 9. Cronograma de hitos

| Semana | Hito |
|--------|------|
| Semana 1 | Definición de formato, diccionario, reglas de validación; muestra inicial |
| Semana 2 | Construcción del dataset de verificación y primer pipeline OCR |
| Semana 3 | Validación por campo, flujo de revisión humana, bitácora y persistencia en Sheets |
| Semana 4 | Despliegue, auditoría de desempeño, manual de captura y reporte |

---

## 10. Riesgos principales

| Riesgo | Respuesta |
|--------|-----------|
| Baja calidad de imagen | Lineamientos de captura, validación automática de legibilidad, rechazo controlado |
| Cambios no controlados del formato | Control de cambios, versionado de plantillas, trazabilidad por versión |
| Errores silenciosos en campos críticos | Reglas de consistencia, revisión humana por umbral de confianza, auditorías |

---

## 11. Indicadores de seguimiento

- Exactitud por campo en muestra auditada
- Porcentaje de campos auto extraídos sin revisión
- Tiempo promedio entre captura y disponibilidad en base de datos
- Porcentaje de registros rechazados por baja calidad y causas predominantes
