using DemoTareas.Data;
using DemoTareas.Models;

namespace DemoTareas.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly TareasDbContext _context;

    public CategoriaRepository(TareasDbContext context)
    {
        _context = context;
    }

    public IList<Categoria> GetAll()
    {
        return _context.Categorias.OrderBy(c => c.Nombre).ToList();
    }

    public Categoria? GetById(int id)
    {
        return _context.Categorias.Find(id);
    }

    public Categoria? GetByNombre(string nombre)
    {
        return _context.Categorias
            .FirstOrDefault(c => c.Nombre.ToLower() == nombre.ToLower());
    }

    public void Add(Categoria categoria)
    {
        _context.Categorias.Add(categoria);
        _context.SaveChanges();
    }

    public void Update(Categoria categoria)
    {
        _context.Categorias.Update(categoria);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var categoria = _context.Categorias.Find(id);
        if (categoria is not null)
        {
            _context.Categorias.Remove(categoria);
            _context.SaveChanges();
        }
    }
}
