using Grpc.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PMS.Server.Data.Repositories;
using PMS.Server.Models;
using PMS.Server.Services;
using PMS.Services.Product;

namespace PMS.Server.UnitTests;

internal sealed class ProductLookupServiceTests
{
    [Test]
    public void GetProduct_ReturnsExpectedProduct_WhenSuppliedExpectedIdOfProduct()
    {
        // Arrange
        const int expectedId = 1;
        var expectedProduct = new ProductModel
        {
            Id = expectedId,
            Name = "test",
            Price = 100,
        };

        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(m => m.GetProductById(expectedId)).Returns(expectedProduct);

        var service = new ProductLookupService(repoMock.Object, NullLogger<ProductLookupService>.Instance);

        // Act
        var response = service.GetProductById(new GetProductByIdRequest { Id = expectedId }, null!).Result;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo(expectedProduct.Id));
            Assert.That(response.Name, Is.EqualTo(expectedProduct.Name));
            Assert.That(response.Price, Is.EqualTo(expectedProduct.Price));
        });
    }

    [Test]
    public void GetProduct_ThrowsRpcNotFoundException_WhenSuppliedInvalidId()
    {
        // Arrange
        const int expectedId = 1;

        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(m => m.GetProductById(expectedId)).Returns((ProductModel?)null);

        var service = new ProductLookupService(repoMock.Object, NullLogger<ProductLookupService>.Instance);

        // Act

        // Assert
        var exception = Assert.Throws<RpcException>(() => service.GetProductById(new GetProductByIdRequest { Id = expectedId }, null!));

        Assert.That(exception.StatusCode, Is.EqualTo(StatusCode.NotFound));
    }
}