namespace InventarioActivos.Autenticacion;
using ViewModels;
using Microsoft.Maui.Controls;

public partial class Login : ContentPage
{
	public Login(LoginViewModel loginViewModel)
	{
		InitializeComponent();

		BindingContext = loginViewModel;
    }

	

	
} 