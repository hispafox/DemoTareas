using DemoTareas.Services;
using DemoTareas.Tests.Fakes;

namespace DemoTareas.Tests;

public class CategoriaServiceTests
{
    private static CategoriaService CrearServicio() =>
        new CategoriaService(new FakeCategoriaRepository());

    [Fact]
    public void Add_NombreVacio_ThrowsArgumentException()
    {
        var service = CrearServicio();

        Assert.Throws<ArgumentException>(() => service.Add("   "));
    }

    [Fact]
    public void Add_NombreDuplicado_ThrowsInvalidOperationException()
    {
        var service = CrearServicio();
        service.Add("Trabajo");

        Assert.Throws<InvalidOperationException>(() => service.Add("Trabajo"));
    }

    [Fact]
    public void Add_NombreDuplicadoCaseInsensitive_ThrowsInvalidOperationException()
    {
        var service = CrearServicio();
        service.Add("Trabajo");

        Assert.Throws<InvalidOperationException>(() => service.Add("trabajo"));
    }

    [Fact]
    public void Add_NombreValido_SePersiste()
    {
        var service = CrearServicio();

        service.Add("Personal");

        Assert.Single(service.GetAll());
        Assert.Equal("Personal", service.GetAll()[0].Nombre);
    }

    [Fact]
    public void Delete_CategoriaExistente_SeElimina()
    {
        var repo = new FakeCategoriaRepository();
        var service = new CategoriaService(repo);
        service.Add("Test");
        var id = service.GetAll()[0].Id;

        service.Delete(id);

        Assert.Empty(service.GetAll());
    }

    [Fact]
    public void Update_NombreDuplicadoDeOtraCategoria_ThrowsInvalidOperationException()
    {
        var repo = new FakeCategoriaRepository();
        var service = new CategoriaService(repo);
        service.Add("Trabajo");
        service.Add("Personal");
        var idPersonal = service.GetAll().First(c => c.Nombre == "Personal").Id;

        Assert.Throws<InvalidOperationException>(() => service.Update(idPersonal, "Trabajo"));
    }
}
