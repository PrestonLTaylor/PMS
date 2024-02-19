using PMS.Services.Product;
using System.ComponentModel.DataAnnotations;

namespace PMS.Server.Models;

internal sealed class ProductModel : IEquatable<ProductModel>, IEquatable<ProductInfo>
{
    [Key]
    public int Id { get; set; }
    // FIXME: Enforce a maximum string length
    public required string Name { get; set; }
    public int Price { get; set; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ProductModel);
    }

    public bool Equals(ProductModel? rhs)
    {
        return rhs is not null && Id == rhs.Id && Name == rhs.Name && Price == rhs.Price;
    }

    // NOTE: Used for tests
    public bool Equals(ProductInfo? rhs)
    {
        return rhs is not null && Id == rhs.Id && Name == rhs.Name && Price == rhs.Price;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Price);
    }
}
