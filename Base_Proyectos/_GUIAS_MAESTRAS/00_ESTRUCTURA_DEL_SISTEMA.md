# 📚 ESTRUCTURA DEL SISTEMA DE DOCUMENTACIÓN

**Este archivo explica cómo está organizado TODO**

---

## 🏗️ ARQUITECTURA

```
Base_Proyectos/
│
├─ 📄 README.md                         (Visión general)
├─ 📄 00_COMIENZA_AQUI.md              (Entrada - léelo PRIMERO)
│
├─ 📁 01_Aspen_Mtell_ODL/              7 PROYECTOS
├─ 📁 02_Agentes_Accionables_BPC/      (con estructura
├─ 📁 03_Almacenamiento_FIFO/           idéntica cada uno)
├─ 📁 04_OCR_Operativo/
├─ 📁 05_Vibracion_Desfibradora/
├─ 📁 06_Deteccion_Crudo/
├─ 📁 07_Optimizacion_Energetica/
│
├─ 📂 _GUIAS_MAESTRAS/                 ← USAS DIARIAMENTE
│   ├─ 00_ESTRUCTURA_DEL_SISTEMA.md    (este archivo)
│   ├─ RESUMEN_EJECUTIVO_DOCUMENTACION.md       (1 página con TODO)
│   ├─ GUIA_RAPIDA_DOCUMENTACION.md            (semana-a-semana)
│   ├─ CONTENIDO_ESPECIFICO_POR_DOCUMENTO.md   (30+ templates)
│   ├─ MATRIZ_VISUAL_DOCUMENTACION.md          (visualización ASCII)
│   └─ GUIA_ESTRUCTURA_DOCUMENTACION_PROYECTOS.md  (carpetas PMI)
│
└─ 📂 _INDICES_REFERENCIAS/            ← BÚSQUEDA / HISTÓRICO
    ├─ INDICE_MAESTRO_DOCUMENTACION.md
    ├─ DOCUMENTACION_PORTAFOLIO_COMPLETADA.md
    ├─ README_PORTAFOLIO_2026.md
    ├─ DOCUMENTACION_EXPANSION_SUMMARY.md
    └─ DOCUMENTACION_PROYECTOS_3_7_COMPLETA.md
```

---

## 📋 QUÉ CONTIENE CADA CARPETA

### 📂 `_GUIAS_MAESTRAS/` (Lo que HACES)

**RESUMEN_EJECUTIVO_DOCUMENTACION.md**
- ¿Qué?: 1 página con TODO el sistema
- ¿Para qué?: Entender rápidamente qué documentar
- ¿Cuándo?: Primera lectura, luego referencia
- ¿Tamaño?: 10 KB
- 📌 **Este es tu documento más importante**

**GUIA_RAPIDA_DOCUMENTACION.md**
- ¿Qué?: Cronograma semana-a-semana (W1-W9)
- ¿Para qué?: Saber qué hacer cada semana
- ¿Cuándo?: Planificación de sprints
- ¿Tamaño?: 13 KB

**CONTENIDO_ESPECIFICO_POR_DOCUMENTO.md**
- ¿Qué?: Templates exactos para 30+ documentos
- ¿Para qué?: Copiar cuando crees cada documento
- ¿Cuándo?: Cuando necesitas crear un documento específico
- ¿Tamaño?: 36 KB (el más grande, pero muy útil)
- 📌 **Mantén este abierto mientras trabajas**

**MATRIZ_VISUAL_DOCUMENTACION.md**
- ¿Qué?: Visualización ASCII de todas las carpetas
- ¿Para qué?: Ver estructura visual completa
- ¿Cuándo?: Cuando necesitas ubicación espacial
- ¿Tamaño?: 46 KB

**GUIA_ESTRUCTURA_DOCUMENTACION_PROYECTOS.md**
- ¿Qué?: Estructura PMI estándar de carpetas
- ¿Para qué?: Entender las 8 carpetas que creas en cada proyecto
- ¿Cuándo?: Cuando configures proyecto nuevo
- ¿Tamaño?: 16 KB

