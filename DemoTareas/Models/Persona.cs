// Autor:   DemoTareas Team
// Fecha:   2026-06-18
// Versión: 1.0

public class Persona
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;

    public override string ToString() => $"{Nombre} {Apellido}".Trim();
    public int Edad { get; set; }
    public string? Departamento { get; set; }
}
