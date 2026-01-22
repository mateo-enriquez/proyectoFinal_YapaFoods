using proyectoFinal_YapaFoods.ViewModels;

namespace proyectoFinal_YapaFoods.Views
{
    public partial class ClientDashboardPage : ContentPage
    {
        // Guardamos una referencia al ViewModel por si necesitamos usar sus métodos (como LoadData)
        ClientDashboardViewModel _vm;

        // Inyectamos el ViewModel en el constructor
        public ClientDashboardPage(ClientDashboardViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        // Cada vez que aparece la pantalla, recargamos los locales (útil si el admin aprobó uno nuevo mientras navegábamos)
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.LoadData();
        }
    }
}