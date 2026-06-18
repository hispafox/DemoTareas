using DemoTareas.Models;
using DemoTareas.Services;
using DemoTareas.Utilities;

namespace DemoTareas.Forms;

public class TareaForm : Form
{
    private readonly ICategoriaService _categoriaService;
    private readonly IPersonaService _personaService;
    private readonly Tarea _tarea;
    private readonly bool _esNueva;

    private Panel _header = null!;
    private Label _lblHeaderTitle = null!;
    private TextBox _txtTitulo = null!;
    private TextBox _txtDescripcion = null!;
    private ComboBox _cboPrioridad = null!;
    private ComboBox _cboCategoria = null!;
    private ComboBox _cboEstado = null!;
    private ComboBox _cboPersona = null!;
    private CheckBox _chkFechaVencimiento = null!;
    private DateTimePicker _dtpFechaVencimiento = null!;
    private Button _btnGuardar = null!;
    private Button _btnCancelar = null!;
    private ErrorProvider _errorProvider = null!;

    public Tarea Tarea => _tarea;

    public TareaForm(ICategoriaService categoriaService, IPersonaService personaService, Tarea? tarea = null)
    {
        _categoriaService = categoriaService;
        _personaService = personaService;
        _esNueva = tarea is null;
        _tarea = tarea ?? new Tarea();
        InitializeComponent();
        ApplyTheme();
        AppTheme.ThemeChanged += ApplyTheme;
        CargarDatos();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        AppTheme.ThemeChanged -= ApplyTheme;
        base.OnFormClosed(e);
    }

    private void ApplyTheme()
    {
        _header.BackColor = AppTheme.AccentColor;
        AppTheme.ApplyFlatButton(_btnGuardar, true);
        AppTheme.ApplyFlatButton(_btnCancelar, false);
    }

    private void InitializeComponent()
    {
        var titulo = _esNueva ? "Nueva tarea" : "Editar tarea";
        Text = titulo;
        Size = new Size(440, 530);
        MinimumSize = new Size(440, 530);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = AppTheme.SurfaceColor;
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = AppTheme.FontSmall;

        _errorProvider = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };

