namespace FmgLib.MauiMarkup;

public static partial class PolylineExtension
{
    public static Microsoft.Maui.Controls.Shapes.Polyline Points(this Microsoft.Maui.Controls.Shapes.Polyline self,
        IList<Point> points)
    {
        foreach (var item in points)
            self.Points.Add(item);
        return self;
    }

    public static Microsoft.Maui.Controls.Shapes.Polyline Points(this Microsoft.Maui.Controls.Shapes.Polyline self,
        params Point[] points)
    {
        foreach (var item in points)
            self.Points.Add(item);
        return self;
    }

    public static Microsoft.Maui.Controls.Shapes.Polyline Points(this Microsoft.Maui.Controls.Shapes.Polyline self,
        Func<Point[]> configure)
    {
        var points = configure();
        foreach (var item in points)
            self.Points.Add(item);
        return self;
    }
    
}
