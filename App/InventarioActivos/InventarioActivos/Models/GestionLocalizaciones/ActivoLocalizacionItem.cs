namespace InventarioActivos.Models.GestionLocalizaciones;

public class ActivoLocalizacionItem 
{
    public int IdActivo {  get; set; }
    public string NombreEquipo { get; set; }

    public int IdLocalizacion { get; set; }
    public string NombreLocalizacion { get; set; }  

    public double Latitud { get; set; } 
    public double Longitud { get; set; }

    public ActivoLocalizacionItem() 
    {
        NombreEquipo = "";
        IdLocalizacion = 0;
        NombreLocalizacion = "";
    }
}