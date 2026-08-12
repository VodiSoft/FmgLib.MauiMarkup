using Microsoft.Extensions.Logging;

namespace FmgLib.MauiMarkup.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
< !--#if (includeContent)-->
            // Localization.json ships as a MauiAsset (Resources/Raw). Loading is synchronous and
            // throws on a missing or malformed file, so translations are guaranteed to be in place
            // before the first page is built. The startup language is whatever the user picked last.
            .UseMauiMarkupLocalization(o => o
                .UseFiles("Localization.json")
                .UseDefaultCulture(Samples.ViewModels.SettingsViewModel.ReadStoredLanguage() ?? "en-US")
                .UseFallbackCulture("en-US"))
< !--#endif-->
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Logging.AddDebug();

        builder.Services
            .AddSingleton<App>()
            .AddSingleton<AppShell>()
< !--#if (!includeContent)-->
            .AddScoped<MainPage>()
< !--#endif-->
< !--#if (includeContent)-->
            .AddScoped<Samples.GettingStartedPage>()
            .AddScoped<Samples.Pages.DashboardPage>()
            .AddScoped<Samples.Pages.ProductsPage>()
            .AddScoped<Samples.Pages.AnimationsPage>()
            .AddScoped<Samples.Pages.SettingsPage>()
< !--#endif-->
            ;

        return builder.Build();
    }
}
