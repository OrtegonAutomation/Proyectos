# Plan de Pruebas (Test Plan) — FIFO

**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Cliente:** ODL Instrumentación y Control  
**Empresa:** IDC Ingeniería  
**Versión:** 1.0  
**Estado:** Borrador  
**Fecha:** 2026-02-16

---

## 1. Introducción

### 1.1 Propósito

Este documento define la estrategia, alcance, enfoque y recursos necesarios para validar que el sistema FIFO de gestión de almacenamiento cumple con todos los requerimientos funcionales (RF-01 a RF-08), no funcionales (RNF-01 a RNF-10) y criterios de aceptación (CA-01 a CA-12) definidos en la documentación del proyecto.

### 1.2 Alcance

El plan cubre:
- Pruebas unitarias de componentes C++ (motor FIFO)
- Pruebas de integración WPF ↔ C++ (comunicación JSON)
- Pruebas funcionales de cada caso de uso (CU-01 a CU-10)
- Pruebas de rendimiento y estrés
- Pruebas de seguridad y control de acceso
- Pruebas de aceptación de usuario (UAT)
- Pruebas de regresión post-corrección

### 1.3 Fuera de Alcance

- Pruebas de penetración de red (no es aplicación web)
- Pruebas de compatibilidad cross-platform (solo Windows Server)
- Pruebas de carga concurrente multi-usuario (operación single-user)

### 1.4 Documentos de Referencia

| Documento | Versión | Ubicación |
|-----------|---------|-----------|
| Requerimientos Funcionales | 0.2 | `Requisitos/02_REQUERIMIENTOS_FUNCIONALES.docx` |
| Requerimientos No Funcionales | 0.2 | `Requisitos/03_REQUERIMIENTOS_NO_FUNCIONALES.docx` |
| Criterios de Aceptación | 0.2 | `Requisitos/CRITERIOS_DE_ACEPTACION.md` |
| Historias de Usuario | 0.2 | `Requisitos/HISTORIAS_DE_USUARIO.md` |
| Casos de Uso | 0.3 | `Requisitos/RESUMEN_ACTUALIZACION.md` |
| ADRs | 1.0 | `Decisiones_arquitectura/ADR_0001 a ADR_0007` |

---

## 2. Estrategia de Pruebas

### 2.1 Niveles de Prueba

```
┌─────────────────────────────────────────────────┐
│         NIVEL 4: PRUEBAS DE ACEPTACIÓN (UAT)    │ ← Cliente ODL ejecuta
│         Escenarios reales en servidor producción │
├─────────────────────────────────────────────────┤
│         NIVEL 3: PRUEBAS DE SISTEMA             │ ← IDC ejecuta
│         End-to-end, rendimiento, seguridad       │
├─────────────────────────────────────────────────┤
│         NIVEL 2: PRUEBAS DE INTEGRACIÓN         │ ← IDC ejecuta
│         WPF ↔ C++, componentes combinados        │
├─────────────────────────────────────────────────┤
│         NIVEL 1: PRUEBAS UNITARIAS              │ ← Desarrolladores
│         Funciones individuales C++ y C#           │
└─────────────────────────────────────────────────┘
```

### 2.2 Tipos de Prueba

