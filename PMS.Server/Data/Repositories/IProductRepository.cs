using PMS.Server.Models;

namespace PMS.Server.Data.Repositories;

internal interface IProductRepository
{
    public ProductModel? GetProductById(int id);
    public IReadOnlyList<ProductModel> GetProductsByPartialName(string partialName);
}
