# Property MultiBinding

A **MultiBinding** feeds a single target property from several sources at once. In FmgLib.MauiMarkup you build one with the same property builder you already use for a normal binding: call `.Path(...)` (or `.Getter(...)`) more than once, then close the chain with a method that combines the collected values.

```csharp
new Label()
    .Text(e => e
        .Path(nameof(Person.FirstName))
        .Path(nameof(Person.LastName))
        .MultiConvert((string first, string last) => $"{first} {last}"))
```

Each `.Path()` opens its own sub binding. Everything that follows it — `.Source()`, `.BindingMode()`, `.StringFormat()`, `.Converter()`, `.Parameter()`, `.Convert()`, `.ConvertBack()`, `.FallbackValue()`, `.TargetNullValue()` — belongs to the sub binding that was opened last. The `Multi…` methods belong to the multi binding as a whole.

A single `.Path()` still produces a plain `Binding`, so nothing changes for ordinary bindings: the multi binding only appears once you declare a second source or call one of the `Multi…` methods.

## Combining values

`MultiConvert` takes the values in declaration order. Its parameter types must match what each sub binding produces, and its return type is the type of the target property.

```csharp
new VerticalStackLayout()
.Children(
    new Slider().Assign(out var width).Minimum(1).Maximum(300),
    new Slider().Assign(out var height).Minimum(1).Maximum(300),

    new Label()
        .Text(e => e
            .Path(nameof(Slider.Value)).Source(width)
            .Path(nameof(Slider.Value)).Source(height)
            .MultiConvert((double w, double h) => $"{w:F0} × {h:F0} = {w * h:F0} px²"))
)
```

Overloads exist for 2 to 9 sub bindings. If the number of `.Path()` calls does not match the number of delegate parameters, the mistake is reported when the binding is built, with both counts in the message.

### Converting a single source first

`.Convert()` belongs to the sub binding it follows, so a source can be reshaped before it reaches `MultiConvert`:

```csharp
new Label()
    .Text(e => e
        .Path(nameof(Person.Age)).Convert((int age) => age >= 18)
        .Path(nameof(Person.FirstName))
        .MultiConvert((bool adult, string name) => adult ? name : $"{name} (minor)"))
```

The first sub binding hands over a `bool` because of its own `.Convert()`, the second hands over the raw `string`. The two names mark the two roles: **`Convert` always belongs to a path, `MultiConvert` always closes the chain.**

### Formatting without a converter

When the combination is pure formatting, `MultiStringFormat` is enough:

```csharp
new Label()
    .Text(e => e
        .Path(nameof(Person.FirstName))
        .Path(nameof(Person.LastName))
        .MultiStringFormat("{0} {1}"))
```

## Compiled multi bindings

`.Getter(...)` opens a compiled sub binding — no reflection, no string paths, and the compiler checks the property names. Sub bindings may produce different types:

```csharp
new Label()
    .Text(e => e
        .Getter(static (PersonViewModel vm) => vm.FirstName)
        .Getter(static (PersonViewModel vm) => vm.Age)
        .MultiConvert((string name, int age) => $"{name} ({age})"))
```

Compiled and string based sub bindings can be mixed freely inside the same multi binding, and `.Setter(...)` still supplies the reverse operation of the compiled sub binding it follows. See [Compiled Bindings](compiled-bindings.md) for the rules a getter expression has to follow.

## Two-way multi bindings

`MultiConvertBack` returns a tuple whose elements are written back in declaration order. Each element then passes through the `ConvertBack()` of its own sub binding, if one was declared.

```csharp
new Entry()
    .Text(e => e
        .Path(nameof(Person.FirstName))
        .Path(nameof(Person.LastName))
        .MultiMode(BindingMode.TwoWay)
        .MultiConvert((string first, string last) => $"{first} {last}")
        .MultiConvertBack((string full) =>
        {
            var parts = full.Split(' ');
            return (parts[0], parts.Length > 1 ? parts[1] : string.Empty);
        }))
```

