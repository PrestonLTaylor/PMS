using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PMS.Server.Data;

namespace PMS.Server.IntegrationTests.Helpers;

internal class GrpcWebApplicationFactory<TProgram>(string _connectionString, string _jwtSecret)
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        var scope = host.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        database.Database.Migrate();

        return host;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // NOTE: We need to change the environment so we don't run our seeding code
        builder.UseEnvironment("Testing");

        Environment.SetEnvironmentVariable("POSTGRESQLCONNSTR_DefaultConnection", _connectionString);
        Environment.SetEnvironmentVariable("JwtValidationOptions__Secret", _jwtSecret);
    }
}
