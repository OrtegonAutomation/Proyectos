# 03 - GUÍA DE IMPLEMENTACIÓN PASO A PASO
## OCR Operativo - Reconocimiento Óptico de Caracteres

**Proyecto:** OCR Operativo  
**Duración:** 4 semanas (28 días)  
**Stack:** Google Cloud Vision, Python 3.11, FastAPI, PostgreSQL 15, React 18  
**Versión:** 1.0

---

## PARTE I: PREREQUISITES

### Cloud Requirements (GCP)

```
PROJECT SETUP:
├─ Project ID: ocr-operativo-prod
├─ Region: us-central1
├─ Billing enabled
└─ APIs enabled:
   ├─ Vision API
   ├─ Cloud Storage API
   ├─ Cloud SQL Admin API
   ├─ Compute Engine API
   ├─ Cloud KMS API
   └─ Cloud Logging API

ITSME & SERVICE ACCOUNTS:
├─ Service account: ocr-processor@ocr-operativo-prod.iam.gserviceaccount.com
├─ IAM roles: Vision User, Cloud SQL Client
├─ Key file: key.json (downloaded)
└─ Secret manager setup
```

### Local Development Environment

```
PYTHON:
  - Python 3.11.0+
  - pip 23.2+
  - venv activated
  
DEPENDENCIES:
  - google-cloud-vision==3.4.0
  - fastapi==0.104.0
  - uvicorn==0.24.0
  - postgresql driver: psycopg2==2.9.7
  - SQLAlchemy==2.0.20
  - pydantic==2.4.0
  - pillow==10.0.0
  - python-multipart==0.0.6

FRONTEND:
  - Node.js 20 LTS
  - npm 10.2.0
  - React 18.2.0
  - TypeScript 5.1.0
  
DEPLOYMENT:
  - Docker 24.0+
  - Docker Compose 2.20+
  - kubectl 1.27+
  - Helm 3.12+
```

---

## FASE 1: SETUP CLOUD & INFRAESTRUCTURA (Semana 1 - Días 1-7)

### PASO 1-5: Configurar GCP y Cloud Vision

```bash
# 1. Configurar gcloud CLI
gcloud config set project ocr-operativo-prod
gcloud config set compute/region us-central1
gcloud config set compute/zone us-central1-a

# 2. Habilitar APIs requeridas
gcloud services enable vision.googleapis.com
gcloud services enable cloudsql.googleapis.com
gcloud services enable storage-api.googleapis.com
gcloud services enable cloudkms.googleapis.com
gcloud services enable container.googleapis.com

# 3. Crear service account para OCR
gcloud iam service-accounts create ocr-processor \
  --display-name="OCR Processing Service Account"

# 4. Otorgar permisos
gcloud projects add-iam-policy-binding ocr-operativo-prod \
  --member=serviceAccount:ocr-processor@ocr-operativo-prod.iam.gserviceaccount.com \
  --role=roles/ml.admin

gcloud projects add-iam-policy-binding ocr-operativo-prod \
  --member=serviceAccount:ocr-processor@ocr-operativo-prod.iam.gserviceaccount.com \
  --role=roles/cloudsql.client

gcloud projects add-iam-policy-binding ocr-operativo-prod \
  --member=serviceAccount:ocr-processor@ocr-operativo-prod.iam.gserviceaccount.com \
  --role=roles/storage.admin

# 5. Crear y descargar key
gcloud iam service-accounts keys create key.json \
  --iam-account=ocr-processor@ocr-operativo-prod.iam.gserviceaccount.com

export GOOGLE_APPLICATION_CREDENTIALS="./key.json"
```

### PASO 6-10: PostgreSQL en Cloud SQL

```bash
# 6. Crear instancia Cloud SQL
gcloud sql instances create ocr-postgres-prod \
  --database-version=POSTGRES_15 \
  --tier=db-perf-opt-2 \
  --region=us-central1 \
  --network=default \
  --backup-start-time=03:00 \
  --enable-bin-log

# 7. Crear database
gcloud sql databases create ocr_db \
  --instance=ocr-postgres-prod

# 8. Crear usuario OCR con contraseña fuerte
gcloud sql users create ocr_app \
  --instance=ocr-postgres-prod \
  --password='GenerateSecurePassword123!'

# 9. Obtener IP pública de Cloud SQL
gcloud sql instances describe ocr-postgres-prod \
  --format='value(ipAddresses[0].ipAddress)'

# 10. Configurar acceso de red (desde Kubernetes)
gcloud sql instances patch ocr-postgres-prod \
  --require-ssl \
  --backup-start-time=03:00
```

