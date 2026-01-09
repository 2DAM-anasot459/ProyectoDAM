namespace InventarioActivos.Models.GestionUsuarios;

public class RolItem
{
	public int IdRol { get; set; }
	public string NombreRol { get; set; }

	public RolItem()
	{
		NombreRol = "";
    }
}