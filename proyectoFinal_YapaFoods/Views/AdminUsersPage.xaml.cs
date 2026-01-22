using proyectoFinal_YapaFoods.ViewModels;

namespace proyectoFinal_YapaFoods.Views
{
    public partial class AdminUsersPage : ContentPage
    {
        AdminUsersViewModel _vm;
        public AdminUsersPage(AdminUsersViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vm.LoadData();
        }
    }
}