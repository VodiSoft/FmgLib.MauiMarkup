#nullable enable

namespace FmgLib.MauiMarkup;

public sealed class PropertyContext<T>
{
    public BindableObject BindableObject { get; }

    public BindableProperty Property { get; }

    /// <summary>
    /// Initializes a new instance of the <c>PropertyContext</c> class.
    /// </summary>
    /// <param name="bindableObject">The value used for <paramref name="bindableObject"/>.</param>
    /// <param name="property">The value used for <paramref name="property"/>.</param>
    public PropertyContext(BindableObject bindableObject, BindableProperty property)
    {
        BindableObject = bindableObject ?? throw new ArgumentNullException(nameof(bindableObject));
        Property = property ?? throw new ArgumentNullException(nameof(property));
    }
}
