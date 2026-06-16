using DemoTareas.Models;
using DemoTareas.Services;
using DemoTareas.Utilities;

namespace DemoTareas.Forms;

public partial class TareaItemControl : UserControl
{
    private readonly Tarea _tarea;
    private readonly ITareaService _tareaService;
    private bool _hover;

    public event EventHandler<int>? ToggleRequested;
    public event EventHandler<int>? EditRequested;
    public event EventHandler<int>? DeleteRequested;

    public TareaItemControl(Tarea tarea, ITareaService tareaService)
    {
        _tarea = tarea;
        _tareaService = tareaService;
        InitializeComponent();
        Height = 52;
        Margin = new Padding(0, 0, 0, 1);
        BackColor = AppTheme.SurfaceColor;
        BorderStyle = BorderStyle.None;
        TabStop = false;
    }

    private void InitializeComponent()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        MouseEnter += (s, e) => { _hover = true; Invalidate(); };
        MouseLeave += (s, e) => { _hover = false; Invalidate(); };
        DoubleClick += (s, e) => EditRequested?.Invoke(this, _tarea.Id);

        var menu = new ContextMenuStrip();
        menu.Items.Add("Editar", null, (s, e) => EditRequested?.Invoke(this, _tarea.Id));
        menu.Items.Add("Eliminar", null, (s, e) => DeleteRequested?.Invoke(this, _tarea.Id));
        ContextMenuStrip = menu;

        MouseDown += (s, e) =>
        {
            if (e.Button == MouseButtons.Right)
                ContextMenuStrip.Show(this, e.Location);
        };
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.Clear(_hover ? AppTheme.HoverColor : BackColor);

        var rect = new Rectangle(12, 16, 18, 18);
        AppTheme.DrawCircularCheckbox(e.Graphics, rect, _tarea.Estado == EstadoTarea.Completada);

        var title = _tarea.Titulo;
        var titleColor = _tarea.Estado == EstadoTarea.Completada ? AppTheme.TextCompleted : AppTheme.TextPrimary;
        using var titleFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        e.Graphics.DrawString(title, titleFont, new SolidBrush(titleColor), 40, 9);

        var meta = string.Join(" · ", new[]
        {
            _tarea.Categoria?.Nombre ?? "Sin categoría",
            _tarea.Prioridad.ToString(),
            _tarea.FechaVencimiento.HasValue ? _tarea.FechaVencimiento.Value.ToString("dd/MM") : "Sin fecha"
        });
        using var metaBrush = new SolidBrush(_tarea.FechaVencimiento.HasValue && _tarea.FechaVencimiento.Value < DateOnly.FromDateTime(DateTime.Today)
            ? AppTheme.DueDateColor
            : AppTheme.TextSecondary);
        using var metaFont = new Font("Segoe UI", 8.25F, FontStyle.Regular);
        e.Graphics.DrawString(meta, metaFont, metaBrush, 40, 29);

        var starColor = _tarea.Prioridad == Prioridad.Alta ? AppTheme.StarColor : AppTheme.TextSecondary;
        e.Graphics.DrawString(_tarea.Prioridad == Prioridad.Alta ? "★" : "☆", new Font("Segoe UI", 10.5F, FontStyle.Bold), new SolidBrush(starColor), Width - 36, 14);

        using var borderPen = new Pen(AppTheme.BorderColor, 1F);
        e.Graphics.DrawLine(borderPen, 0, Height - 1, Width, Height - 1);
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);
        if (e.Button == MouseButtons.Left && new Rectangle(8, 14, 26, 26).Contains(e.Location))
        {
            ToggleRequested?.Invoke(this, _tarea.Id);
            Invalidate();
        }
    }
}
