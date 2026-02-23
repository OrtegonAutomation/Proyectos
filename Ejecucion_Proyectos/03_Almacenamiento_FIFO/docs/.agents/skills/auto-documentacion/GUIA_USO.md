# Guía de Uso: Auto-Documentación

## Instalación

El skill ya está disponible en:
```
.agents/skills/auto-documentacion/SKILL.md
```

El agente especializado está disponible en:
```
.agents/agents.md
```

## Uso Rápido

### 1. Generar Documentación de Código

**Comando:**
```
@copilot generar-docs-codigo [ruta/opcional]
```

**Ejemplo:**
```
@copilot Analiza src/FileProcessor.cpp y genera docstrings Doxygen 
para todas las funciones públicas
```

**Resultado:**
- Docstrings en formato Doxygen
- Archivo `API_REFERENCE.md` actualizado
- Comentarios explicativos en código complejo

---

### 2. Documentar Decisiones Arquitectónicas

**Comando:**
```
@copilot Crear ADR sobre: [tema]
```

**Ejemplo:**
```
@copilot Crear ADR sobre la comunicación WPF-C++ usando Named Pipes.
Incluir contexto técnico, decisiones de diseño y alternativas rechazadas.
```

**Resultado:**
- Archivo `Decisiones_arquitectura/ADR_XXXX_*.md`
- Entrada en `INDEX.md`
- Formato estandarizado

---

### 3. Generar Reportes Administrativos

#### a) Reporte de Estado Semanal
```
@copilot Generar WEEKLY_STATUS para la semana del [fecha]
```

**Incluye:**
- Estado general (Verde/Amarillo/Rojo)
- Hitos completados
- Hitos en progreso
- Riesgos identificados
- Próximos pasos

#### b) Actualizar Registro de Riesgos
```
@copilot Actualizar RISK_REGISTER.xlsx con:
- Risk ID, Descripción, Probabilidad, Impacto, Mitigación
```

#### c) Generar Resumen Ejecutivo
```
@copilot Crear EXECUTIVE_SUMMARY.pptx para la semana del [fecha]
```

**Incluye:**
- Portada
- Estado general
- Logros principales
- Riesgos top-3
- Próximos pasos

---

## Ejemplos Completos

### Ejemplo 1: Documentar un módulo C++

```bash
# Solicitud
@copilot generar-docs-codigo src/

# Esto generará:
# 1. Docstrings Doxygen en .cpp/.h
# 2. API_REFERENCE.md con lista de funciones
# 3. Comentarios explicativos donde sea necesario
```

### Ejemplo 2: Crear ADR sobre selección de tecnología

```bash
@copilot Crear un ADR sobre por qué elegimos WPF para la interfaz
en lugar de web frameworks. Incluir:
- Contexto: requisitos de performance, UI responsiva
- Decisión: WPF con C#/.NET
- Consecuencias: mejor control de UI, pero limitado a Windows
- Alternativas: Electron, Qt, Web+Electron
```

### Ejemplo 3: Generar reporte semanal automático

```bash
@copilot Generar reporte de estado para la semana del 18-22 febrero:
- Commits: 23 commits fusionados
- Issues cerrados: 8 bugs, 3 features
- Riesgos nuevos: Retrasó en integración de pruebas
- Estado: Amarillo (en horario, pero riesgos emergentes)
```

---

## Estructura de Salida

### Documentación de Código

```
docs/
├── API_REFERENCE.md
├── src/
│   ├── FileProcessor.cpp (con Doxygen comments)
│   ├── FileProcessor.h (con Doxygen comments)
│   └── ...
```

### Documentación de Arquitectura

```
docs/Decisiones_arquitectura/
├── INDEX.md (tabla de ADRs)
├── ADR_0001_Arquitectura_WPF_CPP.md
├── ADR_0002_Comunicacion_Named_Pipes.md
└── ADR_0003_*.md
```

### Reportes Administrativos

```
docs/project_management/
├── WEEKLY_STATUS_2026-02-18.docx
├── RISK_REGISTER.xlsx
└── EXECUTIVE_SUMMARY_2026-02-18.pptx
```

---

## Restricciones Importantes

❌ **Nunca documentar:**
- Credenciales, API keys, secrets
- Información confidencial del cliente
- Rutas absolutas de usuarios

✅ **Siempre incluir:**
- Timestamp de generación
- Versión de la documentación
- Referencias cruzadas
- Autor (cuando sea relevante)

---

## Integración con Git

Los documentos generados se deben hacer commit regularmente:

```bash
git add docs/
git commit -m "docs: actualizar documentación automática

- API_REFERENCE.md actualizado
- WEEKLY_STATUS generado
- Nuevos ADRs documentados

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>"
```

---

## Tips y Trucos

1. **Revisar antes de publicar**: Los reportes deben ser revisados por el PM
2. **Mantener versionado**: Todo en git, fácil de rastrear cambios
3. **Usar templates**: Los ADRs y reportes tienen formatos estandarizados
4. **Automatizar cronograma**: 
   - Reportes: Viernes 3 PM
   - Risk Register: Viernes 10 AM
   - Documentación de código: Post-PR

---

## Soporte y Extensiones

Para agregar nuevas convenciones o formatos:
1. Editar `.agents/skills/auto-documentacion/SKILL.md`
2. Actualizar `.agents/agents.md` con nuevas responsabilidades
3. Documentar ejemplos en esta guía

Para invocaciones personalizadas:
```
@documentacion_agent [solicitud personalizada]
```
