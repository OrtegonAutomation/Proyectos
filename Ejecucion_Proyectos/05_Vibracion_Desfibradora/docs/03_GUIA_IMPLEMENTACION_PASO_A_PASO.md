# GUÍA IMPLEMENTACIÓN - VIBRACIÓN-DESFIBRADORA
**Duración:** 12 semanas | **Stack:** Python, TensorFlow, InfluxDB, Grafana, MQTT

## FASE 1: SENSORES Y RECOLECCIÓN (Semanas 1-4)

### Paso 1-10: Instalación Sensores
```bash
# 1. Instalar 20 acelerómetros (MPU-9250) en equipos
# Puntos: motor principal, desfibradora rotor, bearings (3), transmisión

# 2. Configurar MQTT broker
docker run -d -p 1883:1883 eclipse-mosquitto:latest

# 3. NodeMCU/ESP32 firmware para cada sensor
# Leer acelerómetro + enviar MQTT

# 4. Validar conectividad
mqtt_sub -h localhost -t 'sensors/+' -v

# 5. Calibración inicial
python calibrate_sensors.py --config sensors.json
```

### Paso 11-20: InfluxDB Setup
```bash
# Instalar InfluxDB
docker run -d -p 8086:8086 -v influxdb-storage:/var/lib/influxdb2 influxdb:latest

# Crear bucket para vibraciones
influx bucket create --name vibracion_data --retention 52w

# Configurar Telegraf para MQTT→InfluxDB
cat > telegraf.conf << 'EOF'
[[inputs.mqtt_consumer]]
  servers = ["tcp://localhost:1883"]
  topics = ["sensors/+"]
  data_format = "json"

[[outputs.influxdb_v2]]
  urls = ["http://localhost:8086"]
  bucket = "vibracion_data"
EOF

telegraf --config telegraf.conf
```

### Paso 21-30: Histórico de Datos
```bash
# Importar 12 meses de datos históricos
python import_historical_data.py \
  --source desfibradora_logs.csv \
  --destination influxdb \
  --start 2023-01 --end 2024-01

# Validar importación
influx query 'from(bucket:"vibracion_data") |> range(start: -365d) |> count()'

# Resultado: ~1.5M registros
```

---

## FASE 2: SIGNAL PROCESSING (Semanas 5-7)

### Paso 31-40: Extracción de Features
```python
# extract_features.py
import numpy as np
from scipy import signal, fft

def extract_vibration_features(accelerometer_data):
    # Time domain features
    features = {
        'mean': np.mean(accelerometer_data),
        'std': np.std(accelerometer_data),
        'rms': np.sqrt(np.mean(accelerometer_data**2)),
        'peak': np.max(np.abs(accelerometer_data)),
        'crest_factor': np.max(np.abs(accelerometer_data)) / np.sqrt(np.mean(accelerometer_data**2))
    }
    
    # Frequency domain (FFT)
    fft_vals = np.abs(fft.fft(accelerometer_data))
    features['fft_peak'] = np.max(fft_vals)
    features['fft_energy'] = np.sum(fft_vals**2)
    
    # Wavelet analysis
    coeffs = signal.morlet(N=256, w=6)
    cwt = signal.cwt(accelerometer_data, signal.morlet)
    features['wavelet_energy'] = np.sum(np.abs(cwt)**2)
    
    return features

# Calcular 50+ features para cada sensor
# Normalizar con StandardScaler
```

---

## FASE 3: MACHINE LEARNING (Semanas 6-8)

### Paso 41-50: LSTM Model
```python
# lstm_model.py
from tensorflow import keras
from tensorflow.keras import layers

model = keras.Sequential([
    layers.LSTM(128, input_shape=(timesteps, n_features), return_sequences=True),
    layers.Dropout(0.2),
    layers.LSTM(64, return_sequences=False),
    layers.Dense(32, activation='relu'),
    layers.Dense(1)  # TTF prediction in hours
])

model.compile(optimizer='adam', loss='mse', metrics=['mae'])

# Entrenar con 12 meses de datos históricos
history = model.fit(X_train, y_train, epochs=50, batch_size=32, validation_split=0.2)

# Validar RMSE < 10%
from sklearn.metrics import mean_squared_error
rmse = np.sqrt(mean_squared_error(y_test, model.predict(X_test)))
assert rmse < 0.1 * np.mean(y_test), f"RMSE {rmse} exceeds 10%"
```

### Paso 51-60: Anomaly Detection
```python
# anomaly_detection.py
from sklearn.ensemble import IsolationForest

# Entrenar Isolation Forest en datos normales
iso_forest = IsolationForest(contamination=0.05, random_state=42)
iso_forest.fit(X_normal)

# Predecir anomalías (datos nuevos)
predictions = iso_forest.predict(X_test)  # -1 = anomaly, 1 = normal

# Calcular F1 score
from sklearn.metrics import f1_score
f1 = f1_score(y_true, predictions == -1)
assert f1 > 0.92, f"F1 score {f1} below 0.92"
```

---

## FASE 4: DASHBOARD Y DEPLOYMENT (Semanas 9-12)

### Paso 61-70: Grafana Dashboards
```bash
# Crear dashboard Grafana con:
# - Real-time vibration levels (graph)
# - TTF predictions (gauge)
# - Anomaly alerts (table)
# - Historical trends (heatmap)
# - Sensor status (stat panels)

# API endpoint
curl -X POST http://localhost:3000/api/dashboards/db \
  -H "Authorization: Bearer " \
  -d @dashboard.json
```

### Paso 71-80: Production Deployment
```bash
# Kubernetes deployment
kubectl apply -f k8s/vibration-stack.yaml

# Services:
# - MQTT broker (pub/sub)
# - InfluxDB (storage)
# - TensorFlow model (API)
# - Grafana (visualization)
# - Prometheus (monitoring)

# Validar
kubectl get pods -n vibracion
curl http://vibration-api:5000/health
```

---

## TROUBLESHOOTING (20+ PROBLEMAS)

| Problema | Solución |
|----------|----------|
| Sensor drift | Recalibración semanal |
| MQTT message loss | QoS=1, persistent storage |
| InfluxDB query slow | Índices, downsampling |
| Model degradation | Monthly retraining |
| High latency | Vectorize predictions |
| Memory OOM | Batch processing |
| GPU unavailable | CPU fallback, optimization |
| Dashboard timeout | Query optimization |
| Alert fatigue | Threshold tuning |
| Data corruption | Backup/restore |
| Operator confusion | UI/UX improvements |
| False alarms | Anomaly threshold adjustment |
| Missing sensors | Alert, degraded mode |
| Network issues | Local cache, retry logic |
| Clock skew | NTP synchronization |
| Disk full | Archive old data |
| CPU high | Profile, optimize code |
| Connection pool | Scale connections |
| Rate limiting | Batch requests |
| Security concerns | Encryption, access control |

---

**FIN GUÍA VIBRACIÓN**
