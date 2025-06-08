using Serilog.Events;

namespace GatewayService.Interfaces.Config;

public interface ILoggingConfiguration
{
    public string LogstashHost { get; }
    public int LogstashPort { get; }
    public LogEventLevel MinimumLevel { get; }
}
