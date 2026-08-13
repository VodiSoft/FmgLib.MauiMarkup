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
