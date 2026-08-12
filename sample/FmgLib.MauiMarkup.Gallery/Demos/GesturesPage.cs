using FmgLib.MauiMarkup.Gallery.Controls;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Touch and pointer input: every recognizer, fluent, with its events attached inline.
/// </summary>
public partial class GesturesPage : DemoPage
{
    public GesturesPage() => this.InitializeHotReload();

    protected override string DemoTitle => "Gestures";

    protected override string DemoSummary =>
        "GestureRecognizers takes any number of recognizers, and each one's properties and events are fluent — which is what makes any view tappable, draggable or zoomable.";

    protected override IView[] BuildSections() =>
    [
        Tap(),
        Pan(),
        Pinch(),
        Swipe()
    ];

    private static IView Tap()
    {
        var status = new Label().Text("Waiting for a tap…").Muted().TextCenterHorizontal();
        var taps = 0;

        return Demo.Section(
            "Tap",
            "NumberOfTapsRequired separates single from double taps. Command instead of OnTapped is what makes a plain Label MVVM-clickable.",
            Demo.Stage(
                new Grid()
                .ColumnDefinitions(e => e.Star().Star())
                .ColumnSpacing(Ui.Gap)
                .Children(
                    new Border()
                        .Stage(12)
                        .HeightRequest(92)
                        .Content(new Label().Text("Single tap").TextCenter().FontAttributes(Bold))
                        .GestureRecognizers(
                            new TapGestureRecognizer().OnTapped((_, _) =>
                            {
                                taps++;
                                status.Text = $"Single tap — {taps} so far";
                            })
                        ),

                    new Border()
                        .Column(1)
                        .Stage(12)
                        .HeightRequest(92)
                        .Content(new Label().Text("Double tap").TextCenter().FontAttributes(Bold))
                        .GestureRecognizers(
                            new TapGestureRecognizer()
                                .NumberOfTapsRequired(2)
                                .OnTapped((_, _) => status.Text = "Double tap detected")
                        )
                ),
                status
            ),
            Demo.Code("""
                new Border().GestureRecognizers(
                    new TapGestureRecognizer()
                        .NumberOfTapsRequired(2)
                        .OnTapped((s, e) => status.Text = "Double tap")
                )

                // MVVM: any view becomes clickable
                new Label().Text("See all").GestureRecognizers(
                    new TapGestureRecognizer().Command(vm.ShowAllCommand))
                """));
    }

    private static IView Pan()
    {
        double startX = 0, startY = 0;
        var puck = new Border()
            .SizeRequest(64, 64)
            .StrokeThickness(0)
            .Background(Ui.BrandGradient())
            .Center()
            .Content(new Label().Text("drag").FontSize(11).TextColor(Colors.White).TextCenter());

        puck.GestureRecognizers(
            new PanGestureRecognizer().OnPanUpdated((_, e) =>
            {
                switch (e.StatusType)
                {
                    case GestureStatus.Running:
                        puck.TranslationX = Math.Clamp(startX + e.TotalX, -110, 110);
                        puck.TranslationY = Math.Clamp(startY + e.TotalY, -50, 50);
                        break;

                    case GestureStatus.Completed:
                        startX = puck.TranslationX;
                        startY = puck.TranslationY;
                        break;
                }
            })
        );

        return Demo.Section(
            "Pan",
            "PanGestureRecognizer streams deltas while the finger or mouse moves. Remember the offset on Completed, or the next drag jumps back to the origin.",
            new Border().Stage().HeightRequest(150).Content(puck),
            Demo.Code("""
                new PanGestureRecognizer().OnPanUpdated((s, e) =>
                {
                    switch (e.StatusType)
                    {
                        case GestureStatus.Running:
                            puck.TranslationX = startX + e.TotalX;
                            puck.TranslationY = startY + e.TotalY;
                            break;
                        case GestureStatus.Completed:
                            startX = puck.TranslationX;
                            startY = puck.TranslationY;
                            break;
                    }
                })
                """));
    }

    private static IView Pinch()
    {
        var scale = 1.0;
        var target = new Label().Text("🖼").FontSize(52).TextCenter();

        var surface = new Border()
            .Stage()
            .HeightRequest(150)
            .Content(target)
            .GestureRecognizers(
                new PinchGestureRecognizer().OnPinchUpdated((_, e) =>
                {
                    if (e.Status == GestureStatus.Running)
                    {
                        scale = Math.Clamp(scale * e.Scale, 0.5, 3.0);
                        target.Scale = scale;
                    }
                })
            );

        return Demo.Section(
            "Pinch",
            "Two-finger zoom on touch devices; on a trackpad the same recognizer receives the pinch gesture.",
            surface,
            Demo.Code("""
                new PinchGestureRecognizer().OnPinchUpdated((s, e) =>
                {
                    if (e.Status == GestureStatus.Running)
                        target.Scale = Math.Clamp(target.Scale * e.Scale, 0.5, 3.0);
                })
                """));
    }

    private static IView Swipe()
    {
        var readout = new Label().Text("Swipe the panel in any direction.").Muted().TextCenter();

        var panel = new Border()
            .Stage()
            .HeightRequest(120)
            .Content(readout);

        foreach (var direction in new[] { SwipeDirection.Left, SwipeDirection.Right, SwipeDirection.Up, SwipeDirection.Down })
        {
            var captured = direction;

            panel.GestureRecognizers(
                new SwipeGestureRecognizer()
                    .Direction(captured)
                    .Threshold(40)
                    .OnSwiped((_, _) => readout.Text = $"Swiped {captured}")
            );
        }

        return Demo.Section(
            "Swipe and pointer",
            "A SwipeGestureRecognizer handles one direction, so add one per direction you care about. PointerGestureRecognizer covers desktop hover.",
            panel,
            new Border()
            .Stage()
            .HeightRequest(76)
            .Content(new Label().Assign(out var hover).Text("Hover me (desktop)").TextCenter().Muted())
            .GestureRecognizers(
                new PointerGestureRecognizer()
                    .OnPointerEntered((_, _) => hover.Text = "Pointer entered")
                    .OnPointerExited((_, _) => hover.Text = "Pointer exited")
            ),
            Demo.Code("""
                new SwipeGestureRecognizer()
                    .Direction(SwipeDirection.Left)
                    .Threshold(40)
                    .OnSwiped((s, e) => readout.Text = "Swiped left")

                new PointerGestureRecognizer()
                    .OnPointerEntered((s, e) => hover.Text = "Pointer entered")
                """));
    }
}
