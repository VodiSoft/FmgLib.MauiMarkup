# Menus

FmgLib.MauiMarkup covers both MAUI menu systems fluently:

- **Context menus** (`MenuFlyout`) — right-click / long-press menus on individual controls (desktop-focused).
- **Menu bars** (`MenuBarItem`) — the top application menu on Windows and macOS.

## Context Menus (`FlyoutBase.ContextFlyout`)

Attach a `MenuFlyout` to any control with `ContextFlyout(...)`. Menu items are added via collection-initializer syntax and configured fluently:

```csharp
new Grid()
.Assign(out var grid)
.Children(
    new Image()
        .Source("dotnet_bot.png")
        .ContextFlyout(new MenuFlyout()
        {
            new MenuFlyoutItem()
                .Text("Copy")
                .OnClicked(e => Console.WriteLine("Copy")),

            new MenuFlyoutItem()
                .Text("Paste")
                .OnClicked(e => Console.WriteLine("Paste")),

            new MenuFlyoutSubItem()
            {
                new MenuFlyoutItem()
                    .Text("Blue")
                    .OnClicked(e => grid.BackgroundColor = Colors.Blue),
                new MenuFlyoutItem()
                    .Text("Red")
                    .OnClicked(e => grid.BackgroundColor = Colors.Red),
                new MenuFlyoutItem()
                    .Text("Black")
                    .OnClicked(e => grid.BackgroundColor = Colors.Black)
            }
            .Text("Background color")
        })
)
```

Notes:

- `MenuFlyoutSubItem` nests a submenu; give it `.Text(...)` after the initializer (fluent methods chain off the initializer block).
- `MenuFlyoutSeparatorItem` adds divider lines.
- Items support `.IconImageSource(...)`, `.IsEnabled(...)`, and `.Command(...)` for MVVM:

```csharp
new MenuFlyoutItem()
    .Text("Delete")
    .IconImageSource("trash.png")
    .Command(BindingContext.DeleteCommand)
    .CommandParameter(e => e.Path("."))
```

- Context flyouts are effective on **Windows and Mac Catalyst**; on mobile prefer `SwipeView` actions or long-press gestures.

## Menu Bars (`Page.MenuBarItems`)

A menu bar is defined on a page with `MenuBarItems(...)`. Each `MenuBarItem` is a top-level menu whose children are flyout items:

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
                    .Text("Exit")
                    .OnClicked(e => Application.Current.Quit()),
            }
            .Text("My Menu"),

            new MenuBarItem()
            {
                new MenuFlyoutItem()
                    .Text("Copy")
                    .OnClicked(e => Console.WriteLine("Copy"))
                    .KeyboardAccelerators(
                        new KeyboardAccelerator()
                            .Key("C")
                            .Modifiers(KeyboardAcceleratorModifiers.Ctrl)
                    ),

                new MenuFlyoutItem()
                    .Text("Paste")
                    .OnClicked(e => Console.WriteLine("Paste"))
                    .KeyboardAccelerators(
                        new KeyboardAccelerator()
                            .Key("V")
                            .Modifiers(KeyboardAcceleratorModifiers.Ctrl)
                    ),
            }
            .Text("Edit"),

            new MenuBarItem()
            {
                new MenuFlyoutItem()
                    .Text("Blue")
                    .OnClicked(e => this.BackgroundColor = Colors.Blue),
                new MenuFlyoutItem()
                    .Text("Dark")
                    .OnClicked(e => this.BackgroundColor = Colors.Black),
            }
            .Text("Theme")
        });

        this.Content(/* page content */);
    }
}
```

### Keyboard accelerators

`KeyboardAccelerators(...)` binds shortcuts to menu items. Combine modifiers with `|`:

```csharp
new KeyboardAccelerator()
    .Key("S")
    .Modifiers(KeyboardAcceleratorModifiers.Ctrl | KeyboardAcceleratorModifiers.Shift)  // Ctrl+Shift+S
```

### Platform behavior

- **Windows** — menu bar renders at the top of the window.
- **Mac Catalyst** — items merge into the system menu bar.
- **iOS/Android** — menu bars are not displayed; guard with `DeviceInfo.Idiom == DeviceIdiom.Desktop` if you build cross-platform pages.

### Submenus in menu bars

`MenuFlyoutSubItem` works inside `MenuBarItem` the same way:

```csharp
new MenuBarItem()
{
    new MenuFlyoutSubItem()
    {
        new MenuFlyoutItem().Text("Turkish").OnClicked(_ => SetLang("tr-TR")),
        new MenuFlyoutItem().Text("English").OnClicked(_ => SetLang("en-US")),
    }
    .Text("Language"),
}
.Text("Settings")
```

## Toolbar Items

Related but distinct: page-level toolbar buttons use `ToolbarItems(...)` with the fluent `ToolbarItem` API:

```csharp
this.ToolbarItems(
    new ToolbarItem()
        .Text("Refresh")
        .IconImageSource("refresh.png")
        .OnClicked(async _ => await ViewModel.RefreshAsync())
);
```

## Related Topics

- [Event Handlers](event-handlers.md)
- [Shell Applications](shell-navigation.md) — shell-level `MenuItem`s and flyout menus
