# 02 - Plan de Testeo
## OCR Operativo - Reconocimiento Optico de Caracteres

**Proyecto:** OCR Operativo
**Version:** 1.0
**Fecha de Creacion:** 24 de Febrero de 2026
**Responsable QA:** Por asignar
**Aprobado por:** Pendiente
**Estado:** Borrador

---

## Tabla de Contenidos

1. [Introduccion](#1-introduccion)
2. [Estrategia de Pruebas](#2-estrategia-de-pruebas)
3. [Criterios de Entrada, Salida y Suspension](#3-criterios-de-entrada-salida-y-suspension)
4. [Ambiente de Pruebas](#4-ambiente-de-pruebas)
5. [Datasets de Prueba](#5-datasets-de-prueba)
6. [Planificacion](#6-planificacion)
7. [Gestion de Defectos](#7-gestion-de-defectos)
8. [Riesgos de Testing](#8-riesgos-de-testing)
9. [Entregables](#9-entregables)
10. [Aprobaciones](#10-aprobaciones)

---

## 1. Introduccion

### 1.1 Proposito

Este documento define la estrategia, alcance, recursos y cronograma de las actividades de prueba para el proyecto OCR Operativo. El objetivo es verificar y validar que el sistema cumple con todos los requerimientos funcionales y no funcionales establecidos, garantizando un nivel de calidad apto para produccion.

El sistema OCR Operativo procesa documentos operativos (facturas, recibos, ordenes de compra, bitacoras) mediante reconocimiento optico de caracteres, con integracion directa al sistema ERP SAP para automatizar la captura de datos.

### 1.2 Alcance

Las pruebas cubren los siguientes componentes y funcionalidades:

**En alcance:**
- API REST (FastAPI/Python 3.11) - Endpoints de carga, procesamiento, consulta y administracion
- Motor OCR primario (Google Cloud Vision) y motor de fallback (Tesseract)
- Pipeline de preprocesamiento de imagenes
- Modelo de clasificacion de documentos (ML)
- Validacion y post-procesamiento de datos extraidos
- Integracion bidireccional con SAP/ERP via OData connector
- Cola y flujo de revision manual (UI React 18)
- Base de datos PostgreSQL 15 (esquemas, consultas, integridad)
- Seguridad (OAuth 2.0, AES-256, TLS 1.3, RBAC)
- Rendimiento y escalabilidad en Kubernetes/GCP
- Monitoreo y alertas (Prometheus + Grafana)

**Fuera de alcance:**
- Pruebas del sistema SAP interno (se asume funcional; se valida solo la integracion)
- Pruebas de infraestructura de red GCP (responsabilidad de Google)
- Pruebas de hardware de escaneo de documentos
- Pruebas de carga en produccion real (solo staging)
- Pruebas de sistema operativo o navegador en versiones legacy (IE, Edge Legacy)
- Desarrollo de nuevas funcionalidades no incluidas en el alcance del proyecto

### 1.3 Documentos de Referencia

| Documento | Version | Ubicacion |
|-----------|---------|-----------|
| 00_VISION_Y_GOBERNANZA.md | 1.0 | /docs/ |
| 01_ESTRUCTURA_ARQUITECTURA_TECNICA.md | 1.0 | /docs/ |
| 02_PLAN_DE_TRABAJO_DETALLADO.md | 1.0 | /docs/ |
| 03_GUIA_IMPLEMENTACION_PASO_A_PASO.md | 1.0 | /docs/ |
| 04_CRITERIOS_DE_EXITO_Y_CHECKLIST_MILESTONES.md | 1.0 | /docs/ |
| 05_METODOLOGIA_Y_ABORDAJE_ESTRATEGICO.md | 1.0 | /docs/ |
| 01_Casos_Test.md | 1.0 | /docs/testing/ |

---

## 2. Estrategia de Pruebas

### 2.1 Niveles de Prueba

#### 2.1.1 Pruebas Unitarias

| Aspecto | Detalle |
|---------|---------|
| **Objetivo** | Verificar el comportamiento correcto de funciones, metodos y clases individuales |
| **Responsable** | Equipo de desarrollo |
| **Herramientas** | pytest (Python), Jest (React), coverage.py |
| **Cobertura minima** | 80% de cobertura de codigo |
| **Automatizacion** | 100% automatizado, integrado en CI/CD pipeline |
| **Frecuencia** | En cada commit y pull request |

**Componentes a cubrir:**
- Servicios de preprocesamiento de imagen (rotacion, limpieza, mejora)
- Servicio OCR (extraccion de texto, calculo de confidence)
- Modelo de clasificacion de documentos
- Validadores de datos (formatos, reglas de negocio)
- Transformadores de datos para SAP
- Utilidades de encriptacion y seguridad
- Componentes React (renderizado, interacciones)

#### 2.1.2 Pruebas de Integracion

| Aspecto | Detalle |
|---------|---------|
| **Objetivo** | Verificar la interaccion correcta entre componentes del sistema |
| **Responsable** | QA Engineer + Desarrollo |
| **Herramientas** | pytest + httpx (API), TestContainers (BD), Playwright (E2E) |
| **Automatizacion** | 90% automatizado |
| **Frecuencia** | Diaria en pipeline CI/CD |

**Integraciones a validar:**
- FastAPI <-> Google Cloud Vision API
- FastAPI <-> Tesseract (fallback)
- FastAPI <-> PostgreSQL 15
- FastAPI <-> Google Cloud Storage
- FastAPI <-> SAP OData connector
- FastAPI <-> Redis (cache)
- React Frontend <-> FastAPI Backend
- Kubernetes <-> servicios internos

#### 2.1.3 Pruebas de Sistema

| Aspecto | Detalle |
|---------|---------|
| **Objetivo** | Validar el sistema completo end-to-end en un ambiente integrado |
| **Responsable** | QA Lead |
| **Herramientas** | Playwright (E2E), k6 (carga), OWASP ZAP (seguridad) |
| **Automatizacion** | 70% automatizado, 30% manual |
| **Frecuencia** | Al finalizar cada sprint / milestone |

**Flujos end-to-end:**
1. Carga de documento -> OCR -> Clasificacion -> Validacion -> SAP (flujo feliz)
2. Carga de documento -> OCR -> Baja confianza -> Revision manual -> Aprobacion -> SAP
3. Carga de documento -> OCR -> Error -> Reintento -> Exito
4. Carga masiva -> Procesamiento paralelo -> Resultados consolidados
5. Busqueda y consulta de documentos procesados

#### 2.1.4 Pruebas de Aceptacion de Usuario (UAT)

| Aspecto | Detalle |
|---------|---------|
| **Objetivo** | Validar que el sistema cumple las expectativas del negocio |
| **Responsable** | Product Owner + Usuarios finales |
| **Herramientas** | Manual, formularios de aceptacion |
| **Automatizacion** | 0% (completamente manual) |
| **Frecuencia** | Pre-produccion (1 ciclo formal) |

**Referencia:** Ver documento 05_Plan_Aceptacion_de_usuario.md para detalle completo.

### 2.2 Tipos de Prueba

| Tipo | Descripcion | Prioridad | Herramienta |
|------|-------------|-----------|-------------|
| **Funcional** | Validacion de requisitos funcionales | Critica | pytest, Playwright |
| **Rendimiento** | Carga, estres, volumen, escalabilidad | Critica | k6, Prometheus |
| **Seguridad** | Vulnerabilidades, autenticacion, cifrado | Critica | OWASP ZAP, Snyk |
| **Precision OCR** | Exactitud de extraccion de texto | Critica | Scripts custom + ground truth |
| **Integracion SAP** | Sincronizacion y mapeo de datos | Alta | pytest + SAP sandbox |
| **Usabilidad** | Experiencia de usuario, accesibilidad | Alta | Manual + Lighthouse |
| **Regresion** | Verificar que cambios no rompen funcionalidad existente | Alta | Suite automatizada |
| **Compatibilidad** | Navegadores (Chrome, Firefox, Edge), resoluciones | Media | BrowserStack |
| **Recuperacion** | Failover, restauracion de datos, resiliencia | Media | Scripts de simulacion |

### 2.3 Enfoque por Componente

#### Backend (FastAPI / Python 3.11)
- Pruebas unitarias con pytest para cada servicio y endpoint
- Pruebas de integracion con TestContainers para PostgreSQL
- Mocking de servicios externos (Vision API, SAP) en unitarias
- Pruebas de contrato para APIs

#### Motor OCR
- Dataset de referencia con ground truth para medir precision
- Pruebas A/B entre Vision API y Tesseract
- Pruebas con variaciones de calidad de imagen
- Medicion de F1-score, precision y recall por tipo de documento

#### Frontend (React 18)
- Jest + React Testing Library para componentes
- Playwright para pruebas E2E
- Pruebas de accesibilidad con axe-core
- Pruebas de rendimiento con Lighthouse

#### Base de Datos (PostgreSQL 15)
- Pruebas de integridad referencial
- Pruebas de migraciones (up/down)
- Pruebas de rendimiento de consultas (EXPLAIN ANALYZE)
- Pruebas de backup y restauracion

#### Infraestructura (Kubernetes / GCP)
- Pruebas de despliegue (rolling update, rollback)
- Pruebas de autoescalado (HPA)
- Pruebas de monitoreo y alertas
- Pruebas de health checks y readiness probes

---

## 3. Criterios de Entrada, Salida y Suspension

### 3.1 Criterios de Entrada

Para iniciar la ejecucion de pruebas, se deben cumplir **todos** los siguientes criterios:

| # | Criterio | Verificacion |
|---|----------|-------------|
| CE-01 | Codigo fuente deployado en ambiente de pruebas (staging) | Deployment exitoso verificado |
| CE-02 | Base de datos con esquemas migrados y datos de referencia | Script de seed ejecutado |
| CE-03 | Casos de test revisados y aprobados por QA Lead | Sign-off en documento 01_Casos_Test.md |
| CE-04 | Ambientes de prueba operativos y accesibles | Health checks verdes |
| CE-05 | Datos de prueba preparados y cargados | Datasets verificados |
| CE-06 | Herramientas de prueba configuradas y validadas | Smoke test de herramientas |
| CE-07 | Pruebas unitarias con cobertura >= 80% y pasando | Reporte de CI/CD |
| CE-08 | Documentacion de API actualizada (OpenAPI 3.0) | Swagger accesible y actual |
| CE-09 | Defectos criticos de ciclos anteriores resueltos | Tracker de defectos revisado |
| CE-10 | Conectividad a SAP sandbox verificada | Ping + autenticacion exitosa |

### 3.2 Criterios de Salida

Las pruebas se consideran completadas exitosamente cuando se cumplen **todos** los siguientes criterios:

| # | Criterio | Umbral |
|---|----------|--------|
| CS-01 | Ejecucion de casos de test | 100% de casos criticos y alta ejecutados |
| CS-02 | Tasa de exito en casos criticos | 100% pasados |
| CS-03 | Tasa de exito en casos alta prioridad | >= 95% pasados |
| CS-04 | Tasa de exito global | >= 90% pasados |
| CS-05 | Defectos criticos abiertos | 0 (cero) |
| CS-06 | Defectos altos abiertos | <= 1 |
| CS-07 | Precision OCR verificada | >= 95% en dataset de referencia |
| CS-08 | Rendimiento verificado | Latencia P95 < 5s, throughput 1000 docs/dia |
| CS-09 | Seguridad verificada | 0 vulnerabilidades criticas (OWASP) |
| CS-10 | UAT completado | Sign-off de Product Owner obtenido |
| CS-11 | Resumen de testeo generado | Documento 04_Resumen_de_testeo.md completado |
| CS-12 | Regression suite estable | 100% de regression tests pasando |

### 3.3 Criterios de Suspension

La ejecucion de pruebas se suspende si ocurre cualquiera de las siguientes condiciones:

| # | Condicion de Suspension | Accion Requerida |
|---|------------------------|------------------|
| CSU-01 | Ambiente de pruebas caido por mas de 2 horas | Escalar a Ops, bloquear ejecucion |
| CSU-02 | Mas de 3 defectos criticos abiertos simultaneamente | Devolver build a desarrollo |
| CSU-03 | Tasa de fallo > 40% en un area completa | Reunion de triage con desarrollo |
| CSU-04 | Datos de prueba corrompidos o no disponibles | Regenerar datasets, pausar pruebas |
| CSU-05 | Vision API o SAP sandbox no disponible | Esperar restauracion, ejecutar pruebas offline posibles |
| CSU-06 | Cambio de alcance significativo del proyecto | Re-planificacion de testing requerida |

**Procedimiento de reanudacion:** Una vez resuelta la condicion de suspension, QA Lead evalua el impacto, actualiza el plan y reanuda la ejecucion con aprobacion del PM.

---

## 4. Ambiente de Pruebas

### 4.1 Ambientes Disponibles

#### Ambiente de Desarrollo (Dev)

| Componente | Especificacion |
|------------|---------------|
| **Proposito** | Pruebas unitarias y de integracion tempranas |
| **API Backend** | FastAPI en contenedor Docker local, Python 3.11 |
| **Base de Datos** | PostgreSQL 15 en Docker (TestContainers) |
| **OCR Engine** | Tesseract local (Vision API mockeada) |
| **Frontend** | React 18 dev server (localhost:3000) |
| **SAP** | Mock server (WireMock) |
| **Infraestructura** | Docker Compose local |
| **Datos** | Dataset small (50 documentos) |
| **Acceso** | Solo desarrolladores |

#### Ambiente de Staging (Pre-produccion)

| Componente | Especificacion |
|------------|---------------|
| **Proposito** | Pruebas de sistema, integracion completa, rendimiento, seguridad |
| **API Backend** | FastAPI en GKE (2 replicas), Python 3.11 |
| **Base de Datos** | Cloud SQL PostgreSQL 15, 50GB, HA habilitado |
| **OCR Engine** | Google Cloud Vision API (quota staging) + Tesseract fallback |
| **Frontend** | React 18 servido por Nginx en GKE |
| **SAP** | SAP sandbox/QA environment |
| **Cache** | Redis en Memorystore (1GB) |
| **Storage** | Google Cloud Storage (bucket staging) |
| **Monitoreo** | Prometheus + Grafana (dashboards staging) |
| **Kubernetes** | GKE cluster staging (3 nodos, e2-standard-4) |
| **Datos** | Dataset medium/large (500-5000 documentos) |
| **Acceso** | Equipo QA + Desarrollo |

#### Ambiente de Produccion (Referencia)

| Componente | Especificacion |
|------------|---------------|
| **Proposito** | Solo referencia; no se ejecutan pruebas destructivas |
| **API Backend** | FastAPI en GKE (3+ replicas, HPA), Python 3.11 |
| **Base de Datos** | Cloud SQL PostgreSQL 15, 200GB, HA + read replicas |
| **OCR Engine** | Google Cloud Vision API (quota produccion) + Tesseract fallback |
| **Frontend** | React 18 servido por Nginx + CDN |
| **SAP** | SAP produccion |
| **Cache** | Redis en Memorystore (4GB) |
| **Storage** | Google Cloud Storage (bucket produccion, lifecycle policies) |
| **Monitoreo** | Prometheus + Grafana + PagerDuty |
| **Kubernetes** | GKE cluster produccion (5+ nodos, e2-standard-8) |
| **Acceso** | Solo Ops + monitoreo |

### 4.2 Requisitos de Ambiente para Testing

- Staging debe ser una replica fidedigna de produccion en configuracion (puede variar en escala)
- Datos de prueba deben ser anonimizados (sin PII real)
- SAP sandbox debe tener los mismos schemas/tablas que produccion
- Vision API en staging debe usar la misma version de modelo que produccion
- Los pipelines CI/CD deben ejecutar smoke tests automaticos post-deploy

---

## 5. Datasets de Prueba

### 5.1 Composicion de Datasets

| Dataset | Tamano | Facturas | Recibos | OC | Bitacoras | Otros | Uso |
|---------|--------|----------|---------|-----|-----------|-------|-----|
| **Small** | 50 docs | 15 | 15 | 10 | 5 | 5 | Dev, pruebas unitarias |
| **Medium** | 500 docs | 150 | 150 | 100 | 50 | 50 | Staging, pruebas funcionales |
| **Large** | 2,000 docs | 600 | 600 | 400 | 200 | 200 | Pruebas de volumen |
| **Stress** | 5,000 docs | 1,500 | 1,500 | 1,000 | 500 | 500 | Pruebas de estres |
| **Edge** | 100 docs | 20 | 20 | 15 | 10 | 35 | Casos limite y anomalias |

### 5.2 Caracteristicas por Dataset

#### Dataset Small (50 documentos)
- Documentos de alta calidad (300+ DPI)
- Texto claro y legible
- Formatos estandar (PDF, JPG, PNG)
- Ground truth verificado manualmente al 100%
- Utilizado para validacion rapida y desarrollo

#### Dataset Medium (500 documentos)
- Mezcla de calidades (72-600 DPI)
- Incluye documentos con ruido leve
- Todos los formatos soportados
- Ground truth verificado manualmente al 100%
- Dataset principal para validacion de precision (F1-score)
- Referencia: CA-H4 (500 documentos de referencia con ground truth)

#### Dataset Large (2,000 documentos)
- Representativo de carga real de produccion
- Variacion amplia de calidades y formatos
- Ground truth verificado para subset de 500 documentos
- Utilizado para pruebas de volumen y regresion

#### Dataset Stress (5,000 documentos)
- Tamano equivalente a 5 dias de carga pico
- Incluye documentos de todos los tamanos (1KB - 50MB)
- Sin ground truth completo (solo metricas de sistema)
- Utilizado para pruebas de estres y capacidad

#### Dataset Edge (100 documentos)
- Documentos con anomalias especificas:
  - Paginas en blanco (10)
  - Texto manuscrito (10)
  - Baja resolucion extrema (10)
  - Rotaciones diversas (10)
  - Watermarks (10)
  - Documentos corruptos (5)
  - PDFs protegidos (5)
  - Idiomas no soportados (10)
  - Documentos de 100+ paginas (5)
  - Contenido mixto (15)
  - Formatos no soportados (10)

### 5.3 Gestion de Datos de Prueba

- Todos los datasets se almacenan en Google Cloud Storage (bucket: `ocr-testing-datasets`)
- Datos con PII deben ser anonimizados antes de su uso en testing
- Versionado de datasets con nomenclatura: `dataset-{tipo}-v{version}-{fecha}`
- Responsable de mantenimiento de datasets: QA Engineer
- Actualizacion de ground truth al menos 1 vez por ciclo de pruebas

---

## 6. Planificacion

### 6.1 Fases de Testing

| Fase | Semana | Actividades | Responsable | Dependencias |
|------|--------|-------------|-------------|--------------|
| **Fase 0: Preparacion** | Semana 1 | Configurar ambientes, preparar datasets, revisar casos de test, configurar herramientas | QA Lead | Codigo en staging |
| **Fase 1: Pruebas Unitarias** | Semana 1-2 | Verificar cobertura >= 80%, ejecutar suite completa, corregir fallos | Desarrollo | Build exitoso |
| **Fase 2: Pruebas de Integracion** | Semana 2 | API endpoints, BD, Vision API, Tesseract, Cloud Storage | QA + Dev | Unitarias pasando |
| **Fase 3: Pruebas Funcionales** | Semana 2-3 | Ejecucion de TC-0001 a TC-0082 (funcionales) | QA Team | Integracion estable |
| **Fase 4: Pruebas de Precision OCR** | Semana 3 | Dataset 500 docs, F1-score, confidence scores, edge cases | QA + Data | Dataset medium listo |
| **Fase 5: Integracion SAP** | Semana 3 | TC-0061 a TC-0072, sincronizacion, mapeo, reconciliacion | QA + Dev SAP | SAP sandbox activo |
| **Fase 6: Pruebas de Rendimiento** | Semana 3-4 | TC-0083 a TC-0092, carga, estres, escalabilidad | QA + Ops | Sistema estable |
| **Fase 7: Pruebas de Seguridad** | Semana 4 | TC-0093 a TC-0102, OWASP, penetration testing basico | QA + Security | Sistema estable |
| **Fase 8: Pruebas de Usabilidad** | Semana 4 | TC-0103 a TC-0110, accesibilidad, UX | QA + UX | Frontend estable |
| **Fase 9: Regresion Completa** | Semana 4 | Suite completa de regresion automatizada + manual | QA Team | Defectos corregidos |
| **Fase 10: UAT** | Semana 4-5 | Ejecucion con usuarios reales, sign-off | PO + Usuarios | Regresion pasando |

### 6.2 Cronograma Detallado

```
Semana 1:  [====] Preparacion + Unitarias
Semana 2:  [====] Integracion + Funcionales (inicio)
Semana 3:  [====] Funcionales + Precision OCR + SAP + Rendimiento (inicio)
Semana 4:  [====] Rendimiento + Seguridad + Usabilidad + Regresion
Semana 5:  [==  ] UAT + Cierre + Resumen de Testeo
```

### 6.3 Dependencias Criticas

| Dependencia | Impacto si no se cumple | Plan de mitigacion |
|-------------|------------------------|--------------------|
| SAP sandbox disponible | No se pueden ejecutar 12 casos de integracion | Usar mock server como alternativa temporal |
| Vision API quota en staging | Limita pruebas de precision y volumen | Pre-solicitar quota suficiente con 2 semanas de anticipacion |
| Dataset ground truth verificado | No se puede medir F1-score real | Iniciar verificacion manual en paralelo con desarrollo |
| Equipo QA completo | Retrasos en ejecucion | Cross-training de desarrolladores en testing |
| Kubernetes cluster staging operativo | No se pueden ejecutar pruebas de rendimiento | Alternativa en Docker Compose con limitaciones |

### 6.4 Estimacion de Esfuerzo

| Actividad | Horas Estimadas | Recursos |
|-----------|----------------|----------|
| Preparacion de ambientes y datos | 16h | QA Lead, Ops |
| Ejecucion pruebas funcionales (manuales) | 40h | 2 QA Engineers |
| Ejecucion pruebas automatizadas | 8h (configuracion) + CI/CD | QA Lead |
| Pruebas de precision OCR | 16h | QA + Data Analyst |
| Pruebas de rendimiento | 16h | QA + Ops |
| Pruebas de seguridad | 12h | QA + Security |
| UAT | 16h | PO + Usuarios |
| Gestion de defectos y retesting | 24h | QA Team |
| Documentacion y reportes | 12h | QA Lead |
| **Total estimado** | **~160h** | **~3-4 semanas** |

---

## 7. Gestion de Defectos

### 7.1 Clasificacion de Severidad

| Severidad | Codigo | Descripcion | Ejemplo | SLA Resolucion |
|-----------|--------|-------------|---------|----------------|
| **Critica** | S1 | Sistema inoperante, perdida de datos, brecha de seguridad | Perdida de documentos procesados, crash del servicio OCR, acceso no autorizado | 4 horas |
| **Alta** | S2 | Funcionalidad principal degradada, sin workaround | Precision OCR < 90%, sincronizacion SAP fallando > 10%, cola de revision bloqueada | 24 horas |
| **Media** | S3 | Funcionalidad afectada con workaround disponible | Latencia > 10s en documentos grandes, errores de formato en exports, UI lenta | 72 horas |
| **Baja** | S4 | Defecto cosmetico o menor, no afecta funcionalidad | Typo en interfaz, alineacion de elementos, mejora de mensaje de error | Siguiente release |

### 7.2 Ciclo de Vida de Defectos

```
Nuevo --> Asignado --> En Progreso --> Resuelto --> En Verificacion --> Cerrado
  |                       |              |                |
  +-- Rechazado           +-- Bloqueado  +-- Reabierto ---+
  |                                      |
  +-- Duplicado                          +-- Diferido
```

**Estados:**
1. **Nuevo:** Defecto reportado por QA con evidencia
2. **Asignado:** Triaged y asignado a desarrollador
3. **En Progreso:** Desarrollador trabajando en la correccion
4. **Resuelto:** Fix aplicado, pendiente de verificacion por QA
5. **En Verificacion:** QA ejecuta retest del defecto
6. **Cerrado:** Verificado como corregido
7. **Reabierto:** Retest fallo, defecto persiste
8. **Bloqueado:** No se puede resolver por dependencia externa
9. **Rechazado:** No es defecto (comportamiento esperado)
10. **Duplicado:** Ya reportado bajo otro ID
11. **Diferido:** Pospuesto a futuro release por decision de PO

### 7.3 Formato de Reporte de Defecto

Cada defecto debe incluir:

| Campo | Descripcion |
|-------|-------------|
| **ID** | BUG-XXXX (auto-generado) |
| **Titulo** | Descripcion concisa del problema |
| **Severidad** | S1 / S2 / S3 / S4 |
| **Prioridad** | Critica / Alta / Media / Baja |
| **Caso de Test** | TC-XXXX asociado |
| **Ambiente** | Dev / Staging / Produccion |
| **Pasos para reproducir** | Pasos numerados y claros |
| **Resultado actual** | Lo que ocurre actualmente |
| **Resultado esperado** | Lo que deberia ocurrir |
| **Evidencia** | Screenshots, logs, videos |
| **Componente** | Backend / Frontend / OCR / SAP / DB / Infra |
| **Asignado a** | Nombre del desarrollador |
| **Fecha reporte** | DD/MM/YYYY |
| **Fecha resolucion** | DD/MM/YYYY (cuando aplique) |

### 7.4 Metricas de Defectos

| Metrica | Formula | Target |
|---------|---------|--------|
| **Densidad de defectos** | # defectos / KLOC | < 5 por KLOC |
| **Tasa de deteccion** | Defectos encontrados en testing / Total defectos | > 90% |
| **Tasa de reinyeccion** | Defectos reabiertos / Total resueltos | < 10% |
| **Tiempo medio de resolucion (S1)** | Promedio horas desde reporte a cierre (S1) | < 4 horas |
| **Tiempo medio de resolucion (S2)** | Promedio horas desde reporte a cierre (S2) | < 24 horas |
| **Defectos abiertos al cierre** | # defectos S1+S2 abiertos al go-live | 0 S1, <= 1 S2 |
| **Eficiencia de testing** | Defectos encontrados / Horas de testing | Tendencia creciente |

### 7.5 Reuniones de Triage

- **Frecuencia:** Diaria durante ejecucion de pruebas (15 min standup)
- **Participantes:** QA Lead, Tech Lead, PM
- **Agenda:** Revisar defectos nuevos, priorizar, asignar, revisar bloqueantes
- **Escalacion:** Defectos S1 se escalan inmediatamente (no esperan triage)

---

## 8. Riesgos de Testing

| # | Riesgo | Probabilidad | Impacto | Mitigacion |
|---|--------|-------------|---------|------------|
| RT-01 | Vision API no disponible durante testing | Media | Alto | Mantener Tesseract como fallback, pre-cachear resultados de Vision API para dataset de referencia |
| RT-02 | SAP sandbox con datos desactualizados | Alta | Alto | Solicitar refresh de datos 1 semana antes de pruebas de integracion |
| RT-03 | Dataset ground truth con errores de etiquetado | Media | Alto | Doble verificacion por 2 personas independientes, revision cruzada |
| RT-04 | Tiempo insuficiente para completar todos los casos | Media | Medio | Priorizar por criticidad, automatizar donde sea posible, escalar recursos |
| RT-05 | Ambiente de staging inestable | Baja | Alto | Monitoreo proactivo, equipo Ops en standby durante testing |
| RT-06 | Precision OCR no alcanza 95% en primera iteracion | Media | Alto | Planificar ciclo adicional de ajustes y re-testing |
| RT-07 | Cambios de alcance durante fase de testing | Baja | Medio | Change management formal, re-evaluacion de impacto en testing |
| RT-08 | Dependencia de equipo SAP para resolver issues de integracion | Alta | Medio | Identificar contacto SAP dedicado, SLA de soporte definido |
| RT-09 | Pruebas de seguridad revelan vulnerabilidades criticas | Media | Alto | Planificar buffer de 3 dias para remediacion de seguridad |
| RT-10 | Baja disponibilidad de usuarios para UAT | Alta | Medio | Programar UAT con 3 semanas de anticipacion, tener backup users |

---

## 9. Entregables

| # | Entregable | Responsable | Fecha Estimada | Estado |
|---|-----------|-------------|----------------|--------|
| E-01 | 01_Casos_Test.md - Catalogo completo de casos de test | QA Lead | Semana 1 | Completado |
| E-02 | 02_Plan_de_testeo.md - Este documento | QA Lead | Semana 1 | Completado |
| E-03 | Suite de pruebas automatizadas (pytest + Playwright) | QA Engineer | Semana 2 | Pendiente |
| E-04 | Reporte de precision OCR (F1-score por tipo de documento) | QA + Data | Semana 3 | Pendiente |
| E-05 | Reporte de pruebas de rendimiento (k6) | QA + Ops | Semana 4 | Pendiente |
| E-06 | Reporte de pruebas de seguridad (OWASP ZAP) | QA + Security | Semana 4 | Pendiente |
| E-07 | 04_Resumen_de_testeo.md - Resumen ejecutivo de testing | QA Lead | Semana 5 | Pendiente |
| E-08 | 05_Plan_Aceptacion_de_usuario.md - Plan de UAT | QA Lead + PO | Semana 1 | Completado |
| E-09 | Formulario de sign-off UAT firmado | Product Owner | Semana 5 | Pendiente |
| E-10 | Log de defectos final con estado de cada defecto | QA Lead | Semana 5 | Pendiente |

---

## 10. Aprobaciones

| Rol | Nombre | Firma | Fecha |
|-----|--------|-------|-------|
| QA Lead | __________________ | __________________ | ____/____/________ |
| Tech Lead | __________________ | __________________ | ____/____/________ |
| Product Owner | __________________ | __________________ | ____/____/________ |
| Project Manager | __________________ | __________________ | ____/____/________ |
| Security Officer | __________________ | __________________ | ____/____/________ |

**Estado de Aprobacion:** Pendiente

---

## Control de Versiones

| Version | Fecha | Autor | Cambios |
|---------|-------|-------|---------|
| 1.0 | 24/02/2026 | QA Lead | Creacion inicial del plan de testeo |

---

**Nota:** Este plan de testeo es un documento vivo que sera actualizado a medida que avance el proyecto. Cualquier cambio significativo requiere revision y aprobacion por parte del QA Lead y el PM.
