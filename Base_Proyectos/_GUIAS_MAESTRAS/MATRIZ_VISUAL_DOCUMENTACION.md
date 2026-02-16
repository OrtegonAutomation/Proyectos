# MATRIZ VISUAL: DOCUMENTACIÓN POR CARPETA Y FORMATO

**Referencia Rápida**: Qué documento va en cada carpeta, en qué formato, con qué contenido

---

## MATRIZ COMPLETA DE DOCUMENTACIÓN

```
┌─ PROJECT MANAGEMENT ────────────────────────────────────────────────────────────┐
│ Gobernanza, autorización, tracking PMI                                         │
├────────────────────────────────────────────────────────────────────────────────┤
│ 📄 PROJECT_CHARTER                      [DOCX → PDF]  🔒 Firmado, formal       │
│    ├─ Autorización oficial del proyecto                                        │
│    ├─ Objetivos SMART, valor, ROI                                              │
│    ├─ Sponsor signature, Team, Constraints                                     │
│    └─ Firmado por: Sponsor, PM, IT Director                                    │
│                                                                                 │
│ 📄 SCOPE_STATEMENT                      [DOCX]       (No signature needed)      │
│    ├─ Qué está IN (detallado)                                                   │
│    ├─ Qué está OUT (explícito)                                                  │
│    ├─ Constraints & Assumptions                                                │
│    └─ Acceptance Criteria                                                      │
│                                                                                 │
│ 📄 STAKEHOLDER_MANAGEMENT               [DOCX + XLSX] (Matriz de tracking)     │
│    ├─ Quiénes son stakeholders                                                  │
│    ├─ Análisis: Poder × Interés                                                │
│    ├─ Estrategia engagement por grupo                                          │
│    └─ Communication matrix: quién-qué-cuándo                                   │
│                                                                                 │
│ 📊 RACI_MATRIX                          [XLSX]       (Responsabilidades)        │
│    ├─ A = Accountable (authority)                                              │
│    ├─ R = Responsible (does work)                                              │
│    ├─ C = Consulted (input)                                                     │
│    └─ I = Informed (kept in loop)                                              │
│                                                                                 │
│ 📊 RISK_REGISTER         ← VIVO ✓       [XLSX]       (Actualizar SEMANALMENTE) │
│    ├─ Risk ID | Description | Category                                        │
│    ├─ Probability | Impact | Priority                                         │
│    ├─ Owner | Mitigation | Status                                             │
│    └─ Actualización: Viernes 10am                                             │
│                                                                                 │
│ 📊 CHANGE_LOG            ← VIVO ✓       [XLSX]       (Ad-hoc, cuando cambios)  │
│    ├─ Change ID | Description | Reason                                        │
│    ├─ Impact Area (Scope/Schedule/Budget)                                     │
│    ├─ Status: Requested/Approved/Implemented                                 │
│    └─ Regla: SIN aprobación en este log = SIN implementación                  │
│                                                                                 │
│ 📄 COMMUNICATIONS_PLAN                  [DOCX]       (Inicial, no cambia)      │
│    ├─ Objetivos comunicación                                                    │
│    ├─ Stakeholder × Message × Format × Frequency                              │
│    ├─ Meeting cadence (daily, weekly, bi-weekly)                              │
│    └─ Escalation communication                                                │
│                                                                                 │
│ 📄 WEEKLY_STATUS_TEMPLATE  ← VIVO ✓     [DOCX]       (Template para llenar)    │
│    ├─ Executive summary (1 párrafo)                                           │
│    ├─ Metrics vs Baseline                                                      │
│    ├─ Accomplishments / In Progress / Blockers                                │
│    ├─ Plan for next week                                                       │
│    └─ Decisions required                                                      │
│    ╰─ Enviado: Cada VIERNES a Steering Committee                              │
│                                                                                 │
│ 📄 STEERING_MEETING_NOTES               [DOCX]       (Después de cada mtg)     │
│    ├─ Attendees, date, agenda                                                  │
│    ├─ Status reviewed, decisions made                                         │
│    ├─ Action items with owners & due dates                                    │
│    └─ Sign-offs: _________________ Fecha: _____                               │
│                                                                                 │
│ 📝 LESSONS_LEARNED       ← VIVO ✓       [MARKDOWN]   (Actualizar bi-weekly)    │
│    ├─ What went well + future replication                                     │
│    ├─ What could improve + corrective action                                  │
│    ├─ Category: Technical / Process / Team / Communication                    │
│    └─ Actualización: Después de retrospectives                                │
│                                                                                 │
│ 📁 MONTHLY_REPORTS/ (carpeta histórica)                                        │
│    ├─ STATUS_REPORT_JAN_2026.pdf                                              │
│    ├─ STATUS_REPORT_FEB_2026.pdf                                              │
│    └─ STEERING_NOTES_JAN_20.docx                                              │
└────────────────────────────────────────────────────────────────────────────────┘

┌─ ARCHITECTURE_DECISIONS ───────────────────────────────────────────────────────┐
│ Documentar PORQUÉ se tomó cada decisión técnica importante                    │
├────────────────────────────────────────────────────────────────────────────────┤
│ 📝 ADR_0001_[Decision]                  [MARKDOWN]   (En Git para history)     │
│ 📝 ADR_0002_[Decision]                  [MARKDOWN]   (Decisions preserved)     │
│ 📝 ADR_0003_[Decision]                  [MARKDOWN]   (Future reference)        │
│    ├─ Problem statement                                                        │
│    ├─ Solution chosen + Why                                                    │
│    ├─ Alternatives considered + Why NOT                                       │
│    ├─ Consequences: ✓ Positives | ✗ Negatives                                 │
│    └─ Status: ACCEPTED / PROPOSED / REJECTED                                  │
│                                                                                 │
│ ℹ️  Ejemplos:                                                                   │
│    - ADR-0001: Use PostgreSQL instead of MongoDB                              │
│    - ADR-0002: Use Kubernetes for deployment                                  │
│    - ADR-0003: Use Python for data processing                                 │
└────────────────────────────────────────────────────────────────────────────────┘

┌─ REQUIREMENTS ─────────────────────────────────────────────────────────────────┐
│ Especificaciones funcionales y no-funcionales (QUÉ construir)                 │
├────────────────────────────────────────────────────────────────────────────────┤
│ 📄 FUNCTIONAL_REQUIREMENTS               [DOCX → PDF]  (Formal, freezed)       │
│    ├─ FR-1.1 Feature 1                                                         │
│    │   ├─ Description                                                          │
│    │   ├─ User can: [action 1], [action 2]                                    │
│    │   ├─ System shall: [requirement 1], [requirement 2]                      │
│    │   └─ Acceptance Criteria: [list]                                         │
│    │                                                                            │
│    ├─ FR-1.2 Feature 2                                                         │
│    ├─ FR-1.3 Feature 3                                                         │
│    └─ ... (10-50+ features depending on scope)                                │
│                                                                                 │
│ 📄 NON_FUNCTIONAL_REQUIREMENTS           [DOCX]       (Performance, Security)  │
│    ├─ Performance: Response time < 2 sec, throughput 10K users                │
│    ├─ Reliability: 99.5% uptime SLA, MTTR < 15 min                           │
│    ├─ Security: LDAP/AD, RBAC, TLS 1.2+, AES-256 encryption                  │
│    ├─ Scalability: Support 5000+ users, 100x data volume                      │
│    ├─ Maintainability: Code documented, runbooks provided                     │
│    └─ Compliance: GDPR, SOC 2, 90-day retention                              │
│                                                                                 │
│ 📊 USER_STORIES                         [XLSX]       (Live tracker)            │
│    ├─ US-001 | As a [role], I want [action], so that [benefit]               │
│    │   ├─ Priority: Must Have / Should / Could / Won't                       │
│    │   ├─ Acceptance Criteria: [list]                                        │
│    │   ├─ Estimated effort: [story points]                                   │
│    │   └─ Status: Backlog / In Progress / Done / Blocked                     │
│    │                                                                            │
│    └─ (30-50+ stories for typical project)                                    │
│                                                                                 │
│ 📝 ACCEPTANCE_CRITERIA                  [MARKDOWN]   (Vivo en repo)            │
│    └─ Criterios claros para cada entrega major                                │
└────────────────────────────────────────────────────────────────────────────────┘

┌─ TESTING ──────────────────────────────────────────────────────────────────────┐
│ Planes, casos, y resultados de testing                                        │
├────────────────────────────────────────────────────────────────────────────────┤
│ 📄 TEST_PLAN                            [DOCX]       (Estrategia, inicial)     │
│    ├─ Test strategy (Levels, Types, Approach)                                 │
│    ├─ Test scope (What will / won't be tested)                                │
│    ├─ Test environment specs                                                   │
│    ├─ Schedule: Unit (W1-3) → Integration (W4-5) → System (W6-7) → UAT (W8)  │
│    ├─ Defect severity levels                                                   │
│    ├─ Exit criteria (all critical resolved, 95%+ pass rate, performance OK)   │
│    └─ Roles: QA Lead, Developers, QA Engineers, Users                        │
│                                                                                 │
│ 📊 TEST_CASES                           [XLSX]       (Tracker de casos)        │
│    ├─ TC-001 | Module | Description | Preconditions                          │
│    │   ├─ Test Steps (numbered)                                               │
│    │   ├─ Expected Result                                                      │
│    │   ├─ Actual Result [filled during execution]                            │
│    │   └─ Status: Pass / Fail / Blocked                                      │
│    │                                                                            │
│    └─ (100-300+ test cases depending on scope)                                │
│                                                                                 │
│ 📄 UAT_PLAN                             [DOCX]       (User Acceptance)         │
│    ├─ User acceptance testing strategy                                        │
│    ├─ Test scenarios from user perspective                                    │
│    ├─ UAT participants                                                         │
│    └─ Sign-off procedures                                                      │
│                                                                                 │
│ 📊 TEST_RESULTS                ← VIVO ✓ [XLSX]       (Actualizar DIARIA/SEMA)  │
│    ├─ Weekly testing summary                                                   │
│    ├─ Unit testing: Total / Passed / Failed / Coverage                        │
│    ├─ Integration testing: Total / Passed / Failed / Critical issues          │
│    ├─ System testing: Total / Passed / Failed / Performance issues            │
│    │                                                                            │
│    ├─ Issues found this week:                                                 │
│    │   └─ Table: Issue ID | Severity | Description | Owner | Target Fix     │
│    │                                                                            │
│    └─ Exit criteria status: ON TRACK / AT RISK / BLOCKED                     │
└────────────────────────────────────────────────────────────────────────────────┘

┌─ OPERATIONS ────────────────────────────────────────────────────────────────────┐
│ Documentación operacional (para IT/Operations después de go-live)             │
├────────────────────────────────────────────────────────────────────────────────┤
│ 📝 RUNBOOK                    ← VIVO ✓   [MARKDOWN]   (Copy-paste ready)       │
│    ├─ Daily operations (Health check, Backups, etc)                           │
│    │   └─ Step-by-step WITH COMMANDS ready to execute                        │
│    │                                                                            │
│    ├─ Weekly maintenance procedures                                            │
│    │                                                                            │
│    └─ Troubleshooting section                                                  │
│        ├─ Problem: High CPU Usage                                              │
│        │   └─ Step 1 → Step 2 → Step 3 → Escalate if not fixed               │
│        │                                                                        │
│        └─ Problem: Database Connection Timeout                                │
│            └─ [Similar structure]                                             │
│                                                                                 │
│ 📝 PLAYBOOK                             [MARKDOWN]   (Crisis procedures)       │
│    ├─ Scenario 1: System Down                                                 │
│    │   ├─ Immediate actions (Page engineer, establish war room)               │
│    │   ├─ Investigation commands (check status, logs, etc)                    │
│    │   ├─ Resolution options in order (restart → check DB → failover)        │
│    │   └─ If not resolved in 30 min → Call vendor, escalate                  │
│    │                                                                            │
│    ├─ Scenario 2: High Error Rate                                             │
│    └─ Scenario 3: Data Corruption                                             │
│                                                                                 │
│ 📊 DEPLOYMENT_CHECKLIST                 [XLSX]       (Interactive checkpoints) │
│    ├─ PRE-DEPLOYMENT (24h before)                                             │
│    │   ├─ ☐ All tests passing                                                 │
│    │   ├─ ☐ Backup created & verified                                        │
│    │   ├─ ☐ Rollback procedure tested                                        │
│    │   └─ ☐ Communication plan executed                                       │
│    │                                                                            │
│    ├─ DEPLOYMENT EXECUTION                                                     │
│    │   ├─ ☐ Take system offline                                              │
│    │   ├─ ☐ Stop services                                                     │
│    │   ├─ ☐ Deploy new version                                               │
│    │   └─ ☐ Run database migrations                                          │
│    │                                                                            │
│    └─ POST-DEPLOYMENT (1h after)                                              │
│        ├─ ☐ Smoke test: [list basic tests]                                   │
│        ├─ ☐ Performance acceptable                                            │
│        └─ ☐ Error logs clean                                                  │
│                                                                                 │
│ 📝 TROUBLESHOOTING_GUIDE                [MARKDOWN]   (Common issues & fixes)   │
│    └─ Quick reference for Ops team                                            │
│                                                                                 │
│ 📄 SLA_DEFINITION                       [DOCX]       (Service Level Agreements)│
│    ├─ Uptime SLA: 99.5%                                                       │
│    ├─ MTTR (Mean Time To Recover): < 15 minutes                              │
│    └─ Support hours: 24/7 / Business hours / On-call                         │
│                                                                                 │
│ 📊 MAINTENANCE_SCHEDULE                 [XLSX]       (Planned maintenance)     │
│    ├─ Weekly maintenance windows                                               │
│    ├─ Monthly maintenance tasks                                                │
│    └─ Quarterly maintenance schedule                                          │
└────────────────────────────────────────────────────────────────────────────────┘

┌─ COMPLIANCE ────────────────────────────────────────────────────────────────────┐
│ Seguridad y cumplimiento regulatorio                                          │
├────────────────────────────────────────────────────────────────────────────────┤
│ 📄 SECURITY_POLICY                      [DOCX]       (Formal policy)           │
│    ├─ Password policy                                                          │
│    ├─ Access to sensitive data                                                 │
│    ├─ VPN/remote work policy                                                   │
│    └─ Incident reporting procedures                                           │
│                                                                                 │
│ 📊 SECURITY_CHECKLIST       ← VIVO ✓    [XLSX]       (Validación de comply)   │
│    ├─ Authentication & Authorization                                          │
│    │   ├─ ☐ LDAP/AD integration configured                                   │
│    │   ├─ ☐ MFA enabled for admins                                           │
│    │   └─ ☐ RBAC implemented                                                  │
│    │                                                                            │
│    ├─ Data Protection                                                          │
│    │   ├─ ☐ Encryption in transit (TLS 1.2+)                                 │
│    │   ├─ ☐ Encryption at rest (AES-256)                                     │
│    │   └─ ☐ Secrets not in code                                              │
│    │                                                                            │
│    ├─ Network & Infrastructure                                                │
│    │   ├─ ☐ Firewall rules defined                                           │
│    │   └─ ☐ Network segmentation                                             │
│    │                                                                            │
│    ├─ Logging & Audit                                                         │
│    │   ├─ ☐ All access logged                                                │
│    │   └─ ☐ Audit trail: 90+ days retention                                  │
│    │                                                                            │
│    └─ Sign-off: Security Officer _________ IT Director _________             │
│                                                                                 │
│ 📄 COMPLIANCE_REQUIREMENTS               [DOCX]       (Regulatory)             │
│    ├─ GDPR compliance requirements                                             │
│    ├─ SOC 2 requirements                                                       │
│    ├─ Local regulations                                                        │
│    └─ Data retention requirements                                              │
│                                                                                 │
│ 📄 AUDIT_TRAIL_REQUIREMENTS              [MARKDOWN]   (Vivo en repo)           │
│    └─ Qué eventos loguear, retención, acceso a logs                           │
│                                                                                 │
│ 📄 SECURITY_TESTING_RESULTS              [PDF]        (Penetration test)       │
│    ├─ Vulnerabilities found                                                    │
│    ├─ Critical issues (resolved)                                              │
│    └─ Security sign-off                                                        │
└────────────────────────────────────────────────────────────────────────────────┘

┌─ TRAINING ─────────────────────────────────────────────────────────────────────┐
│ Materiales de capacitación para usuarios finales                             │
├────────────────────────────────────────────────────────────────────────────────┤
│ 📄 TRAINING_PLAN                        [DOCX]       (Strategy & schedule)     │
│    ├─ Training objectives                                                      │
│    ├─ Schedule: Date, Duration, Audience, Trainer                            │
│    ├─ Modules to cover                                                         │
│    └─ Success metrics                                                          │
│                                                                                 │
│ 📄 USER_MANUAL                          [PDF]        (For users)               │
│    ├─ How to use each feature                                                 │
│    ├─ Screenshots & step-by-step                                              │
│    └─ FAQ section                                                              │
│                                                                                 │
│ 📄 QUICK_START_GUIDE                    [PDF]        (One-page reference)      │
│    └─ Essential steps to get started                                          │
│                                                                                 │
│ 📄 ADMINISTRATOR_GUIDE                  [DOCX]       (For IT/Admins)           │
│    ├─ System administration                                                    │
│    ├─ User management                                                          │
│    ├─ Backups & maintenance                                                    │
│    └─ Troubleshooting                                                          │
│                                                                                 │
│ 📝 FAQ                                  [MARKDOWN]   (Vivo, add as Qs come)   │
│    └─ Preguntas frecuentes & respuestas                                        │
│                                                                                 │
│ 📁 VIDEO_TRANSCRIPTS/                                                          │
│    └─ Transcripción de videos de training (markdown)                          │
└────────────────────────────────────────────────────────────────────────────────┘

┌─ STAKEHOLDER_COMMUNICATION ────────────────────────────────────────────────────┐
│ Reportes, presentaciones, comunicaciones ejecutivas                           │
├────────────────────────────────────────────────────────────────────────────────┤
│ 📊 EXECUTIVE_SUMMARY_PRESENTATION       [PPTX]       (Para Sponsor & C-level)  │
│    ├─ Slide 1: Project status (GREEN / YELLOW / RED)                          │
│    ├─ Slide 2: Key metrics (Schedule, Budget, Quality)                        │
│    ├─ Slide 3: Accomplishments                                                 │
│    ├─ Slide 4: Risks & mitigation                                             │
│    ├─ Slide 5: Next steps                                                      │
│    └─ Slide 6: Questions?                                                      │
│                                                                                 │
│ 📊 WEEKLY_STATUS_PRESENTATION            [PPTX]       (Visual version)         │
│    ├─ Same structure as EXECUTIVE_SUMMARY but more detailed                  │
│    └─ Used in steering meetings                                               │
│                                                                                 │
│ 📄 STEERING_MEETING_AGENDA_TEMPLATE      [DOCX]       (Pre-meeting)            │
│    ├─ Meeting date & time                                                      │
│    ├─ Attendees expected                                                       │
│    ├─ Agenda items with time allocation                                       │
│    └─ Pre-reads / background materials                                        │
│                                                                                 │
│ 📁 MONTHLY_REPORTS/                     [PDF]        (Histórico)               │
│    ├─ REPORT_JAN_2026.pdf                                                      │
│    ├─ REPORT_FEB_2026.pdf                                                      │
│    └─ [Each month saved as PDF for archive]                                   │
│                                                                                 │
│ 📁 STEERING_NOTES/                      [DOCX]       (After each meeting)      │
│    ├─ Decisions made                                                           │
│    ├─ Action items with owners & dates                                        │
│    └─ Approvals obtained                                                       │
│                                                                                 │
│ 📊 RISK_ESCALATIONS                     [XLSX]       (Tracker)                 │
│    ├─ When: Risk Priority went HIGH or CRITICAL                              │
│    ├─ Who escalated to whom                                                    │
│    ├─ Resolution action                                                        │
│    └─ Close date                                                               │
└────────────────────────────────────────────────────────────────────────────────┘

┌─ DELIVERABLES ─────────────────────────────────────────────────────────────────┐
│ Documentos formales de aceptación (final)                                     │
├────────────────────────────────────────────────────────────────────────────────┤
│ 📄 PROJECT_CHARTER_SIGNED.pdf            [PDF]        (First document)         │
│    └─ Archivar original firmado                                               │
│                                                                                 │
│ 📄 ACCEPTANCE_SIGN_OFF.docx              [DOCX/PDF]   (Firma de aceptación)   │
│    ├─ Checklist que todo está completo                                        │
│    ├─ Testing results                                                          │
│    ├─ No outstanding critical issues                                          │
│    └─ Signatures: Tech Lead, PM, Sponsor, Users                              │
│                                                                                 │
│ 📄 GO_LIVE_REPORT.pdf                   [PDF]        (Reporte formal)          │
│    ├─ Date & time of go-live                                                  │
│    ├─ What was deployed                                                        │
│    ├─ Any issues encountered                                                   │
│    ├─ Resolution actions                                                       │
│    └─ Approvals & sign-off                                                    │
│                                                                                 │
│ 📄 DEPLOYMENT_VALIDATION_REPORT.pdf      [PDF]        (Post-deployment)        │
│    ├─ Validation checklist results                                            │
│    ├─ Performance metrics                                                      │
│    ├─ User feedback                                                            │
│    └─ Lessons learned for next release                                        │
│                                                                                 │
│ 📄 HANDOVER_PACKAGE.docx                 [DOCX]       (Para Operations)        │
│    ├─ System overview                                                          │
│    ├─ Links to all runbooks                                                    │
│    ├─ Contact list (support, escalation)                                      │
│    ├─ Known issues & workarounds                                              │
│    └─ 24/7 support procedures                                                 │
└────────────────────────────────────────────────────────────────────────────────┘

┌─ FINANCIAL ─────────────────────────────────────────────────────────────────────┐
│ Tracking financiero & presupuesto                                             │
├────────────────────────────────────────────────────────────────────────────────┤
│ 📊 BUDGET_BASELINE                       [XLSX]       (Initial budget)         │
│    ├─ Budget por categoría (Salarios, Software, Hardware, etc)               │
│    └─ Total aprobado                                                           │
│                                                                                 │
│ 📊 BUDGET_TRACKING          ← VIVO ✓    [XLSX]       (Actualizar mensual)     │
│    ├─ Columnas: Category | Baseline | Spent-YTD | Forecast | Variance       │
│    ├─ Monthly actuals vs budget                                               │
│    └─ Forecast vs final                                                        │
│                                                                                 │
│ 📊 BURN_DOWN_CHART          ← VIVO ✓    [XLSX]       (Visual tracking)         │
│    ├─ X-axis: Week                                                             │
│    ├─ Y-axis: $ remaining in budget                                           │
│    └─ Line: Should follow budget line vs actual                               │
│                                                                                 │
│ 📊 CHANGE_REQUEST_LOG                   [XLSX]       (Costo de cambios)        │
│    ├─ Change ID | Requested Date | Estimated Cost | Approved? | Actual Cost  │
│    └─ Total of all change requests                                            │
│                                                                                 │
│ 📄 FINAL_ACCOUNTING.pdf                 [PDF]        (Cierre)                 │
│    ├─ Total spent vs budget                                                    │
│    ├─ Variance analysis                                                        │
│    ├─ Cost per deliverable                                                     │
│    └─ ROI calculation                                                          │
└────────────────────────────────────────────────────────────────────────────────┘

┌─ DIAGRAMS ─────────────────────────────────────────────────────────────────────┐
│ Visuales: Arquitectura, flujos, integraciones                                 │
├────────────────────────────────────────────────────────────────────────────────┤
│ 🎨 SYSTEM_ARCHITECTURE                  [VSDX → PDF]  (Visio diagram)          │
│    └─ Componentes, capas, integraciones                                        │
│                                                                                 │
│ 🎨 DATA_FLOW_DIAGRAM                    [PNG / VSDX]  (Data movements)         │
│    └─ Cómo fluyen datos entre sistemas                                        │
│                                                                                 │
│ 🎨 DEPLOYMENT_ARCHITECTURE               [VSDX → PDF]  (Infraestructura)        │
│    └─ Servidores, networks, firewalls                                         │
│                                                                                 │
│ 🎨 INTEGRATION_DIAGRAM                   [VSDX → PDF]  (API connections)       │
│    └─ Cómo se integran con otros sistemas                                     │
│                                                                                 │
│ 🎨 C4_CONTEXT_DIAGRAM                   [MD / ASCII]  (En markdown)            │
│    └─ C4 context, containers, components                                      │
└────────────────────────────────────────────────────────────────────────────────┘

┌─ KNOWLEDGE_BASE ────────────────────────────────────────────────────────────────┐
│ Información reutilizable, referencias                                         │
├────────────────────────────────────────────────────────────────────────────────┤
│ 📝 TECHNICAL_GLOSSARY                   [MARKDOWN]   (Vivo, agregar términos)  │
│    └─ Términos técnicos definidos, acrónimos                                  │
│                                                                                 │
│ 📊 VENDOR_CONTACTS                      [XLSX]       (Contactos externos)      │
│    ├─ Vendor | Contact | Email | Phone | Escalation                         │
│    └─ Aspen, Cloud provider, etc.                                             │
│                                                                                 │
│ 📝 ASSUMPTIONS_LOG                      [MARKDOWN]   (Supuestos documentados)  │
│    └─ Qué asumimos que era verdadero al empezar                              │
│                                                                                 │
│ 📝 DEPENDENCIES                         [MARKDOWN]   (Dependencias externas)   │
│    └─ Proyectos, sistemas, equipos que dependemos                             │
│                                                                                 │
│ 📄 EXTERNAL_REFERENCES                  [DOCX]       (Links & docs)            │
│    └─ Documentación externa, links, referencias                               │
└────────────────────────────────────────────────────────────────────────────────┘

┌─ ARCHIVE ──────────────────────────────────────────────────────────────────────┐
│ Histórico: Documentos finalizados, freezed, archivados                        │
├────────────────────────────────────────────────────────────────────────────────┤
│ 📄 PHASE_1_CLOSURE_REPORT.pdf                                                  │
│ 📄 PHASE_2_LESSONS_LEARNED.pdf                                                │
│ 📄 [Todos los documentos finalizados como PDF]                                │
│                                                                                 │
│ Propósito: Guardar versión final de todo para auditoría & referencia futura   │
└────────────────────────────────────────────────────────────────────────────────┘
```

