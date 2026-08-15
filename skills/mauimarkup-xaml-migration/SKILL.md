---
name: mauimarkup-xaml-migration
description: Convert .NET MAUI XAML into FmgLib.MauiMarkup fluent C# — pages, resource dictionaries, styles, data templates, triggers and Shell definitions. Use when the user asks to migrate a XAML page or a whole MAUI app to C# markup, to "remove XAML", to translate a XAML snippet, or when a MauiMarkup project still contains .xaml files that need porting.
license: MIT
---

# XAML → FmgLib.MauiMarkup Migration

Requires the `mauimarkup` core skill for the API model. This skill is the translation procedure.

Migration is **incremental and per page** — XAML pages and FmgLib pages coexist in one app, so never
propose a big-bang rewrite unless the user asks for one.

## Procedure for one page

1. Read the `.xaml` **and** its `.xaml.cs`. The code-behind holds event handlers, fields and
   constructor logic that must survive.
2. Create `PageName.cs` (replacing both files) using the standard skeleton.
3. Translate the visual tree top-down, element by element, using the mapping tables below.
4. Move code-behind handlers into the class. Handlers can keep their bodies; change signatures to
   `void OnX(ControlType sender)` for the `Action<T>` shape, or keep `(object sender, EventArgs e)` for
   the classic shape.
5. Delete `PageName.xaml` and `PageName.xaml.cs`. Verify no `InitializeComponent()` remains.
6. Build. Fix any missing method by walking the "method really doesn't exist" list in the core skill.

Register the page exactly as before — DI registration, `Routing.RegisterRoute`, Shell content: nothing
about app architecture changes.

## Element mapping

| XAML | FmgLib.MauiMarkup |
|---|---|
| `<Label Text="Hi" FontSize="32" />` | `new Label().Text("Hi").FontSize(32)` |
| `<ContentPage>…</ContentPage>` | `class P : ContentPage, IFmgLibHotReload` + `this.Content(...)` |
| child elements of a layout | `.Children(child1, child2, …)` (params) |
| `ContentPage.Content` / `Border.Content` | `.Content(view)` |
| `x:Name="btn"` | `.Assign(out var btn)` |
| `Clicked="OnSave"` | `.OnClicked(OnSave)` |
| `{Binding Name}` | `.Text(e => e.Path("Name"))` — better: `.Text(e => e.Getter(static (VM vm) => vm.Name))` |
| `{Binding Value, Source={x:Reference slider}}` | `.Text(e => e.Path("Value").Source(slider))` |
| `{Binding Price, StringFormat='{}{0:C}'}` | `.Text(e => e.Path("Price").StringFormat("{0:C}"))` |
| `{Binding X, Mode=TwoWay}` | `.Text(e => e.Path("X").BindingMode(BindingMode.TwoWay))` |
| `{Binding X, Converter={StaticResource C}}` | `.Text(e => e.Path("X").Converter(Converters.C))` — or inline `.Convert(...)` |
| `{DynamicResource Key}` | `.BackgroundColor(e => e.DynamicResource("Key"))` |
| `{StaticResource Key}` | a plain C# reference to the object — that's the point of C# markup |
| `{AppThemeBinding Light=…, Dark=…}` | `.TextColor(e => e.OnLight(a).OnDark(b))` |
| `{OnPlatform iOS=…, Default=…}` | `.Margin(e => e.OniOS(a).Default(b))` |
| `{OnIdiom Phone=…, Desktop=…}` | `.FontSize(e => e.OnPhone(a).OnDesktop(b).Default(c))` |
| `Grid.Row="1" Grid.ColumnSpan="2"` | `.Row(1).ColumnSpan(2)` |
| `RowDefinitions="Auto,*,64"` | `.RowDefinitions(e => e.Auto().Star().Absolute(64))` |
| `ColumnDefinitions="2*,0.5*,0.5*,0.5*"` | `.ColumnDefinitions(e => e.Star(2).Star(0.5, count: 3))` |
| `HorizontalOptions="Center"` | `.CenterHorizontal()` (or `.HorizontalOptions(LayoutOptions.Center)`) |
| `HorizontalOptions="Center" VerticalOptions="Center"` | `.Center()` |
| `HorizontalTextAlignment="Center"` | `.TextCenter()` |
| `Shell.TabBarIsVisible="False"` | `.ShellTabBarIsVisible(false)` |
| `SemanticProperties.Description="…"` | `.SemanticDescription("…")` |
| `<Style TargetType="Button">` | `new Style<Button>(e => e…)` |
| `BasedOn="{StaticResource Base}"` | `new Style<Button>(baseStyle, e => e…)` |
| `<Setter Property="P" Value="V" />` | `.P(V)` inside the style lambda |
| `<DataTemplate>` in `ItemTemplate` | `.ItemTemplate(() => rowView)` |
| `<DataTrigger Binding="…" Value="…">` | `new DataTrigger(typeof(T)).Binding(e => e…).Value(v).Setters(new Setters<T>(e => e…))` |
| `<VisualState x:Name="Normal">` | `new VisualState<T>(VisualStates.T.Normal, e => e…)` |
| `<Span>` in `FormattedString` | `new FormattedString().Spans(new Span().Text("…"))` |
| `<ResourceDictionary>` | `new ResourceDictionary { style1, style2 }` passed to `.Resources(...)` |
| `MergedDictionaries` | `new ResourceDictionary().MergedDictionaries(a, b)` |

