# ADR-0002: Seleccion de Google Cloud Vision API como Motor OCR Primario

**Estado:** Aceptada
**Fecha:** 2026-02-24
**Autor:** IDC Ingenieria
**Proyecto:** OCR Operativo - Reconocimiento Optico de Caracteres

---

## Contexto

El nucleo del proyecto OCR Operativo es la capacidad de extraer texto de documentos escaneados, fotografias de documentos fisicos y PDFs digitalizados con una precision minima del 95%. Los documentos a procesar incluyen facturas, ordenes de compra, remisiones, certificados y documentos tecnicos de ingenieria, muchos de los cuales contienen texto en espanol con caracteres especiales (tildes, ene), tablas, logotipos y sellos que dificultan el reconocimiento.

Se requiere un motor OCR que sea preciso, rapido (< 5 segundos por documento), escalable para manejar picos de carga (1.000+ documentos/dia) y que se integre de forma natural con la infraestructura existente en GCP. Adicionalmente, se necesita una estrategia de fallback para garantizar disponibilidad ante posibles interrupciones del servicio principal.

## Decision

Se adopta **Google Cloud Vision API** como motor OCR primario y **Tesseract OCR 5.x** como motor de fallback local. La arquitectura implementa un patron de fallback automatico con las siguientes reglas:

1. **Flujo primario:** Todo documento se procesa primero con Google Cloud Vision API (endpoint `TEXT_DETECTION` para texto simple, `DOCUMENT_TEXT_DETECTION` para documentos estructurados).
2. **Fallback automatico:** Si Google Cloud Vision no esta disponible (timeout > 10s, error 5xx, o cuota excedida), el sistema redirige automaticamente a Tesseract OCR local.
3. **Validacion cruzada:** Para documentos criticos (facturas, ordenes de compra), se ejecutan ambos motores y se comparan resultados para maximizar la precision.
4. **Pre-procesamiento unificado:** Antes de enviar a cualquier motor, las imagenes pasan por un pipeline de pre-procesamiento (binarizacion, deskew, eliminacion de ruido) usando OpenCV.

## Alternativas Consideradas

### 1. Tesseract OCR como motor unico
| Aspecto | Evaluacion |
|---|---|
| Costo | Gratuito, open source (Apache 2.0) |
| Precision base | 70-85% en documentos tipicos sin pre-procesamiento |
| Precision con pre-procesamiento | 80-90% con pipeline optimizado |
| Soporte de espanol | Bueno, con modelos de idioma entrenados (spa.traineddata) |
| Documentos estructurados | Limitado; no detecta tablas, formularios ni layout de forma nativa |
| Escalabilidad | Depende de CPU local; requiere infraestructura propia para escalar |
| Latencia | 2-8 segundos dependiendo de complejidad y hardware |

**Descartado como motor primario:** La precision base no alcanza el objetivo del 95%, especialmente en documentos con layouts complejos, sellos, logotipos superpuestos o baja calidad de escaneo. Sin embargo, se retiene como fallback por su costo cero y operacion offline.

### 2. Amazon Textract
| Aspecto | Evaluacion |
|---|---|
| Precision | 95%+ en documentos en ingles; 90-93% en espanol |
| Extraccion estructurada | Excelente; detecta tablas, formularios y pares clave-valor nativamente |
| Costo | $1.50 por 1.000 paginas (deteccion de texto), $15 por 1.000 paginas (tablas/formularios) |
| Integracion con GCP | Requiere comunicacion cross-cloud, aumentando latencia y complejidad |
| Latencia | 3-7 segundos por pagina |

**Descartado:** La integracion cross-cloud (AWS desde GCP) anade complejidad operativa, latencia adicional y riesgo de transferencia de datos entre proveedores. La precision en espanol es inferior a Google Cloud Vision.

### 3. Azure Computer Vision / Azure AI Document Intelligence
| Aspecto | Evaluacion |
|---|---|
| Precision | 93-96% en documentos multiidioma |
| Modelos preentrenados | Excelentes para facturas, recibos y documentos de identidad |
| Costo | $1.00 por 1.000 imagenes (OCR), $10 por 1.000 paginas (Document Intelligence) |
| Integracion con GCP | Misma problematica cross-cloud que Textract |
| Latencia | 2-5 segundos por pagina |

**Descartado:** Pese a tener modelos preentrenados relevantes (facturas), la penalizacion por operacion cross-cloud y la fragmentacion de la infraestructura en dos proveedores cloud no justifican la diferencia marginal en capacidades.

### 4. Solucion hibrida multi-cloud (mejor motor segun tipo de documento)
| Aspecto | Evaluacion |
|---|---|
| Precision potencial | Maxima (97%+) al seleccionar el mejor motor por tipo de documento |
| Complejidad operativa | Muy alta; multiples SDKs, contratos, facturaciones y puntos de fallo |
| Costo | Variable e impredecible; dificil de presupuestar |
| Mantenimiento | Requiere expertise en multiples plataformas cloud |

