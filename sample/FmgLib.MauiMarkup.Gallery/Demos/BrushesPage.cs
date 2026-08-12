using FmgLib.MauiMarkup.Gallery.Controls;
using Microsoft.Maui.Controls.Shapes;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Gradients as first-class fluent objects, on any Brush-typed property.
/// </summary>
public partial class BrushesPage : DemoPage
{
    public BrushesPage() => this.InitializeHotReload();

    protected override string DemoTitle => "Gradients & Brushes";

    protected override string DemoSummary =>
        "Any Brush property — Background above all — takes solid colours, linear gradients and radial gradients, built with the same fluent syntax.";

    protected override IView[] BuildSections() =>
    [
        Directions(),
        Radial(),
        OnShapesAndText(),
        Interactive()
    ];

    private static LinearGradientBrush Linear(Point start, Point end, params Color[] colors)
    {
        var brush = new LinearGradientBrush().StartPoint(start).EndPoint(end);
        var stops = new GradientStop[colors.Length];

        for (var index = 0; index < colors.Length; index++)
            stops[index] = new GradientStop(colors[index], colors.Length == 1 ? 0f : index / (float)(colors.Length - 1));

        return brush.GradientStops(stops);
    }

    private static IView Directions()
        => Demo.Section(
            "LinearGradientBrush",
            "StartPoint and EndPoint are proportional: (0,0) is top-left and (1,1) bottom-right, so the direction is the vector between them.",
            Demo.WrapStage(
                Demo.Swatch("→ horizontal", border => border
                    .Background(Linear(new Point(0, 0), new Point(1, 0), AppColors.Accent, AppColors.Cyan))),

                Demo.Swatch("↓ vertical", border => border
                    .Background(Linear(new Point(0, 0), new Point(0, 1), AppColors.Violet, AppColors.Magenta))),

                Demo.Swatch("↘ diagonal", border => border
                    .Background(Linear(new Point(0, 0), new Point(1, 1), AppColors.AccentDeep, AppColors.Accent, AppColors.Magenta))),

                Demo.Swatch("multi-stop", border => border
                    .Background(Linear(new Point(0, 0), new Point(1, 1),
                        AppColors.Warning, AppColors.Danger, AppColors.Magenta, AppColors.Violet)))
            ),
            Demo.Code("""
                new Border().Background(
                    new LinearGradientBrush()
                    .StartPoint(new Point(0, 0))
                    .EndPoint(new Point(1, 1))
                    .GradientStops(
                        new GradientStop(Colors.Yellow, 0.0f),
                        new GradientStop(Colors.Red, 0.5f),
                        new GradientStop(Colors.Blue, 1.0f)))
                """));

    private static IView Radial()
        => Demo.Section(
            "RadialGradientBrush",
            "Radiates from Center out to Radius — both proportional — which is what gives a surface a light source.",
            Demo.WrapStage(
                Demo.Swatch("centred", border => border
                    .Background(new RadialGradientBrush()
                        .Center(new Point(0.5, 0.5))
                        .Radius(0.7)
                        .GradientStops(
                            new GradientStop(Colors.White, 0f),
                            new GradientStop(AppColors.Accent, 1f)))),

                Demo.Swatch("off-centre", border => border
                    .Background(new RadialGradientBrush()
                        .Center(new Point(0.25, 0.2))
                        .Radius(0.9)
                        .GradientStops(
                            new GradientStop(AppColors.Cyan, 0f),
                            new GradientStop(AppColors.AccentDeep, 1f)))),

                Demo.Swatch("spotlight", border => border
                    .Background(new RadialGradientBrush()
                        .Center(new Point(0.5, 0.5))
                        .Radius(0.35)
                        .GradientStops(
                            new GradientStop(AppColors.Warning, 0f),
                            new GradientStop(AppColors.Warning.WithAlpha(0f), 1f))))
            ),
            Demo.Code("""
                new BoxView().Background(
                    new RadialGradientBrush()
                    .Center(new Point(0.5, 0.5))
                    .Radius(0.6)
                    .GradientStops(
                        new GradientStop(Colors.White, 0.0f),
                        new GradientStop(Colors.MidnightBlue, 1.0f)))
                """));

    private static IView OnShapesAndText()
        => Demo.Section(
            "Brushes are not just backgrounds",
            "A Shape's Fill and Stroke are Brush properties too, so the same gradient paints an outline or an icon.",
            Demo.WrapStage(
                Demo.Swatch("gradient fill", border => border
                    .BackgroundColor(Colors.Transparent)
                    .Content(
                        new Ellipse()
                            .SizeRequest(66, 66)
                            .Center()
                            .Fill(Linear(new Point(0, 0), new Point(1, 1), AppColors.Accent, AppColors.Magenta)))),

                Demo.Swatch("gradient stroke", border => border
                    .BackgroundColor(Colors.Transparent)
                    .Content(
                        new Ellipse()
                            .SizeRequest(66, 66)
                            .Center()
                            .Fill(new SolidColorBrush(Colors.Transparent))
                            .StrokeThickness(7)
                            .Stroke(Linear(new Point(0, 0), new Point(1, 1), AppColors.Cyan, AppColors.Violet)))),

                Demo.Swatch("gradient border", border => border
                    .Padding(3)
                    .Background(Linear(new Point(0, 0), new Point(1, 1), AppColors.Warning, AppColors.Magenta))
                    .Content(
                        new Border()
                            .Surface()
                            .StrokeThickness(0)
                            .StrokeShape(new RoundRectangle().CornerRadius(9))
                            .Content(new Label().Text("card").FontSize(11).TextCenter())))
            ),
            Demo.Code("""
                new Ellipse()
                    .Fill(gradient)                 // Brush
                    .Stroke(anotherGradient)        // Brush
                    .StrokeThickness(7)

                // "gradient border": a gradient Border with a small padding, wrapping a solid one.
                """));

    private static IView Interactive()
    {
        var surface = new Border()
            .HeightRequest(120)
            .StrokeThickness(0)
            .StrokeShape(new RoundRectangle().CornerRadius(16))
            .Background(Linear(new Point(0, 0), new Point(1, 0), AppColors.Accent, AppColors.Magenta))
            .Content(new Label().Text("Drag the slider").TextColor(Colors.White).FontAttributes(Bold).TextCenter());

        var slider = new Slider()
            .Minimum(0)
            .Maximum(1)
            .Value(0)
            .OnValueChanged((_, e) =>
                surface.Background = Linear(
                    new Point(0, e.NewValue),
                    new Point(1, 1 - e.NewValue),
                    AppColors.Accent,
                    AppColors.Magenta));

        return Demo.Section(
            "Brushes are ordinary objects",
            "Because a brush is just an object, rebuilding it at runtime is a plain assignment — no XAML resource juggling.",
            Demo.Stage(surface, slider),
            Demo.Code("""
                slider.OnValueChanged((s, e) =>
                    surface.Background = new LinearGradientBrush()
                        .StartPoint(new Point(0, e.NewValue))
                        .EndPoint(new Point(1, 1 - e.NewValue))
                        .GradientStops(…));
                """),
            Demo.Note("Background (Brush) paints over BackgroundColor (Color) when both are set."));
    }
}
