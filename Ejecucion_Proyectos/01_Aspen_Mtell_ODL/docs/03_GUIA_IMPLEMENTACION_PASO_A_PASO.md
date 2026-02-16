# ASPEN MTELL ODL - GUÍA DE IMPLEMENTACIÓN PASO A PASO

**Versión**: 1.0  
**Documento**: 03_GUIA_IMPLEMENTACION_PASO_A_PASO.md  
**Público**: Tech team, implementadores

---

## 1. FASE DE INICIALIZACIÓN (Semanas 1-4)

### 1.1 Antes de Iniciar
**Checklist Pre-requisitos**:
- [ ] Project Charter aprobado y firmado
- [ ] Equipo completo designado (roles claros)
- [ ] Presupuesto confirmado y disponible
- [ ] Sponsor ejecutivo identificado y comprometido
- [ ] Accesos a sistemas críticos otorgados
- [ ] VPN/Bastion hosts configurados para acceso remoto
- [ ] Herramientas de colaboración (Jira, Confluence, Teams) listas
- [ ] Procesos de comunicación definidos

### 1.2 Kickoff Meeting
**Participantes**: Sponsor, PM, Tech Lead, representantes de Ops, IT, Aspen

**Agenda** (2 horas):
1. Presentación de Visión y Objetivos (15 min)
2. Revisión de Scope y Constraints (15 min)
3. Presentación del Equipo y Roles (15 min)
4. Descripción de Procesos & Gobernanza (20 min)
5. Q&A y Compromisos (30 min)
6. Próximos pasos y Comunicaciones (15 min)

**Acción Inmediata**: Agendar reuniones semanales de steering

### 1.3 Setup de Gobernanza
```
Crear y comunicar:
✓ Calendario de reuniones (steering, tech, users)
✓ Proceso de escalación
✓ Canales de comunicación (Slack, Teams, etc)
✓ Documentación en repositorio central
✓ Sistema de tickets para issues/requests
```

---

## 2. FASE DE PLANIFICACIÓN DETALLADA (Semanas 5-9)

### 2.1 Construcción de Plan Detallado

**Paso 1**: Crear WBS (Work Breakdown Structure)
- Descomponer en tareas < 40 horas
- Identificar dependencias
- Estimar duración (3-point estimation: optimista, probable, pesimista)

**Paso 2**: Construir Schedule
- Usar herramienta de Gantt (MS Project, Smartsheet, Asana)
- Critical path analysis
- Identificar holguras
- Incluir buffers de contingencia

**Paso 3**: Asignar Recursos
- RACI matrix para cada paquete de trabajo
- Verificar disponibilidad
- Identificar gaps de skills
- Plan de cross-training

**Paso 4**: Definir Quality Gates
```
Gate 0: Project Charter Signed
Gate 1: Detailed Plan Approved
Gate 2: Infrastructure Ready
Gate 3: Integrations Functional
Gate 4: Models Validated
Gate 5: UAT Passed
Gate 6: Go-Live Approved
Gate 7: Adoption 85%
Gate 8: Value Captured
Gate 9: Lessons Learned
```

### 2.2 Procurement

**Actividades**:
1. Identificar necesidades (licencias, hardware, servicios)
2. Crear especificaciones técnicas detalladas
3. Enviar RFQs (Request for Quotation)
4. Evaluar propuestas
5. Negociar términos
6. Emitir POs (Purchase Orders)

**Ítems críticos a comprar**:
- Licencias Aspen Mtell (3 años recomendado)
- Servicios de Consultoría Aspen
- Hardware servidor (si on-premise)
- Software DB (SQL Server Enterprise, PostgreSQL)

**Timeline**: Semanas 6-7, delivery esperado semana 10

### 2.3 Preparación Técnica Inicial

**Crear documentación**:
- [ ] System Requirements Specification (SRS)
- [ ] Technical Architecture Document
- [ ] Database Design Document
- [ ] API Specifications
- [ ] Deployment Architecture

