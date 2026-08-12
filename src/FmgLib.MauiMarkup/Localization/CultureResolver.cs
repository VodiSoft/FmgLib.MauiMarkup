#nullable enable

using System.Globalization;

namespace FmgLib.MauiMarkup;

/// <summary>
/// Culture parsing and fallback-chain construction shared by the JSON translator and the app builder
/// extensions.
/// </summary>
internal static class CultureResolver
{
    /// <summary>
    /// Parses a culture name, turning the two failure modes users actually hit into messages that say
    /// what to do.
    /// </summary>
    /// <param name="cultureName">The culture name to parse.</param>
    /// <param name="parameterName">Parameter name reported in the exception.</param>
    /// <returns>The parsed culture.</returns>
    public static CultureInfo Parse(string cultureName, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
            throw new ArgumentException("A culture name is required.", parameterName);

        // The single most common mistake: a JSON file name reaching the culture parameter, because the
        // legacy overload takes `defaultLang` BEFORE `params string[] filePaths`. Left alone this
        // surfaces as CultureNotFoundException("Common.json"), which tells the user nothing.
        if (LooksLikeFilePath(cultureName))
        {
            throw new ArgumentException(
                $"'{cultureName}' looks like a file name, not a culture name. The first argument of " +
                "UseMauiMarkupLocalization is the STARTUP CULTURE, and the file list comes after it. " +
                "Either name the argument — UseMauiMarkupLocalization(filePaths: new[] { \"" + cultureName + "\" }) — " +
                "or use the options overload: " +
                "UseMauiMarkupLocalization(o => o.UseFiles(\"" + cultureName + "\").UseDefaultCulture(\"en-US\")).",
                parameterName);
        }

        try
        {
            return CultureInfo.GetCultureInfo(cultureName);
        }
        catch (CultureNotFoundException exception)
        {
            throw new ArgumentException(
                $"'{cultureName}' is not a valid culture name. Use a standard name such as 'en-US' or 'tr-TR'.",
                parameterName,
                exception);
        }
    }

    /// <summary>
    /// Builds the ordered list of culture names to try for one lookup: the culture itself, then each
    /// parent (<c>tr-TR</c> → <c>tr</c>), then the configured fallback and its parents.
    /// </summary>
    /// <remarks>
    /// RESX gets this for free from <c>ResourceManager</c>. The JSON translator had no fallback at all,
    /// so a file written with <c>"tr"</c> keys produced nothing on a <c>tr-TR</c> device. The chain is
    /// computed once per culture change, not per lookup.
    /// </remarks>
    /// <param name="culture">The active culture.</param>
    /// <param name="fallback">Optional fallback culture consulted after <paramref name="culture"/>'s chain.</param>
    /// <returns>Culture names to probe, most specific first, without duplicates.</returns>
    public static IReadOnlyList<string> BuildChain(CultureInfo culture, CultureInfo? fallback)
    {
        var chain = new List<string>(6);

        AddChain(culture);
        AddChain(fallback);

        return chain;

        void AddChain(CultureInfo? start)
        {
            for (var current = start;
                 current != null && !string.IsNullOrEmpty(current.Name);
                 current = ReferenceEquals(current, current.Parent) ? null : current.Parent)
            {
                if (!chain.Contains(current.Name, StringComparer.OrdinalIgnoreCase))
                    chain.Add(current.Name);
            }
        }
    }

    private static bool LooksLikeFilePath(string value)
    {
        if (value.IndexOf('/') >= 0 || value.IndexOf('\\') >= 0)
            return true;

        var extension = Path.GetExtension(value);
        return !string.IsNullOrEmpty(extension) && extension.Length > 1;
    }
}
