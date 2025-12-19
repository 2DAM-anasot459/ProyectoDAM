namespace InventarioActivos.Administrador;

public partial class AdministradorHome : ContentPage
{
	public AdministradorHome()
	{
		InitializeComponent();
		BindingContext = new ViewModels.AdministradorHomeViewModel();
    }
}