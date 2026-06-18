using DemoTareas.Models;

namespace DemoTareas.Repositories;

public interface IPersonaRepository
{
    IList<Persona> GetAll();
    Persona? GetById(int id);
    Persona? GetByNombreCompleto(string nombre, string apellido);
    void Add(Persona persona);
    void Update(Persona persona);
    void Delete(int id);
}
