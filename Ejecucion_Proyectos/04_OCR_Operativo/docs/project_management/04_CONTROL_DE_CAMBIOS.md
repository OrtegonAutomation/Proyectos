# CONTROL DE CAMBIOS DEL PROYECTO

| Campo | Detalle |
|---|---|
| **Proyecto** | OCR Operativo - Sistema de Digitalizacion de Registros Operacionales |
| **Codigo** | PRY-04-OCR |
| **Version** | 1.0 |
| **Fecha de Creacion** | 2026-02-24 |
| **Ultima Actualizacion** | 2026-02-24 |
| **Estado** | Activo |
| **Preparado por** | Project Manager - IDC Ingenieria |

---

## 1. Objetivo

Este documento establece el procedimiento formal para la gestion de cambios en el proyecto OCR Operativo. Su proposito es asegurar que cualquier modificacion al alcance, cronograma, presupuesto o entregables del proyecto sea evaluada, aprobada y documentada de manera controlada.

---

## 2. Procedimiento de Control de Cambios

### 2.1 Diagrama del Proceso

```
[1. Identificacion]     Cualquier miembro del equipo o stakeholder identifica
        |                la necesidad de un cambio.
        v
[2. Registro]           El solicitante completa el formulario de solicitud
        |                de cambio y lo entrega al PM.
        v
[3. Analisis]           El PM y los responsables tecnicos evaluan el impacto
        |                en alcance, cronograma, presupuesto y calidad.
        v
[4. Clasificacion]      Se clasifica el cambio segun su nivel de impacto
        |                (Critico, Mayor, Menor, Cosmético).
        v
[5. Evaluacion]         El Comite de Control de Cambios (CCC) revisa
        |                la solicitud con el analisis de impacto.
        v
[6. Decision]           El CCC aprueba, rechaza o difiere el cambio.
   /    |    \
  v     v     v
[Aprobado] [Rechazado] [Diferido]
  |           |              |
  v           v              v
[7. Impl.]  [Notif.]   [Backlog]
  |
  v
[8. Verificacion]       Se verifica que el cambio fue implementado
        |                correctamente y sin efectos colaterales.
        v
[9. Cierre]             Se actualiza el registro y se cierra la solicitud.
```

### 2.2 Pasos Detallados

#### Paso 1: Identificacion del Cambio
- Cualquier miembro del equipo, stakeholder o usuario puede identificar la necesidad de un cambio.
- El cambio puede originarse por: nuevos requerimientos, defectos, mejoras, cambios externos o riesgos materializados.

#### Paso 2: Registro de la Solicitud
- El solicitante completa la seccion de registro en la tabla de control de cambios.
- Se asigna un numero unico de solicitud (CC-XXX).
- Se describe el cambio solicitado con el mayor detalle posible.

#### Paso 3: Analisis de Impacto
El Project Manager coordina el analisis de impacto con los responsables tecnicos. Se evalua:

| Dimension | Preguntas Clave |
|---|---|
| **Alcance** | Modifica entregables? Agrega o elimina funcionalidad? |
| **Cronograma** | Cuantos dias adicionales requiere? Afecta la ruta critica? |
| **Presupuesto** | Cuanto costo adicional genera? Se requiere aprobacion de presupuesto? |
| **Calidad** | Afecta criterios de aceptacion? Requiere pruebas adicionales? |
| **Riesgos** | Introduce nuevos riesgos? Mitiga riesgos existentes? |
| **Recursos** | Requiere recursos adicionales? Reasignacion de equipo? |

#### Paso 4: Clasificacion del Cambio
Se clasifica segun la tabla de la seccion 3.

#### Paso 5: Evaluacion por el Comite
El Comite de Control de Cambios se reune (o comunica via asincrona para cambios menores) para evaluar la solicitud.

#### Paso 6: Decision
- **Aprobado**: Se procede con la implementacion segun el plan de accion definido.
- **Rechazado**: Se notifica al solicitante con la justificacion. Se archiva la solicitud.
- **Diferido**: Se registra en el backlog para evaluacion futura. Se define fecha de re-evaluacion.

#### Paso 7: Implementacion
- Se actualiza el plan del proyecto (cronograma, presupuesto, alcance) segun corresponda.
- Se asignan las tareas al equipo responsable.
- Se comunica al equipo completo.

#### Paso 8: Verificacion
- El QA verifica que el cambio fue implementado correctamente.
- Se ejecutan pruebas de regresion si aplica.

#### Paso 9: Cierre
- Se actualiza el estado en la tabla de registro.
- Se documentan las lecciones aprendidas si aplica.

---

## 3. Clasificacion de Cambios

