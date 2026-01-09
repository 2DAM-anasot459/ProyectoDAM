using InventarioActivos.ViewModels.Usuario;
namespace InventarioActivos.Usuario;

public partial class PanelPrincipal : ContentPage
{
	private readonly PanelPrincipalViewModel vm;
	public PanelPrincipal(PanelPrincipalViewModel viewModel)
	{
		InitializeComponent();
		vm = viewModel;
		BindingContext = viewModel;
    }
}