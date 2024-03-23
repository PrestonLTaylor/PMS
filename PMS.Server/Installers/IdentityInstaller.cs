using PMS.Server.Data;
using PMS.Server.Models;

namespace PMS.Server.Installers;

public static class IdentityInstaller
{
    public static IServiceCollection AddPMSUserIdentity(this IServiceCollection services)
    {
        services.AddIdentity<UserModel, DefaultRole>(options =>
        {
            // NOTE: This program is intented for "internal" use, so "secure" passwords aren't needed.
            // FIXME: Should still have a configuration value for changing this.
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 4;
        }).AddEntityFrameworkStores<DatabaseContext>();

        return services;
    }
}
