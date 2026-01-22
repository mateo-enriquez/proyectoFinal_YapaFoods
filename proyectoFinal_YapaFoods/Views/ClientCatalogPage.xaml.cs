using proyectoFinal_YapaFoods.ViewModels;

namespace proyectoFinal_YapaFoods.Views
{
    public partial class ClientCatalogPage : ContentPage
    {
        public ClientCatalogPage(ClientCatalogViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}