# Requerimientos No Funcionales - OCR Operativo

| Campo | Detalle |
|-------|---------|
| **Proyecto** | OCR Operativo |
| **Versión** | 1.0 |
| **Fecha** | 2026-02-24 |
| **Estado** | En revisión |
| **Autor** | IDC Ingeniería |

---

## Tabla de Contenidos

1. [RNF-01: Rendimiento](#rnf-01-rendimiento)
2. [RNF-02: Disponibilidad](#rnf-02-disponibilidad)
3. [RNF-03: Escalabilidad](#rnf-03-escalabilidad)
4. [RNF-04: Seguridad](#rnf-04-seguridad)
5. [RNF-05: Usabilidad](#rnf-05-usabilidad)
6. [RNF-06: Mantenibilidad](#rnf-06-mantenibilidad)
7. [RNF-07: Compatibilidad](#rnf-07-compatibilidad)
8. [RNF-08: Confiabilidad](#rnf-08-confiabilidad)
9. [RNF-09: Compliance (GDPR)](#rnf-09-compliance-gdpr)
10. [RNF-10: Monitoreo y Observabilidad](#rnf-10-monitoreo-y-observabilidad)
11. [Matriz de Requerimientos No Funcionales](#matriz-de-requerimientos-no-funcionales)

---

## RNF-01: Rendimiento

| Campo | Detalle |
|-------|---------|
| **ID** | RNF-01 |
| **Categoría** | Rendimiento (Performance) |
| **Prioridad** | Alta |
| **Verificable** | Si |

### Descripción

El sistema debe procesar documentos dentro de los tiempos establecidos, manteniendo un throughput mínimo de 1000 documentos por día en condiciones normales de operación.

### Métricas y umbrales

| Métrica | Valor objetivo | Valor mínimo aceptable | Método de medición |
|---------|---------------|------------------------|-------------------|
| Tiempo de procesamiento OCR por documento | < 3 segundos | < 5 segundos | Medición end-to-end desde recepción hasta resultado |
| Tiempo de respuesta API (endpoints CRUD) | < 200 ms | < 500 ms | Percentil 95 (P95) |
| Tiempo de carga de interfaz web | < 2 segundos | < 4 segundos | First Contentful Paint (FCP) |
| Throughput diario | >= 1500 docs/día | >= 1000 docs/día | Contador de documentos procesados en 24h |
| Tiempo de sincronización ERP | < 2 segundos | < 5 segundos | Tiempo desde envío hasta confirmación SAP |
| Tiempo de generación de reportes | < 10 segundos | < 30 segundos | Para reportes de hasta 10.000 registros |
| Uso de CPU bajo carga normal | < 60% | < 80% | Monitoreo continuo de infraestructura |
| Uso de memoria bajo carga normal | < 70% | < 85% | Monitoreo continuo de infraestructura |

### Escenarios de prueba de rendimiento

| Escenario | Descripción | Carga | Criterio de éxito |
|-----------|-------------|-------|-------------------|
| Carga normal | Procesamiento en horario laboral | 50 docs/hora | Tiempo < 3s por documento |
| Carga pico | Procesamiento batch + individual simultáneo | 200 docs/hora | Tiempo < 5s por documento |
| Carga sostenida | 8 horas continuas de procesamiento | 1000+ docs | Sin degradación > 10% |
| Concurrencia | 20 usuarios simultáneos en interfaz | 20 sesiones | Respuesta API < 500ms |

### Condiciones

- Las métricas aplican bajo condiciones normales de red (latencia < 50ms al servicio OCR).
- El procesamiento batch puede tener un rendimiento reducido en un 20% respecto al individual.
- Los tiempos de sincronización ERP dependen de la disponibilidad y carga del sistema SAP.

---

## RNF-02: Disponibilidad

| Campo | Detalle |
|-------|---------|
| **ID** | RNF-02 |
| **Categoría** | Disponibilidad (Availability) |
| **Prioridad** | Alta |
| **Verificable** | Si |

### Descripción

El sistema debe estar disponible al menos el 99.5% del tiempo durante el horario de operación, con mecanismos de recuperación automática ante fallos.

### Métricas y umbrales

| Métrica | Valor objetivo | Valor mínimo aceptable |
|---------|---------------|------------------------|
| Disponibilidad mensual | 99.9% | 99.5% |
| Tiempo de inactividad planificado máximo | 4 horas/mes | 8 horas/mes |
| Tiempo de inactividad no planificado máximo | 0.5 horas/mes | 2 horas/mes |
| Tiempo de recuperación (RTO) | < 15 minutos | < 30 minutos |
| Punto de recuperación (RPO) | < 5 minutos | < 15 minutos |
| Tiempo entre fallos (MTBF) | > 720 horas | > 360 horas |
| Tiempo de reparación (MTTR) | < 30 minutos | < 60 minutos |

### Cálculo de disponibilidad

```
Disponibilidad = (Tiempo total - Tiempo inactivo) / Tiempo total x 100

99.5% = máximo 3.6 horas de inactividad al mes
99.9% = máximo 43.8 minutos de inactividad al mes
```

### Estrategias de alta disponibilidad

| Componente | Estrategia | Descripción |
|------------|-----------|-------------|
| API (FastAPI) | Múltiples instancias | Mínimo 2 instancias activas con balanceo de carga |
| Base de datos (PostgreSQL) | Réplica streaming | Primary + 1 réplica con failover automático |
| Almacenamiento | Redundancia | Almacenamiento con replicación en 2 zonas |
| Motor OCR | Fallback | Google Cloud Vision + Tesseract local |
| Colas de procesamiento | Persistencia | Cola persistente con confirmación de procesamiento |

### Ventana de mantenimiento

- Horario preferido: Domingos 02:00 - 06:00 (hora local).
- Notificación previa: mínimo 48 horas.
- Actualizaciones críticas de seguridad: aplicación inmediata sin ventana planificada.

---

## RNF-03: Escalabilidad

| Campo | Detalle |
|-------|---------|
| **ID** | RNF-03 |
| **Categoría** | Escalabilidad (Scalability) |
| **Prioridad** | Media |
| **Verificable** | Si |

### Descripción

El sistema debe escalar horizontalmente para soportar un crecimiento futuro de hasta 5000 documentos diarios sin cambios arquitectónicos significativos.

### Métricas y umbrales

| Métrica | Valor actual | Proyección 1 año | Proyección 3 años |
|---------|-------------|-------------------|-------------------|
| Documentos/día | 1000 | 2500 | 5000 |
| Usuarios concurrentes | 20 | 50 | 100 |
| Almacenamiento mensual | 50 GB | 125 GB | 250 GB |
| Registros de auditoría | 100K/mes | 250K/mes | 500K/mes |

### Estrategia de escalamiento

| Componente | Tipo de escalamiento | Mecanismo |
|------------|---------------------|-----------|
| API FastAPI | Horizontal | Contenedores Docker con auto-scaling basado en CPU/memoria |
| Workers OCR | Horizontal | Pool de workers escalable según tamaño de cola |
| PostgreSQL | Vertical (corto plazo) / Horizontal (largo plazo) | Upgrade de instancia + particionamiento de tablas |
| Frontend React | Horizontal | CDN + múltiples instancias con balanceo |
| Almacenamiento | Horizontal | Object storage con auto-scaling |

### Reglas de auto-scaling

```yaml
autoscaling:
  api_service:
    min_replicas: 2
    max_replicas: 8
    target_cpu_utilization: 70%
    scale_up_cooldown: 60s
    scale_down_cooldown: 300s

  ocr_workers:
    min_replicas: 2
    max_replicas: 16
    target_queue_length: 50  # documentos en cola
    scale_up_cooldown: 30s
    scale_down_cooldown: 600s
```

### Particionamiento de base de datos

```sql
-- Particionamiento por rango de fecha para documentos
CREATE TABLE documents (
    id UUID NOT NULL,
    created_at TIMESTAMPTZ NOT NULL,
    document_type VARCHAR(50),
    status VARCHAR(50),
    -- ... otros campos
) PARTITION BY RANGE (created_at);

CREATE TABLE documents_2026_q1 PARTITION OF documents
    FOR VALUES FROM ('2026-01-01') TO ('2026-04-01');
CREATE TABLE documents_2026_q2 PARTITION OF documents
    FOR VALUES FROM ('2026-04-01') TO ('2026-07-01');
```

---

## RNF-04: Seguridad

| Campo | Detalle |
|-------|---------|
| **ID** | RNF-04 |
| **Categoría** | Seguridad (Security) |
| **Prioridad** | Alta |
| **Verificable** | Si |

### Descripción

El sistema debe implementar controles de seguridad en todas las capas (autenticación, autorización, cifrado, auditoría) para proteger los datos operativos y cumplir con los estándares de seguridad aplicables.

### Controles de seguridad

#### Autenticación

| Control | Descripción | Estándar |
|---------|-------------|----------|
| Protocolo | OAuth 2.0 con flujo Authorization Code + PKCE | RFC 6749, RFC 7636 |
| Tokens | JWT con expiración de 1 hora, refresh token de 8 horas | RFC 7519 |
| MFA | Autenticación multifactor obligatoria para roles Administrador y Auditor | OWASP |
| Intentos fallidos | Bloqueo de cuenta tras 5 intentos fallidos (15 minutos) | OWASP |
| Contraseñas | Mínimo 12 caracteres, 1 mayúscula, 1 número, 1 símbolo especial | NIST SP 800-63B |

#### Autorización (RBAC)

| Rol | Documentos | Revisión | Configuración | Auditoría | Reportes |
|-----|-----------|----------|--------------|-----------|----------|
| Operador | Cargar, Ver propios | - | - | - | Propios |
| Revisor | Ver asignados | Revisar, Aprobar, Rechazar | - | - | De revisión |
| Administrador | Todos | Todos | Completa | Ver | Todos |
| Auditor | Ver (solo lectura) | Ver (solo lectura) | Ver (solo lectura) | Completa | Todos |

#### Cifrado

| Ámbito | Algoritmo | Detalle |
|--------|-----------|---------|
| Datos en tránsito | TLS 1.3 | Todas las comunicaciones HTTP/API |
| Datos en reposo | AES-256-GCM | Documentos almacenados y datos sensibles en BD |
| Tokens | RS256 | Firma de JWT |
| Hashes | SHA-256 | Integridad de documentos y registros de auditoría |
| Credenciales ERP | AES-256 | Almacenamiento de credenciales de integración |

#### Seguridad de la API

| Control | Descripción |
|---------|-------------|
| Rate limiting | 100 requests/minuto por usuario, 1000/minuto global |
| CORS | Orígenes permitidos configurables |
| Headers de seguridad | X-Content-Type-Options, X-Frame-Options, CSP, HSTS |
| Validación de entrada | Sanitización de todos los inputs, prevención de SQL injection y XSS |
| Tamaño de payload | Máximo 50 MB por request |

### Pruebas de seguridad requeridas

| Tipo | Frecuencia | Responsable |
|------|-----------|-------------|
| Análisis de vulnerabilidades (SAST) | En cada despliegue | Pipeline CI/CD |
| Análisis de dependencias (SCA) | Semanal | Pipeline CI/CD |
| Pruebas de penetración | Semestral | Equipo externo |
| Revisión de código seguro | En cada PR | Equipo de desarrollo |

---

## RNF-05: Usabilidad

| Campo | Detalle |
|-------|---------|
| **ID** | RNF-05 |
| **Categoría** | Usabilidad (Usability) |
| **Prioridad** | Media |
| **Verificable** | Si |

### Descripción

El sistema debe proporcionar una interfaz intuitiva y eficiente que permita a los usuarios realizar sus tareas con mínima capacitación y reducido margen de error.

### Métricas y umbrales

| Métrica | Valor objetivo | Método de medición |
|---------|---------------|-------------------|
| Tiempo de aprendizaje (Operador) | < 2 horas | Prueba con usuarios nuevos |
| Tiempo de aprendizaje (Revisor) | < 4 horas | Prueba con usuarios nuevos |
| Tasa de error de usuario | < 5% | Porcentaje de acciones erróneas / acciones totales |
| Satisfacción del usuario (SUS) | >= 75/100 | Cuestionario System Usability Scale |
| Tareas completadas sin ayuda | >= 90% | Prueba de usabilidad |
| Clics para completar tarea principal | <= 5 clics | Análisis de flujo de usuario |

### Principios de diseño

| Principio | Aplicación |
|-----------|-----------|
| Consistencia | Componentes UI uniformes en todas las pantallas (React 18 component library) |
| Retroalimentación | Mensajes de éxito/error claros, barras de progreso, indicadores de estado |
| Prevención de errores | Validación en tiempo real, confirmación para acciones destructivas |
| Reconocimiento | Iconos descriptivos, labels claros, estados visuales diferenciados por color |
| Accesibilidad | Cumplimiento WCAG 2.1 nivel AA |
| Responsividad | Diseño adaptable a pantallas de 1024px a 2560px |

### Requisitos de accesibilidad (WCAG 2.1 AA)

| Criterio | Descripción |
|----------|-------------|
| Contraste | Ratio mínimo 4.5:1 para texto normal, 3:1 para texto grande |
| Navegación por teclado | Todas las funciones accesibles sin ratón |
| Etiquetas ARIA | Componentes interactivos con etiquetas descriptivas |
| Textos alternativos | Imágenes e iconos con atributo alt descriptivo |
| Tamaño de fuente | Mínimo 14px, escalable hasta 200% sin pérdida de funcionalidad |

---

## RNF-06: Mantenibilidad

| Campo | Detalle |
|-------|---------|
| **ID** | RNF-06 |
| **Categoría** | Mantenibilidad (Maintainability) |
| **Prioridad** | Media |
| **Verificable** | Si |

### Descripción

El sistema debe ser fácil de mantener, actualizar y extender, siguiendo las mejores prácticas de desarrollo de software y documentación.

### Métricas y umbrales

| Métrica | Valor objetivo | Método de medición |
|---------|---------------|-------------------|
| Cobertura de tests | >= 80% | pytest --cov (backend), jest --coverage (frontend) |
| Complejidad ciclomática máxima | <= 10 por función | Análisis estático (radon, SonarQube) |
| Tiempo de despliegue | < 15 minutos | Pipeline CI/CD end-to-end |
| Tiempo para agregar nuevo tipo de documento | < 8 horas de desarrollo | Medición de esfuerzo |
| Documentación de API | 100% endpoints documentados | OpenAPI/Swagger coverage |
| Deuda técnica | < 5% del código total | SonarQube technical debt ratio |

### Stack tecnológico

| Capa | Tecnología | Versión |
|------|-----------|---------|
| Backend | Python | 3.11 |
| Framework API | FastAPI | 0.100+ |
| OCR principal | Google Cloud Vision API | v1 |
| OCR respaldo | Tesseract | 5.x |
| Base de datos | PostgreSQL | 15 |
| Frontend | React | 18 |
| ORM | SQLAlchemy | 2.x |
| Migraciones | Alembic | 1.x |
| Tests backend | pytest | 7.x |
| Tests frontend | Jest + React Testing Library | 29.x / 14.x |
| Contenedores | Docker | 24.x |
| CI/CD | GitHub Actions | - |

### Estándares de código

```yaml
backend:
  linter: ruff
  formatter: black (line-length: 120)
  type_checker: mypy (strict mode)
  docstrings: Google style
  naming: snake_case (funciones/variables), PascalCase (clases)

frontend:
  linter: eslint (airbnb config)
  formatter: prettier
  type_checker: TypeScript strict
  naming: camelCase (funciones/variables), PascalCase (componentes)

git:
  branching: GitFlow
  commits: Conventional Commits (feat:, fix:, docs:, refactor:, test:)
  reviews: Mínimo 1 aprobación requerida
```

---

## RNF-07: Compatibilidad

| Campo | Detalle |
|-------|---------|
| **ID** | RNF-07 |
| **Categoría** | Compatibilidad (Compatibility) |
| **Prioridad** | Media |
| **Verificable** | Si |

### Descripción

El sistema debe ser compatible con los navegadores, sistemas operativos y formatos de documento requeridos por la organización.

### Compatibilidad de navegadores

| Navegador | Versión mínima | Soporte |
|-----------|---------------|---------|
| Google Chrome | 100+ | Completo |
| Mozilla Firefox | 100+ | Completo |
| Microsoft Edge | 100+ | Completo |
| Safari | 16+ | Completo |
| Internet Explorer | - | No soportado |

### Compatibilidad de formatos de documento

| Formato | Extensiones | Soporte | Notas |
|---------|------------|---------|-------|
| PDF | `.pdf` | Completo | PDF/A, PDF 1.4-2.0, PDF escaneado |
| PNG | `.png` | Completo | 8-bit y 24-bit, con/sin alpha |
| JPEG | `.jpg`, `.jpeg` | Completo | Baseline y progressive |
| TIFF | `.tiff`, `.tif` | Completo | Single y multi-page, compresión LZW/ZIP |

### Compatibilidad de resolución de documentos

| Resolución | Soporte | Calidad esperada |
|-----------|---------|------------------|
| >= 300 DPI | Óptimo | Precisión >= 95% |
| 200-299 DPI | Aceptable | Precisión >= 90% |
| 150-199 DPI | Mínimo | Precisión >= 80%, pre-procesamiento automático |
| < 150 DPI | Degradado | Se advierte al usuario, puede requerir revisión manual |

### Compatibilidad de integración

| Sistema | Protocolo | Versión |
|---------|-----------|---------|
| SAP S/4HANA | OData v4 | 4.01 |
| SAP ECC | OData v2 | Compatible con adaptador |
| API REST externa | HTTP/HTTPS | 1.1 / 2.0 |
| Almacenamiento | S3 Compatible | v4 signing |

---

## RNF-08: Confiabilidad

| Campo | Detalle |
|-------|---------|
| **ID** | RNF-08 |
| **Categoría** | Confiabilidad (Reliability) |
| **Prioridad** | Alta |
| **Verificable** | Si |

### Descripción

El sistema debe garantizar cero pérdida de datos en todas las operaciones, con mecanismos de recuperación ante fallos y respaldo automático.

### Métricas y umbrales

| Métrica | Valor objetivo | Valor mínimo aceptable |
|---------|---------------|------------------------|
| Pérdida de datos | 0 (cero) | 0 (cero) |
| Integridad de documentos | 100% | 100% |
| Éxito en procesamiento OCR | >= 99% | >= 97% |
| Éxito en sincronización ERP | >= 98% | >= 95% |
| Backups exitosos | 100% | 100% |

### Estrategias de confiabilidad

| Estrategia | Implementación |
|-----------|----------------|
| Transacciones atómicas | Todas las operaciones de base de datos en transacciones con rollback automático |
| Idempotencia | Endpoints de API idempotentes para reintento seguro |
| Cola persistente | Cola de procesamiento con confirmación (at-least-once delivery) |
| Verificación de integridad | Hash SHA-256 para cada documento almacenado, verificación periódica |
| Backup automático | Base de datos: backup completo diario, incremental cada hora |
| Replicación | PostgreSQL streaming replication con réplica síncrona |

### Política de backups

| Tipo | Frecuencia | Retención | Ubicación |
|------|-----------|-----------|-----------|
| Backup completo BD | Diario (02:00) | 30 días | Almacenamiento remoto cifrado |
| Backup incremental BD | Cada hora | 7 días | Almacenamiento remoto cifrado |
| Backup documentos | Diario (03:00) | 365 días | Almacenamiento georedundante |
| Backup configuración | En cada cambio | Ilimitado (versionado) | Repositorio Git + almacenamiento remoto |

### Manejo de errores

```python
# Ejemplo de patrón de manejo de errores con reintento
from tenacity import retry, stop_after_attempt, wait_exponential

@retry(
    stop=stop_after_attempt(3),
    wait=wait_exponential(multiplier=1, min=4, max=60),
    reraise=True
)
async def process_document_ocr(document_id: str) -> OCRResult:
    """
    Procesa un documento con OCR con reintentos automáticos.
    En caso de fallo permanente, el documento se marca como error
    y se notifica al administrador.
    """
    try:
        result = await ocr_engine.process(document_id)
        return result
    except OCRTemporaryError as e:
        logger.warning(f"Error temporal OCR para {document_id}: {e}")
        raise  # Reintenta
    except OCRPermanentError as e:
        logger.error(f"Error permanente OCR para {document_id}: {e}")
        await mark_document_error(document_id, str(e))
        await notify_admin(document_id, str(e))
        raise  # No reintenta (error permanente)
```

---

## RNF-09: Compliance (GDPR)

| Campo | Detalle |
|-------|---------|
| **ID** | RNF-09 |
| **Categoría** | Cumplimiento normativo (Compliance) |
| **Prioridad** | Alta |
| **Verificable** | Si |

### Descripción

El sistema debe cumplir con el Reglamento General de Protección de Datos (GDPR) y las normativas locales de protección de datos aplicables.

### Requisitos GDPR

| Artículo | Requisito | Implementación |
|----------|-----------|----------------|
| Art. 5 | Principio de minimización | Solo recopilar datos necesarios para el procesamiento OCR |
| Art. 6 | Base legal para procesamiento | Consentimiento o interés legítimo documentado |
| Art. 12-14 | Derecho de información | Política de privacidad accesible, aviso de procesamiento |
| Art. 15 | Derecho de acceso | API/interfaz para que usuarios consulten sus datos |
| Art. 16 | Derecho de rectificación | Funcionalidad para corregir datos personales |
| Art. 17 | Derecho al olvido | Funcionalidad de eliminación completa de datos y documentos |
| Art. 20 | Portabilidad de datos | Exportación de datos en formato estructurado (JSON, CSV) |
| Art. 25 | Privacidad por diseño | Cifrado, pseudonimización, minimización de datos |
| Art. 30 | Registro de actividades | Registro completo de tratamiento de datos personales |
| Art. 32 | Seguridad del tratamiento | Cifrado AES-256, TLS 1.3, control de acceso |
| Art. 33 | Notificación de brechas | Proceso documentado, notificación en < 72 horas |
| Art. 35 | Evaluación de impacto (DPIA) | DPIA documentada para el procesamiento OCR |

### Datos personales identificados en documentos

| Tipo de dato | Categoría GDPR | Tratamiento |
|-------------|----------------|-------------|
| Nombres de personas | Dato personal | Cifrado en reposo, acceso restringido |
| RUC/NIF | Dato personal | Cifrado en reposo, acceso restringido |
| Direcciones | Dato personal | Cifrado en reposo, acceso restringido |
| Firmas | Dato personal | Cifrado en reposo, eliminación tras procesamiento si no es necesario |
| Emails | Dato personal | Cifrado en reposo, acceso restringido |

### Retención de datos

| Tipo de dato | Período de retención | Acción al expirar |
|-------------|---------------------|--------------------|
| Documentos originales | 1 año (configurable) | Eliminación segura (overwrite + delete) |
| Datos extraídos | 2 años | Anonimización o eliminación |
| Registros de auditoría | 2 años | Archivado cifrado o eliminación |
| Datos de usuario | Mientras la cuenta esté activa + 6 meses | Eliminación completa |
| Backups con datos personales | Mismo período que datos originales | Eliminación segura |

### Proceso de eliminación segura

```
1. Solicitud de eliminación recibida (usuario o política de retención)
2. Verificación de identidad del solicitante
3. Identificación de todos los datos asociados (documentos, extracciones, auditoría)
4. Eliminación de datos en base de datos (DELETE + VACUUM)
5. Eliminación de documentos originales (overwrite con datos aleatorios + delete)
6. Eliminación de datos en backups (en siguiente rotación de backup)
7. Generación de certificado de eliminación
8. Registro del evento de eliminación en auditoría (sin datos personales)
```

---

## RNF-10: Monitoreo y Observabilidad

| Campo | Detalle |
|-------|---------|
| **ID** | RNF-10 |
| **Categoría** | Monitoreo y Observabilidad (Monitoring & Observability) |
| **Prioridad** | Media |
| **Verificable** | Si |

### Descripción

El sistema debe implementar monitoreo integral con métricas, logs estructurados, trazas distribuidas y alertas para garantizar la visibilidad operativa y la detección temprana de problemas.

### Los tres pilares de observabilidad

#### 1. Métricas

| Métrica | Tipo | Descripción | Alerta |
|---------|------|-------------|--------|
| `ocr_processing_duration_seconds` | Histograma | Tiempo de procesamiento OCR | > 5s (warning), > 10s (critical) |
| `ocr_confidence_score` | Histograma | Distribución de puntajes de confianza | Media < 0.70 (warning) |
| `documents_processed_total` | Contador | Total de documentos procesados | < 40/hora en horario laboral (warning) |
| `erp_sync_failures_total` | Contador | Fallos de sincronización ERP | > 5 en 1 hora (critical) |
| `manual_review_queue_size` | Gauge | Tamaño de la cola de revisión | > 100 documentos (warning) |
| `api_request_duration_seconds` | Histograma | Latencia de endpoints API | P95 > 500ms (warning) |
| `api_errors_total` | Contador | Errores HTTP 5xx | > 10 en 5 minutos (critical) |
| `disk_usage_percent` | Gauge | Uso de disco del almacenamiento | > 80% (warning), > 90% (critical) |
| `db_connections_active` | Gauge | Conexiones activas a PostgreSQL | > 80% del pool (warning) |

#### 2. Logs estructurados

```json
{
  "timestamp": "2026-02-24T10:30:05.123Z",
  "level": "INFO",
  "service": "ocr-processor",
  "trace_id": "abc123def456",
  "span_id": "789ghi012",
  "message": "Document processed successfully",
  "document_id": "a3f7c2e1-...",
  "ocr_engine": "google_cloud_vision",
  "processing_time_ms": 2340,
  "confidence": 0.94,
  "document_type": "invoice"
}
```

| Nivel | Uso | Retención |
|-------|-----|-----------|
| DEBUG | Desarrollo y troubleshooting detallado | 3 días |
| INFO | Operaciones normales del sistema | 30 días |
| WARNING | Situaciones anómalas que no impiden operación | 90 días |
| ERROR | Errores que impactan funcionalidad | 180 días |
| CRITICAL | Fallos que requieren intervención inmediata | 365 días |

#### 3. Trazas distribuidas

| Componente | Instrumentación |
|-----------|----------------|
| FastAPI | OpenTelemetry middleware automático |
| Google Cloud Vision | Span personalizado con duración y resultado |
| Tesseract | Span personalizado con duración y resultado |
| PostgreSQL | SQLAlchemy instrumentation |
| HTTP Client (SAP) | httpx instrumentation |

### Alertas y notificaciones

| Severidad | Canal | Tiempo de respuesta esperado |
|-----------|-------|------------------------------|
| Critical | Email + SMS + Slack | < 15 minutos |
| Warning | Email + Slack | < 1 hora |
| Info | Slack | Siguiente día hábil |

### Dashboard de monitoreo

| Panel | Métricas mostradas |
|-------|-------------------|
| Salud general | Uptime, latencia API, errores 5xx, uso CPU/memoria |
| Procesamiento OCR | Documentos/hora, confianza promedio, tiempo procesamiento, motor usado |
| Cola de revisión | Tamaño cola, tiempo promedio revisión, documentos por revisor |
| Integración ERP | Sincronizaciones exitosas/fallidas, latencia, reintentos |
| Almacenamiento | Uso de disco, documentos almacenados, tasa de crecimiento |

### Health check endpoint

```
GET /api/v1/health

Response (200 OK):
{
  "status": "healthy",
  "version": "1.0.0",
  "timestamp": "2026-02-24T10:30:05Z",
  "checks": {
    "database": {"status": "up", "response_time_ms": 5},
    "google_cloud_vision": {"status": "up", "response_time_ms": 120},
    "tesseract": {"status": "up", "response_time_ms": 15},
    "storage": {"status": "up", "available_gb": 450},
    "erp_connection": {"status": "up", "response_time_ms": 230}
  }
}
```

---

## Matriz de Requerimientos No Funcionales

| ID | Categoría | Prioridad | Métrica clave | Valor objetivo | RF relacionados |
|----|-----------|-----------|---------------|---------------|----------------|
| RNF-01 | Rendimiento | Alta | Tiempo procesamiento OCR | < 5s/doc | RF-02, RF-10 |
| RNF-02 | Disponibilidad | Alta | Uptime mensual | >= 99.5% | Todos |
| RNF-03 | Escalabilidad | Media | Throughput futuro | 5000 docs/día | RF-02, RF-10 |
| RNF-04 | Seguridad | Alta | Vulnerabilidades críticas | 0 | Todos |
| RNF-05 | Usabilidad | Media | SUS Score | >= 75/100 | RF-06, RF-07 |
| RNF-06 | Mantenibilidad | Media | Cobertura de tests | >= 80% | Todos |
| RNF-07 | Compatibilidad | Media | Navegadores soportados | Chrome, Firefox, Edge, Safari | RF-06, RF-07 |
| RNF-08 | Confiabilidad | Alta | Pérdida de datos | 0 | RF-01, RF-02, RF-05 |
| RNF-09 | Compliance | Alta | Cumplimiento GDPR | 100% artículos aplicables | RF-09 |
| RNF-10 | Monitoreo | Media | Cobertura de alertas | 100% métricas críticas | Todos |

---

*Documento generado el 2026-02-24 por IDC Ingeniería.*
