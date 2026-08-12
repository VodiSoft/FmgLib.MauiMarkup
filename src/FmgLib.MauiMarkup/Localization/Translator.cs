#nullable enable

using System.Globalization;
using FmgLib.MauiMarkup.Localization;

namespace FmgLib.MauiMarkup;

/// <summary>
/// Translator backed by the JSON files loaded into <see cref="LocalizationData"/>.
/// </summary>
/// <remarks>
/// The loaded data is key-major (<c>key → {culture → text}</c>) because that shape is the friendliest
/// to hand-edit. Resolving against it directly costs two dictionary probes per lookup AND has to walk
/// the culture fallback chain on every read. Instead, a flat <c>key → text</c> index for the active
/// culture is built once whenever the culture (or the data) changes: lookups become a single probe and
/// the fallback chain is walked once per language switch rather than once per bound label.
/// </remarks>
public class Translator : BaseTranslator
{
    public static Translator Instance { get; set; } = new Translator();

    private Dictionary<string, string>? index;

    /// <summary>Translation of <paramref name="key"/> in <see cref="BaseTranslator.CurrentCulture"/>.</summary>
    /// <param name="key">The translation key.</param>
    public string this[string key]
    {
        get
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            var current = index ??= BuildIndex();

            return current.TryGetValue(key, out var translation) ? translation : ResolveMissing(key);
        }
    }

    /// <summary>
    /// Translation of <paramref name="key"/> in an explicit culture, independent of the active one.
    /// </summary>
    /// <param name="key">The translation key.</param>
    /// <param name="culture">The culture to resolve in.</param>
    /// <returns>The translation, or the configured missing-key result.</returns>
    public string TranslateString(string key, CultureInfo culture)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        if (culture is null)
            throw new ArgumentNullException(nameof(culture));

        var data = LocalizationData.Data;

        if (data != null && data.TryGetValue(key, out var translations))
        {
            foreach (var cultureName in CultureResolver.BuildChain(culture, FallbackCulture))
            {
                if (translations.TryGetValue(cultureName, out var translation))
                    return translation;
            }
        }

        return ResolveMissing(key);
    }

    /// <summary>
    /// Drops the cached per-culture index and refreshes every bound translation. Called when the loaded
    /// data is replaced, so translations that arrive after the first page is on screen still show up.
    /// </summary>
    public void Refresh()
    {
        index = null;
        OnPropertyChanged();
    }

    /// <inheritdoc/>
    protected override void OnCultureChanged() => index = null;

    private Dictionary<string, string> BuildIndex()
    {
        var data = LocalizationData.Data;
        var result = new Dictionary<string, string>(data?.Count ?? 0, StringComparer.Ordinal);

        if (data is null)
            return result;

        var chain = CultureChain();

        foreach (var entry in data)
        {
            foreach (var cultureName in chain)
            {
                if (entry.Value != null && entry.Value.TryGetValue(cultureName, out var translation))
                {
                    result[entry.Key] = translation;
                    break;
                }
            }
        }

        return result;
    }
}
