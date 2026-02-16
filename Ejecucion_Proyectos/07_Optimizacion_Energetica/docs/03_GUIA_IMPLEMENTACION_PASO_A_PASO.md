# GUÍA IMPLEMENTACIÓN - OPTIMIZACIÓN ENERGÉTICA
**Duración:** 24 semanas | **Stack:** Python, Spark, Tableau, PostgreSQL

## FASES DE IMPLEMENTACIÓN

### FASE 1: BASELINE (12 SEMANAS)

#### Paso 1-10: Data Collection (Semanas 1-6)
```bash
# Conectar a SCADA y sistemas de billing
python connect_scada.py --host scada.plant.com --protocol OPC-UA

# Ingesta de datos históricos 12 meses
python ingest_historical.py \
  --start 2023-01 --end 2024-01 \
  --variables power,temp,humidity,production

# Resultado: 1.8B+ datapoints en PostgreSQL
```

#### Paso 11-20: EDA y Baseline Model (Semanas 7-12)
```python
# eda_and_baseline.py
import pyspark.sql as sql
import pandas as pd
from sklearn.preprocessing import StandardScaler
from statsmodels.tsa.seasonal import seasonal_decompose

# Load data
df = spark.read.parquet('energy_data.parquet')

# Exploratory Data Analysis
print(df.describe())
print(df.corr())

# Seasonal decomposition
ts_data = df.groupby('date').agg({'energy_consumption': 'sum'})
decomposition = seasonal_decompose(ts_data, model='additive', period=365)

# Baseline model (multiple regression)
from sklearn.linear_model import LinearRegression
X = df[['temp', 'humidity', 'production_volume', 'day_of_week']]
y = df['energy_consumption']

model = LinearRegression()
model.fit(X_train, y_train)

# Validate MAPE < 10%
from sklearn.metrics import mean_absolute_percentage_error
mape = mean_absolute_percentage_error(y_test, model.predict(X_test))
assert mape < 0.10, f"MAPE {mape} > 0.10"
```

---

### FASE 2: RECOMENDACIONES (8 SEMANAS)

#### Paso 21-40: Opportunity Identification
```python
# identify_opportunities.py
opportunities = [
    {'id': 1, 'category': 'HVAC', 'description': 'Optimize thermostat setpoints', 'potential_savings': 400000},
    {'id': 2, 'category': 'Steam', 'description': 'Fix steam trap leaks', 'potential_savings': 250000},
    {'id': 3, 'category': 'Compressed Air', 'description': 'Reduce line pressure', 'potential_savings': 150000},
    {'id': 4, 'category': 'Lighting', 'description': 'LED upgrade campaign', 'potential_savings': 100000},
    {'id': 5, 'category': 'Motors', 'description': 'VFD installation on pumps', 'potential_savings': 80000},
    # ... 45+ more opportunities
]

# ROI Calculation
def calculate_roi(opportunity, implementation_cost, years=3):
    annual_savings = opportunity['potential_savings']
    total_savings = annual_savings * years
    roi = (total_savings - implementation_cost) / implementation_cost
    payback_period = implementation_cost / annual_savings
    return {'roi': roi, 'payback_years': payback_period}

# Prioritize by ROI
roi_results = sorted(opportunities, key=lambda x: calculate_roi(x, 50000)['roi'], reverse=True)
top_50 = roi_results[:50]

# Save to dashboard
df_opportunities = pd.DataFrame(top_50)
df_opportunities.to_csv('opportunities.csv')
```

---

### FASE 3: FORECASTING (3 SEMANAS)

#### Paso 41-50: ARIMA & LSTM Models
```python
# forecast_models.py
import statsmodels.api as sm
from tensorflow.keras import Sequential
from tensorflow.keras.layers import LSTM, Dense

# ARIMA Model
arima_model = sm.tsa.ARIMA(train_data, order=(1,1,1))
arima_fit = arima_model.fit()
arima_forecast = arima_fit.forecast(steps=30)

# LSTM Model
model = Sequential([
    LSTM(128, input_shape=(lookback, 1), return_sequences=True),
    LSTM(64),
    Dense(32, activation='relu'),
    Dense(1)
])
model.compile(optimizer='adam', loss='mse')
model.fit(X_train, y_train, epochs=50, batch_size=32)
lstm_forecast = model.predict(X_test)

# Ensemble (average both models)
ensemble_forecast = (arima_forecast + lstm_forecast) / 2

# Validate MAPE < 15%
mape = mean_absolute_percentage_error(y_test, ensemble_forecast)
assert mape < 0.15, f"MAPE {mape} > 0.15"
```

---

### FASE 4: DASHBOARD & DEPLOYMENT (1 SEMANA)

#### Paso 51-60: Tableau Dashboard
```bash
# Create Tableau dashboard with:
# - Real-time energy consumption (line chart)
# - Baseline vs actual (comparison)
# - 30-day forecast (prediction)
# - Top 50 opportunities (table, sortable)
# - Monthly ROI report (KPI cards)
# - Savings achieved to date (progress bar)

# API endpoint for dashboard data
python dashboard_api.py --port 5000
curl http://localhost:5000/api/energy_status
curl http://localhost:5000/api/opportunities
curl http://localhost:5000/api/forecast
```

#### Paso 61-70: SCADA Integration
```python
# scada_integration.py
from opcua import Client

client = Client("opc.tcp://scada.plant.com:4840")
client.connect()

# Read real-time values
energy_node = client.get_node("ns=2;i=4")
energy_value = energy_node.get_value()

# Compare with forecast, trigger alerts if >10% deviation
baseline = get_baseline_forecast()
deviation = abs(energy_value - baseline) / baseline

if deviation > 0.10:
    send_alert(f"Energy consumption {deviation*100:.1f}% above forecast")

client.disconnect()
```

---

## TROUBLESHOOTING (20+ PROBLEMAS)

| Problema | Solución |
|----------|----------|
| Data quality <99% | Implement validators, data cleaning |
| Baseline MAPE >10% | Add more features, domain analysis |
| Missing SCADA connection | Use historical files, batch processing |
| Forecast drift | Monthly retraining, continuous validation |
| ROI calculation errors | Domain expert review, sensitivity analysis |
| Adoption resistance | Change management, clear benefits |
| Integration delays | Mock SCADA for testing |
| Performance slow | Spark optimization, caching |
| Weather data unavailable | Use temperature proxy from SCADA |
| Seasonal anomalies | Identify patterns, adjust model |
| Outliers in data | Robust statistics, outlier detection |
| Feature engineering | Domain expert guidance |
| Model overfitting | Cross-validation, regularization |
| Dashboard slow | Query optimization, caching |
| API latency | Batch predictions, caching |
| User confusion | Training sessions, documentation |
| Budget overrun | Prioritize features |
| Timeline delays | Scope reduction |
| Team turnover | Documentation key |
| Monitoring gaps | Configure alerts, dashboards |

---

**FIN GUÍA OPTIMIZACIÓN ENERGÉTICA**
