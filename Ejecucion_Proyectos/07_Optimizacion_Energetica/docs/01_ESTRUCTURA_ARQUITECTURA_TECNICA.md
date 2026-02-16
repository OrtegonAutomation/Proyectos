# 01 - ESTRUCTURA Y ARQUITECTURA TÉCNICA
## Optimización Energética - Sistema de Gestión Inteligente

---

## 1. DIAGRAMA C4 - NIVEL 1

```
┌──────────────────────────────────────────────────┐
│    SISTEMA OPTIMIZACIÓN ENERGÉTICA                │
│     (Energy Management & Optimization)            │
├──────────────────────────────────────────────────┤
│                                                  │
│  Medidores    Ingesta      Analytics    Control  │
│  Energía      ┌────────┐   ┌───────┐  ┌────────┐│
│  ┌────────┐   │ Tiempo │──>│ ML    │─>│ Engine ││
│  │Activos │──>│Series  │   │Optim  │  │Control ││
│  │SCADA   │   │ Energy │   │      │   │Automát││
│  └────────┘   └────────┘   └───────┘  └────────┘│
│                    │            │         │     │
│                    └────────────┴─────────┘     │
│                         │                       │
│         Dashboard | Recomendaciones             │
│         Reportes | Historial                    │
│                                                 │
└──────────────────────────────────────────────────┘
```

---

## 2. ARQUITECTURA DE 6 CAPAS

### 2.1 CAPA DE PRESENTACIÓN
- Dashboard en tiempo real (React 18.2)
- Recomendaciones operacionales
- Alertas de sobreconsumo
- Stack: React, TypeScript, Chart.js

### 2.2 CAPA DE APLICACIÓN
- Servicio análisis consumo
- Motor de optimización
- Gestor de recomendaciones
- Stack: Node.js, Express, TypeScript

### 2.3 CAPA DE INTEGRACIÓN
- Adaptador SCADA (Modbus, OPC-UA)
- Conector SAP (compras energía)
- APIs de mercado eléctrico
- Kafka para streaming

### 2.4 CAPA DE DOMINIO
- Entidades: Medición, Equipo, Recomendación
- Agregado: EnergyOptimization
- Lógica de optimización

### 2.5 CAPA DE PERSISTENCIA
- TimescaleDB 2.11 (series temporales)
- PostgreSQL 15 (datos transaccionales)
- Redis 7.0 (caché)
- S3 (modelos ML)

### 2.6 CAPA DE INFRAESTRUCTURA
- ML: TensorFlow 2.13, scikit-learn
- Python 3.11 (optimización)
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
07_Optimizacion_Energetica/
├── src/
│   ├── presentation/
│   │   ├── controllers/EnergyController.ts
│   │   ├── routes/optimization.routes.ts
│   │   └── dto/ConsumptionDataDTO.ts
│   ├── application/
│   │   ├── services/EnergyAnalysisService.ts
│   │   ├── services/OptimizationService.ts
│   │   ├── usecases/AnalyzeConsumptionUseCase.ts
│   │   └── validators/EnergyValidator.ts
│   ├── domain/
│   │   ├── entities/EnergyMeasurement.entity.ts
│   │   ├── entities/Equipment.entity.ts
│   │   ├── aggregates/OptimizationAggregate.ts
│   │   └── repositories/MeasurementRepository.interface.ts
│   ├── infrastructure/
│   │   ├── scada/SCADAAdapter.ts
│   │   ├── scada/ModbusClient.ts
│   │   ├── ml/OptimizationEngine.ts
│   │   ├── ml/optimizer.py
│   │   ├── timeseries/TimescaleDBAdapter.ts
│   │   ├── external/SAPConnector.ts
│   │   └── external/MarketDataService.ts
│   ├── optimization/
│   │   ├── LoadForecaster.ts
│   │   ├── PricingAnalyzer.ts
│   │   └── ScheduleOptimizer.py
│   └── main.ts
├── tests/
│   ├── unit/
│   ├── integration/
│   └── e2e/
├── ml-models/
│   ├── forecasting/
│   ├── optimization/
│   └── evaluation/
├── config/
│   ├── scada.config.ts
│   ├── optimization.config.ts
│   └── market.config.ts
└── scripts/
    ├── data-preparation/
    ├── model-training/
    └── batch-optimization/
