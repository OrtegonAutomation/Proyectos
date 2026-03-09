# Casos de Uso - OCR Operativo

| Campo | Detalle |
|-------|---------|
| **Proyecto** | OCR Operativo |
| **Versión** | 1.0 |
| **Fecha** | 2026-02-24 |
| **Estado** | En revisión |
| **Autor** | IDC Ingeniería |

---

## Tabla de Contenidos

1. [Diagrama General de Actores](#diagrama-general-de-actores)
2. [Casos de Uso](#casos-de-uso)
   - [CU-01: Cargar Documento](#cu-01-cargar-documento)
   - [CU-02: Procesar Documento con OCR](#cu-02-procesar-documento-con-ocr)
   - [CU-03: Revisar Extracción de Datos](#cu-03-revisar-extracción-de-datos)
   - [CU-04: Validar Resultados Extraídos](#cu-04-validar-resultados-extraídos)
   - [CU-05: Enviar Datos a ERP/SAP](#cu-05-enviar-datos-a-erpsap)
   - [CU-06: Revisión Manual de Documentos](#cu-06-revisión-manual-de-documentos)
   - [CU-07: Generar Reportes](#cu-07-generar-reportes)
   - [CU-08: Configurar Sistema OCR](#cu-08-configurar-sistema-ocr)
   - [CU-09: Consultar Pista de Auditoría](#cu-09-consultar-pista-de-auditoría)
   - [CU-10: Procesamiento Batch de Documentos](#cu-10-procesamiento-batch-de-documentos)
   - [CU-11: Gestionar Usuarios y Roles](#cu-11-gestionar-usuarios-y-roles)
   - [CU-12: Clasificar Documento Automáticamente](#cu-12-clasificar-documento-automáticamente)
3. [Matriz de Trazabilidad](#matriz-de-trazabilidad)

---

## Diagrama General de Actores

| Actor | Descripción | Permisos principales |
|-------|-------------|----------------------|
| **Operador** | Responsable de la carga y procesamiento inicial de documentos | Cargar, procesar, consultar estado |
| **Revisor** | Encargado de revisar y corregir extracciones de baja confianza | Revisar, corregir, aprobar, rechazar |
| **Administrador** | Gestiona la configuración del sistema y usuarios | Configurar, gestionar usuarios, reportes globales |
| **Auditor** | Consulta la trazabilidad y genera reportes de cumplimiento | Consultar auditoría, generar reportes de compliance |
| **Sistema ERP/SAP** | Sistema externo que recibe los datos validados vía OData | Recibir datos, confirmar recepción |
| **Motor OCR** | Componente interno (Google Cloud Vision / Tesseract) | Procesar imágenes, extraer texto |

---

## Casos de Uso

---

### CU-01: Cargar Documento

| Campo | Detalle |
|-------|---------|
| **ID** | CU-01 |
| **Nombre** | Cargar Documento |
| **Actor principal** | Operador |
| **Actores secundarios** | Motor OCR (invocación posterior) |
| **Prioridad** | Alta |
| **Complejidad** | Media |

**Descripción:**
El Operador carga uno o varios documentos operativos al sistema para su posterior procesamiento OCR. El sistema acepta formatos PDF, PNG, JPG y TIFF.

**Precondiciones:**
- El Operador ha iniciado sesión con credenciales válidas (OAuth 2.0).
- El Operador tiene el rol `Operador` asignado.
- Existe conexión estable con el servidor de almacenamiento.

**Flujo principal:**

| Paso | Acción |
|------|--------|
| 1 | El Operador accede al módulo de carga de documentos desde el dashboard. |
| 2 | El sistema presenta la interfaz de carga con zona de arrastrar y soltar (drag & drop). |
| 3 | El Operador selecciona uno o más archivos desde su sistema local o los arrastra a la zona de carga. |
| 4 | El sistema valida el formato de cada archivo (PDF, PNG, JPG, TIFF). |
| 5 | El sistema valida el tamaño de cada archivo (máximo 50 MB por archivo). |
| 6 | El sistema muestra una barra de progreso durante la subida. |
| 7 | El sistema almacena los archivos con cifrado AES-256 en el repositorio. |
| 8 | El sistema genera un identificador único (UUID) para cada documento. |
| 9 | El sistema registra la carga en la pista de auditoría (usuario, fecha, hora, nombre de archivo, tamaño). |
| 10 | El sistema muestra confirmación de carga exitosa con el listado de documentos subidos. |

**Flujos alternativos:**

| ID | Condición | Acción |
|----|-----------|--------|
| FA-01.1 | Formato de archivo no soportado | El sistema rechaza el archivo, muestra mensaje de error indicando los formatos válidos y permite al Operador seleccionar otro archivo. |
| FA-01.2 | Archivo excede tamaño máximo | El sistema rechaza el archivo, muestra mensaje indicando el límite de 50 MB y sugiere comprimir o dividir el documento. |
| FA-01.3 | Error de conexión durante la carga | El sistema pausa la carga, muestra mensaje de reconexión y reintenta automáticamente hasta 3 veces. Si falla, notifica al Operador. |
| FA-01.4 | Archivo duplicado detectado | El sistema alerta al Operador sobre el duplicado (basado en hash SHA-256) y solicita confirmación para continuar o cancelar. |
| FA-01.5 | Archivo corrupto o ilegible | El sistema rechaza el archivo con mensaje descriptivo y registra el evento en el log de errores. |

**Postcondiciones:**
- Los documentos quedan almacenados en el repositorio con estado `pendiente_procesamiento`.
- Se genera un registro de auditoría para cada documento cargado.
- Los documentos están disponibles para procesamiento OCR.

**Reglas de negocio:**
- Formatos aceptados: PDF, PNG, JPG, TIFF.
- Tamaño máximo por archivo: 50 MB.
- Tamaño máximo por lote de carga: 500 MB.
- Máximo 50 archivos por operación de carga.

---

### CU-02: Procesar Documento con OCR

| Campo | Detalle |
|-------|---------|
| **ID** | CU-02 |
| **Nombre** | Procesar Documento con OCR |
| **Actor principal** | Sistema (automático) / Operador (manual) |
| **Actores secundarios** | Motor OCR (Google Cloud Vision, Tesseract) |
| **Prioridad** | Alta |
| **Complejidad** | Alta |

**Descripción:**
El sistema procesa un documento cargado mediante el motor OCR principal (Google Cloud Vision). Si el motor principal no está disponible o el resultado tiene confianza inferior al 60%, se utiliza Tesseract como motor de respaldo. Se extraen campos clave según el tipo de documento.

**Precondiciones:**
- El documento existe en el repositorio con estado `pendiente_procesamiento`.
- Al menos un motor OCR está operativo (Google Cloud Vision o Tesseract).
- El documento ha pasado la validación de formato.

**Flujo principal:**

| Paso | Acción |
|------|--------|
| 1 | El sistema selecciona el siguiente documento de la cola de procesamiento (FIFO). |
| 2 | El sistema envía el documento al motor OCR principal (Google Cloud Vision). |
| 3 | El motor OCR analiza el documento y extrae texto con coordenadas. |
| 4 | El sistema calcula el puntaje de confianza global del documento. |
| 5 | El sistema identifica y extrae campos estructurados según el tipo de documento detectado. |
| 6 | El sistema almacena los resultados de extracción en la base de datos PostgreSQL. |
| 7 | El sistema actualiza el estado del documento según el puntaje de confianza: `procesado` (>=70%) o `revision_manual` (<70%). |
| 8 | El sistema registra el evento de procesamiento en la pista de auditoría (tiempo, motor, confianza). |
| 9 | El sistema notifica al Operador del resultado del procesamiento. |

**Flujos alternativos:**

| ID | Condición | Acción |
|----|-----------|--------|
| FA-02.1 | Google Cloud Vision no disponible | El sistema automáticamente cambia a Tesseract como motor de respaldo y registra el fallback en el log. |
| FA-02.2 | Confianza global < 60% con motor principal | El sistema reprocesa con Tesseract. Si la confianza combinada sigue siendo < 70%, envía a cola de revisión manual. |
| FA-02.3 | Tiempo de procesamiento excede 5 segundos | El sistema registra una alerta de rendimiento, completa el procesamiento y marca el documento para revisión de optimización. |
| FA-02.4 | Documento contiene múltiples páginas | El sistema procesa cada página individualmente y combina los resultados en un solo registro de extracción. |
| FA-02.5 | Error irrecuperable en procesamiento | El sistema marca el documento como `error_procesamiento`, registra el detalle del error y notifica al Administrador. |

**Postcondiciones:**
- El documento tiene un registro de extracción OCR asociado.
- El puntaje de confianza está calculado y almacenado.
- El documento está clasificado para siguiente paso (validación o revisión manual).
- El evento queda registrado en la pista de auditoría.

**Reglas de negocio:**
- Tiempo máximo de procesamiento por documento: 5 segundos.
- Umbral de confianza para aprobación automática: >= 70%.
- Motor principal: Google Cloud Vision.
- Motor de respaldo: Tesseract.
- Objetivo de precisión: >= 95%.

---

### CU-03: Revisar Extracción de Datos

| Campo | Detalle |
|-------|---------|
| **ID** | CU-03 |
| **Nombre** | Revisar Extracción de Datos |
| **Actor principal** | Revisor |
| **Actores secundarios** | - |
| **Prioridad** | Alta |
| **Complejidad** | Media |

**Descripción:**
El Revisor examina los datos extraídos por el motor OCR para un documento procesado, compara los datos extraídos con el documento original y confirma o corrige los valores.

**Precondiciones:**
- El documento ha sido procesado por el motor OCR.
- El Revisor ha iniciado sesión con el rol `Revisor`.
- El documento tiene estado `procesado` o `revision_manual`.

**Flujo principal:**

| Paso | Acción |
|------|--------|
| 1 | El Revisor accede a la bandeja de documentos procesados. |
| 2 | El sistema muestra la lista de documentos pendientes de revisión, ordenados por prioridad y fecha. |
| 3 | El Revisor selecciona un documento de la lista. |
| 4 | El sistema muestra una vista dividida: documento original a la izquierda y datos extraídos a la derecha. |
| 5 | El sistema resalta los campos con confianza baja (< 80%) en color amarillo y los campos con confianza muy baja (< 60%) en rojo. |
| 6 | El Revisor verifica cada campo extraído contra el documento original. |
| 7 | El Revisor corrige los campos incorrectos directamente en el formulario de datos. |
| 8 | El Revisor marca el documento como `revisado` y confirma los cambios. |
| 9 | El sistema almacena las correcciones y registra la revisión en la pista de auditoría. |

**Flujos alternativos:**

| ID | Condición | Acción |
|----|-----------|--------|
| FA-03.1 | Documento ilegible o de mala calidad | El Revisor marca el documento como `ilegible`, registra observaciones y el documento se reenvía al Operador para nueva carga. |
| FA-03.2 | El Revisor no puede completar la revisión | El Revisor guarda el progreso parcial y el documento permanece en la cola para continuar después. |
| FA-03.3 | Discrepancia significativa en datos | El Revisor puede escalar el documento al Administrador con comentarios detallados. |

**Postcondiciones:**
- Los datos extraídos están verificados y corregidos si fue necesario.
- El documento tiene estado `revisado`.
- Las correcciones quedan registradas en la pista de auditoría con detalle de cambios.

---

### CU-04: Validar Resultados Extraídos

| Campo | Detalle |
|-------|---------|
| **ID** | CU-04 |
| **Nombre** | Validar Resultados Extraídos |
| **Actor principal** | Sistema (automático) |
| **Actores secundarios** | Revisor (en caso de fallos de validación) |
| **Prioridad** | Alta |
| **Complejidad** | Alta |

**Descripción:**
El sistema aplica reglas de negocio configuradas para validar los datos extraídos del documento. Las validaciones incluyen formato de campos, rangos numéricos, consistencia de datos y validación cruzada con datos maestros del ERP.

**Precondiciones:**
- El documento ha sido procesado por OCR y tiene datos extraídos.
- Las reglas de validación están configuradas para el tipo de documento.
- Existe conectividad con el ERP/SAP para validación cruzada (si aplica).

**Flujo principal:**

| Paso | Acción |
|------|--------|
| 1 | El sistema selecciona un documento con estado `procesado` o `revisado`. |
| 2 | El sistema identifica el tipo de documento y carga las reglas de validación correspondientes. |
| 3 | El sistema aplica validaciones de formato (RUC/NIF, fechas, montos). |
| 4 | El sistema aplica validaciones de negocio (totales cuadren, IVA correcto, etc.). |
| 5 | El sistema ejecuta validación cruzada con datos maestros del ERP (proveedor, artículos, precios). |
| 6 | El sistema genera un reporte de validación con el resultado de cada regla. |
| 7 | Si todas las validaciones pasan: el documento cambia a estado `validado`. |
| 8 | Si alguna validación falla: el documento cambia a estado `pendiente_correccion` y se notifica al Revisor. |
| 9 | El sistema registra los resultados de validación en la pista de auditoría. |

**Flujos alternativos:**

| ID | Condición | Acción |
|----|-----------|--------|
| FA-04.1 | ERP no disponible para validación cruzada | El sistema aplica únicamente validaciones locales, marca la validación cruzada como `pendiente` y programa un reintento. |
| FA-04.2 | Reglas de validación no definidas para el tipo | El sistema envía el documento a revisión manual con alerta de configuración faltante para el Administrador. |
| FA-04.3 | Múltiples errores de validación | El sistema agrupa los errores y genera una ficha de corrección priorizada para el Revisor. |

**Postcondiciones:**
- El documento tiene un estado de validación definido (`validado` o `pendiente_correccion`).
- El reporte de validación está almacenado y asociado al documento.
- Los documentos con errores están en la cola del Revisor correspondiente.

**Reglas de validación por tipo de documento:**

| Tipo | Validaciones |
|------|-------------|
| Factura | RUC/NIF emisor, fecha, subtotal + IVA = total, número secuencial |
| Recibo | Fecha, monto, concepto no vacío |
| Orden de compra | Número OC, proveedor, ítems con precio unitario y cantidad |
| Bitácora operativa | Fecha, turno, operador, campos obligatorios completos |
| Nota de entrega | Número guía, proveedor/receptor, ítems, cantidades |

---

### CU-05: Enviar Datos a ERP/SAP

| Campo | Detalle |
|-------|---------|
| **ID** | CU-05 |
| **Nombre** | Enviar Datos a ERP/SAP |
| **Actor principal** | Sistema (automático) / Administrador (manual) |
| **Actores secundarios** | Sistema ERP/SAP |
| **Prioridad** | Alta |
| **Complejidad** | Alta |

**Descripción:**
El sistema envía los datos validados al sistema ERP/SAP mediante la interfaz OData. El envío incluye mecanismos de reintento, confirmación de recepción y registro de auditoría completo.

**Precondiciones:**
- El documento tiene estado `validado`.
- La conexión OData con el ERP/SAP está configurada y activa.
- Las credenciales de integración son válidas.

**Flujo principal:**

| Paso | Acción |
|------|--------|
| 1 | El sistema selecciona documentos validados pendientes de envío al ERP. |
| 2 | El sistema transforma los datos extraídos al formato OData requerido por SAP. |
| 3 | El sistema establece conexión segura (TLS 1.3) con el endpoint OData. |
| 4 | El sistema envía los datos al ERP/SAP. |
| 5 | El ERP/SAP confirma la recepción y devuelve el número de registro SAP. |
| 6 | El sistema actualiza el estado del documento a `sincronizado_erp`. |
| 7 | El sistema almacena el número de registro SAP como referencia cruzada. |
| 8 | El sistema registra el envío exitoso en la pista de auditoría. |

**Flujos alternativos:**

| ID | Condición | Acción |
|----|-----------|--------|
| FA-05.1 | ERP no disponible | El sistema coloca el documento en cola de reintento. Se reintenta a los 1, 5 y 15 minutos. Si falla 3 veces, notifica al Administrador. |
| FA-05.2 | Error de validación en ERP | El ERP rechaza los datos. El sistema registra el motivo, marca el documento como `rechazado_erp` y notifica al Revisor. |
| FA-05.3 | Timeout en comunicación | El sistema registra el timeout, verifica si el dato fue recibido (consulta de estado) y reintenta si es necesario. |
| FA-05.4 | Datos duplicados en ERP | El ERP detecta duplicado. El sistema marca el documento como `duplicado_erp` y alerta al Administrador. |

**Postcondiciones:**
- Los datos del documento están registrados en el ERP/SAP.
- Existe referencia cruzada entre el ID del documento y el número SAP.
- El evento de sincronización está registrado en la pista de auditoría.

**Ejemplo de payload OData:**

```json
{
  "DocumentType": "INVOICE",
  "VendorId": "V-10045",
  "DocumentNumber": "FAC-2026-001234",
  "DocumentDate": "2026-02-20",
  "TotalAmount": 15680.50,
  "Currency": "USD",
  "LineItems": [
    {
      "MaterialCode": "MAT-500",
      "Description": "Rodamiento SKF 6205",
      "Quantity": 50,
      "UnitPrice": 245.00,
      "TotalLine": 12250.00
    }
  ],
  "OcrConfidence": 0.94,
  "SourceDocumentId": "a3f7c2e1-4b89-4d6a-9c3f-8e2b1d7a5c4f"
}
```

---

### CU-06: Revisión Manual de Documentos

| Campo | Detalle |
|-------|---------|
| **ID** | CU-06 |
| **Nombre** | Revisión Manual de Documentos |
| **Actor principal** | Revisor |
| **Actores secundarios** | - |
| **Prioridad** | Alta |
| **Complejidad** | Media |

**Descripción:**
El Revisor gestiona la cola de documentos que requieren intervención manual debido a baja confianza en la extracción OCR (< 70%). El flujo incluye la corrección de datos, re-procesamiento parcial y aprobación o rechazo del documento.

**Precondiciones:**
- Existen documentos en la cola de revisión manual con estado `revision_manual`.
- El Revisor ha iniciado sesión con el rol `Revisor`.

**Flujo principal:**

| Paso | Acción |
|------|--------|
| 1 | El Revisor accede a la cola de revisión manual. |
| 2 | El sistema muestra los documentos ordenados por antigüedad (FIFO) con indicadores de confianza. |
| 3 | El Revisor selecciona un documento y lo bloquea para evitar edición concurrente. |
| 4 | El sistema presenta la vista de comparación: imagen original vs. datos extraídos. |
| 5 | El sistema resalta zonas del documento correspondientes a cada campo extraído. |
| 6 | El Revisor corrige manualmente los campos incorrectos. |
| 7 | El Revisor puede solicitar un re-procesamiento parcial de una zona específica. |
| 8 | El Revisor confirma las correcciones y aprueba el documento. |
| 9 | El sistema actualiza los datos, recalcula la confianza y cambia el estado a `revisado_manual`. |
| 10 | El sistema libera el bloqueo del documento. |

**Flujos alternativos:**

| ID | Condición | Acción |
|----|-----------|--------|
| FA-06.1 | Documento completamente ilegible | El Revisor rechaza el documento con motivo `ilegible`. Se notifica al Operador para nueva carga. |
| FA-06.2 | Revisor no completa la revisión | El sistema guarda progreso parcial y libera el bloqueo tras 30 minutos de inactividad. |
| FA-06.3 | Documento requiere aprobación de Administrador | El Revisor escala el documento con comentarios. El Administrador recibe notificación para aprobar. |

**Postcondiciones:**
- El documento ha sido revisado y sus datos corregidos.
- El estado del documento es `revisado_manual`, `rechazado` o `escalado`.
- El bloqueo del documento fue liberado.
- Todas las correcciones están registradas en la pista de auditoría.

---

### CU-07: Generar Reportes

| Campo | Detalle |
|-------|---------|
| **ID** | CU-07 |
| **Nombre** | Generar Reportes |
| **Actor principal** | Administrador / Auditor |
| **Actores secundarios** | - |
| **Prioridad** | Media |
| **Complejidad** | Media |

**Descripción:**
El Administrador o el Auditor genera reportes operativos y de cumplimiento del sistema OCR, incluyendo métricas de procesamiento, precisión, volumen, tiempos y trazabilidad.

**Precondiciones:**
- El usuario ha iniciado sesión con rol `Administrador` o `Auditor`.
- Existen datos de procesamiento en el sistema.

**Flujo principal:**

| Paso | Acción |
|------|--------|
| 1 | El usuario accede al módulo de reportes desde el menú principal. |
| 2 | El sistema muestra los tipos de reporte disponibles. |
| 3 | El usuario selecciona el tipo de reporte deseado. |
| 4 | El usuario configura los filtros: rango de fechas, tipo de documento, estado, usuario. |
| 5 | El sistema genera el reporte con datos actualizados. |
| 6 | El sistema muestra el reporte en pantalla con gráficos y tablas. |
| 7 | El usuario puede exportar el reporte en formato PDF, Excel o CSV. |
| 8 | El sistema registra la generación del reporte en la pista de auditoría. |

**Tipos de reporte disponibles:**

| Reporte | Descripción | Roles |
|---------|-------------|-------|
| Volumen de procesamiento | Documentos procesados por día/semana/mes | Administrador, Auditor |
| Precisión OCR | Tasa de acierto global y por tipo de documento | Administrador, Auditor |
| Tiempos de procesamiento | Tiempo promedio, máximo, mínimo por documento | Administrador |
| Cola de revisión | Estado de la cola, tiempo promedio de revisión | Administrador, Revisor |
| Integración ERP | Envíos exitosos, rechazados, pendientes | Administrador, Auditor |
| Trazabilidad | Historial completo de un documento específico | Auditor |
| Cumplimiento GDPR | Datos personales procesados, retención, eliminación | Auditor |

**Flujos alternativos:**

| ID | Condición | Acción |
|----|-----------|--------|
| FA-07.1 | No hay datos para el rango seleccionado | El sistema muestra mensaje indicando que no hay datos y sugiere ampliar el rango. |
| FA-07.2 | Reporte con volumen muy grande | El sistema genera el reporte en segundo plano y notifica al usuario cuando esté listo. |

**Postcondiciones:**
- El reporte ha sido generado y mostrado al usuario.
- El reporte exportado está disponible para descarga.
- La generación del reporte está registrada en la pista de auditoría.

---

### CU-08: Configurar Sistema OCR

| Campo | Detalle |
|-------|---------|
| **ID** | CU-08 |
| **Nombre** | Configurar Sistema OCR |
| **Actor principal** | Administrador |
| **Actores secundarios** | - |
| **Prioridad** | Media |
| **Complejidad** | Alta |

**Descripción:**
El Administrador configura los parámetros del sistema OCR, incluyendo umbrales de confianza, reglas de validación, plantillas de extracción, integración ERP y parámetros de los motores OCR.

**Precondiciones:**
- El Administrador ha iniciado sesión con el rol `Administrador`.
- El sistema está operativo.

**Flujo principal:**

| Paso | Acción |
|------|--------|
| 1 | El Administrador accede al módulo de configuración. |
| 2 | El sistema muestra las categorías de configuración disponibles. |
| 3 | El Administrador selecciona la categoría a modificar. |
| 4 | El sistema muestra los parámetros actuales con sus valores. |
| 5 | El Administrador modifica los valores deseados. |
| 6 | El sistema valida los nuevos valores (rangos permitidos, formatos). |
| 7 | El Administrador confirma los cambios. |
| 8 | El sistema aplica la nueva configuración y registra el cambio en auditoría. |
| 9 | El sistema muestra confirmación de que la configuración ha sido actualizada. |

**Categorías de configuración:**

| Categoría | Parámetros |
|-----------|------------|
| Motores OCR | Motor principal, motor de respaldo, idiomas, DPI mínimo |
| Umbrales | Confianza mínima aprobación (default 70%), confianza fallback (default 60%) |
| Validación | Reglas por tipo de documento, campos obligatorios, expresiones regulares |
| Integración ERP | URL endpoint OData, credenciales, timeout, reintentos |
| Almacenamiento | Retención de documentos, política de cifrado, cuotas |
| Notificaciones | Eventos que disparan notificación, canales (email, in-app) |

**Flujos alternativos:**

| ID | Condición | Acción |
|----|-----------|--------|
| FA-08.1 | Valor fuera de rango permitido | El sistema rechaza el valor y muestra el rango aceptado. |
| FA-08.2 | Configuración ERP inválida | El sistema ejecuta una prueba de conexión y muestra el resultado antes de guardar. |
| FA-08.3 | Configuración puede causar impacto operativo | El sistema advierte al Administrador del impacto potencial y solicita confirmación explícita. |

**Postcondiciones:**
- La configuración del sistema está actualizada.
- Los cambios de configuración están registrados en la pista de auditoría con valores anteriores y nuevos.
- Los cambios aplican inmediatamente o al siguiente ciclo de procesamiento según la categoría.

---

### CU-09: Consultar Pista de Auditoría

| Campo | Detalle |
|-------|---------|
| **ID** | CU-09 |
| **Nombre** | Consultar Pista de Auditoría |
| **Actor principal** | Auditor |
| **Actores secundarios** | Administrador |
| **Prioridad** | Alta |
| **Complejidad** | Baja |

**Descripción:**
El Auditor o el Administrador consulta el registro completo de eventos del sistema para verificar el cumplimiento normativo, investigar incidentes o generar reportes de trazabilidad.

**Precondiciones:**
- El usuario ha iniciado sesión con rol `Auditor` o `Administrador`.
- Existen registros de auditoría en el sistema.

**Flujo principal:**

| Paso | Acción |
|------|--------|
| 1 | El usuario accede al módulo de auditoría. |
| 2 | El sistema muestra el panel de búsqueda de eventos de auditoría. |
| 3 | El usuario configura los filtros: rango de fechas, tipo de evento, usuario, documento. |
| 4 | El sistema ejecuta la búsqueda y muestra los resultados paginados. |
| 5 | El usuario selecciona un evento para ver el detalle completo. |
| 6 | El sistema muestra toda la información del evento: timestamp, usuario, acción, datos antes/después, dirección IP. |
| 7 | El usuario puede exportar los resultados de búsqueda en formato CSV o PDF. |

**Tipos de evento auditados:**

| Evento | Descripción |
|--------|-------------|
| `DOC_UPLOAD` | Carga de documento |
| `OCR_PROCESS` | Procesamiento OCR (motor, confianza, tiempo) |
| `DOC_REVIEW` | Revisión de documento (cambios realizados) |
| `DOC_VALIDATE` | Resultado de validación |
| `ERP_SYNC` | Envío a ERP (éxito, error, reintento) |
| `CONFIG_CHANGE` | Cambio de configuración (valores antes/después) |
| `USER_LOGIN` | Inicio de sesión |
| `USER_ACTION` | Acciones de usuario relevantes |
| `SYSTEM_ERROR` | Errores del sistema |

**Flujos alternativos:**

| ID | Condición | Acción |
|----|-----------|--------|
| FA-09.1 | Búsqueda sin resultados | El sistema indica que no hay eventos para los filtros seleccionados. |
| FA-09.2 | Volumen de resultados muy grande | El sistema limita la vista a 1000 registros y ofrece exportación completa en segundo plano. |

**Postcondiciones:**
- El usuario ha consultado los registros de auditoría requeridos.
- Los registros de auditoría son inmutables (solo lectura).
- La propia consulta de auditoría queda registrada.

---

### CU-10: Procesamiento Batch de Documentos

| Campo | Detalle |
|-------|---------|
| **ID** | CU-10 |
| **Nombre** | Procesamiento Batch de Documentos |
| **Actor principal** | Operador / Administrador |
| **Actores secundarios** | Motor OCR |
| **Prioridad** | Media |
| **Complejidad** | Alta |

**Descripción:**
El sistema permite la carga y procesamiento masivo de documentos en lotes (batch). El procesamiento se ejecuta en segundo plano con seguimiento de progreso en tiempo real y notificación al completar.

**Precondiciones:**
- El usuario tiene rol `Operador` o `Administrador`.
- Los documentos del lote cumplen con los formatos soportados.
- El sistema tiene capacidad de procesamiento disponible.

**Flujo principal:**

| Paso | Acción |
|------|--------|
| 1 | El usuario accede al módulo de procesamiento batch. |
| 2 | El usuario selecciona una carpeta o múltiples archivos (hasta 500 documentos). |
| 3 | El sistema valida todos los archivos del lote. |
| 4 | El usuario configura parámetros del batch: prioridad, tipo de documento (si es uniforme), notificaciones. |
| 5 | El sistema crea el trabajo batch con un identificador único. |
| 6 | El sistema inicia el procesamiento en segundo plano, distribuyendo documentos en workers paralelos. |
| 7 | El sistema actualiza el progreso en tiempo real (porcentaje, documentos procesados/total). |
| 8 | Al completar, el sistema genera un resumen del batch: exitosos, con errores, enviados a revisión manual. |
| 9 | El sistema notifica al usuario del resultado vía email e in-app. |

**Flujos alternativos:**

| ID | Condición | Acción |
|----|-----------|--------|
| FA-10.1 | Algunos archivos del lote son inválidos | El sistema procesa los válidos y genera reporte de archivos rechazados. |
| FA-10.2 | El usuario cancela el batch en curso | El sistema detiene el procesamiento, mantiene los documentos ya procesados y marca el batch como `cancelado`. |
| FA-10.3 | Error en un documento individual | El sistema continúa con los demás documentos y marca el fallido para revisión. |
| FA-10.4 | Capacidad del sistema insuficiente | El sistema encola el batch y lo ejecuta cuando haya recursos disponibles. Notifica el tiempo estimado. |

**Postcondiciones:**
- Todos los documentos del lote han sido procesados o marcados con error.
- El resumen del batch está disponible para consulta.
- Cada documento individual tiene su propio registro de auditoría.

---

### CU-11: Gestionar Usuarios y Roles

| Campo | Detalle |
|-------|---------|
| **ID** | CU-11 |
| **Nombre** | Gestionar Usuarios y Roles |
| **Actor principal** | Administrador |
| **Actores secundarios** | - |
| **Prioridad** | Media |
| **Complejidad** | Baja |

**Descripción:**
El Administrador gestiona los usuarios del sistema y sus roles asignados (Operador, Revisor, Administrador, Auditor). Incluye creación, modificación, desactivación de usuarios y asignación de permisos.

**Precondiciones:**
- El Administrador ha iniciado sesión con el rol `Administrador`.

**Flujo principal:**

| Paso | Acción |
|------|--------|
| 1 | El Administrador accede al módulo de gestión de usuarios. |
| 2 | El sistema muestra la lista de usuarios con su estado y roles. |
| 3 | El Administrador selecciona la acción: crear, editar o desactivar usuario. |
| 4 | Para crear: ingresa datos del usuario (nombre, email, rol). |
| 5 | El sistema valida los datos y crea el usuario con credenciales OAuth 2.0. |
| 6 | El sistema envía notificación al nuevo usuario con instrucciones de acceso. |
| 7 | El sistema registra la acción en la pista de auditoría. |

**Flujos alternativos:**

| ID | Condición | Acción |
|----|-----------|--------|
| FA-11.1 | Email ya registrado | El sistema rechaza la creación e informa al Administrador. |
| FA-11.2 | Desactivar último Administrador | El sistema impide la acción para garantizar que siempre exista al menos un Administrador activo. |

**Postcondiciones:**
- El usuario ha sido creado, modificado o desactivado según la acción.
- La acción está registrada en la pista de auditoría.

---

### CU-12: Clasificar Documento Automáticamente

| Campo | Detalle |
|-------|---------|
| **ID** | CU-12 |
| **Nombre** | Clasificar Documento Automáticamente |
| **Actor principal** | Sistema (automático) |
| **Actores secundarios** | Motor OCR |
| **Prioridad** | Alta |
| **Complejidad** | Alta |

**Descripción:**
El sistema clasifica automáticamente cada documento cargado según su tipo (factura, recibo, orden de compra, bitácora operativa, nota de entrega) utilizando análisis de contenido y patrones.

**Precondiciones:**
- El documento ha sido cargado exitosamente.
- Las plantillas de clasificación están configuradas.

**Flujo principal:**

| Paso | Acción |
|------|--------|
| 1 | El sistema recibe un documento recién cargado. |
| 2 | El sistema realiza un análisis preliminar del contenido (encabezados, palabras clave, estructura). |
| 3 | El sistema compara el contenido contra las plantillas de clasificación configuradas. |
| 4 | El sistema asigna un tipo de documento con un puntaje de confianza. |
| 5 | Si la confianza de clasificación es >= 85%, se acepta automáticamente. |
| 6 | El sistema asocia las reglas de extracción y validación correspondientes al tipo detectado. |
| 7 | El sistema registra la clasificación en los metadatos del documento. |

**Flujos alternativos:**

| ID | Condición | Acción |
|----|-----------|--------|
| FA-12.1 | Confianza de clasificación < 85% | El sistema solicita al Operador que confirme o corrija el tipo de documento manualmente. |
| FA-12.2 | Tipo de documento no reconocido | El sistema clasifica como `tipo_desconocido` y envía a revisión manual con alerta al Administrador. |

**Postcondiciones:**
- El documento tiene un tipo asignado.
- Las reglas de extracción y validación correspondientes están asociadas.
- La clasificación está registrada en la pista de auditoría.

---

## Matriz de Trazabilidad

| Caso de Uso | RF asociados | RNF asociados | Roles | Prioridad |
|-------------|-------------|---------------|-------|-----------|
| CU-01: Cargar Documento | RF-01, RF-09 | RNF-01, RNF-03, RNF-04, RNF-08 | Operador | Alta |
| CU-02: Procesar Documento con OCR | RF-02, RF-09 | RNF-01, RNF-02, RNF-06, RNF-10 | Sistema, Operador | Alta |
| CU-03: Revisar Extracción de Datos | RF-06, RF-09 | RNF-05, RNF-07 | Revisor | Alta |
| CU-04: Validar Resultados Extraídos | RF-04, RF-09 | RNF-01, RNF-08 | Sistema, Revisor | Alta |
| CU-05: Enviar Datos a ERP/SAP | RF-05, RF-09 | RNF-01, RNF-02, RNF-04, RNF-08 | Sistema, Administrador | Alta |
| CU-06: Revisión Manual de Documentos | RF-06, RF-09 | RNF-05, RNF-07 | Revisor | Alta |
| CU-07: Generar Reportes | RF-07, RF-09 | RNF-05, RNF-07 | Administrador, Auditor | Media |
| CU-08: Configurar Sistema OCR | RF-08, RF-09 | RNF-04, RNF-06 | Administrador | Media |
| CU-09: Consultar Pista de Auditoría | RF-09 | RNF-04, RNF-08, RNF-09 | Auditor, Administrador | Alta |
| CU-10: Procesamiento Batch | RF-10, RF-01, RF-02, RF-09 | RNF-01, RNF-03, RNF-10 | Operador, Administrador | Media |
| CU-11: Gestionar Usuarios y Roles | RF-08, RF-09 | RNF-04, RNF-05 | Administrador | Media |
| CU-12: Clasificar Documento | RF-03, RF-09 | RNF-01, RNF-06 | Sistema | Alta |

---

*Documento generado el 2026-02-24 por IDC Ingeniería.*
