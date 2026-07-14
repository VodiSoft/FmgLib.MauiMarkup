# Application Styling

FmgLib.MauiMarkup replaces XAML `<Style>` elements with the strongly-typed **`Style<T>`** class. Inside a style, **the same fluent property methods you use on controls** define setters — one API to learn, two contexts to use it in.

## Defining a Style

```csharp
new Style<Button>(e => e
    .TextColor(e => e.OnLight(Colors.White).OnDark(AppColors.Primary))
    .BackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(Colors.White))
    .FontFamily("OpenSansRegular")
    .FontSize(14)
    .CornerRadius(8)
    .Padding(new Thickness(14, 10))
    .MinimumHeightRequest(44)
    .MinimumWidthRequest(44))
```

The lambda receives a `SettersContext<Button>`; each call adds a `Setter` to the style. Theme (`OnLight`/`OnDark`), platform, idiom and `DynamicResource` builders all work inside setters, exactly as on live controls ([Fluent Properties](fluent-properties.md)).

`Style<T>` converts implicitly to `Microsoft.Maui.Controls.Style`, so it drops into any place MAUI expects a style.

## Applying Styles

### Implicit — via ResourceDictionary

A style added to resources applies to **all controls of the target type** in scope:

```csharp
// App-wide (App.cs)
this.Resources(
    new ResourceDictionary
    {
        new Style<Button>(e => e.BackgroundColor(AppColors.Primary).CornerRadius(8)),
        new Style<Frame>(e => e
            .HasShadow(false)
            .BorderColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray950))
            .CornerRadius(8)),
        new Style<Label>(e => e.TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.White))),
    }
);
```

Page-level and even layout-level `Resources(...)` scoping works the same as XAML.

### Explicit — per control

```csharp
var dangerButton = new Style<Button>(e => e
    .BackgroundColor(Colors.Red)
    .TextColor(Colors.White));

new Button().Text("Delete").Style(dangerButton)
```

A common organization pattern — a static styles class:

```csharp
public static class AppStyles
{
    public static Style<Button> Primary { get; } = new(e => e
        .BackgroundColor(AppColors.Primary)
        .TextColor(Colors.White)
        .CornerRadius(8));

    public static Style<Button> Secondary { get; } = new(e => e
        .BackgroundColor(Colors.Transparent)
        .TextColor(AppColors.Primary));

    public static ResourceDictionary Default { get; } = new()
    {
        Primary, Secondary,
        new Style<Entry>(e => e.FontSize(16)),
    };
}

// in App:
this.Resources(new ResourceDictionary().MergedDictionaries(AppStyles.Default));

// per control:
new Button().Text("Save").Style(AppStyles.Primary)
```

## Constructor Options

`Style<T>` has constructors mirroring XAML style features:

```csharp
// inheritance
var baseText  = new Style<Label>(e => e.FontFamily("OpenSansRegular").FontSize(14));
var heading   = new Style<Label>(baseText, e => e.FontSize(24).FontAttributes(FontAttributes.Bold));

// apply to derived types (e.g. style Button and its subclasses)
var allButtons = new Style<Button>(applyToDerivedTypes: true, e => e.CornerRadius(8));

// both
var special = new Style<Button>(basedOn: allButtons, applyToDerivedTypes: true, e => e.FontSize(16));
```

| Constructor parameter | XAML equivalent |
|---|---|
| `basedOn` | `BasedOn="{StaticResource …}"` |
| `applyToDerivedTypes` | `ApplyToDerivedTypes="True"` |
| `buildSetters` lambda | the `<Setter>` list |

## Adding Triggers, Visual States and Actions

`Style<T>` supports collection-initializer syntax; you can add:

- `Setter` objects (via `SomeProperty.Set(value)`)
- `TriggerBase` objects ([Triggers](triggers.md))
- `VisualStateGroup` / `VisualState<T>` objects ([Visual States](visual-states.md))
- `Action<T>` — arbitrary code run against each styled control

```csharp
new ResourceDictionary
{
    new Style<Button>(e => e
        .TextColor(e => e.OnLight(Colors.White).OnDark(AppColors.Primary))
        .BackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(Colors.White))
        .FontSize(14)
        .CornerRadius(8))
    {
        new VisualState<Button>(VisualStates.Button.Normal, e => e
            .TextColor(e => e.OnLight(Colors.White).OnDark(AppColors.Primary))
            .BackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(Colors.White))),

        new VisualState<Button>(VisualStates.Button.Disabled, e => e
            .TextColor(e => e.OnLight(AppColors.Gray950).OnDark(AppColors.Gray200))
            .BackgroundColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray600))),
    },
};
```

Mixing raw setters and triggers in the initializer form:

```csharp
new Style<Entry>
{
    Entry.BackgroundColorProperty.Set(Colors.Black),
    Entry.TextColorProperty.Set(Colors.White),

    new Trigger(typeof(Entry))
        .Property(Entry.IsFocusedProperty)
        .Value(true)
        .Setters(new Setters<Entry>(e => e
            .BackgroundColor(Colors.Yellow)
            .TextColor(Colors.Black))),
}
```

## Theming Strategies

Three complementary tools; use them together:

1. **`OnLight`/`OnDark` in setters** — automatic OS theme response:

   ```csharp
   new Style<Label>(e => e.TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.White)))
   ```

2. **Dynamic resources** — user-selectable themes at runtime:

   ```csharp
   new Style<Button>(e => e.BackgroundColor(e => e.DynamicResource("AccentColor")))

   // theme switch:
   Application.Current!.Resources["AccentColor"] = Colors.Purple;
   ```

3. **Merged dictionaries** — swap whole style sets:

   ```csharp
   this.Resources(new ResourceDictionary().MergedDictionaries(
       isCompact ? CompactStyles.Default : ComfortableStyles.Default));
   ```

## Custom Extension Methods in Styles

Your own fluent methods participate in styles if you implement the `SettersContext<T>` overloads — the [Custom Extension Methods](custom-extension-methods.md) page shows the full four-overload template:

```csharp
new Style<Label>(e => e
    .FontSize(20)          // built-in
    .MyBrandTypography())  // yours
```

## Related Topics

- [Visual States](visual-states.md)
- [Triggers](triggers.md)
- [Fluent Properties](fluent-properties.md)
- [Custom Extension Methods](custom-extension-methods.md)
