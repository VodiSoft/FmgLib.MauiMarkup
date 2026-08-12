using FmgLib.MauiMarkup.Gallery.Controls;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Grid definitions and child placement — MAUI's most verbose layout, made short.
/// </summary>
public partial class GridPage : DemoPage
{
    public GridPage() => this.InitializeHotReload();

    protected override string DemoTitle => "Grid";

    protected override string DemoSummary =>
        "Row and column definitions become a builder lambda with three verbs — Star, Auto and Absolute — and a count parameter that kills copy-paste.";

    protected override IView[] BuildSections() =>
    [
        Definitions(),
        Counting(),
        Spans(),
        Overlay()
    ];

    private static View Cell(string text, Color tint)
        => new Border()
            .StrokeThickness(0)
            .BackgroundColor(tint.WithAlpha(0.18f))
            .Padding(Ui.GapSm)
            .Content(new Label().Text(text).FontSize(11.5).Mono().TextColor(tint).TextCenter());

    private static IView Definitions()
        => Demo.Section(
            "Star, Auto and Absolute",
            "Star takes a proportional share of what is left, Auto sizes to content, Absolute is fixed. The lambda reads in the same order as the XAML string it replaces.",
            new Border()
            .Stage()
            .Content(
                new Grid()
                .RowDefinitions(e => e.Auto().Star().Absolute(44))
                .ColumnDefinitions(e => e.Absolute(90).Star(3).Star(1))
                .HeightRequest(220)
                .RowSpacing(6)
                .ColumnSpacing(6)
                .Children(
                    Cell("Absolute(90)", AppColors.Accent),
                    Cell("Star(3)", AppColors.Violet).Column(1),
                    Cell("Star(1)", AppColors.Magenta).Column(2),

                    Cell("row: Star()", AppColors.Info).Row(1).ColumnSpan(3),

                    Cell("row: Absolute(44)", AppColors.Success).Row(2).ColumnSpan(3)
                )
            ),
            Demo.Code("""
                new Grid()
                .RowDefinitions(e => e.Auto().Star().Absolute(44))
                .ColumnDefinitions(e => e.Absolute(90).Star(3).Star(1))

                // XAML equivalent:
                // <Grid RowDefinitions="Auto,*,44" ColumnDefinitions="90,3*,*">
                """));

    private static IView Counting()
    {
        var strip = new Grid()
            .ColumnDefinitions(e => e.Star(1, count: 7))
            .ColumnSpacing(6)
            .HeightRequest(64);

        var days = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

        for (var index = 0; index < days.Length; index++)
        {
            var isWeekend = index >= 5;

            strip.Children.Add(
                new Border()
                    .Column(index)
                    .Stage(10)
                    .Padding(4)
                    .Content(
                        new VerticalStackLayout()
                        .Center()
                        .Spacing(2)
                        .Children(
                            new Label().Text(days[index]).FontSize(10).Muted().TextCenterHorizontal(),
                            new Label()
                                .Text($"{index + 1}")
                                .FontSize(15)
                                .FontAttributes(Bold)
                                .TextColor(isWeekend ? AppColors.Magenta : AppColors.Accent)
                                .TextCenterHorizontal()
                        )
                    )
            );
        }

        return Demo.Section(
            "The count parameter",
            "Uniform grids need one call, not seven. This whole week strip is a single ColumnDefinitions line.",
            strip,
            Demo.Code("""
                new Grid()
                    .ColumnDefinitions(e => e.Star(1, count: 7))
                    .ColumnSpacing(6)
                """));
    }

    private static IView Spans()
        => Demo.Section(
            "Placing and spanning",
            "Row, Column, RowSpan and ColumnSpan are ordinary fluent methods; GridSpan sets both spans at once. Row and column default to 0, so the first cell needs no calls at all.",
            new Border()
            .Stage()
            .Content(
                new Grid()
                .RowDefinitions(e => e.Star(1, count: 3))
                .ColumnDefinitions(e => e.Star(1, count: 3))
                .HeightRequest(200)
                .RowSpacing(6)
                .ColumnSpacing(6)
                .Children(
                    Cell("GridSpan(2, 2)", AppColors.Accent).GridSpan(column: 2, row: 2),
                    Cell("Column(2)", AppColors.Violet).Column(2),
                    Cell("Column(2)\nRow(1)", AppColors.Magenta).Column(2).Row(1),
                    Cell("Row(2)\nColumnSpan(3)", AppColors.Success).Row(2).ColumnSpan(3)
                )
            ),
            Demo.Code("""
                new Border().GridSpan(column: 2, row: 2)      // both spans
                new Label().Column(2).Row(1)                  // placement
                new BoxView().Row(2).ColumnSpan(3)
                """));

    private static IView Overlay()
        => Demo.Section(
            "A grid with no definitions is an overlay",
            "Children with no row or column all land in the single implicit cell and stack in declaration order — the shortest way to caption an image or float a badge.",
            new Border()
            .StrokeThickness(0)
            .Padding(0)
            .Content(
                new Grid()
                .HeightRequest(160)
                .Children(
                    new Border()
                        .StrokeThickness(0)
                        .Background(Ui.BrandGradient(1, 0))
                        .Content(new Label().Text("🖼").FontSize(44).TextCenter()),

                    new Border()
                        .Pill(Colors.Black)
                        .BackgroundColor(Colors.Black.WithAlpha(0.55f))
                        .Margin(Ui.Gap)
                        .AlignTopRight()
                        .Content(new Label().Text("NEW").FontSize(10).FontAttributes(Bold).TextColor(Colors.White)),

                    new Label()
                        .Text("Caption over the image")
                        .TextColor(Colors.White)
                        .FontAttributes(Bold)
                        .Margin(Ui.Gap)
                        .AlignBottomCenter()
                )
            ),
            Demo.Code("""
                new Grid().Children(
                    new Image().Source("photo.png"),
                    new Label().Text("Caption").AlignBottomCenter().TextColor(Colors.White)
                )
                """));
}
