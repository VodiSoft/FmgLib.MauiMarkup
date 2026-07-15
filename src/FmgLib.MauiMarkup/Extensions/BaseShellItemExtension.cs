namespace FmgLib.MauiMarkup;

public static partial class BaseShellItemExtension
{

    public static T ShellItemTemplate<T>(this T self,
        DataTemplate shellItemTemplate)
        where T : BaseShellItem
    {
        self.SetValue(Shell.ItemTemplateProperty, shellItemTemplate);
        return self;
    }

    public static T ShellItemTemplate<T>(this T self, Func<PropertyContext<DataTemplate>, IPropertyBuilder<DataTemplate>> configure)
        where T : BaseShellItem
    {
        var context = new PropertyContext<DataTemplate>(self, Shell.ItemTemplateProperty);
        configure(context).Build();
        return self;
    }

    public static SettersContext<T> ShellItemTemplate<T>(this SettersContext<T> self,
        DataTemplate shellItemTemplate)
        where T : BaseShellItem
    {
        self.XamlSetters.Add(new Setter { Property = Shell.ItemTemplateProperty, Value = shellItemTemplate });
        return self;
    }

    public static SettersContext<T> ShellItemTemplate<T>(this SettersContext<T> self, Func<PropertySettersContext<DataTemplate>, IPropertySettersBuilder<DataTemplate>> configure)
        where T : BaseShellItem
    {
        var context = new PropertySettersContext<DataTemplate>(self.XamlSetters, Shell.ItemTemplateProperty);
        configure(context).Build();
        return self;
    }

    public static T ShellItemTemplate<T>(this T self, Func<object> loadTemplate)
        where T : BaseShellItem
    {
        self.SetValue(Shell.ItemTemplateProperty, new DataTemplate(loadTemplate));
        return self;
    }

    public static DataTemplate GetShellItemTemplateValue<T>(this T self)
        where T : BaseShellItem
    {
        return (DataTemplate)self.GetValue(Shell.ItemTemplateProperty);
    }

}