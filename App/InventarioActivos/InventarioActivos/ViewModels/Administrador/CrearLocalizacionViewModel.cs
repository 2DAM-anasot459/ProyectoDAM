using System.Collections.ObjectModel;
using System.Windows.Input;
using InventarioActivos.Models.GestionLocalizaciones;
using InventarioActivos.Services;
namespace InventarioActivos.ViewModels.Administrador;

public class CrearLocalizacionViewModel : BaseViewModel
{
	private readonly LocalizacionService localizacionService;

	public ObservableCollection<ActivoLocalizacionItem> ActivosSinLocalizacion { get; }

	private ActivoLocalizacionItem activoSeleccionado;
	public ActivoLocalizacionItem ActivoSeleccionado
	{
		get { return activoSeleccionado; }
		set
		{
			activoSeleccionado = value;
			OnPropertyChanged();
			OnPropertyChanged("TextoActivoSeleccionado");
		}
	}

	private int idActivoInicial;

	private double latitud;
	private double longitud;

	public string TextoActivoSeleccionado
	{
		get
		{
			if (activoSeleccionado == null) return "Selecciona un activo o vuelve atrás.";
			return "Activo seleccionado: " + activoSeleccionado.NombreEquipo;
		}
	}

	public ICommand GuardarCommand { get; }
	public ICommand VolverCommand { get; }

	public CrearLocalizacionViewModel(LocalizacionService service)
	{
		localizacionService = service;

		ActivosSinLocalizacion = new ObservableCollection<ActivoLocalizacionItem>();

		latitud = -1;
		longitud = -1;
		idActivoInicial = 0;

		GuardarCommand = new Command(Guardar);
		VolverCommand = new Command(Volver);
	}

	public void SetActivoInicial(int idActivo)
	{
		idActivoInicial = idActivo;
	}

	public void SetCoordenadas(double lat, double lon)
	{
		latitud = lat;
		longitud = lon;
	}

	public async Task CargarAsync()
	{
		ActivosSinLocalizacion.Clear();

		List<ActivoLocalizacionItem> lista = await localizacionService.ObtenerActivosSinLocalizacionAsync();

		for (int i = 0; i < lista.Count; i++)
		{
			ActivosSinLocalizacion.Add(lista[i]);
		}

		if (idActivoInicial > 0)
		{
			ActivoLocalizacionItem encontrado = null;

			for (int i = 0; i < ActivosSinLocalizacion.Count; i++)
			{
				if (ActivosSinLocalizacion[i].IdActivo == idActivoInicial)
				{
					encontrado = ActivosSinLocalizacion[i];
					break;
				}
			}

			if (encontrado != null)
			{
				ActivoSeleccionado = encontrado;
			}
		}

		if (ActivoSeleccionado == null)
		{
			if (ActivosSinLocalizacion.Count > 0)
			{
				ActivoSeleccionado = ActivosSinLocalizacion[0];
			}
		}
	}

	private async void Guardar()
	{
		if (Shell.Current == null) return;

		if(ActivoSeleccionado == null)
		{
			await Shell.Current.DisplayAlert("Error", "Seleccione un activo", "Aceptar");
			return;
		}

		if(latitud < 0 || longitud < 0)
		{
			await Shell.Current.DisplayAlert("Error", "Toca el mapa para colocar la chincheta.", "Aceptar");
			return;
		}

		string nombre = await Shell.Current.DisplayPromptAsync(
			"Nombre de la localización",
			"Escribe el nombre de la localización:",
			"Aceptar",
			"Cancelar");
		if (nombre == null) return;
		

		nombre = nombre.Trim();
		if (nombre.Length == 0)
		{
			await Shell.Current.DisplayAlert("Error", "El nombre no puede estar vacío.", "Aceptar");
			return;

		}

		bool confirmar = await Shell.Current.DisplayAlert(
			"Confirmar",
			"¿Asignar '" + nombre + "' al activo '" + ActivoSeleccionado.NombreEquipo + "'?",
			"Sí",
			"No");

		if (!confirmar) return;

		try
		{
			bool ok = await localizacionService.CrearLocalizacionYAsignarAsync(
				ActivoSeleccionado.IdActivo,
				nombre,
				latitud,
				longitud);
			if (!ok)
			{
				await Shell.Current.DisplayAlert("Error", "No se ha podido guardar la localización.", "Aceptar");
				return;
			}

			await Shell.Current.DisplayAlert("Éxito", "Localización asignada correctamente", "Aceptar");
			await Shell.Current.GoToAsync("..");
		}
		catch (Exception ex)
		{
			await Shell.Current.DisplayAlert("Error", "Ocurrió un error: " + ex.Message, "Aceptar");
		}
	}

	private async void Volver()
	{
		if(Shell.Current == null) return;
        await Shell.Current.GoToAsync("..");
    }
	
}