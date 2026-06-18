// Autor:   DemoTareas Team
// Fecha:   2026-06-18
// Versión: 1.0

using DemoTareas.Models;

namespace DemoTareas.Services;

public interface ICategoriaService
{
    IList<Categoria> GetAll();
    void Add(string nombre);
    void Update(int id, string nuevoNombre);
    void Delete(int id);
}
