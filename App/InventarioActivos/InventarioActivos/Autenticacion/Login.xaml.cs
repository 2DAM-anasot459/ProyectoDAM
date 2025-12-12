namespace InventarioActivos.Autenticacion;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
    }

	private async void OnIniciarSesionClick(object sender, EventArgs e)
	{
		//Leer los valores de los campos de entrada
		string usuario = CampoUsuario.Text;
		if (usuario != null)
		{
			usuario = usuario.Trim();
        }
        string contrasena = CampoContrasena.Text;
		if (contrasena != null) {
			contrasena = contrasena.Trim();
        }

        //Validar que los campos no esten vacios
        if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
		{
			await DisplayAlert("Error", "Por favor, ingrese su usuario y contraseña.", "Aceptar");
			return;
        }

        //Logica de autenticacion
        //Administrador
        if (usuario == "admin" && contrasena == "admin123")
		{
			//Navegar a la pagina principal del administrador
			if (Application.Current != null)
			{
				Application.Current.Windows[0].Page = new NavigationPage(new Administrador.AdministradorHome());

			}
			return;
		}
        //Tecnico
		if (usuario == "tecnico" && contrasena == "tec123")
		{
			
			if (Application.Current != null)
			{
				Application.Current.Windows[0].Page = new NavigationPage(new Usuario.PanelPrincipal());
			}
			return;
		}
        //Usuario invalido
		await DisplayAlert("Error", "Usuario o contraseña incorrectos.", "Aceptar");


    }

    //Metodo para cuando el usuario ha olvidado su contraseña
	private async void OnOlvidoContrasenaTapped(object sender, TappedEventArgs e)
	{
		await Navigation.PushAsync(new Autenticacion.CambioContrasena());
    }
}