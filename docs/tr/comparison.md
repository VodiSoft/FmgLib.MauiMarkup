# FmgLib.MauiMarkup ve CommunityToolkit.Maui.Markup

İki kütüphane de aynı soruyu yanıtlıyor — *.NET MAUI arayüzünü XAML yerine C# ile nasıl yazarım?* — ve ikisi de
bunu fluent genişletme metotlarıyla yapıyor. Bu sayfa ikisinin dürüst, özellik özellik karşılaştırması:
FmgLib.MauiMarkup'ta olup Community Toolkit'te olmayanlar — ve Community Toolkit'in daha doğru tercih olduğu
durumlar.

> **Veriler 13 Ağustos 2026 tarihinde doğrulanmıştır.** Paket sürümleri ve indirme sayıları değişir; özellik
> karşılaştırması o tarihteki resmî dokümantasyonlara dayanır. Geri dönüşü zor bir karar vermeden önce
> kaynakları kontrol edin.

## Bir bakışta

| | [FmgLib.MauiMarkup](https://www.nuget.org/packages/FmgLib.MauiMarkup/) | [CommunityToolkit.Maui.Markup](https://www.nuget.org/packages/CommunityToolkit.Maui.Markup/) |
|---|---|---|
| **Son sürüm** | 10.3.0 | 8.0.0 |
| **Yayın** | Ağustos 2026 | Temmuz 2026 |
| **Toplam indirme** | ~19 B | ~1,0 M |
| **Arkasındaki yapı** | Bağımsız (VodiSoft) | .NET Foundation / Community Toolkit |
| **Hedef framework'ler** | net9.0 **ve** net10.0 | net10.0 |
| **Lisans** | MIT | MIT |
| **API nasıl üretiliyor** | Roslyn source generator | Elle yazılmış, seçilmiş genişletmeler |

---

## Özellik karşılaştırması

Simgeler: **●** hazır geliyor · **◐** kısmi / elle iş gerekiyor · **○** yok

### Kapsam — MAUI'nin ne kadarına fluent erişebiliyorsunuz

| | FmgLib | CTK.Markup |
|---|:--:|:--:|
| **Her** kontrolün **her** bindable özelliği için fluent metot | ● | ○ |
| **Her** event için fluent `On<Event>` metodu | ● | ○ |
| **3. parti kontroller, sıfır yapılandırma** | ● | ○ |
| 3. parti kontroller, tip başına opt-in | ● | ○ |
| Attached property'ler (`Grid.Row`, `Shell.*`, `Semantic*`, …) | ● | ● |
| Grid satır/sütun tanım builder'ları | ● | ● |
| Layout option ve metin hizalama yardımcıları | ● | ● |

**Tek en büyük fark bu.** CommunityToolkit.Maui.Markup **seçilmiş** bir genişletme kümesi sunuyor — `Label`,
`Image`, `Grid`, `VisualElement`, `ItemsView`, `Placeholder` ve bir düzine kadar aile daha. Kapsadığı özellikler
için mükemmel; ama kimsenin helper yazmadığı bir özelliğe ihtiyacınız olduğu anda zincirin ortasında nesne
başlatıcı sözdizimine düşüyorsunuz:

```csharp
// CommunityToolkit — helper olmayan yerde iki stili karıştırmak
new Entry
{
    Keyboard = Keyboard.Numeric,          // fluent helper yok → nesne başlatıcı
    ReturnType = ReturnType.Done,
}
.Placeholder("Sayı girin")                // helper var → fluent
.FontSize(15)
.Height(44);
```

```csharp
// FmgLib — her bindable özellik üretildiği için zincir hiç kopmaz
new Entry()
    .Keyboard(Keyboard.Numeric)
    .ReturnType(ReturnType.Done)
    .Placeholder("Sayı girin")
    .FontSize(15)
    .HeightRequest(44);
```

### 3. parti kontroller

| | FmgLib | CTK.Markup |
|---|:--:|:--:|
| Referans verilen kontrol kütüphaneleri için fluent metot üretir | ● | ○ |
| Attribute veya tip tip bildirim gerektirmez | ● | — |
| Büyük çözümler için opt-in mod (`[MauiMarkup(typeof(T))]`) | ● | — |
| 3. parti kontrollerin attached property'leri | ● | ○ |

Farkın en açık olduğu yer burası. Community Toolkit ile bir Syncfusion veya DevExpress kontrolü yalnızca genel
`VisualElement`/`View` yardımcılarını alır — o kontrole özgü her özellik düz atama olarak kalır.

```csharp
// FmgLib — tek bir MSBuild özelliği, referans verdiğiniz her kontrol kütüphanesi fluent olur.
<MauiMarkupSourceGenerator>true</MauiMarkupSourceGenerator>

new SfButton().Text("Satın al").CornerRadius(8)     // Syncfusion
new SKLottieView().Source(…).RepeatCount(-1)        // SkiaSharp.Extended
new CameraView().IsTorchOn(true).OnFrameReady(…)    // ZXing
```

### Veri bağlama

| | FmgLib | CTK.Markup |
|---|:--:|:--:|
| String yollu binding | ● | ● |
| Derlenmiş / tipli binding (reflection yok) | ● | ● |
| Satır içi `Convert` / `ConvertBack` lambda'ları | ● | ● |
| MultiBinding | ● | ● |
| **Tipli** MultiBinding (2–9 parametre, `object[]` yok) | ● | ◐¹ |
| Derlenmiş ve string alt-binding'lerin tek MultiBinding'de karışması | ● | ○ |
| `FallbackValue` / `TargetNullValue` | ● | ● |
| Binding'in doğrudan özelliğin içinde yazılması | ● | ◐² |

¹ `FuncMultiConverter` ile mümkün; tipli parametreler yerine konumsal/`object[]` değerler alır.
² Community Toolkit, `BindableProperty`'yi tekrar adlandıran ayrı bir `.Bind(Property, …)` çağrısıyla bağlar.

```csharp
// CommunityToolkit — binding özelliği bir kez daha adlandırır
new Entry().Bind(Entry.TextProperty,
    static (ViewModel vm) => vm.RegistrationCode,
    static (ViewModel vm, string text) => vm.RegistrationCode = text)

// FmgLib — binding, zaten ayarladığınız özelliğin içinde durur
new Entry().Text(e => e
    .Getter(static (ViewModel vm) => vm.RegistrationCode)
    .Setter(static (ViewModel vm, string text) => vm.RegistrationCode = text)
    .BindingMode(BindingMode.TwoWay))
```

Ve tipli multi-binding'ler — delegate parametreleri, bildirim sırasıyla alt-binding tipleridir:

```csharp
new Button().IsEnabled(e => e
    .Path("AcceptedTerms")
    .Path("ConfirmedEmail")
    .MultiConvert((bool terms, bool email) => terms && email))

// derlenmiş ve string alt-binding'ler serbestçe karışır:
new Label().Text(e => e
    .Getter(static (OrderVm vm) => vm.Total)
    .Path("ItemCount")
    .MultiConvert((decimal total, int count) => $"{count} ürün — {total:C}"))
```

### Görünüm

| | FmgLib | CTK.Markup |
|---|:--:|:--:|
| Kontrollerdekiyle aynı fluent metotlarla `Style<T>` | ● | ● |
| `VisualState<T>` yardımcısı | ● | ○ |
| İsimli state sabitleri (`VisualStates.Button.Pressed`) | ● | ○ |
| Trigger'lar: property / data / multi / event | ● | ○ |
| Her animasyonlanabilir özellik için üretilmiş `Animate<Property>To` | ● | ○ |
| Visual state'e girişte çalışan animasyonlar | ● | ○ |
| Satır içi açık/koyu değer (`OnLight` / `OnDark`) | ● | ◐³ |
| Satır içi idiom değeri (`OnPhone` / `OnTablet` / `OnDesktop`) | ● | ○ |
| Satır içi platform değeri (`OniOS` / `OnAndroid` / …) | ● | ○ |
| Aynı lambda içinde `DynamicResource` | ● | ● |

³ Özellik çağrısının içinde bir değer olarak değil, `AppThemeBinding`/dinamik kaynak yardımcıları üzerinden.

FmgLib'in farkı, **bunların hepsinin zaten ayarladığınız özelliğin üzerindeki tek bir lambda'da** olması; böylece
bir değer aynı anda temaya duyarlı, idiom'a duyarlı ve bağlı olabiliyor — zincirden çıkmadan:

```csharp
new Label()
    .TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.White))     // gerçek AppThemeBinding
    .FontSize(e => e.OnPhone(13.0).OnTablet(15.0).OnDesktop(17.0))
    .Margin(e => e.OniOS(new Thickness(0, 20, 0, 0)).Default(new Thickness(0)))
    .Text(e => e.Path("Title"))
```

`OnLight`/`OnDark` gerçek bir `AppThemeBinding` üretir; tema değiştiğinde çalışan arayüz kendini yeniden boyar —
sayfa yeniden kurulmaz, kaynak sözlüğü boşaltılıp doldurulmaz.

### Yerelleştirme (Localization)

| | FmgLib | CTK.Markup |
|---|:--:|:--:|
| Dahili yerelleştirme | ● | ○ |
| JSON dil dosyaları | ● | ○ |
| RESX / `ResourceManager` | ● | ○ |
| Anlık dil değişimi (sayfa yeniden yüklenmeden) | ● | ○ |
| Kültür yedekleme zinciri (`tr-TR` → `tr` → varsayılan) | ● | ○ |
| Değer içeren çeviriler (`TranslateFormat`) | ● | ○ |
| Sağdan sola bağlama (kültürden `FlowDirection`) | ● | ○ |
| Eksik anahtar politikası (anahtar / boş / işaret / hata) | ● | ○ |

Community Toolkit hiç yerelleştirme sunmuyor — ayrı bir paket ekleyip `INotifyPropertyChanged` tazelemesini
kendiniz kuruyorsunuz.

```csharp
builder.UseMauiMarkupLocalization(o => o
    .UseFiles("Localization.json")
    .UseDefaultCulture("tr-TR")
    .UseFallbackCulture("en-US"));

new Label().Text(e => e.Translate("Greeting"))
new Label().Text(e => e.TranslateFormat("WelcomeUser", nameof(vm.UserName)))
this.FlowDirection(e => e.FromCulture())        // Arapça/İbranice'de sayfayı aynalar

Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));   // her şey kendini yeniden okur
```

### Geliştirici deneyimi

| | FmgLib | CTK.Markup |
|---|:--:|:--:|
| Standart .NET Hot Reload ile çalışır | ● | ● |
| **Hot reload'da arayüz metodunuzu yeniden çalıştırır** (`Build()`) | ● | ○ |
| Zayıf referanslı kayıt — hot reload sayfa sızdırmaz | ● | — |
| Hazır sayfa taban sınıfları (`FmgLibContentPage<TVm>`) | ● | ○ |
| `dotnet new` proje şablonu | ● | ○ |
| Tam kapsamlı galeri örnek uygulaması | ● | ◐ |
| Dokümantasyon | 36 sayfa | Microsoft Learn |
| **Türkçe dokümantasyon** | ● | ○ |

Community Toolkit ile .NET Hot Reload kodunuza uygulanır ama arayüz kurulumunuzu yeniden çağıran bir şey yoktur;
markup değişikliğini görmek için genelde sayfayı yeniden açarsınız. FmgLib'in `IFmgLibHotReload` + `Build()`
kalıbı, uygulanan her düzenlemede kurulumu yeniden çalıştırır — ve sayfaları **zayıf referansla** kaydeder, yani
hot reload kapatılmış bir sayfayı asla hayatta tutmaz (sızıntı dedektörleri sessiz kalır).

---

## FmgLib.MauiMarkup'ın fazlası, tek listede

Aşağıdakilerin hepsi FmgLib'de var ve CommunityToolkit.Maui.Markup'ta **hiçbiri** yok:

1. **Referans verilen her 3. parti kontrol için otomatik fluent üretimi** — tek MSBuild bayrağı, attribute yok.
2. **Her kontrolün her bindable özelliği**, dört overload kalıbıyla — zincir hiç kopmaz.
3. **Her event için `On<Event>`**, iki biçimde (tipli sender veya tam event args).
4. **Yerelleştirme** — JSON ve RESX, anlık değişim, yedekleme zinciri, formatlı çeviriler, RTL, eksik anahtar politikası.
5. **Arayüz metodunuzu yeniden çalıştıran hot reload**, zayıf kayıt ve hazır sayfa tabanlarıyla.
6. Özelliğin üzerinde satır içi **idiom ve platform değer builder'ları**.
7. 2–9 parametreli **tipli `MultiConvert`**, ve derlenmiş/string alt-binding'lerin serbest karışımı.
8. Her animasyonlanabilir özellik için **üretilmiş `Animate<Property>To`** — await edilebilir ve birleştirilebilir.
9. **İsimli state sabitleriyle `VisualState<T>`** ve state'e girişte animasyon.
10. **Fluent trigger'lar** — property, data, multi ve event.
11. Tek paket sürümünden **.NET 9 ve .NET 10**.
12. **İngilizce ve Türkçe dokümantasyon**, 24 sayfalık galeri örneği ve `dotnet new` şablonu.

## Kaynaklar

- [CommunityToolkit.Maui.Markup — Microsoft Learn](https://learn.microsoft.com/dotnet/communitytoolkit/maui/markup/markup) · [GitHub](https://github.com/CommunityToolkit/Maui.Markup) · [NuGet](https://www.nuget.org/packages/CommunityToolkit.Maui.Markup)
- [FmgLib.MauiMarkup — NuGet](https://www.nuget.org/packages/FmgLib.MauiMarkup)

## İlgili Konular

- [Başlarken](getting-started.md)
- [3. Parti Kontroller](third-party-controls.md) — sıfır yapılandırmalı generator
- [Yerelleştirme (JSON)](localization-json.md) · [Yerelleştirme (RESX)](localization-resx.md)
- [XAML'den C#'a](xaml-to-csharp.md)
