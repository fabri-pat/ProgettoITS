using System.Net.Mail;
using app.Helpers;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace app.Services
{
    public class EmailService : IEMailService
    {
        private readonly MailSettings appSettings;

        public EmailService(IOptions<MailSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }
        public async Task SendAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(System.Text.Encoding.UTF8, appSettings.DisplayName, appSettings.EmailFrom));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };


            var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(appSettings.SmtpHost, appSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(appSettings.SmtpUser, appSettings.SmtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}