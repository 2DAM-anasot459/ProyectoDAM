using InventarioActivos.Models.DatosActivos;
using InventarioActivos.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace InventarioActivos.ViewModels.Usuario;

public class BusquedaActivosViewModel : BaseViewModel
{
    private readonly ActivosService activosService;

    public ObservableCollection<ItemActivos> Activos { get; }

    private List<ItemActivos> listaCompleta;

    private bool cargado;

    private string filtroActivo;
    public string FiltroActivo
    {
        get { return filtroActivo; }
        set
        {
            filtroActivo = value;
            OnPropertyChanged();
            if(cargado == false)
            {
                AplicarFiltro();
            }
            
        }
    }

    public ICommand RecargarCommand { get; }

    public BusquedaActivosViewModel(ActivosService service)
    {
        activosService = service;

        Title = "Buscar Activos";

        Activos = new ObservableCollection<ItemActivos>();
        listaCompleta = new List<ItemActivos>();

        filtroActivo = "";

        RecargarCommand = new Command(Recargar);


    }

    private async void Recargar()
    {
        await CargarAsync();
    }

    public async Task CargarAsync()
    {
        cargado = true;

        Activos.Clear();

        listaCompleta = await activosService.ObtenerActivosResumenAsync("");
        
        for (int i = 0; i < listaCompleta.Count; i++)
        {
            Activos.Add(listaCompleta[i]);
        }
        cargado = false;
    }

    private void AplicarFiltro()
    {
        if (listaCompleta == null) return;

        string texto = FiltroActivo;
        if (texto == null) texto = "";
        texto = texto.Trim().ToLower();

        Activos.Clear();

        for (int i = 0; i < listaCompleta.Count; i++)
        {
            ItemActivos item = listaCompleta[i];

            string nombre = item.NombreEquipo;
            if (nombre == null) nombre = "";
            nombre = nombre.ToLower();

            if (texto.Length == 0 || nombre.Contains(texto))
                Activos.Add(item);
        }
    }
}