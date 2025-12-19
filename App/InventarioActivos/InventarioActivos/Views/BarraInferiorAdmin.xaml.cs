namespace InventarioActivos.Views;

public partial class BarraInferiorAdmin : ContentView
{
	public BarraInferiorAdmin()
	{
		InitializeComponent();
	}

	private async void OnInicioTapped(object sender, EventArgs e)
	{
		if(Shell.Current != null)
		{
			await Shell.Current.GoToAsync("//admin/home");
		}

		
	}

    private async void OnIncidenciasTapped(object sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//admin/incidencias");
        }


    }

    private async void OnLocalizacionTapped(object sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//admin/localizaciones");
        }


    }

    private async void OnUsuarioTapped(object sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//admin/usuarios");
        }


    }
}