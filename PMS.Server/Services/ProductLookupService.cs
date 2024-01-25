using Grpc.Core;
using PMS.Server.Data.Mappers;
using PMS.Server.Data.Repositories;
using PMS.Services.Product;

namespace PMS.Server.Services;

internal sealed class ProductLookupService : ProductLookup.ProductLookupBase
{
    public ProductLookupService(IProductRepository repo, ILogger<ProductLookupService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public override Task<ProductInfo> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Product with an id of {Id} was requested.", request.Id);

        var product = _repo.GetProductById(request.Id);
        if (product is null)
        {
            _logger.LogWarning("Product with an id of {Id} was not found.", request.Id);
            throw new RpcException(new Status(StatusCode.NotFound, $"Product with an id of {request.Id} was not found."));
        }

        return Task.FromResult(_mapper.ProductModelToInfo(product));
    }

    private readonly IProductRepository _repo;
    private readonly ILogger<ProductLookupService> _logger;
    private readonly ProductMapper _mapper = new();
}
