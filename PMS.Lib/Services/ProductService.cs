using Grpc.Core;
using PMS.Lib.Data;
using PMS.Lib.Data.Mappers;
using PMS.Services.Product;

namespace PMS.Lib.Services;

internal sealed class ProductService : IProductService
{
    public ProductService(ProductLookup.ProductLookupClient productLookupClient)
    {
        _productLookupClient = productLookupClient;
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        ProductInfo response;
        try
        {
            response = await _productLookupClient.GetProductAsync(new GetProductRequest { Id = id });
        }
        catch (RpcException)
        {
            // TODO: Introduce logging for the client
            return null;
        }

        return _mapper.ProductInfoToProduct(response);
    }

    private readonly ProductLookup.ProductLookupClient _productLookupClient;
    private readonly ProductMapper _mapper = new();
}
