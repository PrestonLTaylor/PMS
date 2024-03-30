using PMS.Lib.Services;
using PMS.Services.Authentication;
using PMS.Services.Product;

namespace PMS.Lib;

static public class Installer
{
    static public IServiceCollection AddPMSServices(this IServiceCollection services, string address)
    {
        services.AddGrpcClient<Login.LoginClient>(options =>
        {
            options.Address = new Uri(address);
        });

        var tokenService = new TokenService();
        services.AddSingleton(tokenService);

        AddGrpcClientsThatNeedCredentials(services, tokenService, address);

        services.AddTransient<ILoginService, LoginService>();
        services.AddTransient<IProductService, ProductService>();

        return services;
    }

    static private void AddGrpcClientsThatNeedCredentials(IServiceCollection services, TokenService tokenService, string address)
    {
        services.AddGrpcClient<ProductLookup.ProductLookupClient>(options =>
        {
            options.Address = new Uri(address);
        })
        .AddCallCredentials((context, metadata) =>
        {
            if (!string.IsNullOrEmpty(tokenService.Token))
            {
                metadata.Add("Authorization", $"Bearer {tokenService.Token}");
            }

            return Task.CompletedTask;
        })
        .ConfigureChannel(options =>
        {
            // FIXME: Configure this to be false unless development build
            options.UnsafeUseInsecureChannelCallCredentials = true;
        });
    }
}
