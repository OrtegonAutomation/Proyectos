# Requerimientos Funcionales - OCR Operativo

| Campo | Detalle |
|-------|---------|
| **Proyecto** | OCR Operativo |
| **Versión** | 1.0 |
| **Fecha** | 2026-02-24 |
| **Estado** | En revisión |
| **Autor** | IDC Ingeniería |

---

## Tabla de Contenidos

1. [RF-01: Carga de Documentos](#rf-01-carga-de-documentos)
2. [RF-02: Procesamiento OCR](#rf-02-procesamiento-ocr)
3. [RF-03: Clasificación de Documentos](#rf-03-clasificación-de-documentos)
4. [RF-04: Validación de Datos Extraídos](#rf-04-validación-de-datos-extraídos)
5. [RF-05: Integración ERP/SAP](#rf-05-integración-erpsap)
6. [RF-06: Cola de Revisión Manual](#rf-06-cola-de-revisión-manual)
7. [RF-07: Dashboard y Reportes](#rf-07-dashboard-y-reportes)
8. [RF-08: Gestión de Configuración](#rf-08-gestión-de-configuración)
9. [RF-09: Auditoría y Trazabilidad](#rf-09-auditoría-y-trazabilidad)
10. [RF-10: Procesamiento Batch](#rf-10-procesamiento-batch)
11. [Matriz de Prioridades](#matriz-de-prioridades)

---

## RF-01: Carga de Documentos

| Campo | Detalle |
|-------|---------|
| **ID** | RF-01 |
| **Nombre** | Carga de Documentos |
| **Prioridad** | Alta |
| **Complejidad** | Media |
| **Casos de uso** | CU-01, CU-10 |
| **Sprint estimado** | Sprint 1 |

### Descripción

El sistema debe permitir la carga individual y múltiple de documentos operativos en los formatos soportados. La interfaz debe ofrecer una experiencia intuitiva con retroalimentación visual durante el proceso de carga.

### Sub-requerimientos

| ID | Descripción | Prioridad |
|----|-------------|-----------|
| RF-01.1 | El sistema debe soportar carga de archivos en formatos PDF, PNG, JPG y TIFF. | Alta |
| RF-01.2 | El sistema debe permitir carga múltiple de hasta 50 archivos simultáneos. | Alta |
| RF-01.3 | El sistema debe soportar carga mediante drag & drop en la interfaz web. | Media |
| RF-01.4 | El sistema debe validar el formato y tamaño de cada archivo antes de aceptarlo (máximo 50 MB por archivo). | Alta |
| RF-01.5 | El sistema debe mostrar barra de progreso individual por cada archivo en carga. | Media |
| RF-01.6 | El sistema debe detectar archivos duplicados basándose en hash SHA-256 y alertar al usuario. | Media |
| RF-01.7 | El sistema debe generar un identificador UUID v4 único para cada documento cargado. | Alta |
| RF-01.8 | El sistema debe almacenar los documentos con cifrado AES-256 en reposo. | Alta |
| RF-01.9 | El sistema debe soportar la carga mediante API REST para integraciones externas. | Media |
| RF-01.10 | El sistema debe validar la integridad del archivo mediante checksum tras la carga. | Alta |

### Endpoint API

```
POST /api/v1/documents/upload
Content-Type: multipart/form-data

Request:
  - files: File[] (requerido, máximo 50)
  - document_type: string (opcional, enum: invoice, receipt, purchase_order, log, delivery_note)
  - priority: string (opcional, enum: normal, high, urgent)
  - metadata: JSON (opcional)

Response (201 Created):
{
  "batch_id": "uuid",
  "documents": [
    {
      "id": "uuid",
      "filename": "factura_001.pdf",
      "status": "pending_processing",
      "size_bytes": 245780,
      "format": "pdf",
      "sha256": "a1b2c3...",
      "uploaded_at": "2026-02-24T10:30:00Z"
    }
  ],
  "total_uploaded": 1,
  "total_rejected": 0
}
```

### Reglas de negocio

- Formatos permitidos: `.pdf`, `.png`, `.jpg`, `.jpeg`, `.tiff`, `.tif`.
- Tamaño máximo por archivo: 50 MB.
- Tamaño máximo por lote: 500 MB.
- Los archivos con extensión incorrecta (extensión no coincide con contenido real) deben ser rechazados.
- Los documentos cargados se almacenan con estado inicial `pendiente_procesamiento`.

---

## RF-02: Procesamiento OCR

| Campo | Detalle |
|-------|---------|
| **ID** | RF-02 |
| **Nombre** | Procesamiento OCR |
| **Prioridad** | Alta |
| **Complejidad** | Alta |
| **Casos de uso** | CU-02, CU-10 |
| **Sprint estimado** | Sprint 1-2 |

### Descripción

El sistema debe procesar los documentos cargados mediante motores OCR para extraer texto y datos estructurados. Utiliza Google Cloud Vision como motor principal y Tesseract como motor de respaldo. Cada extracción debe incluir un puntaje de confianza por campo y global.

### Sub-requerimientos

| ID | Descripción | Prioridad |
|----|-------------|-----------|
| RF-02.1 | El sistema debe utilizar Google Cloud Vision API como motor OCR principal. | Alta |
| RF-02.2 | El sistema debe utilizar Tesseract como motor OCR de respaldo cuando el motor principal no esté disponible. | Alta |
| RF-02.3 | El sistema debe extraer texto completo del documento con coordenadas de posición. | Alta |
| RF-02.4 | El sistema debe calcular un puntaje de confianza (0.0 a 1.0) para cada campo extraído. | Alta |
| RF-02.5 | El sistema debe calcular un puntaje de confianza global del documento (promedio ponderado). | Alta |
| RF-02.6 | El sistema debe procesar documentos multi-página combinando resultados. | Alta |
| RF-02.7 | El sistema debe soportar extracción de texto en español e inglés. | Alta |
| RF-02.8 | El sistema debe aplicar pre-procesamiento de imagen (rotación, desenfoque, contraste) cuando sea necesario. | Media |
| RF-02.9 | El sistema debe almacenar tanto el texto crudo como los datos estructurados extraídos. | Alta |
| RF-02.10 | El sistema debe cambiar automáticamente al motor de respaldo cuando la confianza del motor principal sea < 60%. | Alta |
| RF-02.11 | El sistema debe procesar cada documento en un tiempo máximo de 5 segundos. | Alta |

### Campos extraídos por tipo de documento

**Factura:**

| Campo | Tipo | Obligatorio | Ejemplo |
|-------|------|-------------|---------|
| `numero_factura` | string | Si | "FAC-2026-001234" |
| `fecha_emision` | date | Si | "2026-02-20" |
| `ruc_emisor` | string | Si | "20512345678" |
| `razon_social_emisor` | string | Si | "Suministros Industriales S.A." |
| `subtotal` | decimal | Si | 13280.93 |
| `iva` | decimal | Si | 2399.57 |
| `total` | decimal | Si | 15680.50 |
| `moneda` | string | Si | "USD" |
| `items` | array | Si | [{descripcion, cantidad, precio_unitario, total}] |

**Recibo:**

| Campo | Tipo | Obligatorio | Ejemplo |
|-------|------|-------------|---------|
| `numero_recibo` | string | Si | "REC-2026-0567" |
| `fecha` | date | Si | "2026-02-20" |
| `monto` | decimal | Si | 450.00 |
| `concepto` | string | Si | "Pago servicio mantenimiento" |
| `recibido_de` | string | Si | "Juan Pérez" |

**Orden de Compra:**

| Campo | Tipo | Obligatorio | Ejemplo |
|-------|------|-------------|---------|
| `numero_oc` | string | Si | "OC-2026-0089" |
| `fecha` | date | Si | "2026-02-18" |
| `proveedor` | string | Si | "Aceros del Norte S.A." |
| `items` | array | Si | [{codigo, descripcion, cantidad, precio_unitario}] |
| `total` | decimal | Si | 45000.00 |
| `condiciones_pago` | string | No | "30 días" |

**Bitácora Operativa:**

| Campo | Tipo | Obligatorio | Ejemplo |
|-------|------|-------------|---------|
| `fecha` | date | Si | "2026-02-20" |
| `turno` | string | Si | "Mañana" |
| `operador_turno` | string | Si | "Miguel Sánchez" |
| `novedades` | text | Si | "Cambio de filtro línea 3..." |
| `equipo` | string | No | "Compresor CP-400" |

**Nota de Entrega:**

| Campo | Tipo | Obligatorio | Ejemplo |
|-------|------|-------------|---------|
| `numero_guia` | string | Si | "GR-2026-01234" |
| `fecha_entrega` | date | Si | "2026-02-20" |
| `remitente` | string | Si | "Distribuidora Central S.A." |
| `destinatario` | string | Si | "Planta Norte" |
| `items` | array | Si | [{descripcion, cantidad, unidad}] |

### Modelo de datos de salida OCR

```json
{
  "document_id": "a3f7c2e1-4b89-4d6a-9c3f-8e2b1d7a5c4f",
  "ocr_engine": "google_cloud_vision",
  "processing_time_ms": 2340,
  "global_confidence": 0.94,
  "document_type": "invoice",
  "raw_text": "FACTURA\nNro: FAC-2026-001234\n...",
  "extracted_fields": {
    "numero_factura": {
      "value": "FAC-2026-001234",
      "confidence": 0.98,
      "bounding_box": {"x": 120, "y": 45, "w": 200, "h": 30}
    },
    "total": {
      "value": 15680.50,
      "confidence": 0.92,
      "bounding_box": {"x": 400, "y": 680, "w": 150, "h": 25}
    }
  },
  "processed_at": "2026-02-24T10:30:05Z"
}
```

---

## RF-03: Clasificación de Documentos

| Campo | Detalle |
|-------|---------|
| **ID** | RF-03 |
| **Nombre** | Clasificación de Documentos |
| **Prioridad** | Alta |
| **Complejidad** | Alta |
| **Casos de uso** | CU-12 |
| **Sprint estimado** | Sprint 2 |

### Descripción

El sistema debe clasificar automáticamente cada documento cargado en una de las categorías soportadas, utilizando análisis de contenido, palabras clave y patrones estructurales.

### Sub-requerimientos

| ID | Descripción | Prioridad |
|----|-------------|-----------|
| RF-03.1 | El sistema debe clasificar documentos en: factura, recibo, orden de compra, bitácora operativa, nota de entrega. | Alta |
| RF-03.2 | El sistema debe asignar un puntaje de confianza a la clasificación. | Alta |
| RF-03.3 | La clasificación automática debe tener una precisión >= 90%. | Alta |
| RF-03.4 | Si la confianza de clasificación es < 85%, el sistema debe solicitar confirmación manual al Operador. | Alta |
| RF-03.5 | El sistema debe permitir al Operador corregir la clasificación asignada. | Media |
| RF-03.6 | El sistema debe utilizar patrones configurables (palabras clave, estructura) para cada tipo de documento. | Media |
| RF-03.7 | El sistema debe aprender de las correcciones manuales para mejorar la clasificación futura. | Baja |

### Patrones de clasificación

| Tipo | Palabras clave | Patrones estructurales |
|------|---------------|----------------------|
| Factura | "FACTURA", "INVOICE", "RUC", "IGV", "IVA", "SUBTOTAL" | Encabezado con datos fiscales, tabla de ítems, totales |
| Recibo | "RECIBO", "RECEIPT", "RECIBÍ" | Monto único, concepto, firma |
| Orden de compra | "ORDEN DE COMPRA", "PURCHASE ORDER", "O/C" | Lista de artículos, proveedor, condiciones |
| Bitácora operativa | "BITÁCORA", "LOG", "TURNO", "NOVEDADES" | Formato tabular con fecha/turno, texto libre |
| Nota de entrega | "GUÍA DE REMISIÓN", "NOTA DE ENTREGA", "DELIVERY NOTE" | Remitente/destinatario, lista de ítems |

---

## RF-04: Validación de Datos Extraídos

| Campo | Detalle |
|-------|---------|
| **ID** | RF-04 |
| **Nombre** | Validación de Datos Extraídos |
| **Prioridad** | Alta |
| **Complejidad** | Alta |
| **Casos de uso** | CU-04 |
| **Sprint estimado** | Sprint 2-3 |

### Descripción

El sistema debe aplicar reglas de validación configurables a los datos extraídos por OCR, incluyendo validaciones de formato, lógica de negocio y validación cruzada con datos maestros del ERP.

### Sub-requerimientos

| ID | Descripción | Prioridad |
|----|-------------|-----------|
| RF-04.1 | El sistema debe validar formatos de campos: RUC/NIF (11/9 dígitos), fechas (ISO 8601), montos (decimal con 2 decimales). | Alta |
| RF-04.2 | El sistema debe validar coherencia matemática: subtotal + impuestos = total. | Alta |
| RF-04.3 | El sistema debe validar que la fecha del documento no sea futura ni anterior a 2 años. | Media |
| RF-04.4 | El sistema debe validar datos contra maestros del ERP: proveedores, materiales, precios referenciales. | Alta |
| RF-04.5 | El sistema debe soportar reglas de validación configurables por tipo de documento. | Alta |
| RF-04.6 | El sistema debe generar un reporte de validación con detalle de cada regla aplicada. | Alta |
| RF-04.7 | El sistema debe asignar estado `validado` o `pendiente_correccion` según resultado. | Alta |
| RF-04.8 | El sistema debe soportar tolerancia configurable para validaciones numéricas (default: +/- 0.5%). | Media |

### Ejemplo de reglas de validación para facturas

```python
REGLAS_VALIDACION_FACTURA = {
    "numero_factura": {
        "tipo": "formato",
        "patron": r"^[A-Z]{1,4}-\d{4}-\d{4,8}$",
        "mensaje_error": "Formato de número de factura inválido"
    },
    "ruc_emisor": {
        "tipo": "formato",
        "patron": r"^\d{11}$",
        "mensaje_error": "RUC debe tener 11 dígitos"
    },
    "coherencia_total": {
        "tipo": "logica",
        "regla": "subtotal + iva == total",
        "tolerancia": 0.005,
        "mensaje_error": "El total no coincide con subtotal + IVA"
    },
    "proveedor_erp": {
        "tipo": "cruzada",
        "fuente": "SAP.BP_VENDOR",
        "campo_busqueda": "ruc_emisor",
        "mensaje_error": "Proveedor no encontrado en maestro SAP"
    }
}
```

### Resultado de validación

```json
{
  "document_id": "uuid",
  "validation_status": "failed",
  "total_rules": 8,
  "passed": 6,
  "failed": 2,
  "results": [
    {
      "rule_id": "coherencia_total",
      "status": "passed",
      "details": "13280.93 + 2399.57 = 15680.50 (esperado: 15680.50)"
    },
    {
      "rule_id": "proveedor_erp",
      "status": "failed",
      "details": "RUC 20512345678 no encontrado en maestro de proveedores SAP",
      "severity": "high"
    }
  ]
}
```

---

## RF-05: Integración ERP/SAP

| Campo | Detalle |
|-------|---------|
| **ID** | RF-05 |
| **Nombre** | Integración ERP/SAP |
| **Prioridad** | Alta |
| **Complejidad** | Alta |
| **Casos de uso** | CU-05 |
| **Sprint estimado** | Sprint 3-4 |

### Descripción

El sistema debe integrarse con el ERP/SAP mediante el protocolo OData para sincronizar los datos extraídos y validados. La integración debe incluir mecanismos de reintento, confirmación de recepción y trazabilidad completa.

### Sub-requerimientos

| ID | Descripción | Prioridad |
|----|-------------|-----------|
| RF-05.1 | El sistema debe enviar datos validados al ERP/SAP mediante OData v4. | Alta |
| RF-05.2 | El sistema debe establecer conexión cifrada (TLS 1.3) con el endpoint OData. | Alta |
| RF-05.3 | El sistema debe implementar mecanismo de reintento con backoff exponencial (1, 5, 15 minutos). | Alta |
| RF-05.4 | El sistema debe almacenar la referencia cruzada (ID documento OCR <-> número SAP). | Alta |
| RF-05.5 | El sistema debe verificar la recepción exitosa del dato en SAP antes de marcar como sincronizado. | Alta |
| RF-05.6 | El sistema debe manejar respuestas de error de SAP y mapearlas a estados internos. | Alta |
| RF-05.7 | El sistema debe soportar envío individual y por lotes al ERP. | Media |
| RF-05.8 | El sistema debe registrar cada intento de envío en la pista de auditoría. | Alta |
| RF-05.9 | El sistema debe notificar al Administrador cuando se agoten los reintentos. | Alta |
| RF-05.10 | El sistema debe permitir reenvío manual de documentos rechazados por el ERP. | Media |

### Mapeo de entidades OCR a SAP

| Tipo Documento OCR | Entidad SAP | Servicio OData |
|---------------------|-------------|----------------|
| Factura | `SupplierInvoice` | `/API_SUPPLIERINVOICE_PROCESS_SRV` |
| Orden de compra | `PurchaseOrder` | `/API_PURCHASEORDER_PROCESS_SRV` |
| Nota de entrega | `InboundDelivery` | `/API_INBOUND_DELIVERY_SRV` |
| Recibo | `PaymentDocument` | `/API_PAYMENT_SRV` |
| Bitácora operativa | `MaintenanceNotification` | `/API_MAINTNOTIFICATION` |

### Flujo de reintento

```
Intento 1 (inmediato) -> Fallo -> Espera 1 min
Intento 2              -> Fallo -> Espera 5 min
Intento 3              -> Fallo -> Espera 15 min
Intento 4 (final)      -> Fallo -> Marca como "error_erp" + Notifica Administrador
```

### Ejemplo de configuración de integración

```yaml
erp_integration:
  endpoint: "https://sap.empresa.com/sap/opu/odata4/sap/"
  auth:
    type: "oauth2"
    token_url: "https://sap.empresa.com/oauth/token"
    client_id: "${SAP_CLIENT_ID}"
    client_secret: "${SAP_CLIENT_SECRET}"
  retry:
    max_attempts: 4
    backoff_intervals: [60, 300, 900]  # segundos
  timeout: 30  # segundos por request
  batch_size: 50  # documentos por lote
  tls_version: "1.3"
```

---

## RF-06: Cola de Revisión Manual

| Campo | Detalle |
|-------|---------|
| **ID** | RF-06 |
| **Nombre** | Cola de Revisión Manual |
| **Prioridad** | Alta |
| **Complejidad** | Media |
| **Casos de uso** | CU-03, CU-06 |
| **Sprint estimado** | Sprint 2-3 |

### Descripción

El sistema debe proporcionar una interfaz de usuario para la revisión y corrección manual de documentos que no cumplan con el umbral de confianza requerido (< 70%). La interfaz debe facilitar la comparación visual entre el documento original y los datos extraídos.

### Sub-requerimientos

| ID | Descripción | Prioridad |
|----|-------------|-----------|
| RF-06.1 | El sistema debe enrutar automáticamente los documentos con confianza < 70% a la cola de revisión. | Alta |
| RF-06.2 | La interfaz debe mostrar vista dividida: documento original (izquierda) y formulario de datos (derecha). | Alta |
| RF-06.3 | El sistema debe resaltar campos con confianza baja: amarillo (60-79%) y rojo (< 60%). | Alta |
| RF-06.4 | El sistema debe permitir zoom y navegación en el documento original. | Media |
| RF-06.5 | El sistema debe resaltar la zona del documento correspondiente al campo seleccionado. | Alta |
| RF-06.6 | El sistema debe implementar bloqueo de documento para evitar edición concurrente. | Alta |
| RF-06.7 | El sistema debe liberar el bloqueo automáticamente tras 30 minutos de inactividad. | Media |
| RF-06.8 | El sistema debe permitir guardar progreso parcial de la revisión. | Media |
| RF-06.9 | El sistema debe permitir re-procesar zonas específicas del documento con OCR. | Media |
| RF-06.10 | El sistema debe permitir aprobar, rechazar o escalar el documento. | Alta |
| RF-06.11 | La cola debe ordenarse por FIFO con opción de priorizar documentos urgentes. | Alta |

### Wireframe de la interfaz de revisión

```
+------------------------------------------------------------------+
| OCR Operativo - Revisión Manual          [Ana García] [Cerrar]   |
+------------------------------------------------------------------+
| Cola: 23 pendientes | En revisión: FAC-2026-001234               |
+-------------------------------+----------------------------------+
|                               |  Datos Extraídos                 |
|   [Documento Original]       |                                  |
|                               |  Nro. Factura: [FAC-2026-001234]|
|   +---------------------+    |  Fecha:        [2026-02-20]     |
|   |                     |    |  RUC Emisor:   [20512345678] *  |
|   |   (Imagen del       |    |  Razón Social: [Suministr...] * |
|   |    documento)       |    |  Subtotal:     [13280.93]      |
|   |                     |    |  IVA:          [2399.57]       |
|   |   [+ Zoom] [- Zoom] |    |  Total:        [15680.50]      |
|   +---------------------+    |                                  |
|                               |  * Campo con baja confianza     |
|   Página 1 de 3              |                                  |
|   [<Anterior] [Siguiente>]   |  [Aprobar] [Rechazar] [Escalar] |
+-------------------------------+----------------------------------+
```

---

## RF-07: Dashboard y Reportes

| Campo | Detalle |
|-------|---------|
| **ID** | RF-07 |
| **Nombre** | Dashboard y Reportes |
| **Prioridad** | Media |
| **Complejidad** | Media |
| **Casos de uso** | CU-07 |
| **Sprint estimado** | Sprint 4-5 |

### Descripción

El sistema debe proporcionar un dashboard interactivo con métricas clave del procesamiento OCR y un módulo de generación de reportes exportables.

### Sub-requerimientos

| ID | Descripción | Prioridad |
|----|-------------|-----------|
| RF-07.1 | El dashboard debe mostrar métricas en tiempo real: documentos procesados hoy, en cola, tasa de éxito. | Alta |
| RF-07.2 | El dashboard debe incluir gráfico de volumen de procesamiento por día/semana/mes. | Alta |
| RF-07.3 | El dashboard debe mostrar la tasa de precisión OCR promedio (objetivo >= 95%). | Alta |
| RF-07.4 | El dashboard debe mostrar el estado de la cola de revisión manual. | Media |
| RF-07.5 | El dashboard debe mostrar el estado de sincronización con ERP. | Media |
| RF-07.6 | El sistema debe permitir generar reportes por rango de fechas, tipo de documento, estado y usuario. | Alta |
| RF-07.7 | El sistema debe permitir exportar reportes en PDF, Excel (XLSX) y CSV. | Alta |
| RF-07.8 | El sistema debe generar reportes de cumplimiento GDPR. | Media |
| RF-07.9 | El sistema debe permitir programar reportes automáticos (diarios, semanales, mensuales). | Baja |
| RF-07.10 | Los reportes de auditoría deben estar disponibles exclusivamente para roles Auditor y Administrador. | Alta |

### Métricas del dashboard

| Métrica | Descripción | Actualización |
|---------|-------------|---------------|
| Documentos procesados hoy | Contador del día actual | Tiempo real |
| Tasa de éxito OCR | % documentos con confianza >= 70% | Cada 5 minutos |
| Confianza promedio | Media de puntaje de confianza global | Cada 5 minutos |
| Cola de revisión | Documentos pendientes de revisión manual | Tiempo real |
| Tiempo promedio procesamiento | Segundos promedio por documento | Cada hora |
| Sincronización ERP | Documentos enviados / pendientes / error | Cada 5 minutos |
| Documentos por tipo | Distribución por tipo de documento | Cada hora |

---

## RF-08: Gestión de Configuración

| Campo | Detalle |
|-------|---------|
| **ID** | RF-08 |
| **Nombre** | Gestión de Configuración |
| **Prioridad** | Media |
| **Complejidad** | Media |
| **Casos de uso** | CU-08, CU-11 |
| **Sprint estimado** | Sprint 3 |

### Descripción

El sistema debe proporcionar una interfaz de administración para configurar los parámetros del sistema OCR, gestionar usuarios y roles, y ajustar los umbrales y reglas de procesamiento.

### Sub-requerimientos

| ID | Descripción | Prioridad |
|----|-------------|-----------|
| RF-08.1 | El sistema debe permitir configurar los umbrales de confianza (aprobación automática, fallback, revisión). | Alta |
| RF-08.2 | El sistema debe permitir configurar parámetros de los motores OCR (idioma, DPI mínimo, pre-procesamiento). | Alta |
| RF-08.3 | El sistema debe permitir administrar reglas de validación por tipo de documento. | Alta |
| RF-08.4 | El sistema debe permitir configurar la integración ERP (endpoint, credenciales, timeout, reintentos). | Alta |
| RF-08.5 | El sistema debe permitir gestionar usuarios: crear, editar, desactivar. | Alta |
| RF-08.6 | El sistema debe permitir asignar roles: Operador, Revisor, Administrador, Auditor. | Alta |
| RF-08.7 | El sistema debe permitir configurar notificaciones (eventos, canales). | Media |
| RF-08.8 | El sistema debe permitir configurar políticas de retención de documentos. | Media |
| RF-08.9 | El sistema debe validar cambios de configuración antes de aplicarlos. | Alta |
| RF-08.10 | Cada cambio de configuración debe registrar valores anteriores y nuevos en auditoría. | Alta |

### Parámetros configurables

```yaml
ocr_config:
  primary_engine: "google_cloud_vision"  # google_cloud_vision | tesseract
  fallback_engine: "tesseract"
  languages: ["es", "en"]
  min_dpi: 150
  preprocessing:
    auto_rotate: true
    deskew: true
    contrast_enhancement: false

thresholds:
  auto_approval: 0.70        # Confianza mínima para aprobación automática
  fallback_trigger: 0.60     # Confianza que dispara el motor de respaldo
  classification_min: 0.85   # Confianza mínima para clasificación automática

validation:
  numeric_tolerance: 0.005   # Tolerancia para validaciones numéricas (0.5%)
  date_range_past_years: 2   # Años hacia atrás permitidos

retention:
  documents_days: 365        # Días de retención de documentos
  audit_logs_days: 730       # Días de retención de logs de auditoría

notifications:
  channels: ["email", "in_app"]
  events:
    - "batch_complete"
    - "erp_sync_failure"
    - "manual_review_escalation"
    - "system_error"
```

---

## RF-09: Auditoría y Trazabilidad

| Campo | Detalle |
|-------|---------|
| **ID** | RF-09 |
| **Nombre** | Auditoría y Trazabilidad |
| **Prioridad** | Alta |
| **Complejidad** | Media |
| **Casos de uso** | CU-09 |
| **Sprint estimado** | Sprint 2 |

### Descripción

El sistema debe mantener un registro inmutable de todas las acciones realizadas sobre cada documento y sobre la configuración del sistema, garantizando la trazabilidad completa del ciclo de vida de cada documento.

### Sub-requerimientos

| ID | Descripción | Prioridad |
|----|-------------|-----------|
| RF-09.1 | El sistema debe registrar cada acción sobre un documento: carga, procesamiento, revisión, validación, envío ERP. | Alta |
| RF-09.2 | Cada registro de auditoría debe incluir: timestamp, usuario, acción, datos antes/después, dirección IP. | Alta |
| RF-09.3 | Los registros de auditoría deben ser inmutables (no se pueden modificar ni eliminar). | Alta |
| RF-09.4 | El sistema debe registrar cambios de configuración con valores anteriores y nuevos. | Alta |
| RF-09.5 | El sistema debe registrar inicios y cierres de sesión. | Alta |
| RF-09.6 | El sistema debe permitir búsqueda y filtrado de registros de auditoría. | Alta |
| RF-09.7 | El sistema debe permitir exportar registros de auditoría en CSV y PDF. | Media |
| RF-09.8 | Los registros de auditoría deben retenerse por un mínimo de 2 años (configurable). | Alta |
| RF-09.9 | El sistema debe generar un hash de integridad para cada registro de auditoría. | Alta |
| RF-09.10 | El acceso a los registros de auditoría debe estar restringido a roles Auditor y Administrador. | Alta |

### Estructura de registro de auditoría

```json
{
  "audit_id": "uuid",
  "timestamp": "2026-02-24T10:30:05.123Z",
  "event_type": "DOC_REVIEW",
  "user_id": "uuid",
  "user_name": "Ana García",
  "user_role": "Revisor",
  "ip_address": "192.168.1.45",
  "resource_type": "document",
  "resource_id": "a3f7c2e1-4b89-4d6a-9c3f-8e2b1d7a5c4f",
  "action": "field_correction",
  "details": {
    "field": "total",
    "old_value": "15860.50",
    "new_value": "15680.50",
    "reason": "Error de OCR en dígito"
  },
  "integrity_hash": "sha256:9f86d081884c7d659a2feaa0c55ad015..."
}
```

### Esquema de base de datos

```sql
CREATE TABLE audit_log (
    audit_id        UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    timestamp       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    event_type      VARCHAR(50) NOT NULL,
    user_id         UUID REFERENCES users(id),
    user_name       VARCHAR(255),
    user_role       VARCHAR(50),
    ip_address      INET,
    resource_type   VARCHAR(50),
    resource_id     UUID,
    action          VARCHAR(100) NOT NULL,
    details         JSONB,
    integrity_hash  VARCHAR(128) NOT NULL,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Índices para búsqueda eficiente
CREATE INDEX idx_audit_timestamp ON audit_log(timestamp);
CREATE INDEX idx_audit_resource ON audit_log(resource_type, resource_id);
CREATE INDEX idx_audit_user ON audit_log(user_id);
CREATE INDEX idx_audit_event ON audit_log(event_type);

-- La tabla NO permite UPDATE ni DELETE (inmutabilidad)
REVOKE UPDATE, DELETE ON audit_log FROM PUBLIC;
```

---

## RF-10: Procesamiento Batch

| Campo | Detalle |
|-------|---------|
| **ID** | RF-10 |
| **Nombre** | Procesamiento Batch |
| **Prioridad** | Media |
| **Complejidad** | Alta |
| **Casos de uso** | CU-10 |
| **Sprint estimado** | Sprint 4 |

### Descripción

El sistema debe soportar la carga y procesamiento masivo de documentos en lotes, con procesamiento en segundo plano, seguimiento de progreso en tiempo real y generación de resumen al completar.

### Sub-requerimientos

| ID | Descripción | Prioridad |
|----|-------------|-----------|
| RF-10.1 | El sistema debe permitir carga de hasta 500 documentos por lote. | Alta |
| RF-10.2 | El sistema debe procesar documentos del lote en paralelo (workers concurrentes). | Alta |
| RF-10.3 | El sistema debe mostrar progreso del batch en tiempo real (porcentaje, documentos procesados/total). | Alta |
| RF-10.4 | El sistema debe generar un resumen al completar el batch: exitosos, errores, enviados a revisión. | Alta |
| RF-10.5 | El sistema debe permitir cancelar un batch en curso. | Media |
| RF-10.6 | El sistema debe continuar procesando el batch si un documento individual falla. | Alta |
| RF-10.7 | El sistema debe notificar al usuario al completar o fallar el batch. | Alta |
| RF-10.8 | El sistema debe permitir configurar la prioridad del batch (normal, alta, urgente). | Media |
| RF-10.9 | El sistema debe mantener la capacidad de procesamiento de documentos individuales durante un batch. | Alta |
| RF-10.10 | El procesamiento batch debe respetar el throughput objetivo de 1000+ documentos/día. | Alta |

### Endpoint API batch

```
POST /api/v1/batch/create
Content-Type: multipart/form-data

Request:
  - files: File[] (requerido, máximo 500)
  - priority: string (opcional, enum: normal, high, urgent)
  - document_type: string (opcional, forzar tipo para todo el lote)
  - notify_email: string (opcional)

Response (202 Accepted):
{
  "batch_id": "uuid",
  "total_documents": 250,
  "status": "queued",
  "estimated_time_minutes": 12,
  "created_at": "2026-02-24T10:00:00Z"
}

GET /api/v1/batch/{batch_id}/status

Response (200 OK):
{
  "batch_id": "uuid",
  "status": "processing",
  "total": 250,
  "processed": 180,
  "successful": 165,
  "failed": 5,
  "manual_review": 10,
  "remaining": 70,
  "progress_percent": 72.0,
  "elapsed_time_seconds": 540,
  "estimated_remaining_seconds": 210
}
```

---

## Matriz de Prioridades

| ID | Requerimiento | Prioridad | Complejidad | Sprint | Dependencias |
|----|---------------|-----------|-------------|--------|-------------|
| RF-01 | Carga de Documentos | Alta | Media | 1 | - |
| RF-02 | Procesamiento OCR | Alta | Alta | 1-2 | RF-01 |
| RF-03 | Clasificación de Documentos | Alta | Alta | 2 | RF-02 |
| RF-04 | Validación de Datos Extraídos | Alta | Alta | 2-3 | RF-02, RF-03 |
| RF-05 | Integración ERP/SAP | Alta | Alta | 3-4 | RF-04 |
| RF-06 | Cola de Revisión Manual | Alta | Media | 2-3 | RF-02 |
| RF-07 | Dashboard y Reportes | Media | Media | 4-5 | RF-02, RF-05 |
| RF-08 | Gestión de Configuración | Media | Media | 3 | RF-02 |
| RF-09 | Auditoría y Trazabilidad | Alta | Media | 2 | RF-01 |
| RF-10 | Procesamiento Batch | Media | Alta | 4 | RF-01, RF-02 |

### Diagrama de dependencias

```
RF-01 (Carga) ──> RF-02 (OCR) ──> RF-03 (Clasificación) ──> RF-04 (Validación) ──> RF-05 (ERP)
      │                │                                             │
      │                ├──> RF-06 (Revisión Manual)                  │
      │                │                                             │
      ├──> RF-09 (Auditoría)                                        │
      │                                                              │
      ├──> RF-10 (Batch) ──> RF-02                                   │
                                                                     │
                       RF-07 (Dashboard) <──────────────────────────┘
                       RF-08 (Configuración)
```

---

*Documento generado el 2026-02-24 por IDC Ingeniería.*
