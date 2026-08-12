namespace FmgLib.MauiMarkup.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
< !--#if (includeContent)-->
        Routing.RegisterRoute(nameof(Samples.Pages.DashboardPage), typeof(Samples.Pages.DashboardPage));
        Routing.RegisterRoute(nameof(Samples.Pages.ProductsPage), typeof(Samples.Pages.ProductsPage));
        Routing.RegisterRoute(nameof(Samples.Pages.AnimationsPage), typeof(Samples.Pages.AnimationsPage));
        Routing.RegisterRoute(nameof(Samples.Pages.SettingsPage), typeof(Samples.Pages.SettingsPage));
< !--#endif-->

        this
        .FlyoutBehavior(FlyoutBehavior.Disabled)
        .Items(
< !--#if (!includeContent)-->
            new ShellContent()
            .Title("Home")
            .ContentTemplate(() => new MainPage())
            .Route(nameof(MainPage))
< !--#endif-->
< !--#if (includeContent)-->
            // A TabBar groups the entries into bottom tabs. Two ShellContent items placed directly
            // in Shell.Items would each become their own flyout entry instead, unreachable while
            // FlyoutBehavior is Disabled.
            //
            // The titles are bound, not assigned, so the tab bar follows the selected language like
            // every other translated string on screen.
            new TabBar()
            .Items(
                new ShellContent()
                .Title(e => e.Translate("Nav_GettingStarted"))
                .ContentTemplate(() => new Samples.GettingStartedPage())
                .Route("GettingStarted"),

                new ShellContent()
                .Title(e => e.Translate("Nav_Settings"))
                .ContentTemplate(() => new Samples.Pages.SettingsPage())
                .Route("Settings")
            )
< !--#endif-->
        );
    }
}
