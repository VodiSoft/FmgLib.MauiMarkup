using FmgLib.MauiMarkup.Gallery.Controls;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Layout options and text alignment — the two things people confuse most often.
/// </summary>
public partial class LayoutPage : DemoPage
{
    public LayoutPage() => this.InitializeHotReload();

    protected override string DemoTitle => "Layout & Alignment";

    protected override string DemoSummary =>
        "Every LayoutOptions combination collapses into one readable method — and the same idea applies again to text inside a control.";

    protected override IView[] BuildSections() =>
    [
        NinePositions(),
        TheConfusion(),
        FillAndStack()
    ];

    private static IView NinePositions()
    {
        var grid = new Grid()
            .RowDefinitions(e => e.Absolute(74, count: 3))
            .ColumnDefinitions(e => e.Star(1, count: 3))
            .RowSpacing(Ui.GapSm)
            .ColumnSpacing(Ui.GapSm);

        (string name, Func<Label, Label> apply)[] cells =
        [
            ("AlignTopLeft", l => l.AlignTopLeft()),
            ("AlignTopCenter", l => l.AlignTopCenter()),
            ("AlignTopRight", l => l.AlignTopRight()),
            ("AlignCenterLeft", l => l.AlignCenterLeft()),
            ("Center", l => l.Center()),
            ("AlignCenterRight", l => l.AlignCenterRight()),
            ("AlignBottomLeft", l => l.AlignBottomLeft()),
            ("AlignBottomCenter", l => l.AlignBottomCenter()),
            ("AlignBottomRight", l => l.AlignBottomRight())
        ];

        for (var index = 0; index < cells.Length; index++)
        {
            var (name, apply) = cells[index];

            grid.Children.Add(
                new Border()
                    .Row(index / 3)
                    .Column(index % 3)
                    .Stage(10)
                    .Padding(Ui.GapSm)
                    .Content(apply(new Label().Text(name).FontSize(10.5).Mono().TextColor(AppColors.Accent)))
            );
        }

        return Demo.Section(
            "Nine positions, nine methods",
            "Each cell holds the same Label; only the alignment method differs. Think of the names as a grid of (vertical, horizontal) values.",
            grid,
            Demo.Code("""
                new Label().Text("Ready.").AlignBottomCenter()
                new Label().Text("⚙").AlignBottomRight().Margin(0, 0, 12, 12)

                // …and the general form for anything computed at runtime:
                new Label().AlignLayout(vertical: LayoutOptions.End, horizontal: LayoutOptions.Center)
                """));
    }

    private static IView TheConfusion()
        => Demo.Section(
            "Center() vs. TextCenter()",
            "Center() positions the VIEW inside its parent. TextCenter() positions the TEXT inside the view. On a label sized to its content, TextCenter() looks like it does nothing — because the box is exactly as big as the text.",
            new Grid()
            .ColumnDefinitions(e => e.Star().Star())
            .ColumnSpacing(Ui.Gap)
            .Children(
                new Border()
                    .Stage(12)
                    .HeightRequest(120)
                    .Content(
                        new Label()
                            .Text(".Center()")
                            .Mono()
                            .BackgroundColor(AppColors.Accent.WithAlpha(0.15f))
                            .Center()
                    ),

                new Border()
                    .Column(1)
                    .Stage(12)
                    .HeightRequest(120)
                    .Content(
                        new Label()
                            .Text(".FillBothDirections()\n.TextCenter()")
                            .Mono()
                            .BackgroundColor(AppColors.Magenta.WithAlpha(0.15f))
                            .FillBothDirections()
                            .TextCenter()
                    )
            ),
            Demo.Note("Give the control space first, then align inside it — the two are almost always used together."));

    private static IView FillAndStack()
        => Demo.Section(
            "Filling and stacking",
            "The helpers apply to any View — layouts included, so a whole stack can be centred in one call.",
            Demo.Stage(
                new HorizontalStackLayout()
                .Spacing(Ui.GapSm)
                .Children(
                    new Border().Stage(8).Padding(Ui.GapSm).Content(new Label().Text("Auto").FontSize(12)),
                    new Border().Stage(8).Padding(Ui.GapSm).FillHorizontal().Content(new Label().Text("FillHorizontal()").FontSize(12).TextCenterHorizontal())
                ),

                new Border()
                    .Stage(10)
                    .HeightRequest(90)
                    .Content(
                        new VerticalStackLayout()
                            .Center()
                            .Spacing(2)
                            .Children(
                                new Label().Text("A centred stack").FontAttributes(Bold).TextCenterHorizontal(),
                                new Label().Text("the layout itself is centred").Muted().FontSize(12).TextCenterHorizontal()
                            )
                    )
            ),
            Demo.Code("""
                new VerticalStackLayout()
                    .Center()                     // the LAYOUT is centred in its parent
                    .Children(
                        new Label().Text("A centred stack").TextCenterHorizontal()
                    )
                """));
}
