using PMS.Server.Data.Repositories;

namespace PMS.Server.UnitTests;

internal class ProductRepositoryTests
{
    // NOTE: This is just a filler tests before the functionality is implemented
    [Test]
    public void GetProductById_ReturnsMilk_WithAPriceOf100_WithSameIdAsRequest()
    {
        // Arrange
        const int expectedId = 1;
        var service = new ProductRepository();

        // Act
        var response = service.GetProductById(expectedId);

        // Assert
        Assert.That(response, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(response.Value.Id, Is.EqualTo(expectedId));
            Assert.That(response.Value.Name, Is.EqualTo("Milk"));
            Assert.That(response.Value.Price, Is.EqualTo(100));
        });
    }
}
