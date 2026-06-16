using DemoTareas.Models;

namespace DemoTareas.Services;

public interface ITareaService
{
    IList<Tarea> GetAll();
    IList<Tarea> GetFiltered(EstadoTarea? estado = null, int? categoriaId = null, Prioridad? prioridad = null);
    void Add(Tarea tarea);
    void Update(Tarea tarea);
    void Delete(int id);
    void ToggleComplete(int id);
}
