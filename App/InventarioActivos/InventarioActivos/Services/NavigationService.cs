namespace InventarioActivos.Services;

public class NavigationService
{
	public async Task IrAHomeAdmin()
	{
		if(Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//admin/home");
        }
    }

    public async Task IrAHomeTecnico()
    {
        if (Shell.Current != null)
        {
          await Shell.Current.GoToAsync("//tec/home");
        }
    }

    public async Task IrALogin()
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}