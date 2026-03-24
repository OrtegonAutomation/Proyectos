# Estructura Google Sheets - OCR Operativo v2

**Document ID:** `1YsDdmcT9m6TJEiz6W17XiLIMAuRicgYPML2bQl5RNYY`  
**Hoja:** `Registros_OCR`

## Criterio de carga

- Una foto puede contener **varias filas** del formato.
- El workflow debe generar **una fila por item detectado**.
- Los datos generales del documento se repiten en cada fila para facilitar filtros y consultas.

## Estructura observada en el PDF

El formato `doc20943920260126103509.pdf` tiene esta estructura:

- Cabecera del documento:
  - `Fecha Emision`
  - `Ubicacion`
  - `Planta`
- Grilla principal por fila:
  - `Fecha`: `Dia`, `Mes`, `Ano`
  - `Hora Inicio`: valor y franja `AM/PM`
  - `Hora Fin`: valor y franja `AM/PM`
  - `Tiempo Falla (min)`
  - `Equipo Afectado`: `C1`, `C2`, `C3`, `C4`, `C5`, `C6`, `BBA MIEL 1`, `BBA MIEL 2`, `MEZCLADOR`, `GUSANILLO`
  - `Tipo de falla (indique numero)`: `Mecanica`, `Electrica`, `Instrumentacion`, `Otros`
  - `Observaciones`
  - `Firma Hallazgo`

## Columnas recomendadas para la hoja

La fila 1 de `Registros_OCR` debe tener exactamente estos encabezados:

| Columna | Nombre | Tipo | Descripcion |
|---------|--------|------|-------------|
| A | Timestamp | ISO 8601 | Fecha/hora del procesamiento |
| B | Chat_ID | Texto | Chat de Telegram origen |
| C | Update_ID | Texto | Update de Telegram |
| D | Message_ID | Texto | Message ID de Telegram |
| E | File_ID_Origen | Texto | `file_id` de la foto origen |
| F | Fecha_Emision_Documento | Texto | Fecha de emision del encabezado |
| G | Ubicacion | Texto | Ubicacion del encabezado |
| H | Planta | Texto | Planta del encabezado |
| I | Item_Index | Numerico | Consecutivo de fila detectada dentro del documento |
| J | Fecha_Dia | Texto | Dia reportado en la fila |
| K | Fecha_Mes | Texto | Mes reportado en la fila |
| L | Fecha_Anio | Texto | Ano reportado en la fila |
| M | Fecha_Registro | Texto | Fecha normalizada `DD/MM/YYYY` |
| N | Hora_Inicio | Texto | Hora de inicio detectada |
| O | Periodo_Inicio | Texto | `AM` o `PM` |
| P | Hora_Fin | Texto | Hora de fin detectada |
| Q | Periodo_Fin | Texto | `AM` o `PM` |
| R | Tiempo_Falla_Min | Texto | Minutos de falla |
| S | Equipo_C1 | Booleano | Casilla marcada para C1 |
| T | Equipo_C2 | Booleano | Casilla marcada para C2 |
| U | Equipo_C3 | Booleano | Casilla marcada para C3 |
| V | Equipo_C4 | Booleano | Casilla marcada para C4 |
| W | Equipo_C5 | Booleano | Casilla marcada para C5 |
| X | Equipo_C6 | Booleano | Casilla marcada para C6 |
| Y | Equipo_BBA_MIEL_1 | Booleano | Casilla marcada para BBA MIEL 1 |
| Z | Equipo_BBA_MIEL_2 | Booleano | Casilla marcada para BBA MIEL 2 |
| AA | Equipo_MEZCLADOR | Booleano | Casilla marcada para MEZCLADOR |
| AB | Equipo_GUSANILLO | Booleano | Casilla marcada para GUSANILLO |
| AC | Equipos_Afectados | Texto | Lista consolidada de equipos marcados |
| AD | Tipo_Falla_Categoria | Texto | Categoria principal inferida |
| AE | Tipo_Falla_Codigo | Texto | Codigo principal inferido |
| AF | Tipo_Falla_Mecanica | Texto | Valor escrito en columna mecanica |
| AG | Tipo_Falla_Electrica | Texto | Valor escrito en columna electrica |
| AH | Tipo_Falla_Instrumentacion | Texto | Valor escrito en columna instrumentacion |
| AI | Tipo_Falla_Otros | Texto | Valor escrito en columna otros |
| AJ | Observaciones | Texto | Observacion de la fila |
| AK | Firma_Hallazgo | Texto | Nombre o firma asociada a la fila |
| AL | Estado_OCR | Texto | `Procesado` o `Requiere Revision` |
| AM | Respuesta_Cruda | Texto | Respuesta completa de Gemini para auditoria |

## Notas de implementacion

1. **Una fila del documento = una fila en Sheets.**
2. Si una foto contiene 8 registros diligenciados, el workflow debe insertar **8 filas**.
3. Los campos de cabecera del documento se repiten en cada fila.
4. Si Gemini no puede leer un valor, debe devolver `N/A`.
5. Las columnas booleanas de equipo permiten analisis posteriores por activo.
6. `Respuesta_Cruda` se conserva para depuracion y auditoria.
