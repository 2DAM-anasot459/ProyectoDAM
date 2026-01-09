using System.Collections.ObjectModel;
using System.Windows.Input;
using InventarioActivos.Models.GestionUsuarios;
using InventarioActivos.Services;
namespace InventarioActivos.ViewModels.Administrador;

public class GestionUsuariosViewModel : BaseViewModel
{
    private readonly UsuarioService usuarioService;

    public ObservableCollection<UsuarioListadoItem> Usuarios { get; }
    private List<UsuarioListadoItem> listaUsuarios;

    private string filtroNombre;
    public string FiltroNombre
    {
        get { return filtroNombre; }
        set
        {
            filtroNombre = value;
            OnPropertyChanged();
            AplicarFiltro();
        }
    }

    public ICommand CargarCommand { get; }
    public ICommand NuevoUsuarioCommand { get; }
    public ICommand EditarUsuarioCommand { get; }
    public ICommand EliminarUsuarioCommand { get; }


    public GestionUsuariosViewModel(UsuarioService service)
    {
        usuarioService = service;

        Title = "Gestión de Usuarios";

        Usuarios = new ObservableCollection<UsuarioListadoItem>();
        listaUsuarios = new List<UsuarioListadoItem>();
        filtroNombre = "";

        CargarCommand = new Command(Cargar);
        NuevoUsuarioCommand = new Command(NuevoUsuario);
        EditarUsuarioCommand = new Command(EditarUsuario);
        EliminarUsuarioCommand = new Command(EliminarUsuario);

    }

    public async Task CargarAsync()
    {
        Usuarios.Clear();

        List<UsuarioListadoItem> lista = await usuarioService.ObtenerUsuariosAsync();
        listaUsuarios = lista;

        for (int i = 0; i < listaUsuarios.Count; i++)
        {
            Usuarios.Add(listaUsuarios[i]);
        }

        AplicarFiltro();
    }


    private async void Cargar()
    {
        try
        {
            await CargarAsync();
        }
        catch(Exception ex)
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Error", "No se pudo cargar los usuarios: " + ex.Message, "Aceptar");
        }
    }

    private void AplicarFiltro() 
    {
        string texto = filtroNombre;
        if (texto == null) texto = "";
        texto = texto.Trim().ToLower();

        Usuarios.Clear();

        for(int i = 0; i < listaUsuarios.Count; i++)
        {
            UsuarioListadoItem usuarioLista = listaUsuarios[i];

            string nomnbre = usuarioLista.Nombre;
            if (nomnbre == null) nomnbre = "";
            nomnbre = nomnbre.ToLower();

            string apellidos = usuarioLista.Apellidos;
            if (apellidos == null) apellidos = "";
            apellidos = apellidos.ToLower();

            if (texto.Length == 0 || nomnbre.Contains(texto) || apellidos.Contains(texto))
            {
                Usuarios.Add(usuarioLista);
            }
        }
    }

    private async void NuevoUsuario()
    {
        if (Shell.Current == null) return;
        await Shell.Current.GoToAsync("admin/crearUsuario");
    }

    private async void EditarUsuario(object parametro)
    {
        if(Shell.Current == null) return;
        if(parametro == null) return;

        UsuarioListadoItem u = (UsuarioListadoItem)parametro;
        await Shell.Current.GoToAsync("admin/editarUsuario?IdUsuario=" + u.IdUsuario);
    }

    private async void EliminarUsuario(object parametro)
    {
        if (Shell.Current == null) return;
        if(parametro == null) return;

        UsuarioListadoItem u = (UsuarioListadoItem)parametro;

        bool confirmar = await Shell.Current.DisplayAlert(
            "Confirmar",
            "¿Seguro que quieres eliminar al usuario " + u.Nombre + "?",
            "Sí",
            "No");
        if (!confirmar) return;

        try
        {
            bool ok = await usuarioService.EliminarUsuarioAsync(u.IdUsuario);
            if (!ok)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo eliminar al usuario", "Aceptar");
                return;
            }

            await Shell.Current.DisplayAlert("Éxito", "Usuario eliminado", "Acpetar");
            await CargarAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "No se pudo eliminar: " + ex.Message, "Aceptar");
        }

    }


}