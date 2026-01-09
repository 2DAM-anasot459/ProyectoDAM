using System.Collections.ObjectModel;
using System.Windows.Input;
using InventarioActivos.Models.GestionUsuarios;
using InventarioActivos.Services;
namespace InventarioActivos.ViewModels.Administrador;

public class EditarUsuarioViewModel : BaseViewModel
{
    private readonly UsuarioService usuarioService;

    private int idUsuario;

    public ObservableCollection<RolItem> RolItems { get; }

    private string nombre;
    public string Nombre
    {
        get { return nombre; }
        set { nombre = value; OnPropertyChanged(); }
    }

    private string apellidos;
    public string Apellidos
    {
        get { return apellidos; }
        set { apellidos = value; OnPropertyChanged(); }
    }

    private string nombreUsuario;
    public string NombreUsuario
    {
        get { return nombreUsuario; }
        set { nombreUsuario = value; OnPropertyChanged(); }
    }

    private string contrasena;
    public string Contrasena
    {
        get { return contrasena; }
        set { contrasena = value; OnPropertyChanged(); }
    }

    private RolItem rolSeleccionado;
    public RolItem RolSeleccionado
    {
        get { return rolSeleccionado; }
        set { rolSeleccionado = value; OnPropertyChanged(); }
    }

    public ICommand GuardarCommand { get; }
    public ICommand VolverCommand { get; }

    public EditarUsuarioViewModel(UsuarioService service) 
    {
        usuarioService = service;

        Title = "Editar Usuario";

        RolItems = new ObservableCollection<RolItem>();

        nombre = "";
        apellidos = "";
        nombreUsuario = "";
        contrasena = "";

        GuardarCommand = new Command(Guardar);
        VolverCommand = new Command(Volver);
    }

    public void SetIdUsuario(int id)
    {
        idUsuario = id;
    }

    public async Task CargarAsync()
    {
        RolItems.Clear();

        List<RolItem> roles = await usuarioService.ObtenerRolesAsync();
        for (int i = 0; i < roles.Count; i++)
        {
            RolItems.Add(roles[i]);
        }

        UsuarioListadoItem us = await usuarioService.ObtenerUsuarioPorIdAsync(idUsuario);
        if (us == null)
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Error", "No se encontró el usuario.", "Aceptar");
            return;
        }

        Nombre = us.Nombre;
        Apellidos = us.Apellidos;
        NombreUsuario = us.NombreUsuario;
        Contrasena = us.Contrasena;

        RolSeleccionado = BuscarRolPorId(us.IdRol);
    }

    private RolItem BuscarRolPorId(int idRol)
    {
        RolItem rol = null;

        for (int i = 0;i < RolItems.Count; i++)
        {
            if (RolItems[i].IdRol == idRol)
            {
                rol = RolItems[i];
                break;
            }
        }

        if (rol == null)
            rol = new RolItem();
        return rol;
    }

    private async void Guardar()
    {
        if (Shell.Current == null) return;

        string no = Nombre;
        if (no == null) no = "";
        no = no.Trim();

        string ap = Apellidos;
        if (ap == null) ap = "";
        ap = ap.Trim();

        string nu = NombreUsuario;
        if (nu == null) nu = "";
        nu = nu.Trim();

        string co = Contrasena;
        if (co == null) co = "";
        co = co.Trim();

        if (no.Length == 0 || ap.Length == 0 || nu.Length == 0 || co.Length == 0)
        {
            await Shell.Current.DisplayAlert("Error", "Todos los campos son obligatorios", "Acpetar");
            return;
        }

        if (RolSeleccionado == null || RolSeleccionado.IdRol <= 0)
        {
            await Shell.Current.DisplayAlert("Error", "Debe seleccionar un tipo de usuario", "Aceptar");
            return;
        }

        try
        {
            bool ok = await usuarioService.ActualizarUsuarioAsync(
                idUsuario,
                no,
                ap,
                nu,
                co,
                RolSeleccionado.IdRol);

            if (!ok)
            {
                await Shell.Current.DisplayAlert("Error", "No se ha podido actualizar el ususario", "Aceptar");
                return;
            }

            await Shell.Current.DisplayAlert("Éxito", "Usuario actualizado correctamente", "Aceptar");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Ocurrio un error: " + ex.Message, "Aceptar");
        }
    }

    private async void Volver()
    {
        if (Shell.Current == null) return;
        await Shell.Current.GoToAsync("..");
    }

}