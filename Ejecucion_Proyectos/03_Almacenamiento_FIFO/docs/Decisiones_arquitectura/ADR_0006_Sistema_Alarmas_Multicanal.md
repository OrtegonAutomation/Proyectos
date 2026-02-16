# ADR-0006: Sistema de Alarmas Multicanal con Coalescing y Escalación

**Estado:** Aceptada  
**Fecha:** 2026-02-16  
**Autor:** IDC Ingeniería  
**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Cliente:** ODL Instrumentación y Control

---

## Contexto

El sistema FIFO opera sobre un servidor de monitoreo industrial crítico. Cuando ocurren eventos importantes (disco lleno, fallo de ejecución, crecimiento anormal), los operadores deben ser notificados de inmediato para actuar antes de que el servidor de monitoreo deje de funcionar.

Requisitos clave (CA-06):
- Alarma dentro de 30 segundos del evento (CA-06-04)
- Múltiples canales: syslog, email, evento Windows (CA-06-06)
- Coalescing: no repetir alarma si condición persiste (CA-06-09)
- Silenciar por mantenimiento (CA-06-10)
- Historial auditable (CA-06-08)
- Mensajes en español profesional (CA-06-07)

## Decisión

**Se implementa sistema de alarmas con tres canales simultáneos (email, syslog, evento Windows), coalescing por tipo de evento, escalación automática por tiempo, y capacidad de silencio temporal.**

### Canales de Notificación

| Canal | Uso Principal | Configuración |
|-------|--------------|---------------|
| **Evento Windows** | Registro local inmediato | Event Log: Application, Source: FIFO-Manager |
| **Email (SMTP)** | Notificación a operadores y gerentes | Servidor SMTP, destinatarios configurables |
| **Syslog** | Integración con sistemas de monitoreo centralizados | UDP/TCP a servidor syslog configurado |

### Tipos de Alarma

| Nivel | Descripción | Canales | Ejemplo |
|-------|-------------|---------|---------|
| **CRÍTICA** | Acción inmediata requerida | Email + Syslog + Windows Event | Espacio < 15%, fallo de ejecución |
| **ADVERTENCIA** | Atención próxima requerida | Email + Windows Event | Crecimiento anormal, threshold cercano |
| **INFORMATIVA** | Registro para auditoría | Windows Event + Syslog | Ejecución exitosa, cambio de política |

### Coalescing

- Si una alarma CRÍTICA de "espacio bajo" se generó hace < 4 horas y la condición persiste, NO se reenvía
- Si la condición empeora (ej: de 12% a 8%), se genera nueva alarma con contexto actualizado
- Si la condición se resuelve y vuelve a ocurrir, se trata como alarma nueva

### Escalación

```
T+0:    Alarma CRÍTICA → Email a operadores + Syslog + Windows Event
T+2h:   Si no se atendió → Email a ingeniero de guardia (María)
T+4h:   Si no se atendió → Email a gerente de operaciones (Roberto)
```

### Silencio Temporal

- Operador puede silenciar alarma por 1-24 horas (configurable)
- Silencio se registra en bitácora: quién, cuándo, por cuánto tiempo, razón
- Al expirar silencio, si condición persiste, se genera nueva alarma
- Alarmas silenciadas siguen registrándose en bitácora (solo se suprime notificación)

## Alternativas Consideradas

### Alternativa 1: Solo email
- **Pros:** Simple, universal, familiar
- **Contras:** Depende de conectividad de red y servidor SMTP; latencia variable; sin registro local si red falla; no integra con SIEM

### Alternativa 2: Solo Windows Event Log
- **Pros:** Nativo, sin dependencias externas, siempre disponible
- **Contras:** Operadores no revisan Event Log proactivamente; sin notificación push; requiere herramientas de monitoreo adicionales

### Alternativa 3: Integración con plataforma de monitoreo (PRTG, Nagios)
- **Pros:** Profesional, dashboards, escalación incorporada
- **Contras:** Dependencia de infraestructura que ODL puede no tener; complejidad de integración; costo de licencias; overhead para un solo servidor

### Alternativa 4: SMS / WhatsApp
- **Pros:** Notificación push inmediata en celular
- **Contras:** Requiere gateway SMS (costo); WhatsApp no tiene API empresarial estable; dependencia de servicio externo; no auditable de forma estándar

## Justificación

1. **Resiliencia:** Tres canales independientes garantizan que al menos uno funcione incluso con fallas parciales de red
2. **Integración empresarial:** Syslog y Windows Event permiten integración futura con SIEM sin cambios en la aplicación
3. **Notificación proactiva:** Email llega al buzón del operador sin que tenga que buscar (a diferencia de Event Log)
4. **Coalescing inteligente:** Evita fatiga de alarmas que lleva a operadores a ignorar notificaciones (alarm fatigue)
5. **Escalación automática:** Si operador no responde, la alarma sube de nivel sin intervención manual
6. **Sin dependencias externas:** Los tres canales usan infraestructura estándar de Windows/red corporativa

## Consecuencias

### Positivas
- Operador notificado dentro de 30 segundos por múltiples vías
- Escalación automática previene que alarmas se pierdan
- Coalescing reduce ruido manteniendo visibilidad
- Historial completo para auditoría
- Integración con SIEM futura sin cambios

### Negativas
- Configuración de SMTP requiere datos de servidor de correo (ODL debe proveer)
- Tres canales implican tres posibles puntos de falla a diagnosticar
- Coalescing puede retrasar re-notificación si condición cambia levemente
- Testing de email/syslog requiere infraestructura de red funcional

### Configuración Requerida del Cliente (ODL)

| Parámetro | Descripción | Ejemplo |
|-----------|-------------|---------|
| SMTP Server | Servidor de correo | mail.odl.com:587 |
| Email To | Destinatarios operacionales | operaciones@odl.com |
| Email Escalación | Destinatarios escalación | ingenieria@idc.com |
| Syslog Server | Servidor syslog (si aplica) | 192.168.1.100:514 |
| Escalación T1 | Tiempo primera escalación | 2 horas |
| Escalación T2 | Tiempo segunda escalación | 4 horas |

---

**Revisores:** [Pendiente]  
**Aprobado por:** [Pendiente]
