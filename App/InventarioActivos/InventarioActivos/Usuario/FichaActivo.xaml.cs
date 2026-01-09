using System;
using System.Collections.Generic;
using InventarioActivos.ViewModels.Usuario;

namespace InventarioActivos.Usuario;

public partial class FichaActivo : ContentPage, IQueryAttributable
{
	private readonly FichaActivoVewModel vm;
    private bool cargado;
	public FichaActivo(FichaActivoVewModel viewModel)
	{
		InitializeComponent();
        vm = viewModel;

		BindingContext = viewModel;
        cargado = false;
    }

    public  async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (vm == null) return;
        if (query == null) return;

        if (query.ContainsKey("IdActivo") == false) return;

        object valor = query["IdActivo"];
        if (valor == null) return;

        int id = 0;

        if (valor is int)
        {
            id = (int)valor;
        }
        else
        {
            string texto = valor.ToString();
            if (int.TryParse(texto, out id) == false) return;
        }

        if (id <= 0) return;

        vm.SetIdActivo(id);

        if (cargado) return;
        cargado = true;

        try
        {
            await vm.CargarAsync();
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync("Error al cargar los item del activo: " + ex.Message);

            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Error", "No se pudo cargar los item del activo: " + ex.Message, "Aceptar");
        }
    }



}
