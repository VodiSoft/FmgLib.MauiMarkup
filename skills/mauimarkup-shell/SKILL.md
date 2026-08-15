---
name: mauimarkup-shell
description: Build the .NET MAUI app skeleton in FmgLib.MauiMarkup fluent C# — Shell with flyouts and tabs, ShellContent routes, flyout templates, per-page Shell attached properties, Application/Window setup, NavigationPage/TabbedPage/FlyoutPage and menus. Use when creating or editing AppShell, App.cs, MauiProgram.cs, navigation structure or window/title-bar setup in a C# markup MAUI app.
license: MIT
---

# App Shell, Navigation & Windows

Requires the `mauimarkup` core skill.

## Shell hierarchy

```
Shell
└── FlyoutItem            flyout entries
    ├── Tab               bottom tabs
    │   └── ShellContent  top tabs / pages
    └── ShellContent
TabBar                    tabs with no flyout
```

```csharp
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
                        new ShellContent().Title("Home").ContentTemplate(() => new HomePage()),
                        new ShellContent().Title("Example").ContentTemplate(() => new ExamplePage())
                    ),
                    new ShellContent().Title("Grid").ContentTemplate(() => new GridPage())
                )
            )
        );
    }
}
```

**`ContentTemplate(() => new SomePage())` takes a lambda** (`Func<object>`) or a `DataTemplate`.
Passing a page *instance* is not a valid overload — this is the single most common mistake here. The
lambda also makes the page lazy: it is created when first navigated to. `.Content(new SomePage())`
exists for eager creation but is rarely what you want.

On newer MAUI versions prefer the window form:

```csharp
protected override Window CreateWindow(IActivationState? activationState)
    => new Window(new AppShell());
```

## Tabs-only app

```csharp
new Shell()
.FlyoutBehavior(FlyoutBehavior.Disabled)
.Items(
    new TabBar().Items(
        new ShellContent().Title("Home").Icon("home.png").ContentTemplate(() => new HomePage()),
        new ShellContent().Title("Search").Icon("search.png").ContentTemplate(() => new SearchPage()),
        new ShellContent().Title("Profile").Icon("user.png").ContentTemplate(() => new ProfilePage())
    ))
```

## Flyout appearance

```csharp
new Shell()
    .FlyoutBehavior(FlyoutBehavior.Flyout)        // Flyout | Locked | Disabled
    .FlyoutBackgroundColor(Colors.WhiteSmoke)
    .FlyoutHeader(
        new Grid().HeightRequest(120).Children(
            new Image().Source("banner.png").Aspect(Aspect.AspectFill),
            new Label().Text("My App").TextColor(Colors.White).AlignBottomLeft().Margin(16)))
    .FlyoutFooter(new Label().Text("v1.0.0").TextCenter().Padding(8))
    .ItemTemplate(() => new ShellItemTemplate())
    .MenuItemTemplate(() => new MenuItemTemplateView())
```

Inside `ItemTemplate` the binding context is the shell item, so `Title` and `FlyoutIcon` are the paths:

```csharp
public class ShellItemTemplate : ContentView
{
    public ShellItemTemplate() =>
        this.Content(
            new Grid()
            .ColumnDefinitions(e => e.Star(0.2).Star(0.8))
            .Children(
                new Image().Source(e => e.Path("FlyoutIcon")).Margin(5).HeightRequest(45),
                new Label().Column(1).Text(e => e.Path("Title")).FontSize(20).CenterVertical()
            ));
}
```

## Chrome colors

```csharp
new Shell()
    .ShellBackgroundColor(AppColors.Primary)      // nav bar background
    .ShellForegroundColor(Colors.White)           // nav bar icons/text
    .ShellTitleColor(Colors.White)
    .ShellTabBarBackgroundColor(Colors.White)
    .ShellTabBarTitleColor(AppColors.Primary)
    .ShellTabBarUnselectedColor(Colors.Gray)
```

These are `Shell.*` attached properties, so the same methods work **per page** — a page can hide the
tab bar or recolor the nav bar just for itself.

## Per-page Shell properties

```csharp
public class CheckoutPage : FmgLibContentPage
{
    public override void Build() =>
        this
        .ShellTabBarIsVisible(false)
        .ShellNavBarHasShadow(false)
        .ShellPresentationMode(PresentationMode.ModalAnimated)
        .ShellTitleView(
            new HorizontalStackLayout().Spacing(6).Children(
                new Image().Source("cart.png").SizeRequest(22, 22),
                new Label().Text("Checkout").FontSize(18).CenterVertical()))
        .ShellBackButtonBehavior(
            new BackButtonBehavior()
                .TextOverride("Back")
                .Command(new Command(async () => await ConfirmLeaveAsync())))
        .Content(/* … */);
}
```

