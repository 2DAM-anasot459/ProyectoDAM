using InventarioActivos.ViewModels.Usuario;

namespace InventarioActivos.Usuario;

public partial class BusquedaActivos : ContentPage
{
	private readonly BusquedaActivosViewModel vm;
	public BusquedaActivos(BusquedaActivosViewModel viewModel)
	{
		InitializeComponent();
		vm  = viewModel;

		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (vm == null) return;

        try
        {
            await vm.CargarAsync();
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync("Error al cargar los activos: " + ex.Message);

            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Error", "No se pudo cargar los activos: " + ex.Message, "Aceptar");
        }


    }
}