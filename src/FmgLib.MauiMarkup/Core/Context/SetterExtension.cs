namespace FmgLib.MauiMarkup;

public static class SetterExtension
{
    /// <summary>
    /// Creates a XAML <see cref="Setter"/> for the given property, e.g. inside a
    /// <c>Style&lt;T&gt;</c> collection initializer:
    /// <c>Entry.TextColorProperty.Set(Colors.White)</c>.
    /// </summary>
    /// <param name="property">The bindable property the setter targets.</param>
    /// <param name="value">The value to assign.</param>
    /// <returns>The created <see cref="Setter"/>.</returns>
    public static Setter Set(this BindableProperty property, object value)
    {
        return new Setter { Property = property, Value = value };
    }
}
