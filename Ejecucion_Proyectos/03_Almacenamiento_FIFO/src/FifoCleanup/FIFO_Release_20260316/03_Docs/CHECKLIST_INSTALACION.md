# Checklist instalación FIFO

- [ ] Validar servidor Windows x64 y permisos de escritura.
- [ ] Confirmar ruta del paquete `FIFO_Release_20260316`.
- [ ] Ejecutar `02_Scripts/01_Instalar_FIFO.ps1`.
- [ ] Verificar en `C:\Apps\FIFO`:
  - [ ] `FifoCleanup.exe`
  - [ ] `fifo_config.json`
  - [ ] `bitacora\`
- [ ] Ajustar `storagePath` en `fifo_config.json` si aplica.
- [ ] Validar `enableEmailAlerts=false` para servidores sin internet (recomendado en este despliegue).
- [ ] Solo si existe relay SMTP interno: configurar `smtpHost/smtpPort/smtpFrom` y luego habilitar `enableEmailAlerts=true`.
- [ ] Si el servidor NO tiene internet: usar relay SMTP interno corporativo o dejar `enableEmailAlerts=false`.
- [ ] Ejecutar `02_Scripts/02_SmokeTest_FIFO.ps1`.
- [ ] Confirmar creación de `bitacora_YYYYMMDD.csv`.
- [ ] Validar tarea programada `FIFO_AutoStart` (arranque automático tras reinicio).
- [ ] Registrar evidencia en `EVIDENCIA_PRUEBAS.md`.
- [ ] Validar plan de rollback con `03_Rollback_FIFO.ps1`.