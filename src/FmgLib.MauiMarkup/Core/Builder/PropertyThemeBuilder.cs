// Portions of this file incorporate and extend code originally from Sharp.UI.
// Copyright (c) 2022 Pawel Krzywdzinski
// Licensed under the MIT License. See THIRD-PARTY-NOTICES for details.

#nullable enable

namespace FmgLib.MauiMarkup;

/// <summary>
/// Applies a different value per application theme.
/// <para>
/// The values are handed to an <see cref="AppThemeBinding"/> rather than being resolved once, so the
/// property follows every later theme change — the operating system switching to dark mode as well as the
/// app setting <see cref="Application.UserAppTheme"/> itself.
/// </para>
/// </summary>
public sealed class PropertyThemeBuilder<T> : IPropertyBuilder<T>
{
    private object? lightValue;

    private object? darkValue;

    private object? defaultValue;

    private bool lightIsSet;

    private bool darkIsSet;

    private bool defaultIsSet;

    private Func<PropertyContext<T>, IPropertyBuilder<T>>? lightConfigure;

    private Func<PropertyContext<T>, IPropertyBuilder<T>>? darkConfigure;

    private Func<PropertyContext<T>, IPropertyBuilder<T>>? defaultConfigure;

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
        if (HasConfigure)
            return BuildFromConfigure();

        if (lightIsSet || darkIsSet)
        {
            // SetAppTheme is supported public API and covers everything except an explicit Default, so the
            // common case never needs the binding to be built by hand. A side that was not declared repeats
            // the other one, which is what the binding does through its own Default.
            if (!defaultIsSet)
            {
                Context.BindableObject.SetAppTheme(
                    Context.Property,
                    lightIsSet ? lightValue : darkValue,
                    darkIsSet ? darkValue : lightValue);
            }
            else if (AppThemeBindingFactory.IsSupported)
            {
                Context.BindableObject.SetBinding(Context.Property, CreateBinding());
            }
            else
            {
                Context.BindableObject.SetValue(Context.Property, ResolveOnce());
            }

            return true;
        }

        if (defaultIsSet)
        {
            Context.BindableObject.SetValue(Context.Property, defaultValue);
            return true;
        }

        return false;
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
    public PropertyThemeBuilder<T> OnLight(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
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
    public PropertyThemeBuilder<T> OnDark(T value)
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
    public PropertyThemeBuilder<T> OnDark(Func<PropertyContext<T>, IPropertyBuilder<T>> configure)
    {
        if (!darkIsSet)
        {
            darkConfigure = configure;
            darkIsSet = true;
        }

        return this;
    }

    private bool HasConfigure => lightConfigure is not null || darkConfigure is not null || defaultConfigure is not null;

    private BindingBase CreateBinding()
        => AppThemeBindingFactory.Create(lightValue, lightIsSet, darkValue, darkIsSet, defaultValue, defaultIsSet);

    private object? ResolveOnce()
        => AppThemeBindingFactory.ResolveOnce(lightValue, lightIsSet, darkValue, darkIsSet, defaultValue, defaultIsSet);

    /// <summary>
    /// Nested builders — <c>OnPlatform</c>, <c>OnIdiom</c>, <c>DynamicResource</c> — cannot be carried by an
    /// <see cref="AppThemeBinding"/>, so a builder that uses them is resolved once against the theme in
    /// effect at build time and does not follow later theme changes.
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

    private bool Apply(Func<PropertyContext<T>, IPropertyBuilder<T>>? configure, object? value)
    {
        if (configure is not null)
            return configure(Context).Build();

        Context.BindableObject.SetValue(Context.Property, value);
        return true;
    }
}
