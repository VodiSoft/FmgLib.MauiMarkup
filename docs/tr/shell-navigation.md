# Shell Uygulamaları

.NET MAUI Shell; flyout menüleri, sekmeler ve URI tabanlı navigasyon sağlar. FmgLib.MauiMarkup tüm shell hiyerarşisini fluent kurar — `Shell`, `FlyoutItem`, `Tab`, `ShellContent` ve şablonları.

## Eksiksiz Bir Shell

```csharp
using FmgLib.MauiMarkup;

public partial class App : Application
{
    public App()
    {
        this.MainPage(
            new Shell()
            .ItemTemplate(() => new ShellItemTemplate())
            .Resources(new ResourceDictionary().MergedDictionaries(AppStyles.Default))
            .Items(
                new FlyoutItem()
                .FlyoutDisplayOptions(FlyoutDisplayOptions.AsMultipleItems)
                .Items(
                    new Tab()
                    .Title("Ana")
                    .Items(
                        new ShellContent()
                            .Title("Hello Page")
                            .ContentTemplate(() => new HelloWorldPage()),
                        new ShellContent()
                            .Title("Example Page")
                            .ContentTemplate(() => new ExamplePage())
                    ),

                    new ShellContent()
                        .Title("Grid")
                        .ContentTemplate(() => new GridPage())
                )
            )
        );
    }
}
```

Hiyerarşi XAML Shell'i birebir yansıtır:

```
Shell
└── FlyoutItem (flyout girdileri)
    ├── Tab (alt sekmeler)
    │   └── ShellContent (üst sekmeler / sayfalar)
    └── ShellContent
```

- `FlyoutDisplayOptions.AsMultipleItems` her çocuğu kendi flyout girdisine yükseltir.
- `ContentTemplate(() => new SomePage())` bir **`Func<object>` lambda** (veya `DataTemplate` örneği) alır — sayfa istendiğinde, girdiye ilk gidildiğinde oluşturulur. Doğrudan sayfa örneği geçmek geçerli bir overload değildir; lambda formunu kullanın. (Eager oluşturma `.Content(new SomePage())` ile mümkündür ama önerilen varsayılan lazy şablondur.)

## Flyout Görünümünü Özelleştirme

Shell'e `ItemTemplate` verin ve bir şablon view tanımlayın. Şablonun içinde binding'ler shell öğesine (`Title`, `FlyoutIcon`) göre çözülür:

```csharp
public class ShellItemTemplate : ContentView
{
    public ShellItemTemplate()
    {
        this
        .Content(
            new Grid()
            .ColumnDefinitions(e => e.Star(0.2).Star(0.8))
            .Children(
                new Image()
                    .Source(e => e.Path("FlyoutIcon"))
                    .Margin(5)
                    .HeightRequest(45),

                new Label()
                    .Column(1)
                    .Text(e => e.Path("Title"))
                    .FontSize(20)
                    .FontAttributes(FontAttributes.Italic)
                    .CenterVertical()
            )
        );
    }
}
```

Diğer flyout düzeyi özelleştirmeler:

```csharp
new Shell()
    .FlyoutBehavior(FlyoutBehavior.Flyout)          // veya Locked / Disabled
    .FlyoutBackgroundColor(Colors.WhiteSmoke)
    .FlyoutHeader(
        new Grid().HeightRequest(120).Children(
            new Image().Source("banner.png").Aspect(Aspect.AspectFill),
            new Label().Text("Uygulamam").TextColor(Colors.White).AlignBottomLeft().Margin(16)
        )
    )
    .FlyoutFooter(new Label().Text("v1.0.0").TextCenter().Padding(8))
    .MenuItemTemplate(() => new MenuItemTemplateView())
```

## İkonlar, Route'lar ve Kayıt

```csharp
new ShellContent()
    .Title("Siparişler")
    .Icon("orders.png")
    .Route("orders")
    .ContentTemplate(() => new OrdersPage())
```

Ek (shell'de görünmeyen) route'ları her zamanki gibi kaydedin:

```csharp
Routing.RegisterRoute("orders/detail", typeof(OrderDetailPage));
```

Sonra gezinin:

```csharp
await Shell.Current.GoToAsync("orders/detail?id=42");
await Shell.Current.GoToAsync("..");                    // geri
```

## Sayfa Başına Shell Attached Property'leri

Her `Shell.*` attached property'sinin sayfalarda kullanılabilir bir fluent metodu vardır (tam tablo: [Attached Property'ler](attached-properties.md)):

```csharp
public partial class CheckoutPage : ContentPage, IFmgLibHotReload
{
    public CheckoutPage() => this.InitializeHotReload();

    public void Build() =>
        this
        .ShellTabBarIsVisible(false)                 // burada tab bar'ı gizle
        .ShellNavBarHasShadow(false)
        .ShellTitleView(                             // özel başlık view'ı
            new HorizontalStackLayout().Spacing(6).Children(
                new Image().Source("cart.png").SizeRequest(22, 22),
                new Label().Text("Ödeme").FontSize(18).CenterVertical()
            )
        )
        .ShellBackButtonBehavior(
            new BackButtonBehavior()
                .TextOverride("Geri")
                .Command(new Command(async () => await ConfirmLeaveAsync()))
        )
        .Content(/* ... */);
}
```

Shell chrome renklendirme (genelde Shell'in kendisinde bir kez, veya sayfa başına):

```csharp
new Shell()
    .ShellBackgroundColor(AppColors.Primary)   // nav bar arka planı
    .ShellForegroundColor(Colors.White)        // nav bar ikon/metin
    .ShellTitleColor(Colors.White)
    .ShellTabBarBackgroundColor(Colors.White)
    .ShellTabBarTitleColor(AppColors.Primary)
    .ShellTabBarUnselectedColor(Colors.Gray)
```

## Yalnızca Sekmeli Uygulama (flyout yok)

```csharp
new Shell()
.FlyoutBehavior(FlyoutBehavior.Disabled)
.Items(
    new TabBar()
    .Items(
        new ShellContent().Title("Ana").Icon("home.png").ContentTemplate(() => new HomePage()),
        new ShellContent().Title("Arama").Icon("search.png").ContentTemplate(() => new SearchPage()),
        new ShellContent().Title("Profil").Icon("user.png").ContentTemplate(() => new ProfilePage())
    )
)
```

## Arama İşleyicisi (Search Handler)

```csharp
public class ProductSearchHandler : SearchHandler
{
    protected override void OnQueryChanged(string oldValue, string newValue)
    {
        ItemsSource = string.IsNullOrWhiteSpace(newValue)
            ? null
            : ProductService.Search(newValue);
    }
}

// bir sayfada:
this.ShellSearchHandler(
    new ProductSearchHandler()
        .Placeholder("Ürün ara…")
        .ShowsResults(true)
);
```

## Shell Menü Öğeleri

Flyout'ta düz aksiyon girdileri:

```csharp
new Shell()
.Items(
    // ... flyout öğeleri ...
)
.InvokeOnElement(shell => shell.Items.Add(
    new MenuItem()
        .Text("Çıkış")
        .IconImageSource("logout.png")
        .OnClicked(async _ => await AuthService.LogoutAsync())
))
```

## İlgili Konular

- [Attached Property'ler](attached-properties.md) — tüm `Shell.*` metotları
- [Hot Reload](hot-reload.md) — shell içeriğinde kullanılan sayfa yapısı
- [Menüler](menus.md) — pencere menü çubukları ve bağlam menüleri
