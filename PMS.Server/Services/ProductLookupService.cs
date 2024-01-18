using Grpc.Core;
using PMS.Services.Product;

namespace PMS.Server.Services;

internal sealed class ProductLookupService : ProductLookup.ProductLookupBase
{
    public ProductLookupService(ILogger<ProductLookupService> logger)
    {
        _logger = logger;
    }

    public override Task<ProductInfo> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Product with an id of {Id} was requested.", request.Id);
        return Task.FromResult(new ProductInfo
        {
            Id = request.Id,
            Name = "Milk",
            Price = 100,
        });
    }

    private readonly ILogger<ProductLookupService> _logger;
}
