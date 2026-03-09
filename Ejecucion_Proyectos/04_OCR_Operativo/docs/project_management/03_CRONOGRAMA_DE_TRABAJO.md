# CRONOGRAMA DE TRABAJO

| Campo | Detalle |
|---|---|
| **Proyecto** | OCR Operativo - Sistema de Digitalizacion de Registros Operacionales |
| **Codigo** | PRY-04-OCR |
| **Version** | 1.0 |
| **Fecha de Creacion** | 2026-02-24 |
| **Ultima Actualizacion** | 2026-02-24 |
| **Duracion Total** | 4 semanas (28 dias calendario) |
| **Estado** | Borrador |
| **Preparado por** | Project Manager - IDC Ingenieria |

---

## 1. Fases del Proyecto

### Fase 1 - Fundamentos (Semana 1: Dias 1-7)

| ID | Tarea | Duracion | Inicio | Fin | Responsable | Dependencia |
|---|---|---|---|---|---|---|
| 1.1 | Kick-off y alineacion del equipo | 1 dia | Dia 1 | Dia 1 | PM | - |
| 1.2 | Configuracion entorno de desarrollo | 1 dia | Dia 1 | Dia 1 | DevOps | - |
| 1.3 | Diseno de arquitectura del sistema | 2 dias | Dia 1 | Dia 2 | ML Eng + Backend 1 | - |
| 1.4 | Configuracion proyecto GCP | 1 dia | Dia 2 | Dia 2 | DevOps | 1.2 |
| 1.5 | Integracion Google Cloud Vision API | 2 dias | Dia 2 | Dia 3 | ML Engineer | 1.2, 1.4 |
| 1.6 | Pipeline preprocesamiento de imagenes | 2 dias | Dia 3 | Dia 4 | ML Engineer | 1.5 |
| 1.7 | Diseno esquema base de datos PostgreSQL | 2 dias | Dia 2 | Dia 3 | Backend 1 | 1.3 |
| 1.8 | Implementacion modelos de datos y migraciones | 2 dias | Dia 4 | Dia 5 | Backend 1 | 1.7 |
| 1.9 | Modulo extraccion y estructuracion de texto | 2 dias | Dia 5 | Dia 6 | ML Engineer | 1.6 |
| 1.10 | Analisis APIs SAP/ERP | 2 dias | Dia 2 | Dia 3 | Backend 2 | 1.1 |
| 1.11 | Setup proyecto frontend React 18 | 1 dia | Dia 3 | Dia 3 | Frontend Dev | 1.3 |
| 1.12 | Configuracion pipeline CI/CD base | 2 dias | Dia 3 | Dia 4 | DevOps | 1.4 |
| 1.13 | Validacion precision con dataset prueba | 1 dia | Dia 7 | Dia 7 | ML Eng + QA | 1.9 |
| **H1** | **HITO: Motor OCR funcional (>= 90% precision)** | - | - | **Dia 7** | **Todos** | **1.13** |

### Fase 2 - Desarrollo Core (Semana 2: Dias 8-14)

| ID | Tarea | Duracion | Inicio | Fin | Responsable | Dependencia |
|---|---|---|---|---|---|---|
| 2.1 | Endpoints API ingesta de documentos | 2 dias | Dia 8 | Dia 9 | Backend 1 | 1.8 |
| 2.2 | Endpoints API consulta y gestion | 2 dias | Dia 10 | Dia 11 | Backend 1 | 2.1 |
| 2.3 | Logica de validacion y reglas de negocio | 2 dias | Dia 10 | Dia 11 | Backend 2 | 2.1 |
| 2.4 | Autenticacion y autorizacion API | 2 dias | Dia 8 | Dia 9 | Backend 2 | 1.8 |
| 2.5 | Componente carga de documentos (React) | 2 dias | Dia 8 | Dia 9 | Frontend Dev | 1.11 |
| 2.6 | Vista resultados OCR (React) | 2 dias | Dia 10 | Dia 11 | Frontend Dev | 2.5, 2.1 |
| 2.7 | Interfaz correccion manual (React) | 2 dias | Dia 12 | Dia 13 | Frontend Dev | 2.6 |
| 2.8 | Dashboard metricas basico (React) | 1 dia | Dia 14 | Dia 14 | Frontend Dev | 2.7 |
| 2.9 | Pruebas unitarias backend | 2 dias | Dia 12 | Dia 13 | QA + Backend 1 | 2.2, 2.3 |
| 2.10 | Pruebas unitarias frontend | 1 dia | Dia 14 | Dia 14 | QA | 2.7 |
| 2.11 | Deploy a ambiente staging | 1 dia | Dia 12 | Dia 12 | DevOps | 2.1, 1.12 |
| 2.12 | Optimizacion precision OCR | 2 dias | Dia 8 | Dia 9 | ML Engineer | H1 |
| **H2** | **HITO: API y Frontend funcionales en staging** | - | - | **Dia 14** | **Todos** | **2.9, 2.10** |

