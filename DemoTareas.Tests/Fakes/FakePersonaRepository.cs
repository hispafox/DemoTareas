using DemoTareas.Models;
using DemoTareas.Repositories;

namespace DemoTareas.Tests.Fakes;

public class FakePersonaRepository : IPersonaRepository
{
    private readonly List<Persona> _personas = new();
    private int _nextId = 1;

    public IList<Persona> GetAll() => _personas.ToList();

    public Persona? GetById(int id) => _personas.FirstOrDefault(p => p.Id == id);

    public Persona? GetByNombreCompleto(string nombre, string apellido) =>
        _personas.FirstOrDefault(p => p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)
            && p.Apellido.Equals(apellido, StringComparison.OrdinalIgnoreCase));

    public void Add(Persona persona)
    {
        persona.Id = _nextId++;
        _personas.Add(persona);
    }

    public void Update(Persona persona)
    {
        var index = _personas.FindIndex(p => p.Id == persona.Id);
        if (index >= 0) _personas[index] = persona;
    }

    public void Delete(int id)
    {
        var persona = _personas.FirstOrDefault(p => p.Id == id);
        if (persona is not null) _personas.Remove(persona);
    }
}