| Nivel | Descripcion | Impacto en Cronograma | Impacto en Presupuesto | Aprobador |
|---|---|---|---|---|
| **Critico** | Cambia fundamentalmente el alcance, los objetivos o la viabilidad del proyecto | > 5 dias | > $30,000 USD | Sponsor + Gerencia |
| **Mayor** | Modifica entregables principales, afecta integracion SAP o precision OCR | 2-5 dias | $10,000 - $30,000 USD | Sponsor |
| **Menor** | Ajustes a funcionalidad existente, mejoras de UI, optimizaciones | 1-2 dias | < $10,000 USD | Project Manager |
| **Cosmetico** | Cambios visuales, textuales o de documentacion sin impacto funcional | < 1 dia | Sin costo adicional | Project Manager |

### Tiempos de Respuesta por Clasificacion

| Nivel | Tiempo Maximo de Evaluacion | Tiempo Maximo de Decision |
|---|---|---|
| Critico | 4 horas | 24 horas |
| Mayor | 8 horas | 48 horas |
| Menor | 24 horas | 48 horas |
| Cosmetico | 48 horas | Siguiente reunion de equipo |

---

## 4. Comite de Control de Cambios (CCC)

| Miembro | Rol | Voto |
|---|---|---|
| Sponsor del Proyecto | Aprobador final para cambios criticos y mayores | Decisivo |
| Project Manager | Coordinador del comite, evaluador de impacto | Consultivo |
| ML Engineer | Evaluador tecnico OCR/ML | Consultivo |
| Backend Engineer 1 | Evaluador tecnico API/BD | Consultivo |
| Lider de Operaciones | Representante del usuario final | Consultivo |

### Reglas de Operacion del CCC
- Para cambios **criticos**: Se requiere reunion sincrona del comite completo.
- Para cambios **mayores**: Se puede resolver via comunicacion asincrona (correo/chat) con plazo de 48h.
- Para cambios **menores/cosmeticos**: El PM tiene autoridad de decision autonoma.
- En caso de empate o desacuerdo, el Sponsor tiene voto de desempate.

---

## 5. Proceso de Aprobacion

### 5.1 Matriz de Autoridad

| Tipo de Cambio | PM | Sponsor | Gerencia |
|---|---|---|---|
| Cosmetico | Aprueba | Informa | - |
| Menor | Aprueba | Informa | - |
| Mayor | Recomienda | Aprueba | Informa |
| Critico | Recomienda | Recomienda | Aprueba |

### 5.2 Criterios de Aprobacion

Un cambio sera aprobado si cumple **al menos 3** de los siguientes criterios:

- [ ] Es necesario para cumplir los criterios de aceptacion del proyecto.
- [ ] Su beneficio supera claramente su costo e impacto.
- [ ] No compromete la fecha de entrega del proyecto (o el retraso es aceptable).
- [ ] No excede el presupuesto de contingencia disponible.
- [ ] Esta alineado con los objetivos estrategicos del proyecto.
- [ ] Los riesgos introducidos son manejables con el plan de mitigacion propuesto.

### 5.3 Criterios de Rechazo

Un cambio sera rechazado si cumple **cualquiera** de los siguientes:

- Compromete la viabilidad del proyecto.
- El costo excede el presupuesto total (incluyendo contingencia) sin aprobacion de presupuesto adicional.
- Introduce riesgos criticos no mitigables.
- Esta fuera del alcance definido y no hay justificacion de negocio.

---

## 6. Impacto en Cronograma y Presupuesto

### 6.1 Presupuesto de Contingencia

| Concepto | Monto |
|---|---|
| Presupuesto base | $200,000 - $280,000 USD |
| Contingencia incluida (15%) | $25,500 - $36,000 USD |
| Contingencia utilizada | $0 USD |
| Contingencia disponible | $25,500 - $36,000 USD |

### 6.2 Registro de Impacto Acumulado

| Metrica | Linea Base | Acumulado Cambios | Actual |
|---|---|---|---|
| Duracion del proyecto | 28 dias | 0 dias | 28 dias |
| Presupuesto total | $200,000 - $280,000 | $0 | $200,000 - $280,000 |
| Numero de entregables | 9 principales | 0 | 9 principales |
| Precision OCR objetivo | >= 95% | Sin cambio | >= 95% |

---

## 7. Registro de Solicitudes de Cambio

### Tabla de Registro

