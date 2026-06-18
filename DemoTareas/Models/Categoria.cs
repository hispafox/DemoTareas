// Autor:   DemoTareas Team
// Fecha:   2026-06-18
// Versión: 1.0

namespace DemoTareas.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
