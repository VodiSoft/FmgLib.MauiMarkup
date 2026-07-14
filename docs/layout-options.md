# Layout Options

Setting `HorizontalOptions` and `VerticalOptions` is one of the most repetitive tasks in MAUI. FmgLib.MauiMarkup collapses every combination into a single, readable method on any `View`.

## Quick Example

```csharp
new StackLayout()
.Children(
    new Label().Text("Hello, World!").Center()
)
```

`Center()` sets both `HorizontalOptions` and `VerticalOptions` to `LayoutOptions.Center`.

## Full Method Reference

Think of the names as a 4×4 grid of `(vertical, horizontal)` values, with shortcuts for the common cases.

### Single-axis helpers

| Method | Sets |
|---|---|
| `CenterHorizontal()` | `HorizontalOptions = Center` |
| `CenterVertical()` | `VerticalOptions = Center` |
| `Center()` | both `Center` |
| `AlignLeft()` | `HorizontalOptions = Start` |
| `AlignRight()` | `HorizontalOptions = End` |
| `AlignTop()` | `VerticalOptions = Start` |
| `AlignBottom()` | `VerticalOptions = End` |
| `FillHorizontal()` | `HorizontalOptions = Fill` |
| `FillVertical()` | `VerticalOptions = Fill` |
| `FillBothDirections()` | both `Fill` |

### Two-axis combinations

| Method | Vertical | Horizontal |
|---|---|---|
| `AlignTopLeft()` | Start | Start |
| `AlignTopCenter()` | Start | Center |
| `AlignTopRight()` | Start | End |
| `AlignTopFill()` | Start | Fill |
| `AlignCenterLeft()` | Center | Start |
| `AlignCenterRight()` | Center | End |
| `AlignCenterFill()` | Center | Fill |
| `AlignBottomLeft()` | End | Start |
| `AlignBottomCenter()` | End | Center |
| `AlignBottomRight()` | End | End |
| `AlignBottomFill()` | End | Fill |
| `AlignFillLeft()` | Fill | Start |
| `AlignFillCenter()` | Fill | Center |
| `AlignFillRight()` | Fill | End |

### The general form

Any combination not covered above (or computed at runtime):

```csharp
new Label().AlignLayout(vertical: LayoutOptions.End, horizontal: LayoutOptions.Center)
```

You can always fall back to the raw properties too:

```csharp
new Label()
    .HorizontalOptions(LayoutOptions.Start)
    .VerticalOptions(LayoutOptions.Fill)
```

## Worked Example

A footer bar pinned to the bottom, its content centered, with a right-aligned settings icon:

```csharp
new Grid()
.Children(
    new Label()
        .Text("Ready.")
        .AlignBottomCenter(),

    new ImageButton()
        .Source("gear.png")
        .SizeRequest(28, 28)
        .AlignBottomRight()
        .Margin(0, 0, 12, 12)
)
```

A hero card centered on the page:

```csharp
this.Content(
    new Border()
        .SizeRequest(300, 180)
        .Center()                    // centered in the page
        .Content(
            new VerticalStackLayout()
            .Center()                // stack centered in the border
            .Children(
                new Label().Text("Welcome").FontSize(28).CenterHorizontal(),
                new Label().Text("Sign in to continue").CenterHorizontal()
            )
        )
);
```

## Notes & Gotchas

- **Layout options vs. text alignment.** `Center()` positions the *view within its parent*. To center the *text inside a label*, use `TextCenter()` — see [Text Alignment](text-alignment.md). They are frequently combined:

  ```csharp
  new Label().Text("Title").FillHorizontal().TextCenter()
  ```

- **Grid children default to Fill.** Inside a `Grid`, a child fills its cell unless you set options — use these helpers to position within the cell.
- **`Expand` options are obsolete in MAUI.** The library intentionally exposes only `Start/Center/End/Fill`; use `Grid` star sizing or `FlexLayout` grow factors for expansion behavior.
- These helpers apply to **any `View`** — layouts included (`new VerticalStackLayout().Center()` centers the whole stack).

## Related Topics

- [Text Alignment](text-alignment.md)
- [Grid](grid.md)
- [Fluent Properties](fluent-properties.md)
