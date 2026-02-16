# GUÍA: ESTRUCTURA DE DOCUMENTACIÓN DEL PROYECTO - MEJORES PRÁCTICAS PMI & SOFTWARE

**Versión**: 1.0  
**Propósito**: Definir dónde va cada documento de proyecto (no código, sino documentación escrita de PMI, procesos, decisiones)

---

## 1. ESTRUCTURA RECOMENDADA DE CARPETAS DE DOCUMENTACIÓN

```
/[Proyecto]/
│
├── /docs/                              # RAÍZ DE DOCUMENTACIÓN
│   │
│   ├── /project_management/            # Documentos de Gobernanza & PMI
│   │   ├── PROJECT_CHARTER.md          # Inicio formal del proyecto
│   │   ├── SCOPE_STATEMENT.md          # Declaración de alcance detallada
│   │   ├── STAKEHOLDER_MANAGEMENT.md   # Análisis y estrategia stakeholders
│   │   ├── PROJECT_SCHEDULE.md         # Schedule baseline (actualizable)
│   │   ├── BUDGET_PLAN.md              # Presupuesto y burn-down
│   │   ├── RISK_REGISTER.md            # Registro de riesgos (vivo)
│   │   ├── COMMUNICATIONS_PLAN.md      # Plan de comunicación formal
│   │   ├── QUALITY_PLAN.md             # Plan de QA/Testing
│   │   ├── PROCUREMENT_PLAN.md         # Contrataciones y vendors
│   │   ├── CHANGE_LOG.md               # Registro de cambios aprobados
│   │   └── LESSONS_LEARNED.md          # Lecciones (se actualiza monthly)
│   │
│   ├── /architecture_decisions/        # Decisiones Técnicas (ADR)
│   │   ├── ADR_0001_TECH_STACK.md      # Por qué se eligió esta tecnología
│   │   ├── ADR_0002_DATABASE_STRATEGY.md
│   │   ├── ADR_0003_DEPLOYMENT_APPROACH.md
│   │   ├── ADR_0004_SECURITY_APPROACH.md
│   │   └── ADR_TEMPLATE.md             # Template para nuevas ADRs
│   │
│   ├── /processes/                     # Procedimientos Operacionales
│   │   ├── DEVELOPMENT_PROCESS.md      # Cómo desarrollamos
│   │   ├── TESTING_PROCESS.md          # Cómo testeamos
│   │   ├── CODE_REVIEW_PROCESS.md      # Code review estándares
│   │   ├── DEPLOYMENT_PROCESS.md       # Cómo deployamos
│   │   ├── INCIDENT_RESPONSE.md        # Qué hacer si hay crisis
│   │   ├── ESCALATION_PROCEDURES.md    # Cuándo y cómo escalar
│   │   └── ONBOARDING_CHECKLIST.md     # Para nuevos miembros del team
│   │
│   ├── /requirements/                  # Especificaciones Funcionales
│   │   ├── FUNCTIONAL_REQUIREMENTS.md  # Qué hace el sistema
│   │   ├── NON_FUNCTIONAL_REQUIREMENTS.md # Performance, seguridad, etc
│   │   ├── USER_STORIES.md             # Historias de usuario
│   │   ├── ACCEPTANCE_CRITERIA.md      # Criterios de aceptación
│   │   └── USE_CASES.md                # Casos de uso detallados
│   │
│   ├── /testing/                       # Planes y Resultados de Testing
│   │   ├── TEST_PLAN.md                # Estrategia de testing
│   │   ├── TEST_CASES.md               # Casos de prueba documentados
│   │   ├── UAT_PLAN.md                 # Plan de User Acceptance Testing
│   │   ├── TEST_RESULTS.md             # Resultados ejecutados (actualizado)
│   │   ├── BUG_REPORT_TEMPLATE.md      # Template para reportar bugs
│   │   └── DEFECT_TRACKING_LOG.md      # Log de defectos encontrados
│   │
│   ├── /training/                      # Materiales de Capacitación
│   │   ├── TRAINING_PLAN.md            # Plan de capacitación
│   │   ├── TRAINING_MATERIALS.md       # Links a manuales, videos
│   │   ├── USER_MANUAL.md              # Manual del usuario final
│   │   ├── ADMINISTRATOR_GUIDE.md      # Guía para admin/IT
│   │   ├── TROUBLESHOOTING_GUIDE.md    # Solución de problemas comunes
│   │   └── FAQ.md                      # Preguntas frecuentes
│   │
│   ├── /operations/                    # Documentación Operacional
│   │   ├── RUNBOOK.md                  # Instrucciones paso a paso para Ops
│   │   ├── PLAYBOOK.md                 # Respuestas a escenarios de crisis
│   │   ├── SLA_DEFINITION.md           # Service Level Agreements
│   │   ├── MONITORING_PLAN.md          # Qué monitorear y cómo
│   │   ├── BACKUP_RECOVERY_PLAN.md     # Disaster recovery procedures
│   │   └── MAINTENANCE_SCHEDULE.md     # Ventanas de mantenimiento
│   │
│   ├── /compliance/                    # Cumplimiento & Seguridad
│   │   ├── SECURITY_POLICY.md          # Políticas de seguridad
│   │   ├── DATA_PRIVACY_PLAN.md        # GDPR, local compliance
│   │   ├── AUDIT_TRAIL_REQUIREMENTS.md # Auditoría y trazabilidad
│   │   ├── COMPLIANCE_CHECKLIST.md     # Checklist regulatorio
│   │   └── SECURITY_TESTING_RESULTS.md # Resultados de penetration tests
│   │
│   ├── /stakeholder_communication/     # Reportes y Comunicaciones
│   │   ├── WEEKLY_STATUS_TEMPLATE.md   # Template para reportes
│   │   ├── EXECUTIVE_SUMMARY_TEMPLATE.md
│   │   ├── MONTHLY_REPORTS/            # Carpeta con reportes históricos
│   │   │   ├── REPORT_JAN_2026.md
│   │   │   └── REPORT_FEB_2026.md
│   │   ├── STEERING_MEETING_NOTES/     # Actas de reuniones
│   │   └── RISK_ESCALATIONS.md         # Escalaciones documentadas
│   │
│   ├── /deliverables/                  # Entregables Formales
│   │   ├── ACCEPTANCE_SIGN_OFF.md      # Firmas de aceptación
│   │   ├── DEPLOYMENT_CHECKLIST.md     # Validaciones pre-go-live
│   │   ├── GO_LIVE_REPORT.md           # Reporte de go-live
│   │   └── HANDOVER_PACKAGE.md         # Documentos para Operations
│   │
│   ├── /financial/                     # Aspectos Financieros
│   │   ├── BUDGET_BASELINE.md          # Presupuesto inicial
│   │   ├── BURN_DOWN_CHART.md          # Gasto vs plan (actualizado)
│   │   ├── CHANGE_REQUEST_LOG.md       # Cambios + costo impacto
│   │   └── FINAL_ACCOUNTING.md         # Cierre financiero
│   │
│   └── /knowledge_base/                # Información Reutilizable
│       ├── TECHNICAL_GLOSSARY.md       # Glosario de términos
│       ├── VENDOR_CONTACTS.md          # Proveedores y contactos
│       ├── ASSUMPTIONS_LOG.md          # Supuestos documentados
│       ├── DEPENDENCIES.md             # Dependencias externas
│       └── REFERENCES.md               # Links a documentación externa
│
├── /src/                               # CÓDIGO (separate de docs)
├── /tests/                             # PRUEBAS
└── /config/                            # CONFIGURACIÓN
```

