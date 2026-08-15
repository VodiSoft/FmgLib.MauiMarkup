---
name: mauimarkup-thirdparty
description: Generate FmgLib.MauiMarkup fluent extensions for third-party and custom MAUI controls with the Roslyn source generator — [MauiMarkup(typeof(X))], [MauiMarkupAttachedProp], the MauiMarkupSourceGenerator MSBuild switch, base-class generation, the New suffix rule and deliberately skipped members. Use when a control from Syncfusion, UraniumUI, SkiaSharp, ZXing, InputKit, DevExpress, DrawnUI or the user's own library has no fluent methods, or when a fluent method is unexpectedly missing or ambiguous.
license: MIT
---

# Third-Party & Custom Controls

Requires the `mauimarkup` core skill.

FmgLib's fluent methods come from a Roslyn source generator. **The MAUI surface ships pre-generated
inside the library; anything else must be opted in.** If a third-party control has no fluent methods,
that is the expected state, not a bug — three mechanisms turn them on.

## 1. `[MauiMarkup(typeof(...))]` — opt-in per control (recommended)

Put the attribute on **any class** in the project that consumes the control. The class is just an
anchor; its own contents are irrelevant.

```csharp
using FmgLib.MauiMarkup;
using SkiaSharp.Extended.UI.Controls;
using ZXing.Net.Maui.Controls;
using UraniumUI.Material.Controls;

namespace MyApp;

[MauiMarkup(typeof(CameraView))]
[MauiMarkup(typeof(SKLottieView), typeof(SKFileLottieImageSource))]
[MauiMarkup(typeof(TextField), typeof(EditorField), typeof(InputField))]
public static class MauiProgram
{
    public static MauiApp CreateMauiApp() { /* … */ }
}
```

- The constructor takes **1..N types** — batch related controls.
- **Multiple attributes** may sit on one class.
- `MauiProgram` is a popular single anchor, but a dedicated `Markup.cs` is cleaner in bigger apps.

For each type the generator emits the standard four property overloads (core skill,
`references/cheatsheet.md`) for every **bindable property** and both `On<Event>` shapes for every
**event**, in the
`FmgLib.MauiMarkup` namespace — so your existing using covers them:

```csharp
new TextField()
    .Title("Password")
    .AccentColor(Colors.CadetBlue)
    .IsPassword(true),

new SKLottieView()
    .Source(new SKFileLottieImageSource().File("iconapp.json"))
    .RepeatCount(-1)
    .SizeRequest(250, 250),

new CameraView()
    .CameraLocation(CameraLocation.Front)
    .IsTorchOn(e => e.Path("TorchEnabled"))
    .OnFrameReady((s, e) => Analyze(e))
```

Styles work too, because the `SettersContext` overloads are generated:
`new Style<CameraView>(e => e.IsTorchOn(false))`.

The generator only needs the type to derive from `BindableObject` — behaviors, image sources and other
non-visual bindable objects are fair game.

## 2. `[MauiMarkupAttachedProp]` — attached properties

Four constructor parameters, in order:

| # | Parameter | Meaning |
|---|---|---|
| 1 | `controlType` | the class **declaring** the attached property |
| 2 | `propertyName` | the `BindableProperty` field name (use `nameof`) |
| 3 | `returnType` | the property's value type |
| 4 | `declaringType` | the type it will be **applied to** |

```csharp
[MauiMarkupAttachedProp(typeof(InputKit.Shared.Controls.FormView),
                        nameof(InputKit.Shared.Controls.FormView.IsSubmitButtonProperty),
                        typeof(bool),
                        typeof(Button))]
[MauiMarkup(typeof(InputKit.Shared.Controls.FormView))]
public class MyFormView { }
```

Naming follows the usual owner+property rule:

```csharp
new Button().Text("Login").FormViewIsSubmitButton(true)
```

## 3. Automatic mode

```xml
<PropertyGroup>
  <MauiMarkupSourceGenerator>true</MauiMarkupSourceGenerator>
</PropertyGroup>
```

