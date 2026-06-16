using DemoTareas.Models;
using DemoTareas.Repositories;

namespace DemoTareas.Tests.Fakes;

public class FakeTareaRepository : ITareaRepository
{
    private readonly List<Tarea> _tareas = new();
    private int _nextId = 1;

    public IList<Tarea> GetAll() => _tareas.ToList();

    public Tarea? GetById(int id) => _tareas.FirstOrDefault(t => t.Id == id);

    public void Add(Tarea tarea)
    {
        tarea.Id = _nextId++;
        _tareas.Add(tarea);
    }

    public void Update(Tarea tarea)
    {
        var index = _tareas.FindIndex(t => t.Id == tarea.Id);
        if (index >= 0) _tareas[index] = tarea;
    }

    public void Delete(int id)
    {
        var tarea = _tareas.FirstOrDefault(t => t.Id == id);
        if (tarea is not null) _tareas.Remove(tarea);
    }
}
