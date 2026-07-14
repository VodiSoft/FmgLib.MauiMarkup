# Gradients & Brushes

Any property of type `Brush` — most notably `VisualElement.Background` — accepts solid colors, linear gradients and radial gradients, all buildable fluently.

## LinearGradientBrush

A gradient along an axis defined by `StartPoint` → `EndPoint` (proportional coordinates, `(0,0)` = top-left, `(1,1)` = bottom-right).

```csharp
new Border()
.Background(
    new LinearGradientBrush()
    .StartPoint(new Point(0, 0))
    .EndPoint(new Point(1, 1))          // top-left → bottom-right diagonal
    .GradientStops(
        new List<GradientStop>
        {
            new GradientStop(Colors.Yellow,    0.0f),
            new GradientStop(Colors.Red,       0.25f),
            new GradientStop(Colors.Blue,      0.75f),
            new GradientStop(Colors.LimeGreen, 1.0f)
        }
    )
)
```

Each `GradientStop` pairs a color with an offset (0–1) along the axis. Stops can also be configured fluently:

```csharp
new GradientStop().Color(Colors.Yellow).Offset(0.0f)
```

Common directions:

| Direction | StartPoint | EndPoint |
|---|---|---|
| Horizontal (left→right) | `(0, 0)` | `(1, 0)` |
| Vertical (top→bottom) | `(0, 0)` | `(0, 1)` |
| Diagonal | `(0, 0)` | `(1, 1)` |

## RadialGradientBrush

Radiates from a `Center` outward to `Radius` (both proportional):

```csharp
new BoxView()
.Background(
    new RadialGradientBrush()
    .Center(new Point(0.5, 0.5))
    .Radius(0.6)
    .GradientStops(
        new List<GradientStop>
        {
            new GradientStop(Colors.White,      0.0f),
            new GradientStop(Colors.SkyBlue,    0.6f),
            new GradientStop(Colors.MidnightBlue, 1.0f)
        }
    )
)
```

## SolidColorBrush

For completeness — `Background` also takes plain brushes and colors:

```csharp
new Border().Background(new SolidColorBrush(Colors.Coral))
new Border().Background(Colors.Coral)          // implicit conversion
new Border().BackgroundColor(Colors.Coral)     // the Color property
```

> `Background` (Brush) paints over `BackgroundColor` (Color) when both are set.

## Practical Recipes

**Gradient page header:**

```csharp
new Grid()
.HeightRequest(180)
.Background(
    new LinearGradientBrush()
    .StartPoint(new Point(0, 0))
    .EndPoint(new Point(0, 1))
    .GradientStops(new List<GradientStop>
    {
        new GradientStop("#4C3BCF".ToColor(), 0f),
        new GradientStop("#4B70F5".ToColor(), 1f)
    })
)
.Children(
    new Label().Text("Welcome back").FontSize(28).TextColor(Colors.White).AlignBottomLeft().Margin(20)
)
```

*(Note `"#4C3BCF".ToColor()` — the library's string→`Color` helper.)*

**Gradient button via a reusable brush:**

```csharp
static LinearGradientBrush PrimaryGradient => new LinearGradientBrush()
    .StartPoint(new Point(0, 0))
    .EndPoint(new Point(1, 0))
    .GradientStops(new List<GradientStop>
    {
        new GradientStop(Colors.MediumPurple, 0f),
        new GradientStop(Colors.MediumSlateBlue, 1f)
    });

new Button().Text("Continue").Background(PrimaryGradient).TextColor(Colors.White)
```

**Theme-aware gradient** (via the property builder):

```csharp
new Border()
    .Background(e => e
        .OnLight(PrimaryGradient)
        .OnDark(new SolidColorBrush("#222233".ToColor())))
```

## Shadows

Closely related to backgrounds — every `VisualElement` supports a fluent `Shadow`:

```csharp
new Border()
    .Background(Colors.White)
    .Shadow(
        new Shadow()
            .Brush(Colors.Black)
            .Offset(new Point(0, 4))
            .Radius(12)
            .Opacity(0.25f)
    )
```

## Related Topics

- [Fluent Properties](fluent-properties.md) — theme/platform-aware brush values
- [Styling](styling.md) — putting brushes in app-wide styles
