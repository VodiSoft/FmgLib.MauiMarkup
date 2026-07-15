namespace FmgLib.MauiMarkup;

public static partial class NavigationPageExtension
{
    
    public static T HasBackButton<T>(this T self,
            bool hasBackButton)
            where T : Microsoft.Maui.Controls.NavigationPage
    {
        self.SetValue(Microsoft.Maui.Controls.NavigationPage.HasBackButtonProperty, hasBackButton);
        return self;
    }

    public static T HasBackButton<T>(this T self, Func<PropertyContext<bool>, IPropertyBuilder<bool>> configure)
        where T : Microsoft.Maui.Controls.NavigationPage
    {
        var context = new PropertyContext<bool>(self, Microsoft.Maui.Controls.NavigationPage.HasBackButtonProperty);
        configure(context).Build();
        return self;
    }

    public static SettersContext<T> HasBackButton<T>(this SettersContext<T> self,
        bool hasBackButton)
        where T : Microsoft.Maui.Controls.NavigationPage
    {
        self.XamlSetters.Add(new Setter { Property = Microsoft.Maui.Controls.NavigationPage.HasBackButtonProperty, Value = hasBackButton });
        return self;
    }

    public static SettersContext<T> HasBackButton<T>(this SettersContext<T> self, Func<PropertySettersContext<bool>, IPropertySettersBuilder<bool>> configure)
        where T : Microsoft.Maui.Controls.NavigationPage
    {
        var context = new PropertySettersContext<bool>(self.XamlSetters, Microsoft.Maui.Controls.NavigationPage.HasBackButtonProperty);
        configure(context).Build();
        return self;
    }

    public static T TitleIconImageSource<T>(this T self,
            Microsoft.Maui.Controls.ImageSource titleIconImageSource)
            where T : Microsoft.Maui.Controls.NavigationPage
    {
        self.SetValue(Microsoft.Maui.Controls.NavigationPage.TitleIconImageSourceProperty, titleIconImageSource);
        return self;
    }

    public static T TitleIconImageSource<T>(this T self, Func<PropertyContext<Microsoft.Maui.Controls.ImageSource>, IPropertyBuilder<Microsoft.Maui.Controls.ImageSource>> configure)
        where T : Microsoft.Maui.Controls.NavigationPage
    {
        var context = new PropertyContext<Microsoft.Maui.Controls.ImageSource>(self, Microsoft.Maui.Controls.NavigationPage.TitleIconImageSourceProperty);
        configure(context).Build();
        return self;
    }

    public static SettersContext<T> TitleIconImageSource<T>(this SettersContext<T> self,
        Microsoft.Maui.Controls.ImageSource titleIconImageSource)
        where T : Microsoft.Maui.Controls.NavigationPage
    {
        self.XamlSetters.Add(new Setter { Property = Microsoft.Maui.Controls.NavigationPage.TitleIconImageSourceProperty, Value = titleIconImageSource });
        return self;
    }

    public static SettersContext<T> TitleIconImageSource<T>(this SettersContext<T> self, Func<PropertySettersContext<Microsoft.Maui.Controls.ImageSource>, IPropertySettersBuilder<Microsoft.Maui.Controls.ImageSource>> configure)
        where T : Microsoft.Maui.Controls.NavigationPage
    {
        var context = new PropertySettersContext<Microsoft.Maui.Controls.ImageSource>(self.XamlSetters, Microsoft.Maui.Controls.NavigationPage.TitleIconImageSourceProperty);
        configure(context).Build();
        return self;
    }

    public static T IconColor<T>(this T self,
            Microsoft.Maui.Graphics.Color iconColor)
            where T : Microsoft.Maui.Controls.NavigationPage
    {
        self.SetValue(Microsoft.Maui.Controls.NavigationPage.IconColorProperty, iconColor);
        return self;
    }

    public static T IconColor<T>(this T self, Func<PropertyContext<Microsoft.Maui.Graphics.Color>, IPropertyBuilder<Microsoft.Maui.Graphics.Color>> configure)
        where T : Microsoft.Maui.Controls.NavigationPage
    {
        var context = new PropertyContext<Microsoft.Maui.Graphics.Color>(self, Microsoft.Maui.Controls.NavigationPage.IconColorProperty);
        configure(context).Build();
        return self;
    }

    public static SettersContext<T> IconColor<T>(this SettersContext<T> self,
        Microsoft.Maui.Graphics.Color iconColor)
        where T : Microsoft.Maui.Controls.NavigationPage
    {
        self.XamlSetters.Add(new Setter { Property = Microsoft.Maui.Controls.NavigationPage.IconColorProperty, Value = iconColor });
        return self;
    }

    public static SettersContext<T> IconColor<T>(this SettersContext<T> self, Func<PropertySettersContext<Microsoft.Maui.Graphics.Color>, IPropertySettersBuilder<Microsoft.Maui.Graphics.Color>> configure)
        where T : Microsoft.Maui.Controls.NavigationPage
    {
        var context = new PropertySettersContext<Microsoft.Maui.Graphics.Color>(self.XamlSetters, Microsoft.Maui.Controls.NavigationPage.IconColorProperty);
        configure(context).Build();
        return self;
    }

    public static T TitleView<T>(this T self,
            Microsoft.Maui.Controls.View titleView)
            where T : Microsoft.Maui.Controls.NavigationPage
    {
        self.SetValue(Microsoft.Maui.Controls.NavigationPage.TitleViewProperty, titleView);
        return self;
    }

    public static T TitleView<T>(this T self, Func<PropertyContext<Microsoft.Maui.Controls.View>, IPropertyBuilder<Microsoft.Maui.Controls.View>> configure)
        where T : Microsoft.Maui.Controls.NavigationPage
    {
        var context = new PropertyContext<Microsoft.Maui.Controls.View>(self, Microsoft.Maui.Controls.NavigationPage.TitleViewProperty);
        configure(context).Build();
        return self;
    }

    public static SettersContext<T> TitleView<T>(this SettersContext<T> self,
        Microsoft.Maui.Controls.View titleView)
        where T : Microsoft.Maui.Controls.NavigationPage
    {
        self.XamlSetters.Add(new Setter { Property = Microsoft.Maui.Controls.NavigationPage.TitleViewProperty, Value = titleView });
        return self;
    }

    public static SettersContext<T> TitleView<T>(this SettersContext<T> self, Func<PropertySettersContext<Microsoft.Maui.Controls.View>, IPropertySettersBuilder<Microsoft.Maui.Controls.View>> configure)
        where T : Microsoft.Maui.Controls.NavigationPage
    {
        var context = new PropertySettersContext<Microsoft.Maui.Controls.View>(self.XamlSetters, Microsoft.Maui.Controls.NavigationPage.TitleViewProperty);
        configure(context).Build();
        return self;
    }

}
