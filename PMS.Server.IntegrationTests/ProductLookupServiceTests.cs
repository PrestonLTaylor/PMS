using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using PMS.Server.IntegrationTests.Helpers;
using PMS.Server.Models;
using PMS.Services.Product;

namespace PMS.Server.IntegrationTests;

internal sealed class ProductLookupServiceTests : GrpcIntergrationBase
{
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

        var channel = CreateGrpcChannel(MakeAllRequestsAuthenticated);

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

        var channel = CreateGrpcChannel(MakeAllRequestsAuthenticated);

        var request = new GetProductByIdRequest { Id = invalidId };
        var client = new ProductLookup.ProductLookupClient(channel);

        // Act
        var exception = Assert.Throws<RpcException>(() => client.GetProductById(request));

        // Assert
        Assert.That(exception.StatusCode, Is.EqualTo(StatusCode.NotFound));
    }

    [Test]
    public void GetProductById_ThrowsUnauthenticated_WhenCallingWhileUnauthenticated()
    {
        // Arrange
        var channel = CreateGrpcChannel();

        var request = new GetProductByIdRequest { Id = 0 };
        var client = new ProductLookup.ProductLookupClient(channel);

        // Act
        var exception = Assert.Throws<RpcException>(() => client.GetProductById(request));

        // Assert
        Assert.That(exception.StatusCode, Is.EqualTo(StatusCode.Unauthenticated));
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

        var channel = CreateGrpcChannel(MakeAllRequestsAuthenticated);

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

        var channel = CreateGrpcChannel(MakeAllRequestsAuthenticated);

        var request = new GetProductsByPartialNameRequest { PartialName = invalidName };
        var client = new ProductLookup.ProductLookupClient(channel);

        // Act
        var response = client.GetProductsByPartialName(request);
        var actualProducts = await response.ResponseStream.ReadAllAsync().ToListAsync();

        // Assert
        Assert.That(response.GetStatus(), Is.EqualTo(Status.DefaultSuccess));
        Assert.That(actualProducts, Has.Count.EqualTo(0));
    }

    [Test]
    public void GetProductsByPartialName_ThrowsUnauthenticated_WhenCallingWhileUnauthenticated()
    {
        // Arrange
        var channel = CreateGrpcChannel();

        var request = new GetProductsByPartialNameRequest { PartialName = "" };
        var client = new ProductLookup.ProductLookupClient(channel);

        // Act
        var exception = Assert.ThrowsAsync<RpcException>(async () =>
        {
            var response = client.GetProductsByPartialName(request);
            await response.ResponseStream.ReadAllAsync().ToListAsync();
        });

        // Assert
        Assert.That(exception.StatusCode, Is.EqualTo(StatusCode.Unauthenticated));
    }

    // NOTE: This function needs to be called after CreateClient
    private async Task InsertProductIntoPsqlContainer(ProductModel product)
    {
        await psqlContainer.ExecScriptAsync($"INSERT INTO \"Products\" VALUES ({product.Id}, '{product.Name}', {product.Price})");
    }

    private void MakeAllRequestsAuthenticated(IServiceCollection services)
    {
        // We replace the authenticate scheme provider to use our testing authentication provider to authenticate all requests
        services.AddTransient<IAuthenticationSchemeProvider, TestingAuthSchemeProvider>();
    }
}