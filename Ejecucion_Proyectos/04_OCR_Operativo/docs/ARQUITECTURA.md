# Arquitectura del Sistema — OCR Operativo v2

**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Cliente:** IDC Ingeniería / Confiabilidad Ingenio Pichichí (primer usuario final)
**Autor:** IDC Ingeniería
**Fecha:** Marzo 2026

---

## 1. Vista General

OCR Operativo es un **workflow n8n** que convierte Telegram en una interfaz operativa para dos procesos conectados:
- **Cargue OCR:** digitalización de registros operativos fotografiados → filas estructuradas en Google Sheets
- **Consulta conversacional:** preguntas en lenguaje natural respondidas con evidencia real de la hoja

### Diagrama de alto nivel

```
┌─────────────────────────────────────────────────────────┐
│                  TELEGRAM (Interface)                    │
│  Usuario → Foto / Texto / Botón                         │
└───────────────────┬─────────────────────────────────────┘
                    │ Webhook
┌───────────────────▼─────────────────────────────────────┐
│                  n8n Workflow                            │
│                                                          │
│  TelegramTrigger                                         │
│       │                                                  │
│  ResolverConversacion  ←── Estado por chatId            │
│  (control de modo: menu / ocr_wait_photo / chat_query)  │
│       │                                                  │
│  RouterMenu                                              │
│  ├── [ocr_prompt]   → PedirFoto                         │
│  ├── [chat_prompt]  → ActivarModoConsulta               │
│  ├── [ocr_process]  → Rama OCR ──────────────────┐      │
│  ├── [chat_query]   → Rama Consulta ────────────┐│      │
│  └── [menu]         → MostrarMenu               ││      │
│                                                  ││      │
│  RAMA OCR:                                       ││      │
│  NotificarProcesando → Analyze an image (Gemini) ││      │
│  → FormatearDatos → GuardarEnSheets              ││      │
│  → ResumenCarga → NotificarExito ────────────────┘│      │
│                                                   │      │
│  RAMA CONSULTA:                                   │      │
│  NotificarConsultando → ProcesarConsulta          │      │
│  → Message a model (Gemini + Sheets Tool)         │      │
│  → FormatearRespuestaConsulta → ResponderConsulta ┘      │
│                                                          │
└──────────┬────────────────────────┬─────────────────────┘
           │                        │
┌──────────▼───────────┐  ┌────────▼──────────────────────┐
│  Google Gemini API   │  │  Google Sheets                │
│  - gemini-2.5-flash  │  │  Spreadsheet:                 │
│  - Vision (OCR)      │  │  1YsDdmcT9m6TJEiz6W17XiLI... │
│  - Chat (consulta)   │  │  Pestaña: Registros_OCR       │
└──────────────────────┘  └───────────────────────────────┘
```

---

## 2. Componentes

### 2.1 Nodos de Entrada y Estado

| Nodo | Tipo | Responsabilidad |
|------|------|----------------|
| `TelegramTrigger` | Trigger | Escucha `message` y `callback_query`. Descarga binarios de fotos. |
| `ResolverConversacion` | Code | Control central: lee texto/botones/fotos, calcula `route`, gestiona estado por `chatId` via `getWorkflowStaticData`. |
| `RouterMenu` | Switch | Enruta a 5 salidas según route calculada. |

**Estado persistido por chatId:**
- `chatModes`: `menu` / `ocr_wait_photo` / `chat_query`
- `chatHistories`: últimos turnos de la conversación (para contexto en consultas)

### 2.2 Nodos de UX / Mensajería Inmediata

| Nodo | Función |
|------|---------|
| `MostrarMenu` | Puerta de entrada. Botones: "Subir registro" / "Consultar datos" |
| `PedirFoto` | Explica calidad esperada de la imagen |
| `ActivarModoConsulta` | Abre modo chat y muestra ejemplos de preguntas |
| `NotificarProcesando` | Confirmación inmediata mientras corre OCR |
| `NotificarConsultando` | Avisa que se está buscando con evidencia real |
| `NotificarExito` | Resumen del cargue en lenguaje operativo |

### 2.3 Rama OCR

| Nodo | Tipo | Responsabilidad |
|------|------|----------------|
| `Analyze an image` | Google Gemini (Vision) | Recibe la foto. Prompt exige JSON estricto con `documento` + `items[]`. Un foto puede producir múltiples filas. |
| `FormatearDatos` | Code | Convierte salida de Gemini en filas operativas: normaliza fechas, corrige códigos, unifica equipos por checks/lista, genera una fila por ítem, conserva `Respuesta_Cruda`. Marca `Estado_OCR = Requiere Revision` si parseo no es confiable. |
| `GuardarEnSheets` | Google Sheets | Append en `Registros_OCR`. Cada ítem = una fila independiente. |
| `ResumenCarga` | Code | Cuenta filas guardadas, detecta filas para revisión, calcula rango de fechas. |
| `NotificarExito` | Telegram | Devuelve resumen corto y útil al operador. |

### 2.4 Rama Consulta Conversacional

