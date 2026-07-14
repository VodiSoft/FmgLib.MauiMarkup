# Application, Windows & Other Root Pages

Beyond pages and views, FmgLib.MauiMarkup covers the application object, windows, and the non-Shell root page types (`NavigationPage`, `TabbedPage`, `FlyoutPage`).

## `Application` Extensions

Used in your `App` class:

```csharp
public partial class App : Application
{
    public App()
    {
        this
        .Resources(AppStyles.Default)                       // app-wide ResourceDictionary
        .UserAppTheme(AppTheme.Unspecified)                 // force Light/Dark or follow OS
        .MainPage(new AppShell())                           // root page
        .OnRequestedThemeChanged((s, e) =>                  // OS theme change event
            Console.WriteLine($"Theme: {e.RequestedTheme}"));
    }
}
```

| Method | Purpose |
|---|---|
| `.MainPage(Page)` | Sets the root page |
| `.Resources(ResourceDictionary)` | App-level resources ([Styling](styling.md)) |
| `.UserAppTheme(AppTheme)` | Override the OS theme (`Light`/`Dark`/`Unspecified`) |
| `.AccentColor(Color)` | App accent color |
| `.OnRequestedThemeChanged(...)` | React to OS light/dark switches |
| `.OnModalPushed/Popped/Pushing/Popping(...)` | Modal navigation events |
| `.OnPageAppearing/OnPageDisappearing(...)` | Global page lifecycle events |

> On newer MAUI versions you may prefer overriding `CreateWindow` and returning `new Window(new AppShell())` — both approaches work; `MainPage(...)` remains the fluent shortcut.

## `Window` Extensions

Windows are fluent-enabled — sizing, position, title and lifecycle events:

```csharp
protected override Window CreateWindow(IActivationState? state) =>
    new Window(new AppShell())
        .Title("My App")
        .Width(1100).Height(720)                  // desktop platforms
        .MinimumWidth(800).MinimumHeight(600)
        .X(100).Y(100)
        .OnCreated(w => Console.WriteLine("window created"))
        .OnActivated(w => { /* got focus */ })
        .OnDeactivated(w => { /* lost focus */ })
        .OnResumed(w => { /* returned to foreground */ })
        .OnStopped(w => { /* backgrounded */ })
        .OnDestroying(w => SaveState());
```

These window lifecycle events are the cleanest place for app-suspend/resume logic on MAUI.

### TitleBar (Windows, .NET 9+)

The `TitleBar` control customizes the window chrome and is fully fluent:

```csharp
new Window(new AppShell())
    .TitleBar(
        new TitleBar()
            .Icon("appicon.png")
            .Title("My App")
            .Subtitle("Preview")
            .BackgroundColor("#20242B".ToColor())
            .ForegroundColor(Colors.White)
            .Content(new SearchBar().Placeholder("Search…").MaximumWidthRequest(400))
            .TrailingContent(new ImageButton().Source("avatar.png").SizeRequest(28, 28))
    )
```

`PassthroughElements(...)` marks interactive children so clicks reach them instead of dragging the window.

## Non-Shell Root Pages

[Shell](shell-navigation.md) is the recommended default, but the classic containers are fully supported.

### NavigationPage

```csharp
this.MainPage(
    new NavigationPage(new HomePage())
        .BarBackgroundColor(AppColors.Primary)
        .BarTextColor(Colors.White)
);

// push/pop as usual:
await Navigation.PushAsync(new DetailPage());
await Navigation.PopAsync();
```

Per-page tweaks use the `NavigationPage.*` [attached-property methods](attached-properties.md#navigationpage-set-on-pages):

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
            new HomePage().Title("Home").IconImageSource("home.png"),
            new SettingsPage().Title("Settings").IconImageSource("gear.png")
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
                .Title("Menu")
                .Content(
                    new VerticalStackLayout().Children(
                        new Button().Text("Home").OnClicked(_ => Navigate(new HomePage())),
                        new Button().Text("Orders").OnClicked(_ => Navigate(new OrdersPage()))
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

## Modal Pages

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
                .Content(new Label().Text("Are you sure?"))
        )
);
```

Track modals globally via the `Application` events above (`OnModalPushed`, `OnModalPopped`).

## Related Topics

- [Shell Applications](shell-navigation.md)
- [Attached Properties](attached-properties.md)
- [Getting Started](getting-started.md)
