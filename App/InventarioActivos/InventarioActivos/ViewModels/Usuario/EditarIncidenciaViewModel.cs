using InventarioActivos.Models.GestionIncidencias;
using System.Windows.Input;
using InventarioActivos.Services;
using System.Collections.ObjectModel;
namespace InventarioActivos.ViewModels.Usuario;

public class EditarIncidenciaViewModel : BaseViewModel
{
    private readonly IncidenciaService incidenciaService;

    private int idIncidencia;
    private int idTecnico;
    private int idUsuarioAsignadoOrigen;

    public ObservableCollection<ActivoItem> Activos { get; }
    public ObservableCollection<EstadoItem> Estados { get; }

    private string titulo;
    public string Titulo
    {
        get { return titulo; }
        set
        {
            titulo = value; OnPropertyChanged();
        }
    }

    private string descripcion;
    public string Descripcion
    {
        get { return descripcion; }
        set
        {
            descripcion = value; OnPropertyChanged();
        }
    }

    private ActivoItem activoSeleccionado;
    public ActivoItem ActivoSeleccionado
    {
        get { return activoSeleccionado; }
        set
        {
            activoSeleccionado = value; OnPropertyChanged();
        }
    }

    private EstadoItem estadoSeleccionado;
    public EstadoItem EstadoSeleccionado
    {
        get { return estadoSeleccionado; }
        set
        {
            estadoSeleccionado = value; OnPropertyChanged();
        }
    }

    private DateTime fechaCreacion;
    public DateTime FechaCreacion
    {
        get { return fechaCreacion; }
        set
        {
            fechaCreacion = value; OnPropertyChanged();
        }
    }

    private DateTime fechaFinalizacion;
    public DateTime FechaFinalizacion
    {
        get { return fechaFinalizacion; }
        set
        {
            fechaFinalizacion = value; OnPropertyChanged();
        }
    }


    public ICommand GuardarCommand { get; }
    public ICommand VolverCommand { get; }

    public EditarIncidenciaViewModel(IncidenciaService service)
    {
        incidenciaService = service;

        Title = "Panel de Incidencia";

        Activos = new ObservableCollection<ActivoItem>();
        Estados = new ObservableCollection<EstadoItem>();

        titulo = "";
        descripcion = "";
        fechaCreacion = DateTime.Today;
        fechaFinalizacion= DateTime.MinValue;

        idTecnico = 0;
        idUsuarioAsignadoOrigen = 0;


        GuardarCommand = new Command(GuardarIncidencia);
        VolverCommand = new Command(Volver);
    }

    public void SetIdIncidencia(int id)
    {
        idIncidencia = id;
    }

    public async Task CargarAsync()
    {
        Activos.Clear();
        Estados.Clear();

        if(Shell.Current == null) return;

        idTecnico = Preferences.Get("ID_USUARIO_LOGEADO", 0);
        if (idTecnico <= 0)
        {
            await Shell.Current.DisplayAlert("Error", "No se ha encontrado al técnico logeado.", "Aceptar");
            return;
        }


        List<ActivoItem> listaActivos = await incidenciaService.ObtenerActivosAsync();
        for (int i = 0; i < listaActivos.Count; i++)
        {
            Activos.Add(listaActivos[i]);
        }

        List<EstadoItem> listaEstados = await incidenciaService.ObtenerEstadosAsync();
        for (int i = 0; i < listaEstados.Count; i++)
        {
            Estados.Add(listaEstados[i]);
        }

        EstadoIncidencias ei = await incidenciaService.ObtenerIncidenciaPorIdAsync(idIncidencia);
        if (ei == null)
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Error", "No se encontró la incidencia especificada.", "Aceptar");
            return;
        }

