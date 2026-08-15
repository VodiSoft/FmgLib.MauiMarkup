# FmgLib.MauiMarkup — Bindings Reference

Every fluent property method accepts a **builder lambda** (`e => e…`). The lambda must *return* the
chain — `e => e.Path("X")`, never `e => { e.Path("X"); }`.

## Builder methods

| Method | XAML equivalent | Notes |
|---|---|---|
| `.Path(string)` | `Path=` | `"."` binds to the source itself; `"A.B"`, `"Items[0]"`, `"Text.Length"` all work |
| `.Getter(static (VM vm) => vm.X)` | compiled binding | preferred for view-model paths |
| `.Setter(static (VM vm, T v) => vm.X = v)` | — | required for `TwoWay`/`OneWayToSource` compiled bindings |
| `.Source(object)` | `Source=` | defaults to the inherited `BindingContext` |
| `.BindingMode(BindingMode)` | `Mode=` | `OneWay`, `TwoWay`, `OneTime`, `OneWayToSource`, `Default` |
| `.StringFormat(string)` | `StringFormat=` | |
| `.Converter(IValueConverter)` | `Converter=` | |
| `.Parameter(object)` | `ConverterParameter=` | |
| `.Convert<Q,R>(Func<Q,R>)` | — | inline converter, types inferred from the lambda |
| `.ConvertBack<R,Q>(Func<R,Q>)` | — | inline reverse converter |
| `.FallbackValue(object)` | `FallbackValue=` | used when the path fails to resolve |
| `.TargetNullValue(object)` | `TargetNullValue=` | used when the value resolves to `null` |
| `.Bindings(params BindingBase[])` | `MultiBinding` | ready-made child bindings |
| `.MultiConvert(...)` | multi-value converter | closes a multi-binding built from several `Path`/`Getter` calls |
| `.OnLight/.OnDark/.Default` | `AppThemeBinding` | live — follows OS and `UserAppTheme` changes |
| `.OnPhone/.OnTablet/.OnDesktop/.OnTV/.OnWatch/.Default` | `OnIdiom` | |
| `.OnAndroid/.OniOS/.OnMacCatalyst/.OnWinUI/.OnTizen/.Default` | `OnPlatform` | |
| `.DynamicResource(string)` | `DynamicResource` | tracks later `Resources[key]` replacement |
| `.Translate(key)` / `.TranslateFormat(key, paths…)` | — | JSON localization, live |
| `.TranslateResx(key)` / `.TranslateResxFormat(key, paths…)` | — | RESX localization, live |
| `.FromCulture()` | — | on `FlowDirection`: RTL follows the selected culture |

## String paths vs. compiled bindings

Prefer `Getter` for every view-model path: no reflection at runtime, wrong paths become compile
errors, and renames follow automatically.

```csharp
// string path — resolved by reflection, typo fails silently
new Label().Text(e => e.Path("UserName"));

// compiled — checked at compile time
new Label().Text(e => e.Getter(static (ProfileViewModel vm) => vm.UserName));
```

Mark the lambda `static`: it prevents accidental closure capture and states the intent.

Two-way needs the reverse operation:

```csharp
new Entry().Text(e => e
    .Getter(static (ProfileViewModel vm) => vm.Name)
    .Setter(static (ProfileViewModel vm, string v) => vm.Name = v)
    .BindingMode(BindingMode.TwoWay))
```

### What a `Getter` expression may contain

Valid — simple property access, null-conditional chains, indexers, casts:

```csharp
static (VM vm) => vm.Name
static (VM vm) => vm.Address?.Street
static (VM vm) => vm.PhoneNumbers[0]
static (VM vm) => vm.Config["Font"]
static (Label l) => ((VM)l.BindingContext).Name
```

Invalid — method calls, string concatenation, interpolation, arithmetic:

```csharp
static (VM vm) => vm.GetAddress()                 // ✗
static (VM vm) => $"Name: {vm.Name}"              // ✗
static (VM vm) => vm.First + " " + vm.Last        // ✗
```

Keep the getter simple and add `.Convert(...)`, or expose a computed property on the view model.

### Migration cheat sheet

| String binding | Compiled binding |
|---|---|
| `e.Path("Name")` | `e.Getter(static (VM vm) => vm.Name)` |
| `e.Path("Address.City")` | `e.Getter(static (VM vm) => vm.Address.City)` |
| `e.Path("Text").Source(entry)` | `e.Getter(static (Entry x) => x.Text).Source(entry)` |
| `e.Path("Name").BindingMode(TwoWay)` | add `.Setter(static (VM vm, string v) => vm.Name = v)` |
| `new Binding().Path("X")` inside `Bindings(...)` | `Binding.Create(static (VM vm) => vm.X)` |

