using PMS.Lib.Services;
using PMS.Services.Product;

namespace PMS.Lib;

static public class Installer
{
    static public IServiceCollection AddPMSServices(this IServiceCollection services, string address)
    {
        services.AddGrpcClient<ProductLookup.ProductLookupClient>(options =>
        {
            options.Address = new Uri(address);
        });

        services.AddTransient<IProductService, ProductService>();

        return services;
    }
}
