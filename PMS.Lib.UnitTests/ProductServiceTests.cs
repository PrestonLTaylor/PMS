using Grpc.Core;
using Moq;
using OneOf.Types;
using PMS.Lib.Data;
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
        productLookupMock.Setup(m => m.GetProductByIdAsync(new GetProductByIdRequest { Id = expectedId }, null, null, default)).Returns(grpcResponse);

        var repo = new ProductService(productLookupMock.Object);

        // Act
        var response = await repo.GetProductByIdAsync(expectedId);

        // Assert
        var actualResponse = response.Value as Product;
        Assert.That(actualResponse, Is.Not.Null);
        Assert.That(actualResponse, Is.EqualTo(expectedProduct));
    }

    [Test]
    public async Task GetProductByIdAsync_ReturnsNotFound_WhenSuppliedInvalidId()
    {
        // Arrange
        const int invalidId = 1;

        var grpcResponse = CallHelpers.CreateResponse<ProductInfo>(StatusCode.NotFound);
        var productLookupMock = new Mock<ProductLookup.ProductLookupClient>();
        productLookupMock.Setup(m => m.GetProductByIdAsync(new GetProductByIdRequest { Id = invalidId }, null, null, default)).Returns(grpcResponse);

        var repo = new ProductService(productLookupMock.Object);

        // Act
        var response = await repo.GetProductByIdAsync(invalidId);

        // Assert
        var actualResponse = response.Value as NotFound?;
        Assert.That(actualResponse, Is.Not.Null);
    }

    [Test]
    public async Task GetProductByIdAsync_ReturnsGrpcError_WhenANonNotFoundGrpcErrorOccurs()
    {
        // Arrange
        const int invalidId = 1;
        const StatusCode expectedStatusCode = StatusCode.Unimplemented;

        var grpcResponse = CallHelpers.CreateResponse<ProductInfo>(expectedStatusCode);
        var productLookupMock = new Mock<ProductLookup.ProductLookupClient>();
        productLookupMock.Setup(m => m.GetProductByIdAsync(new GetProductByIdRequest { Id = invalidId }, null, null, default)).Returns(grpcResponse);

        var repo = new ProductService(productLookupMock.Object);

        // Act
        var response = await repo.GetProductByIdAsync(invalidId);

        // Assert
        var actualResponse = response.Value as GrpcError;
        Assert.That(actualResponse, Is.Not.Null);
        Assert.That(actualResponse.StatusCode, Is.EqualTo(expectedStatusCode));
    }

    [Test]
    public async Task GetProductsByPartialNameAsync_ReturnsExpectedProduct_WhenSuppliedExpectedIdOfProduct()
    {
        // Arrange
        const string fullProductName = "Bread";
        var expectedProduct = new ProductInfo
        {
            Id = 1,
            Name = fullProductName,
            Price = 100,
        };
        var productData = new List<ProductInfo>()
        {
            expectedProduct
        };

        var grpcResponse = CallHelpers.CreateStreamingResponse(productData);
        var productLookupMock = new Mock<ProductLookup.ProductLookupClient>();
        productLookupMock.Setup(m => m.GetProductsByPartialName(new GetProductsByPartialNameRequest { PartialName = fullProductName }, null, null, default))
            .Returns(grpcResponse);

        var repo = new ProductService(productLookupMock.Object);

        // Act
        var response = await repo.GetProductsByPartialNameAsync(fullProductName);

        // Assert
        var actualProductList = response.Value as IReadOnlyList<Product>;
        Assert.That(actualProductList, Is.Not.Null);
        Assert.That(actualProductList, Has.Count.EqualTo(1));

        var actualProduct = actualProductList[0];
        Assert.That(actualProduct, Is.EqualTo(expectedProduct));
    }

    [Test]
    public async Task GetProductsByPartialNameAsync_ReturnsNotFound_WhenSuppliedInvalidName()
    {
        // Arrange
        const string invalidProductName = "Invalid";
        var productData = new List<ProductInfo>() {};

        var grpcResponse = CallHelpers.CreateStreamingResponse(productData);
        var productLookupMock = new Mock<ProductLookup.ProductLookupClient>();
        productLookupMock.Setup(m => m.GetProductsByPartialName(new GetProductsByPartialNameRequest { PartialName = invalidProductName }, null, null, default))
            .Returns(grpcResponse);

        var repo = new ProductService(productLookupMock.Object);

        // Act
        var response = await repo.GetProductsByPartialNameAsync(invalidProductName);

        // Assert
        var actualResponse = response.Value as NotFound?;
        Assert.That(actualResponse, Is.Not.Null);
    }

    [Test]
    public async Task GetProductsByPartialNameAsync_ReturnsGrpcError_WhenANonNotFoundGrpcErrorOccurs()
    {
        // Arrange
        const string invalidProductName = "Invalid";
        const StatusCode expectedStatusCode = StatusCode.Unimplemented;

        var grpcException = new RpcException(new Status(expectedStatusCode, ""));
        var productLookupMock = new Mock<ProductLookup.ProductLookupClient>();
        productLookupMock.Setup(m => m.GetProductsByPartialName(new GetProductsByPartialNameRequest { PartialName = invalidProductName }, null, null, default))
            .Throws(grpcException);

        var repo = new ProductService(productLookupMock.Object);

        // Act
        var response = await repo.GetProductsByPartialNameAsync(invalidProductName);

        // Assert
        var actualResponse = response.Value as GrpcError;
        Assert.That(actualResponse, Is.Not.Null);
        Assert.That(actualResponse.StatusCode, Is.EqualTo(expectedStatusCode));
    }
}