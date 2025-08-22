using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using DisasterReport.Services.Services.Interfaces;
namespace DisasterReport.Services.Services.Implementations
{
 
    public class EmailService : IEmailServices
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Disaster Report", _config["Email:From"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body };

            // Use fully qualified name to avoid ambiguity
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(_config["Email:SmtpServer"], int.Parse(_config["Email:Port"]), false);
            await smtp.AuthenticateAsync(_config["Email:Username"], _config["Email:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}