namespace InventarioActivos.Models.GestionLocalizaciones;

public class LocalizacionItem 
{
	public int IdLocalizacion {  get; set; }
	public string Nombre { get; set; }
	public double Latitud { get; set; }
	public double Longitud { get; set; }

	public LocalizacionItem() 
	{
		Nombre = "";
		Latitud = 0;
		Longitud = 0;
	}
}