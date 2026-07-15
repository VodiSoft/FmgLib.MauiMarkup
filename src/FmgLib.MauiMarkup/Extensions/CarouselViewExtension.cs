namespace FmgLib.MauiMarkup;

public static partial class CarouselViewExtension
{

    public static T VisibleViews<T>(this T self,
        IList<Microsoft.Maui.Controls.View> visibleViews)
        where T : Microsoft.Maui.Controls.CarouselView
    {
        foreach (var item in visibleViews)
            self.VisibleViews.Add(item);
        return self;
    }

    public static T VisibleViews<T>(this T self,
        params Microsoft.Maui.Controls.View[] visibleViews)
        where T : Microsoft.Maui.Controls.CarouselView
    {
        foreach (var item in visibleViews)
            self.VisibleViews.Add(item);
        return self;
    }

    public static T VisibleViews<T>(this T self,
        Func<Microsoft.Maui.Controls.View[]> configure)
        where T : Microsoft.Maui.Controls.CarouselView
    {
        var visibleViews = configure();
        foreach (var item in visibleViews)
            self.VisibleViews.Add(item);
        return self;
    }

}
