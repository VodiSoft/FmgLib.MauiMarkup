using System.ComponentModel;
using System.Globalization;
using System.Resources;
using FmgLib.MauiMarkup.Localization;

namespace FmgLib.MauiMarkup;

public class TranslatorResx : BaseTranslator, INotifyPropertyChanged
{
    internal static ResourceManager ResourceManager { get; set; }

    public static TranslatorResx Instance { get; set; } = new TranslatorResx();

    public string this[string key]
    {
        get
        {
            return ResourceManager.GetString(key, CurrentCulture);
        }
    }

    /// <summary>
    /// Executes the <c>TranslateString</c> operation.
    /// </summary>
    /// <param name="key">The value used for <paramref name="key"/>.</param>
    /// <param name="culture">The value used for <paramref name="culture"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    public string TranslateString(string key, CultureInfo culture)
    {
        return ResourceManager.GetString(key, culture);
    }
}
