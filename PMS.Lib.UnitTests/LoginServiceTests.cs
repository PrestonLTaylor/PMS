using Grpc.Core;
using Moq;
using PMS.Lib.Services;
using PMS.Lib.UnitTests.Helpers;
using PMS.Services.Authentication;

namespace PMS.Lib.UnitTests;

internal sealed class LoginServiceTests
{
    [Test]
    public async Task LoginAsync_ReturnsJwtResponse_WhenSuppliedValidUsernameAndPassword()
    {
        // Arrange
        const string validUsername = "valid";
        const string validPassword = "valid";
        const string expectedToken = "token";
        var expectedResponse = new JWTResponse
        {
            Token = expectedToken
        };

        var grpcResponse = CallHelpers.CreateResponse(expectedResponse);
        var loginClientMock = new Mock<Login.LoginClient>();
        loginClientMock
            .Setup(m => m.LoginAsync(new LoginCredentials { Username = validUsername, Password = validPassword }, null, null, default))
            .Returns(grpcResponse);

        var tokenServiceMock = new Mock<TokenService>();
        tokenServiceMock
            .SetupSet(m => m.Token = expectedToken)
            .Verifiable();

        var service = new LoginService(loginClientMock.Object, tokenServiceMock.Object);

        // Act
        var response = await service.LoginAsync(validUsername, validPassword);

        // Assert
        var actualToken = response.Value as Authenticated?;
        Assert.That(actualToken, Is.Not.Null);

        tokenServiceMock.VerifyAll();
    }

    [Test]
    public async Task LoginAsync_ReturnsUnauthenticatd_WhenSuppliedInvalidUsernameOrPassword()
    {
        // Arrange
        const string invalidUsername = "invalid";
        const string invalidPassword = "invalid";

        var grpcResponse = CallHelpers.CreateResponse<JWTResponse>(StatusCode.Unauthenticated);
        var loginClientMock = new Mock<Login.LoginClient>();
        loginClientMock
            .Setup(m => m.LoginAsync(new LoginCredentials { Username = invalidUsername, Password = invalidPassword }, null, null, default))
            .Returns(grpcResponse);

        var tokenServiceMock = new Mock<TokenService>();

        var service = new LoginService(loginClientMock.Object, tokenServiceMock.Object);

        // Act
        var response = await service.LoginAsync(invalidUsername, invalidPassword);

        // Assert
        var actualResponse = response.Value as Unauthenticated?;
        Assert.That(actualResponse, Is.Not.Null);
    }

    [Test]
    public async Task LoginAsync_ReturnsGrpcError_WhenANonUnauthenticatdGrpcErrorOccurs()
    {
        // Arrange
        const StatusCode expectedStatusCode = StatusCode.Unimplemented;

        var grpcResponse = CallHelpers.CreateResponse<JWTResponse>(expectedStatusCode);
        var loginClientMock = new Mock<Login.LoginClient>();
        loginClientMock
            .Setup(m => m.LoginAsync(It.IsAny<LoginCredentials>(), null, null, default))
            .Returns(grpcResponse);

        var tokenServiceMock = new Mock<TokenService>();

        var service = new LoginService(loginClientMock.Object, tokenServiceMock.Object);

        // Act
        var response = await service.LoginAsync("", "");

        // Assert
        var actualResponse = response.Value as GrpcError;
        Assert.That(actualResponse, Is.Not.Null);
        Assert.That(actualResponse.StatusCode, Is.EqualTo(expectedStatusCode));
    }
}
