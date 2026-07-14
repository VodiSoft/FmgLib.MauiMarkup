# Getting Started

This page walks you from zero to a running FmgLib.MauiMarkup application, either by creating a brand-new project from the official template or by adding the library to an existing MAUI app.

## Prerequisites

- .NET SDK 9 or 10 with the MAUI workload installed (`dotnet workload install maui`)
- Any IDE: Visual Studio, VS Code (with the .NET MAUI extension), or Rider

## Option A — Create a New Project from the Template (recommended)

FmgLib publishes a `dotnet new` template that produces a ready-to-run MAUI app already wired for FmgLib.MauiMarkup (no XAML pages, hot reload configured).

**1. Install the template:**

```bash
dotnet new install FmgLib.MauiMarkup.Template
```

**2. Create the project:**

```bash
dotnet new fmglib-mauimarkup-app -o MyNewApp
```

**Template parameters:**

| Parameter | Values | Default | Description |
|---|---|---|---|
| `--netMajor` | `10`, `9` | `10` | Target .NET / MAUI major version. Also selects matching `Microsoft.Maui.Controls` and `FmgLib.MauiMarkup` package versions. |
| `--includeContent` | `true`, `false` | `false` | `false` creates a minimal home page; `true` scaffolds a richer sample experience with example pages you can learn from and delete later. |

Examples:

```bash
# .NET 10, minimal single page
dotnet new fmglib-mauimarkup-app -o MyApp

# .NET 9 with sample content pages
dotnet new fmglib-mauimarkup-app -o MyApp --netMajor 9 --includeContent true
```

**3. Run it:**

```bash
cd MyApp
dotnet build -t:Run -f net10.0-android    # or -f net10.0-ios / net10.0-maccatalyst
```

On Windows you can also just press F5 in Visual Studio.

## Option B — Add to an Existing MAUI Project

Install the NuGet package:

```bash
dotnet add package FmgLib.MauiMarkup
```

That is all — there is no `builder.UseFmgLibMauiMarkup()` registration step. The package ships:

- **fluent extension methods** for every MAUI control (compiled into the library), and
- **a Roslyn source generator** that can additionally generate fluent methods for third-party controls (see [Third-Party Controls](third-party-controls.md)).

> Only the optional [JSON](localization-json.md) / [RESX](localization-resx.md) localization feature requires a call in `MauiProgram.cs` (`UseMauiMarkupLocalization…`).

### Converting an existing XAML page

You can migrate incrementally — XAML pages and FmgLib pages coexist happily in one app. A typical conversion:

**Before (`MainPage.xaml` + code-behind):**

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             x:Class="MyApp.MainPage">
    <VerticalStackLayout Spacing="25" Padding="30">
        <Label Text="Hello, World!" FontSize="32" HorizontalOptions="Center" />
        <Button x:Name="CounterBtn" Text="Click me" Clicked="OnCounterClicked" />
    </VerticalStackLayout>
</ContentPage>
```

**After (single `MainPage.cs`, delete the `.xaml` and `.xaml.cs`):**

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

Key points:

- `using FmgLib.MauiMarkup;` brings all fluent extensions into scope. Add it as a `global using` in one file (e.g. `GlobalUsings.cs`) so you never type it again:

  ```csharp
  global using FmgLib.MauiMarkup;
  ```

- Implementing `IFmgLibHotReload` + calling `this.InitializeHotReload()` gives you .NET Hot Reload for the UI (the `Build()` method re-runs when you edit code while debugging). This is optional but strongly recommended — details in [Hot Reload](hot-reload.md).
- If you prefer, you can skip hot reload entirely and build the UI directly in the constructor:

  ```csharp
  public MainPage()
  {
      this.Content(new Label().Text("Hi"));
  }
  ```

### Registering the page

Nothing changes compared to normal MAUI. For example in `App.cs`:

```csharp
public partial class App : Application
{
    public App()
    {
        this.MainPage(new AppShell());   // fluent setter for Application.MainPage
    }
}
```

or, on newer MAUI versions using windows:

```csharp
protected override Window CreateWindow(IActivationState? activationState)
    => new Window(new AppShell());
```

## Anatomy of a Markup Page

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
        .Title("Login")
        .BackgroundColor(Colors.White)
        .Content(                                                // 5
            new VerticalStackLayout()
            .Children(                                           // 6
                new Entry().Placeholder("E-mail").Assign(out var email),
                new Entry().Placeholder("Password").IsPassword(true),
                new Button().Text("Sign in")
            )
        );
    }
}
```

1. A page is just a plain C# class deriving from a MAUI page type.
2. `InitializeHotReload()` calls `Build()` once and re-invokes it on hot reload.
3. All UI construction lives in `Build()`.
4. `this` is a `ContentPage`, so every page-level fluent method (`Title`, `BackgroundColor`, `Content`, …) chains off it.
5. `Content(...)` sets `ContentPage.Content`.
6. Container methods such as `Children(...)` accept the child views as `params`, so the code indentation mirrors the visual tree exactly like XAML nesting.

## Where to Go Next

- [From XAML to C#](xaml-to-csharp.md) — the mental model for translating any XAML snippet.
- [Fluent Properties](fluent-properties.md) — everything the property methods can do (bindings, per-platform values, dynamic resources…).
- [Hot Reload](hot-reload.md) — the recommended page lifecycle, including the `FmgLibContentPage<TViewModel>` MVVM base class.
