using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioActivos.Services;
using Microsoft.Maui.Storage;

namespace InventarioActivos.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
	private AuthService authService;
	private NavigationService navigationService;

	[ObservableProperty]
	private string usuario;
	[ObservableProperty]
	private string contrasena;

    public LoginViewModel(AuthService auth, NavigationService nav)
	{ 
		this.authService = auth;
		this.navigationService = nav;

        Title = "Iniciar Sesión";
		usuario = "";
		contrasena = "";
    }



	[RelayCommand]
	private async Task IniciarSesionAsync()
	{
		string user = Usuario;
		string pass = Contrasena;

		if(user == null) user = "";
		if(pass == null) pass = "";

		user = user.Trim();
		pass = pass.Trim();

		if(user.Length == 0 || pass.Length == 0)
		{
			if(Shell.Current != null)
			{
				await Shell.Current.DisplayAlert("Error", "Por favor, ingrese su usuario y contraseña.", "Aceptar");
            }

			return;
        }

		var result = await authService.LoginAsync(user, pass);
		if(result.Ok == false)
		{
			if(Shell.Current != null)
			{
				await Shell.Current.DisplayAlert("Error", "Usuario o contraseña incorrectos.", "Aceptar");
			}
			return;
        }

		//Guarda el id del usuario que inicia sesión
		Preferences.Set("ID_USUARIO_LOGEADO", result.IdUsuario);
		String rolPref = result.Rol;
		if (rolPref == null) rolPref = "";
		rolPref = rolPref.Trim();
		Preferences.Set("ROL_USUARIO_LOGEADO", rolPref);


		//Según el tipo de usuario nos movemos a una pantalla u a otra
		if(rolPref == "Administrador")
		{
			await navigationService.IrAHomeAdmin();
			return;
        }

		if(rolPref == "Técnico" || rolPref == "Tecnico")
		{
			await navigationService.IrAHomeTecnico();
			return;
        }

		if(Shell.Current != null)
		{
			await Shell.Current.DisplayAlert("Error", "El usuario no tiene un rol asignado.", "Aceptar");
        }



    }

	[RelayCommand]
	private async Task OlvidoContrasenaAsync()
	{
		if(Shell.Current != null)
		{
			await Shell.Current.GoToAsync("auth/cambioContrasena");
        }
    }


}