using System.Collections.ObjectModel;
using System.Windows.Input;
using InventarioActivos.Models.GestionLocalizaciones;
using InventarioActivos.Services;
namespace InventarioActivos.ViewModels.Administrador;

public class EditarLocalizacionViewModel : BaseViewModel
{
	private readonly LocalizacionService localizacionService;

	public ObservableCollection<ActivoLocalizacionItem> ActivosConLocalizacion {  get; }

    private ActivoLocalizacionItem activoSeleccionado;
    public ActivoLocalizacionItem ActivoSeleccionado
    {
        get { return activoSeleccionado; }
        set
        {
            activoSeleccionado = value;
            OnPropertyChanged();
            OnPropertyChanged("TextoActivoSeleccionado");

            if(activoSeleccionado != null)
            {
                LatitudNueva = activoSeleccionado.Latitud;
                LongitudNueva = activoSeleccionado.Longitud;

                DispararCoordenadas(LatitudNueva, LongitudNueva);
            }
        }
    }

    private int idActivoInicial;

    public double LatitudNueva {  get; private set; }
    public double LongitudNueva { get; private set; }

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

    public event Action<double, double> CoordenadasCambiadas;

    public EditarLocalizacionViewModel (LocalizacionService service)
    {
        localizacionService = service;

        ActivosConLocalizacion = new ObservableCollection<ActivoLocalizacionItem>();

        LatitudNueva = -1;
        LongitudNueva = -1;
        idActivoInicial = 0;

        GuardarCommand = new Command(Guardar);
        VolverCommand = new Command(Volver);
    }

    public void SetActivoInicial(int idActivo)
    {
        idActivoInicial = idActivo;
    }

    public void SetCoordenadasNuevas(double lat, double lon)
    {
        LatitudNueva = lat;
        LongitudNueva = lon;
    }

    private void DispararCoordenadas(double lat, double lon)
    {
        if(CoordenadasCambiadas != null)
        {
            CoordenadasCambiadas(lat, lon);
        }
    }

    public async Task CargarAsync()
    {
        ActivosConLocalizacion.Clear();

        List<ActivoLocalizacionItem> lista = await localizacionService.ObtenerActivosConLocalizacionAsync();

        for (int i = 0; i < lista.Count; i++)
        {
            ActivosConLocalizacion.Add(lista[i]);
        }

        if (idActivoInicial > 0)
        {
            ActivoLocalizacionItem encontrado = null;

            for (int i = 0; i < ActivosConLocalizacion.Count; i++)
            {
                if (ActivosConLocalizacion[i].IdActivo == idActivoInicial)
                {
                    encontrado = ActivosConLocalizacion[i];
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
            if (ActivosConLocalizacion.Count > 0)
            {
                ActivoSeleccionado = ActivosConLocalizacion[0];
            }
        }
    }

    private async void Guardar()
    {
        if (Shell.Current == null) return;

        if (ActivoSeleccionado == null)
        {
            await Shell.Current.DisplayAlert("Error", "Seleccione un activo", "Aceptar");
            return;
        }

        if (LatitudNueva < 0 || LongitudNueva < 0)
        {
            await Shell.Current.DisplayAlert("Error", "Toca el mapa para colocar la nueva chincheta.", "Aceptar");
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
                LatitudNueva,
                LongitudNueva);
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
        if (Shell.Current == null) return;
        await Shell.Current.GoToAsync("..");
    }
}