Full list of `Shell*` methods: `references/layout.md` in the core skill.

## Routes and navigation

```csharp
new ShellContent().Title("Orders").Icon("orders.png").Route("orders")
    .ContentTemplate(() => new OrdersPage())

Routing.RegisterRoute("orders/detail", typeof(OrderDetailPage));

await Shell.Current.GoToAsync("orders/detail?id=42");
await Shell.Current.GoToAsync("..");
```

Registered routes resolve through the DI container, so `OrderDetailPage(OrderDetailViewModel vm)` works.

## Search handler

```csharp
public class ProductSearchHandler : SearchHandler
{
    protected override void OnQueryChanged(string oldValue, string newValue) =>
        ItemsSource = string.IsNullOrWhiteSpace(newValue) ? null : ProductService.Search(newValue);
}

// on a page:
this.ShellSearchHandler(new ProductSearchHandler().Placeholder("Search products…").ShowsResults(true))
```

## Flyout menu items

`MenuItem`s are not `ShellContent`, so add them to `Shell.Items` directly:

```csharp
new Shell()
.Items(/* flyout items */)
.InvokeOnElement(shell => shell.Items.Add(
    new MenuItem()
        .Text("Logout")
        .IconImageSource("logout.png")
        .OnClicked(async _ => await AuthService.LogoutAsync())))
```

## Without Shell

`NavigationPage`, `TabbedPage` and `FlyoutPage` all build fluently:

```csharp
new NavigationPage(new HomePage())
    .BarBackgroundColor(AppColors.Primary)
    .BarTextColor(Colors.White)

new TabbedPage().Children(new HomePage(), new SearchPage(), new ProfilePage())

new FlyoutPage()
    .Flyout(new MenuPage())
    .Detail(new NavigationPage(new HomePage()))
```

Per-page `NavigationPage.*` attached properties follow the owner+property rule:
`.NavigationPageHasNavigationBar(false)`, `.NavigationPageTitleView(view)`,
`.NavigationPageBackButtonTitle("Back")`.

## Window and title bar

```csharp
protected override Window CreateWindow(IActivationState? state) =>
    new Window(new AppShell())
        .Title("My App")
        .Width(1280).Height(800)
        .TitleBar(new TitleBar()
            .Title("My App")
            .Icon("appicon.png")
            .BackgroundColor(AppColors.Primary));
```

Window lifecycle events chain like any other: `.OnCreated(...)`, `.OnActivated(...)`,
`.OnDeactivated(...)`, `.OnStopped(...)`, `.OnResumed(...)`, `.OnDestroying(...)`.

## Menu bars and context menus

```csharp
// desktop menu bar, on a page
this.MenuBarItems(
    new MenuBarItem().Text("File").Items(
        new MenuFlyoutItem().Text("Open").OnClicked(_ => Open())
            .KeyboardAccelerators(new KeyboardAccelerator().Modifiers(KeyboardAcceleratorModifiers.Ctrl).Key("O")),
        new MenuFlyoutSeparator(),
        new MenuFlyoutItem().Text("Exit").OnClicked(_ => Application.Current!.Quit())))

// right-click / long-press menu, on any view
new Image().Source("photo.png").ContextFlyout(
    new MenuFlyout().Items(
        new MenuFlyoutItem().Text("Copy").OnClicked(_ => Copy()),
        new MenuFlyoutSubItem() { /* nested items */ }.Text("Share")))
```

Note the ordering rule: on types using collection initializers (`MenuFlyoutSubItem`), fluent calls come
**after** the `{ … }` block.

## Pitfalls

- `ContentTemplate(page)` with an instance → compile error. Use `ContentTemplate(() => page)`.
- A shell whose `Build()`-equivalent edits don't hot-reload: give `AppShell` the same
  `IFmgLibHotReload` + `InitializeHotReload()` treatment as a page.
- `FlyoutDisplayOptions.AsMultipleItems` promotes each child to its own flyout entry — omit it and the
  `FlyoutItem` shows as one entry.
- Shell chrome set on `Shell` is inherited by pages; a per-page value overrides it. Set defaults once.
