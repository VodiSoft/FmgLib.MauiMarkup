// Portions of this file incorporate and extend code originally from Sharp.UI.
// Copyright (c) 2022 Pawel Krzywdzinski
// Licensed under the MIT License. See THIRD-PARTY-NOTICES for details.

using System.Linq.Expressions;

namespace FmgLib.MauiMarkup;

public static class PropertySettersContextExtension
{
    public static PropertySettersBindingBuilder<T> Getter<TContext, T, TValue>(this PropertySettersContext<T> self, Expression<Func<TContext, TValue>> getter)
        => new PropertySettersBindingBuilder<T>(self).Getter<TContext, TValue>(getter);
    
    public static PropertySettersBindingBuilder<T> Path<T>(this PropertySettersContext<T> self, string path)
        => new PropertySettersBindingBuilder<T>(self).Path(path);

    public static PropertySettersMultiBindingBuilder<T> Bindings<T>(this PropertySettersContext<T> self, params BindingBase[] bindings)
        => new PropertySettersMultiBindingBuilder<T>(self).Bindings(bindings);

    public static PropertySettersIdiomBuilder<T> OnPhone<T>(this PropertySettersContext<T> self, T value)
        => new PropertySettersIdiomBuilder<T>(self).OnPhone(value);

    public static PropertySettersIdiomBuilder<T> OnPhone<T>(this PropertySettersContext<T> self, Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
        => new PropertySettersIdiomBuilder<T>(self).OnPhone(configure);

    public static PropertySettersIdiomBuilder<T> OnTablet<T>(this PropertySettersContext<T> self, T value)
        => new PropertySettersIdiomBuilder<T>(self).OnTablet(value);

    public static PropertySettersIdiomBuilder<T> OnTablet<T>(this PropertySettersContext<T> self, Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
        => new PropertySettersIdiomBuilder<T>(self).OnTablet(configure);

    public static PropertySettersIdiomBuilder<T> OnDesktop<T>(this PropertySettersContext<T> self, T value)
        => new PropertySettersIdiomBuilder<T>(self).OnDesktop(value);

    public static PropertySettersIdiomBuilder<T> OnDesktop<T>(this PropertySettersContext<T> self, Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
        => new PropertySettersIdiomBuilder<T>(self).OnDesktop(configure);

    public static PropertySettersIdiomBuilder<T> OnTV<T>(this PropertySettersContext<T> self, T value)
        => new PropertySettersIdiomBuilder<T>(self).OnTV(value);

    public static PropertySettersIdiomBuilder<T> OnTV<T>(this PropertySettersContext<T> self, Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
        => new PropertySettersIdiomBuilder<T>(self).OnTV(configure);

    public static PropertySettersIdiomBuilder<T> OnWatch<T>(this PropertySettersContext<T> self, T value)
        => new PropertySettersIdiomBuilder<T>(self).OnWatch(value);

    public static PropertySettersIdiomBuilder<T> OnWatch<T>(this PropertySettersContext<T> self, Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
        => new PropertySettersIdiomBuilder<T>(self).OnWatch(configure);

    public static PropertySettersPlatformBuilder<T> OnMacCatalyst<T>(this PropertySettersContext<T> self, T value)
        => new PropertySettersPlatformBuilder<T>(self).OnMacCatalyst(value);

    public static PropertySettersPlatformBuilder<T> OnMacCatalyst<T>(this PropertySettersContext<T> self, Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
        => new PropertySettersPlatformBuilder<T>(self).OnMacCatalyst(configure);

    public static PropertySettersPlatformBuilder<T> OniOS<T>(this PropertySettersContext<T> self, T value)
        => new PropertySettersPlatformBuilder<T>(self).OniOS(value);

    public static PropertySettersPlatformBuilder<T> OniOS<T>(this PropertySettersContext<T> self, Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
        => new PropertySettersPlatformBuilder<T>(self).OniOS(configure);

    public static PropertySettersPlatformBuilder<T> OnAndroid<T>(this PropertySettersContext<T> self, T value)
        => new PropertySettersPlatformBuilder<T>(self).OnAndroid(value);

    public static PropertySettersPlatformBuilder<T> OnAndroid<T>(this PropertySettersContext<T> self, Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
        => new PropertySettersPlatformBuilder<T>(self).OnAndroid(configure);

    public static PropertySettersPlatformBuilder<T> OnWinUI<T>(this PropertySettersContext<T> self, T value)
        => new PropertySettersPlatformBuilder<T>(self).OnWinUI(value);

    public static PropertySettersPlatformBuilder<T> OnWinUI<T>(this PropertySettersContext<T> self, Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
        => new PropertySettersPlatformBuilder<T>(self).OnWinUI(configure);

    public static PropertySettersPlatformBuilder<T> OnTizen<T>(this PropertySettersContext<T> self, T value)
        => new PropertySettersPlatformBuilder<T>(self).OnTizen(value);

    public static PropertySettersPlatformBuilder<T> OnTizen<T>(this PropertySettersContext<T> self, Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
        => new PropertySettersPlatformBuilder<T>(self).OnTizen(configure);

    public static PropertySettersThemeBuilder<T> OnLight<T>(this PropertySettersContext<T> self, T value)
        => new PropertySettersThemeBuilder<T>(self).OnLight(value);

    public static PropertySettersThemeBuilder<T> OnLight<T>(this PropertySettersContext<T> self, Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
        => new PropertySettersThemeBuilder<T>(self).OnLight(configure);

    public static PropertySettersThemeBuilder<T> OnDark<T>(this PropertySettersContext<T> self, T value)
        => new PropertySettersThemeBuilder<T>(self).OnDark(value);

    public static PropertySettersThemeBuilder<T> OnDark<T>(this PropertySettersContext<T> self, Func<PropertySettersContext<T>, IPropertySettersBuilder<T>> configure)
        => new PropertySettersThemeBuilder<T>(self).OnDark(configure);

    public static PropertySettersDynamicResourcesBuilder<T> DynamicResource<T>(this PropertySettersContext<T> self, string key)
        => new PropertySettersDynamicResourcesBuilder<T>(self).DynamicResource(key);
}