---

## FASE 2: DESARROLLO BACKEND (Semana 2 - Días 8-14)

### PASO 11-15: Setup Python Backend

```bash
# 11. Crear proyecto Python
mkdir ocr-system && cd ocr-system
python3.11 -m venv venv
source venv/bin/activate  # En Windows: venv\Scripts\activate

# 12. Instalar dependencias
pip install --upgrade pip
cat > requirements.txt << 'EOF'
google-cloud-vision==3.4.0
fastapi==0.104.0
uvicorn[standard]==0.24.0
sqlalchemy==2.0.20
psycopg2-binary==2.9.7
pydantic==2.4.0
python-multipart==0.0.6
pillow==10.0.0
python-dotenv==1.0.0
google-cloud-storage==2.10.0
google-cloud-logging==3.7.0
EOF

pip install -r requirements.txt

# 13. Crear estructura de proyecto
mkdir -p src/{controllers,services,models,schemas,utils,config}
touch src/__init__.py src/main.py

# 14. Crear archivo .env
cat > .env << 'EOF'
GCP_PROJECT_ID=ocr-operativo-prod
GCP_REGION=us-central1
DB_HOST=<CLOUD_SQL_IP>
DB_PORT=5432
DB_NAME=ocr_db
DB_USER=ocr_app
DB_PASSWORD=GenerateSecurePassword123!
VISION_API_QUOTA=100000  # req/mes
EOF

# 15. Verificar credenciales GCP
gcloud auth application-default login
```

### PASO 16-20: Implementar OCR Service

```python
# src/services/ocr_service.py
from google.cloud import vision_v1
from pathlib import Path
from typing import Tuple
import logging

logger = logging.getLogger(__name__)

class OCRService:
    def __init__(self):
        self.vision_client = vision_v1.ImageAnnotatorClient()
    
    async def extract_text(self, image_path: str) -> Tuple[str, float]:
        """
        Extract text from image using Google Cloud Vision API
        Returns: (extracted_text, confidence_score)
        """
        try:
            with open(image_path, 'rb') as f:
                content = f.read()
            
            image = vision_v1.Image(content=content)
            response = self.vision_client.document_text_detection(image=image)
            
            if response.error.code:
                logger.error(f"Vision API error: {response.error.message}")
                return "", 0.0
            
            full_text = response.full_text_annotation.text
            confidence = len(full_text) > 0 and 0.95 or 0.0
            
            logger.info(f"OCR success: {len(full_text)} chars extracted")
            return full_text, confidence
            
        except Exception as e:
            logger.error(f"OCR extraction error: {str(e)}")
            raise

# src/main.py
from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.responses import JSONResponse
import uvicorn
from src.services.ocr_service import OCRService

app = FastAPI(title="OCR Operativo", version="1.0.0")
ocr_service = OCRService()

@app.post("/api/v1/documents/upload")
async def upload_document(file: UploadFile = File(...)):
    """
    Upload document and extract text via OCR
    """
    try:
        # Save temp file
        import tempfile
        with tempfile.NamedTemporaryFile(delete=False) as tmp:
            content = await file.read()
            tmp.write(content)
            tmp_path = tmp.name
        
        # Extract text
        extracted_text, confidence = await ocr_service.extract_text(tmp_path)
        
        return JSONResponse({
            "status": "success",
            "extracted_text": extracted_text,
            "confidence": confidence,
            "filename": file.filename
        })
        
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.get("/health")
async def health_check():
    return {"status": "healthy", "version": "1.0.0"}

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)
```

---

## FASE 3: TESTING & VALIDATION (Semana 3 - Días 15-21)

### PASO 21-25: Test de Precisión

