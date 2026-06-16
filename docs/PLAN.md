# Plan: Aplicación de tareas Windows Forms .NET 10

## Decisiones fijadas
- Persistencia: SQLite con Entity Framework Core
- Categorías: sí, desde el inicio
- Prioridad (Alta/Media/Baja): sí
- Fecha de vencimiento: sí
- Pruebas unitarias: xUnit (proyecto separado)

---

## Fase 0 – Guardar plan
1. Guardar este plan como `PLAN.md` en `c:\w\DemoTareas\`

## Fase 1 – Scaffolding de la solución
2. Crear solución `DemoTareas.sln` con `dotnet new sln`
3. Crear proyecto principal `DemoTareas` (winforms, net10.0-windows)
4. Crear proyecto de pruebas `DemoTareas.Tests` (xunit, net10.0)
5. Añadir referencias: Tests → DemoTareas

## Fase 2 – Modelos de dominio (Models/)
6. Enum `Prioridad`: Alta, Media, Baja
7. Enum `EstadoTarea`: Pendiente, Completada
8. Clase `Categoria`: Id (int), Nombre (string)
9. Clase `Tarea`: Id (int), Titulo, Descripcion, Estado, Prioridad, FechaCreacion, FechaVencimiento (DateOnly?), CategoriaId (int?), Categoria (nav prop)

## Fase 3 – Acceso a datos (Data/ y Repositories/)
10. Instalar NuGet: `Microsoft.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Tools`
11. Clase `TareasDbContext` con DbSet<Tarea> y DbSet<Categoria>; base de datos en carpeta AppData del usuario
12. Interfaces `ITareaRepository` y `ICategoriaRepository`
13. Implementaciones `TareaRepository` y `CategoriaRepository`; `EnsureCreated` al arrancar

## Fase 4 – Servicios (Services/)
14. Interfaz `ITareaService` con: `GetAll`, `GetFiltered(estado?, categoria?, prioridad?)`, `Add`, `Update`, `Delete`, `ToggleComplete`
15. Implementación `TareaService`; valida Titulo no vacío y FechaVencimiento no en el pasado
16. Interfaz `ICategoriaService` con: `GetAll`, `Add`, `Update`, `Delete`
17. Implementación `CategoriaService`; valida nombre único

## Fase 5 – Formularios (Forms/)
18. `MainForm`: DataGridView con tareas, panel de filtros (combo estado/categoría/prioridad), botones Nuevo, Editar, Completar/Reabrir, Eliminar, Categorías
19. `TareaForm`: modal crear/editar con Titulo, Descripcion, Prioridad (combo), FechaVencimiento (DateTimePicker anulable), Categoria (combo)
20. `CategoriaForm`: modal gestión de categorías (listar, añadir, editar, eliminar)
21. `Program.cs`: composición manual DbContext → Repositories → Services → MainForm

## Fase 6 – Pruebas unitarias (DemoTareas.Tests/)
22. `TareaServiceTests`: título vacío lanza excepción, fecha pasada lanza excepción, ToggleComplete cambia estado, filtros por categoría y prioridad
23. `CategoriaServiceTests`: nombre duplicado lanza excepción, eliminar categoría desvincula tareas
24. Repositorios fake en memoria para aislar los servicios

---

## Archivos clave
- `DemoTareas/Models/Tarea.cs`, `Categoria.cs`, `Prioridad.cs`, `EstadoTarea.cs`
- `DemoTareas/Data/TareasDbContext.cs`
- `DemoTareas/Repositories/ITareaRepository.cs`, `TareaRepository.cs`, `ICategoriaRepository.cs`, `CategoriaRepository.cs`
- `DemoTareas/Services/ITareaService.cs`, `TareaService.cs`, `ICategoriaService.cs`, `CategoriaService.cs`
- `DemoTareas/Forms/MainForm.cs`, `TareaForm.cs`, `CategoriaForm.cs`
- `DemoTareas/Program.cs`
- `DemoTareas.Tests/TareaServiceTests.cs`, `CategoriaServiceTests.cs`

## Verificación
1. `dotnet build DemoTareas.sln` → sin errores
2. `dotnet test DemoTareas.Tests` → todos los tests en verde
3. Ejecutar la app: crear categorías, crear/editar/completar/eliminar tareas, aplicar filtros, cerrar y reabrir → datos persisten

## Fuera de alcance
- Migraciones EF Core explícitas (se usa EnsureCreated)
- Autenticación o multi-usuario
- Notificaciones o recordatorios de fecha
- UI avanzada (temas, iconos personalizados)
