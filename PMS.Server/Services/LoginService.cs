using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using PMS.Server.Models;
using PMS.Services.Authentication;

namespace PMS.Server.Services;

public sealed class LoginService(UserManager<UserModel> _userManager, IJwtGeneratorService _jwtGenerator, ILogger<LoginService> _logger) : Login.LoginBase
{
    public override async Task<JWTResponse> Login(LoginCredentials request, ServerCallContext context)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user is null)
        {
            _logger.LogWarning("Login request with invalid username {Username}", request.Username);
            throw new RpcException(new Status(StatusCode.Unauthenticated, $"Invalid username or password supplied"));
        }

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            _logger.LogWarning("Login request with invalid password for user {Username}", request.Username);
            throw new RpcException(new Status(StatusCode.Unauthenticated, $"Invalid username or password supplied"));
        }

        var jwtToken = _jwtGenerator.Generate(user);
        return new JWTResponse { Token = jwtToken };
    }
}
