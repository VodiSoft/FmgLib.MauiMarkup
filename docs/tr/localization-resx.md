# RESX Dosyalarıyla Yerelleştirme

Ekibiniz zaten `.resx` kaynak dosyalarıyla çeviri yapıyorsa (klasik .NET iş akışı), FmgLib.MauiMarkup doğrudan onlara takılır — [JSON varyantıyla](localization-json.md) aynı canlı-değişim davranışıyla, sizin `ResourceManager`'ınız tarafından sürülür.

## 1. RESX Kaynaklarını Oluşturun

Projenize kaynak dosyaları ekleyin, örn. `Resources/Languages` altında:

```
AppResources.resx          (varsayılan dil)
AppResources.tr-TR.resx
AppResources.fr-FR.resx
```

Her dosya aynı anahtarları çevrilmiş değerlerle içerir (`Hello` → "Hello World!" / "Merhaba Dünya!"). Visual Studio, statik `ResourceManager`'lı `AppResources` sınıfını üretir.

## 2. `MauiProgram.cs`'te Kaydedin

```csharp
builder
    .UseMauiApp<App>()
    .UseMauiMarkupLocalizationWithResx(AppResources.ResourceManager);

// veya açık başlangıç diliyle:
// .UseMauiMarkupLocalizationWithResx(AppResources.ResourceManager, "en-US");
```

## 3. Metinleri `TranslateResx` ile Bağlayın

```csharp
new Label()
    .Text(e => e.TranslateResx("Hello"))
    .FontSize(32)
    .CenterHorizontal()
    .SemanticHeadingLevel(SemanticHeadingLevel.Level1),

new Label()
    .Text(e => e.TranslateResx(nameof(AppResources.Msg)))   // güçlü tipli anahtar!
    .FontSize(18)
    .CenterHorizontal()
    .SemanticDescription(e => e.TranslateResx("Msg"))
```

`nameof(AppResources.Msg)` formu önerilendir — bir kaynak anahtarını yeniden adlandırmak derleme zamanı kontrollü bir refactoring olur.

JSON varyantında olduğu gibi her string özellik çevrilebilir:

```csharp
new Entry().Placeholder(e => e.TranslateResx(nameof(AppResources.EnterEmail)))
this.Title(e => e.TranslateResx(nameof(AppResources.SettingsTitle)))
```

## 4. Çalışma Zamanında Dil Değiştirin

```csharp
TranslatorResx.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));
```

`TranslateResx` ile bağlanan tüm özellikler anında güncellenir.

Dil seçici örneği:

```csharp
new VerticalStackLayout()
.Center()
.Children(
    new RadioButton()
        .IsChecked(TranslatorResx.Instance.CurrentCulture.Name == "tr-TR")
        .Content("tr-TR")
        .OnCheckedChanged((sender, e) =>
        {
            if (e.Value)
                TranslatorResx.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));
        }),

    new RadioButton()
        .IsChecked(TranslatorResx.Instance.CurrentCulture.Name == "en-US")
        .Content("en-US")
        .OnCheckedChanged((sender, e) =>
        {
            if (e.Value)
                TranslatorResx.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));
        })
)
```

## Kodda Çeviri Okuma

```csharp
string msg = TranslatorResx.Instance[nameof(AppResources.Msg)];
await DisplayAlert("Bilgi", msg, "OK");

// string genişletme karşılıkları:
string hello   = "Hello".ToTranslateResx();            // geçerli kültür
string helloTr = "Hello".ToTranslateResx("tr-TR");     // açık kültür
```

## Notlar ve İpuçları

- **Kültür yedeği (fallback)** standart `ResourceManager` kurallarını izler: `tr-TR` → `tr` → varsayılan kaynaklar. Nötr `.resx`'i eksiksiz tutun.
- **Seçimi kalıcılaştırın** `Preferences` ile ve başlangıçta yeniden uygulayın (bkz. [Yerelleştirme (JSON)](localization-json.md#seçimi-kalıcılaştırma) deseni).
- JSON ve RESX sistemleri bağımsızdır (`Translator` vs. `TranslatorResx`); ikisini bir uygulamada *kullanabilirsiniz* ama birinde standartlaşmak işleri basit tutar.
- Biçimli string'ler: kalıbı kaynaklarda saklayın (`"WelcomeUser" = "Hoş geldin, {0}!"`) ve kodda bir [converter](binding-converters.md) veya `string.Format` ile birleştirin.

## İlgili Konular

- [Yerelleştirme (JSON)](localization-json.md) — JSON-vs-RESX karşılaştırma tablosunu içerir
- [Fluent Özellikler](fluent-properties.md)
