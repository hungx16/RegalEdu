using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Settings;

namespace RegalEdu.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            var message = new MimeMessage ( );

            // From
            message.From.Add (new MailboxAddress (_emailSettings.FromName, _emailSettings.FromEmail));
            // To
            message.To.Add (MailboxAddress.Parse (toEmail));
            // Subject
            message.Subject = subject;

            // Body
            if (isHtml)
                message.Body = new TextPart (MimeKit.Text.TextFormat.Html) { Text = body };
            else
                message.Body = new TextPart (MimeKit.Text.TextFormat.Plain) { Text = body };

            using var client = new SmtpClient ( );

            // Kết nối đến SMTP
            var secureSocket = _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
            await client.ConnectAsync (_emailSettings.SmtpServer, _emailSettings.SmtpPort, secureSocket);

            // Đăng nhập
            await client.AuthenticateAsync (_emailSettings.SmtpUser, _emailSettings.SmtpPassword);

            // Gửi mail
            await client.SendAsync (message);

            // Ngắt kết nối
            await client.DisconnectAsync (true);
        }
    }
}
