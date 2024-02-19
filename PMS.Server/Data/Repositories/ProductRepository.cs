using PMS.Server.Models;

namespace PMS.Server.Data.Repositories;

internal sealed class ProductRepository : IProductRepository
{
    public ProductRepository(DatabaseContext context)
    {
        _context = context;
    }

    public ProductModel? GetProductById(int id)
    {
        // FIXME: Convert into an async function
        return _context.Products.FirstOrDefault(p => p.Id == id);
    }

    public IReadOnlyList<ProductModel> GetProductsByPartialName(string partialName)
    {
        return _context.Products.Where(p => p.Name.Contains(partialName)).ToList();
    }

    private readonly DatabaseContext _context;
}
