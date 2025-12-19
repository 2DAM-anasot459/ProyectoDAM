namespace InventarioActivos.Services;

public class NavigationService : ContentPage
{
	public void IrAHomeAdmin()
	{
		if (Application.Current != null)
		{
			if (Application.Current.Windows.Count > 0)
			{
				Application.Current.Windows[0].Page = new NavigationPage(new Administrador.AdministradorHome());
            }
        }
    }

    public void IrAHomeTecnico()
    {
        if (Application.Current != null)
        {
            if (Application.Current.Windows.Count > 0)
            {
                Application.Current.Windows[0].Page = new NavigationPage(new Usuario.PanelPrincipal());
            }
        }
    }
}