namespace FmgLib.MauiMarkup.App;

public partial class App : Application
{
    public App()
    {
        this.Resources(new ResourceDictionary().MergedDictionaries(AppStyles.Default));
< !--#if (includeContent)-->

        // Re-apply the theme the user picked last. The language is restored in MauiProgram, before
        // the first page is built, so the app never flashes the wrong one.
        Samples.ViewModels.SettingsViewModel.ApplyStoredTheme();
< !--#endif-->
    }

    protected override Window CreateWindow(IActivationState? activationState) => new Window(new AppShell());
}
