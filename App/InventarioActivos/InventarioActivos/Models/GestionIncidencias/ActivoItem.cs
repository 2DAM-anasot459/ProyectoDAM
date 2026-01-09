namespace InventarioActivos.Models.GestionIncidencias;

public class ActivoItem
{
	public int IdActivo { get; set; }
	public string NombreActivo { get; set; }

	public ActivoItem()
	{
		NombreActivo = "";
    }

}