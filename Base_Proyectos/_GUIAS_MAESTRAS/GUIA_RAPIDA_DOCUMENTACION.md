# GUÍA RÁPIDA: Estructura Completa de Documentación

**Para el ingeniero de analítica predictiva IDC**

---

## 🎯 VISIÓN GENERAL (TL;DR)

Tu portafolio de 7 proyectos necesita:

```
TOTAL de documentos por proyecto: 30-40 archivos

Carpetas principales:
├─ /docs/project_management/     (PMI formal: Charter, Plans, Tracking)
├─ /docs/architecture_decisions/  (ADRs - decisiones técnicas)
├─ /docs/requirements/            (Qué construir)
├─ /docs/testing/                (Planes, casos, resultados)
├─ /docs/operations/             (Runbooks, procedures)
├─ /docs/compliance/             (Security, audit)
├─ /docs/training/               (Manuales, guides)
├─ /docs/stakeholder_comms/      (Reports, presentations)
└─ /docs/archive/                (Histórico freezed)
```

---

## 📋 DOCUMENTOS ESENCIALES (El mínimo que necesitas)

Para cada proyecto, estos documentos son OBLIGATORIOS:

### FASE 1: INICIACIÓN (Semana 1)
```
☑ PROJECT_CHARTER.pdf              (Aprobación formal, firmas)
☑ SCOPE_STATEMENT.docx             (Qué está IN/OUT)
☑ STAKEHOLDER_ANALYSIS.xlsx        (Quiénes, intereses, estrategia)
☑ COMMUNICATIONS_PLAN.docx         (Quién se comunica qué, cuándo)
```

### FASE 2: PLANIFICACIÓN (Semana 1-2)
```
☑ FUNCTIONAL_REQUIREMENTS.docx     (FR-001 through FR-NNN)
☑ NON_FUNCTIONAL_REQUIREMENTS.docx (Performance, security, availability)
☑ ARCHITECTURE_OVERVIEW.md         (High-level tech design)
☑ PROJECT_PLAN.md                  (WBS, schedule, dependencies)
☑ RISK_REGISTER.xlsx               ← ACTUALIZAR VIERNES (VIVO)
☑ TEST_PLAN.docx                   (Strategy de testing)
```

### FASE 3: EJECUCIÓN (Semana 3-8)
```
☑ USER_STORIES.xlsx                ← ACTUALIZAR semanalmente (VIVO)
☑ ADR_0001.md, ADR_0002.md...     (Decisiones arquitectónicas)
☑ TEST_CASES.xlsx                  ← ACTUALIZAR durante testing (VIVO)
☑ TEST_RESULTS.xlsx                ← ACTUALIZAR semanalmente (VIVO)
☑ CHANGE_LOG.xlsx                  ← ACTUALIZAR ad-hoc (VIVO)
☑ WEEKLY_STATUS_REPORT.docx        ← TODOS LOS VIERNES (VIVO)
☑ LESSONS_LEARNED.md               ← ACTUALIZAR bi-weekly (VIVO)
```

### FASE 4: CIERRE (Semana 9)
```
☑ RUNBOOK.md                       (Procedimientos operacionales)
☑ SECURITY_CHECKLIST.xlsx          (Validar compliance antes de deploy)
☑ ACCEPTANCE_SIGN_OFF.pdf          (Aceptación formal + firmas)
☑ GO_LIVE_REPORT.pdf              (Reporte de deployment)
```

### DOCUMENTOS CONTINUOS
```
☑ STEERING_MEETING_NOTES.docx     (Después de cada steering meeting)
☑ EXECUTIVE_SUMMARY_PPTX          (Antes de cada steering)
☑ BUDGET_TRACKING.xlsx            (Mensual)
```

---

## 🗂️ DÓNDE VA CADA DOCUMENTO

