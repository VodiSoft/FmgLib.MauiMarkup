# Binding Converter'ları

Converter'lar, bağlanan değeri kaynaktan hedefe (ve isteğe bağlı olarak geri) giderken dönüştürür. FmgLib.MauiMarkup klasik `IValueConverter` örneklerini **ve** — çok daha pratik olan — satır içi dönüştürme fonksiyonlarını destekler; çoğu converter için sınıf yazmak hiç gerekmez.

## Satır İçi `Convert` — converter sınıfı gerekmez

Property builder'ın `Convert<Q>(Func<Q, T>)` metodu ham kaynak değerini (`Q`) alır ve hedef özellik tipini (`T`) döndürür:

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

Burada her öğe (bir `int`) `BackgroundColor`'a (bir `Color`) bağlanır; dönüşümü lambda yapar. Çift/tek öğeler farklı arka plan alır.

Günlük hayattan daha fazla örnek:

```csharp
// bool → Color
new Label()
    .TextColor(e => e.Path("IsError").Convert((bool err) => err ? Colors.Red : Colors.Black))

// int → bool  (sayıdan görünürlük)
new Label()
    .Text("Sepetiniz boş")
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

## İki Yönlü: `ConvertBack`

`TwoWay` binding'ler için düzenlemeleri kaynağa geri çeviren `ConvertBack<Q>(Func<T, Q>)` ekleyin:

```csharp
new Entry()
    .Text(e => e
        .Path("Price")
        .BindingMode(BindingMode.TwoWay)
        .Convert((decimal p) => p.ToString("F2"))
        .ConvertBack((string s) => decimal.TryParse(s, out var d) ? d : 0m))
```

## Klasik `IValueConverter`

Mevcut converter'lar (sizin veya bir toolkit'in) `.Converter(...)` ve `.Parameter(...)` ile takılır:

```csharp
public class InverterConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) => !(bool)value;
    public object ConvertBack(object value, Type t, object p, CultureInfo c) => !(bool)value;
}

new CheckBox()
    .IsChecked(e => e.Path("IsSuspended").Converter(new InverterConverter()))
```

Parametreyle:

```csharp
new Label()
    .Text(e => e
        .Path("Amount")
        .Converter(new CurrencyConverter())
        .Parameter("USD"))
```

> **İpucu:** converter'lar çoğu durumda durumsuzdur — binding başına yeni örnek yaratmak yerine `static readonly` alanlarla tek örneği paylaşın:
>
> ```csharp
> static readonly InverterConverter Inverter = new();
> ```

## Düşük Seviyeli `Bind()` ile Converter'lar

`Bind()` overload'ları hem converter örneklerini hem dönüştürme func'larını kabul eder:

```csharp
// örnek
new ImageButton()
    .Bind(ImageButton.SourceProperty, nameof(ProductVM.IsFavorite),
          converter: new BoolToFavoriteImageConverter())

// func'lar (tipli, sınıfsız)
new Label()
    .Bind<Label, bool, TextDecorations>(Label.TextDecorationsProperty,
        nameof(ProductVM.IsDiscount),
        convert: isDiscount => isDiscount ? TextDecorations.Strikethrough : TextDecorations.None)

// converter parametresiyle func'lar
new Label()
    .Bind<Label, double, string, string>(Label.TextProperty, "Total",
        convert: (total, currency) => $"{total:F2} {currency}",
        converterParameter: "₺")
```

## Yaklaşım Seçimi

| Durum | Öneri |
|---|---|
| Tek yerde kullanılan tek seferlik dönüşüm | Satır içi `Convert(...)` lambda |
| Ayrıştırma içeren iki yönlü düzenleme | `Convert` + `ConvertBack` |
| Birçok sayfada yeniden kullanılan / DI veya durum gerektiren | `IValueConverter` sınıfı + `.Converter(...)` |
| `MultiBinding` içinde dönüşüm | `IMultiValueConverter` — bkz. [MultiBinding](multi-binding.md) |
| VM özelliği olarak daha iyi ifade edilen karmaşık mantık | Converter'ı atlayın; view model'de hesaplanmış özellik sunun |

Son satır vurgulanmaya değer: UI'niz C# olduğundan, hesaplanmış bir view-model özelliği (`public Color StatusColor => …`) çoğu zaman her converter'dan daha basit ve test edilebilirdir.

## İlgili Konular

- [Özellik Bağlama](data-binding.md)
- [MultiBinding](multi-binding.md) — `IMultiValueConverter`, `AllTrueMultiConverter` tarzı desenler
- [Derlenmiş Binding'ler](compiled-bindings.md)
