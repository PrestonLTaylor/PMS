using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PMS.Server.Models;
using PMS.Server.Services;
using PMS.Services.Authentication;

namespace PMS.Server.UnitTests;

internal sealed class LoginServiceTests
{
    [Test]
    public async Task Login_ReturnsExpectedJwtToken_WhenProvidingValidUsernameAndPassword()
    {
        // Arrange
        const string validUsername = "valid";
        const string validPassword = "valid";
        const string expectedToken = "token";

        var user = new UserModel(validUsername);

        var userManagerMock = CreateUserManagerMock();
        userManagerMock
            .Setup(m => m.FindByNameAsync(validUsername))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(m => m.CheckPasswordAsync(user, validPassword))
            .ReturnsAsync(true);

        var jwtGeneratorMock = new Mock<IJwtGeneratorService>();
        jwtGeneratorMock
            .Setup(m => m.Generate(user))
            .Returns(expectedToken);

        var service = new LoginService(userManagerMock.Object, jwtGeneratorMock.Object, NullLogger);

        // Act
        var response = await service.Login(new LoginCredentials { Username = validUsername, Password = validPassword }, null!);

        // Assert
        Assert.That(response.Token, Is.EqualTo(expectedToken));
    }

    [Test]
    public void Login_ThrowsGrpcUnauthenticated_WhenProvidingInvalidUsername()
    {
        // Arrange
        const string invalidUsername = "invalid";

        var userManagerMock = CreateUserManagerMock();
        userManagerMock
            .Setup(m => m.FindByNameAsync(invalidUsername))
            .ReturnsAsync((UserModel?)null);

        var jwtGeneratorMock = new Mock<IJwtGeneratorService>();

        var service = new LoginService(userManagerMock.Object, jwtGeneratorMock.Object, NullLogger);

        // Act
        var exception = Assert.ThrowsAsync<RpcException>(async () =>
            await service.Login(new LoginCredentials { Username = invalidUsername, Password = "" }, null!));

        // Assert

        Assert.That(exception.StatusCode, Is.EqualTo(StatusCode.Unauthenticated));
    }

    [Test]
    public void Login_ThrowsGrpcUnauthenticated_WhenProvidingInvalidPassword()
    {
        // Arrange
        const string validUsername = "valid";
        const string invalidPassword = "invalid";

        var user = new UserModel(validUsername);

        var userManagerMock = CreateUserManagerMock();
        userManagerMock
            .Setup(m => m.FindByNameAsync(validUsername))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(m => m.CheckPasswordAsync(user, invalidPassword))
            .ReturnsAsync(false);

        var jwtGeneratorMock = new Mock<IJwtGeneratorService>();

        var service = new LoginService(userManagerMock.Object, jwtGeneratorMock.Object, NullLogger);

        // Act
        var exception = Assert.ThrowsAsync<RpcException>(async () =>
            await service.Login(new LoginCredentials { Username = validUsername, Password = "" }, null!));

        // Assert
        Assert.That(exception.StatusCode, Is.EqualTo(StatusCode.Unauthenticated));
    }

    private Mock<UserManager<UserModel>> CreateUserManagerMock()
    {
        var userStoreMock = new Mock<IUserStore<UserModel>>();
        return new Mock<UserManager<UserModel>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    private static NullLogger<LoginService> NullLogger { get; } = new();
}
