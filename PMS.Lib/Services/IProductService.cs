using OneOf;
using OneOf.Types;
using PMS.Lib.Data;

namespace PMS.Lib.Services;

public interface IProductService
{
    public Task<OneOf<Product, NotFound, GrpcError>> GetProductByIdAsync(int id);
    public Task<OneOf<IReadOnlyList<Product>, NotFound, GrpcError>> GetProductsByPartialNameAsync(string partialName);
}
