namespace InventarioActivos.Autenticacion;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
    }

	private async void OnIniciarSesionClicked(object sender, EventArgs e)
	{
		string usuario = CampoUsuario.Text;
		if (usuario == null) usuario = "";
		usuario = usuario.Trim();

		string contrasena = CampoContrasena.Text;
		if (contrasena == null) contrasena = "";
		contrasena = contrasena.Trim();

		if (usuario.Length == 0 || contrasena.Length == 0)
		{
			await DisplayAlert("Error", "Por favor, ingrese su usuario y contraseña.", "Aceptar");
			return;
		}

		if (usuario == "admin" && contrasena == "admin123")
		{
			await Shell.Current.GoToAsync("//PanelAdministrador");
			return;
		}


		if (usuario == "tecnico" && contrasena == "tec123")
		{
			await Shell.Current.GoToAsync("//PanelTecnico");
			return;
		}

		await DisplayAlert("Error", "Usuario o contraseña incorrectos.", "Aceptar");

	}

	private async void OnOlvidarContrasenaTapped(object sender, EventArgs e)
	{
		if (Shell.Current != null)
		{
			await Shell.Current.GoToAsync("auth/cambioContrasena");
		}
    }
} 