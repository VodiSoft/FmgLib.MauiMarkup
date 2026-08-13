# FmgLib.MauiMarkup vs. CommunityToolkit.Maui.Markup

Both libraries answer the same question — *how do I build a .NET MAUI UI in C# instead of XAML?* — and both do
it with fluent extension methods. This page is an honest, feature-by-feature comparison of the two: what
FmgLib.MauiMarkup does that the Community Toolkit does not, and where the Community Toolkit is the better
choice.

> **Data verified on 13 August 2026.** Package versions and download counts move; the feature comparison comes
> from each project's own public documentation on that date. Check the linked sources before making a decision
> you cannot easily reverse.

## At a glance

| | [FmgLib.MauiMarkup](https://www.nuget.org/packages/FmgLib.MauiMarkup/) | [CommunityToolkit.Maui.Markup](https://www.nuget.org/packages/CommunityToolkit.Maui.Markup/) |
|---|---|---|
| **Latest version** | 10.3.0 | 8.0.0 |
| **Released** | August 2026 | July 2026 |
| **Total downloads** | ~19 K | ~1.0 M |
| **Backing** | Independent (VodiSoft) | .NET Foundation / Community Toolkit |
| **Target frameworks** | net9.0 **and** net10.0 | net10.0 |
| **License** | MIT | MIT |
| **How the API is produced** | Roslyn source generator | Hand-written, curated extensions |

---

## Feature comparison

Legend: **●** built in · **◐** partial / manual work needed · **○** not provided

### Coverage — how much of MAUI you can reach fluently

| | FmgLib | CTK.Markup |
|---|:--:|:--:|
| Fluent method for **every** bindable property of **every** control | ● | ○ |
| Fluent `On<Event>` method for **every** event | ● | ○ |
| **Third-party controls, zero configuration** | ● | ○ |
| Third-party controls, per-type opt-in | ● | ○ |
| Attached properties (`Grid.Row`, `Shell.*`, `Semantic*`, …) | ● | ● |
| Grid row/column definition builders | ● | ● |
| Layout-option and text-alignment helpers | ● | ● |

**This is the biggest single difference.** CommunityToolkit.Maui.Markup ships a *curated* set of extensions —
`Label`, `Image`, `Grid`, `VisualElement`, `ItemsView`, `Placeholder` and about a dozen more families. It is
excellent for the properties it covers, but the moment you need a property nobody wrote a helper for, you drop
back to object-initializer syntax mid-chain:

```csharp
// CommunityToolkit — mixing the two styles when a helper does not exist
new Entry
{
    Keyboard = Keyboard.Numeric,          // no fluent helper → object initializer
    ReturnType = ReturnType.Done,
}
.Placeholder("Enter number")              // helper exists → fluent
.FontSize(15)
.Height(44);
```

```csharp
// FmgLib — every bindable property is generated, so the chain never breaks
new Entry()
    .Keyboard(Keyboard.Numeric)
    .ReturnType(ReturnType.Done)
    .Placeholder("Enter number")
    .FontSize(15)
    .HeightRequest(44);
```

### Third-party controls

| | FmgLib | CTK.Markup |
|---|:--:|:--:|
| Generates fluent methods for referenced control libraries | ● | ○ |
| Requires no attributes or per-type declarations | ● | — |
| Opt-in mode for large solutions (`[MauiMarkup(typeof(T))]`) | ● | — |
| Attached properties of third-party controls | ● | ○ |

This is where the gap is widest. With the Community Toolkit, a Syncfusion or DevExpress control only receives
the generic `VisualElement`/`View` helpers — every property specific to that control stays a plain assignment.

```csharp
// FmgLib — one MSBuild property, and every referenced control library becomes fluent.
<MauiMarkupSourceGenerator>true</MauiMarkupSourceGenerator>

new SfButton().Text("Buy").CornerRadius(8)          // Syncfusion
new SKLottieView().Source(…).RepeatCount(-1)        // SkiaSharp.Extended
new CameraView().IsTorchOn(true).OnFrameReady(…)    // ZXing
```

### Data binding

| | FmgLib | CTK.Markup |
|---|:--:|:--:|
| String-path bindings | ● | ● |
| Compiled / typed bindings (no reflection) | ● | ● |
| Inline `Convert` / `ConvertBack` lambdas | ● | ● |
| MultiBinding | ● | ● |
| **Typed** MultiBinding (2–9 parameters, no `object[]`) | ● | ◐¹ |
| Compiled and string sub-bindings mixed in one MultiBinding | ● | ○ |
| `FallbackValue` / `TargetNullValue` | ● | ● |
| Bindings written inside the property call itself | ● | ◐² |

¹ Available through `FuncMultiConverter`, which receives positional/`object[]` values rather than typed parameters.
² The Community Toolkit binds through a separate `.Bind(Property, …)` call naming the `BindableProperty`.

```csharp
// CommunityToolkit — the binding names the property again
new Entry().Bind(Entry.TextProperty,
    static (ViewModel vm) => vm.RegistrationCode,
    static (ViewModel vm, string text) => vm.RegistrationCode = text)

// FmgLib — the binding lives in the property you are already setting
new Entry().Text(e => e
    .Getter(static (ViewModel vm) => vm.RegistrationCode)
    .Setter(static (ViewModel vm, string text) => vm.RegistrationCode = text)
    .BindingMode(BindingMode.TwoWay))
```

And typed multi-bindings, where the delegate parameters are the sub-binding types in declaration order:

```csharp
new Button().IsEnabled(e => e
    .Path("AcceptedTerms")
    .Path("ConfirmedEmail")
    .MultiConvert((bool terms, bool email) => terms && email))

// compiled and string sub-bindings mix freely:
new Label().Text(e => e
    .Getter(static (OrderVm vm) => vm.Total)
    .Path("ItemCount")
    .MultiConvert((decimal total, int count) => $"{count} items — {total:C}"))
```

### Appearance

| | FmgLib | CTK.Markup |
|---|:--:|:--:|
| `Style<T>` with the same fluent methods as controls | ● | ● |
| `VisualState<T>` helper | ● | ○ |
| Named state constants (`VisualStates.Button.Pressed`) | ● | ○ |
| Triggers: property / data / multi / event | ● | ○ |
| Generated `Animate<Property>To` for every animatable property | ● | ○ |
| Animations that run on visual-state entry | ● | ○ |
| Light/dark values inline (`OnLight` / `OnDark`) | ● | ◐³ |
| Idiom values inline (`OnPhone` / `OnTablet` / `OnDesktop`) | ● | ○ |
| Platform values inline (`OniOS` / `OnAndroid` / …) | ● | ○ |
| `DynamicResource` inline in the same lambda | ● | ● |

³ Available via `AppThemeBinding`/dynamic-resource helpers rather than as a value inside the property call.

The FmgLib difference is that **all of these live in one lambda on the property you are already setting**, so a
value can be theme-aware, idiom-aware and bound without leaving the chain:

```csharp
new Label()
    .TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.White))     // real AppThemeBinding
    .FontSize(e => e.OnPhone(13.0).OnTablet(15.0).OnDesktop(17.0))
    .Margin(e => e.OniOS(new Thickness(0, 20, 0, 0)).Default(new Thickness(0)))
    .Text(e => e.Path("Title"))
```

`OnLight`/`OnDark` produces a real `AppThemeBinding`, so switching the theme repaints the running UI — no page
rebuild, no clearing and refilling resource dictionaries.

### Localization

| | FmgLib | CTK.Markup |
|---|:--:|:--:|
| Built-in localization | ● | ○ |
| JSON language files | ● | ○ |
| RESX / `ResourceManager` | ● | ○ |
| Live language switching (no page reload) | ● | ○ |
| Culture fallback chain (`tr-TR` → `tr` → default) | ● | ○ |
| Translations with values (`TranslateFormat`) | ● | ○ |
| Right-to-left binding (`FlowDirection` from culture) | ● | ○ |
| Missing-key policy (key / empty / marker / throw) | ● | ○ |

The Community Toolkit ships no localization at all — you add a separate package and wire the
`INotifyPropertyChanged` re-reads yourself.

```csharp
builder.UseMauiMarkupLocalization(o => o
    .UseFiles("Localization.json")
    .UseDefaultCulture("en-US")
    .UseFallbackCulture("en-US"));

new Label().Text(e => e.Translate("Greeting"))
new Label().Text(e => e.TranslateFormat("WelcomeUser", nameof(vm.UserName)))
this.FlowDirection(e => e.FromCulture())        // mirrors the page for Arabic/Hebrew

Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));   // everything re-reads itself
```

### Developer experience

| | FmgLib | CTK.Markup |
|---|:--:|:--:|
| Works with standard .NET Hot Reload | ● | ● |
| **Re-runs your UI method on hot reload** (`Build()`) | ● | ○ |
| Weak registration — hot reload never leaks pages | ● | — |
| Ready-made page bases (`FmgLibContentPage<TVm>`) | ● | ○ |
| `dotnet new` project template | ● | ○ |
| Full gallery sample app | ● | ◐ |
| Documentation | 36 pages | Microsoft Learn |

With the Community Toolkit, .NET Hot Reload applies to your code but nothing re-invokes your UI construction, so
you usually restart the page to see a markup edit. FmgLib's `IFmgLibHotReload` + `Build()` pattern re-runs
construction on every applied edit — and registers pages **weakly**, so hot reload never keeps a popped page
alive (leak detectors stay quiet).

---

## What FmgLib.MauiMarkup adds, in one list

Everything below exists in FmgLib and in **none** of CommunityToolkit.Maui.Markup:

1. **Automatic fluent generation for every referenced third-party control** — one MSBuild flag, no attributes.
2. **Every bindable property of every control**, in all four overload shapes — the chain never breaks.
3. **`On<Event>` for every event**, in two shapes (typed sender, or full event args).
4. **Localization** — JSON and RESX, live switching, fallback chain, formatted translations, RTL, missing-key policy.
5. **Hot reload that re-runs your UI method**, with weak registration and ready-made page bases.
6. **Idiom and platform value builders** inline on the property.
7. **Typed `MultiConvert`** with 2–9 parameters, and compiled/string sub-bindings mixed freely.
8. **Generated `Animate<Property>To`** for every animatable property, awaitable and composable.
9. **`VisualState<T>` with named state constants** and animations on state entry.
10. **Fluent triggers** — property, data, multi and event.
11. **.NET 9 and .NET 10** from a single package version.
12. **Documentation in English and Turkish**, plus a 24-page gallery sample and a `dotnet new` template.

## Sources

- [CommunityToolkit.Maui.Markup on Microsoft Learn](https://learn.microsoft.com/dotnet/communitytoolkit/maui/markup/markup) · [GitHub](https://github.com/CommunityToolkit/Maui.Markup) · [NuGet](https://www.nuget.org/packages/CommunityToolkit.Maui.Markup)
- [FmgLib.MauiMarkup on NuGet](https://www.nuget.org/packages/FmgLib.MauiMarkup)

## Related Topics

- [Getting Started](getting-started.md)
- [Third-Party Controls](third-party-controls.md) — the zero-configuration generator
- [Localization (JSON)](localization-json.md) · [Localization (RESX)](localization-resx.md)
- [From XAML to C#](xaml-to-csharp.md)
