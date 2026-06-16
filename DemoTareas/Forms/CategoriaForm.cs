using DemoTareas.Services;

namespace DemoTareas.Forms;

public class CategoriaForm : Form
{
    private readonly ICategoriaService _categoriaService;

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
        RefrescarLista();
    }

    private void InitializeComponent()
    {
        Text = "Gestionar categorías";
        Size = new Size(360, 380);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        var lblNombre = new Label { Text = "Nombre:", Location = new Point(12, 12), AutoSize = true };
        _txtNombre = new TextBox { Location = new Point(12, 30), Width = 240, MaxLength = 100 };

        _btnAnadir = new Button { Text = "Añadir", Location = new Point(262, 29), Width = 70 };
        _btnAnadir.Click += BtnAnadir_Click;

        _lstCategorias = new ListBox
        {
            Location = new Point(12, 68),
            Width = 320,
            Height = 210,
            DisplayMember = "Nombre"
        };

        _btnEditar = new Button { Text = "Editar", Location = new Point(12, 295), Width = 80 };
        _btnEditar.Click += BtnEditar_Click;

        _btnEliminar = new Button { Text = "Eliminar", Location = new Point(102, 295), Width = 80 };
        _btnEliminar.Click += BtnEliminar_Click;

        _btnCerrar = new Button
        {
            Text = "Cerrar",
            Location = new Point(252, 295),
            Width = 80,
            DialogResult = DialogResult.Cancel
        };

        Controls.AddRange(new Control[]
        {
            lblNombre, _txtNombre, _btnAnadir,
            _lstCategorias,
            _btnEditar, _btnEliminar, _btnCerrar
        });

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

        var nuevoNombre = Microsoft.VisualBasic.Interaction.InputBox(
            "Nuevo nombre:", "Editar categoría", seleccionada.Nombre);

        if (string.IsNullOrWhiteSpace(nuevoNombre)) return;

        try
        {
            _categoriaService.Update(seleccionada.Id, nuevoNombre);
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
