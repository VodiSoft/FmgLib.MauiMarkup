namespace FmgLib.MauiMarkup;

public static partial class ScrollViewExtension
{
    
    public static T OnScrollToRequested<T>(this T self, EventHandler<ScrollToRequestedEventArgs> handler)
        where T : ScrollView
    {
        self.ScrollToRequested += handler;
        return self;
    }
    
    public static T OnScrollToRequested<T>(this T self, Action<T> action)
        where T : ScrollView
    {
        self.ScrollToRequested += (o, arg) => action(self);
        return self;
    }
    
}
