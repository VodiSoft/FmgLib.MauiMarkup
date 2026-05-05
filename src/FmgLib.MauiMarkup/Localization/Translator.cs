using System.ComponentModel;
using System.Globalization;
using FmgLib.MauiMarkup.Localization;

namespace FmgLib.MauiMarkup;

public class Translator : BaseTranslator, INotifyPropertyChanged
{
    public static Translator Instance { get; set; } = new Translator();

    public string this[string key]
    {
        get
        {
            return LocalizationData.Data.GetTranslation(key, CurrentCulture.Name);
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
        return LocalizationData.Data.GetTranslation(key, culture.Name);
    }
}

