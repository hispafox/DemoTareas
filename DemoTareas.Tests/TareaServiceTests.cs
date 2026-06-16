using DemoTareas.Models;
using DemoTareas.Services;
using DemoTareas.Tests.Fakes;

namespace DemoTareas.Tests;

public class TareaServiceTests
{
    private static TareaService CrearServicio() =>
        new TareaService(new FakeTareaRepository());

    [Fact]
    public void Add_TituloVacio_ThrowsArgumentException()
    {
        var service = CrearServicio();
        var tarea = new Tarea { Titulo = "   " };

        Assert.Throws<ArgumentException>(() => service.Add(tarea));
    }

    [Fact]
    public void Add_FechaVencimientoPasada_ThrowsArgumentException()
    {
        var service = CrearServicio();
        var tarea = new Tarea
        {
            Titulo = "Test",
            FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(-1))
        };

        Assert.Throws<ArgumentException>(() => service.Add(tarea));
    }

    [Fact]
    public void Add_TareaValida_SePersiste()
    {
        var service = CrearServicio();
        var tarea = new Tarea { Titulo = "Mi tarea" };

        service.Add(tarea);

        Assert.Single(service.GetAll());
    }

    [Fact]
    public void ToggleComplete_TareaPendiente_CambiaACompletada()
    {
        var repo = new FakeTareaRepository();
        var tarea = new Tarea { Titulo = "Test", Estado = EstadoTarea.Pendiente };
        repo.Add(tarea);
        var service = new TareaService(repo);

        service.ToggleComplete(tarea.Id);

        Assert.Equal(EstadoTarea.Completada, repo.GetById(tarea.Id)!.Estado);
    }

    [Fact]
    public void ToggleComplete_TareaCompletada_CambiaAPendiente()
    {
        var repo = new FakeTareaRepository();
        var tarea = new Tarea { Titulo = "Test", Estado = EstadoTarea.Completada };
        repo.Add(tarea);
        var service = new TareaService(repo);

        service.ToggleComplete(tarea.Id);

        Assert.Equal(EstadoTarea.Pendiente, repo.GetById(tarea.Id)!.Estado);
    }

    [Fact]
    public void GetFiltered_PorCategoria_DevuelveSoloLasDeEsaCategoria()
    {
        var repo = new FakeTareaRepository();
        repo.Add(new Tarea { Titulo = "A", CategoriaId = 1 });
        repo.Add(new Tarea { Titulo = "B", CategoriaId = 2 });
        repo.Add(new Tarea { Titulo = "C", CategoriaId = 1 });
        var service = new TareaService(repo);

        var result = service.GetFiltered(categoriaId: 1);

        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal(1, t.CategoriaId));
    }

    [Fact]
    public void GetFiltered_PorPrioridad_DevuelveSoloLasDeEsaPrioridad()
    {
        var repo = new FakeTareaRepository();
        repo.Add(new Tarea { Titulo = "A", Prioridad = Prioridad.Alta });
        repo.Add(new Tarea { Titulo = "B", Prioridad = Prioridad.Baja });
        repo.Add(new Tarea { Titulo = "C", Prioridad = Prioridad.Alta });
        var service = new TareaService(repo);

        var result = service.GetFiltered(prioridad: Prioridad.Alta);

        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal(Prioridad.Alta, t.Prioridad));
    }
}
