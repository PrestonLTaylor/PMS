using PMS.Services.Product;

namespace PMS.Lib.Data;

public sealed class Product : IEquatable<ProductInfo>
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public int Price { get; init; }

    // NOTE: Used in tests
    public bool Equals(ProductInfo? rhs)
    {
        return rhs is not null && Id == rhs.Id && Name == rhs.Name && Price == rhs.Price;
    }
}
