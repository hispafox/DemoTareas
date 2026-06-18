// Autor:   DemoTareas Team
// Fecha:   2026-06-18
// Versión: 1.0

using DemoTareas.Data;
using DemoTareas.Forms;
using DemoTareas.Repositories;
using DemoTareas.Services;
using DemoTareas.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DemoTareas;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (s, e) =>
            MessageBox.Show(e.Exception.ToString(), "Error no controlado", MessageBoxButtons.OK, MessageBoxIcon.Error);
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            MessageBox.Show(e.ExceptionObject?.ToString(), "Error crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);

        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        ApplicationConfiguration.Initialize();
#pragma warning disable WFO5001
        Application.SetColorMode(SystemColorMode.System);
#pragma warning restore WFO5001
        ThemeSettings.Load();

        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DemoTareas", "startup-error.log");

        try
        {
            using var dbContext = new TareasDbContext();
            dbContext.Database.Migrate();
            DataSeeder.Seed(dbContext);

            var tareaRepository = new TareaRepository(dbContext);
            var categoriaRepository = new CategoriaRepository(dbContext);
            var personaRepository = new PersonaRepository(dbContext);

            var tareaService = new TareaService(tareaRepository);
            var categoriaService = new CategoriaService(categoriaRepository);
            var personaService = new PersonaService(personaRepository);

            Application.Run(new MainForm(tareaService, categoriaService, personaService));
        }
        catch (Exception ex)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            File.WriteAllText(logPath, ex.ToString());
            MessageBox.Show($"Error:\n{ex.Message}\n\nLog guardado en:\n{logPath}",
                "Error al iniciar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