`MultiMode()` sets the mode of the multi binding itself. A single sub binding can still override it with its own `.BindingMode()`, which is how you keep one source read-only inside a two-way multi binding.

## Boolean aggregates

For the common "enable this when all of that is true" case no delegate is needed at all:

```csharp
new Button()
    .Text("Sign up")
    .IsEnabled(e => e
        .Path(nameof(SignUpViewModel.AcceptedTerms))
        .Path(nameof(SignUpViewModel.AcceptedPrivacy))
        .Path(nameof(SignUpViewModel.IsEmailVerified))
        .MultiAll())
```

| Helper | True when |
|---|---|
| `.MultiAll()` | every sub binding is `true` |
| `.MultiAny()` | at least one sub binding is `true` |
| `.MultiNone()` | no sub binding is `true` |
| `.MultiAtLeast(n)` | at least `n` sub bindings are `true` |
| `.MultiExactly(n)` | exactly `n` sub bindings are `true` |

They apply to a `bool` property and expect every sub binding to produce a `bool` — directly, or through that sub binding's own `.Convert()`:

```csharp
new Button()
    .IsEnabled(e => e
        .Path(nameof(Entry.Text)).Source(nameEntry).Convert((string s) => !string.IsNullOrWhiteSpace(s))
        .Path(nameof(Entry.Text)).Source(mailEntry).Convert((string s) => !string.IsNullOrWhiteSpace(s))
        .MultiAll())
```

Because the aggregates work with any number of sub bindings, the count is not validated: `MultiAtLeast(3)` over two sub bindings is simply always `false`.

## A dynamic number of sub bindings

When the sub bindings are produced by a loop and their number is not known in advance, use `MultiConvertRaw`. The typed form unboxes every value to one type and reports a mismatch the same way `MultiConvert` does:

```csharp
new Label()
    .Text(e => e
        .Path("Basket.Food").Convert((decimal v) => (double)v)
        .Path("Basket.Drinks").Convert((decimal v) => (double)v)
        .Path("Basket.Delivery").Convert((decimal v) => (double)v)
        .MultiConvertRaw<double>(values => values.Sum().ToString("C")))
```

The untyped form hands you the raw `object?[]` in declaration order plus an optional reverse delegate; you are responsible for the casts:

```csharp
.MultiConvertRaw(
    values => Describe(values),
    value => Split(value))
```

## Multi binding methods

| Method | Description |
|---|---|
| `.MultiConvert(...)` | Combines 2 to 9 sub binding values into the property value. |
| `.MultiConvertBack(...)` | Reverse of `MultiConvert`, returning a tuple in declaration order. |
| `.MultiConvertRaw<Q>(...)` / `.MultiConvertRaw(...)` | Dynamic number of sub bindings, typed or raw. |
| `.MultiStringFormat(string)` | Positional formatting (`{0}`, `{1}`, …) instead of a converter. |
| `.MultiConverter(IMultiValueConverter)` | Your own multi value converter. |
| `.MultiParameter(object)` | `ConverterParameter` of the multi binding. |
| `.MultiMode(BindingMode)` | Mode of the multi binding as a whole. |
| `.MultiFallbackValue(object)` / `.MultiTargetNullValue(object)` | As in single bindings, at multi binding level. |
| `.MultiAll()` / `.MultiAny()` / `.MultiNone()` / `.MultiAtLeast(n)` / `.MultiExactly(n)` | Boolean aggregates. |

## When values are still missing

A multi binding is evaluated as soon as the first sub binding resolves, so the other slots may still be empty. In that case the target property keeps its current value instead of being overwritten with a null: an update is skipped while any sub binding is unresolved, or while a source is `null` and the matching delegate parameter is a non-nullable value type. A `null` for a parameter that accepts it (a `string`, a nullable value type, any reference type) is passed through as usual.

If a delegate parameter is declared with a type the binding never produces, the exception names the property, the offending path and the index of the value, instead of failing somewhere inside the MAUI binding pipeline.

