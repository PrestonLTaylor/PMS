using PMS.Lib.Services;
using System.IdentityModel.Tokens.Jwt;

namespace PMS.Lib.UnitTests;

internal sealed class TokenServiceTests
{
    [Test]
    public void Expiration_MustBeLessThatNow_WhenCreatingTokenService()
    {
        // Arrange
        var service = new TokenService();

        // Act

        // Assert
        Assert.That(service.Expiration, Is.LessThan(DateTime.UtcNow));
    }

    [Test]
    public void Expiration_MustHaveTheSameExpirationAsProvidedToken_AfterSettingToken()
    {
        // Arrange
        var token = new JwtSecurityToken(null, null, null, null, DateTime.UtcNow, null);
        var expectedExpiration = token.ValidTo;
        var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

        var service = new TokenService();

        // Act
        service.Token = encodedToken;

        // Assert
        Assert.That(service.Expiration, Is.EqualTo(expectedExpiration));
    }
}
