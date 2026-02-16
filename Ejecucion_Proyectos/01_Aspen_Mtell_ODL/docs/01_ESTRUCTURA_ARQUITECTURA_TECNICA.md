# ASPEN MTELL ODL - ARQUITECTURA TÉCNICA Y ESTRUCTURA DE CARPETAS

**Versión**: 1.0  
**Documento**: 01_ESTRUCTURA_ARQUITECTURA_TECNICA.md  
**Propósito**: Definir arquitectura técnica, estructura de directorios, convenciones y mejores prácticas de desarrollo

---

## 1. ARQUITECTURA DE SOLUCIÓN

### 1.1 Diagrama C4 - Contexto Nivel 1
```
┌──────────────────────────────────────────────────────────────┐
│                    ASPEN MTELL PLATFORM                       │
│                     (Confiabilidad)                           │
└──────────────────┬────────────────────────────────────────────┘
                   │
      ┌────────────┼────────────┐
      │            │            │
   ┌──▼─┐     ┌───▼────┐  ┌───▼────┐
   │SCADA│     │  ERP   │  │  BI    │
   │Data │     │(Maint.)│  │(Power) │
   └─────┘     └────────┘  └────────┘
      │
   ┌──▼──────────────────────────┐
   │  Mtell Data Integration     │
   │ (Historiadores, APIs)       │
   └──┬───────────────────────────┘
      │
   ┌──▼──────────────────────────┐
   │ Mtell Analytics Engine      │
   │ (Modelos Predictivos)       │
   └──┬───────────────────────────┘
      │
   ┌──▼──────────────────────────────────┐
   │     Dashboards & Reporting          │
   │ (Operacional, Mantenimiento, Exec)  │
   └─────────────────────────────────────┘
      │
   ┌──▼──────────────────────────────────┐
   │        Users & Applications         │
   │ (Operators, Maintenance, Managers)  │
   └──────────────────────────────────────┘
```

### 1.2 Arquitectura de Capas Técnicas

```
PRESENTACIÓN
├─ Dashboards Operacionales (Real-time)
├─ Dashboards Mantenimiento (Planificación)
├─ Dashboards Ejecutivos (KPIs)
└─ Alertas & Notificaciones

APLICACIÓN / LÓGICA
├─ APIs REST de Mtell
├─ Motor de Reglas (Alertas)
├─ Gestión de Usuarios & Seguridad
└─ Integración de Workflows

DATOS & ANÁLISIS
├─ Data Warehouse (Consolidación)
├─ Modelos Predictivos (RUL, Anomalías)
├─ Machine Learning Engines
└─ Data Quality & Governance

INTEGRACIÓN
├─ Conectores SCADA (Historiadores)
├─ Conectores ERP (Mantenimiento)
├─ APIs de 3ros
└─ File-based Imports/Exports

INFRAESTRUCTURA
├─ Servidores Mtell (On-premise/Cloud)
├─ Bases de Datos
├─ Almacenamiento Histórico
├─ Networking & Seguridad
└─ Backup & DR
```

---

## 2. ESTRUCTURA DE DOCUMENTACIÓN DEL PROYECTO (PMI & Procesos)

**Ubicación**: `/docs/`  
**Propósito**: Separar documentación de proyecto (PMI, procesos, decisiones) del código técnico  
**Referencia Completa**: Ver `GUIA_ESTRUCTURA_DOCUMENTACION_PROYECTOS.md` en Base_Proyectos

### Carpetas de Documentación:

#### `/docs/project_management/`
- **PROJECT_CHARTER.md** - Autorización formal del proyecto (firmado)
- **SCOPE_STATEMENT.md** - Qué está IN/OUT del proyecto
- **STAKEHOLDER_MANAGEMENT.md** - Análisis y estrategia de stakeholders
- **RISK_REGISTER.md** - Registro de riesgos (VIVO - actualizar semanalmente)
- **CHANGE_LOG.md** - Cambios aprobados y su impacto
- **COMMUNICATIONS_PLAN.md** - Cadencia y audiencias de reportes
- **WEEKLY_STATUS.md** - Reportes de avance (template)
- **LESSONS_LEARNED.md** - Lecciones (actualizar bi-weekly)

#### `/docs/architecture_decisions/`
- **ADR_0001_[DECISION].md**, **ADR_0002_[DECISION].md**, etc.
- Documentar PORQUÉ se tomó cada decisión técnica importante
- Alternativas consideradas, consecuencias

