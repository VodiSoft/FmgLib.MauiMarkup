# Utility Extensions

Small helpers that don't belong to a single control but show up everywhere in FmgLib.MauiMarkup code.

## Color Helpers — `string` → `Color`

Hex strings convert directly, no `Color.FromArgb` noise:

```csharp
"#FF3366".ToColor()          // auto-detects; works with #RGB, #RRGGBB, #AARRGGBB
"#804C3BCF".ToColorFromArgb()  // explicit ARGB channel order
"#4C3BCF80".ToColorFromRgba()  // explicit RGBA channel order
```

```csharp
new Border().BackgroundColor("#1E1E2E".ToColor())
```

## `Assign` / `InvokeOnElement`

Covered in depth in [Object References](assign-and-references.md); listed here for completeness:

```csharp
new Entry().Assign(out var entry)                       // capture reference
new Entry().InvokeOnElement(e => e.Focus())             // arbitrary code mid-chain
```

## Collection Helpers

### `Add(configure)` — fluent chains inside collection initializers

Layouts and other `IEnumerable` bindable objects accept **lambdas in collection initializers**, letting you mix initializer syntax with fluent configuration:

```csharp
new VerticalStackLayout
{
    new Label().Text("Row 1"),
    l => l.Spacing(12).Padding(16),      // Action<T> — configures the layout itself
    new Label().Text("Row 2"),
}
```

### `AddRangeMarkup` — bulk-add to any collection property

Adds items to a collection-typed property (creating the collection if `null`), returning the owner for chaining:

```csharp
grid.AddRangeMarkup(g => g.Children, view1, view2, view3);
picker.AddRangeMarkup(p => p.Items, "Small", "Medium", "Large");
```

Useful in helper methods that append to existing trees rather than rebuilding them.

## Translation String Extensions

Code-side (non-binding) translations — see [Localization (JSON)](localization-json.md) / [RESX](localization-resx.md):

```csharp
"Hello".ToTranslate()             // JSON translator, current culture
"Hello".ToTranslate("tr-TR")      // explicit culture
"Hello".ToTranslateResx()         // RESX translator
"Hello".ToTranslateResx("tr-TR")
```

## Grid Definition Collections

The `Auto` / `Star` / `Absolute` builders exist both in the lambda form and as collection extensions — see [Grid](grid.md):

```csharp
new Grid().ColumnDefinitions(new ColumnDefinitionCollection().Absolute(100).Star())
```

## Style & VisualState Interop Helpers

For advanced scenarios that mix FmgLib styles with raw MAUI API:

```csharp
Style style = myFmgStyle;                              // Style<T> → Style (implicit)
var groups = style.GetVisualStateGroupList();          // access/extend VSM groups
var common = groups.GetCommonStatesVisualStateGroup(); // the "CommonStates" group
groups.Add(new VisualState<Button>("Custom", e => e.Opacity(0.5)));  // add a single state
```

`BindableProperty.Set(value)` creates a `Setter` — used inside style/trigger initializers ([Triggers](triggers.md)):

```csharp
Entry.TextColorProperty.Set(Colors.White)
```

## `RegisterName` — MAUI name scopes

```csharp
new Label().RegisterName("title", pageRoot)
```

Details in [Object References](assign-and-references.md#registername--the-literal-xname-equivalent).

## Related Topics

- [Assign & References](assign-and-references.md)
- [Grid](grid.md)
- [Styling](styling.md)
- [Localization (JSON)](localization-json.md)
