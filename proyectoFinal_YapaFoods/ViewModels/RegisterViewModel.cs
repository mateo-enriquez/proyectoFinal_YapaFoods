using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using proyectoFinal_YapaFoods.Models;
using proyectoFinal_YapaFoods.Services;

namespace proyectoFinal_YapaFoods.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;
        private readonly CorreoService _correoService;

        private string _nombre;
        private string _correo;
        private string _password;
        private string _confirmPassword;

        public RegisterViewModel(DatabaseService dbService, CorreoService correoService)
        {
            _dbService = dbService;
            _correoService = correoService;
            RegisterCommand = new Command(OnRegisterClicked);
            LoginCommand = new Command(async () => await Shell.Current.GoToAsync("..")); // Volver atrás
        }

        public string Nombre { get => _nombre; set => SetProperty(ref _nombre, value); }
        public string Correo { get => _correo; set => SetProperty(ref _correo, value); }
        public string Password { get => _password; set => SetProperty(ref _password, value); }
        public string ConfirmPassword { get => _confirmPassword; set => SetProperty(ref _confirmPassword, value); }

        public ICommand RegisterCommand { get; }
        public ICommand LoginCommand { get; }

        private async void OnRegisterClicked()
        {
            if (IsBusy) return;
            IsBusy = true;

            // 1. Validaciones básicas
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Correo) ||
                string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await ShowAlert("Error", "Todos los campos son obligatorios");
                IsBusy = false; return;
            }

            if (Password != ConfirmPassword)
            {
                await ShowAlert("Error", "Las contraseñas no coinciden");
                IsBusy = false; return;
            }

            // Validar complejidad (1 Mayúscula, 1 Número) - Implementación simple
            if (!System.Text.RegularExpressions.Regex.IsMatch(Password, @"^(?=.*[A-Z])(?=.*\d).+$"))
            {
                await ShowAlert("Seguridad", "La contraseña debe tener al menos una mayúscula y un número.");
                IsBusy = false; return;
            }

            // 2. Verificar si correo ya existe
            var connection = await _dbService.GetConnectionAsync();
            var existente = await connection.Table<Usuario>()
                                .Where(u => u.Correo == Correo).FirstOrDefaultAsync();
            if (existente != null)
            {
                await ShowAlert("Error", "El correo ya está registrado");
                IsBusy = false; return;
            }

            try
            {
                // 3. Crear Usuario (Hash Password)
                var nuevoUsuario = new Usuario
                {
                    Nombre = Nombre,
                    Correo = Correo,
                    Password = BCrypt.Net.BCrypt.HashPassword(Password),
                    RolId = 4, // 4 = Cliente (según tu SeedData)
                    IsVerified = false
                };

                await connection.InsertAsync(nuevoUsuario);

                // 4. Generar OTP
                var random = new Random();
                string codigoOtp = random.Next(100000, 999999).ToString();

                var otpRecord = new OTP
                {
                    Correo = Correo,
                    Codigo = codigoOtp,
                    Expiracion = DateTime.Now.AddMinutes(5),
                    UltimoEnvio = DateTime.Now
                };
                await connection.InsertAsync(otpRecord);

                // 5. Enviar Correo
                await _correoService.EnviarCorreo(Correo, codigoOtp);

                await ShowAlert("Éxito", "Cuenta creada. Por favor verifica tu código enviado al correo.");

                // 6. Navegar a verificación (Implementaremos esta vista luego)
                // await Shell.Current.GoToAsync($"{nameof(VerificationPage)}?correo={Correo}");

                // Por ahora, volvemos al login para probar
                await Shell.Current.GoToAsync($"VerificationPage?email={Correo}");
            }
            catch (Exception ex)
            {
                await ShowAlert("Error", $"Ocurrió un error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ShowAlert(string title, string message) =>
            await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }
}