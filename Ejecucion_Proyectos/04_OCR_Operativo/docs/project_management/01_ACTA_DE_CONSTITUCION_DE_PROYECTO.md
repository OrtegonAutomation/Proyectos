# ACTA DE CONSTITUCION DE PROYECTO

| Campo | Detalle |
|---|---|
| **Proyecto** | OCR Operativo - Sistema de Digitalizacion de Registros Operacionales |
| **Codigo** | PRY-04-OCR |
| **Version** | 1.0 |
| **Fecha de Creacion** | 2026-02-24 |
| **Ultima Actualizacion** | 2026-02-24 |
| **Estado** | Borrador |
| **Prioridad** | MEDIA |
| **Preparado por** | Project Manager - IDC Ingenieria |

---

## 1. Justificacion del Proyecto

El area de operaciones actualmente gestiona registros operacionales de forma manual o semi-manual, lo que genera:

- **Errores de transcripcion** frecuentes que impactan la calidad de los datos en los sistemas ERP/SAP.
- **Tiempos elevados** de procesamiento y captura de informacion operacional.
- **Falta de trazabilidad** en el flujo de documentos fisicos a digitales.
- **Cuellos de botella** en la integracion de datos operacionales con los sistemas centrales.

La implementacion de un sistema OCR con precision superior al 95% permitira automatizar la digitalizacion de estos registros, reduciendo tiempos de procesamiento, minimizando errores humanos y habilitando la integracion directa con SAP/ERP.

### Beneficios Esperados

| Beneficio | Impacto Estimado |
|---|---|
| Reduccion de errores de transcripcion | > 90% |
| Reduccion de tiempo de procesamiento | > 70% |
| Disponibilidad de datos en tiempo real | < 5 min post-escaneo |
| Eliminacion de doble captura manual | 100% |

---

## 2. Objetivos SMART

### Objetivo General

Desarrollar e implementar un sistema OCR operativo que digitalice registros operacionales con una precision minima del 95%, integrado con SAP/ERP, en un plazo de 4 semanas y dentro del presupuesto de $200,000 - $280,000 USD.

### Objetivos Especificos

| # | Objetivo | Especifico | Medible | Alcanzable | Relevante | Temporal |
|---|---|---|---|---|---|---|
| O1 | Implementar motor OCR con Google Cloud Vision | Pipeline de extraccion de texto de imagenes/PDF | Precision >= 95% en conjunto de pruebas | Tecnologia probada (GCV) | Automatiza captura manual | Semana 1-2 |
| O2 | Desarrollar API REST de procesamiento | Endpoints FastAPI para ingesta y consulta | Throughput >= 50 docs/min | Stack Python validado | Centraliza logica de negocio | Semana 1-3 |
| O3 | Construir interfaz de usuario web | Dashboard React para operadores | Tiempo de carga < 2s, usabilidad > 4/5 | React 18 con equipo capacitado | Reduce friccion operativa | Semana 2-3 |
| O4 | Integrar con SAP/ERP | Conexion bidireccional de datos | 100% de campos mapeados correctamente | APIs SAP disponibles | Elimina doble captura | Semana 3-4 |
| O5 | Desplegar en produccion GCP/K8s | Infraestructura escalable y monitoreada | Disponibilidad >= 99.5% | Equipo DevOps asignado | Garantiza operacion continua | Semana 4 |

---

## 3. Alcance del Proyecto

### 3.1 Dentro del Alcance (IN)

- Desarrollo del motor de procesamiento OCR utilizando Google Cloud Vision API.
- API REST con FastAPI para ingesta de documentos, procesamiento y consulta de resultados.
- Base de datos PostgreSQL para almacenamiento de registros procesados y metadatos.
- Interfaz web en React 18 para carga de documentos, revision de resultados y correccion manual.
- Integracion con SAP/ERP para envio automatico de datos extraidos.
- Pipeline de validacion y correccion de datos post-OCR.
- Despliegue en Google Cloud Platform con Kubernetes.
- Documentacion tecnica y de usuario.
- Pruebas unitarias, de integracion y de aceptacion.
- Capacitacion basica al equipo de operaciones.

### 3.2 Fuera del Alcance (OUT)

- Digitalizacion de documentos historicos (backlog anterior al go-live).
- Desarrollo de aplicacion movil nativa.
- Integracion con sistemas distintos a SAP/ERP.
- Hardware de escaneo (se asume infraestructura existente).
- Soporte post-produccion mas alla de 2 semanas despues del go-live.
- Entrenamiento de modelos ML personalizados (se usa GCV como servicio).
- Procesamiento de documentos en idiomas distintos al espanol.
- Migracion de datos legacy.

