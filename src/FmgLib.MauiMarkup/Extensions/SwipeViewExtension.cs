using ISwipeItem = Microsoft.Maui.Controls.ISwipeItem;

namespace FmgLib.MauiMarkup;

public static partial class SwipeViewExtension
{
    
    public static T LeftItems<T>(this T self,
        IList<ISwipeItem> leftItems)
        where T : SwipeView
    {
        foreach (var item in leftItems)
            self.LeftItems.Add(item);
        return self;
    }

    public static T LeftItems<T>(this T self,
        params ISwipeItem[] leftItems)
        where T : SwipeView
    {
        foreach (var item in leftItems)
            self.LeftItems.Add(item);
        return self;
    }

    public static T LeftItems<T>(this T self,
        Func<ISwipeItem[]> configure)
        where T : SwipeView
    {
        var leftItems = configure();
        foreach (var item in leftItems)
            self.LeftItems.Add(item);
        return self;
    }
    
    public static T RightItems<T>(this T self,
        IList<ISwipeItem> rightItems)
        where T : SwipeView
    {
        foreach (var item in rightItems)
            self.RightItems.Add(item);
        return self;
    }

    public static T RightItems<T>(this T self,
        params ISwipeItem[] rightItems)
        where T : SwipeView
    {
        foreach (var item in rightItems)
            self.RightItems.Add(item);
        return self;
    }

    public static T RightItems<T>(this T self,
        Func<ISwipeItem[]> configure)
        where T : SwipeView
    {
        var rightItems = configure();
        foreach (var item in rightItems)
            self.RightItems.Add(item);
        return self;
    }
    
    public static T TopItems<T>(this T self,
        IList<ISwipeItem> topItems)
        where T : SwipeView
    {
        foreach (var item in topItems)
            self.TopItems.Add(item);
        return self;
    }

    public static T TopItems<T>(this T self,
        params ISwipeItem[] topItems)
        where T : SwipeView
    {
        foreach (var item in topItems)
            self.TopItems.Add(item);
        return self;
    }

    public static T TopItems<T>(this T self,
        Func<ISwipeItem[]> configure)
        where T : SwipeView
    {
        var topItems = configure();
        foreach (var item in topItems)
            self.TopItems.Add(item);
        return self;
    }
    
    public static T BottomItems<T>(this T self,
        IList<ISwipeItem> bottomItems)
        where T : SwipeView
    {
        foreach (var item in bottomItems)
            self.BottomItems.Add(item);
        return self;
    }

    public static T BottomItems<T>(this T self,
        params ISwipeItem[] bottomItems)
        where T : SwipeView
    {
        foreach (var item in bottomItems)
            self.BottomItems.Add(item);
        return self;
    }

    public static T BottomItems<T>(this T self,
        Func<ISwipeItem[]> configure)
        where T : SwipeView
    {
        var bottomItems = configure();
        foreach (var item in bottomItems)
            self.BottomItems.Add(item);
        return self;
    }
    
    public static T OnOpenRequested<T>(this T self, EventHandler<OpenRequestedEventArgs> handler)
        where T : SwipeView
    {
        self.OpenRequested += handler;
        return self;
    }
    
    public static T OnOpenRequested<T>(this T self, Action<T> action)
        where T : SwipeView
    {
        self.OpenRequested += (o, arg) => action(self);
        return self;
    }
    
    public static T OnCloseRequested<T>(this T self, EventHandler<CloseRequestedEventArgs> handler)
        where T : SwipeView
    {
        self.CloseRequested += handler;
        return self;
    }
    
    public static T OnCloseRequested<T>(this T self, Action<T> action)
        where T : SwipeView
    {
        self.CloseRequested += (o, arg) => action(self);
        return self;
    }
    
}
