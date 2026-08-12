using FmgLib.MauiMarkup.Gallery.Controls;
using FmgLib.MauiMarkup.Gallery.Models;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Several sources feeding one property — declared by calling Path more than once.
/// </summary>
public partial class MultiBindingPage : DemoPage
{
    private readonly DemoViewModel viewModel = new();

    public MultiBindingPage()
    {
        BindingContext = viewModel;

        this.InitializeHotReload();
    }

    protected override string DemoTitle => "MultiBinding";

    protected override string DemoSummary =>
        "Call Path twice and the builder produces a MultiBinding instead of a Binding. A closing MultiConvert combines the values — with the types you declared.";

    protected override IView[] BuildSections() =>
    [
        Combining(),
        Aggregating(),
        MixedSources()
    ];

    private static IView Combining()
        => Demo.Section(
            "Two paths, one label",
            "The MultiConvert delegate's parameters line up with the sub-bindings in declaration order, and they are strongly typed — no object[] casting.",
            Demo.Stage(
                new Entry().Placeholder("First name").Text(e => e.Path(nameof(DemoViewModel.FirstName)).BindingMode(BindingMode.TwoWay)),
                new Entry().Placeholder("Last name").Text(e => e.Path(nameof(DemoViewModel.LastName)).BindingMode(BindingMode.TwoWay)),

                new Label()
                    .Text(e => e
                        .Path(nameof(DemoViewModel.FirstName))
                        .Path(nameof(DemoViewModel.LastName))
                        .MultiConvert((string first, string last) => $"{last?.ToUpperInvariant()}, {first}"))
                    .FontAttributes(Bold)
                    .TextColor(AppColors.Accent),

                new Label()
                    .Text(e => e
                        .Path(nameof(DemoViewModel.FirstName))
                        .Path(nameof(DemoViewModel.LastName))
                        .MultiStringFormat("Shortcut: {0} {1} — MultiStringFormat needs no converter at all"))
                    .Muted()
                    .FontSize(12)
            ),
            Demo.Code("""
                new Label().Text(e => e
                    .Path("FirstName")
                    .Path("LastName")
                    .MultiConvert((string first, string last) => $"{last?.ToUpperInvariant()}, {first}"))
                """));

    private static IView Aggregating()
        => Demo.Section(
            "Aggregating booleans",
            "The classic 'enable the button when every box is ticked' rule, expressed where it belongs — on the button — instead of in a computed view-model property.",
            Demo.Stage(
                new HorizontalStackLayout()
                .Spacing(Ui.GapSm)
                .Children(
                    new CheckBox().IsChecked(e => e.Path(nameof(DemoViewModel.AcceptedTerms)).BindingMode(BindingMode.TwoWay)),
                    new Label().Text("I accept the terms").CenterVertical()
                ),

                new HorizontalStackLayout()
                .Spacing(Ui.GapSm)
                .Children(
                    new CheckBox().IsChecked(e => e.Path(nameof(DemoViewModel.ConfirmedEmail)).BindingMode(BindingMode.TwoWay)),
                    new Label().Text("I confirmed my e-mail").CenterVertical()
                ),

                new Button()
                    .Text("Submit")
                    .CenterHorizontal()
                    .IsEnabled(e => e
                        .Path(nameof(DemoViewModel.AcceptedTerms))
                        .Path(nameof(DemoViewModel.ConfirmedEmail))
                        .MultiConvert((bool terms, bool email) => terms && email))
            ),
            Demo.Code("""
                new Button()
                    .Text("Submit")
                    .IsEnabled(e => e
                        .Path("AcceptedTerms")
                        .Path("ConfirmedEmail")
                        .MultiConvert((bool terms, bool email) => terms && email))
                """));

    private static IView MixedSources()
        => Demo.Section(
            "Mixing sources and types",
            "Sub-bindings are independent: each may have its own Source, its own Convert, and its own type. Only the closing MultiConvert has to agree with all of them.",
            Demo.Stage(
                new Slider().Assign(out var quantity).Minimum(1).Maximum(12).Value(3),

                new Label()
                    .Text(e => e
                        .Path(nameof(Slider.Value)).Source(quantity).Convert((double v) => (int)Math.Round(v))
                        .Path(nameof(DemoViewModel.Budget))
                        .MultiConvert((int units, double budget) =>
                            $"{units} × items — {(units * 9.5 <= budget ? "within" : "over")} a {budget:C0} budget"))
                    .FontAttributes(Bold),

                new Label()
                    .Text(e => e
                        .Path(nameof(Slider.Value)).Source(quantity).Convert((double v) => (int)Math.Round(v))
                        .Path(nameof(DemoViewModel.FirstName))
                        .MultiConvert((int units, string name) => $"{name} is ordering {units} unit{(units == 1 ? "" : "s")}."))
                    .Muted()
            ),
            Demo.Code("""
                new Label().Text(e => e
                    .Path(nameof(Slider.Value)).Source(quantity).Convert((double v) => (int)Math.Round(v))
                    .Path("Budget")
                    .MultiConvert((int units, double budget) => …))
                """),
            Demo.Note("Each sub-binding's own Convert runs first; MultiConvert then receives the converted values."));
}
