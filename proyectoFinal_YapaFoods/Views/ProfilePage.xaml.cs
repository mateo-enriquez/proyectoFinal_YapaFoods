using proyectoFinal_YapaFoods.ViewModels;

namespace proyectoFinal_YapaFoods.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage(ProfileViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}