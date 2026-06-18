// Autor:   DemoTareas Team
// Fecha:   2026-06-18
// Versión: 1.0

using DemoTareas.Models;
using DemoTareas.Repositories;

namespace DemoTareas.Services;

public class TareaService : ITareaService
{
    private readonly ITareaRepository _repository;

    public TareaService(ITareaRepository repository)
    {
        _repository = repository;
    }

    public IList<Tarea> GetAll() => _repository.GetAll();

    public IList<Tarea> GetFiltered(EstadoTarea? estado = null, int? categoriaId = null, Prioridad? prioridad = null)
    {
        return _repository.GetFiltered(estado, categoriaId, prioridad);
    }

    public void Add(Tarea tarea)
    {
        ValidarTareaBase(tarea);
        ValidarFechaVencimiento(tarea);
        tarea.FechaCreacion = DateTime.UtcNow;
        _repository.Add(tarea);
    }

    public void Update(Tarea tarea)
    {
        ValidarTareaBase(tarea);
        _repository.Update(tarea);
    }

    public void Delete(int id) => _repository.Delete(id);

    public void ToggleComplete(int id)
    {
        var tarea = _repository.GetById(id)
            ?? throw new InvalidOperationException($"No se encontró la tarea con Id {id}.");

        tarea.Estado = tarea.Estado == EstadoTarea.Pendiente
            ? EstadoTarea.Completada
            : EstadoTarea.Pendiente;

        _repository.Update(tarea);
    }

    private static void ValidarTareaBase(Tarea tarea)
    {
        if (string.IsNullOrWhiteSpace(tarea.Titulo))
            throw new ArgumentException("El título de la tarea no puede estar vacío.");
    }

    private static void ValidarFechaVencimiento(Tarea tarea)
    {
        if (tarea.FechaVencimiento.HasValue)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            if (tarea.FechaVencimiento.Value < hoy)
                throw new ArgumentException("La fecha de vencimiento no puede ser anterior a hoy.");
        }
    }
}
