namespace FmgLib.MauiMarkup;

public static partial class PolyBezierSegmentExtension
{
    public static Microsoft.Maui.Controls.Shapes.PolyBezierSegment Points(this Microsoft.Maui.Controls.Shapes.PolyBezierSegment self,
        IList<Point> points)
    {
        foreach (var item in points)
            self.Points.Add(item);
        return self;
    }

    public static Microsoft.Maui.Controls.Shapes.PolyBezierSegment Points(this Microsoft.Maui.Controls.Shapes.PolyBezierSegment self,
        params Point[] points)
    {
        foreach (var item in points)
            self.Points.Add(item);
        return self;
    }

    public static Microsoft.Maui.Controls.Shapes.PolyBezierSegment Points(this Microsoft.Maui.Controls.Shapes.PolyBezierSegment self,
        Func<Point[]> configure)
    {
        var points = configure();
        foreach (var item in points)
            self.Points.Add(item);
        return self;
    }
    
}
