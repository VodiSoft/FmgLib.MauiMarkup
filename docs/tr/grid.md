# Grid Tanımı

`Grid`, MAUI'nin en çok iş gören layout'udur. FmgLib.MauiMarkup onun iki hantal kısmını — satır/sütun tanımları ve çocuk yerleştirme — kısa ve tip güvenli hâle getirir.

## Satır ve Sütun Tanımları

`RowDefinitions` / `ColumnDefinitions`, üç metotlu bir builder lambda'sı alır:

| Builder metodu | Anlamı | XAML karşılığı |
|---|---|---|
| `Star(double value = 1.0, int count = 1)` | Kalan alandan oransal pay | `2*`, `*` |
| `Auto(int count = 1)` | İçeriğe göre boyut | `Auto` |
| `Absolute(double value, int count = 1)` | Sabit birim | `100` |

İsteğe bağlı `count` parametresi tanımı tekrarlar — tekdüze grid'lerde kopyala-yapıştırı ortadan kaldırır.

```csharp
new Grid()
.RowDefinitions(e => e.Star(2).Star(0.5, count: 3))
.ColumnDefinitions(e => e.Absolute(100).Star())
.Children(
    // ...
)
```

Bu şunları oluşturur:

- **4 satır** — ilki 2 "star" alır (1-star satırın iki katı alan); 2–4. satırlar 0.5'er star.
- **2 sütun** — sabit 100 birimlik bir sütun, ardından kalan genişliği alan bir sütun.

Karşılaştırma için eşdeğer XAML:

```xml
<Grid RowDefinitions="2*,0.5*,0.5*,0.5*" ColumnDefinitions="100,*">
```

Karışık örnek:

```csharp
new Grid()
.RowDefinitions(e => e.Auto().Star().Auto())        // başlık / içerik / alt bilgi
.ColumnDefinitions(e => e.Star(3).Star(7))          // %30 / %70
```

## Çocukları Yerleştirme

Attached-property yardımcıları çocukları konumlandırır (bkz. [Attached Property'ler](attached-properties.md)):

| Metot | XAML karşılığı |
|---|---|
| `.Row(int)` | `Grid.Row` |
| `.Column(int)` | `Grid.Column` |
| `.RowSpan(int)` | `Grid.RowSpan` |
| `.ColumnSpan(int)` | `Grid.ColumnSpan` |
| `.GridSpan(column, row)` | iki span birden |

Satır ve sütun varsayılanı 0'dır; ilk hücre çağrı gerektirmez.

## Tam Örnek

Klasik 2×2 renkli grid:

```csharp
new Grid()
.RowDefinitions(e => e.Star(2).Star())
.ColumnDefinitions(e => e.Absolute(200).Star())
.Children(
    new BoxView().Color(Colors.Green),
    new Label().Text("Column 0, Row 0"),

    new BoxView().Color(Colors.Blue).Column(1).Row(0),
    new Label().Text("Column 1, Row 0").Column(1).Row(0),

    new BoxView().Color(Colors.Teal).Column(0).Row(1),
    new Label().Text("Column 0, Row 1").Column(0).Row(1),

    new BoxView().Color(Colors.Purple).Column(1).Row(1),
    new Label().Text("Column 1, Row 1").Column(1).Row(1)
)
```

Birden çok çocuk aynı hücreyi paylaşabilir (z-sırasında üst üste binerler; sonrakiler üstte çizilir). Sıralamayı `.ZIndex(int)` ile değiştirebilirsiniz.

## Gerçek Dünya Yerleşimi — ürün kartı

```csharp
new Grid()
.RowDefinitions(e => e.Star(1).Star(6).Star(2).Star(1))
.Padding(5)
.Children(
    // satır 0: favori ikonu + indirim rozeti
    new Grid()
    .Row(0)
    .ColumnDefinitions(e => e.Star(6).Star(4))
    .Children(
        new ImageButton().Source("heart.png").AlignLeft().SizeRequest(30, 30),
        new Frame().Column(1).CornerRadius(20).BackgroundColor(Colors.Red)
            .Content(new Label().Text("-50%").TextColor(Colors.White).Center())
    ),

    // satır 1: ürün görseli
    new Image().Source("product.png").SizeRequest(80, 80).Row(1).CenterHorizontal(),

    // satır 2: ad + fiyat
    new VerticalStackLayout().Row(2).Children(
        new Label().Text("Ekşi Mayalı Ekmek").FontAttributes(FontAttributes.Bold),
        new Label().Text("₺49,90").TextColor(Colors.Green)
    ),

    // satır 3: aksiyon
    new Button().Row(3).Text("Sepete ekle").HeightRequest(35)
)
```

## Boşluk ve Boyutlandırma

Tüm normal `Grid` özellikleri fluent olarak mevcuttur:

```csharp
new Grid()
.RowSpacing(8)
.ColumnSpacing(8)
.Padding(16)
.BackgroundColor(Colors.WhiteSmoke)
```

Ekstra kolaylık — iki boşluğu tek çağrıda ayarlayın:

```csharp
new Grid().Spacing(8)          // ColumnSpacing = RowSpacing = 8
new Grid().Spacing(12, 4)      // ColumnSpacing = 12, RowSpacing = 4
```

## Alternatif: Koleksiyon Tabanlı Tanımlar

Elinizde hazır tanım koleksiyonları varsa (veya açık nesneleri tercih ediyorsanız) overload'lar onları doğrudan kabul eder; aynı `Auto/Star/Absolute` yardımcıları `ColumnDefinitionCollection` / `RowDefinitionCollection` üzerinde de genişletme olarak vardır:

```csharp
new Grid()
.ColumnDefinitions(new ColumnDefinitionCollection().Absolute(100).Star())
```

## İpuçları

- **Tekdüze grid'ler için `count:` tercih edin:** `e => e.Star(1, count: 7)` tek çağrıda 7 satırlık takvim şeridi oluşturur.
- **`Auto` satırlar her geçişte içerik ölçer** — başlık/alt bilgi için kullanın, uzun listeler için değil (uzun listeyi `Star` satırdaki `CollectionView`'a koyun).
- **Tanımsız yerleştirilen çocuklar** tek örtük hücreye düşer — tanımsız `Grid`, pratik bir katman (overlay) konteyneridir:

  ```csharp
  new Grid().Children(
      new Image().Source("photo.png"),
      new Label().Text("Alt yazı").AlignBottomCenter().TextColor(Colors.White)
  )
  ```

## İlgili Konular

- [Yerleşim Seçenekleri](layout-options.md) — hücre içinde çocukları konumlandırma
- [Attached Property'ler](attached-properties.md)
