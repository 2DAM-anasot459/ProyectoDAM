namespace InventarioActivos.Models.GestionUsuarios;

public class UsuarioListadoItem
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; }
    public string Apellidos { get; set; }
    public string TipoUsuario { get; set; }

    public string NombreUsuario { get; set; }
    public string Contrasena { get; set; }
    public int IdRol { get; set; }

    public UsuarioListadoItem()
    {
        Nombre = "";
        Apellidos = "";
        TipoUsuario = "";
        NombreUsuario = "";
        Contrasena = "";
        IdRol = 0;
    }
}