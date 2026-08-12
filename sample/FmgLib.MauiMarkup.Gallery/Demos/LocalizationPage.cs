using System.Globalization;
using FmgLib.MauiMarkup.Gallery.Controls;
using FmgLib.MauiMarkup.Gallery.Models;
using Microsoft.Maui.Controls.Shapes;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Live language switching: translations as bindings, formatted translations, and right-to-left.
/// </summary>
public partial class LocalizationPage : DemoPage
{
    private readonly DemoViewModel viewModel = new();

    private HorizontalStackLayout languageRow = null!;

    private static readonly (string Culture, string Key, string Flag)[] Languages =
    [
        ("en-US", "Language_English", "🇬🇧"),
        ("tr-TR", "Language_Turkish", "🇹🇷"),
        ("ar-SA", "Language_Arabic", "🇸🇦")
    ];

    public LocalizationPage()
    {
        BindingContext = viewModel;

        this.InitializeHotReload();
    }

    protected override string DemoTitle => "Localization";

    protected override string DemoSummary =>
        "Translate binds a property to a key instead of a value, so changing the culture re-reads every bound string in place — no page reload, no navigation.";

    protected override IView[] BuildSections() =>
    [
        Switcher(),
        LiveText(),
        Formatted(),
        Rtl(),
        Fallbacks()
    ];

    private IView Switcher()
    {
        languageRow = new HorizontalStackLayout().Spacing(Ui.GapSm);
        RenderLanguages();

        return Demo.Section(
            "Pick a language",
            "Everything below is bound, not assigned. Nothing on this page is rebuilt when you switch — only the highlight of the selected chip, which is drawn state rather than a binding.",
            new Border().Stage().Content(languageRow),
            Demo.Code("""
                builder.UseMauiMarkupLocalization(o => o
                    .UseFiles("Localization.json")
                    .UseDefaultCulture("en-US")
                    .UseFallbackCulture("en-US"));

                Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));
                """));
    }

