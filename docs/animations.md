# Animations

FmgLib.MauiMarkup generates an **`Animate<Property>To`** helper for every animatable bindable property (`double`, `Color`, and other interpolatable types) on every control — in addition to MAUI's built-in `TranslateTo`, `FadeTo`, `ScaleTo`, `RotateTo`, which of course still work.

## Generated `Animate…To` Methods

The pattern:

```csharp
Task<bool> Animate<PropertyName>To(T value, uint length = 250, Easing? easing = null)
```

Examples that exist out of the box:

```csharp
await label.AnimateFontSizeTo(40);                                  // double
await box.AnimateBackgroundColorTo(Colors.Teal, length: 500);       // Color
await border.AnimateOpacityTo(0, easing: Easing.CubicOut);          // double
await view.AnimateHeightRequestTo(320);                             // double
await view.AnimateSizeRequestTo(200, 120);                          // width + height together
await progressBar.AnimateProgressTo(0.8);
```

The returned `Task<bool>` completes when the animation finishes (`true` if it was cancelled, matching MAUI's animation convention), so animations compose with `async/await`:

```csharp
new Button()
    .Text("Save")
    .OnClicked(async b =>
    {
        await b.AnimateBackgroundColorTo(Colors.Green, 200);
        await Task.Delay(400);
        await b.AnimateBackgroundColorTo(AppColors.Primary, 200);
    })
```

Under the hood these run on the control's animation manager via the library's `Transformations.AnimateAsync` helper, interpolating from the current value to the target — the same mechanism you can reuse in [custom extension methods](custom-extension-methods.md).

## Combining Animations

**Sequential** — await one after another:

```csharp
await image.AnimateOpacityTo(0, 150);
image.Source = "next.png";
await image.AnimateOpacityTo(1, 150);
```

**Parallel** — start together, await all:

```csharp
await Task.WhenAll(
    card.AnimateBackgroundColorTo(Colors.LightYellow, 300),
    card.TranslateTo(0, -8, 300, Easing.CubicOut),
    card.AnimateOpacityTo(1, 300)
);
```

## MAUI Built-ins Still Apply

All standard `ViewExtensions` animations work on markup-built views:

```csharp
new Image()
    .Source("bell.png")
    .Assign(out var bell)
    .GestureRecognizers(new TapGestureRecognizer().OnTapped(async (s, e) =>
    {
        await bell.RotateTo(15, 60);
        await bell.RotateTo(-15, 120);
        await bell.RotateTo(0, 60);
    }))
```

For full keyframe control, `Microsoft.Maui.Controls.Animation` remains available:

```csharp
new Animation(v => box.Scale = v, 1, 1.4)
    .Commit(box, "pulse", length: 400, easing: Easing.SinInOut,
            repeat: () => true);
```

## Entrance Animations with Lifecycle Events

Combine [event handlers](event-handlers.md) with animations for page/view entrances:

```csharp
new VerticalStackLayout()
    .Opacity(0)
    .TranslationY(24)
    .OnLoaded(async v =>
    {
        await Task.WhenAll(
            v.AnimateOpacityTo(1, 350),
            v.TranslateTo(0, 0, 350, Easing.CubicOut));
    })
    .Children(/* ... */)
```

## Animations in Visual States

[`VisualState<T>`](visual-states.md) accepts async actions that run on state entry — the declarative home for interaction animations:

```csharp
new VisualState<Button>(VisualStates.Button.Pressed)
{
    async b => await b.ScaleTo(0.96, 80)
},
new VisualState<Button>(VisualStates.Button.Normal)
{
    async b => await b.ScaleTo(1, 80)
}
```

## Writing Your Own `Animate…To`

The template used by the generator is public API — reuse it for custom properties:

```csharp
public static Task<bool> AnimateCornerRadiusTo<T>(this T self, int value,
    uint length = 250, Easing? easing = null)
    where T : Button
{
    double from = self.CornerRadius;
    var transform = (double t) => Transformations.DoubleTransform(from, value, t);
    var callback = (double v) => { self.CornerRadius = (int)v; };
    return Transformations.AnimateAsync<double>(self, "AnimateCornerRadiusTo", transform, callback, length, easing);
}
```

See [Custom Extension Methods](custom-extension-methods.md) for the full pattern.

## Performance Tips

- Animate **transform properties** (`TranslationX/Y`, `Scale`, `Rotation`, `Opacity`) when possible — they avoid relayout. `Animate…RequestTo` size animations trigger layout each frame; keep them short and small.
- Don't start animations in `Build()` itself on hot-reload-enabled pages — `Build()` re-runs on every reload. Use `OnLoaded`/`OnAppearing` instead.
- Cancel long-running loops when the page disappears (`this.OnDisappearing(p => p.AbortAnimation("pulse"))`).

## Related Topics

- [Visual States](visual-states.md)
- [Event Handlers](event-handlers.md)
- [Custom Extension Methods](custom-extension-methods.md)
