# Manual de Captura de Fotografías — OCR Operativo

**Proyecto:** Agente OCR Operativo para Digitalización de Registros
**Autor:** IDC Ingeniería
**Fecha:** Marzo 2026

---

## 1. Propósito

Este manual define los criterios mínimos de calidad que debe cumplir una fotografía del formato de fallas centrifugas para que el sistema OCR pueda extraer los datos correctamente.

Una fotografía de mala calidad puede generar registros con `Estado_OCR = Requiere Revision`, aumentando el trabajo manual de corrección.

---

## 2. Criterios de calidad de imagen

### 2.1 Encuadre

| Criterio | Correcto | Incorrecto |
|---------|---------|-----------|
| Formato completo visible | Todo el formato entra en la foto | Se corta el encabezado o las últimas filas |
| Sin elementos ajenos | Solo el formato en la foto | Manos, objetos sobre el papel |
| Orientación | Horizontal o vertical alineado | Formato torcido más de 15° |

### 2.2 Iluminación

| Criterio | Correcto | Incorrecto |
|---------|---------|-----------|
| Iluminación uniforme | Toda la hoja iluminada por igual | Sombras sobre campos o grilla |
| Sin sobreexposición | Campos legibles | Zonas blancas/quemadas por flash |
| Sin reflejos | Papel mate visible | Reflejo del flash sobre papel brillante |

### 2.3 Enfoque y nitidez

| Criterio | Correcto | Incorrecto |
|---------|---------|-----------|
| Texto nítido | Números y letras claramente legibles | Foto movida o desenfocada |
| Checkboxes legibles | Se distingue si están marcados o vacíos | Casillas borrosas o muy pequeñas |

---

## 3. Pasos para tomar una buena foto

1. **Colocar el formato** sobre una superficie plana y sin arrugas
2. **Asegurarse de tener buena iluminación** — luz natural difusa o luz artificial uniforme sin flash directo
3. **Posicionar el móvil** directamente encima del formato, perpendicular al papel (no inclinado)
4. **Encuadrar el formato completo** — verificar que encabezado y todas las filas estén en el cuadro
5. **Esperar que el autofocus confirme enfoque** antes de tomar la foto
6. **Verificar la foto** antes de enviarla — zoom sobre los campos para confirmar legibilidad

---

## 4. Qué hacer si la foto no quedó bien

- Tomar una nueva foto siguiendo los criterios anteriores
- Enviar la nueva foto al bot (puede enviar varias fotos; cada una se procesará por separado)
- Si el registro ya se guardó con `Estado_OCR = Requiere Revision`, corregir directamente en Google Sheets

---

## 5. Formatos soportados

Actualmente solo se soporta el **formato de Fallas Centrifugas** (código `doc20943920260126103509`).

Para solicitar soporte de un nuevo formato, contactar a IDC Ingeniería con una muestra del formato y descripción de los campos.
