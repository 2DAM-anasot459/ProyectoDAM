using System.Collections.ObjectModel;
using System.Windows.Input;
using InventarioActivos.Models.GestionLocalizaciones;
using InventarioActivos.Services;

namespace InventarioActivos.ViewModels.Usuario;

public class MapaViewModel : BaseViewModel
{
	private readonly LocalizacionService localizacionService;

	public ObservableCollection<ActivoLocalizacionItem> Activos { get; }

	private ActivoLocalizacionItem activoSeleccionado;
	public ActivoLocalizacionItem ActivoSeleccionado
	{
		get { return activoSeleccionado; }
		set
		{
			activoSeleccionado = value;
			OnPropertyChanged();
			OnPropertyChanged("NombreActivoSeleccionado");
		}
	}

	public string NombreActivoSeleccionado
	{
		get
		{
			if (activoSeleccionado == null) return "";
			return activoSeleccionado.NombreEquipo;
		}
	}

	public ICommand VerActivoCommand {  get; }
	public ICommand CrearIncidenciaCommand { get; }

	public MapaViewModel(LocalizacionService service)
	{
		localizacionService = service;

		Title = "Mapa Interactivo";

		Activos = new ObservableCollection<ActivoLocalizacionItem>();

		VerActivoCommand = new Command(VerActivo);
		CrearIncidenciaCommand = new Command(CrearIncidencia);
	}

	public async Task CargarAsync()
	{
		Activos.Clear();

		List<ActivoLocalizacionItem> lista = await localizacionService.ObtenerActivosConLocalizacionAsync();
		for (int i = 0; i < lista.Count; i++)
		{
			Activos.Add(lista[i]);
		}

		if (lista.Count > 0)
		{
			ActivoSeleccionado = Activos[0];
		}
	}

	public async void VerActivo()
	{
		if(Shell.Current == null) return;
		if (ActivoSeleccionado == null) return;

		var parametros = new Dictionary<string, object>();
		parametros.Add("IdActivo", ActivoSeleccionado.IdActivo);

		await Shell.Current.GoToAsync("tec/fichaActivo", parametros);
	}

	public async void CrearIncidencia()
	{
        if (Shell.Current == null) return;
        if (ActivoSeleccionado == null) return;

        var parametros = new Dictionary<string, object>();
        parametros.Add("IdActivo", ActivoSeleccionado.IdActivo);

        await Shell.Current.GoToAsync("tec/nuevaIncidencia", parametros);
    }
	
}