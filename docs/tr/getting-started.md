# Başlarken

Bu sayfa, sıfırdan çalışan bir FmgLib.MauiMarkup uygulamasına iki yoldan ulaştırır: resmi şablondan yeni proje oluşturmak veya mevcut bir MAUI uygulamasına kütüphaneyi eklemek.

## Ön Koşullar

- MAUI workload'u kurulu .NET SDK 9 veya 10 (`dotnet workload install maui`)
- Herhangi bir IDE: Visual Studio, VS Code (.NET MAUI eklentisiyle) veya Rider

## Seçenek A — Şablondan Yeni Proje (önerilen)

FmgLib, FmgLib.MauiMarkup için hazır (XAML sayfası olmayan, hot reload yapılandırılmış) bir MAUI uygulaması üreten `dotnet new` şablonu yayınlar.

**1. Şablonu kurun:**

```bash
dotnet new install FmgLib.MauiMarkup.Template
```

**2. Projeyi oluşturun:**

```bash
dotnet new fmglib-mauimarkup-app -o YeniUygulamam
```

**Şablon parametreleri:**

| Parametre | Değerler | Varsayılan | Açıklama |
|---|---|---|---|
| `--netMajor` | `10`, `9` | `10` | Hedef .NET / MAUI ana sürümü. Uyumlu `Microsoft.Maui.Controls` ve `FmgLib.MauiMarkup` paket sürümlerini de seçer. |
| `--includeContent` | `true`, `false` | `false` | `false` minimal bir ana sayfa üretir; `true` öğrenip sonra silebileceğiniz örnek sayfalarla daha zengin bir deneyim kurar. |

Örnekler:

```bash
# .NET 10, minimal tek sayfa
dotnet new fmglib-mauimarkup-app -o MyApp

# .NET 9, örnek içerik sayfalarıyla
dotnet new fmglib-mauimarkup-app -o MyApp --netMajor 9 --includeContent true
```

**3. Çalıştırın:**

```bash
cd MyApp
dotnet build -t:Run -f net10.0-android    # veya -f net10.0-ios / net10.0-maccatalyst
```

Windows'ta Visual Studio içinde F5 de yeterlidir.

## Seçenek B — Mevcut MAUI Projesine Ekleme

NuGet paketini kurun:

```bash
dotnet add package FmgLib.MauiMarkup
```

Hepsi bu — `builder.UseFmgLibMauiMarkup()` gibi bir kayıt adımı **yoktur**. Paket şunları içerir:

- her MAUI kontrolü için **fluent genişletme metotları** (kütüphaneye derlenmiş halde) ve
- üçüncü parti kontroller için de fluent metot üretebilen **bir Roslyn source generator** (bkz. [Üçüncü Parti Kontroller](third-party-controls.md)).

> Yalnızca isteğe bağlı [JSON](localization-json.md) / [RESX](localization-resx.md) yerelleştirme özelliği `MauiProgram.cs` içinde bir çağrı gerektirir (`UseMauiMarkupLocalization…`).

### Mevcut bir XAML sayfasını dönüştürme

Kademeli geçiş yapabilirsiniz — XAML sayfaları ile FmgLib sayfaları aynı uygulamada sorunsuz birlikte yaşar. Tipik bir dönüşüm:

**Önce (`MainPage.xaml` + code-behind):**

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             x:Class="MyApp.MainPage">
    <VerticalStackLayout Spacing="25" Padding="30">
        <Label Text="Hello, World!" FontSize="32" HorizontalOptions="Center" />
        <Button x:Name="CounterBtn" Text="Click me" Clicked="OnCounterClicked" />
    </VerticalStackLayout>
</ContentPage>
```

**Sonra (tek `MainPage.cs`; `.xaml` ve `.xaml.cs` dosyalarını silin):**

```csharp
using FmgLib.MauiMarkup;

namespace MyApp;

