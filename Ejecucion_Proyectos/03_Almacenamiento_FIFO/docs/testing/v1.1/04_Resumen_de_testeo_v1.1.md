# Resumen de Testeo v1.1 — Nuevas Funcionalidades FIFO

**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Versión:** 1.1  
**Fecha de ejecución:** 2026-03-19 18:12:14  
**Ejecutor:** Camilo Ortegon

---

## 1. Resultado global

| Métrica | Valor |
|--------|-------|
| Total casos | 4 |
| Pasados | 4 |
| Fallados | 0 |
| N/A | 0 |
| Estado | ✅ Aprobado |

---

## 2. Resultados por caso

| ID | Área | Resultado | Observación |
|----|------|-----------|-------------|
| TC-11-01 | v1.1-Reinicio | ✅ PASÓ | RF-07 y RF-08 reactivados tras reinicio simulado |
| TC-11-02 | v1.1-Monitoreo | ✅ PASÓ | Monitoreo en tiempo real activo sin limpiezas (`PreventiveCleanups=0`) |
| TC-11-03 | v1.1-Monitoreo | ✅ PASÓ | RF-07 entra en ventana de gracia al reactivar y no limpia de inmediato |
| TC-11-04 | v1.1-Alertas | ✅ PASÓ | Error SMTP manejado sin afectar ejecución (sin internet) |

---

## 3. Evidencias generadas

- TSV: `D:\FifoTestBed\Reportes\v1.1\TestReport_20260319_181214.tsv`
- Excel: `docs/testing/v1.1/01_Casos_Test_v1.1.xlsx`

---

## 4. Conclusión

La validación de v1.1 confirma el comportamiento esperado para:
- reactivación de servicios tras reinicio,
- monitoreo en tiempo real y reactivación de RF-07/RF-08 sin disparar limpieza automática inmediata al arrancar,
- resiliencia operativa en servidores sin conectividad a internet para SMTP externo.

**Configuración operativa adoptada para despliegue actual:**
- `enableEmailAlerts = false` (sin internet)
- `startupMonitoringGraceMinutes = 10` (reactiva RF con monitoreo sin limpieza inmediata)
- Alertamiento vía bitácora local (`bitacora_*.csv`) hasta disponer de relay SMTP interno.

**Recomendación:** ✅ Aprobado para uso en comité técnico v1.1.
