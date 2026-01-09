using System.Collections.ObjectModel;
using System.Windows.Input;
using InventarioActivos.Models.GestionLocalizaciones;
using InventarioActivos.Services;
namespace InventarioActivos.ViewModels.Administrador;

public class GestionLocalizacionViewModel : BaseViewModel
{
    private readonly LocalizacionService localizacionService;

    public ObservableCollection<ActivoLocalizacionItem> Activos { get; }

    private List<ActivoLocalizacionItem> listaCompleta;

    private string filtroActivo;
    public string FiltroActivo
    {
        get {  return filtroActivo; }
        set
        {
            filtroActivo = value;
            OnPropertyChanged();
            AplicarFiltro();
        }
    }

    public ICommand NuevaLocalizacionCommand { get; }
    public ICommand EditarLocalizacionCommand { get; }
    public ICommand EliminarLocalizacionCommand { get; }

    public GestionLocalizacionViewModel(LocalizacionService service)
    {

        localizacionService = service;

        Title = "Gestión Localización";

        Activos = new ObservableCollection<ActivoLocalizacionItem>();

        NuevaLocalizacionCommand = new Command(NuevaLocalizacion);
        EditarLocalizacionCommand = new Command(EditarLocalizacion);
        EliminarLocalizacionCommand = new Command(EliminarLocalizacion);
    }

    public async Task CargarAsync()
    {
        Activos.Clear();

       listaCompleta = await localizacionService.ObtenerActivosConYSinLocalizacionAsync();
        for (int i = 0; i < listaCompleta.Count; i++)
        {
            Activos.Add(listaCompleta[i]);
        }
    }

    private void AplicarFiltro()
    {
        string texto = FiltroActivo;
        if (texto == null) texto = ""; 
        texto = texto.Trim().ToLower();

        Activos.Clear();

        for (int i = 0;i < listaCompleta.Count; i++)
        {
            var item = listaCompleta[i];

            string nombre = item.NombreEquipo;
            if (nombre == null) nombre = "";
            nombre = nombre.ToLower();

            if(texto.Length == 0 || nombre.Contains(texto))
                Activos.Add(item);
        }
    }

    private async void NuevaLocalizacion(object parametro)
    {
        if(Shell.Current ==  null) return;
        if(parametro == null) return;  

        ActivoLocalizacionItem item =(ActivoLocalizacionItem)parametro;

        await Shell.Current.GoToAsync("admin/crearLocalizacion?IdActivo=" + item.IdActivo);
    }

    private async void EditarLocalizacion(object parametro)
    {
        if(Shell.Current == null) return;
        if(parametro == null) return;

        ActivoLocalizacionItem item = (ActivoLocalizacionItem)parametro;

        if(item.IdLocalizacion <= 0)
        {
            await Shell.Current.DisplayAlert("Aviso", "Este activo no tiene localización asignada", "Aceptar");
            return;
        }

        await Shell.Current.GoToAsync("admin/editarLocalizacion?IdActivo=" + item.IdActivo + "&IdLocalizacion=" + item.IdLocalizacion);

    }

    private async void EliminarLocalizacion(object parametro)
    {
        if (Shell.Current == null) return;
        if (parametro == null) return;

        ActivoLocalizacionItem item = (ActivoLocalizacionItem)parametro;

        if (item.IdLocalizacion <= 0)
        {
            await Shell.Current.DisplayAlert("Aviso", "Este activo no tiene localización asignada", "Aceptar");
            return;
        }

        bool confirmar = await Shell.Current.DisplayAlert(
            "Confirmar",
            "¿Quieres quitar la localización del activo " + item.NombreEquipo + "?",
            "Si",
            "No");

        if (!confirmar) return;

        try
        {
            bool ok = await localizacionService.QuitarYBorrarLocalizacionDeActivoAsync(item.IdActivo);
            if (!ok)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo quitar la localización", "Aceptar");
                return;
            }

            await Shell.Current.DisplayAlert("Éxito", "Localización quitada del activo", "Aceptar");
            await CargarAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "No se pudo quitar la localizacion: " + ex.Message, "Aceptar");
        }
    }

}