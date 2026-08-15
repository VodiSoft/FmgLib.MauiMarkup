# FmgLib.MauiMarkup — API Cheatsheet

Dense map of the whole surface. Everything here follows from two rules: property `P` → `.P(...)`,
event `E` → `.OnE(...)`.

## Setup

```bash
dotnet add package FmgLib.MauiMarkup
```

```csharp
// GlobalUsings.cs
global using FmgLib.MauiMarkup;
```

Project template (optional):

```bash
dotnet new install FmgLib.MauiMarkup.Template
dotnet new fmglib-mauimarkup-app -o MyApp --netMajor 10 --includeContent false
```

`--netMajor` accepts `9` or `10` (default `10`). `--includeContent true` scaffolds sample pages.

No `MauiProgram` registration is required. `.UseMauiMarkupLocalization(...)` is the only opt-in call.

## Containers — what sets children

| Container | Method | Notes |
|---|---|---|
| `ContentPage`, `ContentView`, `Border`, `Frame`, `ScrollView`, `RefreshView` | `.Content(view)` | single child |
| `VerticalStackLayout`, `HorizontalStackLayout`, `StackLayout`, `Grid`, `FlexLayout`, `AbsoluteLayout` | `.Children(params IView[])` | `params`, so nesting mirrors the tree |
| `Shell`, `FlyoutItem`, `Tab`, `TabBar` | `.Items(...)` | see `mauimarkup-shell` |
| `CollectionView`, `CarouselView`, `ListView`, `Picker` | `.ItemsSource(...)` + `.ItemTemplate(() => view)` | see `mauimarkup-collections` |
| any layout | `.BindableLayoutItemsSource(...)` + `.BindableLayoutItemTemplate(...)` | no virtualization |
| `SwipeView` | `.Content(...)`, `.LeftItems(...)`, `.RightItems(...)` | |
| `Grid` with no definitions | `.Children(...)` | a single implicit cell = overlay container |

Adding children from a sequence — `Children` takes an array, so LINQ works:

```csharp
new VerticalStackLayout().Children(
    days.Select(d => (IView)new Label().Text(d)).ToArray())
```

Rule of thumb: **fixed data known at build time → LINQ; dynamic data → `ItemsSource` + template.**

## Multi-property shorthands

| Call | Expands to |
|---|---|
| `.SizeRequest(w, h)` / `.SizeRequest(size)` | `WidthRequest` + `HeightRequest` |
| `.Margin(all)` / `.Margin(h, v)` / `.Margin(l, t, r, b)` | `Margin` with a built `Thickness` |
| `.Padding(...)` (same overloads) | `Padding` |
| `.GridSpan(column: 2, row: 1)` | `Grid.ColumnSpan` + `Grid.RowSpan` |
| `.AbsoluteLayoutBounds(x, y, w, h)` | `AbsoluteLayout.LayoutBounds` without a `Rect` |
| `.Center()` / `.FillBothDirections()` / `.AlignTopRight()` … | `HorizontalOptions` + `VerticalOptions` |
| `.TextCenter()` / `.TextTopLeft()` … | `HorizontalTextAlignment` + `VerticalTextAlignment` |
| `await x.AnimateSizeRequestTo(w, h)` | width + height animated together |

String helper: `"#FF3366".ToColor()` (plus `ToColorFromArgb()`, `ToColorFromRgba()`).

## `BindableObject` helpers

| Method | Purpose |
|---|---|
| `.Assign(out var x)` | capture the control (the `x:Name` replacement) |
| `.InvokeOnElement(x => …)` | run arbitrary code mid-chain and keep chaining |
| `.RegisterName("id", root)` | MAUI name-scope interop (rarely needed) |
| `.BindingContext(obj)` | set `BindingContext`; also accepts a builder |
| `.Bind(BindableProperty, path, …)` | low-level binding for attached/dynamic properties |
| `.BindTemplatedParent(prop, path)` | `RelativeBindingSource.TemplatedParent` for `ControlTemplate`s |
| `.AppThemeBinding(prop, light, dark)` / `.AppThemeColorBinding(...)` | theme value on an arbitrary property |
| `.OnPropertyChanged(h)` / `.OnPropertyChanging(h)` / `.OnBindingContextChanged(h)` | fluent event subscription |
| `.Triggers(...)` | attach triggers to this instance |
| `.VisualStateGroups(new VisualStateGroupList { … })` | attach visual states |
| `.GestureRecognizers(...)` | tap / pan / pinch / swipe / pointer / drag-drop |
| `.Resources(new ResourceDictionary { … })` | scoped resources (app, page or layout level) |

## Gesture recognizers

```csharp
new Image()
    .Source("photo.png")
    .GestureRecognizers(
        new TapGestureRecognizer().NumberOfTapsRequired(2).OnTapped((s, e) => Zoom()),
        new PanGestureRecognizer().OnPanUpdated((s, e) => Move(e.TotalX, e.TotalY)),
        new PinchGestureRecognizer().OnPinchUpdated((s, e) => Scale(e.Scale)),
        new SwipeGestureRecognizer().Direction(SwipeDirection.Left).OnSwiped((s, e) => Next()))
```

## Common events

