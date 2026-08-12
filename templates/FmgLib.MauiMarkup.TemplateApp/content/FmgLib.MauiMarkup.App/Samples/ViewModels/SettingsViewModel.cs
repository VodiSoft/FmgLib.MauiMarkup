using System.Globalization;
using System.Windows.Input;
using FmgLib.MauiMarkup.App.Samples.Models;

namespace FmgLib.MauiMarkup.App.Samples.ViewModels;

/// <summary>
/// State behind the settings sample: the selected language and theme, plus the live values the page
/// shows to prove that switching either one needs no page reload.
/// </summary>
public class SettingsViewModel : BaseViewModel
{
    public const string LanguagePreferenceKey = "app_language";
    public const string ThemePreferenceKey = "app_theme";

    public SettingsViewModel()
    {
        SelectLanguageCommand = new RelayCommand<LanguageOption>(SelectLanguage);
        SelectThemeCommand = new RelayCommand<ThemeOption>(SelectTheme);
        SelectedTheme = ReadStoredTheme();
    }

    /// <summary>Languages offered by the sample. Arabic is here so right-to-left is visible.</summary>
    public IReadOnlyList<LanguageOption> Languages { get; } =
    [
        new("en-US", "Language_English", "🇬🇧"),
        new("tr-TR", "Language_Turkish", "🇹🇷"),
        new("ar-SA", "Language_Arabic", "🇸🇦")
    ];

    public IReadOnlyList<ThemeOption> Themes { get; } =
    [
        new(AppTheme.Unspecified, "Theme_System", "🌗"),
        new(AppTheme.Light, "Theme_Light", "☀️"),
        new(AppTheme.Dark, "Theme_Dark", "🌙")
    ];

    public ICommand SelectLanguageCommand { get; }

    public ICommand SelectThemeCommand { get; }

    /// <summary>Culture currently selected, so the page can highlight the active row.</summary>
    public string SelectedCultureName => Translator.Instance.CurrentCulture.Name;

    /// <summary>The language's own name — shown through <c>TranslateFormat</c>, so it stays live.</summary>
    public string SelectedLanguageName => Translator.Instance.CurrentCulture.NativeName;

    /// <summary>Reads as a translated string rather than "LTR"/"RTL", so the demo is legible.</summary>
    public string DirectionName =>
        Translator.Instance[Translator.Instance.IsRightToLeft ? "Direction_RightToLeft" : "Direction_LeftToRight"];

    /// <summary>Formatted by <c>TranslateFormat</c> with the selected culture — the date follows the language.</summary>
    public DateTime Today => DateTime.Now;

    /// <summary>Same idea for currency: <c>{0:C}</c> renders $ / ₺ / ر.س depending on the language.</summary>
    public decimal Amount => 1234.5m;

    private AppTheme selectedTheme;

    public AppTheme SelectedTheme
    {
        get => selectedTheme;
        private set => SetProperty(ref selectedTheme, value);
    }

    /// <summary>
    /// Applies the language stored on a previous run. Called once at startup, before the first page is
    /// built, so the app opens in the user's language instead of flashing the device one.
    /// </summary>
    /// <returns>The culture name to start in, or <see langword="null"/> to keep the device culture.</returns>
    public static string? ReadStoredLanguage()
    {
        var stored = Preferences.Get(LanguagePreferenceKey, string.Empty);
        return string.IsNullOrWhiteSpace(stored) ? null : stored;
    }

    /// <summary>Applies the theme stored on a previous run.</summary>
    public static void ApplyStoredTheme()
    {
        if (Application.Current is { } application)
            application.UserAppTheme = ReadStoredTheme();
    }

    private void SelectLanguage(LanguageOption? option)
    {
        if (option is null)
            return;

        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo(option.CultureName));
        Preferences.Set(LanguagePreferenceKey, option.CultureName);

        // Every `Translate` binding refreshes itself off the translator. These properties are the
        // view model's own contribution to that picture (the selected row, the values fed into
        // TranslateFormat), so they need their own notification — null means "all of them".
        OnPropertyChanged(null);
    }

    private void SelectTheme(ThemeOption? option)
    {
        if (option is null || Application.Current is not { } application)
            return;

        application.UserAppTheme = option.Theme;
        Preferences.Set(ThemePreferenceKey, option.Theme.ToString());
        SelectedTheme = option.Theme;
    }

    private static AppTheme ReadStoredTheme()
        => Enum.TryParse<AppTheme>(Preferences.Get(ThemePreferenceKey, nameof(AppTheme.Unspecified)), out var theme)
            ? theme
            : AppTheme.Unspecified;
}
