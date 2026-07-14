# Collections & Templates

Item-based controls (`CollectionView`, `CarouselView`, `ListView`, `Picker`, `IndicatorView`) and bindable layouts all follow the same recipe in FmgLib.MauiMarkup: an items source plus a template lambda.

## `ItemsSource`

```csharp
new CollectionView().ItemsSource(myList)                       // direct value
new CollectionView().ItemsSource(e => e.Path("Products"))      // bound to the view model
```

Use `ObservableCollection<T>` when items are added/removed at runtime.

## `ItemTemplate`

The key overload takes a **lambda producing the item view** — it is the `<DataTemplate>` of the markup world:

```csharp
new CollectionView()
.ItemsSource(e => e.Path("Products"))
.ItemTemplate(() =>
    new VerticalStackLayout()
    .Padding(10)
    .Children(
        new Label().Text(e => e.Path("Name")).FontSize(16),
        new Label().Text(e => e.Path("Price").StringFormat("{0:C}")).TextColor(Colors.Green)
    )
)
```

Inside the template, each realized view's `BindingContext` is the item, so `Path` calls target item properties (or use [compiled bindings](compiled-bindings.md): `e.Getter(static (ProductVM p) => p.Name)`).

All available overloads:

| Overload | Use |
|---|---|
| `.ItemTemplate(() => view)` | Lambda creating the row (most common) |
| `.ItemTemplate(new DataTemplate(...))` | Existing `DataTemplate` instance |
| `.ItemTemplate(new MyTemplateSelector())` | Via `.ItemTemplate(DataTemplate)` with a `DataTemplateSelector` (selectors derive from `DataTemplate`) |
| `.ItemTemplate(e => e.Path(...))` | Bind the template itself |

### Reusable row views

Extract templates into `ContentView`s (hot-reload friendly, unit-testable):

```csharp
public class ProductRow : ContentView, IFmgLibHotReload
{
    public ProductRow() => this.InitializeHotReload();

    public void Build() =>
        this.Content(
            new Border().Padding(12).Content(
                new Label().Text(e => e.Getter(static (ProductVM p) => p.Name))
            )
        );
}

new CollectionView().ItemTemplate(() => new ProductRow())
```

## Layout, Selection, EmptyView

```csharp
new CollectionView()
.SelectionMode(SelectionMode.Single)
.OnSelectionChanged((s, e) => Open(e.CurrentSelection.FirstOrDefault() as ProductVM))
.ItemsLayout(new LinearItemsLayout(ItemsLayoutOrientation.Horizontal).ItemSpacing(10))
.EmptyView(
    new VerticalStackLayout()
    .Center()
    .Children(
        new Label()
            .Text("No records found.")
            .TextColor(Colors.Red)
            .FontAttributes(FontAttributes.Bold)
            .FontSize(18)
    )
)
.ItemTemplate(() => /* ... */)
```

Grid layout with two columns:

```csharp
.ItemsLayout(new GridItemsLayout(span: 2, ItemsLayoutOrientation.Vertical)
    .VerticalItemSpacing(8)
    .HorizontalItemSpacing(8))
```

Infinite scrolling:

```csharp
new CollectionView()
    .RemainingItemsThreshold(4)
    .OnRemainingItemsThresholdReached(async (s, e) => await vm.LoadMoreAsync())
```

## `CarouselView` + `IndicatorView`

```csharp
new VerticalStackLayout()
.Children(
    new CarouselView()
        .Assign(out var carousel)
        .ItemsSource(banners)
        .HeightRequest(200)
        .ItemTemplate(() =>
            new Image().Source(e => e.Path("ImageUrl")).Aspect(Aspect.AspectFill)),

    new IndicatorView()
        .Assign(out var indicators)
        .IndicatorColor(Colors.LightGray)
        .SelectedIndicatorColor(Colors.DarkSlateBlue)
        .CenterHorizontal()
        .InvokeOnElement(iv => carousel.IndicatorView = iv)
)
```

## BindableLayout — templated items in any layout

For small item counts inside regular layouts (`VerticalStackLayout`, `FlexLayout`, `Grid`…), attach an items source to the layout itself:

| Method | Attached property |
|---|---|
| `.BindableLayoutItemsSource(source)` | `BindableLayout.ItemsSource` |
| `.BindableLayoutItemTemplate(template)` | `BindableLayout.ItemTemplate` |
| `.BindableItemTemplateSelector(selector)` | `BindableLayout.ItemTemplateSelector` |
| `.BindableLayoutEmptyView(view)` | `BindableLayout.EmptyView` |
| `.BindableLayoutEmptyViewTemplate(template)` | `BindableLayout.EmptyViewTemplate` |

```csharp
new HorizontalStackLayout()
    .Spacing(6)
    .BindableLayoutItemsSource(vm.Tags)
    .BindableLayoutItemTemplate(new DataTemplate(() =>
        new Border()
            .Padding(8, 4)
            .StrokeShape(new RoundRectangle().CornerRadius(10))
            .Content(new Label().Text(e => e.Path(".")).FontSize(12))
    ))
```

> BindableLayout creates **all** item views immediately (no virtualization) — keep it to dozens of items, and use `CollectionView` for long lists.

### Flex item helpers

For children of `FlexLayout` (including templated ones), per-item flex attached properties are available on any `View`:

```csharp
new Frame()
    .FlexBasis(FlexBasis.Auto)
    .FlexGrow(1)
    .FlexShrink(0)
    .FlexAlignSelf(FlexAlignSelf.Center)
    .FlexOrder(2)
```

Example — a wrapping tag cloud:

```csharp
new FlexLayout()
    .Wrap(FlexWrap.Wrap)
    .JustifyContent(FlexJustify.Start)
    .BindableLayoutItemsSource(vm.Categories)
    .BindableLayoutItemTemplate(new DataTemplate(() =>
        new Frame()
            .CornerRadius(15)
            .Padding(10, 6)
            .Margin(2)
            .FlexBasis(FlexBasis.Auto)
            .Content(new Label().Text(e => e.Path("Name")).FontSize(12))
    ))
```

## Building Items with Plain C#

Since UI is code, LINQ sometimes beats templates for **static** collections:

```csharp
new VerticalStackLayout()
.Children(
    daysOfWeek.Select(d =>
        (IView)new Label().Text(d).FontSize(16).Padding(4)
    ).ToArray()
)
```

Rule of thumb: dynamic data → `ItemsSource` + template (updates automatically); fixed data known at build time → LINQ/loops.

## Picker

```csharp
new Picker()
    .Title("Country")
    .ItemsSource(countries)
    .ItemDisplayBinding(new Binding("Name"))
    .SelectedItem(e => e.Path("SelectedCountry").BindingMode(BindingMode.TwoWay))
```

## Pull-to-Refresh

```csharp
new RefreshView()
    .IsRefreshing(e => e.Path("IsBusy"))
    .Command(e => e.Path("RefreshCommand"))
    .Content(
        new CollectionView()
            .ItemsSource(e => e.Path("Items"))
            .ItemTemplate(() => /* ... */)
    )
```

## Related Topics

- [Property Bindings](data-binding.md) — template binding contexts
- [Compiled Bindings](compiled-bindings.md)
- [Attached Properties](attached-properties.md)
