using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PMS.Client.ViewModels;
using PMS.Client.Views;
using PMS.Lib;

namespace PMS.Client;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


        // TODO: Don't use wait and put this code in an async function somewhere
        SetupConfigurationsAsync(builder.Configuration).Wait();
        SetupPMSServices(builder);

        builder.Services.AddTransient<MainMenuViewModel>();
        builder.Services.AddTransient<ProductLookupByIdViewModel>();

        builder.Services.AddTransient<ProductLookupById>();
        builder.Services.AddTransient<MainMenu>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
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
}
