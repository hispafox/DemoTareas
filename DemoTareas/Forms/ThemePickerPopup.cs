// Autor:   DemoTareas Team
// Fecha:   2026-06-18
// Versión: 1.0

using DemoTareas.Utilities;

namespace DemoTareas.Forms;

/// <summary>
/// Pequeño popup "Elegir un tema" con una paleta de colores. Al pulsar un color
/// aplica el tema, lo persiste y notifica mediante <see cref="ThemeSelected"/>.
/// Se cierra al perder el foco, como un menú emergente.
/// </summary>
public sealed class ThemePickerPopup : Form
{
    private const int SwatchSize = 30;
    private const int SwatchGap = 8;

    /// <summary>Nombre del tema elegido por el usuario.</summary>
    public event EventHandler<string>? ThemeSelected;

    public ThemePickerPopup()
    {
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        ShowInTaskbar = false;
        BackColor = AppTheme.SurfaceColor;
        Padding = new Padding(14);

        var lbl = new Label
        {
            Text = "Elegir un tema",
            AutoSize = true,
            Font = AppTheme.FontHeader,
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(14, 12)
        };

        var flow = new FlowLayoutPanel
        {
            Location = new Point(14, 44),
            AutoSize = true,
            MaximumSize = new Size(4 * (SwatchSize + SwatchGap), 0),
            Padding = Padding.Empty,
            Margin = Padding.Empty
        };

        foreach (var option in AppTheme.Palette)
            flow.Controls.Add(CreateSwatch(option));

        Controls.Add(lbl);
        Controls.Add(flow);

        // Tamaño según el contenido (4 columnas).
        ClientSize = new Size(
            28 + 4 * (SwatchSize + SwatchGap),
            44 + flow.Height + 14);
    }

    private Control CreateSwatch(AppTheme.ThemeOption option)
    {
        var btn = new Button
        {
            Width = SwatchSize,
            Height = SwatchSize,
            Margin = new Padding(0, 0, SwatchGap, SwatchGap),
            BackColor = option.Accent,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            TabStop = false
        };
        btn.FlatAppearance.BorderColor = option.Name == AppTheme.CurrentThemeName
            ? AppTheme.TextPrimary
            : AppTheme.BorderColor;
        btn.FlatAppearance.BorderSize = option.Name == AppTheme.CurrentThemeName ? 2 : 1;

        var tip = new ToolTip();
        tip.SetToolTip(btn, option.Name);

        btn.Click += (s, e) =>
        {
            AppTheme.ApplyTheme(option.Name);
            ThemeSettings.Save(option.Name);
            ThemeSelected?.Invoke(this, option.Name);
            Close();
        };

        return btn;
    }

    protected override void OnDeactivate(EventArgs e)
    {
        base.OnDeactivate(e);
        Close();
    }
}
