# 01 - ESTRUCTURA Y ARQUITECTURA TÉCNICA
## Detección de Crudo Incipiente - Sistema Predictivo

---

## 1. DIAGRAMA C4 - NIVEL 1

```
┌──────────────────────────────────────────────────┐
│   SISTEMA DETECCIÓN CRUDO INCIPIENTE             │
│      (Early Crude Degradation Detection)         │
├──────────────────────────────────────────────────┤
│                                                  │
│  Sensores    Análisis      ML Model    Alertas  │
│  ┌────────┐  ┌──────────┐  ┌──────┐  ┌────────┐│
│  │Química │─>│Parámetros│─>│ Deep │─>│ Engine ││
│  │ Físicos│  │  Control │  │ Learn│  │ Alertas││
│  └────────┘  └──────────┘  └──────┘  └────────┘│
│       │           │            │          │    │
│       └────────────┴────────────┴──────────┘    │
│                     │                           │
│                     v                           │
│            Dashboards de Monitoreo              │
│            Historial de Eventos                 │
│            Reportes Predictivos                 │
│                                                 │
└──────────────────────────────────────────────────┘
```

---

## 2. ARQUITECTURA DE 6 CAPAS

### 2.1 CAPA DE PRESENTACIÓN
- Dashboard en tiempo real (React 18.2)
- Visualización de indicadores
- Sistema de alertas visual
- Stack: React, TypeScript, Tailwind

### 2.2 CAPA DE APLICACIÓN
- Servicio análisis de parámetros
- Motor de predicción (ML)
- Gestor de alertas
- Stack: Node.js, TypeScript, Express 4.18

### 2.3 CAPA DE INTEGRACIÓN
- Adaptador de sensores (REST, MQTT)
- Conector a laboratorio (APIs)
- Integración ERP/LIMS
- Kafka para eventos

### 2.4 CAPA DE DOMINIO
- Entidades: Muestra, Parámetro, Predicción
- Agregado: QualityMonitoring
- Lógica predictiva

### 2.5 CAPA DE PERSISTENCIA
- PostgreSQL 15 (datos transaccionales)
- InfluxDB 2.6 (series temporales)
- Redis 7.0 (caché de modelos)
- Blob storage (muestras históricas)

### 2.6 CAPA DE INFRAESTRUCTURA
- ML: TensorFlow 2.13, PyTorch 2.0
- Procesamiento: Python 3.11, pandas
- Kubernetes 1.27
- Monitoreo: Prometheus, Grafana

---

## 2A. ESTRUCTURA DE DOCUMENTACIÓN DEL PROYECTO (PMI & Procesos)

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

## 3. ESTRUCTURA DE CARPETAS

```
06_Deteccion_Crudo/
├── src/
│   ├── presentation/
│   │   ├── controllers/QualityController.ts
│   │   ├── routes/detection.routes.ts
│   │   └── dto/SampleDataDTO.ts
│   ├── application/
│   │   ├── services/QualityAnalysisService.ts
│   │   ├── services/PredictionService.ts
│   │   ├── usecases/DetectDegradationUseCase.ts
│   │   └── validators/QualityValidator.ts
│   ├── domain/
│   │   ├── entities/CrudeSample.entity.ts
│   │   ├── entities/QualityParameter.entity.ts
│   │   ├── aggregates/QualityMonitoringAggregate.ts
│   │   └── repositories/SampleRepository.interface.ts
│   ├── infrastructure/
│   │   ├── ml/DegradationDetector.ts
│   │   ├── ml/models/
│   │   ├── sensors/SensorGateway.ts
│   │   ├── database/PostgresQualityRepo.ts
│   │   ├── timeseries/InfluxDBAdapter.ts
│   │   └── external/LABConnector.ts
│   ├── analysis/
│   │   ├── ParameterAnalyzer.ts
│   │   ├── TrendAnalyzer.py
│   │   └── CorrelationAnalyzer.ts
│   └── main.ts
├── tests/
│   ├── unit/
│   ├── integration/
│   └── e2e/
├── ml-models/
│   ├── training/
│   ├── evaluation/
│   └── models/
├── config/
│   ├── ml.config.ts
│   ├── sensors.config.ts
│   └── app.config.ts
└── scripts/
    ├── data-preparation/
    ├── model-training/
    └── batch-inference/
```

