namespace FmgLib.MauiMarkup;

public sealed class PropertySettersPlatformBuilder<T> : IPropertySettersBuilder<T>
{
    public PropertySettersContext<T> Context { get; set; }

    T newValue;
    T defaultValue;
    Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> defaultConfigure;

    bool isSet;
    bool defaultIsSet;
    bool buildValue;

    /// <summary>
    /// Initializes a new instance of the <c>PropertySettersPlatformBuilder</c> class.
    /// </summary>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    public PropertySettersPlatformBuilder(PropertySettersContext<T> context)
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
    public PropertySettersPlatformBuilder<T> Default(T value)
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
    public PropertySettersPlatformBuilder<T> Default(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
    {
        if (!defaultIsSet)
        {
            this.defaultConfigure = configure;
            this.defaultIsSet = true;
        }
        return this;
    }


    /// <summary>
    /// Executes the <c>OnMacCatalyst</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersPlatformBuilder<T> OnMacCatalyst(T value)
    {
        if (!isSet && DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.MacCatalyst)
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
    public PropertySettersPlatformBuilder<T> OnMacCatalyst(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.MacCatalyst)
            isSet = configure(Context).Build();
        return this;
    }


    /// <summary>
    /// Executes the <c>OniOS</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersPlatformBuilder<T> OniOS(T value)
    {
        if (!isSet && DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.iOS)
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
    public PropertySettersPlatformBuilder<T> OniOS(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.iOS)
            isSet = configure(Context).Build();
        return this;
    }


    /// <summary>
    /// Executes the <c>OnAndroid</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersPlatformBuilder<T> OnAndroid(T value)
    {
        if (!isSet && DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.Android)
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
    public PropertySettersPlatformBuilder<T> OnAndroid(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.Android)
            isSet = configure(Context).Build();
        return this;
    }


    /// <summary>
    /// Executes the <c>OnWinUI</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersPlatformBuilder<T> OnWinUI(T value)
    {
        if (!isSet && DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.WinUI)
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
    public PropertySettersPlatformBuilder<T> OnWinUI(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.WinUI)
            isSet = configure(Context).Build();
        return this;
    }


    /// <summary>
    /// Executes the <c>OnTizen</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersPlatformBuilder<T> OnTizen(T value)
    {
        if (!isSet && DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.Tizen)
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
    public PropertySettersPlatformBuilder<T> OnTizen(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
    {
        if (!isSet && DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.Tizen)
            isSet = configure(Context).Build();
        return this;
    }
}
