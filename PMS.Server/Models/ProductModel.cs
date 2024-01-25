using System.ComponentModel.DataAnnotations;

namespace PMS.Server.Models;

internal sealed class ProductModel
{
    [Key]
    public int Id { get; set; }
    // FIXME: Enforce a maximum string length
    public required string Name { get; set; }
    public int Price { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not ProductModel) return false;

        var rhs = (obj as ProductModel)!;

        return Id == rhs.Id && Name == rhs.Name && Price == rhs.Price;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Price);
    }
}
