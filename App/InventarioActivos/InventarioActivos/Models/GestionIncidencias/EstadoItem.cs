namespace InventarioActivos.Models.GestionIncidencias;

public class EstadoItem
{
	public int IdEstado { get; set; }
	public string Tipo { get; set; }

	public EstadoItem()
	{
		Tipo = "";
    }
}