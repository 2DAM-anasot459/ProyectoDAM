using System.Windows.Input;
namespace InventarioActivos.ViewModels.Usuario;

public class PanelPrincipalViewModel : BaseViewModel
{
	public ICommand IrActivosCommand { get; set; }
	public ICommand IrMapaCommand { get; set; }
	public ICommand IrIncidenciasCommand { get; set; }

	public PanelPrincipalViewModel()
	{
		Title = "Panel Principal";

		IrActivosCommand = new Command(IrActivo);
		IrMapaCommand = new Command(IrMapa);
		IrIncidenciasCommand = new Command(IrIncidencia);
    }

	public async void IrActivo()
	{
		await Shell.Current.GoToAsync("//tec/activos");
	}

    public async void IrMapa()
    {
        await Shell.Current.GoToAsync("//tec/mapa");
    }

    public async void IrIncidencia()
    {
        await Shell.Current.GoToAsync("//tec/incidencias");
    }

}