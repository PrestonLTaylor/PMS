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
    public void GetProductById_ReturnsExpectedProduct_WhenSuppliedExpectedIdOfProduct()
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
        Assert.That(response, Is.EqualTo(expectedProduct));
    }

    [Test]
    public void GetProductById_ThrowsRpcNotFoundException_WhenSuppliedInvalidId()
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

    [Test]
    public async Task GetProductsByPartialName_ReturnsSpecifiedProducts_WhenRepoReturnsSameProducts()
    {
        // Arrange
        const string fullProductName = "Brown Bread";
        var expectedProductData = new List<ProductModel>()
        {
            new() { Id = 1, Name = fullProductName, Price = 100 },
        };

        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(m => m.GetProductsByPartialName(fullProductName)).Returns(expectedProductData);

        var actualProductData = new List<ProductInfo>();
        var streamMock = new Mock<IServerStreamWriter<ProductInfo>>();
        streamMock.Setup(m => m.WriteAsync(It.IsAny<ProductInfo>()))
            .Callback<ProductInfo>(actualProductData.Add);

        var service = new ProductLookupService(repoMock.Object, NullLogger<ProductLookupService>.Instance);

        // Act
        await service.GetProductsByPartialName(new GetProductsByPartialNameRequest { PartialName = fullProductName }, streamMock.Object, null!);

        // Assert
        Assert.That(actualProductData, Has.Count.EqualTo(expectedProductData.Count));

        for (int i = 0; i < actualProductData.Count; ++i)
        {
            var actualProduct = actualProductData[i];
            var expectedProduct = expectedProductData[i];

            Assert.That(actualProduct, Is.EqualTo(expectedProduct));
        }
    }

    // NOTE: This test is to make sure we don't throw a NotFound GrpcException
    [Test]
    public async Task GetProductsByPartialName_ReturnsNoProducts_WhenRepoReturnsEmptyList()
    {
        // Arrange
        var repoMock = new Mock<IProductRepository>();
        repoMock.Setup(m => m.GetProductsByPartialName(It.IsAny<string>())).Returns(new List<ProductModel>());

        var actualProductData = new List<ProductInfo>();
        var streamMock = new Mock<IServerStreamWriter<ProductInfo>>();
        streamMock.Setup(m => m.WriteAsync(It.IsAny<ProductInfo>()))
            .Callback<ProductInfo>(actualProductData.Add);

        var service = new ProductLookupService(repoMock.Object, NullLogger<ProductLookupService>.Instance);

        // Act
        await service.GetProductsByPartialName(new GetProductsByPartialNameRequest { PartialName = "" }, streamMock.Object, null!);

        // Assert
        Assert.That(actualProductData, Has.Count.Zero);
    }
}