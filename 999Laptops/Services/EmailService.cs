using System.Net;
using System.Net.Mail;
using LaptopEcommerceAndMore.Interfaces;

namespace LaptopEcommerceAndMore.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var server = _config["SmtpSettings:Server"];
            var port = int.Parse(_config["SmtpSettings:Port"]);
            var user = _config["SmtpSettings:Username"];
            var pass = _config["SmtpSettings:Password"];

            using (var client = new SmtpClient(server, port))
            {
                client.Credentials = new NetworkCredential(user, pass);
                client.EnableSsl = false;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("no-reply@electro.com", "Electro Security"),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true // Ð? b?n có th? g?i mail b?ng HTML cho d?p
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