```

---

## 4. COMPONENTES DETALLADOS (15+)

| # | Componente | Responsabilidad | Stack | Input | Output |
|---|-----------|-----------------|-------|-------|--------|
| 1 | API REST | Enrutamiento | Express 4.18 | HTTP | JSON |
| 2 | SCADA Gateway | Lectura medidores | Modbus/OPC-UA | Telemetría SCADA | Events |
| 3 | Energy Ingester | Buffer de energía | Kafka + Redis | Mediciones | Series procesadas |
| 4 | Load Forecaster | Predicción carga | TensorFlow 2.13 | Histórico + clima | Forecast 24h |
| 5 | Price Analyzer | Análisis precios | Python pandas | Precios mercado | Trends precios |
| 6 | Schedule Optimizer | Optimización schedule | Scipy + Pulp | Forecast + precios | Plan optimal |
| 7 | Equipment Manager | Gestión equipos | Node.js | Configs | Parámetros control |
| 8 | Recommendation Engine | Generar recomendaciones | Node.js | Análisis | Acciones recomendadas |
| 9 | TimeSeries Store | Almacenamiento datos | TimescaleDB 2.11 | Mediciones | Queries optimizadas |
| 10 | Cache Layer | Caché | Redis 7.0 | Queries frecuentes | Datos en caché |
| 11 | SAP Connector | Integración SAP | Node-odata | Compras energía | Sincronización |
| 12 | Model Manager | Gestión modelos | MLflow | Modelos entrenados | Versioning |
| 13 | Alert Engine | Alertas | Node.js | Thresholds | Notificaciones |
| 14 | Reportero | Reportes | Reportlab | Datos agregados | PDF/Excel |
| 15 | Audit Logger | Auditoría | Winston + ELK | Eventos | Logs centralizados |

---

## 5. STACK TECNOLÓGICO

```
BACKEND:
  - Node.js 20 LTS + TypeScript 5.1
  - Express 4.18 (APIs REST)
  - Python 3.11 (optimización)
  - FastAPI 0.104 (APIs Python)

MACHINE LEARNING:
  - TensorFlow 2.13 (forecasting)
  - Prophet 1.1 (series temporales)
  - scikit-learn 1.3 (modelos)
  - XGBoost 2.0 (ensemble)
  - pandas 2.0 (análisis)

OPTIMIZATION:
  - PuLP 2.7 (linear programming)
  - SciPy 1.11 (optimización)
  - Pyomo 6.5 (constraint programming)
  - CPLEX / Gurobi (solvers - opcional)

SCADA & INDUSTRIAL:
  - pymodbus 3.4 (Modbus protocol)
  - python-opcua 0.98 (OPC-UA)
  - modbus-tk (alternativo)

DATABASE:
  - TimescaleDB 2.11 (time-series)
  - PostgreSQL 15.3 (datos)
  - Redis 7.0 (caché)
  - InfluxDB 2.6 (alternativo)

EXTERNAL DATA:
  - APIs de mercado eléctrico
  - APIs de clima
  - APIs de precios

MONITORING:
  - Prometheus 2.45
  - Grafana 10.0
  - ELK Stack 8.8
```

---

## 6. DATA FLOW: Optimizar Consumo

```
Lectura Medidores (SCADA)
    ↓ (Modbus/OPC-UA)
Normalizar unidades
    ↓
Buffer en Kafka (1 min)
    ↓
Ingesta en TimescaleDB
    ↓
Análisis de Carga Actual
├─ Demanda real vs predicción
├─ Equipos activos
└─ Factores ambientales
    ↓
Forecast de Carga (24h)
├─ ML model con histórico
├─ Factores estacionales
└─ Predicción de demanda pico
    ↓
Obtener Precios Mercado
├─ API mercado eléctrico
├─ Proyectar precios horarios
└─ Identificar horas baratas
    ↓
Optimización (Linear Programming)
├─ Objetivo: Minimizar costo
├─ Restricciones: Capacidad equipo
├─ Restricciones: SLA operacional
└─ Generar plan horario
    ↓
Validar Plan
├─ Posibilidad física
├─ Riesgos de producción
└─ Cambios de equipos necesarios
    ↓
IF plan válido
    ├─ Generar recomendaciones
    ├─ Enviar a dashboard
    ├─ Log en BD
    └─ Alertar operadores
    ↓
Si permitido
    ├─ Enviar comandos SCADA
    ├─ Cambiar setpoints
    └─ Monitorear resultados
    ↓
