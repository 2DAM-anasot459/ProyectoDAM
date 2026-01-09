using InventarioActivos.Models.GestionIncidencias;
using InventarioActivos.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace InventarioActivos.ViewModels.Administrador;

public class NuevaIncidenciaViewModel : BaseViewModel
{
	private readonly IncidenciaService incidenciaService;

	public ObservableCollection<ActivoItem> Activos { get; }
    public ObservableCollection<EstadoItem> Estados { get; }
    public ObservableCollection<UsuarioItem> Usuarios { get; }


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

	private UsuarioItem usuarioSeleccionado;
	public UsuarioItem UsuarioSeleccionado
	{
		get { return usuarioSeleccionado; }
		set { usuarioSeleccionado = value; OnPropertyChanged(); }
    }

	private ActivoItem activoSeleccionado;
	public ActivoItem ActivoSeleccionado
	{
		get { return activoSeleccionado; }
		set { activoSeleccionado = value; OnPropertyChanged(); }
    }

	private EstadoItem estadoSeleccionado;
	public EstadoItem EstadoSeleccionado
	{
		get { return estadoSeleccionado; }
		set { estadoSeleccionado = value; OnPropertyChanged(); }
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

        Title = "Gestion de Incidencias";

		Activos = new ObservableCollection<ActivoItem>();
		Estados = new ObservableCollection<EstadoItem>();
		Usuarios = new ObservableCollection<UsuarioItem>();

        titulo = "";
		descripcion = "";		
		fechaCreacion = DateTime.Now;

		CrearIncidenciaCommand = new Command(CrearIncidencia);
		VolverCommand = new Command(Volver);
    }


	public async Task CargarAsync()
	{
		Activos.Clear();
		Estados.Clear();
		Usuarios.Clear();
		List<ActivoItem> listaActivos = await incidenciaService.ObtenerActivosAsync();
		for(int	i = 0; i < listaActivos.Count; i++)
		{
			Activos.Add(listaActivos[i]);
        }

		List<EstadoItem> listaEstados = await incidenciaService.ObtenerEstadosAsync();
		for(int i = 0; i < listaEstados.Count; i++)
		{
			Estados.Add(listaEstados[i]);
        }

		List<UsuarioItem> listaUsuarios = await incidenciaService.ObtenerUsuariosAsync();
		for(int i = 0; i < listaUsuarios.Count; i++)
		{
			Usuarios.Add(listaUsuarios[i]);
        }

		if(Activos.Count > 0) ActivoSeleccionado = Activos[0];
		if(Estados.Count > 0) EstadoSeleccionado = Estados[0];
		if(Usuarios.Count > 0) UsuarioSeleccionado = Usuarios[0];
    }

	private async void CrearIncidencia()
	{
		// Lógica para crear la incidencia
		if(Shell.Current == null) return;

		string titulo = Titulo;
		if(titulo == null) titulo = "";
		titulo = titulo.Trim();

		string descripcion = Descripcion;
		if(descripcion == null) descripcion = "";
		descripcion = descripcion.Trim();

		if(titulo.Length == 0 || descripcion.Length == 0)
		{
			await Shell.Current.DisplayAlert("Error", "El título y la descripción son obligatorios.", "Aceptar");
			return;
        }

		if(ActivoSeleccionado == null || EstadoSeleccionado == null || UsuarioSeleccionado == null)
		{
			await Shell.Current.DisplayAlert("Error", "Debe seleccionar un activo, un estado y un usuario asignado.", "Aceptar");
			return;
        }

        //Fecha de finalización. Si el estado es Terminado, se pone la fecha actual. Si no, null.
		DateTime fechaFinalizacion = DateTime.MinValue;

		string tipo = EstadoSeleccionado.Tipo;
		if(tipo == null) tipo = "";
		tipo = tipo.Trim();

		if(string.Compare(tipo, "Terminada", true) == 0)
		{
			fechaFinalizacion = DateTime.Today;
        }

		try
		{
			int nuevaIncidenciaId = await incidenciaService.CrearIncidenciaAsync(
				titulo,
				descripcion,
                FechaCreacion,
                fechaFinalizacion,
                ActivoSeleccionado.IdActivo,
				EstadoSeleccionado.IdEstado,
				UsuarioSeleccionado.IdUsuario
				
			);

            await Shell.Current.DisplayAlert("Éxito", $"Incidencia creada con ID: {nuevaIncidenciaId}", "Aceptar");
            await Shell.Current.GoToAsync("..");
        }catch(Exception ex)
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