**Preparar ambiente**:
- [ ] Obtener accesos a SCADA y ERP
- [ ] Documenta estructura actual de datos
- [ ] Mapeo de campos (data mapping)
- [ ] Identificar historiadores y APIs disponibles

---

## 3. FASE DE INFRAESTRUCTURA (Semanas 10-13)

### 3.1 Setup del Servidor Mtell

**Paso 1**: Aprovisionar Hardware
```bash
# Especificaciones Mínimas (producción 500 equipos):
- CPU: 16 cores / 32 vCPU (cloud)
- RAM: 64 GB
- Almacenamiento: 2 TB (SSD para BD, HDD para históricos)
- Red: 1 Gbps (conexión a SCADA)
- OS: Windows Server 2019+ o RHEL 8+
```

**Paso 2**: Instalar Aspen Mtell
```bash
# Descarga media de instalación desde portal Aspen
# Ejecutar instalador en OS limpio
# Validar instalación (verification tests)
# Activar licencias (contactar Aspen)

# Verificación post-instalación
curl http://[servidor]:8080/health  # Debe retornar 200
```

**Paso 3**: Configurar Mtell
- [ ] Network settings (IPs, DNS, firewalls)
- [ ] Database connection
- [ ] Security certificates (SSL/TLS)
- [ ] User authentication (LDAP/AD)
- [ ] Backup settings

### 3.2 Setup de Base de Datos

**Paso 1**: Crear Instancia DB
```sql
-- Create database
CREATE DATABASE aspen_mtell_prod
    COLLATE SQL_Latin1_General_CP1_CI_AS;

-- Create datafile
ALTER DATABASE aspen_mtell_prod
    ADD FILE (
        NAME = mtell_data,
        FILENAME = 'D:\Data\mtell_data.mdf',
        SIZE = 500MB,
        FILEGROWTH = 100MB
    );

-- Create logfile
ALTER DATABASE aspen_mtell_prod
    ADD LOG FILE (
        NAME = mtell_log,
        FILENAME = 'L:\Logs\mtell_log.ldf',
        SIZE = 100MB,
        FILEGROWTH = 50MB
    );
```

**Paso 2**: Crear Esquema
- [ ] Ejecutar scripts Mtell (provided by vendor)
- [ ] Crear índices según especificación
- [ ] Setup de particiones (por tiempo si datos masivos)
- [ ] Crear vistas para reporting

**Paso 3**: Backup & Recovery
```bash
# Configurar backup automático
# Full backup: Semanal (domingo 2:00 AM)
# Differential backup: Diario (excepto domingo)
# Log backup: Cada 15 minutos

# Realizar test de restore
# Documentar recovery procedures
```

### 3.3 Networking & Seguridad

**Paso 1**: Firewall Rules
```
Allow inbound:
- Port 8080: Mtell web interface (usuarios)
- Port 443: HTTPS (if TLS)
- Port 1433: DB (from app servers only)
- Port 4900: SCADA connector (from specific IPs)

Allow outbound:
- Port 123: NTP (time sync)
- Port 443: HTTPS (software updates)
- DNS: Port 53
```

**Paso 2**: SSL/TLS Certificates
- [ ] Generar CSR (Certificate Signing Request)
- [ ] Obtener certificado de CA corporativa
- [ ] Instalar certificado en servidor
- [ ] Configurar HTTPS en Mtell
- [ ] Test con navegadores

**Paso 3**: Autenticación
```
Integrar con Active Directory:
1. Configurar LDAP binding
2. Crear grupos AD para roles Mtell
3. Map LDAP groups a Mtell roles
4. Test: Validar usuarios pueden autenticarse
```

### 3.4 Monitoreo & Alertas

**Paso 1**: Setup Prometheus + Grafana
```yaml
# prometheus.yml
global:
  scrape_interval: 15s

scrape_configs:
  - job_name: 'mtell'
    static_configs:
      - targets: ['[servidor]:9090']
```

