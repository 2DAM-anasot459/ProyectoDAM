using CommunityToolkit.Mvvm.ComponentModel;

namespace InventarioActivos.ViewModels;

public partial class BaseViewModel : ObservableObject
{
	private bool isBusy;
	public bool IsBusy
	{
		get { return isBusy; }
		set { SetProperty(ref isBusy, value); }

	}

	private string title = "";
	public string Title
	{
		get { return title; }
		set { SetProperty(ref title, value); }
    }
}