    private void RenderLanguages()
    {
        languageRow.Children.Clear();

        foreach (var (culture, key, flag) in Languages)
        {
            var isSelected = Translator.Instance.CurrentCulture.Name == culture;
            var captured = culture;

            var text = new Label()
                .Text(e => e.Translate(key))
                .FontSize(13)
                .FontAttributes(isSelected ? Bold : None)
                .CenterVertical();

            var chip = new Border()
                .StrokeThickness(isSelected ? 0 : 1)
                .Stroke(e => e
                    .OnLight(new SolidColorBrush(AppColors.BorderLight))
                    .OnDark(new SolidColorBrush(AppColors.BorderDark)))
                .StrokeShape(new RoundRectangle().CornerRadius(999))
                .Padding(14, 8)
                .Content(
                    new HorizontalStackLayout()
                    .Spacing(Ui.GapSm)
                    .Children(new Label().Text(flag).FontSize(15).CenterVertical(), text)
                )
                .GestureRecognizers(
                    new TapGestureRecognizer().OnTapped((_, _) =>
                    {
                        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo(captured));
                        RenderLanguages();
                    })
                );

            if (isSelected)
            {
                chip.BackgroundColor(AppColors.Accent);
                text.TextColor(Colors.White);
            }
            else
            {
                chip.BackgroundColor(e => e.OnLight(AppColors.SurfaceLight).OnDark(AppColors.SurfaceDark));
                text.TextColor(e => e.OnLight(AppColors.MutedLight).OnDark(AppColors.MutedDark));
            }

            languageRow.Children.Add(chip);
        }
    }

    private static IView LiveText()
        => Demo.Section(
            "Translate — a binding, not a lookup",
            "Translate works on any string property, not just Text: placeholders, titles and semantic descriptions all take it.",
            Demo.Stage(
                new Label()
                    .Text(e => e.Translate("Greeting"))
                    .FontSize(22)
                    .FontAttributes(Bold)
                    .TextColor(AppColors.Accent),

                new Label().Text(e => e.Translate("Tagline")).Muted(),

                new Entry().Placeholder(e => e.Translate("Language"))
            ),
            Demo.Code("""
                new Label().Text(e => e.Translate("Greeting"))
                new Entry().Placeholder(e => e.Translate("Language"))

                // Not live — a snapshot, for alerts and logs:
                await DisplayAlert("", "Greeting".ToTranslate(), "OK");
                """));

    private static IView Formatted()
        => Demo.Section(
            "TranslateFormat — translations with values",
            "A translated sentence usually carries a runtime value. TranslateFormat binds the translation AND the arguments, so it reacts to a language change and to a value change alike.",
            Demo.Stage(
                new Entry()
                    .Placeholder("Your name")
                    .Text(e => e.Path(nameof(DemoViewModel.FirstName)).BindingMode(BindingMode.TwoWay)),

                new Label()
                    .Text(e => e.TranslateFormat("WelcomeUser", nameof(DemoViewModel.FirstName)))
                    .FontAttributes(Bold),

                new Label()
                    .Text(e => e.TranslateFormat("CartSummary", nameof(DemoViewModel.Affordable), nameof(DemoViewModel.Budget)))
                    .Muted(),

                new Label()
                    .Text(e => e.TranslateFormat("Today", nameof(DemoViewModel.Today)))
                    .Muted()
            ),
            Demo.Code("""
                new Label().Text(e => e.TranslateFormat("WelcomeUser", nameof(vm.UserName)))
                new Label().Text(e => e.TranslateFormat("CartSummary", nameof(vm.Count), nameof(vm.Total)))
                """),
            Demo.Note("Placeholders are formatted with the SELECTED culture, so {1:C} and {0:D} follow the language — the date above switches calendar in Arabic."));

    private static IView Rtl()
        => Demo.Section(
            "Right to left",
            "Translating an Arabic UI without mirroring it leaves the layout wrong. Bind FlowDirection to the culture once and the whole subtree flips.",
            new Border()
            .Stage()
            .FlowDirection(e => e.FromCulture())
            .Content(
                new VerticalStackLayout()
                .Spacing(Ui.GapSm)
                .Children(
                    new HorizontalStackLayout()
                    .Spacing(Ui.GapSm)
                    .Children(
                        new Label().Text("🧭").FontSize(20),
                        new Label().Text(e => e.Translate("Greeting")).FontAttributes(Bold).CenterVertical()
                    ),

                    // The argument is not a view-model property here but the translator's own
                    // IsRightToLeft, so the line updates from the same notification that re-reads
                    // every other translation on the page.
                    new Label()
                        .Text(e => e
                            .Path(nameof(FmgLib.MauiMarkup.Localization.BaseTranslator.IsRightToLeft))
                            .Source(Translator.Instance)
                            .Convert((bool rightToLeft) => string.Format(
                                "Direction".ToTranslate(),
                                (rightToLeft ? "Direction_RightToLeft" : "Direction_LeftToRight").ToTranslate())))
                        .Muted()
                )
            ),
            Demo.Code("""
                this.FlowDirection(e => e.FromCulture())      // whole page

                new Border().FlowDirection(e => e.FromCulture())   // or one subtree
                """),
            Demo.Note("Translator.Instance.IsRightToLeft and .FlowDirection are available in code too."));

    private static IView Fallbacks()
        => Demo.Section(
            "Fallback and missing keys",
            "A lookup walks the culture chain — tr-TR → tr → the configured fallback — so a file written with neutral keys still resolves on a regional device. A key with no translation returns the key by default; Marker or Throw make gaps impossible to miss.",
            Demo.Stage(
                new Label().Text(e => e.Translate("MissingKeyDemo")).Muted(),
                new Label().Text(e => e.Translate("ThisKeyDoesNotExist")).Mono().TextColor(AppColors.Warning)
            ),
            Demo.Code("""
                builder.UseMauiMarkupLocalization(o => o
                    .UseFiles("Localization.json")
                    .UseFallbackCulture("en-US")
                    .OnMissingTranslation(MissingTranslationBehavior.Marker));   // renders ⟦Key⟧
                """));
}
