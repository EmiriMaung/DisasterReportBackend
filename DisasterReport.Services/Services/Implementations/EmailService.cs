using DisasterReport.Services.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
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
            email.From.Add(new MailboxAddress("Hands of Hope", _config["Email:From"]));
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

        public async Task SendOtpEmailAsync(string toEmail, string otpCode)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_config["Email:From"]));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = "Your Login Code";

                var builder = new BodyBuilder();
                builder.HtmlBody = $"<p>Your one-time passcode is:</p><h2>{otpCode}</h2>";
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();

                await smtp.ConnectAsync(_config["Email:SmtpServer"], int.Parse(_config["Email:Port"]), SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(_config["Email:Username"], _config["Email:Password"]);

                await smtp.SendAsync(email);

                await smtp.DisconnectAsync(true);

                Console.WriteLine($"Successfully sent OTP to {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}