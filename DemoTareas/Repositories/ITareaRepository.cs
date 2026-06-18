// Autor:   DemoTareas Team
// Fecha:   2026-06-18
// Versión: 1.0

using DemoTareas.Models;

namespace DemoTareas.Repositories;

public interface ITareaRepository
{
    IList<Tarea> GetAll();
    IList<Tarea> GetFiltered(EstadoTarea? estado, int? categoriaId, Prioridad? prioridad);
    Tarea? GetById(int id);
    void Add(Tarea tarea);
    void Update(Tarea tarea);
    void Delete(int id);
}
