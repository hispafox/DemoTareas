// Autor:   DemoTareas Team
// Fecha:   2026-06-18
// Versión: 1.0

using System.Text.Json;

namespace DemoTareas.Utilities;

/// <summary>
/// Lee y guarda el tema elegido por el usuario en un pequeño JSON dentro de
/// %AppData%/DemoTareas, junto a la base de datos.
/// </summary>
public static class ThemeSettings
{
    private sealed record Settings(string ThemeName);

    private static string FilePath
    {
        get
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = Path.Combine(appData, "DemoTareas");
            Directory.CreateDirectory(folder);
            return Path.Combine(folder, "theme.json");
        }
    }

    /// <summary>Aplica a <see cref="AppTheme"/> el tema guardado; si no hay, deja el predeterminado.</summary>
    public static void Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return;
            var settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(FilePath));
            if (settings is not null)
                AppTheme.ApplyTheme(settings.ThemeName);
        }
        catch
        {
            // Si el archivo está corrupto o no se puede leer, se ignora y se usa el tema por defecto.
        }
    }

    /// <summary>Guarda el nombre del tema indicado para futuras sesiones.</summary>
    public static void Save(string themeName)
    {
        try
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(new Settings(themeName)));
        }
        catch
        {
            // La persistencia del tema no es crítica; si falla no interrumpimos al usuario.
        }
    }
}
