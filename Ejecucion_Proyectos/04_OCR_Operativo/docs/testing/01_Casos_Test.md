# 01 - Casos de Test
## OCR Operativo - Reconocimiento Optico de Caracteres

**Proyecto:** OCR Operativo
**Version:** 1.0
**Fecha de Creacion:** 24 de Febrero de 2026
**Responsable QA:** Por asignar
**Estado General:** Pendiente de ejecucion
**Total de Casos:** 120

---

## Resumen por Area

| Area | Cantidad | Prioridad Critica | Prioridad Alta | Prioridad Media | Prioridad Baja |
|------|----------|--------------------|----------------|-----------------|----------------|
| Carga de Documentos | 15 | 4 | 6 | 3 | 2 |
| Procesamiento OCR | 20 | 8 | 7 | 3 | 2 |
| Clasificacion de Documentos | 10 | 3 | 4 | 2 | 1 |
| Validacion de Datos | 15 | 5 | 6 | 3 | 1 |
| Integracion ERP/SAP | 12 | 5 | 4 | 2 | 1 |
| Revision Manual | 10 | 3 | 4 | 2 | 1 |
| Rendimiento | 10 | 4 | 3 | 2 | 1 |
| Seguridad | 10 | 5 | 3 | 1 | 1 |
| Usabilidad | 8 | 1 | 3 | 3 | 1 |
| Edge Cases | 10 | 3 | 4 | 2 | 1 |
| **Total** | **120** | **41** | **44** | **23** | **12** |

---

## Convenciones

- **ID:** TC-XXXX (numeracion secuencial por area)
- **Prioridad:** Critica / Alta / Media / Baja
- **CA:** Criterio de Aceptacion asociado (referencia a doc 04_CRITERIOS_DE_EXITO)
- **Estado:** Pendiente / En Ejecucion / Pasado / Fallido / Bloqueado
- **Precondicion:** Condiciones necesarias antes de ejecutar el caso

---

## 1. Carga de Documentos (15 casos)

| ID | Descripcion | Precondicion | Resultado Esperado | Prioridad | CA Asociado | Estado |
|----|-------------|--------------|---------------------|-----------|-------------|--------|
| TC-0001 | Carga exitosa de factura en formato PDF | Usuario autenticado, sistema operativo | Documento almacenado en Cloud Storage, registro creado en BD, respuesta HTTP 201 | Critica | CA-H3: Endpoint POST /api/v1/documents/upload operativo | Pendiente |
| TC-0002 | Carga exitosa de recibo en formato JPG | Usuario autenticado | Imagen convertida y almacenada, registro en BD con metadata | Critica | CA-H3: Endpoint upload operativo | Pendiente |
| TC-0003 | Carga exitosa de orden de compra en formato PNG | Usuario autenticado | Documento procesado y almacenado correctamente | Alta | CA-H3: Endpoint upload operativo | Pendiente |
| TC-0004 | Carga exitosa de bitacora operativa en formato TIFF | Usuario autenticado | Conversion TIFF exitosa, documento almacenado | Alta | CA-H3: Endpoint upload operativo | Pendiente |
| TC-0005 | Carga de documento con tamano maximo permitido (50MB) | Usuario autenticado | Documento aceptado y procesado sin errores | Alta | CA-H7: Performance baseline | Pendiente |
| TC-0006 | Rechazo de documento que excede tamano maximo (>50MB) | Usuario autenticado | Error HTTP 413, mensaje descriptivo al usuario | Alta | CA-H3: Manejo de errores | Pendiente |
| TC-0007 | Rechazo de formato no soportado (.exe, .zip) | Usuario autenticado | Error HTTP 415, mensaje indicando formatos validos | Alta | CA-H3: Manejo de errores | Pendiente |
| TC-0008 | Carga simultanea de multiples documentos (batch de 10) | Usuario autenticado, sistema estable | Todos los documentos procesados, IDs unicos asignados | Critica | CA-H7: Capacidad 1000+ docs/dia | Pendiente |
| TC-0009 | Carga de documento con nombre conteniendo caracteres especiales | Usuario autenticado | Nombre sanitizado, documento almacenado correctamente | Media | CA-H3: Almacenamiento resultados en BD | Pendiente |
| TC-0010 | Carga de documento PDF multipagina (20+ paginas) | Usuario autenticado | Todas las paginas procesadas, texto extraido por pagina | Alta | CA-H3: Procesamiento OCR basico | Pendiente |
| TC-0011 | Carga de documento con conexion inestable (simulacion de timeout) | Usuario autenticado, herramienta de simulacion de red | Reintento automatico o mensaje de error apropiado, sin corrupcion | Critica | CA-H3: Manejo de errores y excepciones | Pendiente |
| TC-0012 | Carga de documento duplicado (mismo hash) | Documento ya existe en sistema | Sistema detecta duplicado, notifica al usuario, permite o rechaza segun config | Media | CA-H3: Almacenamiento resultados en BD | Pendiente |
| TC-0013 | Carga de documento con metadata EXIF completa | Usuario autenticado | Metadata extraida y almacenada junto al documento | Media | CA-H3: Almacenamiento resultados en BD | Pendiente |
| TC-0014 | Validacion de respuesta API al cargar documento (estructura JSON) | Usuario autenticado | Respuesta contiene: id, status, timestamp, filename, size, type | Baja | CA-H3: Documentacion API completa | Pendiente |
| TC-0015 | Carga de documento por usuario sin permisos de escritura | Usuario con rol solo lectura | Error HTTP 403, acceso denegado registrado en audit log | Baja | CA-H8: Audit trail completo | Pendiente |

