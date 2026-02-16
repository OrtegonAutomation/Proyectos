# ÍNDICE MAESTRO: Sistema de Documentación IDC 2026

**Ingeniero de Analítica Predictiva - Referencia rápida de todos los materiales**

---

## 🎯 EMPIEZA AQUÍ (PRIMERO LEE ESTO)

### Para entender el SISTEMA COMPLETO:
1. **[RESUMEN_EJECUTIVO_DOCUMENTACION.md](./RESUMEN_EJECUTIVO_DOCUMENTACION.md)** ← 📍 EMPIEZA AQUÍ
   - 1 página con TODO
   - Qué documentar, cuándo, dónde
   - Checklist + próximos pasos

2. **[GUIA_RAPIDA_DOCUMENTACION.md](./GUIA_RAPIDA_DOCUMENTACION.md)**
   - Guía operacional semana-a-semana
   - Checklist por fase del proyecto
   - Ejemplos de cómo llenar para P3 (FIFO)

3. **[MATRIZ_VISUAL_DOCUMENTACION.md](./MATRIZ_VISUAL_DOCUMENTACION.md)**
   - Visualización ASCII completa
   - 8 carpetas PMI con todos los documentos
   - Qué va en cada carpeta

4. **[CONTENIDO_ESPECIFICO_POR_DOCUMENTO.md](./CONTENIDO_ESPECIFICO_POR_DOCUMENTO.md)**
   - TEMPLATES detallados para 30+ documentos
   - Ejemplos reales de contenido exacto
   - Cuando leerlo: Cuando crees cada documento

---

## 📁 ESTRUCTURA POR PROYECTO (TODOS)

Cada proyecto tiene esta estructura:

```
[Proyecto_ID]/
├─ docs/
│  ├─ project_management/          (PMI formal)
│  │  ├─ 01_CHARTER/
│  │  ├─ 02_SCOPE/
│  │  ├─ 03_PLANNING/
│  │  └─ 04_TRACKING/ ← ACTUALIZACIONES VIERNES
│  ├─ architecture_decisions/      (ADRs técnicos)
│  ├─ requirements/                (Qué construir)
│  ├─ testing/                     (Plans, cases, results)
│  ├─ operations/                  (Runbooks, playbooks)
│  ├─ compliance/                  (Security, audit)
│  ├─ stakeholder_comms/           (Reports, presentations)
│  └─ archive/                     (Final freezed PDFs)
├─ src/                            (Code)
├─ tests/                          (Tests)
└─ README.md
```

---

## 🗂️ 7 PROYECTOS DE ESTE AÑO

### ✅ P1: Aspen Mtell ODL
- **Duración**: 12 meses (ALTA)
- **Documentación**: 43 archivos en 8 carpetas
- **Ubicación**: `01_Aspen_Mtell_ODL/docs/`
- **Prioridad**: ALTA (visión experta)
- **Stack**: Aspen + Python + SQL Server

### ✅ P2: Agentes Accionables BPC
- **Duración**: 12 meses (ALTA)
- **Documentación**: 43 archivos
- **Ubicación**: `02_Agentes_Accionables_BPC/docs/`
- **Prioridad**: ALTA (captura de valor)
- **Stack**: .NET + Docker

### ✅ P3: Almacenamiento FIFO ⭐ PRIORIDAD
- **Duración**: 1 mes (ALTA)
- **Documentación**: 35 archivos (comprimido)
- **Ubicación**: `03_Almacenamiento_FIFO/docs/`
- **Prioridad**: MÁXIMA (continuidad operacional)
- **Stack**: C++17 + WPF (low-power)
- **Detalles**: Ver `03_Almacenamiento_FIFO/docs/00_VISION_Y_GOBERNANZA.md`

### ✅ P4: OCR Operativo
- **Duración**: 1 mes
- **Documentación**: 25 archivos (mínimo)
- **Ubicación**: `04_OCR_Operativo/docs/`
- **Prioridad**: MEDIA
- **Stack**: Python OCR

