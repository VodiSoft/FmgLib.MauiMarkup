namespace FmgLib.MauiMarkup;

public static partial class ShellContentExtension
{

    public static T MenuItems<T>(this T self,
        Func<MenuItem[]> configure)
        where T : Microsoft.Maui.Controls.ShellContent
    {
        var menuItems = configure();
        foreach (var item in menuItems)
            self.MenuItems.Add(item);
        return self;
    }

    public static T MenuItems<T>(this T self,
        params MenuItem[] menuItems)
        where T : Microsoft.Maui.Controls.ShellContent
    {
        foreach (var item in menuItems)
            self.MenuItems.Add(item);
        return self;
    }

}
