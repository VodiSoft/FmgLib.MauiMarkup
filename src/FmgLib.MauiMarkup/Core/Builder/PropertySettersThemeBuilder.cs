// Portions of this file incorporate and extend code originally from Sharp.UI.
// Copyright (c) 2022 Pawel Krzywdzinski
// Licensed under the MIT License. See THIRD-PARTY-NOTICES for details.

#nullable enable

namespace FmgLib.MauiMarkup;

/// <summary>
/// The style setter counterpart of <see cref="PropertyThemeBuilder{T}"/>: the setter stores an
/// <see cref="AppThemeBinding"/>, so every control the style is applied to follows later theme changes.
/// </summary>
public sealed class PropertySettersThemeBuilder<T> : IPropertySettersBuilder<T>
{
    private object? lightValue;

    private object? darkValue;

    private object? defaultValue;

    private bool lightIsSet;

    private bool darkIsSet;

    private bool defaultIsSet;

    private Func<PropertySettersContext<T>, IPropertySettersBuilder<T>>? lightConfigure;

    private Func<PropertySettersContext<T>, IPropertySettersBuilder<T>>? darkConfigure;

    private Func<PropertySettersContext<T>, IPropertySettersBuilder<T>>? defaultConfigure;

    public PropertySettersContext<T> Context { get; set; }

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
        if (HasConfigure)
            return BuildFromConfigure();

        if (lightIsSet || darkIsSet)
        {
            AddSetter(AppThemeBindingFactory.IsSupported ? CreateBinding() : ResolveOnce());
            return true;
        }

        if (defaultIsSet)
        {
            AddSetter(defaultValue);
            return true;
        }

        return false;
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
    public PropertySettersThemeBuilder<T> Default(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
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
    public PropertySettersThemeBuilder<T> OnLight(T value)
    {
        if (!lightIsSet)
        {
            lightValue = value;
            lightIsSet = true;
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
        if (!lightIsSet)
        {
            lightConfigure = configure;
            lightIsSet = true;
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>OnDark</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersThemeBuilder<T> OnDark(T value)
    {
        if (!darkIsSet)
        {
            darkValue = value;
            darkIsSet = true;
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
        if (!darkIsSet)
        {
            darkConfigure = configure;
            darkIsSet = true;
        }

        return this;
    }

    private bool HasConfigure => lightConfigure is not null || darkConfigure is not null || defaultConfigure is not null;

    private void AddSetter(object? value)
        => Context.XamlSetters.Add(new Setter { Property = Context.Property, Value = value });

    private BindingBase CreateBinding()
        => AppThemeBindingFactory.Create(lightValue, lightIsSet, darkValue, darkIsSet, defaultValue, defaultIsSet);

    private object? ResolveOnce()
        => AppThemeBindingFactory.ResolveOnce(lightValue, lightIsSet, darkValue, darkIsSet, defaultValue, defaultIsSet);

    /// <summary>
    /// Nested builders cannot be carried by an <see cref="AppThemeBinding"/>, so a builder that uses them is
    /// resolved once against the theme in effect at build time.
    /// </summary>
    private bool BuildFromConfigure()
    {
        var theme = Application.Current?.RequestedTheme ?? AppTheme.Unspecified;

        if (theme == AppTheme.Light && lightIsSet)
            return Apply(lightConfigure, lightValue);

        if (theme == AppTheme.Dark && darkIsSet)
            return Apply(darkConfigure, darkValue);

        return defaultIsSet && Apply(defaultConfigure, defaultValue);
    }

    private bool Apply(Func<PropertySettersContext<T>, IPropertySettersBuilder<T>>? configure, object? value)
    {
        if (configure is not null)
            return configure(Context).Build();

        AddSetter(value);
        return true;
    }
}
