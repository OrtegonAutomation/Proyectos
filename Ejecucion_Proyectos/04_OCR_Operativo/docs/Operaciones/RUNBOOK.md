# Runbook de Operaciones - OCR Operativo

**Proyecto:** OCR Operativo
**Versión:** 1.0
**Última actualización:** 2026-02-24
**Responsable:** Equipo de Operaciones IDC Ingeniería

---

## Tabla de Contenidos

1. [Información del Sistema](#1-información-del-sistema)
2. [Procedimientos Diarios](#2-procedimientos-diarios)
3. [Procedimientos Semanales](#3-procedimientos-semanales)
4. [Procedimientos Mensuales](#4-procedimientos-mensuales)
5. [Comandos Útiles](#5-comandos-útiles)
6. [Contactos de Escalación](#6-contactos-de-escalación)

---

## 1. Información del Sistema

### 1.1 Arquitectura General

| Componente | Tecnología | Versión |
|---|---|---|
| Backend API | FastAPI (Python) | 3.11 |
| Frontend | React | 18.x |
| Motor OCR primario | Google Cloud Vision API | v1 |
| Motor OCR fallback | Tesseract | 5.x |
| Base de datos | PostgreSQL (Cloud SQL) | 15 |
| Cache | Redis | 7.x |
| Cola de mensajes | Kafka | 3.x |
| Monitoreo | Prometheus + Grafana | - |
| Orquestación | Kubernetes (GKE) | - |
| Contenedores | Docker | - |
| Integración ERP | SAP | - |

### 1.2 Endpoints Principales

| Endpoint | Descripción | Puerto |
|---|---|---|
| `/api/v1/documents/upload` | Carga y procesamiento de documentos | 8000 |
| `/health` | Health check del sistema | 8000 |
| `/metrics` | Métricas Prometheus | 8000 |
| `/docs` | Documentación Swagger/OpenAPI | 8000 |
| Grafana Dashboard | Monitoreo visual | 3000 |
| Prometheus | Recolección de métricas | 9090 |
| Redis | Cache en memoria | 6379 |
| Kafka | Cola de mensajes | 9092 |
| PostgreSQL | Base de datos | 5432 |

### 1.3 Entornos

| Entorno | Cluster GKE | Namespace | URL Base |
|---|---|---|---|
| Producción | `ocr-prod-cluster` | `ocr-prod` | `https://ocr.idcingenieria.com` |
| Staging | `ocr-staging-cluster` | `ocr-staging` | `https://ocr-staging.idcingenieria.com` |
| Desarrollo | `ocr-dev-cluster` | `ocr-dev` | `https://ocr-dev.idcingenieria.com` |

### 1.4 Ubicación de Credenciales

| Credencial | Ubicación |
|---|---|
| GCP Service Account | `gs://ocr-operativo-secrets/sa-key.json` |
| Kubernetes Secrets | Namespace `ocr-prod` / Secret `ocr-credentials` |
| PostgreSQL | Secret `cloudsql-credentials` |
| Redis | Secret `redis-credentials` |
| SAP/ERP API Key | Secret `sap-integration-credentials` |
| Kafka | Secret `kafka-credentials` |
| Grafana Admin | Secret `grafana-admin-credentials` |
| Vision API Key | Gestionada vía Workload Identity |

### 1.5 Repositorios

| Repositorio | Descripción |
|---|---|
| `github.com/OrtegonAutomation/ocr-operativo` | Código fuente principal |
| `github.com/OrtegonAutomation/ocr-infra` | Infraestructura como código (Terraform) |
| `gcr.io/ocr-operativo/api` | Registro de imágenes Docker |

---

## 2. Procedimientos Diarios

### 2.1 Health Check del Sistema

**Frecuencia:** Cada mañana a las 08:00
**Duración estimada:** 10 minutos
**Responsable:** Operador de turno

**Paso 1:** Verificar que todos los pods estén corriendo en Kubernetes.

```bash
kubectl get pods -n ocr-prod -o wide
```

Resultado esperado: Todos los pods deben estar en estado `Running` con `READY` completo (e.g., `1/1` o `2/2`).

**Paso 2:** Verificar el endpoint de salud de la API.

```bash
curl -s -o /dev/null -w "%{http_code}" https://ocr.idcingenieria.com/health
```

Resultado esperado: Código HTTP `200`.

**Paso 3:** Verificar la respuesta detallada del health check.

```bash
curl -s https://ocr.idcingenieria.com/health | python -m json.tool
```

Resultado esperado: Todos los componentes deben reportar `"status": "healthy"` (base de datos, cache, cola de mensajes, Vision API).

**Paso 4:** Verificar el estado de los nodos del cluster.

```bash
kubectl get nodes -o wide
```

Resultado esperado: Todos los nodos en estado `Ready`.

**Paso 5:** Verificar que no haya alertas activas en Prometheus.

```bash
curl -s https://ocr.idcingenieria.com/metrics | grep -i "alert"
```

**Paso 6:** Verificar el estado de Cloud SQL.

```bash
gcloud sql instances describe ocr-prod-db --format="value(state)"
```

Resultado esperado: `RUNNABLE`.

**Acción si falla:** Escalar al ingeniero de guardia. Ver sección de [Contactos de Escalación](#6-contactos-de-escalación).

---

### 2.2 Verificar Procesamiento OCR

**Frecuencia:** Cada mañana a las 08:15
**Duración estimada:** 10 minutos

**Paso 1:** Verificar el número de documentos procesados en las últimas 24 horas.

```bash
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
from datetime import datetime, timedelta
session = get_session()
count = session.execute(
    'SELECT COUNT(*) FROM documents WHERE created_at > NOW() - INTERVAL 24 HOUR'
).scalar()
print(f'Documentos procesados (últimas 24h): {count}')
"
```

**Paso 2:** Verificar documentos con errores de procesamiento.

```bash
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
errors = session.execute(
    \"SELECT id, error_message, created_at FROM documents WHERE status = 'error' AND created_at > NOW() - INTERVAL '24 HOUR' ORDER BY created_at DESC LIMIT 10\"
).fetchall()
for e in errors:
    print(f'ID: {e[0]} | Error: {e[1]} | Fecha: {e[2]}')
print(f'Total errores: {len(errors)}')
"
```

**Paso 3:** Verificar la tasa de éxito del OCR.

```bash
curl -s https://ocr.idcingenieria.com/metrics | grep "ocr_processing_success_rate"
```

Resultado esperado: Tasa de éxito superior al 95%.

**Paso 4:** Verificar que el motor de fallback (Tesseract) no esté siendo usado excesivamente.

```bash
curl -s https://ocr.idcingenieria.com/metrics | grep "ocr_fallback_usage_total"
```

Resultado esperado: El uso de Tesseract como fallback no debe superar el 10% del total de procesamiento.

**Paso 5:** Verificar tiempos de procesamiento promedio.

```bash
curl -s https://ocr.idcingenieria.com/metrics | grep "ocr_processing_duration_seconds"
```

Resultado esperado: Tiempo promedio menor a 5 segundos por documento.

**Acción si falla:** Si la tasa de error es superior al 5%, revisar los logs del servicio OCR y escalar si es necesario.

---

### 2.3 Verificar Sincronización ERP (SAP)

**Frecuencia:** Cada mañana a las 08:30
**Duración estimada:** 10 minutos

**Paso 1:** Verificar el estado del conector SAP.

```bash
kubectl get pods -n ocr-prod -l app=sap-connector
```

Resultado esperado: Pod en estado `Running`.

**Paso 2:** Verificar los logs de sincronización recientes.

```bash
kubectl logs -n ocr-prod deploy/sap-connector --tail=50 --since=24h | grep -E "(ERROR|SUCCESS|SYNC)"
```

**Paso 3:** Verificar documentos pendientes de sincronización.

```bash
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
pending = session.execute(
    \"SELECT COUNT(*) FROM documents WHERE erp_sync_status = 'pending'\"
).scalar()
failed = session.execute(
    \"SELECT COUNT(*) FROM documents WHERE erp_sync_status = 'failed'\"
).scalar()
print(f'Pendientes de sincronización: {pending}')
print(f'Fallidos en sincronización: {failed}')
"
```

Resultado esperado: Pendientes menor a 50, fallidos menor a 5.

**Paso 4:** Verificar la latencia de sincronización.

```bash
curl -s https://ocr.idcingenieria.com/metrics | grep "erp_sync_latency_seconds"
```

Resultado esperado: Latencia promedio menor a 30 segundos.

**Paso 5:** Reintentar sincronizaciones fallidas si es necesario.

```bash
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.tasks.erp_sync import retry_failed_syncs
result = retry_failed_syncs()
print(f'Reintentos iniciados: {result}')
"
```

**Acción si falla:** Si hay más de 10 sincronizaciones fallidas, verificar conectividad con SAP y escalar al equipo de integración.

---

### 2.4 Revisar Cola de Revisión Manual

**Frecuencia:** Cada mañana a las 08:45
**Duración estimada:** 5 minutos

**Paso 1:** Verificar documentos en cola de revisión manual.

```bash
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
result = session.execute(
    \"SELECT COUNT(*), AVG(EXTRACT(EPOCH FROM (NOW() - created_at))/3600) as avg_hours FROM documents WHERE status = 'manual_review'\"
).fetchone()
print(f'Documentos en revisión manual: {result[0]}')
print(f'Tiempo promedio en cola: {result[1]:.1f} horas')
"
```

Resultado esperado: Cola menor a 100 documentos, tiempo promedio menor a 4 horas.

**Paso 2:** Verificar el estado de Kafka para la cola de revisión.

```bash
kubectl exec -n ocr-prod deploy/kafka -- kafka-consumer-groups.sh \
  --bootstrap-server localhost:9092 \
  --describe --group ocr-manual-review-group
```

Resultado esperado: LAG (retraso) menor a 100 mensajes.

**Paso 3:** Verificar que los revisores están activos.

```bash
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
reviewers = session.execute(
    \"SELECT username, COUNT(*) as reviewed FROM review_log WHERE reviewed_at > NOW() - INTERVAL '24 HOUR' GROUP BY username ORDER BY reviewed DESC\"
).fetchall()
for r in reviewers:
    print(f'Revisor: {r[0]} | Documentos revisados: {r[1]}')
"
```

**Acción si falla:** Si la cola supera los 200 documentos, notificar al supervisor para asignar revisores adicionales.

---

### 2.5 Verificar Métricas en Grafana

**Frecuencia:** Cada mañana a las 09:00
**Duración estimada:** 10 minutos

**Paso 1:** Acceder al dashboard de Grafana.

```
URL: https://grafana.idcingenieria.com/d/ocr-overview
Credenciales: Ver Secret grafana-admin-credentials
```

**Paso 2:** Verificar los siguientes paneles del dashboard principal.

| Panel | Métrica | Umbral Aceptable |
|---|---|---|
| CPU Usage | Uso de CPU por pod | < 70% |
| Memory Usage | Uso de memoria por pod | < 80% |
| Request Rate | Solicitudes por segundo | Consistente con patrón histórico |
| Error Rate | Tasa de errores HTTP | < 1% |
| OCR Processing Time | Tiempo de procesamiento p95 | < 10 segundos |
| Queue Depth | Profundidad de cola Kafka | < 500 mensajes |
| DB Connections | Conexiones activas a PostgreSQL | < 80% del pool |
| Cache Hit Rate | Tasa de aciertos Redis | > 85% |

**Paso 3:** Verificar alertas configuradas.

```bash
curl -s -H "Authorization: Bearer $(kubectl get secret -n ocr-prod grafana-admin-credentials -o jsonpath='{.data.api-key}' | base64 -d)" \
  https://grafana.idcingenieria.com/api/alerts | python -m json.tool
```

**Paso 4:** Exportar snapshot del dashboard si se detectan anomalías.

```bash
curl -s -H "Authorization: Bearer $GRAFANA_API_KEY" \
  "https://grafana.idcingenieria.com/api/snapshots" \
  -X POST -H "Content-Type: application/json" \
  -d '{"dashboard": {"id": "ocr-overview"}, "expires": 86400}'
```

**Acción si falla:** Documentar cualquier anomalía y reportar en el canal de operaciones.

---

## 3. Procedimientos Semanales

### 3.1 Revisión de Performance

**Frecuencia:** Cada lunes a las 10:00
**Duración estimada:** 30 minutos

**Paso 1:** Obtener métricas de rendimiento de la última semana.

```bash
curl -s "https://prometheus.idcingenieria.com/api/v1/query_range" \
  --data-urlencode "query=rate(http_requests_total{namespace='ocr-prod'}[1h])" \
  --data-urlencode "start=$(date -d '7 days ago' +%s)" \
  --data-urlencode "end=$(date +%s)" \
  --data-urlencode "step=3600" | python -m json.tool
```

**Paso 2:** Verificar latencias p50, p90 y p99.

```bash
curl -s "https://prometheus.idcingenieria.com/api/v1/query" \
  --data-urlencode "query=histogram_quantile(0.99, rate(http_request_duration_seconds_bucket{namespace='ocr-prod'}[7d]))" \
  | python -m json.tool
```

Resultado esperado: p99 menor a 15 segundos.

**Paso 3:** Comparar con la semana anterior.

```bash
curl -s "https://prometheus.idcingenieria.com/api/v1/query" \
  --data-urlencode "query=avg_over_time(ocr_processing_duration_seconds{namespace='ocr-prod'}[7d])" \
  | python -m json.tool
```

**Paso 4:** Verificar el uso de recursos del cluster.

```bash
kubectl top nodes
kubectl top pods -n ocr-prod --sort-by=cpu
kubectl top pods -n ocr-prod --sort-by=memory
```

**Paso 5:** Revisar si hay pods que hayan sido reiniciados.

```bash
kubectl get pods -n ocr-prod -o custom-columns=\
NAME:.metadata.name,\
RESTARTS:.status.containerStatuses[0].restartCount,\
LAST_STATE:.status.containerStatuses[0].lastState.terminated.reason \
--sort-by=.status.containerStatuses[0].restartCount
```

**Paso 6:** Generar reporte semanal de rendimiento y enviarlo al equipo.

---

### 3.2 Verificación de Backups

**Frecuencia:** Cada martes a las 10:00
**Duración estimada:** 20 minutos

**Paso 1:** Verificar los backups automáticos de Cloud SQL.

```bash
gcloud sql backups list --instance=ocr-prod-db --limit=7
```

Resultado esperado: Un backup exitoso por cada día de la semana.

**Paso 2:** Verificar la integridad del último backup.

```bash
gcloud sql backups describe $(gcloud sql backups list --instance=ocr-prod-db --limit=1 --format="value(id)") \
  --instance=ocr-prod-db \
  --format="table(status, startTime, endTime, type)"
```

Resultado esperado: Estado `SUCCESSFUL`.

**Paso 3:** Verificar backups de Redis.

```bash
kubectl exec -n ocr-prod deploy/redis -- redis-cli LASTSAVE
kubectl exec -n ocr-prod deploy/redis -- redis-cli DBSIZE
```

**Paso 4:** Verificar backups de configuraciones de Kafka.

```bash
kubectl get configmap -n ocr-prod -l app=kafka -o yaml > /dev/null && echo "ConfigMaps de Kafka accesibles"
```

**Paso 5:** Realizar test de restauración en entorno de staging (mensualmente o cuando sea necesario).

```bash
gcloud sql backups restore BACKUP_ID \
  --restore-instance=ocr-staging-db \
  --backup-instance=ocr-prod-db \
  --async
```

**Acción si falla:** Si no hay backups de los últimos 2 días, escalar inmediatamente al DBA y al responsable de infraestructura.

---

### 3.3 Revisión de Logs

**Frecuencia:** Cada miércoles a las 10:00
**Duración estimada:** 30 minutos

**Paso 1:** Buscar errores críticos en los logs de la aplicación.

```bash
kubectl logs -n ocr-prod deploy/ocr-api --since=168h | grep -c "CRITICAL\|FATAL"
```

**Paso 2:** Analizar los errores más frecuentes.

```bash
kubectl logs -n ocr-prod deploy/ocr-api --since=168h | grep "ERROR" | \
  awk -F': ' '{print $NF}' | sort | uniq -c | sort -rn | head -20
```

**Paso 3:** Revisar logs de Cloud Logging para patrones sospechosos.

```bash
gcloud logging read "resource.type=k8s_container AND resource.labels.namespace_name=ocr-prod AND severity>=ERROR AND timestamp>=\"$(date -d '7 days ago' -u +%Y-%m-%dT%H:%M:%SZ)\"" \
  --limit=50 \
  --format="table(timestamp, jsonPayload.message)"
```

**Paso 4:** Revisar eventos de Kubernetes.

```bash
kubectl get events -n ocr-prod --sort-by='.lastTimestamp' | tail -30
```

**Paso 5:** Verificar logs del conector SAP.

```bash
kubectl logs -n ocr-prod deploy/sap-connector --since=168h | grep -E "ERROR|WARN" | tail -20
```

**Paso 6:** Verificar rotación de logs y espacio en disco.

```bash
kubectl exec -n ocr-prod deploy/ocr-api -- df -h /var/log
```

**Acción si falla:** Documentar patrones de errores recurrentes y crear tickets de mejora.

---

### 3.4 Seguimiento de Costos

**Frecuencia:** Cada viernes a las 10:00
**Duración estimada:** 15 minutos

**Paso 1:** Verificar el gasto acumulado del proyecto en GCP.

```bash
gcloud billing budgets describe BUDGET_ID --billing-account=BILLING_ACCOUNT_ID \
  --format="table(displayName, amount.specifiedAmount.currencyCode, amount.specifiedAmount.units, budgetFilter)"
```

**Paso 2:** Verificar el consumo de la API de Cloud Vision.

```bash
gcloud services list --enabled --filter="name:vision.googleapis.com" --format=json
gcloud alpha billing accounts describe BILLING_ACCOUNT_ID --format=json
```

**Paso 3:** Obtener el desglose de costos por servicio.

```bash
gcloud billing accounts get-iam-policy BILLING_ACCOUNT_ID
# Alternativamente, revisar en la consola de GCP:
# https://console.cloud.google.com/billing/BILLING_ACCOUNT_ID/reports
```

**Paso 4:** Verificar el uso de recursos de Kubernetes.

```bash
kubectl resource-quota -n ocr-prod
kubectl get limitranges -n ocr-prod -o yaml
```

**Paso 5:** Comparar costos con el presupuesto mensual aprobado.

| Servicio | Presupuesto Mensual | Gasto Actual | % Utilizado |
|---|---|---|---|
| GKE Cluster | - | - | - |
| Cloud SQL | - | - | - |
| Cloud Vision API | - | - | - |
| Cloud Storage | - | - | - |
| Networking | - | - | - |

**Acción si falla:** Si el gasto supera el 80% del presupuesto mensual antes del día 20, notificar al gerente de proyecto.

---

## 4. Procedimientos Mensuales

### 4.1 Parcheo de Seguridad

**Frecuencia:** Primer lunes de cada mes
**Duración estimada:** 2-4 horas
**Ventana de mantenimiento:** 22:00 - 02:00 (horario local)

**Paso 1:** Verificar actualizaciones disponibles para las imágenes base.

```bash
# Verificar vulnerabilidades en imágenes Docker
gcloud artifacts docker images scan gcr.io/ocr-operativo/api:latest \
  --format="table(vulnerability.effectiveSeverity, vulnerability.shortDescription, vulnerability.fixAvailable)"
```

**Paso 2:** Verificar actualizaciones de dependencias Python.

```bash
kubectl exec -n ocr-prod deploy/ocr-api -- pip list --outdated --format=columns
```

**Paso 3:** Verificar actualizaciones del cluster GKE.

```bash
gcloud container get-server-config --zone=us-central1-a --format="table(validMasterVersions)"
gcloud container clusters describe ocr-prod-cluster --zone=us-central1-a --format="value(currentMasterVersion)"
```

**Paso 4:** Planificar y ejecutar actualizaciones en entorno de staging primero.

```bash
# Actualizar imagen en staging
kubectl set image -n ocr-staging deploy/ocr-api ocr-api=gcr.io/ocr-operativo/api:NEW_VERSION
kubectl rollout status -n ocr-staging deploy/ocr-api --timeout=300s
```

**Paso 5:** Ejecutar suite de pruebas completa en staging.

```bash
kubectl exec -n ocr-staging deploy/ocr-api -- pytest tests/ -v --tb=short
```

**Paso 6:** Si las pruebas pasan, aplicar en producción durante la ventana de mantenimiento.

```bash
kubectl set image -n ocr-prod deploy/ocr-api ocr-api=gcr.io/ocr-operativo/api:NEW_VERSION
kubectl rollout status -n ocr-prod deploy/ocr-api --timeout=300s
```

**Paso 7:** Actualizar Cloud SQL si hay parches disponibles.

```bash
gcloud sql instances patch ocr-prod-db --maintenance-window-day=SUN --maintenance-window-hour=2
```

**Paso 8:** Documentar todos los parches aplicados en el registro de cambios.

---

### 4.2 Revisión de Capacidad

**Frecuencia:** Segundo lunes de cada mes
**Duración estimada:** 1 hora

**Paso 1:** Revisar el uso histórico de CPU y memoria.

```bash
# Uso promedio de CPU en el último mes
curl -s "https://prometheus.idcingenieria.com/api/v1/query" \
  --data-urlencode "query=avg_over_time(container_cpu_usage_seconds_total{namespace='ocr-prod'}[30d])" \
  | python -m json.tool
```

**Paso 2:** Revisar el crecimiento de la base de datos.

```bash
gcloud sql instances describe ocr-prod-db --format="table(settings.dataDiskSizeGb, settings.dataDiskType)"
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
size = session.execute(\"SELECT pg_size_pretty(pg_database_size('ocr_prod'))\").scalar()
tables = session.execute(\"SELECT relname, pg_size_pretty(pg_total_relation_size(relid)) FROM pg_catalog.pg_statio_user_tables ORDER BY pg_total_relation_size(relid) DESC LIMIT 10\").fetchall()
print(f'Tamaño total de la DB: {size}')
for t in tables:
    print(f'  {t[0]}: {t[1]}')
"
```

**Paso 3:** Revisar el uso de disco de Cloud Storage.

```bash
gsutil du -s gs://ocr-operativo-documents/
gsutil du -s gs://ocr-operativo-processed/
```

**Paso 4:** Evaluar la necesidad de escalamiento.

```bash
kubectl get hpa -n ocr-prod
kubectl describe hpa ocr-api-hpa -n ocr-prod
```

**Paso 5:** Proyectar necesidades de capacidad para los próximos 3 meses basándose en la tendencia.

**Paso 6:** Si se requiere escalamiento, generar solicitud de cambio con justificación de costos.

---

### 4.3 Revisión de Precisión del Modelo OCR

**Frecuencia:** Tercer lunes de cada mes
**Duración estimada:** 2 horas

**Paso 1:** Obtener métricas de precisión del último mes.

```bash
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
from datetime import datetime, timedelta
session = get_session()
result = session.execute(\"\"\"
    SELECT
        COUNT(*) as total,
        SUM(CASE WHEN confidence_score >= 0.95 THEN 1 ELSE 0 END) as high_confidence,
        SUM(CASE WHEN confidence_score >= 0.80 AND confidence_score < 0.95 THEN 1 ELSE 0 END) as medium_confidence,
        SUM(CASE WHEN confidence_score < 0.80 THEN 1 ELSE 0 END) as low_confidence,
        AVG(confidence_score) as avg_confidence,
        SUM(CASE WHEN status = 'manual_review' THEN 1 ELSE 0 END) as manual_review
    FROM documents
    WHERE created_at > NOW() - INTERVAL '30 days'
\"\"\").fetchone()
print(f'Total documentos procesados: {result[0]}')
print(f'Alta confianza (>=95%): {result[1]} ({result[1]/result[0]*100:.1f}%)')
print(f'Media confianza (80-95%): {result[2]} ({result[2]/result[0]*100:.1f}%)')
print(f'Baja confianza (<80%): {result[3]} ({result[3]/result[0]*100:.1f}%)')
print(f'Confianza promedio: {result[4]:.2%}')
print(f'Enviados a revisión manual: {result[5]} ({result[5]/result[0]*100:.1f}%)')
"
```

**Paso 2:** Analizar precisión por tipo de documento.

```bash
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
results = session.execute(\"\"\"
    SELECT document_type, COUNT(*), AVG(confidence_score),
           SUM(CASE WHEN status='manual_review' THEN 1 ELSE 0 END)
    FROM documents
    WHERE created_at > NOW() - INTERVAL '30 days'
    GROUP BY document_type
    ORDER BY AVG(confidence_score) ASC
\"\"\").fetchall()
for r in results:
    print(f'Tipo: {r[0]} | Total: {r[1]} | Confianza prom: {r[2]:.2%} | Revisión manual: {r[3]}')
"
```

**Paso 3:** Comparar con el mes anterior para detectar degradación.

**Paso 4:** Si la precisión ha bajado más de un 2%, investigar las causas y documentar hallazgos.

**Paso 5:** Evaluar si se necesitan ajustes en los umbrales de confianza o en el preprocesamiento de imágenes.

---

## 5. Comandos Útiles

### 5.1 Kubernetes (kubectl)

```bash
# Ver todos los recursos del namespace de producción
kubectl get all -n ocr-prod

# Ver logs en tiempo real de la API
kubectl logs -n ocr-prod deploy/ocr-api -f --tail=100

# Ver logs de un pod específico
kubectl logs -n ocr-prod POD_NAME -c CONTAINER_NAME

# Ejecutar un shell interactivo en un pod
kubectl exec -it -n ocr-prod deploy/ocr-api -- /bin/bash

# Escalar manualmente el deployment
kubectl scale deploy/ocr-api -n ocr-prod --replicas=5

# Ver el estado de un rollout
kubectl rollout status deploy/ocr-api -n ocr-prod

# Hacer rollback del último deployment
kubectl rollout undo deploy/ocr-api -n ocr-prod

# Ver el historial de rollouts
kubectl rollout history deploy/ocr-api -n ocr-prod

# Ver los recursos (CPU/memoria) de los pods
kubectl top pods -n ocr-prod

# Ver eventos recientes del namespace
kubectl get events -n ocr-prod --sort-by='.lastTimestamp' | tail -20

# Ver la configuración del HPA
kubectl get hpa -n ocr-prod -o yaml

# Forzar restart de un deployment (rolling restart)
kubectl rollout restart deploy/ocr-api -n ocr-prod

# Port-forward para acceso local
kubectl port-forward -n ocr-prod svc/ocr-api 8000:8000

# Copiar archivos desde/hacia un pod
kubectl cp -n ocr-prod POD_NAME:/path/to/file ./local-file
```

### 5.2 Google Cloud (gcloud)

```bash
# Autenticarse con GCP
gcloud auth login
gcloud config set project ocr-operativo

# Conectarse al cluster GKE
gcloud container clusters get-credentials ocr-prod-cluster --zone=us-central1-a

# Ver instancias de Cloud SQL
gcloud sql instances list

# Conectarse a Cloud SQL via proxy
cloud-sql-proxy ocr-operativo:us-central1:ocr-prod-db --port=5432

# Ver logs de Cloud Logging
gcloud logging read "resource.type=k8s_container AND resource.labels.namespace_name=ocr-prod" --limit=20

# Ver el estado de los servicios habilitados
gcloud services list --enabled

# Ver las cuotas de la API de Vision
gcloud alpha services quota list --service=vision.googleapis.com

# Listar buckets de almacenamiento
gsutil ls gs://ocr-operativo-*

# Ver políticas IAM del proyecto
gcloud projects get-iam-policy ocr-operativo
```

### 5.3 PostgreSQL (psql)

```bash
# Conectarse a la base de datos (vía Cloud SQL Proxy)
psql -h 127.0.0.1 -U ocr_admin -d ocr_prod -p 5432

# Consultas útiles dentro de psql
# Ver tamaño de la base de datos
SELECT pg_size_pretty(pg_database_size('ocr_prod'));

# Ver las tablas más grandes
SELECT relname, pg_size_pretty(pg_total_relation_size(relid))
FROM pg_catalog.pg_statio_user_tables
ORDER BY pg_total_relation_size(relid) DESC
LIMIT 10;

# Ver conexiones activas
SELECT count(*), state FROM pg_stat_activity GROUP BY state;

# Ver queries lentas (más de 5 segundos)
SELECT pid, now() - pg_stat_activity.query_start AS duration, query, state
FROM pg_stat_activity
WHERE (now() - pg_stat_activity.query_start) > interval '5 seconds'
AND state != 'idle'
ORDER BY duration DESC;

# Ver el estado de replicación
SELECT * FROM pg_stat_replication;

# Terminar una query lenta
SELECT pg_terminate_backend(PID);

# Ver índices no utilizados
SELECT indexrelid::regclass as index, relid::regclass as table,
       idx_scan as index_scans
FROM pg_stat_user_indexes
WHERE idx_scan = 0 AND indexrelid::regclass::text NOT LIKE '%_pkey';
```

### 5.4 Docker

```bash
# Construir la imagen Docker
docker build -t gcr.io/ocr-operativo/api:TAG .

# Ejecutar localmente para testing
docker run -p 8000:8000 --env-file .env gcr.io/ocr-operativo/api:TAG

# Subir imagen al registro
docker push gcr.io/ocr-operativo/api:TAG

# Ver logs de un contenedor
docker logs CONTAINER_ID --tail=100 -f

# Inspeccionar una imagen
docker inspect gcr.io/ocr-operativo/api:TAG

# Limpiar imágenes no utilizadas
docker image prune -a --filter "until=720h"

# Escanear vulnerabilidades
docker scan gcr.io/ocr-operativo/api:TAG
```

### 5.5 Redis

```bash
# Conectarse a Redis desde un pod
kubectl exec -it -n ocr-prod deploy/redis -- redis-cli

# Comandos útiles dentro de redis-cli
INFO memory
INFO stats
DBSIZE
KEYS ocr:cache:*
TTL key_name
MONITOR  # Ver comandos en tiempo real (usar con precaución en producción)
CLIENT LIST
FLUSHDB  # PELIGRO: Borrar toda la caché (solo si es necesario)
```

### 5.6 Kafka

```bash
# Listar topics
kubectl exec -n ocr-prod deploy/kafka -- kafka-topics.sh \
  --bootstrap-server localhost:9092 --list

# Describir un topic
kubectl exec -n ocr-prod deploy/kafka -- kafka-topics.sh \
  --bootstrap-server localhost:9092 --describe --topic ocr-documents

# Ver consumer groups
kubectl exec -n ocr-prod deploy/kafka -- kafka-consumer-groups.sh \
  --bootstrap-server localhost:9092 --list

# Ver el lag de un consumer group
kubectl exec -n ocr-prod deploy/kafka -- kafka-consumer-groups.sh \
  --bootstrap-server localhost:9092 --describe --group ocr-processing-group

# Consumir mensajes de un topic (para debugging)
kubectl exec -n ocr-prod deploy/kafka -- kafka-console-consumer.sh \
  --bootstrap-server localhost:9092 --topic ocr-documents --from-beginning --max-messages 5
```

---

## 6. Contactos de Escalación

### Niveles de Escalación

| Nivel | Tiempo Respuesta | Quién Contactar | Cuándo Escalar |
|---|---|---|---|
| **L1 - Operador** | Inmediato | Operador de turno | Monitoreo diario, alertas menores |
| **L2 - Ingeniero** | 15 minutos | Ingeniero de guardia | Pods caídos, errores recurrentes, degradación de servicio |
| **L3 - Arquitecto** | 30 minutos | Arquitecto de solución | Problemas de arquitectura, fallos de integración SAP |
| **L4 - Gerencia** | 1 hora | Gerente de proyecto | Caída total del servicio, incidentes de seguridad |

### Directorio de Contactos

| Rol | Nombre | Teléfono | Email | Disponibilidad |
|---|---|---|---|---|
| Operador de turno | [NOMBRE] | [TELÉFONO] | [EMAIL] | Lun-Vie 08:00-18:00 |
| Ingeniero de guardia | [NOMBRE] | [TELÉFONO] | [EMAIL] | 24/7 rotativo |
| DBA / Cloud SQL | [NOMBRE] | [TELÉFONO] | [EMAIL] | Lun-Vie 09:00-18:00 |
| Integración SAP | [NOMBRE] | [TELÉFONO] | [EMAIL] | Lun-Vie 09:00-17:00 |
| Arquitecto de solución | [NOMBRE] | [TELÉFONO] | [EMAIL] | Lun-Vie 09:00-18:00 |
| Gerente de proyecto | [NOMBRE] | [TELÉFONO] | [EMAIL] | Lun-Vie 08:00-19:00 |
| Soporte GCP | Google Cloud | [CASO DE SOPORTE] | support.google.com | 24/7 |

### Canales de Comunicación

| Canal | Uso |
|---|---|
| Slack `#ocr-operaciones` | Comunicación diaria del equipo de operaciones |
| Slack `#ocr-incidentes` | Reporte y seguimiento de incidentes |
| Email `ocr-ops@idcingenieria.com` | Comunicaciones formales y reportes |
| Teams (reunión de guerra) | Incidentes críticos con múltiples participantes |

### Procedimiento de Escalación

1. **Detectar** el problema y verificar que no es un falso positivo.
2. **Documentar** el problema con fecha, hora, síntomas y comandos de diagnóstico ejecutados.
3. **Intentar resolver** según los procedimientos de este runbook y la guía de troubleshooting.
4. **Si no se resuelve en 15 minutos**, escalar al nivel L2.
5. **Si no se resuelve en 30 minutos adicionales**, escalar al nivel L3.
6. **Si el servicio está completamente caído**, escalar directamente al nivel L4.
7. **Post-incidente:** Crear un reporte de incidente con análisis de causa raíz dentro de las 48 horas.

---

> **Nota:** Este runbook debe ser revisado y actualizado trimestralmente o cuando se realicen cambios significativos en la infraestructura o los procedimientos operativos.
