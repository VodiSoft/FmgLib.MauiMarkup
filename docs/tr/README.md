# FmgLib.MauiMarkup Dokümantasyonu

**FmgLib.MauiMarkup**, .NET MAUI için akıcı (fluent) bir C# markup kütüphanesidir. Tüm kullanıcı arayüzünüzü — XAML'e hiç ihtiyaç duymadan — saf C# ile geliştirmenizi sağlar. Her MAUI kontrolünün her bindable özelliği ve olayı (event) için otomatik üretilmiş, okunabilir ve zincirlenebilir genişletme metotları sunar.

```csharp
new Label()
    .Text("Merhaba, FmgLib!")
    .FontSize(30)
    .TextColor(Colors.Green)
    .Center()
```

Bu dokümantasyon, kütüphane için eksiksiz ve kendi kendine yeten bir rehberdir. Proje README'sindeki her başlık burada çok daha derinlemesine ele alınmış; ayrıca kütüphanenin kaynak kodundan çıkarılan, README'de yer almayan kullanım kalıpları da eklenmiştir.

> 🇬🇧 English version of this documentation lives in the parent [docs/](../README.md) folder.

## İçindekiler

### 1. Temeller

| Doküman | Öğrenecekleriniz |
|---|---|
| [Başlarken](getting-started.md) | Kurulum, proje şablonları, ilk uygulama, proje yapısı |
| [XAML'den C#'a](xaml-to-csharp.md) | XAML kavramlarının FmgLib.MauiMarkup karşılıkları, yan yana dönüşümler |
| [Fluent Özellikler](fluent-properties.md) | Dört overload kalıbı; tema/platform/idiom bazlı değerler, dinamik kaynaklar |
| [Nesne Referansları (Assign)](assign-and-references.md) | `Assign`, `InvokeOnElement`, `RegisterName`; alan tanımlamadan kontrole erişim |

### 2. Veri Bağlama (Data Binding)

