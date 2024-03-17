using Testcontainers.PostgreSql;

namespace PMS.Server.IntegrationTests.Helpers;

// NOTE: We need an instance per test case so we can dispose the TestContainers
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
internal class GrpcIntergrationBase 
{
    public HttpClient CreateClient()
    {
        var factory = new GrpcWebApplicationFactory<Program>(psqlContainer.GetConnectionString());

        return factory.CreateClient();
    }

    protected readonly PostgreSqlContainer psqlContainer = new PostgreSqlBuilder()
        .WithDatabase("testdb")
        .WithUsername("testusername")
        .WithPassword("testpassword")
        .Build();
}
