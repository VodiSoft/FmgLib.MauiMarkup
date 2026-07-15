namespace FmgLib.MauiMarkup;

public static partial class TransformGroupExtension
{
    public static Microsoft.Maui.Controls.Shapes.TransformGroup Children(this Microsoft.Maui.Controls.Shapes.TransformGroup self,
        IList<Microsoft.Maui.Controls.Shapes.Transform> children)
    {
        foreach (var item in children)
            self.Children.Add(item);
        return self;
    }

    public static Microsoft.Maui.Controls.Shapes.TransformGroup Children(this Microsoft.Maui.Controls.Shapes.TransformGroup self,
        params Microsoft.Maui.Controls.Shapes.Transform[] children)
    {
        foreach (var item in children)
            self.Children.Add(item);
        return self;
    }

    public static Microsoft.Maui.Controls.Shapes.TransformGroup Children(this Microsoft.Maui.Controls.Shapes.TransformGroup self,
        Func<Microsoft.Maui.Controls.Shapes.Transform[]> configure)
    {
        var children = configure();
        foreach (var item in children)
            self.Children.Add(item);
        return self;
    }
    
}
