namespace FmgLib.MauiMarkup;

public static partial class BoxViewExtension
{

    public static Microsoft.Maui.Controls.BoxView CornerRadius(this Microsoft.Maui.Controls.BoxView self,
        double topLeft, double topRight, double bottomLeft, double bottomRight)
    {
        self.SetValue(Microsoft.Maui.Controls.BoxView.CornerRadiusProperty, new CornerRadius(topLeft, topRight, bottomLeft, bottomRight));
        return self;
    }

    public static SettersContext<Microsoft.Maui.Controls.BoxView> CornerRadius(this SettersContext<Microsoft.Maui.Controls.BoxView> self,
        double topLeft, double topRight, double bottomLeft, double bottomRight)
    {
        self.XamlSetters.Add(new Setter { Property = Microsoft.Maui.Controls.BoxView.CornerRadiusProperty, Value = new CornerRadius(topLeft, topRight, bottomLeft, bottomRight) });
        return self;
    }
}
