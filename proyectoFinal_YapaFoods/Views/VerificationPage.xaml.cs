using proyectoFinal_YapaFoods.ViewModels;
namespace proyectoFinal_YapaFoods.Views
{
    public partial class VerificationPage : ContentPage
    {
        public VerificationPage(VerificationViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}