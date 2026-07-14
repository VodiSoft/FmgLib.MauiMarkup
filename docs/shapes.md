# Shapes & Geometries

MAUI's vector shape controls (`Microsoft.Maui.Controls.Shapes`) are fully fluent-enabled: `Line`, `Rectangle`, `RoundRectangle`, `Ellipse`, `Polygon`, `Polyline` and `Path`, plus all geometry and transform types.

## Common Shape Properties

Every shape derives from `Shape`, so these methods work on all of them:

| Method | Description |
|---|---|
| `.Fill(Brush)` | Interior brush (solid color or [gradient](gradients-and-brushes.md)) |
| `.Stroke(Brush)` | Outline brush |
| `.StrokeThickness(double)` | Outline width |
| `.StrokeDashArray(...)` / `.StrokeDashOffset(double)` | Dashed outlines |
| `.StrokeLineCap(PenLineCap)` / `.StrokeLineJoin(PenLineJoin)` | Line end/corner styles |
| `.StrokeMiterLimit(double)` | Miter clipping |
| `.Aspect(Stretch)` | How the geometry stretches into the bounds |

`double`-typed ones also get [`Animate…To` helpers](animations.md) (`AnimateStrokeThicknessTo`, `AnimateStrokeDashOffsetTo`, …).

## Basic Shapes

```csharp
// Line
new Line()
    .X1(0).Y1(0).X2(200).Y2(0)
    .Stroke(Colors.Gray)
    .StrokeThickness(1)

// Rectangle & RoundRectangle
new Rectangle()
    .Fill(Colors.CornflowerBlue)
    .SizeRequest(120, 60)

new RoundRectangle()
    .CornerRadius(12)                       // or CornerRadius(tl, tr, bl, br)
    .Fill(Colors.LightSteelBlue)
    .SizeRequest(120, 60)

// Ellipse (circle when width == height)
new Ellipse()
    .Fill(Colors.Tomato)
    .SizeRequest(80, 80)

// Polygon — closed; Polyline — open
new Polygon()
    .Points(new PointCollection { new(0, 48), new(0, 144), new(96, 150), new(100, 24), new(48, 0) })
    .Fill(Colors.LightPink)
    .Stroke(Colors.Red)
    .StrokeThickness(3)

new Polyline()
    .Points(new PointCollection { new(0, 0), new(30, 60), new(60, 20), new(90, 80) })
    .Stroke(Colors.SeaGreen)
    .StrokeThickness(2)
```

## `Path` and Geometries

`Path.Data(...)` accepts any `Geometry` — all of which are fluent-enabled: `LineGeometry`, `EllipseGeometry`, `RectangleGeometry`, `RoundRectangleGeometry`, `GeometryGroup`, `PathGeometry` (with `PathFigure`, `LineSegment`, `ArcSegment`, `BezierSegment`, `QuadraticBezierSegment`, `Poly*Segment`s).

```csharp
new Microsoft.Maui.Controls.Shapes.Path()
    .Stroke(Colors.DarkSlateBlue)
    .StrokeThickness(2)
    .Fill(Colors.Lavender)
    .Data(
        new PathGeometry()
        .Figures(
            new PathFigure()
                .StartPoint(new Point(10, 100))
                .Segments(
                    new LineSegment().Point(new Point(60, 20)),
                    new ArcSegment()
                        .Point(new Point(140, 20))
                        .Size(new Size(40, 40))
                        .SweepDirection(SweepDirection.Clockwise),
                    new BezierSegment()
                        .Point1(new Point(160, 60))
                        .Point2(new Point(190, 90))
                        .Point3(new Point(120, 140))
                )
                .IsClosed(true)
        )
    )
```

Grouped geometries:

```csharp
new Microsoft.Maui.Controls.Shapes.Path()
    .Fill(Colors.Orchid)
    .Data(
        new GeometryGroup()
        .Children(
            new EllipseGeometry().Center(new Point(50, 50)).RadiusX(40).RadiusY(40),
            new RectangleGeometry().Rect(new Rect(60, 60, 80, 40))
        )
    )
```

> **SVG path strings:** `Path.Data` in XAML accepts `"M10,100 L60,20 …"` via a type converter. In C#, convert explicitly when you have a path string:
>
> ```csharp
> using Microsoft.Maui.Controls.Shapes;
>
> var geometry = (Geometry)new PathGeometryConverter()
>     .ConvertFromInvariantString("M13.9,16.2 L32,16.2 32,32 13.9,32 Z");
>
> new Path().Fill(Colors.Black).Data(geometry)
> ```

## Clipping Views

Geometries also drive `VisualElement.Clip` — e.g. a circular avatar:

```csharp
new Image()
    .Source("profile.jpg")
    .SizeRequest(96, 96)
    .Clip(new EllipseGeometry().Center(new Point(48, 48)).RadiusX(48).RadiusY(48))
```

## Transforms

Render transforms on geometries/shapes are fluent too: `RotateTransform`, `ScaleTransform`, `SkewTransform`, `TranslateTransform`, `MatrixTransform`, `CompositeTransform`, `TransformGroup`:

```csharp
new RectangleGeometry()
    .Rect(new Rect(0, 0, 100, 50))
    .Transform(new RotateTransform().Angle(45).CenterX(50).CenterY(25))
```

## Practical Recipes

**Divider line:**

```csharp
static Line Divider() => new Line().X2(1000).Stroke(Colors.LightGray).StrokeThickness(1);
```

**Dashed "upload here" drop zone:**

```csharp
new Rectangle()
    .Stroke(Colors.Gray)
    .StrokeThickness(2)
    .StrokeDashArray(new double[] { 4, 3 })
    .RadiusX(10).RadiusY(10)
    .HeightRequest(120)
```

**Progress ring (arc):** compose an `ArcSegment` inside a `PathGeometry` and update its `Point`/`IsLargeArc` from code via a captured reference ([Assign](assign-and-references.md)).

**Border shapes:** `Border.StrokeShape` takes a shape — the most common use of `RoundRectangle`:

```csharp
new Border()
    .StrokeShape(new RoundRectangle().CornerRadius(16))
    .Content(/* ... */)
```

## Related Topics

- [Gradients & Brushes](gradients-and-brushes.md) — fills and strokes
- [Animations](animations.md)
- [Fluent Properties](fluent-properties.md)
