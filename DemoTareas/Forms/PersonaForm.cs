using DemoTareas.Models;
using DemoTareas.Services;

namespace DemoTareas.Forms;

public class PersonaForm : Form
{
    private readonly IPersonaService _personaService;

    private ListBox _lstPersonas = null!;
    private TextBox _txtNombre = null!;
    private TextBox _txtApellido = null!;
    private NumericUpDown _numEdad = null!;
    private TextBox _txtDepartamento = null!;
    private Button _btnAgregar = null!;
    private Button _btnEditar = null!;
    private Button _btnEliminar = null!;
    private Button _btnCerrar = null!;

    public PersonaForm(IPersonaService personaService)
    {
        _personaService = personaService;
        InitializeComponent();
        RefrescarLista();
    }

    private void InitializeComponent()
    {
        Text = "Gestionar personas";
        Size = new Size(420, 500);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        var lblNombre = new Label { Text = "Nombre:", Location = new Point(12, 14), AutoSize = true };
        _txtNombre = new TextBox { Location = new Point(12, 32), Width = 180 };

        var lblApellido = new Label { Text = "Apellido:", Location = new Point(210, 14), AutoSize = true };
        _txtApellido = new TextBox { Location = new Point(210, 32), Width = 180 };

        var lblEdad = new Label { Text = "Edad:", Location = new Point(12, 68), AutoSize = true };
        _numEdad = new NumericUpDown { Location = new Point(12, 86), Width = 100, Minimum = 0, Maximum = 120 };

        var lblDepartamento = new Label { Text = "Departamento:", Location = new Point(12, 120), AutoSize = true };
        _txtDepartamento = new TextBox { Location = new Point(12, 138), Width = 378 };

        _btnAgregar = new Button { Text = "Agregar", Location = new Point(130, 176), Width = 80 };
        _btnAgregar.Click += BtnAgregar_Click;

        _btnEditar = new Button { Text = "Editar", Location = new Point(220, 176), Width = 80 };
        _btnEditar.Click += BtnEditar_Click;

        _btnEliminar = new Button { Text = "Eliminar", Location = new Point(310, 176), Width = 80 };
        _btnEliminar.Click += BtnEliminar_Click;

        _lstPersonas = new ListBox
        {
            Location = new Point(12, 218),
            Width = 378,
            Height = 195,
            DisplayMember = "NombreCompleto"
        };
        _lstPersonas.SelectedIndexChanged += LstPersonas_SelectedIndexChanged;

        _btnCerrar = new Button
        {
            Text = "Cerrar",
            Location = new Point(310, 424),
            Width = 80,
            DialogResult = DialogResult.Cancel
        };

        Controls.AddRange(new Control[]
        {
            lblNombre, _txtNombre,
            lblApellido, _txtApellido,
            lblEdad, _numEdad,
            lblDepartamento, _txtDepartamento,
            _btnAgregar, _btnEditar, _btnEliminar,
            _lstPersonas,
            _btnCerrar
        });

        CancelButton = _btnCerrar;
    }

    private void RefrescarLista()
    {
        _lstPersonas.DataSource = null;
        _lstPersonas.DataSource = _personaService.GetAll().Select(p => new
        {
            p.Id,
            NombreCompleto = $"{p.Nombre} {p.Apellido} — Edad: {p.Edad}" +
                (string.IsNullOrEmpty(p.Departamento) ? "" : $" — {p.Departamento}")
        }).ToList();
        _lstPersonas.DisplayMember = "NombreCompleto";
    }

    private void BtnAgregar_Click(object? sender, EventArgs e)
    {
        try
        {
            _personaService.Add(new Persona
            {
                Nombre = _txtNombre.Text.Trim(),
                Apellido = _txtApellido.Text.Trim(),
                Edad = (int)_numEdad.Value,
                Departamento = string.IsNullOrWhiteSpace(_txtDepartamento.Text) ? null : _txtDepartamento.Text.Trim()
            });
            LimpiarCampos();
            RefrescarLista();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnEditar_Click(object? sender, EventArgs e)
    {
        if (_lstPersonas.SelectedValue is not { } selected) return;

        var id = (int)((dynamic)selected).Id;
        var actual = _personaService.GetAll().FirstOrDefault(p => p.Id == id);
        if (actual is null) return;

        try
        {
            _personaService.Update(new Persona
            {
                Id = actual.Id,
                Nombre = _txtNombre.Text.Trim(),
                Apellido = _txtApellido.Text.Trim(),
                Edad = (int)_numEdad.Value,
                Departamento = string.IsNullOrWhiteSpace(_txtDepartamento.Text) ? null : _txtDepartamento.Text.Trim()
            });
            RefrescarLista();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnEliminar_Click(object? sender, EventArgs e)
    {
        if (_lstPersonas.SelectedValue is not { } selected) return;

        var id = (int)((dynamic)selected).Id;
        var actual = _personaService.GetAll().FirstOrDefault(p => p.Id == id);
        if (actual is null) return;

        var confirm = MessageBox.Show($"¿Eliminar a {actual.Nombre} {actual.Apellido}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        _personaService.Delete(id);
        RefrescarLista();
    }

    private void LimpiarCampos()
    {
        _txtNombre.Clear();
        _txtApellido.Clear();
        _numEdad.Value = 0;
        _txtDepartamento.Clear();
    }

    private void LstPersonas_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_lstPersonas.SelectedValue is not { } selected) return;
        var id = (int)((dynamic)selected).Id;
        var actual = _personaService.GetAll().FirstOrDefault(p => p.Id == id);
        if (actual is null) return;
        _txtNombre.Text = actual.Nombre;
        _txtApellido.Text = actual.Apellido;
        _numEdad.Value = actual.Edad;
        _txtDepartamento.Text = actual.Departamento ?? string.Empty;
    }
}