| Tipo | Objetivo | Herramienta | Responsable |
|------|----------|-------------|-------------|
| **Unitarias** | Validar funciones individuales | Google Test (C++), NUnit (C#) | Desarrollador |
| **Integración** | Validar comunicación WPF ↔ C++ | Scripts automatizados | Desarrollador/QA |
| **Funcionales** | Validar casos de uso completos | Manual + scripts | QA / Ingeniero |
| **Rendimiento** | Validar tiempos de respuesta (CA-07) | Cronómetro + scripts | QA |
| **Estrés** | Validar comportamiento bajo carga extrema | Generador de archivos | QA |
| **Seguridad** | Validar control de acceso (CA-09) | Manual | QA / Ingeniero |
| **Regresión** | Validar que correcciones no rompen funcionalidad | Suite automatizada | Desarrollador |
| **UAT** | Validar aceptación del cliente | Manual, escenarios reales | Cliente ODL |

### 2.3 Enfoque de Pruebas por Componente

| Componente | Enfoque | Prioridad |
|------------|---------|-----------|
| Motor FIFO (C++) | Unitarias exhaustivas + integración | Crítica |
| Inventario de disco | Funcional + rendimiento | Alta |
| Política de retención | Funcional + edge cases | Alta |
| Simulación | Funcional + determinismo | Alta |
| Eliminación por lotes | Funcional + seguridad | Crítica |
| Bitácora | Funcional + integridad | Alta |
| Alarmas | Funcional + canales | Media |
| UI WPF | Funcional + usabilidad | Media |
| RF-07 (Programada) | Integración + temporización | Alta |
| RF-08 (Preventiva) | Integración + eventos | Alta |

---

## 3. Criterios de Entrada y Salida

### 3.1 Criterios de Entrada (para iniciar pruebas)

- [ ] Código fuente compilado sin errores
- [ ] Ambiente de pruebas configurado (servidor Windows con espacio controlado)
- [ ] Datos de prueba generados (estructura de archivos sintéticos)
- [ ] Documentación de requisitos aprobada (versión ≥ 0.2)
- [ ] Casos de prueba escritos y revisados
- [ ] Herramientas de testing instaladas y verificadas

### 3.2 Criterios de Salida (para finalizar pruebas)

- [ ] 100% de casos de prueba críticos ejecutados
- [ ] 0 defectos de severidad Crítica o Alta abiertos
- [ ] ≥ 95% de casos de prueba pasados
- [ ] Pruebas de rendimiento dentro de umbrales definidos (CA-07)
- [ ] Pruebas de seguridad sin vulnerabilidades críticas
- [ ] UAT aprobado por cliente ODL
- [ ] Reporte de pruebas firmado

### 3.3 Criterios de Suspensión

- Defecto bloqueante que impide ejecutar > 30% de los casos
- Ambiente de pruebas no disponible por > 4 horas
- Cambio de requisitos que invalida > 20% de los casos existentes

---

## 4. Ambiente de Pruebas

### 4.1 Ambiente de Desarrollo/QA

| Componente | Especificación |
|------------|---------------|
| Sistema Operativo | Windows 10/11 Pro o Windows Server 2019+ |
| CPU | Intel i5 o superior |
| RAM | 8 GB mínimo |
| Disco | 100 GB disponibles para datos de prueba |
| Software | Visual Studio 2022, .NET 6+, compilador C++17 |

### 4.2 Ambiente de Pre-Producción

| Componente | Especificación |
|------------|---------------|
| Sistema Operativo | Windows Server (igual a producción ODL) |
| Disco | 500 GB con estructura simulando Assets reales |
| Red | Conectividad para pruebas de email/syslog |
| Usuarios | Cuentas con roles: admin, operador, lectura |

### 4.3 Datos de Prueba

| Dataset | Descripción | Tamaño |
|---------|-------------|--------|
| DS-SMALL | 100 archivos, 1 GB total, 2 Assets | Pruebas unitarias |
| DS-MEDIUM | 10,000 archivos, 50 GB total, 5 Assets | Pruebas funcionales |
| DS-LARGE | 100,000 archivos, 500 GB total, 10 Assets | Pruebas rendimiento |
| DS-STRESS | 1,000,000 archivos, 2 TB total, 20 Assets | Pruebas estrés |
| DS-EDGE | Archivos con nombres especiales, rutas largas, permisos restringidos | Edge cases |

---

## 5. Planificación de Pruebas

### 5.1 Fases de Ejecución

| Fase | Descripción | Semana | Responsable |
|------|-------------|--------|-------------|
| **Fase 1** | Pruebas unitarias C++ (motor FIFO) | Semana 1-2 | Desarrollador |
| **Fase 2** | Pruebas unitarias C# (UI, parsers) | Semana 1-2 | Desarrollador |
| **Fase 3** | Pruebas de integración WPF ↔ C++ | Semana 2-3 | QA |
| **Fase 4** | Pruebas funcionales (CU-01 a CU-10) | Semana 2-3 | QA |
| **Fase 5** | Pruebas de rendimiento y estrés | Semana 3 | QA |
| **Fase 6** | Pruebas de seguridad | Semana 3 | QA / Ingeniero |
| **Fase 7** | Pruebas de regresión | Semana 3-4 | QA |
| **Fase 8** | UAT en servidor ODL | Semana 4 | Cliente ODL |

### 5.2 Dependencias

```
Fase 1 (Unit C++) ──┐
                     ├──→ Fase 3 (Integración) ──→ Fase 4 (Funcional) ──┐
Fase 2 (Unit C#) ───┘                                                    │
                                                         Fase 5 (Rendimiento) ──┐
                                                         Fase 6 (Seguridad) ────┤
                                                                                 ├──→ Fase 7 (Regresión) ──→ Fase 8 (UAT)
```

---

## 6. Resumen de Casos de Prueba por Área

### 6.1 Distribución

| Área | # Casos | Prioridad | Referencia CA |
|------|---------|-----------|---------------|
| Inventario de almacenamiento | 12 | Alta | CA-01 |
| Política de retención | 14 | Alta | CA-02 |
| Simulación FIFO | 14 | Alta | CA-03 |
| Eliminación producción | 16 | Crítica | CA-04 |
| Bitácora y auditoría | 14 | Alta | CA-05 |
| Alarmas y notificaciones | 12 | Alta | CA-06 |
| Rendimiento | 12 | Media | CA-07 |
| Confiabilidad | 12 | Alta | CA-08 |
| Seguridad | 12 | Alta | CA-09 |
| Usabilidad | 12 | Media | CA-10 |
| RF-07 Programada | 8 | Alta | HU-07 |
| RF-08 Preventiva | 10 | Alta | HU-05 |
| Integración WPF ↔ C++ | 8 | Alta | ADR-0001 |
| Edge Cases / Negativos | 10 | Media | — |
| **TOTAL** | **166** | | |

### 6.2 Detalles en TEST_CASES.xlsx

Todos los casos de prueba están detallados en el archivo `TEST_CASES.xlsx` con:
- ID único (TC-XXXX)
- Descripción
- Precondiciones
- Pasos detallados
- Resultado esperado
- Prioridad (Crítica/Alta/Media/Baja)
- Criterio de aceptación asociado
- Estado de ejecución
- Resultado real (al ejecutar)

---

## 7. Gestión de Defectos

### 7.1 Clasificación de Severidad

| Severidad | Descripción | SLA Resolución | Ejemplo |
|-----------|-------------|----------------|---------|
| **S1 - Crítica** | Sistema no funciona o elimina datos incorrectamente | Inmediato | Elimina archivos en lista blanca |
| **S2 - Alta** | Funcionalidad principal afectada pero con workaround | 24 horas | Bitácora no registra operación |
| **S3 - Media** | Funcionalidad menor afectada | 48 horas | Export CSV con formato incorrecto |
| **S4 - Baja** | Cosmético o mejora | Próxima versión | Tooltip con texto cortado |

### 7.2 Ciclo de Vida del Defecto

```
NUEVO → ASIGNADO → EN PROGRESO → RESUELTO → VERIFICADO → CERRADO
                                      ↓
                                  REABIERTO ← (si verificación falla)
```

### 7.3 Métricas de Defectos

- Densidad: defectos / KLOC
- Tasa de reapertura: defectos reabiertos / defectos cerrados
- Tiempo medio de resolución por severidad
- Distribución por componente

---

## 8. Riesgos de Testing

| Riesgo | Probabilidad | Impacto | Mitigación |
|--------|-------------|---------|------------|
| Servidor ODL no disponible para UAT | Media | Alto | Preparar ambiente espejo en IDC |
| Datos de prueba insuficientes (no simulan realidad) | Media | Alto | Solicitar muestra anónima de estructura real a ODL |
| Defectos de rendimiento en datasets grandes | Alta | Alto | Ejecutar pruebas de rendimiento temprano (Fase 3) |
| Cambios de requisitos durante testing | Baja | Medio | Proceso de control de cambios con impacto en test cases |
| Falta de disponibilidad del equipo de pruebas | Baja | Alto | Cross-training entre desarrolladores |

---

## 9. Entregables de Testing

| Entregable | Formato | Responsable | Cuándo |
|------------|---------|-------------|--------|
| Plan de Pruebas (este documento) | Markdown/PDF | QA Lead | Antes de Fase 1 |
| Casos de Prueba | Excel (TEST_CASES.xlsx) | QA | Antes de Fase 1 |
| Plan UAT | Markdown/PDF | QA + Cliente | Antes de Fase 8 |
| Resultados de ejecución | Excel (test_results_live.xlsx) | QA | Durante ejecución |
| Reporte de defectos | Excel/Tracker | QA | Continuo |
| Resumen final de pruebas | PDF (TEST_RESULTS_SUMMARY) | QA Lead | Post Fase 8 |
| Certificación de aceptación | Documento firmado | Cliente ODL | Post UAT |

---

## 10. Aprobaciones

| Rol | Nombre | Firma | Fecha |
|-----|--------|-------|-------|
| QA Lead | [Pendiente] | | |
| Líder Técnico | [Pendiente] | | |
| Gerente de Proyecto IDC | [Pendiente] | | |
| Representante ODL | [Pendiente] | | |

---

**Versión:** 1.0  
**Estado:** Borrador  
**Próxima revisión:** [Pendiente]
