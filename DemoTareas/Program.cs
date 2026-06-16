using DemoTareas.Data;
using DemoTareas.Forms;
using DemoTareas.Repositories;
using DemoTareas.Services;
using DemoTareas.Utilities;

namespace DemoTareas;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        ThemeSettings.Load();

        using var dbContext = new TareasDbContext();
        dbContext.Database.EnsureCreated();
        DataSeeder.Seed(dbContext);

        var tareaRepository = new TareaRepository(dbContext);
        var categoriaRepository = new CategoriaRepository(dbContext);

        var tareaService = new TareaService(tareaRepository);
        var categoriaService = new CategoriaService(categoriaRepository);

        Application.Run(new MainForm(tareaService, categoriaService));
    }
}