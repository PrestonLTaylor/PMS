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
    public void GetProduct_ReturnsExpectedProduct_WhenSuppliedExpectedIdOfProduct()
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
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo(expectedProduct.Id));
            Assert.That(response.Name, Is.EqualTo(expectedProduct.Name));
            Assert.That(response.Price, Is.EqualTo(expectedProduct.Price));
        });
    }

    [Test]
    public void GetProduct_ThrowsRpcNotFoundException_WhenSuppliedInvalidIdOfProduct()
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