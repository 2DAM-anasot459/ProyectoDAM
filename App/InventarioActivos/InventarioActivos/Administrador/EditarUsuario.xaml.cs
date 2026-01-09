using InventarioActivos.ViewModels.Administrador;
namespace InventarioActivos.Administrador;

public partial class EditarUsuario : ContentPage, IQueryAttributable
{
	private readonly EditarUsuarioViewModel vm;
	public EditarUsuario(EditarUsuarioViewModel viewModel)
	{
		InitializeComponent();

		vm = viewModel;
		BindingContext = viewModel;
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query == null) return;

		if (query.ContainsKey("IdUsuario"))
		{
			object obj = query["IdUsuario"];
			if (obj == null) return;

			int id = Convert.ToInt32(obj);
			vm.SetIdUsuario(id);
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
			if (Shell.Current != null)
				await Shell.Current.DisplayAlert("Error", "No se pudo cargar los usuarios: " + ex.Message, "Aceptar");
		}
	}




    private void AbrirRolTapped(object sender, EventArgs e)
	{
		if(PickerRoles == null) return;	
		PickerRoles.Focus();
	}
}