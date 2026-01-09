namespace InventarioActivos
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Rutas autenticación
            Routing.RegisterRoute("auth/cambioContrasena", typeof(Autenticacion.CambioContrasena));


            //Rutas de navegación administrador
            Routing.RegisterRoute("admin/nuevaIncidencia", typeof(Administrador.NuevaIncidencia));
            Routing.RegisterRoute("admin/editarIncidencia", typeof(Administrador.EditarIncidencia));
            Routing.RegisterRoute("admin/crearUsuario", typeof(Administrador.CrearUsuario));
            Routing.RegisterRoute("admin/editarUsuario", typeof(Administrador.EditarUsuario));
            Routing.RegisterRoute("admin/crearLocalizacion", typeof(Administrador.CrearLocalizacion));
            Routing.RegisterRoute("admin/editarLocalizacion", typeof(Administrador.EditarLocalizacion));

            //Rutas de navegación tecnico
            Routing.RegisterRoute("tec/nuevaIncidencia", typeof(Usuario.NuevaIncidencia));
            Routing.RegisterRoute("tec/editarIncidencia", typeof(Usuario.EditarIncidencia));
            Routing.RegisterRoute("tec/fichaActivo", typeof(Usuario.FichaActivo));

        }
    }
}
