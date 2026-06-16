using DemoTareas.Models;

namespace DemoTareas.Repositories;

public interface ITareaRepository
{
    IList<Tarea> GetAll();
    Tarea? GetById(int id);
    void Add(Tarea tarea);
    void Update(Tarea tarea);
    void Delete(int id);
}
