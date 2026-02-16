# ADR-0004: Bitácora de Auditoría Inmutable con Rotación y Firma Digital

**Estado:** Aceptada  
**Fecha:** 2026-02-16  
**Autor:** IDC Ingeniería  
**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Cliente:** ODL Instrumentación y Control

---

## Contexto

El sistema FIFO elimina archivos de forma automática y manual en un servidor de monitoreo crítico. Se requiere trazabilidad completa para:

- Auditoría interna y externa (HU-13): ¿qué se eliminó, cuándo, por qué, quién lo autorizó?
- Troubleshooting (HU-10): ¿por qué falló una ejecución?
- Cumplimiento normativo: demostrar que no hubo eliminaciones no autorizadas
- Confianza del cliente: ODL necesita evidencia de que el sistema opera correctamente

Requisitos específicos de la bitácora (CA-05):
- Timestamp exacto por operación
- Inmutable después de cierre de sesión
- Retención mínima 90 días
- Rotación a los 100 MB
- Hash o firma digital para detectar modificaciones
- Exportación a CSV

## Decisión

**Se implementa sistema de bitácora basado en archivos de texto plano con firma SHA-256, rotación automática por tamaño y compresión de archivos históricos.**

### Estructura de Archivo

```
[2026-02-16 10:45:22] OPERACIÓN: CLEANUP | RESULTADO: OK | TIPO: Programada (RF-07) | USUARIO: SYSTEM
  - Política: policy_v2.ini
  - Archivos eliminados: 252
  - Espacio liberado: 32.5 GB
  - Assets procesados: Asset1, Asset2, Asset3
  - Proyección que motivó ejecución: 87% en 24h (threshold: 85%)
  - Duración: 47 segundos
  - SHA256_LINEA: a1b2c3d4...
```

### Ciclo de Vida

1. **Activa:** `fifo_bitacora.log` — archivo actual, append-only
2. **Cerrada:** Al alcanzar 100 MB → renombrar a `fifo_bitacora_20260216_104522.log`
3. **Firmada:** Se calcula SHA-256 del archivo cerrado → `fifo_bitacora_20260216_104522.sha256`
4. **Comprimida:** ZIP del archivo cerrado + firma → `fifo_bitacora_20260216_104522.zip`
5. **Purgada:** Archivos ZIP > 90 días se eliminan (configurable)

### Protección de Integridad

- Cada línea incluye hash parcial para detección de modificación inline
- Archivo cerrado tiene SHA-256 completo en archivo separado
- Permisos NTFS: solo SYSTEM y Administrators pueden escribir; operadores solo lectura
- Archivo activo es append-only (no se puede truncar sin permisos elevados)

## Alternativas Consideradas

### Alternativa 1: Base de datos SQLite
- **Pros:** Consultas SQL rápidas, índices, transaccional
- **Contras:** Archivo binario no legible directamente; requiere herramientas para inspección; corrupción de DB puede perder todo el histórico; overhead de SQLite para escrituras secuenciales simples

### Alternativa 2: Windows Event Log
- **Pros:** Integración nativa, herramientas de visualización estándar, centralización con SIEM
- **Contras:** No portable; límite de tamaño de evento; difícil exportar a CSV; no permite firma digital personalizada; estructura rígida

### Alternativa 3: Syslog remoto
- **Pros:** Centralizado, inmutable por diseño (servidor separado)
- **Contras:** Requiere infraestructura de red; no disponible offline; latencia de red; ODL puede no tener servidor syslog dedicado

### Alternativa 4: Base de datos relacional (SQL Server / PostgreSQL)
- **Pros:** Escalable, consultas complejas, backup integrado
- **Contras:** Overhead operacional excesivo para un solo servidor; requiere instalación y mantenimiento de DBMS; aumenta requisitos de infraestructura

## Justificación

1. **Simplicidad:** Archivos de texto plano son legibles con Notepad, sin herramientas especiales
2. **Inmutabilidad:** Permisos NTFS + hash SHA-256 proveen integridad verificable sin infraestructura adicional
3. **Rendimiento:** Append-only es la operación de I/O más rápida; no afecta rendimiento del motor FIFO
4. **Rotación predecible:** 100 MB por archivo evita archivos gigantes; compresión ZIP reduce almacenamiento ~70%
5. **Portabilidad:** Archivo ZIP con log + firma es auto-contenido para entrega a auditores externos
6. **Consulta eficiente:** UI (WPF) indexa registros en memoria para filtrado rápido (CA-05-08, < 5 segundos para 1000 registros)

## Consecuencias

### Positivas
- Trazabilidad completa verificable por terceros
- Archivos legibles sin herramientas especiales
- Bajo consumo de disco (compresión ZIP)
- Resistente a corrupción (archivos independientes)
- Exportación a CSV trivial (parsing de texto estructurado)

### Negativas
- Consultas complejas requieren cargar archivo en memoria (no hay SQL nativo)
- Parsing de texto es más lento que consulta de base de datos para volúmenes grandes
- SHA-256 por línea agrega overhead de escritura (~5% por operación)
- Archivos de texto son modificables si se obtienen permisos de SYSTEM

### Mitigaciones
- UI pre-carga e indexa bitácora activa al iniciar (cache en memoria)
- Para auditorías, archivo ZIP + SHA-256 externo provee verificación definitiva
- Backup diario automático de bitácora activa (CA-08-08)

## Esquema de Registro

| Campo | Tipo | Ejemplo |
|-------|------|---------|
| Timestamp | ISO 8601 | 2026-02-16 10:45:22 |
| Operación | Enum | INVENTORY, SIMULATE, CLEANUP, CONFIG_CHANGE, ALARM |
| Resultado | Enum | OK, ERROR, ABORTED, SKIPPED |
| Tipo | String | Programada (RF-07), Preventiva (RF-08), Manual |
| Usuario | String | SYSTEM, maria.ingenieria |
| Detalles | Multi-línea | Archivos eliminados, espacio liberado, etc. |
| Hash línea | SHA-256 parcial | Primeros 16 caracteres |

---

**Revisores:** [Pendiente]  
**Aprobado por:** [Pendiente]
