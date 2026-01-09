using InventarioActivos.ViewModels.Administrador;
namespace InventarioActivos.Administrador;

public partial class NuevaIncidencia : ContentPage
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
		if (vm != null)
		{
			await vm.CargarAsync();
        }
    }

	private void AbirActivoTapped(object sender, EventArgs e)
	{
		if (PickerActivos != null) PickerActivos.Focus();
	}

    private void AbrirEstadoTapped(object sender, EventArgs e)
    {
        if (PickerEstado != null) PickerEstado.Focus();
    }

    private void AbirUsuarioTapped(object sender, EventArgs e)
    {
        if (PickerUsuarios != null) PickerUsuarios.Focus();
    }


}