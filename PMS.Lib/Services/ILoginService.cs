using OneOf;

namespace PMS.Lib.Services;

public interface ILoginService
{
    Task<OneOf<Authenticated, Unauthenticated, GrpcError>> LoginAsync(string username, string password);
}
