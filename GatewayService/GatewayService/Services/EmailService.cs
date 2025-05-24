using GatewayService.Interfaces.Services;
using System.Net.Mail;
using System.Net;
using GatewayService.Interfaces.Config;

namespace GatewayService.Services;

public class EmailService : IEmailService
{
    private readonly IEmailConfiguration _emailConfig;

    public EmailService(IEmailConfiguration configuration)
    {
        _emailConfig = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpHost = _emailConfig.SmtpHost;
        var smtpPort = int.Parse(_emailConfig.SmtpPort);
        var smtpUser = _emailConfig.SmtpUsername;
        var smtpPass = _emailConfig.SmtpPassword;
        var from = _emailConfig.SmtpFrom;

        Console.WriteLine($"{smtpHost}\n {smtpPort}\n {smtpUser}\n {smtpPass}\n {from}");

        using (var client = new SmtpClient(smtpHost, smtpPort))
        {
            client.EnableSsl = true; // встановіть відповідно до вашого SMTP-сервера
            client.Credentials = new NetworkCredential(smtpUser, smtpPass);

            using (var message = new MailMessage(from, to, subject, body))
            {
                client.Send(message);
            }
        }
    }
}
