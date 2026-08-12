#nullable enable

using System.Globalization;
using System.Resources;

namespace FmgLib.MauiMarkup;

/// <summary>
/// <c>MauiAppBuilder</c> entry points for the localization system.
/// </summary>
public static class LocalizationAppBuilderExtension
{
    /// <summary>
    /// Registers JSON localization.
    /// </summary>
    /// <remarks>
    /// The recommended overload. Unlike the positional one it cannot confuse a file name with a culture
    /// name, and it is where the fallback culture, missing-key policy and culture-sync mode are set.
    /// <code>
    /// builder.UseMauiMarkupLocalization(o => o
    ///     .UseFiles("Common.json", "Checkout.json")
    ///     .UseDefaultCulture("en-US")
    ///     .UseFallbackCulture("en-US"));
    /// </code>
    /// Loading is synchronous and throws on a missing or malformed file, so a broken language file fails
    /// loudly at startup instead of leaving the app showing raw keys.
    /// </remarks>
    /// <param name="builder">The app builder.</param>
    /// <param name="configure">Configures files, cultures and policies.</param>
    /// <returns>The same builder, for chaining.</returns>
    public static MauiAppBuilder UseMauiMarkupLocalization(this MauiAppBuilder builder, Action<LocalizationOptions> configure)
    {
        if (configure is null)
            throw new ArgumentNullException(nameof(configure));

        var options = new LocalizationOptions();
        configure(options);

        options.ApplyTo(Translator.Instance);

        JsonLocalizationExtensions.LoadLocalizationFiles(options.Files.ToArray());

        if (options.DefaultCulture != null)
            Translator.Instance.ChangeCulture(options.DefaultCulture);

        return builder;
    }

    /// <summary>
    /// Registers JSON localization with an optional startup culture and file list.
    /// </summary>
    /// <remarks>
    /// <paramref name="defaultLang"/> comes first, so <c>UseMauiMarkupLocalization("Common.json", "Checkout.json")</c>
    /// passes a FILE NAME as the culture. That used to surface as a bare
    /// <see cref="CultureNotFoundException"/>; it is now rejected with a message that names the fix.
    /// Prefer <see cref="UseMauiMarkupLocalization(MauiAppBuilder, Action{LocalizationOptions})"/>.
    /// </remarks>
    /// <param name="builder">The app builder.</param>
    /// <param name="defaultLang">Startup culture name, or <see langword="null"/> to keep the device culture.</param>
    /// <param name="filePaths">JSON files to load; empty loads <c>Localization.json</c>.</param>
    /// <returns>The same builder, for chaining.</returns>
    public static MauiAppBuilder UseMauiMarkupLocalization(this MauiAppBuilder builder, string? defaultLang = null, params string[] filePaths)
    {
        var culture = string.IsNullOrEmpty(defaultLang)
            ? null
            : CultureResolver.Parse(defaultLang!, nameof(defaultLang));

        JsonLocalizationExtensions.LoadLocalizationFiles(filePaths);

        if (culture != null)
            Translator.Instance.ChangeCulture(culture);

        return builder;
    }

    /// <summary>
    /// Registers RESX localization.
    /// </summary>
    /// <param name="builder">The app builder.</param>
    /// <param name="resourceManager">The generated resource class's <c>ResourceManager</c>.</param>
    /// <param name="configure">Configures cultures and policies. Files on the options are ignored here.</param>
    /// <returns>The same builder, for chaining.</returns>
    public static MauiAppBuilder UseMauiMarkupLocalizationWithResx(this MauiAppBuilder builder, ResourceManager resourceManager, Action<LocalizationOptions> configure)
    {
        if (resourceManager is null)
            throw new ArgumentNullException(nameof(resourceManager));

        if (configure is null)
            throw new ArgumentNullException(nameof(configure));

        var options = new LocalizationOptions();
        configure(options);

        TranslatorResx.ResourceManager = resourceManager;
        options.ApplyTo(TranslatorResx.Instance);

        if (options.DefaultCulture != null)
            TranslatorResx.Instance.ChangeCulture(options.DefaultCulture);

        return builder;
    }

    /// <summary>
    /// Registers RESX localization with an optional startup culture.
    /// </summary>
    /// <param name="builder">The app builder.</param>
    /// <param name="resourceManager">The generated resource class's <c>ResourceManager</c>.</param>
    /// <param name="defaultLang">Startup culture name, or <see langword="null"/> to keep the device culture.</param>
    /// <returns>The same builder, for chaining.</returns>
    public static MauiAppBuilder UseMauiMarkupLocalizationWithResx(this MauiAppBuilder builder, ResourceManager resourceManager, string? defaultLang = null)
    {
        if (resourceManager is null)
            throw new ArgumentNullException(nameof(resourceManager));

        TranslatorResx.ResourceManager = resourceManager;

        if (!string.IsNullOrEmpty(defaultLang))
            TranslatorResx.Instance.ChangeCulture(CultureResolver.Parse(defaultLang!, nameof(defaultLang)));

        return builder;
    }
}