---

## 2. Procesamiento OCR (20 casos)

| ID | Descripcion | Precondicion | Resultado Esperado | Prioridad | CA Asociado | Estado |
|----|-------------|--------------|---------------------|-----------|-------------|--------|
| TC-0016 | Extraccion de texto de factura con texto claro y legible | Documento cargado en sistema | Texto extraido con precision >= 95%, campos identificados | Critica | CA-H4: Precision OCR >= 95% | Pendiente |
| TC-0017 | Extraccion de texto de recibo con calidad media | Documento cargado | Texto extraido con precision >= 90%, confidence score calculado | Critica | CA-H4: Confidence scores calibrados | Pendiente |
| TC-0018 | Extraccion via Google Cloud Vision como motor primario | Documento cargado, Vision API activa | Respuesta de Vision API procesada, campos mapeados correctamente | Critica | CA-H2: Vision API habilitada | Pendiente |
| TC-0019 | Fallback a Tesseract cuando Vision API no disponible | Vision API inaccesible (simulado) | Tesseract procesa documento, resultado almacenado con flag de fallback | Critica | CA-H3: Procesamiento OCR basico (Tesseract) | Pendiente |
| TC-0020 | Procesamiento de documento en espanol con tildes y ene | Documento en espanol cargado | Caracteres especiales (a, e, i, o, u con tilde, n) correctamente extraidos | Critica | CA-H4: Precision OCR >= 95% | Pendiente |
| TC-0021 | Procesamiento de documento bilingue (espanol/ingles) | Documento bilingue cargado | Ambos idiomas detectados y texto extraido correctamente | Alta | CA-H4: Precision OCR >= 95% | Pendiente |
| TC-0022 | Extraccion de campos numericos (montos, cantidades, fechas) | Factura cargada | Numeros, montos con decimales y fechas extraidas sin errores de formato | Critica | CA-H4: Precision OCR >= 95% | Pendiente |
| TC-0023 | Extraccion de tablas estructuradas en facturas | Factura con tabla de items cargada | Filas y columnas correctamente identificadas y estructuradas | Critica | CA-H4: Post-processing especifico por tipo | Pendiente |
| TC-0024 | Procesamiento de imagen con rotacion (90, 180, 270 grados) | Documento rotado cargado | Preprocesamiento corrige rotacion, texto extraido correctamente | Alta | CA-H3: Servicio de preprocesamiento de imagen | Pendiente |
| TC-0025 | Procesamiento de imagen con baja resolucion (72 DPI) | Imagen baja resolucion cargada | Sistema mejora imagen o reporta calidad insuficiente con score | Alta | CA-H3: Servicio de preprocesamiento de imagen | Pendiente |
| TC-0026 | Procesamiento de documento escaneado con manchas/ruido | Documento con ruido cargado | Preprocesamiento reduce ruido, texto legible extraido | Alta | CA-H3: Servicio de preprocesamiento de imagen | Pendiente |
| TC-0027 | Calculo correcto de confidence score por campo extraido | Documento procesado | Score de confianza entre 0-100% asignado a cada campo, media > 92% | Critica | CA-H4: Confidence scores calibrados por tipo | Pendiente |
| TC-0028 | Procesamiento de orden de compra con campos especificos | Orden de compra cargada | Campos: numero OC, proveedor, items, total, fecha extraidos | Alta | CA-H4: Post-processing especifico por tipo | Pendiente |
| TC-0029 | Procesamiento de bitacora operativa con formato libre | Bitacora cargada | Texto libre extraido, campos clave identificados donde sea posible | Alta | CA-H4: Post-processing especifico por tipo | Pendiente |
| TC-0030 | Verificacion de correccion ortografica post-OCR | Documento procesado | Hunspell/LanguageTool corrige errores tipicos de OCR sin alterar datos | Media | CA-H4: Correcciones ortograficas | Pendiente |
| TC-0031 | Procesamiento paralelo de 10 documentos simultaneos | 10 documentos en cola | Todos procesados sin errores, tiempos dentro de SLA | Alta | CA-H7: Worker threads | Pendiente |
| TC-0032 | Generacion de resultado estructurado JSON post-OCR | Documento procesado | JSON con campos: text, confidence, bounding_boxes, metadata | Media | CA-H3: Almacenamiento resultados en BD | Pendiente |
| TC-0033 | Almacenamiento correcto de resultados OCR en PostgreSQL | Documento procesado | Registro completo en tabla documents con todos los campos | Alta | CA-H3: Almacenamiento resultados en BD | Pendiente |
| TC-0034 | Logging de todo el proceso OCR (inicio, pasos, fin) | Documento en procesamiento | Logs estructurados con timestamps, duracion por paso, resultado | Media | CA-H3: Logging estructurado | Pendiente |
| TC-0035 | Reprocesamiento de documento cuando falla OCR inicial | Documento con error de procesamiento | Reintento automatico (hasta 3 intentos), resultado final registrado | Baja | CA-H3: Manejo de errores y excepciones | Pendiente |

