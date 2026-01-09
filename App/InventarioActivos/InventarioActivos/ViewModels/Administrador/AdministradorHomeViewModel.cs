using System.Windows.Input;

namespace InventarioActivos.ViewModels.Administrador;

public class AdministradorHomeViewModel : BaseViewModel
{
    //Botones de la paginal principal del administrador
    public ICommand IrIncidenciasCommand { get; }
	public ICommand IrLocalizacionesCommand { get; }
    public ICommand IrUsuariosCommand { get; }

   

    public AdministradorHomeViewModel()
	{
		Title = "Panel de Administración";

        IrIncidenciasCommand = new Command(IrIncidencias);
        IrLocalizacionesCommand = new Command(IrLocalizaciones);
        IrUsuariosCommand = new Command(IrUsuarios);

     

    }

    

    private void IrIncidencias()
    {
        Navegar("//admin/incidencias");
    }

    private void IrLocalizaciones()
    {
        Navegar("//admin/localizaciones");
    }
    private void IrUsuarios()
    {
        Navegar("//admin/usuarios");
    }

    public async void Navegar(string ruta)
    {
        if(Shell.Current != null)
        {
            await Shell.Current.GoToAsync(ruta);
        }
    }
}