### ✅ P5: Vibración Desfibradora
- **Duración**: 1 trimestre (ALTA)
- **Documentación**: 35 archivos
- **Ubicación**: `05_Vibracion_Desfibradora/docs/`
- **Prioridad**: ALTA (mantenimiento)
- **Stack**: Python ML

### ✅ P6: Detección Crudo
- **Duración**: 1 mes
- **Documentación**: 25 archivos
- **Ubicación**: `06_Deteccion_Crudo/docs/`
- **Prioridad**: MEDIA
- **Stack**: Python classification

### ✅ P7: Optimización Energética
- **Duración**: 1 semestre (ALTA)
- **Documentación**: 40 archivos
- **Ubicación**: `07_Optimizacion_Energetica/docs/`
- **Prioridad**: ALTA (ahorro operacional)
- **Stack**: Python optimization

---

## 📋 30-40 DOCUMENTOS POR PROYECTO (TIPOS)

### 🔴 CRÍTICOS (TODOS NECESITAN)
- PROJECT_CHARTER.pdf
- SCOPE_STATEMENT.docx
- FUNCTIONAL_REQUIREMENTS.pdf
- TEST_PLAN.docx
- ACCEPTANCE_SIGN_OFF.pdf

### 🟠 IMPORTANTES (MAYORÍA)
- RISK_REGISTER.xlsx ← ACTUALIZAR VIERNES
- WEEKLY_STATUS_REPORT.docx ← TODOS LOS VIERNES
- ARCHITECTURE_OVERVIEW.md
- RUNBOOK.md
- SECURITY_CHECKLIST.xlsx

### 🟡 COMPLEMENTARIOS (MEJORES PRÁCTICAS)
- ADR_*.md (Architecture Decision Records)
- LESSONS_LEARNED.md
- PLAYBOOK.md
- STEERING_MEETING_NOTES.docx
- BUDGET_TRACKING.xlsx

### 🟢 TRACKING (EN VIVO)
- USER_STORIES.xlsx ← Actualizar semanal
- TEST_CASES.xlsx ← Actualizar durante testing
- TEST_RESULTS.xlsx ← Actualizar diario/semanal
- CHANGE_LOG.xlsx ← Actualizar ad-hoc

---

## 📊 CALENDARIOS & FRECUENCIAS

### SEMANAL (MISMO DÍA/HORA)

| Viernes 10:00 AM | Viernes 3:00 PM | Bi-weekly | Ad-hoc |
|---|---|---|---|
| 🔴 RISK_REGISTER update | 🔴 WEEKLY_STATUS_REPORT | LESSONS_LEARNED (post-retro) | CHANGE_LOG (cuando se pide) |
| Email a steering | Email + PPTX visual | +30 min después standup | Cualquier momento |
| PM owner | PM owner | Team owner | PM owner |

### IMPORTANTES (NO OLVIDAR)
- Viernes 10:00 AM: Risk register (¿qué riesgos nuevos? ¿impactos?)
- Viernes 3:00 PM: Status report (½ página, métricas, próximos pasos)
- Después de cada retrospective: Lessons learned (qué aprendemos)
- Cuando pidan cambios: Change log (qué está en espera, aprobado, rechazado)

---

## 🎯 DOCUMENTOS REFERENCIA POR SITUACIÓN

### "Necesito crear el PROJECT_CHARTER"
→ Lee: `CONTENIDO_ESPECIFICO_POR_DOCUMENTO.md` Sección 1.1  
→ Tiempo: 1-2 días  
→ Requiere: Sponsor, presupuesto, timeline, objetivos SMART

### "Necesito crear FUNCIONAL_REQUIREMENTS"
→ Lee: `CONTENIDO_ESPECIFICO_POR_DOCUMENTO.md` Sección 3.1  
→ Tiempo: 2-3 días  
→ Requiere: User stories, conversaciones con stakeholders