---

## 4. Entregables Principales

| # | Entregable | Descripcion | Fase |
|---|---|---|---|
| E1 | Motor OCR funcional | Pipeline de procesamiento con GCV integrado | Fase 1 |
| E2 | API REST documentada | Endpoints FastAPI con documentacion OpenAPI/Swagger | Fase 2 |
| E3 | Base de datos operativa | Esquema PostgreSQL con modelos de datos | Fase 1 |
| E4 | Interfaz web | Dashboard React para operadores | Fase 2-3 |
| E5 | Integracion SAP/ERP | Modulo de sincronizacion bidireccional | Fase 3 |
| E6 | Infraestructura cloud | Cluster K8s en GCP con CI/CD | Fase 3-4 |
| E7 | Suite de pruebas | Tests unitarios, integracion y E2E | Fase 3-4 |
| E8 | Documentacion | Tecnica, de usuario y de operacion | Fase 4 |
| E9 | Capacitacion | Material y sesiones para equipo de operaciones | Fase 4 |

---

## 5. Hitos Criticos

| # | Hito | Fecha Objetivo | Criterio de Cumplimiento |
|---|---|---|---|
| H1 | **Fin Fase 1 - Fundamentos** | Semana 1 (Dia 7) | Motor OCR procesando con >= 90% precision en ambiente dev |
| H2 | **Fin Fase 2 - Desarrollo Core** | Semana 2 (Dia 14) | API y frontend funcionales en ambiente de staging |
| H3 | **Fin Fase 3 - Integracion** | Semana 3 (Dia 21) | Integracion SAP validada, pruebas de aceptacion iniciadas |
| H4 | **Fin Fase 4 - Produccion** | Semana 4 (Dia 28) | Sistema desplegado en produccion, capacitacion completada |

### Criterios de Paso de Fase

- **Fase 1 a Fase 2**: Motor OCR validado con dataset de prueba, esquema de BD aprobado.
- **Fase 2 a Fase 3**: API con cobertura de tests > 80%, frontend con flujos principales funcionales.
- **Fase 3 a Fase 4**: Integracion SAP funcionando end-to-end, pruebas de aceptacion con > 90% de casos exitosos.
- **Go-Live**: Precision OCR >= 95%, pruebas UAT aprobadas, documentacion completa.

---

## 6. Presupuesto Estimado

### Resumen Financiero

| Concepto | Estimacion Baja | Estimacion Alta |
|---|---|---|
| **Recursos Humanos** | $140,000 | $190,000 |
| **Infraestructura Cloud (GCP)** | $15,000 | $25,000 |
| **Licencias y APIs (GCV)** | $10,000 | $18,000 |
| **Herramientas y Software** | $5,000 | $7,000 |
| **Contingencia (15%)** | $25,500 | $36,000 |
| **Capacitacion** | $4,500 | $4,000 |
| **TOTAL** | **$200,000** | **$280,000** |

### Desglose por Rol (Estimacion mensual)

| Rol | Cantidad | Costo Unitario Estimado | Subtotal |
|---|---|---|---|
| ML Engineer | 1 | $28,000 - $35,000 | $28,000 - $35,000 |
| Backend Engineer | 2 | $22,000 - $30,000 c/u | $44,000 - $60,000 |
| Frontend Developer | 1 | $20,000 - $28,000 | $20,000 - $28,000 |
| QA Engineer | 1 | $18,000 - $22,000 | $18,000 - $22,000 |
| DevOps Engineer | 1 | $22,000 - $30,000 | $22,000 - $30,000 |
| Project Manager | 1 | $15,000 - $20,000 | $15,000 - $20,000 |

---

## 7. Equipo del Proyecto

| Rol | Responsabilidades Principales | Dedicacion |
|---|---|---|
| **Project Manager** | Coordinacion general, seguimiento, comunicacion con stakeholders, gestion de riesgos | 100% |
| **ML Engineer** | Diseno e implementacion del pipeline OCR, optimizacion de precision, validacion de modelos | 100% |
| **Backend Engineer 1** | Desarrollo API FastAPI, logica de negocio, integracion con base de datos | 100% |
| **Backend Engineer 2** | Integracion SAP/ERP, servicios de validacion, APIs de soporte | 100% |
| **Frontend Developer** | Desarrollo dashboard React, UX/UI, integracion con API | 100% |
| **QA Engineer** | Estrategia de pruebas, automatizacion, pruebas UAT, reporte de defectos | 100% |
| **DevOps Engineer** | Infraestructura GCP/K8s, CI/CD, monitoreo, seguridad | 100% |

