# FmgLib.MauiMarkup — Styling & Theming Reference

## `Style<T>`

The strongly-typed replacement for `<Style>`. **The same fluent property methods** define setters, so
there is one API to learn and two contexts to use it in.

```csharp
new Style<Button>(e => e
    .TextColor(e => e.OnLight(Colors.White).OnDark(AppColors.Primary))
    .BackgroundColor(AppColors.Primary)
    .FontFamily("OpenSansRegular")
    .FontSize(14)
    .CornerRadius(8)
    .Padding(new Thickness(14, 10))
    .MinimumHeightRequest(44))
```

`Style<T>` converts implicitly to `Microsoft.Maui.Controls.Style`, so it fits anywhere MAUI expects one.

### Constructors

| Parameter | XAML equivalent |
|---|---|
| `basedOn:` | `BasedOn="{StaticResource …}"` |
| `applyToDerivedTypes:` | `ApplyToDerivedTypes="True"` |
| the `buildSetters` lambda | the `<Setter>` list |

```csharp
var baseText = new Style<Label>(e => e.FontFamily("OpenSansRegular").FontSize(14));
var heading  = new Style<Label>(baseText, e => e.FontSize(24).FontAttributes(FontAttributes.Bold));
var buttons  = new Style<Button>(applyToDerivedTypes: true, e => e.CornerRadius(8));
var special  = new Style<Button>(basedOn: buttons, applyToDerivedTypes: true, e => e.FontSize(16));
```

### Applying

Implicit — every control of the target type in scope:

```csharp
this.Resources(new ResourceDictionary
{
    new Style<Button>(e => e.BackgroundColor(AppColors.Primary).CornerRadius(8)),
    new Style<Label>(e => e.TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.White))),
});
```

Explicit — one control: `new Button().Text("Delete").Style(dangerButton)`.

Page-level and layout-level `.Resources(...)` scope exactly like XAML.

### Recommended organization

```csharp
public static class AppStyles
{
    public static Style<Button> Primary { get; } = new(e => e
        .BackgroundColor(AppColors.Primary).TextColor(Colors.White).CornerRadius(8));

    public static ResourceDictionary Default { get; } = new()
    {
        Primary,
        new Style<Entry>(e => e.FontSize(16)),
    };
}

// App:
this.Resources(new ResourceDictionary().MergedDictionaries(AppStyles.Default));
```

Pair it with a static `AppColors` class — design tokens in one place, refactorable, IntelliSense-able.

### Collection-initializer contents

A `Style<T>` initializer accepts `Setter`s (`SomeProperty.Set(value)`), `TriggerBase`s,
`VisualStateGroup` / `VisualState<T>` objects, and `Action<T>` entries run against each styled control.

```csharp
new Style<Entry>
{
    Entry.BackgroundColorProperty.Set(Colors.Black),
    Entry.TextColorProperty.Set(Colors.White),

    new Trigger(typeof(Entry))
        .Property(Entry.IsFocusedProperty)
        .Value(true)
        .Setters(new Setters<Entry>(e => e.BackgroundColor(Colors.Yellow).TextColor(Colors.Black))),
}
```

> Trailing fluent calls must come **after** the `{ … }` block:
> `new MenuFlyoutSubItem() { /* items */ }.Text("Submenu")`.

## Visual states

```csharp
new Style<Button>(e => e.FontSize(14).CornerRadius(8))
{
    new VisualState<Button>(VisualStates.Button.Normal, e => e
        .TextColor(e => e.OnLight(Colors.White).OnDark(AppColors.Primary))
        .BackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(Colors.White))),

    new VisualState<Button>(VisualStates.Button.PointerOver, e => e
        .BackgroundColor(AppColors.PrimaryDark)),

    new VisualState<Button>(VisualStates.Button.Disabled, e => e
        .TextColor(AppColors.Gray950).BackgroundColor(AppColors.Gray200)),
}
```

**Always define `Normal`.** The VSM only restores properties some state sets.

### State-name constants — never magic strings

| Class | Constants |
|---|---|
| `VisualStates.VisualElement` | `Normal`, `Disabled`, `Focused`, `PointerOver` |
| `VisualStates.Button` | + `Pressed` |
| `VisualStates.ImageButton` | + `Pressed` |
| `VisualStates.Switch` | + `On`, `Off` |
| `VisualStates.RadioButton` | + `Checked`, `Unchecked` |
| `VisualStates.CheckBox` | + `IsChecked` |
| `VisualStates.CollectionView` | + `Selected` |
| `VisualStates.CarouselView` | + `DefaultItem`, `CurrentItem`, `PreviousItem`, `NextItem` |

### Directly on a control

