namespace InventarioActivos.Models.Auth;

public class LoginResult
{
	public bool Ok { get; set; }
	public int IdUsuario { get; set; }
	public string Rol { get; set; }

	public LoginResult()
	{
		Ok = false;
		IdUsuario = 0;
		Rol = "";
    }
}