# Text Alignment Helpers (`ITextAlignment`)

Every control implementing `ITextAlignment` — `Label`, `Entry`, `Editor`, `SearchBar`, `Picker`, `TimePicker`, `DatePicker`, `EntryCell`, and any third-party control implementing the interface — receives a family of extension methods that set `HorizontalTextAlignment` and `VerticalTextAlignment` in one call.

> These generated methods require the control to expose **both** text-alignment bindable properties; the source generator emits them only when both exist.

## Quick Example

```csharp
new Label().Text("Centered").TextCenter()
```

Centers the text both horizontally and vertically **within the control's own bounds**.

## Method Reference

### Single axis

| Method | Sets |
|---|---|
| `TextCenterHorizontal()` | `HorizontalTextAlignment = Center` |
| `TextCenterVertical()` | `VerticalTextAlignment = Center` |
| `TextLeft()` | `HorizontalTextAlignment = Start` |
| `TextRight()` | `HorizontalTextAlignment = End` |
| `TextTop()` | `VerticalTextAlignment = Start` |
| `TextBottom()` | `VerticalTextAlignment = End` |

### Both axes

| Method | Vertical | Horizontal |
|---|---|---|
| `TextCenter()` | Center | Center |
| `TextTopLeft()` | Start | Start |
| `TextTopCenter()` | Start | Center |
| `TextTopRight()` | Start | End |
| `TextCenterLeft()` | Center | Start |
| `TextCenterRight()` | Center | End |
| `TextBottomLeft()` | End | Start |
| `TextBottomCenter()` | End | Center |
| `TextBottomRight()` | End | End |

### General form

```csharp
new Label().AlignText(vertical: TextAlignment.End, horizontal: TextAlignment.Center)
```

## Text Alignment vs. Layout Options

A frequent point of confusion:

- **`TextCenter()`** aligns the *text inside the control's box*.
- **`Center()`** aligns the *control inside its parent* ([Layout Options](layout-options.md)).

If a `Label` is sized to its content, `TextCenter()` appears to do nothing — the box is exactly as big as the text. Give the control space, then align:

```csharp
// A full-width header with centered text
new Label()
    .Text("Dashboard")
    .FontSize(24)
    .FillHorizontal()   // control takes the full row
    .TextCenter()       // text centered inside it
    .HeightRequest(56)
```

## Practical Examples

**Numeric entry, right-aligned like a calculator:**

```csharp
new Entry()
    .Keyboard(Keyboard.Numeric)
    .Placeholder("0.00")
    .TextRight()
```

**Table-style rows:**

```csharp
new Grid()
.ColumnDefinitions(e => e.Star(6).Star(2).Star(2))
.Children(
    new Label().Text("Product"),
    new Label().Text("Qty").Column(1).TextCenter(),
    new Label().Text("Price").Column(2).TextRight()
)
```

**Vertically centered label in a fixed-height bar:**

```csharp
new Label()
    .Text("Status: online")
    .HeightRequest(44)
    .TextCenterVertical()
```

## Related Topics

- [Layout Options](layout-options.md)
- [Fluent Properties](fluent-properties.md)
