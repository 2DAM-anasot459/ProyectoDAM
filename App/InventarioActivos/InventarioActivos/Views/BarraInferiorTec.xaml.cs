namespace InventarioActivos.Views;

public partial class BarraInferiorTec : ContentView
{
	public BarraInferiorTec()
	{
		InitializeComponent();
	}

    private async void OnInicioTapped(object sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//tec/home");
        }


    }

    private async void OnActivosTapped(object sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//tec/activos");
        }


    }

    private async void OnMapaTapped(object sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//tec/mapa");
        }


    }

    private async void OnIncidenciaTapped(object sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//tec/incidencias");
        }


    }
}