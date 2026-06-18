---
name: mantenimiento-tareas
description: Reutiliza el patrón ya aplicado para mantener tareas y aplica el mismo flujo a cualquier otro mantenimiento del proyecto.
---

# Skill: mantenimiento de tareas

## Objetivo
Usa este skill cuando quieras implementar cualquier otro mantenimiento del proyecto siguiendo el mismo patrón que ya se usa para gestionar tareas.

## Patrón base que debe reutilizarse
La funcionalidad de tareas ya está organizada en capas claras:

1. Modelo de dominio
2. Repositorio de acceso a datos
3. Servicio de negocio con validaciones
4. Formulario de UI para crear/editar
5. Integración en el formulario principal
6. Pruebas unitarias

## Flujo recomendado para cualquier mantenimiento

### 1. Revisar el modelo base
Antes de tocar código, identifica:
- qué entidad se quiere mantener,
- qué propiedades son obligatorias,
- qué reglas de negocio existen,
- si hay relaciones con otras entidades.

En tareas, el modelo base está en:
- DemoTareas/Models/Tarea.cs
- DemoTareas/Services/TareaService.cs
- DemoTareas/Forms/TareaForm.cs
- DemoTareas/Forms/MainForm.cs

### 2. Gestionar esquema y migraciones (obligatorio)
Antes de arrancar la aplicación o de confiar en la persistencia, asegúrate de:
- añadir la entidad al `DbContext` con `DbSet<Entidad>`;
- crear una migración EF Core con `dotnet ef migrations add <NombreMigracion>`;
- aplicar la migración con `dotnet ef database update`;
- evitar usar `EnsureCreated()` como única estrategia cuando se añaden nuevas tablas o cambios de esquema.

Regla importante: si una tabla no existe al arrancar la app, la causa suele ser que el esquema no se ha migrado. El arranque debe usar `Database.Migrate()` para que la base de datos quede alineada con el modelo.

Implementa la entidad con el mismo patrón:
- interfaz en Repositories/ITareaRepository.cs
- implementación en Repositories/TareaRepository.cs

Reglas:
- usar EF Core sobre TareasDbContext,
- añadir `DbSet<Entidad>` y, si aplica, restricciones/índices en `OnModelCreating`,
- crear y aplicar migraciones con EF Core antes de ejecutar la app,
- incluir dependencias relacionadas si hace falta,
- exponer operaciones CRUD básicas,
- guardar cambios con SaveChanges().

### 3. Crear la capa de servicio
El servicio debe contener la lógica de negocio y validaciones.

Para tareas, el servicio ya hace esto:
- validar título no vacío,
- validar fechas de vencimiento,
- encontrar la entidad antes de modificar,
- delegar al repositorio la persistencia,
- exponer operaciones como Add, Update, Delete, ToggleComplete.

### 4. Crear el formulario de mantenimiento
El formulario debe:
- ser modal si se trata de crear/editar,
- mostrar los campos necesarios,
- validar entrada con ErrorProvider si aplica,
- devolver el objeto editado con DialogResult.OK,
- no mezclar lógica compleja en el evento Click.

### 5. Integrar la acción en el formulario principal
En MainForm se hace lo siguiente:
- cargar la lista de elementos,
- refrescar filtros y listado,
- abrir el formulario de edición,
- invocar el servicio para guardar o eliminar,
- actualizar la vista después de cada operación.

### 6. Añadir pruebas unitarias
Si se implementa una nueva entidad o mantenimiento, añade pruebas que cubran:
- validación de datos obligatorios,
- reglas de negocio específicas,
- operaciones CRUD básicas,
- comportamiento de eliminación o relación con otras entidades.

## Patrón de ejemplo que debe seguirse
Para tareas, la secuencia real es:

1. Tarea (modelo)
2. ITareaRepository + TareaRepository
3. ITareaService + TareaService
4. TareaForm
5. MainForm
6. pruebas en DemoTareas.Tests

## Reglas de implementación
- Mantén separación de responsabilidades: formulario, servicio, repositorio.
- No pongas lógica de negocio en eventos de UI.
- Usa validaciones claras y mensajes comprensibles.
- Reutiliza el mismo esquema para cualquier otra entidad.
- Si hay reglas como duplicados o dependencias, colócalas en el servicio.
- Siempre valida que el esquema de la base de datos esté actualizado mediante migraciones antes de lanzar la UI.
- Si añades una nueva entidad, recuerda: 1) `DbSet<>` en el `DbContext`, 2) migración EF Core, 3) `Database.Migrate()` al arrancar la app.
- Refresca la vista después de cada operación de mantenimiento.

## Resultado esperado
Cuando uses esta skill, debes poder implementar cualquier otro mantenimiento con el mismo nivel de consistencia que la gestión de tareas:
- crear,
- editar,
- eliminar,
- filtrar,
- validar,
- persistir,
- y mostrar información al usuario.

## Instrucción final para el modelo
Cuando el usuario pida implementar otro mantenimiento, sigue esta secuencia:
1. identificar la entidad y sus reglas,
2. crear repositorio,
3. crear servicio con validaciones,
4. crear formulario de mantenimiento,
5. integrarlo en la pantalla principal,
6. añadir pruebas si aplica.
