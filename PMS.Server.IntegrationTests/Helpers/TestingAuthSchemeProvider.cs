using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace PMS.Server.IntegrationTests.Helpers;

internal sealed class TestingAuthSchemeProvider : AuthenticationSchemeProvider
{
    public TestingAuthSchemeProvider(IOptions<AuthenticationOptions> options)
            : base(options)
    {
    }

    public override Task<AuthenticationScheme?> GetSchemeAsync(string name)
    {
        if (name == JwtBearerDefaults.AuthenticationScheme)
        {
            var scheme = new AuthenticationScheme(
                JwtBearerDefaults.AuthenticationScheme,
                JwtBearerDefaults.AuthenticationScheme,
                typeof(TestingAuthHandler)
            );
            return Task.FromResult(scheme)!;
        }

        return base.GetSchemeAsync(name);
    }
}
