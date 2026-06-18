namespace DemoTareas.Models;

public class Tarea
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public EstadoTarea Estado { get; set; } = EstadoTarea.Pendiente;
    public Prioridad Prioridad { get; set; } = Prioridad.Media;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateOnly? FechaVencimiento { get; set; }

    public int? CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public int? PersonaId { get; set; }
    public Persona? Persona { get; set; }
}
