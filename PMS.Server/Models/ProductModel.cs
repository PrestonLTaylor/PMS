using System.ComponentModel.DataAnnotations;

namespace PMS.Server.Models;

internal class ProductModel
{
    [Key]
    public int Id { get; set; }
    // FIXME: Enforce a maximum string length
    public required string Name { get; set; }
    public int Price { get; set; }
}
