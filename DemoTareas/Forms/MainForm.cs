// Autor:   DemoTareas Team
// Fecha:   2026-06-18
// Versión: 1.0

using DemoTareas.Models;
using DemoTareas.Services;
using DemoTareas.Utilities;

namespace DemoTareas.Forms;

public class MainForm : Form
{
    private readonly ITareaService _tareaService;
    private readonly ICategoriaService _categoriaService;
    private readonly IPersonaService _personaService;

    private static readonly string[] OpcionesOrden =
    [
        "Fecha (reciente)",
        "Fecha (antigua)",
        "Prioridad (alta→baja)",
        "Prioridad (baja→alta)",
        "Vencimiento (próximo)",
        "Nombre (A→Z)"
    ];

    // Controles recoloreados al cambiar el tema.
    private Panel _topBar = null!;
    private Label _addIcon = null!;
    private Button _btnTema = null!;
    private FlowLayoutPanel _flowTareas = null!;
    private ComboBox _cboFiltroEstado = null!;
    private ComboBox _cboFiltroCategoria = null!;
    private ComboBox _cboFiltroPrioridad = null!;
    private ComboBox _cboOrdenar = null!;
    private TextBox _txtBusqueda = null!;
    private Label _lblEmpty = null!;
    private Button _btnCategorias = null!;
    private Button _btnPersonas = null!;
    private TextBox _txtNueva = null!;
    private Label _lblCount = null!;

    public MainForm(ITareaService tareaService, ICategoriaService categoriaService, IPersonaService personaService)
    {
        _tareaService = tareaService;
        _categoriaService = categoriaService;
        _personaService = personaService;
        InitializeComponent();
        ApplyThemeToUi();
        AppTheme.ThemeChanged += ApplyThemeToUi;
        RefrescarFiltros();
        RefrescarGrid();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        AppTheme.ThemeChanged -= ApplyThemeToUi;
        base.OnFormClosed(e);
    }

    private void InitializeComponent()
    {
        Text = "Mis tareas";
        Size = new Size(900, 640);
        MinimumSize = new Size(760, 480);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = AppTheme.BackgroundColor;
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = AppTheme.FontSmall;

        _topBar = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = AppTheme.AccentColor };
        var title = new Label { Text = "Mis tareas", ForeColor = Color.White, Font = AppTheme.FontTitle, Location = new Point(18, 18), AutoSize = true };

