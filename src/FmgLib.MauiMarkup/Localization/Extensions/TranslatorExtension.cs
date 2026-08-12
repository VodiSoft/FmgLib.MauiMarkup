#nullable enable

using System.Globalization;
using FmgLib.MauiMarkup.Localization;

namespace FmgLib.MauiMarkup;

/// <summary>
/// Fluent property builders that bind a property to a translation, so it re-reads itself whenever the
/// culture changes.
/// </summary>
public static class TranslatorExtension
{
    /// <summary>
    /// Binds a string property to a JSON translation key.
    /// </summary>
    /// <param name="self">The property context.</param>
    /// <param name="key">The translation key.</param>
    /// <returns>The binding builder, for further configuration.</returns>
    public static PropertyBindingBuilder<string> Translate(this PropertyContext<string> self, string key)
    {
        return new PropertyBindingBuilder<string>(self).Path($"[{key}]").Source(Translator.Instance).BindingMode(BindingMode.OneWay);
    }

    /// <summary>
    /// Binds a string property to a RESX resource key.
    /// </summary>
    /// <param name="self">The property context.</param>
    /// <param name="key">The resource key.</param>
    /// <returns>The binding builder, for further configuration.</returns>
    public static PropertyBindingBuilder<string> TranslateResx(this PropertyContext<string> self, string key)
    {
        return new PropertyBindingBuilder<string>(self).Path($"[{key}]").Source(TranslatorResx.Instance).BindingMode(BindingMode.OneWay);
    }

    /// <summary>
    /// Binds a string property to a JSON translation used as a composite format string, with the
    /// arguments taken from the binding context.
    /// </summary>
    /// <remarks>
    /// A translated sentence rarely stands alone — <c>"Welcome, {0}!"</c> / <c>"Hoş geldin, {0}!"</c>.
    /// <c>Translate</c> alone cannot express that, and formatting in code produces a string that never
    /// updates. This builds a <c>MultiBinding</c> over the translation plus one binding per argument
    /// path, so the label re-renders when the LANGUAGE changes and when any ARGUMENT changes:
    /// <code>
    /// new Label().Text(e => e.TranslateFormat("WelcomeUser", nameof(vm.UserName)))
    /// new Label().Text(e => e.TranslateFormat("CartSummary", nameof(vm.ItemCount), nameof(vm.Total)))
    /// </code>
    /// Arguments are formatted with the translator's current culture, so <c>{0:C}</c> and <c>{1:d}</c>
    /// follow the selected language rather than the device's.
    /// </remarks>
    /// <param name="self">The property context.</param>
    /// <param name="key">The translation key whose value is the format string.</param>
    /// <param name="argumentPaths">Binding paths, resolved against the binding context, for <c>{0}</c>, <c>{1}</c>, …</param>
    /// <returns>The binding builder, for further configuration.</returns>
    public static PropertyBindingBuilder<string> TranslateFormat(this PropertyContext<string> self, string key, params string[] argumentPaths)
        => Format(self, Translator.Instance, key, argumentPaths);

    /// <summary>
    /// RESX counterpart of <see cref="TranslateFormat(PropertyContext{string}, string, string[])"/>.
    /// </summary>
    /// <param name="self">The property context.</param>
    /// <param name="key">The resource key whose value is the format string.</param>
    /// <param name="argumentPaths">Binding paths, resolved against the binding context, for <c>{0}</c>, <c>{1}</c>, …</param>
    /// <returns>The binding builder, for further configuration.</returns>
    public static PropertyBindingBuilder<string> TranslateResxFormat(this PropertyContext<string> self, string key, params string[] argumentPaths)
        => Format(self, TranslatorResx.Instance, key, argumentPaths);

    /// <summary>
    /// Binds a <see cref="FlowDirection"/> property to the active culture, so the layout mirrors itself
    /// for right-to-left languages the moment the language is switched.
    /// </summary>
    /// <remarks>
    /// Translating the strings of an Arabic or Hebrew UI without mirroring it leaves the layout wrong in
    /// a way no amount of translation fixes. Apply it once, on the page:
    /// <code>
    /// this.FlowDirection(e => e.FromCulture())
    /// </code>
    /// </remarks>
    /// <param name="self">The property context.</param>
    /// <param name="translator">Translator to follow; defaults to <see cref="Translator.Instance"/>.</param>
    /// <returns>The binding builder, for further configuration.</returns>
    public static PropertyBindingBuilder<FlowDirection> FromCulture(this PropertyContext<FlowDirection> self, BaseTranslator? translator = null)
    {
        return new PropertyBindingBuilder<FlowDirection>(self)
            .Path(nameof(BaseTranslator.FlowDirection))
            .Source(translator ?? Translator.Instance)
            .BindingMode(BindingMode.OneWay);
    }

    private static PropertyBindingBuilder<string> Format(PropertyContext<string> self, BaseTranslator translator, string key, string[] argumentPaths)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        var builder = new PropertyBindingBuilder<string>(self)
            .Path($"[{key}]")
            .Source(translator);

        if (argumentPaths != null)
        {
            foreach (var path in argumentPaths)
            {
                if (string.IsNullOrWhiteSpace(path))
                    throw new ArgumentException("An argument path cannot be empty.", nameof(argumentPaths));

                // No Source(): these resolve against the element's BindingContext, i.e. the view model.
                builder.Path(path);
            }
        }

        return builder
            .MultiMode(BindingMode.OneWay)
            .MultiConvertRaw(values => Combine(values, translator.CurrentCulture));
    }

    private static string Combine(object?[] values, CultureInfo culture)
    {
        if (values is null || values.Length == 0)
            return string.Empty;

        var pattern = values[0] as string ?? string.Empty;

        if (values.Length == 1)
            return pattern;

        var arguments = new object?[values.Length - 1];
        Array.Copy(values, 1, arguments, 0, arguments.Length);

        try
        {
            return string.Format(culture, pattern, arguments);
        }
        catch (FormatException)
        {
            // A translator edited "{0}" out of one language must not crash the app in that language —
            // show the unformatted pattern, which makes the broken translation obvious on screen.
            return pattern;
        }
    }
}