## Control-to-control binding

```csharp
new Slider().Assign(out var slider).Minimum(0).Maximum(100),
new Label().Text(e => e.Path("Value").Source(slider).StringFormat("Value: {0:F0}"))
```

## Converters

Inline is the default choice — no converter class:

```csharp
new Label().TextColor(e => e.Path("IsError").Convert((bool err) => err ? Colors.Red : Colors.Black))
new Label().IsVisible(e => e.Path("Items.Count").Convert((int c) => c == 0))
new Image().Source(e => e.Path("Status").Convert((OrderStatus s) => s switch
{
    OrderStatus.Shipped   => "truck.png",
    OrderStatus.Delivered => "check.png",
    _                     => "clock.png"
}))
```

Two-way with parsing:

```csharp
new Entry().Text(e => e
    .Path("Price")
    .BindingMode(BindingMode.TwoWay)
    .Convert((decimal p) => p.ToString("F2"))
    .ConvertBack((string s) => decimal.TryParse(s, out var d) ? d : 0m))
```

Classic converters plug in with `.Converter(...)` / `.Parameter(...)`. Share stateless converters
through a `static readonly` field instead of allocating one per binding.

**Often the best converter is no converter:** because the UI is C#, a computed view-model property
(`public Color StatusColor => …`) is simpler and unit-testable.

## Multi-binding

Call `Path`/`Getter` more than once, then close with `MultiConvert`:

```csharp
new Label().Text(e => e
    .Getter(static (PersonViewModel vm) => vm.Name)
    .Getter(static (PersonViewModel vm) => vm.Age)
    .MultiConvert((string name, int age) => $"{name} ({age})"))
```

Compiled and string-path sub-bindings can be mixed; a `Setter` belongs to the `Getter` it follows.

With ready-made bindings and an `IMultiValueConverter`:

```csharp
new CheckBox().IsChecked(e => e
    .Bindings(
        Binding.Create(static (MainPageViewModel m) => m.IsOver16),
        Binding.Create(static (MainPageViewModel m) => m.HasPassedTest),
        Binding.Create(static (MainPageViewModel m) => m.IsSuspended))
    .Converter(new AllTrueMultiConverter())
    .FallbackValue("Is Error.")
    .TargetNullValue("Is Null."))
```

`.Bindings(...)` plus `.StringFormat("{0} : {1} : {2}")` covers the display-only case with no
converter at all.

## The low-level `Bind()` API

For attached properties, helper methods, or a `BindableProperty` you hold in a variable:

```csharp
public static T Bind<T>(this T self,
    BindableProperty targetProperty,
    string path = ".",
    BindingMode mode = BindingMode.Default,
    IValueConverter? converter = null,
    object? converterParameter = null,
    string? stringFormat = null,
    object? source = null,
    object? targetNullValue = null,
    object? fallbackValue = null) where T : BindableObject;
```

Generic overloads take conversion funcs instead of converter classes, and 2–4 source overloads combine
bindings with a value-tuple converter:

```csharp
new Label().Bind<Label, bool, Color>(Label.TextColorProperty, "IsError",
    convert: isError => isError ? Colors.Red : Colors.Black)

new Label().Bind<Label, string, string, string>(Label.TextProperty,
    new Binding("FirstName"), new Binding("LastName"),
    convert: n => $"{n.Item1} {n.Item2}")
```

Prefer the builder syntax; reach for `Bind()` only when the target is an attached property or you are
writing a reusable helper.

## Bindings inside templates

The template's `BindingContext` is the item:

```csharp
.ItemTemplate(() => new Label().Text(e => e.Getter(static (ProductVM p) => p.Name)))
```

To reach the **page's** view model from inside a template, capture it and pass the reference directly —
no `RelativeSource` gymnastics needed, because this is C#:

```csharp
public void Build()
{
    var vm = (CatalogViewModel)BindingContext;

    this.Content(new CollectionView()
        .ItemsSource(vm.Products)
        .ItemTemplate(() => new Button()
            .Text("Add to cart")
            .Command(vm.AddToCartCommand)                 // page VM, captured
            .Bind(Button.CommandParameterProperty, ".")   // the item itself
        ));
}
```

## When not to bind

A value that never changes is a plain `SetValue` — `.Text(vm.Title)` costs nothing and `.Text(e =>
e.Path("Title"))` sets up a binding for no reason. Bind what changes; assign what doesn't.