---

## 3. Clasificacion de Documentos (10 casos)

| ID | Descripcion | Precondicion | Resultado Esperado | Prioridad | CA Asociado | Estado |
|----|-------------|--------------|---------------------|-----------|-------------|--------|
| TC-0036 | Clasificacion automatica de factura | Factura procesada por OCR | Tipo = "factura" asignado con confidence > 90% | Critica | CA-H4: Modelo ML de clasificacion | Pendiente |
| TC-0037 | Clasificacion automatica de recibo | Recibo procesado por OCR | Tipo = "recibo" asignado con confidence > 90% | Critica | CA-H4: Modelo ML de clasificacion | Pendiente |
| TC-0038 | Clasificacion automatica de orden de compra | OC procesada por OCR | Tipo = "orden_compra" asignado con confidence > 90% | Critica | CA-H4: Modelo ML de clasificacion | Pendiente |
| TC-0039 | Clasificacion automatica de bitacora operativa | Bitacora procesada por OCR | Tipo = "bitacora" asignado con confidence > 85% | Alta | CA-H4: Modelo ML de clasificacion | Pendiente |
| TC-0040 | Clasificacion de documento con tipo ambiguo | Documento ambiguo procesado | Documento marcado con confidence < 70%, enviado a revision manual | Alta | CA-H6: <5% documentos requieren revision manual | Pendiente |
| TC-0041 | Clasificacion de documento en tipo no registrado | Documento de tipo desconocido | Tipo = "otro" asignado, alerta generada, enviado a revision | Alta | CA-H4: 95% casos edge manejados | Pendiente |
| TC-0042 | Verificacion de precision del modelo de clasificacion (500 docs) | Dataset de 500 documentos etiquetados | Precision global >= 95%, recall >= 93% por categoria | Alta | CA-H4: Validacion cruzada (5-fold) | Pendiente |
| TC-0043 | Reclasificacion manual de documento mal clasificado | Documento clasificado incorrectamente | Usuario puede reclasificar, feedback almacenado para reentrenamiento | Media | CA-H6: Feedback loop para mejorar OCR | Pendiente |
| TC-0044 | Clasificacion correcta de documento con multiples idiomas | Documento multilingue procesado | Tipo correcto asignado independientemente del idioma | Media | CA-H4: Precision OCR >= 95% | Pendiente |
| TC-0045 | Verificacion de que clasificacion no bloquea pipeline si falla | Error en modelo de clasificacion | Documento continua al siguiente paso con tipo "sin_clasificar" | Baja | CA-H3: Manejo de errores y excepciones | Pendiente |

