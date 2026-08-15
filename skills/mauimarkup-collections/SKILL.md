---
name: mauimarkup-collections
description: Build lists and templated content in FmgLib.MauiMarkup — CollectionView, CarouselView, IndicatorView, Picker, ItemsSource/ItemTemplate lambdas, template selectors, grid and linear item layouts, EmptyView, infinite scroll, pull-to-refresh, BindableLayout and FlexLayout item helpers. Use when rendering a list, feed, grid of cards, carousel, tag cloud or any repeated UI from data in a C# markup MAUI app.
license: MIT
---

# Collections & Templates

Requires the `mauimarkup` core skill.

Every item-based control follows one recipe: **an items source plus a template lambda.**

```csharp
new CollectionView()
.ItemsSource(e => e.Path("Products"))
.ItemTemplate(() =>
    new VerticalStackLayout().Padding(10).Children(
        new Label().Text(e => e.Getter(static (ProductVM p) => p.Name)).FontSize(16),
        new Label().Text(e => e.Getter(static (ProductVM p) => p.Price).StringFormat("{0:C}"))
    ))
```

Inside a template the `BindingContext` is the **item**, so `Path`/`Getter` target item properties.
Use `ObservableCollection<T>` when items are added or removed at runtime.

## `ItemTemplate` overloads

| Overload | Use |
|---|---|
| `.ItemTemplate(() => view)` | lambda creating the row — the default choice |
| `.ItemTemplate(new DataTemplate(...))` | an existing `DataTemplate` |
| `.ItemTemplate(new MyTemplateSelector())` | a `DataTemplateSelector` (it derives from `DataTemplate`) |
| `.ItemTemplate(e => e.Path(...))` | bind the template itself |

`ItemsSource` takes a direct value or a binding: `.ItemsSource(vm.Products)` /
`.ItemsSource(e => e.Path("Products"))`.

## Reusable row views

Extract rows into `ContentView`s — hot-reload friendly, testable, and reusable across pages:

```csharp
public class ProductRow : ContentView, IFmgLibHotReload
{
    public ProductRow() => this.InitializeHotReload();

    public void Build() =>
        this.Content(
            new Border().Padding(12).Content(
                new Label().Text(e => e.Getter(static (ProductVM p) => p.Name))));
}

new CollectionView().ItemTemplate(() => new ProductRow())
```

## Layout, selection, empty state

```csharp
new CollectionView()
.SelectionMode(SelectionMode.Single)
.OnSelectionChanged((s, e) => Open(e.CurrentSelection.FirstOrDefault() as ProductVM))
.ItemsLayout(new LinearItemsLayout(ItemsLayoutOrientation.Horizontal).ItemSpacing(10))
.EmptyView(
    new VerticalStackLayout().Center().Children(
        new Label().Text("No records found.").FontSize(18).TextCenter()))
.ItemTemplate(() => /* … */)
```

Two-column grid:

```csharp
.ItemsLayout(new GridItemsLayout(span: 2, ItemsLayoutOrientation.Vertical)
    .VerticalItemSpacing(8)
    .HorizontalItemSpacing(8))
```

Grouping, headers and footers are plain properties: `.IsGrouped(true)`, `.GroupHeaderTemplate(() => …)`,
`.Header(view)`, `.Footer(view)`.

## Infinite scroll

```csharp
new CollectionView()
    .RemainingItemsThreshold(4)
    .OnRemainingItemsThresholdReached(async (s, e) => await vm.LoadMoreAsync())
```

## Pull to refresh

```csharp
new RefreshView()
    .IsRefreshing(e => e.Path("IsBusy").BindingMode(BindingMode.TwoWay))
    .Command(e => e.Path("RefreshCommand"))
    .Content(
        new CollectionView().ItemsSource(e => e.Path("Items")).ItemTemplate(() => /* … */))
```

## Carousel + indicators

```csharp
new VerticalStackLayout().Children(
    new CarouselView()
        .Assign(out var carousel)
        .ItemsSource(banners)
        .HeightRequest(200)
        .ItemTemplate(() => new Image().Source(e => e.Path("ImageUrl")).Aspect(Aspect.AspectFill)),

    new IndicatorView()
        .IndicatorColor(Colors.LightGray)
        .SelectedIndicatorColor(Colors.DarkSlateBlue)
        .CenterHorizontal()
        .InvokeOnElement(iv => carousel.IndicatorView = iv))
```

`CarouselView.IndicatorView` is not a bindable property, hence `InvokeOnElement`.

## Picker

```csharp
new Picker()
    .Title("Country")
    .ItemsSource(countries)
    .ItemDisplayBinding(new Binding("Name"))
    .SelectedItem(e => e.Path("SelectedCountry").BindingMode(BindingMode.TwoWay))
```

## BindableLayout — templates in a normal layout

For small item counts inside any layout:

```csharp
new FlexLayout()
    .Wrap(FlexWrap.Wrap)
    .JustifyContent(FlexJustify.Start)
    .BindableLayoutItemsSource(vm.Categories)
    .BindableLayoutItemTemplate(new DataTemplate(() =>
        new Border()
            .Padding(10, 6).Margin(2)
            .StrokeShape(new RoundRectangle().CornerRadius(15))
            .FlexBasis(FlexBasis.Auto)
            .Content(new Label().Text(e => e.Path("Name")).FontSize(12))))
```

| Method | Attached property |
|---|---|
| `.BindableLayoutItemsSource(source)` | `BindableLayout.ItemsSource` |
| `.BindableLayoutItemTemplate(template)` | `BindableLayout.ItemTemplate` |
| `.BindableItemTemplateSelector(selector)` | `BindableLayout.TemplateSelector` |
| `.BindableLayoutEmptyView(view)` | `BindableLayout.EmptyView` |
| `.BindableLayoutEmptyViewTemplate(template)` | `BindableLayout.EmptyViewTemplate` |

> **BindableLayout creates every item view immediately — no virtualization.** Dozens of items, not
> hundreds. Anything long or unbounded belongs in a `CollectionView`.

FlexLayout per-item helpers work on any `View`: `.FlexBasis()`, `.FlexGrow()`, `.FlexShrink()`,
`.FlexAlignSelf()`, `.FlexOrder()`.

## Templates vs. plain C#

Because the UI is code, LINQ often beats a template:

```csharp
new VerticalStackLayout().Children(
    daysOfWeek.Select(d => (IView)new Label().Text(d).FontSize(16).Padding(4)).ToArray())
```

Rule: **fixed data known at build time → LINQ; dynamic data that changes → `ItemsSource` + template.**
Only `ItemsSource` reacts to collection changes.

## Reaching the page's view model from a row

Capture it — no relative bindings needed:

```csharp
var vm = (CatalogViewModel)BindingContext;

new CollectionView()
    .ItemsSource(vm.Products)
    .ItemTemplate(() => new Button()
        .Text("Add to cart")
        .Command(vm.AddToCartCommand)
        .Bind(Button.CommandParameterProperty, "."))     // "." = this row's item
```

## Performance

- Use **compiled bindings** (`Getter`) in templates — templates are the hottest binding path in the app.
- Keep row trees shallow; a deep nest per row multiplies across every realized cell.
- Don't put a `CollectionView` inside a `ScrollView` or an `Auto`-sized grid row — it loses
  virtualization. Give it a `Star` row.
- Prefer `Border` over `Frame` for cards (`Frame` is heavier and effectively legacy).
- `SelectionMode(SelectionMode.None)` when rows aren't selectable — it skips selection bookkeeping.
