# 01 - ESTRUCTURA Y ARQUITECTURA TÉCNICA
## OCR Operativo - Reconocimiento Óptico de Caracteres

---

## 1. DIAGRAMA C4 - NIVEL 1

```
┌─────────────────────────────────────────────────────┐
│         SISTEMA OCR OPERATIVO                       │
│    (Optical Character Recognition System)          │
├─────────────────────────────────────────────────────┤
│                                                     │
│  Documentos   OCR Engine    Validación    Destino  │
│     ┌──┐        ┌──────┐     ┌──────┐    ┌──────┐ │
│     │  │───────>│      │────>│      │───>│      │ │
│     └──┘        └──────┘     └──────┘    └──────┘ │
│  Físicos/        ML Models   Reglas       BD/APIs  │
│  Digital                     Negocio              │
│                                                     │
└─────────────────────────────────────────────────────┘
```

---

## 2. ARQUITECTURA DE 6 CAPAS

### 2.1 CAPA DE PRESENTACIÓN
- Upload de documentos (Web/Mobile)
- Dashboard de procesamiento
- Vista de resultados validados
- Stack: React 18.2, React Native 0.72

### 2.2 CAPA DE APLICACIÓN
- Servicio de procesamiento OCR
- Motor de validación
- Orquestación de flujos
- Stack: Node.js Express 4.18, TypeScript 5.1

### 2.3 CAPA DE INTEGRACIÓN
- Conectores SAP/ERP
- APIs externas de verificación
- Message Queue para procesamiento async
- Stack: Kafka 3.4, REST APIs

### 2.4 CAPA DE DOMINIO
- Entidades: Documento, ExtractionResult, Validación
- Agregados: DocumentProcessing
- Lógica de negocio OCR

### 2.5 CAPA DE PERSISTENCIA
- Base de datos PostgreSQL 15
- Cache Redis 7.0
- Storage de documentos (S3)
- ORM: TypeORM 0.3

### 2.6 CAPA DE INFRAESTRUCTURA
- Procesamiento: Tesseract OCR 5.3, OpenCV 4.8
- ML Models: TensorFlow 2.13 (opcional)
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
04_OCR_Operativo/
├── src/
│   ├── presentation/
│   │   ├── controllers/DocumentController.ts
│   │   ├── routes/document.routes.ts
│   │   ├── dto/UploadDocumentDTO.ts
│   │   └── middleware/documentValidation.middleware.ts
│   ├── application/
│   │   ├── services/OCRService.ts
│   │   ├── services/ValidationService.ts
│   │   ├── usecases/ProcessDocumentUseCase.ts
│   │   └── validators/ExtractionValidator.ts
│   ├── domain/
│   │   ├── entities/Document.entity.ts
│   │   ├── entities/ExtractionResult.entity.ts
│   │   ├── aggregates/DocumentProcessingAggregate.ts
│   │   └── repositories/DocumentRepository.interface.ts
│   ├── infrastructure/
│   │   ├── ocr/TesseractOCRAdapter.ts
│   │   ├── ocr/TensorFlowAdapter.ts
│   │   ├── database/PostgresDocumentRepo.ts
│   │   ├── storage/S3StorageService.ts
│   │   ├── external/SAPConnector.ts
│   │   └── messaging/KafkaProducer.ts
│   ├── shared/
│   │   ├── constants/OCRConstants.ts
│   │   ├── utils/ImageUtils.ts
│   │   └── types/index.ts
│   └── main.ts
├── tests/
│   ├── unit/services/OCRService.test.ts
│   ├── integration/ocr-integration.test.ts
│   └── e2e/document-processing.e2e.ts
├── config/
│   ├── ocr.config.ts
│   ├── tesseract.config.ts
│   └── app.config.ts
├── models/
│   ├── tesseract-data/
│   └── tensorflow-models/ (si aplica)
└── scripts/
    ├── train-models/
    └── batch-processing/
```

---

## 4. COMPONENTES DETALLADOS (15+)

| # | Componente | Responsabilidad | Stack | Input | Output |
|---|-----------|-----------------|-------|-------|--------|
| 1 | API Gateway | Enrutamiento y auth | Express + Kong | HTTP requests | JSON responses |
| 2 | Gestor Documentos | Almacenamiento y metadata | Node.js + PostgreSQL | Upload files | Document IDs |
| 3 | Engine OCR | Extracción de texto | Tesseract 5.3 | Imágenes | Texto extraído |
| 4 | Preprocesador Imagen | Mejora de imagen | OpenCV 4.8 | Imágenes crudas | Imágenes optimizadas |
| 5 | ML Classifier | Clasificación documenta | TensorFlow 2.13 | Texto/Imagen | Tipo documento |
| 6 | Validador | Verificación reglas | Node.js | Datos extraídos | Validación OK/Error |
| 7 | Corretor Ortográfico | Corrección post-OCR | Hunspell + LanguageTool | Texto OCR | Texto corregido |
| 8 | Mapper Campos | Mapeo a campos BD | Node.js | Texto + reglas | Objetos mapeados |
| 9 | Integrador SAP | Envío a ERP | Node-odata | Datos validados | Confirmación SAP |
| 10 | Queue Procesador | Encola tasks async | Kafka 3.4 | Jobs | Ejecución async |
| 11 | Scheduler | Tareas programadas | node-schedule | Triggers | Ejecuciones |
| 12 | Cache Manager | Cache de modelos | Redis 7.0 | Queries | Datos en caché |
| 13 | Monitor Calidad | Métricas OCR | Prometheus | Extractiones | Métricas/Alertas |
| 14 | Auditor | Logging de cambios | Winston + ELK | Cambios | Logs auditados |
| 15 | Reportero | Generación reportes | Reportlab | Datos procesados | PDF/Excel |

---

## 5. STACK TECNOLÓGICO

```
BACKEND:
  - Runtime: Node.js 20 LTS
  - Lenguaje: TypeScript 5.1
  - Framework: Express 4.18
  - ORM: TypeORM 0.3

