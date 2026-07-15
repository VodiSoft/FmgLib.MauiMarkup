namespace FmgLib.MauiMarkup;

public static partial class ContentViewExtension
{
    public static T Content_ContentProp<T>(this T self,
            Microsoft.Maui.Controls.View content)
            where T : Microsoft.Maui.Controls.ContentView
    {
        self.Content = content;
        return self;
    }

    public static T Content_ContentProp<T>(this T self,
        Func<Microsoft.Maui.Controls.View> configure)
        where T : Microsoft.Maui.Controls.ContentView
    {
        var content = configure();
        self.Content = content;
        return self;
    }

}