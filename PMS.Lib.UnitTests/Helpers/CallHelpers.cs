using Grpc.Core;

namespace PMS.Lib.UnitTests.Helpers;

// Modified from: https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/grpc/test-services/sample/Tests/Client/Helpers/CallHelpers.cs
internal static class CallHelpers
{
    public static AsyncUnaryCall<TResponse> CreateResponse<TResponse>(TResponse response)
    {
        return new AsyncUnaryCall<TResponse>(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });
    }

    public static AsyncUnaryCall<TResponse> CreateResponse<TResponse>(StatusCode statusCode)
    {
        var status = new Status(statusCode, string.Empty);
        return new AsyncUnaryCall<TResponse>(
            Task.FromException<TResponse>(new RpcException(status)),
            Task.FromResult(new Metadata()),
            () => status,
            () => new Metadata(),
            () => { });
    }
}
