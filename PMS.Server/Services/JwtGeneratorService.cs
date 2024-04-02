using PMS.Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PMS.Server.Options;
using Microsoft.Extensions.Options;

namespace PMS.Server.Services;

public sealed class JwtGeneratorService : IJwtGeneratorService
{
    public JwtGeneratorService(IOptions<JwtValidationOptions> validationOptions)
    {
        issuer = validationOptions.Value.Issuer;
        audience = validationOptions.Value.Audience;
        secret = validationOptions.Value.Secret;
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
