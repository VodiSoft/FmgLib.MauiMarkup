namespace FmgLib.MauiMarkup;

public sealed class PropertySettersDynamicResourcesBuilder<T> : IPropertySettersBuilder<T>
{
    public PropertySettersContext<T> Context { get; set; }

    string key = null;

    /// <summary>
    /// Initializes a new instance of the <c>PropertySettersDynamicResourcesBuilder</c> class.
    /// </summary>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    public PropertySettersDynamicResourcesBuilder(PropertySettersContext<T> context)
    {
        Context = context;
    }

    /// <summary>
    /// Builds the configuration for the <c>Build</c> operation.
    /// </summary>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    public bool Build()
    {
        if (!string.IsNullOrEmpty(key))
        {
            Context.XamlSetters.Add(new Setter { Property = Context.Property, Value = new Microsoft.Maui.Controls.Internals.DynamicResource(key) });
        }

        return false;
    }

    /// <summary>
    /// Executes the <c>DynamicResource</c> operation.
    /// </summary>
    /// <param name="key">The value used for <paramref name="key"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersDynamicResourcesBuilder<T> DynamicResource(string key) { this.key = key; return this; }
}
