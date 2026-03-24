# Especificación Técnica — OCR Operativo v2

**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Cliente:** IDC Ingeniería / Confiabilidad Ingenio Pichichí
**Autor:** IDC Ingeniería
**Fecha:** Marzo 2026

---

## 1. Formato objetivo — Registro de Fallas Centrifugas

### 1.1 Estructura del documento

El formato `doc20943920260126103509.pdf` (Fallas Centrifugas) tiene la siguiente estructura:

**Cabecera del documento (se repite en cada fila generada):**

| Campo | Descripción |
|-------|-------------|
| `Fecha_Emision_Documento` | Fecha de emisión del encabezado del formato |
| `Ubicacion` | Ubicación de la planta/área |
| `Planta` | Nombre de la planta |

**Grilla principal (una fila por registro):**

| Campo | Tipo | Valores posibles |
|-------|------|-----------------|
| `Fecha_Dia` | Texto | Día del registro |
| `Fecha_Mes` | Texto | Mes del registro |
| `Fecha_Anio` | Texto | Año del registro |
| `Hora_Inicio` | Texto | Hora de inicio de la falla |
| `Periodo_Inicio` | Texto | `AM` / `PM` |
| `Hora_Fin` | Texto | Hora de fin de la falla |
| `Periodo_Fin` | Texto | `AM` / `PM` |
| `Tiempo_Falla_Min` | Numérico | Minutos de duración |
| Equipos afectados | Booleano por equipo | C1, C2, C3, C4, C5, C6, BBA MIEL 1, BBA MIEL 2, MEZCLADOR, GUSANILLO |
| `Tipo_Falla_Mecanica` | Texto | Código o número de falla mecánica |
| `Tipo_Falla_Electrica` | Texto | Código o número de falla eléctrica |
| `Tipo_Falla_Instrumentacion` | Texto | Código o número de falla de instrumentación |
| `Tipo_Falla_Otros` | Texto | Valor en columna otros |
| `Observaciones` | Texto | Observación de la fila |
| `Firma_Hallazgo` | Texto | Nombre o firma del responsable |

---

## 2. Estructura de datos en Google Sheets

### 2.1 Tabla completa de columnas — `Registros_OCR`

| Col | Nombre | Tipo | Descripción |
|-----|--------|------|-------------|
| A | `Timestamp` | ISO 8601 | Fecha/hora del procesamiento por el workflow |
| B | `Chat_ID` | Texto | Chat de Telegram de origen |
| C | `Update_ID` | Texto | Update de Telegram |
| D | `Message_ID` | Texto | Message ID de Telegram |
| E | `File_ID_Origen` | Texto | `file_id` de la foto origen (trazabilidad) |
| F | `Fecha_Emision_Documento` | Texto | Fecha de emisión del encabezado |
| G | `Ubicacion` | Texto | Ubicación del encabezado |
| H | `Planta` | Texto | Planta del encabezado |
| I | `Item_Index` | Numérico | Consecutivo de fila detectada dentro del documento |
| J | `Fecha_Dia` | Texto | Día reportado en la fila |
| K | `Fecha_Mes` | Texto | Mes reportado en la fila |
| L | `Fecha_Anio` | Texto | Año reportado |
| M | `Fecha_Registro` | Texto | Fecha normalizada `DD/MM/YYYY` |
| N | `Hora_Inicio` | Texto | Hora de inicio detectada |
| O | `Periodo_Inicio` | Texto | `AM` o `PM` |
| P | `Hora_Fin` | Texto | Hora de fin detectada |
| Q | `Periodo_Fin` | Texto | `AM` o `PM` |
| R | `Tiempo_Falla_Min` | Texto | Minutos de falla |
| S | `Equipo_C1` | Booleano | Casilla marcada para C1 |
| T | `Equipo_C2` | Booleano | Casilla marcada para C2 |
| U | `Equipo_C3` | Booleano | Casilla marcada para C3 |
| V | `Equipo_C4` | Booleano | Casilla marcada para C4 |
| W | `Equipo_C5` | Booleano | Casilla marcada para C5 |
| X | `Equipo_C6` | Booleano | Casilla marcada para C6 |
| Y | `Equipo_BBA_MIEL_1` | Booleano | Casilla marcada para BBA MIEL 1 |
| Z | `Equipo_BBA_MIEL_2` | Booleano | Casilla marcada para BBA MIEL 2 |
| AA | `Equipo_MEZCLADOR` | Booleano | Casilla marcada para MEZCLADOR |
| AB | `Equipo_GUSANILLO` | Booleano | Casilla marcada para GUSANILLO |
| AC | `Equipos_Afectados` | Texto | Lista consolidada de equipos marcados |
| AD | `Tipo_Falla_Categoria` | Texto | Categoría principal inferida |
| AE | `Tipo_Falla_Codigo` | Texto | Código principal inferido |
| AF | `Tipo_Falla_Mecanica` | Texto | Valor escrito en columna mecánica |
| AG | `Tipo_Falla_Electrica` | Texto | Valor escrito en columna eléctrica |
| AH | `Tipo_Falla_Instrumentacion` | Texto | Valor escrito en columna instrumentación |
| AI | `Tipo_Falla_Otros` | Texto | Valor en columna otros |
| AJ | `Observaciones` | Texto | Observación de la fila |
| AK | `Firma_Hallazgo` | Texto | Nombre o firma asociada a la fila |
| AL | `Estado_OCR` | Texto | `Procesado` o `Requiere Revision` |
| AM | `Respuesta_Cruda` | Texto | Respuesta completa de Gemini para auditoría |