### Fase 3 - Integracion (Semana 3: Dias 15-21)

| ID | Tarea | Duracion | Inicio | Fin | Responsable | Dependencia |
|---|---|---|---|---|---|---|
| 3.1 | Mapeo campos OCR a campos SAP | 2 dias | Dia 15 | Dia 16 | Backend 2 | 1.10, H2 |
| 3.2 | Desarrollo modulo sincronizacion SAP | 3 dias | Dia 16 | Dia 18 | Backend 2 | 3.1 |
| 3.3 | Validacion de datos pre-envio a SAP | 1 dia | Dia 19 | Dia 19 | Backend 2 | 3.2 |
| 3.4 | Manejo de errores y reintentos SAP | 1 dia | Dia 20 | Dia 20 | Backend 2 | 3.3 |
| 3.5 | Integracion end-to-end del sistema | 2 dias | Dia 17 | Dia 18 | Backend 1 + ML Eng | H2 |
| 3.6 | Pruebas de integracion completas | 2 dias | Dia 19 | Dia 20 | QA | 3.5, 3.4 |
| 3.7 | Pruebas de rendimiento y carga | 2 dias | Dia 19 | Dia 20 | QA + DevOps | 3.5 |
| 3.8 | Configuracion monitoreo y alertas | 2 dias | Dia 15 | Dia 16 | DevOps | 2.11 |
| 3.9 | Ajustes de UI por feedback operaciones | 2 dias | Dia 15 | Dia 16 | Frontend Dev | H2 |
| 3.10 | Correccion de defectos encontrados | 2 dias | Dia 20 | Dia 21 | Backend 1 + Frontend | 3.6 |
| 3.11 | Preparacion ambiente produccion | 2 dias | Dia 17 | Dia 18 | DevOps | 3.8 |
| **H3** | **HITO: Integracion SAP validada, UAT iniciadas** | - | - | **Dia 21** | **Todos** | **3.6, 3.7** |

### Fase 4 - Produccion y Cierre (Semana 4: Dias 22-28)

| ID | Tarea | Duracion | Inicio | Fin | Responsable | Dependencia |
|---|---|---|---|---|---|---|
| 4.1 | Pruebas de aceptacion de usuario (UAT) | 3 dias | Dia 22 | Dia 24 | QA + Operaciones | H3 |
| 4.2 | Correccion defectos criticos UAT | 2 dias | Dia 23 | Dia 24 | Backend + Frontend | 4.1 (parcial) |
| 4.3 | Despliegue a produccion | 1 dia | Dia 25 | Dia 25 | DevOps | 4.1, 4.2 |
| 4.4 | Smoke tests en produccion | 1 dia | Dia 25 | Dia 25 | QA | 4.3 |
| 4.5 | Documentacion tecnica final | 3 dias | Dia 22 | Dia 24 | ML Eng + Backend 1 | H3 |
| 4.6 | Manual de usuario | 2 dias | Dia 22 | Dia 23 | Frontend Dev | H3 |
| 4.7 | Guia de operacion y runbooks | 2 dias | Dia 24 | Dia 25 | DevOps | 3.8 |
| 4.8 | Capacitacion equipo operaciones | 2 dias | Dia 26 | Dia 27 | PM + Frontend Dev | 4.3, 4.6 |
| 4.9 | Validacion final en produccion | 1 dia | Dia 27 | Dia 27 | QA + Operaciones | 4.4 |
| 4.10 | Lecciones aprendidas y retrospectiva | 1 dia | Dia 28 | Dia 28 | PM + Todos | 4.9 |
| 4.11 | Cierre formal del proyecto | 1 dia | Dia 28 | Dia 28 | PM | 4.10 |
| **H4** | **HITO: Go-Live y cierre del proyecto** | - | - | **Dia 28** | **Todos** | **4.11** |

