# GUÍA COMPLETA: CONTENIDO ESPECÍFICO POR DOCUMENTO Y FORMATO

**Versión**: 2.0  
**Propósito**: Definir exactamente QUÉ va en CADA documento, en QUÉ formato, con QUÉ contenido

---

## 1. FORMATOS DE DOCUMENTACIÓN PERMITIDOS

| Formato | Uso | Ejemplos | Ubicación |
|---------|-----|----------|-----------|
| **Markdown (.md)** | Documentación técnica, viva, en control de versiones | Código specs, procedimientos, ADRs | `/docs/` (Git) |
| **Word (.docx)** | Documentos formales editables, colaboración | Charter, planes, requerimientos | `/docs/` (versioned) |
| **PDF** | Documentos formales finales, auditoría, archivos | Sign-off, deliverables, freezed docs | `/docs/archive/` (histórico) |
| **PowerPoint (.pptx)** | Presentaciones ejecutivas, stakeholder meetings | Resúmenes, status, recomendaciones | `/docs/presentations/` |
| **Excel (.xlsx)** | Datos, tracking, matrices | RACI, Risk register, Gantt | `/docs/tracking/` |
| **Visio (.vsdx)** | Diagramas arquitectura, flujos | C4, data flows, workflows | `/docs/diagrams/` |
| **Otros** | Según necesidad | Recursos especializados | Según corresponda |

---

## 2. ESTRUCTURA DE CARPETAS CON FORMATOS

```
/docs/
│
├── project_management/                 # Documentos PMI formales
│   ├── PROJECT_CHARTER.docx           # Formal, firmado, editable
│   ├── PROJECT_CHARTER_APPROVED.pdf   # Versión finalizada
│   ├── SCOPE_STATEMENT.docx
│   ├── STAKEHOLDER_MANAGEMENT.docx
│   ├── RACI_MATRIX.xlsx               # Tracking matrix
│   ├── RISK_REGISTER.xlsx             # Vivo (actualizar semanalmente)
│   ├── CHANGE_LOG.xlsx                # Vivo (tracker de cambios)
│   ├── COMMUNICATIONS_PLAN.docx
│   ├── WEEKLY_STATUS_TEMPLATE.docx    # Template
│   ├── MONTHLY_REPORTS/               # Carpeta histórica
│   │   ├── STATUS_REPORT_JAN_2026.pdf
│   │   ├── STATUS_REPORT_FEB_2026.pdf
│   │   └── STEERING_NOTES_FEB_15.docx
│   └── LESSONS_LEARNED.md             # Vivo (actualizar bi-weekly)
│
├── architecture_decisions/             # ADRs (Architecture Decision Records)
│   ├── ADR_0001_[Decision].md         # Markdown (vivo en repo)
│   ├── ADR_0002_[Decision].md
│   └── ADR_TEMPLATE.md                # Template de referencia
│
├── requirements/                       # Especificaciones formales
│   ├── FUNCTIONAL_REQUIREMENTS.docx   # Formal, editable
│   ├── FUNCTIONAL_REQUIREMENTS_FINAL.pdf
│   ├── NON_FUNCTIONAL_REQUIREMENTS.docx
│   ├── USER_STORIES.xlsx              # Tracking de historias
│   ├── ACCEPTANCE_CRITERIA.md         # Vivo en repo
│   └── USE_CASES.md                   # Puede ser MD o DOCX
│
├── testing/                            # Documentación de testing
│   ├── TEST_PLAN.docx                 # Formal
│   ├── TEST_CASES.xlsx                # Tracker de casos
│   ├── UAT_PLAN.docx                  # User Acceptance Testing
│   ├── TEST_RESULTS_SUMMARY.pdf       # Resumen final
│   └── test_results_live.xlsx         # Vivo (daily updates)
│
├── operations/                         # Documentación operacional
│   ├── RUNBOOK.md                     # Copy-paste ready (Markdown)
│   ├── RUNBOOK_PROCEDURES.pdf         # Versión formal para impresión
│   ├── PLAYBOOK.md                    # Crisis procedures
│   ├── DEPLOYMENT_CHECKLIST.xlsx      # Checklist interactivo
│   ├── DEPLOYMENT_CHECKLIST.pdf       # Versión para print
│   ├── TROUBLESHOOTING_GUIDE.md
│   ├── SLA_DEFINITION.docx
│   └── MAINTENANCE_SCHEDULE.xlsx
│
├── compliance/                         # Seguridad & cumplimiento
│   ├── SECURITY_POLICY.docx           # Formal
│   ├── SECURITY_CHECKLIST.xlsx        # Checklist de validación
│   ├── COMPLIANCE_REQUIREMENTS.docx   # Regulatorio
│   ├── AUDIT_TRAIL_REQUIREMENTS.md
│   └── SECURITY_TESTING_RESULTS.pdf   # Resultado penetration test
│
├── training/                           # Materiales de capacitación
│   ├── TRAINING_PLAN.docx
│   ├── USER_MANUAL.pdf                # Documento formal
│   ├── QUICK_START_GUIDE.pdf
│   ├── ADMIN_GUIDE.docx
│   ├── VIDEO_TRANSCRIPTS.md
│   ├── FAQ.md                         # Markdown
│   └── TRAINING_MATERIALS_INDEX.docx
│
├── stakeholder_communication/          # Reportes y presentaciones
│   ├── EXECUTIVE_SUMMARY_TEMPLATE.pptx # Presentación ejecutiva
│   ├── WEEKLY_STATUS_PRESENTATION.pptx # Presentation visual
│   ├── MONTHLY_REPORTS/
│   │   ├── REPORT_JAN_2026.pdf
│   │   ├── REPORT_FEB_2026.pdf
│   │   └── STEERING_MEETING_NOTES.docx
│   ├── STEERING_AGENDA_TEMPLATE.docx
│   └── RISK_ESCALATIONS.xlsx          # Tracker de escalaciones
│
├── deliverables/                       # Entregables finales
│   ├── PROJECT_CHARTER_SIGNED.pdf     # Acta de inicio
│   ├── ACCEPTANCE_SIGN_OFF.docx       # Formulario de aceptación
│   ├── GO_LIVE_REPORT.pdf             # Reporte formal
│   ├── DEPLOYMENT_VALIDATION_REPORT.pdf
│   └── HANDOVER_PACKAGE.docx          # Para Operations
│
├── financial/                          # Aspectos financieros
│   ├── BUDGET_BASELINE.xlsx           # Budget inicial
│   ├── BUDGET_TRACKING.xlsx           # Vivo (mensual)
│   ├── BURN_DOWN_CHART.xlsx           # Tracking visual
│   ├── CHANGE_REQUEST_LOG.xlsx        # Cambios + costo
│   └── FINAL_ACCOUNTING.pdf           # Cierre financiero
│
├── diagrams/                           # Diagramas y visuales
│   ├── SYSTEM_ARCHITECTURE.vsdx       # Visio diagrama
│   ├── SYSTEM_ARCHITECTURE.pdf        # Versión exportada
│   ├── DATA_FLOW_DIAGRAM.png          # Exportado de Visio
│   ├── DEPLOYMENT_ARCHITECTURE.pdf
│   ├── C4_CONTEXT_DIAGRAM.md          # ASCII o Markdown
│   └── INTEGRATION_DIAGRAM.vsdx
│
├── knowledge_base/                     # Información reutilizable
│   ├── TECHNICAL_GLOSSARY.md          # Markdown (vivo)
│   ├── VENDOR_CONTACTS.xlsx           # Contactos
│   ├── ASSUMPTIONS_LOG.md
│   ├── DEPENDENCIES.md
│   └── EXTERNAL_REFERENCES.docx       # Links y referencias
│
└── archive/                            # Histórico de documentos finalizados
    ├── PHASE_1_CLOSURE_REPORT.pdf
    ├── PHASE_2_LESSONS_LEARNED.pdf
    └── [documentos históricos freezed]
```

