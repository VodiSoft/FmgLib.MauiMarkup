namespace FmgLib.MauiMarkup;

public sealed class PropertyDynamicResourcesBuilder<T> : IPropertyBuilder<T>
{
    public PropertyContext<T> Context { get; set; }

    string key = null;

    /// <summary>
    /// Initializes a new instance of the <c>PropertyDynamicResourcesBuilder</c> class.
    /// </summary>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    public PropertyDynamicResourcesBuilder(PropertyContext<T> context)
    {
        Context = context;
    }

    /// <summary>
    /// Builds the configuration for the <c>Build</c> operation.
    /// </summary>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    public bool Build()
    {
        if (key != null)
        {
            if (Context.BindableObject is Microsoft.Maui.Controls.Internals.IDynamicResourceHandler resourceHandler)
            {
                resourceHandler.SetDynamicResource(Context.Property, key);
                return true;
            }
            throw new ArgumentException($"Property {Context.Property.PropertyName} of {Context.BindableObject.GetType().ToString()} can not use dynamic resources");
        }
        return false;
    }

    /// <summary>
    /// Executes the <c>DynamicResource</c> operation.
    /// </summary>
    /// <param name="key">The value used for <paramref name="key"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyDynamicResourcesBuilder<T> DynamicResource(string key) { this.key = key; return this; }
}
