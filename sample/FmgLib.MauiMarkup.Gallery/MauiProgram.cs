using Microsoft.Extensions.Logging;

namespace FmgLib.MauiMarkup.Gallery;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            // The gallery's Localization demo switches between English, Turkish and Arabic at
            // runtime; the file ships as a MauiAsset under Resources/Raw.
            .UseMauiMarkupLocalization(o => o
                .UseFiles("Localization.json")
                .UseDefaultCulture("en-US")
                .UseFallbackCulture("en-US"))
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Logging.AddDebug();

        return builder.Build();
    }
}