**Paso 2**: Crear Dashboards
- [ ] Disponibilidad de aplicación (% uptime)
- [ ] Performance de BD (queries/sec, CPU, memoria)
- [ ] Disk space usage
- [ ] Error rates

**Paso 3**: Alertas
```yaml
# alerts.yaml
groups:
  - name: mtell_health
    rules:
      - alert: HighCPU
        expr: cpu_usage > 80
        for: 5m
        annotations:
          summary: "High CPU on mtell server"
```

**Paso 4**: Test de Alertas
- [ ] Simular falla de aplicación → verificar alerta
- [ ] Simular alto CPU → verificar alerta
- [ ] Simular falla de conectividad → verificar alerta

---

## 4. FASE DE INTEGRACIÓN DE DATOS (Semanas 14-18)

### 4.1 Integración SCADA

**Paso 1**: Documentar Historiador Actual
```
Para cada historiador (PI System, OSIsoft, custom):
- [ ] Especificación técnica
- [ ] Tags disponibles (lista completa)
- [ ] Rates de datos (frequency)
- [ ] Data types (analog, digital, string)
- [ ] Ejemplos de datos
```

**Paso 2**: Desarrollar Conector
```python
# scada_connector.py
class HistorianConnector:
    def __init__(self, server, db):
        self.server = server
        self.db = db
    
    def extract_data(self, tag_id, start_time, end_time):
        """Extract historical data from historian"""
        # query historian
        # transform to standardized format
        # validate and return
        pass
    
    def validate_data(self, data):
        """Validate data quality"""
        checks = [
            self._check_null_values,
            self._check_outliers,
            self._check_timestamps,
        ]
        for check in checks:
            if not check(data):
                raise DataQualityError()

# Tests
def test_extract_data():
    connector = HistorianConnector('pi.server.com', mtell_db)
    data = connector.extract_data('PUMP_01', start, end)
    assert len(data) > 0
    assert all(d['timestamp'] for d in data)
```

**Paso 3**: Deploy Conector
- [ ] Install en servidor Mtell
- [ ] Test con datos reales
- [ ] Validar conexión continua
- [ ] Configurar auto-restart si cae

### 4.2 Integración ERP

**Paso 1**: Mapeo de Datos
```
ERP                        Mtell
─────────────────────────────────
WORK_ORDER        →        Maintenance_Event
EQUIPMENT_ID      →        Equipment_ID
MAINT_DATE        →        Event_Date
MAINT_TYPE        →        Work_Type
MAINT_HOURS       →        Labor_Hours
STATUS             →        Status
```

**Paso 2**: Crear Sincronizador
```python
# erp_sync.py
class ERPSynchronizer:
    def __init__(self, erp_conn, mtell_conn):
        self.erp = erp_conn
        self.mtell = mtell_conn
    
    def sync_maintenance_events(self):
        """Sync maintenance records from ERP to Mtell"""
        # Query ERP for new/modified events
        # Transform data
        # Load into Mtell
        # Log status
        
    def schedule_recurring(self):
        """Schedule daily sync at 2:00 AM"""
        scheduler.add_job(
            self.sync_maintenance_events,
            'cron',
            hour=2,
            minute=0
        )
```

**Paso 3**: Validación de Sincronización
- [ ] Test: Crear WO en ERP → Verificar en Mtell
- [ ] Test: Modificar WO en ERP → Verificar update en Mtell
- [ ] Test: Eliminar WO → Verificar cascada
- [ ] Validar integridad de datos

### 4.3 Data Quality Framework

**Paso 1**: Definir Reglas de Validación
```python
# data_quality.py
class DataValidator:
    RULES = {
        'temperature': {
            'min': 20,  # Celsius
            'max': 120,
            'allow_null': False,
        },
        'pressure': {
            'min': 0,
            'max': 500,  # PSI
            'allow_null': False,
        },
    }
    
    def validate(self, data):
        errors = []
        for field, rules in self.RULES.items():
            if not self._validate_field(data[field], rules):
                errors.append(f"{field} failed validation")
        return errors
```

