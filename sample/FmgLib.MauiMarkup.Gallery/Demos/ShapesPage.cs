using FmgLib.MauiMarkup.Gallery.Controls;
using Microsoft.Maui.Controls.Shapes;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Vector shapes, geometries, clipping and transforms — all fluent.
/// </summary>
public partial class ShapesPage : DemoPage
{
    public ShapesPage() => this.InitializeHotReload();

    protected override string DemoTitle => "Shapes & Paths";

    protected override string DemoSummary =>
        "Line, Rectangle, Ellipse, Polygon, Polyline and Path — plus every geometry and transform type — are ordinary fluent objects.";

    protected override IView[] BuildSections() =>
    [
        Primitives(),
        Paths(),
        Clipping(),
        Recipes()
    ];

    private static View Framed(string caption, View shape)
        => new VerticalStackLayout()
            .Spacing(Ui.GapXs)
            .Margin(0, 0, Ui.Gap, Ui.Gap)
            .Children(
                new Border()
                    .Stage(12)
                    .SizeRequest(132, 108)
                    .Content(shape),
                new Label().Text(caption).Muted().FontSize(11).WidthRequest(132).TextCenterHorizontal()
            );

    private static IView Primitives()
        => Demo.Section(
            "The primitives",
            "Every shape derives from Shape, so Fill, Stroke, StrokeThickness and the dash properties work on all of them.",
            Demo.WrapStage(
                Framed("Ellipse",
                    new Ellipse()
                        .Fill(new SolidColorBrush(AppColors.Accent))
                        .SizeRequest(74, 74)
                        .Center()),

                Framed("RoundRectangle",
                    new RoundRectangle()
                        .CornerRadius(16)
                        .Fill(new SolidColorBrush(AppColors.Violet))
                        .SizeRequest(92, 62)
                        .Center()),

                Framed("Polygon",
                    new Polygon()
                        .Points([new(40, 0), new(80, 30), new(64, 76), new(16, 76), new(0, 30)])
                        .Fill(new SolidColorBrush(AppColors.Magenta.WithAlpha(0.35f)))
                        .Stroke(new SolidColorBrush(AppColors.Magenta))
                        .StrokeThickness(2)
                        .Center()),

                Framed("Polyline",
                    new Polyline()
                        .Points([new(0, 60), new(24, 18), new(48, 46), new(72, 6), new(92, 34)])
                        .Stroke(new SolidColorBrush(AppColors.Success))
                        .StrokeThickness(3)
                        .StrokeLineCap(PenLineCap.Round)
                        .StrokeLineJoin(PenLineJoin.Round)
                        .Center()),

                Framed("Dashed Rectangle",
                    new Rectangle()
                        .Stroke(new SolidColorBrush(AppColors.Info))
                        .StrokeThickness(2)
                        .StrokeDashArray([4, 3])
                        .RadiusX(10)
                        .RadiusY(10)
                        .SizeRequest(96, 68)
                        .Center()),

                Framed("Line",
                    new Line()
                        .X1(0).Y1(0).X2(96).Y2(0)
                        .Stroke(new SolidColorBrush(AppColors.Warning))
                        .StrokeThickness(3)
                        .Center())
            ),
            Demo.Code("""
                new Ellipse().Fill(new SolidColorBrush(AppColors.Accent)).SizeRequest(74, 74)

                new Rectangle()
                    .Stroke(new SolidColorBrush(AppColors.Info))
                    .StrokeThickness(2)
                    .StrokeDashArray([4, 3])
                    .RadiusX(10).RadiusY(10)
                """));

