using InventarioActivos.Models.GestionLocalizaciones;
using InventarioActivos.ViewModels.Usuario;
using Microsoft.Maui.Layouts;
namespace InventarioActivos.Usuario;

public partial class Mapa : ContentPage
{
    private readonly MapaViewModel vm;
    //Variables para poder hacer zoom en la imagen y arrastrar (pan) la imagen es todas las direcciones
    private double escalaActual = 1;
    private double escalaInicial = 1;

    private double desplazamientoX = 0;
    private double desplazamientoY = 0;

    private double panInicioX = 0;
    private double panInicioY = 0;

    //Variables para definir el tamaño real de la imagen
    private bool estaHaciendoPinch = false;
    private const double MAP_W = 7205.0;
    private const double MAP_H = 4606.0;
    private const double MAP_RATIO = MAP_W / MAP_H;

    private double baseW, baseH, baseX, baseY;
    public Mapa(MapaViewModel viewModel)
    {
        InitializeComponent();

        vm = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            await vm.CargarAsync();
            DibujarChinchetas();
        }
        catch (Exception ex)
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Error", "No se pudo cargar: " + ex.Message, "Aceptar");
        }
    }

    //Permite ajustar al tamaño de la pantalla
    private void OnVisorMapaSizeChanged(object sender, EventArgs e)
    {
        LayoutBaseRest();
        DibujarChinchetas();
        OcultarPopup();
    }

    //Con este metodo ajustamos el mapa sin deformarlo
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

        PanelChinchetas.WidthRequest = baseW;
        PanelChinchetas.HeightRequest = baseH;

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

    //Muestra la imagen de la chinceta en el punto exacto
    private void DibujarChinchetas()
    {
        if(PanelChinchetas == null || vm == null) return;

        PanelChinchetas.Children.Clear();

        if (baseW <= 0 || baseH <= 0) return;

        for (int i = 0; i < vm.Activos.Count; i++)
        {
            ActivoLocalizacionItem item = vm.Activos[i];

            double x = item.Longitud * baseW;
            double y = item.Latitud * baseH;

            Image pin = new Image();
            pin.Source = "chincheta.png";
            pin.WidthRequest = 32;
            pin.HeightRequest = 32;

            double izquierda = x - 16;
            double arriba = y - 32;

            AbsoluteLayout.SetLayoutBounds(pin, new Rect(izquierda, arriba, 32, 32));
            AbsoluteLayout.SetLayoutFlags(pin, AbsoluteLayoutFlags.None);

            //Permite hacer click sobre la chincheta 
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += OnPinTapped;
            pin.GestureRecognizers.Add(tap);

            pin.BindingContext = item;

            PanelChinchetas.Children.Add(pin);
        }

    }

    //Traemos la información del activo seleccionado y se abre el poopup
    private void OnPinTapped(object sender, TappedEventArgs e)
    {
        Image pin = sender as Image;
        if ( pin == null ) return;
        
        ActivoLocalizacionItem item = pin.BindingContext as ActivoLocalizacionItem;
        if ( item == null ) return;

        vm.ActivoSeleccionado = item;

        double x = item.Longitud * baseW;
        double y = item.Latitud * baseH;

        MostrarPopupMapa(x, y);
    } 

    //Muestra el popup y calculamos para situarlo cerca de la chincheta y que no se salga del mapa
    private void MostrarPopupMapa(double mapaX, double mapaY)
    {
        if(OverlayPopup == null || PopupActivo == null || ContenedorMapa == null) return;

        OverlayPopup.IsVisible = true;
        PopupActivo.IsVisible = true;

        double popupW = 280;
        double popupH = 190;

        double left = mapaX - (popupW / 2.0);
        double top = mapaY - 32 - popupH - 10;

        if(left < 0) left = 0;
        if((left + popupW) > baseW) left = baseW - popupW;

        if(top < 0) top = 0;
        if((top + popupH) > baseH) top = baseH - popupH;

        AbsoluteLayout.SetLayoutBounds(PopupActivo, new Rect(left, top, popupW, popupH));
        AbsoluteLayout.SetLayoutFlags(PopupActivo, AbsoluteLayoutFlags.None);
    }

    //Al tocar en cualquier parte de la pantalla se cierrra el popup 
    //Exeptuando el propio popup para poder pinchar sobre los botones del popup
    private void OcultarPopup()
    {
        if (OverlayPopup == null || PopupActivo == null) return;
        PopupActivo.IsVisible = false;
        OverlayPopup.IsVisible = false;
    }

    private void OnCerrarPopupTapped (object sender, TappedEventArgs e)
    {
        OcultarPopup();
    }

    private bool bloquearCierrePopTap = false;

    private void OnPopupTapped(object sender, TappedEventArgs e)
    {
        bloquearCierrePopTap = true;
    }

    private void OnMapaTapped(object sender, TappedEventArgs e)
    {
        CerrarPopup();
    } 

    private void OnPantallaTapped(object sender, TappedEventArgs e)
    {
        CerrarPopup();
    }

    private void CerrarPopup()
    {
        if (bloquearCierrePopTap)
        {
            bloquearCierrePopTap = false;
            return;
        }

        OcultarPopup();
    }
    //Doble tap para resetear el tamaño de la imagen al original
    private void OnMapaDoubleTapped(object sender, TappedEventArgs e)
    {
        ResetZoomPan();
        DibujarChinchetas();
        OcultarPopup();
    }
    //PAN para desplazar mapa
    private void OnPanUpdate(object sender, PanUpdatedEventArgs e)
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
    private void OnPinchUpdate(object sender, PinchGestureUpdatedEventArgs e)
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