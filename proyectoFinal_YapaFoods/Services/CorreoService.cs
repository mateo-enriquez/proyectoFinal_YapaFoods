using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace proyectoFinal_YapaFoods.Services
{
    public class CorreoService
    {
        public async Task EnviarCorreo(string destinatario, string codigo)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("YapaFoods Project", "no-reply@yapafoods.com"));
            message.To.Add(new MailboxAddress("Usuario", destinatario));
            message.Subject = "Código de Verificación de Seguridad";

            message.Body = new TextPart("plain")
            {
                Text = $"Tu código de verificación es: {codigo}\nEste código expirará en 5 minutos."
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    // Recuerda poner tus credenciales reales de Mailtrap aquí
                    await client.ConnectAsync("sandbox.smtp.mailtrap.io", 2525, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("f3d5352d3d9956", "f32cf1e947b9d2");
                    await client.SendAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al enviar correo: {ex.Message}");
                    throw; // Re-lanzar para manejarlo en la UI si es necesario
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}