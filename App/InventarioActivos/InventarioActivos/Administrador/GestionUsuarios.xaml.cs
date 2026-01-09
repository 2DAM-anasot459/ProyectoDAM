using InventarioActivos.Models.GestionUsuarios;
using InventarioActivos.ViewModels.Administrador;
namespace InventarioActivos.Administrador;

public partial class GestionUsuarios : ContentPage
{
	private readonly GestionUsuariosViewModel vm;
	public GestionUsuarios(GestionUsuariosViewModel viewModel)
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
        }catch (Exception ex)
		{
			if (Shell.Current != null)
				await Shell.Current.DisplayAlert("Error", "No se pudo cargar los usuarios: " + ex.Message, "Aceptar");
        }

    }

	private void OnEditarUsuarioTapped(object sender, EventArgs e)
	{
		try
		{
			Image image = (Image)sender;
			UsuarioListadoItem usuario = (UsuarioListadoItem)image.BindingContext;
			vm.EditarUsuarioCommand.Execute(usuario);
		}
		catch (Exception ex)
		{
            if (Shell.Current != null)
                Shell.Current.DisplayAlert("Error", "No se pudo acceder al usuario: " + ex.Message, "Aceptar");
        }
	}



    private void OnEliminarUsuarioTapped(object sender, EventArgs e)
	{
		try
		{
			Image img = (Image)sender;
			UsuarioListadoItem usuario = (UsuarioListadoItem)img.BindingContext;
			vm.EliminarUsuarioCommand.Execute(usuario);
        }catch(Exception ex)
		{
			if (Shell.Current != null)
				Shell.Current.DisplayAlert("Error", "No se pudo eliminar el usuario: " + ex.Message, "Aceptar");
        }
    }
}