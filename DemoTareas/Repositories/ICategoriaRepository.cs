// Autor:   DemoTareas Team
// Fecha:   2026-06-18
// Versión: 1.0

using DemoTareas.Models;

namespace DemoTareas.Repositories;

public interface ICategoriaRepository
{
    IList<Categoria> GetAll();
    Categoria? GetById(int id);
    Categoria? GetByNombre(string nombre);
    void Add(Categoria categoria);
    void Update(Categoria categoria);
    void Delete(int id);
}
