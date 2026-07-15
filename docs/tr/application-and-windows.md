# Application, Pencereler ve Diğer Kök Sayfalar

Sayfa ve view'ların ötesinde FmgLib.MauiMarkup, application nesnesini, pencereleri ve Shell dışı kök sayfa tiplerini (`NavigationPage`, `TabbedPage`, `FlyoutPage`) kapsar.

## `Application` Genişletmeleri

`App` sınıfınızda kullanılır:

```csharp
public partial class App : Application
{
    public App()
    {
        this
        .Resources(AppStyles.Default)                       // uygulama geneli ResourceDictionary
        .UserAppTheme(AppTheme.Unspecified)                 // Light/Dark zorla veya OS'u takip et
        .MainPage(new AppShell())                           // kök sayfa
        .OnRequestedThemeChanged((s, e) =>                  // OS tema değişimi olayı
            Console.WriteLine($"Tema: {e.RequestedTheme}"));
    }
}
```

| Metot | Amaç |
|---|---|
| `.MainPage(Page)` | Kök sayfayı ayarlar |
| `.Resources(ResourceDictionary)` | Uygulama düzeyi kaynaklar ([Stiller](styling.md)) |
| `.UserAppTheme(AppTheme)` | OS temasını geçersiz kıl (`Light`/`Dark`/`Unspecified`) |
| `.AccentColor(Color)` | Uygulama vurgu rengi |
| `.OnRequestedThemeChanged(...)` | OS açık/koyu değişimine tepki |
| `.OnModalPushed/Popped/Pushing/Popping(...)` | Modal navigasyon olayları |
| `.OnPageAppearing/OnPageDisappearing(...)` | Global sayfa yaşam döngüsü olayları |

> Daha yeni MAUI sürümlerinde `CreateWindow`'u override edip `new Window(new AppShell())` döndürmeyi tercih edebilirsiniz — ikisi de çalışır; `MainPage(...)` fluent kısayoldur.

## `Window` Genişletmeleri

Pencereler fluent'tir — boyutlandırma, konum, başlık ve yaşam döngüsü olayları:

```csharp
protected override Window CreateWindow(IActivationState? state) =>
    new Window(new AppShell())
        .Title("Uygulamam")
        .Width(1100).Height(720)                  // masaüstü platformlar
        .MinimumWidth(800).MinimumHeight(600)
        .X(100).Y(100)
        .OnCreated(w => Console.WriteLine("pencere oluştu"))
        .OnActivated(w => { /* odak aldı */ })
        .OnDeactivated(w => { /* odak kaybetti */ })
        .OnResumed(w => { /* öne döndü */ })
        .OnStopped(w => { /* arka plana gitti */ })
        .OnDestroying(w => SaveState());
```

Bu pencere yaşam döngüsü olayları, MAUI'de uygulama askıya-alma/devam mantığı için en temiz yerdir.

### TitleBar (Windows, .NET 9+)

`TitleBar` kontrolü pencere chrome'unu özelleştirir ve tamamen fluent'tir:

```csharp
new Window(new AppShell())
    .TitleBar(
        new TitleBar()
            .Icon("appicon.png")
            .Title("Uygulamam")
            .Subtitle("Önizleme")
            .BackgroundColor("#20242B".ToColor())
            .ForegroundColor(Colors.White)
            .Content(new SearchBar().Placeholder("Ara…").MaximumWidthRequest(400))
            .TrailingContent(new ImageButton().Source("avatar.png").SizeRequest(28, 28))
    )
```

`PassthroughElements(...)` etkileşimli çocukları işaretler; böylece tıklamalar pencereyi sürüklemek yerine onlara ulaşır.

## Shell Dışı Kök Sayfalar

[Shell](shell-navigation.md) önerilen varsayılandır, ama klasik konteynerler de tam desteklenir.

### NavigationPage

```csharp
this.MainPage(
    new NavigationPage(new HomePage())
        .BarBackgroundColor(AppColors.Primary)
        .BarTextColor(Colors.White)
);

// her zamanki gibi push/pop:
await Navigation.PushAsync(new DetailPage());
await Navigation.PopAsync();
```

Sayfa başına ayarlar `NavigationPage.*` [attached-property metotlarını](attached-properties.md) kullanır:

```csharp
public void Build() =>
    this
    .NavigationPageHasNavigationBar(false)
    .NavigationPageBackButtonTitle("")
    .Content(/* ... */);
```

### TabbedPage

```csharp
this.MainPage(
    new TabbedPage()
        .Children(
            new HomePage().Title("Ana").IconImageSource("home.png"),
            new SettingsPage().Title("Ayarlar").IconImageSource("gear.png")
        )
        .BarBackgroundColor(Colors.White)
        .SelectedTabColor(AppColors.Primary)
        .UnselectedTabColor(Colors.Gray)
        .OnCurrentPageChanged(tp => Console.WriteLine(tp.CurrentPage.Title))
);
```

### FlyoutPage

```csharp
this.MainPage(
    new FlyoutPage()
        .Flyout(
            new ContentPage()
                .Title("Menü")
                .Content(
                    new VerticalStackLayout().Children(
                        new Button().Text("Ana").OnClicked(_ => Navigate(new HomePage())),
                        new Button().Text("Siparişler").OnClicked(_ => Navigate(new OrdersPage()))
                    )
                )
        )
        .Detail(new NavigationPage(new HomePage()))
        .FlyoutLayoutBehavior(FlyoutLayoutBehavior.Popover)
        .Assign(out flyoutPage)
);

void Navigate(Page page)
{
    flyoutPage.Detail = new NavigationPage(page);
    flyoutPage.IsPresented = false;
}
```

## Modal Sayfalar

```csharp
await Navigation.PushModalAsync(
    new ContentPage()
        .BackgroundColor(Colors.Black.WithAlpha(0.6f))
        .Content(
            new Border()
                .Center()
                .Padding(24)
                .BackgroundColor(Colors.White)
                .StrokeShape(new RoundRectangle().CornerRadius(16))
                .Content(new Label().Text("Emin misiniz?"))
        )
);
```

Modalleri global izleyin (yukarıdaki `Application` olayları: `OnModalPushed`, `OnModalPopped`).

## İlgili Konular

- [Shell Uygulamaları](shell-navigation.md)
- [Attached Property'ler](attached-properties.md)
- [Başlarken](getting-started.md)