| Nodo | Tipo | Responsabilidad |
|------|------|----------------|
| `ProcesarConsulta` | Code | Toma pregunta actual, recupera historial por `chatId`, construye prompt con esquema de columnas y contexto reciente. |
| `Message a model` | Google Gemini (Chat) | Modelo `gemini-2.5-flash`. Responde apoyado en Tool de Sheets. No responde de memoria sobre datos. |
| `Get row(s) in sheet` | Google Sheets Tool | Tool disponible al modelo para consultar `Registros_OCR` con evidencia real. |
| `FormatearRespuestaConsulta` | Code | Extrae texto útil de la respuesta, guarda memoria corta, recorta historial. |
| `ResponderConsulta` | Telegram | Entrega respuesta y mantiene botón para salir del modo consulta. |

---

## 3. Flujos de datos principales

### 3.1 Cargue OCR

```
Usuario envía foto en Telegram
  → TelegramTrigger (descarga binario)
  → ResolverConversacion: route = ocr_process
  → RouterMenu → Rama OCR
    → NotificarProcesando (respuesta inmediata)
    → Analyze an image (Gemini Vision)
      → JSON: { documento: {cabecera...}, items: [{...}, {...}] }
    → FormatearDatos
      → N filas operativas (una por ítem del documento)
      → Estado_OCR: Procesado | Requiere Revision
    → GuardarEnSheets (append N filas)
    → ResumenCarga (conteo, rango de fechas, alertas de revisión)
    → NotificarExito → Usuario
```

### 3.2 Consulta Conversacional

```
Usuario escribe pregunta en Telegram
  → TelegramTrigger
  → ResolverConversacion: route = chat_query
  → RouterMenu → Rama Consulta
    → NotificarConsultando (respuesta inmediata)
    → ProcesarConsulta
      → Construye prompt con esquema + historial
    → Message a model (Gemini)
      → [si necesita datos] Get row(s) in sheet (Tool)
        → Lee Registros_OCR
      → Genera respuesta con evidencia
    → FormatearRespuestaConsulta
      → Actualiza historial corto (memoria de sesión)
    → ResponderConsulta → Usuario
```

### 3.3 Gestión de Estado de Sesión

```
ResolverConversacion
  ← getWorkflowStaticData('global')
  → Lee chatModes[chatId]
  → Lee chatHistories[chatId]
  → Calcula route según modo actual + tipo de mensaje
  → Actualiza estado si hay cambio de modo
```

---

## 4. Decisiones de arquitectura

| ADR | Decisión | Estado |
|-----|----------|--------|
| [ADR-0001](Decisiones_arquitectura/ADR_0001_Arquitectura_n8n_Gemini.md) | Plataforma n8n + Gemini + Telegram como stack principal | Implementada |
| [ADR-0002](Decisiones_arquitectura/ADR_0002_Motor_OCR_Gemini_Vision.md) | Gemini Vision como motor OCR (vs. Google Vision API dedicada) | Implementada |
| [ADR-0003](Decisiones_arquitectura/ADR_0003_Persistencia_Google_Sheets.md) | Google Sheets como base de datos operativa | Implementada |
| [ADR-0004](Decisiones_arquitectura/ADR_0004_Workflow_OCR_Consulta.md) | Workflow dual OCR + consulta conversacional en mismo bot | Implementada |

---

## 5. Infraestructura y dependencias

| Componente | Detalle |
|-----------|---------|
| Plataforma de orquestación | n8n (self-hosted o cloud) |
| Motor OCR / IA | Google Gemini API (`gemini-2.5-flash`) |
| Interfaz de usuario | Telegram Bot |
| Base de datos operativa | Google Sheets — ID: `1YsDdmcT9m6TJEiz6W17XiLIMAuRicgYPML2bQl5RNYY` |
| Pestaña de datos | `Registros_OCR` |

### Credenciales requeridas

| Credencial | Uso |
|-----------|-----|
| Telegram Bot Token | `TelegramTrigger` + nodos de respuesta |
| Google Sheets OAuth | `GuardarEnSheets` + Tool de consulta |
| Google Gemini API Key | `Analyze an image` + `Message a model` |

---

## 6. Estructura de datos — Google Sheets (`Registros_OCR`)

Ver [ESTRUCTURA_GOOGLE_SHEETS.md](../ESTRUCTURA_GOOGLE_SHEETS.md) para la especificación completa de columnas A–AM.

**Regla principal:** Una fila del documento operativo = una fila en la hoja. Si una foto contiene 8 registros diligenciados, el workflow inserta **8 filas**.

**Campos de trazabilidad:**
- `Timestamp`, `Chat_ID`, `Update_ID`, `Message_ID`, `File_ID_Origen` — origen del cargue
- `Estado_OCR` — `Procesado` o `Requiere Revision`
- `Respuesta_Cruda` — respuesta completa de Gemini para auditoría

---

## 7. Funcionalidades pendientes / mejoras futuras

| Funcionalidad | Estado |
|--------------|--------|
| Soporte de múltiples formatos de documento | Pendiente (v2+) |
| Validación automática de calidad de imagen antes de OCR | Pendiente |
| Tablero de KPIs de calidad de captura (% campos OK, % revisión) | Pendiente |
| Filtros/paginación en consulta para hojas con alto volumen | Pendiente |
| Limpieza de nodos heredados (`LeerDatosSheetsConsulta`, `RouterConsultaIA`) | Pendiente |
| Flujo de revisión humana con notificación activa al revisor | Pendiente |
