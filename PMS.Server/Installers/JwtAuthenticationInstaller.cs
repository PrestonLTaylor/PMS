using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PMS.Server.Options;
using PMS.Server.Services;
using System.Text;

namespace PMS.Server.Installers;

public static class JwtAuthenticationInstaller
{
    public static void AddJwtBasedAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection(nameof(JwtValidationOptions));
        services.Configure<JwtValidationOptions>(section);

        var jwtValidationOptions = section.Get<JwtValidationOptions>()!;
        var signingKey = CreateSigningKeyFromSecret(jwtValidationOptions.Secret);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtValidationOptions.Issuer,
                    ValidAudience = jwtValidationOptions.Audience,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    // Default clock skew is 5 minutes, so it extends our token's lifetimes, which we don't want
                    ClockSkew = TimeSpan.Zero,
                    // Every time there is an authentication request, we want the latest token validation parameters from our configuration
                };
            });

        services.AddTransient<IJwtGeneratorService, JwtGeneratorService>();
    }

    private static SymmetricSecurityKey CreateSigningKeyFromSecret(string secret)
    {
        var keyAsBytes = Encoding.UTF8.GetBytes(secret);
        return new SymmetricSecurityKey(keyAsBytes);
    }
}