### 2.2 Reglas de escritura

1. Una fila del documento = una fila en `Registros_OCR`.
2. Si una foto contiene N registros diligenciados, el workflow inserta **N filas**.
3. Los campos de cabecera del documento se repiten en cada fila.
4. Si Gemini no puede leer un valor, devuelve `N/A`.
5. Las columnas booleanas de equipo permiten análisis posteriores por activo.
6. `Respuesta_Cruda` se conserva siempre para depuración y auditoría.

---

## 3. Prompt OCR — Nodo `Analyze an image`

### 3.1 Contrato de salida (JSON estricto)

```json
{
  "documento": {
    "fecha_emision": "...",
    "ubicacion": "...",
    "planta": "..."
  },
  "items": [
    {
      "item_index": 1,
      "fecha_dia": "...",
      "fecha_mes": "...",
      "fecha_anio": "...",
      "hora_inicio": "...",
      "periodo_inicio": "AM|PM",
      "hora_fin": "...",
      "periodo_fin": "AM|PM",
      "tiempo_falla_min": "...",
      "equipos": {
        "C1": true,
        "C2": false,
        "...": "..."
      },
      "tipo_falla_mecanica": "...",
      "tipo_falla_electrica": "...",
      "tipo_falla_instrumentacion": "...",
      "tipo_falla_otros": "...",
      "observaciones": "...",
      "firma_hallazgo": "..."
    }
  ]
}
```

### 3.2 Reglas de transformación en `FormatearDatos`

| Transformación | Descripción |
|---------------|-------------|
| Normalización de año | `22` → `2022`, `26` → `2026` si el valor es de 2 dígitos |
| Corrección de códigos | `22` → `2.2` cuando corresponde a código de falla |
| Unificación de equipos | Combina detección por checkboxes marcados y por lista textual |
| Fecha normalizada | `DD/MM/YYYY` construida de `fecha_dia` + `fecha_mes` + `fecha_anio` |
| Estado OCR | `Requiere Revision` si el parseo no fue confiable; `Procesado` si es limpio |

---

## 4. Lógica de estado conversacional

### 4.1 Modos de chat

| Modo | Descripción | Activado por |
|------|-------------|-------------|
| `menu` | Estado inicial / neutro | Botón "Volver al menú" o inicio |
| `ocr_wait_photo` | Espera foto para OCR | Botón "Subir registro" |
| `chat_query` | Modo consulta conversacional | Botón "Consultar datos" |

### 4.2 Tabla de routing

| Modo actual | Tipo de mensaje | Route calculada |
|-------------|----------------|-----------------|
| `*` | Botón "Subir registro" | `ocr_prompt` |
| `*` | Botón "Consultar datos" | `chat_prompt` |
| `*` | Botón "Volver al menú" | `menu` |
| `ocr_wait_photo` | Foto recibida | `ocr_process` |
| `chat_query` | Texto recibido | `chat_query` |
| `*` | Texto sin modo activo | `menu` |

### 4.3 Memoria conversacional

- **Scope:** por `chatId` usando `getWorkflowStaticData('global')`
- **Estructura:** `chatHistories[chatId]` = últimos N turnos (pregunta + respuesta)
- **Retención:** se recorta después de cada turno para no crecer indefinidamente
- **Limpieza:** al cambiar de modo o volver al menú, el historial se borra

---

## 5. Modelo de confianza y calidad

### 5.1 Estados de calidad por registro

| Estado | Criterio | Acción |
|--------|----------|--------|
| `Procesado` | Parseo limpio, todos los campos obligatorios presentes | Ninguna |
| `Requiere Revision` | Estructura inesperada, campos faltantes críticos, parseo parcial | Notificación al operador en resumen de cargue |

### 5.2 Indicadores operativos (ver criterios de aceptación)

| Indicador | Objetivo |
|-----------|---------|
| % registros `Procesado` sin intervención | ≥ 98% en operación estable |
| Latencia captura → disponible en Sheets | < 1 hora |
| % campos `N/A` en muestra auditada | Referencia para mejora continua |
| Tiempo de respuesta consulta conversacional | < 30 segundos |

---

## 6. Consideraciones de seguridad y operación

- Las credenciales (Telegram, Sheets, Gemini) se gestionan en el gestor de credenciales de n8n.
- Las imágenes se transmiten directamente a Gemini API; no se almacenan en n8n.
- La hoja `Registros_OCR` debe tener permisos de escritura solo para la cuenta de servicio del workflow.
- `Respuesta_Cruda` puede contener texto sensible del documento; acceso a la hoja debe ser controlado.
