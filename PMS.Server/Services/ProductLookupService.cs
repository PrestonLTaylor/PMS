using Grpc.Core;
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

        return Task.FromResult(new ProductInfo
        {
            Id = product.Value.Id,
            Name = product.Value.Name,
            Price = product.Value.Price,
        });
    }

    private readonly IProductRepository _repo;
    private readonly ILogger<ProductLookupService> _logger;
}
