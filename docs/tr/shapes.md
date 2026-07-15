# Şekiller ve Geometriler

MAUI'nin vektör şekil kontrolleri (`Microsoft.Maui.Controls.Shapes`) tam fluent desteklidir: `Line`, `Rectangle`, `RoundRectangle`, `Ellipse`, `Polygon`, `Polyline` ve `Path`; ayrıca tüm geometri ve transform tipleri.

## Ortak Şekil Özellikleri

Her şekil `Shape`'ten türediğinden bu metotlar hepsinde çalışır:

| Metot | Açıklama |
|---|---|
| `.Fill(Brush)` | İç fırça (düz renk veya [gradyan](gradients-and-brushes.md)) |
| `.Stroke(Brush)` | Kontur fırçası |
| `.StrokeThickness(double)` | Kontur kalınlığı |
| `.StrokeDashArray(...)` / `.StrokeDashOffset(double)` | Kesikli kontur |
| `.StrokeLineCap(PenLineCap)` / `.StrokeLineJoin(PenLineJoin)` | Uç/köşe stilleri |
| `.StrokeMiterLimit(double)` | Miter sınırı |
| `.Aspect(Stretch)` | Geometrinin sınırlara nasıl gerileceği |

`double` tipli olanlar ayrıca [`Animate…To` yardımcıları](animations.md) alır (`AnimateStrokeThicknessTo`, `AnimateStrokeDashOffsetTo`, …).

## Temel Şekiller

```csharp
// Line
new Line()
    .X1(0).Y1(0).X2(200).Y2(0)
    .Stroke(Colors.Gray)
    .StrokeThickness(1)

// Rectangle ve RoundRectangle
new Rectangle()
    .Fill(Colors.CornflowerBlue)
    .SizeRequest(120, 60)

new RoundRectangle()
    .CornerRadius(12)                       // veya CornerRadius(tl, tr, bl, br)
    .Fill(Colors.LightSteelBlue)
    .SizeRequest(120, 60)

// Ellipse (genişlik == yükseklik ise daire)
new Ellipse()
    .Fill(Colors.Tomato)
    .SizeRequest(80, 80)

// Polygon — kapalı; Polyline — açık
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

## `Path` ve Geometriler

`Path.Data(...)` her `Geometry`'yi kabul eder — hepsi fluent'tir: `LineGeometry`, `EllipseGeometry`, `RectangleGeometry`, `RoundRectangleGeometry`, `GeometryGroup`, `PathGeometry` (`PathFigure`, `LineSegment`, `ArcSegment`, `BezierSegment`, `QuadraticBezierSegment`, `Poly*Segment`'lerle).

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

Gruplanmış geometriler:

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

> **SVG path string'leri:** XAML'de `Path.Data`, tip dönüştürücü sayesinde `"M10,100 L60,20 …"` alır. C#'ta elinizde path string'i varsa açıkça dönüştürün:
>
> ```csharp
> using Microsoft.Maui.Controls.Shapes;
>
> var geometry = (Geometry)new PathGeometryConverter()
>     .ConvertFromInvariantString("M13.9,16.2 L32,16.2 32,32 13.9,32 Z");
>
> new Path().Fill(Colors.Black).Data(geometry)
> ```

## View Kırpma (Clip)

Geometriler `VisualElement.Clip`'i de sürer — örn. dairesel avatar:

```csharp
new Image()
    .Source("profile.jpg")
    .SizeRequest(96, 96)
    .Clip(new EllipseGeometry().Center(new Point(48, 48)).RadiusX(48).RadiusY(48))
```

## Transform'lar

Geometri/şekil transform'ları da fluent'tir: `RotateTransform`, `ScaleTransform`, `SkewTransform`, `TranslateTransform`, `MatrixTransform`, `CompositeTransform`, `TransformGroup`:

```csharp
new RectangleGeometry()
    .Rect(new Rect(0, 0, 100, 50))
    .Transform(new RotateTransform().Angle(45).CenterX(50).CenterY(25))
```

## Pratik Tarifler

**Ayırıcı çizgi:**

```csharp
static Line Divider() => new Line().X2(1000).Stroke(Colors.LightGray).StrokeThickness(1);
```

**Kesikli "buraya yükle" bırakma alanı:**

```csharp
new Rectangle()
    .Stroke(Colors.Gray)
    .StrokeThickness(2)
    .StrokeDashArray(new double[] { 4, 3 })
    .RadiusX(10).RadiusY(10)
    .HeightRequest(120)
```

**İlerleme halkası (yay):** `PathGeometry` içinde bir `ArcSegment` kurun; `Point`/`IsLargeArc` değerlerini [Assign](assign-and-references.md) ile yakalanan referans üzerinden koddan güncelleyin.

**Border şekilleri:** `Border.StrokeShape` bir şekil alır — `RoundRectangle`'ın en yaygın kullanımı:

```csharp
new Border()
    .StrokeShape(new RoundRectangle().CornerRadius(16))
    .Content(/* ... */)
```

## İlgili Konular

- [Gradyanlar ve Fırçalar](gradients-and-brushes.md) — dolgu ve konturlar
- [Animasyonlar](animations.md)
- [Fluent Özellikler](fluent-properties.md)
