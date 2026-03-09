# ADR-0001: Seleccion de Python 3.11 y FastAPI como Stack de Backend

**Estado:** Aceptada
**Fecha:** 2026-02-24
**Autor:** IDC Ingenieria
**Proyecto:** OCR Operativo - Reconocimiento Optico de Caracteres

---

## Contexto

El proyecto OCR Operativo requiere un backend capaz de orquestar el procesamiento de documentos a gran escala (objetivo: 1.000+ documentos/dia), integrar multiples motores OCR (Google Cloud Vision como primario, Tesseract como fallback), comunicarse con sistemas SAP/ERP via OData y exponer APIs REST para un frontend React 18. Se necesita un framework que ofrezca alto rendimiento en operaciones I/O-bound, un ecosistema maduro para procesamiento de imagenes y machine learning, y que permita iteraciones rapidas de desarrollo dado el alcance del proyecto.

La decision del stack de backend es fundacional, ya que condiciona la velocidad de desarrollo, la disponibilidad de librerias para OCR y vision artificial, la facilidad de integracion con servicios cloud (GCP) y la capacidad del equipo para mantener y evolucionar el sistema.

## Decision

Se adopta **Python 3.11** como lenguaje principal del backend y **FastAPI** como framework web, desplegado sobre **Uvicorn** con workers asincrono. La arquitectura seguira un patron de microservicios ligeros orquestados en Kubernetes sobre GCP.

Componentes clave de la decision:
- **Python 3.11** por sus mejoras de rendimiento (10-60% mas rapido que 3.10) y soporte nativo de tipado avanzado.
- **FastAPI** como framework asincrono con generacion automatica de documentacion OpenAPI.
- **Uvicorn** como servidor ASGI de alto rendimiento.
- **Pydantic v2** para validacion de datos y serializacion.
- Uso de `asyncio` y `concurrent.futures` para paralelizar llamadas a APIs externas (Google Vision, SAP OData).

## Alternativas Consideradas

### 1. Node.js + Express/NestJS
| Aspecto | Evaluacion |
|---|---|
| Rendimiento asincrono | Excelente para I/O-bound, comparable a FastAPI |
| Ecosistema OCR/ML | Limitado; las librerias de vision artificial y ML son significativamente inferiores a Python (no hay equivalente maduro a OpenCV, Pillow, scikit-image) |
| Integracion Google Cloud Vision | SDK disponible, pero menos documentacion y ejemplos que Python |
| Tipado | TypeScript aporta tipado estatico, pero el ecosistema de validacion es menos maduro que Pydantic |
| Equipo | Requeriria reentrenamiento del equipo en ecosistema Node para procesamiento de imagenes |

**Descartado:** El ecosistema de procesamiento de imagenes y ML en JavaScript es insuficiente para las necesidades del proyecto.

### 2. Go (Golang) + Gin/Fiber
| Aspecto | Evaluacion |
|---|---|
| Rendimiento puro | Superior en CPU-bound y concurrencia masiva |
| Ecosistema OCR/ML | Muy limitado; dependeria de bindings CGO a librerias C/C++ con complejidad operativa alta |
| Velocidad de desarrollo | Mas lenta debido a la verbosidad del lenguaje y menor cantidad de librerias de alto nivel |
| Integracion GCP | SDK oficial disponible, buena calidad |
| Equipo | Curva de aprendizaje significativa; el equipo no tiene experiencia previa en Go |

**Descartado:** La falta de ecosistema ML/OCR nativo y la curva de aprendizaje no justifican la ganancia de rendimiento puro, dado que el cuello de botella es I/O (llamadas a APIs externas), no CPU.

### 3. Java + Spring Boot
| Aspecto | Evaluacion |
|---|---|
| Rendimiento | Excelente tras calentamiento de JVM; alto consumo de memoria base |
| Ecosistema OCR/ML | Bueno (Tess4J, DL4J), aunque inferior al ecosistema Python |
| Velocidad de desarrollo | Mas lenta; mayor boilerplate y ciclos de compilacion |
| Integracion SAP | Nativa via SAP Cloud SDK for Java, ventaja potencial |
| Infraestructura | Mayor consumo de recursos (memoria JVM), impacto en costos de Kubernetes |

**Descartado:** El overhead de memoria y la menor velocidad de iteracion no compensan la ventaja en integracion SAP, que se resuelve adecuadamente con las librerias OData de Python.

