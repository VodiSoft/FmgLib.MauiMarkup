using FmgLib.MauiMarkup.App.Samples.Models;
using FmgLib.MauiMarkup.App.Samples.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace FmgLib.MauiMarkup.App.Samples.Pages;

/// <summary>
/// Language and theme switching, live.
///
/// Everything here is a binding, not a value: picking a language re-reads every translated string in
/// place, and the page mirrors itself for Arabic because <c>FlowDirection</c> is bound to the culture.
/// Nothing is reloaded, re-navigated or rebuilt.
/// </summary>
public partial class SettingsPage : ContentPage, IFmgLibHotReload
{
    private readonly SettingsViewModel _viewModel;

    public SettingsPage()
    {
        _viewModel = new SettingsViewModel();
        BindingContext = _viewModel;

        this.InitializeHotReload();
    }

    public void Build()
    {
        this
        // One line is all the right-to-left support this page needs: pick Arabic and the whole
        // layout mirrors, because the culture drives it.
        .FlowDirection(e => e.FromCulture())
        .Title(e => e.Translate("Settings_Title"))
        .Content(
            new ScrollView()
            .Content(
                new VerticalStackLayout()
                .Padding(20)
                .Spacing(24)
                .Children(
                    Header(),
                    LanguageSection(),
                    ThemeSection(),
                    LiveValuesSection()
                )
            )
        );
    }

    private VerticalStackLayout Header()
    {
        return new VerticalStackLayout()
        .Spacing(6)
        .Children(
            new Label()
            .Text(e => e.Translate("Settings_Title"))
            .FontSize(28)
            .FontAttributes(Bold)
            .TextColor(e => e.OnLight(AppColors.Gray950).OnDark(White)),

            new Label()
            .Text(e => e.Translate("Settings_Subtitle"))
            .FontSize(14)
            .TextColor(e => e.OnLight(AppColors.Gray500).OnDark(AppColors.Gray200))
        );
    }

    private VerticalStackLayout LanguageSection()
    {
        var rows = new VerticalStackLayout().Spacing(10);

        foreach (var language in _viewModel.Languages)
            rows.Children.Add(LanguageRow(language));

        return Section("🌍", "Settings_Language", rows);
    }

    private Border LanguageRow(LanguageOption language)
    {
        var isSelected = _viewModel.SelectedCultureName == language.CultureName;

        return new Border()
            .Padding(14)
            .StrokeThickness(isSelected ? 2 : 1)
            .Stroke(isSelected ? AppColors.Primary : AppColors.Gray200)
            .StrokeShape(new RoundRectangle().CornerRadius(12))
            .BackgroundColor(e => e.OnLight(White).OnDark(AppColors.Gray900))
            .GestureRecognizers(
                new TapGestureRecognizer()
                .OnTapped((s, e) =>
                {
                    _viewModel.SelectLanguageCommand.Execute(language);

                    // Selection highlighting is drawn, not bound, so this one control is rebuilt.
                    // The TRANSLATIONS above are bindings and refresh on their own.
                    Build();
                })
            )
            .Content(
                new HorizontalStackLayout()
                .Spacing(12)
                .Children(
                    new Label()
                    .Text(language.Flag)
                    .FontSize(22)
                    .CenterVertical(),

                    new Label()
                    .Text(e => e.Translate(language.TranslationKey))
                    .FontSize(16)
                    .FontAttributes(isSelected ? Bold : None)
                    .TextColor(e => e.OnLight(AppColors.Gray950).OnDark(White))
                    .CenterVertical(),

                    new Label()
                    .Text(isSelected ? "✓" : string.Empty)
                    .FontSize(18)
                    .TextColor(AppColors.Primary)
                    .FontAttributes(Bold)
                    .CenterVertical()
                )
            );
    }

    private VerticalStackLayout ThemeSection()
    {
        var rows = new HorizontalStackLayout().Spacing(10);

        foreach (var theme in _viewModel.Themes)
            rows.Children.Add(ThemeChip(theme));

        return Section("🎨", "Settings_Theme", rows);
    }

    private Border ThemeChip(ThemeOption theme)
    {
        var isSelected = _viewModel.SelectedTheme == theme.Theme;

        return new Border()
            .Padding(14, 10)
            .StrokeThickness(isSelected ? 2 : 1)
            .Stroke(isSelected ? AppColors.Primary : AppColors.Gray200)
            .StrokeShape(new RoundRectangle().CornerRadius(20))
            .BackgroundColor(e => e.OnLight(White).OnDark(AppColors.Gray900))
            .GestureRecognizers(
                new TapGestureRecognizer()
                .OnTapped((s, e) =>
                {
                    _viewModel.SelectThemeCommand.Execute(theme);
                    Build();
                })
            )
            .Content(
                new HorizontalStackLayout()
                .Spacing(8)
                .Children(
                    new Label()
                    .Text(theme.Icon)
                    .FontSize(16)
                    .CenterVertical(),

                    new Label()
                    .Text(e => e.Translate(theme.TranslationKey))
                    .FontSize(14)
                    .FontAttributes(isSelected ? Bold : None)
                    .TextColor(e => e.OnLight(AppColors.Gray950).OnDark(White))
                    .CenterVertical()
                )
            );
    }

    private VerticalStackLayout LiveValuesSection()
    {
        var values = new VerticalStackLayout()
        .Spacing(8)
        .Children(
            // TranslateFormat binds the translation AND the argument, so these lines react to a
            // language change and to a value change alike.
            ValueLine(e => e.TranslateFormat("Settings_CurrentLanguage", nameof(SettingsViewModel.SelectedLanguageName))),
            ValueLine(e => e.TranslateFormat("Settings_Direction", nameof(SettingsViewModel.DirectionName))),
            ValueLine(e => e.TranslateFormat("Settings_Today", nameof(SettingsViewModel.Today))),
            ValueLine(e => e.TranslateFormat("Settings_Amount", nameof(SettingsViewModel.Amount))),

            new Label()
            .Text(e => e.Translate("Settings_FormatHint"))
            .FontSize(12)
            .TextColor(e => e.OnLight(AppColors.Gray500).OnDark(AppColors.Gray300))
            .Margin(0, 6, 0, 0),

            new Label()
            .Text(e => e.Translate("Settings_RtlHint"))
            .FontSize(12)
            .TextColor(e => e.OnLight(AppColors.Gray500).OnDark(AppColors.Gray300))
        );

        return Section("⚡", "Settings_Live", values);
    }

    private Label ValueLine(Func<PropertyContext<string>, IPropertyBuilder<string>> text)
    {
        return new Label()
            .Text(text)
            .FontSize(14)
            .TextColor(e => e.OnLight(AppColors.Gray900).OnDark(AppColors.Gray100));
    }

    private VerticalStackLayout Section(string icon, string titleKey, View content)
    {
        return new VerticalStackLayout()
        .Spacing(12)
        .Children(
            new HorizontalStackLayout()
            .Spacing(8)
            .Children(
                new Label()
                .Text(icon)
                .FontSize(18)
                .CenterVertical(),

                new Label()
                .Text(e => e.Translate(titleKey))
                .FontSize(20)
                .FontAttributes(Bold)
                .TextColor(e => e.OnLight(AppColors.Gray950).OnDark(White))
                .CenterVertical()
            ),

            content
        );
    }
}
