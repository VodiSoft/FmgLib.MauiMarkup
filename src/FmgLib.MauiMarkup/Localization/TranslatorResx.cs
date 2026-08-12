#nullable enable

using System.Globalization;
using System.Resources;
using FmgLib.MauiMarkup.Localization;

namespace FmgLib.MauiMarkup;

/// <summary>
/// Translator backed by a <see cref="System.Resources.ResourceManager"/>, i.e. the classic <c>.resx</c>
/// workflow. Culture fallback (<c>tr-TR</c> → <c>tr</c> → neutral) is handled by the resource manager.
/// </summary>
public class TranslatorResx : BaseTranslator
{
    internal static ResourceManager? ResourceManager { get; set; }

    public static TranslatorResx Instance { get; set; } = new TranslatorResx();

    /// <summary>Translation of <paramref name="key"/> in <see cref="BaseTranslator.CurrentCulture"/>.</summary>
    /// <param name="key">The resource key.</param>
    public string this[string key] => TranslateString(key, CurrentCulture);

    /// <summary>
    /// Translation of <paramref name="key"/> in an explicit culture, independent of the active one.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <param name="culture">The culture to resolve in.</param>
    /// <returns>The translation, or the configured missing-key result.</returns>
    public string TranslateString(string key, CultureInfo culture)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        if (culture is null)
            throw new ArgumentNullException(nameof(culture));

        var resourceManager = ResourceManager
            ?? throw new InvalidOperationException(
                "No ResourceManager is registered. Call " +
                "builder.UseMauiMarkupLocalizationWithResx(AppResources.ResourceManager) in MauiProgram " +
                "before using TranslatorResx or TranslateResx(...).");

        string? translation = null;

        try
        {
            translation = resourceManager.GetString(key, culture);

            // A satellite assembly for the requested culture may be missing while the neutral resources
            // are present — resolve through the configured fallback before giving up.
            if (translation is null && FallbackCulture != null)
                translation = resourceManager.GetString(key, FallbackCulture);
        }
        catch (MissingManifestResourceException)
        {
            // No resource set at all for this culture: treat exactly like a missing key so the JSON and
            // RESX backends behave identically instead of one throwing and the other returning the key.
        }

        // ResourceManager returns null for an unknown key, which used to reach the binding as null and
        // render an empty label — silently different from the JSON translator, which returns the key.
        return translation ?? ResolveMissing(key);
    }
}
