namespace FmgLib.MauiMarkup;

public static partial class PointerGestureRecognizerExtension
{
    
    public static PointerGestureRecognizer PointerEnteredCommandParameter(this PointerGestureRecognizer self,
        System.Windows.Input.ICommand pointerEnteredCommandParameter)
    {
        self.SetValue(PointerGestureRecognizer.PointerEnteredCommandParameterProperty, pointerEnteredCommandParameter);
        return self;
    }
    
    public static PointerGestureRecognizer PointerEnteredCommandParameter(this PointerGestureRecognizer self, Func<PropertyContext<System.Windows.Input.ICommand>, IPropertyBuilder<System.Windows.Input.ICommand>> configure)
    {
        var context = new PropertyContext<System.Windows.Input.ICommand>(self, PointerGestureRecognizer.PointerEnteredCommandParameterProperty);
        configure(context).Build();
        return self;
    }
    
    public static SettersContext<PointerGestureRecognizer> PointerEnteredCommandParameter(this SettersContext<PointerGestureRecognizer> self,
        System.Windows.Input.ICommand pointerEnteredCommandParameter)
    {
        self.XamlSetters.Add(new Setter { Property = PointerGestureRecognizer.PointerEnteredCommandParameterProperty, Value = pointerEnteredCommandParameter });
        return self;
    }
    
    public static SettersContext<PointerGestureRecognizer> PointerEnteredCommandParameter(this SettersContext<PointerGestureRecognizer> self, Func<PropertySettersContext<System.Windows.Input.ICommand>, IPropertySettersBuilder<System.Windows.Input.ICommand>> configure)
    {
        var context = new PropertySettersContext<System.Windows.Input.ICommand>(self.XamlSetters, PointerGestureRecognizer.PointerEnteredCommandParameterProperty);
        configure(context).Build();
        return self;
    }
    
    public static PointerGestureRecognizer PointerExitedCommandParameter(this PointerGestureRecognizer self,
        System.Windows.Input.ICommand pointerExitedCommandParameter)
    {
        self.SetValue(PointerGestureRecognizer.PointerExitedCommandParameterProperty, pointerExitedCommandParameter);
        return self;
    }
    
    public static PointerGestureRecognizer PointerExitedCommandParameter(this PointerGestureRecognizer self, Func<PropertyContext<System.Windows.Input.ICommand>, IPropertyBuilder<System.Windows.Input.ICommand>> configure)
    {
        var context = new PropertyContext<System.Windows.Input.ICommand>(self, PointerGestureRecognizer.PointerExitedCommandParameterProperty);
        configure(context).Build();
        return self;
    }
    
    public static SettersContext<PointerGestureRecognizer> PointerExitedCommandParameter(this SettersContext<PointerGestureRecognizer> self,
        System.Windows.Input.ICommand pointerExitedCommandParameter)
    {
        self.XamlSetters.Add(new Setter { Property = PointerGestureRecognizer.PointerExitedCommandParameterProperty, Value = pointerExitedCommandParameter });
        return self;
    }
    
    public static SettersContext<PointerGestureRecognizer> PointerExitedCommandParameter(this SettersContext<PointerGestureRecognizer> self, Func<PropertySettersContext<System.Windows.Input.ICommand>, IPropertySettersBuilder<System.Windows.Input.ICommand>> configure)
    {
        var context = new PropertySettersContext<System.Windows.Input.ICommand>(self.XamlSetters, PointerGestureRecognizer.PointerExitedCommandParameterProperty);
        configure(context).Build();
        return self;
    }
    
    public static PointerGestureRecognizer PointerMovedCommandParameter(this PointerGestureRecognizer self,
        System.Windows.Input.ICommand pointerMovedCommandParameter)
    {
        self.SetValue(PointerGestureRecognizer.PointerMovedCommandParameterProperty, pointerMovedCommandParameter);
        return self;
    }
    
    public static PointerGestureRecognizer PointerMovedCommandParameter(this PointerGestureRecognizer self, Func<PropertyContext<System.Windows.Input.ICommand>, IPropertyBuilder<System.Windows.Input.ICommand>> configure)
    {
        var context = new PropertyContext<System.Windows.Input.ICommand>(self, PointerGestureRecognizer.PointerMovedCommandParameterProperty);
        configure(context).Build();
        return self;
    }
    
    public static SettersContext<PointerGestureRecognizer> PointerMovedCommandParameter(this SettersContext<PointerGestureRecognizer> self,
        System.Windows.Input.ICommand pointerMovedCommandParameter)
    {
        self.XamlSetters.Add(new Setter { Property = PointerGestureRecognizer.PointerMovedCommandParameterProperty, Value = pointerMovedCommandParameter });
        return self;
    }
    
    public static SettersContext<PointerGestureRecognizer> PointerMovedCommandParameter(this SettersContext<PointerGestureRecognizer> self, Func<PropertySettersContext<System.Windows.Input.ICommand>, IPropertySettersBuilder<System.Windows.Input.ICommand>> configure)
    {
        var context = new PropertySettersContext<System.Windows.Input.ICommand>(self.XamlSetters, PointerGestureRecognizer.PointerMovedCommandParameterProperty);
        configure(context).Build();
        return self;
    }
    
}
