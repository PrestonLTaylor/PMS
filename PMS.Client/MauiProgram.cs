using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PMS.Client.Services;
using PMS.Client.ViewModels;
using PMS.Client.Views;
using PMS.Lib;
using Shiny;

namespace PMS.Client;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseShiny()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if WINDOWS
        builder.Services.AddHostedService<LogOutIfTokenHasExpiredJob>();
#else
        builder.Services.AddJob(typeof(LogOutIfTokenHasExpiredJob));
#endif

        // TODO: Don't use wait and put this code in an async function somewhere
        SetupConfigurationsAsync(builder.Configuration).Wait();
        SetupPMSServices(builder);

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<MainMenuViewModel>();
        builder.Services.AddTransient<ProductLookupByIdViewModel>();
        builder.Services.AddTransient<ProductLookupByNameViewModel>();

        builder.Services.AddTransient<Login>();
        builder.Services.AddTransient<MainMenu>();
        builder.Services.AddTransient<ProductLookupById>();
        builder.Services.AddTransient<ProductLookupByName>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

        var app = builder.Build();

#if WINDOWS
        StartHostedServices(app.Services);
#endif

        return app;
    }

    private static async Task SetupConfigurationsAsync(ConfigurationManager configuration)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("appSettings.json");
        var appSettingsConfig = new ConfigurationBuilder().AddJsonStream(stream).Build();
        configuration.AddConfiguration(appSettingsConfig);
    }

    private static void SetupPMSServices(MauiAppBuilder builder)
    {
        // TODO: Show an error page instead of throwing an exception
        var serverAddress = builder.Configuration["pmsServerAddress"] ?? throw new InvalidOperationException("Unable to find address of pms server");
        builder.Services.AddPMSServices(serverAddress);
    }

#if WINDOWS
    private static void StartHostedServices(IServiceProvider services)
    {
        var hostedServices = services.GetRequiredService<IEnumerable<IHostedService>>();
        foreach (var service in hostedServices)
        {
            service.StartAsync(default).Wait();
        }
    }
#endif
}
