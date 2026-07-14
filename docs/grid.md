# Grid Definition

`Grid` is the workhorse layout of MAUI. FmgLib.MauiMarkup makes its two verbose parts — row/column definitions and child placement — concise and type-safe.

## Row & Column Definitions

`RowDefinitions` / `ColumnDefinitions` accept a builder lambda with three methods:

| Builder method | Meaning | XAML equivalent |
|---|---|---|
| `Star(double value = 1.0, int count = 1)` | Proportional share of remaining space | `2*`, `*` |
| `Auto(int count = 1)` | Size to content | `Auto` |
| `Absolute(double value, int count = 1)` | Fixed device-independent units | `100` |

The optional `count` parameter repeats the definition, which removes copy-paste for uniform grids.

```csharp
new Grid()
.RowDefinitions(e => e.Star(2).Star(0.5, count: 3))
.ColumnDefinitions(e => e.Absolute(100).Star())
.Children(
    // ...
)
```

This creates:

- **4 rows** — the first takes 2 “stars” (twice the space of a 1-star row); rows 2–4 take 0.5 stars each.
- **2 columns** — a fixed 100-unit column, then a column absorbing all remaining width.

Equivalent XAML for comparison:

```xml
<Grid RowDefinitions="2*,0.5*,0.5*,0.5*" ColumnDefinitions="100,*">
```

Mixed example:

```csharp
new Grid()
.RowDefinitions(e => e.Auto().Star().Auto())        // header / content / footer
.ColumnDefinitions(e => e.Star(3).Star(7))          // 30% / 70%
```

## Placing Children

Attached-property helpers position children (see [Attached Properties](attached-properties.md)):

| Method | XAML equivalent |
|---|---|
| `.Row(int)` | `Grid.Row` |
| `.Column(int)` | `Grid.Column` |
| `.RowSpan(int)` | `Grid.RowSpan` |
| `.ColumnSpan(int)` | `Grid.ColumnSpan` |
| `.GridSpan(column, row)` | both spans at once |

Row and column default to 0, so the first cell needs no calls.

## Complete Example

A classic 2×2 colored grid:

```csharp
new Grid()
.RowDefinitions(e => e.Star(2).Star())
.ColumnDefinitions(e => e.Absolute(200).Star())
.Children(
    new BoxView().Color(Colors.Green),
    new Label().Text("Column 0, Row 0"),

    new BoxView().Color(Colors.Blue).Column(1).Row(0),
    new Label().Text("Column 1, Row 0").Column(1).Row(0),

    new BoxView().Color(Colors.Teal).Column(0).Row(1),
    new Label().Text("Column 0, Row 1").Column(0).Row(1),

    new BoxView().Color(Colors.Purple).Column(1).Row(1),
    new Label().Text("Column 1, Row 1").Column(1).Row(1)
)
```

Multiple children may share a cell (they stack in z-order; later children draw on top). Use `.ZIndex(int)` to override stacking.

## Real-World Layout — a product card

```csharp
new Grid()
.RowDefinitions(e => e.Star(1).Star(6).Star(2).Star(1))
.Padding(5)
.Children(
    // row 0: favorite icon + discount badge
    new Grid()
    .Row(0)
    .ColumnDefinitions(e => e.Star(6).Star(4))
    .Children(
        new ImageButton().Source("heart.png").AlignLeft().SizeRequest(30, 30),
        new Frame().Column(1).CornerRadius(20).BackgroundColor(Colors.Red)
            .Content(new Label().Text("-50%").TextColor(Colors.White).Center())
    ),

    // row 1: product image
    new Image().Source("product.png").SizeRequest(80, 80).Row(1).CenterHorizontal(),

    // row 2: name + price
    new VerticalStackLayout().Row(2).Children(
        new Label().Text("Sourdough Bread").FontAttributes(FontAttributes.Bold),
        new Label().Text("$4.90").TextColor(Colors.Green)
    ),

    // row 3: action
    new Button().Row(3).Text("Add to cart").HeightRequest(35)
)
```

## Spacing and Sizing

All regular `Grid` properties are available fluently:

```csharp
new Grid()
.RowSpacing(8)
.ColumnSpacing(8)
.Padding(16)
.BackgroundColor(Colors.WhiteSmoke)
```

## Alternative: Collection-Based Definitions

If you already have definition collections (or prefer explicit objects), overloads accept them directly, and the same `Auto/Star/Absolute` helpers exist as extensions on `ColumnDefinitionCollection` / `RowDefinitionCollection`:

```csharp
new Grid()
.ColumnDefinitions(new ColumnDefinitionCollection().Absolute(100).Star())
```

## Tips

- **Prefer `count:` for uniform grids:** `e => e.Star(1, count: 7)` creates a 7-row calendar strip in one call.
- **`Auto` rows measure content each pass** — use them for headers/footers, not for long lists (put a `CollectionView` in a `Star` row instead).
- **Children placed without definitions** all land in the single implicit cell — a `Grid` with no definitions is a convenient overlay container:

  ```csharp
  new Grid().Children(
      new Image().Source("photo.png"),
      new Label().Text("Caption").AlignBottomCenter().TextColor(Colors.White)
  )
  ```

## Related Topics

- [Layout Options](layout-options.md) — positioning children within cells
- [Attached Properties](attached-properties.md)
