using InventarioActivos.ViewModels.Usuario;

namespace InventarioActivos.Usuario;

public partial class PanelIncidencias : ContentPage
{
    private readonly PanelIncidenciasViewModel vm;
    public PanelIncidencias(PanelIncidenciasViewModel viewModel)
	{
		InitializeComponent();       
        vm = viewModel;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (vm != null)
            await vm.CargarIncidencias();
    }
}