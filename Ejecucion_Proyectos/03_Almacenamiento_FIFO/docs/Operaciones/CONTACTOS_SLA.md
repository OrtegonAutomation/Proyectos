# Contactos de Soporte y Acuerdos de Nivel de Servicio (SLA)

**Proyecto:** Gestión de Almacenamiento FIFO  
**Cliente:** ODL Instrumentación y Control  
**Proveedor:** IDC Ingeniería  
**Fecha:** Marzo 2026  
**Vigencia:** Marzo 2026 – Marzo 2027

---

## 1. Contactos de soporte

### IDC Ingeniería — Equipo técnico

| Rol | Nombre | Correo | Teléfono | Horario |
|-----|--------|--------|----------|---------|
| Líder de proyecto | Camilo Andres Ortegon Cuevas | [cortegon@idc-confiabilidad.com] | [3004205942] | L-V 8:00–17:00 |
| Desarrollador principal | Camilo Andres Ortegon Cuevas | [cortegon@idc-confiabilidad.com] | [3004205942]| L-V 8:00–17:00 |

## 2. Niveles de severidad

| Nivel | Nombre | Descripción | Ejemplo |
|-------|--------|-------------|---------|
| **S1** | Crítico | El sistema está completamente inoperativo o hay riesgo inminente de pérdida de datos de monitoreo | Disco > 98%, aplicación no inicia, RF-07/RF-08 no funcionan |
| **S2** | Alto | Funcionalidad principal degradada pero el sistema sigue operando | Limpieza no libera suficiente espacio, inventario tarda > 2 minutos |
| **S3** | Medio | Funcionalidad secundaria afectada, existe workaround | Gráficas no se muestran, exportación de bitácora falla |
| **S4** | Bajo | Solicitud menor o cosmética | Error de texto, solicitud de mejora, consulta general |

---

## 3. Acuerdos de nivel de servicio (SLA)

### Tiempos de respuesta

| Severidad | Tiempo de respuesta | Tiempo de resolución | Canal |
|-----------|--------------------|--------------------|-------|
| **S1 — Crítico** | 1 hora (horario laboral) | 4 horas | Teléfono + correo |
| **S2 — Alto** | 4 horas (horario laboral) | 1 día hábil | Correo + teléfono |
| **S3 — Medio** | 1 día hábil | 3 días hábiles | Correo |
| **S4 — Bajo** | 2 días hábiles | 5 días hábiles | Correo |

### Horario de soporte

| Tipo | Horario | Disponibilidad |
|------|---------|---------------|
| Soporte estándar | Lunes a Viernes, 8:00 – 17:00 (hora local) | Incluido |
| Soporte fuera de horario (S1) | Sábados 9:00 – 13:00 | Solo S1, previa coordinación |
| Soporte domingo/festivos | No disponible | — |

---

## 4. Proceso de escalamiento

### Nivel 1 — Operador

1. Intentar resolver usando la [Guía de Troubleshooting](TROUBLESHOOTING.md) y el [Runbook](RUNBOOK.md)
2. Si no se resuelve en 15 minutos → escalar a Nivel 2

### Nivel 2 — Ingeniero ODL

1. Revisar la bitácora del sistema en busca de errores
2. Verificar configuración y estado de RF-07/RF-08
3. Si no se resuelve en 30 minutos → escalar a Nivel 3

### Nivel 3 — Soporte IDC

1. Contactar a IDC por el canal correspondiente a la severidad
2. Proporcionar:
   - Descripción del problema
   - Captura del Dashboard / Bitácora
   - Archivo `fifo_config.json`
   - Archivos de bitácora recientes (`bitacora/bitacora_*.csv`)
3. IDC evalúa remotamente o agenda visita si es necesario

### Diagrama de escalamiento

```
Operador (15 min) → Ingeniero ODL (30 min) → Soporte IDC (según SLA)
      │                     │                        │
      └── Runbook           └── Bitácora + Config    └── Acceso remoto / visita
```

---

## 5. Canales de comunicación

| Canal | Uso | Observaciones |
|-------|-----|--------------|
| **Correo electrónico** | Todos los niveles | Incluir asunto: `[FIFO-Sn] Descripción breve` |
| **Teléfono** | S1 y S2 | Llamar directamente al contacto IDC asignado |
| **Teams / WhatsApp** | Comunicación rápida | Solo para coordinación, no como canal formal |
| **Acceso remoto** | Resolución de problemas | TeamViewer / AnyDesk preinstalado en servidor |

---

## 6. Información requerida para reportar un incidente

Al contactar soporte, incluir:

| Información | Obligatorio | Cómo obtener |
|-------------|------------|-------------|
| Severidad (S1–S4) | ✅ | Según tabla de severidades |
| Descripción del problema | ✅ | Qué pasó, cuándo, qué se intentó |
| Captura del Dashboard | ✅ | Captura de pantalla |
| Estado del semáforo | ✅ | Verde/Amarillo/Rojo |
| Estado RF-07/RF-08 | ✅ | Activo/Inactivo desde pestaña Ejecución |
| Últimas entradas de bitácora | ✅ | Exportar CSV o captura de pestaña Bitácora |
| Archivo `fifo_config.json` | Recomendado | Ubicado en carpeta de la aplicación |
| Versión de la aplicación | Recomendado | — |
| Eventos del sistema Windows | Si aplica | Event Viewer → Application |

---

## 7. Mantenimiento planificado

| Actividad | Frecuencia | Responsable | Duración esperada |
|-----------|-----------|-------------|-------------------|
| Verificación rutinaria | Diaria | Operador ODL | 5 min |
| Revisión de bitácora | Semanal | Operador ODL | 10 min |
| Verificación de configuración | Mensual | Ingeniero ODL | 15 min |
| Exportación de bitácora | Trimestral | Operador ODL | 10 min |
| Actualización de .NET Runtime | Trimestral | IDC / Ing. ODL | 30 min |
| Revisión de política FIFO | Anual | Ingeniero ODL + IDC | 1 hora |

---

## 8. Términos y condiciones

- El SLA está vigente durante el período indicado (1 año desde la entrega)
- No incluye modificaciones al software (nuevas funcionalidades = cotización separada)
- Incluye corrección de defectos (bugs) en funcionalidades entregadas
- El acceso remoto al servidor requiere autorización previa de ODL
- IDC no es responsable por pérdida de datos causada por: corte eléctrico, falla de disco, manipulación manual de archivos fuera del sistema