        idUsuarioAsignadoOrigen = ei.IdeUsuarioAsignado;
        if(idUsuarioAsignadoOrigen != idTecnico)
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Error", "Esta incidencia no está asignada a este usuario", "Aceptar");
            return;
        }


        Titulo = ei.Titulo;
        Descripcion = ei.Descripcion;
        FechaCreacion = ei.FechaCreacion;
        FechaFinalizacion = ei.FechaFinalizacion;

        int idActivo = await ObtenerIdActivoDeIncidencia(idIncidencia);
        int idEstado = await ObtenerIdEstadoDeIncidencia(idIncidencia);

        ActivoSeleccionado = BuscarActivoPorId(idActivo);
        EstadoSeleccionado = BuscarEstadoPorId(idEstado);
       
    }

    private ActivoItem BuscarActivoPorId(int idActivo)
    {
        ActivoItem activoItem = null;

        for (int i = 0; i < Activos.Count; i++)
        {
            if (Activos[i].IdActivo == idActivo)
            {
                activoItem = Activos[i];
                break;
            }
        }

        if (activoItem == null)
        {
            activoItem = new ActivoItem();
        }
        return activoItem;
    }

    private EstadoItem BuscarEstadoPorId(int idEstado)
    {

        EstadoItem estadoItem = null;
        for (int i = 0; i < Estados.Count; i++)
        {
            if (Estados[i].IdEstado == idEstado)
            {
                estadoItem = Estados[i];
                break;
            }
        }

        if (estadoItem == null)
        {
            estadoItem = new EstadoItem();
        }
        return estadoItem;
    }


    private async Task<int> ObtenerIdActivoDeIncidencia(int idActivo)
    {
        int valor = 0;

        try
        {
            using (MySqlConnector.MySqlConnection connection = Data.DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string sql = "SELECT ID_ACTIVO " +
                    "FROM INCIDENCIA " +
                    "WHERE NUMERO_INCIDENCIA = @Id;";
                using (MySqlConnector.MySqlCommand command = new MySqlConnector.MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", idActivo);
                    object result = await command.ExecuteScalarAsync();
                    if (result != null)
                    {
                        valor = Convert.ToInt32(result);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync("Error al obtener el ID del activo de la incidencia: " + ex.Message);
            valor = 0;
        }
        return valor;
    }

    private async Task<int> ObtenerIdEstadoDeIncidencia(int idEstado)
    {
        int valor = 0;
        try
        {
            using (MySqlConnector.MySqlConnection connection = Data.DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string sql = "SELECT ID_ESTADO " +
                    "FROM INCIDENCIA " +
                    "WHERE NUMERO_INCIDENCIA = @Id;";
                using (MySqlConnector.MySqlCommand command = new MySqlConnector.MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", idEstado);
                    object result = await command.ExecuteScalarAsync();
                    if (result != null)
                    {
                        valor = Convert.ToInt32(result);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync("Error al obtener el ID del estado de la incidencia: " + ex.Message);
            valor = 0;
        }
        return valor;
    }

    private async void GuardarIncidencia()
    {
        if (Shell.Current == null) return;

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

        if (EstadoSeleccionado == null)
        {
            await Shell.Current.DisplayAlert("Error", "Debe seleccionar un estado.", "Aceptar");
            return;
        }

       
        if (idTecnico <= 0)
        {
            await Shell.Current.DisplayAlert("Error", "No se ha encontrado al técnico logeado", "Aceptar");
            return;
        }

        bool esTerminada = false;       
        if (EstadoSeleccionado != null)
        {
            if (EstadoSeleccionado.Tipo == "Terminada")
            {
                esTerminada = true;
            }
        }
        DateTime fechaFinalizacionGuardar = DateTime.MinValue;
        if (esTerminada)
        {
            if(FechaFinalizacion != DateTime.MinValue)
            {
                fechaFinalizacionGuardar = fechaFinalizacion;
            }
            else
            {
                fechaFinalizacionGuardar = DateTime.Now;
            }
        }
        else
        {
            fechaFinalizacionGuardar = DateTime.MinValue;
        }
     


        try
        {
            bool exito = await incidenciaService.ActualizarIncidenciaAsync(
                idIncidencia,
                titulo,
                descripcion,
                ActivoSeleccionado.IdActivo,
                EstadoSeleccionado.IdEstado,
                idTecnico,
                FechaCreacion,
                fechaFinalizacionGuardar
            );

            if (!exito)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo actualizar la incidencia.", "Aceptar");
                return;
            }

            await Shell.Current.DisplayAlert("Éxito", "La incidencia ha sido actualizada correctamente.", "Aceptar");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Ocurrió un error al actualizar la incidencia: " + ex.Message, "Aceptar");
        }
    }

    private async void Volver()
    {
        if (Shell.Current == null) return;
        await Shell.Current.GoToAsync("..");
    }
}