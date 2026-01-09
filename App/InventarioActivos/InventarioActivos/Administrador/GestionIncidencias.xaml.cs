using InventarioActivos.ViewModels.Administrador;

namespace InventarioActivos.Administrador;

public partial class GestionIncidencias : ContentPage
{
    private readonly GestionIncidenciasViewModel vm;
    public GestionIncidencias(GestionIncidenciasViewModel viewModel)
    {
        InitializeComponent();

        vm = viewModel;
        BindingContext = vm;
    }

    // Recargamos en OnAppearing para reflejar cambios tras crear/editar/eliminar incidencias.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (vm != null)
            await vm.CargarIncidencias();
    }
}