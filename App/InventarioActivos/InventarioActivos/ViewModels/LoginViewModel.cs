using CommunityToolkit.Mvvm.Input;
using InventarioActivos.Services;

namespace InventarioActivos.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
	private AuthService authService;
	private NavigationService navigationService;
	public LoginViewModel(AuthService auth, NavigationService nav)
	{ 
		authService = auth;
		navigationService = nav;
		Title = "Iniciar Sesión";
    }

	public string Usuario { get; set; } = "";
	public string Contrasena { get; set; } = "";

	[RelayCommand]
	private async Task IniciarSesionAsync()
	{
		string usuario = Usuario;
		if (usuario == null) usuario = "";
		usuario = usuario.Trim();

		string contrasena = Contrasena;
		if (contrasena == null) contrasena = "";
		contrasena = contrasena.Trim();

		if(usuario == "" || contrasena == "")
		{
			await Shell.Current.DisplayAlert("Error", "Por favor, ingrese su usuario y contraseña.", "Aceptar");
			return;
        }

		string rol = authService.Login(usuario, contrasena);
		if(rol == "Admin")
		{
			navigationService.IrAHomeAdmin();
			return;
        }

		if(rol == "Tecnico")
		{
			navigationService.IrAHomeTecnico();
			return;
        }

		await Shell.Current.DisplayAlert("Error", "Usuario o contraseña incorrectos.", "Aceptar");
    }

	[RelayCommand]
	private async Task OlvidoContrasenaAsync()
	{
		await Shell.Current.Navigation.PushAsync(new Autenticacion.CambioContrasena());
    }


}