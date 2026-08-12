using FmgLib.MauiMarkup.Gallery.Controls;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// One page, four platforms, any window size. The three tools that get you there.
/// </summary>
public partial class ResponsivePage : DemoPage
{
    private Label widthReadout = null!;
    private Label breakpointReadout = null!;

    public ResponsivePage()
    {
        SizeChanged += (_, _) => UpdateReadout();

        this.InitializeHotReload();
    }

    protected override string DemoTitle => "Responsive & Adaptive";

    protected override string DemoSummary =>
        "Idiom and platform builders answer 'what am I running on'. Adaptive triggers and size handlers answer 'how much room do I have right now' — which is the one that survives a resized desktop window.";

    protected override IView[] BuildSections() =>
    [
        LiveSize(),
        IdiomAndPlatform(),
        AdaptiveStates(),
        ReflowingLayout()
    ];

    private IView LiveSize()
    {
        widthReadout = new Label().Text("—").FontSize(30).FontAttributes(Bold).TextColor(AppColors.Accent).TextCenterHorizontal();
        breakpointReadout = new Label().Text("—").Muted().TextCenterHorizontal();

        UpdateReadout();

        return Demo.Section(
            "Measure, do not assume",
            "Resize the window (or rotate the device) and watch these update. This page's own breakpoints come from the number below, not from the device type.",
            Demo.Stage(widthReadout, breakpointReadout),
            Demo.Code("""
                public ResponsivePage()
                {
                    SizeChanged += (s, e) => UpdateReadout();
                    this.InitializeHotReload();
                }
                """));
    }

    private void UpdateReadout()
    {
        if (widthReadout is null || breakpointReadout is null)
            return;

        widthReadout.Text = $"{Width:F0} × {Height:F0}";
        breakpointReadout.Text = Width switch
        {
            < 640 => "compact — single column",
            < 960 => "medium — two columns",
            < 1320 => "expanded — three columns",
            _ => "wide — four columns"
        };
    }

    private static IView IdiomAndPlatform()
        => Demo.Section(
            "Idiom and platform values",
            "When the difference really is about the device rather than the size, the builder takes a value per idiom or per platform — resolved for you, with a Default fallback.",
            Demo.Stage(
                new Border()
                    .Stage(10)
                    .Padding(e => e.OnPhone(new Thickness(12)).OnDesktop(new Thickness(28)).Default(new Thickness(18)))
                    .Content(
                        new Label()
                            .Text("My padding and font size come from the idiom builder.")
                            .FontSize(e => e.OnPhone(13.0).OnTablet(15.0).OnDesktop(17.0).Default(14.0))
                            .TextCenterHorizontal()
                    ),

                new HorizontalStackLayout()
                .Spacing(Ui.GapSm)
                .CenterHorizontal()
                .Children(
                    Demo.Chip($"Idiom: {DeviceInfo.Idiom}", AppColors.Violet),
                    Demo.Chip($"Platform: {DeviceInfo.Platform}", AppColors.Info)
                )
            ),
            Demo.Code("""
                new Label()
                    .FontSize(e => e.OnPhone(13.0).OnTablet(15.0).OnDesktop(17.0).Default(14.0))
                    .Margin(e => e.OniOS(new Thickness(0, 20, 0, 0)).Default(new Thickness(0)))
                """));

    private static IView AdaptiveStates()
        => Demo.Section(
            "Adaptive triggers",
            "A visual state can be driven by the window size instead of by interaction, which keeps a responsive rule declarative. Resize the window past 700pt to see this switch.",
            new Border()
            .Stage()
            .Content(
                new Grid()
                .HeightRequest(96)
                .Assign(out var adaptive)
                .Children(
                    new Label()
                        .Text("I am styled by an AdaptiveTrigger.")
                        .TextCenter()
                        .FontAttributes(Bold)
                )
                .VisualStateGroups(
                    new VisualStateGroupList
                    {
                        new VisualState<Grid>("Wide", e => e
                            .BackgroundColor(AppColors.Success.WithAlpha(0.18f)))
                        {
                            new AdaptiveTrigger().MinWindowWidth(700)
                        },
                        new VisualState<Grid>("Narrow", e => e
                            .BackgroundColor(AppColors.Warning.WithAlpha(0.18f)))
                        {
                            new AdaptiveTrigger().MinWindowWidth(0)
                        },
                    })
            ),
            Demo.Code("""
                new Grid().VisualStateGroups(
                    new VisualStateGroupList
                    {
                        new VisualState<Grid>("Wide", e => e.BackgroundColor(Colors.White))
                        {
                            new AdaptiveTrigger().MinWindowWidth(700)
                        },
                        new VisualState<Grid>("Narrow", e => e.BackgroundColor(Colors.WhiteSmoke))
                        {
                            new AdaptiveTrigger().MinWindowWidth(0)
                        },
                    })
                """),
            Demo.Note("OrientationStateTrigger, DeviceStateTrigger and CompareStateTrigger work the same way."));

    private static IView ReflowingLayout()
    {
        // A FlexLayout with wrapping is the least code for a "cards that reflow" layout: no
        // breakpoints, no handlers — the items simply take the next line when they run out of room.
        var flow = new FlexLayout()
            .Wrap(FlexWrap.Wrap)
            .JustifyContent(FlexJustify.Start)
            .AlignItems(FlexAlignItems.Start);

        (string title, string body, Color tint)[] cards =
        [
            ("Composable", "Helpers stack on helpers.", AppColors.Accent),
            ("Typed", "The chain keeps the control type.", AppColors.Violet),
            ("Generated", "Third-party controls too.", AppColors.Magenta),
            ("Live", "Themes repaint, no rebuild.", AppColors.Success)
        ];

        foreach (var (title, body, tint) in cards)
        {
            flow.Children.Add(
                new Border()
                    .Card(14)
                    .Padding(Ui.Gap)
                    .Margin(0, 0, Ui.GapSm, Ui.GapSm)
                    .WidthRequest(190)
                    .FlexBasis(FlexBasis.Auto)
                    .Content(
                        new VerticalStackLayout()
                        .Spacing(2)
                        .Children(
                            new Label().Text(title).FontAttributes(Bold).TextColor(tint),
                            new Label().Text(body).Muted().FontSize(12)
                        )
                    )
            );
        }

        return Demo.Section(
            "Reflowing without breakpoints",
            "A wrapping FlexLayout needs no size handler at all — the cards take the next line when they run out of room. Narrow the window and watch.",
            new Border().Stage().Content(flow),
            Demo.Code("""
                new FlexLayout()
                    .Wrap(FlexWrap.Wrap)
                    .JustifyContent(FlexJustify.Start)
                    .Children(
                        new Border().WidthRequest(190).FlexBasis(FlexBasis.Auto).Content(…)
                    )
                """),
            Demo.Note("The gallery's home screen uses the other approach — a CollectionView whose GridItemsLayout.Span is recalculated on SizeChanged — because it also needs virtualization."));
    }
}
