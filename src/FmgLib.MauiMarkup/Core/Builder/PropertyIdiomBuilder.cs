namespace FmgLib.MauiMarkup;

public sealed class PropertyIdiomBuilder<T> : IPropertyBuilder<T>
{
    private T newValue;

    private T defaultValue;

    private Func<PropertyContext<T>, IPropertyBuilder<T>> defaultConfigure;

    private bool isSet;

    private bool defaultIsSet;

    private bool buildValue;

    public PropertyContext<T> Context { get; set; }

    /// <summary>
    /// Initializes a new instance of the <c>PropertyIdiomBuilder</c> class.
    /// </summary>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    public PropertyIdiomBuilder(PropertyContext<T> context)
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
    public PropertyIdiomBuilder<T> Default(T value)
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
    public PropertyIdiomBuilder<T> Default(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
    {
        if (!defaultIsSet)
        {
            defaultConfigure = configure;
            defaultIsSet = true;
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnPhone</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyIdiomBuilder<T> OnPhone(T value)
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
    public PropertyIdiomBuilder<T> OnPhone(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Idiom == DeviceIdiom.Phone)
        {
            isSet = configure(Context).Build();
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnTablet</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyIdiomBuilder<T> OnTablet(T value)
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
    public PropertyIdiomBuilder<T> OnTablet(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Idiom == DeviceIdiom.Tablet)
        {
            isSet = configure(Context).Build();
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnDesktop</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyIdiomBuilder<T> OnDesktop(T value)
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
    public PropertyIdiomBuilder<T> OnDesktop(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Idiom == DeviceIdiom.Desktop)
        {
            isSet = configure(Context).Build();
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnTV</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyIdiomBuilder<T> OnTV(T value)
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
    public PropertyIdiomBuilder<T> OnTV(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Idiom == DeviceIdiom.TV)
        {
            isSet = configure(Context).Build();
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnWatch</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyIdiomBuilder<T> OnWatch(T value)
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
    public PropertyIdiomBuilder<T> OnWatch(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Idiom == DeviceIdiom.Watch)
        {
            isSet = configure(Context).Build();
        }

        return this;
    }
}