## Using your own `IMultiValueConverter`

```csharp
new CheckBox()
    .IsChecked(e => e
        .Path("Employee.IsOver16")
        .Path("Employee.HasPassedTest")
        .Path("Employee.IsSuspended").Convert((bool suspended) => !suspended)
        .MultiConverter(new AllTrueMultiConverter())
        .MultiFallbackValue(false))
```

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

Returning `BindableProperty.UnsetValue` makes the binding fall back to `MultiFallbackValue`.

## Ready made `BindingBase` instances

Sub bindings you already hold as objects — including compiled ones created with `Binding.Create` — can be added with `.Bindings(...)`:

```csharp
new CheckBox()
    .IsChecked(e => e
        .Bindings(
            Binding.Create(static (MainPageViewModel vm) => vm.IsOver16),
            Binding.Create(static (MainPageViewModel vm) => vm.HasPassedTest))
        .MultiConvert((bool over16, bool passed) => over16 && passed))
```

Starting the chain with `.Bindings(...)` opens a builder dedicated to ready made child bindings. Its `.Converter(IMultiValueConverter)`, `.Parameter()`, `.StringFormat()`, `.BindingMode()`, `.FallbackValue()` and `.TargetNullValue()` all apply to the multi binding itself, so code written against earlier versions keeps working unchanged:

```csharp
new CheckBox()
    .IsChecked(e => e
        .Bindings(
            new Binding().Path("Employee.IsOver16"),
            new Binding().Path("Employee.HasPassedTest"))
        .Converter(new AllTrueMultiConverter())
        .FallbackValue(false))
```

The `MultiConvert`, `MultiConvertBack`, `MultiConvertRaw` and boolean aggregate methods are available here too, so a converter class is never mandatory. For new code prefer `.Path()` / `.Getter()`: the values stay typed and each source can be shaped on its own.

## Typed multi bindings via `Bind()`

The low-level `Bind()` extension also has typed overloads for 2, 3 or 4 sources whose values arrive as a tuple:

```csharp
new Label()
    .Bind<Label, string, string, string>(Label.TextProperty,
        new Binding("FirstName"),
        new Binding("LastName"),
        convert: n => $"{n.Item1} {n.Item2}")
```

There are also variants accepting a `converterParameter` and a `convertBack` function for two-way scenarios.

## When to use a MultiBinding

- A target property genuinely depends on **several independently changing sources**.
- You cannot (or do not want to) add a computed property to the view model.

Otherwise prefer a computed view-model property that raises `PropertyChanged` — it is easier to test and debug than binding plumbing.

## Migrating from earlier versions

The multi binding API is new in 10.2.0; single bindings behave exactly as before. Four call shapes changed:

| Before | Now |
|---|---|
| `.Convert<double>(v => v > 10)` | `.Convert((double v) => v > 10)` — or `.Convert<double, bool>(...)` |
| `.ConvertBack<int>(v => …)` | `.ConvertBack((string v) => …)` — or `.ConvertBack<string, int>(...)` |
| `.Getter<PersonViewModel>(vm => vm.Name)` | `.Getter(static (PersonViewModel vm) => vm.Name)` |
| `.Setter<PersonViewModel>((vm, v) => vm.Name = v)` | `.Setter(static (PersonViewModel vm, string v) => vm.Name = v)` |

In every case the type-inferred form used throughout the documentation is unchanged; only explicit type
arguments have to be dropped or completed, because each of these methods now carries a second type parameter
for the value it produces.

Two behaviours also changed, both of them previously silent bugs:

- Calling `.Path()` twice used to keep only the last path. It now opens two sub bindings, and a multi binding
  without a combining method reports that when it is built.
- Combining `.Converter(...)` with `.Convert(...)` on the same sub binding used to overwrite one with the
  other silently; it now throws.

## Related Topics

- [Property Bindings](data-binding.md)
- [Binding Converters](binding-converters.md)
- [Compiled Bindings](compiled-bindings.md)
