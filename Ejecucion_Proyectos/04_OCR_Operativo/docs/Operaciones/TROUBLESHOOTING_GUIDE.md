# Guía de Troubleshooting - OCR Operativo

**Proyecto:** OCR Operativo
**Versión:** 1.0
**Última actualización:** 2026-02-24
**Responsable:** Equipo de Operaciones IDC Ingeniería

---

## Tabla de Contenidos

1. [OCR Engine](#1-ocr-engine)
2. [Base de Datos (PostgreSQL / Cloud SQL)](#2-base-de-datos-postgresql--cloud-sql)
3. [Kubernetes / GKE](#3-kubernetes--gke)
4. [API (FastAPI)](#4-api-fastapi)
5. [Integración ERP / SAP](#5-integración-erp--sap)
6. [Google Cloud Vision API](#6-google-cloud-vision-api)
7. [Frontend (React)](#7-frontend-react)

---

## 1. OCR Engine

### Problema 1.1: Precisión de OCR baja (confianza < 80%)

**Síntoma:** Los documentos procesados devuelven un `confidence_score` inferior al 80%, y muchos documentos terminan en la cola de revisión manual.

**Causa probable:**
- Imágenes de baja calidad (baja resolución, mala iluminación, documentos arrugados).
- Documentos en formatos no soportados o con fuentes no estándar.
- Degradación del servicio de Cloud Vision API.

**Diagnóstico:**

```bash
# Verificar la distribución de confianza en los últimos documentos
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
results = session.execute(\"\"\"
    SELECT
        CASE
            WHEN confidence_score >= 0.95 THEN 'Alta (>=95%)'
            WHEN confidence_score >= 0.80 THEN 'Media (80-95%)'
            WHEN confidence_score >= 0.50 THEN 'Baja (50-80%)'
            ELSE 'Muy baja (<50%)'
        END as rango,
        COUNT(*), AVG(confidence_score)
    FROM documents
    WHERE created_at > NOW() - INTERVAL '24 HOUR'
    GROUP BY 1 ORDER BY 3 DESC
\"\"\").fetchall()
for r in results:
    print(f'{r[0]}: {r[1]} documentos (prom: {r[2]:.2%})')
"

# Verificar si el fallback a Tesseract se activa frecuentemente
curl -s https://ocr.idcingenieria.com/metrics | grep "ocr_fallback_usage"

# Verificar logs de procesamiento OCR
kubectl logs -n ocr-prod deploy/ocr-api --since=1h | grep -i "low_confidence\|fallback\|vision_api"
```

**Solución:**

1. Verificar la calidad de las imágenes de entrada revisando muestras de documentos con baja confianza.
2. Ajustar los parámetros de preprocesamiento de imagen (contraste, resolución, rotación) en la configuración.
   ```bash
   kubectl edit configmap -n ocr-prod ocr-processing-config
   # Ajustar: image_preprocessing.min_dpi: 300
   # Ajustar: image_preprocessing.auto_contrast: true
   # Ajustar: image_preprocessing.auto_deskew: true
   ```
3. Reiniciar el deployment para aplicar cambios.
   ```bash
   kubectl rollout restart deploy/ocr-api -n ocr-prod
   ```
4. Si el problema persiste, verificar el estado de la API de Cloud Vision.

**Prevención:**
- Implementar validación de calidad de imagen antes del procesamiento OCR.
- Establecer requisitos mínimos de resolución (300 DPI) para los documentos de entrada.
- Monitorear la métrica `ocr_confidence_score_avg` en Grafana con alerta si baja del 85%.

---

### Problema 1.2: Timeout en procesamiento OCR

**Síntoma:** Las solicitudes de procesamiento OCR expiran con errores `504 Gateway Timeout` o `TimeoutError` en los logs.

**Causa probable:**
- Documentos excesivamente grandes (muchas páginas o alta resolución).
- Cloud Vision API con latencia elevada.
- Recursos insuficientes en el pod de procesamiento.

**Diagnóstico:**

```bash
# Verificar tiempos de procesamiento
curl -s https://ocr.idcingenieria.com/metrics | grep "ocr_processing_duration"

# Verificar si hay documentos grandes en la cola
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
large = session.execute(\"\"\"
    SELECT id, file_size_mb, page_count, processing_time_seconds
    FROM documents
    WHERE processing_time_seconds > 30
    AND created_at > NOW() - INTERVAL '24 HOUR'
    ORDER BY processing_time_seconds DESC LIMIT 10
\"\"\").fetchall()
for d in large:
    print(f'ID: {d[0]} | Tamaño: {d[1]}MB | Páginas: {d[2]} | Tiempo: {d[3]}s')
"

# Verificar recursos del pod
kubectl top pods -n ocr-prod -l app=ocr-api
```

**Solución:**

1. Aumentar el timeout del procesamiento OCR si es necesario.
   ```bash
   kubectl set env deploy/ocr-api -n ocr-prod OCR_PROCESSING_TIMEOUT=120
   ```
2. Implementar procesamiento por lotes para documentos grandes.
3. Si los recursos son insuficientes, escalar verticalmente.
   ```bash
   kubectl patch deploy ocr-api -n ocr-prod -p '{"spec":{"template":{"spec":{"containers":[{"name":"ocr-api","resources":{"limits":{"cpu":"2","memory":"4Gi"},"requests":{"cpu":"1","memory":"2Gi"}}}]}}}}'
   ```
4. Verificar la latencia de Cloud Vision API y, si es elevada, activar Tesseract como motor principal temporalmente.

**Prevención:**
- Establecer límites de tamaño de archivo (50MB, 100 páginas).
- Implementar procesamiento asíncrono para documentos grandes.
- Configurar alertas de latencia en Grafana.

---

### Problema 1.3: Error de API de Cloud Vision (HTTP 500)

**Síntoma:** Los logs muestran errores `google.api_core.exceptions.InternalServerError` o respuestas HTTP 500 desde Cloud Vision API.

**Causa probable:**
- Problema temporal en el servicio de Google Cloud Vision.
- Imagen corrupta o en formato no soportado enviada a la API.

**Diagnóstico:**

```bash
# Verificar logs de errores de Vision API
kubectl logs -n ocr-prod deploy/ocr-api --since=1h | grep -i "vision\|InternalServerError\|500"

# Verificar el estado de Cloud Vision API
gcloud alpha services quota list --service=vision.googleapis.com --consumer="project:ocr-operativo"

# Verificar el estado de salud de GCP
curl -s https://status.cloud.google.com/incidents.json | python -m json.tool | head -50
```

**Solución:**

1. Verificar si el problema es generalizado consultando el dashboard de estado de GCP.
2. Si es un problema temporal, el sistema de fallback a Tesseract debería activarse automáticamente. Verificar que está funcionando.
   ```bash
   curl -s https://ocr.idcingenieria.com/metrics | grep "ocr_engine_active"
   ```
3. Si el fallback no se activa, forzarlo manualmente.
   ```bash
   kubectl set env deploy/ocr-api -n ocr-prod OCR_PRIMARY_ENGINE=tesseract
   ```
4. Una vez que Cloud Vision se recupere, restaurar la configuración original.
   ```bash
   kubectl set env deploy/ocr-api -n ocr-prod OCR_PRIMARY_ENGINE=cloud_vision
   ```

**Prevención:**
- Mantener Tesseract siempre configurado como fallback.
- Monitorear la tasa de errores de Cloud Vision con alertas en Grafana.
- Implementar circuit breaker para la API de Vision.

---

### Problema 1.4: Documentos atascados en estado "processing"

**Síntoma:** Documentos permanecen indefinidamente en estado `processing` sin completarse ni generar error.

**Causa probable:**
- Worker de procesamiento se ha caído sin reportar el error.
- Mensaje perdido en la cola de Kafka.
- Deadlock en el procesamiento.

**Diagnóstico:**

```bash
# Verificar documentos atascados
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
stuck = session.execute(\"\"\"
    SELECT id, created_at, updated_at
    FROM documents
    WHERE status = 'processing'
    AND updated_at < NOW() - INTERVAL '30 MINUTE'
    ORDER BY created_at ASC
\"\"\").fetchall()
print(f'Documentos atascados: {len(stuck)}')
for s in stuck:
    print(f'  ID: {s[0]} | Creado: {s[1]} | Última actualización: {s[2]}')
"

# Verificar el lag de Kafka
kubectl exec -n ocr-prod deploy/kafka -- kafka-consumer-groups.sh \
  --bootstrap-server localhost:9092 --describe --group ocr-processing-group
```

**Solución:**

1. Reprocesar los documentos atascados.
   ```bash
   kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
   from app.db import get_session
   session = get_session()
   session.execute(\"\"\"
       UPDATE documents SET status = 'pending', retry_count = retry_count + 1
       WHERE status = 'processing' AND updated_at < NOW() - INTERVAL '30 MINUTE'
   \"\"\")
   session.commit()
   print('Documentos reasignados a la cola de procesamiento')
   "
   ```
2. Reiniciar los workers de procesamiento.
   ```bash
   kubectl rollout restart deploy/ocr-worker -n ocr-prod
   ```

**Prevención:**
- Implementar un job programado que detecte y reencole documentos atascados automáticamente.
- Configurar timeouts a nivel de worker con reporting de estado.

---

## 2. Base de Datos (PostgreSQL / Cloud SQL)

### Problema 2.1: Error de conexión a la base de datos

**Síntoma:** Los logs muestran `psycopg2.OperationalError: could not connect to server` o `connection refused`.

**Causa probable:**
- Cloud SQL Proxy caído o sin conexión.
- Pool de conexiones agotado.
- Instancia de Cloud SQL en mantenimiento o caída.

**Diagnóstico:**

```bash
# Verificar el estado de Cloud SQL
gcloud sql instances describe ocr-prod-db --format="value(state)"

# Verificar el pod de Cloud SQL Proxy
kubectl get pods -n ocr-prod -l app=cloudsql-proxy

# Verificar logs del proxy
kubectl logs -n ocr-prod -l app=cloudsql-proxy --tail=30

# Verificar conexiones activas
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import engine
result = engine.execute('SELECT count(*) FROM pg_stat_activity').scalar()
max_conn = engine.execute('SHOW max_connections').scalar()
print(f'Conexiones activas: {result} / Máximo: {max_conn}')
"
```

**Solución:**

1. Si Cloud SQL Proxy está caído, reiniciarlo.
   ```bash
   kubectl rollout restart deploy/cloudsql-proxy -n ocr-prod
   ```
2. Si el pool está agotado, terminar conexiones inactivas.
   ```bash
   kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
   from app.db import get_session
   session = get_session()
   session.execute(\"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE state = 'idle' AND query_start < NOW() - INTERVAL '10 MINUTE'\")
   session.commit()
   print('Conexiones inactivas terminadas')
   "
   ```
3. Si la instancia está en mantenimiento, esperar a que se complete y monitorear.
   ```bash
   gcloud sql operations list --instance=ocr-prod-db --limit=5
   ```

**Prevención:**
- Configurar límites adecuados en el pool de conexiones (e.g., `pool_size=20, max_overflow=10`).
- Configurar alertas para cuando las conexiones superen el 80% del máximo.
- Usar connection pooling con PgBouncer.

---

### Problema 2.2: Queries lentas degradan el rendimiento

**Síntoma:** La aplicación responde lentamente, los tiempos de respuesta de la API aumentan. Grafana muestra latencias elevadas en las métricas de base de datos.

**Causa probable:**
- Queries sin índices adecuados.
- Tablas con datos excesivos sin particionamiento.
- Bloqueos (locks) en la base de datos.
- Estadísticas de tablas desactualizadas.

**Diagnóstico:**

```bash
# Verificar queries activas y lentas
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
slow = session.execute(\"\"\"
    SELECT pid, now() - pg_stat_activity.query_start AS duration,
           substring(query, 1, 100) as query_preview, state
    FROM pg_stat_activity
    WHERE (now() - pg_stat_activity.query_start) > interval '5 seconds'
    AND state != 'idle'
    ORDER BY duration DESC
\"\"\").fetchall()
for s in slow:
    print(f'PID: {s[0]} | Duración: {s[1]} | Query: {s[2]}... | Estado: {s[3]}')
"

# Verificar índices faltantes
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
results = session.execute(\"\"\"
    SELECT schemaname, relname, seq_scan, seq_tup_read, idx_scan
    FROM pg_stat_user_tables
    WHERE seq_scan > 1000 AND (idx_scan IS NULL OR idx_scan < seq_scan * 0.1)
    ORDER BY seq_tup_read DESC LIMIT 10
\"\"\").fetchall()
for r in results:
    print(f'Tabla: {r[1]} | Seq scans: {r[2]} | Rows leídas: {r[3]} | Idx scans: {r[4]}')
"

# Verificar bloqueos
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
locks = session.execute(\"\"\"
    SELECT blocked_locks.pid AS blocked_pid,
           blocking_locks.pid AS blocking_pid,
           blocked_activity.query AS blocked_query
    FROM pg_catalog.pg_locks blocked_locks
    JOIN pg_catalog.pg_stat_activity blocked_activity ON blocked_activity.pid = blocked_locks.pid
    JOIN pg_catalog.pg_locks blocking_locks ON blocking_locks.locktype = blocked_locks.locktype
    WHERE NOT blocked_locks.granted
    LIMIT 10
\"\"\").fetchall()
for l in locks:
    print(f'PID bloqueado: {l[0]} | PID bloqueante: {l[1]} | Query: {l[2][:80]}')
"
```

**Solución:**

1. Terminar queries excesivamente lentas si están causando problemas.
   ```bash
   # Reemplazar PID con el PID de la query a terminar
   kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
   from app.db import get_session
   session = get_session()
   session.execute('SELECT pg_terminate_backend(PID)')
   session.commit()
   "
   ```
2. Actualizar estadísticas de tablas.
   ```bash
   kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
   from app.db import get_session
   session = get_session()
   session.execute('ANALYZE documents')
   session.execute('ANALYZE review_log')
   session.commit()
   print('Estadísticas actualizadas')
   "
   ```
3. Crear índices faltantes si se identifican.
4. Considerar particionamiento de tablas grandes.

**Prevención:**
- Ejecutar `ANALYZE` semanalmente en tablas grandes.
- Revisar el plan de ejecución de queries frecuentes con `EXPLAIN ANALYZE`.
- Implementar particionamiento por fecha en la tabla `documents`.
- Configurar `log_min_duration_statement = 1000` para loguear queries lentas.

---

### Problema 2.3: Espacio en disco insuficiente en Cloud SQL

**Síntoma:** Errores `ERROR: could not extend file` o alertas de Cloud SQL indicando que el disco está lleno.

**Causa probable:**
- Crecimiento no previsto de datos.
- Archivos WAL acumulados.
- Tablas temporales o datos huérfanos.

**Diagnóstico:**

```bash
# Verificar uso de disco
gcloud sql instances describe ocr-prod-db --format="table(settings.dataDiskSizeGb, diskEncryptionStatus)"

# Verificar tamaño de las tablas
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
tables = session.execute(\"\"\"
    SELECT relname, pg_size_pretty(pg_total_relation_size(relid))
    FROM pg_catalog.pg_statio_user_tables
    ORDER BY pg_total_relation_size(relid) DESC LIMIT 10
\"\"\").fetchall()
for t in tables:
    print(f'{t[0]}: {t[1]}')
total = session.execute(\"SELECT pg_size_pretty(pg_database_size('ocr_prod'))\").scalar()
print(f'Total base de datos: {total}')
"
```

**Solución:**

1. Aumentar el tamaño del disco de Cloud SQL.
   ```bash
   gcloud sql instances patch ocr-prod-db --database-flags=storage-auto-resize=on
   # O manualmente:
   gcloud sql instances patch ocr-prod-db --storage-size=100GB
   ```
2. Limpiar datos antiguos si la política de retención lo permite.
   ```bash
   kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
   from app.db import get_session
   session = get_session()
   deleted = session.execute(\"\"\"
       DELETE FROM processing_logs WHERE created_at < NOW() - INTERVAL '90 days'
   \"\"\").rowcount
   session.commit()
   print(f'Registros de log eliminados: {deleted}')
   "
   ```
3. Ejecutar VACUUM para recuperar espacio.
   ```bash
   kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
   from app.db import engine
   conn = engine.raw_connection()
   conn.set_isolation_level(0)
   conn.cursor().execute('VACUUM ANALYZE documents')
   conn.close()
   print('VACUUM completado')
   "
   ```

**Prevención:**
- Habilitar `storage-auto-resize` en Cloud SQL.
- Implementar políticas de retención y archivado automático.
- Monitorear el crecimiento de disco con alertas al 80%.

---

### Problema 2.4: Replicación de base de datos con retraso

**Síntoma:** Las réplicas de lectura muestran datos desactualizados. Grafana muestra un aumento en `pg_replication_lag_seconds`.

**Causa probable:**
- Carga alta de escrituras en la instancia principal.
- Red con latencia entre zonas.
- Réplica con recursos insuficientes.

**Diagnóstico:**

```bash
# Verificar el retraso de replicación
gcloud sql instances describe ocr-prod-db-replica --format="value(replicaConfiguration.failoverTarget)"

# Verificar desde la base de datos
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
lag = session.execute('SELECT EXTRACT(EPOCH FROM replay_lag) FROM pg_stat_replication').scalar()
print(f'Retraso de replicación: {lag} segundos')
"
```

**Solución:**

1. Si el retraso es menor a 60 segundos, monitorear; puede ser transitorio.
2. Si es persistente, verificar la carga de la instancia principal y considerar escalar.
3. Reiniciar la réplica si el retraso es excesivo.
   ```bash
   gcloud sql instances restart ocr-prod-db-replica
   ```

**Prevención:**
- Configurar alertas para lag de replicación superior a 30 segundos.
- Asegurar que la réplica tiene recursos equivalentes al primario.

---

## 3. Kubernetes / GKE

### Problema 3.1: Pods en estado CrashLoopBackOff

**Síntoma:** `kubectl get pods` muestra pods en estado `CrashLoopBackOff` con múltiples reinicios.

**Causa probable:**
- Error en la aplicación al iniciar (configuración faltante, dependencia no disponible).
- Falla en health check (liveness probe).
- Imagen Docker corrupta o con errores.

**Diagnóstico:**

```bash
# Ver estado detallado del pod
kubectl describe pod -n ocr-prod POD_NAME

# Ver logs del pod que está crasheando
kubectl logs -n ocr-prod POD_NAME --previous

# Ver eventos del namespace
kubectl get events -n ocr-prod --sort-by='.lastTimestamp' | grep POD_NAME
```

**Solución:**

1. Revisar los logs del contenedor anterior para identificar el error.
   ```bash
   kubectl logs -n ocr-prod POD_NAME --previous -c ocr-api
   ```
2. Si es un problema de configuración, verificar variables de entorno y secrets.
   ```bash
   kubectl get secret -n ocr-prod ocr-credentials -o yaml
   kubectl describe configmap -n ocr-prod ocr-config
   ```
3. Si es un problema de imagen, hacer rollback.
   ```bash
   kubectl rollout undo deploy/ocr-api -n ocr-prod
   kubectl rollout status deploy/ocr-api -n ocr-prod
   ```
4. Si el liveness probe falla, ajustar los parámetros.
   ```bash
   kubectl patch deploy ocr-api -n ocr-prod -p '{"spec":{"template":{"spec":{"containers":[{"name":"ocr-api","livenessProbe":{"initialDelaySeconds":60,"periodSeconds":30,"failureThreshold":5}}]}}}}'
   ```

**Prevención:**
- Configurar `initialDelaySeconds` adecuado para el liveness probe.
- Implementar readiness probe separado del liveness probe.
- Probar las imágenes en staging antes de desplegar en producción.

---

### Problema 3.2: Pods terminados por OOM (Out of Memory)

**Síntoma:** Los pods se reinician con razón `OOMKilled`. `kubectl describe pod` muestra `Last State: Terminated - Reason: OOMKilled`.

**Causa probable:**
- Procesamiento de documentos muy grandes consume demasiada memoria.
- Memory leak en la aplicación.
- Límites de memoria demasiado bajos para la carga actual.

**Diagnóstico:**

```bash
# Verificar el uso de memoria actual
kubectl top pods -n ocr-prod -l app=ocr-api

# Ver los límites configurados
kubectl get deploy -n ocr-prod ocr-api -o jsonpath='{.spec.template.spec.containers[0].resources}'

# Verificar si hubo OOMKill recientemente
kubectl get events -n ocr-prod --field-selector reason=OOMKilling

# Ver el historial de restarts
kubectl get pods -n ocr-prod -l app=ocr-api -o custom-columns=\
NAME:.metadata.name,RESTARTS:.status.containerStatuses[0].restartCount,\
LAST_STATE:.status.containerStatuses[0].lastState.terminated.reason
```

**Solución:**

1. Aumentar los límites de memoria del deployment.
   ```bash
   kubectl patch deploy ocr-api -n ocr-prod -p '{"spec":{"template":{"spec":{"containers":[{"name":"ocr-api","resources":{"limits":{"memory":"4Gi"},"requests":{"memory":"2Gi"}}}]}}}}'
   ```
2. Si es un memory leak, identificar y corregir en el código. Reiniciar como medida temporal.
   ```bash
   kubectl rollout restart deploy/ocr-api -n ocr-prod
   ```
3. Implementar límites de tamaño de archivo para evitar procesamiento de documentos excesivamente grandes.

**Prevención:**
- Configurar Vertical Pod Autoscaler (VPA) para ajuste automático.
- Monitorear el uso de memoria con alertas al 80% del límite.
- Implementar streaming para procesamiento de documentos grandes.

---

### Problema 3.3: Problemas de autoescalamiento (HPA)

**Síntoma:** El Horizontal Pod Autoscaler no escala los pods cuando la carga aumenta, o escala excesivamente (flapping).

**Causa probable:**
- Metrics Server no está reportando métricas correctamente.
- Umbrales de escalamiento mal configurados.
- Recursos (requests/limits) no definidos correctamente.

**Diagnóstico:**

```bash
# Ver el estado del HPA
kubectl get hpa -n ocr-prod ocr-api-hpa -o yaml

# Ver las métricas que está usando el HPA
kubectl describe hpa ocr-api-hpa -n ocr-prod

# Verificar que Metrics Server está funcionando
kubectl top pods -n ocr-prod

# Verificar condiciones del HPA
kubectl get hpa -n ocr-prod ocr-api-hpa -o jsonpath='{.status.conditions[*].message}'
```

**Solución:**

1. Si Metrics Server no reporta, reiniciarlo.
   ```bash
   kubectl rollout restart deploy/metrics-server -n kube-system
   ```
2. Ajustar los umbrales del HPA.
   ```bash
   kubectl patch hpa ocr-api-hpa -n ocr-prod -p '{"spec":{"metrics":[{"type":"Resource","resource":{"name":"cpu","target":{"type":"Utilization","averageUtilization":70}}}],"minReplicas":2,"maxReplicas":10}}'
   ```
3. Si hay flapping, aumentar el `stabilizationWindowSeconds`.
   ```bash
   kubectl patch hpa ocr-api-hpa -n ocr-prod -p '{"spec":{"behavior":{"scaleDown":{"stabilizationWindowSeconds":300}}}}'
   ```

**Prevención:**
- Configurar `stabilizationWindowSeconds` para evitar escalamiento excesivo.
- Definir `requests` de CPU precisos basados en el perfil de carga real.
- Monitorear las decisiones del HPA en Grafana.

---

### Problema 3.4: Nodos del cluster no disponibles

**Síntoma:** `kubectl get nodes` muestra nodos en estado `NotReady`. Los pods no pueden ser programados.

**Causa probable:**
- Problema de red en GCP.
- Nodo con recursos agotados.
- Problemas con el kubelet del nodo.

**Diagnóstico:**

```bash
# Ver el estado de los nodos
kubectl get nodes -o wide

# Describir el nodo problemático
kubectl describe node NODE_NAME

# Ver condiciones del nodo
kubectl get node NODE_NAME -o jsonpath='{.status.conditions[*]}' | python -m json.tool
```

**Solución:**

1. Si es un problema temporal, esperar 5 minutos. GKE puede auto-reparar nodos.
2. Si persiste, drenar el nodo y recrearlo.
   ```bash
   kubectl drain NODE_NAME --ignore-daemonsets --delete-emptydir-data
   gcloud compute instances delete NODE_NAME --zone=us-central1-a
   # GKE auto-provisioning creará un nuevo nodo
   ```
3. Si el cluster no tiene suficientes nodos, escalar manualmente.
   ```bash
   gcloud container clusters resize ocr-prod-cluster --num-nodes=4 --zone=us-central1-a
   ```

**Prevención:**
- Habilitar auto-repair en el node pool de GKE.
- Configurar node auto-provisioning.
- Mantener nodos en múltiples zonas para alta disponibilidad.

---

## 4. API (FastAPI)

### Problema 4.1: Errores HTTP 502/503

**Síntoma:** Los clientes reciben respuestas 502 Bad Gateway o 503 Service Unavailable al llamar a la API.

**Causa probable:**
- Pods de la API no están listos (readiness probe fallando).
- Ingress controller con problemas.
- La aplicación tarda demasiado en arrancar.

**Diagnóstico:**

```bash
# Verificar el estado de los pods
kubectl get pods -n ocr-prod -l app=ocr-api

# Verificar readiness de los pods
kubectl describe endpoints ocr-api-service -n ocr-prod

# Verificar el ingress
kubectl describe ingress -n ocr-prod ocr-ingress

# Verificar logs del ingress controller
kubectl logs -n ingress-nginx deploy/ingress-nginx-controller --tail=30 | grep "502\|503"

# Test directo al servicio (bypasseando ingress)
kubectl port-forward -n ocr-prod svc/ocr-api 8000:8000 &
curl -s http://localhost:8000/health
```

**Solución:**

1. Si los pods no están listos, verificar qué falla en el readiness probe.
   ```bash
   kubectl logs -n ocr-prod POD_NAME | grep -i "startup\|ready\|health"
   ```
2. Si es un problema del ingress, reiniciar el controller.
   ```bash
   kubectl rollout restart deploy/ingress-nginx-controller -n ingress-nginx
   ```
3. Si la aplicación tarda en arrancar, aumentar el `initialDelaySeconds` del readiness probe.
4. Escalar si la carga excede la capacidad.
   ```bash
   kubectl scale deploy/ocr-api -n ocr-prod --replicas=5
   ```

**Prevención:**
- Configurar readiness probe con parámetros adecuados.
- Implementar circuit breaker en el ingress.
- Monitorear la tasa de errores 5xx en Grafana.

---

### Problema 4.2: Rate limiting activado

**Síntoma:** Los clientes reciben respuestas HTTP 429 Too Many Requests. Los logs muestran `Rate limit exceeded`.

**Causa probable:**
- Un cliente o integración está haciendo demasiadas solicitudes.
- Configuración de rate limiting demasiado restrictiva.
- Bot o scraper atacando el endpoint.

**Diagnóstico:**

```bash
# Ver las IPs con más solicitudes
kubectl logs -n ocr-prod deploy/ocr-api --since=1h | grep "429" | awk '{print $1}' | sort | uniq -c | sort -rn | head -10

# Verificar la configuración actual de rate limiting
kubectl get configmap -n ocr-prod ocr-config -o jsonpath='{.data.RATE_LIMIT_CONFIG}'

# Verificar métricas de rate limiting
curl -s https://ocr.idcingenieria.com/metrics | grep "rate_limit"
```

**Solución:**

1. Si es un cliente legítimo, aumentar su límite.
   ```bash
   kubectl edit configmap -n ocr-prod ocr-config
   # Ajustar RATE_LIMIT_PER_MINUTE: "100"
   ```
2. Si es tráfico malicioso, bloquear la IP.
   ```bash
   kubectl patch configmap -n ocr-prod ocr-config --type=merge -p '{"data":{"BLOCKED_IPS":"1.2.3.4,5.6.7.8"}}'
   kubectl rollout restart deploy/ocr-api -n ocr-prod
   ```
3. Implementar rate limiting diferenciado por API key.

**Prevención:**
- Configurar rate limiting por tier de cliente (básico, premium, enterprise).
- Implementar WAF (Web Application Firewall) en GCP.
- Monitorear patrones de tráfico anómalos.

---

### Problema 4.3: Errores de autenticación (401/403)

**Síntoma:** Los clientes reciben respuestas HTTP 401 Unauthorized o 403 Forbidden.

**Causa probable:**
- Token JWT expirado o inválido.
- API key revocada o incorrecta.
- Permisos insuficientes para el endpoint solicitado.

**Diagnóstico:**

```bash
# Verificar los logs de autenticación
kubectl logs -n ocr-prod deploy/ocr-api --since=1h | grep -i "auth\|401\|403\|unauthorized\|forbidden"

# Verificar la configuración de JWT
kubectl get secret -n ocr-prod jwt-secret -o jsonpath='{.data.JWT_ALGORITHM}' | base64 -d

# Verificar que el servicio de autenticación está funcionando
curl -s https://ocr.idcingenieria.com/api/v1/auth/verify -H "Authorization: Bearer TEST_TOKEN"
```

**Solución:**

1. Si los tokens están expirando prematuramente, verificar la configuración de TTL.
   ```bash
   kubectl get configmap -n ocr-prod ocr-config -o jsonpath='{.data.JWT_EXPIRATION_HOURS}'
   ```
2. Si una API key fue revocada por error, regenerarla.
3. Verificar que los roles y permisos están correctamente configurados.
4. Si el problema es generalizado, verificar que el secret de JWT no ha sido rotado accidentalmente.

**Prevención:**
- Implementar refresh tokens para evitar expiración abrupta.
- Monitorear la tasa de errores 401/403 con alertas.
- Documentar el proceso de rotación de keys.

---

### Problema 4.4: Respuestas lentas de la API (alta latencia)

**Síntoma:** Los tiempos de respuesta de la API superan los SLA definidos. Grafana muestra aumento en p95/p99.

**Causa probable:**
- Base de datos lenta (ver sección 2.2).
- Cache Redis no funcionando correctamente.
- Procesamiento OCR generando cuellos de botella.
- Recursos de CPU/memoria insuficientes.

**Diagnóstico:**

```bash
# Verificar latencia por endpoint
curl -s https://ocr.idcingenieria.com/metrics | grep "http_request_duration_seconds"

# Verificar cache hit rate
curl -s https://ocr.idcingenieria.com/metrics | grep "cache_hit_rate"

# Verificar conexión a Redis
kubectl exec -n ocr-prod deploy/redis -- redis-cli ping

# Verificar recursos
kubectl top pods -n ocr-prod -l app=ocr-api --sort-by=cpu
```

**Solución:**

1. Si Redis no está respondiendo, reiniciarlo.
   ```bash
   kubectl rollout restart deploy/redis -n ocr-prod
   ```
2. Si la cache hit rate es baja, verificar la configuración de TTL.
   ```bash
   kubectl exec -n ocr-prod deploy/redis -- redis-cli INFO stats | grep "keyspace_hits\|keyspace_misses"
   ```
3. Escalar horizontal si los recursos son insuficientes.
   ```bash
   kubectl scale deploy/ocr-api -n ocr-prod --replicas=5
   ```

**Prevención:**
- Configurar alertas de latencia p95 > 5 segundos.
- Implementar cache warming para datos frecuentes.
- Optimizar queries de base de datos.

---

## 5. Integración ERP / SAP

### Problema 5.1: Fallo de sincronización con SAP

**Síntoma:** Los documentos procesados no se sincronizan con SAP. El campo `erp_sync_status` permanece en `failed`.

**Causa probable:**
- Servicio SAP no disponible o en mantenimiento.
- Credenciales de SAP expiradas.
- Formato de datos incompatible con el esquema de SAP.

**Diagnóstico:**

```bash
# Verificar el estado del conector SAP
kubectl get pods -n ocr-prod -l app=sap-connector

# Verificar logs del conector
kubectl logs -n ocr-prod deploy/sap-connector --tail=50 | grep -i "error\|fail\|timeout"

# Verificar conectividad con SAP
kubectl exec -n ocr-prod deploy/sap-connector -- curl -s -o /dev/null -w "%{http_code}" https://sap-endpoint.idcingenieria.com/api/health

# Verificar documentos fallidos
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
failed = session.execute(\"\"\"
    SELECT id, error_message, retry_count, updated_at
    FROM documents
    WHERE erp_sync_status = 'failed'
    ORDER BY updated_at DESC LIMIT 10
\"\"\").fetchall()
for f in failed:
    print(f'ID: {f[0]} | Error: {f[1]} | Reintentos: {f[2]} | Última: {f[3]}')
"
```

**Solución:**

1. Verificar si SAP está en mantenimiento programado.
2. Si las credenciales expiraron, renovarlas.
   ```bash
   kubectl create secret generic sap-integration-credentials -n ocr-prod \
     --from-literal=SAP_API_KEY=NUEVA_KEY \
     --from-literal=SAP_API_SECRET=NUEVO_SECRET \
     --dry-run=client -o yaml | kubectl apply -f -
   kubectl rollout restart deploy/sap-connector -n ocr-prod
   ```
3. Reintentar las sincronizaciones fallidas.
   ```bash
   kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
   from app.tasks.erp_sync import retry_failed_syncs
   result = retry_failed_syncs(max_retries=3)
   print(f'Sincronizaciones reintentadas: {result}')
   "
   ```

**Prevención:**
- Implementar circuit breaker para la integración con SAP.
- Configurar reintentos exponenciales automáticos.
- Coordinar ventanas de mantenimiento con el equipo de SAP.

---

### Problema 5.2: Timeout en la comunicación con SAP

**Síntoma:** Los logs muestran `ConnectionTimeout` o `ReadTimeout` al comunicarse con SAP. Los tiempos de sincronización exceden los 30 segundos.

**Causa probable:**
- SAP bajo carga alta.
- Problemas de red entre GCP y el entorno SAP.
- Payload demasiado grande.

**Diagnóstico:**

```bash
# Verificar latencia de red hacia SAP
kubectl exec -n ocr-prod deploy/sap-connector -- curl -s -o /dev/null -w "time_total: %{time_total}s\n" https://sap-endpoint.idcingenieria.com/api/health

# Verificar métricas de sincronización
curl -s https://ocr.idcingenieria.com/metrics | grep "erp_sync_duration\|erp_sync_timeout"

# Verificar el tamaño de los payloads
kubectl logs -n ocr-prod deploy/sap-connector --since=1h | grep -i "payload_size\|timeout"
```

**Solución:**

1. Aumentar el timeout de conexión si es necesario.
   ```bash
   kubectl set env deploy/sap-connector -n ocr-prod SAP_TIMEOUT_SECONDS=60
   ```
2. Implementar envío por lotes para reducir el tamaño de cada solicitud.
3. Coordinar con el equipo de SAP para verificar la salud de su servicio.

**Prevención:**
- Configurar timeouts y reintentos adecuados.
- Implementar envío asíncrono con cola de mensajes.
- Monitorear la latencia de la integración con alertas.

---

### Problema 5.3: Discrepancia de datos entre OCR y SAP

**Síntoma:** Los datos en SAP no coinciden con los datos procesados por el OCR. Se reportan facturas con montos incorrectos o campos faltantes.

**Causa probable:**
- Mapeo de campos incorrecto entre OCR y el esquema de SAP.
- Truncamiento de datos durante la transformación.
- Problemas de codificación de caracteres.

**Diagnóstico:**

```bash
# Comparar un documento específico en ambos sistemas
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
doc = session.execute(\"SELECT id, ocr_data, erp_data FROM documents WHERE id = 'DOC_ID'\").fetchone()
import json
print('Datos OCR:', json.dumps(json.loads(doc[1]), indent=2, ensure_ascii=False))
print('Datos ERP:', json.dumps(json.loads(doc[2]), indent=2, ensure_ascii=False))
"

# Verificar logs de transformación
kubectl logs -n ocr-prod deploy/sap-connector --since=24h | grep -i "transform\|mapping\|mismatch"
```

**Solución:**

1. Identificar los campos con discrepancias y revisar el mapeo de transformación.
2. Corregir la configuración de mapeo.
   ```bash
   kubectl edit configmap -n ocr-prod sap-field-mapping
   ```
3. Reprocesar los documentos afectados después de corregir el mapeo.

**Prevención:**
- Implementar validación de datos antes del envío a SAP.
- Configurar alertas para discrepancias detectadas automáticamente.
- Mantener tests de integración que verifiquen el mapeo de campos.

---

## 6. Google Cloud Vision API

### Problema 6.1: Cuota de API excedida

**Síntoma:** Errores `google.api_core.exceptions.ResourceExhausted: 429 Quota exceeded` en los logs.

**Causa probable:**
- Se ha excedido la cuota de solicitudes por minuto o por día.
- Pico de carga no previsto.
- Procesamiento duplicado de documentos.

**Diagnóstico:**

```bash
# Verificar las cuotas actuales
gcloud alpha services quota list --service=vision.googleapis.com --consumer="project:ocr-operativo"

# Verificar el uso de la API
gcloud logging read "resource.type=api AND protoPayload.serviceName=vision.googleapis.com AND severity>=WARNING" --limit=20

# Verificar si hay procesamiento duplicado
kubectl exec -n ocr-prod deploy/ocr-api -- python -c "
from app.db import get_session
session = get_session()
dupes = session.execute(\"\"\"
    SELECT file_hash, COUNT(*) as cnt
    FROM documents
    WHERE created_at > NOW() - INTERVAL '1 HOUR'
    GROUP BY file_hash
    HAVING COUNT(*) > 1
\"\"\").fetchall()
for d in dupes:
    print(f'Hash: {d[0]} | Procesado: {d[1]} veces')
"
```

**Solución:**

1. Activar el fallback a Tesseract inmediatamente para aliviar la carga.
   ```bash
   kubectl set env deploy/ocr-api -n ocr-prod OCR_VISION_API_FALLBACK_THRESHOLD=0.5
   ```
2. Solicitar aumento de cuota a Google Cloud.
   ```bash
   # Via consola: https://console.cloud.google.com/apis/api/vision.googleapis.com/quotas
   # O via gcloud
   gcloud alpha services quota update --service=vision.googleapis.com \
     --consumer="project:ocr-operativo" \
     --metric=vision.googleapis.com/default --unit=1/min/{project} --value=1200
   ```
3. Implementar deduplicación para evitar procesamiento repetido.

**Prevención:**
- Implementar deduplicación basada en hash del archivo.
- Configurar rate limiting interno para las llamadas a Cloud Vision.
- Monitorear el uso de cuota con alertas al 80%.

---

### Problema 6.2: Error de autenticación con Cloud Vision

**Síntoma:** Errores `google.auth.exceptions.DefaultCredentialsError` o `403 Forbidden` al llamar a la API de Vision.

**Causa probable:**
- Service Account sin permisos adecuados.
- Workload Identity mal configurado.
- Key de Service Account expirada.

**Diagnóstico:**

```bash
# Verificar Workload Identity
kubectl describe serviceaccount -n ocr-prod ocr-api-sa

# Verificar la configuración de IAM
gcloud iam service-accounts get-iam-policy ocr-api@ocr-operativo.iam.gserviceaccount.com

# Verificar roles asignados
gcloud projects get-iam-policy ocr-operativo --flatten="bindings[].members" \
  --filter="bindings.members:ocr-api@ocr-operativo.iam.gserviceaccount.com" \
  --format="table(bindings.role)"
```

**Solución:**

1. Si Workload Identity está mal configurado, reparar la asociación.
   ```bash
   gcloud iam service-accounts add-iam-policy-binding \
     ocr-api@ocr-operativo.iam.gserviceaccount.com \
     --role="roles/iam.workloadIdentityUser" \
     --member="serviceAccount:ocr-operativo.svc.id.goog[ocr-prod/ocr-api-sa]"
   ```
2. Si falta el rol de Vision API, agregarlo.
   ```bash
   gcloud projects add-iam-policy-binding ocr-operativo \
     --member="serviceAccount:ocr-api@ocr-operativo.iam.gserviceaccount.com" \
     --role="roles/visionai.user"
   ```
3. Reiniciar los pods para que tomen las nuevas credenciales.
   ```bash
   kubectl rollout restart deploy/ocr-api -n ocr-prod
   ```

**Prevención:**
- Usar Workload Identity en lugar de keys de Service Account.
- Auditar los permisos de IAM mensualmente.
- Configurar alertas para errores de autenticación.

---

### Problema 6.3: Facturación deshabilitada para Cloud Vision

**Síntoma:** Error `403 Cloud Vision API has not been enabled for the project or billing is disabled`.

**Causa probable:**
- La cuenta de facturación fue desvinculada del proyecto.
- Se alcanzó el límite de crédito.
- La API fue deshabilitada accidentalmente.

**Diagnóstico:**

```bash
# Verificar si la API está habilitada
gcloud services list --enabled --filter="name:vision.googleapis.com"

# Verificar la cuenta de facturación
gcloud billing projects describe ocr-operativo
```

**Solución:**

1. Si la API fue deshabilitada, reactivarla.
   ```bash
   gcloud services enable vision.googleapis.com --project=ocr-operativo
   ```
2. Si es un problema de facturación, contactar al administrador financiero para resolver.
3. Mientras se resuelve, activar Tesseract como motor principal.
   ```bash
   kubectl set env deploy/ocr-api -n ocr-prod OCR_PRIMARY_ENGINE=tesseract
   ```

**Prevención:**
- Configurar alertas de presupuesto en GCP Billing.
- Tener una cuenta de facturación de respaldo configurada.
- Monitorear el estado de facturación mensualmente.

---

## 7. Frontend (React)

### Problema 7.1: Página en blanco o error de carga

**Síntoma:** El frontend muestra una página en blanco, errores en la consola del navegador, o un spinner de carga infinito.

**Causa probable:**
- Build de frontend corrupto o desactualizado.
- Archivos estáticos no servidos correctamente.
- Error de JavaScript en la aplicación.

**Diagnóstico:**

```bash
# Verificar que el pod del frontend está corriendo
kubectl get pods -n ocr-prod -l app=ocr-frontend

# Verificar que los archivos estáticos se sirven
kubectl exec -n ocr-prod deploy/ocr-frontend -- ls -la /usr/share/nginx/html/

# Verificar los logs del frontend (nginx)
kubectl logs -n ocr-prod deploy/ocr-frontend --tail=30

# Probar acceso directo
kubectl port-forward -n ocr-prod svc/ocr-frontend 3000:80 &
curl -s -o /dev/null -w "%{http_code}" http://localhost:3000/
```

**Solución:**

1. Si los archivos estáticos faltan, hacer redeploy del frontend.
   ```bash
   kubectl rollout restart deploy/ocr-frontend -n ocr-prod
   ```
2. Si el build está corrupto, reconstruir y redesplegar.
   ```bash
   # En el pipeline de CI/CD, triggear un nuevo build
   # O manualmente:
   kubectl set image deploy/ocr-frontend -n ocr-prod \
     ocr-frontend=gcr.io/ocr-operativo/frontend:LAST_KNOWN_GOOD_TAG
   ```
3. Verificar que la configuración del nginx es correcta.
   ```bash
   kubectl exec -n ocr-prod deploy/ocr-frontend -- nginx -t
   ```

**Prevención:**
- Implementar health checks en el frontend.
- Mantener la última versión estable taggeada para rollback rápido.
- Configurar CDN para servir archivos estáticos.

---

### Problema 7.2: Errores de autenticación en el frontend

**Síntoma:** Los usuarios no pueden iniciar sesión, ven errores de "Sesión expirada" constantemente, o son redirigidos al login repetidamente.

**Causa probable:**
- Backend de autenticación caído.
- Tokens JWT con TTL muy corto.
- Problemas de CORS.
- Cookies bloqueadas por el navegador.

**Diagnóstico:**

```bash
# Verificar el servicio de autenticación
curl -s https://ocr.idcingenieria.com/api/v1/auth/health

# Verificar headers de CORS
curl -s -I -X OPTIONS https://ocr.idcingenieria.com/api/v1/auth/login \
  -H "Origin: https://ocr.idcingenieria.com" \
  -H "Access-Control-Request-Method: POST"

# Verificar la configuración de CORS en la API
kubectl get configmap -n ocr-prod ocr-config -o jsonpath='{.data.CORS_ORIGINS}'
```

**Solución:**

1. Si es un problema de CORS, actualizar la configuración.
   ```bash
   kubectl patch configmap -n ocr-prod ocr-config --type=merge \
     -p '{"data":{"CORS_ORIGINS":"https://ocr.idcingenieria.com,https://ocr-staging.idcingenieria.com"}}'
   kubectl rollout restart deploy/ocr-api -n ocr-prod
   ```
2. Si los tokens expiran muy rápido, ajustar el TTL.
   ```bash
   kubectl set env deploy/ocr-api -n ocr-prod JWT_EXPIRATION_HOURS=24
   ```
3. Verificar que el servicio de autenticación está funcionando y reiniciarlo si es necesario.

**Prevención:**
- Implementar refresh tokens con renovación automática.
- Configurar CORS correctamente desde el inicio.
- Monitorear errores de autenticación en el frontend.

---

### Problema 7.3: Problemas de visualización de documentos

**Síntoma:** Los documentos procesados no se muestran correctamente en el frontend. Las imágenes no cargan, el texto extraído aparece desordenado, o los campos no se mapean visualmente.

**Causa probable:**
- URL de documentos apuntando a un bucket incorrecto.
- Permisos de Cloud Storage incorrectos.
- El visor de documentos no soporta el formato del archivo.

**Diagnóstico:**

```bash
# Verificar que los documentos están accesibles
gsutil ls gs://ocr-operativo-processed/ | tail -5

# Verificar permisos del bucket
gsutil iam get gs://ocr-operativo-processed/

# Verificar URLs firmadas
kubectl logs -n ocr-prod deploy/ocr-api --since=1h | grep -i "signed_url\|storage\|bucket"
```

**Solución:**

1. Verificar que los permisos de Cloud Storage permiten acceso desde el frontend.
   ```bash
   gsutil iam ch serviceAccount:ocr-api@ocr-operativo.iam.gserviceaccount.com:objectViewer \
     gs://ocr-operativo-processed
   ```
2. Si las URLs firmadas están expirando, aumentar el TTL.
   ```bash
   kubectl set env deploy/ocr-api -n ocr-prod SIGNED_URL_EXPIRATION_MINUTES=60
   ```
3. Si es un formato no soportado, agregar el soporte o convertir el documento.

**Prevención:**
- Implementar conversión automática de documentos a formatos compatibles con el visor.
- Configurar TTL adecuado para URLs firmadas.
- Probar la visualización con diferentes tipos de documentos en QA.

---

### Problema 7.4: Rendimiento lento del frontend

**Síntoma:** La aplicación React es lenta al cargar, la navegación entre páginas tarda, o la lista de documentos se congela con muchos registros.

**Causa probable:**
- Bundle de JavaScript demasiado grande.
- Renderizados innecesarios de componentes React.
- Paginación no implementada para listas grandes.
- Llamadas a API excesivas.

**Diagnóstico:**

```bash
# Verificar el tamaño del bundle
kubectl exec -n ocr-prod deploy/ocr-frontend -- ls -lh /usr/share/nginx/html/static/js/

# Verificar los headers de cache
curl -s -I https://ocr.idcingenieria.com/static/js/main.js | grep -i "cache\|etag\|expires"

# Verificar la configuración de compresión
kubectl exec -n ocr-prod deploy/ocr-frontend -- nginx -T | grep gzip
```

**Solución:**

1. Habilitar compresión gzip si no está activa.
   ```bash
   kubectl edit configmap -n ocr-prod nginx-config
   # Agregar: gzip on; gzip_types text/plain application/json application/javascript text/css;
   kubectl rollout restart deploy/ocr-frontend -n ocr-prod
   ```
2. Verificar que se usa code splitting en la configuración de React.
3. Implementar paginación en la API para listas grandes.
4. Configurar headers de cache apropiados para archivos estáticos.

**Prevención:**
- Implementar code splitting y lazy loading en React.
- Usar virtualización para listas largas (e.g., react-virtualized).
- Configurar CDN para archivos estáticos.
- Monitorear el tamaño del bundle en el pipeline de CI/CD.

---

> **Nota:** Esta guía debe actualizarse cada vez que se identifiquen nuevos problemas o se implementen cambios significativos en la arquitectura del sistema. Todos los comandos deben probarse en el entorno de staging antes de ejecutarse en producción.
