# DECLARACION DE ALCANCE DEL PROYECTO

| Campo | Detalle |
|---|---|
| **Proyecto** | OCR Operativo - Sistema de Digitalizacion de Registros Operacionales |
| **Codigo** | PRY-04-OCR |
| **Version** | 1.0 |
| **Fecha de Creacion** | 2026-02-24 |
| **Ultima Actualizacion** | 2026-02-24 |
| **Estado** | Borrador |
| **Referencia** | Acta de Constitucion v1.0 |
| **Preparado por** | Project Manager - IDC Ingenieria |

---

## 1. Descripcion del Producto

### 1.1 Vision General

El Sistema OCR Operativo es una solucion de software integral que automatiza la digitalizacion de registros operacionales fisicos o semi-digitales, transformandolos en datos estructurados con una precision minima del 95%. El sistema se compone de:

- **Motor de procesamiento OCR** basado en Google Cloud Vision para extraccion de texto.
- **API REST** desarrollada en FastAPI (Python 3.11) como capa de logica de negocio.
- **Base de datos relacional** PostgreSQL para persistencia de registros procesados.
- **Interfaz web** en React 18 para interaccion de operadores con el sistema.
- **Modulo de integracion** con SAP/ERP para sincronizacion de datos.
- **Infraestructura cloud** en GCP con orquestacion Kubernetes.

### 1.2 Flujo Principal del Sistema

```
[Documento Fisico] --> [Escaneo] --> [Carga al Sistema (React)]
       |
       v
[API FastAPI] --> [Preprocesamiento de Imagen]
       |
       v
[Google Cloud Vision] --> [Extraccion de Texto]
       |
       v
[Validacion y Estructuracion] --> [PostgreSQL]
       |
       v
[Revision Operador (React)] --> [Correccion Manual si aplica]
       |
       v
[Envio a SAP/ERP] --> [Confirmacion]
```

### 1.3 Tipos de Documentos Soportados

| Tipo | Formato | Prioridad |
|---|---|---|
| Registros de produccion | PDF, JPG, PNG | Alta |
| Ordenes de trabajo | PDF, JPG, PNG | Alta |
| Reportes operacionales diarios | PDF | Alta |
| Formularios de inspeccion | PDF, JPG, PNG | Media |
| Registros de inventario | PDF | Media |

---

## 2. Entregables del Proyecto

### 2.1 Entregables de Software

| # | Entregable | Descripcion | Criterio de Aceptacion |
|---|---|---|---|
| ES-01 | Motor OCR | Pipeline de procesamiento con GCV, preprocesamiento de imagen, extraccion y estructuracion de datos | Precision >= 95% en dataset de prueba |
| ES-02 | API REST | Servicio FastAPI con endpoints para carga, procesamiento, consulta y gestion de documentos | Documentacion OpenAPI completa, tests > 80% cobertura |
| ES-03 | Modelo de datos | Esquema PostgreSQL con tablas, indices, relaciones y migraciones | Esquema revisado y aprobado, migraciones reproducibles |
| ES-04 | Frontend web | Dashboard React 18 con flujos de carga, revision, correccion y reportes | Usabilidad aprobada por operaciones, responsive |
| ES-05 | Modulo SAP/ERP | Servicio de integracion bidireccional con SAP | Mapeo 100% de campos validado, sync exitoso |
| ES-06 | Pipeline CI/CD | Configuracion de build, test y deploy automatizado | Deploy automatico a staging y produccion |

### 2.2 Entregables de Infraestructura

| # | Entregable | Descripcion | Criterio de Aceptacion |
|---|---|---|---|
| EI-01 | Cluster Kubernetes | Configuracion K8s en GCP con namespaces, autoscaling y networking | Alta disponibilidad >= 99.5% |
| EI-02 | Configuracion GCP | Proyecto GCP con servicios habilitados, IAM, networking | Seguridad y accesos validados |
| EI-03 | Monitoreo y alertas | Dashboard de monitoreo, logs centralizados, alertas configuradas | Cobertura de metricas criticas |

### 2.3 Entregables de Documentacion

| # | Entregable | Descripcion |
|---|---|---|
| ED-01 | Documentacion tecnica | Arquitectura, diagramas, guias de configuracion |
| ED-02 | Manual de usuario | Guia paso a paso para operadores |
| ED-03 | Guia de operacion | Procedimientos de soporte, troubleshooting, runbooks |
| ED-04 | Documentacion API | Swagger/OpenAPI generada automaticamente |

### 2.4 Entregables de Gestion

| # | Entregable | Descripcion |
|---|---|---|
| EG-01 | Acta de constitucion | Este documento y sus actualizaciones |
| EG-02 | Reportes de avance | Reportes semanales de progreso |
| EG-03 | Acta de cierre | Documento de cierre formal del proyecto |

---

## 3. Criterios de Aceptacion

### 3.1 Criterios Funcionales

