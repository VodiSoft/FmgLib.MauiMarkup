namespace FmgLib.MauiMarkup;

public static partial class GeometryGroupExtension
{
    public static T Children<T>(this T self,
        IList<Microsoft.Maui.Controls.Shapes.Geometry> children)
        where T : Microsoft.Maui.Controls.Shapes.GeometryGroup
    {
        foreach (var item in children)
            self.Children.Add(item);
        return self;
    }

    public static T Children<T>(this T self,
        params Microsoft.Maui.Controls.Shapes.Geometry[] children)
        where T : Microsoft.Maui.Controls.Shapes.GeometryGroup
    {
        foreach (var item in children)
            self.Children.Add(item);
        return self;
    }

    public static T Children<T>(this T self,
        Func<Microsoft.Maui.Controls.Shapes.Geometry[]> configure)
        where T : Microsoft.Maui.Controls.Shapes.GeometryGroup
    {
        var children = configure();
        foreach (var item in children)
            self.Children.Add(item);
        return self;
    }
    
}