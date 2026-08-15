---
name: mauimarkup-styling
description: Style and theme FmgLib.MauiMarkup apps — Style<T> with typed setters, resource dictionaries, BasedOn, visual states with VisualStates constants, property/data/multi/event triggers, light-dark AppThemeBinding, DynamicResource theme switching, gradients, shadows and Animate…To animations. Use when building a design system, adding dark mode, defining app-wide styles, adding hover/press/disabled states, or animating a control in a C# markup MAUI app.
license: MIT
---

# Styling, Theming, States & Animation

Requires the `mauimarkup` core skill. The full tables live in that skill's
`references/styling-theming.md`; this skill is the working method.

## Design-system layout that scales

Three static classes and one merge — this is the structure to reach for by default:

```csharp
public static class AppColors
{
    public static readonly Color Primary     = Color.FromArgb("#4F46E5");
    public static readonly Color PrimaryDark = Color.FromArgb("#3730A3");
    public static readonly Color Surface     = Color.FromArgb("#FFFFFF");
    public static readonly Color SurfaceDark = Color.FromArgb("#111827");
}

public static class AppStyles
{
    public static Style<Button> Primary { get; } = new(e => e
        .BackgroundColor(AppColors.Primary)
        .TextColor(Colors.White)
        .CornerRadius(10)
        .Padding(new Thickness(16, 12))
        .MinimumHeightRequest(44));

    public static Style<Label> Heading { get; } = new(e => e
        .FontSize(24)
        .FontAttributes(FontAttributes.Bold)
        .TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.White)));

    public static ResourceDictionary Default { get; } = new()
    {
        // implicit styles — apply to every control of the type in scope
        new Style<Label>(e => e.TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.White))),
        new Style<Entry>(e => e.FontSize(16)),
        Primary, Heading,
    };
}

// App.cs
this.Resources(new ResourceDictionary().MergedDictionaries(AppStyles.Default));
```

Explicit application: `new Button().Text("Save").Style(AppStyles.Primary)`.

Why static classes rather than string-keyed resources: `AppStyles.Primary` is compile-checked,
refactorable, and go-to-definition works. Keep something in the `ResourceDictionary` only when it must
be an implicit style or the target of a `DynamicResource` swap.

## Dark mode — one decision

```csharp
.TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.White))
```

This is a real `AppThemeBinding`: it follows the OS theme **and** `Application.Current.UserAppTheme` at
runtime, repainting controls already on screen. No rebuild, no dictionary reload.

```csharp
// ✘ evaluated once — the single most common theming bug
.TextColor(Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black)
```

One caveat: a **nested** builder (`.OnDark(l => l.DynamicResource("X"))`) cannot be carried by a theme
binding and resolves once at build time. Keep plain values in both branches when the theme must switch
live.

For user-selectable themes beyond light/dark, use `DynamicResource`:

```csharp
new Style<Button>(e => e.BackgroundColor(e => e.DynamicResource("AccentColor")));
Application.Current!.Resources["AccentColor"] = Colors.Purple;   // repaints everything bound to it
```

## Visual states — interaction feedback

```csharp
new Style<Button>(e => e.FontSize(14).CornerRadius(10))
{
    new VisualState<Button>(VisualStates.Button.Normal, e => e
        .BackgroundColor(AppColors.Primary).TextColor(Colors.White)),

    new VisualState<Button>(VisualStates.Button.PointerOver, e => e
        .BackgroundColor(AppColors.PrimaryDark)),

    new VisualState<Button>(VisualStates.Button.Pressed, e => e
        .BackgroundColor(AppColors.PrimaryDark))
    {
        async b => await b.ScaleTo(0.97, 80)          // runs on state entry
    },

    new VisualState<Button>(VisualStates.Button.Disabled, e => e
        .BackgroundColor(Colors.LightGray).TextColor(Colors.Gray)),
}
```

Rules that save debugging time:

- **Always define `Normal`.** The VSM restores only properties some state sets.
- Use `VisualStates.*` constants, never string literals — `VisualStates.Button.Pressed`,
  `VisualStates.VisualElement.Focused`, `VisualStates.CollectionView.Selected`, etc.
- Directly on a control, pass a `VisualStateGroupList` to `.VisualStateGroups(...)`. States written
  straight into the list land in `CommonStates`; wrap them in a `VisualStateGroup().Name(...).States(...)`
  for a custom group. `VisualStateGroup` does not support `{ … }` initializers.

Adaptive layout without any code-behind:

