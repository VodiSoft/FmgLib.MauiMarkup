# Animasyonlar

FmgLib.MauiMarkup, her kontrolün her animasyona uygun bindable özelliği (`double`, `Color` ve diğer interpolasyona uygun tipler) için bir **`Animate<Property>To`** yardımcısı üretir — MAUI'nin yerleşik `TranslateTo`, `FadeTo`, `ScaleTo`, `RotateTo` metotları da elbette çalışmaya devam eder.

## Üretilen `Animate…To` Metotları

Kalıp:

```csharp
Task<bool> Animate<PropertyName>To(T value, uint length = 250, Easing? easing = null)
```

Kutudan gelen örnekler:

```csharp
await label.AnimateFontSizeTo(40);                                  // double
await box.AnimateBackgroundColorTo(Colors.Teal, length: 500);       // Color
await border.AnimateOpacityTo(0, easing: Easing.CubicOut);          // double
await view.AnimateHeightRequestTo(320);                             // double
await view.AnimateSizeRequestTo(200, 120);                          // genişlik + yükseklik birlikte
await progressBar.AnimateProgressTo(0.8);
```

Dönen `Task<bool>` animasyon bitince tamamlanır (iptal edildiyse `true` — MAUI'nin animasyon konvansiyonuyla uyumlu), böylece animasyonlar `async/await` ile birleşir:

```csharp
new Button()
    .Text("Kaydet")
    .OnClicked(async b =>
    {
        await b.AnimateBackgroundColorTo(Colors.Green, 200);
        await Task.Delay(400);
        await b.AnimateBackgroundColorTo(AppColors.Primary, 200);
    })
```

Bunlar arka planda kontrolün animasyon yöneticisi üzerinden kütüphanenin `Transformations.AnimateAsync` yardımcısıyla çalışır; geçerli değerden hedefe interpolasyon yapar — bu mekanizmayı [özel genişletme metotlarında](custom-extension-methods.md) yeniden kullanabilirsiniz.

## Animasyonları Birleştirme

**Sıralı** — birini diğerinden sonra bekleyin:

```csharp
await image.AnimateOpacityTo(0, 150);
image.Source = "next.png";
await image.AnimateOpacityTo(1, 150);
```

**Paralel** — birlikte başlatın, hepsini bekleyin:

```csharp
await Task.WhenAll(
    card.AnimateBackgroundColorTo(Colors.LightYellow, 300),
    card.TranslateTo(0, -8, 300, Easing.CubicOut),
    card.AnimateOpacityTo(1, 300)
);
```

## MAUI Yerleşikleri Hâlâ Geçerli

Tüm standart `ViewExtensions` animasyonları markup ile kurulan view'larda çalışır:

```csharp
new Image()
    .Source("bell.png")
    .Assign(out var bell)
    .GestureRecognizers(new TapGestureRecognizer().OnTapped(async (s, e) =>
    {
        await bell.RotateTo(15, 60);
        await bell.RotateTo(-15, 120);
        await bell.RotateTo(0, 60);
    }))
```

Tam keyframe kontrolü için `Microsoft.Maui.Controls.Animation` mevcut kalır:

```csharp
new Animation(v => box.Scale = v, 1, 1.4)
    .Commit(box, "pulse", length: 400, easing: Easing.SinInOut,
            repeat: () => true);
```

## Yaşam Döngüsü Olaylarıyla Giriş Animasyonları

[Olay işleyicileri](event-handlers.md) animasyonlarla birleştirip sayfa/view girişleri yapın:

```csharp
new VerticalStackLayout()
    .Opacity(0)
    .TranslationY(24)
    .OnLoaded(async v =>
    {
        await Task.WhenAll(
            v.AnimateOpacityTo(1, 350),
            v.TranslateTo(0, 0, 350, Easing.CubicOut));
    })
    .Children(/* ... */)
```

## Visual State İçinde Animasyonlar

[`VisualState<T>`](visual-states.md), state'e girişte çalışan async aksiyonlar kabul eder — etkileşim animasyonlarının bildirimsel yeri:

```csharp
new VisualState<Button>(VisualStates.Button.Pressed)
{
    async b => await b.ScaleTo(0.96, 80)
},
new VisualState<Button>(VisualStates.Button.Normal)
{
    async b => await b.ScaleTo(1, 80)
}
```

## Kendi `Animate…To`'nuzu Yazmak

Generator'ın kullandığı şablon public API'dir — özel özellikler için yeniden kullanın:

```csharp
public static Task<bool> AnimateCornerRadiusTo<T>(this T self, int value,
    uint length = 250, Easing? easing = null)
    where T : Button
{
    double from = self.CornerRadius;
    var transform = (double t) => Transformations.DoubleTransform(from, value, t);
    var callback = (double v) => { self.CornerRadius = (int)v; };
    return Transformations.AnimateAsync<double>(self, "AnimateCornerRadiusTo", transform, callback, length, easing);
}
```

Tam kalıp için bkz. [Özel Genişletme Metotları](custom-extension-methods.md).

## Performans İpuçları

- Mümkünse **transform özelliklerini** (`TranslationX/Y`, `Scale`, `Rotation`, `Opacity`) animasyonlayın — yeniden yerleşimden kaçınırlar. `Animate…RequestTo` boyut animasyonları her karede yerleşim tetikler; kısa ve küçük tutun.
- Hot-reload etkin sayfalarda animasyonu `Build()` içinde başlatmayın — `Build()` her reload'da yeniden çalışır. `OnLoaded`/`OnAppearing` kullanın.
- Sayfa kaybolunca uzun döngüleri iptal edin (`this.OnDisappearing(p => p.AbortAnimation("pulse"))`).

## İlgili Konular

- [Visual State'ler](visual-states.md)
- [Olay İşleyiciler](event-handlers.md)
- [Özel Genişletme Metotları](custom-extension-methods.md)
