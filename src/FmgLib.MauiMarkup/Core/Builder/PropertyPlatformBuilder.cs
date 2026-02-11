namespace FmgLib.MauiMarkup;

public sealed class PropertyPlatformBuilder<T> : IPropertyBuilder<T>
{
    private T newValue;

    private T defaultValue;

    private Func<PropertyContext<T>, IPropertyBuilder<T>> defaultConfigure;

    private bool isSet;

    private bool defaultIsSet;

    private bool buildValue;

    public PropertyContext<T> Context { get; set; }

    /// <summary>
    /// Initializes a new instance of the <c>PropertyPlatformBuilder</c> class.
    /// </summary>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    public PropertyPlatformBuilder(PropertyContext<T> context)
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
    public PropertyPlatformBuilder<T> Default(T value)
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
    public PropertyPlatformBuilder<T> Default(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
    {
        if (!defaultIsSet)
        {
            defaultConfigure = configure;
            defaultIsSet = true;
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnMacCatalyst</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyPlatformBuilder<T> OnMacCatalyst(T value)
    {
        if (!isSet && DeviceInfo.Platform == DevicePlatform.MacCatalyst)
        {
            newValue = value;
            buildValue = true;
            isSet = true;
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnMacCatalyst</c> operation.
    /// </summary>
    /// <param name="configure">The value used for <paramref name="configure"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyPlatformBuilder<T> OnMacCatalyst(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Platform == DevicePlatform.MacCatalyst)
        {
            isSet = configure(Context).Build();
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OniOS</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyPlatformBuilder<T> OniOS(T value)
    {
        if (!isSet && DeviceInfo.Platform == DevicePlatform.iOS)
        {
            newValue = value;
            buildValue = true;
            isSet = true;
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OniOS</c> operation.
    /// </summary>
    /// <param name="configure">The value used for <paramref name="configure"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyPlatformBuilder<T> OniOS(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Platform == DevicePlatform.iOS)
        {
            isSet = configure(Context).Build();
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnAndroid</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyPlatformBuilder<T> OnAndroid(T value)
    {
        if (!isSet && DeviceInfo.Platform == DevicePlatform.Android)
        {
            newValue = value;
            buildValue = true;
            isSet = true;
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnAndroid</c> operation.
    /// </summary>
    /// <param name="configure">The value used for <paramref name="configure"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyPlatformBuilder<T> OnAndroid(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Platform == DevicePlatform.Android)
        {
            isSet = configure(Context).Build();
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnWinUI</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyPlatformBuilder<T> OnWinUI(T value)
    {
        if (!isSet && DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            newValue = value;
            buildValue = true;
            isSet = true;
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnWinUI</c> operation.
    /// </summary>
    /// <param name="configure">The value used for <paramref name="configure"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyPlatformBuilder<T> OnWinUI(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            isSet = configure(Context).Build();
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnTizen</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyPlatformBuilder<T> OnTizen(T value)
    {
        if (!isSet && DeviceInfo.Platform == DevicePlatform.Tizen)
        {
            newValue = value;
            buildValue = true;
            isSet = true;
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnTizen</c> operation.
    /// </summary>
    /// <param name="configure">The value used for <paramref name="configure"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyPlatformBuilder<T> OnTizen(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Platform == DevicePlatform.Tizen)
        {
            isSet = configure(Context).Build();
        }

        return this;
    }
}