---

## 3. CONTENIDO ESPECÍFICO POR DOCUMENTO

### 3.1 PROJECT_MANAGEMENT Folder

#### **PROJECT_CHARTER.docx / .pdf**
**Formato**: DOCX (editable) → PDF (final firmado)  
**Frecuencia**: Una sola vez (inicio proyecto)  
**Propósito**: Autorización formal del proyecto  

**Tabla de Contenidos**:
```
1. EXECUTIVE SUMMARY (0.5 pág)
   - Qué es el proyecto en 1 párrafo
   - Por qué es importante
   - Valor esperado

2. PROJECT AUTHORIZATION (1 pág)
   - Sponsor: [Name, Title, Signature]
   - PM: [Name, Signature]
   - Stakeholders: [Names]
   - Approval Date & Signature Line
   
3. BUSINESS CASE
   - Problema identificado
   - Solución propuesta
   - Beneficios esperados
   - ROI estimate
   
4. PROJECT OBJECTIVES
   - 5-10 objetivos SMART
   - Tabla: Objetivo | Métrica | Target | Timeline
   
5. HIGH-LEVEL SCOPE
   - What's IN (3-5 items)
   - What's OUT (3-5 items)
   - Constraints (time, budget, resources)
   
6. KEY STAKEHOLDERS
   - Table: Name | Role | Interest | Power | Influence
   
7. HIGH-LEVEL RISKS
   - 5-10 riesgos principales
   - Tabla: Risk | Probability | Impact | Mitigation
   
8. BUDGET & RESOURCES
   - Total budget estimate
   - Team size & key roles
   - Timeline estimate
   
9. APPROVALS
   - Sponsor: _________________ Date: _____
   - PM: _________________ Date: _____
   - Director IT: _________________ Date: _____
```

**Firmado por**: Sponsor, PM, IT Director  
**Guardado como**: `/docs/project_management/PROJECT_CHARTER_APPROVED.pdf`

---

#### **SCOPE_STATEMENT.docx**
**Formato**: DOCX (editable)  
**Frecuencia**: Una sola vez (planificación), update si hay cambios aprobados  
**Propósito**: Descripción detallada de qué está IN y OUT

