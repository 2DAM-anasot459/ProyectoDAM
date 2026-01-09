using System.Collections.ObjectModel;

namespace InventarioActivos.Models.DatosActivos;

public class ItemActivos
{
	public int IdActivo {  get; set; }
	public string NombreEquipo { get; set; }
	public string NombreLocalizacion { get; set; }
	public string UsuarioActual { get; set; }
	//Hardware
	public string CpuNombre { get; set; }
	public int CpuNumeroNucleos { get; set; }
	public string CpuFabricante { get; set; }
	public int RamCapacidad { get; set; }
	public int RanurasRam { get; set; }
	public string TipoRam { get; set; }
	public int RamNumeroRarunaras { get; set; }
	public string PlacaModelo { get; set; }
	public string PlacaVersion { get; set; }
	public string ModeloDisco { get; set; }
	public int CapacidadDisco { get; set; }
	public int DiscoDuroCapacidadTotal { get; set; }
	public string RedTipo {  get; set; }

	//Software
	public string SistemaOperativo { get; set; }
	public string VersionSO { get; set; }
	public DateTime SOUltimoArranque { get; set; }
	public string EstadoDefender { get; set; }

	//Programas
	public ObservableCollection<string> ProgramasInstalados { get; set; }
	public int TotalProgramas { get; set; }
		

	public ItemActivos()
	{
		NombreEquipo = "";
		NombreLocalizacion = "";
		UsuarioActual = "";

		CpuNombre = "";
		CpuNumeroNucleos = 0;
		CpuFabricante = "";
		RamCapacidad = 0;
        RanurasRam = 0;
		TipoRam = "";
        RamNumeroRarunaras= 0;
		PlacaModelo = "";
		PlacaVersion = "";
        ModeloDisco = "";
		CapacidadDisco = 0;
		DiscoDuroCapacidadTotal = 0;
		RedTipo = "";

        SistemaOperativo = "";
		VersionSO = "";
		SOUltimoArranque = DateTime.MinValue;
        EstadoDefender = "";

		ProgramasInstalados = new ObservableCollection<string>();
		TotalProgramas = 0;
	}
	
}