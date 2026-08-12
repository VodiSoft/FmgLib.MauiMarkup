using FmgLib.MauiMarkup.Gallery.Controls;
using FmgLib.MauiMarkup.Gallery.Models;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// The property builder as a binding engine: paths, sources, modes, formats and inline converters.
/// </summary>
public partial class BindingPage : DemoPage
{
    private readonly DemoViewModel viewModel = new();

    public BindingPage()
    {
        BindingContext = viewModel;

        this.InitializeHotReload();
    }

    protected override string DemoTitle => "Data Binding";

    protected override string DemoSummary =>
        "Any property method accepts a lambda instead of a value. Inside it, Path/Source/BindingMode/StringFormat cover everything {Binding} does in XAML.";

    protected override IView[] BuildSections() =>
    [
        TwoWay(),
        Formatting(),
        InlineConverters(),
        Fallbacks()
    ];

    private static IView TwoWay()
        => Demo.Section(
            "Two-way binding to a view model",
            "The default source is the inherited BindingContext, so an MVVM page reads naturally. Type in the fields and the summary follows.",
            Demo.Stage(
                new Entry()
                    .Placeholder("First name")
                    .Text(e => e.Path(nameof(DemoViewModel.FirstName)).BindingMode(BindingMode.TwoWay)),

                new Entry()
                    .Placeholder("Last name")
                    .Text(e => e.Path(nameof(DemoViewModel.LastName)).BindingMode(BindingMode.TwoWay)),

                new Label()
                    .Text(e => e.Path(nameof(DemoViewModel.FullName)).StringFormat("Full name: {0}"))
                    .FontAttributes(Bold)
                    .TextColor(AppColors.Accent)
            ),
            Demo.Code("""
                new Entry().Text(e => e.Path("FirstName").BindingMode(BindingMode.TwoWay))

                new Label().Text(e => e.Path("FullName").StringFormat("Full name: {0}"))
                """));

    private static IView Formatting()
        => Demo.Section(
            "Formatting and control-to-control binding",
            "StringFormat is applied to the resolved value; Source retargets the binding away from the BindingContext — here to the slider itself.",
            Demo.Stage(
                new Slider()
                    .Assign(out var slider)
                    .Minimum(0)
                    .Maximum(360)
                    .Value(120),

                new Label()
                    .Text(e => e.Path(nameof(Slider.Value)).Source(slider).StringFormat("{0:F0}°"))
                    .Mono()
                    .TextCenterHorizontal(),

                new Border()
                    .Stage(12)
                    .HeightRequest(84)
                    .Content(
                        new Label()
                            .Text("↑")
                            .FontSize(38)
                            .TextCenter()
                            .Rotation(e => e.Path(nameof(Slider.Value)).Source(slider))
                    )
            ),
            Demo.Code("""
                new Slider().Assign(out var slider).Maximum(360),

                new Label()
                    .Text(e => e.Path(nameof(Slider.Value)).Source(slider).StringFormat("{0:F0}°"))
                    .Rotation(e => e.Path(nameof(Slider.Value)).Source(slider))
                """));

    private IView InlineConverters()
        => Demo.Section(
            "Inline converters",
            "Convert takes a plain function, so the vast majority of converters never need a class. ConvertBack does the same on the way out for two-way bindings.",
            Demo.Stage(
                new Slider()
                    .Minimum(0)
                    .Maximum(200)
                    .Value(e => e.Path(nameof(DemoViewModel.Budget)).BindingMode(BindingMode.TwoWay)),

                new Label()
                    .Text(e => e.Path(nameof(DemoViewModel.Budget)).Convert((double b) => $"Budget: {b:C0}"))
                    .FontAttributes(Bold),

                new Label()
                    .Text(e => e.Path(nameof(DemoViewModel.Affordable)).Convert((int n) => $"{n} of {DemoViewModel.Catalogue.Count} products fit."))
                    .Muted(),

                // The same source value driving a colour instead of text.
                new BoxView()
                    .HeightRequest(8)
                    .CornerRadius(4)
                    .Color(e => e.Path(nameof(DemoViewModel.Budget))
                        .Convert((double b) => b < 40 ? AppColors.Danger : b < 120 ? AppColors.Warning : AppColors.Success))
            ),
            Demo.Code("""
                new Label()
                    .Text(e => e.Path("Budget").Convert((double b) => $"Budget: {b:C0}"))

                new BoxView()
                    .Color(e => e.Path("Budget")
                        .Convert((double b) => b < 40 ? Colors.Red : Colors.Green))
                """),
            Demo.Note("A reusable IValueConverter still works — pass it with .Converter(...) and its argument with .Parameter(...)."));

    private static IView Fallbacks()
        => Demo.Section(
            "Fallbacks",
            "FallbackValue covers a path that fails to resolve; TargetNullValue covers a path that resolves to null. Both keep an empty state from looking broken.",
            Demo.Stage(
                new Label()
                    .Text(e => e.Path("ThisPropertyDoesNotExist").FallbackValue("FallbackValue kicked in"))
                    .Mono()
                    .TextColor(AppColors.Warning),

                new Label()
                    .Text(e => e.Path(nameof(DemoViewModel.LastAction)).TargetNullValue("—"))
                    .Muted()
            ),
            Demo.Code("""
                new Label().Text(e => e.Path("Missing").FallbackValue("—"))
                new Label().Text(e => e.Path("Nickname").TargetNullValue("no nickname"))
                """));
}
