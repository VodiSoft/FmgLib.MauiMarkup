namespace FmgLib.MauiMarkup;

public static partial class ShellExtension
{
    
    public static T Items<T>(this T self,
        params ShellItem[] items)
        where T : Shell
    {
        foreach (var item in items)
            self.Items.Add(item);
        return self;
    }

    public static T Items<T>(this T self,
        Func<ShellItem[]> configure)
        where T : Shell
    {
        var items = configure();
        foreach (var item in items)
            self.Items.Add(item);
        return self;
    }
    
}
