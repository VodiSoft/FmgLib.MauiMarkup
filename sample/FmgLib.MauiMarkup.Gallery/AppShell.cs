using FmgLib.MauiMarkup.Gallery.Models;

namespace FmgLib.MauiMarkup.Gallery;

/// <summary>
/// Shell root. Routes come straight from <see cref="DemoCatalog"/>, so a new demo needs no changes
/// here at all.
/// </summary>
public partial class AppShell : Shell
{
    public AppShell()
    {
        DemoCatalog.RegisterRoutes();

        this
        .FlyoutBehavior(FlyoutBehavior.Disabled)
        .BackgroundColor(e => e.OnLight(AppColors.PageLight).OnDark(AppColors.PageDark))
        .ShellForegroundColor(Colors.White)
        .ShellTitleColor(Colors.White)
        .ShellBackgroundColor(e => e.OnLight(AppColors.AccentDeep).OnDark(AppColors.SurfaceDark))
        .Items(
            new ShellContent()
            .Title("Gallery")
            .ContentTemplate(() => new HomePage())
            .Route(nameof(HomePage))
        );
    }
}
