# Property MultiBinding

A **MultiBinding** combines several bindings into a single target property value. FmgLib.MauiMarkup exposes it through the property builder's `Bindings(...)` method — add as many `BindingBase` instances as you need.

## Builder Syntax

```csharp
public partial class MainPage : ContentPage, IFmgLibHotReload
{
    private readonly MainPageViewModel viewModel;

    public MainPage()
    {
        viewModel = new MainPageViewModel();
        this.InitializeHotReload();
    }

    public void Build()
    {
        this
        .BindingContext(viewModel)
        .Content(
            new VerticalStackLayout()
            .Spacing(20)
            .Children(
                // Combine three bool sources into one IsChecked value
                new CheckBox()
                .IsChecked(e => e
                    .Bindings(
                        new Binding().Path("Employee.IsOver16"),
                        new Binding().Path("Employee.HasPassedTest"),
                        new Binding().Path("Employee.IsSuspended").Converter(new InverterConverter())
                    )
                    .Converter(new AllTrueMultiConverter())
                    .FallbackValue("Is Error.")
                    .TargetNullValue("Is Null.")
                ),

                // Combine three values into one formatted string — no converter needed
                new Label()
                .Text(e => e
                    .Bindings(
                        new Binding().Path("Employee.Id"),
                        new Binding().Path("Employee.Name"),
                        new Binding().Path("Employee.IsSuspended")
                    )
                    .StringFormat("{0} : {1} : {2}")
                    .FallbackValue("Is Error.")
                    .TargetNullValue("Is Null.")
                )
            )
        );
    }
}
```

### Multi-binding builder methods

| Method | Description |
|---|---|
| `.Bindings(params BindingBase[])` | The child bindings (unlimited count). |
| `.StringFormat(string)` | Formats the child values positionally (`{0}`, `{1}`, …). If set, a converter is optional. |
| `.Converter(IMultiValueConverter)` | Combines the child values into one result. |
| `.Parameter(string)` | `ConverterParameter` for the multi-converter. |
| `.BindingMode(mode)` | Binding mode of the multi-binding. |
| `.FallbackValue(object)` / `.TargetNullValue(object)` | As in single bindings. |

Note that each **child** `Binding` is configured with the library's fluent `Binding` extensions: `.Path(…)`, `.Source(…)`, `.Converter(…)`, `.ConverterParameter(…)`, `.UpdateSourceEventName(…)`. (Other `Binding` members, e.g. `Mode`, can be set via object initializer: `new Binding { Mode = BindingMode.OneWay }.Path("X")`.)

## Writing an `IMultiValueConverter`

```csharp
public class AllTrueMultiConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Any(v => v is not bool))
            return BindableProperty.UnsetValue;

        return values.OfType<bool>().All(b => b);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
```

Returning `BindableProperty.UnsetValue` makes the binding fall back to `FallbackValue`.

## Practical Examples

**Full name from two properties:**

```csharp
new Label()
    .Text(e => e
        .Bindings(
            new Binding().Path("FirstName"),
            new Binding().Path("LastName"))
        .StringFormat("{0} {1}"))
```

**Enable a button only when the form is valid:**

```csharp
new Entry().Assign(out var user).Placeholder("User"),
new Entry().Assign(out var pass).Placeholder("Password").IsPassword(true),

new Button()
    .Text("Sign in")
    .IsEnabled(e => e
        .Bindings(
            new Binding().Path("Text.Length").Source(user),
            new Binding().Path("Text.Length").Source(pass))
        .Converter(new AllPositiveMultiConverter()))
```

**Compiled child bindings** (see [Compiled Bindings](compiled-bindings.md)) — with .NET MAUI 9+ you can build the child bindings from expressions using `Binding.Create`, keeping compile-time safety inside a multi-binding:

```csharp
new CheckBox()
.IsChecked(e => e
    .Bindings(
        Binding.Create(static (MainPageViewModel vm) => vm.IsOver16),
        Binding.Create(static (MainPageViewModel vm) => vm.HasPassedTest),
        Binding.Create(static (MainPageViewModel vm) => vm.IsSuspended)
    )
    .Converter(new AllTrueMultiConverter()))
```

## Typed Multi-Binding via `Bind()`

If you prefer func-based combination without a converter class, the low-level `Bind()` extension has typed overloads for 2, 3 or 4 sources whose values arrive as a tuple:

```csharp
new Label()
    .Bind<Label, string, string, string>(Label.TextProperty,
        new Binding("FirstName"),
        new Binding("LastName"),
        convert: n => $"{n.Item1} {n.Item2}")

new Button()
    .Bind<Button, bool, bool, bool, bool>(Button.IsEnabledProperty,
        new Binding("HasUser"),
        new Binding("HasPassword"),
        new Binding("AcceptedTerms"),
        convert: v => v.Item1 && v.Item2 && v.Item3)
```

There are also variants accepting a `converterParameter` and a `convertBack` function for two-way scenarios.

## When to Use MultiBinding

- A target property genuinely depends on **several independently-changing sources**.
- You cannot (or don't want to) add a computed property to the view model.

Otherwise, prefer a computed VM property raising `PropertyChanged` — it is easier to test and debug than converter plumbing.

## Related Topics

- [Property Bindings](data-binding.md)
- [Binding Converters](binding-converters.md)
- [Compiled Bindings](compiled-bindings.md)
