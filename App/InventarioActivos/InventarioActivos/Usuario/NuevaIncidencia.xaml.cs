using InventarioActivos.ViewModels.Usuario;

namespace InventarioActivos.Usuario;

public partial class NuevaIncidencia : ContentPage, IQueryAttributable
{
    private readonly NuevaIncidenciaViewModel vm;
    public NuevaIncidencia(NuevaIncidenciaViewModel viewModel)
	{
		InitializeComponent();
        vm = viewModel;
        BindingContext = viewModel;
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
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Error", "No se pudo cargar: " + ex.Message, "Aceptar");
        }
    }

    private void AbirActivoTapped(object sender, EventArgs e)
    {
        if (PickerActivos != null) PickerActivos.Focus();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> attributes)
    {
        if(attributes == null) return;

        if (attributes.ContainsKey("IdActivo"))
        {
            object obj = attributes["IdActivo"];
            if (obj == null) return ;

            int id = Convert.ToInt32(obj);  
            vm.SetActivoInicial(id);
        }
    }

}