using System.Windows.Input;

namespace InventarioActivos.ViewModels;

public class AdministradorHomeViewModel : BaseViewModel
{
    //Botones de la paginal principal del administrador
    public ICommand IrIncidenciasCommand { get; }
	public ICommand IrLocalizacionesCommand { get; }
    public ICommand IrUsuariosCommand { get; }

    //Botones de navigación inferior
    public ICommand IrHomeCommand { get; }
    public ICommand IrIncidenciasMenuCommand { get; }
    public ICommand IrLocalizacionesMenuCommand { get; }
    public ICommand IrUsuariosMenuCommand { get; }

    public AdministradorHomeViewModel()
	{
		Title = "Panel de Administración";

        IrIncidenciasCommand = new Command(IrIncidencias);
        IrLocalizacionesCommand = new Command(IrLocalizaciones);
        IrUsuariosCommand = new Command(IrUsuarios);

        IrHomeCommand = new Command(IrHome);
        IrIncidenciasMenuCommand = new Command(IrIncidencias);
        IrLocalizacionesMenuCommand = new Command(IrLocalizaciones);
        IrUsuariosMenuCommand = new Command(IrUsuarios);

    }

    private async void IrHome()
    {
        await Shell.Current.GoToAsync("PanelAdministrador");
    }

    private async void IrIncidencias()
    {
        await Shell.Current.GoToAsync("GestionIncidencias");
    }

    private async void IrLocalizaciones()
    {
        await Shell.Current.GoToAsync("GestionLocalizaciones");
    }
    private async void IrUsuarios()
    {
        await Shell.Current.GoToAsync("GestionUsuarios");
    }
}