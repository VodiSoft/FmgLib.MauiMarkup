namespace FmgLib.MauiMarkup;

public sealed class PropertyThemeBuilder<T> : IPropertyBuilder<T>
{
    private T newValue;

    private T defaultValue;

    private Func<PropertyContext<T>, IPropertyBuilder<T>> defaultConfigure;

    private bool isSet;

    private bool defaultIsSet;

    private bool buildValue;

    public PropertyContext<T> Context { get; set; }

    /// <summary>
    /// Initializes a new instance of the <c>PropertyThemeBuilder</c> class.
    /// </summary>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    public PropertyThemeBuilder(PropertyContext<T> context)
    {
        Context = context;
    }

    /// <summary>
    /// Builds the configuration for the <c>Build</c> operation.
    /// </summary>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    public bool Build()
    {
        if (buildValue)
        {
            Context.BindableObject.SetValue(Context.Property, newValue);
        }
        else if (!isSet && defaultIsSet)
        {
            if (defaultConfigure != null)
            {
                isSet = defaultConfigure(Context).Build();
            }
            else
            {
                Context.BindableObject.SetValue(Context.Property, defaultValue);
            }
        }

        return isSet;
    }

    /// <summary>
    /// Executes the <c>Default</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyThemeBuilder<T> Default(T value)
    {
        if (!defaultIsSet)
        {
            defaultValue = value;
            defaultIsSet = true;
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>Default</c> operation.
    /// </summary>
    /// <param name="configure">The value used for <paramref name="configure"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyThemeBuilder<T> Default(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
    {
        if (!defaultIsSet)
        {
            defaultConfigure = configure;
            defaultIsSet = true;
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnLight</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyThemeBuilder<T> OnLight(T value)
    {
        if (!isSet)
        {
            Application? current = Application.Current;
            if (current != null && current.RequestedTheme == AppTheme.Light)
            {
                newValue = value;
                buildValue = true;
                isSet = true;
            }
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnLight</c> operation.
    /// </summary>
    /// <param name="configure">The value used for <paramref name="configure"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyThemeBuilder<T> OnLight(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
    {
        if (!isSet)
        {
            Application? current = Application.Current;
            if (current != null && current.RequestedTheme == AppTheme.Light)
            {
                isSet = configure(Context).Build();
            }
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnDark</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyThemeBuilder<T> OnDark(T value)
    {
        if (!isSet)
        {
            Application? current = Application.Current;
            if (current != null && current.RequestedTheme == AppTheme.Dark)
            {
                newValue = value;
                buildValue = true;
                isSet = true;
            }
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnDark</c> operation.
    /// </summary>
    /// <param name="configure">The value used for <paramref name="configure"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyThemeBuilder<T> OnDark(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
    {
        if (!isSet)
        {
            Application? current = Application.Current;
            if (current != null && current.RequestedTheme == AppTheme.Dark)
            {
                isSet = configure(Context).Build();
            }
        }

        return this;
    }
}
