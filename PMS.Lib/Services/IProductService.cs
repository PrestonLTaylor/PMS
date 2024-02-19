using PMS.Lib.Data;

namespace PMS.Lib.Services;

public interface IProductService
{
    public Task<Product?> GetProductByIdAsync(int id);
    public Task<IReadOnlyList<Product>> GetProductsByPartialNameAsync(string partialName);
}
