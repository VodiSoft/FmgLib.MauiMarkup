namespace FmgLib.MauiMarkup;

public static partial class ButtonExtension
{

    public static T Padding<T>(this T self, double horizontalSize, double verticalSize) where T : Microsoft.Maui.Controls.Button
    {
        self.SetValue(Microsoft.Maui.Controls.Button.PaddingProperty, new Thickness(horizontalSize, verticalSize));
        return self;
    }

    public static T Padding<T>(this T self, double left, double top, double right, double bottom) where T : Microsoft.Maui.Controls.Button
    {
        self.SetValue(Microsoft.Maui.Controls.Button.PaddingProperty, new Thickness(left, top, right, bottom));
        return self;
    }

    public static T Padding<T>(this T self, object _ = null, double left = 0.0, double top = 0.0, double right = 0.0, double bottom = 0.0) where T : Microsoft.Maui.Controls.Button
    {
        self.SetValue(Microsoft.Maui.Controls.Button.PaddingProperty, new Thickness(left, top, right, bottom));
        return self;
    }

}
