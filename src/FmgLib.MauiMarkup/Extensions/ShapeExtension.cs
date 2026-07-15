namespace FmgLib.MauiMarkup;

public static partial class ShapeExtension
{
    
    public static T StrokeDashArray<T>(this T self,
        IList<double> strokeDashArray)
        where T : Microsoft.Maui.Controls.Shapes.Shape
    {
        foreach (var item in strokeDashArray)
            self.StrokeDashArray.Add(item);
        return self;
    }

    public static T StrokeDashArray<T>(this T self,
        params double[] strokeDashArray)
        where T : Microsoft.Maui.Controls.Shapes.Shape
    {
        foreach (var item in strokeDashArray)
            self.StrokeDashArray.Add(item);
        return self;
    }
    
}