---

## 2. Diagrama de Gantt (ASCII)

```
TAREA / RECURSO              | S1-D1 | S1-D2 | S1-D3 | S1-D4 | S1-D5 | S1-D6 | S1-D7 | S2-D8 | S2-D9 | S2-D10| S2-D11| S2-D12| S2-D13| S2-D14| S3-D15| S3-D16| S3-D17| S3-D18| S3-D19| S3-D20| S3-D21| S4-D22| S4-D23| S4-D24| S4-D25| S4-D26| S4-D27| S4-D28|
------------------------------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|-------|
GESTION DEL PROYECTO (PM)     | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx | xxxxx |
                              |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
--- FASE 1: FUNDAMENTOS ---   |=======|=======|=======|=======|=======|=======|=======|       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Kick-off                      | ##    |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Config entorno dev (DevOps)   | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Diseno arquitectura           | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Config GCP (DevOps)           |       | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Integ. GCV API (ML Eng)       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Preproc. imagenes (ML Eng)    |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Esquema BD (Backend 1)        |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Modelos datos (Backend 1)     |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Extraccion texto (ML Eng)     |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Analisis APIs SAP (Backend 2) |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Setup React (Frontend)        |       |       | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
CI/CD base (DevOps)           |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Validacion precision          |       |       |       |       |       |       | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
>>> HITO H1 <<<               |       |       |       |       |       |       | ***** |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
                              |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
--- FASE 2: DESARROLLO ---    |       |       |       |       |       |       |       |=======|=======|=======|=======|=======|=======|=======|       |       |       |       |       |       |       |       |       |       |       |       |       |       |
API ingesta (Backend 1)       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
API consulta (Backend 1)      |       |       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Validacion/reglas (Backend 2) |       |       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Auth API (Backend 2)          |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Carga docs (Frontend)         |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Vista resultados (Frontend)   |       |       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Correccion manual (Frontend)  |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Dashboard metricas (Frontend) |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Tests backend (QA)            |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Tests frontend (QA)           |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Deploy staging (DevOps)       |       |       |       |       |       |       |       |       |       |       |       | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
Optim. precision (ML Eng)     |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
>>> HITO H2 <<<               |       |       |       |       |       |       |       |       |       |       |       |       |       | ***** |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
                              |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
--- FASE 3: INTEGRACION ---   |       |       |       |       |       |       |       |       |       |       |       |       |       |       |=======|=======|=======|=======|=======|=======|=======|       |       |       |       |       |       |       |
Mapeo campos SAP (Backend 2)  |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |
Modulo sync SAP (Backend 2)   |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### | ##### |       |       |       |       |       |       |       |       |       |       |
Valid. pre-envio (Backend 2)  |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### |       |       |       |       |       |       |       |       |       |
Errores/reintentos (Backend2) |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### |       |       |       |       |       |       |       |       |
Integ. E2E (Backend1 + ML)    |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |
Tests integracion (QA)        |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |
Tests carga (QA + DevOps)     |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |
Monitoreo/alertas (DevOps)    |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |
Ajustes UI (Frontend)         |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |       |       |
Correccion defectos           |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |
Prep. ambiente prod (DevOps)  |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |       |       |       |       |       |
>>> HITO H3 <<<               |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ***** |       |       |       |       |       |       |       |
                              |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |
--- FASE 4: PRODUCCION ---    |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |=======|=======|=======|=======|=======|=======|=======|
UAT (QA + Operaciones)        |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### | ##### |       |       |       |       |
Correc. defectos UAT          |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |
Deploy produccion (DevOps)    |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### |       |       |       |
Smoke tests (QA)              |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### |       |       |       |
Doc. tecnica (ML + Backend)   |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### | ##### |       |       |       |       |
Manual usuario (Frontend)     |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |       |       |
Guia operacion (DevOps)       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### |       |       |       |
Capacitacion (PM + Frontend)  |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### | ##### |       |
Valid. final prod (QA + Ops)  |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### |       |
Lecciones/retrospectiva       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### |
Cierre formal                 |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ##### |
>>> HITO H4 (GO-LIVE) <<<    |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       |       | ***** |

LEYENDA:  ##### = Actividad en progreso  |  ***** = Hito  |  ======= = Rango de fase  |  xxxxx = Actividad continua
```

