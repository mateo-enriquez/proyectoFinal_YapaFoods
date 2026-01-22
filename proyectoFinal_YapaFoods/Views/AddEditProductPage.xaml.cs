using proyectoFinal_YapaFoods.ViewModels;

namespace proyectoFinal_YapaFoods.Views
{
    public partial class AddEditProductPage : ContentPage
    {
        public AddEditProductPage(AddEditProductViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}