using Grpc.Core;
using Grpc.Net.Client;
using PMS.Server.IntegrationTests.Helpers;
using PMS.Server.Models;
using PMS.Services.Product;

namespace PMS.Server.IntegrationTests;

internal sealed class ProductLookupServiceTests : GrpcIntergrationBase
{
    [SetUp]
    public async Task SetupTestContainers()
    {
        await psqlContainer.StartAsync();
    }

    [TearDown]
    public async Task DisposeTestContainers()
    {
        await psqlContainer.DisposeAsync();
    }

    [Test]
    public async Task GetProductById_ReturnsExpectedProduct_WhenSuppliedExpectedIdOfProduct()
    {
        // Arrange
        const int expectedId = 1;
        var expectedProduct = new ProductModel
        {
            Id = expectedId,
            Name = "Test",
            Price = 100,
        };

        var channel = CreateGrpcChannel();

        var request = new GetProductByIdRequest { Id = expectedId };
        var client = new ProductLookup.ProductLookupClient(channel);

        await InsertProductIntoPsqlContainer(expectedProduct);

        // Act
        var response = await client.GetProductByIdAsync(request);

        // Assert
        Assert.That(response, Is.EqualTo(expectedProduct));
    }

    [Test]
    public void GetProductById_ThrowsRpcNotFoundException_WhenSuppliedInvalidIdOfProduct()
    {
        // Arrange
        const int invalidId = 1;

        var channel = CreateGrpcChannel();

        var request = new GetProductByIdRequest { Id = invalidId };
        var client = new ProductLookup.ProductLookupClient(channel);

        // Act
        var exception = Assert.Throws<RpcException>(() => client.GetProductById(request));

        // Assert
        Assert.That(exception.StatusCode, Is.EqualTo(StatusCode.NotFound));
    }

    [Test]
    public async Task GetProductsByPartialName_ReturnsSpecifiedProducts_WhenRepoReturnsSameProducts()
    {
        // Arrange
        const string partialProductName = "Bread";
        var expectedProduct = new ProductModel
        {
            Id = 1,
            Name = "Brown " + partialProductName,
            Price = 100
        };

        var channel = CreateGrpcChannel();

        await InsertProductIntoPsqlContainer(expectedProduct);

        var request = new GetProductsByPartialNameRequest { PartialName = partialProductName };
        var client = new ProductLookup.ProductLookupClient(channel);

        // Act
        var response = client.GetProductsByPartialName(request);
        var actualProducts = await response.ResponseStream.ReadAllAsync().ToListAsync();

        // Assert
        Assert.That(response.GetStatus(), Is.EqualTo(Status.DefaultSuccess));
        Assert.That(actualProducts, Has.Count.EqualTo(1));

        var actualProduct = actualProducts[0];
        Assert.That(actualProduct, Is.EqualTo(expectedProduct));
    }

    // NOTE: This test is to make sure we don't throw a NotFound GrpcException
    [Test]
    public async Task GetProductsByPartialName_ReturnsAnEmptyProductList_WhenSuppliedInvalidName()
    {
        // Arrange
        const string invalidName = "Name";

        var channel = CreateGrpcChannel();

        var request = new GetProductsByPartialNameRequest { PartialName = invalidName };
        var client = new ProductLookup.ProductLookupClient(channel);

        // Act
        var response = client.GetProductsByPartialName(request);
        var actualProducts = await response.ResponseStream.ReadAllAsync().ToListAsync();

        // Assert
        Assert.That(response.GetStatus(), Is.EqualTo(Status.DefaultSuccess));
        Assert.That(actualProducts, Has.Count.EqualTo(0));
    }

    private GrpcChannel CreateGrpcChannel()
    {
        var client = CreateClient();
        return GrpcChannel.ForAddress(client.BaseAddress!, new GrpcChannelOptions
        {
            HttpClient = client
        });
    }

    // NOTE: This function needs to be called after CreateClient
    private async Task InsertProductIntoPsqlContainer(ProductModel product)
    {
        await psqlContainer.ExecScriptAsync($"INSERT INTO \"Products\" VALUES ({product.Id}, '{product.Name}', {product.Price})");
    }
}