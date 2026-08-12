# JSON Dosyalarıyla Yerelleştirme

FmgLib.MauiMarkup, JSON dosyalarıyla beslenen hafif bir yerelleştirme sistemi içerir — **canlı dil değiştirme** ile: dil değiştiğinde bağlı metinler anında güncellenir, sayfa yeniden yüklenmez.

## 1. `MauiProgram.cs`'te Kaydedin

```csharp
builder
    .UseMauiApp<App>()
    .UseMauiMarkupLocalization();
```

Önerilen kullanım **options overload'ı** — dosya adını kültür adıyla karıştıramaz ve yedek kültür, eksik anahtar politikası ile kültür senkronizasyonu burada ayarlanır:

```csharp
builder.UseMauiMarkupLocalization(o => o
    .UseFiles("Common.json", "Checkout.json")   // sırayla birleşir; çakışan anahtarda sonraki kazanır
    .UseDefaultCulture("tr-TR")                 // başlangıç dili
    .UseFallbackCulture("en-US"));              // geçerli kültürden sonuç çıkmazsa kullanılır
```

Kısa biçimler:

```csharp
// varsayılan: uygulama paketinde "Localization.json" arar
.UseMauiMarkupLocalization()

// başlangıç dilini ayarla
.UseMauiMarkupLocalization(defaultLang: "tr-TR")

// başlangıç dili + özel dosyalar
.UseMauiMarkupLocalization(defaultLang: "tr-TR", "Loc1.json", "Loc2.json")

// sadece dosyalar — argüman MUTLAKA isimlendirilmeli, çünkü ilk konumsal parametre kültürdür
.UseMauiMarkupLocalization(filePaths: new[] { "Localization1.json", "/Languages/Temp1.json" })
```

> **İlk argümana dikkat.** `UseMauiMarkupLocalization(defaultLang, params filePaths)` imzasında kültür başta olduğu için `UseMauiMarkupLocalization("Common.json", "Checkout.json")` çağrısı bir *dosya adını* kültür olarak geçer. Bu artık başlangıçta, çözümü söyleyen bir mesajla reddediliyor — ya `filePaths:` kullanın ya da options overload'ını.

Yükleme **senkrondur ve hata fırlatır**: eksik veya bozuk bir dil dosyası, her etiketin ham anahtarını göstermesi yerine uygulamayı başlangıçta düşürür.

## 2. JSON Dil Dosyasını Oluşturun

Yapı: `{ "anahtar": { "dilKodu": "çeviri", ... }, ... }`

```json
{
  "Hello": {
    "tr-TR": "Merhaba Dünya!",
    "en-US": "Hello World!"
  },
  "Msg": {
    "tr-TR": "Deneme amaçlı yapılmıştır.",
    "en-US": "It was made for testing purposes."
  }
}
```

- Anahtarlar herhangi bir kelime veya ifade olabilir — regex/adlandırma kısıtı yok.
- Dil anahtarları da serbesttir, ama standart kültür adları (`en-US`, `tr-TR`, `fr-FR`) önerilir çünkü `CultureInfo` ile hizalanır.

> **Kritik:** JSON dosyasının **Build Action'ı `MauiAsset` olmalıdır** (`FileSystem.OpenAppPackageFileAsync` ile okunur). `.csproj`'da:
>
> ```xml
> <ItemGroup>
>   <MauiAsset Include="Localization.json" />
> </ItemGroup>
> ```

## 3. Metinleri `Translate` ile Bağlayın

Property builder kabul eden her yerde:

```csharp
new Label()
    .Text(e => e.Translate("Hello"))
    .FontSize(32)
    .CenterHorizontal()
    .SemanticHeadingLevel(SemanticHeadingLevel.Level1),

new Label()
    .Text(e => e.Translate("Msg"))
    .FontSize(18)
    .CenterHorizontal()
    .SemanticDescription(e => e.Translate("Msg"))
```

`Translate` **her string özellikte** çalışır, yalnızca `Text`'te değil — placeholder, başlık, tooltip:

```csharp
new Entry().Placeholder(e => e.Translate("EnterEmail"))
this.Title(e => e.Translate("SettingsTitle"))
```

## 4. Çalışma Zamanında Dil Değiştirin

```csharp
Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));
```

