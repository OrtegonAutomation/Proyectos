# ADR-0003: Seleccion de PostgreSQL 15 como Base de Datos de Persistencia

**Estado:** Aceptada
**Fecha:** 2026-02-24
**Autor:** IDC Ingenieria
**Proyecto:** OCR Operativo - Reconocimiento Optico de Caracteres

---

## Contexto

El proyecto OCR Operativo necesita una base de datos para almacenar de forma persistente los metadatos de documentos procesados, resultados de OCR (texto extraido, coordenadas de regiones, niveles de confianza por palabra), informacion de usuarios, registros de auditoria, configuraciones del sistema y datos de integracion con SAP/ERP. La capa de cache en Redis ya esta definida para datos volatiles y de acceso frecuente; esta decision aborda unicamente la persistencia durable.

Los requisitos clave para la base de datos son:
- **Integridad referencial:** Los documentos tienen relaciones complejas (documento -> paginas -> regiones -> palabras -> integraciones SAP).
- **Consultas analiticas:** Se requieren reportes de precision por tipo de documento, tiempos de procesamiento, volumenes por periodo y metricas de calidad.
- **Busqueda de texto:** Capacidad de buscar dentro del texto extraido por OCR para localizar documentos.
- **Datos semi-estructurados:** Los resultados crudos de Google Cloud Vision son JSON con estructura variable segun el tipo de documento.
- **Volumen estimado:** ~1.000 documentos/dia, ~3.000 paginas/dia, ~30.000 regiones de texto/dia. Crecimiento anual estimado de 50-100 GB.
- **Cumplimiento y auditoria:** Trazabilidad completa de quien proceso que documento, cuando y con que resultado.

## Decision

Se adopta **PostgreSQL 15** como sistema de gestion de base de datos relacional principal, desplegado como servicio gestionado en **Cloud SQL for PostgreSQL** en GCP. La arquitectura de datos incluye:

1. **Cloud SQL for PostgreSQL 15** como instancia primaria con alta disponibilidad (regional).
2. **Replica de lectura** para consultas analiticas y reportes, evitando impacto en la instancia transaccional.
3. **Uso de JSONB** para almacenar resultados crudos de OCR y metadatos variables sin sacrificar la capacidad de consulta.
4. **Full-text search nativo** con configuracion de diccionario en espanol para busqueda dentro del texto extraido.
5. **Particionamiento por rango de fechas** en tablas de alto volumen (resultados OCR, logs de auditoria) para mantener el rendimiento a largo plazo.
6. **Redis** se mantiene como capa de cache complementaria (no reemplaza ni compite con PostgreSQL).

## Alternativas Consideradas

### 1. MongoDB 7.x
| Aspecto | Evaluacion |
|---|---|
| Modelo de datos | Documentos JSON nativos; flexible para resultados OCR variables |
| Integridad referencial | Limitada; no soporta JOINs eficientes ni foreign keys nativas |
| Consultas analiticas | Aggregation pipeline potente pero mas complejo que SQL estandar |
| Busqueda de texto | Atlas Search (basado en Lucene) es superior para full-text search |
| Transacciones | Soportadas desde 4.0, pero con overhead y limitaciones en sharded clusters |
| Ecosistema en GCP | MongoDB Atlas disponible, pero es un servicio tercero (no nativo de GCP) |
| Costo | Atlas en GCP es 30-50% mas caro que Cloud SQL para cargas equivalentes |

**Descartado:** La falta de integridad referencial nativa es un riesgo critico para un sistema que maneja relaciones complejas (documentos -> paginas -> regiones -> integraciones SAP). La flexibilidad de esquema de MongoDB no aporta ventaja significativa, dado que PostgreSQL JSONB cubre las necesidades de datos semi-estructurados. El costo adicional de Atlas no se justifica.

