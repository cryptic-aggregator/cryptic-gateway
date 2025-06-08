using GatewayService.Interfaces.Config;
using Serilog.Events;

namespace GatewayService.Services.Config;

public class LoggingConfiguration : ILoggingConfiguration
{
    public LoggingConfiguration()
    {
        var host = Environment.GetEnvironmentVariable("LogstashHost")
                   ?? throw new Exception("Logstash__Host is missing in environment variables");
        var portText = Environment.GetEnvironmentVariable("LogstashPort")
                       ?? throw new Exception("Logstash__Port is missing in environment variables");
        var levelText = Environment.GetEnvironmentVariable("LogstashMinimumLevel")
                        ?? throw new Exception("Logstash__MinimumLevel is missing in environment variables");

        if (!int.TryParse(portText, out var port))
            throw new Exception("Logstash__Port is not a valid integer");

        if (!Enum.TryParse<Serilog.Events.LogEventLevel>(levelText, ignoreCase: true, out var minLevel))
            throw new Exception("Logstash__MinimumLevel is not a valid Serilog LogEventLevel");

        LogstashHost = host;
        LogstashPort = port;
        MinimumLevel = minLevel;
    }

    public string LogstashHost { get; }
    public int LogstashPort { get; }
    public LogEventLevel MinimumLevel { get; }

    
}

