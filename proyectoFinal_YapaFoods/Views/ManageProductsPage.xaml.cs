using proyectoFinal_YapaFoods.ViewModels;
namespace proyectoFinal_YapaFoods.Views
{
    public partial class ManageProductsPage : ContentPage
    {
        ManageProductsViewModel _vm;
        public ManageProductsPage(ManageProductsViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }
        protected override void OnAppearing() { base.OnAppearing(); _vm.LoadCommand.Execute(null); }
    }
}       