---

## 4. Validacion de Datos (15 casos)

| ID | Descripcion | Precondicion | Resultado Esperado | Prioridad | CA Asociado | Estado |
|----|-------------|--------------|---------------------|-----------|-------------|--------|
| TC-0046 | Validacion de formato de RUT/NIF en facturas | Factura procesada con RUT extraido | RUT validado contra algoritmo de verificacion, flag si invalido | Critica | CA-H4: Post-processing especifico | Pendiente |
| TC-0047 | Validacion de formato de fecha (DD/MM/YYYY, YYYY-MM-DD) | Documento con fechas extraidas | Fechas normalizadas a formato ISO 8601 | Alta | CA-H4: Post-processing especifico | Pendiente |
| TC-0048 | Validacion de consistencia de montos (subtotal + IVA = total) | Factura con montos extraidos | Calculo verificado, discrepancias reportadas como alerta | Critica | CA-H4: Post-processing especifico | Pendiente |
| TC-0049 | Validacion de campos obligatorios en factura | Factura procesada | Campos: emisor, receptor, fecha, numero, monto total presentes | Critica | CA-H5: Mapeo campos validado | Pendiente |
| TC-0050 | Validacion de campos obligatorios en orden de compra | OC procesada | Campos: numero OC, proveedor, items, total, fecha validados | Critica | CA-H5: Mapeo campos validado | Pendiente |
| TC-0051 | Deteccion de campos faltantes y envio a revision manual | Documento con campos incompletos | Documento marcado con lista de campos faltantes, enviado a cola | Alta | CA-H6: Workflow aprobacion funcional | Pendiente |
| TC-0052 | Validacion de formato de moneda (CLP, USD, EUR) | Documento con montos | Moneda detectada y normalizada, formato consistente | Alta | CA-H4: Post-processing especifico | Pendiente |
| TC-0053 | Validacion de numero de factura contra patron esperado | Factura procesada | Formato de numero validado contra regex configurable | Alta | CA-H4: Post-processing especifico | Pendiente |
| TC-0054 | Validacion de datos de proveedor contra maestro de proveedores | Factura con datos de proveedor | Proveedor verificado contra base maestra de SAP | Critica | CA-H5: Mapeo campos validado contra requerimientos | Pendiente |
| TC-0055 | Deteccion de valores atipicos (outliers) en montos | Factura con monto inusual | Alerta generada si monto excede umbral configurable | Alta | CA-H4: Post-processing especifico | Pendiente |
| TC-0056 | Validacion de campos numericos (no negativos donde no aplica) | Documento con numeros extraidos | Cantidades y montos validados como positivos, alertas si negativos | Alta | CA-H4: Post-processing especifico | Pendiente |
| TC-0057 | Validacion de direcciones y datos de contacto | Documento con datos de contacto | Formato de email, telefono y direccion verificados | Media | CA-H4: Post-processing especifico | Pendiente |
| TC-0058 | Validacion cruzada entre campos relacionados | Documento procesado | Consistencia entre proveedor-RUT, fecha-periodo, etc. verificada | Media | CA-H4: Validacion cruzada | Pendiente |
| TC-0059 | Transformacion de datos pre-envio a SAP | Documento validado | Datos transformados al formato requerido por SAP sin perdida | Media | CA-H5: Transformacion datos pre-envio | Pendiente |
| TC-0060 | Reporte de errores de validacion con detalle por campo | Documento con errores | JSON con lista de errores: campo, valor_encontrado, regla_violada, severidad | Baja | CA-H3: Logging estructurado | Pendiente |

---

## 5. Integracion ERP/SAP (12 casos)

