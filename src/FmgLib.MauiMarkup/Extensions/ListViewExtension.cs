namespace FmgLib.MauiMarkup;

public static partial class ListViewExtension
{

    public static T OnScrollToRequested<T>(this T self, EventHandler<ScrollToRequestedEventArgs> handler)
        where T : ListView
    {
        self.ScrollToRequested += handler;
        return self;
    }
    
    public static T OnScrollToRequested<T>(this T self, Action<T> action)
        where T : ListView
    {
        self.ScrollToRequested += (o, arg) => action(self);
        return self;
    }
    
}