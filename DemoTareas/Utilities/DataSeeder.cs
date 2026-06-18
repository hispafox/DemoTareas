// Autor:   DemoTareas Team
// Fecha:   2026-06-18
// Versión: 1.0

using DemoTareas.Data;
using DemoTareas.Models;

namespace DemoTareas.Utilities;

public static class DataSeeder
{
    public static void Seed(TareasDbContext context)
    {
        if (context.Categorias.Any() || context.Tareas.Any())
            return;

        var trabajo = new Categoria { Nombre = "Trabajo" };
        var personal = new Categoria { Nombre = "Personal" };
        var hogar = new Categoria { Nombre = "Hogar" };

        context.Categorias.AddRange(trabajo, personal, hogar);
        context.SaveChanges();

        var hoy = DateOnly.FromDateTime(DateTime.Today);

        context.Tareas.AddRange(
            new Tarea
            {
                Titulo = "Preparar informe mensual",
                Descripcion = "Consolidar métricas del mes y enviar al equipo.",
                Estado = EstadoTarea.Pendiente,
                Prioridad = Prioridad.Alta,
                FechaVencimiento = hoy.AddDays(3),
                CategoriaId = trabajo.Id
            },
            new Tarea
            {
                Titulo = "Revisar correos pendientes",
                Descripcion = "Responder correos sin contestar de esta semana.",
                Estado = EstadoTarea.Pendiente,
                Prioridad = Prioridad.Media,
                FechaVencimiento = hoy.AddDays(1),
                CategoriaId = trabajo.Id
            },
            new Tarea
            {
                Titulo = "Reunión de seguimiento",
                Descripcion = "Revisar avances del sprint con el equipo.",
                Estado = EstadoTarea.Completada,
                Prioridad = Prioridad.Alta,
                CategoriaId = trabajo.Id
            },
            new Tarea
            {
                Titulo = "Hacer ejercicio",
                Descripcion = "30 minutos de cardio.",
                Estado = EstadoTarea.Pendiente,
                Prioridad = Prioridad.Media,
                FechaVencimiento = hoy.AddDays(7),
                CategoriaId = personal.Id
            },
            new Tarea
            {
                Titulo = "Leer capítulo del libro",
                Estado = EstadoTarea.Completada,
                Prioridad = Prioridad.Baja,
                CategoriaId = personal.Id
            },
            new Tarea
            {
                Titulo = "Llamar al médico",
                Descripcion = "Pedir cita para revisión anual.",
                Estado = EstadoTarea.Pendiente,
                Prioridad = Prioridad.Alta,
                FechaVencimiento = hoy.AddDays(5),
                CategoriaId = personal.Id
            },
            new Tarea
            {
                Titulo = "Comprar víveres",
                Descripcion = "Leche, pan, frutas y verduras.",
                Estado = EstadoTarea.Pendiente,
                Prioridad = Prioridad.Media,
                FechaVencimiento = hoy.AddDays(2),
                CategoriaId = hogar.Id
            },
            new Tarea
            {
                Titulo = "Limpiar el garaje",
                Estado = EstadoTarea.Pendiente,
                Prioridad = Prioridad.Baja,
                CategoriaId = hogar.Id
            },
            new Tarea
            {
                Titulo = "Pagar factura de la luz",
                Estado = EstadoTarea.Completada,
                Prioridad = Prioridad.Alta,
                CategoriaId = hogar.Id
            },
            new Tarea
            {
                Titulo = "Tarea sin categoría de ejemplo",
                Descripcion = "Esta tarea no tiene categoría asignada.",
                Estado = EstadoTarea.Pendiente,
                Prioridad = Prioridad.Baja
            }
        );

        context.SaveChanges();
    }
}
