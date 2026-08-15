---
name: mauimarkup
description: Build .NET MAUI user interfaces in pure C# with FmgLib.MauiMarkup fluent markup — pages, layouts, bindings, styles, events, no XAML. Use whenever the project references FmgLib.MauiMarkup, whenever a file contains `using FmgLib.MauiMarkup`, `IFmgLibHotReload`, `InitializeHotReload()`, `FmgLibContentPage`, `Style<T>`, `.Assign(out var …)` or `e => e.Path(...)`, and whenever the user asks to write, convert, review or debug a MAUI screen in C# markup instead of XAML.
license: MIT
---

# FmgLib.MauiMarkup — Core

Fluent C# markup for .NET MAUI. Every **bindable property** of every control has a chainable
extension method **with the same name as the property**; every **event** has an `On<EventName>`
method. Every method returns the control (generic `T`), so chains preserve the concrete type and the
indentation of the code mirrors the visual tree.

**The single most important rule: do not guess API names — derive them.** Property `Foo` → `.Foo(...)`.
Event `Bar` → `.OnBar(...)`. Everything else in this skill is a consequence of that rule.

```csharp
new Label()
    .Text("Hello, FmgLib!")
    .FontSize(30)
    .TextColor(Colors.Green)
    .Center()
```

## Non-negotiables

1. `using FmgLib.MauiMarkup;` must be in scope. Prefer a `global using FmgLib.MauiMarkup;` in
   `GlobalUsings.cs` — then never write it again.
2. **Never emit XAML** for a page in a MauiMarkup project. No `.xaml`, no `.xaml.cs`, no
   `InitializeComponent()`. A page is one `.cs` file.
3. There is **no** `builder.UseFmgLibMauiMarkup()`. Installing the NuGet package is the whole setup.
   The only `MauiProgram` call this library ever needs is `UseMauiMarkupLocalization(...)`, and only
   if you use localization.
4. Fluent methods exist for **`BindableProperty`s only**. A plain CLR property has no fluent method —
   use `.InvokeOnElement(x => x.Prop = value)` instead. Don't invent a method and hope.
5. Third-party controls (Syncfusion, UraniumUI, SkiaSharp, ZXing, DevExpress, your own libraries) have
   **no** fluent methods until the source generator is told about them → `mauimarkup-thirdparty` skill.

## Page skeleton — use this every time

```csharp
using FmgLib.MauiMarkup;

namespace MyApp.Pages;

public partial class MainPage : ContentPage, IFmgLibHotReload
{
    private readonly MainPageViewModel viewModel;   // state lives in fields

    public MainPage(MainPageViewModel viewModel)
    {
        this.viewModel = viewModel;
        this.InitializeHotReload();                 // calls Build() now, re-calls it on hot reload
    }

    public void Build() =>
        this
        .Title("Home")
        .BindingContext(viewModel)
        .Content(
            new VerticalStackLayout()
            .Spacing(24)
            .Padding(30)
            .Children(
                new Label()
                    .Text(e => e.Getter(static (MainPageViewModel vm) => vm.Greeting))
                    .FontSize(32)
                    .CenterHorizontal(),

                new Button()
                    .Text("Click me")
                    .Command(viewModel.IncrementCommand)
                    .CenterHorizontal()
            )
        );
}
```

`Build()` may run **many times** per debug session. It must be idempotent and must not own state.
See "Build() discipline" below and the `mauimarkup-hotreload` skill.

Alternatives to `IFmgLibHotReload` + `InitializeHotReload()`:

| Base | When |
|---|---|
| `FmgLibContentPage` | No view model; base class calls `public override void Build()` for you |
| `FmgLibContentPage<TViewModel>` | MVVM — VM passed to the constructor, assigned to `BindingContext` **before** the first `Build()`, and `BindingContext` is re-typed so `BindingContext.SaveCommand` needs no cast |
| Plain constructor, no `Build()` | Throwaway/simple views; you lose hot reload |

## The four property overload shapes

Every property `P` of type `V` on control `C` gets:

```csharp
.P(V value)                                                   // 1 direct value
.P(e => …)                     // Func<PropertyContext<V>, IPropertyBuilder<V>>   2 builder
// 3 & 4: the same two shapes on SettersContext<T>, used automatically inside Style<T>
```

You only ever *write* shapes 1 and 2. Shapes 3–4 light up on their own inside
`new Style<C>(e => e.P(...))` — same method names, different context.

