namespace FmgLib.MauiMarkup;

public static partial class PageExtension
{

    public static T IsBusy<T>(this T self,
        bool isBusy)
        where T : Page
    {
        self.SetValue(Page.IsBusyProperty, isBusy);
        return self;
    }

    public static T IsBusy<T>(this T self, Func<PropertyContext<bool>, IPropertyBuilder<bool>> configure)
        where T : Page
    {
        var context = new PropertyContext<bool>(self, Page.IsBusyProperty);
        configure(context).Build();
        return self;
    }

    public static SettersContext<T> IsBusy<T>(this SettersContext<T> self,
        bool isBusy)
        where T : Page
    {
        self.XamlSetters.Add(new Setter { Property = Page.IsBusyProperty, Value = isBusy });
        return self;
    }

    public static SettersContext<T> IsBusy<T>(this SettersContext<T> self, Func<PropertySettersContext<bool>, IPropertySettersBuilder<bool>> configure)
        where T : Page
    {
        var context = new PropertySettersContext<bool>(self.XamlSetters, Page.IsBusyProperty);
        configure(context).Build();
        return self;
    }

    public static T Padding<T>(this T self, double horizontalSize, double verticalSize) where T : Page
    {
        self.SetValue(Page.PaddingProperty, new Thickness(horizontalSize, verticalSize));
        return self;
    }

    public static T Padding<T>(this T self, double left, double top, double right, double bottom) where T : Page
    {
        self.SetValue(Page.PaddingProperty, new Thickness(left, top, right, bottom));
        return self;
    }

    public static T Padding<T>(this T self, object _ = null, double left = 0.0, double top = 0.0, double right = 0.0, double bottom = 0.0) where T : Page
    {
        self.SetValue(Page.PaddingProperty, new Thickness(left, top, right, bottom));
        return self;
    }

    public static T ContainerArea<T>(this T self,
        Rect containerArea)
        where T : Page
    {
        self.ContainerArea = containerArea;
        return self;
    }

    public static T OnLayoutChanged<T>(this T self, EventHandler handler)
        where T : Page
    {
        self.LayoutChanged += handler;
        return self;
    }

    public static T OnLayoutChanged<T>(this T self, Action<T> action)
        where T : Page
    {
        self.LayoutChanged += (o, arg) => action(self);
        return self;
    }

}
