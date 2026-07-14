# User-Defined Extension Methods

You can extend FmgLib.MauiMarkup with your own fluent methods — either **semantic shorthands** composed from existing methods, or **full property methods** that participate in bindings, styles and animations exactly like the built-ins.

## Level 1 — Composition Shorthands

The simplest and most useful kind: wrap recurring chains in a static method. No library machinery needed, because every fluent method returns `T`:

```csharp
public static class MarkupHelpers
{
    public static T PrimaryText<T>(this T self) where T : Label
        => self.FontSize(16).TextColor(AppColors.TextPrimary).FontFamily("OpenSansRegular");

    public static T Card<T>(this T self) where T : Border
        => self
            .BackgroundColor(e => e.OnLight(Colors.White).OnDark("#1E1E2E".ToColor()))
            .StrokeThickness(0)
            .Padding(16)
            .Shadow(new Shadow().Radius(10).Opacity(0.15f));
}

// usage:
new Label().Text("Total").PrimaryText()
new Border().Card().Content(/* ... */)
```

Keep the `<T>` generic + `where T : ...` constraint pattern so the concrete type flows through the chain.

## Level 2 — Full Property Methods (the four-overload template)

To make a property behave like a native FmgLib property — supporting direct values, the [builder lambda](fluent-properties.md) (bindings, `OnLight/OnDark`, `DynamicResource`…), and [styles](styling.md) — implement the four standard overloads. Here is the complete template for `Label.FontSize` (exactly what the generator emits):

```csharp
public static class MyLabelExtensions
{
    // 1. Direct value
    public static T FontSize<T>(this T self, double fontSize)
        where T : Microsoft.Maui.Controls.Label
    {
        self.SetValue(Microsoft.Maui.Controls.Label.FontSizeProperty, fontSize);
        return self;
    }

    // 2. Property builder — enables e.Path(...), e.OnLight(...), e.DynamicResource(...) etc.
    public static T FontSize<T>(this T self,
        Func<PropertyContext<double>, IPropertyBuilder<double>> configure)
        where T : Microsoft.Maui.Controls.Label
    {
        var context = new PropertyContext<double>(self, Microsoft.Maui.Controls.Label.FontSizeProperty);
        configure(context).Build();
        return self;
    }

    // 3. Style setter
    public static SettersContext<T> FontSize<T>(this SettersContext<T> self, double fontSize)
        where T : Microsoft.Maui.Controls.Label
    {
        self.XamlSetters.Add(new Setter
        {
            Property = Microsoft.Maui.Controls.Label.FontSizeProperty,
            Value = fontSize
        });
        return self;
    }

    // 4. Style setter with builder
    public static SettersContext<T> FontSize<T>(this SettersContext<T> self,
        Func<PropertySettersContext<double>, IPropertySettersBuilder<double>> configure)
        where T : Microsoft.Maui.Controls.Label
    {
        var context = new PropertySettersContext<double>(self.XamlSetters,
            Microsoft.Maui.Controls.Label.FontSizeProperty);
        configure(context).Build();
        return self;
    }
}
```

The ingredients:

- **`PropertyContext<TValue>`** — pairs the control with the `BindableProperty`; the builder returned by your lambda knows how to bind/set it.
- **`SettersContext<T>`** — collects `Setter`s inside a `Style<T>` definition.
- Always target the `BindableProperty` with `SetValue` (not the CLR property) so styles/bindings/triggers keep working.

With all four in place, every usage pattern lights up:

```csharp
new Label().FontSize(28);                                          // value
new Label().FontSize(e => e.Path("MyFontSize").Source(myModel));   // binding
new Label().FontSize(e => e.OnPhone(30).OnTablet(50).Default(40)); // idiom
new Style<Label>(e => e.FontSize(20).CenterVertical());            // style
```

## Level 3 — Animation Helpers

Add an `Animate…To` for your property using the library's `Transformations` utility (see [Animations](animations.md)):

```csharp
public static Task<bool> AnimateFontSizeTo<T>(this T self, double value,
    uint length = 250, Easing? easing = null)
    where T : Microsoft.Maui.Controls.Label
{
    double fromValue = self.FontSize;
    var transform = (double t) => Transformations.DoubleTransform(fromValue, value, t);
    var callback = (double actValue) => { self.FontSize = actValue; };
    return Transformations.AnimateAsync<double>(self, "AnimateFontSizeTo", transform, callback, length, easing);
}
```

```csharp
await titleLabel.AnimateFontSizeTo(40, 300, Easing.CubicOut);
```

## When to Write Manual Extensions vs. Use the Generator

| Situation | Approach |
|---|---|
| Third-party control with `BindableProperty`s / events | `[MauiMarkup(typeof(...))]` — [the generator](third-party-controls.md) writes all overloads for you |
| Your own `BindableObject` control | Also the generator — annotate it from the app project |
| Plain CLR property (no `BindableProperty`) | Manual extension (only shape 1 is meaningful — no binding support without a `BindableProperty`) |
| Design-system shorthands (`PrimaryText`, `Card`) | Level 1 composition |
| Multi-property convenience (`SizeRequest`-style) | Level 1 composition |
| Custom animations | Level 3 |

## Design Tips

- **Name after the property**, not the action: `CornerRadius(8)`, not `SetCornerRadius(8)` — consistency with the rest of the API.
- Put shared helpers in a dedicated static class per control family (`LabelHelpers`, `LayoutHelpers`) inside a `namespace FmgLib.MauiMarkup` block if you want them to appear with zero extra `using`s — or keep your own namespace for clarity.
- Return `T` (the generic), never the concrete base type, or you'll break chains for derived controls.

## Related Topics

- [Fluent Properties](fluent-properties.md) — the overload contract
- [Third-Party Controls](third-party-controls.md) — letting the generator do this work
- [Animations](animations.md)
