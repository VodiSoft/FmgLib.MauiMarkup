# Yerleşim Seçenekleri

`HorizontalOptions` ve `VerticalOptions` ayarlamak MAUI'nin en tekrarlı işlerinden biridir. FmgLib.MauiMarkup her kombinasyonu, herhangi bir `View` üzerinde tek ve okunabilir bir metoda indirger.

## Hızlı Örnek

```csharp
new StackLayout()
.Children(
    new Label().Text("Merhaba, Dünya!").Center()
)
```

`Center()`, hem `HorizontalOptions` hem `VerticalOptions`'ı `LayoutOptions.Center` yapar.

## Tam Metot Referansı

Adları, `(dikey, yatay)` değerlerinden oluşan 4×4'lük bir tablo gibi düşünün; yaygın durumlar için kısayollar vardır.

### Tek eksen yardımcıları

| Metot | Ayarladığı |
|---|---|
| `CenterHorizontal()` | `HorizontalOptions = Center` |
| `CenterVertical()` | `VerticalOptions = Center` |
| `Center()` | ikisi de `Center` |
| `AlignLeft()` | `HorizontalOptions = Start` |
| `AlignRight()` | `HorizontalOptions = End` |
| `AlignTop()` | `VerticalOptions = Start` |
| `AlignBottom()` | `VerticalOptions = End` |
| `FillHorizontal()` | `HorizontalOptions = Fill` |
| `FillVertical()` | `VerticalOptions = Fill` |
| `FillBothDirections()` | ikisi de `Fill` |

### İki eksen kombinasyonları

| Metot | Dikey | Yatay |
|---|---|---|
| `AlignTopLeft()` | Start | Start |
| `AlignTopCenter()` | Start | Center |
| `AlignTopRight()` | Start | End |
| `AlignTopFill()` | Start | Fill |
| `AlignCenterLeft()` | Center | Start |
| `AlignCenterRight()` | Center | End |
| `AlignCenterFill()` | Center | Fill |
| `AlignBottomLeft()` | End | Start |
| `AlignBottomCenter()` | End | Center |
| `AlignBottomRight()` | End | End |
| `AlignBottomFill()` | End | Fill |
| `AlignFillLeft()` | Fill | Start |
| `AlignFillCenter()` | Fill | Center |
| `AlignFillRight()` | Fill | End |

### Genel form

Yukarıda olmayan (veya çalışma zamanında hesaplanan) her kombinasyon:

```csharp
new Label().AlignLayout(vertical: LayoutOptions.End, horizontal: LayoutOptions.Center)
```

Ham özelliklere de her zaman dönebilirsiniz:

```csharp
new Label()
    .HorizontalOptions(LayoutOptions.Start)
    .VerticalOptions(LayoutOptions.Fill)
```

## Çalışan Örnek

Alta sabitlenmiş, içeriği ortalanmış, sağa yaslanmış ayar ikonlu bir alt çubuk:

```csharp
new Grid()
.Children(
    new Label()
        .Text("Hazır.")
        .AlignBottomCenter(),

    new ImageButton()
        .Source("gear.png")
        .SizeRequest(28, 28)
        .AlignBottomRight()
        .Margin(0, 0, 12, 12)
)
```

Sayfada ortalanmış bir hero kartı:

```csharp
this.Content(
    new Border()
        .SizeRequest(300, 180)
        .Center()                    // sayfada ortalanır
        .Content(
            new VerticalStackLayout()
            .Center()                // stack border içinde ortalanır
            .Children(
                new Label().Text("Hoş geldiniz").FontSize(28).CenterHorizontal(),
                new Label().Text("Devam etmek için giriş yapın").CenterHorizontal()
            )
        )
);
```

## Notlar ve Tuzaklar

- **Yerleşim seçenekleri ile metin hizalama farklıdır.** `Center()`, *view'ı ebeveyni içinde* konumlandırır. *Label'ın içindeki metni* ortalamak için `TextCenter()` kullanın — bkz. [Metin Hizalama](text-alignment.md). Sıkça birlikte kullanılırlar:

  ```csharp
  new Label().Text("Başlık").FillHorizontal().TextCenter()
  ```

- **Grid çocukları varsayılan olarak Fill'dir.** Bir `Grid` içinde çocuk, seçenek verilmedikçe hücresini doldurur — hücre içinde konumlandırmak için bu yardımcıları kullanın.
- **`Expand` seçenekleri MAUI'de kalktı.** Kütüphane bilinçli olarak yalnızca `Start/Center/End/Fill` sunar; genişleme davranışı için `Grid` star boyutlandırması veya `FlexLayout` grow katsayıları kullanın.
- Bu yardımcılar **her `View`'a** uygulanır — layout'lar dahil (`new VerticalStackLayout().Center()` tüm stack'i ortalar).

## İlgili Konular

- [Metin Hizalama](text-alignment.md)
- [Grid](grid.md)
- [Fluent Özellikler](fluent-properties.md)
