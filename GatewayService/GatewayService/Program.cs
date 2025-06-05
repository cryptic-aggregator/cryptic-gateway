using GatewayService.DI;
using GatewayService.Interfaces.Config;
using GatewayService.Middleware;
using GatewayService.Services.Config;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Network;
using System.Net;



var builder = WebApplication.CreateBuilder(args);

var cfg = new ConfigService();

var loggerConfigService = new LoggingConfiguration();

// Робимо DI-запис для нашої конфігурації логування
builder.Services.AddSingleton<ILoggingConfiguration, LoggingConfiguration>();

// Тепер підключаємо Serilog як Host Logger
builder.Host.UseSerilog((ctx, services, loggerCfg) =>
{
    var logConfig = services.GetRequiredService<ILoggingConfiguration>();

    // Перетворюємо рядок на IPAddress
    if (!IPAddress.TryParse(logConfig.LogstashHost, out var ip))
        throw new Exception($"Cannot parse '{logConfig.LogstashHost}' as an IP address.");

    loggerCfg
        .MinimumLevel.Is(logConfig.MinimumLevel)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        )
        // Ось викликаємо перевантаження, яке приймає IPAddress + порт
        .WriteTo.TCPSink(
            ipAddress: ip,
            port: logConfig.LogstashPort,
            textFormatter: new CompactJsonFormatter(),
            restrictedToMinimumLevel: logConfig.MinimumLevel
        );
});

Log.Information("Starting up Gateway with Serilog…");

builder.Services.InjectConfiguration(cfg);
builder.Services.ConfigureMicroservices(cfg);
builder.Services.ConfigrePasswordResetService();
builder.Services.ConfigureRepositories();
builder.Services.ConfigureServices();

builder.Services.AddSingleton<IJwtConfiguration, JwtConfiguration>();

using (var sp = builder.Services.BuildServiceProvider())
{
    var jwtConfig = sp.GetRequiredService<IJwtConfiguration>();
    builder.Services.AddJwtAuthentication(jwtConfig);
}

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.  
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

Log.Information("=== Serilog ping test: це пробний запис у Logstash.");
Log.Warning("=== Serilog ping test Warning level.");
Log.Error("=== Serilog ping test Error level.");


var logger = app.Services.GetRequiredService<ILogger<Program>>();

Log.Information("Gateway started up in {Environment} environment.", builder.Environment.EnvironmentName);

// Налаштування HTTP конвеєру
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowAnyOrigin());

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserClaimsMiddleware>();
app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Gateway terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}
