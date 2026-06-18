// Autor:   DemoTareas Team
// Fecha:   2026-06-18
// Versión: 1.0

using DemoTareas.Data;
using DemoTareas.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoTareas.Repositories;

public class TareaRepository : ITareaRepository
{
    private readonly TareasDbContext _context;

    public TareaRepository(TareasDbContext context)
    {
        _context = context;
    }

    public IList<Tarea> GetAll()
    {
        return _context.Tareas
            .Include(t => t.Categoria)
            .Include(t => t.Persona)
            .OrderByDescending(t => t.FechaCreacion)
            .ToList();
    }

    public IList<Tarea> GetFiltered(EstadoTarea? estado, int? categoriaId, Prioridad? prioridad)
    {
        IQueryable<Tarea> query = _context.Tareas.Include(t => t.Categoria).Include(t => t.Persona);

        if (estado.HasValue)
            query = query.Where(t => t.Estado == estado.Value);

        if (categoriaId.HasValue)
            query = query.Where(t => t.CategoriaId == categoriaId.Value);

        if (prioridad.HasValue)
            query = query.Where(t => t.Prioridad == prioridad.Value);

        return query.OrderByDescending(t => t.FechaCreacion).ToList();
    }

    public Tarea? GetById(int id)
    {
        return _context.Tareas
            .Include(t => t.Categoria)
            .Include(t => t.Persona)
            .FirstOrDefault(t => t.Id == id);
    }

    public void Add(Tarea tarea)
    {
        _context.Tareas.Add(tarea);
        _context.SaveChanges();
    }

    public void Update(Tarea tarea)
    {
        _context.Tareas.Update(tarea);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var tarea = _context.Tareas.Find(id);
        if (tarea is not null)
        {
            _context.Tareas.Remove(tarea);
            _context.SaveChanges();
        }
    }
}
