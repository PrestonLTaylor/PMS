using PMS.Server.Models;
using PMS.Services.Product;
using Riok.Mapperly.Abstractions;

namespace PMS.Server.Data.Mappers;

[Mapper]
internal partial class ProductMapper
{
    internal partial ProductInfo ProductModelToInfo(ProductModel model);
}
