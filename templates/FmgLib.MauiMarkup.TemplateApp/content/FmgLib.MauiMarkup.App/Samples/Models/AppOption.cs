namespace FmgLib.MauiMarkup.App.Samples.Models;

/// <summary>
/// One selectable language in the settings sample.
/// </summary>
/// <param name="CultureName">Culture passed to <c>Translator.Instance.ChangeCulture</c>, e.g. <c>tr-TR</c>.</param>
/// <param name="TranslationKey">Localization key for the label, so the list itself is translated too.</param>
/// <param name="Flag">Emoji shown next to the label.</param>
public record LanguageOption(string CultureName, string TranslationKey, string Flag);

/// <summary>
/// One selectable theme in the settings sample.
/// </summary>
/// <param name="Theme">Value assigned to <c>Application.Current.UserAppTheme</c>.</param>
/// <param name="TranslationKey">Localization key for the label.</param>
/// <param name="Icon">Emoji shown next to the label.</param>
public record ThemeOption(AppTheme Theme, string TranslationKey, string Icon);
