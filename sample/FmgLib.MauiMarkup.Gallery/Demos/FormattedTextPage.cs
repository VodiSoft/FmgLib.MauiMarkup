using FmgLib.MauiMarkup.Gallery.Controls;
using FmgLib.MauiMarkup.Gallery.Models;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Mixed typography — and tappable segments — inside a single label.
/// </summary>
public partial class FormattedTextPage : DemoPage
{
    private readonly DemoViewModel viewModel = new();

    public FormattedTextPage()
    {
        BindingContext = viewModel;

        this.InitializeHotReload();
    }

    protected override string DemoTitle => "Formatted Text";

    protected override string DemoSummary =>
        "FormattedText with Spans mixes fonts, colours and sizes inside one Label — and because a Span is a GestureElement, individual segments can be tapped.";

    protected override IView[] BuildSections() =>
    [
        Mixing(),
        Bound(),
        Links(),
        Typography()
    ];

    private static IView Mixing()
        => Demo.Section(
            "Spans",
            "FormattedString.Spans takes the segments as params, and each Span supports the full fluent property set.",
            Demo.Stage(
                new Label()
                    .FormattedText(
                        new FormattedString()
                        .Spans(
                            new Span().Text("Total: "),
                            new Span()
                                .Text("$49.90")
                                .FontAttributes(Bold)
                                .FontSize(20)
                                .TextColor(AppColors.Success),
                            new Span()
                                .Text("  (incl. VAT)")
                                .FontSize(11)
                                .TextColor(AppColors.MutedLight)
                        )
                    ),

                new Label()
                    .FormattedText(
                        new FormattedString()
                        .Spans(
                            new Span().Text("Was ").FontSize(13),
                            new Span()
                                .Text("$79.00")
                                .FontSize(13)
                                .TextDecorations(TextDecorations.Strikethrough)
                                .TextColor(AppColors.MutedLight),
                            new Span().Text("  →  ").FontSize(13),
                            new Span()
                                .Text("$49.90")
                                .FontSize(13)
                                .FontAttributes(Bold)
                                .TextColor(AppColors.Danger)
                        )
                    )
            ),
            Demo.Code("""
                new Label().FormattedText(
                    new FormattedString().Spans(
                        new Span().Text("Total: "),
                        new Span().Text("$49.90").FontAttributes(Bold).TextColor(Colors.SeaGreen),
                        new Span().Text("  (incl. VAT)").FontSize(11).TextColor(Colors.Gray)))
                """));

    private static IView Bound()
        => Demo.Section(
            "Spans are bindable",
            "A Span is a BindableObject, so the property builder works inside it exactly as on a control. Type below and only the middle segment changes.",
            Demo.Stage(
                new Entry()
                    .Placeholder("Your name")
                    .Text(e => e.Path(nameof(DemoViewModel.FirstName)).BindingMode(BindingMode.TwoWay)),

                new Label()
                    .FontSize(17)
                    .FormattedText(
                        new FormattedString()
                        .Spans(
                            new Span().Text("Welcome back, "),
                            new Span()
                                .Text(e => e.Path(nameof(DemoViewModel.FirstName)))
                                .FontAttributes(Bold)
                                .TextColor(AppColors.Accent),
                            new Span().Text("!")
                        )
                    )
            ),
            Demo.Code("""
                new Span()
                    .Text(e => e.Path("FirstName"))
                    .FontAttributes(Bold)
                """));

    private static IView Links()
    {
        var status = new Label().Text("Nothing tapped yet.").Muted().FontSize(12);

        return Demo.Section(
            "Tappable spans",
            "Span derives from GestureElement, so gesture recognizers attach to individual segments — the standard inline-link pattern.",
            Demo.Stage(
                new Label()
                    .FormattedText(
                        new FormattedString()
                        .Spans(
                            new Span().Text("I agree to the "),
                            new Span()
                                .Text("terms of service")
                                .TextColor(AppColors.Accent)
                                .TextDecorations(TextDecorations.Underline)
                                .GestureRecognizers(
                                    new TapGestureRecognizer().OnTapped((_, _) => status.Text = "Tapped: terms of service")),
                            new Span().Text(" and the "),
                            new Span()
                                .Text("privacy policy")
                                .TextColor(AppColors.Accent)
                                .TextDecorations(TextDecorations.Underline)
                                .GestureRecognizers(
                                    new TapGestureRecognizer().OnTapped((_, _) => status.Text = "Tapped: privacy policy")),
                            new Span().Text(".")
                        )
                    ),
                status
            ),
            Demo.Code("""
                new Span()
                    .Text("terms of service")
                    .TextColor(AppColors.Accent)
                    .TextDecorations(TextDecorations.Underline)
                    .GestureRecognizers(
                        new TapGestureRecognizer().OnTapped((s, e) => OpenTerms()))
                """));
    }

    private static IView Typography()
        => Demo.Section(
            "Typography properties",
            "Character spacing, line height and text transform are ordinary fluent properties — on labels and on spans alike.",
            Demo.Stage(
                new Label().Text("CharacterSpacing(4)").CharacterSpacing(4).FontSize(15),
                new Label().Text("TextTransform.Uppercase").TextTransform(TextTransform.Uppercase).FontSize(15),
                new Label()
                    .Text("LineHeight(1.8) — noticeable once the text wraps across more than one line, which this sentence is long enough to do on most screens.")
                    .LineHeight(1.8)
                    .FontSize(13),
                new Label()
                    .Text("MaxLines(2) with TailTruncation keeps a card's height predictable no matter how long the source text turns out to be in practice.")
                    .MaxLines(2)
                    .LineBreakMode(LineBreakMode.TailTruncation)
                    .FontSize(13)
                    .Muted()
            ));
}