```csharp
new Grid().VisualStateGroups(new VisualStateGroupList
{
    new VisualState<Grid>("Wide",   e => e.ColumnSpacing(24)) { new AdaptiveTrigger().MinWindowWidth(800) },
    new VisualState<Grid>("Narrow", e => e.ColumnSpacing(8))  { new AdaptiveTrigger().MinWindowWidth(0) },
})
```

Other state triggers: `CompareStateTrigger`, `DeviceStateTrigger`, `OrientationStateTrigger`,
`StateTrigger`.

## Triggers — condition-driven properties

```csharp
new Button()
    .Text("Save")
    .Triggers(
        new DataTrigger(typeof(Button))
            .Binding(e => e.Path("Text.Length").Source(entry))
            .Value(0)
            .Setters(new Setters<Button>(e => e.IsEnabled(false))))
```

`Setters(new Setters<T>(e => e…))` is the typed setter builder — the same fluent methods again.
Property triggers use `.Property(X.YProperty).Value(v)`; `MultiTrigger` combines `PropertyCondition` /
`BindingCondition`; `EventTrigger` runs `TriggerAction<T>`s and reverts nothing on its own.

In a `Style<T>` a trigger applies app-wide; on a control it applies to that instance.

## Choosing the mechanism

| Situation | Use |
|---|---|
| A value that never changes | direct value |
| Driven by the view model | binding (`Getter`) |
| Focus / hover / press / disabled feedback | visual states in a style |
| Window size or orientation | visual states + `AdaptiveTrigger` / `OrientationStateTrigger` |
| One property flipping on a condition | `DataTrigger` |
| Reusable behavior across controls | a `Behavior<T>` |
| One-off | event handler |

If the logic is really domain logic (`IsError → Red`), a computed view-model property beats every
option above and is unit-testable.

## Gradients, shadows, shapes

```csharp
new Border()
    .StrokeShape(new RoundRectangle().CornerRadius(16))
    .StrokeThickness(0)
    .Background(new LinearGradientBrush()
        .StartPoint(new Point(0, 0)).EndPoint(new Point(1, 1))
        .GradientStops(
            new GradientStop().Color(AppColors.Primary).Offset(0.0f),
            new GradientStop().Color(Colors.Teal).Offset(1.0f)))
    .Shadow(new Shadow().Radius(12).Opacity(0.18f).Offset(new Point(0, 6)))
    .Content(/* … */)
```

`RadialGradientBrush` takes `.Center(...)` and `.Radius(...)`. `.Clip(geometry)` clips any view.

## Animation

Every interpolatable bindable property gets `AnimatePTo(value, uint length = 250, Easing? easing = null)`
returning `Task<bool>`:

```csharp
await label.AnimateFontSizeTo(40, 300, Easing.CubicOut);
await box.AnimateBackgroundColorTo(Colors.Teal, 500);
await view.AnimateSizeRequestTo(200, 120);
```

Sequential is `await` after `await`; parallel is `Task.WhenAll`:

```csharp
await Task.WhenAll(
    card.AnimateOpacityTo(1, 300),
    card.TranslateTo(0, 0, 300, Easing.CubicOut));
```

Entrance animation, correctly placed (**not** in `Build()`, which re-runs on every hot reload):

```csharp
new VerticalStackLayout()
    .Opacity(0).TranslationY(24)
    .OnLoaded(async v => await Task.WhenAll(
        v.AnimateOpacityTo(1, 350),
        v.TranslateTo(0, 0, 350, Easing.CubicOut)))
    .Children(/* … */)
```

MAUI's own `TranslateTo`/`FadeTo`/`ScaleTo`/`RotateTo` and `new Animation(...).Commit(...)` still work.
Cancel loops on disappear: `this.OnDisappearing(p => p.AbortAnimation("pulse"))`.

Prefer transforms (`TranslationX/Y`, `Scale`, `Rotation`, `Opacity`) over layout properties —
`Animate…RequestTo` triggers a relayout every frame.

## Custom design-system methods

Wrap recurring chains; keep the generic so the type flows:

```csharp
public static T Card<T>(this T self) where T : Border
    => self.StrokeThickness(0)
           .StrokeShape(new RoundRectangle().CornerRadius(16))
           .Padding(16)
           .BackgroundColor(e => e.OnLight(AppColors.Surface).OnDark(AppColors.SurfaceDark))
           .Shadow(new Shadow().Radius(10).Opacity(0.12f));

new Border().Card().Content(/* … */)
```

To make a custom method usable **inside `Style<T>`** as well, implement the `SettersContext<T>`
overloads — the four-overload template is in the core skill's `references/cheatsheet.md`.
