# FmgLib.MauiMarkup Documentation

**FmgLib.MauiMarkup** is a fluent C# markup library for .NET MAUI. It lets you build your entire user interface in pure C# — no XAML required — using readable, chainable extension methods that are automatically generated for every bindable property and event of every MAUI control.

```csharp
new Label()
    .Text("Hello, FmgLib!")
    .FontSize(30)
    .TextColor(Colors.Green)
    .Center()
```

This documentation is a complete, self-contained guide to the library. Every topic from the project README is covered here in much greater depth, together with usage patterns discovered directly from the library source code.

## Table of Contents

### 1. Fundamentals

| Document | What you will learn |
|---|---|
| [Getting Started](getting-started.md) | Installation, project templates, first app, project structure |
| [From XAML to C#](xaml-to-csharp.md) | How XAML concepts map to FmgLib.MauiMarkup, side-by-side conversions |
| [Fluent Properties](fluent-properties.md) | The four overload patterns, theme/platform/idiom-aware values, dynamic resources |
| [Object References (Assign)](assign-and-references.md) | `Assign`, `InvokeOnElement`, `RegisterName`, referencing controls without fields |

### 2. Data Binding

| Document | What you will learn |
|---|---|
| [Property Bindings](data-binding.md) | `Path`, `Source`, `BindingMode`, `StringFormat`, fallback values, the low-level `Bind()` API |
| [Binding Converters](binding-converters.md) | Inline `Convert`/`ConvertBack`, `IValueConverter`, func-based converters |
| [MultiBinding](multi-binding.md) | Combining several bindings into one property, `IMultiValueConverter` |
| [Compiled Bindings](compiled-bindings.md) | `Getter`/`Setter` expression bindings, `Binding.Create`, performance |

### 3. Layout

| Document | What you will learn |
|---|---|
| [Layout Options](layout-options.md) | Alignment and fill helpers (`Center`, `AlignTopLeft`, `FillHorizontal`, …) |
| [Grid](grid.md) | Row/column definition builders (`Star`, `Auto`, `Absolute`), positioning children |
| [Text Alignment](text-alignment.md) | `ITextAlignment` helpers (`TextCenter`, `TextTopLeft`, …) |
| [Attached Properties](attached-properties.md) | Full mapping table and examples for Grid, Shell, Semantic, Automation, … |

### 4. Interaction

| Document | What you will learn |
|---|---|
| [Event Handlers](event-handlers.md) | `On<EventName>` methods, method groups vs. inline lambdas |
| [Gesture Recognizers](gesture-recognizers.md) | Tap, pan, pointer, swipe, pinch, drag & drop |
| [Behaviors](behaviors.md) | Attaching reusable behaviors, writing custom behaviors |
| [Triggers](triggers.md) | Property, data, event, multi and state triggers |
| [Menus](menus.md) | Context menus (`MenuFlyout`), menu bars, keyboard accelerators |
| [SwipeView](swipeview.md) | Swipe-to-action rows, `SwipeItem`s, custom swipe content |

### 5. Appearance

| Document | What you will learn |
|---|---|
| [Styling](styling.md) | `Style<T>`, resource dictionaries, `BasedOn`, derived types, app-wide themes |
| [Visual States](visual-states.md) | `VisualState<T>`, built-in state names, state-driven animations |
| [Gradients & Brushes](gradients-and-brushes.md) | Linear/radial gradients, solid brushes, shadows |
| [Shapes & Geometries](shapes.md) | Lines, rectangles, ellipses, polygons, `Path` geometries, clipping |
| [Formatted Text](formatted-text.md) | `FormattedString`/`Span`, mixed styling, tappable inline links |
| [Animations](animations.md) | Generated `Animate…To` helpers, MAUI animation interop |

### 6. Application Architecture

| Document | What you will learn |
|---|---|
| [Shell Applications](shell-navigation.md) | Building a Shell in C#, flyouts, tabs, templates, navigation |
| [Application & Windows](application-and-windows.md) | `Application` setup, window lifecycle, `TitleBar`, NavigationPage/TabbedPage/FlyoutPage |
| [Hot Reload](hot-reload.md) | `IFmgLibHotReload`, `FmgLibContentPage`, MVVM page bases |
| [Collections & Templates](collections-and-templates.md) | `CollectionView`, `ItemTemplate` overloads, `BindableLayout`, `EmptyView` |
| [Localization (JSON)](localization-json.md) | JSON-file based localization, runtime language switching |
| [Localization (RESX)](localization-resx.md) | RESX-based localization with `TranslatorResx` |

### 7. Extensibility & Reference

| Document | What you will learn |
|---|---|
| [Third-Party Controls](third-party-controls.md) | `[MauiMarkup]`, `[MauiMarkupAttachedProp]`, automatic generator mode |
| [Custom Extension Methods](custom-extension-methods.md) | Writing your own fluent methods that work everywhere |
| [Utilities](utilities.md) | `ToColor`, collection helpers, `AddRangeMarkup`, style interop |
| [Complete Examples](complete-examples.md) | Full pages: login screen, product list, settings page, MVVM patterns |
| [Tips & Troubleshooting](tips-and-troubleshooting.md) | Common pitfalls, naming rules, FAQ |

> 🇹🇷 Bu dokümantasyonun Türkçe sürümü [tr/](tr/README.md) klasöründedir.

## Suggested Learning Path

1. **New to the library?** Read [Getting Started](getting-started.md), then [From XAML to C#](xaml-to-csharp.md) and [Fluent Properties](fluent-properties.md). These three explain 80% of everyday usage.
2. **Building real pages?** Continue with [Layout Options](layout-options.md), [Grid](grid.md), [Property Bindings](data-binding.md) and [Hot Reload](hot-reload.md).
3. **Polishing an app?** [Styling](styling.md), [Visual States](visual-states.md), [Triggers](triggers.md) and [Animations](animations.md).
4. **Shipping to multiple markets?** [Localization (JSON)](localization-json.md) or [Localization (RESX)](localization-resx.md).
5. **Using SkiaSharp, ZXing, UraniumUI, InputKit…?** [Third-Party Controls](third-party-controls.md).

## Package Overview

| Package | Purpose |
|---|---|
| [`FmgLib.MauiMarkup`](https://www.nuget.org/packages/FmgLib.MauiMarkup/) | The markup library itself + the Roslyn source generator |
| [`FmgLib.MauiMarkup.Template`](https://www.nuget.org/packages/FmgLib.MauiMarkup.Template/) | `dotnet new` project template (`fmglib-mauimarkup-app`) |

## Core Idea in 30 Seconds

Every bindable property of every MAUI control gets a **fluent extension method with the same name as the property**. Every event gets an **`On<EventName>` method**. All methods return the control itself, so calls chain naturally and the nesting of your C# code mirrors the visual tree — just like XAML, but with full IntelliSense, refactoring, compile-time safety, and no context switching between two languages.

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
