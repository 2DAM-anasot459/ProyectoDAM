using System.Threading.Tasks;

namespace InventarioActivos
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
           
        }

        protected override  Window CreateWindow(IActivationState activationState)
        {
            AppShell shell = new AppShell();
            Window window = new Window(shell);

            Dispatcher.Dispatch(async delegate
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.GoToAsync("//login");
                }
            });
            
            return window;
        }
    }
} 