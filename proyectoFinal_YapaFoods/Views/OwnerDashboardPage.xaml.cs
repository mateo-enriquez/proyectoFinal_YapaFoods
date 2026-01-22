using proyectoFinal_YapaFoods.ViewModels;

namespace proyectoFinal_YapaFoods.Views
{
    public partial class OwnerDashboardPage : ContentPage
    {
        private readonly OwnerLocalesViewModel _vm;

        public OwnerDashboardPage(OwnerLocalesViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Recargamos los datos cada vez que entra a la pantalla
            // por si agregó un local nuevo o cambió el estado.
            _vm.LoadCommand.Execute(null);
        }
    }
}