**Paso 2**: Implementar Checks Diarios
- [ ] Null value checks
- [ ] Outlier detection
- [ ] Timestamp consistency
- [ ] Duplicate detection
- [ ] Range validation
- [ ] Data staleness checks

**Paso 3**: Reporting de Calidad
```
Daily Data Quality Report:
- Total records ingested: X
- Records with issues: Y (Z%)
- Top issues: [list]
- Action items: [list]
```

---

## 5. FASE DE MODELADO PREDICTIVO (Semanas 19-22)

### 5.1 Análisis Exploratorio de Datos (EDA)

```python
# eda.py
import pandas as pd
import matplotlib.pyplot as plt

# Load data
df = pd.read_csv('equipment_historical_data.csv')

# Exploración básica
print(df.describe())  # Estadísticas
print(df.isnull().sum())  # Nulls
print(df.dtypes)  # Types

# Visualizaciones
plt.figure(figsize=(12, 4))
plt.subplot(1, 2, 1)
plt.hist(df['temperature'], bins=30)
plt.title('Temperature Distribution')

plt.subplot(1, 2, 2)
df.groupby('day')['failures'].sum().plot()
plt.title('Failures per Day')

plt.show()
```

**Hallazgos esperados**:
- [ ] Estadísticas por sensor
- [ ] Correlaciones entre variables
- [ ] Patrones temporales
- [ ] Anomalías evidentes

### 5.2 Feature Engineering

```python
# feature_engineering.py
def create_features(df):
    """Create ML features from raw data"""
    features = pd.DataFrame()
    
    # Temporal features
    features['hour_of_day'] = df['timestamp'].dt.hour
    features['day_of_week'] = df['timestamp'].dt.dayofweek
    
    # Statistical features (rolling windows)
    features['temp_rolling_mean_24h'] = df['temperature'].rolling('24h').mean()
    features['temp_rolling_std_24h'] = df['temperature'].rolling('24h').std()
    features['pressure_max_last_7d'] = df['pressure'].rolling('7d').max()
    
    # Rate of change
    features['temp_rate_of_change'] = df['temperature'].diff() / df['timestamp'].diff().dt.total_seconds()
    
    # Lag features (previous values)
    features['temp_lag_1h'] = df['temperature'].shift(60)  # 1 hour ago
    features['temp_lag_24h'] = df['temperature'].shift(1440)  # 1 day ago
    
    return features

# Validate features
features_df = create_features(df)
assert features_df.isnull().sum().sum() < len(features_df) * 0.05  # < 5% nulls acceptable
```

### 5.3 Entrenamiento de Modelos

```python
# train_models.py
from sklearn.ensemble import RandomForestRegressor, GradientBoostingRegressor
from xgboost import XGBRegressor
from sklearn.model_selection import train_test_split, cross_val_score

# Prepare data
X = features_df  # Features
y = df['remaining_useful_life']  # Target

X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2)

# Train multiple models
models = {
    'RF': RandomForestRegressor(n_estimators=100, max_depth=10),
    'XGB': XGBRegressor(n_estimators=100, learning_rate=0.1),
    'GB': GradientBoostingRegressor(n_estimators=100),
}

results = {}
for name, model in models.items():
    model.fit(X_train, y_train)
    score = model.score(X_test, y_test)
    results[name] = score
    print(f"{name}: R² = {score:.4f}")

# Select best model
best_model_name = max(results, key=results.get)
best_model = models[best_model_name]
```

**Criterios de éxito**:
- [ ] R² > 0.85 en test set
- [ ] MAE < 5% de rango de valores
- [ ] Cross-validation score > 0.80

### 5.4 Validación con Datos Históricos

