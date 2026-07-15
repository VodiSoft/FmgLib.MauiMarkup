# Visual State'ler

Visual State Manager, kontrolün görünümünü durumuna göre değiştirir — `Normal`, `Focused`, `Disabled`, `PointerOver`, `Pressed` vb. FmgLib.MauiMarkup bunu güçlü tipli **`VisualState<T>`** sınıfıyla sarar; stillere veya doğrudan kontrollere takılır, hatta **state'e giriş animasyonlarını** destekler.

## Visual State Tanımlama

`VisualState<T>`, state adını ve setter lambda'sını alır — her yerdeki aynı fluent özellik API'si:

```csharp
new VisualState<Button>(VisualStates.Button.Normal, e => e
    .TextColor(Colors.White)
    .BackgroundColor(AppColors.Primary))
```

### Yerleşik state adları — `VisualStates` yardımcısı

Sihirli string'ler yerine kütüphaneyle gelen sabitler sınıfını kullanın:

| Sınıf | Sabitler |
|---|---|
| `VisualStates.VisualElement` | `Normal`, `Disabled`, `Focused`, `PointerOver` |
| `VisualStates.Button` | + `Pressed` |
| `VisualStates.ImageButton` | + `Pressed` |
| `VisualStates.Switch` | + `On`, `Off` |
| `VisualStates.RadioButton` | + `Checked`, `Unchecked` |
| `VisualStates.CheckBox` | + `IsChecked` |
| `VisualStates.CollectionView` | + `Selected` |
| `VisualStates.CarouselView` | + `DefaultItem`, `CurrentItem`, `PreviousItem`, `NextItem` |

(Her kontrol sınıfı ortak `VisualElement` state'lerini miras alır; `VisualStates.Button.Focused` da geçerlidir.)

## Stilde Visual State'ler

En yaygın yerleşim — [`Style<T>`](styling.md) koleksiyon başlatıcısında, uygulama genelinde:

```csharp
new Style<Button>(e => e
    .FontSize(14)
    .CornerRadius(8))
{
    new VisualState<Button>(VisualStates.Button.Normal, e => e
        .TextColor(e => e.OnLight(Colors.White).OnDark(AppColors.Primary))
        .BackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(Colors.White))),

    new VisualState<Button>(VisualStates.Button.PointerOver, e => e
        .BackgroundColor(AppColors.PrimaryDark)),

    new VisualState<Button>(VisualStates.Button.Disabled, e => e
        .TextColor(e => e.OnLight(AppColors.Gray950).OnDark(AppColors.Gray200))
        .BackgroundColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray600))),
}
```

> **`Normal`'ı her zaman tanımlayın.** VSM yalnızca bir state'in ayarladığı özellikleri geri yükler; `Normal`'ın açık tanımı diğer state'lerden temiz dönüşü garanti eder.

## Doğrudan Kontrolde Visual State'ler

`VisualStateGroups` attached-property metodunu kullanın:

```csharp
new Entry()
    .Placeholder("E-posta")
    .VisualStateGroups(
        new VisualStateGroup
        {
            new VisualState<Entry>(VisualStates.VisualElement.Normal, e => e
                .BackgroundColor(Colors.White)),
            new VisualState<Entry>(VisualStates.VisualElement.Focused, e => e
                .BackgroundColor(Colors.LightYellow)),
        }
    )
```

## Visual State İçinde Animasyonlar

`VisualState<T>` koleksiyon başlatıcısında `Action<T>` girdileri kabul eder — **state'e girildiğinde** çalışırlar; async MAUI animasyonları state geçişine dönüşür:

```csharp
new Style<Button>(e => e.FontSize(20))
{
    new VisualState<Button>(VisualStates.Button.Normal, e => e
        .FontSize(33)
        .TextColor(AppColors.Gray200))
    {
        async button => {
            await button.RotateTo(0);     // Normal'a girişte animasyon
        }
    },

    new VisualState<Button>(VisualStates.Button.Disabled, e => e
        .FontSize(20)
        .TextColor(AppColors.Gray600))
    {
        async button => {
            await button.RotateTo(180);   // Disabled'a girişte animasyon
        }
    },
}
```

Özellik düzeyi animasyonlar için kütüphanenin ürettiği [`Animate…To` yardımcılarıyla](animations.md) birleştirin:

```csharp
new VisualState<Button>(VisualStates.Button.PointerOver)
{
    async b => await b.AnimateBackgroundColorTo(Colors.DarkSlateBlue, length: 150)
}
```

## State Trigger'lar — koşullara bağlı state'ler

Bir `VisualState<T>`, kontrol etkileşimi yerine **state trigger'larla** da sürülebilir. Bu, duyarlı/adaptif yerleşimleri mümkün kılar:

```csharp
new VisualStateGroup
{
    new VisualState<Grid>("Wide", e => e.BackgroundColor(Colors.White))
    {
        new AdaptiveTrigger().MinWindowWidth(800)
    },
    new VisualState<Grid>("Narrow", e => e.BackgroundColor(Colors.WhiteSmoke))
    {
        new AdaptiveTrigger().MinWindowWidth(0)
    },
}
```

Fluent destekli state trigger'lar:

| Trigger | Aktifleşme koşulu |
|---|---|
| `AdaptiveTrigger` | Pencere boyutu `MinWindowWidth`/`MinWindowHeight` eşiğini geçince |
| `CompareStateTrigger` | Bağlanan `Property`, `Value`'ya eşit olunca |
| `DeviceStateTrigger` | Belirli `Device` (platform) üzerinde çalışırken |
| `OrientationStateTrigger` | Cihaz yönelimi eşleşince |
| `StateTrigger` | `IsActive` ayarlanınca (manuel kontrol) |

Örnek — yönelime bağlı yerleşim:

```csharp
new VisualStateGroup
{
    new VisualState<StackLayout>("Portrait", e => e.Orientation(StackOrientation.Vertical))
    {
        new OrientationStateTrigger().Orientation(DisplayOrientation.Portrait)
    },
    new VisualState<StackLayout>("Landscape", e => e.Orientation(StackOrientation.Horizontal))
    {
        new OrientationStateTrigger().Orientation(DisplayOrientation.Landscape)
    },
}
```

## Programatik State Değişimi

Standart MAUI geçerlidir:

```csharp
VisualStateManager.GoToState(myButton, "CustomState");
```

Özel state adları çalışır — kendi adınızla bir `VisualState<T>` tanımlayıp koddan tetikleyin.

## Visual State mi, Trigger mı?

| | Visual state'ler | [Trigger'lar](triggers.md) |
|---|---|---|
| Sürücü | Adlandırılmış kontrol durumları (+ state trigger'lar) | Özellik değerleri / binding'ler / olaylar |
| Karşılıklı dışlayıcı | Evet, grup içinde | Hayır |
| Animasyon desteği | Evet (aksiyon girdileri) | `EventTrigger` aksiyonlarıyla |
| En uygun | Etkileşim geri bildirimi, adaptif yerleşim | Veriye bağlı özellik değişimleri |

## İlgili Konular

- [Stiller](styling.md)
- [Animasyonlar](animations.md)
- [Trigger'lar](triggers.md)
