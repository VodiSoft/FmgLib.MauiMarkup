namespace FmgLib.MauiMarkup;

public static partial class PathFigureExtension
{
    public static Microsoft.Maui.Controls.Shapes.PathFigure Segments(this Microsoft.Maui.Controls.Shapes.PathFigure self,
        IList<Microsoft.Maui.Controls.Shapes.PathSegment> segments)
    {
        foreach (var item in segments)
            self.Segments.Add(item);
        return self;
    }

    public static Microsoft.Maui.Controls.Shapes.PathFigure Segments(this Microsoft.Maui.Controls.Shapes.PathFigure self,
        params Microsoft.Maui.Controls.Shapes.PathSegment[] segments)
    {
        foreach (var item in segments)
            self.Segments.Add(item);
        return self;
    }

    public static Microsoft.Maui.Controls.Shapes.PathFigure Segments(this Microsoft.Maui.Controls.Shapes.PathFigure self,
        Func<Microsoft.Maui.Controls.Shapes.PathSegment[]> configure)
    {
        var segments = configure();
        foreach (var item in segments)
            self.Segments.Add(item);
        return self;
    }
    
}
