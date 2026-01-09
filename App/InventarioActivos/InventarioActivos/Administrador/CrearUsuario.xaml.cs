using InventarioActivos.ViewModels.Administrador;
namespace InventarioActivos.Administrador;

public partial class CrearUsuario : ContentPage
{
	private readonly CrearUsuarioViewModel vm;
    public CrearUsuario(CrearUsuarioViewModel viewModel)
	{
		InitializeComponent();

		vm = viewModel;
		BindingContext = viewModel;
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await vm.CargarAsync();

    }

	private void AbrirRolTapped(object sender, EventArgs e)
	{
		if(PickerRoles != null)
			PickerRoles.Focus();
    }
}