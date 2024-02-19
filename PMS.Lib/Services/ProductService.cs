using Grpc.Core;
using PMS.Lib.Data;
using PMS.Lib.Data.Mappers;
using PMS.Services.Product;

namespace PMS.Lib.Services;

internal sealed class ProductService(ProductLookup.ProductLookupClient _productLookupClient) : IProductService
{
    // TODO: Return a discriminated union so consumers can use actual errors instead of just returning null
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        ProductInfo response;
        try
        {
            response = await _productLookupClient.GetProductByIdAsync(new GetProductByIdRequest { Id = id });
        }
        catch (RpcException)
        {
            return null;
        }

        return _mapper.ProductInfoToProduct(response);
    }

    public async Task<IReadOnlyList<Product>> GetProductsByPartialNameAsync(string partialName)
    {
        List<ProductInfo> response;

        try
        {
            var streamingCall = _productLookupClient.GetProductsByPartialName(new GetProductsByPartialNameRequest { PartialName = partialName });
            response = await streamingCall.ResponseStream.ReadAllAsync().ToListAsync();
        }
        catch (RpcException)
        {
            // TODO: See TODO above GetProductByIdAsync
            return [];
        }

        return _mapper.ProductInfoListToProductList(response);
    }

    private readonly ProductMapper _mapper = new();
}