| ID | Descripcion | Precondicion | Resultado Esperado | Prioridad | CA Asociado | Estado |
|----|-------------|--------------|---------------------|-----------|-------------|--------|
| TC-0061 | Sincronizacion exitosa de factura a SAP (modulo AP) | Factura validada y aprobada | Documento creado en SAP AP, ID SAP retornado y almacenado | Critica | CA-H5: Conector SAP funcional | Pendiente |
| TC-0062 | Sincronizacion exitosa de orden de compra a SAP (modulo MM) | OC validada | Documento creado en SAP MM, referencia cruzada almacenada | Critica | CA-H5: Conector SAP funcional | Pendiente |
| TC-0063 | Mapeo correcto de campos OCR a campos SAP | Documento procesado y validado | Todos los campos mapeados segun matriz de correspondencia definida | Critica | CA-H5: Mapeo campos validado | Pendiente |
| TC-0064 | Manejo de error de conexion con SAP (timeout) | SAP inaccesible (simulado) | Reintento con exponential backoff, maximo 5 intentos, log de error | Critica | CA-H5: Reintentos implementado | Pendiente |
| TC-0065 | Envio a dead-letter queue tras fallos persistentes | 5 reintentos fallidos | Documento movido a DLQ, alerta generada, no se pierde informacion | Critica | CA-H5: Dead-letter queue para fallos | Pendiente |
| TC-0066 | Registro completo en audit trail de sincronizacion | Documento sincronizado | Registro con: timestamp, doc_id, sap_id, status, user, duracion | Alta | CA-H5: Audit trail completo | Pendiente |
| TC-0067 | Validacion pre-envio a SAP (datos completos y formateados) | Documento listo para sincronizar | Validacion ejecutada, solo documentos validos enviados a SAP | Alta | CA-H5: Validacion pre-envio a SAP | Pendiente |
| TC-0068 | Reconciliacion automatica entre OCR y SAP | Documentos sincronizados | Reporte de reconciliacion: docs enviados vs creados en SAP, discrepancias | Alta | CA-H5: Reconciliacion automatica | Pendiente |
| TC-0069 | Sincronizacion masiva de 100 documentos | 100 documentos validados | Todos sincronizados en <30 minutos, 99%+ tasa exito | Alta | CA-H5: 500+ docs sincronizados | Pendiente |
| TC-0070 | Autenticacion correcta contra SAP (Basic Auth + mTLS) | Credenciales SAP configuradas | Conexion establecida, token valido obtenido, sesion segura | Media | CA-H5: Autenticacion SAP configurada | Pendiente |
| TC-0071 | Dashboard de estado de sincronizaciones | Documentos en distintos estados de sync | Dashboard muestra: pendientes, en proceso, exitosos, fallidos, en DLQ | Media | CA-H5: Dashboard sincronizacion | Pendiente |
| TC-0072 | Alerta automatica cuando tasa de error de sync > 5% | Errores de sincronizacion simulados | Alerta enviada a canales configurados (email, Slack), incidente creado | Baja | CA-H5: Alertas errores sincronizacion | Pendiente |

---

## 6. Revision Manual (10 casos)

| ID | Descripcion | Precondicion | Resultado Esperado | Prioridad | CA Asociado | Estado |
|----|-------------|--------------|---------------------|-----------|-------------|--------|
| TC-0073 | Visualizacion de documento + datos extraidos en UI de revision | Documento en cola de revision | UI muestra imagen del documento lado a lado con campos extraidos | Critica | CA-H6: UI revision manual construida | Pendiente |
| TC-0074 | Correccion inline de campo extraido incorrecto | Documento en revision | Revisor edita campo, cambio guardado, historial de correccion registrado | Critica | CA-H6: Herramientas correccion inline | Pendiente |
| TC-0075 | Aprobacion de documento revisado | Documento corregido | Estado cambia a "aprobado", documento continua pipeline a SAP | Critica | CA-H6: Workflow aprobacion funcional | Pendiente |
| TC-0076 | Rechazo de documento con motivo | Documento en revision | Estado cambia a "rechazado", motivo registrado, notificacion generada | Alta | CA-H6: Flujo aprobacion/rechazo/correccion | Pendiente |
| TC-0077 | Notificacion a revisor cuando documento ingresa a cola | Nuevo documento en cola | Notificacion push/email enviada al revisor asignado | Alta | CA-H6: Notificaciones para revisores | Pendiente |
| TC-0078 | Auto-routing de documentos segun tipo y disponibilidad de revisor | Documentos en cola | Documentos asignados automaticamente a revisores disponibles segun tipo | Alta | CA-H6: Auto-routing de documentos | Pendiente |
| TC-0079 | Escalacion automatica si documento sin revisar > 5 minutos | Documento en cola sin atender | Escalacion a siguiente nivel, alerta generada | Alta | CA-H6: Reglas escalacion | Pendiente |
| TC-0080 | Cumplimiento de SLA de revision (< 2 horas) | Documentos en cola durante periodo | 95%+ documentos revisados dentro de 2 horas | Media | CA-H6: SLA revision cumplido (max 2h) | Pendiente |
| TC-0081 | Feedback loop: correccion manual mejora futuras extracciones | Multiples correcciones acumuladas | Datos de correccion almacenados para reentrenamiento del modelo | Media | CA-H6: Feedback loop implementado | Pendiente |
| TC-0082 | Metricas de productividad por revisor | Revisores activos con historial | Dashboard muestra: docs/hora, precision, tiempo promedio por revisor | Baja | CA-H6: Metricas productividad revisor | Pendiente |

