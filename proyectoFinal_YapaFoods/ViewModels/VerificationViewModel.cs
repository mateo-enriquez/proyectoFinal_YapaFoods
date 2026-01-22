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
    [QueryProperty(nameof(UserEmail), "email")]
    public class VerificationViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;
        private readonly CorreoService _correoService;
        private string _userEmail;
        private string _codigo;

        public VerificationViewModel(DatabaseService dbService, CorreoService correoService)
        {
            _dbService = dbService;
            _correoService = correoService;
            VerifyCommand = new Command(OnVerify);
            ResendCommand = new Command(OnResend);
        }

        public string UserEmail
        {
            get => _userEmail;
            set => SetProperty(ref _userEmail, value);
        }

        public string Codigo
        {
            get => _codigo;
            set => SetProperty(ref _codigo, value);
        }

        public ICommand VerifyCommand { get; }
        public ICommand ResendCommand { get; }

        private async void OnVerify()
        {
            if (string.IsNullOrWhiteSpace(Codigo))
            {
                await ShowAlert("Error", "Ingresa el código de verificación");
                return;
            }

            IsBusy = true;
            try
            {
                var conn = await _dbService.GetConnectionAsync();

                // 1. Buscar el OTP más reciente para este correo
                var registroOtp = await conn.Table<OTP>()
                                    .Where(o => o.Correo == UserEmail)
                                    .OrderByDescending(o => o.Id) // El más nuevo
                                    .FirstOrDefaultAsync();

                if (registroOtp == null)
                {
                    await ShowAlert("Error", "No se encontró un código para este usuario.");
                    IsBusy = false; return;
                }

                // 2. Validar coincidencia y expiración
                if (registroOtp.Codigo != Codigo)
                {
                    await ShowAlert("Error", "Código incorrecto.");
                    IsBusy = false; return;
                }

                if (registroOtp.Expiracion < DateTime.Now)
                {
                    await ShowAlert("Error", "El código ha expirado. Solicita uno nuevo.");
                    IsBusy = false; return;
                }

                // 3. ¡Éxito! Verificar al usuario
                var usuario = await conn.Table<Usuario>().Where(u => u.Correo == UserEmail).FirstOrDefaultAsync();
                if (usuario != null)
                {
                    usuario.IsVerified = true;
                    await conn.UpdateAsync(usuario);

                    await ShowAlert("Verificado", "Tu cuenta ha sido activada exitosamente.");

                    // Redirigir al Login o Dashboard. Por seguridad, volvemos al login.
                    await Shell.Current.GoToAsync("//LoginPage");
                }
            }
            catch (Exception ex)
            {
                await ShowAlert("Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnResend()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var random = new Random();
                string nuevoCodigo = random.Next(100000, 999999).ToString();

                var conn = await _dbService.GetConnectionAsync();
                var otpRecord = new OTP
                {
                    Correo = UserEmail,
                    Codigo = nuevoCodigo,
                    Expiracion = DateTime.Now.AddMinutes(5),
                    UltimoEnvio = DateTime.Now
                };
                await conn.InsertAsync(otpRecord);
                await _correoService.EnviarCorreo(UserEmail, nuevoCodigo);
                await ShowAlert("Enviado", "Se ha enviado un nuevo código a tu correo.");
            }
            catch (Exception ex)
            {
                await ShowAlert("Error", $"No se pudo enviar: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ShowAlert(string title, string msg) =>
            await Application.Current.MainPage.DisplayAlert(title, msg, "OK");
    }
}
