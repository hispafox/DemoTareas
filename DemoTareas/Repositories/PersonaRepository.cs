using DemoTareas.Data;
using DemoTareas.Models;

namespace DemoTareas.Repositories;

public class PersonaRepository : IPersonaRepository
{
    private readonly TareasDbContext _context;

    public PersonaRepository(TareasDbContext context)
    {
        _context = context;
    }

    public IList<Persona> GetAll() => _context.Personas.OrderBy(p => p.Apellido).ThenBy(p => p.Nombre).ToList();

    public Persona? GetById(int id) => _context.Personas.Find(id);

    public Persona? GetByNombreCompleto(string nombre, string apellido) =>
        _context.Personas.FirstOrDefault(p => p.Nombre.ToLower() == nombre.ToLower().Trim()
            && p.Apellido.ToLower() == apellido.ToLower().Trim());

    public void Add(Persona persona)
    {
        _context.Personas.Add(persona);
        _context.SaveChanges();
    }

    public void Update(Persona persona)
    {
        _context.Personas.Update(persona);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var persona = _context.Personas.Find(id);
        if (persona is not null)
        {
            _context.Personas.Remove(persona);
            _context.SaveChanges();
        }
    }
}
