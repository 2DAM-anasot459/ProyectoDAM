using InventarioActivos.Models.GestionIncidencias;
using InventarioActivos.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace InventarioActivos.ViewModels.Administrador;

public class GestionIncidenciasViewModel : BaseViewModel
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
	public ICommand EliminarCommand { get; }

	public ICommand ToggleFiltroCommand { get; }
    public ICommand AplicarFiltroCommand { get; }
	public ICommand AbrirEstadoCommand { get; }
	public ICommand AbrirActivoCommand { get; }
	public ICommand LimpiarFiltrosCommand { get; }

    public ICommand CerrarFiltroCommand { get; }
	public ICommand SeleccionarEstadoCommand { get; }
	public ICommand BuscarActivoCommand { get; }

    public GestionIncidenciasViewModel(IncidenciaService service)
	{
		incidenciaService = service;	

        Title = "Gestión de Incidencias";
		Incidencias = new ObservableCollection<EstadoIncidencias>();	

		todasLasIncidencias = new List<EstadoIncidencias>();

        FiltroActivo = false;
		FiltroEstadoActivo = false;
		FiltroActivosActivo = false;

		EstadoSeleccionado = "";
		TextoActivo = "";

		CrearCommand = new Command(IrCrear);
		EditarCommand = new Command(IrEditar);
		EliminarCommand = new Command(EliminarCommandExecute);

		ToggleFiltroCommand = new Command(ToggleFiltro);
		AbrirEstadoCommand = new Command(AbrirEstado);
		AbrirActivoCommand = new Command(AbrirActivo);
		AplicarFiltroCommand = new Command(AplicarFiltro);
		LimpiarFiltrosCommand = new Command(LimpiarFiltros);

        CerrarFiltroCommand = new Command(CerrarFiltro);
		SeleccionarEstadoCommand = new Command(SeleccionarEstado);
		BuscarActivoCommand = new Command(BuscarActivo);


    }

	public async Task CargarIncidencias()
	{
		try
		{
           
            var lista = await incidenciaService.ObtenerIncidenciaAsync();

			todasLasIncidencias.Clear();
			todasLasIncidencias.AddRange(lista);

            AplicarFiltro();
        }
        catch (Exception ex)
		{
			await Console.Out.WriteLineAsync("Error al cargar las incidencias: " + ex.Message);

			if (Shell.Current != null)
			await Shell.Current.DisplayAlert("Error", "No se pudieron cargar las incidencias: " +  ex.Message, "Aceptar");
        }

    }

    private async void IrCrear()
	{
		if (Shell.Current == null) return;
        await Shell.Current.GoToAsync("admin/nuevaIncidencia");
    }

	private async void IrEditar()
	{
		if(Shell.Current == null) return;
		if(IncidenciaSeleccionada == null)
		{
			await Shell.Current.DisplayAlert("Error", "Debe seleccionar una incidencia para editarla.", "Aceptar");
			return;
        }
		int id = IncidenciaSeleccionada.IdIncidencia;
		Dictionary<string, object> parametros = new Dictionary<string, object>();
		parametros.Add("IdIncidencia", id);
        await Shell.Current.GoToAsync("admin/editarIncidencia", parametros);
    }



	private async Task Eliminar()
	{
		if(Shell.Current == null) return;

		if(IncidenciaSeleccionada == null)
		{
			await Shell.Current.DisplayAlert("Error", "Debe seleccionar una incidencia para eliminarla.", "Aceptar");
			return;
		}
		bool confirmacion = await Shell.Current.DisplayAlert("Confirmar eliminación", "¿Está seguro de que desea eliminar la incidencia seleccionada?", "Sí", "No");
		if (!confirmacion) return;

		try
		{
			int idIncidencia = IncidenciaSeleccionada.IdIncidencia;
			bool eliminado = await incidenciaService.EliminarIncidenciaAsync(idIncidencia);

			if (!eliminado)
			{
				await Shell.Current.DisplayAlert("Error", "No se pudo eliminar la incidencia.", "Aceptar");
				return;
            }

			EstadoIncidencias borrar = null;
			foreach(EstadoIncidencias incidencia in todasLasIncidencias)
			{
				if(incidencia.IdIncidencia == idIncidencia)
				{
					borrar = incidencia;
					break;
                }
            }

			if(borrar != null)
			{
				todasLasIncidencias.Remove(borrar);
				await CargarIncidencias();
            }

            IncidenciaSeleccionada = null;

			await Shell.Current.DisplayAlert("Éxito", "La incidencia ha sido eliminada correctamente.", "Aceptar");

        }
        catch(Exception ex)
		{
			await Shell.Current.DisplayAlert("Error", "No se pudo eliminar la incidencia: " + ex.Message, "Aceptar");
			return;
        }

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

	private async void EliminarCommandExecute()
	{
		await Eliminar();
	}

	private void SeleccionarEstado(object parametro)
	{
		if (parametro == null) return;
		string texto = parametro.ToString();
		if(texto == null) texto = "";
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

		if(todasLasIncidencias == null) return;

        string estado = EstadoSeleccionado;

		if(estado == null) estado = "";
		estado = estado.Trim();

		string texto = TextoActivo;
		if(texto == null) texto = "";
		texto = texto.Trim().ToLower();
		texto = texto.ToLower();

		foreach (var incidencia in todasLasIncidencias)
		{
			bool cumpleEstado = true;
			bool cumpleActivo = true;
			if (estado.Length > 0)
			{
				string estadoIncidencia = incidencia.Estado.ToString();
		
				if(estadoIncidencia != estado)
				{
					cumpleEstado = false;
                }
            }
			if (texto.Length > 0)
			{
				string nombreActivo = incidencia.ActivoNombre.ToLower();
				if (nombreActivo == null) nombreActivo = "";
				nombreActivo = nombreActivo.ToLower();

				if(nombreActivo.Contains(texto) == false)
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