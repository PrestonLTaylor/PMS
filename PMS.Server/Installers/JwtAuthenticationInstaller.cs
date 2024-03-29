using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace PMS.Server.Installers;

public static class JwtAuthenticationInstaller
{
    public static void AddJwtBasedAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var issuer = GetIssuerFromConfiguration(configuration);
        var audience = GetAudienceFromConfiguration(configuration);
        var signingKey = GetSigningKeyFromConfiguration(configuration);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    // Default clock skew is 5 minutes, so it extends our token's lifetimes, which we don't want
                    ClockSkew = TimeSpan.Zero,
                };
            });
    }

    private static string GetIssuerFromConfiguration(IConfiguration configuration)
    {
        var issuer = configuration.GetValue<string>(JWT_ISSUER_VARIABLE);
        if (issuer is null)
        {
            Log.Fatal("Unable to find JWT issuer variable '{JWTIssuerVariable}'.", JWT_ISSUER_VARIABLE);
            throw new InvalidOperationException($"Unable to find JWT issuer variable '{JWT_ISSUER_VARIABLE}'.");
        }

        return issuer;
    }

    private static string GetAudienceFromConfiguration(IConfiguration configuration)
    {
        var audience = configuration.GetValue<string>(JWT_AUDIENCE_VARIABLE);
        if (audience is null)
        {
            Log.Fatal("Unable to find JWT audience variable '{JWTAudienceVariable}'.", JWT_AUDIENCE_VARIABLE);
            throw new InvalidOperationException($"Unable to find JWT audience variable '{JWT_AUDIENCE_VARIABLE}'.");
        }

        return audience;
    }

    private static SymmetricSecurityKey GetSigningKeyFromConfiguration(IConfiguration configuration)
    {
        var secret = configuration.GetValue<string>(JWT_SECRET_VARIABLE);
        if (secret is null)
        {
            Log.Fatal("Unable to find JWT secret variable '{JWTSecretVariable}'.", JWT_SECRET_VARIABLE);
            throw new InvalidOperationException($"Unable to find JWT secret variable '{JWT_SECRET_VARIABLE}'.");
        }

        var keyAsBytes = Encoding.UTF8.GetBytes(secret);
        return new SymmetricSecurityKey(keyAsBytes);
    }

    const string JWT_ISSUER_VARIABLE = "jwt-issuer";
    const string JWT_AUDIENCE_VARIABLE = "jwt-audience";
    const string JWT_SECRET_VARIABLE = "jwt-secret";
}