The generator then scans referenced assemblies and generates for every eligible public
`BindableObject`. The required `CompilerVisibleProperty` wiring is injected by the NuGet package's
`buildTransitive` props — nothing else to configure.

**Trade-off:** convenient, but it generates code for everything it finds, which grows compile time in
large solutions. Start with attributes; switch to automatic only if the attribute list becomes tedious,
and switch back if builds slow down.

## Base classes come for free

Annotating a leaf control also generates for its **eligible third-party base classes**, where most of
the bindable surface usually lives:

```csharp
[MauiMarkup(typeof(SfButton))]
public class Markup { }

new SfButton()
    .Text("Login")
    .Command(vm.LoginCommand)    // declared on ButtonBase — generated automatically
    .FontSize(15)                // ButtonBase
    .StrokeThickness(1)          // SfButton
```

`[MauiMarkup(typeof(SfButton))]` produces both `SfButtonExtension` **and** `ButtonBaseExtension`. MAUI
core base classes are never regenerated — their extensions already ship in the library.

## The redefinition rules (why a method looks "missing")

| Case | Result |
|---|---|
| Derived class redeclares a base property with the **same type** (e.g. `SfButton.TextColor`) | **No duplicate, no suffix.** The base extension (`TextColor<T>() where T : ButtonBase`) already serves the derived control; duplicating it would make every call ambiguous (CS0121) |
| Derived class redeclares with a **different type** (e.g. `SfAvatarView.Background`: `Brush` → `Color`) | The derived method gets the **`New` suffix** |

```csharp
new SfAvatarView()
    .BackgroundNew(Colors.LightBlue)   // SfAvatarView's own Color Background
    .Background(someBrush)             // inherited VisualElement Brush Background
```

So: a method that seems absent on the leaf type is almost always served by a base-class extension under
the same name — IntelliSense will still offer it.

## Members the generator deliberately skips

The control still generates; only these members are omitted, because wrapping them would break **your**
build:

| Member shape | Why |
|---|---|
| `protected` / `internal` / `private protected` setter | `self.Prop = value` is inaccessible (CS0272). *If a matching `BindableProperty` exists, the method IS generated and routes through `SetValue`* |
| `init`-only setter | assignable only in an object initializer (CS8852) |
| Read-only collection with no callable `Add` (`Queue<T>`, `LinkedList<T>`, `ReadOnlyCollection<T>`) | the generated `foreach … Add(item)` body would not compile (CS1929) |
| `static` property or event | an instance-fluent call would write to the type (CS0176) |
| Member type (or a generic argument) is not `public` | a public extension cannot expose it (CS0053) |
| Member or its type is `[Obsolete(…, error: true)]` | any mention is a hard error (CS0619) |
| `ref`-returning property, `ref struct` type (e.g. `Span<T>`) | cannot be captured by the overloads |

For one of these, write a manual extension (core skill, `references/cheatsheet.md` → Custom extensions)
or use `.InvokeOnElement(x => …)`.

## Your own controls

Identical: give the control `BindableProperty`s and annotate it from the app project. A custom control
with bindable properties gets the same four overloads and full binding/style/animation support:

```csharp
public class RatingView : ContentView
{
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(int), typeof(RatingView), 0);

    public int Value { get => (int)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
}

[MauiMarkup(typeof(RatingView))]
public class Markup { }

new RatingView().Value(e => e.Getter(static (ProductVM p) => p.Stars))
```

A plain CLR property gets nothing — bindings need a `BindableProperty`. Add one if the value should be
bindable; otherwise use `InvokeOnElement`.

## Diagnosing

1. Build once — generated code only exists after a compile.
2. Inspect it: **Analyzers → FmgLib.MauiMarkup.Generator** in the IDE shows every generated file.
3. Method still absent? Walk the table above: is it a real `BindableProperty`? Does it need the `New`
   suffix? Is it served by a base class? Is it on the skip list?
4. Generation happens **in the project where the attribute or MSBuild property lives** — usually the app
   project or a shared UI project referenced by it. An attribute in a class library that the app doesn't
   reference generates nothing useful.