**Tabla de Contenidos**:
```
1. PROJECT OVERVIEW
   - Nombre del proyecto
   - PM & Sponsor
   - Duration & Budget
   
2. DETAILED IN-SCOPE
   - Features/Functionality (lista detallada)
   - Systems to be integrated
   - Deliverables expected
   - Capabilities included
   
3. EXPLICIT OUT-OF-SCOPE
   - What WON'T be done
   - Items deferred to Phase 2
   - Integration with other projects
   - Future enhancements
   
4. CONSTRAINTS
   - Time constraints
   - Budget constraints
   - Resource availability
   - Technical constraints
   - Organizational constraints
   
5. ASSUMPTIONS
   - Data availability assumptions
   - Stakeholder availability
   - Infrastructure assumptions
   - Business process assumptions
   
6. ACCEPTANCE CRITERIA
   - Criteria for each major deliverable
   - Quality standards
   - Performance requirements
   
7. APPROVAL & SIGN-OFF
   - Stakeholder review date
   - Approvals: _________________ Date: _____
```

---

#### **STAKEHOLDER_MANAGEMENT.docx**
**Formato**: DOCX + EXCEL matriz  
**Frecuencia**: Initial (planificación), update si stakeholders change

**Tabla de Contenidos**:
```
1. STAKEHOLDER IDENTIFICATION
   - Tabla: Name | Organization | Role | Contact
   
2. STAKEHOLDER ANALYSIS (matriz en EXCEL)
   - Table: Stakeholder | Interest | Power | Influence | Position | Strategy
   
3. ENGAGEMENT STRATEGY
   - Para CADA stakeholder group:
     * Interest statement
     * Engagement approach
     * Communication frequency
     * Success definition
     
4. COMMUNICATION PLAN
   - Tabla: Audience | Message | Format | Frequency | Owner
   
5. ESCALATION PROCEDURES
   - Who escalates to whom
   - When to escalate
   - Escalation matrix
```

**Matriz en EXCEL**: `/docs/tracking/STAKEHOLDER_MATRIX.xlsx`

---

#### **RACI_MATRIX.xlsx**
**Formato**: EXCEL (spreadsheet)  
**Frecuencia**: Initial + update si roles change  
**Propósito**: Clarificar quién es Responsible, Accountable, Consulted, Informed

**Estructura**:
```
COLUMNS:
- Task / Deliverable
- PM
- Tech Lead
- Analytics
- QA
- Change Manager
- Sponsor

ROWS (para cada major task):
- Planning
- Design
- Development
- Testing
- Deployment
- Training
- Go-Live
- Support

VALUES:
A = Accountable (final authority)
R = Responsible (does the work)
C = Consulted (provide input)
I = Informed (kept in loop)

RULE: Cada task debe tener:
- 1 Accountable
- 1+ Responsible
- 0+ Consulted
- 0+ Informed
```

---

#### **RISK_REGISTER.xlsx** ← DOCUMENTO VIVO
**Formato**: EXCEL (actualizar semanalmente)  
**Propósito**: Tracking de riesgos en tiempo real

**Estructura**:
```
COLUMNS:
A. Risk ID (R001, R002, ...)
B. Risk Description
C. Category (Technical, Schedule, Resource, Financial, etc.)
D. Probability (High/Medium/Low)
E. Impact (High/Medium/Low)
F. Priority (P×I)
G. Owner (Who's responsible)
H. Mitigation Strategy
I. Mitigation Status (Not Started / In Progress / Complete)
J. Contingency Plan
K. Status (Open / Mitigated / Closed)
L. Last Updated (date)
M. Target Resolution Date
N. Notes

ROWS: 10-20 riesgos identificados

ACTUALIZACION: Cada VIERNES
- Cambiar status de mitigaciones
- Agregar nuevos riesgos si aparecen
- Cerrar riesgos mitigados
- Escalalar si priority sube
```

**Actualización**: Viernes 10am  
**Owner**: Project Manager

---

#### **CHANGE_LOG.xlsx** ← DOCUMENTO VIVO
**Formato**: EXCEL (ad-hoc updates)  
**Propósito**: Registro de cambios aprobados (scope, plan, budget)

**Estructura**:
```
COLUMNS:
A. Change ID (CR-001, CR-002, ...)
B. Date Requested
C. Description of Change
D. Reason for Change
E. Impact Area (Scope/Schedule/Budget/Quality)
F. Estimated Impact (time/cost)
G. Requested By
H. Status (Requested / Analyzed / Approved / Rejected / Implemented)
I. Approved By
J. Implementation Date
K. Actual Impact (vs estimated)
L. Notes

ROWS: 1 row per change request

REGLA: NINGÚN cambio puede implementarse sin aprobación en este log
```

**Actualización**: Ad-hoc (cuando se aprueba cambio)  
**Owner**: Project Manager

---

#### **COMMUNICATIONS_PLAN.docx**
**Formato**: DOCX  
**Frecuencia**: Initial (planificación)

**Tabla de Contenidos**:
```
1. COMMUNICATION OBJECTIVES
   - What needs to be communicated
   - Why (business rationale)
   
2. STAKEHOLDER COMMUNICATION MATRIX
   Tabla: Stakeholder | Message | Format | Frequency | Owner | Success Metric
   
   Examples:
   - Sponsor | Weekly status | Email summary | Weekly Fri | PM | Read & no questions
   - Operators | New features | Demo | Bi-weekly | Change Mgr | Training completion
   - IT | Technical changes | Tech meeting | Weekly | Tech Lead | Decisions documented
   
3. MEETING CADENCE
   - Daily standup (15 min)
   - Weekly tech sync (1.5h)
   - Bi-weekly steering (2h)
   - Monthly executive review (1h)
   
4. REPORTING TEMPLATES
   - Weekly status template reference
   - Monthly report template reference
   - Executive summary template reference
   
5. ESCALATION COMMUNICATION
   - When to escalate (conditions)
   - To whom (escalation matrix)
   - Format (email, meeting, call)
   
6. CRISIS COMMUNICATION
   - What if project is at risk?
   - Immediate notification to whom?
   - Response protocol
```

