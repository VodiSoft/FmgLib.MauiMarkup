# Shell Applications

.NET MAUI Shell provides flyout menus, tabs and URI-based navigation. FmgLib.MauiMarkup builds the entire shell hierarchy fluently — `Shell`, `FlyoutItem`, `Tab`, `ShellContent` and their templates.

## A Complete Shell

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
                    .Title("Main")
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

The hierarchy mirrors XAML Shell exactly:

```
Shell
└── FlyoutItem (flyout entries)
    ├── Tab (bottom tabs)
    │   └── ShellContent (top tabs / pages)
    └── ShellContent
```

- `FlyoutDisplayOptions.AsMultipleItems` promotes each child to its own flyout entry.
- `ContentTemplate(() => new SomePage())` takes a **`Func<object>` lambda** (or a `DataTemplate` instance) — the page is created on demand, when the entry is first navigated to. Passing a page instance directly is not a valid overload; use the lambda form. (Eagerly-created content is possible via `.Content(new SomePage())`, but the lazy template is the recommended default.)

## Customizing the Flyout Appearance

Set `ItemTemplate` on the shell and define a template view. Inside the template, bindings resolve against the shell item (`Title`, `FlyoutIcon`):

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

Other flyout-level customizations:

```csharp
new Shell()
    .FlyoutBehavior(FlyoutBehavior.Flyout)          // or Locked / Disabled
    .FlyoutBackgroundColor(Colors.WhiteSmoke)
    .FlyoutHeader(
        new Grid().HeightRequest(120).Children(
            new Image().Source("banner.png").Aspect(Aspect.AspectFill),
            new Label().Text("My App").TextColor(Colors.White).AlignBottomLeft().Margin(16)
        )
    )
    .FlyoutFooter(new Label().Text("v1.0.0").TextCenter().Padding(8))
    .MenuItemTemplate(() => new MenuItemTemplateView())
```

## Icons, Routes and Registration

```csharp
new ShellContent()
    .Title("Orders")
    .Icon("orders.png")
    .Route("orders")
    .ContentTemplate(() => new OrdersPage())
```

Register additional (non-shell-visible) routes as usual:

```csharp
Routing.RegisterRoute("orders/detail", typeof(OrderDetailPage));
```

Then navigate:

```csharp
await Shell.Current.GoToAsync("orders/detail?id=42");
await Shell.Current.GoToAsync("..");                    // back
```

## Per-Page Shell Attached Properties

Every `Shell.*` attached property has a fluent method usable on pages (full table in [Attached Properties](attached-properties.md)):

```csharp
public partial class CheckoutPage : ContentPage, IFmgLibHotReload
{
    public CheckoutPage() => this.InitializeHotReload();

    public void Build() =>
        this
        .ShellTabBarIsVisible(false)                 // hide tab bar here
        .ShellNavBarHasShadow(false)
        .ShellTitleView(                             // custom title view
            new HorizontalStackLayout().Spacing(6).Children(
                new Image().Source("cart.png").SizeRequest(22, 22),
                new Label().Text("Checkout").FontSize(18).CenterVertical()
            )
        )
        .ShellBackButtonBehavior(
            new BackButtonBehavior()
                .TextOverride("Back")
                .Command(new Command(async () => await ConfirmLeaveAsync()))
        )
        .Content(/* ... */);
}
```

Shell chrome coloring (typically set once on the Shell itself, or per page):

```csharp
new Shell()
    .ShellBackgroundColor(AppColors.Primary)   // nav bar background
    .ShellForegroundColor(Colors.White)        // nav bar icons/text
    .ShellTitleColor(Colors.White)
    .ShellTabBarBackgroundColor(Colors.White)
    .ShellTabBarTitleColor(AppColors.Primary)
    .ShellTabBarUnselectedColor(Colors.Gray)
```

## Tabs-Only App (no flyout)

```csharp
new Shell()
.FlyoutBehavior(FlyoutBehavior.Disabled)
.Items(
    new TabBar()
    .Items(
        new ShellContent().Title("Home").Icon("home.png").ContentTemplate(() => new HomePage()),
        new ShellContent().Title("Search").Icon("search.png").ContentTemplate(() => new SearchPage()),
        new ShellContent().Title("Profile").Icon("user.png").ContentTemplate(() => new ProfilePage())
    )
)
```

## Search Handler

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

// on a page:
this.ShellSearchHandler(
    new ProductSearchHandler()
        .Placeholder("Search products…")
        .ShowsResults(true)
);
```

## Shell Menu Items

Plain action entries in the flyout:

```csharp
new Shell()
.Items(
    // ... flyout items ...
)
.InvokeOnElement(shell => shell.Items.Add(
    new MenuItem()
        .Text("Logout")
        .IconImageSource("logout.png")
        .OnClicked(async _ => await AuthService.LogoutAsync())
))
```

## Related Topics

- [Attached Properties](attached-properties.md) — all `Shell.*` methods
- [Hot Reload](hot-reload.md) — page structure used inside shell content
- [Menus](menus.md) — window menu bars and context menus