**00_ESTRUCTURA_DEL_SISTEMA.md** (este archivo)
- ¿Qué?: Explicación de cómo está todo organizado
- ¿Para qué?: Navegar el sistema
- ¿Cuándo?: Cuando necesites entender la arquitectura

---

### 📂 `_INDICES_REFERENCIAS/` (Búsqueda & Histórico)

**INDICE_MAESTRO_DOCUMENTACION.md**
- ¿Qué?: Índice completo y navegable
- ¿Para qué?: Encontrar cualquier cosa
- ¿Cuándo?: Búsqueda específica

**DOCUMENTACION_PORTAFOLIO_COMPLETADA.md**
- ¿Qué?: Estado + checklist de completitud
- ¿Para qué?: Verificar que todo está listo
- ¿Cuándo?: Cierre de proyecto

**README_PORTAFOLIO_2026.md**
- ¿Qué?: Overview de los 7 proyectos
- ¿Para qué?: Visión rápida del portafolio
- ¿Cuándo?: Referencia ejecutiva

**DOCUMENTACION_EXPANSION_SUMMARY.md**
- ¿Qué?: Resumen de ampliaciones realizadas
- ¿Para qué?: Histórico de cambios
- ¿Cuándo?: Referencia histórica

**DOCUMENTACION_PROYECTOS_3_7_COMPLETA.md**
- ¿Qué?: Histórico completo
- ¿Para qué?: Referencia y trazabilidad
- ¿Cuándo?: Auditoría / referencia

---

### 📁 CADA PROYECTO (01-07)

Cada proyecto tiene estructura idéntica:

```
[Proyecto]/
├─ docs/                          (30-40 documentos en 8 carpetas)
│  ├─ project_management/         (PMI formal)
│  ├─ architecture_decisions/     (ADRs técnicos)
│  ├─ requirements/               (Especificaciones)
│  ├─ testing/                    (Planes, casos, resultados)
│  ├─ operations/                 (Runbooks, playbooks)
│  ├─ compliance/                 (Security, audit)
│  ├─ stakeholder_comms/         (Reports, presentations)
│  └─ archive/                    (PDFs finales)
│
├─ src/                           (Código fuente)
├─ tests/                         (Tests unitarios)
└─ README.md                      (Overview proyecto)
```

---

## 🎯 FLUJO DE USO

### CUANDO INICIAS UN PROYECTO (Semana 1)

```
1. Abre: README.md (orientación)
2. Abre: 00_COMIENZA_AQUI.md (entrada)
3. Lee: _GUIAS_MAESTRAS/RESUMEN_EJECUTIVO (15 min)
4. Crea: Estructura /docs/ en proyecto (30 min)
5. Descarga: Templates de _GUIAS_MAESTRAS/CONTENIDO_ESPECIFICO
6. Guardas: En shared folder (Teams/OneDrive)
7. Creas: PROJECT_CHARTER.pdf (1-2 días)
```

### DURANTE EL PROYECTO (Semanas 2-9)

```
1. Consulta: _GUIAS_MAESTRAS/GUIA_RAPIDA (¿qué hacer esta semana?)
2. Usas: _GUIAS_MAESTRAS/CONTENIDO_ESPECIFICO (templates para documentos)
3. Actualizas: VIERNES 10am RISK_REGISTER.xlsx (30 min)
4. Completas: VIERNES 3pm WEEKLY_STATUS.docx (30 min)
5. Creas: VIERNES 3pm EXECUTIVE_SUMMARY.pptx (30 min)
```

### AL CERRAR PROYECTO (Semana 9+)

```
1. Consulta: _INDICES_REFERENCIAS/DOCUMENTACION_PORTAFOLIO_COMPLETADA
2. Verificas: Todos los 30-40 documentos completados
3. Archivas: PDFs finales en /docs/archive/
4. Cierras: Proyecto en steering
```