---

## 3. Hitos con Fechas

| # | Hito | Dia | Semana | Criterio de Cumplimiento | Estado |
|---|---|---|---|---|---|
| H1 | Motor OCR funcional | Dia 7 | Fin Semana 1 | OCR procesando con >= 90% precision en dev | Pendiente |
| H2 | API y Frontend en staging | Dia 14 | Fin Semana 2 | Flujos principales funcionales en staging | Pendiente |
| H3 | Integracion SAP validada | Dia 21 | Fin Semana 3 | E2E funcional, UAT iniciadas | Pendiente |
| H4 | Go-Live y cierre | Dia 28 | Fin Semana 4 | Produccion activa, capacitacion completada | Pendiente |

### Hitos Internos Adicionales

| # | Hito Interno | Dia | Responsable |
|---|---|---|---|
| HI-01 | Arquitectura aprobada | Dia 2 | ML Engineer |
| HI-02 | Esquema BD aprobado | Dia 3 | Backend 1 |
| HI-03 | Primer documento procesado E2E | Dia 10 | ML Eng + Backend 1 |
| HI-04 | Ambiente staging disponible | Dia 12 | DevOps |
| HI-05 | Integracion SAP primera transaccion | Dia 18 | Backend 2 |
| HI-06 | Cobertura tests >= 80% | Dia 20 | QA |
| HI-07 | UAT aprobadas | Dia 24 | QA + Operaciones |
| HI-08 | Deploy produccion exitoso | Dia 25 | DevOps |

---

## 4. Dependencias

### 4.1 Mapa de Dependencias Criticas

```
1.1 Kick-off
  |
  +---> 1.2 Config entorno --> 1.4 Config GCP --> 1.5 Integ. GCV
  |                                                    |
  +---> 1.3 Arquitectura --+--> 1.7 Esquema BD        v
  |                        |         |           1.6 Preproc. imagen
  |                        |         v                 |
  |                        |    1.8 Modelos datos      v
  |                        |         |           1.9 Extraccion texto
  |                        |         |                 |
  |                        +---> 1.11 Setup React      v
  |                                  |           1.13 Validacion (H1)
  +---> 1.10 Analisis SAP           |                 |
             |                       v                 v
             |              2.5 Carga docs      2.1 API ingesta
             |                   |                   |
             |                   v                   v
             |              2.6 Vista result.   2.2 API consulta
             |                   |                   |
             |                   v                   v
             |              2.7 Correccion      2.9 Tests backend (H2)
             |                   |                   |
             v                   v                   v
        3.1 Mapeo SAP      3.9 Ajustes UI      3.5 Integ. E2E
             |                                       |
             v                                       v
        3.2 Sync SAP -----> 3.3 Valid. -----> 3.6 Tests integ. (H3)
                                                     |
                                                     v
                                              4.1 UAT
                                                     |
                                                     v
                                              4.3 Deploy prod (H4)
```

### 4.2 Dependencias Externas

| # | Dependencia | Proveedor | Fase | Impacto si no se cumple |
|---|---|---|---|---|
| DE-01 | Documentos de muestra (200+) | Equipo Operaciones | Fase 1 | Bloqueo en calibracion OCR |
| DE-02 | Credenciales GCP | Area de TI | Fase 1 | Bloqueo en infraestructura |
| DE-03 | Acceso APIs SAP/ERP | Area de TI | Fase 1-3 | Bloqueo en integracion |
| DE-04 | Disponibilidad usuarios UAT | Equipo Operaciones | Fase 4 | Retraso en aceptacion |
| DE-05 | Aprobacion sponsor para go-live | Gerencia | Fase 4 | Retraso en despliegue |

---

## 5. Ruta Critica

La ruta critica del proyecto es la secuencia mas larga de tareas dependientes que determina la duracion minima del proyecto.

### Ruta Critica Identificada

