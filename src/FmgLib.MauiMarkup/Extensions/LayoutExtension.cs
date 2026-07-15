namespace FmgLib.MauiMarkup;

public static partial class LayoutExtension
{

    public static T Padding<T>(this T self, double horizontalSize, double verticalSize) where T : Layout
    {
        self.SetValue(Layout.PaddingProperty, new Thickness(horizontalSize, verticalSize));
        return self;
    }

    public static T Padding<T>(this T self, double left, double top, double right, double bottom) where T : Layout
    {
        self.SetValue(Layout.PaddingProperty, new Thickness(left, top, right, bottom));
        return self;
    }

    public static T Padding<T>(this T self, object _ = null, double left = 0.0, double top = 0.0, double right = 0.0, double bottom = 0.0) where T : Layout
    {
        self.SetValue(Layout.PaddingProperty, new Thickness(left, top, right, bottom));
        return self;
    }

    public static T IgnoreSafeArea<T>(this T self,
        bool ignoreSafeArea)
        where T : Layout
    {
        self.IgnoreSafeArea = ignoreSafeArea;
        return self;
    }
    
}
