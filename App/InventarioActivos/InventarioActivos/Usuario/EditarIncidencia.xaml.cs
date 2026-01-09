using InventarioActivos.ViewModels.Administrador;

namespace InventarioActivos.Usuario;

public partial class EditarIncidencia : ContentPage, IQueryAttributable
{
    private readonly EditarIncidenciaViewModel vm;
    public EditarIncidencia(EditarIncidenciaViewModel viewModel)
    {
        InitializeComponent();
        vm = viewModel;
        BindingContext = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query == null) return;

        if (query.ContainsKey("IdIncidencia"))
        {
            object objId = query["IdIncidencia"];
            if (objId != null)
            {
                int id = Convert.ToInt32(objId);
                vm.SetIdIncidencia(id);
            }
        }

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await vm.CargarAsync();
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync("Error al cargar la incidencia: " + ex.Message);

            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Error", "No se pudo cargar la incidencia: " + ex.Message, "Aceptar");
        }


    }

    private void AbrirActivoTapped(object sender, EventArgs e)
    {

        if (PickerActivos != null)
        {
            PickerActivos.Focus();
        }
    }

    private void AbrirEstadoTapped(object sender, EventArgs e)
    {
        if (PickerEstados != null)
        {
            PickerEstados.Focus();
        }
    }

}