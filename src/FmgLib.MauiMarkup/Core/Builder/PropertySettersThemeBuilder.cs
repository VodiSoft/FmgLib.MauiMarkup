namespace FmgLib.MauiMarkup;

public sealed class PropertySettersThemeBuilder<T> : IPropertySettersBuilder<T>
{
    public PropertySettersContext<T> Context { get; set; }

    T newValue;
    T defaultValue;
    Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> defaultConfigure;

    bool isSet;
    bool defaultIsSet;
    bool buildValue;

    /// <summary>
    /// Initializes a new instance of the <c>PropertySettersThemeBuilder</c> class.
    /// </summary>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    public PropertySettersThemeBuilder(PropertySettersContext<T> context)
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
            Context.XamlSetters.Add(new Setter { Property = Context.Property, Value = newValue });
        else if (!isSet)
        {
            if (defaultIsSet)
            {
                if (defaultConfigure != null)
                    isSet = defaultConfigure(Context).Build();
                else
                    Context.XamlSetters.Add(new Setter { Property = Context.Property, Value = defaultValue });
            }

        }
        return isSet;
    }


    /// <summary>
    /// Executes the <c>Default</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersThemeBuilder<T> Default(T value)
    {
        if (!defaultIsSet)
        {
            this.defaultValue = value;
            this.defaultIsSet = true;
        }
        return this;
    }

    /// <summary>
    /// Executes the <c>Default</c> operation.
    /// </summary>
    /// <param name="configure">The value used for <paramref name="configure"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersThemeBuilder<T> Default(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
    {
        if (!defaultIsSet)
        {
            this.defaultConfigure = configure;
            this.defaultIsSet = true;
        }
        return this;
    }


    /// <summary>
    /// Executes the <c>OnLight</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersThemeBuilder<T> OnLight(T value)
    {
        if (!isSet && Application.Current?.RequestedTheme == AppTheme.Light)
        {
            newValue = value;
            buildValue = true;
            isSet = true;
        }
        return this;
    }

    /// <summary>
    /// Executes the <c>OnLight</c> operation.
    /// </summary>
    /// <param name="configure">The value used for <paramref name="configure"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersThemeBuilder<T> OnLight(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
    {
        if (!isSet && Application.Current?.RequestedTheme == AppTheme.Light)
            isSet = configure(Context).Build();
        return this;
    }


    /// <summary>
    /// Executes the <c>OnDark</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersThemeBuilder<T> OnDark(T value)
    {
        if (!isSet && Application.Current?.RequestedTheme == AppTheme.Dark)
        {
            newValue = value;
            buildValue = true;
            isSet = true;
        }
        return this;
    }

    /// <summary>
    /// Executes the <c>OnDark</c> operation.
    /// </summary>
    /// <param name="configure">The value used for <paramref name="configure"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersThemeBuilder<T> OnDark(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
    {
        if (!isSet && Application.Current?.RequestedTheme == AppTheme.Dark)
            isSet = configure(Context).Build();
        return this;
    }
}
