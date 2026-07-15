namespace FmgLib.MauiMarkup;

public static partial class ShellItemExtension
{
    
    public static T Items<T>(this T self,
        params ShellSection[] items)
        where T : ShellItem
    {
        foreach (var item in items)
            self.Items.Add(item);
        return self;
    }

    public static T Items<T>(this T self,
        Func<ShellSection[]> configure)
        where T : ShellItem
    {
        var items = configure();
        foreach (var item in items)
            self.Items.Add(item);
        return self;
    }
    
}