# Fluent Özellikler

Her kontrolün her bindable özelliği, bir fluent genişletme metodu ailesi alır. Bu aileyi anlamak — her zaman aynı dört kalıptır — kütüphanenin tamamının kilidini açar; çünkü kalıp, source generator'ın işlediği [üçüncü parti kontroller](third-party-controls.md) dahil tüm kontrollerde tekrarlanır.

## Dört Overload Kalıbı

`Label.FontSize` gibi bir özellik için kütüphane şunları sağlar:

```csharp
// 1. Doğrudan değer — zamanın %90'ında kullandığınız
public static T FontSize<T>(this T self, double fontSize) where T : Label;

// 2. Property builder — binding'ler, tema/platform/idiom değerleri, dinamik kaynaklar
public static T FontSize<T>(this T self,
    Func<PropertyContext<double>, IPropertyBuilder<double>> configure) where T : Label;

// 3. Stil setter'ı — Style<T> tanımları içinde kullanılır
public static SettersContext<T> FontSize<T>(this SettersContext<T> self, double fontSize) where T : Label;

// 4. Builder'lı stil setter'ı — stiller içinde binding'ler vb.
public static SettersContext<T> FontSize<T>(this SettersContext<T> self,
    Func<PropertySettersContext<double>, IPropertySettersBuilder<double>> configure) where T : Label;
```

Ek olarak `double`, `Color` gibi tipteki özellikler bir **animasyon yardımcısı** alır (bkz. [Animasyonlar](animations.md)):

```csharp
Task<bool> AnimateFontSizeTo(double value, uint length = 250, Easing? easing = null);
```

3–4 numaralı kalıpları asla doğrudan çağırmazsınız; `new Style<T>(e => e...)` içinde özellik çağrısı yazdığınızda otomatik devreye girerler — aynı metot adları her iki bağlamda da çalışır.

## Kalıp 1 — Doğrudan Değerler

```csharp
new Label()
    .Text("Bu bir test")
    .Padding(20)
    .FontSize(30)
    .Center()
```

Tüm metotlar `T` üzerinde generic olduğundan ve `T` döndürdüğünden, zincirleme somut kontrol tipini korur: `.Text(...)` sonrasında elinizde hâlâ bir `Label` vardır, `Label`'a özgü metotlar kullanılabilir kalır.

## Kalıp 2 — Property Builder (`e => e…`)

Lambda overload'u bir `PropertyContext<TValue>` alır (geleneksel adıyla `e`) ve *sabit değer olmayan* her şeyin kapısıdır:

### Binding'ler

```csharp
new Label().Text(e => e.Path("UserName"));                       // BindingContext.UserName'e bağla
new Label().Text(e => e.Path("Value").Source(slider));           // başka bir kontrole bağla
new Label().Text(e => e.Path("Price").StringFormat("{0:C}"));    // biçimlendir
new Entry().Text(e => e.Path("Query").BindingMode(BindingMode.TwoWay));
```

Tüm binding yetenekleri (converter'lar, fallback değerleri, multi-binding, derlenmiş getter binding'leri) şu dokümanlarda: [Özellik Bağlama](data-binding.md), [Binding Converter'ları](binding-converters.md), [MultiBinding](multi-binding.md) ve [Derlenmiş Binding'ler](compiled-bindings.md).

### Uygulama teması değerleri (`{AppThemeBinding}`)

```csharp
new Label()
    .TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.Teal))
```

| Metot | Uygulanma koşulu |
|---|---|
| `OnLight(value)` | Tema Light iken |
| `OnDark(value)` | Tema Dark iken |
| `Default(value)` | Yedek / belirtilmemiş tema |

Kullanıcı işletim sistemi temasını değiştirdiğinde değer canlı olarak güncellenir — bu gerçek bir `AppThemeBinding`'dir, tek seferlik kontrol değildir.

### Cihaz idiom değerleri (`{OnIdiom}`)

```csharp
new Label()
    .FontSize(e => e.OnDesktop(80).OnPhone(30).Default(50))
```

Mevcut: `OnPhone`, `OnTablet`, `OnDesktop`, `OnTV`, `OnWatch`, `Default`.

### Platform değerleri (`{OnPlatform}`)

```csharp
new Label()
    .Margin(e => e.OniOS(new Thickness(0, 20, 0, 0)).Default(new Thickness(0)))
```