        // Header
        _header = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = AppTheme.AccentColor };
        _lblHeaderTitle = new Label
        {
            Text = titulo,
            ForeColor = Color.White,
            Font = AppTheme.FontTitle,
            Location = new Point(18, 16),
            AutoSize = true
        };
        _header.Controls.Add(_lblHeaderTitle);

        // Footer con botones
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 56, BackColor = AppTheme.SurfaceColor };
        footer.Paint += (s, e) => e.Graphics.DrawLine(new Pen(AppTheme.BorderColor, 1), 0, 0, footer.Width, 0);

        _btnGuardar = new Button
        {
            Text = "Guardar",
            Width = 96,
            Height = 32,
            Margin = new Padding(8, 12, 8, 12),
            DialogResult = DialogResult.None
        };
        _btnGuardar.Click += BtnGuardar_Click;

        _btnCancelar = new Button
        {
            Text = "Cancelar",
            Width = 96,
            Height = 32,
            Margin = new Padding(0, 12, 0, 12),
            DialogResult = DialogResult.Cancel
        };

        var btnFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 0, 8, 0)
        };
        btnFlow.Controls.Add(_btnGuardar);
        btnFlow.Controls.Add(_btnCancelar);
        footer.Controls.Add(btnFlow);

        // Cuerpo con TableLayoutPanel
        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 11,
            Padding = new Padding(16)
        };
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        for (int i = 0; i < 10; i++)
            content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));  // filler

        // Fila 3 (descripción textarea): altura fija
        content.RowStyles[3] = new RowStyle(SizeType.Absolute, 76f);

        // — Título —
        var lblTitulo = new Label
        {
            Text = "TÍTULO *",
            AutoSize = true,
            ForeColor = AppTheme.TextSecondary,
            Font = AppTheme.FontMeta,
            Margin = new Padding(0, 0, 0, 2)
        };
        _txtTitulo = new TextBox
        {
            Dock = DockStyle.Fill,
            MaxLength = 200,
            BorderStyle = BorderStyle.FixedSingle,
            Font = AppTheme.FontItem,
            Margin = new Padding(0, 0, 0, 10)
        };
        _txtTitulo.Validating += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(_txtTitulo.Text))
            {
                _errorProvider.SetError(_txtTitulo, "El título es obligatorio.");
                e.Cancel = true;
            }
            else _errorProvider.SetError(_txtTitulo, string.Empty);
        };
        content.Controls.Add(lblTitulo, 0, 0);
        content.SetColumnSpan(lblTitulo, 2);
        content.Controls.Add(_txtTitulo, 0, 1);
        content.SetColumnSpan(_txtTitulo, 2);

        // — Descripción —
        var lblDescripcion = new Label
        {
            Text = "DESCRIPCIÓN",
            AutoSize = true,
            ForeColor = AppTheme.TextSecondary,
            Font = AppTheme.FontMeta,
            Margin = new Padding(0, 0, 0, 2)
        };
        _txtDescripcion = new TextBox
        {
            Dock = DockStyle.Fill,
            MaxLength = 1000,
            Multiline = true,
            BorderStyle = BorderStyle.FixedSingle,
            Font = AppTheme.FontItem,
            Margin = new Padding(0, 0, 0, 10)
        };
        content.Controls.Add(lblDescripcion, 0, 2);
        content.SetColumnSpan(lblDescripcion, 2);
        content.Controls.Add(_txtDescripcion, 0, 3);
        content.SetColumnSpan(_txtDescripcion, 2);

        // — Prioridad | Categoría —
        var lblPrioridad = new Label
        {
            Text = "PRIORIDAD",
            AutoSize = true,
            ForeColor = AppTheme.TextSecondary,
            Font = AppTheme.FontMeta,
            Margin = new Padding(0, 0, 0, 2)
        };
        _cboPrioridad = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Flat,
            Font = AppTheme.FontItem,
            Margin = new Padding(0, 0, 8, 10)
        };
        _cboPrioridad.Items.AddRange(Enum.GetNames<Prioridad>());

        var lblCategoria = new Label
        {
            Text = "CATEGORÍA",
            AutoSize = true,
            ForeColor = AppTheme.TextSecondary,
            Font = AppTheme.FontMeta,
            Margin = new Padding(8, 0, 0, 2)
        };
        _cboCategoria = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Flat,
            Font = AppTheme.FontItem,
            Margin = new Padding(8, 0, 0, 10)
        };
        content.Controls.Add(lblPrioridad, 0, 4);
        content.Controls.Add(lblCategoria, 1, 4);
        content.Controls.Add(_cboPrioridad, 0, 5);
        content.Controls.Add(_cboCategoria, 1, 5);

        // — Estado | Persona —
        var lblEstado = new Label
        {
            Text = "ESTADO",
            AutoSize = true,
            ForeColor = AppTheme.TextSecondary,
            Font = AppTheme.FontMeta,
            Margin = new Padding(0, 0, 0, 2)
        };
        _cboEstado = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Flat,
            Font = AppTheme.FontItem,
            Margin = new Padding(0, 0, 8, 10)
        };
        _cboEstado.Items.AddRange(Enum.GetNames<EstadoTarea>());

        var lblPersona = new Label
        {
            Text = "PERSONA ASIGNADA",
            AutoSize = true,
            ForeColor = AppTheme.TextSecondary,
            Font = AppTheme.FontMeta,
            Margin = new Padding(8, 0, 0, 2)
        };
        _cboPersona = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Flat,
            Font = AppTheme.FontItem,
            Margin = new Padding(8, 0, 0, 10)
        };
        content.Controls.Add(lblEstado, 0, 6);
        content.Controls.Add(lblPersona, 1, 6);
        content.Controls.Add(_cboEstado, 0, 7);
        content.Controls.Add(_cboPersona, 1, 7);

        // — Fecha de vencimiento —
        _chkFechaVencimiento = new CheckBox
        {
            Text = "Fecha de vencimiento",
            AutoSize = true,
            Font = AppTheme.FontItem,
            ForeColor = AppTheme.TextPrimary,
            Margin = new Padding(0, 0, 0, 4)
        };
        _chkFechaVencimiento.CheckedChanged += (s, e) =>
            _dtpFechaVencimiento.Enabled = _chkFechaVencimiento.Checked;

        _dtpFechaVencimiento = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            MinDate = DateTime.Today,
            Enabled = false,
            Font = AppTheme.FontItem,
            Margin = new Padding(0, 0, 0, 0)
        };
        content.Controls.Add(_chkFechaVencimiento, 0, 8);
        content.SetColumnSpan(_chkFechaVencimiento, 2);
        content.Controls.Add(_dtpFechaVencimiento, 0, 9);

        Controls.Add(content);
        Controls.Add(footer);
        Controls.Add(_header);

        AcceptButton = _btnGuardar;
        CancelButton = _btnCancelar;
    }

    private void CargarDatos()
    {
        _txtTitulo.Text = _tarea.Titulo;
        _txtDescripcion.Text = _tarea.Descripcion ?? string.Empty;
        _cboPrioridad.SelectedIndex = (int)_tarea.Prioridad - 1;

        _cboCategoria.Items.Add("(Sin categoría)");
        foreach (var cat in _categoriaService.GetAll())
            _cboCategoria.Items.Add(cat);
        _cboCategoria.DisplayMember = "Nombre";

        if (_tarea.CategoriaId.HasValue)
        {
            var cat = _cboCategoria.Items.OfType<Categoria>()
                .FirstOrDefault(c => c.Id == _tarea.CategoriaId.Value);
            _cboCategoria.SelectedItem = cat ?? _cboCategoria.Items[0];
        }
        else
        {
            _cboCategoria.SelectedIndex = 0;
        }

        _cboEstado.SelectedIndex = (int)_tarea.Estado;

        _cboPersona.Items.Add("(Sin asignar)");
        foreach (var p in _personaService.GetAll())
            _cboPersona.Items.Add(p);
        if (_tarea.PersonaId.HasValue)
        {
            var persona = _cboPersona.Items.OfType<Persona>()
                .FirstOrDefault(p => p.Id == _tarea.PersonaId.Value);
            _cboPersona.SelectedItem = persona ?? _cboPersona.Items[0];
        }
        else
        {
            _cboPersona.SelectedIndex = 0;
        }

        if (_tarea.FechaVencimiento.HasValue)
        {
            _chkFechaVencimiento.Checked = true;
            _dtpFechaVencimiento.Enabled = true;
            _dtpFechaVencimiento.Value = _tarea.FechaVencimiento.Value.ToDateTime(TimeOnly.MinValue);
        }
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (!ValidateChildren()) return;

        _tarea.Titulo = _txtTitulo.Text.Trim();
        _tarea.Descripcion = string.IsNullOrWhiteSpace(_txtDescripcion.Text) ? null : _txtDescripcion.Text.Trim();
        _tarea.Prioridad = (Prioridad)(_cboPrioridad.SelectedIndex + 1);

        if (_cboCategoria.SelectedItem is Categoria cat)
        {
            _tarea.CategoriaId = cat.Id;
            _tarea.Categoria = cat;
        }
        else
        {
            _tarea.CategoriaId = null;
            _tarea.Categoria = null;
        }

        _tarea.Estado = (EstadoTarea)_cboEstado.SelectedIndex;

        if (_cboPersona.SelectedItem is Persona persona)
        {
            _tarea.PersonaId = persona.Id;
            _tarea.Persona = persona;
        }
        else
        {
            _tarea.PersonaId = null;
            _tarea.Persona = null;
        }

        _tarea.FechaVencimiento = _chkFechaVencimiento.Checked
            ? DateOnly.FromDateTime(_dtpFechaVencimiento.Value)
            : null;

        DialogResult = DialogResult.OK;
    }
}
