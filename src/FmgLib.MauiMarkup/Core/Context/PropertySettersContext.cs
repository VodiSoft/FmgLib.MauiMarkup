namespace FmgLib.MauiMarkup;

public sealed class PropertySettersContext<T>
{
    public List<Setter> XamlSetters = new List<Setter>();

    public BindableProperty Property { get; set; }

    /// <summary>
    /// Initializes a new instance of the <c>PropertySettersContext</c> class.
    /// </summary>
    /// <param name="xamlSetters">The value used for <paramref name="xamlSetters"/>.</param>
    /// <param name="property">The value used for <paramref name="property"/>.</param>
    public PropertySettersContext(List<Setter> xamlSetters, BindableProperty property)
    {
        XamlSetters = xamlSetters;
        Property = property;
    }
}
