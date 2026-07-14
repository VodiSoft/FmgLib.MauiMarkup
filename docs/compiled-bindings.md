# Compiled Bindings in Code

String-path bindings (`e.Path("Name")`) are resolved at runtime with reflection. **Compiled bindings** replace the string with a typed expression, giving you:

- **Performance** — the binding expression is resolved at compile time; no reflection per update.
- **Build-time safety** — an invalid path is a compiler error, not a silent runtime failure.
- **IntelliSense & refactoring** — rename the VM property and the binding follows.

## `Getter` — the compiled `Path`

Wherever you would write `Path`, you can write `Getter`:

```csharp
// before — string path, runtime reflection
new Label().Text(e => e.Path("Text").Source(entry));

// after — compiled
new Label().Text(e => e.Getter(static (Entry entry) => entry.Text).Source(entry));
```

With a view model:

```csharp
new Label()
    .Text(e => e.Getter(static (PersonViewModel vm) => vm.Name))

new Label()
    .Text(e => e
        .Getter(static (PersonViewModel vm) => vm.Address.City)
        .StringFormat("City: {0}"))
```

All other builder methods (`Source`, `BindingMode`, `StringFormat`, `Converter`, `Convert`, `FallbackValue`, `TargetNullValue`…) combine with `Getter` exactly as with `Path`.

> Mark the lambda `static` — it prevents accidental closure captures and makes the intent (a pure property access) explicit.

## `Setter` — enabling two-way updates

For `TwoWay`/`OneWayToSource` compiled bindings, supply the reverse operation:

```csharp
new Entry()
    .Text(e => e
        .Getter(static (PersonViewModel vm) => vm.Name)
        .Setter(static (PersonViewModel vm, string value) => vm.Name = value)
        .BindingMode(BindingMode.TwoWay))
```

## What Expressions Are Valid?

The getter must be a **simple property access expression**. Supported:

```csharp
// Property access (incl. null-conditional chains)
static (PersonViewModel vm) => vm.Name;
static (PersonViewModel vm) => vm.Address?.Street;

// Array / indexer access
static (PersonViewModel vm) => vm.PhoneNumbers[0];
static (PersonViewModel vm) => vm.Config["Font"];

// Casts
static (Label label) => (label.BindingContext as PersonViewModel).Name;
static (Label label) => ((PersonViewModel)label.BindingContext).Name;
```

Not supported (these need a converter or a computed VM property instead):

```csharp
// Method calls
static (PersonViewModel vm) => vm.GetAddress();
static (PersonViewModel vm) => vm.Address?.ToString();

// Complex expressions
static (PersonViewModel vm) => vm.Address?.Street + " " + vm.Address?.City;
static (PersonViewModel vm) => $"Name: {vm.Name}";
```

If you need a transformation, keep the getter simple and add `Convert`:

```csharp
new Label()
    .Text(e => e
        .Getter(static (PersonViewModel vm) => vm.Name)
        .Convert((string name) => $"Name: {name}"))
```

## `Binding.Create` (MAUI 9+)

.NET MAUI 9 added `BindingBase.Create`, which builds a typed binding object directly from a `Func`. It pairs perfectly with FmgLib's `Bindings(...)` for compiled **multi-bindings**:

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
                new CheckBox()
                .IsChecked(e => e
                    .Bindings(
                        Binding.Create(static (MainPageViewModel m) => m.IsOver16),
                        Binding.Create(static (MainPageViewModel m) => m.HasPassedTest),
                        Binding.Create(static (MainPageViewModel m) => m.IsSuspended)
                    )
                    .Converter(new AllTrueMultiConverter())
                    .FallbackValue("Is Error.")
                    .TargetNullValue("Is Null.")
                ),

                new Label()
                .Text(e => e
                    .Bindings(
                        Binding.Create(static (MainPageViewModel m) => m.Id),
                        Binding.Create(static (MainPageViewModel m) => m.Name),
                        Binding.Create(static (MainPageViewModel m) => m.IsSuspended)
                    )
                    .StringFormat("{0} : {1} : {2}")
                )
            )
        );
    }
}
```

## Compiled Bindings in Templates

Inside item templates the binding context is the item, so type the lambda accordingly:

```csharp
new CollectionView()
.ItemsSource(vm.Products)
.ItemTemplate(() =>
    new VerticalStackLayout().Children(
        new Label().Text(e => e.Getter(static (ProductVM p) => p.Name)),
        new Label().Text(e => e
            .Getter(static (ProductVM p) => p.Price)
            .StringFormat("{0:C}"))
    )
)
```

## Migration Cheat Sheet

| String binding | Compiled binding |
|---|---|
| `e.Path("Name")` | `e.Getter(static (VM vm) => vm.Name)` |
| `e.Path("Address.City")` | `e.Getter(static (VM vm) => vm.Address.City)` |
| `e.Path("Text").Source(entry)` | `e.Getter(static (Entry x) => x.Text).Source(entry)` |
| `e.Path("Name").BindingMode(TwoWay)` | add `.Setter(static (VM vm, string v) => vm.Name = v)` |
| `new Binding().Path("X")` in `Bindings(...)` | `Binding.Create(static (VM vm) => vm.X)` |

## Related Topics

- [Property Bindings](data-binding.md)
- [MultiBinding](multi-binding.md)
- [Binding Converters](binding-converters.md)
