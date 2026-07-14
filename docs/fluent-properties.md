# Fluent Properties

Every bindable property of every control gets a family of fluent extension methods. Understanding this family — it is always the same four shapes — unlocks the whole library, because the pattern repeats for all controls, including [third-party controls](third-party-controls.md) processed by the source generator.

## The Four Overload Shapes

For a property like `Label.FontSize`, the library provides:

```csharp
// 1. Direct value — the one you use 90% of the time
public static T FontSize<T>(this T self, double fontSize) where T : Label;

// 2. Property builder — bindings, theme/platform/idiom values, dynamic resources
public static T FontSize<T>(this T self,
    Func<PropertyContext<double>, IPropertyBuilder<double>> configure) where T : Label;

// 3. Style setter — used inside Style<T> definitions
public static SettersContext<T> FontSize<T>(this SettersContext<T> self, double fontSize) where T : Label;

// 4. Style setter with builder — bindings etc. inside styles
public static SettersContext<T> FontSize<T>(this SettersContext<T> self,
    Func<PropertySettersContext<double>, IPropertySettersBuilder<double>> configure) where T : Label;
```

Additionally, properties of type `double`, `Color`, `Thickness`, `Rect`, etc. get an **animation helper** (see [Animations](animations.md)):

```csharp
Task<bool> AnimateFontSizeTo(double value, uint length = 250, Easing? easing = null);
```

You never call shapes 3–4 directly; they light up automatically when you write property calls inside `new Style<T>(e => e...)` — the same method names work in both contexts.

## Shape 1 — Direct Values

```csharp
new Label()
    .Text("This is a test")
    .Padding(20)
    .FontSize(30)
    .Center()
```

Because all methods are generic over `T` and return `T`, chaining preserves the concrete control type: after `.Text(...)` you still have a `Label`, so `Label`-only methods remain available.

## Shape 2 — The Property Builder (`e => e…`)

The lambda overload receives a `PropertyContext<TValue>` (conventionally named `e`) and is the gateway to everything that is *not* a constant value:

### Bindings

```csharp
new Label().Text(e => e.Path("UserName"));                       // bind to BindingContext.UserName
new Label().Text(e => e.Path("Value").Source(slider));           // bind to another control
new Label().Text(e => e.Path("Price").StringFormat("{0:C}"));    // format
new Entry().Text(e => e.Path("Query").BindingMode(BindingMode.TwoWay));
```

Full binding capabilities (converters, fallback values, multi-binding, compiled getter bindings) are documented in [Property Bindings](data-binding.md), [Binding Converters](binding-converters.md), [MultiBinding](multi-binding.md) and [Compiled Bindings](compiled-bindings.md).

### App theme values (`{AppThemeBinding}`)

```csharp
new Label()
    .TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.Teal))
```

| Method | Applies when |
|---|---|
| `OnLight(value)` | App theme is Light |
| `OnDark(value)` | App theme is Dark |
| `Default(value)` | Fallback / unspecified theme |

The value updates live when the user switches the OS theme — this is a real `AppThemeBinding`, not a one-time check.

### Device idiom values (`{OnIdiom}`)

```csharp
new Label()
    .FontSize(e => e.OnDesktop(80).OnPhone(30).Default(50))
```

Available: `OnPhone`, `OnTablet`, `OnDesktop`, `OnTV`, `OnWatch`, `Default`.

### Platform values (`{OnPlatform}`)

```csharp
new Label()
    .Margin(e => e.OniOS(new Thickness(0, 20, 0, 0)).Default(new Thickness(0)))
```

Available: `OnAndroid`, `OniOS`, `OnMacCatalyst`, `OnWinUI`, `OnTizen`, `Default`.

### Dynamic resources (`{DynamicResource}`)

```csharp
new Frame()
    .BackgroundColor(e => e.DynamicResource("CardBackground"))
```

The property tracks the resource key: replace `Resources["CardBackground"]` at runtime and the UI updates.

### Localization

```csharp
new Label().Text(e => e.Translate("Hello"));        // JSON localization
new Label().Text(e => e.TranslateResx("Hello"));    // RESX localization
```

These are live bindings too — calling `Translator.Instance.ChangeCulture(...)` re-translates every bound property instantly. See [Localization (JSON)](localization-json.md) and [Localization (RESX)](localization-resx.md).

### Nesting builders

Idiom/theme/platform builders compose: each branch can itself take a builder instead of a raw value.

```csharp
new Label()
    .TextColor(e => e
        .OnLight(Colors.Black)
        .OnDark(l => l.DynamicResource("DarkAccent")))   // dark theme uses a dynamic resource
```

## Shapes 3–4 — Inside Styles

The exact same method names work inside a `Style<T>` definition; there they produce XAML `Setter`s instead of touching a live control:

```csharp
new Style<Button>(e => e
    .TextColor(e => e.OnLight(Colors.White).OnDark(AppColors.Primary))
    .BackgroundColor(AppColors.Primary)
    .FontSize(14)
    .CornerRadius(8)
    .Padding(new Thickness(14, 10)))
```

See [Styling](styling.md) for the complete story.

## Convenience Shorthands Worth Knowing

Beyond 1:1 property mapping, the library adds combined setters:

| Method | Expands to |
|---|---|
| `.SizeRequest(w, h)` / `.SizeRequest(size)` | `WidthRequest` + `HeightRequest` |
| `.Margin(10)` / `.Margin(h, v)` / `.Margin(l, t, r, b)` | `Margin` with a built `Thickness` |
| `.Padding(...)` (same overloads) | `Padding` |
| `.Center()`, `.FillHorizontal()`, `.AlignTopRight()`, … | `HorizontalOptions`/`VerticalOptions` combos — see [Layout Options](layout-options.md) |
| `.TextCenter()`, `.TextTopLeft()`, … | `HorizontalTextAlignment`/`VerticalTextAlignment` combos — see [Text Alignment](text-alignment.md) |
| `.GridSpan(column: 2, row: 1)` | `Grid.ColumnSpan` + `Grid.RowSpan` |
| `.AbsoluteLayoutBounds(x, y, w, h)` | `AbsoluteLayout.LayoutBounds` without constructing a `Rect` |

There are also small utility extensions on plain types:

```csharp
"#FF3366".ToColor()          // string → Color (hex, with ToColorFromArgb / ToColorFromRgba variants)
```

## Setting Anything Not Covered

Two escape hatches guarantee you are never blocked:

**`InvokeOnElement`** — run arbitrary code against the control mid-chain:

```csharp
new Entry()
    .Placeholder("Name")
    .InvokeOnElement(entry => entry.ReturnCommand = someCommand)
```

**Plain C#** — since views are just objects, you can always assign afterwards:

```csharp
var entry = new Entry().Placeholder("Name");
entry.ReturnType = ReturnType.Done;
```

## Related Topics

- [Property Bindings](data-binding.md) — the builder's binding features in depth
- [Styling](styling.md) — the setters context
- [Custom Extension Methods](custom-extension-methods.md) — add the same four shapes to your own properties