### 2. MySQL 8.x (Cloud SQL)
| Aspecto | Evaluacion |
|---|---|
| Modelo de datos | Relacional completo con soporte JSON mejorado en 8.x |
| Soporte JSONB | JSON nativo pero con capacidades de consulta inferiores a PostgreSQL JSONB |
| Full-text search | Soportado pero con configuracion de idioma espanol limitada |
| Particionamiento | Soportado pero con mas restricciones que PostgreSQL |
| Tipos de datos avanzados | Sin arrays nativos, sin tipos de rango, sin tipos geometricos |
| Funciones ventana | Soportadas desde 8.0, pero con menos optimizaciones |
| Extensibilidad | Limitada comparada con el sistema de extensiones de PostgreSQL |

**Descartado:** MySQL es una alternativa viable pero inferior en capacidades especificas criticas para el proyecto: JSONB con indexacion GIN, full-text search con diccionarios de idioma configurables, y tipos de datos avanzados. La diferencia de rendimiento en consultas analiticas complejas favorece a PostgreSQL.

### 3. Amazon DynamoDB
| Aspecto | Evaluacion |
|---|---|
| Modelo de datos | Clave-valor/documento; altamente escalable |
| Consultas | Limitadas a clave primaria y indices secundarios; sin JOINs |
| Consultas analiticas | Requiere exportar a otro servicio (Athena, Redshift) |
| Busqueda de texto | No soportada nativamente; requiere OpenSearch adicional |
| Costo | Modelo por peticion puede ser economico o muy caro dependiendo de patrones de acceso |
| Integracion con GCP | Servicio exclusivo de AWS; requiere operacion cross-cloud |

**Descartado:** DynamoDB es inadecuado para las necesidades analiticas y relacionales del proyecto. La ausencia de JOINs, la imposibilidad de consultas ad-hoc complejas y la operacion cross-cloud lo descalifican completamente.

### 4. PostgreSQL 15 auto-gestionado en Kubernetes
| Aspecto | Evaluacion |
|---|---|
| Control | Total control sobre configuracion, versiones y extensiones |
| Costo | Menor costo directo de licencia (open source) |
| Complejidad operativa | Alta; requiere gestion de backups, failover, actualizaciones, monitoreo |
| Alta disponibilidad | Posible con operadores como CloudNativePG, pero complejo de configurar y mantener |
| Riesgo | Mayor riesgo de perdida de datos por error operativo |

**Descartado:** La complejidad operativa de gestionar PostgreSQL en Kubernetes no se justifica para el tamano actual del equipo. Cloud SQL elimina la carga de administracion de base de datos, backups automaticos, failover y actualizaciones de seguridad.

## Justificacion

1. **Integridad referencial robusta:** PostgreSQL garantiza la consistencia de las relaciones complejas entre documentos, paginas, regiones de texto, resultados OCR e integraciones SAP mediante foreign keys, constraints y transacciones ACID completas. Esto es critico para un sistema que alimenta datos a un ERP.

2. **JSONB para datos semi-estructurados:** Los resultados crudos de Google Cloud Vision API (estructura variable segun tipo de deteccion) se almacenan en columnas JSONB con indices GIN, permitiendo consultas directas sobre el JSON sin sacrificar el modelo relacional para el resto de los datos.

3. **Full-text search nativo en espanol:** PostgreSQL soporta configuraciones de full-text search con diccionarios de idioma (`spanish`), incluyendo stemming y stop words en espanol. Esto permite buscar documentos por contenido OCR sin necesidad de un motor de busqueda externo (Elasticsearch) para el volumen actual del proyecto.

4. **Servicio gestionado en GCP:** Cloud SQL for PostgreSQL elimina la carga operativa de administracion de base de datos, proporcionando backups automaticos, failover regional, actualizaciones de seguridad y monitoreo integrado con Cloud Monitoring.

5. **Capacidades analiticas avanzadas:** Funciones ventana, CTEs recursivos, vistas materializadas y particionamiento nativo permiten construir reportes complejos de metricas de OCR (precision por tipo de documento, tendencias temporales, cuellos de botella) directamente en la base de datos.

