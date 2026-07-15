# JSON Dosyalarıyla Yerelleştirme

FmgLib.MauiMarkup, JSON dosyalarıyla beslenen hafif bir yerelleştirme sistemi içerir — **canlı dil değiştirme** ile: dil değiştiğinde bağlı metinler anında güncellenir, sayfa yeniden yüklenmez.

## 1. `MauiProgram.cs`'te Kaydedin

```csharp
builder
    .UseMauiApp<App>()
    .UseMauiMarkupLocalization();
```

Overload'lar:

```csharp
// varsayılan: uygulama paketinde "Localization.json" arar
.UseMauiMarkupLocalization()

// başlangıç dilini ayarla
.UseMauiMarkupLocalization(defaultLang: "tr-TR")

// özel dosya(lar) — birden çok dosya birleştirilir (çakışan anahtarda sonraki kazanır)
.UseMauiMarkupLocalization(defaultLang: "tr-TR", "Loc1.json", "Loc2.json")
.UseMauiMarkupLocalization(filePaths: new[] { "Localization1.json", "Localization2.json", "/Languages/Temp1.json" })
```

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

## Kodda Çeviri Okuma

UI olmayan string'ler (uyarı, log) için translator'ı doğrudan indeksleyin — veya `ToTranslate` string genişletmesini kullanın:

```csharp
string title = Translator.Instance["Hello"];
await DisplayAlert(Translator.Instance["Hello"], Translator.Instance["Msg"], "OK");

// string genişletme karşılıkları:
string hello   = "Hello".ToTranslate();            // geçerli kültür
string helloTr = "Hello".ToTranslate("tr-TR");     // açık kültür
```

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

- **Özelliğe göre bölün:** birden çok dosya geçin — `UseMauiMarkupLocalization("Common.json", "Checkout.json", "Settings.json")`. Dosyalar tek sözlükte birleşir; çakışan anahtarda sonraki dosyalar öncekileri geçersiz kılar.
- **Eksik anahtarlar:** anlamlı anahtar adları tercih edin (`"Login_InvalidPassword"`) ki çevrilmemiş anahtarlar test sırasında görünür olsun.
- JSON hatalıysa başlangıçta beklenen formatı açıklayan bir `FileLoadException` fırlatılır — dosyaları CI'ın parçası olarak doğrulayın.

## JSON mı, RESX mı?

| | JSON (bu sayfa) | [RESX](localization-resx.md) |
|---|---|---|
| Dosya formatı | Tüm diller için tek dosya | Dil başına bir `.resx` |
| Araçlar | Herhangi bir metin editörü | Visual Studio kaynak editörü, mevcut kurumsal iş akışları |
| Anahtar erişimi | String anahtarlar | String anahtarlar + üretilen güçlü tipli sınıf (`nameof` desteği) |
| Çalışma zamanı değişimi | `Translator.Instance` | `TranslatorResx.Instance` |
| Binding metodu | `e.Translate("Key")` | `e.TranslateResx("Key")` |

İkisi de canlı değişimi destekler; çeviri iş akışınıza uyanı seçin.

## İlgili Konular

- [Yerelleştirme (RESX)](localization-resx.md)
- [Fluent Özellikler](fluent-properties.md)