Interpolatable properties (`double`, `Color`, `Thickness`, …) additionally get
`AnimatePTo(value, uint length = 250, Easing? easing = null)` returning `Task<bool>`.

### Shape 2 — everything that is not a constant

The builder lambda **must return the chain** (`e => e.Path("X")`), never a statement body.

| Builder call | Produces |
|---|---|
| `.Path("Name")`, `.Source(x)`, `.BindingMode(...)`, `.StringFormat(...)` | a `Binding` |
| `.Getter(static (VM vm) => vm.Name)` | a **compiled** binding (preferred for VM paths) |
| `.OnLight(a).OnDark(b)` | a live `AppThemeBinding` |
| `.OnPhone(a).OnTablet(b).OnDesktop(c).Default(d)` | `OnIdiom` (also `OnTV`, `OnWatch`) |
| `.OnAndroid(a).OniOS(b).OnWinUI(c).Default(d)` | `OnPlatform` (also `OnMacCatalyst`, `OnTizen`) |
| `.DynamicResource("Key")` | `DynamicResource` |
| `.Translate("Key")` / `.TranslateResx("Key")` | live localization binding |
| `.Convert((int n) => …)` / `.ConvertBack(...)` | inline converter, no converter class |
| `.Path(...)` twice + `.MultiConvert(...)` | a `MultiBinding` |

```csharp
new Label()
    .Text(e => e.Path("Price").StringFormat("{0:C}"))
    .TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.White))
    .FontSize(e => e.OnDesktop(20).OnPhone(15).Default(16))
    .Margin(e => e.OniOS(new Thickness(0, 20, 0, 0)).Default(new Thickness(0)))
```

Full binding reference: `references/bindings.md`.

## Layout

```csharp
new Grid()
.RowDefinitions(e => e.Auto().Star().Absolute(64))     // Auto/Star/Absolute, each takes count:
.ColumnDefinitions(e => e.Star(3).Star(7))
.RowSpacing(8).ColumnSpacing(8).Padding(16)
.Children(
    new Label().Text("Header"),                        // row 0, col 0 by default
    new CollectionView().Row(1).ColumnSpan(2),
    new Button().Text("Save").Row(2).Column(1)
)
```

- Placement: `.Row(i) .Column(i) .RowSpan(n) .ColumnSpan(n)` — and `.GridSpan(column, row)` for both.
  Grid placement is the **only** attached property that drops its owner prefix.
- Alignment inside the parent: `.Center() .CenterHorizontal() .CenterVertical() .AlignTopLeft()
  .AlignBottomRight() .FillHorizontal() .FillBothDirections()` … and the general
  `.AlignLayout(vertical:, horizontal:)`.
- Alignment of **text inside** a control: `.TextCenter() .TextTopLeft() …`
  `Center()` positions the control; `TextCenter()` positions the glyphs. They are frequently combined.
- Shorthands: `.SizeRequest(w, h)`, `.Margin(10)` / `(h, v)` / `(l, t, r, b)`, `.Padding(...)` same,
  `.AbsoluteLayoutBounds(x, y, w, h)`.

Full table: `references/layout.md`.

## Events

Two shapes for every event. Prefer the second — the sender is already typed:

```csharp
new Entry().OnTextChanged((sender, e) => Search(e.NewTextValue));   // classic, gives EventArgs
new Button().OnClicked(b => b.Text = "Clicked");                    // Action<T>, no boilerplate
new Button().OnClicked(OnSavePressed);                              // method group: void OnSavePressed(Button sender)
```

Page lifecycle chains too: `this.OnAppearing(async p => await vm.RefreshAsync()).Content(...)`.

**Never** subscribe to a long-lived object (`Application.Current`, a static service, a singleton VM)
inside `Build()` — subscriptions stack up on every hot reload. Do that in the constructor.

## References to other controls

```csharp
new Slider().Assign(out var slider).Minimum(0).Maximum(100),
new Label().Text(e => e.Path("Value").Source(slider).StringFormat("{0:F0}"))
```

`Assign(out T obj)` is the `x:Name` replacement — declare locals **inside** `Build()`. If an earlier
control needs a later one, pre-declare the variable (`Button submit = null!;`) and `Assign(out submit)`.

`InvokeOnElement(x => …)` is the escape hatch for anything without a fluent method:

```csharp
new CollectionView()
    .ItemsSource(items)
    .InvokeOnElement(cv => cv.ScrollTo(items.Count - 1, position: ScrollToPosition.End))
```

## Styling in one breath

