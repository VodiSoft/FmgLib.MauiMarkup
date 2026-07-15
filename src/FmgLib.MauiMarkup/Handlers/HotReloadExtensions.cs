namespace FmgLib.MauiMarkup;

public static class HotReloadExtensions
{
    public static TPage InitializeHotReload<TPage>(this TPage page) where TPage : IFmgLibHotReload
    {
        page.Build();

        // Always register — registration is a single weak reference (never extends the page's
        // lifetime, costs nothing in production where no update ever arrives) and gating it on
        // capability checks proved fragile: some launchers (e.g. dotnet watch driving an iOS
        // simulator) deliver updates even when the capability flags read false at startup.
        FmgLibHotReloadHandler.Register(page);

        return page;
    }
}