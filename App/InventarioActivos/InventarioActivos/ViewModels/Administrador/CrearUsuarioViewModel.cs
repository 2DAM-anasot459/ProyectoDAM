using System.Collections.ObjectModel;
using System.Windows.Input;
using InventarioActivos.Models.GestionUsuarios;
using InventarioActivos.Services;
namespace InventarioActivos.ViewModels.Administrador;

public class CrearUsuarioViewModel : BaseViewModel
{
	private readonly UsuarioService usuarioService;

	public ObservableCollection<RolItem> Roles { get; }

	private string nombre;
	public string Nombre
	{
		get { return nombre; }
		set
		{
			nombre = value;
			OnPropertyChanged();
		}
    }

	private string apellidos;
	public string Apellidos
	{
		get { return apellidos; }
		set
		{
			apellidos = value;
			OnPropertyChanged();
		}
    }

	private string nombreUsuario;
	public string NombreUsuario
	{
		get { return nombreUsuario; }
		set
		{
			nombreUsuario = value;
			OnPropertyChanged();
		}
    }

	private string contrasena;
	public string Contrasena
	{
		get { return contrasena; }
		set
		{
			contrasena = value;
			OnPropertyChanged();
		}
    }

	private RolItem rolSeleccionado;
	public RolItem RolSeleccionado
	{
		get { return rolSeleccionado; }
		set
		{
			rolSeleccionado = value;
			OnPropertyChanged();
		}
    }

	public ICommand CrearUsuarioCommand { get; }
	public ICommand VolverCommand { get; }

	public CrearUsuarioViewModel(UsuarioService service)
	{
		usuarioService = service;

		Title = "Crear Nuevo Usuario";

		Roles = new ObservableCollection<RolItem>();

		nombre = "";
		apellidos = "";
		nombreUsuario = "";
		contrasena = "";

		rolSeleccionado = new RolItem();

		CrearUsuarioCommand = new Command(CrearUsuario);
		VolverCommand = new Command(Volver);
    }

	public async Task CargarAsync()
	{
		Roles.Clear();
		List<RolItem> listaRoles = await usuarioService.ObtenerRolesAsync();
		for (int i = 0; i < listaRoles.Count; i++)
		{
			Roles.Add(listaRoles[i]);
        }

		if (Roles.Count > 0) RolSeleccionado = Roles[0];
    }

	public async void CrearUsuario()
	{
		if(Shell.Current == null) return;	
		if(RolSeleccionado == null || RolSeleccionado.IdRol <= 0)
		{
			await Shell.Current.DisplayAlert("Error", "Debe seleccionar un rol válido.", "Aceptar");
			return;
        }

		try
		{
			bool ok = await usuarioService.CrearUsuarioAsync(
				Nombre,
				Apellidos,
				NombreUsuario,
				Contrasena,
				RolSeleccionado.IdRol);

			if (!ok)
			{
				await Shell.Current.DisplayAlert("Error", "No se pudo crear el usuario. Revise los datos e inténtelo de nuevo.", "Aceptar");
				return;
            }
			await Shell.Current.DisplayAlert("Éxito", "Usuario creado correctamente.", "Aceptar");
			await Shell.Current.GoToAsync("..");
        }catch (Exception ex)
		{
			await Shell.Current.DisplayAlert("Error", "No se pudo crear el usuario: " + ex.Message, "Aceptar");
        }
    }

	public async void Volver()
	{
		if(Shell.Current == null) return;

        await Shell.Current.GoToAsync("..");
		
	}
}