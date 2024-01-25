using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PMS.Server.Data;
using PMS.Server.Data.Repositories;
using PMS.Server.Models;

namespace PMS.Server.UnitTests;

internal class ProductRepositoryTests
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
        var productData = new List<ProductModel>
        {
            expectedProduct
        };

        var productSetMock = CreateDbSetMock(productData.AsQueryable());
        var contextMock = CreateDatabaseContextMock(productSetMock);
        var service = new ProductRepository(contextMock.Object);

        // Act
        var response = service.GetProductById(expectedId);

        // Assert
        Assert.That(response, Is.EqualTo(expectedProduct));
    }

    [Test]
    public void GetProductById_ReturnsNull_WhenSuppliedInvalidId()
    {
        // Arrange
        const int invalidId = 1;
        var productData = new List<ProductModel>();

        var productSetMock = CreateDbSetMock(productData.AsQueryable());
        var contextMock = CreateDatabaseContextMock(productSetMock);
        var service = new ProductRepository(contextMock.Object);

        // Act
        var response = service.GetProductById(invalidId);

        // Assert
        Assert.That(response, Is.Null);
    }

    private Mock<DbSet<T>> CreateDbSetMock<T>(IQueryable<T> setData) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();

        // Sets up the queryable properties so extension methods will work on the DbSet, https://learn.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking?redirectedfrom=MSDN
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(setData.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(setData.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(setData.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => setData.GetEnumerator());

        return mockSet;
    }

    private Mock<DatabaseContext> CreateDatabaseContextMock(Mock<DbSet<ProductModel>> productSetMock)
    {
        var configurationMock = new Mock<IConfiguration>();
        var contextMock = new Mock<DatabaseContext>(NullLogger<DatabaseContext>.Instance, configurationMock.Object);
        contextMock.Setup(m => m.Products).Returns(productSetMock.Object);

        return contextMock;
    }
}