---

## 2. CARPETA: PROJECT_MANAGEMENT (Documentos Clave)

### 2.1 PROJECT_CHARTER.md
**Qué es**: Documento formal que autoriza el proyecto  
**Quién lo crea**: Project Manager con Sponsor  
**Cuándo**: Inicio del proyecto (Week 1)  
**Contenido**:
- Autorización y firma del Sponsor
- Objetivos SMART
- Justificación del negocio
- Alto nivel de riesgos iniciales
- Recursos asignados
- Contacto del PM

**Ejemplo de encabezado**:
```markdown
# PROJECT CHARTER - [PROYECTO]

Autorizado por: [Sponsor Name]
Firma: _________________ Fecha: _________

PM: [Tu Nombre]
Equipo Principal: [Names]
```

### 2.2 SCOPE_STATEMENT.md
**Qué es**: Descripción detallada de qué está IN y OUT  
**Actualización**: Si hay cambios aprobados  
**Contenido**:
- Descripción detallada de funcionalidad incluida
- Explícitamente qué NO está incluido
- Restricciones
- Supuestos
- Criterios de aceptación

### 2.3 STAKEHOLDER_MANAGEMENT.md
**Contenido**:
- Matriz de stakeholders (nombre, rol, interés, poder)
- Estrategia de engagement por grupo
- Frecuencia de comunicación
- Canales de escalación

### 2.4 RISK_REGISTER.md (VIVO - actualizar semanalmente)
**Estructura**:
```
| # | Riesgo | Prob | Impact | Status | Mitigation | Owner | Update Date |
|----|--------|------|--------|--------|-----------|-------|-------------|
| R1 | Calidad de datos | Media | Alto | Open | Auditoría | Tech Lead | 2026-02-10 |
```

