#nullable enable

using System.Globalization;

namespace FmgLib.MauiMarkup;

/// <summary>
/// What a translator returns when a key has no translation in the current culture chain.
/// The same policy applies to the JSON and the RESX translator, so switching backend does not
/// silently switch behaviour (RESX used to return <see langword="null"/> where JSON returned the key).
/// </summary>
public enum MissingTranslationBehavior
{
    /// <summary>Return the key itself — the default, and the most useful during development.</summary>
    ReturnKey,

    /// <summary>Return an empty string, for screens where a missing label should simply collapse.</summary>
    ReturnEmpty,

    /// <summary>Return the key wrapped in brackets (<c>⟦Key⟧</c>) so gaps are impossible to miss on screen.</summary>
    Marker,

    /// <summary>Throw a <see cref="KeyNotFoundException"/>; pair with a UI test run to fail the build on gaps.</summary>
    Throw
}

/// <summary>
/// How far a culture change propagates beyond the translator itself.
/// </summary>
/// <remarks>
/// Changing the app language only re-reads translated strings. Dates, numbers and currency come from
/// <see cref="CultureInfo.CurrentCulture"/>, and <c>ResourceManager</c>/satellite assemblies come from
/// <see cref="CultureInfo.CurrentUICulture"/> — neither of which follows a translator automatically.
/// The result is a half-translated app: English labels next to Turkish dates.
/// </remarks>
public enum CultureSyncMode
{
    /// <summary>Only the translator's own bindings change. Ambient formatting stays on the device culture.</summary>
    None,

    /// <summary>Also set <see cref="CultureInfo.DefaultThreadCurrentUICulture"/> — resource lookup follows, formatting does not.</summary>
    UICultureOnly,

    /// <summary>Also set both <see cref="CultureInfo.DefaultThreadCurrentCulture"/> and <see cref="CultureInfo.DefaultThreadCurrentUICulture"/>. The default.</summary>
    Full
}

/// <summary>
/// Configuration for <c>UseMauiMarkupLocalization</c> / <c>UseMauiMarkupLocalizationWithResx</c>.
/// </summary>
/// <remarks>
/// This exists because the original
/// <c>UseMauiMarkupLocalization(string defaultLang = null, params string[] filePaths)</c> shape cannot
/// tell a culture name from a file name: <c>UseMauiMarkupLocalization("Common.json", "Checkout.json")</c>
/// binds the first argument to <c>defaultLang</c> and throws <see cref="CultureNotFoundException"/> at
/// startup. An options delegate has no such ambiguity and leaves room for settings — missing-key policy,
/// fallback culture, culture sync — that would otherwise each need another overload.
/// </remarks>
public sealed class LocalizationOptions
{
    /// <summary>Culture selected at startup. <see langword="null"/> keeps the device culture.</summary>
    public CultureInfo? DefaultCulture { get; set; }

    /// <summary>
    /// Culture consulted after the current culture's own parent chain is exhausted — the equivalent of
    /// the neutral <c>.resx</c>. Typically the language the keys were authored in.
    /// </summary>
    public CultureInfo? FallbackCulture { get; set; }

    /// <summary>JSON files to load, in order; later files win on duplicate keys. Empty means <c>Localization.json</c>.</summary>
    public IList<string> Files { get; } = new List<string>();

    /// <summary>What to return for a key with no translation. Defaults to <see cref="MissingTranslationBehavior.ReturnKey"/>.</summary>
    public MissingTranslationBehavior MissingTranslation { get; set; } = MissingTranslationBehavior.ReturnKey;

    /// <summary>How far a culture change propagates. Defaults to <see cref="CultureSyncMode.Full"/>.</summary>
    public CultureSyncMode CultureSync { get; set; } = CultureSyncMode.Full;

    /// <summary>Adds JSON files to load.</summary>
    /// <param name="files">Paths inside the app package (build action <c>MauiAsset</c>).</param>
    /// <returns>The same options instance, for chaining.</returns>
    public LocalizationOptions UseFiles(params string[] files)
    {
        if (files != null)
        {
            foreach (var file in files)
            {
                if (!string.IsNullOrWhiteSpace(file))
                    Files.Add(file);
            }
        }

        return this;
    }

    /// <summary>Sets the startup culture.</summary>
    /// <param name="cultureName">A culture name such as <c>en-US</c>.</param>
    /// <returns>The same options instance, for chaining.</returns>
    public LocalizationOptions UseDefaultCulture(string cultureName)
        => UseDefaultCulture(CultureResolver.Parse(cultureName, nameof(cultureName)));

    /// <inheritdoc cref="UseDefaultCulture(string)"/>
    /// <param name="culture">The culture to start in.</param>
    public LocalizationOptions UseDefaultCulture(CultureInfo culture)
    {
        DefaultCulture = culture ?? throw new ArgumentNullException(nameof(culture));
        return this;
    }

    /// <summary>Sets the culture used when the current culture's parent chain yields nothing.</summary>
    /// <param name="cultureName">A culture name such as <c>en-US</c>.</param>
    /// <returns>The same options instance, for chaining.</returns>
    public LocalizationOptions UseFallbackCulture(string cultureName)
        => UseFallbackCulture(CultureResolver.Parse(cultureName, nameof(cultureName)));

    /// <inheritdoc cref="UseFallbackCulture(string)"/>
    /// <param name="culture">The fallback culture.</param>
    public LocalizationOptions UseFallbackCulture(CultureInfo culture)
    {
        FallbackCulture = culture ?? throw new ArgumentNullException(nameof(culture));
        return this;
    }

    /// <summary>Sets the missing-key policy.</summary>
    /// <param name="behavior">The behaviour to apply.</param>
    /// <returns>The same options instance, for chaining.</returns>
    public LocalizationOptions OnMissingTranslation(MissingTranslationBehavior behavior)
    {
        MissingTranslation = behavior;
        return this;
    }

    /// <summary>Sets how far a culture change propagates.</summary>
    /// <param name="mode">The propagation mode.</param>
    /// <returns>The same options instance, for chaining.</returns>
    public LocalizationOptions SyncCulture(CultureSyncMode mode)
    {
        CultureSync = mode;
        return this;
    }

    /// <summary>Applies the culture-independent settings to a translator.</summary>
    /// <param name="translator">The translator to configure.</param>
    internal void ApplyTo(Localization.BaseTranslator translator)
    {
        translator.MissingTranslation = MissingTranslation;
        translator.CultureSync = CultureSync;
        translator.FallbackCulture = FallbackCulture;
    }
}
