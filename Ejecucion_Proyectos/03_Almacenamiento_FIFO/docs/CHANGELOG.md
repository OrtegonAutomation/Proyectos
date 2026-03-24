# Changelog — FifoCleanup

Todos los cambios notables de este proyecto se documentan en este archivo.

El formato está basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.1.0/).

---

## [1.0.0] — 2026-03-09

### Primera versión de producción

Aprobada para deployment en servidor SRVODLRTDNMICA tras 89 tests (82 pasados, 7 N/A por funcionalidades diferidas).

### Agregado
- **RF-01:** Inventario inteligente de almacenamiento con estructura `Asset/Variable/E|F/YYYY/MM/DD`
- **RF-02:** Gestión de configuración JSON con validación de rangos y valores por defecto calibrados
- **RF-03:** Motor de simulación con datos sintéticos en disco real y simulación continua
- **RF-04:** Limpieza FIFO general (proporcional) y local (por Asset/Variable) con throttling de I/O
- **RF-05:** Bitácora de auditoría CSV append-only con rotación (100 MB) y retención configurable (90 días)
- **RF-07:** Servicio de limpieza programada con proyección histórica de 7 días
- **RF-08:** Servicio de monitoreo preventivo con FileSystemWatcher y evaluación por lotes
- Interfaz WPF con 6 pestañas: Dashboard, Configuración, Simulación, Ejecución, Bitácora
- Semáforo de estado (verde/amarillo/rojo) con gráficas LiveCharts
- Manejo global de excepciones (Dispatcher, Domain, Task)
- Soporte para `MaxStorageSizeGB` (capacidad virtual)
- Suite de pruebas: 89 tests automatizados con reportes TSV y Excel

### No incluido (diferido a v2.0)
- **RF-06:** Sistema de alarmas multicanal (email, syslog, Windows Event Log)
- **RF-09:** Control de acceso basado en roles (RBAC)
- Lista blanca de rutas protegidas
- Patrones de exclusión con wildcards
- Versionado automático de configuración
- Ejecución como servicio de Windows

---

## [0.9.0] — 2026-02-23

### Versión de pruebas

### Agregado
- Implementación completa de RF-01 a RF-05, RF-07, RF-08
- Suite de pruebas inicial (89 tests)

### Corregido
- **BUG-001:** RF-08 fallaba al iniciar si `StoragePath` no existía en la máquina
  - Solución: 3 niveles de validación (ConfigurationService, PreventiveMonitorService, ExecutionViewModel)
- **BUG-002:** Inventario contaba carpetas de tendencias en el total de datos
  - Solución: Solo se cuentan subcarpetas E y F

---

## [0.3.0] — 2026-02-16

### Migración a WPF

### Cambiado
- Migración de casos de uso CLI a interfaz WPF de 6 pestañas
- MVVM con CommunityToolkit.Mvvm

### Decisiones de arquitectura
- ADR-0001: Arquitectura 100% C#/.NET 8.0 (simplificado desde diseño WPF+C++)
- ADR-0002: Estrategia FIFO dual RF-07/RF-08
- ADR-0003: Formato JSON unificado (simplificado desde diseño INI+JSON)
- ADR-0004: Bitácora CSV inmutable
- ADR-0005: Eliminación por lotes con validación
- ADR-0006: Sistema de alarmas (diseño, diferido a v2.0)
- ADR-0007: Simulación determinística

---

## [0.2.0] — 2026-02-01

### Diseño inicial

### Agregado
- Criterios de aceptación (CA-01 a CA-12)
- Historias de usuario (HU-01 a HU-16)
- Requerimientos funcionales y no funcionales
- Plan de testeo

---

## [0.1.0] — 2026-01-15

### Inicio del proyecto

### Agregado
- Acta de constitución del proyecto
- Declaración de alcance
- Cronograma de trabajo

---

*Mantenido por IDC Ingeniería*
