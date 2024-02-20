using Grpc.Core;

namespace PMS.Lib;

public sealed class GrpcError(RpcException _exception)
{
    public StatusCode StatusCode => _exception.StatusCode;
    // TODO: Extract user friendly error messages
    public string Message => _exception.Message;
}
