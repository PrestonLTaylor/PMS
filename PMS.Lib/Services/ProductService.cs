using Grpc.Core;
using OneOf;
using OneOf.Types;
using PMS.Lib.Data;
using PMS.Lib.Data.Mappers;
using PMS.Services.Product;

namespace PMS.Lib.Services;

internal sealed class ProductService(ProductLookup.ProductLookupClient _productLookupClient) : IProductService
{
    public async Task<OneOf<Product, NotFound, GrpcError>> GetProductByIdAsync(int id)
    {
        ProductInfo response;
        try
        {
            response = await _productLookupClient.GetProductByIdAsync(new GetProductByIdRequest { Id = id });
        }
        catch (RpcException ex)
        {
            return ex.StatusCode switch
            {
                StatusCode.NotFound => new NotFound(),
                _ => new GrpcError(ex)
            };
        }

        return _mapper.ProductInfoToProduct(response);
    }

    public async Task<OneOf<IReadOnlyList<Product>, NotFound, GrpcError>> GetProductsByPartialNameAsync(string partialName)
    {
        List<ProductInfo> response;

        try
        {
            var streamingCall = _productLookupClient.GetProductsByPartialName(new GetProductsByPartialNameRequest { PartialName = partialName });
            response = await streamingCall.ResponseStream.ReadAllAsync().ToListAsync();
        }
        catch (RpcException ex)
        {
            // NOTE: We don't return a NotFound RpcException if no products are found
            return new GrpcError(ex);
        }

        if (response.Count == 0) return new NotFound();

        return _mapper.ProductInfoListToProductList(response);
    }

    private readonly ProductMapper _mapper = new();
}
