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
    public class ForgotPasswordViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;
        private readonly CorreoService _correoService;

        private string _correo;
        private string _codigo;
        private string _newPassword;
        private bool _isCodeSent; // Controla si mostramos el campo de código y nueva pass

        public ForgotPasswordViewModel(DatabaseService dbService, CorreoService correoService)
        {
            _dbService = dbService;
            _correoService = correoService;
            SendCommand = new Command(OnSendCode);
            ResetCommand = new Command(OnResetPassword);
            IsCodeSent = false;
        }

        public string Correo { get => _correo; set => SetProperty(ref _correo, value); }
        public string Codigo { get => _codigo; set => SetProperty(ref _codigo, value); }
        public string NewPassword { get => _newPassword; set => SetProperty(ref _newPassword, value); }

        public bool IsCodeSent
        {
            get => _isCodeSent;
            set => SetProperty(ref _isCodeSent, value);
        }

        public ICommand SendCommand { get; }
        public ICommand ResetCommand { get; }

        private async void OnSendCode()
        {
            if (string.IsNullOrWhiteSpace(Correo)) return;
            IsBusy = true;

            var conn = await _dbService.GetConnectionAsync();
            var user = await conn.Table<Usuario>().Where(u => u.Correo == Correo).FirstOrDefaultAsync();

            if (user == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Correo no registrado", "OK");
                IsBusy = false; return;
            }

            // Generar OTP
            var random = new Random();
            string otp = random.Next(100000, 999999).ToString();

            var otpRecord = new OTP { Correo = Correo, Codigo = otp, Expiracion = DateTime.Now.AddMinutes(5), UltimoEnvio = DateTime.Now };
            await conn.InsertAsync(otpRecord);

            await _correoService.EnviarCorreo(Correo, otp);

            await Application.Current.MainPage.DisplayAlert("Enviado", "Revisa tu correo", "OK");
            IsCodeSent = true; // Esto habilitará la segunda parte del formulario
            IsBusy = false;
        }

        private async void OnResetPassword()
        {
            if (string.IsNullOrWhiteSpace(Codigo) || string.IsNullOrWhiteSpace(NewPassword)) return;
            IsBusy = true;

            var conn = await _dbService.GetConnectionAsync();
            var otpRecord = await conn.Table<OTP>()
                                .Where(o => o.Correo == Correo)
                                .OrderByDescending(o => o.Id).FirstOrDefaultAsync();

            if (otpRecord == null || otpRecord.Codigo != Codigo || otpRecord.Expiracion < DateTime.Now)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Código inválido o expirado", "OK");
                IsBusy = false; return;
            }

            // Actualizar Password
            var user = await conn.Table<Usuario>().Where(u => u.Correo == Correo).FirstAsync();
            user.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            // Aprovechamos de verificarlo si no lo estaba
            user.IsVerified = true;

            await conn.UpdateAsync(user);

            await Application.Current.MainPage.DisplayAlert("Éxito", "Contraseña restablecida. Inicia sesión.", "OK");
            await Shell.Current.GoToAsync(".."); // Volver al Login
            IsBusy = false;
        }
    }
}