        var rightZone = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Width = 360,
            Padding = new Padding(0, 14, 12, 0),
            BackColor = Color.Transparent
        };

        _btnTema = new Button { Text = "🎨 Tema", AutoSize = true, Height = 28, Cursor = Cursors.Hand, TabStop = false, Margin = new Padding(0, 0, 0, 0) };
        _btnTema.Click += (s, e) => AbrirSelectorTema();

        var btnNueva = new Button { Text = "+ Nueva tarea", AutoSize = true, Height = 28, Cursor = Cursors.Hand, TabStop = false, Margin = new Padding(0, 0, 8, 0) };
        btnNueva.Click += (s, e) => NuevaTarea();
        btnNueva.FlatStyle = FlatStyle.Flat;
        btnNueva.FlatAppearance.BorderSize = 0;
        btnNueva.BackColor = Color.FromArgb(26, 74, 156);
        btnNueva.ForeColor = Color.White;
        btnNueva.Font = AppTheme.FontSmall;
        btnNueva.UseVisualStyleBackColor = false;

        _lblCount = new Label { Text = "0 tareas", ForeColor = Color.FromArgb(225, 235, 255), Font = AppTheme.FontSmall, AutoSize = true, Margin = new Padding(0, 6, 14, 0) };

        rightZone.Controls.Add(_btnTema);
        rightZone.Controls.Add(btnNueva);
        rightZone.Controls.Add(_lblCount);
        _topBar.Controls.AddRange(new Control[] { title, rightZone });

        var panelFiltros = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 48,
            ColumnCount = 5,
            RowCount = 1,
            BackColor = AppTheme.FilterBarColor,
            Padding = new Padding(4, 0, 4, 0)
        };
        panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 148));  // Estado
        panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 168));  // Categoría
        panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 148));  // Prioridad
        panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));  // Ordenar
        panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));   // Búsqueda
        panelFiltros.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _cboFiltroEstado = new ComboBox { FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.FontSmall, Dock = DockStyle.Fill, Margin = new Padding(0) };
        _cboFiltroEstado.Items.Add("(Todos)");
        _cboFiltroEstado.Items.AddRange(Enum.GetNames<EstadoTarea>());
        _cboFiltroEstado.SelectedIndex = 0;
        _cboFiltroEstado.SelectedIndexChanged += (s, e) => RefrescarGrid();

        _cboFiltroCategoria = new ComboBox { FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.FontSmall, Dock = DockStyle.Fill, Margin = new Padding(0) };
        _cboFiltroCategoria.SelectedIndexChanged += (s, e) => RefrescarGrid();

        _cboFiltroPrioridad = new ComboBox { FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.FontSmall, Dock = DockStyle.Fill, Margin = new Padding(0) };
        _cboFiltroPrioridad.Items.Add("(Todas)");
        _cboFiltroPrioridad.Items.AddRange(Enum.GetNames<Prioridad>());
        _cboFiltroPrioridad.SelectedIndex = 0;
        _cboFiltroPrioridad.SelectedIndexChanged += (s, e) => RefrescarGrid();

        _cboOrdenar = new ComboBox { FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.FontSmall, Dock = DockStyle.Fill, Margin = new Padding(0) };
        _cboOrdenar.Items.AddRange(OpcionesOrden);
        _cboOrdenar.SelectedIndex = 0;
        _cboOrdenar.SelectedIndexChanged += (s, e) => RefrescarGrid();

        _txtBusqueda = new TextBox
        {
            BorderStyle = BorderStyle.FixedSingle,
            Font = AppTheme.FontSmall,
            PlaceholderText = "🔍 Buscar tareas...",
            Dock = DockStyle.Fill,
            Margin = new Padding(0)
        };
        _txtBusqueda.TextChanged += (s, e) => RefrescarGrid();

        panelFiltros.Controls.Add(CeldaFiltro("ESTADO", _cboFiltroEstado), 0, 0);
        panelFiltros.Controls.Add(CeldaFiltro("CATEGORÍA", _cboFiltroCategoria), 1, 0);
        panelFiltros.Controls.Add(CeldaFiltro("PRIORIDAD", _cboFiltroPrioridad), 2, 0);
        panelFiltros.Controls.Add(CeldaFiltro("ORDENAR", _cboOrdenar), 3, 0);
        panelFiltros.Controls.Add(CeldaFiltro("BUSCAR", _txtBusqueda), 4, 0);

        _flowTareas = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, WrapContents = false, FlowDirection = FlowDirection.TopDown, BackColor = AppTheme.BackgroundColor, Padding = new Padding(8, 8, 8, 8) };
        _flowTareas.SizeChanged += (s, e) => AjustarAnchoItems();

        _lblEmpty = new Label
        {
            Text = "¡Añade tu primera tarea! 🎉",
            Font = AppTheme.FontItem,
            ForeColor = AppTheme.TextSecondary,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
            Visible = false
        };

        var tableAdd = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 64,
            ColumnCount = 4,
            RowCount = 1,
            BackColor = AppTheme.SurfaceColor,
            Padding = new Padding(0, 1, 0, 0)
        };
        tableAdd.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48));   // icono +
        tableAdd.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));   // TextBox
        tableAdd.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 106));  // Categorías
        tableAdd.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));  // Personas
        tableAdd.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tableAdd.Paint += (s, e) => e.Graphics.DrawLine(new Pen(AppTheme.BorderColor, 1), 0, 0, tableAdd.Width, 0);

        _addIcon = new Label
        {
            Text = "+", Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            ForeColor = AppTheme.AccentColor, Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };

        _txtNueva = new TextBox
        {
            BorderStyle = BorderStyle.None, Font = AppTheme.FontItem,
            PlaceholderText = "Añadir una tarea",
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
            Margin = new Padding(0, 22, 8, 0)
        };
        _txtNueva.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; CrearNuevaRapida(); } };

        _btnCategorias = new Button
        {
            Text = "Categorías", FlatStyle = FlatStyle.Flat,
            Dock = DockStyle.Fill, Margin = new Padding(2, 12, 2, 12)
        };
        AppTheme.ApplyFlatButton(_btnCategorias, false);
        _btnCategorias.Click += (s, e) => AbrirCategorias();

        _btnPersonas = new Button
        {
            Text = "Personas", FlatStyle = FlatStyle.Flat,
            Dock = DockStyle.Fill, Margin = new Padding(2, 12, 4, 12)
        };
        AppTheme.ApplyFlatButton(_btnPersonas, false);
        _btnPersonas.Click += (s, e) => AbrirPersonas();

        tableAdd.Controls.Add(_addIcon, 0, 0);
        tableAdd.Controls.Add(_txtNueva, 1, 0);
        tableAdd.Controls.Add(_btnCategorias, 2, 0);
        tableAdd.Controls.Add(_btnPersonas, 3, 0);

        Controls.Add(_flowTareas);
        Controls.Add(_lblEmpty);
        Controls.Add(tableAdd);
        Controls.Add(panelFiltros);
        Controls.Add(_topBar);
    }

    private static Panel CeldaFiltro(string etiqueta, Control control)
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4, 6, 4, 2) };
        var lbl = new Label { Text = etiqueta, Font = AppTheme.FontMeta, ForeColor = AppTheme.TextSecondary, Dock = DockStyle.Top, Height = 13 };
        control.Dock = DockStyle.Fill;
        panel.Controls.Add(control);
        panel.Controls.Add(lbl);
        return panel;
    }


    /// <summary>Recolorea los elementos dependientes del acento con el tema activo.</summary>
    private void ApplyThemeToUi()
    {
        _topBar.BackColor = AppTheme.AccentColor;
        _addIcon.ForeColor = AppTheme.AccentColor;

        _btnTema.FlatStyle = FlatStyle.Flat;
        _btnTema.FlatAppearance.BorderSize = 0;
        _btnTema.FlatAppearance.MouseOverBackColor = AppTheme.AccentDark;
        _btnTema.BackColor = AppTheme.AccentDark;
        _btnTema.ForeColor = Color.White;
        _btnTema.Font = AppTheme.FontSmall;
        _btnTema.UseVisualStyleBackColor = false;

        AppTheme.ApplyFlatButton(_btnCategorias, false);
        AppTheme.ApplyFlatButton(_btnPersonas, false);

        // Los ítems leen el acento al pintarse; basta con repintarlos.
        foreach (Control c in _flowTareas.Controls)
            c.Invalidate();
    }

    private void AbrirSelectorTema()
    {
        var popup = new ThemePickerPopup();
        popup.FormClosed += (s, e) => popup.Dispose();
        var origin = _btnTema.Parent!.PointToScreen(_btnTema.Location);
        popup.Location = new Point(
            origin.X + _btnTema.Width - popup.Width,
            origin.Y + _btnTema.Height + 6);
        popup.Show(this);
    }

    private void RefrescarFiltros()
    {
        var categorias = _categoriaService.GetAll();
        _cboFiltroCategoria.Items.Clear();
        _cboFiltroCategoria.Items.Add("(Todas)");
        foreach (var c in categorias)
            _cboFiltroCategoria.Items.Add(c);
        _cboFiltroCategoria.DisplayMember = "Nombre";
        _cboFiltroCategoria.SelectedIndex = 0;
    }

    private void RefrescarGrid()
    {
        EstadoTarea? estado = _cboFiltroEstado.SelectedIndex > 0
            ? Enum.Parse<EstadoTarea>(_cboFiltroEstado.SelectedItem!.ToString()!)
            : null;

        int? catId = _cboFiltroCategoria.SelectedItem is Categoria cat ? cat.Id : null;

        Prioridad? prioridad = _cboFiltroPrioridad.SelectedIndex > 0
            ? Enum.Parse<Prioridad>(_cboFiltroPrioridad.SelectedItem!.ToString()!)
            : null;

        var busqueda = _txtBusqueda.Text.Trim();

        var tareas = _tareaService.GetFiltered(estado, catId, prioridad);

        if (!string.IsNullOrEmpty(busqueda))
            tareas = tareas.Where(t =>
                t.Titulo.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                (t.Descripcion?.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();

        tareas = _cboOrdenar.SelectedIndex switch
        {
            1 => tareas.OrderBy(t => t.FechaCreacion).ToList(),
            2 => tareas.OrderBy(t => (int)t.Prioridad).ToList(),          // Alta=1 → primero
            3 => tareas.OrderByDescending(t => (int)t.Prioridad).ToList(), // Baja=3 → primero
            4 => tareas.OrderBy(t => t.FechaVencimiento ?? DateOnly.MaxValue).ToList(),
            5 => tareas.OrderBy(t => t.Titulo, StringComparer.OrdinalIgnoreCase).ToList(),
            _ => tareas.OrderByDescending(t => t.FechaCreacion).ToList()
        };

        _flowTareas.SuspendLayout();
        _flowTareas.Controls.Clear();
        foreach (var tarea in tareas)
        {
            var item = new TareaItemControl(tarea, _tareaService);
            item.ToggleRequested += (s, id) => ToggleCompletarTarea(id);
            item.EditRequested += (s, id) => EditarTarea(id);
            item.DeleteRequested += (s, id) => EliminarTarea(id);
            _flowTareas.Controls.Add(item);
        }
        _flowTareas.ResumeLayout();

        AjustarAnchoItems();
        _lblCount.Text = $"{tareas.Count} tareas";

        var totalTareas = _tareaService.GetAll().Count;
        if (tareas.Count == 0)
        {
            _lblEmpty.Text = totalTareas == 0
                ? "¡Añade tu primera tarea! 🎉"
                : "No hay tareas con estos filtros";
            _lblEmpty.Visible = true;
            _flowTareas.Visible = false;
        }
        else
        {
            _lblEmpty.Visible = false;
            _flowTareas.Visible = true;
        }
    }
    
       private void AbrirCategorias()
    {
        using var form = new CategoriaForm(_categoriaService);
        form.ShowDialog(this);
        RefrescarFiltros();
        RefrescarGrid();
    }

    private void AbrirPersonas()
    {
        using var form = new PersonaForm(_personaService);
        form.ShowDialog(this);
    }
    /// <summary>Hace que cada ítem ocupe el ancho disponible del panel (el FlowLayoutPanel no estira sus hijos).</summary>
    private void AjustarAnchoItems()
    {
        int ancho = _flowTareas.ClientSize.Width - _flowTareas.Padding.Horizontal;
        if (ancho <= 0) return;
        foreach (Control item in _flowTareas.Controls)
            item.Width = ancho;
    }

    private void CrearNuevaRapida()
    {
        if (string.IsNullOrWhiteSpace(_txtNueva.Text)) return;
        try
        {
            _tareaService.Add(new Tarea { Titulo = _txtNueva.Text.Trim() });
            _txtNueva.Clear();
            RefrescarFiltros();
            RefrescarGrid();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void NuevaTarea()
    {
        using var form = new TareaForm(_categoriaService, _personaService);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                _tareaService.Add(form.Tarea);
                RefrescarFiltros();
                RefrescarGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void EditarTarea(int id)
    {
        var tarea = _tareaService.GetAll().FirstOrDefault(t => t.Id == id);
        if (tarea is null) return;

        using var form = new TareaForm(_categoriaService, _personaService, tarea);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                _tareaService.Update(form.Tarea);
                RefrescarFiltros();
                RefrescarGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al actualizar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void ToggleCompletarTarea(int id)
    {
        try
        {
            _tareaService.ToggleComplete(id);
            RefrescarGrid();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void EliminarTarea(int id)
    {
        var tarea = _tareaService.GetAll().FirstOrDefault(t => t.Id == id);
        if (tarea is null) return;

        var confirm = MessageBox.Show(
            $"¿Eliminar la tarea '{tarea.Titulo}'?",
            "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes) return;

        try
        {
            _tareaService.Delete(tarea.Id);
            RefrescarGrid();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error al eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

 
}
