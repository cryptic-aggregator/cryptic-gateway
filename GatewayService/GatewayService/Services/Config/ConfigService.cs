using System.Text;
using Cryptic_Domain.Database.Config.Interfaces;
using GatewayService.Interfaces.Config;

namespace GatewayService.Services.Config;

public class ConfigService : IDatabaseConfiguration, IMicroservicesConfig
{
    public ConfigService() 
    {
        ConnString = Environment.GetEnvironmentVariable(nameof(this.ConnString)) ?? throw DrawAllConfigVars();
        Schema = $"\"{Environment.GetEnvironmentVariable(nameof(this.Schema))}\"" ?? throw DrawAllConfigVars();
        JwtSecret = Environment.GetEnvironmentVariable(nameof(this.JwtSecret)) ?? throw DrawAllConfigVars();
        
        BlockchainInteractionConnString = Environment.GetEnvironmentVariable(nameof(this.BlockchainInteractionConnString)) ?? throw DrawAllConfigVars();
        PortfolioConfigurationConnString = Environment.GetEnvironmentVariable(nameof(this.PortfolioConfigurationConnString)) ?? throw DrawAllConfigVars();
    }
    
    public string ConnString { get; private set; }
    
    public string Schema { get; private set; }
    
    public string JwtSecret {get; private set;}
    
    public string BlockchainInteractionConnString { get; private set; }
    
    public string PortfolioConfigurationConnString { get; private set; }

    private Exception DrawAllConfigVars()
    {
        var configType = this.GetType();
        var props = configType.GetProperties();
        var sb = new StringBuilder("\nYou must provide all undefined variables by environment\n");
        foreach (var prop in props)
        {
            sb.AppendLine($"available setting {prop.Name}, with type: {prop.PropertyType.Name}, current val = {prop.GetValue(this)}");
        }
        return new Exception(sb.ToString());
    }
}