```
1.1 Kick-off (D1)
  --> 1.2 Config entorno (D1)
    --> 1.4 Config GCP (D2)
      --> 1.5 Integ. GCV (D2-D3)
        --> 1.6 Preproc. imagen (D3-D4)
          --> 1.9 Extraccion texto (D5-D6)
            --> 1.13 Validacion [H1] (D7)
              --> 2.1 API ingesta (D8-D9)
                --> 2.2 API consulta (D10-D11)
                  --> 2.9 Tests backend [H2] (D12-D13)
                    --> 3.5 Integ. E2E (D17-D18)
                      --> 3.6 Tests integracion (D19-D20) [H3]
                        --> 4.1 UAT (D22-D24)
                          --> 4.3 Deploy produccion (D25)
                            --> 4.8 Capacitacion (D26-D27) [H4]
```

**Duracion ruta critica: 28 dias (sin holgura)**

### Tareas con Holgura

| Tarea | Holgura (dias) | Observacion |
|---|---|---|
| 1.10 Analisis APIs SAP | 12 dias | Puede completarse hasta Dia 15 sin impacto |
| 1.11 Setup React | 5 dias | Puede completarse hasta Dia 8 sin impacto |
| 3.8 Monitoreo/alertas | 9 dias | Puede completarse hasta Dia 25 sin impacto |
| 3.9 Ajustes UI | 5 dias | Puede completarse hasta Dia 21 sin impacto |
| 4.5 Doc. tecnica | 1 dia | Holgura minima, vigilar |

---

## 6. Recursos por Fase

### Asignacion de Recursos

| Recurso | Fase 1 | Fase 2 | Fase 3 | Fase 4 |
|---|---|---|---|---|
| **Project Manager** | Planificacion, kick-off, seguimiento | Seguimiento, gestion riesgos | Seguimiento, validacion | UAT, capacitacion, cierre |
| **ML Engineer** | GCV, preprocesamiento, extraccion | Optimizacion precision | Integracion E2E | Documentacion tecnica |
| **Backend Engineer 1** | Esquema BD, modelos de datos | API REST (ingesta, consulta) | Integracion E2E | Documentacion, correcciones |
| **Backend Engineer 2** | Analisis APIs SAP | Validacion, reglas, auth | Integracion SAP completa | Correcciones, soporte UAT |
| **Frontend Developer** | Setup React | Componentes UI completos | Ajustes por feedback | Manual usuario, capacitacion |
| **QA Engineer** | Validacion precision | Tests unitarios | Tests integracion, carga | UAT, smoke tests, validacion |
| **DevOps Engineer** | Config entorno, GCP, CI/CD | Deploy staging | Monitoreo, amb. produccion | Deploy produccion, runbooks |

### Carga de Trabajo Semanal (horas estimadas por recurso)

| Recurso | Semana 1 | Semana 2 | Semana 3 | Semana 4 | Total |
|---|---|---|---|---|---|
| Project Manager | 40h | 40h | 40h | 40h | 160h |
| ML Engineer | 45h | 35h | 35h | 30h | 145h |
| Backend Engineer 1 | 40h | 45h | 40h | 30h | 155h |
| Backend Engineer 2 | 30h | 40h | 45h | 30h | 145h |
| Frontend Developer | 20h | 45h | 30h | 35h | 130h |
| QA Engineer | 15h | 35h | 45h | 45h | 140h |
| DevOps Engineer | 40h | 25h | 35h | 35h | 135h |
| **Total equipo** | **230h** | **265h** | **270h** | **245h** | **1,010h** |

---

## 7. Reuniones de Seguimiento

| Reunion | Frecuencia | Duracion | Participantes | Dia/Hora |
|---|---|---|---|---|
| Daily standup | Diaria | 15 min | Todo el equipo | Cada dia, 9:00 AM |
| Revision de fase | Semanal | 1 hora | Todo el equipo + Sponsor | Viernes, 3:00 PM |
| Demo a stakeholders | Bi-semanal | 30 min | PM + Sponsor + Operaciones | Dia 14 y Dia 28 |
| Retrospectiva | Al cierre | 1.5 horas | Todo el equipo | Dia 28 |

---

## Historial de Versiones

| Version | Fecha | Autor | Descripcion del Cambio |
|---|---|---|---|
| 1.0 | 2026-02-24 | Project Manager | Creacion inicial del documento |

---

*Documento generado como parte de la gestion del proyecto PRY-04-OCR - OCR Operativo.*
