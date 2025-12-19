namespace InventarioActivos.Usuario;

public partial class PanelPrincipal : ContentPage
{
	public PanelPrincipal()
	{
		InitializeComponent();
		BindingContext = new ViewModels.PanelPrincipalViewModel();
    }
}