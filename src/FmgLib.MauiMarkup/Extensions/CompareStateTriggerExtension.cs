namespace FmgLib.MauiMarkup;

public static partial class CompareStateTriggerExtension
{
    public static CompareStateTrigger Binding(this CompareStateTrigger self,
            Func<Binding, Binding> bindingBuilder)
    {
        self.SetValue(CompareStateTrigger.PropertyProperty, bindingBuilder(new Binding()));
        return self;
    }

    public static CompareStateTrigger Property(this CompareStateTrigger self, Func<PropertyContext<object>, IPropertyBuilder<object>> configure)
    {
        var context = new PropertyContext<object>(self, CompareStateTrigger.PropertyProperty);
        configure(context).Build();
        return self;
    }
    
    public static SettersContext<CompareStateTrigger> Property(this SettersContext<CompareStateTrigger> self,
        object property)
    {
        self.XamlSetters.Add(new Setter { Property = CompareStateTrigger.PropertyProperty, Value = property });
        return self;
    }
    
    public static SettersContext<CompareStateTrigger> Property(this SettersContext<CompareStateTrigger> self, Func<PropertySettersContext<object>, IPropertySettersBuilder<object>> configure)
    {
        var context = new PropertySettersContext<object>(self.XamlSetters, CompareStateTrigger.PropertyProperty);
        configure(context).Build();
        return self;
    }
    
}