using GatewayService.Interfaces.Config;

namespace GatewayService.Services.Config;

public class EmailConfiguration : IEmailConfiguration
{
    public EmailConfiguration()
    {
        SmtpHost = Environment.GetEnvironmentVariable("SmtpHost") ?? throw new Exception("SmtpHost is missing in environment variables.");
        SmtpPort = Environment.GetEnvironmentVariable("SmtpPort") ?? throw new Exception("SmtpPort is missing in environment variables.");
        SmtpUsername = Environment.GetEnvironmentVariable("SmtpUsername") ?? throw new Exception("SmtpUsername is missing in environment variables.");
        SmtpPassword = Environment.GetEnvironmentVariable("SmtpPassword") ?? throw new Exception("SmtpPassword is missing in environment variables.");
        SmtpFrom = Environment.GetEnvironmentVariable("SmtpFrom") ?? throw new Exception("SmtpFrom is missing in environment variables.");
    }

    public string SmtpHost { get; set; }
    public string SmtpPort { get; set; }
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }
    public string SmtpFrom { get; set; }
}
