using InventarioActivos.Models.GestionIncidencias;
using InventarioActivos.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Storage;

namespace InventarioActivos.ViewModels.Usuario;

public class PanelIncidenciasViewModel : BaseViewModel
{

    private readonly IncidenciaService incidenciaService;

    public ObservableCollection<EstadoIncidencias> Incidencias { get; }

    private List<EstadoIncidencias> todasLasIncidencias;

    private EstadoIncidencias incidenciaSeleccionada;
    public EstadoIncidencias IncidenciaSeleccionada
    {
        get { return incidenciaSeleccionada; }
        set
        {
            incidenciaSeleccionada = value; OnPropertyChanged();
        }
    }

    private int idTecnico;

    //Control visual del fitro
    private bool filtroActivo;
    public bool FiltroActivo
    {
        get { return filtroActivo; }
        set
        {
            filtroActivo = value; OnPropertyChanged();

        }
    }

    private bool filtroEstadoActivo;
    public bool FiltroEstadoActivo
    {
        get { return filtroEstadoActivo; }
        set
        {
            filtroEstadoActivo = value; OnPropertyChanged();
        }
    }

    private bool filtroActivosActivo;
    public bool FiltroActivosActivo
    {
        get { return filtroActivosActivo; }
        set
        {
            filtroActivosActivo = value; OnPropertyChanged();
        }
    }

    public ObservableCollection<string> EstadosDiponibles { get; }
    private string estadoSeleccionado;
    public string EstadoSeleccionado
    {
        get { return estadoSeleccionado; }
        set
        {
            estadoSeleccionado = value; OnPropertyChanged();
        }
    }

    private string textoActivo;
    public string TextoActivo
    {
        get { return textoActivo; }
        set
        {
            textoActivo = value; OnPropertyChanged();
        }
    }

    public ICommand CrearCommand { get; }
    public ICommand EditarCommand { get; }

    public ICommand ToggleFiltroCommand { get; }
    public ICommand AplicarFiltroCommand { get; }
    public ICommand AbrirEstadoCommand { get; }
    public ICommand AbrirActivoCommand { get; }
    public ICommand LimpiarFiltrosCommand { get; }

    public ICommand CerrarFiltroCommand { get; }
    public ICommand SeleccionarEstadoCommand { get; }
    public ICommand BuscarActivoCommand { get; }

    public PanelIncidenciasViewModel(IncidenciaService service)
    {
        incidenciaService = service;

        Title = "Panel de Incidencias";
        Incidencias = new ObservableCollection<EstadoIncidencias>();
        todasLasIncidencias = new List<EstadoIncidencias>();

        EstadosDiponibles = new ObservableCollection<string>();

        FiltroActivo = false;
        FiltroEstadoActivo = false;
        FiltroActivosActivo = false;

        EstadoSeleccionado = "";
        TextoActivo = "";

        idTecnico = 0;

        CrearCommand = new Command(IrCrear);
        EditarCommand = new Command(IrEditar);

        ToggleFiltroCommand = new Command(ToggleFiltro);
        AbrirEstadoCommand = new Command(AbrirEstado);
        AbrirActivoCommand = new Command(AbrirActivo);
        AplicarFiltroCommand = new Command(AplicarFiltro);
        LimpiarFiltrosCommand = new Command(LimpiarFiltros);

        CerrarFiltroCommand = new Command(CerrarFiltro);
        SeleccionarEstadoCommand = new Command(SeleccionarEstado);
        BuscarActivoCommand = new Command(BuscarActivo);


    }

    public void SetTecnico(int idUsuario)
    {
        idTecnico = idUsuario;
    }

    public async Task CargarIncidencias()
    {
        try
        {

            if(idTecnico <= 0)
            {
                idTecnico = Preferences.Get("ID_USUARIO_LOGEADO", 0);
            }

            if(idTecnico <= 0)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Error", "No se ha encontrado al técnico logeado", "Aceptar");
                }
                return;
            }

            var lista = await incidenciaService.ObtenerIncidenciasAsignadasAsync(idTecnico);

            todasLasIncidencias.Clear();
            todasLasIncidencias.AddRange(lista);

            AplicarFiltro();
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync("Error al cargar las incidencias del técnico: " + ex.Message);

            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Error", "No se pudieron cargar las incidencias: " + ex.Message, "Aceptar");
        }

    }

    private async void IrCrear()
    {
        if (Shell.Current == null) return;
        await Shell.Current.GoToAsync("tec/nuevaIncidencia");
    }

    private async void IrEditar()
    {
        if (Shell.Current == null) return;
        if (IncidenciaSeleccionada == null)
        {
            await Shell.Current.DisplayAlert("Error", "Debe seleccionar una incidencia para editarla.", "Aceptar");
            return;
        }
        int id = IncidenciaSeleccionada.IdIncidencia;
        Dictionary<string, object> parametros = new Dictionary<string, object>();
        parametros.Add("IdIncidencia", id);
        await Shell.Current.GoToAsync("tec/editarIncidencia", parametros);
    }


    private void ToggleFiltro()
    {
        FiltroActivo = !FiltroActivo;
        if (!FiltroActivo)
        {
            FiltroEstadoActivo = false;
            FiltroActivosActivo = false;
        }
    }

    private void AbrirEstado()
    {
        FiltroEstadoActivo = true;
        FiltroActivosActivo = false;
    }

    private void AbrirActivo()
    {
        FiltroEstadoActivo = false;
        FiltroActivosActivo = true;
    }


    private void LimpiarFiltros()
    {
        EstadoSeleccionado = "";
        TextoActivo = "";

        FiltroActivo = false;
        FiltroEstadoActivo = false;
        FiltroActivosActivo = false;

        AplicarFiltro();
    }


    private void CerrarFiltro()
    {
        FiltroActivo = false;
        FiltroEstadoActivo = false;
        FiltroActivosActivo = false;
    }


    private void SeleccionarEstado(object parametro)
    {
        if (parametro == null) return;
        string texto = parametro.ToString();
        if (texto == null) texto = "";
        texto = texto.Trim();

        EstadoSeleccionado = texto;

        FiltroEstadoActivo = false;
        FiltroActivo = false;

        AplicarFiltro();
    }

    private void BuscarActivo()
    {
        EstadoSeleccionado = "";

        FiltroActivo = false;
        FiltroEstadoActivo = false;
        FiltroActivosActivo = false;

        AplicarFiltro();
    }
    private void AplicarFiltro()
    {
        Incidencias.Clear();

        if (todasLasIncidencias == null) return;

        string estado = EstadoSeleccionado;
        if (estado == null) estado = "";
        estado = estado.Trim();

        string texto = TextoActivo;
        if (texto == null) texto = "";
        texto = texto.Trim().ToLower();
        texto = texto.ToLower();

        foreach (var incidencia in todasLasIncidencias)
        {
            bool cumpleEstado = true;
            bool cumpleActivo = true;
            if (estado.Length > 0)
            {
                string estadoIncidencia = incidencia.Estado.ToString();

                if (estadoIncidencia != estado)
                {
                    cumpleEstado = false;
                }
            }
            if (texto.Length > 0)
            {
                string nombreActivo = incidencia.ActivoNombre.ToLower();
                if (nombreActivo == null) nombreActivo = "";
                nombreActivo = nombreActivo.ToLower();

                if (nombreActivo.Contains(texto) == false)
                {
                    cumpleActivo = false;
                }

            }
            if (cumpleEstado && cumpleActivo)
            {
                Incidencias.Add(incidencia);
            }
        }

    }
}