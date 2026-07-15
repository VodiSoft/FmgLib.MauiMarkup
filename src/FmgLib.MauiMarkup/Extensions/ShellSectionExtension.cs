namespace FmgLib.MauiMarkup;

public static partial class ShellSectionExtension
{
    
    public static T Items<T>(this T self,
        params ShellContent[] items)
        where T : ShellSection
    {
        foreach (var item in items)
            self.Items.Add(item);
        return self;
    }

    public static T Items<T>(this T self,
        Func<ShellContent[]> configure)
        where T : ShellSection
    {
        var items = configure();
        foreach (var item in items)
            self.Items.Add(item);
        return self;
    }
    
}