OCR:
  - Tesseract.js 5.3 (Open source)
  - Google Vision API (opcional)
  - AWS Textract (opcional)
  - TensorFlow 2.13 (para modelos custom)

IMAGE PROCESSING:
  - OpenCV.js 4.8
  - Sharp 0.32 (para optimización)
  - ImageMagick (conversión formatos)

DATABASE:
  - PostgreSQL 15.3
  - Redis 7.0
  - S3-compatible storage

MESSAGE QUEUE:
  - Kafka 3.4.1
  - Bull (job queue en Redis)

MONITOREO:
  - Prometheus 2.45
  - Grafana 10.0
  - ELK Stack 8.8
```

---

## 6. DATA FLOW: Procesar Documento

```
Subir PDF
    ↓
Validar formato/tamaño
    ↓
Almacenar en S3
    ↓
Preprocesar imagen (OpenCV)
    ├─ Normalizar contraste
    ├─ Rotar si necesario
    └─ Mejorar resolución
    ↓
Ejecutar OCR (Tesseract)
    ↓
Clasificar tipo (ML)
    ↓
Validar reglas negocio
    ├─ Campos obligatorios
    ├─ Formatos correctos
    └─ Rangos válidos
    ↓
Mapear a estructura BD
    ↓
Enviar a SAP (si válido)
    ↓
Log auditado + notificación
```

---

## 7. MODELOS DE DATOS

```sql
CREATE TABLE documents (
    id SERIAL PRIMARY KEY,
    original_filename VARCHAR(255),
    document_type VARCHAR(50),
    upload_date TIMESTAMP,
    s3_path VARCHAR(500),
    status VARCHAR(20),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE extraction_results (
    id SERIAL PRIMARY KEY,
    document_id INTEGER REFERENCES documents(id),
    extracted_text TEXT,
    confidence_score DECIMAL(5,2),
    processing_time_ms INTEGER,
    method VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE validations (
    id SERIAL PRIMARY KEY,
    extraction_id INTEGER REFERENCES extraction_results(id),
    is_valid BOOLEAN,
    validation_errors JSONB,
    validated_data JSONB,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

---

## 8. EXEMPLO DE CÓDIGO

```typescript
// domain/services/OCRService.ts
@Injectable()
export class OCRService {
  async processDocument(file: Express.Multer.File): Promise<ExtractionResult> {
    // Preprocesar imagen
    const processedImage = await this.preprocessImage(file);
    
    // Ejecutar OCR
    const extractedText = await this.tesseractOCR.extract(processedImage);
    
    // Clasificar documento
    const docType = await this.classifier.classify(extractedText);
    
    // Crear resultado
    return new ExtractionResult(
      extractedText,
      docType,
      extractedText.confidence,
      Date.now()
    );
  }

  private async preprocessImage(file: Express.Multer.File): Promise<Buffer> {
    return sharp(file.buffer)
      .grayscale()
      .normalize()
      .sharpen()
      .toBuffer();
  }
}

// application/usecases/ProcessDocumentUseCase.ts
@Injectable()
export class ProcessDocumentUseCase {
  async execute(dto: ProcessDocumentDTO): Promise<ValidationResult> {
    const document = await this.documentRepo.save(dto);
    const extraction = await this.ocrService.processDocument(dto.file);
    const validation = await this.validationService.validate(extraction);
    
    if (validation.isValid) {
      await this.sapConnector.sendData(validation.validatedData);
    }
    
    return validation;
  }
}
```

---

## 9. SECURITY & COMPLIANCE

- **Autenticación:** OAuth 2.0 / JWT
- **Encriptación:** TLS 1.3 en tránsito, AES-256 en reposo
- **Auditoría:** Todos los cambios registrados
- **Cumplimiento:** GDPR, ISO 27001
- **Data Retention:** 3 años
- **Backup:** Diario

---

## 10. PERFORMANCE TARGETS

| Métrica | Target |
|---------|--------|
| Procesamiento doc (< 5 páginas) | < 10 seg |
| Precisión OCR | > 95% |
| Disponibilidad | 99.5% |
| Response Time API | < 200 ms |

---

## 11. CONVENCIONES

- Clases: PascalCase
- Variables/métodos: camelCase
- Constantes: UPPER_SNAKE_CASE
- Máximo 300 líneas por archivo
- Una clase por archivo

