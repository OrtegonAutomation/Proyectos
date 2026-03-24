# FifoCleanup — Gestión Automática de Almacenamiento FIFO

**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Cliente:** ODL Instrumentación y Control  
**Desarrollador:** IDC Ingeniería  
**Versión:** 1.0.0  
**Plataforma:** Windows (.NET 8.0 / WPF)  
**Servidor destino:** SRVODLRTDNMICA (Intel Xeon E5-2695 v4, 16 GB RAM)

---

## Descripción

FifoCleanup es una aplicación de escritorio WPF que gestiona automáticamente el almacenamiento del servidor de monitoreo industrial de ODL. El sistema aplica una política **FIFO (First In, First Out)** para eliminar los datos más antiguos cuando el disco se acerca a su capacidad, garantizando que el software de monitoreo (Aspen Mtell) nunca se quede sin espacio.

### Características principales

- **Dashboard en tiempo real** — Visualización del estado del disco con semáforo (verde/amarillo/rojo), gráficas de uso y proyección de llenado
- **Inventario inteligente** — Escaneo de la estructura `Asset/Variable/E|F/YYYY/MM/DD` con cálculo de tasas de crecimiento
- **Simulación segura** — Genera datos sintéticos para probar la política FIFO antes de ejecutarla en producción
- **Limpieza FIFO dual:**
  - **RF-07 (Programada):** Evaluación periódica basada en proyección histórica de 7 días
  - **RF-08 (Preventiva):** Monitoreo en tiempo real con `FileSystemWatcher`, reacciona a picos de crecimiento
- **Bitácora de auditoría** — Registro inmutable en CSV de todas las operaciones, con rotación automática y retención configurable

---

## Inicio Rápido

### Prerequisitos

- Windows 10/11 o Windows Server 2016+
- [.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (Desktop Runtime para WPF)
- Acceso de lectura/escritura a la carpeta de datos de monitoreo

### Instalación

1. Copiar la carpeta `FifoCleanup/` publicada al servidor destino (ej: `C:\FifoCleanup\`)
2. Ejecutar `FifoCleanup.exe`
3. En la pestaña **Configuración**, establecer:
   - **Ruta de almacenamiento:** la carpeta raíz donde están los Assets (ej: `D:\MonitoringData`)
   - **Umbral de limpieza:** porcentaje de uso que dispara la limpieza (default: 85%)
   - **Cap de limpieza:** máximo porcentaje a eliminar por ejecución (default: 20%)
4. Guardar configuración
5. Ejecutar un **inventario** desde el Dashboard para verificar que detecte los Assets
6. Ejecutar una **simulación** para validar el comportamiento esperado
7. Activar RF-07 y RF-08 desde la pestaña de Ejecución

### Compilación desde código fuente

```bash
cd src/FifoCleanup
dotnet restore
dotnet build -c Release
dotnet publish FifoCleanup.UI -c Release -o ./publish
```

---

## Estructura del proyecto

```
03_Almacenamiento_FIFO/
├── README.md                          ← Este archivo
├── docs/                              ← Documentación completa
│   ├── Decisiones_arquitectura/       ← 7 ADRs
│   ├── Requisitos/                    ← CU, RF, RNF, HU, CA
│   ├── Operaciones/                   ← Manual de operación, runbook, troubleshooting
│   ├── testing/                       ← Plan y resultados de pruebas
│   └── project_management/            ← Acta, alcance, cronograma
├── src/FifoCleanup/                   ← Código fuente
│   ├── FifoCleanup.Engine/            ← Librería core (Models + Services)
│   ├── FifoCleanup.UI/               ← Aplicación WPF (6 pestañas)
│   └── FifoCleanup.Tests/            ← Suite de pruebas (89 tests)
└── Tests/FifoTestBed/                 ← Datos de prueba y bitácoras de ejemplo
```

---

## Arquitectura

| Proyecto | Tipo | Propósito |
|----------|------|-----------|
| **FifoCleanup.Engine** | Class Library (.NET 8.0) | Motor FIFO: inventario, limpieza, simulación, servicios RF-07/RF-08 |
| **FifoCleanup.UI** | WPF App (.NET 8.0-windows) | Interfaz gráfica de 6 pestañas con MVVM (CommunityToolkit.Mvvm + LiveCharts) |
| **FifoCleanup.Tests** | Console App (.NET 8.0) | 89 tests automatizados con generación de reportes TSV/Excel |

---

## Requerimientos Funcionales

| RF | Descripción | Estado |
|----|-------------|--------|
| RF-01 | Inventario y caracterización del almacenamiento | ✅ Implementado |
| RF-02 | Definición y gestión de política de retención | ✅ Implementado |
| RF-03 | Motor de simulación con datos sintéticos | ✅ Implementado |
| RF-04 | Ejecución FIFO en producción | ✅ Implementado |
| RF-05 | Bitácora de auditoría inmutable | ✅ Implementado |
| RF-06 | Sistema de alarmas multicanal | ⏳ Diferido a v2.0 |
| RF-07 | Limpieza programada (evaluación periódica) | ✅ Implementado |
| RF-08 | Limpieza preventiva (FileSystemWatcher) | ✅ Implementado |
| RF-09 | Control de acceso basado en roles (RBAC) | ⏳ Diferido a v2.0 |

---

## Tests

**89 tests totales** — 82 pasados, 0 fallidos, 7 N/A (funcionalidades diferidas)

```bash
cd src/FifoCleanup
dotnet run --project FifoCleanup.Tests
```

Los reportes se generan automáticamente en `Tests/FifoTestBed/Reportes/`.

---

## Documentación adicional

| Documento | Ubicación |
|-----------|-----------|
| Manual de Operación | [docs/Operaciones/MANUAL_DE_OPERACION.md](docs/Operaciones/MANUAL_DE_OPERACION.md) |
| Runbook | [docs/Operaciones/RUNBOOK.md](docs/Operaciones/RUNBOOK.md) |
| Troubleshooting | [docs/Operaciones/TROUBLESHOOTING.md](docs/Operaciones/TROUBLESHOOTING.md) |
| FAQ | [docs/Operaciones/FAQ.md](docs/Operaciones/FAQ.md) |
| Arquitectura | [docs/ARQUITECTURA.md](docs/ARQUITECTURA.md) |
| Especificación Técnica | [docs/ESPECIFICACION_TECNICA.md](docs/ESPECIFICACION_TECNICA.md) |
| Guía de Configuración | [docs/Operaciones/GUIA_CONFIGURACION.md](docs/Operaciones/GUIA_CONFIGURACION.md) |
| Guía de Instalación | [docs/Operaciones/GUIA_INSTALACION.md](docs/Operaciones/GUIA_INSTALACION.md) |
| Changelog | [docs/CHANGELOG.md](docs/CHANGELOG.md) |
| Contactos y SLA | [docs/Operaciones/CONTACTOS_SLA.md](docs/Operaciones/CONTACTOS_SLA.md) |
| Material de Capacitación | [docs/CAPACITACION.md](docs/CAPACITACION.md) |

---

## Licencia

Software propietario desarrollado por IDC Ingeniería para ODL Instrumentación y Control.  
Todos los derechos reservados © 2026.

---

## Contacto

**IDC Ingeniería** — Soporte técnico  
Para incidentes críticos o consultas, ver [docs/Operaciones/CONTACTOS_SLA.md](docs/Operaciones/CONTACTOS_SLA.md).
