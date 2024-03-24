using Grpc.Net.Client;
using System.Text;
using Testcontainers.PostgreSql;

namespace PMS.Server.IntegrationTests.Helpers;

// NOTE: We need an instance per test case so we can dispose the TestContainers
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
internal class GrpcIntergrationBase 
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

    protected GrpcChannel CreateGrpcChannel()
    {
        var client = CreateClient();
        return GrpcChannel.ForAddress(client.BaseAddress!, new GrpcChannelOptions
        {
            HttpClient = client
        });
    }

    private HttpClient CreateClient()
    {
        var factory = new GrpcWebApplicationFactory<Program>(psqlContainer.GetConnectionString(), jwtSecret);

        services = factory.Services;

        return factory.CreateClient();
    }

    protected IServiceProvider? services;

    protected readonly PostgreSqlContainer psqlContainer = new PostgreSqlBuilder()
        .WithDatabase("testdb")
        .WithUsername("testusername")
        .WithPassword("testpassword")
        .Build();

    // NOTE: We need a key that is a size of at least 128-bits when generating the HMACSH256 hash
    protected readonly string jwtSecret = "LONG_TEST_SECRET_LONG_TEST_SECRET_LONG_TEST_SECRET_LONG_TEST_SECRET";
}
