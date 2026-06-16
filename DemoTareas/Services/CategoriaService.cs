using DemoTareas.Models;
using DemoTareas.Repositories;

namespace DemoTareas.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _repository;

    public CategoriaService(ICategoriaRepository repository)
    {
        _repository = repository;
    }

    public IList<Categoria> GetAll() => _repository.GetAll();

    public void Add(string nombre)
    {
        ValidarNombre(nombre);

        if (_repository.GetByNombre(nombre) is not null)
            throw new InvalidOperationException($"Ya existe una categoría con el nombre '{nombre}'.");

        _repository.Add(new Categoria { Nombre = nombre.Trim() });
    }

    public void Update(int id, string nuevoNombre)
    {
        ValidarNombre(nuevoNombre);

        var existente = _repository.GetByNombre(nuevoNombre);
        if (existente is not null && existente.Id != id)
            throw new InvalidOperationException($"Ya existe una categoría con el nombre '{nuevoNombre}'.");

        var categoria = _repository.GetById(id)
            ?? throw new InvalidOperationException($"No se encontró la categoría con Id {id}.");

        categoria.Nombre = nuevoNombre.Trim();
        _repository.Update(categoria);
    }

    public void Delete(int id) => _repository.Delete(id);

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la categoría no puede estar vacío.");
    }
}
