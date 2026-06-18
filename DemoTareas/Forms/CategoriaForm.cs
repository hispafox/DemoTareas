// Autor:   DemoTareas Team
// Fecha:   2026-06-18
// Versión: 1.0

using DemoTareas.Services;
using DemoTareas.Utilities;

namespace DemoTareas.Forms;

public class CategoriaForm : Form
{
    private readonly ICategoriaService _categoriaService;

    private Panel _header = null!;
    private ListBox _lstCategorias = null!;
    private TextBox _txtNombre = null!;
    private Button _btnAnadir = null!;
    private Button _btnEditar = null!;
    private Button _btnEliminar = null!;
    private Button _btnCerrar = null!;

    public CategoriaForm(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
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
        AppTheme.ApplyFlatButton(_btnAnadir, true);
        AppTheme.ApplyFlatButton(_btnEditar, false);
        AppTheme.ApplyFlatButton(_btnEliminar, false);
        AppTheme.ApplyFlatButton(_btnCerrar, false);
    }

    private void InitializeComponent()
    {
        Text = "Gestionar categorías";
        Size = new Size(380, 460);
        MinimumSize = new Size(380, 420);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = AppTheme.SurfaceColor;
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = AppTheme.FontSmall;

        _header = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = AppTheme.AccentColor };
        var lblHeaderTitle = new Label { Text = "Categorías", ForeColor = Color.White, Font = AppTheme.FontTitle, Location = new Point(18, 16), AutoSize = true };
        _header.Controls.Add(lblHeaderTitle);

        // Sección de alta: label + input + botón Añadir
        var addSection = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(16, 10, 16, 10),
            Height = 72,
            BackColor = AppTheme.SurfaceColor
        };
        addSection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        addSection.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 88f));
        addSection.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        addSection.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        addSection.Paint += (s, e) => e.Graphics.DrawLine(new Pen(AppTheme.BorderColor, 1), 0, addSection.Height - 1, addSection.Width, addSection.Height - 1);

        var lblNombre = new Label
        {
            Text = "NOMBRE",
            AutoSize = true,
            ForeColor = AppTheme.TextSecondary,
            Font = AppTheme.FontMeta,
            Margin = new Padding(0, 0, 0, 2)
        };
        _txtNombre = new TextBox
        {
            Dock = DockStyle.Fill,
            MaxLength = 100,
            Font = AppTheme.FontItem,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 0, 8, 0)
        };
        _btnAnadir = new Button
        {
            Text = "Añadir",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 0)
        };
        _btnAnadir.Click += BtnAnadir_Click;

        addSection.Controls.Add(lblNombre, 0, 0);
        addSection.SetColumnSpan(lblNombre, 2);
        addSection.Controls.Add(_txtNombre, 0, 1);
        addSection.Controls.Add(_btnAnadir, 1, 1);

        // Lista
        _lstCategorias = new ListBox
        {
            Dock = DockStyle.Fill,
            DisplayMember = "Nombre",
            Font = AppTheme.FontItem,
            BackColor = AppTheme.SurfaceColor,
            ForeColor = AppTheme.TextPrimary,
            BorderStyle = BorderStyle.None
        };

        // Footer con botones
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 52, BackColor = AppTheme.SurfaceColor };
        footer.Paint += (s, e) => e.Graphics.DrawLine(new Pen(AppTheme.BorderColor, 1), 0, 0, footer.Width, 0);

        _btnEditar = new Button { Text = "Editar", Width = 88, Height = 32, Margin = new Padding(0, 10, 8, 10) };
        _btnEditar.Click += BtnEditar_Click;

        _btnEliminar = new Button { Text = "Eliminar", Width = 88, Height = 32, Margin = new Padding(0, 10, 0, 10) };
        _btnEliminar.Click += BtnEliminar_Click;

        _btnCerrar = new Button
        {
            Text = "Cerrar",
            Width = 88,
            Height = 32,
            Margin = new Padding(0, 10, 8, 10),
            DialogResult = DialogResult.Cancel
        };

        var leftFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Left,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            Padding = new Padding(8, 0, 0, 0)
        };
        leftFlow.Controls.Add(_btnEditar);
        leftFlow.Controls.Add(_btnEliminar);

        var rightFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            AutoSize = true
        };
        rightFlow.Controls.Add(_btnCerrar);

        footer.Controls.Add(leftFlow);
        footer.Controls.Add(rightFlow);

        Controls.Add(_lstCategorias);
        Controls.Add(footer);
        Controls.Add(addSection);
        Controls.Add(_header);

        CancelButton = _btnCerrar;
    }

    private void RefrescarLista()
    {
        _lstCategorias.DataSource = null;
        _lstCategorias.DataSource = _categoriaService.GetAll();
        _lstCategorias.DisplayMember = "Nombre";
    }

    private void BtnAnadir_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtNombre.Text))
        {
            MessageBox.Show("Escribe un nombre para la categoría.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            _categoriaService.Add(_txtNombre.Text);
            _txtNombre.Clear();
            RefrescarLista();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnEditar_Click(object? sender, EventArgs e)
    {
        if (_lstCategorias.SelectedItem is not Models.Categoria seleccionada) return;

        using var editForm = new Form
        {
            Text = "Editar categoría",
            Size = new Size(340, 170),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            StartPosition = FormStartPosition.CenterParent,
            BackColor = AppTheme.SurfaceColor,
            Font = AppTheme.FontSmall
        };
        var lblEdit = new Label { Text = "NOMBRE", AutoSize = true, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontMeta, Location = new Point(16, 16) };
        var txt = new TextBox { Location = new Point(16, 34), Width = 296, MaxLength = 100, Text = seleccionada.Nombre, BorderStyle = BorderStyle.FixedSingle, Font = AppTheme.FontItem };
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 52, BackColor = AppTheme.SurfaceColor };
        footer.Paint += (s2, e2) => e2.Graphics.DrawLine(new Pen(AppTheme.BorderColor, 1), 0, 0, footer.Width, 0);
        var btnOk = new Button { Text = "Guardar", Width = 88, Height = 32, Margin = new Padding(8, 10, 8, 10), DialogResult = DialogResult.OK };
        var btnCancel = new Button { Text = "Cancelar", Width = 88, Height = 32, Margin = new Padding(0, 10, 0, 10), DialogResult = DialogResult.Cancel };
        AppTheme.ApplyFlatButton(btnOk, true);
        AppTheme.ApplyFlatButton(btnCancel, false);
        var btnFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, Padding = new Padding(0, 0, 8, 0) };
        btnFlow.Controls.Add(btnOk);
        btnFlow.Controls.Add(btnCancel);
        footer.Controls.Add(btnFlow);
        editForm.Controls.AddRange(new Control[] { lblEdit, txt, footer });
        editForm.AcceptButton = btnOk;
        editForm.CancelButton = btnCancel;

        if (editForm.ShowDialog(this) != DialogResult.OK || string.IsNullOrWhiteSpace(txt.Text)) return;

        try
        {
            _categoriaService.Update(seleccionada.Id, txt.Text.Trim());
            RefrescarLista();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnEliminar_Click(object? sender, EventArgs e)
    {
        if (_lstCategorias.SelectedItem is not Models.Categoria seleccionada) return;

        var confirm = MessageBox.Show(
            $"¿Eliminar la categoría '{seleccionada.Nombre}'?\nLas tareas asociadas quedarán sin categoría.",
            "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes) return;

        try
        {
            _categoriaService.Delete(seleccionada.Id);
            RefrescarLista();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
