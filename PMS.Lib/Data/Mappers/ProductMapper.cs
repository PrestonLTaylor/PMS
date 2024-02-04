using PMS.Services.Product;
using Riok.Mapperly.Abstractions;

namespace PMS.Lib.Data.Mappers;

[Mapper]
internal sealed partial class ProductMapper
{
    internal partial Product ProductInfoToProduct(ProductInfo model);
}
