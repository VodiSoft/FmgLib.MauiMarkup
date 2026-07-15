# Gradyanlar ve Fırçalar

`Brush` tipindeki her özellik — en başta `VisualElement.Background` — düz renkleri, doğrusal ve dairesel gradyanları kabul eder; hepsi fluent kurulabilir.

## LinearGradientBrush

`StartPoint` → `EndPoint` ekseninde bir gradyan (oransal koordinatlar: `(0,0)` = sol üst, `(1,1)` = sağ alt).

```csharp
new Border()
.Background(
    new LinearGradientBrush()
    .StartPoint(new Point(0, 0))
    .EndPoint(new Point(1, 1))          // sol üst → sağ alt çapraz
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

Her `GradientStop`, ekseni boyunca bir rengi ofsetle (0–1) eşler. Stop'lar fluent de kurulabilir:

```csharp
new GradientStop().Color(Colors.Yellow).Offset(0.0f)
```

Yaygın yönler:

| Yön | StartPoint | EndPoint |
|---|---|---|
| Yatay (sol→sağ) | `(0, 0)` | `(1, 0)` |
| Dikey (üst→alt) | `(0, 0)` | `(0, 1)` |
| Çapraz | `(0, 0)` | `(1, 1)` |

## RadialGradientBrush

`Center`'dan dışa, `Radius`'a kadar yayılır (ikisi de oransal):

```csharp
new BoxView()
.Background(
    new RadialGradientBrush()
    .Center(new Point(0.5, 0.5))
    .Radius(0.6)
    .GradientStops(
        new List<GradientStop>
        {
            new GradientStop(Colors.White,        0.0f),
            new GradientStop(Colors.SkyBlue,      0.6f),
            new GradientStop(Colors.MidnightBlue, 1.0f)
        }
    )
)
```

## SolidColorBrush

Bütünlük için — `Background` düz fırça ve renkleri de alır:

```csharp
new Border().Background(new SolidColorBrush(Colors.Coral))
new Border().Background(Colors.Coral)          // örtük dönüşüm
new Border().BackgroundColor(Colors.Coral)     // Color özelliği
```

> İkisi de ayarlanmışsa `Background` (Brush), `BackgroundColor`'ın (Color) üzerine çizer.

## Pratik Tarifler

**Gradyanlı sayfa başlığı:**

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
    new Label().Text("Tekrar hoş geldiniz").FontSize(28).TextColor(Colors.White).AlignBottomLeft().Margin(20)
)
```

*(`"#4C3BCF".ToColor()` — kütüphanenin string→`Color` yardımcısı.)*

**Yeniden kullanılabilir fırçayla gradyanlı buton:**

```csharp
static LinearGradientBrush PrimaryGradient => new LinearGradientBrush()
    .StartPoint(new Point(0, 0))
    .EndPoint(new Point(1, 0))
    .GradientStops(new List<GradientStop>
    {
        new GradientStop(Colors.MediumPurple, 0f),
        new GradientStop(Colors.MediumSlateBlue, 1f)
    });

new Button().Text("Devam").Background(PrimaryGradient).TextColor(Colors.White)
```

**Temaya duyarlı gradyan** (property builder ile):

```csharp
new Border()
    .Background(e => e
        .OnLight(PrimaryGradient)
        .OnDark(new SolidColorBrush("#222233".ToColor())))
```

## Gölgeler

Arka planlarla yakından ilişkili — her `VisualElement` fluent `Shadow` destekler:

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

## İlgili Konular

- [Fluent Özellikler](fluent-properties.md) — tema/platform duyarlı fırça değerleri
- [Stiller](styling.md) — fırçaları uygulama geneli stillere koymak
