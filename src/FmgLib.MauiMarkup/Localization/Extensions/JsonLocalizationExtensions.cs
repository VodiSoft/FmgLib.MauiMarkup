#nullable enable

using System.Globalization;
using System.Text.Json;

namespace FmgLib.MauiMarkup;

/// <summary>
/// Loading and lookup helpers for the JSON localization files.
/// </summary>
public static class JsonLocalizationExtensions
{
    private const string DefaultFileName = "Localization.json";

    private const string FormatHint =
        "The expected format is:\n" +
        "{\n" +
        "  \"wordKey\": {\n" +
        "    \"en-US\": \"1st language translation.\",\n" +
        "    \"tr-TR\": \"2nd language translation.\"\n" +
        "  }\n" +
        "}";

    /// <summary>
    /// Replaces the loaded translations with the contents of a JSON string.
    /// </summary>
    /// <param name="jsonContent">The JSON document.</param>
    public static void LoadLocalizationData(this string jsonContent)
    {
        LocalizationData.Data = Parse(jsonContent, source: "the supplied JSON string");
    }

    /// <summary>
    /// Loads and merges JSON language files from the app package, synchronously.
    /// </summary>
    /// <remarks>
    /// Deliberately synchronous: it is called from <c>MauiProgram</c> before the first page exists, and
    /// translations have to be in place by then or every label renders its raw key. The read is pushed
    /// onto the thread pool rather than blocked on directly, so waiting for it cannot deadlock against
    /// the startup synchronization context. Failures are thrown, not swallowed.
    /// </remarks>
    /// <param name="filePaths">Files to load; empty loads <c>Localization.json</c>.</param>
    public static void LoadLocalizationFiles(params string[] filePaths)
        => Task.Run(() => LoadLocalizationFilesAsync(filePaths)).GetAwaiter().GetResult();

    /// <summary>
    /// Loads and merges JSON language files from the app package.
    /// </summary>
    /// <param name="filePaths">Files to load; empty loads <c>Localization.json</c>.</param>
    /// <returns>A task that completes once the translations are published.</returns>
    public static async Task LoadLocalizationFilesAsync(params string[] filePaths)
    {
        var paths = filePaths is null || filePaths.Length == 0
            ? new[] { DefaultFileName }
            : filePaths;

        var merged = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);

        foreach (var filePath in paths)
        {
            var json = await ReadPackageFileAsync(filePath).ConfigureAwait(false);
            var data = Parse(json, source: filePath);

            foreach (var entry in data)
            {
                if (merged.TryGetValue(entry.Key, out var translations))
                {
                    // Later files win per language, so a feature file can override a shared default
                    // without having to repeat every other language of the same key.
                    foreach (var translation in entry.Value)
                        translations[translation.Key] = translation.Value;
                }
                else
                {
                    merged[entry.Key] = new Dictionary<string, string>(entry.Value, StringComparer.OrdinalIgnoreCase);
                }
            }
        }

        LocalizationData.Data = merged;
    }

    /// <inheritdoc cref="LoadLocalizationFilesAsync(string[])"/>
    [Obsolete("Renamed to LoadLocalizationFilesAsync. This overload forwards to it and will be removed in a future major version.")]
    public static Task LoadLocalizationDataAsync(params string[] filePaths)
        => LoadLocalizationFilesAsync(filePaths);

    /// <summary>
    /// Resolves a key against loaded translation data, walking the culture fallback chain
    /// (<c>tr-TR</c> → <c>tr</c>) rather than requiring an exact culture match.
    /// </summary>
    /// <param name="data">The loaded translations.</param>
    /// <param name="key">The translation key.</param>
    /// <param name="languageCode">The culture name to resolve in.</param>
    /// <returns>The translation, or <paramref name="key"/> when there is none.</returns>
    public static string GetTranslation(this Dictionary<string, Dictionary<string, string>>? data, string key, string languageCode)
    {
        if (data == null || key == null || !data.TryGetValue(key, out var translations) || translations == null)
            return key!;

        if (!string.IsNullOrEmpty(languageCode))
        {
            if (translations.TryGetValue(languageCode, out var exact))
                return exact;

            CultureInfo? culture = null;
            try
            {
                culture = CultureInfo.GetCultureInfo(languageCode);
            }
            catch (CultureNotFoundException)
            {
                // A non-standard language key (the docs allow free-form ones): exact match was the only
                // option and it already failed.
            }

            if (culture != null)
            {
                foreach (var cultureName in CultureResolver.BuildChain(culture, fallback: null))
                {
                    if (translations.TryGetValue(cultureName, out var translation))
                        return translation;
                }
            }
        }

        return key;
    }

    private static async Task<string> ReadPackageFileAsync(string filePath)
    {
        try
        {
            using var stream = await FileSystem.Current.OpenAppPackageFileAsync(filePath).ConfigureAwait(false);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            throw new FileLoadException(
                $"The language file '{filePath}' could not be read. Make sure it exists and that its " +
                $"build action is MauiAsset (<MauiAsset Include=\"{filePath}\" /> in the .csproj).\n{exception.Message}",
                exception);
        }
    }

    private static Dictionary<string, Dictionary<string, string>> Parse(string json, string source)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new FileLoadException($"{source} is empty.\n\n{FormatHint}");

        Dictionary<string, Dictionary<string, string>>? data;

        try
        {
            data = JsonSerializer.Deserialize(json, LocalizationJsonContext.Default.LocalizationMap);
        }
        catch (JsonException exception)
        {
            throw new FileLoadException(
                $"There is an error in your language file ({source}).\n{exception.Message}\n\n{FormatHint}",
                exception);
        }

        if (data == null)
            throw new FileLoadException($"{source} contains no translations.\n\n{FormatHint}");

        // Culture names are case-insensitive by convention ("en-us" and "en-US" are the same culture),
        // and hand-edited files mix the two constantly.
        var normalized = new Dictionary<string, Dictionary<string, string>>(data.Count, StringComparer.Ordinal);

        foreach (var entry in data)
        {
            normalized[entry.Key] = entry.Value == null
                ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(entry.Value, StringComparer.OrdinalIgnoreCase);
        }

        return normalized;
    }
}