### "Necesito hacer el TEST_PLAN"
→ Lee: `CONTENIDO_ESPECIFICO_POR_DOCUMENTO.md` Sección 4.1  
→ Tiempo: 1 día  
→ Requiere: Scope, environment specs, QA participation

### "Es VIERNES 10am, ¿qué hago con RISK_REGISTER?"
→ Lee: `RESUMEN_EJECUTIVO_DOCUMENTACION.md` sección "CICLO SEMANAL"  
→ Tiempo: 30 minutos  
→ Requiere: Revisar últimos 7 días, actualizar prioridades

### "Es VIERNES 3pm, ¿cómo hago WEEKLY_STATUS?"
→ Lee: `CONTENIDO_ESPECIFICO_POR_DOCUMENTO.md` Sección 1.8  
→ Tiempo: 30-45 minutos  
→ Template: EMAIL + 1 página DOCX + 5 slides PPTX

### "Terminó proyecto, ¿qué falta?"
→ Lee: `GUIA_RAPIDA_DOCUMENTACION.md` sección "CHECKLIST"  
→ Verifica: 43 documentos, todos archivados, sign-offs completados

---

## 📍 ARCHIVOS MAESTROS EN BASE_PROYECTOS/

```
Base_Proyectos/
├─ RESUMEN_EJECUTIVO_DOCUMENTACION.md      ← 📌 EMPIEZA AQUÍ
├─ GUIA_RAPIDA_DOCUMENTACION.md            ← Operacional
├─ MATRIZ_VISUAL_DOCUMENTACION.md          ← Visualización ASCII
├─ CONTENIDO_ESPECIFICO_POR_DOCUMENTO.md   ← Templates detallados
├─ INDICE_MAESTRO_DOCUMENTACION.md         ← Este archivo
├─ GUIA_ESTRUCTURA_DOCUMENTACION_PROYECTOS.md  ← Estructura carpetas
├─ README_PORTAFOLIO_2026.md               ← Overview portafolio
├─ DOCUMENTACION_EXPANSIÓN_SUMMARY.md      ← Expansiones
├─ DOCUMENTACION_PROYECTOS_3_7_COMPLETA.md ← Histórico
│
└─ Proyectos/
   ├─ 01_Aspen_Mtell_ODL/
   ├─ 02_Agentes_Accionables_BPC/
   ├─ 03_Almacenamiento_FIFO/
   ├─ 04_OCR_Operativo/
   ├─ 05_Vibracion_Desfibradora/
   ├─ 06_Deteccion_Crudo/
   └─ 07_Optimizacion_Energetica/
```

---

## ✅ CHECKLIST: "¿ESTOY PREPARADO?"

Antes de comenzar un proyecto:

- [ ] Leído RESUMEN_EJECUTIVO_DOCUMENTACION.md (15 min)
- [ ] Descargado templates de CONTENIDO_ESPECIFICO_POR_DOCUMENTO.md
- [ ] Creadas carpetas /docs/ en proyecto (8 subcarpetas)
- [ ] Calendar event: Viernes 10am RISK_REGISTER
- [ ] Calendar event: Viernes 3pm WEEKLY_STATUS
- [ ] Shared folder creado para tracking VIVO (XLSX)
- [ ] PM designado para documentación
- [ ] Sponsor informado de cadencia de reportes

---

## 🚀 QUICK START (HOY MISMO)

### Hora 1: Comprende el sistema
1. Lee RESUMEN_EJECUTIVO_DOCUMENTACION.md (10 min)
2. Mira MATRIZ_VISUAL_DOCUMENTACION.md (10 min)
3. Entiende: 8 carpetas, 43 docs, 4 formatos

### Hora 2: Crea estructura
1. Crea carpeta /docs/ con 8 subcarpetas (5 min)
2. Descarga templates de CONTENIDO_ESPECIFICO_POR_DOCUMENTO.md (10 min)
3. Guarda templates en shared folder (5 min)

### Hora 3: Inicia P3 (FIFO - MÁXIMA PRIORIDAD)
1. Crea PROJECT_CHARTER.docx para P3 (45 min)
2. Crea SCOPE_STATEMENT.docx para P3 (30 min)
3. Envía a Sponsor para firma (5 min)

