---
name: ordenar-miembros-clase
description: Reordena los miembros de una clase C# según una normativa clara basada en StyleCop SA1201 y SA1202, sin cambiar su comportamiento.
---

# Ordenación de miembros de una clase C#

Usa esta skill cuando el usuario te pida ordenar, organizar o limpiar los miembros de una clase C#.

## Objetivo
Reordenar los miembros de una clase de forma consistente, legible y alineada con las reglas de estilo habituales en C#:
- por tipo de miembro (SA1201)
- por nivel de acceso (SA1202)
- sin cambiar la lógica ni la semántica del código

## Regla de ordenación recomendada

### 1. Orden por tipo de miembro
Dentro de una clase, los miembros deben aparecer en este orden aproximado:
1. Campos
2. Constructores
3. Finalizadores / destructores
4. Delegados
5. Eventos
6. Enums
7. Interfaces
8. Propiedades
9. Indexadores
10. Métodos
11. Structs y clases anidadas (si existen)

### 2. Orden por visibilidad
Dentro de cada grupo, ordenar por acceso de esta forma:
1. public
2. internal
3. protected internal
4. protected
5. private protected
6. private

## Reglas de ejecución
1. No cambies nombres, firmas, comentarios ni comportamiento.
2. Mueve únicamente los miembros para ordenar la clase.
3. Mantén los miembros estáticos y no estáticos en su bloque correspondiente.
4. Si hay comentarios XML, conserva su ubicación y sentido.
5. Si una clase ya está bien ordenada, no la alteres innecesariamente.
6. Si hay una implementación parcial o una interfaz, conserva la estructura existente y no introduzcas cambios funcionales.

## Resultado esperado
- El código queda más legible y consistente.
- Los miembros aparecen agrupados por categoría y visibilidad.
- No se introduce lógica nueva ni se alteran comportamientos.

## Instrucción final para el modelo
Cuando el usuario pida ordenar una clase, analiza la clase, identifica los miembros y devuelve el código reordenado respetando:
- SA1201 (tipo de miembro)
- SA1202 (nivel de acceso)
- la semántica actual del proyecto

## Ejemplo de uso
Si el usuario dice: "ordena los miembros de esta clase según normativa", aplica esta skill y devuelve únicamente el código reordenado, o una versión revisada de la clase, sin alterar su funcionamiento.
