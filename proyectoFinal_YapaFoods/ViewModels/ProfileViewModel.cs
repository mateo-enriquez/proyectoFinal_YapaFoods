using System.Windows.Input;
using proyectoFinal_YapaFoods.Services;

namespace proyectoFinal_YapaFoods.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public ICommand GoToHistoryCommand { get; }
        public ICommand LogoutCommand { get; }

        public ProfileViewModel()
        {
            GoToHistoryCommand = new Command(async () => await Shell.Current.GoToAsync("HistoryPage"));

            LogoutCommand = new Command(async () =>
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert("Salir", "¿Cerrar sesión?", "Sí", "No");
                if (confirm)
                {
                    // Reiniciamos la navegación al Login
                    Application.Current.MainPage = new NavigationPage(new Views.LoginPage(new LoginViewModel(new DatabaseService())));

                    // Nota: Lo ideal es usar inyección de dependencias para reiniciar la app, 
                    // pero para este ejemplo simple, forzaremos la vista de Login.
                    // Una forma más limpia en MAUI Shell es:
                    // await Shell.Current.GoToAsync("//LoginPage");
                }
            });
        }
    }
}