---

## 4. COMPONENTES DETALLADOS (15+)

| # | Componente | Responsabilidad | Stack | Input | Output |
|---|-----------|-----------------|-------|-------|--------|
| 1 | API REST | Enrutamiento | Express 4.18 | HTTP | JSON |
| 2 | Sensor Gateway | Recepción datos | REST/MQTT | Parámetros | Events |
| 3 | Parameter Analyzer | Análisis individual | Node.js + pandas | Parámetros brutos | Scores normalizados |
| 4 | Correlation Engine | Análisis correlaciones | SciPy | Múltiples params | Correlaciones |
| 5 | Trend Analyzer | Análisis tendencias | Python + pandas | Serie temporal | Trends detectados |
| 6 | Anomaly Detector | Detección de outliers | Isolation Forest | Datos históricos | Anomalías flagged |
| 7 | Degradation Predictor | Predicción degradación | TensorFlow/PyTorch | Features + histórico | Probability + ETA |
| 8 | Alert Engine | Generación alertas | Node.js | Predictions | Notificaciones |
| 9 | Quality DB | Almacenamiento | PostgreSQL 15 | Muestras | Queries |
| 10 | TimeSeries Store | Series temporales | InfluxDB 2.6 | Parámetros | Consultas rápidas |
| 11 | Model Manager | Gestión de modelos | MLflow | Modelos entrenados | Versioning |
| 12 | Cache Layer | Caché de datos | Redis 7.0 | Queries frecuentes | Datos en caché |
| 13 | Lab Connector | Integración laboratorio | REST API | Resultados LAB | Sincronización |
| 14 | Reportero | Reportes | Reportlab + Excel | Datos agregados | PDF/Excel |
| 15 | Audit Logger | Auditoría | Winston + ELK | Eventos | Logs centralizados |

---

## 5. STACK TECNOLÓGICO

```
BACKEND:
  - Node.js 20 LTS + TypeScript 5.1
  - Express 4.18 (APIs)
  - Python 3.11 (análisis)
  - FastAPI 0.104 (APIs Python)

MACHINE LEARNING:
  - TensorFlow 2.13 (redes neuronales)
  - PyTorch 2.0 (modelos avanzados)
  - scikit-learn 1.3 (modelos tradicionales)
  - XGBoost 2.0 (gradient boosting)
  - pandas 2.0 (análisis datos)
  - NumPy 1.24 (cálculos numéricos)

DATA ANALYSIS:
  - SciPy 1.11 (estadística)
  - Matplotlib 3.8 (visualización)
  - Seaborn 0.13 (gráficos avanzados)
  - Statsmodels 0.14 (series temporales)

DATABASE:
  - PostgreSQL 15.3 (transaccional)
  - InfluxDB 2.6 (time-series)
  - Redis 7.0 (caché)
  - S3 (artifacts)

MONITORING:
  - MLflow 2.6 (model tracking)
  - Prometheus 2.45
  - Grafana 10.0
  - ELK Stack 8.8
```

---

## 6. DATA FLOW: Detectar Degradación

```
Muestra de Crudo (Laboratorio)
    ↓
Ingesta de Parámetros
├─ Viscosidad
├─ Densidad API
├─ % Agua
├─ Sulfur
├─ Punto de fluidez
└─ Corrosividad
    ↓
Normalización Parámetros
    ↓
Análisis de Tendencias (vs histórico)
    ↓
Cálculo de Correlaciones
    ↓
Detección de Anomalías
    ├─ Isolation Forest
    └─ If outlier → Flag
    ↓
Extracción de Features
├─ Rate de cambio
├─ Desviación estándar
├─ Cambios recientes
└─ Patrones históricos
    ↓
Predictor ML (Deep Learning)
├─ Classification (degraded/normal)
├─ Probability score
└─ ETA degradación completa
    ↓
IF probability > 60%
    ├─ Generar alerta HIGH
    ├─ Notificar operaciones
    ├─ Sugerir inspección
    └─ Log en BD
    ↓
Almacenar en BD + TimeSeries
    ↓
Actualizar dashboards
```

---

## 7. MODELOS DE DATOS

