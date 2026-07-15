namespace FmgLib.MauiMarkup;

public static partial class BorderExtension
{
    public static T Content_ContentProp<T>(this T self,
        Microsoft.Maui.Controls.View? content)
        where T : Microsoft.Maui.Controls.Border
    {
        self.Content = content;
        return self;
    }

    public static T Content_ContentProp<T>(this T self,
        Func<Microsoft.Maui.Controls.View?> configure)
        where T : Microsoft.Maui.Controls.Border
    {
        var content = configure();
        self.Content = content;
        return self;
    }

}
