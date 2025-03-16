namespace GatewayService.Interfaces.Config;

public interface IEmailConfiguration
{
    public string SmtpHost { get; set; }
    public string SmtpPort { get; set; }
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }
    public string SmtpFrom { get; set; }
}
