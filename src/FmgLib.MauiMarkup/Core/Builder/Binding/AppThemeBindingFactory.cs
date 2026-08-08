#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FmgLib.MauiMarkup;

/// <summary>
/// Creates the binding behind <c>OnLight</c> / <c>OnDark</c>: the same object XAML builds for
/// <c>{AppThemeBinding}</c>.
/// <para>
/// It is what makes a property keep following the theme after the page has been built — .NET MAUI pushes
/// theme changes down the element tree, and the binding re-evaluates itself when they arrive. Resolving the
/// theme once at build time instead would freeze every colour at whatever was in effect when the page was
/// created.
/// </para>
/// <para>
/// The binding type is internal to .NET MAUI up to and including .NET 10 (it becomes public in .NET 11), and
/// the public markup extension cannot be used outside the XAML service infrastructure, so the type is
/// created reflectively and kept alive for trimming through <see cref="DynamicDependencyAttribute"/>.
/// </para>
/// </summary>
internal static class AppThemeBindingFactory
{
    private const string BindingTypeName = "Microsoft.Maui.Controls.AppThemeBinding";

    private static readonly Type? BindingType = typeof(Application).Assembly.GetType(BindingTypeName);

    private static readonly PropertyInfo? LightProperty = BindingType?.GetProperty("Light");

    private static readonly PropertyInfo? DarkProperty = BindingType?.GetProperty("Dark");

    private static readonly PropertyInfo? DefaultProperty = BindingType?.GetProperty("Default");

    /// <summary><see langword="true"/> when this .NET MAUI build exposes the binding this relies on.</summary>
    public static bool IsSupported =>
        BindingType is not null && LightProperty is not null && DarkProperty is not null && DefaultProperty is not null;

    [DynamicDependency(
        DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties,
        BindingTypeName,
        "Microsoft.Maui.Controls")]
    public static BindingBase Create(
        object? light, bool lightIsSet,
        object? dark, bool darkIsSet,
        object? fallback, bool fallbackIsSet)
    {
        var binding = (BindingBase)Activator.CreateInstance(BindingType!)!;

        if (lightIsSet)
            LightProperty!.SetValue(binding, light);

        if (darkIsSet)
            DarkProperty!.SetValue(binding, dark);

        // A theme that was not declared falls back to Default, as does an unspecified system theme.
        // Without this, declaring only one side would push null into the property.
        DefaultProperty!.SetValue(binding, fallbackIsSet ? fallback : lightIsSet ? light : dark);

        return binding;
    }

    /// <summary>
    /// The value to fall back to when the binding cannot be created, so an unexpected .NET MAUI change
    /// degrades to the behaviour of earlier versions — correct at build time, but frozen afterwards — rather
    /// than leaving the property unset.
    /// </summary>
    public static object? ResolveOnce(
        object? light, bool lightIsSet,
        object? dark, bool darkIsSet,
        object? fallback, bool fallbackIsSet)
    {
        var theme = Application.Current?.RequestedTheme ?? AppTheme.Unspecified;

        if (theme == AppTheme.Dark && darkIsSet)
            return dark;

        if (theme != AppTheme.Dark && lightIsSet)
            return light;

        return fallbackIsSet ? fallback : lightIsSet ? light : dark;
    }
}