    private static IView Paths()
        => Demo.Section(
            "Path and geometries",
            "Path.Data accepts any Geometry, and every geometry type — figures, segments, groups — is fluent as well.",
            Demo.WrapStage(
                Framed("PathGeometry",
                    new Path()
                        .Stroke(new SolidColorBrush(AppColors.Accent))
                        .StrokeThickness(2.5)
                        .Fill(new SolidColorBrush(AppColors.Accent.WithAlpha(0.18f)))
                        .Center()
                        .Data(
                            new PathGeometry()
                            .Figures(
                                new PathFigure()
                                    .StartPoint(new Point(6, 70))
                                    .Segments(
                                        new LineSegment().Point(new Point(34, 14)),
                                        new ArcSegment()
                                            .Point(new Point(78, 20))
                                            .Size(new Size(24, 24))
                                            .SweepDirection(SweepDirection.Clockwise),
                                        new BezierSegment()
                                            .Point1(new Point(92, 40))
                                            .Point2(new Point(96, 58))
                                            .Point3(new Point(52, 78)))
                                    .IsClosed(true)
                            )
                        )),

                Framed("GeometryGroup",
                    new Path()
                        .Fill(new SolidColorBrush(AppColors.Violet.WithAlpha(0.55f)))
                        .Center()
                        .Data(
                            new GeometryGroup()
                            .Children(
                                new EllipseGeometry().Center(new Point(34, 34)).RadiusX(30).RadiusY(30),
                                new RectangleGeometry().Rect(new Rect(44, 44, 52, 30))
                            )
                        )),

                Framed("Rotation",
                    new Path()
                        .Fill(new SolidColorBrush(AppColors.Magenta.WithAlpha(0.6f)))
                        .Center()
                        .Rotation(-18)
                        .Data(new RectangleGeometry().Rect(new Rect(0, 0, 78, 40))))
            ),
            Demo.Code("""
                new Path()
                    .Stroke(new SolidColorBrush(AppColors.Accent))
                    .Data(new PathGeometry().Figures(
                        new PathFigure()
                            .StartPoint(new Point(6, 70))
                            .Segments(
                                new LineSegment().Point(new Point(34, 14)),
                                new ArcSegment().Point(new Point(78, 20)).Size(new Size(24, 24)))
                            .IsClosed(true)))
                """),
            Demo.Note("SVG path strings have no type converter in C# — convert explicitly with PathGeometryConverter().ConvertFromInvariantString(\"M10,100 …\")."));

    private static IView Clipping()
        => Demo.Section(
            "Clipping any view",
            "Geometries also drive VisualElement.Clip, which is the shortest circular-avatar in MAUI.",
            Demo.WrapStage(
                Framed("Clip → circle",
                    new Border()
                        .SizeRequest(84, 84)
                        .StrokeThickness(0)
                        .Center()
                        .Background(Ui.BrandGradient())
                        .Content(new Label().Text("AL").FontSize(26).FontAttributes(Bold).TextColor(Colors.White).TextCenter())
                        .Clip(new EllipseGeometry().Center(new Point(42, 42)).RadiusX(42).RadiusY(42))),

                Framed("Border.StrokeShape",
                    new Border()
                        .SizeRequest(96, 68)
                        .StrokeThickness(2)
                        .Stroke(new SolidColorBrush(AppColors.Accent))
                        .StrokeShape(new RoundRectangle().CornerRadius(24, 4, 24, 4))
                        .Center()
                        .Content(new Label().Text("shaped").FontSize(11).TextCenter()))
            ),
            Demo.Code("""
                new Image()
                    .Source("profile.jpg")
                    .SizeRequest(96, 96)
                    .Clip(new EllipseGeometry().Center(new Point(48, 48)).RadiusX(48).RadiusY(48))

                new Border().StrokeShape(new RoundRectangle().CornerRadius(24, 4, 24, 4))
                """));

    private static IView Recipes()
        => Demo.Section(
            "Everyday recipes",
            "Two shapes cover most real screens: a hairline divider and a dashed drop zone.",
            Demo.Stage(
                new Label().Text("Above the divider").Muted().FontSize(12),
                new Line().X2(2000).Stroke(new SolidColorBrush(AppColors.BorderLight)).StrokeThickness(1),
                new Label().Text("Below the divider").Muted().FontSize(12),

                new Grid()
                .HeightRequest(96)
                .Margin(0, Ui.GapSm, 0, 0)
                .Children(
                    new Rectangle()
                        .Stroke(new SolidColorBrush(AppColors.MutedLight))
                        .StrokeThickness(2)
                        .StrokeDashArray([5, 4])
                        .RadiusX(12)
                        .RadiusY(12),

                    new Label().Text("Drop files here").Muted().TextCenter()
                )
            ),
            Demo.Code("""
                static Line Divider() => new Line().X2(2000).Stroke(Colors.LightGray).StrokeThickness(1);

                new Rectangle()
                    .Stroke(Colors.Gray)
                    .StrokeThickness(2)
                    .StrokeDashArray([4, 3])
                    .RadiusX(10).RadiusY(10)
                """));
}