```
PROJECT_CHARTER.pdf
└─ /docs/project_management/01_CHARTER/
   └─ PROJECT_CHARTER_SIGNED.pdf (original firmado)

SCOPE_STATEMENT.docx
└─ /docs/project_management/02_SCOPE/
   └─ SCOPE_STATEMENT_v1.docx

FUNCTIONAL_REQUIREMENTS.docx
└─ /docs/requirements/01_FUNCTIONAL/
   └─ FR_MASTER_v1.docx

ARCHITECTURE.md
└─ /docs/architecture_decisions/
   ├─ ARCHITECTURE_OVERVIEW.md (high-level)
   ├─ ADR_0001_Database_Choice.md
   ├─ ADR_0002_Caching_Strategy.md
   └─ ADR_NNNN_Decision.md

TEST_CASES.xlsx
└─ /docs/testing/
   ├─ TEST_PLAN.docx
   ├─ TEST_CASES_v1.xlsx
   └─ TEST_RESULTS_WEEKLY/
      ├─ RESULTS_WEEK1.xlsx
      ├─ RESULTS_WEEK2.xlsx
      └─ RESULTS_WEEK3.xlsx

RUNBOOK.md
└─ /docs/operations/
   ├─ RUNBOOK.md
   ├─ PLAYBOOK_DisasterRecovery.md
   └─ TROUBLESHOOTING_GUIDE.md

RISK_REGISTER.xlsx
└─ /docs/project_management/03_TRACKING/
   └─ RISK_REGISTER_LIVE.xlsx (NO archivado, current)

WEEKLY_STATUS.docx
└─ /docs/stakeholder_comms/01_STATUS/
   ├─ STATUS_WEEK1.docx
   ├─ STATUS_WEEK2.docx
   └─ STATUS_ARCHIVE/ (PDFs viejos)

LESSONS_LEARNED.md
└─ /docs/project_management/04_CLOSURE/
   └─ LESSONS_LEARNED_FINAL.md
```

---

## 📊 MATRIZ: QUÉ ACTUALIZAR Y CUÁNDO

```
DOCUMENTO                      FRECUENCIA         DÍA/HORA            FORMATO
─────────────────────────────────────────────────────────────────────────────
RISK_REGISTER.xlsx             SEMANAL            Viernes 10:00am     XLSX (live)
CHANGE_LOG.xlsx                AD-HOC             Cuando se solicite   XLSX (live)
WEEKLY_STATUS_REPORT.docx      SEMANAL            Viernes 3:00pm      DOCX (enviado)
BUDGET_TRACKING.xlsx           MENSUAL            1er Viernes         XLSX (live)
TEST_RESULTS.xlsx              SEMANAL/DIARIO     Durante testing     XLSX (live)
USER_STORIES.xlsx              SEMANAL            Viernes (sprint end) XLSX (live)
LESSONS_LEARNED.md             BI-WEEKLY          Post-retro          MD (git)
ADR_NNNN.md                    CUANDO DECISIÓN    Día de decisión      MD (git)
RUNBOOK.md                     CUANDO CAMBIA      Procedimiento nuevo  MD (git)
STEERING_MEETING_NOTES.docx    DESPUÉS MEETING    Dentro 24h           DOCX/PDF
EXECUTIVE_SUMMARY_PPTX         SEMANAL            Antes de steering    PPTX
```

---

## 💾 FORMATO DE ALMACENAMIENTO

### ✅ EN GIT (Control de versiones):
```
MARKDOWN (.md):
├─ RUNBOOK.md
├─ PLAYBOOK_*.md
├─ LESSONS_LEARNED.md
├─ ADR_*.md
├─ ARCHITECTURE_OVERVIEW.md
└─ Todos los documentos técnicos vivos
```

### ✅ EN SHARED FOLDER (Teams/OneDrive):
```
DOCX (.docx) - Documentos colaborativos:
├─ PROJECT_CHARTER.docx (antes de firma)
├─ FUNCTIONAL_REQUIREMENTS.docx
├─ NON_FUNCTIONAL_REQUIREMENTS.docx
├─ PROJECT_PLAN.docx
├─ WEEKLY_STATUS_REPORT_TEMPLATE.docx
├─ COMMUNICATIONS_PLAN.docx
└─ [Otros documentos editables]

XLSX (.xlsx) - Tracking & data:
├─ RISK_REGISTER_LIVE.xlsx
├─ CHANGE_LOG_LIVE.xlsx
├─ USER_STORIES.xlsx
├─ TEST_CASES.xlsx
├─ TEST_RESULTS_WEEKLY.xlsx
├─ BUDGET_TRACKING.xlsx
├─ STAKEHOLDER_ANALYSIS.xlsx
└─ RACI_MATRIX.xlsx

PPTX (.pptx) - Presentaciones:
├─ EXECUTIVE_SUMMARY_WEEKLY.pptx
└─ PROJECT_KICKOFF.pptx
```

### ✅ EN ARCHIVE FOLDER (Final, freezed):
```
PDF (.pdf) - Documentos formales archivados:
├─ PROJECT_CHARTER_SIGNED.pdf
├─ FUNCTIONAL_REQUIREMENTS_APPROVED.pdf
├─ ACCEPTANCE_SIGN_OFF.pdf
├─ GO_LIVE_REPORT.pdf
├─ LESSONS_LEARNED_FINAL.pdf
├─ STEERING_NOTES_JAN_2026.pdf
├─ STEERING_NOTES_FEB_2026.pdf
├─ EXECUTIVE_SUMMARY_WEEK1.pdf
├─ BUDGET_FINAL.pdf
└─ [Todos los documentos "freezed"]
```