---

#### **WEEKLY_STATUS_TEMPLATE.docx**
**Formato**: DOCX (template para llenar cada semana)  
**Frecuencia**: Semanal (enviado cada viernes)

**Estructura**:
```
HEADER:
- Project Name
- Report Period: [Week of Date]
- Prepared by: [PM Name]
- Distribution: [List of recipients]

SECTION 1: EXECUTIVE SUMMARY (1 paragraph)
☐ Overall Status: 🟢 GREEN / 🟡 YELLOW / 🔴 RED
☐ Key accomplishments
☐ Key blockers or concerns

SECTION 2: METRICS vs BASELINE
Table:
| Metric | Baseline | Planned | Actual | Variance | Status |
|--------|----------|---------|--------|----------|--------|
| Schedule | 50% | 50% | 48% | -2% | 🟡 |
| Budget | $500K | $500K | $480K | -$20K | 🟢 |
| Quality | 95% | 95% | 94% | -1% | 🟡 |

SECTION 3: ACCOMPLISHMENTS THIS WEEK
- ✅ [Completed task 1]
- ✅ [Completed milestone]
- ✅ [Issue resolved]

SECTION 4: IN PROGRESS
- [Task] - 60% complete, on track
- [Task] - 40% complete, at risk
- [Task] - 20% complete, blocked

SECTION 5: BLOCKERS & RISKS
Format:
[!] CRITICAL: [Issue] 
     Impact: [What happens if not resolved]
     Action: [What will fix it]
     Owner: [Person responsible]
     Target Resolution: [Date]

[!] HIGH: [Issue]
     Action: [Plan]
     Owner: [Person]
     Target: [Date]

SECTION 6: PLAN FOR NEXT WEEK
- [Major deliverable expected]
- [Milestone target]
- [Key activities]

SECTION 7: DECISIONS REQUIRED
- Decision: [What needs to be decided?]
- Options: [Option A, Option B, Option C]
- Recommendation: [Which one and why?]
- Timeline: [When needed?]

SECTION 8: ATTACHMENTS
- Risk register update
- Change log
- Detailed metrics
```

**Uso**: Copiar template cada viernes y llenar  
**Enviar a**: Steering Committee

---

#### **STEERING_MEETING_NOTES.docx**
**Formato**: DOCX  
**Frecuencia**: After every steering meeting (bi-weekly)

**Tabla de Contenidos**:
```
HEADER:
- Meeting Date
- Attendees: [List names]
- PM: [Name]

AGENDA ITEMS:
1. Project Status Review
   - Current status summary
   - Key metrics
   - Accomplishments

2. Risks & Issues
   - Open risks discussed
   - Issues escalated
   - Decisions made

3. Quality & Scope
   - Quality metrics
   - Any scope discussions
   - Decisions on changes

4. Financial Review
   - Budget status
   - Any cost issues
   - Forecast

5. Decisions Made & Approvals
   - Decision 1: [Approved / Rejected / Deferred]
   - Decision 2: [with date]
   - Sign-offs: ________________

6. Action Items
   Table: Action | Owner | Due Date | Status
   
7. Next Meeting
   - Date & Time
   - Agenda items to discuss
```

---

#### **LESSONS_LEARNED.md** ← DOCUMENTO VIVO
**Formato**: MARKDOWN (actualizar bi-semanalmente)  
**Propósito**: Capturar aprendizajes durante proyecto

**Estructura**:
```markdown
# Lessons Learned - [Project Name]

## Updated: [Date]

### Format for each lesson:
## Lesson #N: [Title]

**Context**: What happened?
**What went well?** 
- Point 1
- Point 2

**What could be improved?**
- Point 1
- Point 2

**Action for future projects**:
- [Specific action]
- [Document/template to update]

**Category**: Technical / Process / Team / Communication

---

## Lesson 1: Early Data Quality Issues Blocked Development
**Context**: Semana 3, intentamos iniciar integración pero datos de SCADA eran inconsistentes.

**What went well?**
- Detectamos el problema early (no esperamos a UAT)
- Equipo se movilizó rápido para investigar

**What could be improved?**
- Data quality audit debería ser pre-requisito antes de iniciar desarrollo
- Risk register should have included "data quality" as higher priority

**Action for future projects**:
- Add "Data Quality Assessment" as Phase 0 prerequisite
- Include in risk register from start
- Create data quality checklist template

**Category**: Process

---

## Lesson 2: Daily Standups Improved Communication
**Context**: Implementamos daily standups en semana 2.

**What went well?**
- Team cohesion mejoró
- Issues encontrados más rápido
- Blockers resueltos en < 1 día (vs previous 3 días)

**Action for future projects**:
- Start daily standups from day 1
- Include in kick-off meeting

**Category**: Communication
```

**Actualización**: Bi-weekly (después de retrospectives)  
**Owner**: Project Manager

---

### 3.2 ARCHITECTURE_DECISIONS Folder