public partial class MainPage : ContentPage, IFmgLibHotReload
{
    int count = 0;

    public MainPage()
    {
        this.InitializeHotReload();
    }

    public void Build()
    {
        this
        .Content(
            new VerticalStackLayout()
            .Spacing(25)
            .Padding(30)
            .Children(
                new Label()
                    .Text("Hello, World!")
                    .FontSize(32)
                    .CenterHorizontal(),

                new Button()
                    .Text("Click me")
                    .OnClicked(OnCounterClicked)
            )
        );
    }

    private void OnCounterClicked(Button sender)
    {
        count++;
        sender.Text = $"Clicked {count} times";
    }
}
```

Önemli noktalar:

- `using FmgLib.MauiMarkup;` tüm fluent genişletmeleri kapsama alır. Bir dosyaya (örn. `GlobalUsings.cs`) `global using` olarak ekleyin, bir daha hiç yazmayın:

  ```csharp
  global using FmgLib.MauiMarkup;
  ```

- `IFmgLibHotReload` implementasyonu + `this.InitializeHotReload()` çağrısı, UI için .NET Hot Reload sağlar (geliştirme sırasında kodu düzenlediğinizde `Build()` yeniden çalışır). İsteğe bağlıdır ama şiddetle önerilir — ayrıntılar: [Hot Reload](hot-reload.md).
- İsterseniz hot reload'u tamamen atlayıp UI'yi doğrudan constructor'da kurabilirsiniz:

  ```csharp
  public MainPage()
  {
      this.Content(new Label().Text("Merhaba"));
  }
  ```

### Sayfayı kaydetme

Normal MAUI'den farkı yok. Örneğin `App.cs` içinde:

```csharp
public partial class App : Application
{
    public App()
    {
        this.MainPage(new AppShell());   // Application.MainPage için fluent setter
    }
}
```

veya pencere kullanan daha yeni MAUI sürümlerinde:

```csharp
protected override Window CreateWindow(IActivationState? activationState)
    => new Window(new AppShell());
```

## Bir Markup Sayfasının Anatomisi

```csharp
public partial class LoginPage : ContentPage, IFmgLibHotReload   // 1
{
    public LoginPage()
    {
        this.InitializeHotReload();                              // 2
    }

    public void Build()                                          // 3
    {
        this                                                     // 4
        .Title("Giriş")
        .BackgroundColor(Colors.White)
        .Content(                                                // 5
            new VerticalStackLayout()
            .Children(                                           // 6
                new Entry().Placeholder("E-posta").Assign(out var email),
                new Entry().Placeholder("Şifre").IsPassword(true),
                new Button().Text("Giriş Yap")
            )
        );
    }
}
```

1. Sayfa, bir MAUI sayfa tipinden türeyen sıradan bir C# sınıfıdır.
2. `InitializeHotReload()`, `Build()` metodunu bir kez çağırır ve hot reload'da yeniden tetikler.
3. Tüm UI kurulumu `Build()` içinde yaşar.
4. `this` bir `ContentPage` olduğundan sayfa düzeyindeki tüm fluent metotlar (`Title`, `BackgroundColor`, `Content`, …) ondan zincirlenir.
5. `Content(...)`, `ContentPage.Content` özelliğini ayarlar.
6. `Children(...)` gibi konteyner metotları çocukları `params` olarak alır; kodun girintisi görsel ağacı aynı XAML iç içeliği gibi yansıtır.

## Sonraki Adımlar

- [XAML'den C#'a](xaml-to-csharp.md) — herhangi bir XAML parçasını çevirmenin zihinsel modeli.
- [Fluent Özellikler](fluent-properties.md) — özellik metotlarının yapabildiği her şey (binding'ler, platform bazlı değerler, dinamik kaynaklar…).
- [Hot Reload](hot-reload.md) — önerilen sayfa yaşam döngüsü ve MVVM için `FmgLibContentPage<TViewModel>` taban sınıfı.