Almacenar resultados
    ↓
Actualizar métricas ahorro
```

---

## 7. MODELOS DE DATOS

```sql
CREATE TABLE energy_measurements (
    time TIMESTAMPTZ NOT NULL,
    equipment_id VARCHAR(50),
    power_kw DECIMAL(10,2),
    energy_kwh DECIMAL(12,2),
    voltage_v DECIMAL(8,2),
    current_a DECIMAL(8,2),
    frequency_hz DECIMAL(6,2),
    power_factor DECIMAL(5,2)
);

SELECT create_hypertable('energy_measurements', 'time', 
    if_not_exists => TRUE);

CREATE TABLE optimization_schedules (
    id SERIAL PRIMARY KEY,
    execution_date DATE NOT NULL,
    hour_of_day INTEGER,
    forecast_load_kw DECIMAL(10,2),
    market_price_usd_per_mwh DECIMAL(8,2),
    recommended_action VARCHAR(200),
    expected_savings_usd DECIMAL(10,2),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE equipment_metrics (
    id SERIAL PRIMARY KEY,
    equipment_id VARCHAR(50),
    daily_consumption_kwh DECIMAL(12,2),
    cost_usd DECIMAL(10,2),
    efficiency_percent DECIMAL(5,2),
    measurement_date DATE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_equipment (equipment_id),
    INDEX idx_date (measurement_date DESC)
);
```

---

## 8. EJEMPLO DE CÓDIGO

```typescript
// application/services/OptimizationService.ts
@Injectable()
export class OptimizationService {
  async optimizeConsumption(date: Date): Promise<Schedule[]> {
    // 1. Obtener histórico
    const historical = await this.getHistoricalData(date);
    
    // 2. Forecast de carga
    const forecast = await this.forecaster.predict(historical);
    
    // 3. Obtener precios mercado
    const prices = await this.marketService.getPrices(date);
    
    // 4. Ejecutar optimización
    const schedule = await this.optimizationEngine.optimize({
      forecast,
      prices,
      constraints: this.getConstraints()
    });
    
    // 5. Validar plan
    const validated = await this.validateSchedule(schedule);
    
    if (validated.isValid) {
      await this.saveAndNotify(validated.schedule);
    }
    
    return validated.schedule;
  }

  private getConstraints(): Constraint[] {
    return [
      { type: 'MAX_CAPACITY', value: 50000 },
      { type: 'MIN_PRESSURE', value: 80 },
      { type: 'RAMP_RATE', value: 1000 }
    ];
  }
}

// Python Optimization Engine
def optimize_energy_schedule(forecast, prices, constraints):
    from pulp import *
    
    prob = LpProblem("Energy_Optimization", LpMinimize)
    
    # Variables: potencia por hora
    power_vars = {h: LpVariable(f"P_h{h}", 0, constraints['MAX_CAPACITY'])
                  for h in range(24)}
    
    # Función objetivo: minimizar costo
    cost = lpSum([power_vars[h] * prices[h] for h in range(24)])
    prob += cost, "Total_Cost"
    
    # Restricciones
    for h in range(24):
        prob += power_vars[h] <= forecast[h] * 1.1  # No exceder 110% forecast
    
    # Resolver
    prob.solve(PULP_CBC_CMD(msg=0))
    
    schedule = [power_vars[h].varValue for h in range(24)]
    total_cost = value(prob.objective)
    
    return {
        'schedule': schedule,
        'total_cost': total_cost,
        'status': LpStatus[prob.status]
    }
```

---

## 9. SECURITY & COMPLIANCE

- **Autenticación:** OAuth 2.0 + JWT
- **Encriptación:** TLS 1.3, AES-256
- **Auditoría:** Todos cambios registrados
- **Cumplimiento:** ISO 50001 (energy management)
- **Datos:** 3 años retención
- **Disponibilidad:** 99.9% SLA

---

## 10. PERFORMANCE TARGETS

| Métrica | Target |
|---------|--------|
| Tiempo optimización | < 5 seg |
| Precisión forecast | > 90% |
| Ahorro energético | 20-30% |
| API response | < 100 ms |
| Disponibilidad | 99.9% |

---

## 11. CONVENCIONES

- TypeScript + Python combinados
- Máximo 300 líneas por archivo
- Nombres descriptivos
- Documentación completa