#### **ADR_NNNN_[Decision Name].md**
**Formato**: MARKDOWN (vivo en control de versiones)  
**Propósito**: Documentar decisiones técnicas importantes

**Estructura**:
```markdown
# ADR-0001: Use PostgreSQL instead of MongoDB

**Status**: ACCEPTED (other options: PROPOSED, REJECTED)  
**Date**: 2026-02-15  
**Owner**: Tech Lead  

## Problem Statement
We need a relational database that...
[Context and problem description]

## Solution
We chose PostgreSQL because...

### Why PostgreSQL?
- ACID compliance guaranteed
- Complex SQL queries needed
- Team expertise in SQL
- Better for our data model

### Why NOT MongoDB?
- Document store too flexible for our schema
- Complex joins would be inefficient
- Team less experienced with NoSQL

### Why NOT Oracle?
- Licensing costs too high ($XXX)
- Overkill for our scale

## Consequences
✓ Positive: Can write complex queries, strong consistency  
✗ Negative: Less flexible for schema changes, must plan migrations

## Alternatives Considered
1. MongoDB - flexibility vs complexity trade-off
2. Oracle - cost prohibitive
3. SQLite - too lightweight for production

## Rationale
Selected PostgreSQL for optimal balance of power, cost, and team capability.

## Follow-up
- Ensure team trained on PostgreSQL optimization
- Plan for migration if ever needed
- Document connection pooling strategy

---

## Related ADRs
- ADR-0002: Use Kubernetes for deployment
- ADR-0003: Use Python for data processing

## References
- PostgreSQL documentation: [link]
- Database selection criteria: [link]
```

**Actualización**: Cuando se toma una decisión técnica importante  
**Versionado**: En Git (history preserved)

---

### 3.3 REQUIREMENTS Folder

#### **FUNCTIONAL_REQUIREMENTS.docx / .pdf**
**Formato**: DOCX (editable) → PDF (final)  
**Propósito**: Especificar exactamente QUÉ construir

**Tabla de Contenidos**:
```
1. OVERVIEW
   - Project name
   - What the system does (1 paragraph)
   - Key capabilities
   
2. FUNCTIONAL REQUIREMENTS BY MODULE
   
   ### Module 1: [Name]
   
   **FR-1.1: Feature Description**
   - [Feature description]
   - User can: [action 1], [action 2]
   - System shall: [requirement 1], [requirement 2]
   - Acceptance Criteria:
     * Criterion 1
     * Criterion 2
   
   **FR-1.2: Another Feature**
   - [Similar structure]
   
3. USER WORKFLOWS
   - Step-by-step walkthroughs
   - Screenshots / mockups
   - Use cases described
   
4. DATA REQUIREMENTS
   - Data inputs
   - Data outputs
   - Data transformations
   
5. INTEGRATION POINTS
   - System A integration points
   - System B integration points
   - APIs to expose
   
6. ASSUMPTIONS & CONSTRAINTS
   - Assumptions made
   - Technical constraints
   - Business constraints
```

**Ejemplo de FR**:
```
**FR-2.3: Real-time Alert Display**

When storage usage exceeds 85%, the system shall:
1. Display a red alert box in the UI
2. Play a 3-second audible alert
3. Log the alert to system event log
4. Send email notification to Operations team

Acceptance Criteria:
- Alert appears within 2 seconds of threshold breach
- Alert persists until acknowledged by user
- Email sent within 5 minutes
- Log entry includes: timestamp, storage location, usage %
```

---

#### **NON_FUNCTIONAL_REQUIREMENTS.docx**
**Formato**: DOCX  
**Propósito**: Performance, seguridad, escalabilidad, etc.

**Tabla de Contenidos**:
```
1. PERFORMANCE
   - Response time: < 2 seconds for 95% of requests
   - Throughput: Support 10,000 concurrent users
   - Data processing: < 1 minute for daily batch
   
2. RELIABILITY & AVAILABILITY
   - Uptime: 99.5% SLA
   - Mean Time To Recovery (MTTR): < 15 minutes
   - Backup frequency: Daily
   
3. SECURITY
   - Authentication: LDAP/AD integration
   - Authorization: Role-based access control
   - Encryption: TLS 1.2+ for transmission, AES-256 at rest
   - Audit trail: All operations logged
   
4. SCALABILITY
   - Must support growth to 5000+ users
   - Must handle 100x current data volume
   - Horizontal scaling capability
   
5. MAINTAINABILITY
   - Code documented
   - Architecture documented
   - Runbooks for operations
   
6. COMPLIANCE
   - GDPR compliant
   - SOC 2 certified
   - Data retention: 90+ days
   
7. USABILITY
   - Intuitive UI (Nielsen heuristics)
   - Training time: < 2 hours
   - Help documentation
```

---

#### **USER_STORIES.xlsx**
**Formato**: EXCEL (tracker)  
**Propósito**: Cada historia de usuario con criterios

**Estructura**:
```
COLUMNS:
A. Story ID (US-001, US-002...)
B. Title (short)
C. User Story (As a [role], I want [action], so that [benefit])
D. Priority (Must Have / Should Have / Could Have / Won't Have)
E. Acceptance Criteria (list)
F. Estimated Effort (story points)
G. Status (Backlog / In Progress / Done / Blocked)
H. Owner (Developer)
I. Target Sprint
J. Notes

EXAMPLE ROW:
US-101 | Display Alert | As an operator, I want to see real-time storage alerts, so that I can respond quickly | Must Have | 
   - Alert displays when usage > 85%
   - Alert persists until dismissed
   - Color coded (red = critical)
   | 5 | In Progress | John Doe | Sprint 3 | Tests written

TOTAL STORIES: 30-50 por proyecto pequeño, 100+ para grandes
```