---

## 🔍 CÓMO BUSCAR COSAS

### "¿Qué documento necesito?" 
→ Abre: `_GUIAS_MAESTRAS/RESUMEN_EJECUTIVO_DOCUMENTACION.md`  
→ Sección: "DOCUMENTOS ESENCIALES"

### "¿Cómo lleno este documento?"
→ Abre: `_GUIAS_MAESTRAS/CONTENIDO_ESPECIFICO_POR_DOCUMENTO.md`  
→ Busca: Por nombre del documento (Ctrl+F)

### "¿Dónde va este archivo?"
→ Abre: `_GUIAS_MAESTRAS/MATRIZ_VISUAL_DOCUMENTACION.md`  
→ Busca: Por nombre o tipo

### "¿Qué hago esta semana?"
→ Abre: `_GUIAS_MAESTRAS/GUIA_RAPIDA_DOCUMENTACION.md`  
→ Busca: "Semana X"

### "¿Necesito este documento para mi proyecto?"
→ Abre: `_INDICES_REFERENCIAS/INDICE_MAESTRO_DOCUMENTACION.md`  
→ Busca: Por nombre

---

## 📊 TAMAÑOS & CONTENIDOS

| Archivo | KB | Secciones | Uso |
|---------|----|-----------|----|
| RESUMEN_EJECUTIVO | 9 | 10 | Primero |
| GUIA_RAPIDA | 13 | 7 | Semanal |
| CONTENIDO_ESPECIFICO | 36 | 8 (43 subsecciones) | Referencias |
| MATRIZ_VISUAL | 46 | 14 | Visual |
| GUIA_ESTRUCTURA | 16 | 6 | Setupup |
| INDICE_MAESTRO | 11 | 10 | Búsqueda |
| OTROS | 28 | - | Histórico |

**Total**: ~175 KB de guías maestras para 230-300 documentos de proyectos

---

## ✅ CHECKLIST: "¿Estoy listo?"

- [ ] He leído README.md (visión general)
- [ ] He leído 00_COMIENZA_AQUI.md (entrada)
- [ ] He leído _GUIAS_MAESTRAS/RESUMEN_EJECUTIVO (sistema completo)
- [ ] Entiendo las 2 carpetas (_GUIAS_MAESTRAS vs _INDICES_REFERENCIAS)
- [ ] Sé dónde está cada guía
- [ ] He descargado templates de CONTENIDO_ESPECIFICO
- [ ] Tengo calendar: Viernes 10am + 3pm (TODOS LOS VIERNES)
- [ ] He creado /docs/ en proyectos

---

## 💡 CLAVES

✅ **_GUIAS_MAESTRAS/** = Lo que HACES (5 archivos esenciales)  
✅ **_INDICES_REFERENCIAS/** = Dónde BUSCAS (5 archivos de referencia)  
✅ **Proyectos 01-07/** = Dónde GUARDAS tus documentos (cada uno /docs/)  
✅ **Raíz** = Lo que VES primero (README + 00_COMIENZA_AQUI)

---

## 🚀 PRÓXIMO PASO

1. Si es tu primera vez: Abre `00_COMIENZA_AQUI.md`
2. Si necesitas referencia: Abre `RESUMEN_EJECUTIVO_DOCUMENTACION.md`
3. Si necesitas buscar: Abre `_INDICES_REFERENCIAS/INDICE_MAESTRO_DOCUMENTACION.md`
4. Si necesitas template: Abre `CONTENIDO_ESPECIFICO_POR_DOCUMENTO.md`

---

**Documento**: _GUIAS_MAESTRAS/00_ESTRUCTURA_DEL_SISTEMA.md  
**Propósito**: Explicar cómo está todo organizado  
**Status**: ✅ Referencia permanente

**Creado**: 11 de febrero, 2026  
**Organización**: Clara y escalable a 7 proyectos
