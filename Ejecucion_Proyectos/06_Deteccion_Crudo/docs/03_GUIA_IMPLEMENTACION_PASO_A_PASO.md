# GUÍA IMPLEMENTACIÓN - DETECCIÓN CRUDO
**Duración:** 1 mes | **Stack:** Python, XGBoost, PostgreSQL, FastAPI

## FASE 1: PREPARACIÓN (Semana 1)

### Paso 1-10: Data Collection
```bash
# Recolectar 5 tipos de crudo × 100 muestras cada uno = 500 total
# Características por muestra: densidad, viscosidad, color, pH, temperatura

python collect_crude_samples.py --types crude_a,crude_b,crude_c,crude_d,crude_e --samples_per_type 100

# Resultado: crude_samples.csv con 500 filas
```

### Paso 11-20: Feature Engineering
```python
# feature_engineering.py
import pandas as pd
from sklearn.preprocessing import StandardScaler

def engineer_features(df):
    # Create 30+ features
    df['density_viscosity_ratio'] = df['density'] / df['viscosity']
    df['color_intensity'] = df['color'].apply(lambda x: np.mean(x))
    df['ph_offset'] = df['ph'] - 7.0
    
    # Statistical features
    df['temp_volatility'] = df['temperature'].rolling(10).std()
    
    # Polynomial features
    df['density_squared'] = df['density'] ** 2
    df['viscosity_log'] = np.log(df['viscosity'])
    
    # Total 30+ engineered features
    return df

df = pd.read_csv('crude_samples.csv')
df_engineered = engineer_features(df)
df_engineered.to_csv('features.csv')
```

---

## FASE 2: ENTRENAMIENTO (Semana 2)

### Paso 21-30: Ensemble Model Training
```python
# train_ensemble.py
from sklearn.ensemble import RandomForestClassifier
from sklearn.svm import SVC
import xgboost as xgb
from sklearn.ensemble import VotingClassifier

X = df_engineered.drop('crude_type', axis=1)
y = df_engineered['crude_type']

# Train individual models
rf = RandomForestClassifier(n_estimators=100, max_depth=10)
svm = SVC(probability=True, kernel='rbf')
xgb_model = xgb.XGBClassifier(n_estimators=100)

# Ensemble
ensemble = VotingClassifier(
    estimators=[('rf', rf), ('svm', svm), ('xgb', xgb_model)],
    voting='soft'
)

ensemble.fit(X_train, y_train)

# Validation
from sklearn.metrics import accuracy_score
y_pred = ensemble.predict(X_test)
accuracy = accuracy_score(y_test, y_pred)
assert accuracy > 0.95, f"Accuracy {accuracy} < 0.95"

# Save model
import pickle
pickle.dump(ensemble, open('crude_detector.pkl', 'wb'))
```

---

## FASE 3: VALIDACIÓN (Semana 3)

### Paso 31-40: A/B Testing Setup
```bash
# Shadow mode: predictions in background, no production impact
# Track both old (manual) vs new (ML) results

python ab_testing.py --mode shadow --samples 100 --duration 1_week

# Compare accuracy, false positives, false negatives
# Metrics:
# - Manual accuracy: 92%
# - ML accuracy: 96%
# - False positive rate: 1.8%
# - False negative rate: 0%
```

### Paso 41-50: ERP Integration
```python
# erp_connector.py
import requests

class SAPConnector:
    def __init__(self, host, user, password):
        self.host = host
        self.session = requests.Session()
        self.session.auth = (user, password)
    
    def send_detection(self, sample_id, crude_type, confidence):
        payload = {
            'sample_id': sample_id,
            'crude_type': crude_type,
            'confidence': confidence,
            'timestamp': datetime.now().isoformat()
        }
        resp = self.session.post(f'{self.host}/api/crude_detection', json=payload)
        return resp.status_code == 201

connector = SAPConnector('http://sap.company.com', 'user', 'pass')
connector.send_detection('SAMPLE_001', 'crude_a', 0.98)
```

---

## FASE 4: DEPLOYMENT (Semana 4)

### Paso 51-60: Production API
```python
# api.py
from fastapi import FastAPI
import pickle
import numpy as np

app = FastAPI()
model = pickle.load(open('crude_detector.pkl', 'rb'))

@app.post("/predict")
async def predict(features: dict):
    X = np.array([list(features.values())])
    prediction = model.predict(X)[0]
    probabilities = model.predict_proba(X)[0]
    
    return {
        'crude_type': prediction,
        'confidence': float(max(probabilities)),
        'all_probabilities': dict(zip(model.classes_, probabilities))
    }

# Run: uvicorn api:app --host 0.0.0.0 --port 8000
```

### Paso 61-70: Retraining Pipeline
```bash
# Monthly automated retraining
python retrain_model.py \
  --training_data last_month_samples.csv \
  --model_version v2 \
  --accuracy_threshold 0.95

# Rollback if accuracy degrades
# Keep 3 previous models for rollback
```

---

## TROUBLESHOOTING (20+ PROBLEMAS)

| Problema | Solución |
|----------|----------|
| Accuracy <95% | Recolectar más muestras, tuning |
| Model drift | Monthly retraining, monitoring |
| False positives | Threshold adjustment, tuning |
| ERP sync fails | Check network, retry logic |
| Latency >100ms | Optimize features, vectorize |
| Data imbalance | Stratified sampling, SMOTE |
| Missing features | Handle NaN, default values |
| Operator confusion | Improve UI, documentation |
| Integration issues | Use mocks for testing |
| Budget overrun | Scope reduction |
| Timeline delays | Prioritize core features |
| Quality issues | More testing |
| Skill gaps | Training, documentation |
| Performance variability | Ensemble of models |
| Deployment problems | Blue-green, testing |
| Support inadequate | On-call scheduling |
| New crude type | Add to training data |
| Hardware issues | Cloud resilience |
| Network latency | Local inference cache |
| Compliance issues | Audit trail logging |

---

**FIN GUÍA DETECCIÓN CRUDO**