**Actualización**: Durante planning & execution  
**Owner**: Product Owner

---

### 3.4 TESTING Folder

#### **TEST_PLAN.docx**
**Formato**: DOCX  
**Propósito**: Estrategia de testing

**Tabla de Contenidos**:
```
1. TEST STRATEGY
   - Levels of testing (Unit, Integration, System, UAT)
   - Types (Functional, Performance, Security, Usability)
   - Approach (risk-based, comprehensive)
   
2. TEST SCOPE
   - What will be tested
   - What won't be tested
   
3. TEST ENVIRONMENT
   - Development environment specs
   - Staging environment specs
   - Production environment specs
   
4. TEST SCHEDULE
   - Phase 1 (Unit testing): Semana 1-3
   - Phase 2 (Integration): Semana 4-5
   - Phase 3 (System): Semana 6-7
   - Phase 4 (UAT): Semana 8
   
5. DEFECT SEVERITY LEVELS
   - Critical: System down
   - High: Major feature broken
   - Medium: Feature degraded
   - Low: Minor issue
   
6. EXIT CRITERIA
   - All critical/high issues resolved
   - 95%+ test pass rate
   - Performance meets NFRs
   - Security testing passed
   
7. ROLES & RESPONSIBILITIES
   - QA Lead: Overall testing coordination
   - Developers: Unit testing
   - QA Engineers: Test execution
   - Users: UAT testing
   
8. TESTING TOOLS
   - Unit testing: Google Test
   - Automation: Selenium / Cucumber
   - Performance: JMeter
   - Manual testing: Excel
```

---

#### **TEST_CASES.xlsx** ← DOCUMENTO VIVO
**Formato**: EXCEL (tracker)  
**Propósito**: Cada test case con pasos y resultados

**Estructura**:
```
COLUMNS:
A. Test Case ID (TC-001, TC-002...)
B. Module/Feature
C. Test Description
D. Preconditions
E. Test Steps (numbered)
F. Expected Result
G. Actual Result
H. Status (Pass / Fail / Blocked)
I. Date Executed
J. Executed By
K. Defects Found (if any)
L. Notes

EXAMPLE:
TC-101 | Alert Display | Verify alert shows when storage > 85% |
   Precondition: System running, storage at 80%
   Steps:
   1. Add data to storage
   2. Monitor until 85% threshold
   3. Check UI for alert
   Expected: Red alert appears, sound plays
   Actual: [Filled during execution]
   Status: Pass
   Date: 2026-02-15
   Executed By: QA1

TOTAL CASES: 100-300+ depending on scope
```

**Actualización**: Daily during testing phases  
**Owner**: QA Lead

---

#### **TEST_RESULTS.xlsx** ← DOCUMENTO VIVO
**Formato**: EXCEL  
**Propósito**: Resumen de testing por semana

**Estructura**:
```
WEEKLY TESTING SUMMARY

Week of: [Date]

UNIT TESTING:
- Total tests run: XXX
- Passed: YYY (Z%)
- Failed: W
- Blocked: V
- Code coverage: XX%

INTEGRATION TESTING:
- Total tests: XXX
- Passed: YYY (Z%)
- Failed: W
- Critical issues: V

SYSTEM TESTING:
- Total tests: XXX
- Passed: YYY (Z%)
- Failed: W
- Performance issues: V

ISSUES FOUND THIS WEEK:
Table:
| Issue ID | Severity | Description | Owner | Target Fix |
|----------|----------|-------------|-------|-----------|
| BUG-001 | Critical | Alert not showing | Dev1 | Fri |
| BUG-002 | High | Slow performance | Dev2 | Next Wed |

EXIT CRITERIA STATUS:
- ☑ All critical issues closed
- ☑ Pass rate > 95%
- ☑ Performance acceptable
- ☑ Security review done
- Status: ON TRACK for UAT next week
```

**Actualización**: Diaria/Semanal  
**Owner**: QA Lead

---

### 3.5 OPERATIONS Folder

#### **RUNBOOK.md**
**Formato**: MARKDOWN (copy-paste ready)  
**Propósito**: Instrucciones step-by-step para Ops

