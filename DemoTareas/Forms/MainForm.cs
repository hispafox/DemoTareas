using DemoTareas.Models;
using DemoTareas.Services;
using DemoTareas.Utilities;

namespace DemoTareas.Forms;

public class MainForm : Form
{
    private readonly ITareaService _tareaService;
    private readonly ICategoriaService _categoriaService;
    private readonly IPersonaService _personaService;

    // Controles recoloreados al cambiar el tema.
    private Panel _topBar = null!;
    private Label _addIcon = null!;
    private Button _btnTema = null!;
    private FlowLayoutPanel _flowTareas = null!;
    private ComboBox _cboFiltroEstado = null!;
    private ComboBox _cboFiltroCategoria = null!;
    private ComboBox _cboFiltroPrioridad = null!;
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

        _topBar = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = AppTheme.AccentColor };
        var title = new Label { Text = "Mis tareas", ForeColor = Color.White, Font = AppTheme.FontTitle, Location = new Point(18, 18), AutoSize = true };

        var rightZone = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Width = 220,
            Padding = new Padding(0, 14, 12, 0),
            BackColor = Color.Transparent
        };

        _btnTema = new Button { Text = "🎨 Tema", AutoSize = true, Height = 28, Cursor = Cursors.Hand, TabStop = false, Margin = new Padding(0, 0, 0, 0) };
        _btnTema.Click += (s, e) => AbrirSelectorTema();

        _lblCount = new Label { Text = "0 tareas", ForeColor = Color.FromArgb(225, 235, 255), Font = AppTheme.FontSmall, AutoSize = true, Margin = new Padding(0, 6, 14, 0) };

        rightZone.Controls.Add(_btnTema);
        rightZone.Controls.Add(_lblCount);
        _topBar.Controls.AddRange(new Control[] { title, rightZone });

        var panelFiltros = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = AppTheme.FilterBarColor };
        var lblEstado = new Label { Text = "Estado", Location = new Point(14, 10), AutoSize = true, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontMeta };
        _cboFiltroEstado = new ComboBox { Location = new Point(60, 7), Width = 120, FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList };
        _cboFiltroEstado.Items.Add("(Todos)");
        _cboFiltroEstado.Items.AddRange(Enum.GetNames<EstadoTarea>());
        _cboFiltroEstado.SelectedIndex = 0;
        _cboFiltroEstado.SelectedIndexChanged += (s, e) => RefrescarGrid();

        var lblCat = new Label { Text = "Categoría", Location = new Point(190, 10), AutoSize = true, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontMeta };
        _cboFiltroCategoria = new ComboBox { Location = new Point(250, 7), Width = 150, FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList };
        _cboFiltroCategoria.SelectedIndexChanged += (s, e) => RefrescarGrid();

        var lblPri = new Label { Text = "Prioridad", Location = new Point(410, 10), AutoSize = true, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontMeta };
        _cboFiltroPrioridad = new ComboBox { Location = new Point(470, 7), Width = 120, FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList };
        _cboFiltroPrioridad.Items.Add("(Todas)");
        _cboFiltroPrioridad.Items.AddRange(Enum.GetNames<Prioridad>());
        _cboFiltroPrioridad.SelectedIndex = 0;
        _cboFiltroPrioridad.SelectedIndexChanged += (s, e) => RefrescarGrid();

        panelFiltros.Controls.AddRange(new Control[]
        {
            lblEstado, _cboFiltroEstado,
            lblCat, _cboFiltroCategoria,
            lblPri, _cboFiltroPrioridad
        });

        _flowTareas = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, WrapContents = false, FlowDirection = FlowDirection.TopDown, BackColor = AppTheme.BackgroundColor, Padding = new Padding(8, 8, 8, 8) };
        _flowTareas.SizeChanged += (s, e) => AjustarAnchoItems();

        var panelAdd = new Panel { Dock = DockStyle.Bottom, Height = 48, BackColor = AppTheme.SurfaceColor, BorderStyle = BorderStyle.FixedSingle };
        _addIcon = new Label { Text = "+", Font = new Font("Segoe UI", 18F, FontStyle.Bold), ForeColor = AppTheme.AccentColor, Location = new Point(14, 8), AutoSize = true };
        _txtNueva = new TextBox { Location = new Point(42, 12), Width = 640, BorderStyle = BorderStyle.None, Font = AppTheme.FontItem, PlaceholderText = "Añadir una tarea" };
        _txtNueva.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; CrearNuevaRapida(); } };
        _btnCategorias = new Button { Text = "Categorías", Location = new Point(695, 8), Width = 90, FlatStyle = FlatStyle.Flat };
        AppTheme.ApplyFlatButton(_btnCategorias, false);
        _btnCategorias.Click += (s, e) => AbrirCategorias();

        _btnPersonas = new Button { Text = "Personas", Location = new Point(790, 8), Width = 90, FlatStyle = FlatStyle.Flat };
        AppTheme.ApplyFlatButton(_btnPersonas, false);
        _btnPersonas.Click += (s, e) => AbrirPersonas();

        panelAdd.Controls.AddRange(new Control[] { _addIcon, _txtNueva, _btnCategorias, _btnPersonas });

        Controls.Add(_flowTareas);
        Controls.Add(panelAdd);
        Controls.Add(panelFiltros);
        Controls.Add(_topBar);
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

        var tareas = _tareaService.GetFiltered(estado, catId, prioridad);

        _flowTareas.Controls.Clear();
        foreach (var tarea in tareas)
        {
            var item = new TareaItemControl(tarea, _tareaService);
            item.ToggleRequested += (s, id) => ToggleCompletarTarea(id);
            item.EditRequested += (s, id) => EditarTarea(id);
            item.DeleteRequested += (s, id) => EliminarTarea(id);
            _flowTareas.Controls.Add(item);
        }

        AjustarAnchoItems();
        _lblCount.Text = $"{tareas.Count} tareas";
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
        using var form = new TareaForm(_categoriaService);
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

        using var form = new TareaForm(_categoriaService, tarea);
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

        _tareaService.Delete(tarea.Id);
        RefrescarGrid();
    }

 
}
