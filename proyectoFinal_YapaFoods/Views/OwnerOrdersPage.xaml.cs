using proyectoFinal_YapaFoods.ViewModels;

namespace proyectoFinal_YapaFoods.Views
{
    public partial class OwnerOrdersPage : ContentPage
    {
        public OwnerOrdersPage(OwnerOrdersViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}