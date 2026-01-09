using InventarioActivos.Models.GestionLocalizaciones;
using InventarioActivos.ViewModels.Administrador;
using Microsoft.Maui.Layouts;
using System.Buffers.Text;
namespace InventarioActivos.Administrador;

public partial class EditarLocalizacion : ContentPage, IQueryAttributable
{
    private double escalaActual = 1;
    private double escalaInicial = 1;

    private double desplazamientoX = 0;
    private double desplazamientoY = 0;

    private double panInicioX = 0;
    private double panInicioY = 0;

    private bool estaHaciendoPinch = false;
    private const double MAP_W = 7205.0;
    private const double MAP_H = 4606.0;
    private const double MAP_RATIO = MAP_W / MAP_H;

    private double baseW, baseH, baseX, baseY;

    private readonly EditarLocalizacionViewModel vm;
    public EditarLocalizacion(EditarLocalizacionViewModel viewModel)
	{
		InitializeComponent();
        vm = viewModel;
        BindingContext = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query == null) return;

        if (query.ContainsKey("IdActivo"))
        {
            object obj = query["IdActivo"];
            if (obj == null) return;

            int id = Convert.ToInt32(obj);
            vm.SetActivoInicial(id);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if(vm != null)
        {
            vm.CoordenadasCambiadas -= OnCoordenadasCambiadas;
            vm.CoordenadasCambiadas += OnCoordenadasCambiadas;
        }

        try
        {
            await vm.CargarAsync();
        }
        catch (Exception ex)
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Error", "No se pudo cargar: " + ex.Message, "Aceptar");
        }
    }

    private void OnCoordenadasCambiadas(double lat, double lon)
    {
        if(baseW <= 0 || baseH <= 0) return;

        double x = lon * baseW;
        double y = lat * baseH;

        MostrarChincheta(x, y);
    }

    private void AbrirActivoTapped(object sender, EventArgs e)
    {
        if (PickerActivos == null) return;
        PickerActivos.Focus();
    }

    private void OnVisorMapaSizeChanged(object sender, EventArgs e)
    {
        LayoutBaseRest();
    }

    private void LayoutBaseRest()
    {
        if (VisorMapa == null || ContenedorMapa == null) return;

        double vw = VisorMapa.Width;
        double vh = VisorMapa.Height;
        if (vw <= 0 || vh <= 0) return;

        if ((vw / vh) > MAP_RATIO)
        {
            baseH = vh;
            baseW = baseH * MAP_RATIO;
        }
        else
        {
            baseW = vw;
            baseH = baseW / MAP_RATIO;
        }

        baseX = (vw - baseW) / 2.0;
        baseY = (vh - baseH) / 2.0;

        ContenedorMapa.WidthRequest = baseW;
        ContenedorMapa.HeightRequest = baseH;
        ContenedorMapa.HorizontalOptions = LayoutOptions.Center;
        ContenedorMapa.VerticalOptions = LayoutOptions.Center;

        PanelChincheta.WidthRequest = baseW;
        PanelChincheta.HeightRequest = baseH;

        if (escalaActual <= 1.0001)
        {
            escalaActual = 1;
            ContenedorMapa.Scale = 1;
            desplazamientoX = 0;
            desplazamientoY = 0;
            ContenedorMapa.TranslationX = 0;
            ContenedorMapa.TranslationY = 0;
        }
        else
        {
            AjustarLimitesPan();
            ContenedorMapa.TranslationX = desplazamientoX;
            ContenedorMapa.TranslationY = desplazamientoY;
        }
    }

    //TAP para colocar chinchetas
    private void OnMapaTapped(object sender, TappedEventArgs e)
    {
        try
        {
            if (VisorMapa == null) return;
            if (vm == null) return;

            var posicion = e.GetPosition(VisorMapa);
            if (!posicion.HasValue) return;

            // Tap en el visor
            double tapX = posicion.Value.X;
            double tapY = posicion.Value.Y;

            if (baseW <= 0 || baseH <= 0) return;

            // Convertimos el tap a coordenadas "del contenido" (deshaciendo pan+zoom)

            double contenidoX = (tapX - baseX - desplazamientoX - baseW / 2.0) / escalaActual + baseW / 2.0;
            double contenidoY = (tapY - baseY - desplazamientoY - baseH / 2.0) / escalaActual + baseH / 2.0;

            // Si toca fuera de la imagen (en bandas), no hacemos nada
            if (contenidoX < 0 || contenidoY < 0 || contenidoX > baseW || contenidoY > baseH) return;

            // Normalizamos 0..1 (esto es lo que guardas como lat/lon)
            double lon = contenidoX / baseW;
            double lat = contenidoY / baseH;

            lon = Math.Max(0, Math.Min(1, lon));
            lat = Math.Max(0, Math.Min(1, lat));

            vm.SetCoordenadasNuevas(lat, lon);


            MostrarChincheta(contenidoX, contenidoY);
        }
        catch (Exception ex)
        {
            if (Shell.Current != null)
                Shell.Current.DisplayAlert("Error", "No se pudo colocar la chincheta: " + ex.Message, "Aceptar");
        }
    }

    private void MostrarChincheta(double mapaX, double mapaY)
    {
        if (PanelChincheta == null) return;

        PanelChincheta.Children.Clear();

        var chincheta = new Image
        {
            Source = "chincheta.png",
            WidthRequest = 32,
            HeightRequest = 32
        };


        double izquierda = mapaX - 16;
        double arriba = mapaY - 32;

        AbsoluteLayout.SetLayoutBounds(chincheta, new Rect(izquierda, arriba, 32, 32));
        AbsoluteLayout.SetLayoutFlags(chincheta, AbsoluteLayoutFlags.None);

        PanelChincheta.Children.Add(chincheta);
    }

    //PAN para desplazar mapa
    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (ContenedorMapa == null || VisorMapa == null) return;

        if (estaHaciendoPinch) return;

        if (e.StatusType == GestureStatus.Started)
        {
            panInicioX = e.TotalX;
            panInicioY = e.TotalY;
            return;
        }

        if (e.StatusType == GestureStatus.Running)
        {
            double dx = e.TotalX - panInicioX;
            double dy = e.TotalY - panInicioY;

            desplazamientoX += dx;
            desplazamientoY += dy;

            AjustarLimitesPan();

            ContenedorMapa.TranslationX = desplazamientoX;
            ContenedorMapa.TranslationY = desplazamientoY;

            panInicioX = e.TotalX;
            panInicioY = e.TotalY;
        }
    }

    //PINCH para zoom
    private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        if (ContenedorMapa == null || VisorMapa == null) return;
        if (baseW <= 0 || baseH <= 0) return;

        switch (e.Status)
        {
            case GestureStatus.Started:
                estaHaciendoPinch = true;
                escalaInicial = escalaActual;
                break;

            case GestureStatus.Running:
                {
                    double nuevaEscala = escalaInicial * e.Scale;
                    nuevaEscala = Math.Max(1, Math.Min(4, nuevaEscala));

                    if (nuevaEscala <= 1.02)
                    {
                        ResetZoomPan();
                        break;
                    }

                    double origenVisorX = e.ScaleOrigin.X * VisorMapa.Width;
                    double origenVisorY = e.ScaleOrigin.Y * VisorMapa.Height;

                    double origenMapaX = origenVisorX - baseX;
                    double origenMapay = origenVisorY - baseY;

                    double factor = nuevaEscala / escalaActual;

                    desplazamientoX = origenMapaX - factor * (origenMapaX - desplazamientoX);
                    desplazamientoY = origenMapay - factor * (origenMapay - desplazamientoY);

                    escalaActual = nuevaEscala;
                    ContenedorMapa.Scale = escalaActual;

                    AjustarLimitesPan();

                    ContenedorMapa.TranslationX = desplazamientoX;
                    ContenedorMapa.TranslationY = desplazamientoY;
                    break;
                }

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                estaHaciendoPinch = false;
                escalaInicial = escalaActual;
                break;

        }


    }
    private void OnMapaDoubleTapped(object sender, TappedEventArgs e)
    {
        ResetZoomPan();
    }
    private void ResetZoomPan()
    {
        escalaActual = 1;
        escalaInicial = 1;
        desplazamientoX = 0;
        desplazamientoY = 0;

        if (ContenedorMapa != null)
        {
            ContenedorMapa.Scale = 1;
            ContenedorMapa.TranslationX = 0;
            ContenedorMapa.TranslationY = 0;
        }
    }

    //Limita el pan para que no se vaya fuera de la vision de la pantalla

    private void AjustarLimitesPan()
    {
        if (VisorMapa == null) return;

        double vw = VisorMapa.Width;
        double vh = VisorMapa.Height;
        if (vw <= 0 || vh <= 0) return;

        if (escalaActual <= 1.0001)
        {
            desplazamientoX = 0;
            desplazamientoY = 0;
            return;
        }
        //Contenido escalado
        double contenidoW = baseW * escalaActual;
        double contenidoH = baseH * escalaActual;

        //Si el contenido es más pequeño que el visor en un eje, centramos ese eje
        double maxX = Math.Max(0, (contenidoW - vw) / 2.0);
        double maxY = Math.Max(0, (contenidoH - vh) / 2.0);

        desplazamientoX = Math.Max(-maxX, Math.Min(maxX, desplazamientoX));
        desplazamientoY = Math.Max(-maxY, Math.Min(maxY, desplazamientoY));
    }
}