---

## RESUMEN: QOSEA RÁPIDO

### 📋 Documentos por Formato

| Formato | # Docs | Propósito | Guardar en |
|---------|--------|-----------|-----------|
| **MARKDOWN (.md)** | 8-10 | Técnico, vivo, control versión | Git (`/docs/`) |
| **DOCX (.docx)** | 15-20 | Formal, editable, colaboración | Shared (Teams) |
| **XLSX (.xlsx)** | 10-12 | Tracking, matrices, datos | Shared (live) |
| **PDF (.pdf)** | 15-20 | Formal final, archivos, distribución | Archive |
| **PPTX (.pptx)** | 2-3 | Presentaciones ejecutivas | Shared |
| **VSDX (.vsdx)** | 2-3 | Diagramas arquitectura | Shared |

### 🔄 Documentos VIVOS (Actualizar Regularmente)

```
SEMANAL (Viernes):
  - RISK_REGISTER.xlsx
  - CHANGE_LOG.xlsx
  - WEEKLY_STATUS.docx
  - BUDGET_TRACKING.xlsx

DIARIA/SEMANAL (Durante testing):
  - TEST_RESULTS.xlsx

BI-WEEKLY (Post-retro):
  - LESSONS_LEARNED.md

MENSUAL:
  - MONTHLY_REPORT.pdf
  - Budget review

AD-HOC:
  - RUNBOOK.md (cuando procedures cambien)
  - PLAYBOOK.md (cuando scenarios nuevos)
  - ADR_NNNN.md (cuando decisiones nuevas)
```

### 📍 Dónde Guardar

```
Git (versionado):           Shared (colaborativo):      Archive (histórico):
- *.md (todos)              - *.docx                    - *.pdf (finales)
- ADR_*.md                  - *.xlsx                    - Phase_closure_*.pdf
- RUNBOOK.md                - *.pptx                    - Yearly backups
- PLAYBOOK.md               - *.vsdx
- LESSONS_LEARNED.md
```

---

**Documento Control**: MATRIZ_VISUAL_DOCUMENTACION-v2.0  
**Referencia Rápida**: Imprime esto o ten a mano para planning
