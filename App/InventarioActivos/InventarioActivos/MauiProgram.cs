using InventarioActivos.Administrador;
using InventarioActivos.Data;
using InventarioActivos.Services;
using InventarioActivos.Usuario;
using InventarioActivos.ViewModels;
using InventarioActivos.ViewModels.Administrador;
using InventarioActivos.ViewModels.Usuario;
using Microsoft.Extensions.Logging;


namespace InventarioActivos
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<NavigationService>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<InventarioActivos.Autenticacion.Login>();

            builder.Services.AddTransient<CambioContrasenaViewModel>();
            builder.Services.AddTransient<InventarioActivos.Autenticacion.CambioContrasena>();

            builder.Services.AddTransient<AdministradorHomeViewModel>();
            builder.Services.AddTransient<AdministradorHome>();

            builder.Services.AddSingleton<IncidenciaService>();
            builder.Services.AddTransient<GestionIncidenciasViewModel>();

            builder.Services.AddSingleton<IncidenciaService>();

            builder.Services.AddTransient<ViewModels.Administrador.EditarIncidenciaViewModel>();
            builder.Services.AddTransient<InventarioActivos.Administrador.EditarIncidencia>();

            builder.Services.AddSingleton<IncidenciaService>();
            builder.Services.AddTransient<ViewModels.Administrador.NuevaIncidenciaViewModel>();

            builder.Services.AddSingleton<UsuarioService>();
            builder.Services.AddTransient<GestionUsuariosViewModel>();
            builder.Services.AddTransient<CrearUsuarioViewModel>();
            builder.Services.AddTransient<EditarUsuarioViewModel>();
            builder.Services.AddTransient<InventarioActivos.Administrador.EditarUsuario>();

            builder.Services.AddSingleton<LocalizacionService>();
            builder.Services.AddTransient<GestionLocalizacionViewModel>();
            builder.Services.AddTransient<GestionLocalizacion>();

            
            builder.Services.AddTransient<CrearLocalizacionViewModel>();
            builder.Services.AddTransient<CrearLocalizacion>();

            builder.Services.AddTransient<EditarLocalizacionViewModel>();
            builder.Services.AddTransient<EditarLocalizacion>();

            builder.Services.AddTransient<PanelPrincipalViewModel>();
            builder.Services.AddTransient<PanelPrincipal>();

            builder.Services.AddTransient<PanelIncidenciasViewModel>();
            builder.Services.AddTransient<PanelIncidencias>();
            builder.Services.AddTransient<ViewModels.Usuario.NuevaIncidenciaViewModel>();
            builder.Services.AddTransient<ViewModels.Usuario.EditarIncidenciaViewModel>();
            builder.Services.AddTransient<InventarioActivos.Usuario.EditarIncidencia>();
            builder.Services.AddSingleton<ActivosService>();
            builder.Services.AddTransient<BusquedaActivosViewModel>();
            builder.Services.AddTransient<InventarioActivos.Usuario.BusquedaActivos>();
            builder.Services.AddTransient<MapaViewModel>();
            builder.Services.AddTransient<InventarioActivos.Usuario.Mapa>();
            builder.Services.AddTransient<FichaActivoVewModel>();
            builder.Services.AddTransient<InventarioActivos.Usuario.FichaActivo>();

#endif

            return builder.Build();
        }
    }
}
