using DemoTareas.Models;
using DemoTareas.Repositories;

namespace DemoTareas.Tests.Fakes;

public class FakeCategoriaRepository : ICategoriaRepository
{
    private readonly List<Categoria> _categorias = new();
    private int _nextId = 1;

    public IList<Categoria> GetAll() => _categorias.ToList();

    public Categoria? GetById(int id) => _categorias.FirstOrDefault(c => c.Id == id);

    public Categoria? GetByNombre(string nombre) =>
        _categorias.FirstOrDefault(c => c.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));

    public void Add(Categoria categoria)
    {
        categoria.Id = _nextId++;
        _categorias.Add(categoria);
    }

    public void Update(Categoria categoria)
    {
        var index = _categorias.FindIndex(c => c.Id == categoria.Id);
        if (index >= 0) _categorias[index] = categoria;
    }

    public void Delete(int id)
    {
        var cat = _categorias.FirstOrDefault(c => c.Id == id);
        if (cat is not null) _categorias.Remove(cat);
    }
}
