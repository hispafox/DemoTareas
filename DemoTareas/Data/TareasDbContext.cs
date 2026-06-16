using DemoTareas.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoTareas.Data;

public class TareasDbContext : DbContext
{
    public DbSet<Tarea> Tareas { get; set; }
    public DbSet<Categoria> Categorias { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dbFolder = Path.Combine(appData, "DemoTareas");
        Directory.CreateDirectory(dbFolder);
        var dbPath = Path.Combine(dbFolder, "tareas.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>()
            .HasIndex(c => c.Nombre)
            .IsUnique();

        modelBuilder.Entity<Tarea>()
            .HasOne(t => t.Categoria)
            .WithMany(c => c.Tareas)
            .HasForeignKey(t => t.CategoriaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
