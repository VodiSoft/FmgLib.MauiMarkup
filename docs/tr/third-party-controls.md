# Üçüncü Parti Kontroller İçin Genişletmeler

FmgLib.MauiMarkup'ın fluent metotları bir **Roslyn source generator** tarafından üretilir; aynı generator *herhangi bir* kütüphanenin kontrolleri için de genişletme üretebilir — SkiaSharp, ZXing.Net.Maui, UraniumUI, InputKit, Syncfusion, DevExpress veya kendi kontrol kütüphaneniz… Üç mekanizma vardır:

1. `[MauiMarkup(typeof(...))]` — kontrol başına opt-in
2. `[MauiMarkupAttachedProp(...)]` — attached property'ler için
3. `MauiMarkupSourceGenerator` MSBuild özelliği — tamamen otomatik tarama

## 1. `[MauiMarkup]` — opt-in üretim

Attribute'ü projenizdeki **herhangi bir sınıfa** koyun (sınıfın kendisi önemsiz — yalnızca attribute için bir çapadır). Geçilen her tip için generator, o tipin **bindable özellikleri** ve **olayları** için fluent metotlar üretir.

```csharp
using FmgLib.MauiMarkup;

namespace GeneratedExam;

[MauiMarkup(typeof(ZXing.Net.Maui.Controls.BarcodeGeneratorView))]
public class MyBarcodeGeneratorView { }

[MauiMarkup(typeof(ZXing.Net.Maui.Controls.CameraView))]
public class MyCameraView { }

[MauiMarkup(typeof(SkiaSharp.Extended.UI.Controls.SKLottieView))]
public class MySkLottieView { }
```

Kurallar:

- Constructor **1..N tip** kabul eder — bir attribute'te birden çok kontrolü toplayın.
- Bir sınıfta **birden çok attribute** olabilir.
- Çapa olarak herhangi bir sınıf çalışır — `MauiProgram` popüler tek nokta:

```csharp
using FmgLib.MauiMarkup;
using SkiaSharp.Extended.UI.Controls;
using ZXing.Net.Maui.Controls;
using UraniumUI.Material.Controls;

namespace MauiApp1;

[MauiMarkup(typeof(CameraView))]
[MauiMarkup(typeof(SKLottieView), typeof(SKFileLottieImageSource), typeof(DataGrid))]
[MauiMarkup(typeof(SKConfettiView), typeof(BarcodeGeneratorView), typeof(InputField), typeof(EditorField), typeof(TextField))]
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        return builder.Build();
    }
}
```

### Üretilen kullanım

Üretilen metotlar `FmgLib.MauiMarkup` ad alanında yaşar; mevcut `using`'iniz onları kapsar:

```csharp
new TextField()
    .Title("Şifre")
    .TitleColor(Colors.LightGray)
    .AccentColor(Colors.CadetBlue)
    .TextColor(Colors.White)
    .IsPassword(true),

new SKLottieView()
    .Source(new SKFileLottieImageSource().File("iconapp.json"))
    .RepeatCount(-1)
    .HeightRequest(250)
    .WidthRequest(250)
```

## Taban Sınıflar Otomatik Üretilir

Bir yaprak kontrolü işaretlemek, onun **uygun üçüncü parti taban sınıfları** için de fluent genişletme üretir — bir kontrolün bindable yüzeyinin çoğu genelde orada yaşar. Örneğin Syncfusion'ın `SfButton`'ı `Command`, `FontSize`, `FontAttributes`, `Text`… özelliklerini `ButtonBase`'ten miras alır; yalnızca `[MauiMarkup(typeof(SfButton))]` hem `SfButtonExtension` **hem** `ButtonBaseExtension` üretir; böylece tüm miras alınan özellikler fluent kullanılabilir:

```csharp
[MauiMarkup(typeof(SfButton))]
public class Markup { }

new SfButton()
    .Text("Giriş")
    .Command(vm.LoginCommand)      // ButtonBase'te bildirilmiş — otomatik üretilir
    .FontSize(15)                  // ButtonBase'te bildirilmiş
    .TextColor(Colors.White)       // SfButton'da aynı tiple yeniden bildirilmiş — ButtonBase karşılar
    .StrokeThickness(1)
```

(MAUI çekirdek taban sınıfları asla yeniden üretilmez — onların genişletmeleri zaten kütüphanede gelir.)

## Yeniden Bildirilen Özellik Kuralları

Kontroller bazen bir taban sınıfın özelliğini yeniden bildirir. Generator iki durumu farklı ele alır:

- **Aynı özellik tipi (yaygın "kolaylık yeniden bildirimi")** — örn. `SfButton.TextColor`'ın `ButtonBase.TextColor`'ı yeniden sunması: türemiş sınıf **çift metot almaz ve ek almaz**. Taban sınıfın generic genişletmesi (`TextColor<T>() where T : ButtonBase`) türemiş kontrolü zaten karşılar; çift üretim her çağrıyı belirsiz yapardı (CS0121).
- **Farklı özellik tipi (gerçek bir `new` yeniden bildirimi)** — örn. `SfAvatarView.Background`'ın `Brush`'ı `Color`'a değiştirmesi: türemiş metot **`New` ekini** alır; çünkü yalnızca builder'ın generic argümanında farklılaşan aynı adlı overload'lar lambda çağrı noktalarını kırar ve sessizce yanlış `BindableProperty`'yi hedefleyebilir:

```csharp
new SfAvatarView()
    .BackgroundNew(Colors.LightBlue)   // SfAvatarView'ın kendi (Color) Background özelliği
    .Background(someBrush)             // miras alınan VisualElement (Brush) Background
```

Yaprak tipte beklediğiniz bir metot "eksik" görünüyorsa, neredeyse her zaman **aynı adla** bir taban sınıf genişletmesi tarafından karşılanır — IntelliSense yine de sunar; yalnızca gerçekten tip değiştiren yeniden bildirimler `New` ekini taşır.

## 2. `[MauiMarkupAttachedProp]` — attached property'ler

Üçüncü parti sınıflarda bildirilen attached property'ler için `MauiMarkupAttachedProp` kullanın. Constructor parametreleri, sırayla:

| # | Parametre | Anlamı |
|---|---|---|
| 1 | `controlType` | Attached property'yi **bildiren** sınıf |
| 2 | `propertyName` | `BindableProperty` alan adı (`nameof` kullanın) |
| 3 | `returnType` | Özelliğin değer tipi |
| 4 | `declaringType` | Özelliğin **uygulanacağı** tip (genişletmenin hedefi) |

Örnek — InputKit'in `FormView.IsSubmitButton`'ı, `Button`'a uygulanır:

```csharp
[MauiMarkupAttachedProp(typeof(InputKit.Shared.Controls.FormView),
                        nameof(InputKit.Shared.Controls.FormView.IsSubmitButtonProperty),
                        typeof(bool),
                        typeof(Button))]
[MauiMarkup(typeof(InputKit.Shared.Controls.FormView))]
public class MyFormView { }
```

Üretilen kullanım (adlandırma: *sahip sınıf + özellik adı*, [yerleşik attached property'lerdeki](attached-properties.md) gibi):

```csharp
new Button()
    .Text("Giriş")
    .FontAttributes(FontAttributes.Bold)
    .FormViewIsSubmitButton(true)
```

## 3. Otomatik Mod — `MauiMarkupSourceGenerator`

Attribute'leri tamamen atlamak için **uygulama projenizin `.csproj`'unda** assembly taramasını açın:

```xml
<PropertyGroup>
  <MauiMarkupSourceGenerator>true</MauiMarkupSourceGenerator>
</PropertyGroup>
```

Açıkken generator, referans edilen üçüncü parti assembly'leri tarar ve uygun her public `BindableObject` kontrolü için otomatik fluent genişletme üretir. (Gerekli `CompilerVisibleProperty` tesisatı NuGet paketi tarafından `buildTransitive` props ile otomatik enjekte edilir — yapılacak başka şey yok.)

> `fmglib-mauimarkup-app` şablonundan oluşturulan projeler bu özelliği kutudan açık getirir.

**Değiş tokuş:** otomatik mod pratiktir ama bulduğu her şey için kod üretir; büyük çözümlerde derleme süresini artırabilir. Attribute yaklaşımı üretimi kullandıklarınızla sınırlı tutar.

## Bir Tip Üretilemezse

Generator dayanıklıdır: bir egzotik tip işlenemezse (eksik geçişli bağımlılık, sürüm uyumsuzluğu) **yalnız o tip atlanır** ve bir uyarı (FMMG002) hangi tipin neden atlandığını isimle söyler; diğer her genişletme yine üretilir. Bir source generator'daki tek exception tüm çıktıyı sileceğinden bu izolasyon önemlidir — projede kafa karıştırıcı CS1955/CS0311 hataları görürseniz **Warnings** panelinde FMMG002/CS8785 arayın.

## Notlar

- Üretim, attribute'ün (veya MSBuild özelliğinin) **bulunduğu projede** olur — genelde uygulama projeniz veya onun referans ettiği paylaşılan bir UI projesi.
- Yeniden üretim derlemede otomatiktir; üretilen kaynaklar IDE'de *Analyzers → FmgLib.MauiMarkup.Generator* altında görünür.
- Generator yalnızca tipin `BindableObject`'ten türemesini gerektirir; behavior'lar, image source'lar ve görsel olmayan bindable nesneler (örn. yukarıdaki `SKFileLottieImageSource`) de dahildir.
- Generator'ın çıkaramadığı her şey için (düz CLR özellikleri, metotlar) [özel genişletme metotları](custom-extension-methods.md) ile birleştirin.

## İlgili Konular

- [Fluent Özellikler](fluent-properties.md) — üretilen overload ailesi
- [Attached Property'ler](attached-properties.md)
- [Özel Genişletme Metotları](custom-extension-methods.md)
