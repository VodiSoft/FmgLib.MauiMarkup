# Menüler

FmgLib.MauiMarkup, MAUI'nin iki menü sistemini de fluent olarak kapsar:

- **Bağlam menüleri** (`MenuFlyout`) — kontrollerde sağ tık / uzun basma menüleri (masaüstü odaklı).
- **Menü çubukları** (`MenuBarItem`) — Windows ve macOS'ta üst uygulama menüsü.

## Bağlam Menüleri (`FlyoutBase.ContextFlyout`)

Herhangi bir kontrole `ContextFlyout(...)` ile `MenuFlyout` bağlayın. Menü öğeleri koleksiyon başlatıcı sözdizimiyle eklenir ve fluent yapılandırılır:

```csharp
new Grid()
.Assign(out var grid)
.Children(
    new Image()
        .Source("dotnet_bot.png")
        .ContextFlyout(new MenuFlyout()
        {
            new MenuFlyoutItem()
                .Text("Kopyala")
                .OnClicked(e => Console.WriteLine("Copy")),

            new MenuFlyoutItem()
                .Text("Yapıştır")
                .OnClicked(e => Console.WriteLine("Paste")),

            new MenuFlyoutSubItem()
            {
                new MenuFlyoutItem()
                    .Text("Mavi")
                    .OnClicked(e => grid.BackgroundColor = Colors.Blue),
                new MenuFlyoutItem()
                    .Text("Kırmızı")
                    .OnClicked(e => grid.BackgroundColor = Colors.Red),
                new MenuFlyoutItem()
                    .Text("Siyah")
                    .OnClicked(e => grid.BackgroundColor = Colors.Black)
            }
            .Text("Arka plan rengi")
        })
)
```

Notlar:

- `MenuFlyoutSubItem` alt menü iç içe koyar; `.Text(...)` çağrısını başlatıcı bloğundan SONRA yapın (fluent metotlar başlatıcı bloğundan zincirlenir).
- `MenuFlyoutSeparatorItem` ayırıcı çizgi ekler.
- Öğeler `.IconImageSource(...)`, `.IsEnabled(...)` ve MVVM için `.Command(...)` destekler:

```csharp
new MenuFlyoutItem()
    .Text("Sil")
    .IconImageSource("trash.png")
    .Command(BindingContext.DeleteCommand)
    .CommandParameter(e => e.Path("."))
```

- Bağlam menüleri **Windows ve Mac Catalyst**'te etkilidir; mobilde `SwipeView` aksiyonlarını veya uzun basma jestlerini tercih edin.

## Menü Çubukları (`Page.MenuBarItems`)

Menü çubuğu sayfada `MenuBarItems(...)` ile tanımlanır. Her `MenuBarItem`, çocukları flyout öğeleri olan bir üst düzey menüdür:

```csharp
public class MenuPage : ContentPage
{
    public MenuPage()
    {
        this.MenuBarItems(new MenuBarItem[]
        {
            new MenuBarItem()
            {
                new MenuFlyoutItem()
                    .Text("Çıkış")
                    .OnClicked(e => Application.Current.Quit()),
            }
            .Text("Menüm"),

            new MenuBarItem()
            {
                new MenuFlyoutItem()
                    .Text("Kopyala")
                    .OnClicked(e => Console.WriteLine("Copy"))
                    .KeyboardAccelerators(
                        new KeyboardAccelerator()
                            .Key("C")
                            .Modifiers(KeyboardAcceleratorModifiers.Ctrl)
                    ),

                new MenuFlyoutItem()
                    .Text("Yapıştır")
                    .OnClicked(e => Console.WriteLine("Paste"))
                    .KeyboardAccelerators(
                        new KeyboardAccelerator()
                            .Key("V")
                            .Modifiers(KeyboardAcceleratorModifiers.Ctrl)
                    ),
            }
            .Text("Düzen"),

            new MenuBarItem()
            {
                new MenuFlyoutItem()
                    .Text("Mavi")
                    .OnClicked(e => this.BackgroundColor = Colors.Blue),
                new MenuFlyoutItem()
                    .Text("Koyu")
                    .OnClicked(e => this.BackgroundColor = Colors.Black),
            }
            .Text("Tema")
        });

        this.Content(/* sayfa içeriği */);
    }
}
```

### Klavye kısayolları

`KeyboardAccelerators(...)` menü öğelerine kısayol bağlar. Değiştiricileri `|` ile birleştirin:

```csharp
new KeyboardAccelerator()
    .Key("S")
    .Modifiers(KeyboardAcceleratorModifiers.Ctrl | KeyboardAcceleratorModifiers.Shift)  // Ctrl+Shift+S
```

### Platform davranışı

- **Windows** — menü çubuğu pencerenin üstünde görünür.
- **Mac Catalyst** — öğeler sistem menü çubuğuna karışır.
- **iOS/Android** — menü çubukları gösterilmez; çapraz platform sayfalarda `DeviceInfo.Idiom == DeviceIdiom.Desktop` ile koruyun.

### Menü çubuğunda alt menüler

`MenuFlyoutSubItem`, `MenuBarItem` içinde de aynı şekilde çalışır:

```csharp
new MenuBarItem()
{
    new MenuFlyoutSubItem()
    {
        new MenuFlyoutItem().Text("Türkçe").OnClicked(_ => SetLang("tr-TR")),
        new MenuFlyoutItem().Text("İngilizce").OnClicked(_ => SetLang("en-US")),
    }
    .Text("Dil"),
}
.Text("Ayarlar")
```

## Toolbar Öğeleri

İlgili ama farklı: sayfa düzeyindeki araç çubuğu butonları fluent `ToolbarItem` API'siyle `ToolbarItems(...)` kullanır:

```csharp
this.ToolbarItems(
    new ToolbarItem()
        .Text("Yenile")
        .IconImageSource("refresh.png")
        .OnClicked(async _ => await ViewModel.RefreshAsync())
);
```

## İlgili Konular

- [Olay İşleyiciler](event-handlers.md)
- [Shell Uygulamaları](shell-navigation.md) — shell düzeyi `MenuItem`'lar ve flyout menüleri
