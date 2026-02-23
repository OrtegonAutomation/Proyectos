---
name: auto-documentacion
description: Automatiza la generación de documentación de código, arquitectura y reportes administrativos
---

# Skill: Auto-Documentación

## Propósito
Este skill automatiza la creación y actualización de documentación en tres categorías:
1. **Documentación de Código** - Docstrings, comentarios, API reference
2. **Documentación de Arquitectura** - Decisiones arquitectónicas, diagramas, ADRs
3. **Reportes Administrativos** - Estado del proyecto, auditorías, reportes de riesgo

## Convenciones de Documentación

### 1. Documentación de Código

#### Docstrings
- **Python/C#**: Usar Google style docstrings
- **C++**: Usar Doxygen comments
- **JavaScript/TypeScript**: Usar JSDoc

```python
def procesar_archivo(path: str, buffer_size: int = 4096) -> bool:
    """Procesa un archivo FIFO siguiendo el protocolo establecido.
    
    Args:
        path: Ruta al archivo FIFO a procesar
        buffer_size: Tamaño del buffer en bytes (default 4096)
    
    Returns:
        True si el procesamiento fue exitoso, False en caso contrario
    
    Raises:
        FileNotFoundError: Si el archivo no existe
        IOError: Si hay error al leer/escribir
    """
```

#### Comentarios de Código
- Explicar el "por qué", no el "qué"
- Una línea antes de bloques lógicos complejos
- Mantener actualizado con cambios

### 2. Documentación de Arquitectura

#### Estructura de ADR (Architecture Decision Record)
```
Decisiones_arquitectura/ADR_XXXX_Titulo_Descriptivo.md

# ADR XXXX: Título Descriptivo

## Estado: [Proposed/Accepted/Deprecated/Superseded]

## Contexto
[Descripción del problema/situación]

## Decisión
[Decisión tomada]

## Consecuencias
[Impacto técnico, positivo y negativo]

## Alternativas Consideradas
[Otras opciones evaluadas]
```

#### Documentación de Componentes
- Incluir responsabilidades del componente
- Interfaces públicas
- Dependencias
- Ejemplo de uso

### 3. Reportes Administrativos

#### Reportes de Estado Semanal (WEEKLY_STATUS.docx)
- **Estado General**: En curso / En riesgo / Completado
- **Progreso**: % completado, hitos alcanzados
- **Riesgos Identificados**: Lista con nivel (Alto/Medio/Bajo)
- **Próximos Pasos**: Actividades previstas

#### Registro de Riesgos (RISK_REGISTER.xlsx)
- ID del riesgo
- Descripción
- Probabilidad (Alta/Media/Baja)
- Impacto (Alto/Medio/Bajo)
- Nivel de Riesgo (Probabilidad × Impacto)
- Plan de Mitigación

#### Resumen Ejecutivo (EXECUTIVE_SUMMARY.pptx)
- Slide 1: Portada (Proyecto, Fecha, Responsable)
- Slide 2: Estado General (KPIs clave)
- Slide 3: Hitos Completados
- Slide 4: Riesgos Principales
- Slide 5: Próximos Pasos
- Slide 6: Contacto

## Tareas Automatizadas

### Cuando se ejecute "generar-docs-codigo"
1. Analizar funciones/métodos sin docstring
2. Generar docstrings siguiendo convenciones del lenguaje
3. Crear archivo API_REFERENCE.md
4. Listar todas las funciones públicas con descripción

### Cuando se ejecute "generar-docs-arquitectura"
1. Revisar cambios en Decisiones_arquitectura/
2. Generar índice de ADRs
3. Crear diagrama de relaciones entre componentes
4. Documentar cambios en interfaces

### Cuando se ejecute "generar-reportes"
1. Recopilar métricas de progreso
2. Identificar riesgos actuales
3. Generar WEEKLY_STATUS.docx
4. Actualizar RISK_REGISTER.xlsx
5. Generar EXECUTIVE_SUMMARY.pptx

## Formato de Solicitud

Para generar documentación automáticamente, usar:

```
@copilot generar-docs-codigo [ruta_opcional]
@copilot generar-docs-arquitectura
@copilot generar-reportes [semana]
```

## Restricciones y Límites

- ✗ No documentar secretos, credentials o información sensible
- ✗ No modificar código de producción automáticamente
- ✗ Reportes deben ser revisados por PM antes de publicarse
- ✓ Mantener tono profesional en documentación técnica
- ✓ Incluir fecha de última actualización
- ✓ Referencias cruzadas entre documentos

## Estructura de Carpetas Esperada

```
proyecto/docs/
├── Decisiones_arquitectura/
│   ├── ADR_0001_*.md
│   ├── ADR_0002_*.md
│   └── INDEX.md
├── project_management/
│   ├── WEEKLY_STATUS.docx
│   ├── EXECUTIVE_SUMMARY.pptx
│   └── RISK_REGISTER.xlsx
├── Requisitos/
├── testing/
└── Operaciones/
```

## Ejemplos de Uso

### Documentar código C++
```
Analiza src/FileProcessor.cpp y genera docstrings Doxygen para:
- class FileProcessor
- método procesoFIFO()
- método validarBuffer()
```

### Documentar decisión arquitectónica
```
Crea ADR sobre la integración WPF-C++:
- Contexto: Necesidad de comunicación entre UI y backend
- Decisión: JSON messages over Named Pipes
- Consecuencias: Latency trade-off, Desacoplamiento de arquitectura
```

### Generar reporte semanal
```
Recopila:
- Commits desde lunes
- Bugs cerrados
- Features completadas
- Riesgos identificados en los issues
Genera WEEKLY_STATUS.docx actualizado
```

## Notas Importantes

- Esta skill usa convenciones PMI basadas en el proyecto 03_Almacenamiento_FIFO
- Los reportes se generan principalmente en formato Office (.docx, .xlsx, .pptx)
- Los ADRs se mantienen en Markdown para control de versiones
- Todos los documentos incluyen timestamp de generación