---

## 7. Rendimiento (10 casos)

| ID | Descripcion | Precondicion | Resultado Esperado | Prioridad | CA Asociado | Estado |
|----|-------------|--------------|---------------------|-----------|-------------|--------|
| TC-0083 | Procesamiento de documento individual en < 5 segundos | Sistema estable, documento estandar | Latencia total (upload a resultado) < 5 segundos (P95) | Critica | CA-H7: Latencia < 5 segundos | Pendiente |
| TC-0084 | Procesamiento de 1000 documentos en un dia (8 horas) | Sistema en condiciones normales | 1000+ documentos procesados sin errores ni degradacion | Critica | CA-H7: Capacidad 1000+ docs/dia | Pendiente |
| TC-0085 | Test de carga con 50 usuarios concurrentes | Herramienta k6/JMeter configurada | API response time < 200ms (P95), sin errores HTTP 5xx | Critica | KPI: 50 usuarios concurrentes | Pendiente |
| TC-0086 | Medicion de latencias P50, P95, P99 bajo carga | Test de carga en ejecucion | P50 < 3s, P95 < 5s, P99 < 10s | Critica | CA-H7: Medicion latencias | Pendiente |
| TC-0087 | Utilizacion de CPU bajo carga pico < 75% | Test de carga en ejecucion | CPU nunca excede 75% en ningun nodo del cluster | Alta | CA-H7: CPU < 75% bajo carga pico | Pendiente |
| TC-0088 | Utilizacion de memoria bajo carga pico < 80% | Test de carga en ejecucion | Memoria nunca excede 80% en ningun nodo | Alta | CA-H7: Memoria < 80% disponible | Pendiente |
| TC-0089 | Escalabilidad horizontal (agregar replicas bajo demanda) | Cluster Kubernetes operativo | Al agregar replicas, throughput aumenta proporcionalmente | Alta | CA-H7: Escalabilidad horizontal verificada | Pendiente |
| TC-0090 | Failover de base de datos sin perdida de datos | Simulacion de fallo de nodo primario BD | Failover automatico, 0 documentos perdidos, < 30s downtime | Media | CA-H7: Zero perdida datos en failover | Pendiente |
| TC-0091 | Recuperacion de worker tras fallo | Simulacion de crash de worker | Worker reiniciado automaticamente, documentos en cola reprocesados | Media | CA-H7: Recuperacion fallos worker | Pendiente |
| TC-0092 | Rendimiento de cache Redis para modelos OCR | Cache Redis configurado y activo | Segundo procesamiento de mismo tipo 50%+ mas rapido | Baja | CA-H7: Caching modelos OCR en Redis | Pendiente |

---

## 8. Seguridad (10 casos)

