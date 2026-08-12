namespace FmgLib.MauiMarkup.Gallery;

/// <summary>
/// Application root. The app-wide styles are registered here once; every page then inherits a
/// consistent look without repeating a single colour.
/// </summary>
public partial class App : Application
{
    public App()
    {
        this
            .Resources(new ResourceDictionary().MergedDictionaries(AppStyles.Default))
            .UserAppTheme(AppTheme.Unspecified);

        SmokeTestAllPages();   // TEMPORARY
    }

    // TEMPORARY: construct every catalogued page so a Build() that throws is reported here instead of
    // only when a user happens to navigate to it.
    private static void SmokeTestAllPages()
    {
        foreach (var demo in Models.DemoCatalog.All)
        {
            try
            {
                _ = Activator.CreateInstance(demo.PageType);
                Console.WriteLine($"SMOKE OK   {demo.Route}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SMOKE FAIL {demo.Route}: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        Console.WriteLine("SMOKE DONE");
    }

    /// <inheritdoc/>
    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(new AppShell())
            .Title("FmgLib.MauiMarkup Gallery")
            .Width(1180)
            .Height(820)
            .MinimumWidth(420)
            .MinimumHeight(560);
}
