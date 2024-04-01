using PMS.Server.Options;

namespace PMS.Server.Installers;

public static class OptionsBinder
{
    static public IServiceCollection BindOptionsFromConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