## Justificacion

1. **Ecosistema lider en OCR y vision artificial:** Python es el estandar de facto para procesamiento de imagenes (OpenCV, Pillow, scikit-image), machine learning (scikit-learn, TensorFlow, PyTorch) y OCR (pytesseract, google-cloud-vision). Esto permite implementar pre-procesamiento de imagenes, post-procesamiento de texto y pipelines de mejora de accuracy sin depender de servicios externos adicionales.

2. **Rendimiento asincrono adecuado al caso de uso:** El cuello de botella del sistema es I/O-bound (llamadas a Google Cloud Vision API, consultas a PostgreSQL, comunicacion con SAP OData). FastAPI sobre Uvicorn maneja esto eficientemente con `async/await`, logrando el objetivo de <5 segundos de procesamiento por documento.

3. **Productividad de desarrollo:** FastAPI genera documentacion OpenAPI automaticamente, Pydantic v2 reduce el codigo de validacion, y el tipado estatico de Python 3.11 mejora la mantenibilidad. Se estima un 30-40% menos de tiempo de desarrollo comparado con Java/Spring Boot.

4. **Integracion nativa con GCP:** El SDK de Google Cloud para Python es el mas documentado y mantenido, con soporte de primera clase para Cloud Vision, Cloud Storage, Pub/Sub y el resto de servicios utilizados en el proyecto.

5. **Capacidad del equipo:** El equipo de IDC Ingenieria tiene experiencia solida en Python, lo que elimina la curva de aprendizaje y reduce el riesgo de errores en las fases iniciales del proyecto.

6. **Comunidad y soporte:** Python tiene la comunidad mas grande en el dominio de procesamiento de documentos e IA, facilitando la resolucion de problemas y el acceso a soluciones probadas.

## Consecuencias

### Positivas
- Acceso al ecosistema mas rico de librerias para OCR, vision artificial y ML.
- Documentacion API automatica con OpenAPI/Swagger, reduciendo esfuerzo de documentacion.
- Desarrollo rapido de prototipos y MVPs con iteraciones cortas.
- Compatibilidad directa con modelos de ML para futuras mejoras de accuracy.
- Despliegue eficiente en contenedores ligeros sobre Kubernetes.

### Negativas
- Rendimiento inferior a Go/Java en operaciones CPU-bound intensivas (mitigado con workers de Celery para tareas pesadas).
- GIL (Global Interpreter Lock) limita el paralelismo real en un solo proceso (mitigado con multiprocessing y arquitectura de microservicios).
- Consumo de memoria mayor que Go para alta concurrencia (mitigado con escalado horizontal en Kubernetes).

### Riesgos
- **Riesgo de rendimiento en picos:** Si el volumen supera significativamente los 1.000 docs/dia, podria requerirse optimizacion agresiva o migracion parcial de componentes criticos a Go/Rust. **Mitigacion:** Arquitectura de microservicios permite reemplazar componentes individuales sin reescribir todo el sistema.
- **Dependencia de librerias de terceros:** El ecosistema Python tiene dependencias transitivas complejas que pueden causar conflictos de versiones. **Mitigacion:** Uso de Poetry para gestion de dependencias y contenedores Docker con versiones fijadas.
- **Seguridad en dependencias:** Mayor superficie de ataque por la cantidad de paquetes. **Mitigacion:** Escaneo automatico con Safety/Snyk en el pipeline CI/CD.

## Metricas de Validacion

| Metrica | Objetivo | Metodo de Medicion |
|---|---|---|
| Tiempo de respuesta API (P95) | < 500ms (sin OCR) | Monitoreo con Prometheus + Grafana |
| Throughput de procesamiento | >= 1.000 docs/dia | Contadores en Redis + dashboards |
| Tiempo de procesamiento OCR end-to-end | < 5 segundos/documento | Trazas distribuidas con OpenTelemetry |
| Tiempo de desarrollo por feature | 30% menor que estimacion Java | Tracking en Jira/herramienta de gestion |
| Disponibilidad del servicio | >= 99.5% | Uptime monitoring en GCP |
| Cobertura de tests | >= 80% | pytest + coverage.py en CI/CD |

---
**Revisores:** [Pendiente]
**Aprobado por:** [Pendiente]
