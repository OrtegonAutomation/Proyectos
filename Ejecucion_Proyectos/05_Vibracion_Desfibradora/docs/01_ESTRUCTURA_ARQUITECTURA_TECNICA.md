# 01 - ESTRUCTURA Y ARQUITECTURA TÉCNICA
## Vibración Desfibradora - Sistema de Monitoreo Predictivo

---

## 1. DIAGRAMA C4 - NIVEL 1

```
┌──────────────────────────────────────────────────────┐
│     SISTEMA MONITOREO VIBRACIÓN DESFIBRADORA        │
│         (Vibration Predictive Monitoring)           │
├──────────────────────────────────────────────────────┤
│                                                      │
│  Sensores    Ingesta      Análisis    Alertas      │
│  Vibración   ┌───────┐    ┌─────────┐ ┌────────┐   │
│  ┌────┐  ──>│ Tiempo│───>│ ML Model│>│ Engine │   │
│  │    │     │Series │    │Analytics│ │Alertas │   │
│  └────┘     └───────┘    └─────────┘ └────────┘   │
│                                │                   │
│                                v                   │
│                        ┌──────────────┐           │
│                        │  Dashboards  │           │
│                        │  Históricos  │           │
│                        │  Reportes    │           │
│                        └──────────────┘           │
│                                                     │
└──────────────────────────────────────────────────────┘
```

---

## 2. ARQUITECTURA DE 6 CAPAS

### 2.1 CAPA DE PRESENTACIÓN
- Dashboard en tiempo real (React 18.2)
- Gráficos de vibración (Chart.js)
- Alertas visuales
- App móvil (React Native)

### 2.2 CAPA DE APLICACIÓN
- Servicio de procesamiento de señales
- Motor de predicción ML
- Servicio de alertas
- Stack: Node.js Express, Python FastAPI

### 2.3 CAPA DE INTEGRACIÓN
- Adaptador IoT sensores (MQTT)
- Conector SAP/CMMS
- APIs de terceros
- Event streaming (Kafka)

### 2.4 CAPA DE DOMINIO
- Entidades: Medición, Equipo, Anomalía
- Agregado: MonitoringSession
- Lógica predictiva

### 2.5 CAPA DE PERSISTENCIA
- TimescaleDB 2.11 (extensión PostgreSQL)
- InfluxDB 2.6 (series temporales)
- Redis 7.0 (caché)
- S3 (modelos ML)

### 2.6 CAPA DE INFRAESTRUCTURA
- Procesamiento: Python 3.11 + SciPy
- ML: TensorFlow 2.13, scikit-learn 1.3
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
05_Vibracion_Desfibradora/
├── src/
│   ├── presentation/
│   │   ├── controllers/MeasurementController.ts
│   │   ├── routes/monitoring.routes.ts
│   │   └── dto/VibrationDataDTO.ts
│   ├── application/
│   │   ├── services/VibrationAnalysisService.ts
│   │   ├── services/PredictiveService.ts
│   │   ├── usecases/ProcessMeasurementUseCase.ts
│   │   └── validators/AnomalyValidator.ts
│   ├── domain/
│   │   ├── entities/Measurement.entity.ts
│   │   ├── entities/Equipment.entity.ts
│   │   ├── aggregates/MonitoringSessionAggregate.ts
│   │   └── repositories/MeasurementRepository.interface.ts
│   ├── infrastructure/
│   │   ├── timeseries/TimescaleDBAdapter.ts
│   │   ├── timeseries/InfluxDBAdapter.ts
│   │   ├── ml/TensorFlowPredictor.ts
│   │   ├── ml/ScikitLearnPredictor.py
│   │   ├── iot/MQTTAdapter.ts
│   │   └── external/SAP-CMConnector.ts
│   ├── signal-processing/
│   │   ├── FFTAnalyzer.ts
│   │   ├── WaveletAnalyzer.py
│   │   └── FilterFactory.ts
│   └── main.ts
├── tests/
│   ├── unit/
│   ├── integration/
│   └── e2e/
├── ml-models/
│   ├── training/
│   ├── models/
│   └── evaluation/
├── config/
│   ├── timeseries.config.ts
│   ├── ml.config.ts
│   └── iot.config.ts
└── scripts/
    ├── data-processing/
    ├── model-training/
    └── batch-analysis/
```

---

## 4. COMPONENTES DETALLADOS (15+)

| # | Componente | Responsabilidad | Stack | Input | Output |
|---|-----------|-----------------|-------|-------|--------|
| 1 | API Gateway | Enrutamiento | Express | HTTP | JSON |
| 2 | MQTT Listener | Recepción sensores | MQTT 5.0 | Telemetría | Events |
| 3 | Ingesta Datos | Buffer temporal | Kafka + Redis | Señales britas | Señales bufferizadas |
| 4 | Analizador FFT | Análisis frecuencias | SciPy 1.11 | Tiempo | Frecuencias |
| 5 | Wavelet Analyzer | Análisis multirresolución | PyWavelets | Señal | Coeficientes |
| 6 | Feature Extractor | Extracción características | scikit-learn | Señal procesada | Vector features |
| 7 | Predictor ML | Detección anomalías | TensorFlow 2.13 | Features | Score anormalidad |
| 8 | Alert Engine | Generación alertas | Node.js | Scores | Notificaciones |
| 9 | TimeSeries DB | Almacenamiento datos | TimescaleDB 2.11 | Mediciones | Queries optimizadas |
| 10 | Historian | Análisis histórico | SQL + Python | Rango temporal | Tendencias |
| 11 | Reportero | Generación reportes | Reportlab | Datos agregados | PDF/Excel |
| 12 | Cache Manager | Cache de sesiones | Redis 7.0 | Queries | Datos en caché |
| 13 | ERP Connector | Integración SAP | Node-odata | Órdenes mantenimiento | Confirmaciones |
| 14 | Logger | Auditoría | Winston + ELK | Eventos | Logs centralizados |
| 15 | Healthchecker | Monitoreo sistema | Prometheus | Métricas | Health status |

---

## 5. STACK TECNOLÓGICO

```
BACKEND:
  - Node.js 20 LTS + TypeScript 5.1
  - Express 4.18 (APIs REST)
  - Python 3.11 (procesamiento señales)
  - FastAPI 0.104 (APIs Python)

