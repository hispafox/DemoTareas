using DemoTareas.Models;
using DemoTareas.Services;
using DemoTareas.Tests.Fakes;

namespace DemoTareas.Tests;

public class PersonaServiceTests
{
    private static PersonaService CrearServicio() =>
        new PersonaService(new FakePersonaRepository());

    [Fact]
    public void Add_NombreVacio_ThrowsArgumentException()
    {
        var service = CrearServicio();

        Assert.Throws<ArgumentException>(() => service.Add(new Persona { Nombre = "   ", Apellido = "García" }));
    }

    [Fact]
    public void Add_ApellidoVacio_ThrowsArgumentException()
    {
        var service = CrearServicio();

        Assert.Throws<ArgumentException>(() => service.Add(new Persona { Nombre = "Ana", Apellido = "   " }));
    }

    [Fact]
    public void Add_PersonaDuplicada_ThrowsInvalidOperationException()
    {
        var service = CrearServicio();
        service.Add(new Persona { Nombre = "Ana", Apellido = "García" });

        Assert.Throws<InvalidOperationException>(() => service.Add(new Persona { Nombre = "ana", Apellido = "garcía" }));
    }

    [Fact]
    public void Add_PersonaValida_SePersiste()
    {
        var service = CrearServicio();

        service.Add(new Persona { Nombre = "Luis", Apellido = "Pérez", Edad = 30 });

        Assert.Single(service.GetAll());
        Assert.Equal("Luis", service.GetAll()[0].Nombre);
    }

    [Fact]
    public void Update_PersonaExistente_ActualizaDatos()
    {
        var repo = new FakePersonaRepository();
        repo.Add(new Persona { Nombre = "Ana", Apellido = "García", Edad = 20 });
        var service = new PersonaService(repo);

        service.Update(new Persona { Id = 1, Nombre = "Ana", Apellido = "López", Edad = 21 });

        var actual = repo.GetById(1);
        Assert.NotNull(actual);
        Assert.Equal("López", actual!.Apellido);
        Assert.Equal(21, actual.Edad);
    }

    [Fact]
    public void Update_PersonaExistente_ActualizaDepartamento()
    {
        var repo = new FakePersonaRepository();
        repo.Add(new Persona { Nombre = "Ana", Apellido = "García", Edad = 20 });
        var service = new PersonaService(repo);

        service.Update(new Persona { Id = 1, Nombre = "Ana", Apellido = "García", Edad = 20, Departamento = "IT" });

        Assert.Equal("IT", repo.GetById(1)!.Departamento);
    }
}
