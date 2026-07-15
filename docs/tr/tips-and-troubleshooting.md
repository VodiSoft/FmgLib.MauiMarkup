# İpuçları ve Sorun Giderme

FmgLib.MauiMarkup ile çalışırken en sık ortaya çıkan sorulara pratik yanıtlar.

## Adlandırma ve Keşfedilebilirlik

**"Bir metot adını nasıl tahmin ederim?"**

- Özellik `Foo` → metot `.Foo(value)`. Her zaman.
- Olay `Bar` → `.OnBar(handler)` veya `.OnBar(sender => …)`.
- Attached property `Owner.Prop` → `.OwnerProp(...)` (`Shell.TitleColor` → `ShellTitleColor`); istisna: sahip önekini atan Grid yerleşimi (`Grid.Row` → `Row`). Tam tablo: [Attached Property'ler](attached-properties.md).
- Bir alt sınıfta `new` ile **farklı tiple** gizlenen özellik → `.PropNameNew(...)` ([ayrıntılar](third-party-controls.md#yeniden-bildirilen-özellik-kuralları)).

**"Beklediğim bir metot yok."**

1. `using FmgLib.MauiMarkup;` (veya global using) var mı kontrol edin.
2. Özellik gerçek bir `BindableProperty` mi? Düz CLR özellikleri fluent metot almaz — [`InvokeOnElement`](assign-and-references.md) veya [özel genişletme](custom-extension-methods.md) kullanın.
3. Üçüncü parti kontrol mü? `[MauiMarkup(typeof(...))]` veya otomatik generator modu gerekir ([Üçüncü Parti Kontroller](third-party-controls.md)).
4. Metot muhtemelen **aynı adla bir taban sınıf genişletmesinde** yaşıyor (yaprak tip onu miras alır). Yalnızca gerçekten tip değiştiren yeniden bildirimler `New` ekini taşır.

## Yaygın Derleme Hataları

**"FmgLib metodu ile başka bir markup kütüphanesi arasında belirsiz çağrı"** — aynı dosyanın using'lerinden diğer fluent-markup paketlerini (örn. `CommunityToolkit.Maui.Markup`) kaldırın veya tam nitelendirin. İki kütüphaneyi bir projede karıştırmak çalışır, ama bir dosyada karıştırmaktan kaçının.

**Bir özellik metodunda "lambda ifadesi dönüştürülemiyor"** — builder lambda'sı builder zincirini **döndürmelidir**: `e => e.Path("X")` (ifade), `e => { e.Path("X"); }` (void döndüren deyim) değil.

**`MenuFlyout` / `Style<T>` / `VisualState<T>`'te koleksiyon başlatıcı sözdizimi başarısız** — fluent çağrılar `{ … }` başlatıcı bloğundan **sonra** gelmeli:

```csharp
new MenuFlyoutSubItem() { /* öğeler */ }.Text("Alt menü")   // ✅
```

## Çalışma Zamanı Tuzakları

**Binding sessizce hiçbir şey yapmıyor**

- `Path` string yazım hatası — bunları derleme hatasına çevirmek için [derlenmiş binding'leri](compiled-bindings.md) (`e.Getter(...)`) tercih edin.
- Yanlış `BindingContext` — şablonların öğeye yeniden bağlandığını unutmayın; kaçmak için `.Source(...)` kullanın ([Özellik Bağlama](data-binding.md)).
- Bağlanan nesne `INotifyPropertyChanged` tetiklemiyor.

**`Center()` metnimi ortalamıyor** — `Center()` kontrolü ebeveyninde konumlandırır; `TextCenter()` metni kontrolün içinde hizalar. Bkz. [Yerleşim Seçenekleri](layout-options.md) vs [Metin Hizalama](text-alignment.md).

**Tema değerleri OS tema değişiminde güncellenmiyor** — builder kullandığınızdan emin olun (`.TextColor(e => e.OnLight(...).OnDark(...))`), bir kez değerlendirilen ternary değil (`.TextColor(isDark ? … : …)`).

**Hot reload sayfayı yenilemiyor** — tam kontrol listesi: [Hot Reload](hot-reload.md#sorun-giderme). Özetle: bir hot-reload kanalından çalıştırın (`dotnet watch run` veya hot reload'lu IDE debug), Debug Console'da `update received` satırını arayın, sayfanın `IFmgLibHotReload` uyguladığından emin olun.

**Hot reload ile düzenlerken durum sıfırlanıyor** — durumu `Build()`'ten alanlara/constructor'a taşıyın.

**Hot reload sonrası çift olay işleyicileri** — `Build()` içinde uzun ömürlü nesnelere bağlanan handler'lar birikir; bunları constructor'a taşıyın.

## Mimari Önerileri

- **Bir sayfa = bir sınıf**, UI `Build()`'te, durum alanlarda, mantık view model'de. [`FmgLibContentPage<TViewModel>`](hot-reload.md#fmglibcontentpagetviewmodel--mvvm-tabanı) tabanı size tipli bir `BindingContext` verir.
- **Tekrarlayan alt ağaçları** private metotlara veya `ContentView` bileşenlerine çıkarın — XAML'e göre en büyük okunabilirlik kazancı budur ([Tam Örnekler](complete-examples.md#4-yeniden-kullanılabilir-bileşen--bindable-özellikli-özel-contentview)).
- **Tasarım token'larını merkezileştirin**: statik bir `AppColors` / `AppStyles` sınıfı artı uygulama düzeyi `ResourceDictionary` ([Stiller](styling.md)).
- Tüm view-model yolları için **derlenmiş binding'leri** tercih edin; string yolları yalnızca hızlı kontrolden-kontrole bağlantı için tutun.
- **Doğru tepki aracını seçin**: sabit → doğrudan değer; VM-sürücülü → binding; duruma bağlı görseller → [visual state'ler](visual-states.md); koşula bağlı özellikler → [trigger'lar](triggers.md); yeniden kullanılabilir kontrol mantığı → [behavior'lar](behaviors.md); tek seferlik → [olay işleyici](event-handlers.md).

## Performans Notları

- Doğrudan değerler (`.FontSize(14)`) düz `SetValue` çağrılarıdır — sıfır binding ek yükü. Hiç değişmeyeni bağlamayın.
- [Derlenmiş binding'ler](compiled-bindings.md) reflection'dan kaçınır; özellikle öğe şablonlarında kullanın.
- `CollectionView` sanallaştırır; [`BindableLayout`](collections-and-templates.md#bindablelayout--herhangi-bir-layoutta-şablonlu-öğeler) sanallaştırmaz — küçük tutun.
- Mümkünse layout özellikleri yerine transform'ları (`Translation`, `Scale`, `Opacity`) animasyonlayın ([Animasyonlar](animations.md#performans-i̇puçları)).
- Otomatik generator modunda üretim referans edilen her kontrolü kapsar — derleme süreleri artarsa açık `[MauiMarkup]` attribute'lerine geçin.

## SSS

**XAML ve FmgLib sayfalarını karıştırabilir miyim?** Evet — sayfa başına. Kademeli geçiş yapın.

**Shell/Navigation/DI/Essentials ile çalışıyor mu?** Evet; kütüphane yalnızca *view'ları nasıl kurduğunuzu* değiştirir, uygulama mimarisini değil. Bkz. [Shell Uygulamaları](shell-navigation.md).

**CommunityToolkit.Mvvm, `ObservableObject`, `RelayCommand` çalışıyor mu?** Tamamen — `e.Path(...)`/`e.Getter(...)` ile bağlayın ve `.Command(...)` her zamanki gibi.

**Kütüphanenin kapsamadığı bir özelliği nasıl ayarlarım?** [`InvokeOnElement`](assign-and-references.md#invokeonelement--zincirin-ortasında-rastgele-kod) veya yakalanan referans üzerinde düz C#.

**Gerçek dünya örnekleri nerede?** Deponun [`sample/`](../../sample) klasöründe — oyunlar ve mağaza tarzı UI'lar dahil eksiksiz uygulamalar.

**Hangi .NET sürümleri destekleniyor?** Güncel paket hattı .NET 10 (MAUI 10) hedefler; [proje şablonu](getting-started.md) `--netMajor` ile .NET 9 veya 10 iskeleti kurabilir.

## İlgili Konular

- [Başlarken](getting-started.md)
- [Tam Örnekler](complete-examples.md)