SIGNAL PROCESSING:
  - SciPy 1.11 (FFT, filtros)
  - NumPy 1.24 (cálculos numéricos)
  - PyWavelets 1.4 (análisis wavelet)

MACHINE LEARNING:
  - TensorFlow 2.13 (redes neuronales)
  - scikit-learn 1.3 (modelos tradicionales)
  - Joblib (persistencia modelos)
  - pandas 2.0 (dataframes)

TIME-SERIES:
  - TimescaleDB 2.11 (extension PostgreSQL)
  - InfluxDB 2.6 (alternativo)
  - PostgreSQL 15.3 (base principal)

IoT & MESSAGING:
  - MQTT 5.0 (sensores)
  - Kafka 3.4 (streaming)
  - RabbitMQ 3.12 (alternativo)

MONITOREO:
  - Prometheus 2.45
  - Grafana 10.0
  - ELK Stack 8.8
```

---

## 6. DATA FLOW: Procesar Medición

```
Sensor Vibración
    ↓ (MQTT)
Ingesta (Buffer 1 seg)
    ↓
Resampling si necesario
    ↓
Análisis FFT (frecuencias)
    ↓
Análisis Wavelet (tiempo-frecuencia)
    ↓
Extracción Features
├─ RMS, Peak, Kurtosis
├─ Aceleración, velocidad
└─ Indicadores de falla
    ↓
Predictor ML
├─ Classification (Normal/Anómalo)
├─ Probability score
└─ Predicted remaining life
    ↓
IF score > threshold
    ├─ Generar alerta
    ├─ Notificar mantenimiento
    ├─ Log en auditoria
    └─ Crear work order SAP
    ↓
Almacenar en TimescaleDB
    ↓
Actualizar dashboards
```

---

## 7. MODELOS DE DATOS

```sql
-- TimescaleDB
CREATE TABLE vibration_measurements (
    time TIMESTAMPTZ NOT NULL,
    equipment_id VARCHAR(50),
    axis CHAR(1), -- X, Y, Z
    frequency_hz DECIMAL(10,2),
    amplitude DECIMAL(12,4),
    acceleration_g DECIMAL(8,4),
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

SELECT create_hypertable('vibration_measurements', 'time', 
    if_not_exists => TRUE);

CREATE TABLE anomalies (
    id SERIAL PRIMARY KEY,
    measurement_id BIGINT,
    anomaly_score DECIMAL(5,2),
    predicted_failure_days INTEGER,
    anomaly_type VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE maintenance_logs (
    id SERIAL PRIMARY KEY,
    equipment_id VARCHAR(50),
    action VARCHAR(200),
    scheduled_date DATE,
    completed_date DATE,
    cost_usd DECIMAL(10,2),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

---

## 8. EJEMPLO DE CÓDIGO

```typescript
// application/VibrationAnalysisService.ts
@Injectable()
export class VibrationAnalysisService {
  async analyzeMeasurement(
    measurement: VibrationMeasurement
  ): Promise<AnalysisResult> {
    // 1. Análisis FFT
    const fftResult = await this.fftAnalyzer.analyze(measurement.signal);
    
    // 2. Análisis Wavelet
    const waveletResult = await this.waveletAnalyzer.analyze(
      measurement.signal
    );
    
    // 3. Extracción características
    const features = {
      rms: this.calculateRMS(measurement.signal),
      peak: Math.max(...measurement.signal),
      kurtosis: this.calculateKurtosis(measurement.signal),
      frequency_peak: fftResult.dominantFrequency,
      ...
    };
    
    // 4. Predicción
    const prediction = await this.predictor.predict(features);
    
    return new AnalysisResult(features, prediction);
  }

  private calculateRMS(signal: number[]): number {
    const sumSquares = signal.reduce((sum, val) => sum + val * val, 0);
    return Math.sqrt(sumSquares / signal.length);
  }
}

// Python ML Predictor
def predict_anomaly(features: dict) -> dict:
    model = joblib.load('models/vibration_classifier.pkl')
    X = prepare_features(features)
    
    probability = model.predict_proba(X)[0]
    is_anomaly = probability[1] > THRESHOLD
    
    remaining_life = estimate_remaining_life(probability)
    
    return {
        'is_anomaly': is_anomaly,
        'anomaly_score': probability[1],
        'remaining_days': remaining_life
    }
```

---

## 9. SECURITY & COMPLIANCE

- **Autenticación:** OAuth 2.0 + JWT
- **Encriptación:** TLS 1.3, AES-256
- **Auditoría:** Todos los cambios registrados
- **Cumplimiento:** ISO 20816 (vibración), ISO 9001
- **Retención:** 5 años de datos
- **Disponibilidad:** 99.9% SLA

---

## 10. PERFORMANCE TARGETS

| Métrica | Target |
|---------|--------|
| Latencia análisis | < 500 ms |
| Precisión predicción | > 92% |
| Tasa falsos positivos | < 5% |
| Disponibilidad | 99.9% |
| Retención datos | 5 años |

---

## 11. CONVENCIONES

- TypeScript + Python combinados
- Máximo 300 líneas por archivo
- Nombres descriptivos de variables
- Documentación de funciones completa
