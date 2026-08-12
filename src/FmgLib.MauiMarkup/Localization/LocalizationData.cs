#nullable enable

namespace FmgLib.MauiMarkup;

/// <summary>
/// The loaded JSON translations, keyed <c>key → {culture → text}</c>.
/// </summary>
public static class LocalizationData
{
    private static Dictionary<string, Dictionary<string, string>>? data;

    /// <summary>
    /// The loaded translations. Assigning refreshes <see cref="Translator"/>: its per-culture index is
    /// dropped and every bound translation is re-read, so language data that arrives after the first
    /// page is on screen (a later load, a downloaded pack) appears immediately instead of leaving raw
    /// keys behind.
    /// </summary>
    public static Dictionary<string, Dictionary<string, string>>? Data
    {
        get => data;
        set
        {
            data = value;
            Translator.Instance.Refresh();
        }
    }
}