**Actualizar cada Friday** con nuevos riesgos, status de mitigaciones

### 2.5 CHANGE_LOG.md (VIVO)
**Qué es**: Cada cambio aprobado al scope/plan se registra aquí

```
## Change Requests Approved

### CR-001: Ampliar scope a 5 BPCs (instead of 3)
- Fecha: 2026-02-15
- Impacto: +2 meses, +$50K
- Aprobado por: [Sponsor]
- Razón: Business need
```

---

## 3. CARPETA: ARCHITECTURE_DECISIONS (ADR - Architecture Decision Records)

**Qué es**: Documento que registra PORQUÉ se tomó una decisión técnica  
**Formato**: ADR-NNNN (0001, 0002, etc.)  
**Cuándo**: Para CADA decisión arquitectónica importante

### Template ADR:
```markdown
# ADR-0001: Usar PostgreSQL en lugar de MongoDB

## Contexto
Necesitamos base de datos para almacenar X millones de registros...

## Decisión
Se eligió PostgreSQL porque:
- ACID compliance garantizado
- SQL queries complejas necesarias
- Costos licensing menores

## Alternativas Consideradas
- MongoDB: Más flexible pero overkill
- Oracle: Muy caro
- SQLite: Demasiado ligero

## Consecuencias
✓ Mejor for complex queries
✗ Menos flexible para schema changes
```

---

## 4. CARPETA: PROCESSES (Cómo Trabajamos)

### 4.1 DEVELOPMENT_PROCESS.md
**Contenido**:
- Cómo un desarrollador trabaja en este proyecto
- Convenciones de código
- Branching strategy (git flow, trunk-based, etc.)
- Code review requirements
- Definition of Done

### 4.2 TESTING_PROCESS.md
**Contenido**:
- Niveles de testing (unit, integration, e2e)
- Herramientas usadas
- Métricas de éxito (coverage%, pass rate)
- Quién testa qué
- Defect severity levels

### 4.3 DEPLOYMENT_PROCESS.md
**Contenido**:
- Pre-deployment checklist
- Pasos de deployment
- Validación post-deployment
- Rollback procedures
- Who approves production changes

---

## 5. CARPETA: REQUIREMENTS (Especificaciones)

**Qué es**: Documentos que definen EXACTAMENTE qué construir

### 5.1 FUNCTIONAL_REQUIREMENTS.md
Descripción de cada feature/módulo:
```
## FR-001: User Authentication
- Usuarios pueden login con email/password
- Soporte para LDAP/AD integration
- Sesiones expiran después de 8h
```

### 5.2 NON_FUNCTIONAL_REQUIREMENTS.md
```
## NFR-001: Performance
- Response time < 2 segundos para 95% de requests
- System debe soportar 10,000 concurrent users

## NFR-002: Security
- Todas las passwords encrypted con bcrypt
- SQL injection prevention via prepared statements
```

---

## 6. CARPETA: TESTING (Planes & Resultados)

### 6.1 TEST_PLAN.md
**Contenido**:
- Estrategia de testing (niveles, tipos)
- Responsabilidades de QA
- Timeline de testing
- Criterios de exit (cuándo consideramos testing done)
- Risk-based testing approach

### 6.2 TEST_RESULTS.md (VIVO - actualizar)
```
# Testing Results - Week of [Date]

## Unit Testing
- Total tests: 1,250
- Passed: 1,248 (99.8%)
- Failed: 2
- Coverage: 82%

## Integration Testing
- Total tests: 150
- Passed: 148 (98.6%)
- Failed: 2 (both high priority, to be fixed this week)

## Issues Found
- [BUG-001] Payment processing fails for amounts > $99,999
- [ENHANCEMENT-001] Add export to PDF functionality
```

---

## 7. CARPETA: OPERATIONS (Para IT/Operations después de go-live)

### 7.1 RUNBOOK.md
**Qué es**: Manual "Copy-paste" para operaciones diarias

```
# Runbook - [Sistema]

## Daily Tasks
### 1. Check System Health (8:00 AM)
$ ssh prod-server
$ sudo systemctl status mtell_service
$ check disk space: df -h
$ check memory: free -m
[Más pasos...]

## Backup Verification (4:00 PM)
$ check backup log: tail -n 100 /var/log/backup.log
[...]
```

### 7.2 PLAYBOOK.md
**Qué es**: Qué hacer si hay una crisis

```
# Playbook - High CPU Usage

## Alert Received: CPU > 80%

### Step 1: Investigate
- Check top processes: top -b -n 1
- Check running queries if DB: SELECT * FROM ...

### Step 2: Immediate Actions
- Restart service? YES/NO
- Scale up? YES/NO

### Step 3: Escalate if needed
- Contact: [Tech Lead Phone]
```

