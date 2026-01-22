using proyectoFinal_YapaFoods.ViewModels;

namespace proyectoFinal_YapaFoods.Views
{
    public partial class NearbyPage : ContentPage
    {
        private readonly NearbyViewModel _vm;

        public NearbyPage(NearbyViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Ejecutamos el comando de refrescar cada vez que aparece la pantalla
            // para actualizar la ubicación GPS y las distancias.
            if (_vm.RefreshCommand.CanExecute(null))
            {
                _vm.RefreshCommand.Execute(null);
            }
        }
    }
}