Mevcut: `OnAndroid`, `OniOS`, `OnMacCatalyst`, `OnWinUI`, `OnTizen`, `Default`.

### Dinamik kaynaklar (`{DynamicResource}`)

```csharp
new Frame()
    .BackgroundColor(e => e.DynamicResource("CardBackground"))
```

Özellik kaynak anahtarını takip eder: çalışma zamanında `Resources["CardBackground"]`'ı değiştirin, UI güncellenir.

### Yerelleştirme

```csharp
new Label().Text(e => e.Translate("Hello"));        // JSON yerelleştirme
new Label().Text(e => e.TranslateResx("Hello"));    // RESX yerelleştirme
```

Bunlar da canlı binding'lerdir — `Translator.Instance.ChangeCulture(...)` çağrısı bağlı tüm özellikleri anında yeniden çevirir. Bkz. [Yerelleştirme (JSON)](localization-json.md) ve [Yerelleştirme (RESX)](localization-resx.md).

### Builder'ları iç içe kullanma

Idiom/tema/platform builder'ları birleşir: her dal, ham değer yerine kendisi bir builder alabilir.

```csharp
new Label()
    .TextColor(e => e
        .OnLight(Colors.Black)
        .OnDark(l => l.DynamicResource("DarkAccent")))   // karanlık tema dinamik kaynak kullanıyor
```

## Kalıplar 3–4 — Stillerin İçinde

Aynı metot adları `Style<T>` tanımında da çalışır; orada canlı bir kontrole dokunmak yerine XAML `Setter`'ları üretirler:

```csharp
new Style<Button>(e => e
    .TextColor(e => e.OnLight(Colors.White).OnDark(AppColors.Primary))
    .BackgroundColor(AppColors.Primary)
    .FontSize(14)
    .CornerRadius(8)
    .Padding(new Thickness(14, 10)))
```

Tam anlatım için bkz. [Stiller](styling.md).

## Bilinmesi Gereken Kolaylık Kısayolları

1:1 özellik eşlemesinin ötesinde kütüphane birleşik setter'lar ekler:

| Metot | Karşılığı |
|---|---|
| `.SizeRequest(w, h)` / `.SizeRequest(size)` | `WidthRequest` + `HeightRequest` |
| `.Margin(10)` / `.Margin(h, v)` / `.Margin(l, t, r, b)` | Hazır `Thickness` ile `Margin` |
| `.Padding(...)` (aynı overload'lar) | `Padding` |
| `.Center()`, `.FillHorizontal()`, `.AlignTopRight()`, … | `HorizontalOptions`/`VerticalOptions` kombinasyonları — bkz. [Yerleşim Seçenekleri](layout-options.md) |
| `.TextCenter()`, `.TextTopLeft()`, … | `HorizontalTextAlignment`/`VerticalTextAlignment` kombinasyonları — bkz. [Metin Hizalama](text-alignment.md) |
| `.GridSpan(column: 2, row: 1)` | `Grid.ColumnSpan` + `Grid.RowSpan` |
| `.AbsoluteLayoutBounds(x, y, w, h)` | `Rect` kurmadan `AbsoluteLayout.LayoutBounds` |

Ayrıca sıradan tipler üzerinde küçük yardımcılar vardır:

```csharp
"#FF3366".ToColor()          // string → Color (hex; ToColorFromArgb / ToColorFromRgba varyantlarıyla)
```

## Kapsanmayan Bir Şeyi Ayarlamak

İki kaçış yolu asla tıkanmamanızı garanti eder:

**`InvokeOnElement`** — zincirin ortasında kontrole karşı rastgele kod çalıştırın:

```csharp
new Entry()
    .Placeholder("Ad")
    .InvokeOnElement(entry => entry.ReturnCommand = someCommand)
```

**Saf C#** — view'lar sıradan nesneler olduğundan sonradan atama her zaman mümkündür:

```csharp
var entry = new Entry().Placeholder("Ad");
entry.ReturnType = ReturnType.Done;
```

## İlgili Konular

- [Özellik Bağlama](data-binding.md) — builder'ın binding yetenekleri derinlemesine
- [Stiller](styling.md) — setters context
- [Özel Genişletme Metotları](custom-extension-methods.md) — aynı dört kalıbı kendi özelliklerinize ekleyin
