namespace FmgLib.MauiMarkup;

public static partial class PathGeometryExtension
{
    public static Microsoft.Maui.Controls.Shapes.PathGeometry Figures(this Microsoft.Maui.Controls.Shapes.PathGeometry self,
        IList<Microsoft.Maui.Controls.Shapes.PathFigure> figures)
    {
        foreach (var item in figures)
            self.Figures.Add(item);
        return self;
    }

    public static Microsoft.Maui.Controls.Shapes.PathGeometry Figures(this Microsoft.Maui.Controls.Shapes.PathGeometry self,
        params Microsoft.Maui.Controls.Shapes.PathFigure[] figures)
    {
        foreach (var item in figures)
            self.Figures.Add(item);
        return self;
    }

    public static Microsoft.Maui.Controls.Shapes.PathGeometry Figures(this Microsoft.Maui.Controls.Shapes.PathGeometry self,
        Func<Microsoft.Maui.Controls.Shapes.PathFigure[]> configure)
    {
        var figures = configure();
        foreach (var item in figures)
            self.Figures.Add(item);
        return self;
    }
    
}