| Doküman | Öğrenecekleriniz |
|---|---|
| [Özellik Bağlama](data-binding.md) | `Path`, `Source`, `BindingMode`, `StringFormat`, fallback değerleri, düşük seviye `Bind()` API'si |
| [Binding Converter'ları](binding-converters.md) | Satır içi `Convert`/`ConvertBack`, `IValueConverter`, func tabanlı converter'lar |
| [MultiBinding](multi-binding.md) | Birden çok binding'i tek özellikte birleştirme, `IMultiValueConverter` |
| [Derlenmiş Binding'ler](compiled-bindings.md) | `Getter`/`Setter` expression binding'leri, `Binding.Create`, performans |

### 3. Yerleşim (Layout)

| Doküman | Öğrenecekleriniz |
|---|---|
| [Yerleşim Seçenekleri](layout-options.md) | Hizalama ve doldurma yardımcıları (`Center`, `AlignTopLeft`, `FillHorizontal`, …) |
| [Grid](grid.md) | Satır/sütun tanım builder'ları (`Star`, `Auto`, `Absolute`), çocukları konumlandırma |
| [Metin Hizalama](text-alignment.md) | `ITextAlignment` yardımcıları (`TextCenter`, `TextTopLeft`, …) |
| [Attached Property'ler](attached-properties.md) | Grid, Shell, Semantic, Automation… için tam eşleme tablosu ve örnekler |

### 4. Etkileşim

| Doküman | Öğrenecekleriniz |
|---|---|
| [Olay İşleyiciler](event-handlers.md) | `On<EventName>` metotları, method group ve satır içi lambda kullanımı |
| [Jest Tanıyıcılar](gesture-recognizers.md) | Tap, pan, pointer, swipe, pinch, sürükle & bırak |
| [Behavior'lar](behaviors.md) | Yeniden kullanılabilir behavior ekleme, özel behavior yazma |
| [Trigger'lar](triggers.md) | Property, data, event, multi ve state trigger'ları |
| [Menüler](menus.md) | Bağlam menüleri (`MenuFlyout`), menü çubukları, klavye kısayolları |
| [SwipeView](swipeview.md) | Kaydırmalı aksiyon satırları, `SwipeItem`, özel swipe içerikleri |

### 5. Görünüm

| Doküman | Öğrenecekleriniz |
|---|---|
| [Stiller](styling.md) | `Style<T>`, resource dictionary'ler, `BasedOn`, türetilmiş tipler, uygulama geneli temalar |
| [Visual State'ler](visual-states.md) | `VisualState<T>`, yerleşik state adları, state'e bağlı animasyonlar |
| [Gradyanlar ve Fırçalar](gradients-and-brushes.md) | Doğrusal/dairesel gradyanlar, düz fırçalar, gölgeler |
| [Şekiller ve Geometriler](shapes.md) | Çizgiler, dikdörtgenler, elipsler, poligonlar, `Path` geometrileri, kırpma |
| [Biçimli Metin](formatted-text.md) | `FormattedString`/`Span`, karışık stiller, tıklanabilir satır içi bağlantılar |
| [Animasyonlar](animations.md) | Üretilen `Animate…To` yardımcıları, MAUI animasyonlarıyla birlikte kullanım |

### 6. Uygulama Mimarisi

| Doküman | Öğrenecekleriniz |
|---|---|
| [Shell Uygulamaları](shell-navigation.md) | C# ile Shell kurma; flyout, sekmeler, şablonlar, navigasyon |
| [Application ve Pencereler](application-and-windows.md) | `Application` kurulumu, pencere yaşam döngüsü, `TitleBar`, NavigationPage/TabbedPage/FlyoutPage |
| [Hot Reload](hot-reload.md) | `IFmgLibHotReload`, `FmgLibContentPage`, MVVM sayfa taban sınıfları, IDE destek matrisi |
| [Koleksiyonlar ve Şablonlar](collections-and-templates.md) | `CollectionView`, `ItemTemplate` overload'ları, `BindableLayout`, `EmptyView` |
| [Yerelleştirme (JSON)](localization-json.md) | JSON dosyası tabanlı yerelleştirme, çalışma zamanında dil değiştirme |
| [Yerelleştirme (RESX)](localization-resx.md) | `TranslatorResx` ile RESX tabanlı yerelleştirme |

### 7. Genişletme ve Referans

| Doküman | Öğrenecekleriniz |
|---|---|
| [Üçüncü Parti Kontroller](third-party-controls.md) | `[MauiMarkup]`, `[MauiMarkupAttachedProp]`, otomatik generator modu |
| [Özel Genişletme Metotları](custom-extension-methods.md) | Her bağlamda çalışan kendi fluent metotlarınızı yazma |
| [Yardımcı Metotlar](utilities.md) | `ToColor`, koleksiyon yardımcıları, `AddRangeMarkup`, stil interop |
| [Tam Örnekler](complete-examples.md) | Eksiksiz sayfalar: giriş ekranı, ürün listesi, ayarlar sayfası, MVVM kalıpları |
| [İpuçları ve Sorun Giderme](tips-and-troubleshooting.md) | Sık yapılan hatalar, adlandırma kuralları, SSS |

## Önerilen Öğrenme Yolu

1. **Kütüphaneye yeni misiniz?** Önce [Başlarken](getting-started.md), ardından [XAML'den C#'a](xaml-to-csharp.md) ve [Fluent Özellikler](fluent-properties.md). Bu üç doküman günlük kullanımın %80'ini kapsar.
2. **Gerçek sayfalar mı geliştiriyorsunuz?** [Yerleşim Seçenekleri](layout-options.md), [Grid](grid.md), [Özellik Bağlama](data-binding.md) ve [Hot Reload](hot-reload.md) ile devam edin.
3. **Uygulamayı cilalıyor musunuz?** [Stiller](styling.md), [Visual State'ler](visual-states.md), [Trigger'lar](triggers.md) ve [Animasyonlar](animations.md).
4. **Birden çok pazara mı çıkıyorsunuz?** [Yerelleştirme (JSON)](localization-json.md) veya [Yerelleştirme (RESX)](localization-resx.md).
5. **SkiaSharp, ZXing, UraniumUI, InputKit… mi kullanıyorsunuz?** [Üçüncü Parti Kontroller](third-party-controls.md).

## Paketler

| Paket | Amaç |
|---|---|
| [`FmgLib.MauiMarkup`](https://www.nuget.org/packages/FmgLib.MauiMarkup/) | Markup kütüphanesinin kendisi + Roslyn source generator |
| [`FmgLib.MauiMarkup.Template`](https://www.nuget.org/packages/FmgLib.MauiMarkup.Template/) | `dotnet new` proje şablonu (`fmglib-mauimarkup-app`) |

## 30 Saniyede Temel Fikir

Her MAUI kontrolünün her bindable özelliği, **özellikle aynı adı taşıyan bir fluent genişletme metodu** alır. Her olay bir **`On<EventName>` metodu** alır. Tüm metotlar kontrolün kendisini döndürür; böylece çağrılar doğal biçimde zincirlenir ve C# kodunuzun girintisi görsel ağacı birebir yansıtır — tıpkı XAML gibi, ama tam IntelliSense, yeniden adlandırma desteği, derleme zamanı güvenliği ile ve iki dil arasında gidip gelmeden.

```csharp
public partial class MainPage : ContentPage, IFmgLibHotReload
{
    int count = 0;

    public MainPage() => this.InitializeHotReload();

    public void Build() =>
        this.Content(
            new VerticalStackLayout()
            .Spacing(25)
            .Padding(30)
            .Center()
            .Children(
                new Image()
                    .Source("dotnet_bot.png")
                    .HeightRequest(200)
                    .CenterHorizontal(),

                new Label()
                    .Text("Merhaba, Dünya!")
                    .FontSize(32)
                    .CenterHorizontal(),

                new Button()
                    .Text("Bana tıkla")
                    .OnClicked(b => b.Text = $"{++count} kez tıklandı")
                    .CenterHorizontal()
            )
        );
}
```