```python
# Backtest el modelo con datos históricos reales
# Comparar predicciones vs realidad

def backtest_model(model, historical_data):
    """Test model against known failures"""
    results = []
    
    for equipment_id in historical_data['equipment_id'].unique():
        eq_data = historical_data[historical_data['equipment_id'] == equipment_id]
        
        # Use data before failure to predict
        pre_failure_data = eq_data[eq_data['timestamp'] < eq_data['failure_date'].iloc[0]]
        features_pre = create_features(pre_failure_data)
        
        # Predict RUL
        predicted_rul = model.predict(features_pre.iloc[-1:])
        actual_rul = (eq_data['failure_date'].iloc[0] - eq_data['timestamp'].iloc[-1]).days
        
        results.append({
            'equipment_id': equipment_id,
            'predicted_rul': predicted_rul[0],
            'actual_rul': actual_rul,
            'error': abs(predicted_rul[0] - actual_rul),
        })
    
    return pd.DataFrame(results)

backtest_df = backtest_model(best_model, historical_data)
print(f"Average error: {backtest_df['error'].mean():.1f} days")
```

---

## 6. FASE DE CONFIGURACIÓN MTELL (Semanas 20-23)

### 6.1 Master Data Setup

**Paso 1**: Cargar Equipos
```python
# load_equipment_master.py
import pandas as pd
from mtell_api import MtellAPI

api = MtellAPI('http://mtell.server:8080')

equipment_df = pd.read_excel('equipment_master.xlsx')
for _, row in equipment_df.iterrows():
    api.create_equipment(
        equipment_id=row['ID'],
        name=row['Name'],
        type=row['Type'],
        location=row['Location'],
        specs=row['Specifications'],
    )
```

**Paso 2**: Mapear Sensores
```
Equipment: PUMP_01
├─ Sensor: PUMP_01_TEMP
│  └─ SCADA Tag: AI_PUMP_01_TEMP_C
├─ Sensor: PUMP_01_PRESSURE
│  └─ SCADA Tag: AI_PUMP_01_PRES_PSI
└─ Sensor: PUMP_01_VIBRATION
   └─ SCADA Tag: AI_PUMP_01_VIBR_MM
```

### 6.2 Cargar Modelos Predictivos

```
Administration > Models > Import
│
├─ RUL_Model_v1.0
│  ├─ Type: Regression
│  ├─ Algorithm: XGBoost
│  ├─ Accuracy: 85.2%
│  └─ Equipment Types: Centrifugal Pump, Compressor
│
├─ Anomaly_Detection_v1.0
│  ├─ Type: Unsupervised
│  ├─ Algorithm: Isolation Forest
│  └─ Accuracy: 92.1%
│
└─ Trend_Analysis_v1.0
   ├─ Type: Statistical
   └─ Methods: Linear regression, Fourier analysis
```

### 6.3 Configurar Alertas

```yaml
# alerts_config.yaml
alerts:
  - name: "High RUL Risk"
    condition: "rul < 7 days"
    severity: "Critical"
    actions:
      - send_email: "maintenance@company.com"
      - create_work_order: true
      - notify_slack: "ops-channel"
  
  - name: "Temperature Anomaly"
    condition: "temperature > (mean + 3*std)"
    severity: "Warning"
    actions:
      - notify_operator: true
  
  - name: "Unusual Vibration Pattern"
    condition: "anomaly_score > 0.8"
    severity: "Medium"
    actions:
      - alert_maintenance: true
```

### 6.4 Crear Dashboards

**Operational Dashboard**:
```
Real-time view (updates every 5 seconds)
├─ KPI Cards
│  ├─ Equipment Online: 247/250
│  ├─ Alerts Active: 8
│  └─ Average RUL: 45 days
├─ Equipment Grid
│  ├─ Status by location
│  ├─ RUL distribution
│  └─ Alert heatmap
└─ Trending Charts
   ├─ Average temperature by hour
   └─ Failure predictions
```

**Maintenance Dashboard**:
```
Planification view (updates hourly)
├─ RUL Scorecard
│  ├─ Critical: 3 equipos < 5 días
│  ├─ High: 12 equipos < 15 días
│  └─ Medium: 28 equipos < 30 días
├─ Recommended Actions
│  └─ Maintenance schedule optimizer
└─ Historical Performance
   └─ Actual vs predicted failures
```

