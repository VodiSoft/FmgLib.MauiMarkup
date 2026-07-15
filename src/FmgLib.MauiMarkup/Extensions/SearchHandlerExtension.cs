using System.Threading;
using System.Threading.Tasks;

namespace FmgLib.MauiMarkup;

public static partial class SearchHandlerExtension
{
    public static Task ShowSoftInputAsync(this SearchHandler self, CancellationToken cancellationToken = default)
    {
        return self.ShowSoftInputAsync(cancellationToken);
    }

    public static Task HideSoftInputAsync(this SearchHandler self, CancellationToken cancellationToken = default)
    {
        return self.HideSoftInputAsync(cancellationToken);
    }

    public static T DisplayMemberName<T>(this T self,
        string displayMemberName)
        where T : SearchHandler
    {
        self.SetValue(SearchHandler.DisplayMemberNameProperty, displayMemberName);
        return self;
    }
    
    public static T DisplayMemberName<T>(this T self, Func<PropertyContext<string>, IPropertyBuilder<string>> configure)
        where T : SearchHandler
    {
        var context = new PropertyContext<string>(self, SearchHandler.DisplayMemberNameProperty);
        configure(context).Build();
        return self;
    }
    
    public static SettersContext<T> DisplayMemberName<T>(this SettersContext<T> self,
        string displayMemberName)
        where T : SearchHandler
    {
        self.XamlSetters.Add(new Setter { Property = SearchHandler.DisplayMemberNameProperty, Value = displayMemberName });
        return self;
    }
    
    public static SettersContext<T> DisplayMemberName<T>(this SettersContext<T> self, Func<PropertySettersContext<string>, IPropertySettersBuilder<string>> configure)
        where T : SearchHandler
    {
        var context = new PropertySettersContext<string>(self.XamlSetters, SearchHandler.DisplayMemberNameProperty);
        configure(context).Build();
        return self;
    }
    
    public static T OnFocusChangeRequested<T>(this T self, EventHandler<VisualElement.FocusRequestArgs> handler)
        where T : SearchHandler
    {
        self.FocusChangeRequested += handler;
        return self;
    }
    
    public static T OnFocusChangeRequested<T>(this T self, Action<T> action)
        where T : SearchHandler
    {
        self.FocusChangeRequested += (o, arg) => action(self);
        return self;
    }
    
}