### Por Viernes:
1. RISK_REGISTER.xlsx completado (30 min)
2. WEEKLY_STATUS.docx completado (30 min)
3. Ambos enviados a steering (5 min)

---

## 💡 CLAVES PARA ÉXITO

✅ **CONSISTENCIA**: Viernes 10am y 3pm NO se negocian  
✅ **ESPECIFICIDAD**: Cada documento tiene propósito claro  
✅ **PROFESIONALISMO**: Firma en Charter = autorización formal  
✅ **TRAZABILIDAD**: Cambios vía Change Log, no improvisado  
✅ **ACTUALIZACIÓN**: Vivos (RISK, STATUS, STORIES) son críticos  
✅ **ARCHIVO**: PDFs finales = evidencia de buen trabajo  

---

## 📞 ¿NECESITAS AYUDA?

| Pregunta | Respuesta | Documento |
|----------|-----------|-----------|
| "¿Qué hago ESTA SEMANA?" | Crea PROJECT_CHARTER | CONTENIDO_ESPECIFICO (1.1) |
| "¿Cómo lleno RISK_REGISTER?" | Template exacto + ejemplo | CONTENIDO_ESPECIFICO (1.5) |
| "¿VIERNES QUÉ HAGO?" | Status report en 30 min | RESUMEN_EJECUTIVO |
| "¿Para P3 (FIFO) qué especial?" | C++/WPF/low-power docs | GUIA_RAPIDA (ejemplo) |
| "¿Cuántos docs necesito?" | Mínimo 15, ideal 40+ | MATRIZ_VISUAL |
| "¿Dónde guardo cada uno?" | Rutas exactas por tipo | GUIA_RAPIDA |

---

## 📈 IMPACTO ESPERADO

**Al completar tu portafolio de 7 proyectos con esta documentación:**

- ✅ 43 documentos × 7 proyectos = **300 archivos profesionales**
- ✅ Demuestras **dominio de PMI + Software Architecture**
- ✅ Creas **portafolio tangible** para futuras oportunidades
- ✅ Estableces **aliados estratégicos** con calidad de trabajo
- ✅ Dejas **continuidad operacional** si te vas del rol
- ✅ Ganas **confianza del Sponsor** con rigor ejecutivo

---

## 🎓 ESTRUCTURA RECOMENDADA DE LECTURA

### Primero (15 minutos):
1. Este índice (INDICE_MAESTRO)
2. RESUMEN_EJECUTIVO_DOCUMENTACION.md

### Segundo (1 hora):
3. GUIA_RAPIDA_DOCUMENTACION.md
4. MATRIZ_VISUAL_DOCUMENTACION.md

### Tercero (Según necesites):
5. CONTENIDO_ESPECIFICO_POR_DOCUMENTO.md (seción por sección)
6. GUIA_ESTRUCTURA_DOCUMENTACION_PROYECTOS.md (carpetas & archivos)

### Referencia Permanente:
- Imprime: RESUMEN_EJECUTIVO_DOCUMENTACION.md
- Bookmarks: CONTENIDO_ESPECIFICO_POR_DOCUMENTO.md secciones
- Calendar: Viernes 10am + 3pm

---

## 🏆 CONCLUSIÓN

Tienes todo lo que necesitas para:
- ✅ Documentar 7 proyectos profesionalmente
- ✅ Seguir mejores prácticas PMI + Software
- ✅ Crear portafolio de carrera
- ✅ Demostrar excelencia en ejecución

**El resto es disciplina de actualizar VIERNES 10am & 3pm.**

---

**Documento**: INDICE_MAESTRO_DOCUMENTACION.md  
**Versión**: 1.0  
**Actualizado**: 2026-02-15  
**Uso**: Referencia y navegación de todo el sistema

---

👉 **Siguiente paso**: Abre RESUMEN_EJECUTIVO_DOCUMENTACION.md
