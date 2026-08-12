#nullable enable

using System.Globalization;

namespace FmgLib.MauiMarkup;

/// <summary>
/// One-shot translation helpers for strings that are not bound — alerts, logs, page arguments.
/// </summary>
/// <remarks>
/// <b>These return a snapshot.</b> <c>new Label().Text("Hello".ToTranslate())</c> compiles and shows the
/// right text, but it never updates when the language changes, because there is no binding behind it.
/// For anything on screen use the property builder instead:
/// <c>new Label().Text(e =&gt; e.Translate("Hello"))</c>.
/// </remarks>
public static class TranslateExtension
{
    /// <summary>Translates a key in the current culture. Not live — see the remarks on the class.</summary>
    /// <param name="key">The translation key.</param>
    /// <returns>The translation at the time of the call.</returns>
    public static string ToTranslate(this string key)
    {
        return Translator.Instance[key];
    }

    /// <summary>Translates a key in an explicit culture. Not live — see the remarks on the class.</summary>
    /// <param name="key">The translation key.</param>
    /// <param name="cultureName">Culture name such as <c>tr-TR</c>.</param>
    /// <returns>The translation at the time of the call.</returns>
    public static string ToTranslate(this string key, string cultureName)
    {
        return Translator.Instance.TranslateString(key, CultureResolver.Parse(cultureName, nameof(cultureName)));
    }

    /// <summary>Translates a RESX key in the current culture. Not live — see the remarks on the class.</summary>
    /// <param name="key">The resource key.</param>
    /// <returns>The translation at the time of the call.</returns>
    public static string ToTranslateResx(this string key)
    {
        return TranslatorResx.Instance[key];
    }

    /// <summary>Translates a RESX key in an explicit culture. Not live — see the remarks on the class.</summary>
    /// <param name="key">The resource key.</param>
    /// <param name="cultureName">Culture name such as <c>tr-TR</c>.</param>
    /// <returns>The translation at the time of the call.</returns>
    public static string ToTranslateResx(this string key, string cultureName)
    {
        return TranslatorResx.Instance.TranslateString(key, CultureResolver.Parse(cultureName, nameof(cultureName)));
    }
}