```csharp
new Style<Button>(e => e
    .BackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(Colors.White))
    .CornerRadius(8)
    .Padding(new Thickness(14, 10)))
```

Added to a `ResourceDictionary` it applies implicitly to every `Button` in scope; passed to
`.Style(...)` it applies to one control. `Style<T>` converts implicitly to MAUI's `Style`.
Constructors take `basedOn:` and `applyToDerivedTypes:`. Visual states, triggers and `Action<T>`
entries go in the collection initializer **before** any trailing fluent calls.
→ `mauimarkup-styling`, `references/styling-theming.md`.

## Lists

```csharp
new CollectionView()
    .ItemsSource(e => e.Path("Products"))
    .ItemTemplate(() =>                               // the DataTemplate of the markup world
        new VerticalStackLayout().Padding(10).Children(
            new Label().Text(e => e.Getter(static (ProductVM p) => p.Name)),
            new Label().Text(e => e.Getter(static (ProductVM p) => p.Price).StringFormat("{0:C}"))
        ))
    .EmptyView(new Label().Text("Nothing here yet.").Center())
```

Inside a template the `BindingContext` is the **item**. → `mauimarkup-collections`.

## Build() discipline

| Do | Don't |
|---|---|
| Describe the whole UI from scratch each call | Mutate the previous tree incrementally |
| Keep view models, counters, services in **fields** set in the constructor | `new MyViewModel()` inside `Build()` |
| `Assign(out var x)` into locals declared in `Build()` | Cache controls in fields and reuse across rebuilds |
| Start animations from `OnLoaded` / `OnAppearing` | Start animations or network calls in `Build()` |
| Subscribe to long-lived events in the constructor | `Application.Current.RequestedThemeChanged += …` in `Build()` |

## Name derivation table

| You want | Write |
|---|---|
| Property `FontSize` | `.FontSize(...)` |
| Event `Clicked` | `.OnClicked(...)` |
| `Grid.Row` / `Grid.ColumnSpan` | `.Row(...)` / `.ColumnSpan(...)` — prefix dropped |
| Any other attached property `Owner.Prop` | `.OwnerProp(...)` — `Shell.TitleColor` → `.ShellTitleColor()`, `SemanticProperties.Hint` → `.SemanticHint()`, `AutomationProperties.Name` → `.AutomationName()`, `ToolTipProperties.Text` → `.ToolTipPropertiesText()`, `BindableLayout.ItemsSource` → `.BindableLayoutItemsSource()` |
| A property a subclass redeclares with a **different type** | `.PropNew(...)` — e.g. `.BackgroundNew(Colors.Red)` |
| A property a subclass redeclares with the **same type** | the base method — no suffix, no duplicate |
| An animation for property `P` | `await x.AnimatePTo(value, length, easing)` |

## When a method really doesn't exist

Work down this list before concluding anything is missing:

1. Is `using FmgLib.MauiMarkup;` present?
2. Is it a real `BindableProperty`, or a plain CLR property? → `InvokeOnElement`.
3. Third-party control? → `[MauiMarkup(typeof(X))]` (`mauimarkup-thirdparty`).
4. Try the `New` suffix.
5. Is it served by a **base class** extension under the same name? (IntelliSense will show it.)
6. Deliberately skipped member? The generator skips non-public setters, `init`-only setters,
   `static` members, non-public member types, hard-`[Obsolete]` members, `ref`/`ref struct` types, and
   read-only collections with no callable `Add`. Write a manual extension for those.

## Reference bundle

Read these when the task needs the detail — they are part of this skill:

| File | Contents |
|---|---|
| `references/cheatsheet.md` | Dense API map: controls, containers, shorthands, escape hatches |
| `references/bindings.md` | Every builder method, compiled bindings, converters, multi-binding, `Bind()` |
| `references/layout.md` | Complete layout-options / text-alignment / attached-property tables |
| `references/styling-theming.md` | `Style<T>`, visual states, triggers, theming strategies |
| `references/pitfalls.md` | Compile errors, silent-binding failures, performance, review checklist |

## Companion skills

`mauimarkup-xaml-migration` · `mauimarkup-mvvm` · `mauimarkup-shell` · `mauimarkup-collections` ·
`mauimarkup-styling` · `mauimarkup-localization` · `mauimarkup-thirdparty` · `mauimarkup-hotreload` ·
`mauimarkup-review`

Docs: https://fmglibmauimarkup.vodisoft.com · Source: https://github.com/VodiSoft/FmgLib.MauiMarkup