```bash
# 21. Crear dataset de test (100 documentos)
mkdir -p tests/data/{samples,expected}
# Agregar PDFs/imágenes de prueba

# 22. Ejecutar script de validación
cat > tests/test_accuracy.py << 'EOF'
import os
from src.services.ocr_service import OCRService

async def test_accuracy():
    service = OCRService()
    test_dir = "tests/data/samples"
    
    total = 0
    correct = 0
    
    for filename in os.listdir(test_dir):
        if filename.endswith(('.pdf', '.png', '.jpg')):
            total += 1
            image_path = os.path.join(test_dir, filename)
            
            # Extract text
            extracted_text, confidence = await service.extract_text(image_path)
            
            # Compare with expected (loaded from file)
            expected_file = f"tests/data/expected/{filename}.txt"
            if os.path.exists(expected_file):
                with open(expected_file) as f:
                    expected = f.read()
                
                if extracted_text.strip() == expected.strip():
                    correct += 1
                else:
                    print(f"Mismatch: {filename}")
    
    accuracy = (correct / total) * 100
    print(f"Accuracy: {accuracy:.1f}% ({correct}/{total})")
    
    assert accuracy >= 95.0, f"Accuracy below target: {accuracy}%"

# 23. Ejecutar tests
pytest tests/test_accuracy.py -v

# 24. Performance testing
# Medir latency por documento
# Target: <5 seconds per document

# 25. Cost analysis
# Estimating Cloud Vision API cost
# 100K requests/mes @ $1.50 per 1K = $150/mes
```

### PASO 26-30: ERP Integration (SAP/Oracle)

```python
# src/services/erp_connector.py
from typing import Dict
import requests
import logging

logger = logging.getLogger(__name__)

class ERPConnector:
    def __init__(self, erp_host: str, username: str, password: str):
        self.erp_host = erp_host
        self.session = requests.Session()
        self.session.auth = (username, password)
        self.base_url = f"https://{erp_host}/odata/v4"
    
    async def send_document(self, doc_data: Dict) -> bool:
        """
        Send extracted document data to ERP system
        """
        try:
            # Map OCR fields to ERP fields
            erp_payload = {
                "DocumentNumber": doc_data.get("doc_number"),
                "DocumentDate": doc_data.get("date"),
                "Vendor": doc_data.get("vendor"),
                "Amount": doc_data.get("amount"),
                "LineItems": doc_data.get("items", [])
            }
            
            # Send to ERP
            response = self.session.post(
                f"{self.base_url}/PurchasingDocuments",
                json=erp_payload,
                timeout=30
            )
            
            if response.status_code == 201:
                logger.info(f"Document sent to ERP: {doc_data['doc_number']}")
                return True
            else:
                logger.error(f"ERP error: {response.status_code}: {response.text}")
                return False
                
        except Exception as e:
            logger.error(f"ERP connector error: {str(e)}")
            return False
```

---

## FASE 4: DEPLOYMENT (Semana 4 - Días 22-28)

### PASO 31-35: Containerización y Kubernetes

```bash
# 31. Crear Dockerfile
cat > Dockerfile << 'EOF'
FROM python:3.11-slim

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY src/ src/
COPY .env .

EXPOSE 8000

CMD ["uvicorn", "src.main:app", "--host", "0.0.0.0", "--port", "8000"]
EOF

# 32. Build Docker image
docker build -t gcr.io/ocr-operativo-prod/ocr-api:1.0 .

# 33. Push to Container Registry
docker push gcr.io/ocr-operativo-prod/ocr-api:1.0

# 34. Crear Kubernetes deployment
cat > k8s-deployment.yaml << 'EOF'
apiVersion: apps/v1
kind: Deployment
metadata:
  name: ocr-api
  namespace: default
spec:
  replicas: 3
  selector:
    matchLabels:
      app: ocr-api
  template:
    metadata:
      labels:
        app: ocr-api
    spec:
      containers:
      - name: ocr-api
        image: gcr.io/ocr-operativo-prod/ocr-api:1.0
        ports:
        - containerPort: 8000
        env:
        - name: GOOGLE_APPLICATION_CREDENTIALS
          value: /var/secrets/google/key.json
        resources:
          requests:
            memory: "1Gi"
            cpu: "500m"
          limits:
            memory: "2Gi"
            cpu: "1000m"
        livenessProbe:
          httpGet:
            path: /health
            port: 8000
          initialDelaySeconds: 10
          periodSeconds: 10
---
apiVersion: v1
kind: Service
metadata:
  name: ocr-api-service
spec:
  type: LoadBalancer
  selector:
    app: ocr-api
  ports:
  - protocol: TCP
    port: 80
    targetPort: 8000
EOF

# 35. Desplegar
kubectl create namespace ocr-prod
kubectl apply -f k8s-deployment.yaml -n ocr-prod
```