---

## ⚡ VELOCIDAD: Cómo crear documentos rápido

### Opción 1: TEMPLATE (Más rápido)
1. Copia template de documento similar
2. Busca/reemplaza datos específicos del proyecto
3. Llena secciones vacías
4. **Tiempo**: 30 min por documento

### Opción 2: AI-ASSISTED (Más rápido aún)
1. Describe proyecto en Copilot: "Crea un PROJECT_CHARTER para [proyecto]"
2. Copilot genera borrador (1 min)
3. Tú editast detalles específicos, datos financieros, nombres
4. Ahorras 80% del tiempo de redacción
5. **Tiempo**: 15 min por documento

### Opción 3: FILL-IN FORMS (Más rápido para repetición)
1. Crea "campos" en templates (usar | para tablas, [ ] para checkboxes)
2. Para cada proyecto, solo llena valores específicos
3. Genera PDF automáticamente
4. **Tiempo**: 5-10 min por documento

---

## 📈 PROGRESIÓN RECOMENDADA POR SEMANA

### SEMANA 1: FUNDACIÓN (3 documentos)
- [ ] PROJECT_CHARTER (1-2 días)
- [ ] SCOPE_STATEMENT (1 día)
- [ ] COMMUNICATIONS_PLAN (½ día)

### SEMANA 2: PLANIFICACIÓN (5 documentos)
- [ ] FUNCTIONAL_REQUIREMENTS (2-3 días)
- [ ] NON_FUNCTIONAL_REQUIREMENTS (1-2 días)
- [ ] TEST_PLAN (1 día)
- [ ] ARCHITECTURE_OVERVIEW (1-2 días)
- [ ] PROJECT_PLAN (1 día)

### SEMANA 3-8: EJECUCIÓN (10+ documentos)
- [ ] ADR_0001.md, ADR_0002.md, etc (ongoing)
- [ ] TEST_CASES (1-2 días, luego update semanal)
- [ ] USER_STORIES (½ día setup, luego update semanal)
- [ ] WEEKLY_STATUS_REPORTS (1 per week, 30 min each)
- [ ] RISK_REGISTER updates (30 min every Friday)
- [ ] CHANGE_LOG updates (as needed)
- [ ] LESSONS_LEARNED (bi-weekly, 30 min)
- [ ] STEERING_MEETING_NOTES (after meetings, 30 min)

### SEMANA 9: CIERRE (5 documentos)
- [ ] RUNBOOK (2-3 días)
- [ ] SECURITY_CHECKLIST (1 día)
- [ ] ACCEPTANCE_SIGN_OFF (½ day)
- [ ] GO_LIVE_REPORT (post-deploy, 1 day)
- [ ] LESSONS_LEARNED_FINAL (2 hrs)

---

## ✍️ CHECKLIST: Antes de "Completar" un proyecto

Verifica que TODOS estos documentos existan:

**PROJECT MANAGEMENT**
- ☑ PROJECT_CHARTER.pdf (firmado)
- ☑ SCOPE_STATEMENT.docx
- ☑ PROJECT_PLAN.docx
- ☑ RACI_MATRIX.xlsx
- ☑ STAKEHOLDER_ANALYSIS.xlsx
- ☑ COMMUNICATIONS_PLAN.docx
- ☑ RISK_REGISTER_FINAL.xlsx
- ☑ CHANGE_LOG_FINAL.xlsx
- ☑ BUDGET_FINAL.pdf
- ☑ LESSONS_LEARNED_FINAL.md

**ARCHITECTURE & DECISIONS**
- ☑ ARCHITECTURE_OVERVIEW.md
- ☑ ADR_0001.md through ADR_NNNN.md (al menos 3-5)

**REQUIREMENTS**
- ☑ FUNCTIONAL_REQUIREMENTS.pdf
- ☑ NON_FUNCTIONAL_REQUIREMENTS.docx
- ☑ USER_STORIES_FINAL.xlsx

**TESTING**
- ☑ TEST_PLAN.docx
- ☑ TEST_CASES_FINAL.xlsx
- ☑ TEST_RESULTS_FINAL.pdf

