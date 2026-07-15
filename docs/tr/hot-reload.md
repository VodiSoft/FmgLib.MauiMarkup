# .NET Hot Reload Desteği

FmgLib.MauiMarkup UI'ları düz C# olduğundan .NET'in yerleşik Hot Reload'u onlara uygulanır — kütüphane, **kod değiştiğinde UI kurulumunuzu yeniden çalıştıran** küçük bir handler ekler; edit'ler çalışan uygulamada yeniden başlatmadan görünür.

## Desen: `IFmgLibHotReload` + `Build()`

1. Sayfada `IFmgLibHotReload`'ı uygulayın (tek bir `void Build()` metodu bildirir).
2. Constructor'da `this.InitializeHotReload()` çağırın.
3. Tüm UI kurulumunu `Build()` içine koyun.

```csharp
public partial class ExamplePage : ContentPage, IFmgLibHotReload
{
    public ExamplePage()
    {
        this.InitializeHotReload();
    }

    public void Build()
    {
        this
        .Content(
            new Label()
            .Text("FmgLib.MauiMarkup")
            .CharacterSpacing(2)
            .FontSize(30)
            .FontAttributes(FontAttributes.Italic)
            .TextColor(Colors.Green)
            .TextCenter()
        );
    }
}
```

### `InitializeHotReload()` ne yapar

- `Build()`'i bir kez hemen çağırır (ilk kurulum).
- Sayfayı hot-reload handler'ına **her zaman** kaydeder — weak reference ile (üretimde hiçbir güncelleme gelmediğinden no-op'tur ve sayfanın ömrünü asla uzatmaz). Runtime bir kod güncellemesi uyguladığında, `Build()` her kayıtlı sayfa için **ana thread'de** yeniden çağrılır (varsayılan `RebuildAllOnUpdate = true` — pratikte tek güvenilir mod: sayfalar, edit'leri onları yenilemeli olan yardımcı sınıf/stilleri kompoze eder ve iOS/Android'deki Mono runtime çoğu zaman boş güncellenen-tip listesi bildirir).
- Her güncelleme debug çıktısına bir tanılama satırı yazar: `FmgLib.MauiMarkup hot reload: update received (types: …) — rebuilding N registered target(s).` — bu satırı görüyorsanız boru hattı uçtan uca çalışıyor demektir.
- Reload sırasında fırlatan bir `Build()` **uygulamayı asla çökertmez**: hata loglanır (`Trace`) ve `FmgLibHotReloadHandler.ReloadFailed` olayıyla iletilir; böylece edit'i düzeltip tekrar kaydedersiniz.

### Bellek güvenliği

Hot-reload kaydı **weak reference** kullanır (`FmgLibHotReloadHandler.Register`): kayıt bir sayfanın ömrünü asla uzatmaz. Sayfa navigasyondan çıkarılıp serbest bırakıldığında normal şekilde çöp toplanır ve kaydı otomatik temizlenir — leak detector'lar (örn. Nalu'nunki) hot reload'a takılı sayfalar bildirmez. Aynı örneği iki kez kaydetmek no-op'tur; canlı bir sayfayı yeniden kurulumlardan çıkarmak isterseniz `FmgLibHotReloadHandler.Unregister(page)` de vardır.

## Hazır Taban Sınıflar

Kütüphane bunu sizin için bağlayan taban sayfalar sunar.

### `FmgLibContentPage`

```csharp
public class HomePage : FmgLibContentPage
{
    public override void Build() =>
        this.Content(
            new Label().Text("Merhaba!").Center()
        );
}
```

Constructor tesisatı yok — taban sınıf hot reload'a kaydolur ve `Build()`'i çağırır.

### `FmgLibContentPage<TViewModel>` — MVVM tabanı

View model'i constructor'da alır, **ilk `Build()` çalışmadan önce** `BindingContext`'e atar (böylece `Build()` view model'i güvenle okuyabilir) ve — güzel kısmı — **`BindingContext` özelliğini yeniden tipler**; VM'inize cast olmadan erişirsiniz:

```csharp
public class ProfilePage : FmgLibContentPage<ProfileViewModel>
{
    public ProfilePage(ProfileViewModel vm) : base(vm) { }

    public override void Build() =>
        this.Content(
            new VerticalStackLayout()
            .Padding(20)
            .Children(
                new Label().Text(e => e.Getter(static (ProfileViewModel v) => v.UserName)),

                new Button()
                    .Text("Yenile")
                    .Command(BindingContext.RefreshCommand)   // tipli! cast yok
            )
        );
}
```

Dependency injection ile doğal eşleşir:

```csharp
builder.Services.AddTransient<ProfileViewModel>();
builder.Services.AddTransient<ProfilePage>();
```

## Reload-Güvenli `Build()` Kuralları

`Build()` bir debug oturumunda **birçok kez** çalışabilir. Buna göre yapılandırın:

**✅ Yapın**

- `Build()`'i idempotent yapın: her seferinde UI'yi sıfırdan tam olarak tanımlamalı (`Content` ayarlamak eski ağacı değiştirir, bu doğal stildir).
- Durumu (sayaç, view model, servis referansı) `Build()`'te değil, **constructor'da başlatılan alanlarda** tutun:

  ```csharp
  private readonly MainPageViewModel viewModel;

  public MainPage()
  {
      viewModel = new MainPageViewModel();   // reload'ları atlatır
      this.InitializeHotReload();
  }

  public void Build() => this.BindingContext(viewModel).Content(/* ... */);
  ```

**❌ Kaçının**

- View model'i `Build()` içinde oluşturmaktan — her reload'da uygulama durumunu sıfırlarsınız.
- **Uzun ömürlü/statik olaylara** `Build()` içinde abone olmaktan (örn. `Application.Current.RequestedThemeChanged += …`) — abonelikler her yeniden kurulumda birikir. Bunu constructor'da yapın.
- Animasyon veya ağ çağrılarını doğrudan `Build()`'te başlatmaktan; `OnLoaded`/`OnAppearing` [olay işleyicilerini](event-handlers.md) kullanın.

## Yalnızca Sayfalar Değil, Her View'da Çalışır

`IFmgLibHotReload` + `InitializeHotReload()` bir `ContentView` üzerinde de geçerlidir:

```csharp
public class ProductCard : ContentView, IFmgLibHotReload
{
    public ProductCard() => this.InitializeHotReload();

    public void Build() => this.Content(/* ... */);
}
```

## VS Code'da Tek Tuş Geliştirme Döngüsü (önerilen)

`fmglib-mauimarkup-app` şablonundan oluşturulan projeler hazır bir `.vscode/tasks.json` ile gelir. Mevcut projeler için ekleyin:

```jsonc
// .vscode/tasks.json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "🔥 Hot Reload: iOS Simulator",
            "type": "shell",
            "command": "dotnet",
            "args": [ "watch", "run", "-f", "net10.0-ios" ],
            "isBackground": true,
            "problemMatcher": []
        },
        {
            "label": "🔥 Hot Reload: Android",
            "type": "shell",
            "command": "dotnet",
            "args": [ "watch", "run", "-f", "net10.0-android" ],
            "isBackground": true,
            "problemMatcher": []
        }
    ]
}
```

Görevi çalıştırın (`Terminal → Run Task…`), `Build()`'i düzenleyin, kaydedin — değişiklik saniyeler içinde cihazda; terminalde hem `dotnet watch 🔥 … applied` hem kütüphanenin `update received … rebuilding N target(s)` onayı görünür. Breakpoint gerektiğinde F5 debug'ı ayrıca kullanın.

## IDE / Kanal Destek Matrisi

FmgLib'in handler'ı **.NET Hot Reload** (`MetadataUpdateHandler`) üzerine oturur; .NET araçlarının çalışan sürece kod güncellemesi iletebildiği her yerde çalışır:

| Kanal | Windows | macOS | Notlar |
|---|---|---|---|
| Visual Studio (F5 debug) | ✅ | — (emekli) | Tam destek, güncellemeler tip listesiyle iletilir |
| VS Code + C# Dev Kit / .NET MAUI eklentisi | ✅ | ✅ | Debug'da **`"csharp.experimental.debug.hotReload": true`** ayarı gerekir — onsuz F5 hiç güncelleme iletmez |
| `dotnet watch run` (CLI, herhangi bir editör) | ✅ | ✅ | Debugger gerekmez — en IDE-bağımsız, en güvenilir yol |
| Rider (debugger) | ❌ | ❌ | Rider'ın debugger'ı MAUI için .NET Hot Reload iletmez — Rider içinde bir `dotnet watch` run configuration kullanın |
| Düz `dotnet run` / Release build | ❌ | ❌ | Güncelleme kanalı yok — tasarım gereği, sıfır ek yük |

> **Öz-teşhis:** debugger bağlıyken süreç güncelleme alamıyorsa kütüphane debug çıktısına tek seferlik bir uyarı yazar (`FmgLib.MauiMarkup hot reload: … MetadataUpdater.IsSupported = false …`) ve hangi ayarı/kanalı düzelteceğinizi tam olarak söyler. Bu mesajı görüyorsanız, o oturumda ne değiştirirseniz değiştirin edit'ler uygulanmayacaktır.

Platform notları:

