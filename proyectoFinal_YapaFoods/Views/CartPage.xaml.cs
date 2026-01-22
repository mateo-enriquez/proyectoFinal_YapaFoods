using proyectoFinal_YapaFoods.ViewModels;

namespace proyectoFinal_YapaFoods.Views
{
    public partial class CartPage : ContentPage
    {
        CartViewModel _vm;

        public CartPage(CartViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Forzamos el recálculo del total cada vez que entramos al carrito
            _vm.RecalcularTotal();
        }
    }
}