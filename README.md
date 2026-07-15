# FmgLib.MauiMarkup

| NuGet Package | Link | Repo Info |
|--------------|------|-------|
| `FmgLib.MauiMarkup` | [![NuGet](https://img.shields.io/nuget/v/FmgLib.MauiMarkup?includePreReleases=true)](https://www.nuget.org/packages/FmgLib.MauiMarkup/) ![NuGet Downloads](https://img.shields.io/nuget/dt/FmgLib.MauiMarkup.svg) | [![GitHub Stars](https://img.shields.io/github/stars/FmgLib/FmgLib.MauiMarkup?style=flat-square&color=blue)](https://github.com/FmgLib/FmgLib.MauiMarkup/stargazers) [![GitHub Forks](https://img.shields.io/github/forks/FmgLib/FmgLib.MauiMarkup?style=flat-square&color=green)](https://github.com/FmgLib/FmgLib.MauiMarkup/forks) [![GitHub last-commit](https://img.shields.io/github/last-commit/FmgLib/FmgLib.MauiMarkup?style=flat-square)](https://github.com/FmgLib/FmgLib.MauiMarkup/commits) |
| `FmgLib.MauiMarkup.Template` | [![NuGet](https://img.shields.io/nuget/v/FmgLib.MauiMarkup.Template?includePreReleases=true)](https://www.nuget.org/packages/FmgLib.MauiMarkup.Template/) ![NuGet Downloads](https://img.shields.io/nuget/dt/FmgLib.MauiMarkup.Template.svg) | - |

**FmgLib.MauiMarkup** is a fluent C# markup library for .NET MAUI: build your entire UI in pure C# — no XAML. Every bindable property of every MAUI control gets a chainable extension method with the same name, and every event gets an `On<EventName>` method, so your code reads like the visual tree while keeping full IntelliSense, refactoring and compile-time safety.

```csharp
public partial class MainPage : ContentPage, IFmgLibHotReload
{
    int count = 0;

    public MainPage() => this.InitializeHotReload();

    public void Build() =>
        this.Content(
            new VerticalStackLayout()
            .Spacing(25)
            .Padding(30)
            .Center()
            .Children(
                new Image()
                    .Source("dotnet_bot.png")
                    .HeightRequest(200)
                    .CenterHorizontal(),

                new Label()
                    .Text("Hello, World!")
                    .FontSize(32)
                    .CenterHorizontal(),

                new Button()
                    .Text("Click me")
                    .OnClicked(b => b.Text = $"Clicked {++count} times")
                    .CenterHorizontal()
            )
        );
}
```

## Quick Start

**New project** (recommended — ships hot-reload tasks, no XAML pages):

```bash
dotnet new install FmgLib.MauiMarkup.Template
dotnet new fmglib-mauimarkup-app -o MyApp
```

Optional parameters: `--netMajor 10|9` (default `10`), `--includeContent true|false` (default `false`, adds sample pages).

**Existing MAUI project:**

```bash
dotnet add package FmgLib.MauiMarkup
```

No registration call needed — the fluent methods and the source generator come with the package. XAML and FmgLib pages coexist, so you can migrate incrementally.

## 📚 Documentation

The complete documentation lives in **[docs/](docs/README.md)** — every feature with detailed explanations and examples. Highlights:

| Topic | Start here |
|---|---|
| Setup, first page, project structure | [Getting Started](docs/getting-started.md) |
| Translating any XAML to fluent C# | [From XAML to C#](docs/xaml-to-csharp.md) |
| Property methods, `OnLight/OnDark`, `OnPlatform`, `OnIdiom`, `DynamicResource` | [Fluent Properties](docs/fluent-properties.md) |
| Bindings, converters, multi-binding, compiled bindings | [Property Bindings](docs/data-binding.md) |
| Layout helpers, `Grid` builders, attached properties | [Layout Options](docs/layout-options.md) · [Grid](docs/grid.md) · [Attached Properties](docs/attached-properties.md) |
| Events, gestures, behaviors, triggers, menus, `SwipeView` | [Event Handlers](docs/event-handlers.md) · [Triggers](docs/triggers.md) |
| `Style<T>`, visual states, gradients, shapes, animations | [Styling](docs/styling.md) · [Visual States](docs/visual-states.md) |
| Shell apps, windows, collections & templates | [Shell Applications](docs/shell-navigation.md) |
| **Hot reload** (IDE support matrix, `dotnet watch` dev loop, troubleshooting) | [Hot Reload](docs/hot-reload.md) |
| JSON / RESX localization with live language switching | [Localization](docs/localization-json.md) |
| **Third-party controls** (`[MauiMarkup]`, auto-generation, Syncfusion/UraniumUI/…) | [Third-Party Controls](docs/third-party-controls.md) |
| Full example pages & common pitfalls | [Complete Examples](docs/complete-examples.md) · [Tips & Troubleshooting](docs/tips-and-troubleshooting.md) |

> 🇹🇷 Dokümantasyonun Türkçe sürümü: **[docs/tr/](docs/tr/README.md)**

## Key Features

- **Every control, every property, every event** — fluent methods generated for the whole MAUI surface, always in sync with the referenced MAUI version.
- **Third-party controls too** — annotate with `[MauiMarkup(typeof(...))]` or set `<MauiMarkupSourceGenerator>true</MauiMarkupSourceGenerator>` and the source generator produces the same fluent API for Syncfusion, UraniumUI, SkiaSharp, ZXing, DevExpress, InputKit…
- **Hot reload built in** — implement `IFmgLibHotReload`, call `InitializeHotReload()`, and your `Build()` re-runs on every code edit (leak-free, crash-isolated, works with `dotnet watch` and IDE hot reload).
- **First-class bindings** — string paths or compiled `Getter` expressions, inline converters, multi-binding, `FallbackValue`/`TargetNullValue`.
- **Theming** — `OnLight/OnDark`, `OnPlatform`, `OnIdiom`, `DynamicResource` on any property; strongly-typed `Style<T>` with triggers and visual states.
- **Localization** — JSON or RESX based, with instant runtime language switching.

## Samples

The [`sample/`](sample) folder contains complete applications: a 2048 game, an F1 TV browser, an order app, third-party integration demos and more.

## License

See [LICENSE](src/FmgLib.MauiMarkup/LICENSE).
