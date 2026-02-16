# ADR-0001: Arquitectura WPF + C++ para Aplicación FIFO

**Estado:** Aceptada  
**Fecha:** 2026-02-16  
**Autor:** IDC Ingeniería  
**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Cliente:** ODL Instrumentación y Control

---

## Contexto

Se requiere una aplicación de escritorio para gestionar almacenamiento FIFO en servidores de monitoreo industrial de ODL. El sistema debe:

- Operar directamente sobre el sistema de archivos Windows con alto rendimiento
- Inventariar hasta 2 TB de datos (5M archivos) en menos de 90 segundos
- Eliminar archivos por lotes con < 10 segundos por lote de 100 archivos
- Consumir máximo 25% CPU y 500 MB RAM
- Proveer interfaz gráfica intuitiva para operadores sin experiencia técnica avanzada
- Ejecutar procesos automáticos en background 24/7 (RF-07 programada + RF-08 preventiva)

## Decisión

**Se adopta arquitectura híbrida WPF (C#/.NET) + C++ nativo con comunicación por mensajes JSON.**

- **Frontend (WPF/C#):** Interfaz gráfica con 6 pestañas (Dashboard, Configuración, Simulación, Ejecución, Bitácora, Reportes)
- **Backend (C++ nativo):** Motor FIFO de alto rendimiento para escaneo de disco, cálculo de eliminaciones y operaciones de I/O intensivas
- **Comunicación:** Mensajes JSON entre procesos (WPF ↔ C++)

## Alternativas Consideradas

### Alternativa 1: Aplicación 100% C# / WPF
- **Pros:** Stack unificado, desarrollo más rápido, ecosistema .NET maduro
- **Contras:** Rendimiento inferior para operaciones intensivas de I/O sobre millones de archivos; garbage collector puede causar pausas impredecibles durante escaneos masivos

### Alternativa 2: Aplicación 100% C++ con Qt o Win32
- **Pros:** Máximo rendimiento, control total de memoria
- **Contras:** Desarrollo de UI significativamente más lento; mayor complejidad para data binding, tablas filtrables, gráficas; curva de aprendizaje de Qt

### Alternativa 3: Aplicación Web (Electron / React)
- **Pros:** UI moderna, cross-platform potencial
- **Contras:** Overhead de Chromium inaceptable (300+ MB RAM solo para el shell); latencia en acceso a sistema de archivos; complejidad de empaquetado; servidor Windows ya tiene restricciones de recursos

### Alternativa 4: CLI pura (PowerShell / scripts)
- **Pros:** Mínimo overhead, fácil de automatizar
- **Contras:** Rechazada por requisitos explícitos de usabilidad (CA-10); operadores necesitan interfaz visual sin entrenamiento técnico extensivo

## Justificación

1. **Rendimiento crítico:** C++ nativo permite escaneo paralelo de disco con `FindFirstFile`/`FindNextFile` sin overhead de marshalling .NET, cumpliendo CA-07 (inventario 500 GB < 60s, 2 TB < 90s)
2. **UI rica requerida:** WPF ofrece data binding nativo, controles de gráficas, tablas filtrables y progress bars que los operadores necesitan (HU-01 a HU-04)
3. **Separación de responsabilidades:** Motor FIFO independiente de la UI permite testing aislado del motor de eliminación sin interfaz gráfica
4. **Compatibilidad Windows:** Ambas tecnologías son nativas de Windows, sin dependencias externas en servidor de producción
5. **Seguridad:** Proceso C++ puede ejecutarse con permisos elevados de forma aislada; WPF corre con permisos de usuario

## Consecuencias

### Positivas
- Rendimiento predecible en operaciones de disco intensivas
- UI profesional con curva de aprendizaje mínima para operadores
- Motor FIFO testeable de forma independiente
- Bajo consumo de recursos en servidor de producción

### Negativas
- Requiere dos competencias de desarrollo (C++ y C#)
- Protocolo JSON entre procesos agrega complejidad de serialización/deserialización
- Debugging más complejo al cruzar frontera de procesos
- Deployment requiere ambos binarios (ejecutable C++ + aplicación WPF)

### Riesgos
- Si la comunicación JSON introduce latencia perceptible, considerar pipes nombrados o memoria compartida
- Si el proceso C++ falla, la UI debe manejar reconexión graceful

## Métricas de Validación

| Métrica | Objetivo | Referencia |
|---------|----------|------------|
| Inventario 500 GB | < 60s | CA-07-01 |
| Inventario 2 TB | < 90s | CA-07-02 |
| Eliminación 100 archivos | < 10s | CA-07-04 |
| CPU máximo | < 25% | CA-07-06 |
| RAM máximo | < 500 MB | CA-07-07 |
| Dashboard carga | < 2s | HU-01 |

---

**Revisores:** [Pendiente]  
**Aprobado por:** [Pendiente]