- **Android emülatör/cihaz (debug)**: yukarıdaki tüm kanallar destekler.
- **iOS / Mac Catalyst (debug)**: .NET Hot Reload, Mono **interpreter**'ını gerektirir; MAUI bunu Debug yapılandırmasında varsayılan açık tutar (`UseInterpreter`). Kapattıysanız Debug için yeniden açın.
- **Çalışma zamanında desteği belirleyen** `System.Reflection.Metadata.MetadataUpdater.IsSupported`'tır (araçlar başlatırken `DOTNET_MODIFIABLE_ASSEMBLIES=debug` ayarlar). `FmgLibHotReloadHandler.IsSupported`'ı kendiniz de kontrol edebilirsiniz (örn. yalnızca-geliştirme banner'ı göstermek için).

## Handler Seçenekleri ve Tanılama

```csharp
// Varsayılan TRUE: her güncellemede tüm kayıtlı sayfalar yeniden kurulur. Hedefli
// yeniden kuruluma (yalnızca kendi/taban tipi değişen sayfalar) geçmek isterseniz:
FmgLibHotReloadHandler.RebuildAllOnUpdate = false;

// Reload hatalarını gözlemleyin (zaten Trace ile loglanır; uygulamayı asla çökertmez):
FmgLibHotReloadHandler.ReloadFailed += (target, ex) =>
    Console.WriteLine($"{target.GetType().Name} için hot reload başarısız: {ex.Message}");

// Kurtarma çıkışı: TÜM kayıtlı sayfaları ŞİMDİ zorla yeniden kur — kodu uygulayan ama
// runtime handler'larını çağırmayan araçlar için. Yalnızca-debug bir jeste/butona bağlayın:
FmgLibHotReloadHandler.RebuildAll();
```

Örnek — bir sayfada yalnızca-debug yenileme jesti:

```csharp
#if DEBUG
this.GestureRecognizers(
    new TapGestureRecognizer()
        .NumberOfTapsRequired(3)
        .OnTapped((s, e) => FmgLibHotReloadHandler.RebuildAll()));
#endif
```

Hedefli-mod eşleşme kuralları: bir güncelleme, runtime kayıtlı hedefin tam tipini, **taban tiplerinden herhangi birini** (ortak taban sayfa düzenlemek tüm türemişleri yeniler), veya bilinmeyen/boş bir tip listesi bildirdiğinde yeniden kurar.

> Yalnızca `IFmgLibHotReload` uygulayan tipler (ve `InitializeHotReload` çağıran, veya `FmgLibContentPage`'ten türeyenler) kendini yeniden kurar. `AppShell`'inize veya düz bir `ContentView`'a yaptığınız edit görünmüyorsa, o tipe de aynı deseni uygulayın.

## Sorun Giderme

| Belirti | Neden / çözüm |
|---|---|
| Edit'ler görünmüyor (VS Code, debug) | Önce Debug Console'da `FmgLib.MauiMarkup hot reload: update received …` satırını arayın. **Satır yoksa**, eklenti değişiklikleri uygulamanın metadata update handler'larını bildirmeden uygulamıştır (Mono/mobil hedeflerde debug-launch kanalının bilinen bir boşluğu — `Hot Reload result: {"result":0, …}` tüm diziler boşken bir belirtidir). Kütüphaneden düzeltilemez: `dotnet watch run` ile iterasyon yapın (tam destekli), veya uygulanmış edit'leri render etmek için yalnızca-debug bir jestten `FmgLibHotReloadHandler.RebuildAll()` tetikleyin. Ayrıca `"csharp.experimental.debug.hotReload": true` ayarlı olsun. |
| Edit'ler görünmüyor (Rider, debug) | Rider'ın debugger'ı MAUI uygulamalarına .NET Hot Reload uygulayamaz. Bir `dotnet watch run -f <tfm>` run configuration oluşturun (emülatör/simülatörle çalışır) — handler'ımız watch'ı debugger'sız destekler. |
| Yardımcı `ContentView`/metot düzenlemesi onu kullanan sayfayı yenilemiyor | Ya bileşende `IFmgLibHotReload`'ı uygulayın (kendini yeniden kurar), ya da `FmgLibHotReloadHandler.RebuildAllOnUpdate = true` bırakın (varsayılan). |
| Edit'te UI durumu sıfırlanıyor | Durum `Build()` içinde yaşıyor; alanlara/constructor'a taşıyın. |
| Edit sonrası olaylar iki kez tetikleniyor | `Build()` içinde uzun ömürlü bir nesneye handler bağlanmış; constructor'a taşıyın. |
| IDE'den "rude edit" mesajları | Bazı kod değişiklikleri (metot imzası değiştirmek, bazı tiplere alan eklemek) .NET Hot Reload'un kapasitesini aşar — oturumu yeniden başlatın. Bu bir runtime/araç sınırıdır, kütüphane değil. |
| iOS cihaz edit'leri yok sayılıyor | Debug'ın Mono interpreter'ını kullandığından (MAUI varsayılanı) ve oturumun IDE/watch tarafından başlatıldığından emin olun. |

## İlgili Konular

- [Başlarken](getting-started.md)
- [Assign ve Referanslar](assign-and-references.md)
- [Olay İşleyiciler](event-handlers.md)
