using InventarioActivos.Models.GestionLocalizaciones;
using InventarioActivos.ViewModels.Administrador;
namespace InventarioActivos.Administrador;

public partial class GestionLocalizacion : ContentPage
{
	private readonly GestionLocalizacionViewModel vm;
	public GestionLocalizacion(GestionLocalizacionViewModel viewModel)
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
		}
		catch (Exception ex)
		{
			if (Shell.Current != null)
				await Shell.Current.DisplayAlert("Error", ex.ToString(), "Aceptar");
		}
	}

	private void OnAddLocalizacionTapped(object sender, EventArgs e)
	{
		try
		{
			Image img = (Image)sender;
			ActivoLocalizacionItem item = (ActivoLocalizacionItem)img.BindingContext;
			vm.NuevaLocalizacionCommand.Execute(item);
		}
		catch (Exception ex)
		{
			if (Shell.Current != null)
				Shell.Current.DisplayAlert("Error", "No se pudo añadir la localizacion: " + ex.Message, "Aceptar");
		}
	}

	private void OnEditLocalizacionTapped(object sender, EventArgs e)
	{
		try
		{
			Image img = (Image)sender;
			ActivoLocalizacionItem item = (ActivoLocalizacionItem)img.BindingContext;
			vm.EditarLocalizacionCommand.Execute(item);
		}
		catch(Exception ex)
		{
            if (Shell.Current != null)
                Shell.Current.DisplayAlert("Error", "No se pudo editar la localizacion: " + ex.Message, "Aceptar");
        }
	}

	private void OnDeleteLocalizacionTapped(object sender, EventArgs e)
	{
		try
		{
			Image img = (Image)sender;
			ActivoLocalizacionItem item = (ActivoLocalizacionItem)img.BindingContext;
			vm.EliminarLocalizacionCommand.Execute(item);
		}
		catch (Exception ex)
		{
			if (Shell.Current != null)
				Shell.Current.DisplayAlert("Error", "No se pudo borrar la localizacion: " + ex.Message, "Aceptar");

		}
	}  


}