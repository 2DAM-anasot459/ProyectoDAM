namespace InventarioActivos.Services;

public class AuthService : ContentPage
{
	public string Login(string usuario, string contrasena)
	{
		if (usuario == null) usuario = "";
		if (contrasena == null) contrasena = "";

		usuario = usuario.Trim();
		contrasena = contrasena.Trim();

		if(usuario == "admin" && contrasena == "admin123") return "Administrador";
		if(usuario == "tecnico" && contrasena == "tec123") return "Tecnico";

		return "";
    }
}