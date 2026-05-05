using System.ComponentModel;
using System.Globalization;

namespace FmgLib.MauiMarkup.Localization;

public abstract class BaseTranslator : INotifyPropertyChanged
{
    public CultureInfo CurrentCulture { get; private set; } = CultureInfo.CurrentCulture;

    /// <summary>
    /// Executes the <c>ChangeCulture</c> operation.
    /// </summary>
    /// <param name="culture">The value used for <paramref name="culture"/>.</param>
    public void ChangeCulture(CultureInfo culture)
    {
        CurrentCulture = culture;
        OnPropertyChanged();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Executes the <c>OnPropertyChanged</c> operation.
    /// </summary>
    public void OnPropertyChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
}
