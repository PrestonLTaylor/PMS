using PMS.Server.Data;
using PMS.Server.Models;
using PMS.Server.Options;

namespace PMS.Server.Installers;

public static class IdentityInstaller
{
    public static IServiceCollection AddPMSUserIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        var passwordValidationOptions = configuration
            .GetRequiredSection(nameof(PasswordValidationOptions))
            .Get<PasswordValidationOptions>()!;

        services.AddIdentity<UserModel, DefaultRole>(options =>
        {
            options.Password.RequireNonAlphanumeric = passwordValidationOptions.RequireNonAlphanumeric;
            options.Password.RequireLowercase = passwordValidationOptions.RequireLowercase;
            options.Password.RequireUppercase = passwordValidationOptions.RequireUppercase;
            options.Password.RequireDigit = passwordValidationOptions.RequireDigit;
            options.Password.RequiredLength = passwordValidationOptions.RequiredLength;
        }).AddEntityFrameworkStores<DatabaseContext>();

        return services;
    }
}