| # Solicitud | Fecha Solicitud | Solicitante | Descripcion del Cambio | Clasificacion | Justificacion | Impacto Cronograma | Impacto Presupuesto | Estado | Aprobador | Fecha Decision | Observaciones |
|---|---|---|---|---|---|---|---|---|---|---|---|
| CC-001 | ____/____/____ | _____________ | _________________________________ | Critico / Mayor / Menor / Cosmetico | _________________________________ | ___ dias | $___________ | Pendiente / Aprobado / Rechazado / Diferido | _____________ | ____/____/____ | _________________________________ |
| CC-002 | ____/____/____ | _____________ | _________________________________ | Critico / Mayor / Menor / Cosmetico | _________________________________ | ___ dias | $___________ | Pendiente / Aprobado / Rechazado / Diferido | _____________ | ____/____/____ | _________________________________ |
| CC-003 | ____/____/____ | _____________ | _________________________________ | Critico / Mayor / Menor / Cosmetico | _________________________________ | ___ dias | $___________ | Pendiente / Aprobado / Rechazado / Diferido | _____________ | ____/____/____ | _________________________________ |
| CC-004 | ____/____/____ | _____________ | _________________________________ | Critico / Mayor / Menor / Cosmetico | _________________________________ | ___ dias | $___________ | Pendiente / Aprobado / Rechazado / Diferido | _____________ | ____/____/____ | _________________________________ |
| CC-005 | ____/____/____ | _____________ | _________________________________ | Critico / Mayor / Menor / Cosmetico | _________________________________ | ___ dias | $___________ | Pendiente / Aprobado / Rechazado / Diferido | _____________ | ____/____/____ | _________________________________ |

### Formulario Detallado de Solicitud de Cambio

Para cada solicitud registrada, completar el siguiente formulario:

```
=============================================================
FORMULARIO DE SOLICITUD DE CAMBIO
=============================================================

Numero de Solicitud: CC-___
Fecha: ____/____/________
Solicitante: _________________________________
Rol: _________________________________

--- DESCRIPCION DEL CAMBIO ---
Descripcion detallada:
_________________________________________________________________
_________________________________________________________________
_________________________________________________________________

Razon/Justificacion:
_________________________________________________________________
_________________________________________________________________

--- ANALISIS DE IMPACTO ---
Impacto en alcance:      [ ] Sin impacto  [ ] Menor  [ ] Mayor  [ ] Critico
Impacto en cronograma:   [ ] Sin impacto  [ ] Menor  [ ] Mayor  [ ] Critico
Impacto en presupuesto:  [ ] Sin impacto  [ ] Menor  [ ] Mayor  [ ] Critico
Impacto en calidad:      [ ] Sin impacto  [ ] Menor  [ ] Mayor  [ ] Critico

Dias adicionales estimados: ___
Costo adicional estimado: $___________
Recursos adicionales requeridos: _________________________________

Riesgos introducidos:
_________________________________________________________________

Alternativas consideradas:
_________________________________________________________________

--- DECISION ---
Clasificacion final: [ ] Critico  [ ] Mayor  [ ] Menor  [ ] Cosmetico
Decision: [ ] Aprobado  [ ] Rechazado  [ ] Diferido
Justificacion de la decision:
_________________________________________________________________

Aprobado por: _________________________________
Fecha de decision: ____/____/________

--- IMPLEMENTACION ---
Responsable de implementacion: _________________________________
Fecha de inicio: ____/____/________
Fecha de fin: ____/____/________
Verificado por: _________________________________
Fecha de verificacion: ____/____/________

=============================================================
```

---

## 8. Indicadores de Control de Cambios

### Metricas a Monitorear

| Indicador | Formula | Meta | Frecuencia |
|---|---|---|---|
| Total de solicitudes | Conteo acumulado | Registro | Semanal |
| % Aprobados | Aprobados / Total * 100 | < 30% | Semanal |
| % Rechazados | Rechazados / Total * 100 | Registro | Semanal |
| Impacto acumulado en cronograma | Suma dias adicionales aprobados | < 3 dias | Semanal |
| Impacto acumulado en presupuesto | Suma costos adicionales aprobados | < $36,000 | Semanal |
| Tiempo promedio de resolucion | Promedio dias (solicitud a decision) | < 2 dias | Semanal |

---

## 9. Comunicacion de Cambios

| Evento | Canal | Audiencia | Responsable |
|---|---|---|---|
| Nueva solicitud registrada | Correo + herramienta de gestion | PM + Evaluadores tecnicos | Solicitante |
| Resultado de evaluacion | Correo | Solicitante + Equipo afectado | PM |
| Cambio aprobado e implementado | Reunion standup + Correo | Todo el equipo | PM |
| Actualizacion de plan del proyecto | Documentos actualizados | Todo el equipo + Sponsor | PM |
| Resumen semanal de cambios | Reporte semanal | Sponsor + Equipo | PM |

---

## Historial de Versiones

| Version | Fecha | Autor | Descripcion del Cambio |
|---|---|---|---|
| 1.0 | 2026-02-24 | Project Manager | Creacion inicial del documento |

---

*Documento generado como parte de la gestion del proyecto PRY-04-OCR - OCR Operativo.*
