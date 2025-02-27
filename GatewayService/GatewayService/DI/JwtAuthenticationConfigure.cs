using Microsoft.AspNetCore.Authentication.JwtBearer;
using GatewayService.Interfaces.Config;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GatewayService.DI;

public static class JwtAuthenticationConfigure
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IJwtConfiguration jwtConfig)
    {
        var key = Encoding.UTF8.GetBytes(jwtConfig.JwtSecret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // для production бажано true
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}
