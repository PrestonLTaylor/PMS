using Microsoft.Extensions.Logging.Abstractions;
using PMS.Server.Services;
using PMS.Services.Product;

namespace PMS.Server.UnitTests;

internal sealed class ProductLookupServiceTests
{
    // NOTE: This is just a filler tests before the functionality is implemented
    [Test]
    public void GetProduct_ReturnsMilk_WithAPriceOf100_WithSameIdAsRequest()
    {
        // Arrange
        const int expectedId = 1;
        var service = new ProductLookupService(NullLogger<ProductLookupService>.Instance);

        // Act
        var response = service.GetProduct(new GetProductRequest { Id = expectedId }, null!).Result;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo(expectedId));
            Assert.That(response.Name, Is.EqualTo("Milk"));
            Assert.That(response.Price, Is.EqualTo(100));
        });
    }
}