namespace FmgLib.MauiMarkup;

public static partial class StyleableElementExtension
{
    public static T StyleClass<T>(this T self, params string[] styleClasses) where T : StyleableElement
    {
        self.StyleClass = styleClasses;
        return self;
    }

    public static T @class<T>(this T self, params string[] classes) where T : StyleableElement
    {
        self.@class = classes;
        return self;
    }
}
