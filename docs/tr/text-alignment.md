# Metin Hizalama Yardımcıları (`ITextAlignment`)

`ITextAlignment` uygulayan her kontrol — `Label`, `Entry`, `Editor`, `SearchBar`, `Picker`, `TimePicker`, `DatePicker`, `EntryCell` ve arayüzü uygulayan her üçüncü parti kontrol — `HorizontalTextAlignment` ve `VerticalTextAlignment`'ı tek çağrıda ayarlayan bir genişletme metodu ailesi alır.

> Bu üretilen metotlar, kontrolün **her iki** metin hizalama bindable özelliğini de sunmasını gerektirir; source generator onları yalnızca ikisi de varsa üretir.

## Hızlı Örnek

```csharp
new Label().Text("Ortalı").TextCenter()
```

Metni, kontrolün kendi sınırları **içinde** hem yatay hem dikey ortalar.

## Metot Referansı

### Tek eksen

| Metot | Ayarladığı |
|---|---|
| `TextCenterHorizontal()` | `HorizontalTextAlignment = Center` |
| `TextCenterVertical()` | `VerticalTextAlignment = Center` |
| `TextLeft()` | `HorizontalTextAlignment = Start` |
| `TextRight()` | `HorizontalTextAlignment = End` |
| `TextTop()` | `VerticalTextAlignment = Start` |
| `TextBottom()` | `VerticalTextAlignment = End` |

### İki eksen

| Metot | Dikey | Yatay |
|---|---|---|
| `TextCenter()` | Center | Center |
| `TextTopLeft()` | Start | Start |
| `TextTopCenter()` | Start | Center |
| `TextTopRight()` | Start | End |
| `TextCenterLeft()` | Center | Start |
| `TextCenterRight()` | Center | End |
| `TextBottomLeft()` | End | Start |
| `TextBottomCenter()` | End | Center |
| `TextBottomRight()` | End | End |

### Genel form

```csharp
new Label().AlignText(vertical: TextAlignment.End, horizontal: TextAlignment.Center)
```

## Metin Hizalama ile Yerleşim Seçenekleri Farkı

Sık karışan bir nokta:

- **`TextCenter()`** *kontrolün kutusu içindeki metni* hizalar.
- **`Center()`** *kontrolü ebeveyni içinde* hizalar ([Yerleşim Seçenekleri](layout-options.md)).

`Label` içeriğine göre boyutlanmışsa `TextCenter()` etkisiz görünür — kutu tam metin kadardır. Kontrole alan verin, sonra hizalayın:

```csharp
// Metni ortalı, tam genişlikte bir başlık
new Label()
    .Text("Panel")
    .FontSize(24)
    .FillHorizontal()   // kontrol satırın tamamını alır
    .TextCenter()       // metin içinde ortalanır
    .HeightRequest(56)
```

## Pratik Örnekler

**Hesap makinesi gibi sağa dayalı sayısal giriş:**

```csharp
new Entry()
    .Keyboard(Keyboard.Numeric)
    .Placeholder("0.00")
    .TextRight()
```

**Tablo tarzı satırlar:**

```csharp
new Grid()
.ColumnDefinitions(e => e.Star(6).Star(2).Star(2))
.Children(
    new Label().Text("Ürün"),
    new Label().Text("Adet").Column(1).TextCenter(),
    new Label().Text("Fiyat").Column(2).TextRight()
)
```

**Sabit yükseklikli çubukta dikey ortalı label:**

```csharp
new Label()
    .Text("Durum: çevrimiçi")
    .HeightRequest(44)
    .TextCenterVertical()
```

## İlgili Konular

- [Yerleşim Seçenekleri](layout-options.md)
- [Fluent Özellikler](fluent-properties.md)
