namespace FmgLib.MauiMarkup;

public static partial class RoundRectangleGeometryExtension
{
    
    public static T Rect<T>(this T self, double x, double y, double width, double height)
            where T : Microsoft.Maui.Controls.Shapes.RectangleGeometry
    {
        self.SetValue(Microsoft.Maui.Controls.Shapes.RoundRectangleGeometry.RectProperty, new Rect(x, y, width, height));
        return self;
    }

    public static Microsoft.Maui.Controls.Shapes.RoundRectangleGeometry CornerRadius(this Microsoft.Maui.Controls.Shapes.RoundRectangleGeometry self,
        double topLeft, double topRight, double bottomLeft, double bottomRight)
    {
        self.SetValue(Microsoft.Maui.Controls.Shapes.RoundRectangle.CornerRadiusProperty, new CornerRadius(topLeft, topRight, bottomLeft, bottomRight));
        return self;
    }

    public static SettersContext<Microsoft.Maui.Controls.Shapes.RoundRectangleGeometry> CornerRadius(this SettersContext<Microsoft.Maui.Controls.Shapes.RoundRectangleGeometry> self,
        double topLeft, double topRight, double bottomLeft, double bottomRight)
    {
        self.XamlSetters.Add(new Setter { Property = Microsoft.Maui.Controls.Shapes.RoundRectangle.CornerRadiusProperty, Value = new CornerRadius(topLeft, topRight, bottomLeft, bottomRight) });
        return self;
    }
}
