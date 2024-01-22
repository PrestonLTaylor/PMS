namespace PMS.Server.Data.Repositories;

static internal class RepositoryInstaller
{
    static public IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IProductRepository, ProductRepository>();

        return services;
    }
}
