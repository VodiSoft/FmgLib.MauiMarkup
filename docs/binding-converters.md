# Binding Converters

Converters transform a bound value on its way from source to target (and optionally back). FmgLib.MauiMarkup supports classic `IValueConverter` instances **and** — much more conveniently — inline conversion functions, so most converters never need a class.

## Inline `Convert` — no converter class required

The property builder's `Convert<Q>(Func<Q, T>)` takes the raw source value (`Q`) and returns the target property type (`T`):

```csharp
public class CustomPage : ContentPage
{
    public List<int> MyNumbers = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    public CustomPage()
    {
        this
        .Content(
            new VerticalStackLayout()
            .Children(
                new CollectionView()
                .ItemsSource(MyNumbers)
                .ItemTemplate(() =>
                    new Label()
                        .FontSize(30)
                        .Text(e => e.Path("."))
                        .TextColor(Colors.Gray)
                        .BackgroundColor(e => e
                            .Path(".")
                            .Convert((int n) => n % 2 == 0 ? Colors.Green : Colors.Yellow)
                        )
                )
            )
        );
    }
}
```

Here each item (an `int`) is bound to `BackgroundColor` (a `Color`); the lambda does the conversion. Even/odd items get different backgrounds.

More everyday examples:

```csharp
// bool → Color
new Label()
    .TextColor(e => e.Path("IsError").Convert((bool err) => err ? Colors.Red : Colors.Black))

// int → bool  (visibility from a count)
new Label()
    .Text("Your cart is empty")
    .IsVisible(e => e.Path("Items.Count").Convert((int c) => c == 0))

// DateTime → string
new Label()
    .Text(e => e.Path("CreatedAt").Convert((DateTime d) => d.ToString("dd MMM yyyy")))

// enum → ImageSource
new Image()
    .Source(e => e.Path("Status").Convert((OrderStatus s) => s switch
    {
        OrderStatus.Shipped   => "truck.png",
        OrderStatus.Delivered => "check.png",
        _                     => "clock.png"
    }))
```

## Two-Way: `ConvertBack`

For `TwoWay` bindings, add `ConvertBack<Q>(Func<T, Q>)` to translate edits back to the source:

```csharp
new Entry()
    .Text(e => e
        .Path("Price")
        .BindingMode(BindingMode.TwoWay)
        .Convert((decimal p) => p.ToString("F2"))
        .ConvertBack((string s) => decimal.TryParse(s, out var d) ? d : 0m))
```

## Classic `IValueConverter`

Existing converters (yours or from a toolkit) plug in with `.Converter(...)` and `.Parameter(...)`:

```csharp
public class InverterConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) => !(bool)value;
    public object ConvertBack(object value, Type t, object p, CultureInfo c) => !(bool)value;
}

new CheckBox()
    .IsChecked(e => e.Path("IsSuspended").Converter(new InverterConverter()))
```

With a parameter:

```csharp
new Label()
    .Text(e => e
        .Path("Amount")
        .Converter(new CurrencyConverter())
        .Parameter("USD"))
```

> **Tip:** converters are stateless in most cases — share single instances via `static readonly` fields instead of allocating one per binding:
>
> ```csharp
> static readonly InverterConverter Inverter = new();
> ```

## Converters with the Low-Level `Bind()` API

`Bind()` overloads accept both converter instances and conversion funcs:

```csharp
// instance
new ImageButton()
    .Bind(ImageButton.SourceProperty, nameof(ProductVM.IsFavorite),
          converter: new BoolToFavoriteImageConverter())

// funcs (typed, no class)
new Label()
    .Bind<Label, bool, TextDecorations>(Label.TextDecorationsProperty,
        nameof(ProductVM.IsDiscount),
        convert: isDiscount => isDiscount ? TextDecorations.Strikethrough : TextDecorations.None)

// funcs with converter parameter
new Label()
    .Bind<Label, double, string, string>(Label.TextProperty, "Total",
        convert: (total, currency) => $"{total:F2} {currency}",
        converterParameter: "₺")
```

## Choosing an Approach

| Situation | Recommendation |
|---|---|
| One-off transformation used in a single place | Inline `Convert(...)` lambda |
| Two-way editing with parsing | `Convert` + `ConvertBack` |
| Reused across many pages / needs DI or state | `IValueConverter` class + `.Converter(...)` |
| Converting inside a `MultiBinding` | `IMultiValueConverter` — see [MultiBinding](multi-binding.md) |
| Complex logic better expressed as a VM property | Skip the converter; expose a computed property on the view model |

The last row is worth emphasizing: because your UI is C#, a computed view-model property (`public Color StatusColor => …`) is often simpler and more testable than any converter.

## Related Topics

- [Property Bindings](data-binding.md)
- [MultiBinding](multi-binding.md) — `IMultiValueConverter`, `AllTrueMultiConverter`-style patterns
- [Compiled Bindings](compiled-bindings.md)
