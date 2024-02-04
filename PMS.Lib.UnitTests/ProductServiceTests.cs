using Grpc.Core;
using Moq;
using PMS.Lib.Services;
using PMS.Lib.UnitTests.Helpers;
using PMS.Services.Product;

namespace PMS.Lib.UnitTests;

internal sealed class ProductServiceTests
{
    [Test]
    public async Task GetProductByIdAsync_ReturnsExpectedProduct_WhenSuppliedExpectedIdOfProduct()
    {
        // Arrange
        const int expectedId = 1;
        var expectedProduct = new ProductInfo
        {
            Id = expectedId,
            Name = "Test",
            Price = 100,
        };

        var grpcResponse = CallHelpers.CreateResponse(expectedProduct);
        var productLookupMock = new Mock<ProductLookup.ProductLookupClient>();
        productLookupMock.Setup(m => m.GetProductAsync(new GetProductRequest { Id = expectedId }, null, null, default)).Returns(grpcResponse);

        var repo = new ProductService(productLookupMock.Object);

        // Act
        var response = await repo.GetProductByIdAsync(expectedId);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo(expectedProduct.Id));
            Assert.That(response.Name, Is.EqualTo(expectedProduct.Name));
            Assert.That(response.Price, Is.EqualTo(expectedProduct.Price));
        });
    }

    [Test]
    public async Task GetProductByIdAsync_ReturnsNull_WhenSuppliedInvalidId()
    {
        // Arrange
        const int invalidId = 1;

        var grpcResponse = CallHelpers.CreateResponse<ProductInfo>(StatusCode.NotFound);
        var productLookupMock = new Mock<ProductLookup.ProductLookupClient>();
        productLookupMock.Setup(m => m.GetProductAsync(new GetProductRequest { Id = invalidId }, null, null, default)).Returns(grpcResponse);

        var repo = new ProductService(productLookupMock.Object);

        // Act
        var response = await repo.GetProductByIdAsync(invalidId);

        // Assert
        Assert.That(response, Is.Null);
    }
}