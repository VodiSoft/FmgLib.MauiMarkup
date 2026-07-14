# Extensions for Third-Party Controls

FmgLib.MauiMarkup's fluent methods are produced by a **Roslyn source generator**, and the same generator can create fluent extensions for controls from *any* library — SkiaSharp, ZXing.Net.Maui, UraniumUI, InputKit, Syncfusion, DevExpress, your own control library… Three mechanisms exist:

1. `[MauiMarkup(typeof(...))]` — opt-in per control
2. `[MauiMarkupAttachedProp(...)]` — for attached properties
3. `MauiMarkupSourceGenerator` MSBuild property — fully automatic scanning

## 1. `[MauiMarkup]` — opt-in generation

Put the attribute on **any class** in your project (the class itself is irrelevant — it is just an anchor for the attribute). For each type passed, the generator emits fluent methods for its **bindable properties** and **events**.

```csharp
using FmgLib.MauiMarkup;

namespace GeneratedExam;

[MauiMarkup(typeof(ZXing.Net.Maui.Controls.BarcodeGeneratorView))]
public class MyBarcodeGeneratorView { }

[MauiMarkup(typeof(ZXing.Net.Maui.Controls.CameraView))]
public class MyCameraView { }

[MauiMarkup(typeof(SkiaSharp.Extended.UI.Controls.SKLottieView))]
public class MySkLottieView { }
```

Rules:

- The constructor accepts **1..N types** — batch several controls per attribute.
- **Multiple attributes** may sit on one class.
- Any class works as the anchor — `MauiProgram` is a popular single location:

```csharp
using FmgLib.MauiMarkup;
using SkiaSharp.Extended.UI.Controls;
using ZXing.Net.Maui.Controls;
using UraniumUI.Material.Controls;

namespace MauiApp1;

[MauiMarkup(typeof(CameraView))]
[MauiMarkup(typeof(SKLottieView), typeof(SKFileLottieImageSource), typeof(DataGrid))]
[MauiMarkup(typeof(SKConfettiView), typeof(BarcodeGeneratorView), typeof(InputField), typeof(EditorField), typeof(TextField))]
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        return builder.Build();
    }
}
```

### What gets generated

Given ZXing's `CameraView` (which declares `IsTorchOnProperty`, `CameraLocationProperty` and a `FrameReady` event), the generator produces a `CameraViewExtension` class in the `FmgLib.MauiMarkup` namespace containing, for **each bindable property**, the standard [four overloads](fluent-properties.md):

```csharp
new CameraView()
    .CameraLocation(CameraLocation.Front)                 // direct value
    .IsTorchOn(e => e.Path("TorchEnabled"))               // property builder (bindings, OnLight/OnDark, …)
    .OnFrameReady((s, e) => Analyze(e))                   // event handler
```

…and for each **event**, the two `On<Event>` shapes ([Event Handlers](event-handlers.md)). Styles work too, because the `SettersContext` overloads are generated:

```csharp
new Style<CameraView>(e => e.IsTorchOn(false))
```

### Usage after generation

Generated methods live in the `FmgLib.MauiMarkup` namespace, so your existing `using` covers them:

```csharp
new TextField()
    .Title("Password")
    .TitleColor(Colors.LightGray)
    .AccentColor(Colors.CadetBlue)
    .TextColor(Colors.White)
    .IsPassword(true),

new SKLottieView()
    .Source(new SKFileLottieImageSource().File("iconapp.json"))
    .RepeatCount(-1)
    .HeightRequest(250)
    .WidthRequest(250)
```

## 2. `[MauiMarkupAttachedProp]` — attached properties

For attached properties declared on third-party classes, use `MauiMarkupAttachedProp`. Constructor parameters, in order:

| # | Parameter | Meaning |
|---|---|---|
| 1 | `controlType` | The class **declaring** the attached property |
| 2 | `propertyName` | The `BindableProperty` field name (use `nameof`) |
| 3 | `returnType` | The property's value type |
| 4 | `declaringType` | The type the property will be **applied to** (the extension's target) |

Example — InputKit's `FormView.IsSubmitButton`, applied to `Button`:

```csharp
[MauiMarkupAttachedProp(typeof(InputKit.Shared.Controls.FormView),
                        nameof(InputKit.Shared.Controls.FormView.IsSubmitButtonProperty),
                        typeof(bool),
                        typeof(Button))]
[MauiMarkup(typeof(InputKit.Shared.Controls.FormView))]
public class MyFormView { }
```

Generated usage (naming: *owner class + property name*, as with [built-in attached properties](attached-properties.md)):

```csharp
new Button()
    .Text("Login")
    .FontAttributes(FontAttributes.Bold)
    .FormViewIsSubmitButton(true)
```

## 3. Automatic Mode — `MauiMarkupSourceGenerator`

To skip attributes entirely, enable assembly scanning in your **app project's `.csproj`**:

```xml
<PropertyGroup>
  <MauiMarkupSourceGenerator>true</MauiMarkupSourceGenerator>
</PropertyGroup>
```

When enabled, the generator scans referenced third-party assemblies and creates fluent extensions for every eligible public `BindableObject` control automatically. (The required `CompilerVisibleProperty` wiring is injected automatically by the NuGet package via `buildTransitive` props — nothing else to configure.)

**Trade-off:** automatic mode is convenient but generates code for everything it finds, which can increase compile time in large solutions. The attribute approach keeps generation scoped to what you use.

## Naming Collision Rule: `PropertyName + New`

If a control **redefines an inherited property with the `new` keyword**, the generated fluent method gets a `New` suffix to avoid ambiguity with the base-class extension.

Example: `SfAvatarView.Background` hides `VisualElement.Background`, so:

```csharp
new SfAvatarView()
    .BackgroundNew(Colors.LightBlue)   // SfAvatarView's own Background property
    .Background(someBrush)             // inherited VisualElement.Background
```

If a method you expect seems "missing", check for the `New`-suffixed variant.

## Practical Notes

- Generation happens **in the project where the attribute (or MSBuild property) lives** — typically your app project, or a shared UI project referenced by the app.
- Regeneration is automatic on build; generated sources are visible under *Analyzers → FmgLib.MauiMarkup.Generator* in the IDE.
- The generator only needs the type to derive from `BindableObject`; behaviors, image sources, and non-visual bindable objects (e.g. `SKFileLottieImageSource` above) are all fair game.
- Combine with [custom extension methods](custom-extension-methods.md) for anything the generator can't infer (regular CLR properties, methods, animations).

## Related Topics

- [Fluent Properties](fluent-properties.md) — the overload family being generated
- [Attached Properties](attached-properties.md)
- [Custom Extension Methods](custom-extension-methods.md)
