namespace FmgLib.MauiMarkup;

public static partial class PolyLineSegmentExtension
{
    public static T Points<T>(this T self,
        IList<Point> points)
        where T : Microsoft.Maui.Controls.Shapes.PolyLineSegment
    {
        foreach (var item in points)
            self.Points.Add(item);
        return self;
    }

    public static T Points<T>(this T self,
        params Point[] points)
        where T : Microsoft.Maui.Controls.Shapes.PolyLineSegment
    {
        foreach (var item in points)
            self.Points.Add(item);
        return self;
    }

    public static T Points<T>(this T self,
        Func<Point[]> configure)
        where T : Microsoft.Maui.Controls.Shapes.PolyLineSegment
    {
        var points = configure();
        foreach (var item in points)
            self.Points.Add(item);
        return self;
    }
    
}