`Translate` ile bağlanan her özellik anında güncellenir (translator `INotifyPropertyChanged` uygular ve binding'ler onu dinler).

Tam bir dil seçici:

```csharp
new VerticalStackLayout()
.Center()
.Children(
    new RadioButton()
        .IsChecked(Translator.Instance.CurrentCulture.Name == "tr-TR")
        .Content("tr-TR")
        .OnCheckedChanged((sender, e) =>
        {
            if (e.Value)
                Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));
        }),

    new RadioButton()
        .IsChecked(Translator.Instance.CurrentCulture.Name == "en-US")
        .Content("en-US")
        .OnCheckedChanged((sender, e) =>
        {
            if (e.Value)
                Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));
        })
)
```

## İçinde Değer Geçen Metinler — `TranslateFormat`

Çevrilen bir cümle genelde çalışma zamanı değeri taşır. `TranslateFormat` hem çeviriyi hem argümanları bağlar; böylece etiket **dil** değişince de **argüman** değişince de yeniden çizilir:

```json
{
  "WelcomeUser": { "tr-TR": "Hoş geldin, {0}!",  "en-US": "Welcome, {0}!" },
  "CartSummary": { "tr-TR": "{0} ürün — {1:C}",  "en-US": "{0} items — {1:C}" }
}
```

```csharp
new Label().Text(e => e.TranslateFormat("WelcomeUser", nameof(vm.UserName)))
new Label().Text(e => e.TranslateFormat("CartSummary", nameof(vm.ItemCount), nameof(vm.Total)))
```

Argüman yolları elemanın `BindingContext`'ine göre çözülür. Yer tutucular **seçili** kültürle biçimlenir; yani `{1:C}` `tr-TR`'de `1.234,50 ₺`, `en-US`'te `$1,234.50` verir. Bir çeviride `{0}` kaybolursa etiket hata fırlatmak yerine ham kalıbı gösterir.

## Sağdan Sola Diller

Arapça/İbranice bir arayüzü çevirip aynalamamak düzeni bozuk bırakır. `FlowDirection`'ı sayfada bir kez kültüre bağlayın:

```csharp
this.FlowDirection(e => e.FromCulture())
```

Kod tarafı için `Translator.Instance.IsRightToLeft` ve `.FlowDirection` de mevcut.

## Eksik Anahtarlar

Varsayılan olarak çevirisi olmayan anahtar, anahtarın kendisi olarak görünür. İstemiyorsanız politikayı değiştirin:

```csharp
builder.UseMauiMarkupLocalization(o => o
    .UseFiles("Localization.json")
    .OnMissingTranslation(MissingTranslationBehavior.Marker));   // ⟦Key⟧ basar — gözden kaçmaz
```

| Davranış | Eksik `Hello` sonucu |
|---|---|
| `ReturnKey` *(varsayılan)* | `Hello` |
| `ReturnEmpty` | *(boş)* |
| `Marker` | `⟦Hello⟧` |
| `Throw` | `KeyNotFoundException` |

RESX translator'ı da aynı ayarı uygular; backend değiştirmek davranış değiştirmez.

## Kodda Çeviri Okuma

UI olmayan string'ler (uyarı, log) için translator'ı doğrudan indeksleyin — veya `ToTranslate` string genişletmesini kullanın:

```csharp
string title = Translator.Instance["Hello"];
await DisplayAlert(Translator.Instance["Hello"], Translator.Instance["Msg"], "OK");

// string genişletme karşılıkları:
string hello   = "Hello".ToTranslate();            // geçerli kültür
string helloTr = "Hello".ToTranslate("tr-TR");     // açık kültür
```

> **Bunlar anlık görüntü döner.** `new Label().Text("Hello".ToTranslate())` derlenir ve doğru metni gösterir ama dil değişince **güncellenmez** — arkasında binding yoktur. Ekrandaki her şey için `.Text(e => e.Translate("Hello"))` kullanın.

## Seçimi Kalıcılaştırma

Kütüphane seçilen kültürü kalıcılaştırmaz; `Preferences` ile birleştirin:

```csharp
// değişimde
Preferences.Set("lang", "tr-TR");

// başlangıçta (örn. App constructor'ında)
var saved = Preferences.Get("lang", "en-US");
Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo(saved));
```

## Büyük Uygulamaları Organize Etme

- **Özelliğe göre bölün:** `UseMauiMarkupLocalization(o => o.UseFiles("Common.json", "Checkout.json", "Settings.json"))`. Dosyalar tek sözlükte birleşir; çakışan anahtarda sonraki dosyalar öncekileri geçersiz kılar — dil bazında, yani bir feature dosyası bir anahtarın tek dilini diğerlerini tekrarlamadan ezebilir.
- **Eksik anahtarlar:** anlamlı anahtar adları tercih edin (`"Login_InvalidPassword"`), Debug derlemelerinde `MissingTranslationBehavior.Marker` düşünün.
- **Kültür yedeklemesi:** arama `tr-TR` → `tr` → ayarlı `FallbackCulture` sırasıyla ilerler. Ortak anahtarları nötr dile (`"tr"`, `"en"`) yazmak tüm bölgesel varyantları tek seferde kapsar.
- Dil dosyası eksik veya hatalıysa başlangıçta beklenen formatı açıklayan bir `FileLoadException` fırlatılır — dosyaları CI'ın parçası olarak doğrulayın.

## JSON mı, RESX mı?

| | JSON (bu sayfa) | [RESX](localization-resx.md) |
|---|---|---|
| Dosya formatı | Tüm diller için tek dosya | Dil başına bir `.resx` |
| Araçlar | Herhangi bir metin editörü | Visual Studio kaynak editörü, mevcut kurumsal iş akışları |
| Anahtar erişimi | String anahtarlar | String anahtarlar + üretilen güçlü tipli sınıf (`nameof` desteği) |
| Çalışma zamanı değişimi | `Translator.Instance` | `TranslatorResx.Instance` |
| Binding metodu | `e.Translate("Key")` | `e.TranslateResx("Key")` |
| Formatlı metin | `e.TranslateFormat("Key", yollar…)` | `e.TranslateResxFormat("Key", yollar…)` |
| Kültür yedeklemesi | `tr-TR` → `tr` → `FallbackCulture` | `ResourceManager` zinciri → nötr `.resx` |

İkisi de canlı değişimi destekler; çeviri iş akışınıza uyanı seçin.

## İlgili Konular

- [Yerelleştirme (RESX)](localization-resx.md)
- [Fluent Özellikler](fluent-properties.md)