## Worked conversion

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui" x:Class="MyApp.MainPage">
    <VerticalStackLayout Spacing="25" Padding="30">
        <Label Text="Hello, World!" FontSize="32" HorizontalOptions="Center" />
        <Button x:Name="CounterBtn" Text="Click me" Clicked="OnCounterClicked" />
    </VerticalStackLayout>
</ContentPage>
```

```csharp
using FmgLib.MauiMarkup;

namespace MyApp;

public partial class MainPage : ContentPage, IFmgLibHotReload
{
    int count = 0;

    public MainPage() => this.InitializeHotReload();

    public void Build() =>
        this.Content(
            new VerticalStackLayout()
            .Spacing(25)
            .Padding(30)
            .Children(
                new Label().Text("Hello, World!").FontSize(32).CenterHorizontal(),
                new Button().Text("Click me").OnClicked(OnCounterClicked)
            )
        );

    private void OnCounterClicked(Button sender)
    {
        count++;
        sender.Text = $"Clicked {count} times";
    }
}
```

## Constructs that need judgement, not a table row

**`x:Reference` forward references.** In XAML any element can reference any other. In C# a variable
must be declared before use. If control A (earlier) references control B (later), pre-declare:

```csharp
Button submit = null!;
this.Content(new VerticalStackLayout().Children(
    new Entry().OnTextChanged((s, e) => submit.IsEnabled = e.NewTextValue?.Length > 0),
    new Button().Text("Submit").IsEnabled(false).Assign(out submit)));
```

**`{StaticResource}` disappears.** A static resource is just a shared object. Put it in a static class
(`AppColors.Primary`, `AppStyles.Card`) and reference it directly — compile-checked, refactorable, and
no dictionary lookup. Only keep it in a `ResourceDictionary` when it must be an *implicit* style or the
target of a `DynamicResource` swap.

**Converters.** Most XAML converters exist only because XAML has no lambdas. Replace them with
`.Convert((T v) => …)` inline, or delete them in favour of a computed view-model property. Keep the
`IValueConverter` class when it is reused widely, has state, or comes from a toolkit.

**`RelativeSource AncestorType` to reach the page VM from a template.** In C# just capture the VM in a
local and reference it inside the template lambda — no relative binding needed.

**Markup extensions with no equivalent** (`x:Static`, `x:Type`, `x:Array`) are plain C#: a static field
reference, `typeof(T)`, an array literal.

**Custom `ControlTemplate` content** keeps its templated-parent binding via
`.BindTemplatedParent(prop, path)`.

## After migrating

- Add a `global using FmgLib.MauiMarkup;` so no page needs the using.
- Prefer `Getter` over the `Path` strings you just transcribed — this is the moment to gain
  compile-time safety on every binding.
- Extract repeated subtrees into private `View`-returning methods or `ContentView` components. This is
  the largest readability win over the XAML you started from, and it has no XAML equivalent.
- Confirm hot reload works (`mauimarkup-hotreload`) — the dev loop you get in return is the main reason
  to migrate.
