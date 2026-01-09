using InventarioActivos.Models.GestionIncidencias;
using InventarioActivos.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace InventarioActivos.ViewModels.Usuario;

public class NuevaIncidenciaViewModel : BaseViewModel
{
    private readonly IncidenciaService incidenciaService;

    public ObservableCollection<ActivoItem> Activos { get; }

    private int idActivoInicial;
    public void SetActivoInicial(int idActivol)
    {
        idActivoInicial = idActivol;
    }

    private string titulo;
    public string Titulo
    {
        get { return titulo; }
        set { titulo = value; OnPropertyChanged(); }
    }

    private string descripcion;
    public string Descripcion
    {
        get { return descripcion; }
        set { descripcion = value; OnPropertyChanged(); }
    }


    private ActivoItem activoSeleccionado;
    public ActivoItem ActivoSeleccionado
    {
        get { return activoSeleccionado; }
        set { activoSeleccionado = value; OnPropertyChanged(); }
    }


    private DateTime fechaCreacion;
    public DateTime FechaCreacion
    {
        get { return fechaCreacion; }
        set { fechaCreacion = value; OnPropertyChanged(); }
    }

    public ICommand CrearIncidenciaCommand { get; }
    public ICommand VolverCommand { get; }

    public NuevaIncidenciaViewModel(IncidenciaService service)
    {
        incidenciaService = service;

        Title = "Panel de Incidencias";

        Activos = new ObservableCollection<ActivoItem>();

        titulo = "";
        descripcion = "";
        fechaCreacion = DateTime.Now;

        CrearIncidenciaCommand = new Command(CrearIncidencia);
        VolverCommand = new Command(Volver);
    }


    public async Task CargarAsync()
    {
        Activos.Clear();
        List<ActivoItem> listaActivos = await incidenciaService.ObtenerActivosAsync();
        for (int i = 0; i < listaActivos.Count; i++)
        {
            Activos.Add(listaActivos[i]);
        }
        
        if(Activos.Count == 0) return;
        
        if(idActivoInicial > 0)
        {
            ActivoItem encontrado = null;
            for (int i = 0; i < Activos.Count;i++)
            {
                if (Activos[i].IdActivo == idActivoInicial)
                {
                    encontrado = Activos[i];
                    break;
                }
            }
            if (encontrado != null)
            {
                ActivoSeleccionado = encontrado;
                return;
            }
        }
        ActivoSeleccionado = Activos[0];
      
    }

    private async void CrearIncidencia()
    {
        // Lógica para crear la incidencia
        if (Shell.Current == null) return;

        int idTecnico = Preferences.Get("ID_USUARIO_LOGEADO", 0);
        if(idTecnico <= 0)
        {
            await Shell.Current.DisplayAlert("Error", "No se ha encontrado al técnico logeado.", "Aceptar");
            return;
        }

        string titulo = Titulo;
        if (titulo == null) titulo = "";
        titulo = titulo.Trim();

        string descripcion = Descripcion;
        if (descripcion == null) descripcion = "";
        descripcion = descripcion.Trim();

        if (titulo.Length == 0 || descripcion.Length == 0)
        {
            await Shell.Current.DisplayAlert("Error", "El título y la descripción son obligatorios.", "Aceptar");
            return;
        }

        if (ActivoSeleccionado == null)
        {
            await Shell.Current.DisplayAlert("Error", "Debe seleccionar un activo.", "Aceptar");
            return;
        }

        //Fecha de finalización. Si el estado es Terminado, se pone la fecha actual. Si no, null.
        DateTime fechaFinalizacion = DateTime.MinValue;


        try
        {
            int idEstadoPendiente = await incidenciaService.ObtenerIdEstadoPendienteAsync();
            if(idEstadoPendiente <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "No se ha encontrado el estado `Pendiente`", "Aceptar");
                return;
            }


            int nuevaIncidenciaId = await incidenciaService.CrearIncidenciaAsync(
                titulo,
                descripcion,
                FechaCreacion,
                fechaFinalizacion,
                ActivoSeleccionado.IdActivo,
                idEstadoPendiente,
                idTecnico
            );

            await Shell.Current.DisplayAlert("Éxito", $"Incidencia creada con ID: {nuevaIncidenciaId}", "Aceptar");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo crear la incidencia. {ex.Message}", "Aceptar");
        }



    }
    private async void Volver()
    {
        if (Shell.Current == null) return;
        await Shell.Current.GoToAsync("..");
    }
}