### Stakeholders Clave

| Stakeholder | Rol | Nivel de Influencia |
|---|---|---|
| Gerencia de Operaciones | Sponsor / Cliente interno | Alto |
| Equipo de Operaciones | Usuarios finales | Alto |
| Area de TI | Soporte infraestructura, integracion SAP | Medio |
| Gerencia General | Aprobacion presupuestal | Alto |

---

## 8. Riesgos Iniciales

| # | Riesgo | Probabilidad | Impacto | Estrategia de Mitigacion |
|---|---|---|---|---|
| R1 | Precision OCR por debajo del 95% en documentos con baja calidad de imagen | Media | Alto | Implementar preprocesamiento de imagen, definir estandares minimos de calidad de escaneo |
| R2 | Retrasos en la integracion SAP/ERP por complejidad de APIs | Media | Alto | Iniciar analisis de APIs SAP en Fase 1, asignar Backend Engineer dedicado |
| R3 | Dependencia de disponibilidad de Google Cloud Vision API | Baja | Alto | Implementar circuit breaker, definir SLA con GCP, tener cache local |
| R4 | Cambios de alcance solicitados por el area de operaciones | Media | Medio | Proceso formal de control de cambios, reuniones semanales de validacion |
| R5 | Rendimiento insuficiente para volumen de documentos esperado | Baja | Medio | Pruebas de carga temprana (Fase 2), arquitectura escalable con K8s |
| R6 | Resistencia al cambio del equipo de operaciones | Media | Medio | Involucrar usuarios desde Fase 2, capacitacion progresiva |
| R7 | Presupuesto insuficiente por costos cloud variables | Baja | Medio | Monitoreo de costos semanal, alertas de presupuesto en GCP |

---

## 9. Criterios de Exito

El proyecto se considerara exitoso cuando se cumplan **todos** los siguientes criterios:

### Criterios Funcionales
- [ ] Precision de OCR >= 95% medida sobre conjunto de pruebas representativo.
- [ ] Tiempo de procesamiento promedio por documento < 30 segundos.
- [ ] Integracion SAP/ERP funcional y validada con datos reales.
- [ ] Interfaz de usuario aprobada por el equipo de operaciones (satisfaccion >= 4/5).

### Criterios Tecnicos
- [ ] Cobertura de pruebas unitarias >= 80%.
- [ ] Disponibilidad del sistema >= 99.5%.
- [ ] Documentacion tecnica completa y actualizada.
- [ ] Pipeline CI/CD funcional con despliegue automatizado.

### Criterios de Gestion
- [ ] Entrega dentro del plazo de 4 semanas (28 dias).
- [ ] Costo final dentro del rango $200,000 - $280,000 USD.
- [ ] Pruebas de aceptacion de usuario (UAT) aprobadas.
- [ ] Capacitacion al equipo de operaciones completada.

---

## 10. Supuestos y Restricciones

### Supuestos
- El equipo de operaciones proporcionara documentos de muestra para pruebas en la primera semana.
- Las APIs de SAP/ERP estaran disponibles y documentadas para integracion.
- La infraestructura de escaneo existente genera imagenes con calidad minima aceptable (>= 300 DPI).
- Los recursos del equipo estaran disponibles al 100% durante las 4 semanas del proyecto.
- Las credenciales y accesos a GCP se proporcionaran antes del inicio del proyecto.

### Restricciones
- Duracion maxima: 4 semanas (28 dias calendario).
- Presupuesto maximo: $280,000 USD.
- Stack tecnologico definido: Python 3.11, FastAPI, Google Cloud Vision, PostgreSQL, React 18, Kubernetes/GCP.
- Documentos a procesar unicamente en idioma espanol.
- Cumplimiento con politicas de seguridad de datos de la organizacion.

---

## 11. Firmas y Aprobaciones

| Rol | Nombre | Firma | Fecha |
|---|---|---|---|
| Sponsor del Proyecto | _________________ | _________________ | ____/____/________ |
| Project Manager | _________________ | _________________ | ____/____/________ |
| Gerente de TI | _________________ | _________________ | ____/____/________ |
| Lider de Operaciones | _________________ | _________________ | ____/____/________ |

> **Estado de Aprobacion:** PENDIENTE

---

## Historial de Versiones

| Version | Fecha | Autor | Descripcion del Cambio |
|---|---|---|---|
| 1.0 | 2026-02-24 | Project Manager | Creacion inicial del documento |

---

*Documento generado como parte de la gestion del proyecto PRY-04-OCR - OCR Operativo.*