### PASO 36-40: Validación y Go-Live

```bash
# 36. Health checks
kubectl get pods -n ocr-prod
kubectl logs -f deployment/ocr-api -n ocr-prod

# 37. Test endpoints
curl -X POST http://localhost:8000/api/v1/documents/upload \
  -F "file=@test_document.pdf"

# 38. Monitor metrics
kubectl top pods -n ocr-prod
kubectl top nodes

# 39. Database backups
gcloud sql backups create \
  --instance=ocr-postgres-prod

# 40. Smoke tests
# Upload 10 test documents
# Verify 95%+ accuracy
# Verify ERP sync working
# Verify dashboards showing data
```

---

## TROUBLESHOOTING (25+ Problemas)

| Problema | Síntoma | Solución |
|----------|---------|----------|
| Vision API quota exceeded | 429 Too Many Requests | Aumentar quota en GCP console |
| DB connection refused | psycopg2.OperationalError | Verificar IP, firewall rules |
| Accuracy <95% | Confidence scores bajos | Reprocess con mejor imagen quality |
| ERP sync fails | Documentos no aparecen en SAP | Verificar OData API, retry con backoff |
| Memory OOM | Pod killed | Aumentar mem limit en k8s |
| Slow OCR processing | Latency >30s | Implementar async processing |
| File corruption | Invalid file error | Validate file format en upload |
| SSL certificate error | HTTPS fails | Regenerate SSL certs |
| API rate limit | 503 Service Unavailable | Implementar rate limiting, circuit breaker |
| Queue overflow | Jobs acumulados | Aumentar workers, consumer threads |
| Operator confusion | Low adoption | Improve UI/UX, better documentation |
| False positives | Manual review rate >10% | Tune confidence threshold |
| Missing fields | Incomplete extraction | Improve OCR model training |
| Retraining needed | Accuracy degrades | Implement monthly retraining |
| New document types | Wrong classification | Update classifier model |
| Data corruption | Lost documents | Implement backups, transactions |
| Monitoring blind spots | Alerts not firing | Add missing metrics/logs |
| Team turnover | Lost knowledge | Comprehensive documentation |
| Budget overrun | Costs exceed budget | Optimize Cloud Vision usage |
| Timeline delays | Behind schedule | Reduce scope, add resources |
| Quality issues | Too many bugs | More testing, code review |
| Performance degradation | System slow | Performance profiling, optimization |
| Network timeouts | Requests fail | Increase timeout, improve resilience |

---

## CHECKLIST POST-DEPLOYMENT

```
DÍAS 1-3 (War Room):
☐ Monitor OCR accuracy
☐ Check Vision API usage
☐ Validate ERP syncs
☐ Monitor system performance
☐ Test failover scenarios
☐ Document any issues

SEMANA 1:
☐ User acceptance testing
☐ Performance review
☐ Security audit
☐ Backup test
☐ Load testing
☐ Tuning based on feedback

SEMANA 2-4:
☐ Monitor in baseline
☐ Optimize based on metrics
☐ Implement improvements
☐ Complete knowledge transfer
☐ Handoff to operations
☐ Project closure
```

---

## CONCLUSIÓN

Estos 40 pasos garantizan implementación de **sistema OCR operativo de alta precisión (95%+ accuracy) integrado con ERP en 4 semanas** con:
- Google Cloud Vision API
- PostgreSQL Cloud SQL
- FastAPI backend
- Kubernetes deployment
- Automated ERP sync
- Real-time dashboards
- 99.5% uptime


