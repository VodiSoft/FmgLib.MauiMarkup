# Kullanıcı Tanımlı Genişletme Metotları

FmgLib.MauiMarkup'ı kendi fluent metotlarınızla genişletebilirsiniz — ya mevcut metotlardan kurulan **anlamsal kısayollar**, ya da yerleşiklerle birebir aynı şekilde binding'lere, stillere ve animasyonlara katılan **tam özellik metotları**.

## Seviye 1 — Kompozisyon Kısayolları

En basit ve en yararlı tür: tekrar eden zincirleri statik bir metotta sarın. Kütüphane makinesine gerek yok, çünkü her fluent metot `T` döndürür:

```csharp
public static class MarkupHelpers
{
    public static T PrimaryText<T>(this T self) where T : Label
        => self.FontSize(16).TextColor(AppColors.TextPrimary).FontFamily("OpenSansRegular");

    public static T Card<T>(this T self) where T : Border
        => self
            .BackgroundColor(e => e.OnLight(Colors.White).OnDark("#1E1E2E".ToColor()))
            .StrokeThickness(0)
            .Padding(16)
            .Shadow(new Shadow().Radius(10).Opacity(0.15f));
}

// kullanım:
new Label().Text("Toplam").PrimaryText()
new Border().Card().Content(/* ... */)
```

Somut tipin zincir boyunca akması için `<T>` generic + `where T : ...` kısıt desenini koruyun.

## Seviye 2 — Tam Özellik Metotları (dört overload şablonu)

Bir özelliğin native FmgLib özelliği gibi davranması için — doğrudan değerleri, [builder lambda'sını](fluent-properties.md) (binding'ler, `OnLight/OnDark`, `DynamicResource`…) ve [stilleri](styling.md) desteklemesi için — dört standart overload'u uygulayın. İşte `Label.FontSize` için tam şablon (generator'ın ürettiğinin aynısı):

```csharp
public static class MyLabelExtensions
{
    // 1. Doğrudan değer
    public static T FontSize<T>(this T self, double fontSize)
        where T : Microsoft.Maui.Controls.Label
    {
        self.SetValue(Microsoft.Maui.Controls.Label.FontSizeProperty, fontSize);
        return self;
    }

    // 2. Property builder — e.Path(...), e.OnLight(...), e.DynamicResource(...) vb. sağlar
    public static T FontSize<T>(this T self,
        Func<PropertyContext<double>, IPropertyBuilder<double>> configure)
        where T : Microsoft.Maui.Controls.Label
    {
        var context = new PropertyContext<double>(self, Microsoft.Maui.Controls.Label.FontSizeProperty);
        configure(context).Build();
        return self;
    }

    // 3. Stil setter'ı
    public static SettersContext<T> FontSize<T>(this SettersContext<T> self, double fontSize)
        where T : Microsoft.Maui.Controls.Label
    {
        self.XamlSetters.Add(new Setter
        {
            Property = Microsoft.Maui.Controls.Label.FontSizeProperty,
            Value = fontSize
        });
        return self;
    }

    // 4. Builder'lı stil setter'ı
    public static SettersContext<T> FontSize<T>(this SettersContext<T> self,
        Func<PropertySettersContext<double>, IPropertySettersBuilder<double>> configure)
        where T : Microsoft.Maui.Controls.Label
    {
        var context = new PropertySettersContext<double>(self.XamlSetters,
            Microsoft.Maui.Controls.Label.FontSizeProperty);
        configure(context).Build();
        return self;
    }
}
```

Bileşenler:

- **`PropertyContext<TValue>`** — kontrolü `BindableProperty` ile eşler; lambda'nızın döndürdüğü builder onu nasıl bağlayacağını/ayarlayacağını bilir.
- **`SettersContext<T>`** — bir `Style<T>` tanımı içindeki `Setter`'ları toplar.
- Stiller/binding'ler/trigger'lar çalışmaya devam etsin diye her zaman `BindableProperty`'yi `SetValue` ile hedefleyin (CLR özelliğini değil).

Dördü de yerinde olunca her kullanım deseni aktifleşir:

```csharp
new Label().FontSize(28);                                          // değer
new Label().FontSize(e => e.Path("MyFontSize").Source(myModel));   // binding
new Label().FontSize(e => e.OnPhone(30).OnTablet(50).Default(40)); // idiom
new Style<Label>(e => e.FontSize(20).CenterVertically());          // stil
```

## Seviye 3 — Animasyon Yardımcıları

Özelliğiniz için kütüphanenin `Transformations` yardımcısıyla bir `Animate…To` ekleyin (bkz. [Animasyonlar](animations.md)):

```csharp
public static Task<bool> AnimateFontSizeTo<T>(this T self, double value,
    uint length = 250, Easing? easing = null)
    where T : Microsoft.Maui.Controls.Label
{
    double fromValue = self.FontSize;
    var transform = (double t) => Transformations.DoubleTransform(fromValue, value, t);
    var callback = (double actValue) => { self.FontSize = actValue; };
    return Transformations.AnimateAsync<double>(self, "AnimateFontSizeTo", transform, callback, length, easing);
}
```

```csharp
await titleLabel.AnimateFontSizeTo(40, 300, Easing.CubicOut);
```

## Manuel Genişletme mi, Generator mı?

| Durum | Yaklaşım |
|---|---|
| `BindableProperty`/olaylı üçüncü parti kontrol | `[MauiMarkup(typeof(...))]` — [generator](third-party-controls.md) tüm overload'ları yazar |
| Kendi `BindableObject` kontrolünüz | Yine generator — uygulama projesinden işaretleyin |
| Düz CLR özelliği (`BindableProperty` yok) | Manuel genişletme (yalnızca 1. şekil anlamlı — `BindableProperty` olmadan binding desteği yok) |
| Tasarım-sistemi kısayolları (`PrimaryText`, `Card`) | Seviye 1 kompozisyon |
| Çok özellikli kolaylık (`SizeRequest` tarzı) | Seviye 1 kompozisyon |
| Özel animasyonlar | Seviye 3 |

## Tasarım İpuçları

- **Aksiyonu değil, özelliği adlandırın**: `CornerRadius(8)`, `SetCornerRadius(8)` değil — API'nin geri kalanıyla tutarlılık.
- Paylaşılan yardımcıları kontrol ailesi başına özel statik sınıflara koyun (`LabelHelpers`, `LayoutHelpers`); ekstra `using` olmadan görünmelerini istiyorsanız `namespace FmgLib.MauiMarkup` bloğunun içine — veya netlik için kendi ad alanınızda tutun.
- `T` (generic) döndürün, somut taban tipini asla — yoksa türemiş kontroller için zincirleri kırarsınız.

## İlgili Konular

- [Fluent Özellikler](fluent-properties.md) — overload sözleşmesi
- [Üçüncü Parti Kontroller](third-party-controls.md) — bu işi generator'a yaptırmak
- [Animasyonlar](animations.md)
