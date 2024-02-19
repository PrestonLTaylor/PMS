using Microsoft.EntityFrameworkCore;
using PMS.Server.Models;

namespace PMS.Server.Data;

internal class DatabaseContext(ILogger<DatabaseContext> _logger, IConfiguration _configuration) : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // FIXME: OnConfiguring isn't eargly called, so will be called during a gRPC call.
        // This will cause the exceptions to be handled and the server will continue to run
        // However, as this is a critical error, we shouldn't expect to continue execution
        var connectionString = _configuration.GetValue<string>("POSTGRESQLCONNSTR_DefaultConnection");
        if (connectionString is null)
        {
            _logger.LogCritical("Unable to find connection string 'POSTGRESQLCONNSTR_DefaultConnection' for postgres database.");
            throw new InvalidOperationException("Unable to find connection string 'POSTGRESQLCONNSTR_DefaultConnection' for postgres database.");
        }

        try
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unable to connect to postgres database.");
            throw;
        }
    }

    public virtual DbSet<ProductModel> Products { get; set; }
}
