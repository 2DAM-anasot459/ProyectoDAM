
using InventarioActivos.ViewModels.Administrador;

namespace InventarioActivos.Administrador;

public partial class AdministradorHome : ContentPage
{
	
	public AdministradorHome(AdministradorHomeViewModel  viewModel)
	{
        InitializeComponent();
		BindingContext = viewModel;
    }
}