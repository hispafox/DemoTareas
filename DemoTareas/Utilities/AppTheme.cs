using System.Drawing;

namespace DemoTareas.Utilities;

public static class AppTheme
{
    /// <summary>Opción de tema seleccionable por el usuario ("Pick a theme").</summary>
    public sealed record ThemeOption(string Name, Color Accent);

    /// <summary>Paleta de temas inspirada en los colores de Microsoft To Do.</summary>
    public static readonly IReadOnlyList<ThemeOption> Palette = new[]
    {
        new ThemeOption("Azul", Color.FromArgb(37, 100, 207)),
        new ThemeOption("Verde", Color.FromArgb(16, 137, 62)),
        new ThemeOption("Verde azulado", Color.FromArgb(1, 133, 116)),
        new ThemeOption("Rojo", Color.FromArgb(196, 43, 28)),
        new ThemeOption("Morado", Color.FromArgb(116, 77, 169)),
        new ThemeOption("Rosa", Color.FromArgb(194, 57, 121)),
        new ThemeOption("Naranja", Color.FromArgb(202, 80, 16)),
        new ThemeOption("Gris", Color.FromArgb(96, 94, 92)),
    };

    private static ThemeOption _current = Palette[0];

    /// <summary>Se dispara cuando el usuario cambia el tema, para que la UI se recoloree.</summary>
    public static event Action? ThemeChanged;

    public static string CurrentThemeName => _current.Name;

    // Colores derivados del acento activo.
    public static Color AccentColor => _current.Accent;
    public static Color AccentDark => Darken(_current.Accent, 0.73f);
    public static Color HoverColor => Tint(_current.Accent, 0.90f);

    // Colores neutros (no dependen del tema).
    public static readonly Color BackgroundColor = Color.FromArgb(243, 242, 241);
    public static readonly Color SurfaceColor = Color.White;
    public static readonly Color BorderColor = Color.FromArgb(225, 223, 221);
    public static readonly Color FilterBarColor = Color.FromArgb(237, 235, 233);
    public static readonly Color TextPrimary = Color.FromArgb(31, 31, 31);
    public static readonly Color TextSecondary = Color.FromArgb(118, 118, 118);
    public static readonly Color TextCompleted = Color.FromArgb(160, 160, 160);
    public static readonly Color StarColor = Color.FromArgb(248, 197, 55);
    public static readonly Color DueDateColor = Color.FromArgb(196, 43, 28);

    public static readonly Font FontTitle = new("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
    public static readonly Font FontHeader = new("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point);
    public static readonly Font FontItem = new("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
    public static readonly Font FontMeta = new("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
    public static readonly Font FontSmall = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

    /// <summary>Aplica el tema cuyo nombre coincide; si no existe, usa el primero de la paleta.</summary>
    public static void ApplyTheme(string? name)
    {
        var option = Palette.FirstOrDefault(t => t.Name == name) ?? Palette[0];
        if (option == _current) return;
        _current = option;
        ThemeChanged?.Invoke();
    }

    public static void ApplyFlatButton(Button button, bool primary)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = primary ? 0 : 1;
        button.FlatAppearance.BorderColor = primary ? AccentColor : BorderColor;
        button.BackColor = primary ? AccentColor : SurfaceColor;
        button.ForeColor = primary ? Color.White : AccentColor;
        button.FlatAppearance.MouseOverBackColor = primary ? AccentDark : Color.FromArgb(237, 235, 250);
        button.FlatAppearance.MouseDownBackColor = primary ? AccentDark : Color.FromArgb(225, 222, 245);
        button.Font = FontSmall;
        button.Height = 32;
        button.UseVisualStyleBackColor = false;
    }

    public static void DrawCircularCheckbox(Graphics g, Rectangle bounds, bool isChecked)
    {
        using var pen = new Pen(isChecked ? AccentColor : TextSecondary, 1.8F);
        g.DrawEllipse(pen, bounds);

        if (isChecked)
        {
            using var fill = new SolidBrush(AccentColor);
            g.FillEllipse(fill, new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2));

            using var checkPen = new Pen(Color.White, 2F);
            g.DrawLine(checkPen, bounds.X + 5, bounds.Y + 9, bounds.X + 8, bounds.Y + 12);
            g.DrawLine(checkPen, bounds.X + 8, bounds.Y + 12, bounds.X + 13, bounds.Y + 6);
        }
    }

    /// <summary>Oscurece un color multiplicando sus componentes (para el acento oscuro).</summary>
    private static Color Darken(Color c, float factor) =>
        Color.FromArgb(c.A, (int)(c.R * factor), (int)(c.G * factor), (int)(c.B * factor));

    /// <summary>Mezcla un color con blanco; amount 0 = color puro, 1 = blanco (para hover).</summary>
    private static Color Tint(Color c, float amount) =>
        Color.FromArgb(
            c.R + (int)((255 - c.R) * amount),
            c.G + (int)((255 - c.G) * amount),
            c.B + (int)((255 - c.B) * amount));
}
