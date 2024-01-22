using PMS.Server.Models;

namespace PMS.Server.Data.Repositories;

internal sealed class ProductRepository : IProductRepository
{
    public ProductModel? GetProductById(int id)
    {
        return new ProductModel
        {
            Id = id,
            Name = "Milk",
            Price = 100,
        };
    }
}
