# Plan de Pruebas v1.1 — Nuevas Funcionalidades FIFO

**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Versión:** 1.1  
**Estado:** Ejecutado  
**Fecha:** 2026-03-19

---

## 1. Objetivo

Validar únicamente las funcionalidades nuevas de v1.1:

1. Reactivación automática de `RF-07` y `RF-08` tras reinicio.
2. Operación en modo monitoreo en tiempo real con control de no eliminación no deseada.
3. Resiliencia de alertas email cuando no hay conectividad a internet.

---

## 2. Alcance

Incluye:
- Suite `FifoCleanup.Tests/TestSuites/V1_1/NewFeaturesV11Tests.cs`
- Ejecución por argumento de versión en `FifoCleanup.Tests/Program.cs`.
- Generación de reportes versionados (`v1.1`).

Excluye:
- Suite completa v1.0 (inventario, limpieza histórica, edge cases legacy).

---

## 3. Casos de prueba v1.1

| ID | Área | Objetivo | Resultado esperado |
|----|------|----------|--------------------|
| TC-11-01 | v1.1-Reinicio | Reactivar RF-07/RF-08 tras reinicio simulado | Ambos servicios quedan activos |
| TC-11-02 | v1.1-Monitoreo | RF-08 detecta eventos en tiempo real sin limpiar | `OnFileDetected=true`, `PreventiveCleanups=0` |
| TC-11-03 | v1.1-Monitoreo | RF-07 en modo monitoreo no ejecuta cleanup | `OnCleanupExecuted=false` |
| TC-11-04 | v1.1-Alertas | Email en entorno sin internet no rompe servicio | Falla controlada (`false`) sin excepción |

---

## 4. Ejecución

```powershell
cd src\FifoCleanup\FifoCleanup.Tests
dotnet run -c Release -- --version 1.1
```

Parámetro de configuración clave validado en v1.1:
- `startupMonitoringGraceMinutes = 10`

---

## 5. Evidencias y salidas

- TSV: `D:\FifoTestBed\Reportes\v1.1\TestReport_*.tsv`
- Excel: `docs/testing/v1.1/01_Casos_Test_v1.1.xlsx`
- Resumen: `docs/testing/v1.1/04_Resumen_de_testeo_v1.1.md`
