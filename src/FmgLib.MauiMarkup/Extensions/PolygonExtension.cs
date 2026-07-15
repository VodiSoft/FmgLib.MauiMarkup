namespace FmgLib.MauiMarkup;

public static partial class PolygonExtension
{
    public static Microsoft.Maui.Controls.Shapes.Polygon Points(this Microsoft.Maui.Controls.Shapes.Polygon self,
        IList<Point> points)
    {
        foreach (var item in points)
            self.Points.Add(item);
        return self;
    }

    public static Microsoft.Maui.Controls.Shapes.Polygon Points(this Microsoft.Maui.Controls.Shapes.Polygon self,
        params Point[] points)
    {
        foreach (var item in points)
            self.Points.Add(item);
        return self;
    }

    public static Microsoft.Maui.Controls.Shapes.Polygon Points(this Microsoft.Maui.Controls.Shapes.Polygon self,
        Func<Point[]> configure)
    {
        var points = configure();
        foreach (var item in points)
            self.Points.Add(item);
        return self;
    }
    
}