**OPERATIONS**
- ☑ RUNBOOK.md
- ☑ PLAYBOOK.md (disaster recovery)
- ☑ TROUBLESHOOTING_GUIDE.md
- ☑ SECURITY_CHECKLIST_APPROVED.xlsx

**STAKEHOLDER COMMUNICATIONS**
- ☑ Weekly status reports (8 copies for 8 weeks)
- ☑ Steering meeting notes (8 copies)
- ☑ Executive summaries (8 copies, as PDFs)

**CLOSURE**
- ☑ ACCEPTANCE_SIGN_OFF.pdf (firmado)
- ☑ GO_LIVE_REPORT.pdf
- ☑ HANDOVER_PACKAGE.docx

---

## 🎓 EJEMPLO: Cómo llena esto para PROYECTO P3 (FIFO)

**P3: Almacenamiento FIFO (1 mes, C++/WPF, Baja potencia)**

Carpetas creadas:
```
03_Almacenamiento_FIFO/
├─ docs/
│  ├─ 00_VISION_Y_GOBERNANZA/
│  │  └─ 00_VISION_Y_GOBERNANZA.md       ← YA EXISTE (refactored)
│  ├─ 01_ARQUITECTURA/
│  │  ├─ 01_ESTRUCTURA_ARQUITECTURA.md   ← YA EXISTE (C++/WPF)
│  │  ├─ ADR_0001_C++_vs_Python.md       ← CREAR
│  │  ├─ ADR_0002_WPF_vs_Qt.md           ← CREAR
│  │  └─ ADR_0003_LocalDB_vs_Cloud.md    ← CREAR
│  ├─ 02_PLANIFICACION/
│  │  ├─ PROJECT_CHARTER.pdf             ← CREAR (FIFO específico)
│  │  ├─ SCOPE_STATEMENT.docx            ← CREAR
│  │  ├─ FUNCTIONAL_REQUIREMENTS.docx    ← CREAR
│  │  └─ PROJECT_PLAN.md                 ← CREAR
│  ├─ 03_TESTING/
│  │  ├─ TEST_PLAN.docx                  ← CREAR
│  │  ├─ TEST_CASES.xlsx                 ← CREAR & actualizar
│  │  └─ TEST_RESULTS_WEEKLY/
│  │     ├─ RESULTS_WEEK1.xlsx           ← CREAR & actualizar
│  │     └─ RESULTS_WEEK2.xlsx
│  ├─ 04_OPERACIONES/
│  │  ├─ RUNBOOK.md                      ← CREAR (low-power específico)
│  │  ├─ PLAYBOOK_LowPowerRecovery.md   ← CREAR
│  │  └─ SECURITY_CHECKLIST.xlsx         ← CREAR
│  ├─ 05_CIERRE/
│  │  ├─ ACCEPTANCE_SIGN_OFF.pdf         ← CREAR
│  │  └─ GO_LIVE_REPORT.pdf              ← CREAR
│  ├─ 06_TRACKING_VIVO/
│  │  ├─ RISK_REGISTER.xlsx              ← CREAR & actualizar VIERNES
│  │  ├─ CHANGE_LOG.xlsx                 ← CREAR & actualizar ad-hoc
│  │  ├─ USER_STORIES.xlsx               ← CREAR & actualizar semanal
│  │  ├─ WEEKLY_STATUS/
│  │  │  ├─ STATUS_WEEK1.docx
│  │  │  └─ STATUS_WEEK2.docx
│  │  └─ LESSONS_LEARNED.md              ← CREAR & actualizar bi-weekly
│  └─ 07_COMUNICACIONES/
│     ├─ STEERING_NOTES/
│     │  ├─ STEERING_20260120.pdf        ← Después de cada meeting
│     │  └─ STEERING_20260127.pdf
│     └─ EXECUTIVE_SUMMARY/
│        ├─ SUMMARY_WEEK1.pdf
│        └─ SUMMARY_WEEK2.pdf
├─ src/                          (Código C++/WPF)
├─ tests/                        (Tests unitarios)
└─ README.md
```

---

## 🚀 PRÓXIMOS PASOS

1. **Crear templates base** (copiar ejemplos de este documento)
2. **Para CADA proyecto**: Adaptar nombres & contenido específico
3. **Automatizar actualizaciones semanales**: Calendar reminders para viernes 10am (Risk), 3pm (Status)
4. **Archivar al cierre**: Convertir todo a PDF, guardar en /archive/

---

**Documento versión**: 2.0  
**Último update**: 2026-02-15  
**Uso**: Referencia rápida para "¿qué documento necesito?"

¡Listo para empezar! 🎯
