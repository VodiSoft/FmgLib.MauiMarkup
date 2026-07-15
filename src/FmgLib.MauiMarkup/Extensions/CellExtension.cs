namespace FmgLib.MauiMarkup;

public static partial class CellExtension
{

    public static T OnForceUpdateSizeRequested<T>(this T self, EventHandler handler)
        where T : Cell
    {
        self.ForceUpdateSizeRequested += handler;
        return self;
    }
    
    public static T OnForceUpdateSizeRequested<T>(this T self, Action<T> action)
        where T : Cell
    {
        self.ForceUpdateSizeRequested += (o, arg) => action(self);
        return self;
    }
    
}
