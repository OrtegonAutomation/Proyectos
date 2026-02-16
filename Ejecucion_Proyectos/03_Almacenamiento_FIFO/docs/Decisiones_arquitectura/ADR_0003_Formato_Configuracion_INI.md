# ADR-0003: Formato INI para Archivos de Configuración de Políticas

**Estado:** Aceptada  
**Fecha:** 2026-02-16  
**Autor:** IDC Ingeniería  
**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Cliente:** ODL Instrumentación y Control

---

## Contexto

El sistema FIFO requiere archivos de configuración para definir políticas de retención que incluyen:

- Umbrales de disco (crítico y recuperación)
- Criterio FIFO (fecha de creación o modificación)
- Lista blanca de rutas protegidas (mínimo 20 rutas)
- Patrones de exclusión con wildcards
- Parámetros de ejecución automática (RF-07 y RF-08)

Estos archivos deben ser:
- Legibles por operadores sin experiencia en programación (CA-02-01)
- Editables manualmente si la UI no está disponible
- Versionables automáticamente (CA-02-07)
- Validables contra errores de sintaxis (CA-02-09)

## Decisión

**Se adopta formato INI como formato principal de configuración de políticas, complementado con JSON para configuración de UI.**

### Configuración de Política (INI)
```ini
[Thresholds]
critical_percent = 85
recovery_percent = 70

[Retention]
fifo_criteria = creation_date

[Whitelist]
path_1 = C:\Monitored\Vibration
path_2 = C:\Monitored\Process

[Exclusions]
pattern_1 = *.log
pattern_2 = *.tmp
```

### Configuración de UI/Aplicación (JSON)
```json
{
  "rf07_frequency_hours": 24,
  "rf07_execution_time": "02:00",
  "rf08_threshold_days": 3,
  "rf08_enabled": true,
  "notification_channels": ["email", "syslog", "windows_event"]
}
```

### Versionado Automático
- Cada cambio genera: `policy_v1.ini` → `policy_v2.ini` → `policy_v3.ini`
- Rollback disponible seleccionando versión anterior (< 10 segundos, CA-02-08)
- Archivo activo es siempre un symlink/copia a la versión vigente

## Alternativas Consideradas

### Alternativa 1: YAML
- **Pros:** Legible, soporta estructuras complejas, comentarios nativos
- **Contras:** Sensible a indentación (errores sutiles); requiere parser externo en C++; operadores ODL no están familiarizados con YAML

### Alternativa 2: JSON para todo
- **Pros:** Parser nativo en C++ (nlohmann/json); estándar universal
- **Contras:** No soporta comentarios (crítico: operadores necesitan explicaciones en español en el archivo); comillas y llaves son confusas para usuarios no técnicos; no legible sin formateador

### Alternativa 3: XML
- **Pros:** Robusto, validable con XSD, estándar empresarial
- **Contras:** Verboso; difícil de editar manualmente; overhead de parsing; operadores lo perciben como "código"

### Alternativa 4: Registro de Windows
- **Pros:** Nativo, seguro, transaccional
- **Contras:** No portable; difícil de versionar; no exportable/legible; requiere herramientas especiales para editar

## Justificación

1. **Legibilidad máxima:** INI es el formato más intuitivo para operadores industriales: `clave = valor` sin sintaxis compleja
2. **Comentarios en español:** INI permite `; Comentario` para explicar cada sección en idioma nativo del operador
3. **Parsing trivial en C++:** `GetPrivateProfileString` de Win32 API parsea INI sin dependencias externas
4. **Historial en Windows:** Operadores de servidores Windows están familiarizados con archivos `.ini` desde hace décadas
5. **Validación simple:** Estructura plana (sección + clave + valor) facilita validación programática contra errores (CA-02-09)
6. **JSON complementario:** Para configuración de UI que no es editada manualmente, JSON ofrece mejor tipado (booleanos, arrays)

## Consecuencias

### Positivas
- Operadores pueden leer y entender política sin herramientas especiales
- Archivo auto-documentado con comentarios en español
- Parsing sin dependencias externas en C++ (Win32 API)
- Versionado simple: copiar archivo con sufijo incremental
- Editable con Notepad en emergencias

### Negativas
- INI no soporta estructuras anidadas (listas de objetos complejos)
- Límite práctico de ~50 entradas por sección antes de ser engorroso
- Dos formatos de configuración (INI + JSON) requieren dos parsers
- INI no tiene tipado fuerte (todo es string, requiere conversión)

### Mitigaciones
- Lista blanca usa convención `path_1`, `path_2`, ..., `path_N` para simular arrays
- JSON solo se usa internamente (UI); operador nunca lo edita directamente
- Validación en carga convierte y verifica tipos (ej: `critical_percent` debe ser entero 50-95)

---

**Revisores:** [Pendiente]  
**Aprobado por:** [Pendiente]
