# PLAN DE TRABAJO DETALLADO - PROYECTO DETECCIÓN CRUDO
**Duración:** 1 mes | **Prioridad:** MEDIA | **Stack:** Python, XGBoost, PostgreSQL

---

## 1. WBS - ESTRUCTURA DESGLOSADA

```
DETECCIÓN CRUDO
├─ FASE 1: Preparación & Diseño (Semana 1)
│  ├─ 1.1 Requisitos funcionales (5 tipos crudo)
│  ├─ 1.2 Arquitectura ML pipeline
│  ├─ 1.3 Especificación de features (30+)
│  └─ 1.4 Plan de datos y validación
│
├─ FASE 2: Ingeniería de Features & Entrenamiento (Semana 2)
│  ├─ 2.1 Recolección datos referencia (5 tipos × 100 muestras)
│  ├─ 2.2 Feature engineering (30+ features)
│  ├─ 2.3 Ensemble model training (RF, SVM, XGBoost)
│  └─ 2.4 Hiperparam tuning y validation
│
├─ FASE 3: Validación & Integration (Semana 3)
│  ├─ 3.1 A/B testing setup (shadow mode)
│  ├─ 3.2 ERP integration (API, datos)
│  ├─ 3.3 Real-time classification API
│  └─ 3.4 Performance monitoring setup
│
└─ FASE 4: Deployment & Monitoreo (Semana 4)
   ├─ 4.1 Producción deployment
   ├─ 4.2 Retraining procedures (mensual)
   ├─ 4.3 Operador training
   └─ 4.4 24/7 support
```

---

## 2. GANTT ASCII - 4 SEMANAS

```
SEMANA 1: Diseño
├─ Requisitos análisis       [████]
├─ Arquitectura diseño       [████]
├─ Data strategy             [████]
└─ Validación plan           [████]

SEMANA 2: Entrenamiento
├─ Data collection           [████████]
├─ Feature engineering       [████████████]
├─ RF model training         [████]
├─ SVM model training        [████]
├─ XGBoost training          [████]
└─ Ensemble tuning           [████]

SEMANA 3: Validación & Integration
├─ A/B testing setup         [████████]
├─ ERP connector dev         [████████]
├─ API implementation        [████]
├─ Shadow mode testing       [████████]
└─ Performance validation    [████]

SEMANA 4: Deployment
├─ Production setup          [████]
├─ Model deployment          [████]
├─ Operator training         [████]
├─ Go-live support           [████]
└─ Documentation             [████]
```

---

## 3. MILESTONES (8-10)

| # | Milestone | Fecha | Criterios |
|---|-----------|-------|-----------|
| 1 | Design Complete | D+5 | Arch approved, 30+ features defined |
| 2 | Data Ready | D+8 | 500 samples × 5 tipos, labeled, balanced |
| 3 | Models Trained | D+12 | 3 models trained, hyperparams tuned |
| 4 | Ensemble Ready | D+14 | Combined model, accuracy 95%+ |
| 5 | A/B Testing | D+18 | Shadow mode, 100 samples validated |
| 6 | ERP Integration | D+20 | Connector working, data sync 100% |
| 7 | Performance OK | D+22 | <100ms latency, <2% FP rate |
| 8 | Production | D+27 | Deployed, 24/7 support active |

---

## 4. PRESUPUESTO: $160K-220K

```
Recursos Humanos (40%)....$64-88K
├─ ML Engineer Lead..........$16K
├─ Backend Engineer..........$12K
├─ QA/Testing...............$8K
├─ Domain Expert.............$6K
└─ DevOps/Support...........$6K

Infraestructura (35%)......$56-77K
├─ Cloud Compute............$24K
├─ PostgreSQL Managed.......$12K
├─ API Gateway..............$8K
└─ Monitoring..............$4K

Herramientas (15%)........$24-33K
├─ ML libraries (scikit, XGB) $6K
├─ Testing tools............$4K
├─ Monitoring.............$3K
└─ Collaboration..........$2K

Capacitación (5%)............$8K
Contingency (5%)............$8K
```

---

## 5. RIESGOS (10+)

| Riesgo | Prob | Impacto | Mitigation |
|--------|------|---------|-----------|
| Accuracy <95% | Media | Crítica | Multiple models, tuning |
| New crude types | Baja | Media | Retraining process, monitoring |
| Model drift | Media | Alta | Monthly retraining |
| ERP integration | Media | Alta | Early connector dev, mocks |
| False positives | Media | Media | Threshold tuning, validation |
| Data imbalance | Alta | Alta | Stratified sampling, oversampling |
| Feature engineering | Media | Media | Domain expert involvement |
| API latency | Baja | Media | Optimization, vectorization |
| Operator adoption | Baja | Media | Training, clear interface |
| Budget overrun | Media | Media | Scope control |

---

## 6. STACK TECNOLÓGICO

**ML Framework:** Python 3.11, scikit-learn, XGBoost, catboost
**Feature Store:** PostgreSQL + custom feature engineering
**API:** FastAPI, Gunicorn, Docker
**Testing:** Pytest, MLflow for experiment tracking
**Deployment:** Kubernetes, ArgoCD
**Monitoring:** Prometheus, Grafana, Datadog

---

## 7. KPIs CRÍTICOS

- **Accuracy:** 95%+ en todos los tipos
- **False Positives:** <2%
- **False Negatives:** 0 (detectar todos)
- **Latency:** <100ms prediction
- **Confidence Threshold:** >0.90
- **Volume:** 1000+ muestras clasificadas/hora
- **ERP Integration:** 100% successful sync
- **Model Stability:** Monthly retraining cycle

---

**FIN PLAN DETECCIÓN CRUDO**
