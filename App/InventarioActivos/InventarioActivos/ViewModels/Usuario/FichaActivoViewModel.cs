using System.Collections.ObjectModel;
using System.Windows.Input;
using InventarioActivos.Models.DatosActivos;
using InventarioActivos.Services;

namespace InventarioActivos.ViewModels.Usuario;

public class FichaActivoVewModel : BaseViewModel
{
    private readonly ActivosService activosService;

    private ItemActivos item;
    public ItemActivos Item
    {
        get { return item; }
        set { item = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<string> Programas { get; }

    private int idActivo;

    public ICommand CrearIncidenciaCommand { get; }
    public ICommand VolverCommand { get; }

    public FichaActivoVewModel(ActivosService service)
    {
        activosService = service;

        Title = "Ficha Activo";

        Programas = new ObservableCollection<string>();
        item = new ItemActivos();
        idActivo = 0;

        CrearIncidenciaCommand = new Command(CrearIncidencia);
        VolverCommand = new Command(Volver);
    }

    public void SetIdActivo(int id)
    {
        idActivo = id;
    }

    public async Task CargarAsync()
    {
        if (idActivo <= 0) return;

        ItemActivos datosActivo = await activosService.ObtenerFichaActivoAsync(idActivo);
        if (datosActivo == null) return;

        Item = datosActivo;

        Programas.Clear();
        for (int i = 0; i < Item.ProgramasInstalados.Count; i++)
        {
            Programas.Add(Item.ProgramasInstalados[i]);
        }
    }

    public async void CrearIncidencia()
    {
        if (Shell.Current == null) return;
        if (Item == null) return;

        var parametros = new Dictionary<string, object>();
        parametros.Add("IdActivo", Item.IdActivo);

        await Shell.Current.GoToAsync("tec/nuevaIncidencia", parametros);
    }

    public async void Volver()
    {
        if (Shell.Current == null) return;
        await Shell.Current.GoToAsync("..");
    }
}