using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioActivos.Services;

namespace InventarioActivos.ViewModels;

public partial class CambioContrasenaViewModel : BaseViewModel
{
    private AuthService AuthService;

    [ObservableProperty]
    private string usuario;

    [ObservableProperty]
    private string contrasenaActual;

    [ObservableProperty]
    private string nuevaContrasena;

    [ObservableProperty]
    private string confirmarContrasena; 

    public CambioContrasenaViewModel(AuthService auth)
    {
        AuthService = auth;
        Title = "Cambio de Contraseña";

        Usuario = "";
        ContrasenaActual = "";
        NuevaContrasena = "";
        ConfirmarContrasena = "";

    }

    [RelayCommand]
    private async Task CambiarAsync()
    {
        string user = Usuario;
        string oldPass = ContrasenaActual;
        string newPass = NuevaContrasena;
        string confirmPass = ConfirmarContrasena;

        if(usuario == null) user = "";
        if(contrasenaActual == null) oldPass = "";
        if (nuevaContrasena == null) newPass = "";
        if(confirmarContrasena == null) confirmPass = "";

        user = user.Trim();
        oldPass = oldPass.Trim();
        newPass = newPass.Trim();
        confirmPass = confirmPass.Trim();

        if(user.Length == 0 || oldPass.Length == 0 || newPass.Length == 0 || confirmPass.Length == 0)
        {
            if(Shell.Current != null)
            {
                await Shell.Current.DisplayAlert("Error", "Todos los campos son obligatorios.", "Aceptar");
            }                
            return;
        }

        if(newPass != confirmPass)
        {
            if(Shell.Current != null)
            {
                await Shell.Current.DisplayAlert("Error", "La nueva contraseña y la confirmación no coinciden.", "Aceptar");
            }                
            return;
        }

        bool exito = await AuthService.CambiarContrasenaAsync(user, oldPass, newPass);
        if(exito == false)
        {
            if(Shell.Current != null)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo cambiar la contraseña. Usuario o contraseña actual incorrectos.", "Aceptar");
            }
            return;
        }

        if(Shell.Current != null)
        {
            await Shell.Current.DisplayAlert("Éxito", "La contraseña ha sido cambiada exitosamente.", "Aceptar");
            await Shell.Current.GoToAsync("//login");
        }
    }

    [RelayCommand]
    private async Task VolverAsync()
    {
        if(Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}