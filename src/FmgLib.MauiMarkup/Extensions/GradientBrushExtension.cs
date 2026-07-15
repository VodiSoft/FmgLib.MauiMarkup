namespace FmgLib.MauiMarkup;

public static partial class GradientBrushExtension
{
    public static T GradientStops<T>(this T self,
        IList<GradientStop> gradientStops)
        where T : GradientBrush
    {
        foreach (var item in gradientStops)
            self.GradientStops.Add(item);
        return self;
    }

    public static T GradientStops<T>(this T self,
        params GradientStop[] gradientStops)
        where T : GradientBrush
    {
        foreach (var item in gradientStops)
            self.GradientStops.Add(item);
        return self;
    }

    public static T GradientStops<T>(this T self,
        Func<GradientStop[]> configure)
        where T : GradientBrush
    {
        var gradientStops = configure();
        foreach (var item in gradientStops)
            self.GradientStops.Add(item);
        return self;
    }
    
}