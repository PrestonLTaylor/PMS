using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PMS.Server.IntegrationTests.Helpers;
using PMS.Server.Models;
using PMS.Services.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PMS.Server.IntegrationTests;

internal sealed class LoginServiceTests : GrpcIntergrationBase
{
    [Test]
    public async Task Login_ReturnsValidJwtToken_WhenProvidingValidUsernameAndPassword()
    {
        // Arrange
        const string validUsername = "valid";
        const string validPassword = "valid";

        var channel = CreateGrpcChannel();

        var userManager = services!.GetRequiredService<UserManager<UserModel>>();
        await userManager.CreateAsync(new UserModel(validUsername), validPassword);

        var client = new Login.LoginClient(channel);

        // Act
        var response = await client.LoginAsync(new LoginCredentials { Username = validUsername, Password = validPassword });

        // Assert
        await AssertThatJwtTokenIsValidAsync(services!.GetRequiredService<IConfiguration>(), response.Token);
    }

    [Test]
    public void Login_ThrowsGrpcUnauthenticated_WhenProvidingInvalidUsername()
    {
        // Arrange
        const string invalidUsername = "invalid";

        var channel = CreateGrpcChannel();

        var request = new LoginCredentials { Username = invalidUsername, Password = "" };
        var client = new Login.LoginClient(channel);

        // Act
        var exception = Assert.Throws<RpcException>(() => client.Login(request));

        // Assert
        Assert.That(exception.StatusCode, Is.EqualTo(StatusCode.Unauthenticated));
    }

    [Test]
    public async Task Login_ThrowsGrpcUnauthenticated_WhenProvidingInvalidPassword()
    {
        // Arrange
        const string validUsername = "valid";
        const string invalidPassword = "invalid";

        var channel = CreateGrpcChannel();

        var userManager = services!.GetRequiredService<UserManager<UserModel>>();
        await userManager.CreateAsync(new UserModel(validUsername), invalidPassword + "different");

        var request = new LoginCredentials { Username = validUsername, Password = invalidPassword };
        var client = new Login.LoginClient(channel);

        // Act
        var exception = Assert.Throws<RpcException>(() => client.Login(request));

        // Assert
        Assert.That(exception.StatusCode, Is.EqualTo(StatusCode.Unauthenticated));
    }

    private async Task AssertThatJwtTokenIsValidAsync(IConfiguration config, string token)
    {
        var keyBytes = Encoding.UTF8.GetBytes(jwtSecret);

        var jwtHandler = new JwtSecurityTokenHandler();
        var validationResult = await jwtHandler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidAlgorithms = [ SecurityAlgorithms.HmacSha256 ],
            ValidIssuer = config.GetValue<string>("jwt-issuer"),
            ValidAudience = config.GetValue<string>("jwt-audience"),
        });

        Assert.That(validationResult.IsValid, Is.True);
    }
}