| # | Criterio | Metrica | Umbral |
|---|---|---|---|
| CA-01 | Precision de OCR | % de caracteres correctamente reconocidos | >= 95% |
| CA-02 | Tiempo de procesamiento | Segundos por documento promedio | < 30 seg |
| CA-03 | Throughput | Documentos procesados por minuto | >= 50 docs/min |
| CA-04 | Integracion SAP | % de campos mapeados correctamente | 100% |
| CA-05 | Satisfaccion de usuario | Escala Likert 1-5 | >= 4.0 |

### 3.2 Criterios No Funcionales

| # | Criterio | Metrica | Umbral |
|---|---|---|---|
| CA-06 | Disponibilidad | Uptime del sistema | >= 99.5% |
| CA-07 | Tiempo de carga UI | Carga inicial del dashboard | < 2 seg |
| CA-08 | Seguridad | Vulnerabilidades criticas | 0 |
| CA-09 | Cobertura de pruebas | % de cobertura unitaria | >= 80% |
| CA-10 | Escalabilidad | Procesamiento bajo carga pico | Sin degradacion > 20% |

### 3.3 Proceso de Aceptacion

1. El equipo de QA ejecuta la suite completa de pruebas y genera reporte.
2. El equipo de operaciones realiza pruebas de aceptacion de usuario (UAT) durante 3 dias habiles.
3. Se documentan y clasifican los defectos encontrados (critico, mayor, menor).
4. **Criterio de aceptacion**: Cero defectos criticos, maximo 3 defectos mayores con plan de remediacion.
5. El sponsor firma el acta de aceptacion formal.

---

## 4. Exclusiones Explicitas

Los siguientes elementos **NO** forman parte del alcance de este proyecto:

| # | Exclusion | Justificacion |
|---|---|---|
| EX-01 | Digitalizacion de documentos historicos (backlog) | Requiere un proyecto separado de migracion |
| EX-02 | Aplicacion movil nativa (iOS/Android) | No requerida en esta fase; la interfaz web es responsive |
| EX-03 | Integracion con sistemas distintos a SAP/ERP | Solo se contempla SAP/ERP como sistema destino |
| EX-04 | Adquisicion de hardware de escaneo | Se asume que existe infraestructura de escaneo |
| EX-05 | Entrenamiento de modelos ML personalizados | Se utiliza Google Cloud Vision como servicio gestionado |
| EX-06 | Procesamiento de documentos en idiomas distintos al espanol | El alcance se limita a documentos en espanol |
| EX-07 | Soporte post-produccion extendido | Se ofrece soporte maximo de 2 semanas post go-live |
| EX-08 | Migracion de datos legacy | No se contempla carga de datos historicos |
| EX-09 | Reconocimiento de escritura manual (handwriting) | Solo texto impreso y formularios estructurados |
| EX-10 | Alta disponibilidad multi-region | Despliegue en una sola region GCP |

---

## 5. Restricciones

### 5.1 Restricciones de Tiempo
- El proyecto debe completarse en un maximo de **4 semanas (28 dias calendario)**.
- Las entregas parciales deben cumplir con los hitos definidos por fase semanal.

### 5.2 Restricciones de Presupuesto
- Presupuesto total: **$200,000 - $280,000 USD**.
- No se autoriza gasto adicional sin aprobacion formal del sponsor.
- Los costos de cloud (GCP) deben monitorearse semanalmente.

### 5.3 Restricciones Tecnologicas
- **Lenguaje backend**: Python 3.11 exclusivamente.
- **Framework API**: FastAPI.
- **Servicio OCR**: Google Cloud Vision API.
- **Base de datos**: PostgreSQL (version 15+).
- **Frontend**: React 18 con TypeScript.
- **Infraestructura**: Google Cloud Platform con Kubernetes.
- **Integracion**: SAP/ERP mediante APIs REST o RFC existentes.

### 5.4 Restricciones Organizacionales
- El equipo de operaciones debe estar disponible para sesiones de validacion al menos 2 horas semanales.
- Los accesos a SAP/ERP para desarrollo/pruebas deben gestionarse a traves del area de TI.
- Todos los datos operacionales deben manejarse conforme a las politicas internas de seguridad de la informacion.

---

## 6. Supuestos

| # | Supuesto | Impacto si no se cumple |
|---|---|---|
| SU-01 | El equipo de operaciones proporcionara al menos 200 documentos de muestra en la primera semana | Retraso en entrenamiento y calibracion del sistema |
| SU-02 | Las APIs de SAP/ERP estan disponibles, documentadas y accesibles desde ambiente de desarrollo | Retraso en la Fase 3 (integracion) |
| SU-03 | La infraestructura de escaneo existente produce imagenes de >= 300 DPI | Precision de OCR podria ser inferior al 95% |
| SU-04 | Los 7 miembros del equipo estaran dedicados al 100% durante las 4 semanas | Extension del cronograma |
| SU-05 | Los accesos y credenciales de GCP se proporcionaran antes del inicio del proyecto | Retraso en configuracion de infraestructura |
| SU-06 | Los formatos de documentos operacionales son estandarizados y consistentes | Mayor esfuerzo en logica de parseo y validacion |
| SU-07 | El ambiente de staging estara disponible desde la Semana 2 | Retraso en pruebas de integracion |
| SU-08 | No existen restricciones de red que impidan la comunicacion GCP - SAP | Rediseno de arquitectura de integracion |
| SU-09 | El sponsor esta disponible para decisiones criticas con respuesta < 24 horas | Bloqueos en aprobaciones |