| Control | Methods |
|---|---|
| `Button` | `OnClicked`, `OnPressed`, `OnReleased` |
| `Entry` / `Editor` | `OnTextChanged`, `OnCompleted`, `OnFocused`, `OnUnfocused` |
| `CheckBox` / `Switch` / `RadioButton` | `OnCheckedChanged` / `OnToggled` / `OnCheckedChanged` |
| `Slider` | `OnValueChanged`, `OnDragStarted`, `OnDragCompleted` |
| `Picker` | `OnSelectedIndexChanged` |
| `CollectionView` | `OnSelectionChanged`, `OnScrolled`, `OnRemainingItemsThresholdReached` |
| `RefreshView` | `OnRefreshing` |
| `ContentPage` | `OnAppearing`, `OnDisappearing`, `OnLoaded`, `OnUnloaded`, `OnNavigatedTo`, `OnNavigatedFrom` |
| `WebView` | `OnNavigating`, `OnNavigated` |

## Shapes, brushes, shadows

```csharp
new Border()
    .StrokeShape(new RoundRectangle().CornerRadius(16))
    .Stroke(new SolidColorBrush(Colors.Gray))
    .StrokeThickness(1)
    .Background(new LinearGradientBrush()
        .StartPoint(new Point(0, 0)).EndPoint(new Point(1, 1))
        .GradientStops(
            new GradientStop().Color(Colors.Indigo).Offset(0.0f),
            new GradientStop().Color(Colors.Teal).Offset(1.0f)))
    .Shadow(new Shadow().Radius(10).Opacity(0.15f).Offset(new Point(0, 4)))
    .Content(/* … */)
```

Shape types (`Line`, `Rectangle`, `RoundRectangle`, `Ellipse`, `Polygon`, `Polyline`, `Path`) all take
their properties fluently; `.Clip(...)` accepts any `Geometry`.

## Formatted text

```csharp
new Label().FormattedText(
    new FormattedString().Spans(
        new Span().Text("Total: ").FontAttributes(FontAttributes.Bold),
        new Span().Text("$49.90").TextColor(Colors.Green),
        new Span().Text(" details")
            .TextDecorations(TextDecorations.Underline)
            .GestureRecognizers(new TapGestureRecognizer().OnTapped((s, e) => Open()))))
```

## Accessibility

```csharp
new Image().Source("logo.png").SemanticDescription("Company logo").SemanticHint("Decorative")
new Label().Text("Settings").SemanticHeadingLevel(SemanticHeadingLevel.Level1)
new Button().Text("?").ToolTipPropertiesText("Opens the help center").AutomationName("HelpButton")
```

## Animations

```csharp
await label.AnimateFontSizeTo(40, 300, Easing.CubicOut);
await box.AnimateBackgroundColorTo(Colors.Teal, 500);
await view.AnimateOpacityTo(0);
await Task.WhenAll(card.AnimateOpacityTo(1, 300), card.TranslateTo(0, 0, 300, Easing.CubicOut));
```

`AnimatePTo` exists for every interpolatable bindable property. MAUI's own `TranslateTo`, `FadeTo`,
`ScaleTo`, `RotateTo` and `new Animation(...)` still work unchanged. Animate transforms
(`TranslationX/Y`, `Scale`, `Rotation`, `Opacity`) rather than layout properties where possible.

## Custom extensions

Level 1 — composition shorthand (covers 95% of needs):

```csharp
public static T Card<T>(this T self) where T : Border
    => self.StrokeThickness(0).Padding(16)
           .BackgroundColor(e => e.OnLight(Colors.White).OnDark("#1E1E2E".ToColor()));
```

Keep the `<T>` generic + `where T :` constraint so the concrete type flows through the chain, and name
the method after the property (`CornerRadius(8)`, not `SetCornerRadius(8)`).

Level 2 — a full property method that also works in bindings and styles needs all four overloads:

```csharp
public static T FontSize<T>(this T self, double v) where T : Label
{ self.SetValue(Label.FontSizeProperty, v); return self; }

public static T FontSize<T>(this T self, Func<PropertyContext<double>, IPropertyBuilder<double>> configure)
    where T : Label
{ configure(new PropertyContext<double>(self, Label.FontSizeProperty)).Build(); return self; }

public static SettersContext<T> FontSize<T>(this SettersContext<T> self, double v) where T : Label
{ self.XamlSetters.Add(new Setter { Property = Label.FontSizeProperty, Value = v }); return self; }

public static SettersContext<T> FontSize<T>(this SettersContext<T> self,
    Func<PropertySettersContext<double>, IPropertySettersBuilder<double>> configure) where T : Label
{ configure(new PropertySettersContext<double>(self.XamlSetters, Label.FontSizeProperty)).Build(); return self; }
```

Always target the `BindableProperty` via `SetValue`, never the CLR property, or styles and triggers
stop working.

Level 3 — a custom animation:

```csharp
public static Task<bool> AnimateCornerRadiusTo<T>(this T self, int value,
    uint length = 250, Easing? easing = null) where T : Button
{
    double from = self.CornerRadius;
    var transform = (double t) => Transformations.DoubleTransform(from, value, t);
    var callback = (double v) => { self.CornerRadius = (int)v; };
    return Transformations.AnimateAsync<double>(self, "AnimateCornerRadiusTo", transform, callback, length, easing);
}
```

For a third-party control's own `BindableProperty`s, don't hand-write any of this — annotate the type
and let the generator do it (`mauimarkup-thirdparty`).
