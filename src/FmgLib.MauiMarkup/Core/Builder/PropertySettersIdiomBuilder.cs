namespace FmgLib.MauiMarkup;

public sealed class PropertySettersIdiomBuilder<T> : IPropertySettersBuilder<T>
{
    public PropertySettersContext<T> Context { get; set; }

    T newValue;
    T defaultValue;
    Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> defaultConfigure;

    bool isSet;
    bool defaultIsSet;
    bool buildValue;

    /// <summary>
    /// Initializes a new instance of the <c>PropertySettersIdiomBuilder</c> class.
    /// </summary>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    public PropertySettersIdiomBuilder(PropertySettersContext<T> context)
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
    public PropertySettersIdiomBuilder<T> Default(T value)
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
    public PropertySettersIdiomBuilder<T> Default(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
    {
        if (!defaultIsSet)
        {
            this.defaultConfigure = configure;
            this.defaultIsSet = true;
        }
        return this;
    }


    /// <summary>
    /// Executes the <c>OnPhone</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersIdiomBuilder<T> OnPhone(T value)
    {
        if (!isSet && DeviceInfo.Idiom == DeviceIdiom.Phone)
        {
            newValue = value;
            buildValue = true;
            isSet = true;
        }
        return this;
    }

    /// <summary>
    /// Executes the <c>OnPhone</c> operation.
    /// </summary>
    /// <param name="configure">The value used for <paramref name="configure"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersIdiomBuilder<T> OnPhone(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Idiom == DeviceIdiom.Phone)
            isSet = configure(Context).Build();
        return this;
    }


    /// <summary>
    /// Executes the <c>OnTablet</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersIdiomBuilder<T> OnTablet(T value)
    {
        if (!isSet && DeviceInfo.Idiom == DeviceIdiom.Tablet)
        {
            newValue = value;
            buildValue = true;
            isSet = true;
        }
        return this;
    }

    /// <summary>
    /// Executes the <c>OnTablet</c> operation.
    /// </summary>
    /// <param name="configure">The value used for <paramref name="configure"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersIdiomBuilder<T> OnTablet(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Idiom == DeviceIdiom.Tablet)
            isSet = configure(Context).Build();
        return this;
    }


    /// <summary>
    /// Executes the <c>OnDesktop</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersIdiomBuilder<T> OnDesktop(T value)
    {
        if (!isSet && DeviceInfo.Idiom == DeviceIdiom.Desktop)
        {
            newValue = value;
            buildValue = true;
            isSet = true;
        }
        return this;
    }

    /// <summary>
    /// Executes the <c>OnDesktop</c> operation.
    /// </summary>
    /// <param name="configure">The value used for <paramref name="configure"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersIdiomBuilder<T> OnDesktop(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Idiom == DeviceIdiom.Desktop)
            isSet = configure(Context).Build();
        return this;
    }


    /// <summary>
    /// Executes the <c>OnTV</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersIdiomBuilder<T> OnTV(T value)
    {
        if (!isSet && DeviceInfo.Idiom == DeviceIdiom.TV)
        {
            newValue = value;
            buildValue = true;
            isSet = true;
        }
        return this;
    }

    /// <summary>
    /// Executes the <c>OnTV</c> operation.
    /// </summary>
    /// <param name="configure">The value used for <paramref name="configure"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersIdiomBuilder<T> OnTV(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Idiom == DeviceIdiom.TV)
            isSet = configure(Context).Build();
        return this;
    }


    /// <summary>
    /// Executes the <c>OnWatch</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersIdiomBuilder<T> OnWatch(T value)
    {
        if (!isSet && DeviceInfo.Idiom == DeviceIdiom.Watch)
        {
            newValue = value;
            buildValue = true;
            isSet = true;
        }
        return this;
    }

    /// <summary>
    /// Executes the <c>OnWatch</c> operation.
    /// </summary>
    /// <param name="configure">The value used for <paramref name="configure"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersIdiomBuilder<T> OnWatch(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Idiom == DeviceIdiom.Watch)
            isSet = configure(Context).Build();
        return this;
    }
}
