using proyectoFinal_YapaFoods.ViewModels;

namespace proyectoFinal_YapaFoods.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}