#### `/docs/requirements/`
- **FUNCTIONAL_REQUIREMENTS.md** - Qué hace exactamente el sistema
- **NON_FUNCTIONAL_REQUIREMENTS.md** - Performance, seguridad, escalabilidad
- **USER_STORIES.md** - Historias de usuario con criterios de aceptación

#### `/docs/testing/`
- **TEST_PLAN.md** - Estrategia de testing
- **TEST_CASES.md** - Casos de prueba
- **UAT_PLAN.md** - User Acceptance Testing
- **TEST_RESULTS.md** - Resultados (VIVO - actualizar daily/weekly)

#### `/docs/operations/`
- **RUNBOOK.md** - Instrucciones step-by-step para Ops (copy-paste ready)
- **PLAYBOOK.md** - Qué hacer si hay crisis
- **DEPLOYMENT_CHECKLIST.md** - Validaciones pre/post deployment
- **TROUBLESHOOTING_GUIDE.md** - Problemas comunes y soluciones

#### `/docs/stakeholder_communication/`
- **WEEKLY_STATUS_TEMPLATE.md** - Template para reportes
- **MONTHLY_REPORTS/** - Carpeta con reportes históricos
- **STEERING_MEETING_NOTES/** - Actas de reuniones

#### `/docs/compliance/`
- **SECURITY_CHECKLIST.md** - Cumplimiento de seguridad
- **COMPLIANCE_REQUIREMENTS.md** - Regulatorio (GDPR, etc)

### Actualización de Documentos (Cadencia):

| Documento | Frecuencia | Owner |
|-----------|-----------|-------|
| RISK_REGISTER | Semanalmente (viernes) | PM |
| CHANGE_LOG | Ad-hoc (cuando se aprueba) | PM |
| TEST_RESULTS | Diaria/Semanal | QA |
| WEEKLY_STATUS | Semanal (viernes) | PM |
| LESSONS_LEARNED | Bi-weekly + final | PM |

### Separación de Código vs Documentación:

```
/docs/                          ← DOCUMENTACIÓN (PMI, procesos, decisiones)
    ├── project_management/     ← Gobernanza
    ├── architecture_decisions/ ← Decisiones técnicas
    ├── requirements/           ← Especificaciones
    ├── testing/                ← Planes y resultados
    └── operations/             ← Runbooks y playbooks

/src/                           ← CÓDIGO (no documentación)
/tests/                         ← TESTS (no documentación)
/config/                        ← CONFIGURACIÓN (no documentación)
```

Esta separación garantiza:
✅ Documentación accesible sin navegar código
✅ Cumplimiento con estándares PMI
✅ Auditable y traceable
✅ Fácil para nuevos miembros del equipo
✅ Histórico y lecciones capturadas

---

## 3. ESTRUCTURA DE CARPETAS DEL PROYECTO

```
01_Aspen_Mtell_ODL/
│
├── docs/                                    # Documentación del Proyecto
│   ├── 00_VISION_Y_GOBERNANZA.md
│   ├── 01_ESTRUCTURA_ARQUITECTURA_TECNICA.md
│   ├── 02_PLAN_DE_TRABAJO_DETALLADO.md
│   ├── 03_GUIA_IMPLEMENTACION_PASO_A_PASO.md
│   ├── 04_CRITERIOS_DE_EXITO_Y_CHECKLIST_MILESTONES.md
│   ├── 05_METODOLOGIA_Y_ABORDAJE_ESTRATEGICO.md
│   ├── ACEPTACION_FINAL.md                 # Actas de aprobación
│   └── CAMBIOS_Y_LECCIONES.md              # Registro de cambios
│
├── config/                                  # Configuraciones Mtell
│   ├── mtell_environment_setup/
│   │   ├── system_configuration.yaml
│   │   ├── database_config.ini
│   │   ├── network_settings.conf
│   │   └── security_policies.xml
│   │
│   ├── data_integrations/
│   │   ├── scada_connector_config.yaml
│   │   │   └── [Historiador 1, 2, 3...]
│   │   ├── erp_connector_config.yaml
│   │   │   └── [SAP, Oracle, custom...]
│   │   └── api_integrations.json
│   │
│   ├── asset_configuration/
│   │   ├── equipment_master.xlsx
│   │   ├── equipment_hierarchy.xml
│   │   ├── equipment_specs.csv
│   │   └── sensor_mappings.json
│   │
│   └── models_config/
│       ├── predictive_models.yaml
│       ├── alert_rules.xml
│       └── kpi_definitions.json
│
├── integrations/                            # Código de Integración
│   ├── scada_integration/
│   │   ├── historiador_connector.py
│   │   ├── data_extraction.py
│   │   ├── validation_rules.py
│   │   └── logging_config.yaml
│   │
│   ├── erp_integration/
│   │   ├── maintenance_sync.py
│   │   ├── work_order_processor.py
│   │   ├── inventory_sync.py
│   │   └── error_handling.py
│   │
│   ├── data_transformation/
│   │   ├── etl_pipeline.py
│   │   ├── data_cleaning.py
│   │   ├── unit_conversion.py
│   │   └── quality_checks.py
│   │
│   └── api_connectors/
│       ├── mtell_api_wrapper.py
│       ├── custom_extensions.py
│       └── third_party_integrations/
│
├── analytics/                               # Análisis & Modelos
│   ├── predictive_models/
│   │   ├── rul_models.py                  # Remaining Useful Life
│   │   ├── anomaly_detection.py           # Detección anormalías
│   │   ├── trend_analysis.py              # Análisis tendencias
│   │   └── model_validation.py
│   │
│   ├── statistical_analysis/
│   │   ├── descriptive_stats.py
│   │   ├── correlation_analysis.py
│   │   └── hypothesis_testing.py
│   │
│   ├── dashboards/
│   │   ├── operational_dashboard.json     # Real-time monitoring
│   │   ├── maintenance_dashboard.json     # Planificación
│   │   ├── executive_dashboard.json       # KPIs & ROI
│   │   └── custom_reports.py
│   │
│   └── model_training/
│       ├── training_scripts.py
│       ├── feature_engineering.py
│       ├── model_selection.py
│       └── hyperparameter_tuning.py
│
├── deployment/                              # Configuración Deployment
│   ├── docker/
│   │   ├── Dockerfile                     # Container image
│   │   ├── docker-compose.yml
│   │   └── .dockerignore
│   │
│   ├── kubernetes/
│   │   ├── mtell_deployment.yaml
│   │   ├── services.yaml
│   │   ├── configmaps.yaml
│   │   └── secrets.yaml
│   │
│   ├── terraform/
│   │   ├── main.tf                        # Infrastructure as Code
│   │   ├── variables.tf
│   │   ├── outputs.tf
│   │   └── environments/
│   │       ├── dev.tfvars
│   │       ├── staging.tfvars
│   │       └── prod.tfvars
│   │
│   ├── ci_cd/
│   │   ├── .github/workflows/
│   │   │   ├── build.yml
│   │   │   ├── test.yml
│   │   │   ├── deploy.yml
│   │   │   └── validation.yml
│   │   ├── ansible/
│   │   │   ├── site.yml
│   │   │   └── roles/
│   │   │       ├── mtell_install/
│   │   │       ├── data_integration/
│   │   │       └── security_hardening/
│   │   └── scripts/
│   │       ├── deploy.sh
│   │       ├── health_check.sh
│   │       └── rollback.sh
│   │
│   └── monitoring/
│       ├── prometheus_config.yaml
│       ├── grafana_dashboards.json
│       └── alerts.yaml
│
├── testing/                                 # Testing & QA
│   ├── unit_tests/
│   │   ├── test_integrations.py
│   │   ├── test_data_quality.py
│   │   ├── test_calculations.py
│   │   └── conftest.py
│   │
│   ├── integration_tests/
│   │   ├── test_scada_integration.py
│   │   ├── test_erp_integration.py
│   │   ├── test_end_to_end.py
│   │   └── test_data_flow.py
│   │
│   ├── performance_tests/
│   │   ├── load_testing.py
│   │   ├── stress_testing.yaml
│   │   └── benchmarks.py
│   │
│   ├── acceptance_tests/
│   │   ├── user_story_tests.robot       # Robot Framework
│   │   ├── ui_automation.py
│   │   └── acceptance_criteria.md
│   │
│   └── test_data/
│       ├── sample_scada_data.csv
│       ├── sample_equipment.json
│       ├── test_scenarios.yaml
│       └── golden_dataset.parquet
│
├── data/                                    # Datos del Proyecto
│   ├── raw/
│   │   ├── scada_extracts/               # Raw data from SCADA
│   │   ├── erp_exports/
│   │   └── manual_inputs/
│   │
│   ├── processed/
│   │   ├── validated_data/               # Validated & cleaned
│   │   ├── aggregated_data/              # Pre-calculated metrics
│   │   └── model_inputs/
│   │
│   ├── model_outputs/
│   │   ├── predictions/
│   │   ├── anomalies/
│   │   └── recommendations/
│   │
│   └── metadata/
│       ├── data_dictionary.json
│       ├── quality_metrics.csv
│       └── lineage_documentation.md
│
├── tools_scripts/                          # Scripts Operacionales
│   ├── initialization/
│   │   ├── setup_environment.sh
│   │   ├── load_master_data.py
│   │   └── validate_connectivity.py
│   │
│   ├── maintenance/
│   │   ├── backup_management.sh
│   │   ├── database_optimization.sql
│   │   ├── log_rotation.yaml
│   │   └── health_monitoring.py
│   │
│   ├── data_management/
│   │   ├── data_import_template.py
│   │   ├── bulk_data_loader.py
│   │   ├── data_export.py
│   │   └── data_archive.sh
│   │
│   ├── reporting/
│   │   ├── generate_monthly_report.py
│   │   ├── export_to_powerbi.py
│   │   └── email_reports.sh
│   │
│   └── troubleshooting/
│       ├── debug_data_issues.py
│       ├── validation_report.py
│       └── performance_analysis.sql
│
├── security/                                # Seguridad & Compliance
│   ├── ssl_certificates/
│   │   ├── server.crt
│   │   ├── server.key
│   │   └── ca_bundle.crt
│   │
│   ├── access_control/
│   │   ├── rbac_definitions.yaml
│   │   ├── user_provisioning.sql
│   │   └── audit_log_config.yaml
│   │
│   ├── encryption/
│   │   ├── encryption_policies.yaml
│   │   └── key_management.txt
│   │
│   └── compliance/
│       ├── security_checklist.md
│       ├── data_retention_policy.md
│       └── audit_trail_requirements.md
│
├── documentation/                          # Documentación Técnica
│   ├── architecture_diagrams/
│   │   ├── system_architecture.png
│   │   ├── data_flow_diagram.png
│   │   ├── deployment_architecture.png
│   │   └── integration_diagram.png
│   │
│   ├── technical_specs/
│   │   ├── api_documentation.md           # OpenAPI/Swagger
│   │   ├── database_schema.md
│   │   ├── data_model.md
│   │   └── system_requirements.md
│   │
│   ├── operational_guides/
│   │   ├── system_administration.md
│   │   ├── user_manual.md
│   │   ├── troubleshooting_guide.md
│   │   └── disaster_recovery_plan.md
│   │
│   ├── training_materials/
│   │   ├── quick_start_guide.pdf
│   │   ├── operator_training.md
│   │   ├── admin_training.md
│   │   └── video_tutorials/
│   │
│   └── release_notes/
│       ├── v1_0_release.md
│       ├── changelog.md
│       └── known_issues.md
│
├── README.md                                # Overview ejecutivo del proyecto
├── requirements.txt                        # Dependencias Python
├── setup.py                                # Setup script para paquetes
├── LICENSE                                 # Licencia del proyecto
├── .gitignore                              # Patrones Git
└── MANIFEST.in                             # Incluye archivos en distribución
```

---

## 4. COMPONENTES TÉCNICOS CLAVE

### 3.1 Mtell Core Engine
**Función**: Motor central de análisis y predicción
- Ingesta de datos en tiempo real
- Cálculos de confiabilidad
- Modelos predictivos integrados
- API REST para consumidores

**Ubicación en Estructura**: `/config/mtell_environment_setup/` y `/analytics/`

### 3.2 Data Integration Layer
**Función**: Conecta sistemas fuentes (SCADA, ERP) con Mtell
- Conectores estándares y custom
- Transformación de datos (ETL)
- Validación de calidad
- Gestión de errores

**Ubicación**: `/integrations/` y `/config/data_integrations/`

### 3.3 Predictive Analytics
**Función**: Generar modelos de RUL, anomalías, tendencias
- Machine Learning pipelines
- Feature engineering
- Model training & validation
- Scoring en tiempo real

**Ubicación**: `/analytics/predictive_models/` y `/analytics/model_training/`

### 3.4 Dashboard & Visualization
**Función**: Mostrar datos a usuarios
- Dashboards operacionales (real-time)
- Dashboards mantenimiento (planificación)
- Dashboards ejecutivos (KPIs)
- Alertas y notificaciones

**Ubicación**: `/analytics/dashboards/`

---

## 5. STACK TECNOLÓGICO RECOMENDADO

### 4.1 Aspen Mtell Platform
- **Versión**: [Version actual recomendada]
- **Licencias**: [Detallar tipos de licencia]
- **Servidor**: Windows Server 2019+ o Linux (RHEL 8+)

### 4.2 Base de Datos
- **Producción**: SQL Server 2019+ o PostgreSQL 13+
- **Data Warehouse**: SQL Server Enterprise o Vertica
- **Cache**: Redis 6+ para performance
- **Almacenamiento Temporal**: Parquet/HDF5 para large datasets

### 4.3 Integración & ETL
- **Framework ETL**: Apache NiFi o Talend
- **Conectores SCADA**: OPC-UA, Historian APIs nativas
- **API Integration**: Python (requests, pandas), Node.js si aplica

### 4.4 Análisis & ML
- **Python**: 3.9+ con librerías
  - pandas, numpy (data manipulation)
  - scikit-learn, XGBoost (ML)
  - statsmodels (estadística)
  - matplotlib, plotly (visualization)
- **R**: si aplica (forecasting con forecast, prophet)

### 4.5 Orquestación & Deployment
- **Containerización**: Docker 20+
- **Orquestación**: Kubernetes 1.20+ (si scale)
- **IaC**: Terraform 1.0+ para infra
- **CI/CD**: GitHub Actions, GitLab CI, o Jenkins

### 4.6 Monitoreo & Alertas
- **Monitoreo**: Prometheus + Grafana
- **Logs**: ELK Stack (Elasticsearch, Logstash, Kibana)
- **Alertas**: PagerDuty o Opsgenie (si crítico)

---

## 6. PATRONES DE DISEÑO

### 5.1 Patrones Aplicables

| Patrón | Contexto de Uso | Implementación |
|--------|-----------------|----------------|
| **Adapter Pattern** | Conectar diferentes historiadores SCADA | `/integrations/scada_integration/` |
| **Factory Pattern** | Crear diferentes tipos de conectores | API factory en integration layer |
| **Observer Pattern** | Alertas en tiempo real cuando hay cambios | Mtell event system |
| **Strategy Pattern** | Diferentes algoritmos de predicción por equipo | `/analytics/predictive_models/` |
| **Repository Pattern** | Acceso uniforme a datos de múltiples fuentes | Data access layer |
| **Pipeline Pattern** | ETL y procesamiento secuencial | `/integrations/data_transformation/` |

### 5.2 Convenciones de Código

**Python**
```python
# Naming conventions
# Classes: PascalCase
class DataValidator:
    pass

# Functions/methods: snake_case
def extract_scada_data():
    pass

# Constants: UPPER_SNAKE_CASE
MAX_RETRY_ATTEMPTS = 3
TIMEOUT_SECONDS = 30

# Type hints: Always use
def process_data(df: pd.DataFrame, equipment_id: str) -> dict:
    pass

# Docstrings: Google style
def calculate_rul(signal_data: np.ndarray) -> float:
    """Calculate Remaining Useful Life from equipment signal.
    
    Args:
        signal_data: Array of sensor readings
        
    Returns:
        float: Estimated RUL in hours
        
    Raises:
        ValueError: If signal data is invalid
    """
    pass
```

**SQL**
```sql
-- Table naming: plural, snake_case
CREATE TABLE equipment_sensors (
    equipment_id VARCHAR(50),
    sensor_id VARCHAR(50),
    sensor_name VARCHAR(100)
);

-- Stored procedures: verb_noun format
CREATE PROCEDURE sp_calculate_daily_kpis
```

**Configuration Files (YAML)**
```yaml
# Consistent indentation (2 spaces)
# Meaningful names with underscore separation
mtell_config:
  database:
    host: localhost
    port: 5432
    connection_timeout: 30
```

---

## 7. CONVENCIONES Y STANDARDS

### 6.1 Convenciones de Nombramiento

| Elemento | Convención | Ejemplo |
|----------|-----------|---------|
| Variables | snake_case | `equipment_status`, `max_temperature` |
| Clases | PascalCase | `DataValidator`, `SCADAConnector` |
| Funciones | snake_case | `extract_data()`, `validate_input()` |
| Constantes | UPPER_SNAKE_CASE | `MAX_RETRIES`, `DEFAULT_TIMEOUT` |
| Archivos | snake_case | `data_validator.py`, `config_loader.py` |
| Directorios | snake_case | `predictive_models/`, `data_integration/` |
| Bases de datos | snake_case | `equipment_master`, `daily_metrics` |
| Tablas | Plural, snake_case | `equipment_sensors`, `maintenance_logs` |
| Columnas | Singular, snake_case | `sensor_id`, `equipment_name` |

### 6.2 Versionado de Código
- **Sistema**: Git con semantic versioning (v1.0.0)
- **Ramas**: 
  - `main`: Producción
  - `develop`: Integración
  - `feature/`: Nuevas características
  - `bugfix/`: Correcciones
  - `hotfix/`: Urgentes a producción
  
### 6.3 Control de Cambios
- **Todos los cambios** requieren Pull Request con revisión
- **Code review checklist**:
  - ✅ Código sigue convenciones
  - ✅ Tests unitarios incluidos (min 80% coverage)
  - ✅ Documentación actualizada
  - ✅ No hay secrets/credentials en código
  - ✅ Performance aceptable

---

## 8. CONSIDERACIONES DE ESCALABILIDAD

### 7.1 Diseño para Escala
- **Stateless services**: Cualquier instancia puede procesar request
- **Database indexing**: Índices en columnas de búsqueda frecuente
- **Caching strategy**: Redis para datos de acceso frecuente
- **Partitioning**: Datos históricos particionados por tiempo
- **Load balancing**: Distribución de carga en múltiples instancias

### 7.2 Proyección de Crecimiento
```
Fase 1 (2026): 500 equipos, 100 usuarios → Single server + DB
Fase 2 (2027): 1500+ equipos, 300+ usuarios → Scaled deployment + read replicas
Fase 3 (2028+): 5000+ equipos → Distributed system, cloud-native
```

---

## 9. SEGURIDAD Y COMPLIANCE

### 8.1 Seguridad en Capas

```
Capa 1: Acceso a Red
├─ Firewall rules (puertos específicos)
├─ VPN/Bastion host si es remote
└─ Network segmentation

Capa 2: Autenticación & Autorización
├─ LDAP/AD integration (single sign-on)
├─ RBAC (Role-based access control)
├─ MFA para usuarios administrativos
└─ Session management & timeouts

Capa 3: Datos en Tránsito
├─ TLS 1.2+ para todas las conexiones
├─ Certificate pinning en APIs críticas
└─ Encrypted VPNs para data transfer

Capa 4: Datos en Reposo
├─ Encryption at rest (AES-256)
├─ Key management service (AWS KMS, Vault)
└─ Secure backup encryption

Capa 5: Auditoría & Compliance
├─ Audit logs para todas las operaciones
├─ Logging centralizado
├─ Alertas para acceso inusual
└─ Retención según política (90+ días)
```

### 8.2 Cumplimiento Regulatorio
- **Data Privacy**: GDPR, local compliance
- **Industrial**: Normas de seguridad operacional
- **Auditoría**: Acceso controllado, trazabilidad completa

---

## 10. MEJORES PRÁCTICAS DE DESARROLLO

### 9.1 Version Control
```bash
# Estructura de commits
[TYPE]: Brief description

Detailed explanation if needed

# Tipos: feat, fix, docs, style, refactor, test, chore
# Ejemplo:
# feat: Add RUL prediction for pump equipment
# 
# Implement XGBoost model for predicting
# remaining useful life of centrifugal pumps
# with 85% accuracy on historical data
```

### 9.2 Testing Strategy
- **Unit Tests**: 80%+ code coverage
- **Integration Tests**: Validar flujos completos
- **Performance Tests**: Baseline y regresión
- **Acceptance Tests**: User story validation
- **Automated**: CI/CD pipeline

### 9.3 Documentation
- **Code documentation**: Docstrings + inline comments (solo lo necesario)
- **API documentation**: OpenAPI/Swagger
- **Operacional**: Runbooks, playbooks, troubleshooting guides
- **Arquitectura**: Diagramas y decisiones (ADR - Architecture Decision Records)

---

## 11. ROADMAP TÉCNICO 2026

| Q | Hito | Componentes |
|---|------|-----------|
| Q1 | Infraestructura & Integración Básica | Servers, DB, SCADA connectors |
| Q2 | Modelos Predictivos | RUL, Anomalías, entrenamiento |
| Q3 | Dashboards & Go-Live | UI, alertas, adopción |
| Q4 | Optimización & Escalabilidad | Performance, v2.0 planning |

---

**Documento Control**: MTELL-01-ARCHITECTURE-v1.0  
**Próxima Revisión**: 2026-03-20  
**Propietario**: Tech Lead, Ingeniero de Analítica
