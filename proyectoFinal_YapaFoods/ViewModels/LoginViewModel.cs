using System.Windows.Input;
using proyectoFinal_YapaFoods.Models;
using proyectoFinal_YapaFoods.Services;
using proyectoFinal_YapaFoods.Views;

namespace proyectoFinal_YapaFoods.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;
        private string _correo;
        private string _password;

        public LoginViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
            LoginCommand = new Command(OnLoginClicked);
            RegisterCommand = new Command(OnRegisterClicked);
            // 1. NUEVO: Comando para recuperar contraseña
            ForgotPasswordCommand = new Command(async () => await Shell.Current.GoToAsync("ForgotPasswordPage"));
        }

        public string Correo
        {
            get => _correo;
            set => SetProperty(ref _correo, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; } // 2. NUEVO

        private async void OnLoginClicked()
        {
            if (string.IsNullOrWhiteSpace(Correo) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Ingrese correo y contraseña", "OK");
                return;
            }

            IsBusy = true;

            var connection = await _dbService.GetConnectionAsync();
            var usuario = await connection.Table<Usuario>()
                            .Where(u => u.Correo == Correo)
                            .FirstOrDefaultAsync();

            IsBusy = false;

            if (usuario == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Usuario o contraseña incorrectos", "OK");
                return;
            }

            bool passwordValido = false;
            try
            {
                passwordValido = BCrypt.Net.BCrypt.Verify(Password, usuario.Password);
            }
            catch (Exception)
            {
                passwordValido = false;
            }

            if (!passwordValido)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Usuario o contraseña incorrectos", "OK");
                return;
            }

            // 3. CAMBIO IMPORTANTE: Bloque de verificación actualizado
            if (!usuario.IsVerified)
            {
                bool respuesta = await Application.Current.MainPage.DisplayAlert(
                    "Cuenta no verificada",
                    "Tu cuenta requiere verificación. ¿Deseas ingresar el código ahora?",
                    "Sí", "Cancelar");

                if (respuesta)
                {
                    await Shell.Current.GoToAsync($"VerificationPage?email={usuario.Correo}");
                }
                return;
            }

            // LOGIN EXITOSO - Redirección según Roles
            OwnerLocalesViewModel.UsuarioIdActual = usuario.Id;

            if (usuario.RolId == 1 || usuario.RolId == 2) // Admin
            {
                await Shell.Current.GoToAsync("AdminDashboardPage");
            }
            else if (usuario.RolId == 3) // Dueño
            {
                await Shell.Current.GoToAsync("OwnerDashboardPage");
            }
            else
            {
                // Cliente
                Application.Current.MainPage = new AppShell(); // Reiniciamos Shell para limpiar navegación anterior
                await Shell.Current.GoToAsync("//ClientDashboardPage"); // Debes crear esta ruta en AppShell.xaml como Tab o ShellContent
            }
        }

        private async void OnRegisterClicked()
        {
            await Shell.Current.GoToAsync("RegisterPage");
        }
    }
}