**Estructura**:
```markdown
# Runbook - [System Name]

## Daily Operations

### 1. Morning Health Check (8:00 AM)

☐ SSH to production server
$ ssh ops-user@prod-server-01

☐ Check system status
$ sudo systemctl status mtell_service
Expected: ● mtell_service.service - Enabled and running

☐ Check disk space
$ df -h
Expected: / partition > 20% free

☐ Check memory
$ free -m
Expected: Available > 2GB

☐ Check error logs for last 24h
$ tail -n 100 /var/log/mtell/error.log
Action if errors: Investigate and escalate if needed

### 2. Backup Verification (4:00 PM)

☐ Check backup log
$ tail -n 50 /var/log/backup.log
Expected: "Backup completed successfully"

☐ Verify backup file size
$ ls -lh /backups/daily/
Expected: File size > [expected size]

### 3. Weekly Maintenance (Friday 6:00 PM)

[Procedures]

## Troubleshooting

### Problem: High CPU Usage (>80%)

**Step 1: Identify the process**
$ top -b -n 1 | head -15
Look for what's consuming CPU

**Step 2: Check logs**
$ tail -n 100 /var/log/mtell/app.log
Look for errors or warnings

**Step 3: Actions**
If mtell process:
  - Option A: Restart service
    $ sudo systemctl restart mtell_service
  - Option B: Scale up resources
    $ kubectl scale deployment mtell --replicas=2
    
If system process:
  - Contact infrastructure team

**Step 4: Monitor**
$ watch -n 5 'top -b -n 1 | head -15'
Watch for 10 minutes

If problem continues → ESCALATE (see below)

### Problem: Database Connection Timeout

[Similar troubleshooting steps]

## ESCALATION

If problem NOT RESOLVED in 30 minutes:

Contact: [Name] ([Phone])
Backup: [Name2] ([Phone2])

Message: "Mtell service having [issue], impact: [what's broken]"
```

---

#### **PLAYBOOK.md**
**Formato**: MARKDOWN  
**Propósito**: Crisis response procedures

**Estructura**:
```markdown
# Playbook - [System Name]

## Scenario 1: System Down (Complete Outage)

**Alert**: Application not responding

### Immediate Actions (First 5 minutes)

1. Page on-call engineer
2. Establish war room (Teams channel: #incident-mtell)
3. Post status: "Investigating service availability"

### Investigation (Next 10 minutes)

```bash
# Check service status
systemctl status mtell_service

# Check server health
free -m
df -h
ps aux | grep mtell

# Check logs
tail -n 50 /var/log/mtell/error.log

# Check database
mysql -u admin -p
> SELECT * FROM health_check;
```

### Resolution (options in order)

**Option A: Restart service (5 min)**
```bash
systemctl restart mtell_service
sleep 30
systemctl status mtell_service
```

If fixed: ✓ Skip to "Communicate"

**Option B: Check database (10 min)**
- If DB connection lost → DBA team needed
- If DB corrupted → Restore from backup

**Option C: Failover to standby (15 min)**
- Switch DNS to standby server
- Verify all systems online

### If NOT resolved in 30 min

1. Call vendor support
2. Escalate to [Director Name]
3. Prepare for rollback

### Post-Incident

1. Document what happened
2. Identify root cause
3. Create preventive action
4. Update this playbook if needed

## Scenario 2: High Error Rate

[Similar structure]

## Scenario 3: Data Corruption

[Similar structure]
```

---

#### **DEPLOYMENT_CHECKLIST.xlsx**
**Formato**: EXCEL (checkbox interactive)  
**Propósito**: Validar antes y después de deployment

**Estructura**:
```
DEPLOYMENT CHECKLIST - [Date]

PRE-DEPLOYMENT (24h before)

☐ All tests passing (>95%)
☐ Backup created and verified
☐ Rollback procedure tested
☐ Communication plan executed
  ☐ Email to stakeholders
  ☐ Teams notification posted
☐ Deployment window scheduled (off-peak)
☐ Stakeholders confirmed availability
☐ Database migrations prepared

DEPLOYMENT EXECUTION

☐ Take system offline (or in maintenance mode)
☐ Stop all services
☐ Backup current state
☐ Deploy new version
☐ Run database migrations
☐ Verify deployment success
  ☐ Check logs for errors
  ☐ Run health check scripts
  ☐ Test critical functionality

POST-DEPLOYMENT (1h after)

☐ Smoke test: [list basic tests]
  ☐ Login works
  ☐ Main feature works
  ☐ Data integrity verified
☐ Performance acceptable
  ☐ Page load time < 2 sec
  ☐ No memory leaks
☐ Error logs clean
☐ Stakeholders notified of success
☐ Monitor closely for 24h

ROLLBACK PROCEDURES (if needed)

If major issue detected:
1. Stop services
2. Restore from backup (created in PRE-DEPLOYMENT step)
3. Restart services
4. Verify
5. Notify stakeholders
6. Document incident
```

---

### 3.6 COMPLIANCE Folder

#### **SECURITY_CHECKLIST.xlsx**
**Formato**: EXCEL (interactive)  
**Propósito**: Validar cumplimiento de seguridad

**Estructura**:
```
SECURITY COMPLIANCE CHECKLIST

PROJECT: [Name]
DATE: [When completed]
REVIEWER: [Who checked]

AUTHENTICATION & AUTHORIZATION
☐ LDAP/AD integration configured
☐ Password policy enforced
☐ MFA enabled for admin users
☐ Session timeout: 8 hours
☐ Role-based access control implemented
☐ User provisioning process documented

DATA PROTECTION
☐ Encryption in transit (TLS 1.2+)
☐ Encryption at rest (AES-256)
☐ Key management procedures documented
☐ Database access restricted to IP whitelist
☐ Secrets not stored in code

NETWORK & INFRASTRUCTURE
☐ Firewall rules defined
☐ Only necessary ports open
☐ VPN required for remote access
☐ Network segmentation in place
☐ DDoS mitigation configured

LOGGING & AUDIT
☐ All access logged
☐ Audit trail retention: 90+ days
☐ Log centralization configured
☐ Alerts for suspicious activity
☐ Backup logs secured

COMPLIANCE & GOVERNANCE
☐ GDPR compliance validated
☐ Data retention policy defined
☐ Data deletion process documented
☐ Breach notification plan
☐ Vendor security assessments done

TESTING & VALIDATION
☐ Penetration testing completed
☐ Vulnerability scan passed
☐ Code security review done
☐ No critical issues remaining

SIGN-OFF
Security Officer: _________________ Date: _____
IT Director: _________________ Date: _____
```

