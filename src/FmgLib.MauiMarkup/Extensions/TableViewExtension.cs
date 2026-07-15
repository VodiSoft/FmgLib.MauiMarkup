namespace FmgLib.MauiMarkup;

public static partial class TableViewExtension
{
    
    public static T Root<T>(this T self,
        IList<TableSection> root)
        where T : TableView
    {
        foreach (var item in root)
            self.Root.Add(item);
        return self;
    }

    public static T Root<T>(this T self,
        params TableSection[] root)
        where T : TableView
    {
        foreach (var item in root)
            self.Root.Add(item);
        return self;
    }

    public static T Root<T>(this T self,
        Func<TableSection[]> configure)
        where T : TableView
    {
        var root = configure();
        foreach (var item in root)
            self.Root.Add(item);
        return self;
    }
    
    public static T OnModelChanged<T>(this T self, EventHandler handler)
        where T : TableView
    {
        self.ModelChanged += handler;
        return self;
    }
    
    public static T OnModelChanged<T>(this T self, Action<T> action)
        where T : TableView
    {
        self.ModelChanged += (o, arg) => action(self);
        return self;
    }
    
}
