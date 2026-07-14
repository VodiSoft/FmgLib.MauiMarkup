# Property Bindings

FmgLib.MauiMarkup exposes MAUI data binding through the **property builder lambda** (`e => e…`) available on every fluent property method, plus a lower-level `Bind()` extension for full control. When the bound source changes, the property updates — exactly like `{Binding}` in XAML.

## The Builder Syntax

Any property method accepts a lambda that configures a binding:

```csharp
new Label().Text(e => e.Path("UserName"));
```

### Builder methods

| Method | XAML equivalent | Description |
|---|---|---|
| `.Path(string)` | `Path=` | Property path on the source. `"."` binds to the source itself. |
| `.Source(object)` | `Source=` | Binding source; defaults to the control's `BindingContext`. |
| `.BindingMode(mode)` | `Mode=` | `OneWay`, `TwoWay`, `OneTime`, `OneWayToSource`, `Default`. |
| `.StringFormat(string)` | `StringFormat=` | Format string applied to the result. |
| `.Converter(IValueConverter)` | `Converter=` | Classic converter instance. |
| `.Parameter(object)` | `ConverterParameter=` | Passed to the converter. |
| `.Convert<Q>(Func<Q,T>)` | — | Inline conversion function (no converter class needed). |
| `.ConvertBack<Q>(Func<T,Q>)` | — | Inline reverse conversion for two-way bindings. |
| `.FallbackValue(object)` | `FallbackValue=` | Used when the binding fails to resolve. |
| `.TargetNullValue(object)` | `TargetNullValue=` | Used when the resolved value is `null`. |
| `.Getter(...)` / `.Setter(...)` | compiled binding | Expression-based bindings — see [Compiled Bindings](compiled-bindings.md). |
| `.Bindings(...)` | `MultiBinding` | Multiple sources — see [MultiBinding](multi-binding.md). |

## Binding to the `BindingContext` (MVVM)

The default source is the inherited `BindingContext`, so a view-model-bound page reads naturally:

```csharp
public class ProfileViewModel : INotifyPropertyChanged
{
    public string UserName { get; set; } = "fmg";
    public string Email { get; set; } = "user@example.com";
    // ... raise PropertyChanged as usual
}

public partial class ProfilePage : ContentPage, IFmgLibHotReload
{
    public ProfilePage() => this.InitializeHotReload();

    public void Build() =>
        this
        .BindingContext(new ProfileViewModel())
        .Content(
            new VerticalStackLayout()
            .Padding(20)
            .Children(
                new Label().Text(e => e.Path("UserName")).FontSize(28),
                new Label().Text(e => e.Path("Email")).TextColor(Colors.Gray),
                new Entry().Text(e => e.Path("UserName").BindingMode(BindingMode.TwoWay))
            )
        );
}
```

Nested paths, indexers and the self path all work as in XAML:

```csharp
.Text(e => e.Path("Address.City"))
.Text(e => e.Path("PhoneNumbers[0]"))
.Text(e => e.Path("."))                 // the item itself (common in templates)
.Text(e => e.Path("Text.Length"))       // sub-properties of properties
```

## Binding Control-to-Control

Capture the source with [`Assign`](assign-and-references.md) and pass it to `.Source(...)`:

```csharp
new Slider().Assign(out var slider).Minimum(0).Maximum(100),

new Label()
    .Text(e => e.Path("Value").Source(slider).StringFormat("Value: {0:F0}"))
```

Two-way example — an `Entry` and a `Label` kept in sync:

```csharp
new Entry().Assign(out var input).Placeholder("Type something"),
new Label().Text(e => e.Path("Text").Source(input))
```

## `FallbackValue` and `TargetNullValue`

```csharp
new Label()
    .Text(e => e
        .Path("Employee.Name")
        .FallbackValue("(binding error)")   // path could not be resolved
        .TargetNullValue("(no name)"))      // path resolved, value is null
```

## The Low-Level `Bind()` API

Every `BindableObject` also gets `Bind(...)` extension overloads for cases where you have a `BindableProperty` in hand (identical in spirit to community-toolkit style binding). This is useful for attached properties or when writing helpers:

```csharp
new SearchBar()
    .Assign(out var search)
    .SearchCommand(vm.SearchCommand)
    .Bind(SearchBar.SearchCommandParameterProperty, "Text", source: search)
```

Full signature of the basic overload:

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

Generic overloads accept **inline conversion functions** instead of converter classes:

```csharp
new Label()
    .Bind<Label, bool, Color>(Label.TextColorProperty, "IsError",
        convert: isError => isError ? Colors.Red : Colors.Black)
```

And multi-source overloads combine 2–4 bindings with a value-tuple converter (see [MultiBinding](multi-binding.md)):

```csharp
new Label()
    .Bind<Label, string, string, string>(Label.TextProperty,
        new Binding("FirstName"),
        new Binding("LastName"),
        convert: names => $"{names.Item1} {names.Item2}")
```

### Other `BindableObject` helpers

| Method | Purpose |
|---|---|
| `.BindingContext(object)` | Sets `BindingContext` fluently (also accepts a builder for binding the context itself). |
| `.BindTemplatedParent(prop, path)` | Binding with `RelativeBindingSource.TemplatedParent` — for `ControlTemplate` content. |
| `.AppThemeBinding(prop, light, dark)` | Theme-dependent value for an arbitrary `BindableProperty`. |
| `.AppThemeColorBinding(prop, light, dark)` | Same, specialized for `Color`. |
| `.OnPropertyChanged(handler)` | Subscribes to `PropertyChanged` fluently. |
| `.OnPropertyChanging(handler)` | Subscribes to `PropertyChanging`. |
| `.OnBindingContextChanged(handler)` | Subscribes to `BindingContextChanged`. |

Example — react to any property change:

```csharp
new Entry()
    .OnPropertyChanged(entry => Console.WriteLine($"Something changed on {entry}"))
```

## Binding in Item Templates

Inside an `ItemTemplate`, the `BindingContext` of each realized view is the item, so plain `Path` calls target item properties:

```csharp
new CollectionView()
.ItemsSource(e => e.Path("Products"))
.ItemTemplate(() =>
    new HorizontalStackLayout().Spacing(8).Children(
        new Label().Text(e => e.Path("Name")),
        new Label().Text(e => e.Path("Price").StringFormat("{0:C}"))
    )
)
```

Binding to the **page's view model from inside a template** uses `Source`:

```csharp
public void Build()
{
    var vm = (CatalogViewModel)BindingContext;

    this.Content(
        new CollectionView()
        .ItemsSource(vm.Products)
        .ItemTemplate(() =>
            new Button()
                .Text("Add to cart")
                .Command(vm.AddToCartCommand)              // page VM — direct reference
                .Bind(Button.CommandParameterProperty, ".") // the item itself as parameter
        )
    );
}
```

## Recommendations

- Prefer **[compiled bindings](compiled-bindings.md)** (`.Getter(...)`) for view-model paths — you get compile-time checking and better performance.
- Prefer the builder syntax over raw `Bind()`; reach for `Bind()` only for attached `BindableProperty` targets or reusable helpers.
- For values that never change, don't bind — pass the value directly (`.Text(vm.Title)` evaluated once) and avoid binding overhead.

## Related Topics

- [Binding Converters](binding-converters.md)
- [MultiBinding](multi-binding.md)
- [Compiled Bindings](compiled-bindings.md)
- [Triggers](triggers.md) — declarative property changes without bindings