**Descartado:** La complejidad operativa y de mantenimiento supera los beneficios marginales de precision. Se reserva como evolucion futura si se requiere precision > 97% en tipos de documento especificos.

## Justificacion

1. **Precision superior en espanol:** Google Cloud Vision API demuestra consistentemente una precision del 95-98% en documentos en espanol, incluyendo caracteres especiales (tildes, ene, signos de apertura), superando a las alternativas evaluadas en este idioma especifico.

2. **Integracion nativa con la infraestructura:** El proyecto ya opera sobre GCP (Kubernetes en GKE, Cloud Storage para documentos). Google Cloud Vision se integra sin latencia adicional de red, con autenticacion via service accounts existentes y sin transferencia de datos fuera de la plataforma.

3. **Deteccion de layout y estructura:** `DOCUMENT_TEXT_DETECTION` proporciona informacion de bloques, parrafos, palabras y simbolos con coordenadas espaciales, permitiendo reconstruir la estructura del documento (tablas, columnas, encabezados) mediante logica de post-procesamiento.

4. **Modelo de costos predecible:** A $1.50 por 1.000 unidades de imagen, el costo para 1.000 documentos/dia (asumiendo 3 paginas promedio) es aproximadamente $135/mes, dentro del presupuesto operativo del proyecto.

5. **Estrategia de fallback robusta:** Tesseract como motor local garantiza que el sistema no se detenga ante interrupciones de Google Cloud Vision, cumpliendo los requisitos de disponibilidad aunque con precision reducida temporalmente.

6. **Latencia competitiva:** Tiempos de respuesta de 1-3 segundos por pagina permiten cumplir holgadamente el objetivo de < 5 segundos de procesamiento end-to-end, incluyendo pre y post-procesamiento.

7. **Soporte de multiples formatos:** Acepta nativamente JPEG, PNG, GIF, BMP, WEBP, RAW, ICO, PDF y TIFF, cubriendo todos los formatos de entrada esperados del proyecto.

## Consecuencias

### Positivas
- Precision del 95%+ alcanzable desde el primer sprint de integracion, sin necesidad de entrenamiento custom.
- Reduccion de complejidad operativa al mantener toda la infraestructura en un solo proveedor cloud (GCP).
- Capacidad de escalado automatico gestionada por Google; sin necesidad de aprovisionar GPU o infraestructura OCR propia.
- Acceso a mejoras continuas del modelo sin esfuerzo de actualizacion (servicio gestionado).
- Pipeline de pre-procesamiento reutilizable para ambos motores (Vision API y Tesseract).

### Negativas
- Dependencia de un servicio externo de pago con posible variacion de precios futura.
- Los documentos se envian a servidores de Google para procesamiento, lo que puede generar preocupaciones de privacidad con documentos altamente confidenciales.
- La precision del fallback (Tesseract) es notablemente inferior (80-90% vs 95-98%), lo que genera una degradacion perceptible durante incidentes.
- No se tiene control sobre el modelo subyacente; cambios en el servicio por parte de Google podrian afectar resultados.

### Riesgos
- **Riesgo de costos no controlados:** Un aumento inesperado del volumen de documentos podria disparar los costos de la API. **Mitigacion:** Implementar alertas de presupuesto en GCP, cuotas de uso diario y cache de resultados en Redis para documentos reprocesados.
- **Riesgo de privacidad de datos:** Documentos sensibles salen de la infraestructura propia hacia Google. **Mitigacion:** Evaluar la necesidad de acuerdos de procesamiento de datos (DPA), usar la opcion de residencia de datos en region especifica y clasificar documentos por nivel de sensibilidad.
- **Riesgo de degradacion del servicio:** Google Cloud Vision podria experimentar latencia elevada o interrupciones. **Mitigacion:** Circuit breaker con fallback automatico a Tesseract, colas de reintento en Redis y alertas de monitoreo.
- **Riesgo de lock-in:** Migrar a otro motor OCR requeriria cambios en la capa de integracion. **Mitigacion:** Implementar una interfaz abstracta (`OCREngine`) que permita intercambiar motores con cambios minimos.

## Metricas de Validacion

| Metrica | Objetivo | Metodo de Medicion |
|---|---|---|
| Precision OCR (Google Vision) | >= 95% | Comparacion automatica contra ground truth en set de validacion de 200 documentos |
| Precision OCR (Tesseract fallback) | >= 85% | Mismo set de validacion |
| Latencia de procesamiento (Vision API) | < 3 segundos/pagina | Trazas con OpenTelemetry |
| Latencia end-to-end (incluyendo pre/post) | < 5 segundos/documento | Trazas con OpenTelemetry |
| Tasa de fallback a Tesseract | < 2% de documentos procesados | Contadores en metricas de aplicacion |
| Costo mensual de Vision API | < $200/mes (para 1.000 docs/dia) | Dashboard de facturacion de GCP |
| Disponibilidad efectiva del OCR | >= 99.9% (combinando ambos motores) | Monitoreo de uptime |

---
**Revisores:** [Pendiente]
**Aprobado por:** [Pendiente]
