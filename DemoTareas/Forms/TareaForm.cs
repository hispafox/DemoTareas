using DemoTareas.Models;
using DemoTareas.Services;

namespace DemoTareas.Forms;

public class TareaForm : Form
{
    private readonly ICategoriaService _categoriaService;
    private readonly Tarea _tarea;
    private readonly bool _esNueva;

    private TextBox _txtTitulo = null!;
    private TextBox _txtDescripcion = null!;
    private ComboBox _cboPrioridad = null!;
    private ComboBox _cboCategoria = null!;
    private CheckBox _chkFechaVencimiento = null!;
    private DateTimePicker _dtpFechaVencimiento = null!;
    private Button _btnGuardar = null!;
    private Button _btnCancelar = null!;
    private ErrorProvider _errorProvider = null!;

    public Tarea Tarea => _tarea;

    public TareaForm(ICategoriaService categoriaService, Tarea? tarea = null)
    {
        _categoriaService = categoriaService;
        _esNueva = tarea is null;
        _tarea = tarea ?? new Tarea();
        InitializeComponent();
        CargarDatos();
    }

    private void InitializeComponent()
    {
        Text = _esNueva ? "Nueva tarea" : "Editar tarea";
        Size = new Size(420, 380);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        _errorProvider = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };

        var lblTitulo = new Label { Text = "Título *", Location = new Point(12, 15), AutoSize = true };
        _txtTitulo = new TextBox { Location = new Point(12, 33), Width = 380, MaxLength = 200 };
        _txtTitulo.Validating += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(_txtTitulo.Text))
            {
                _errorProvider.SetError(_txtTitulo, "El título es obligatorio.");
                e.Cancel = true;
            }
            else _errorProvider.SetError(_txtTitulo, string.Empty);
        };

        var lblDescripcion = new Label { Text = "Descripción", Location = new Point(12, 70), AutoSize = true };
        _txtDescripcion = new TextBox
        {
            Location = new Point(12, 88),
            Width = 380,
            Height = 60,
            Multiline = true,
            MaxLength = 1000
        };

        var lblPrioridad = new Label { Text = "Prioridad", Location = new Point(12, 162), AutoSize = true };
        _cboPrioridad = new ComboBox
        {
            Location = new Point(12, 180),
            Width = 175,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _cboPrioridad.Items.AddRange(Enum.GetNames<Prioridad>());

        var lblCategoria = new Label { Text = "Categoría", Location = new Point(210, 162), AutoSize = true };
        _cboCategoria = new ComboBox
        {
            Location = new Point(210, 180),
            Width = 182,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        _chkFechaVencimiento = new CheckBox
        {
            Text = "Fecha de vencimiento",
            Location = new Point(12, 220),
            AutoSize = true
        };
        _chkFechaVencimiento.CheckedChanged += (s, e) =>
            _dtpFechaVencimiento.Enabled = _chkFechaVencimiento.Checked;

        _dtpFechaVencimiento = new DateTimePicker
        {
            Location = new Point(12, 244),
            Width = 200,
            Format = DateTimePickerFormat.Short,
            MinDate = DateTime.Today,
            Enabled = false
        };

        _btnGuardar = new Button
        {
            Text = "Guardar",
            Location = new Point(226, 305),
            Width = 80,
            DialogResult = DialogResult.None
        };
        _btnGuardar.Click += BtnGuardar_Click;

        _btnCancelar = new Button
        {
            Text = "Cancelar",
            Location = new Point(314, 305),
            Width = 80,
            DialogResult = DialogResult.Cancel
        };

        Controls.AddRange(new Control[]
        {
            lblTitulo, _txtTitulo,
            lblDescripcion, _txtDescripcion,
            lblPrioridad, _cboPrioridad,
            lblCategoria, _cboCategoria,
            _chkFechaVencimiento, _dtpFechaVencimiento,
            _btnGuardar, _btnCancelar
        });

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

        _tarea.FechaVencimiento = _chkFechaVencimiento.Checked
            ? DateOnly.FromDateTime(_dtpFechaVencimiento.Value)
            : null;

        DialogResult = DialogResult.OK;
    }
}
