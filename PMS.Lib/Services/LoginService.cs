using Grpc.Core;
using OneOf;
using PMS.Services.Authentication;

namespace PMS.Lib.Services;

internal sealed class LoginService(Login.LoginClient _loginClient, TokenService _tokenService) : ILoginService
{
    public async Task<OneOf<Authenticated, Unauthenticated, GrpcError>> LoginAsync(string username, string password)
    {
        JWTResponse jwtResponse;
        try
        {
            jwtResponse = await _loginClient.LoginAsync(new LoginCredentials { Username = username, Password = password });
        }
        catch (RpcException ex)
        {
            return ex.StatusCode switch
            {
                StatusCode.Unauthenticated => new Unauthenticated(),
                _ => new GrpcError(ex)
            };
        }

        _tokenService.Token = jwtResponse.Token;

        return new Authenticated();
    }
}
