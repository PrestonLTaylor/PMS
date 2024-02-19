using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PMS.Server.Data.Repositories;
using PMS.Server.IntegrationTests.Helpers;
using PMS.Server.Models;
using PMS.Services.Product;

namespace PMS.Server.IntegrationTests;

internal sealed class ProductLookupServiceTests : IntegrationTestBase
{
    [Test]
    public void GetProductById_ReturnsExpectedProduct_WhenSuppliedExpectedIdOfProduct()
    {
        // Arrange
        const int expectedId = 1;
        var expectedProduct = new ProductModel
        {
            Id = expectedId,
            Name = "Test",
            Price = 100,
        };

        var mockProductRepo = new Mock<IProductRepository>();
        mockProductRepo.Setup(m => m.GetProductById(expectedId)).Returns(expectedProduct);

        var channel = CreateGrpcChannel(services =>
        {
            services.AddSingleton(mockProductRepo.Object);
        });

        var request = new GetProductByIdRequest { Id = expectedId };
        var client = new ProductLookup.ProductLookupClient(channel);

        // Act
        var response = client.GetProductById(request);

        // Assert
        Assert.That(response, Is.EqualTo(expectedProduct));
    }

    [Test]
    public void GetProductById_ThrowsRpcNotFoundException_WhenSuppliedInvalidIdOfProduct()
    {
        // Arrange
        const int invalidId = 1;
        var mockProductRepo = new Mock<IProductRepository>();
        mockProductRepo.Setup(m => m.GetProductById(It.IsAny<int>())).Returns((ProductModel?)null);

        var channel = CreateGrpcChannel(services =>
        {
            services.AddSingleton(mockProductRepo.Object);
        });

        var request = new GetProductByIdRequest { Id = invalidId };
        var client = new ProductLookup.ProductLookupClient(channel);

        // Act

        // Assert
        var exception = Assert.Throws<RpcException>(() => client.GetProductById(request));

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
        var productData = new List<ProductModel>()
        {
            expectedProduct,
        };

        var mockProductRepo = new Mock<IProductRepository>();
        mockProductRepo.Setup(m => m.GetProductsByPartialName(partialProductName)).Returns(productData);

        var channel = CreateGrpcChannel(services =>
        {
            services.AddSingleton(mockProductRepo.Object);
        });

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
        var productData = new List<ProductModel>();

        var mockProductRepo = new Mock<IProductRepository>();
        mockProductRepo.Setup(m => m.GetProductsByPartialName(invalidName)).Returns(productData);

        var channel = CreateGrpcChannel(services =>
        {
            services.AddSingleton(mockProductRepo.Object);
        });

        var request = new GetProductsByPartialNameRequest { PartialName = invalidName };
        var client = new ProductLookup.ProductLookupClient(channel);

        // Act
        var response = client.GetProductsByPartialName(request);
        var actualProducts = await response.ResponseStream.ReadAllAsync().ToListAsync();

        // Assert
        Assert.That(response.GetStatus(), Is.EqualTo(Status.DefaultSuccess));
        Assert.That(actualProducts, Has.Count.EqualTo(0));
    }

    private GrpcChannel CreateGrpcChannel(Action<IServiceCollection> configureTestServices)
    {
        // FIXME/NOTE: We need this testing config to set our environment to testing so we don't run our database seeding code
        var testingConfig = new ConfigurationBuilder()
            .AddCommandLine(["--environment", "Testing" ])
            .Build();

        var testClient = Factory.WithWebHostBuilder(builder =>
        {
            builder.UseConfiguration(testingConfig);
            builder.ConfigureTestServices(configureTestServices);
            builder.ConfigureLogging(config =>
            {
                config.ClearProviders();
            });
        }).CreateClient();

        return GrpcChannel.ForAddress(testClient.BaseAddress!, new GrpcChannelOptions
        {
            HttpClient = testClient
        });
    }
}