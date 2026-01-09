using InventarioActivos.ViewModels;

namespace InventarioActivos.Autenticacion;

public partial class CambioContrasena : ContentPage
{
	public CambioContrasena(CambioContrasenaViewModel cambioContrasenaViewModel)
	{
		InitializeComponent();
		BindingContext = cambioContrasenaViewModel;
    }
}