```sql
CREATE TABLE crude_samples (
    id SERIAL PRIMARY KEY,
    sample_id VARCHAR(50) UNIQUE NOT NULL,
    tank_id VARCHAR(50),
    sample_date DATE NOT NULL,
    received_date TIMESTAMP,
    lab_id VARCHAR(50),
    status VARCHAR(20),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE quality_parameters (
    id SERIAL PRIMARY KEY,
    sample_id INTEGER REFERENCES crude_samples(id),
    parameter_name VARCHAR(100),
    value DECIMAL(12,4),
    unit VARCHAR(50),
    measurement_date TIMESTAMP,
    measured_by VARCHAR(100),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE degradation_predictions (
    id SERIAL PRIMARY KEY,
    sample_id INTEGER REFERENCES crude_samples(id),
    prediction_score DECIMAL(5,2),
    risk_level VARCHAR(20), -- LOW, MEDIUM, HIGH
    predicted_degradation_date DATE,
    confidence DECIMAL(5,2),
    model_version VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_sample_date ON crude_samples(sample_date DESC);
CREATE INDEX idx_risk_level ON degradation_predictions(risk_level);
CREATE INDEX idx_prediction_date ON degradation_predictions(created_at DESC);
```

---

## 8. EJEMPLO DE CÓDIGO

```typescript
// application/services/QualityAnalysisService.ts
@Injectable()
export class QualityAnalysisService {
  async analyzeSample(sample: CrudeSample): Promise<AnalysisResult> {
    // 1. Obtener parámetros históricos
    const historicalParams = await this.getHistoricalParameters(
      sample.tankId
    );
    
    // 2. Normalizar parámetros actuales
    const normalized = this.normalizeParameters(sample.parameters);
    
    // 3. Analizar tendencias
    const trends = this.analyzeTrends(historicalParams, normalized);
    
    // 4. Detectar anomalías
    const anomalies = await this.anomalyDetector.detect(
      normalized,
      historicalParams
    );
    
    // 5. Hacer predicción
    const prediction = await this.degradationPredictor.predict({
      current: normalized,
      trends: trends,
      anomalies: anomalies,
      historical: historicalParams
    });
    
    return new AnalysisResult(normalized, trends, prediction);
  }

  private analyzeTrends(
    historical: QualityParameter[],
    current: QualityParameter
  ): Trend {
    const timeSeries = historical.map(p => p.value);
    const mean = timeSeries.reduce((a, b) => a + b) / timeSeries.length;
    const deviation = Math.abs(current.value - mean);
    const percentChange = (deviation / mean) * 100;
    
    return {
      mean,
      deviation,
      percentChange,
      trend: percentChange > 5 ? 'DEGRADING' : 'STABLE'
    };
  }
}

// Python Predictor
def predict_degradation(features: dict) -> dict:
    model = tf.keras.models.load_model('models/degradation_model.h5')
    scaler = joblib.load('models/feature_scaler.pkl')
    
    X = prepare_features(features)
    X_scaled = scaler.transform(X)
    
    prediction = model.predict(X_scaled)[0]
    probability = prediction[0]
    
    if probability > 0.6:
        estimated_days = predict_remaining_days(features)
    else:
        estimated_days = None
    
    return {
        'is_degrading': probability > 0.6,
        'probability': float(probability),
        'estimated_days': estimated_days,
        'confidence': calculate_confidence(probability)
    }
```

---

## 9. SECURITY & COMPLIANCE

- **Autenticación:** OAuth 2.0 / JWT
- **Encriptación:** TLS 1.3, AES-256
- **Datos sensibles:** Tokenizados
- **Auditoría:** Completa de cambios
- **Cumplimiento:** ISO 9001, API standards
- **Retención:** 7 años de datos

---

## 10. PERFORMANCE TARGETS

| Métrica | Target |
|---------|--------|
| Tiempo análisis | < 2 seg |
| Precisión predicción | > 90% |
| Cobertura detección | > 95% |
| Disponibilidad | 99.9% |
| Latencia API | < 100 ms |

---

## 11. CONVENCIONES

- TypeScript + Python
- Máximo 300 líneas por archivo
- Una responsabilidad por clase
- Nombres claros y descriptivos
