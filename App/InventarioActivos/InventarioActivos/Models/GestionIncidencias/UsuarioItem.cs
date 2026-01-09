namespace InventarioActivos.Models.GestionIncidencias;

public class UsuarioItem
{
	public int IdUsuario { get; set; }
	public string NombreUsuario { get; set; }

	public UsuarioItem()
	{
		NombreUsuario = "";
	}
}