---

## 7. FASE DE TESTING (Semanas 18-25)

### 7.1 Unit Testing

```python
# test_data_quality.py
import pytest
from data_quality import DataValidator

def test_temperature_within_range():
    validator = DataValidator()
    data = {'temperature': 60, 'pressure': 100}
    assert validator.validate(data) == []

def test_temperature_below_minimum():
    validator = DataValidator()
    data = {'temperature': 10, 'pressure': 100}
    errors = validator.validate(data)
    assert 'temperature' in str(errors)

# Run tests
# pytest test_data_quality.py -v
```

### 7.2 Integration Testing

**Test Scenarios**:
- [ ] SCADA connector → Data flows to DB
- [ ] ERP sync → New WO appears in Mtell
- [ ] Data validation → Invalid records are flagged
- [ ] Models → Predictions available in dashboard
- [ ] Alerts → Critical alerts trigger emails

### 7.3 UAT (User Acceptance Testing)

**Test Cases by Role**:

**Operador**:
- [ ] Puede loguear al sistema
- [ ] Ve datos en tiempo real
- [ ] Entiende alertas
- [ ] Puede investigar historiales

**Mantenimiento**:
- [ ] Puede ver equipos por criticidad
- [ ] Recomendaciones son accionables
- [ ] Puede crear WOs desde Mtell
- [ ] Reportes útiles para planificación

**Gerencia**:
- [ ] Dashboard ejecutivo es claro
- [ ] KPIs son correctos
- [ ] ROI se puede demostrar
- [ ] Reports exportables

### 7.4 Sign-off UAT

```
UAT Sign-off:
✓ All test cases passed
✓ No critical issues remaining
✓ Users trained and confident
✓ Sponsor approves go-live

Signed:
- User Representative: _________ Date: _____
- IT Director: _________ Date: _____
- Project Sponsor: _________ Date: _____
```

---

## 8. PROCEDIMIENTOS ESPECIALES

### 8.1 Data Backfill (Cargar datos históricos)

```
Scenario: Se necesitan 2 años de datos históricos para entrenar modelos

Proceso:
1. Extract de SCADA (PI System, etc)
2. Transform a formato Mtell
3. Validar calidad
4. Load en batches (por semana)
5. Verificar completitud

Timeline: 2 semanas
Recursos: Integración (100%), Analytics (50%)
Rollback: Si data corruption, restore de backup
```

### 8.2 Cutover Plan (Go-live)

```
GO-LIVE: Miércoles 1 de julio 2026, 8:00 AM

Pre-cutover (Martes 30 de junio):
- Final data validation
- Backup de todos los sistemas
- Test de rollback procedure
- Brief all teams

Cutover (Miércoles):
06:00 - Final UAT validation run
07:00 - Pause data feeds
08:00 - **GO-LIVE: Mtell goes LIVE**
08:30 - Verify all data streams functional
09:00 - Open dashboards to users
12:00 - Status check

Post-cutover:
- 24/7 support en vivo (on-call)
- Monitoring intensivo (semana 1)
- Daily issues triage
- Weekly steering meetings

Rollback trigger:
- Si > 5 alerts críticos sin explicación
- Si sistema unavailable > 30 min
- Si data corruption detected
```

---

## 9. DOCUMENTACIÓN REQUERIDA A CADA FASE

| Fase | Documentos a Completar |
|------|----------------------|
| Planificación | Plan detallado, RACI, Risk register |
| Infraestructura | Architecture diagrams, deployment guide |
| Integración | Data mapping, integration guide, test results |
| Modelado | Model documentation, validation results |
| Testing | Test plans, test cases, UAT results |
| Go-Live | Go-live plan, runbooks, support matrix |
| Post-Live | Lessons learned, optimization notes |

---

**Documento Control**: MTELL-03-IMPLEMENTATION-v1.0  
**Owner**: Tech Lead, Project Manager  
**Próxima Revisión**: Actualizar semanalmente con avances reales