```csharp
new Entry().VisualStateGroups(new VisualStateGroupList
{
    new VisualState<Entry>(VisualStates.VisualElement.Normal,  e => e.BackgroundColor(Colors.White)),
    new VisualState<Entry>(VisualStates.VisualElement.Focused, e => e.BackgroundColor(Colors.LightYellow)),
})
```

States written straight into a `VisualStateGroupList` land in `CommonStates`. For your own group:

```csharp
new VisualStateGroupList
{
    new VisualStateGroup().Name("SelectionStates").States(
        new VisualState<Grid>("Unselected", e => e.BackgroundColor(Colors.White)),
        new VisualState<Grid>("Selected",   e => e.BackgroundColor(Colors.LightBlue)))
}
```

`VisualStateGroup` has no collection-initializer support — use `VisualStateGroupList` or `.States(...)`.

### Animations on state entry

`VisualState<T>` accepts `Action<T>` entries that run when the state is entered:

```csharp
new VisualState<Button>(VisualStates.Button.Pressed) { async b => await b.ScaleTo(0.96, 80) },
new VisualState<Button>(VisualStates.Button.Normal)  { async b => await b.ScaleTo(1, 80) },
```

### State triggers — adaptive layout

```csharp
new VisualStateGroupList
{
    new VisualState<Grid>("Wide",   e => e.ColumnSpacing(24)) { new AdaptiveTrigger().MinWindowWidth(800) },
    new VisualState<Grid>("Narrow", e => e.ColumnSpacing(8))  { new AdaptiveTrigger().MinWindowWidth(0) },
}
```

| Trigger | Activates when |
|---|---|
| `AdaptiveTrigger` | `MinWindowWidth` / `MinWindowHeight` crossed |
| `CompareStateTrigger` | bound `Property` equals `Value` |
| `DeviceStateTrigger` | running on the given `Device` |
| `OrientationStateTrigger` | orientation matches |
| `StateTrigger` | `IsActive` set manually |

`VisualStateManager.GoToState(control, "CustomState")` still works for hand-driven states.

## Triggers

| Type | Fires when |
|---|---|
| `Trigger` | a property on the same control reaches a value |
| `DataTrigger` | a **binding** reaches a value |
| `EventTrigger` | an event fires (runs `TriggerAction`s) |
| `MultiTrigger` | all conditions hold |

```csharp
new Button()
    .Text("Save")
    .Triggers(
        new DataTrigger(typeof(Button))
            .Binding(e => e.Path("Text.Length").Source(entry))
            .Value(0)
            .Setters(new Setters<Button>(e => e.IsEnabled(false))))
```

```csharp
new MultiTrigger(typeof(Button))
    .Conditions(
        new BindingCondition().Binding(e => e.Path("Text.Length").Source(email)).Value(0),
        new BindingCondition().Binding(e => e.Path("Text.Length").Source(phone)).Value(0))
    .Setters(new Setters<Button>(e => e.IsEnabled(true)))
```

`Setters(new Setters<T>(e => e…))` is the strongly-typed setter builder — the same fluent property
methods again. Event triggers revert nothing automatically; a `TriggerAction<T>` decides each time.

On a control (`.Triggers(...)`) affects one instance; inside a `Style<T>` it applies app-wide.

## Theming — three tools, use together

1. **`OnLight` / `OnDark`** produce a real `AppThemeBinding`. The value follows the OS theme *and*
   `Application.Current.UserAppTheme` at runtime; controls already on screen repaint themselves. No
   page rebuild, no dictionary swap. (Before 10.2.1 this was resolved once at build time.)

   ```csharp
   new Style<Label>(e => e.TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.White)))
   ```

   A ternary (`.TextColor(isDark ? a : b)`) is evaluated **once** and will not follow theme changes —
   this is the most common theming bug.

2. **`DynamicResource`** for user-selectable themes:

   ```csharp
   new Style<Button>(e => e.BackgroundColor(e => e.DynamicResource("AccentColor")));
   Application.Current!.Resources["AccentColor"] = Colors.Purple;
   ```

3. **Merged dictionaries** to swap whole style sets:

   ```csharp
   this.Resources(new ResourceDictionary().MergedDictionaries(
       isCompact ? CompactStyles.Default : ComfortableStyles.Default));
   ```

### Nested builders resolve once

```csharp
.TextColor(e => e.OnLight(Colors.Black).OnDark(l => l.DynamicResource("DarkAccent")))
```

A nested builder cannot be carried by a theme binding, so this form is resolved against the theme in
effect while the page is built and does **not** follow later theme changes. Where the theme must switch
at runtime, keep the plain value form.

## Choosing the right reaction tool

| Situation | Tool |
|---|---|
| Constant | direct value |
| View-model driven | binding (`Getter` preferred) |
| Interaction feedback (focus/press/disabled), adaptive layout | visual states |
| Condition-driven property change | trigger |
| Reusable control logic | behavior |
| One-off | event handler |
