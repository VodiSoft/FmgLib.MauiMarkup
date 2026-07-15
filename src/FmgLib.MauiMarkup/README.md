# FmgLib.MauiMarkup

**FmgLib.MauiMarkup** is a fluent C# markup library for .NET MAUI: build your entire UI in pure C# — no XAML. Every bindable property of every MAUI control gets a chainable extension method with the same name, and every event gets an `On<EventName>` method, with full IntelliSense, refactoring and compile-time safety.

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

**New project** (recommended):

```bash
dotnet new install FmgLib.MauiMarkup.Template
dotnet new fmglib-mauimarkup-app -o MyApp
```

**Existing MAUI project:**

```bash
dotnet add package FmgLib.MauiMarkup
```

No registration call needed. XAML and FmgLib pages coexist, so you can migrate incrementally.

## Key Features

- **Every control, every property, every event** — fluent methods for the whole MAUI surface, always in sync with the referenced MAUI version.
- **Third-party controls too** — `[MauiMarkup(typeof(...))]` or `<MauiMarkupSourceGenerator>true</MauiMarkupSourceGenerator>` generates the same fluent API for Syncfusion, UraniumUI, SkiaSharp, ZXing, DevExpress, InputKit…
- **Hot reload built in** — implement `IFmgLibHotReload`, call `InitializeHotReload()`, and your `Build()` re-runs on every code edit (leak-free, crash-isolated, works with `dotnet watch` and IDE hot reload).
- **First-class bindings** — string paths or compiled `Getter` expressions, inline converters, multi-binding.
- **Theming** — `OnLight/OnDark`, `OnPlatform`, `OnIdiom`, `DynamicResource` on any property; strongly-typed `Style<T>` with triggers and visual states.
- **Localization** — JSON or RESX based, with instant runtime language switching.

## 📚 Documentation

Complete documentation with detailed explanations and examples for every feature:

**https://github.com/FmgLib/FmgLib.MauiMarkup/tree/master/docs**

Highlights: [Getting Started](https://github.com/FmgLib/FmgLib.MauiMarkup/blob/master/docs/getting-started.md) · [From XAML to C#](https://github.com/FmgLib/FmgLib.MauiMarkup/blob/master/docs/xaml-to-csharp.md) · [Fluent Properties](https://github.com/FmgLib/FmgLib.MauiMarkup/blob/master/docs/fluent-properties.md) · [Property Bindings](https://github.com/FmgLib/FmgLib.MauiMarkup/blob/master/docs/data-binding.md) · [Styling](https://github.com/FmgLib/FmgLib.MauiMarkup/blob/master/docs/styling.md) · [Hot Reload](https://github.com/FmgLib/FmgLib.MauiMarkup/blob/master/docs/hot-reload.md) · [Third-Party Controls](https://github.com/FmgLib/FmgLib.MauiMarkup/blob/master/docs/third-party-controls.md) · [Complete Examples](https://github.com/FmgLib/FmgLib.MauiMarkup/blob/master/docs/complete-examples.md)

🇹🇷 Türkçe dokümantasyon: **https://github.com/FmgLib/FmgLib.MauiMarkup/tree/master/docs/tr**

## Samples

Complete sample applications live in the repository's [`sample/`](https://github.com/FmgLib/FmgLib.MauiMarkup/tree/master/sample) folder.