---

## 7. WBS de Alto Nivel (Estructura de Desglose del Trabajo)

```
PRY-04-OCR: OCR Operativo
|
|-- 1.0 GESTION DEL PROYECTO
|   |-- 1.1 Planificacion y kick-off
|   |-- 1.2 Seguimiento semanal
|   |-- 1.3 Gestion de riesgos
|   |-- 1.4 Control de cambios
|   |-- 1.5 Cierre del proyecto
|
|-- 2.0 FASE 1 - FUNDAMENTOS (Semana 1)
|   |-- 2.1 Configuracion del entorno de desarrollo
|   |-- 2.2 Diseno de arquitectura del sistema
|   |-- 2.3 Implementacion del motor OCR base
|   |   |-- 2.3.1 Integracion con Google Cloud Vision API
|   |   |-- 2.3.2 Pipeline de preprocesamiento de imagenes
|   |   |-- 2.3.3 Modulo de extraccion y estructuracion de texto
|   |-- 2.4 Diseno e implementacion del esquema de base de datos
|   |-- 2.5 Configuracion inicial de infraestructura GCP
|   |-- 2.6 Validacion de precision con dataset de prueba
|
|-- 3.0 FASE 2 - DESARROLLO CORE (Semana 2)
|   |-- 3.1 Desarrollo API REST (FastAPI)
|   |   |-- 3.1.1 Endpoints de ingesta de documentos
|   |   |-- 3.1.2 Endpoints de consulta y gestion
|   |   |-- 3.1.3 Logica de validacion y reglas de negocio
|   |   |-- 3.1.4 Autenticacion y autorizacion
|   |-- 3.2 Desarrollo Frontend (React 18)
|   |   |-- 3.2.1 Componente de carga de documentos
|   |   |-- 3.2.2 Vista de resultados OCR
|   |   |-- 3.2.3 Interfaz de correccion manual
|   |   |-- 3.2.4 Dashboard de metricas basico
|   |-- 3.3 Pruebas unitarias backend y frontend
|
|-- 4.0 FASE 3 - INTEGRACION (Semana 3)
|   |-- 4.1 Integracion SAP/ERP
|   |   |-- 4.1.1 Mapeo de campos OCR a campos SAP
|   |   |-- 4.1.2 Desarrollo del modulo de sincronizacion
|   |   |-- 4.1.3 Validacion de datos antes de envio
|   |   |-- 4.1.4 Manejo de errores y reintentos
|   |-- 4.2 Integracion end-to-end del sistema
|   |-- 4.3 Pruebas de integracion
|   |-- 4.4 Pruebas de rendimiento y carga
|   |-- 4.5 Configuracion de monitoreo y alertas
|
|-- 5.0 FASE 4 - PRODUCCION Y CIERRE (Semana 4)
|   |-- 5.1 Pruebas de aceptacion de usuario (UAT)
|   |-- 5.2 Correccion de defectos criticos
|   |-- 5.3 Despliegue a produccion
|   |-- 5.4 Validacion en produccion (smoke tests)
|   |-- 5.5 Documentacion final
|   |   |-- 5.5.1 Documentacion tecnica
|   |   |-- 5.5.2 Manual de usuario
|   |   |-- 5.5.3 Guia de operacion
|   |-- 5.6 Capacitacion al equipo de operaciones
|   |-- 5.7 Cierre formal del proyecto
```

### Diccionario WBS (Paquetes Principales)

| Codigo | Paquete | Responsable | Esfuerzo Estimado |
|---|---|---|---|
| 1.0 | Gestion del proyecto | Project Manager | Continuo (4 semanas) |
| 2.0 | Fundamentos | ML Engineer + DevOps | 5 dias-persona |
| 3.0 | Desarrollo Core | Backend Eng. + Frontend Dev | 10 dias-persona |
| 4.0 | Integracion | Backend Eng. 2 + QA | 8 dias-persona |
| 5.0 | Produccion y Cierre | Todo el equipo | 7 dias-persona |

---

## Historial de Versiones

| Version | Fecha | Autor | Descripcion del Cambio |
|---|---|---|---|
| 1.0 | 2026-02-24 | Project Manager | Creacion inicial del documento |

---

*Documento generado como parte de la gestion del proyecto PRY-04-OCR - OCR Operativo.*