---

## 4. RESUMEN DE ARCHIVOS POR CARPETA

| Carpeta | # de Docs | Formatos | Vivos? | Frecuencia |
|---------|-----------|----------|--------|-----------|
| project_management | 8 | DOCX, XLSX, PDF, MD | Sí (Risk, Change, Status) | Semanal |
| architecture_decisions | 2+ | MD | Sí (cuando hay decisiones) | Ad-hoc |
| requirements | 4 | DOCX, XLSX, MD | No (freezed) | Initial |
| testing | 4 | DOCX, XLSX | Sí (TEST_RESULTS) | Diaria |
| operations | 5 | MD, XLSX, PDF | Sí (Runbook updates) | Mantenimiento |
| compliance | 2 | DOCX, XLSX, PDF | Sí (Security) | Validación |
| training | 5 | DOCX, PDF, MD | No (finalizado) | Initial |
| stakeholder_communication | 4 | PPTX, DOCX, PDF, XLSX | Sí (reportes mensuales) | Semanal |
| deliverables | 3 | DOCX, PDF | No (freezed) | Final |
| financial | 3 | XLSX, PDF | Sí (Budget tracking) | Mensual |
| diagrams | 4 | VSDX, PDF, PNG, MD | No | Design |
| knowledge_base | 4 | MD, XLSX, DOCX | No | Reference |
| archive | Variable | PDF | No | Histórico |

---

## 5. FLUJO DE DOCUMENTOS A LO LARGO DEL PROYECTO

```
WEEK 1-2 (PLANNING)
├─ Create: PROJECT_CHARTER.docx
├─ Create: SCOPE_STATEMENT.docx
├─ Create: STAKEHOLDER_MANAGEMENT.docx
├─ Create: RACI_MATRIX.xlsx
├─ Create: RISK_REGISTER.xlsx
└─ Create: COMMUNICATIONS_PLAN.docx

WEEK 3-4
├─ Create: FUNCTIONAL_REQUIREMENTS.docx
├─ Create: NON_FUNCTIONAL_REQUIREMENTS.docx
├─ Create: TEST_PLAN.docx
├─ Create: ADR_0001.md, ADR_0002.md, etc.
└─ Update: RISK_REGISTER.xlsx (viernes)

WEEK 5-8 (EXECUTION)
├─ Update: RISK_REGISTER.xlsx (weekly)
├─ Update: CHANGE_LOG.xlsx (as needed)
├─ Create: WEEKLY_STATUS.docx (every viernes)
├─ Update: TEST_RESULTS.xlsx (daily)
├─ Create: TEST_CASES.xlsx (fill with test data)
├─ Append: LESSONS_LEARNED.md (bi-weekly)
└─ Update: RUNBOOK.md (if procedures change)

WEEK 9-10 (UAT)
├─ Create: UAT_PLAN.docx
├─ Update: TEST_RESULTS.xlsx (daily, many tests)
├─ Create: ACCEPTANCE_SIGN_OFF.docx
└─ Create: GO_LIVE_REPORT.pdf

WEEK 11 (GO-LIVE)
├─ Update: DEPLOYMENT_CHECKLIST.xlsx
├─ Execute: RUNBOOK.md (procedures)
├─ Create: GO_LIVE_REPORT.pdf
├─ Archive: Older documents to /archive/
└─ Create: FINAL_ACCOUNTING.pdf

ONGOING (POST-GO-LIVE)
├─ Monthly: MONTHLY_REPORT.pdf
├─ Weekly: RISK_REGISTER.xlsx updates
├─ As-needed: CHANGE_LOG.xlsx updates
├─ As-needed: RUNBOOK.md updates
├─ Bi-weekly: LESSONS_LEARNED.md updates
└─ Final: Closure documents & LessonsLearned
```

---

## 6. GUARDAR DOCUMENTOS: DÓNDE Y CÓMO

### Documentos EN CONTROL DE VERSIONES (Git)
```
├─ docs/
   ├─ ADR_*.md              ✓ Git (history important)
   ├─ RUNBOOK.md           ✓ Git (procedures evolve)
   ├─ LESSONS_LEARNED.md   ✓ Git (iterative)
   └─ *.md (todos MD)      ✓ Git
```

### Documentos EN COMPARTIDOS (Teams / SharePoint)
```
├─ project_management/
│  ├─ *.docx              ✓ Shared (collaboration)
│  ├─ *.xlsx              ✓ Shared (live tracking)
│  └─ *.pdf               ✓ Archive (freezed versions)
```

### Documentos EN SERVIDOR LOCAL (respaldo)
```
├─ /docs/archive/
   └─ Final versions of all documents
```

---

**Documento Control**: DOCUMENTATION_CONTENT_SPEC-v2.0  
**Owner**: PMO / Documentation Lead  
**Próxima Revisión**: As-needed (when new doc types emerge)