| ID | Descripcion | Precondicion | Resultado Esperado | Prioridad | CA Asociado | Estado |
|----|-------------|--------------|---------------------|-----------|-------------|--------|
| TC-0093 | Autenticacion OAuth 2.0 + JWT funcional | Sistema configurado con OAuth | Login exitoso genera JWT valido, endpoints protegidos rechazan sin token | Critica | CA-H8: OAuth 2.0 + JWT | Pendiente |
| TC-0094 | Autorizacion RBAC (admin, operador, revisor, lector) | Roles configurados | Cada rol accede solo a funcionalidades permitidas, resto denegado | Critica | CA-H8: Autorizacion RBAC (4+ roles) | Pendiente |
| TC-0095 | Encriptacion AES-256 de datos en reposo | Datos almacenados en BD y Storage | Verificacion de que datos almacenados estan cifrados, no legibles sin clave | Critica | CA-H8: Encriptacion AES-256 en reposo | Pendiente |
| TC-0096 | TLS 1.3 en todos los endpoints | Sistema desplegado | Todas las comunicaciones usan TLS 1.3, no se permiten versiones inferiores | Critica | CA-H8: TLS 1.3 en todos los endpoints | Pendiente |
| TC-0097 | Rate limiting funcional (1000 req/min por usuario) | Sistema operativo | Solicitudes que exceden limite reciben HTTP 429, sin afectar otros usuarios | Critica | CA-H8: Rate limiting | Pendiente |
| TC-0098 | Proteccion contra SQL injection | Endpoint con parametros de busqueda | Payloads maliciosos rechazados, queries parametrizadas verificadas | Alta | CA-H8: Revision codigo SQL injection, XSS | Pendiente |
| TC-0099 | Proteccion contra XSS en interfaz de revision | UI de revision manual | Scripts maliciosos en campos extraidos no se ejecutan, sanitizacion activa | Alta | CA-H8: Revision codigo SQL injection, XSS | Pendiente |
| TC-0100 | Audit trail completo de accesos y cambios | Multiples operaciones realizadas | Cada operacion registrada con: quien, que, cuando, desde_donde, resultado | Alta | CA-H8: Logging acceso completo | Pendiente |
| TC-0101 | Cumplimiento GDPR: derecho al olvido (eliminacion de datos) | Solicitud de eliminacion recibida | Datos del usuario y documentos asociados eliminados, confirmacion generada | Media | CA-H8: GDPR compliance | Pendiente |
| TC-0102 | Scan OWASP ZAP sin vulnerabilidades criticas | Sistema desplegado en staging | 0 vulnerabilidades criticas, < 5 medias, reporte generado | Baja | CA-H8: Scan vulnerabilidades < 5 issues criticas | Pendiente |

---

## 9. Usabilidad (8 casos)

| ID | Descripcion | Precondicion | Resultado Esperado | Prioridad | CA Asociado | Estado |
|----|-------------|--------------|---------------------|-----------|-------------|--------|
| TC-0103 | Interfaz de carga intuitiva (drag & drop + boton) | UI React desplegada | Usuario puede cargar documentos por drag & drop y boton, feedback visual | Critica | KPI: System Usability > 4.5 | Pendiente |
| TC-0104 | Dashboard de estado de documentos claro y funcional | Documentos en distintos estados | Dashboard muestra contadores, filtros, busqueda, estado por documento | Alta | KPI: User Satisfaction > 4.0 | Pendiente |
| TC-0105 | Mensajes de error comprensibles para el usuario final | Errores simulados en distintos flujos | Mensajes en espanol, sin jerga tecnica, con accion sugerida | Alta | CA-H3: Manejo de errores | Pendiente |
| TC-0106 | Tiempo de carga de paginas < 2 segundos | UI React desplegada | Todas las paginas principales cargan en < 2s, LCP < 2.5s | Alta | KPI: API Response Time < 200ms | Pendiente |
| TC-0107 | Navegacion consistente y breadcrumbs | UI React desplegada | Usuario puede navegar entre secciones sin confusion, ubicacion siempre visible | Media | KPI: System Usability > 4.5 | Pendiente |
| TC-0108 | Responsive design para tablets | UI accedida desde tablet | Interfaz usable en pantallas de 768px+, elementos no se solapan | Media | KPI: System Usability > 4.5 | Pendiente |
| TC-0109 | Accesibilidad basica (WCAG 2.1 nivel A) | UI React desplegada | Contraste adecuado, navegacion por teclado, alt texts en imagenes | Media | KPI: System Usability > 4.5 | Pendiente |
| TC-0110 | Soporte para atajos de teclado en revision manual | UI revision manual abierta | Revisor puede aprobar, rechazar, navegar con teclado sin usar raton | Baja | CA-H6: UI revision manual construida | Pendiente |

---

## 10. Edge Cases (10 casos)

