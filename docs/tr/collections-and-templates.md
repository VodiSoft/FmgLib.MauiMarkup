# Koleksiyonlar ve Şablonlar

Öğe tabanlı kontroller (`CollectionView`, `CarouselView`, `ListView`, `Picker`, `IndicatorView`) ve bindable layout'lar FmgLib.MauiMarkup'ta aynı tarifi izler: bir öğe kaynağı artı bir şablon lambda'sı.

## `ItemsSource`

```csharp
new CollectionView().ItemsSource(myList)                       // doğrudan değer
new CollectionView().ItemsSource(e => e.Path("Products"))      // view model'e bağlı
```

Çalışma zamanında öğe eklenip çıkarıldığında `ObservableCollection<T>` kullanın.

## `ItemTemplate`

Anahtar overload, **öğe view'ını üreten lambda'yı** alır — markup dünyasının `<DataTemplate>`'idir:

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

Şablonun içinde, gerçekleşen her view'ın `BindingContext`'i öğedir; `Path` çağrıları öğe özelliklerini hedefler (veya [derlenmiş binding'ler](compiled-bindings.md) kullanın: `e.Getter(static (ProductVM p) => p.Name)`).

Tüm mevcut overload'lar:

| Overload | Kullanım |
|---|---|
| `.ItemTemplate(() => view)` | Satırı oluşturan lambda (en yaygın) |
| `.ItemTemplate(new DataTemplate(...))` | Mevcut `DataTemplate` örneği |
| `.ItemTemplate(new MyTemplateSelector())` | `DataTemplateSelector` ile (selector'lar `DataTemplate`'ten türer) |
| `.ItemTemplate(e => e.Path(...))` | Şablonun kendisini bağlama |

### Yeniden kullanılabilir satır view'ları

Şablonları `ContentView`'lara çıkarın (hot-reload dostu, test edilebilir):

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

## Yerleşim, Seçim, EmptyView

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
            .Text("Kayıt bulunamadı.")
            .TextColor(Colors.Red)
            .FontAttributes(FontAttributes.Bold)
            .FontSize(18)
    )
)
.ItemTemplate(() => /* ... */)
```

İki sütunlu grid yerleşimi:

```csharp
.ItemsLayout(new GridItemsLayout(span: 2, ItemsLayoutOrientation.Vertical)
    .VerticalItemSpacing(8)
    .HorizontalItemSpacing(8))
```

Sonsuz kaydırma:

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

## BindableLayout — herhangi bir layout'ta şablonlu öğeler

Sıradan layout'ların içinde (`VerticalStackLayout`, `FlexLayout`, `Grid`…) küçük öğe sayıları için öğe kaynağını layout'un kendisine bağlayın:

| Metot | Attached property |
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

> BindableLayout **tüm** öğe view'larını hemen oluşturur (sanallaştırma yok) — birkaç düzine öğeyle sınırlı tutun, uzun listeler için `CollectionView` kullanın.

### Flex öğe yardımcıları

`FlexLayout` çocukları için (şablonlu olanlar dahil) öğe başına flex attached property'leri her `View`'da mevcuttur:

```csharp
new Frame()
    .FlexBasis(FlexBasis.Auto)
    .FlexGrow(1)
    .FlexShrink(0)
    .FlexAlignSelf(FlexAlignSelf.Center)
    .FlexOrder(2)
```

Örnek — sarmalayan etiket bulutu:

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

## Düz C# ile Öğe Kurma

UI kod olduğundan, **statik** koleksiyonlar için LINQ bazen şablonu geçer:

```csharp
new VerticalStackLayout()
.Children(
    daysOfWeek.Select(d =>
        (IView)new Label().Text(d).FontSize(16).Padding(4)
    ).ToArray()
)
```

Temel kural: dinamik veri → `ItemsSource` + şablon (otomatik güncellenir); kurulumda bilinen sabit veri → LINQ/döngü.

## Picker

```csharp
new Picker()
    .Title("Ülke")
    .ItemsSource(countries)
    .ItemDisplayBinding(new Binding("Name"))
    .SelectedItem(e => e.Path("SelectedCountry").BindingMode(BindingMode.TwoWay))
```

## Çekerek Yenileme (Pull-to-Refresh)

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

## İlgili Konular

- [Özellik Bağlama](data-binding.md) — şablon binding context'leri
- [Derlenmiş Binding'ler](compiled-bindings.md)
- [Attached Property'ler](attached-properties.md)
