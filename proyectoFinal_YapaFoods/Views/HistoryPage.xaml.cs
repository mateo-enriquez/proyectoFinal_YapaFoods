using proyectoFinal_YapaFoods.ViewModels;

namespace proyectoFinal_YapaFoods.Views
{
    public partial class HistoryPage : ContentPage
    {
        public HistoryPage(HistoryViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}