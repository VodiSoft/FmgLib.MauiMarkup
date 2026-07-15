# Özellik Bağlama (Property Bindings)

FmgLib.MauiMarkup, MAUI data binding'i her fluent özellik metodunda bulunan **property builder lambda'sı** (`e => e…`) ve tam kontrol için düşük seviyeli `Bind()` genişletmesi üzerinden sunar. Bağlanan kaynak değiştiğinde özellik güncellenir — XAML'deki `{Binding}` ile birebir aynı.

## Builder Sözdizimi

Her özellik metodu, binding yapılandıran bir lambda kabul eder:

```csharp
new Label().Text(e => e.Path("UserName"));
```

### Builder metotları

| Metot | XAML karşılığı | Açıklama |
|---|---|---|
| `.Path(string)` | `Path=` | Kaynaktaki özellik yolu. `"."` kaynağın kendisine bağlanır. |
| `.Source(object)` | `Source=` | Binding kaynağı; varsayılan, kontrolün `BindingContext`'idir. |
| `.BindingMode(mode)` | `Mode=` | `OneWay`, `TwoWay`, `OneTime`, `OneWayToSource`, `Default`. |
| `.StringFormat(string)` | `StringFormat=` | Sonuca uygulanan biçim dizesi. |
| `.Converter(IValueConverter)` | `Converter=` | Klasik converter örneği. |
| `.Parameter(object)` | `ConverterParameter=` | Converter'a iletilir. |
| `.Convert<Q>(Func<Q,T>)` | — | Satır içi dönüştürme fonksiyonu (converter sınıfı gerekmez). |
| `.ConvertBack<Q>(Func<T,Q>)` | — | Two-way binding'ler için ters dönüşüm. |
| `.FallbackValue(object)` | `FallbackValue=` | Binding çözülemediğinde kullanılır. |
| `.TargetNullValue(object)` | `TargetNullValue=` | Çözülen değer `null` olduğunda kullanılır. |
| `.Getter(...)` / `.Setter(...)` | derlenmiş binding | Expression tabanlı — bkz. [Derlenmiş Binding'ler](compiled-bindings.md). |
| `.Bindings(...)` | `MultiBinding` | Çoklu kaynak — bkz. [MultiBinding](multi-binding.md). |

## `BindingContext`'e Bağlama (MVVM)

Varsayılan kaynak miras alınan `BindingContext` olduğundan, view-model'e bağlı bir sayfa doğal okunur:

```csharp
public class ProfileViewModel : INotifyPropertyChanged
{
    public string UserName { get; set; } = "fmg";
    public string Email { get; set; } = "user@example.com";
    // ... PropertyChanged'i her zamanki gibi tetikleyin
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

İç içe yollar, indeksleyiciler ve self yolu XAML'deki gibi çalışır:

```csharp
.Text(e => e.Path("Address.City"))
.Text(e => e.Path("PhoneNumbers[0]"))
.Text(e => e.Path("."))                 // öğenin kendisi (şablonlarda yaygın)
.Text(e => e.Path("Text.Length"))       // özelliklerin alt özellikleri
```

## Kontrolden Kontrole Bağlama

Kaynağı [`Assign`](assign-and-references.md) ile yakalayıp `.Source(...)`'a verin:

```csharp
new Slider().Assign(out var slider).Minimum(0).Maximum(100),

new Label()
    .Text(e => e.Path("Value").Source(slider).StringFormat("Value: {0:F0}"))
```

Two-way örnek — senkron bir `Entry` ve `Label`:

```csharp
new Entry().Assign(out var input).Placeholder("Bir şeyler yazın"),
new Label().Text(e => e.Path("Text").Source(input))
```

## `FallbackValue` ve `TargetNullValue`

```csharp
new Label()
    .Text(e => e
        .Path("Employee.Name")
        .FallbackValue("(binding hatası)")   // yol çözülemedi
        .TargetNullValue("(isim yok)"))      // yol çözüldü, değer null
```

## Düşük Seviyeli `Bind()` API'si

Her `BindableObject`, elinizde bir `BindableProperty` olduğunda kullanılan `Bind(...)` overload'larına da sahiptir. Attached property'ler veya yardımcı metotlar yazarken kullanışlıdır:

```csharp
new SearchBar()
    .Assign(out var search)
    .SearchCommand(vm.SearchCommand)
    .Bind(SearchBar.SearchCommandParameterProperty, "Text", source: search)
```

Temel overload'un tam imzası:

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

Generic overload'lar converter sınıfı yerine **satır içi dönüştürme fonksiyonları** kabul eder:

```csharp
new Label()
    .Bind<Label, bool, Color>(Label.TextColorProperty, "IsError",
        convert: isError => isError ? Colors.Red : Colors.Black)
```

Çoklu kaynak overload'ları 2–4 binding'i value-tuple converter'la birleştirir (bkz. [MultiBinding](multi-binding.md)):

```csharp
new Label()
    .Bind<Label, string, string, string>(Label.TextProperty,
        new Binding("FirstName"),
        new Binding("LastName"),
        convert: names => $"{names.Item1} {names.Item2}")
```

### Diğer `BindableObject` yardımcıları

| Metot | Amaç |
|---|---|
| `.BindingContext(object)` | `BindingContext`'i fluent ayarlar (context'in kendisini bağlamak için builder da alır). |
| `.BindTemplatedParent(prop, path)` | `RelativeBindingSource.TemplatedParent` ile binding — `ControlTemplate` içerikleri için. |
| `.AppThemeBinding(prop, light, dark)` | Rastgele bir `BindableProperty` için temaya bağlı değer. |
| `.AppThemeColorBinding(prop, light, dark)` | Aynısı, `Color` için özelleştirilmiş. |
| `.OnPropertyChanged(handler)` | `PropertyChanged`'e fluent abone olur. |
| `.OnPropertyChanging(handler)` | `PropertyChanging`'e abone olur. |
| `.OnBindingContextChanged(handler)` | `BindingContextChanged`'e abone olur. |

Örnek — herhangi bir özellik değişimine tepki:

```csharp
new Entry()
    .OnPropertyChanged(entry => Console.WriteLine($"{entry} üzerinde bir şey değişti"))
```

## Şablonlarda Binding

Bir `ItemTemplate` içinde her gerçekleşen görünümün `BindingContext`'i öğedir; yalın `Path` çağrıları öğe özelliklerini hedefler:

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

Şablonun içinden **sayfanın view model'ine** bağlanmak `Source` kullanır:

```csharp
public void Build()
{
    var vm = (CatalogViewModel)BindingContext;

    this.Content(
        new CollectionView()
        .ItemsSource(vm.Products)
        .ItemTemplate(() =>
            new Button()
                .Text("Sepete ekle")
                .Command(vm.AddToCartCommand)              // sayfa VM'i — doğrudan referans
                .Bind(Button.CommandParameterProperty, ".") // parametre olarak öğenin kendisi
        )
    );
}
```

## Öneriler

- View-model yolları için **[derlenmiş binding'leri](compiled-bindings.md)** (`.Getter(...)`) tercih edin — derleme zamanı kontrolü ve daha iyi performans.
- Builder sözdizimini ham `Bind()`'e tercih edin; `Bind()`'e yalnızca attached `BindableProperty` hedefleri veya yeniden kullanılabilir yardımcılar için başvurun.
- Hiç değişmeyecek değerleri bağlamayın — doğrudan değeri geçin (`.Text(vm.Title)` bir kez değerlendirilir) ve binding maliyetinden kaçının.

## İlgili Konular

- [Binding Converter'ları](binding-converters.md)
- [MultiBinding](multi-binding.md)
- [Derlenmiş Binding'ler](compiled-bindings.md)
- [Trigger'lar](triggers.md) — binding'siz bildirimsel özellik değişimleri