| ID | Descripcion | Precondicion | Resultado Esperado | Prioridad | CA Asociado | Estado |
|----|-------------|--------------|---------------------|-----------|-------------|--------|
| TC-0111 | Documento completamente en blanco (pagina vacia) | Pagina en blanco cargada | Sistema detecta pagina vacia, no genera resultados OCR, notifica al usuario | Critica | CA-H4: 95% casos edge manejados | Pendiente |
| TC-0112 | Documento con texto manuscrito | Documento manuscrito cargado | Sistema intenta OCR, reporta confidence bajo, envia a revision manual | Critica | CA-H4: 95% casos edge manejados | Pendiente |
| TC-0113 | Documento con orientacion mixta (paginas vertical y horizontal) | Documento multipagina cargado | Cada pagina procesada con orientacion correcta detectada | Critica | CA-H4: 95% casos edge manejados | Pendiente |
| TC-0114 | Documento extremadamente largo (100+ paginas) | Documento grande cargado | Procesamiento completo sin timeout, paginacion en resultados | Alta | CA-H7: Performance baseline | Pendiente |
| TC-0115 | Documento con watermark/marca de agua | Documento con watermark cargado | OCR extrae texto util ignorando o marcando el watermark | Alta | CA-H4: 95% casos edge manejados | Pendiente |
| TC-0116 | Documento con multiples tipos de contenido (texto + imagenes + tablas) | Documento mixto cargado | Texto extraido de todas las secciones, tablas estructuradas | Alta | CA-H4: Post-processing especifico | Pendiente |
| TC-0117 | Documento PDF protegido con contrasena | PDF protegido cargado | Error descriptivo indicando que el PDF esta protegido, sugerir desproteger | Alta | CA-H3: Manejo de errores | Pendiente |
| TC-0118 | Documento corrupto o truncado | Archivo corrupto cargado | Error manejado gracefully, documento marcado como no procesable | Media | CA-H3: Manejo de errores y excepciones | Pendiente |
| TC-0119 | Documento con caracteres de idiomas no latinos (chino, arabe) | Documento con caracteres especiales | Sistema detecta idioma no soportado, notifica, no genera resultados erroneos | Media | CA-H4: 95% casos edge manejados | Pendiente |
| TC-0120 | Perdida de conexion durante procesamiento OCR | Fallo de red simulado durante OCR | Documento queda en estado "pendiente", reintento automatico al restaurar | Baja | CA-H3: Manejo de errores y excepciones | Pendiente |

---

## Trazabilidad: Resumen de Criterios de Aceptacion Cubiertos

| Criterio de Aceptacion | Casos de Test Asociados | Cobertura |
|------------------------|------------------------|-----------|
| CA-H2: Vision API habilitada | TC-0018 | 1 caso |
| CA-H3: Endpoint upload operativo | TC-0001 a TC-0015 | 15 casos |
| CA-H3: Procesamiento OCR basico | TC-0016 a TC-0035 | 20 casos |
| CA-H3: Almacenamiento resultados BD | TC-0009, TC-0012, TC-0013, TC-0032, TC-0033 | 5 casos |
| CA-H3: Manejo de errores | TC-0006, TC-0007, TC-0011, TC-0035, TC-0105, TC-0117, TC-0118, TC-0120 | 8 casos |
| CA-H4: Precision OCR >= 95% | TC-0016 a TC-0029, TC-0042 | 15 casos |
| CA-H4: Confidence scores calibrados | TC-0017, TC-0027, TC-0040 | 3 casos |
| CA-H4: Modelo ML clasificacion | TC-0036 a TC-0045 | 10 casos |
| CA-H4: 95% casos edge manejados | TC-0111 a TC-0120 | 10 casos |
| CA-H5: Conector SAP funcional | TC-0061 a TC-0072 | 12 casos |
| CA-H5: Mapeo campos validado | TC-0049, TC-0050, TC-0054, TC-0063 | 4 casos |
| CA-H6: UI revision manual | TC-0073 a TC-0082 | 10 casos |
| CA-H6: Workflow aprobacion | TC-0075, TC-0076, TC-0051 | 3 casos |
| CA-H7: Latencia < 5 segundos | TC-0083, TC-0086 | 2 casos |
| CA-H7: Capacidad 1000+ docs/dia | TC-0084 | 1 caso |
| CA-H7: CPU < 75%, Memoria < 80% | TC-0087, TC-0088 | 2 casos |
| CA-H8: OAuth 2.0, RBAC | TC-0093, TC-0094 | 2 casos |
| CA-H8: Encriptacion AES-256 | TC-0095 | 1 caso |
| CA-H8: TLS 1.3 | TC-0096 | 1 caso |
| CA-H8: GDPR compliance | TC-0101 | 1 caso |
| KPI: Usabilidad > 4.5 | TC-0103 a TC-0110 | 8 casos |

---

## Control de Versiones

| Version | Fecha | Autor | Cambios |
|---------|-------|-------|---------|
| 1.0 | 24/02/2026 | QA Lead | Creacion inicial con 120 casos de test |

---

**Nota:** Todos los casos de test se encuentran en estado "Pendiente" a la espera del inicio de la fase de ejecucion de pruebas. Los casos deben ejecutarse siguiendo el orden de prioridad: Critica > Alta > Media > Baja.
