# Material de Capacitación — FifoCleanup v1.0

**Proyecto:** Gestión de Almacenamiento FIFO para Servidor de Monitoreo  
**Cliente:** ODL Instrumentación y Control  
**Autor:** IDC Ingeniería  
**Fecha:** Marzo 2026  
**Duración estimada de la capacitación:** 2–3 horas

---

## Agenda de Capacitación

| # | Tema | Duración | Tipo |
|---|------|----------|------|
| 1 | [Introducción: ¿Qué es FifoCleanup?](#1-introducción-qué-es-fifocleanup) | 10 min | Presentación |
| 2 | [Conceptos clave](#2-conceptos-clave) | 15 min | Presentación |
| 3 | [Recorrido por la interfaz (6 pestañas)](#3-recorrido-por-la-interfaz) | 20 min | Demo en vivo |
| 4 | [Operación diaria](#4-operación-diaria) | 15 min | Demo + práctica |
| 5 | [Simulación: probar sin riesgo](#5-simulación-probar-sin-riesgo) | 15 min | Práctica guiada |
| 6 | [Ejecución de limpieza: manual y automática](#6-ejecución-de-limpieza) | 20 min | Demo en vivo |
| 7 | [Bitácora y auditoría](#7-bitácora-y-auditoría) | 10 min | Demo |
| 8 | [Configuración: qué cambiar y cuándo](#8-configuración-qué-cambiar-y-cuándo) | 15 min | Presentación |
| 9 | [Troubleshooting básico](#9-troubleshooting-básico) | 15 min | Presentación |
| 10 | [Ejercicios prácticos](#10-ejercicios-prácticos) | 20 min | Práctica |
| 11 | [Preguntas y respuestas](#11-preguntas-y-respuestas) | 25 min | Q&A |

**Total:** ~3 horas

---

## 1. Introducción: ¿Qué es FifoCleanup?

### El problema

El servidor de monitoreo `SRVODLRTDNMICA` almacena datos de vibración y condición de equipos industriales. Estos datos crecen continuamente y, si el disco se llena, el software de monitoreo deja de funcionar.

### La solución

FifoCleanup elimina automáticamente los **datos más antiguos** cuando el disco se acerca a su límite. Siempre preserva los datos más recientes.

### Analogía

Piense en un estante con espacio limitado. Cuando llega un libro nuevo y el estante está lleno, se retira el libro más antiguo para hacer espacio. Eso es FIFO: **First In, First Out** (el primero en entrar es el primero en salir).

### ¿Qué NO hace FifoCleanup?

- No elimina el software de monitoreo ni sus configuraciones
- No toca archivos del sistema operativo
- No envía datos a internet
- No requiere conexión a internet para funcionar

---

## 2. Conceptos clave

### Estructura de datos

```
D:\MonitoringData\
├── Asset001\          ← Un equipo monitoreado (bomba, motor...)
│   ├── 00\            ← Variable de medición
│   │   ├── E\         ← Datos de Eventos
│   │   │   └── 2026\01\15\  ← Un día de datos
│   │   └── F\         ← Datos de Frecuencia
│   │       └── 2026\01\15\
│   └── 01\
└── Asset002\
```

### Semáforo: interpretación rápida

| Color | ¿Qué significa? | ¿Qué hago? |
|-------|-----------------|------------|
| 🟢 Verde (<70%) | Todo bien | Nada, revisar al día siguiente |
| 🟡 Amarillo (70-85%) | Atención | Verificar que los servicios automáticos estén activos |
| 🔴 Rojo (>85%) | Acción necesaria | Verificar RF-07/RF-08 o ejecutar limpieza manual |

### Dos servicios automáticos

| Servicio | ¿Cuándo actúa? | ¿Qué hace? |
|----------|----------------|-------------|
| **RF-07 (Programada)** | Cada 6 horas | Revisa TODO el disco. Si se va a llenar, limpia los datos más antiguos de TODOS los equipos |
| **RF-08 (Preventiva)** | En tiempo real | Si detecta que un equipo está generando muchos datos, limpia solo ese equipo |

---

## 3. Recorrido por la interfaz

### Las 6 pestañas

**Pestaña 1: Dashboard**
- Vista general del disco: uso actual, semáforo, gráficas
- Lista de equipos (Assets) con sus tamaños
- Botón **Escanear**: actualizar la información

**Pestaña 2: Configuración**
- Dónde están los datos (ruta)
- Cuándo activar la limpieza (umbral)
- Cuánto limpiar (cap)
- Parámetros de RF-07 y RF-08

**Pestaña 3: Simulación**
- Probar la limpieza sin borrar nada real
- Genera datos de prueba temporales

**Pestaña 4: Ejecución**
- Botón de limpieza manual
- Activar/desactivar RF-07 y RF-08
- Ver estado de los servicios automáticos

**Pestaña 5: Bitácora**
- Historial de todo lo que ha hecho el sistema
- Filtros por fecha y tipo
- Exportar a CSV para informes

---

## 4. Operación diaria

### Rutina de 5 minutos

1. **Abrir FifoCleanup** (si no está abierto)
2. **Mirar el Dashboard:**
   - ¿De qué color es el semáforo? → Si es 🟢, todo bien
   - ¿Cuánto espacio queda?
3. **Ir a Ejecución:**
   - ¿RF-07 dice "Activo"? → Bien
   - ¿RF-08 dice "Activo"? → Bien
4. **Si todo está verde y activo:** Listo, puede cerrar o minimizar
5. **Si algo está rojo o desactivado:** Ver sección de troubleshooting

### ¿Cada cuánto debo revisar?

| Situación | Frecuencia |
|-----------|-----------|
| Todo normal (🟢) | 1 vez al día, inicio de turno |
| Semáforo amarillo (🟡) | 2 veces al día |
| Semáforo rojo (🔴) | Cada hora hasta que baje |

---

## 5. Simulación: probar sin riesgo

### Ejercicio guiado

1. Ir a pestaña **Simulación**
2. Configurar:
   - Assets: 3
   - Variables por Asset: 2
   - Días de historia: 30
   - Tamaño por carpeta: 50 MB
   - Disco simulado: 100 GB
   - Uso inicial: 80%
3. Clic en **Ejecutar simulación**
4. Esperar las 3 fases
5. Leer el resultado:
   - ¿Cuántas carpetas se eliminarían?
   - ¿Cuánto espacio se liberaría?
   - ¿El uso bajaría del umbral?
6. Clic en **Limpiar datos de prueba** para liberar espacio temporal

### Preguntas de verificación

- ¿La simulación borró datos reales? **No**
- ¿Es seguro ejecutarla varias veces? **Sí**
- ¿Debe ejecutarse antes de cambiar parámetros? **Recomendado**

---

## 6. Ejecución de limpieza

### Limpieza manual (solo si es necesario)

1. Ir a **Ejecución**
2. Clic en **Vista previa** → revisa qué se eliminaría
3. Si está de acuerdo → **Ejecutar limpieza**
4. **Confirmar** en el diálogo
5. Esperar a que termine (barra de progreso)
6. Verificar resultado en Dashboard

### Activar limpieza automática

1. Ir a **Ejecución**
2. Clic en **Activar RF-07** → queda encendido permanentemente
3. Clic en **Activar RF-08** → queda encendido permanentemente
4. **¡Listo!** Los servicios trabajan solos

### ¿Puedo cancelar una limpieza en progreso?

**Sí.** Clic en **Cancelar**. Se detendrá de forma segura. Lo que ya se borró no se recupera, pero no queda nada a medias.

---

## 7. Bitácora y auditoría

### ¿Para qué sirve?

- Saber **qué pasó, cuándo y por qué**
- Verificar que las limpiezas automáticas están funcionando
- Generar informes para auditorías

### Cómo consultarla

1. Ir a pestaña **Bitácora**
2. Aplicar filtros (opcional):
   - Rango de fechas
   - Tipo de evento (limpieza, error, etc.)
3. Revisar las entradas

### Exportar para informes

1. Pestaña **Bitácora** → **Exportar a CSV**
2. Se genera un archivo `.csv` abierto con Excel

---

## 8. Configuración: qué cambiar y cuándo

### Solo cambiar si...

| Parámetro | Cambiar si... |
|-----------|--------------|
| Umbral (85%) | La limpieza se activa muy frecuentemente (subir) o muy tarde (bajar) |
| Cap (20%) | La limpieza no libera suficiente espacio por ejecución (subir) |
| Frecuencia RF-07 (6h) | Se necesita evaluación más/menos frecuente |
| Umbral preventivo RF-08 (3 días) | Se necesita más/menos anticipación |

### Regla de oro

> **Después de cambiar cualquier parámetro, ejecute una simulación para verificar el efecto.**

---

## 9. Troubleshooting básico

### Los 5 problemas más comunes

| # | Problema | Solución rápida |
|---|---------|----------------|
| 1 | Semáforo rojo y no limpia | Verificar RF-07/RF-08 activos. Si no, activarlos. |
| 2 | Error "Ruta no existe" | Verificar Configuración → la ruta debe existir y ser accesible |
| 3 | RF-07 siempre dice "SKIP" | El disco no está lo suficientemente lleno, o el umbral es muy alto |
| 4 | "0 Assets detectados" | Verificar que la ruta apunta a donde están las carpetas de equipos |
| 5 | App no abre | Verificar que .NET 8.0 está instalado |

### Si no puede resolverlo

1. Anotar el problema y el mensaje de error
2. Exportar la bitácora (si puede)
3. Contactar a IDC: ver [Contactos de Soporte](Operaciones/CONTACTOS_SLA.md)

---

## 10. Ejercicios prácticos

### Ejercicio 1: Verificación de Dashboard (2 min)

1. Abrir FifoCleanup
2. Ir a Dashboard → Escanear
3. Responder:
   - ¿Cuántos Assets hay?
   - ¿De qué color es el semáforo?
   - ¿Cuántos días faltan para que se llene?

### Ejercicio 2: Ejecutar una simulación (5 min)

1. Ir a Simulación
2. Configurar con valores moderados (3 Assets, 2 Variables, 20 días)
3. Ejecutar simulación
4. Responder:
   - ¿Cuánto espacio se liberaría?
   - ¿Cuántas carpetas se eliminarían?

### Ejercicio 3: Activar servicios automáticos (2 min)

1. Ir a Ejecución
2. Activar RF-07
3. Activar RF-08
4. Verificar que ambos muestran estado "Activo"

### Ejercicio 4: Consultar bitácora (3 min)

1. Ir a Bitácora
2. Buscar la entrada de inicio de RF-07 (SystemStart)
3. Buscar la entrada de inicio de RF-08 (SystemStart)
4. Exportar las últimas 24 horas a CSV

### Ejercicio 5: Cambiar un parámetro (3 min)

1. Ir a Configuración
2. Cambiar el umbral de 85% a 80%
3. Guardar
4. Ejecutar una simulación para ver el efecto
5. **Restaurar** el umbral a 85%

---

## 11. Preguntas y respuestas

*Espacio para preguntas libres durante la sesión de capacitación.*

### Evaluación post-capacitación

Por favor responder las siguientes preguntas:

1. ¿Qué significa el semáforo amarillo?
2. ¿Cuál es la diferencia entre RF-07 y RF-08?
3. Si el disco está al 92%, ¿qué harías como primera acción?
4. ¿La simulación elimina datos reales?
5. ¿Dónde se consulta el historial de operaciones del sistema?
6. ¿Qué debe hacer al inicio de cada turno?
7. Si la aplicación se cierra, ¿RF-07 y RF-08 siguen funcionando?
8. ¿Cómo se exporta la bitácora para un informe?

### Respuestas esperadas

1. Uso entre 70-85%, monitorear y verificar que los servicios estén activos
2. RF-07 = periódica/global, RF-08 = tiempo real/local
3. Verificar que RF-07 y RF-08 estén activos; si no, activarlos; si no mejora, limpieza manual
4. No, genera datos temporales separados
5. En la pestaña Bitácora
6. Abrir FifoCleanup → Dashboard → verificar semáforo y servicios activos
7. No, se detienen automáticamente
8. Pestaña Bitácora → botón Exportar a CSV

---

## Materiales de referencia

| Documento | Ubicación | Para qué |
|-----------|-----------|----------|
| Manual de Operación | [docs/Operaciones/MANUAL_DE_OPERACION.md](Operaciones/MANUAL_DE_OPERACION.md) | Operación diaria detallada |
| Runbook | [docs/Operaciones/RUNBOOK.md](Operaciones/RUNBOOK.md) | Procedimientos paso a paso |
| Troubleshooting | [docs/Operaciones/TROUBLESHOOTING.md](Operaciones/TROUBLESHOOTING.md) | Resolver problemas |
| FAQ | [docs/Operaciones/FAQ.md](Operaciones/FAQ.md) | Preguntas frecuentes |
| Guía de Configuración | [docs/Operaciones/GUIA_CONFIGURACION.md](Operaciones/GUIA_CONFIGURACION.md) | Detalle de cada parámetro |
