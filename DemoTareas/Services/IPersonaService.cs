using DemoTareas.Models;

namespace DemoTareas.Services;

public interface IPersonaService
{
    IList<Persona> GetAll();
    void Add(Persona persona);
    void Update(Persona persona);
    void Delete(int id);
}
