using proyectoFinal_YapaFoods.ViewModels;
namespace proyectoFinal_YapaFoods.Views
{
    public partial class AddEditLocalPage : ContentPage
    {
        public AddEditLocalPage(AddEditLocalViewModel vm) { InitializeComponent(); BindingContext = vm; }
    }
}