---

## 8. CARPETA: STAKEHOLDER_COMMUNICATION (Reportes)

### 8.1 WEEKLY_STATUS_TEMPLATE.md
```markdown
# Weekly Status Report - Week of [Date]

## Overall Status: 🟢 GREEN / 🟡 YELLOW / 🔴 RED

## Highlights
- ✅ Completed: [Milestone]
- ✅ Completed: [Deliverable]

## In Progress
- [Task] - 75% complete
- [Task] - 50% complete

## Blockers
- [ISSUE] - Impact: [HIGH/MEDIUM] - Owner: [Person]

## Metrics
| Metric | Baseline | Actual | Target |
|--------|----------|--------|--------|
| Schedule | 50% | 52% | On track |
| Quality | 95% pass | 94% pass | 95%+ |
| Budget | $500K | $475K | Within 10% |
```

### 8.2 MONTHLY_REPORTS/ (carpeta histórica)
Guardar cada reporte mensual para referencia histórica

---

## 9. CARPETA: COMPLIANCE (Auditoría & Seguridad)

### 9.1 SECURITY_POLICY.md
**Contenido**:
- Política de contraseñas
- Acceso a datos sensibles
- Política de VPN/remote work
- Incidentes de seguridad (cómo reportar)

### 9.2 COMPLIANCE_CHECKLIST.md
```markdown
# Compliance Checklist

## GDPR
- [ ] Data collection consent documented
- [ ] Right to be forgotten implemented
- [ ] Data breach notification procedure

## SOC 2
- [ ] Access controls in place
- [ ] Change management documented
- [ ] Incident response plan written
```

---

## 10. CARPETA: DELIVERABLES (Aceptación Formal)

### 10.1 ACCEPTANCE_SIGN_OFF.md
**Qué es**: Documento formal de aprobación

```markdown
# Acceptance Sign-Off

## Phase 1: Infrastructure Ready
- [ ] All tests passed
- [ ] Performance validated
- [ ] Security review completed

Signed by:
- Technical Lead: _________________ Date: _____
- Project Manager: _________________ Date: _____
- Sponsor: _________________ Date: _____
```

---

## 11. ACTUALIZACIÓN DE DOCUMENTOS (CADENCIA)

| Documento | Frecuencia | Owner |
|-----------|-----------|-------|
| RISK_REGISTER | Semanal (viernes) | PM |
| CHANGE_LOG | Ad-hoc (cuando se aprueba cambio) | PM |
| TEST_RESULTS | Diaria/Weekly | QA Lead |
| WEEKLY_STATUS | Semanal (viernes) | PM |
| MONTHLY_REPORTS | Mensual (último viernes mes) | PM |
| LESSONS_LEARNED | Bi-weekly + final | PM |
| Otros archivos | As-needed | Responsible owner |

---

## 12. EJEMPLO: Cómo Documentar un Proyecto Real

### Semana 1
- ✅ Crear PROJECT_CHARTER.md (firmado)
- ✅ Crear SCOPE_STATEMENT.md
- ✅ Crear STAKEHOLDER_MANAGEMENT.md
- ✅ Crear RISK_REGISTER.md (inicial)

### Semana 2-4 (Planning)
- ✅ Crear ADR-0001, ADR-0002, etc. (decisiones)
- ✅ Crear DEVELOPMENT_PROCESS.md
- ✅ Crear REQUIREMENTS documentos

### Semana 5+ (Ejecución)
- ✅ Actualizar RISK_REGISTER (viernes)
- ✅ Actualizar CHANGE_LOG (si cambios)
- ✅ Crear WEEKLY_STATUS (viernes)
- ✅ Actualizar TEST_RESULTS (daily/weekly)

### Final del Proyecto
- ✅ Crear LESSONS_LEARNED.md
- ✅ Crear ACCEPTANCE_SIGN_OFF.md
- ✅ Guardar todos reportes en MONTHLY_REPORTS/

---

## 13. BENEFICIOS DE ESTA ESTRUCTURA

✅ **Auditable**: Todo documentado en lugar específico  
✅ **Escalable**: Fácil agregar nuevos documentos  
✅ **PMI-compliant**: Sigue estándares de gestión  
✅ **Operacional**: Runbooks y playbooks listos para Ops  
✅ **Histórico**: Decisiones y cambios trackeados  
✅ **Profesional**: Refleja madurez de proyecto  

---

**Documento Control**: DOCUMENTATION_STRUCTURE_GUIDE-v1.0  
**Owner**: PMO / Documentation Lead  
**Próxima Revisión**: As-needed