6. **Ecosistema maduro con Python/FastAPI:** Las librerias `asyncpg` (asincrona) y `SQLAlchemy 2.0` con soporte async proporcionan integracion de alto rendimiento con FastAPI, aprovechando la naturaleza asincrona del backend.

7. **Costo-eficiencia:** Cloud SQL PostgreSQL en configuracion de alta disponibilidad para el volumen estimado (50-100 GB/ano) tiene un costo mensual de $150-300, significativamente menor que MongoDB Atlas o soluciones cross-cloud.

## Consecuencias

### Positivas
- Modelo de datos relacional con integridad referencial completa para relaciones complejas documento-pagina-region-SAP.
- Capacidad de almacenar y consultar datos semi-estructurados (JSONB) junto con datos relacionales en un solo sistema.
- Busqueda de texto en espanol sin necesidad de infraestructura adicional (Elasticsearch).
- Administracion simplificada mediante Cloud SQL (backups, failover, parches automaticos).
- Compatibilidad total con SQLAlchemy y asyncpg para el stack Python/FastAPI.
- Capacidad de escalar verticalmente (hasta 96 vCPUs, 624 GB RAM en Cloud SQL) y horizontalmente (replicas de lectura).

### Negativas
- Cloud SQL tiene limites de almacenamiento (64 TB) y no escala horizontalmente en escrituras de forma nativa (a diferencia de Spanner o CockroachDB). Para el volumen actual esto no es una limitacion, pero podria serlo a escala masiva.
- Full-text search nativo de PostgreSQL es funcional pero inferior a Elasticsearch/OpenSearch para busquedas complejas con facetas, fuzzy matching avanzado y relevancia personalizada.
- La dependencia de Cloud SQL implica un vendor lock-in parcial con GCP (mitigado por ser PostgreSQL estandar, migrable a cualquier proveedor).

### Riesgos
- **Riesgo de crecimiento de datos no planificado:** Si el volumen de documentos crece significativamente, las tablas de resultados OCR podrian degradar el rendimiento de consultas. **Mitigacion:** Particionamiento por rango de fechas implementado desde el inicio, politica de archivado de datos historicos a Cloud Storage y monitoreo de tamano de tablas.
- **Riesgo de complejidad en migraciones de esquema:** A medida que el proyecto evolucione, las migraciones de base de datos pueden volverse complejas. **Mitigacion:** Uso de Alembic para migraciones versionadas y reversibles, con testing automatico de migraciones en CI/CD.
- **Riesgo de rendimiento en consultas analiticas pesadas:** Consultas de reportes complejos podrian afectar el rendimiento transaccional. **Mitigacion:** Replica de lectura dedicada para consultas analiticas y vistas materializadas con refresh programado.
- **Riesgo de perdida de datos en caso de fallo catastrofico regional:** Aunque Cloud SQL proporciona alta disponibilidad regional, un fallo de zona completa podria causar una interrupcion. **Mitigacion:** Backups automaticos diarios con retencion de 30 dias y backups cross-region configurados.

## Metricas de Validacion

| Metrica | Objetivo | Metodo de Medicion |
|---|---|---|
| Tiempo de respuesta de consultas transaccionales (P95) | < 100ms | Monitoreo con Cloud SQL Insights + OpenTelemetry |
| Tiempo de respuesta de consultas analiticas | < 5 segundos | Cloud SQL Insights en replica de lectura |
| Disponibilidad de la base de datos | >= 99.95% | SLA de Cloud SQL + monitoreo custom |
| Tamano de base de datos al ano 1 | < 100 GB | Monitoreo de almacenamiento en Cloud SQL |
| Tiempo de recuperacion ante desastre (RTO) | < 1 hora | Pruebas periodicas de restore de backup |
| Punto de recuperacion ante desastre (RPO) | < 5 minutos | Verificacion de replicacion sincrona |
| Precision de full-text search | >= 90% recall en busquedas | Tests automaticos contra set de consultas de referencia |

---
**Revisores:** [Pendiente]
**Aprobado por:** [Pendiente]
