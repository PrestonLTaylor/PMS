using PMS.Services.Product;
using Riok.Mapperly.Abstractions;

namespace PMS.Lib.Data.Mappers;

[Mapper]
internal sealed partial class ProductMapper
{
    internal partial Product ProductInfoToProduct(ProductInfo model);

    public List<Product> ProductInfoListToProductList(List<ProductInfo> models)
    {
        List<Product> products = new();
        foreach (var model in models)
        {
            products.Add(ProductInfoToProduct(model));
        }
        return products;
    }
}
