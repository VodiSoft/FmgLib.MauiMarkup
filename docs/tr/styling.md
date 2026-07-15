# Uygulama Stilleri

FmgLib.MauiMarkup, XAML `<Style>` öğelerini güçlü tipli **`Style<T>`** sınıfıyla değiştirir. Bir stilin içinde **kontrollerde kullandığınız aynı fluent özellik metotları** setter'ları tanımlar — öğrenilecek tek API, kullanılacak iki bağlam.

## Stil Tanımlama

```csharp
new Style<Button>(e => e
    .TextColor(e => e.OnLight(Colors.White).OnDark(AppColors.Primary))
    .BackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(Colors.White))
    .FontFamily("OpenSansRegular")
    .FontSize(14)
    .CornerRadius(8)
    .Padding(new Thickness(14, 10))
    .MinimumHeightRequest(44)
    .MinimumWidthRequest(44))
```

Lambda bir `SettersContext<Button>` alır; her çağrı stile bir `Setter` ekler. Tema (`OnLight`/`OnDark`), platform, idiom ve `DynamicResource` builder'ları setter'ların içinde de aynen canlı kontrollerdeki gibi çalışır ([Fluent Özellikler](fluent-properties.md)).

`Style<T>`, `Microsoft.Maui.Controls.Style`'a örtük dönüşür; MAUI'nin stil beklediği her yere konabilir.

## Stilleri Uygulama

### Örtük — ResourceDictionary ile

Kaynaklara eklenen stil, kapsamdaki **hedef tipin tüm kontrollerine** uygulanır:

```csharp
// Uygulama geneli (App.cs)
this.Resources(
    new ResourceDictionary
    {
        new Style<Button>(e => e.BackgroundColor(AppColors.Primary).CornerRadius(8)),
        new Style<Frame>(e => e
            .HasShadow(false)
            .BorderColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray950))
            .CornerRadius(8)),
        new Style<Label>(e => e.TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.White))),
    }
);
```

Sayfa ve hatta layout düzeyinde `Resources(...)` kapsamı XAML'deki gibi çalışır.

### Açık — kontrol başına

```csharp
var dangerButton = new Style<Button>(e => e
    .BackgroundColor(Colors.Red)
    .TextColor(Colors.White));

new Button().Text("Sil").Style(dangerButton)
```

Yaygın organizasyon deseni — statik stiller sınıfı:

```csharp
public static class AppStyles
{
    public static Style<Button> Primary { get; } = new(e => e
        .BackgroundColor(AppColors.Primary)
        .TextColor(Colors.White)
        .CornerRadius(8));

    public static Style<Button> Secondary { get; } = new(e => e
        .BackgroundColor(Colors.Transparent)
        .TextColor(AppColors.Primary));

    public static ResourceDictionary Default { get; } = new()
    {
        Primary, Secondary,
        new Style<Entry>(e => e.FontSize(16)),
    };
}

// App içinde:
this.Resources(new ResourceDictionary().MergedDictionaries(AppStyles.Default));

// kontrol başına:
new Button().Text("Kaydet").Style(AppStyles.Primary)
```

## Constructor Seçenekleri

`Style<T>`, XAML stil özelliklerini yansıtan constructor'lara sahiptir:

```csharp
// kalıtım
var baseText  = new Style<Label>(e => e.FontFamily("OpenSansRegular").FontSize(14));
var heading   = new Style<Label>(baseText, e => e.FontSize(24).FontAttributes(FontAttributes.Bold));

// türetilmiş tiplere uygula (örn. Button ve alt sınıfları)
var allButtons = new Style<Button>(applyToDerivedTypes: true, e => e.CornerRadius(8));

// ikisi birden
var special = new Style<Button>(basedOn: allButtons, applyToDerivedTypes: true, e => e.FontSize(16));
```

| Constructor parametresi | XAML karşılığı |
|---|---|
| `basedOn` | `BasedOn="{StaticResource …}"` |
| `applyToDerivedTypes` | `ApplyToDerivedTypes="True"` |
| `buildSetters` lambda'sı | `<Setter>` listesi |

## Trigger, Visual State ve Aksiyon Ekleme

`Style<T>` koleksiyon başlatıcı sözdizimini destekler; şunları ekleyebilirsiniz:

- `Setter` nesneleri (`SomeProperty.Set(value)` ile)
- `TriggerBase` nesneleri ([Trigger'lar](triggers.md))
- `VisualStateGroup` / `VisualState<T>` nesneleri ([Visual State'ler](visual-states.md))
- `Action<T>` — stillenen her kontrole karşı çalıştırılan rastgele kod

```csharp
new ResourceDictionary
{
    new Style<Button>(e => e
        .TextColor(e => e.OnLight(Colors.White).OnDark(AppColors.Primary))
        .BackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(Colors.White))
        .FontSize(14)
        .CornerRadius(8))
    {
        new VisualState<Button>(VisualStates.Button.Normal, e => e
            .TextColor(e => e.OnLight(Colors.White).OnDark(AppColors.Primary))
            .BackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(Colors.White))),

        new VisualState<Button>(VisualStates.Button.Disabled, e => e
            .TextColor(e => e.OnLight(AppColors.Gray950).OnDark(AppColors.Gray200))
            .BackgroundColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray600))),
    },
};
```

Başlatıcı formunda ham setter ve trigger'ları karıştırma:

```csharp
new Style<Entry>
{
    Entry.BackgroundColorProperty.Set(Colors.Black),
    Entry.TextColorProperty.Set(Colors.White),

    new Trigger(typeof(Entry))
        .Property(Entry.IsFocusedProperty)
        .Value(true)
        .Setters(new Setters<Entry>(e => e
            .BackgroundColor(Colors.Yellow)
            .TextColor(Colors.Black))),
}
```

## Tema Stratejileri

Üç tamamlayıcı araç; birlikte kullanın:

1. **Setter'larda `OnLight`/`OnDark`** — otomatik işletim sistemi teması tepkisi:

   ```csharp
   new Style<Label>(e => e.TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.White)))
   ```

2. **Dinamik kaynaklar** — kullanıcı seçimli çalışma zamanı temaları:

   ```csharp
   new Style<Button>(e => e.BackgroundColor(e => e.DynamicResource("AccentColor")))

   // tema değişimi:
   Application.Current!.Resources["AccentColor"] = Colors.Purple;
   ```

3. **Merged dictionary'ler** — tüm stil setlerini değiştirme:

   ```csharp
   this.Resources(new ResourceDictionary().MergedDictionaries(
       isCompact ? CompactStyles.Default : ComfortableStyles.Default));
   ```

## Stillerde Özel Genişletme Metotları

Kendi fluent metotlarınız `SettersContext<T>` overload'larını uygularsa stillere katılır — dört overload'lı tam şablon [Özel Genişletme Metotları](custom-extension-methods.md) sayfasında:

```csharp
new Style<Label>(e => e
    .FontSize(20)          // yerleşik
    .MyBrandTypography())  // sizin
```

## İlgili Konular

- [Visual State'ler](visual-states.md)
- [Trigger'lar](triggers.md)
- [Fluent Özellikler](fluent-properties.md)
- [Özel Genişletme Metotları](custom-extension-methods.md)
