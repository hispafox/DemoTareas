using DemoTareas.Models;
using DemoTareas.Services;
using DemoTareas.Utilities;

namespace DemoTareas.Forms;

public class PersonaForm : Form
{
    private sealed record PersonaItem(int Id, string NombreCompleto);

    private readonly IPersonaService _personaService;

    private Panel _header = null!;
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
        ApplyTheme();
        AppTheme.ThemeChanged += ApplyTheme;
        RefrescarLista();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        AppTheme.ThemeChanged -= ApplyTheme;
        base.OnFormClosed(e);
    }

    private void ApplyTheme()
    {
        _header.BackColor = AppTheme.AccentColor;
        AppTheme.ApplyFlatButton(_btnAgregar, true);
        AppTheme.ApplyFlatButton(_btnEditar, false);
        AppTheme.ApplyFlatButton(_btnEliminar, false);
        AppTheme.ApplyFlatButton(_btnCerrar, false);
    }

    private void InitializeComponent()
    {
        Text = "Gestionar personas";
        Size = new Size(440, 560);
        MinimumSize = new Size(440, 520);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = AppTheme.SurfaceColor;
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = AppTheme.FontSmall;

        _header = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = AppTheme.AccentColor };
        var lblHeaderTitle = new Label { Text = "Personas", ForeColor = Color.White, Font = AppTheme.FontTitle, Location = new Point(18, 16), AutoSize = true };
        _header.Controls.Add(lblHeaderTitle);

        // Sección de campos (TableLayoutPanel)
        var fields = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            RowCount = 5,
            Padding = new Padding(16, 10, 16, 8),
            BackColor = AppTheme.SurfaceColor
        };
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        for (int i = 0; i < 5; i++)
            fields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        fields.Paint += (s, e) => e.Graphics.DrawLine(new Pen(AppTheme.BorderColor, 1), 0, fields.Height - 1, fields.Width, fields.Height - 1);

        // Fila 0–1: Nombre | Apellido
        var lblNombre = new Label { Text = "NOMBRE", AutoSize = true, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontMeta, Margin = new Padding(0, 0, 0, 2) };
        _txtNombre = new TextBox { Dock = DockStyle.Fill, Font = AppTheme.FontItem, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(0, 0, 8, 8) };
        var lblApellido = new Label { Text = "APELLIDO", AutoSize = true, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontMeta, Margin = new Padding(8, 0, 0, 2) };
        _txtApellido = new TextBox { Dock = DockStyle.Fill, Font = AppTheme.FontItem, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(8, 0, 0, 8) };
        fields.Controls.Add(lblNombre, 0, 0);
        fields.Controls.Add(lblApellido, 1, 0);
        fields.Controls.Add(_txtNombre, 0, 1);
        fields.Controls.Add(_txtApellido, 1, 1);

        // Fila 2–3: Edad | Departamento
        var lblEdad = new Label { Text = "EDAD", AutoSize = true, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontMeta, Margin = new Padding(0, 0, 0, 2) };
        _numEdad = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 0, Maximum = 120, Font = AppTheme.FontItem, Margin = new Padding(0, 0, 8, 8) };
        var lblDepartamento = new Label { Text = "DEPARTAMENTO", AutoSize = true, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontMeta, Margin = new Padding(8, 0, 0, 2) };
        _txtDepartamento = new TextBox { Dock = DockStyle.Fill, Font = AppTheme.FontItem, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(8, 0, 0, 8) };
        fields.Controls.Add(lblEdad, 0, 2);
        fields.Controls.Add(lblDepartamento, 1, 2);
        fields.Controls.Add(_numEdad, 0, 3);
        fields.Controls.Add(_txtDepartamento, 1, 3);

        // Fila 4: botones de acción
        var actionFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Margin = new Padding(0, 4, 0, 4)
        };
        _btnAgregar = new Button { Text = "Agregar", Width = 88, Height = 32, Margin = new Padding(8, 0, 0, 0) };
        _btnAgregar.Click += BtnAgregar_Click;
        _btnEditar = new Button { Text = "Editar", Width = 88, Height = 32, Margin = new Padding(8, 0, 0, 0) };
        _btnEditar.Click += BtnEditar_Click;
        _btnEliminar = new Button { Text = "Eliminar", Width = 88, Height = 32, Margin = new Padding(0, 0, 0, 0) };
        _btnEliminar.Click += BtnEliminar_Click;
        actionFlow.Controls.Add(_btnAgregar);
        actionFlow.Controls.Add(_btnEditar);
        actionFlow.Controls.Add(_btnEliminar);
        fields.Controls.Add(actionFlow, 0, 4);
        fields.SetColumnSpan(actionFlow, 2);

        // Lista
        _lstPersonas = new ListBox
        {
            Dock = DockStyle.Fill,
            DisplayMember = "NombreCompleto",
            Font = AppTheme.FontItem,
            BackColor = AppTheme.SurfaceColor,
            ForeColor = AppTheme.TextPrimary,
            BorderStyle = BorderStyle.None
        };
        _lstPersonas.SelectedIndexChanged += LstPersonas_SelectedIndexChanged;

        // Footer
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 52, BackColor = AppTheme.SurfaceColor };
        footer.Paint += (s, e) => e.Graphics.DrawLine(new Pen(AppTheme.BorderColor, 1), 0, 0, footer.Width, 0);
        _btnCerrar = new Button
        {
            Text = "Cerrar",
            Width = 88,
            Height = 32,
            Margin = new Padding(0, 10, 8, 10),
            DialogResult = DialogResult.Cancel
        };
        var closeBtnFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            AutoSize = true
        };
        closeBtnFlow.Controls.Add(_btnCerrar);
        footer.Controls.Add(closeBtnFlow);

        Controls.Add(_lstPersonas);
        Controls.Add(footer);
        Controls.Add(fields);
        Controls.Add(_header);

        CancelButton = _btnCerrar;
    }

    private void RefrescarLista()
    {
        _lstPersonas.DataSource = null;
        _lstPersonas.DataSource = _personaService.GetAll()
            .Select(p => new PersonaItem(p.Id,
                $"{p.Nombre} {p.Apellido} — Edad: {p.Edad}" +
                (string.IsNullOrEmpty(p.Departamento) ? "" : $" — {p.Departamento}")))
            .ToList();
        _lstPersonas.DisplayMember = nameof(PersonaItem.NombreCompleto);
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
        if (_lstPersonas.SelectedItem is not PersonaItem item) return;

        var actual = _personaService.GetAll().FirstOrDefault(p => p.Id == item.Id);
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
        if (_lstPersonas.SelectedItem is not PersonaItem item) return;

        var actual = _personaService.GetAll().FirstOrDefault(p => p.Id == item.Id);
        if (actual is null) return;

        var confirm = MessageBox.Show($"¿Eliminar a {actual.Nombre} {actual.Apellido}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        _personaService.Delete(item.Id);
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
        if (_lstPersonas.SelectedItem is not PersonaItem item) return;
        var actual = _personaService.GetAll().FirstOrDefault(p => p.Id == item.Id);
        if (actual is null) return;
        _txtNombre.Text = actual.Nombre;
        _txtApellido.Text = actual.Apellido;
        _numEdad.Value = actual.Edad;
        _txtDepartamento.Text = actual.Departamento ?? string.Empty;
    }
}
