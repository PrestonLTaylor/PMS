using PMS.Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PMS.Server.Services;

public sealed class JwtGeneratorService : IJwtGeneratorService
{
    public JwtGeneratorService(IConfiguration configuration)
    {
        // NOTE: If we got here, we already know that these configuration values exist
        // FIXME: Have an IOptions<JwtOptions> injected
        issuer = configuration.GetValue<string>("jwt-issuer")!;
        audience = configuration.GetValue<string>("jwt-audience")!;
        secret = configuration.GetValue<string>("jwt-secret")!;
    }

    public string Generate(UserModel user)
    {
        var claims = CreateClaimsForUser(user);
        var tokenExpiration = CreateExpirationForToken();
        var signingCredentials = CreateSigningCredentials();

        var token = new JwtSecurityToken(issuer, audience, claims, null, tokenExpiration, signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private IEnumerable<Claim> CreateClaimsForUser(UserModel user)
    {
        return
        [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.UserName!)
        ];
    }

    private DateTime CreateExpirationForToken()
    {
        // FIXME: Have a configuration value for how long tokens last
        const int TOKEN_LIFETIME_IN_MINUTES = 30;
        return DateTime.UtcNow.AddMinutes(TOKEN_LIFETIME_IN_MINUTES);
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var secretBytes = Encoding.UTF8.GetBytes(secret);
        return new SigningCredentials(new SymmetricSecurityKey(secretBytes), SecurityAlgorithms.HmacSha256);
    }

    private readonly string issuer;
    private readonly string audience;
    private readonly string secret;
}
