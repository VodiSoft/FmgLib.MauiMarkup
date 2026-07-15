using Microsoft.Maui.Graphics;

namespace FmgLib.MauiMarkup;

public static partial class ElementExtension
{
    
    public static T ContextFlyout<T>(this T self,
       MenuFlyout contextFlyout)
       where T : Element
    {
        self.SetValue(FlyoutBase.ContextFlyoutProperty, contextFlyout);
        return self;
    }

    public static T ContextFlyout<T>(this T self,
       Func<MenuFlyout> configure)
       where T : Element
    {
        var contextFlyout = configure();
        self.SetValue(FlyoutBase.ContextFlyoutProperty, contextFlyout);
        return self;
    }

    public static T ContextFlyout<T>(this T self, Func<PropertyContext<MenuFlyout>, IPropertyBuilder<MenuFlyout>> configure)
        where T : Element
    {
        var context = new PropertyContext<MenuFlyout>(self, FlyoutBase.ContextFlyoutProperty);
        configure(context).Build();
        return self;
    }

    public static SettersContext<T> ContextFlyout<T>(this SettersContext<T> self,
        MenuFlyout contextFlyout)
        where T : Element
    {
        self.XamlSetters.Add(new Setter { Property = FlyoutBase.ContextFlyoutProperty, Value = contextFlyout });
        return self;
    }

    public static SettersContext<T> ContextFlyout<T>(this SettersContext<T> self, Func<PropertySettersContext<MenuFlyout>, IPropertySettersBuilder<MenuFlyout>> configure)
        where T : Element
    {
        var context = new PropertySettersContext<MenuFlyout>(self.XamlSetters, FlyoutBase.ContextFlyoutProperty);
        configure(context).Build();
        return self;
    }

    public static MenuFlyout GetContextFlyoutValue<T>(this T self)
        where T : Element
    {
        return (MenuFlyout)self.GetValue(FlyoutBase.ContextFlyoutProperty);
    }

}