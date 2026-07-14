# Visual States

The Visual State Manager changes a control's appearance based on its state — `Normal`, `Focused`, `Disabled`, `PointerOver`, `Pressed`, and so on. FmgLib.MauiMarkup wraps it with the strongly-typed **`VisualState<T>`** class, which plugs into styles or directly onto controls, and even supports **state-entry animations**.

## Defining Visual States

A `VisualState<T>` takes the state name and a setters lambda — the same fluent property API used everywhere else:

```csharp
new VisualState<Button>(VisualStates.Button.Normal, e => e
    .TextColor(Colors.White)
    .BackgroundColor(AppColors.Primary))
```

### Built-in state names — the `VisualStates` helper

Instead of magic strings, use the constants class shipped with the library:

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

(Each control class inherits the common `VisualElement` states, so `VisualStates.Button.Focused` is also valid.)

## Visual States in a Style

The most common placement — inside a [`Style<T>`](styling.md) collection initializer, applying app-wide:

```csharp
new Style<Button>(e => e
    .FontSize(14)
    .CornerRadius(8))
{
    new VisualState<Button>(VisualStates.Button.Normal, e => e
        .TextColor(e => e.OnLight(Colors.White).OnDark(AppColors.Primary))
        .BackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(Colors.White))),

    new VisualState<Button>(VisualStates.Button.PointerOver, e => e
        .BackgroundColor(AppColors.PrimaryDark)),

    new VisualState<Button>(VisualStates.Button.Disabled, e => e
        .TextColor(e => e.OnLight(AppColors.Gray950).OnDark(AppColors.Gray200))
        .BackgroundColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray600))),
}
```

> **Always define `Normal`.** The VSM only restores properties that some state sets; defining `Normal` explicitly guarantees a clean return from other states.

## Visual States Directly on a Control

Use the `VisualStateGroups` attached-property method:

```csharp
new Entry()
    .Placeholder("E-mail")
    .VisualStateGroups(
        new VisualStateGroup
        {
            new VisualState<Entry>(VisualStates.VisualElement.Normal, e => e
                .BackgroundColor(Colors.White)),
            new VisualState<Entry>(VisualStates.VisualElement.Focused, e => e
                .BackgroundColor(Colors.LightYellow)),
        }
    )
```

## Animations Inside Visual States

`VisualState<T>` accepts `Action<T>` entries in its collection initializer — they run **when the state is entered**, so async MAUI animations become state transitions:

```csharp
new Style<Button>(e => e.FontSize(20))
{
    new VisualState<Button>(VisualStates.Button.Normal, e => e
        .FontSize(33)
        .TextColor(AppColors.Gray200))
    {
        async button => {
            await button.RotateTo(0);     // animate on entering Normal
        }
    },

    new VisualState<Button>(VisualStates.Button.Disabled, e => e
        .FontSize(20)
        .TextColor(AppColors.Gray600))
    {
        async button => {
            await button.RotateTo(180);   // animate on entering Disabled
        }
    },
}
```

Combine with the library's generated [`Animate…To` helpers](animations.md) for property-level animations:

```csharp
new VisualState<Button>(VisualStates.Button.PointerOver)
{
    async b => await b.AnimateBackgroundColorTo(Colors.DarkSlateBlue, length: 150)
}
```

## State Triggers — states driven by conditions

A `VisualState<T>` can also contain **state triggers** instead of being driven by control interaction. This enables responsive/adaptive layouts:

```csharp
new VisualStateGroup
{
    new VisualState<Grid>("Wide", e => e.BackgroundColor(Colors.White))
    {
        new AdaptiveTrigger().MinWindowWidth(800)
    },
    new VisualState<Grid>("Narrow", e => e.BackgroundColor(Colors.WhiteSmoke))
    {
        new AdaptiveTrigger().MinWindowWidth(0)
    },
}
```

Available fluent-enabled state triggers:

| Trigger | Activates when |
|---|---|
| `AdaptiveTrigger` | Window size crosses `MinWindowWidth`/`MinWindowHeight` |
| `CompareStateTrigger` | A bound `Property` equals `Value` |
| `DeviceStateTrigger` | Running on a given `Device` (platform) |
| `OrientationStateTrigger` | Device orientation matches |
| `StateTrigger` | `IsActive` is set (manual control) |

Example — orientation-dependent layout:

```csharp
new VisualStateGroup
{
    new VisualState<StackLayout>("Portrait", e => e.Orientation(StackOrientation.Vertical))
    {
        new OrientationStateTrigger().Orientation(DisplayOrientation.Portrait)
    },
    new VisualState<StackLayout>("Landscape", e => e.Orientation(StackOrientation.Horizontal))
    {
        new OrientationStateTrigger().Orientation(DisplayOrientation.Landscape)
    },
}
```

## Programmatic State Changes

Standard MAUI applies:

```csharp
VisualStateManager.GoToState(myButton, "CustomState");
```

Custom state names work fine — define a `VisualState<T>` with your own name and trigger it from code.

## Visual States vs. Triggers

| | Visual states | [Triggers](triggers.md) |
|---|---|---|
| Driven by | Named control states (+ state triggers) | Property values / bindings / events |
| Mutually exclusive | Yes, within a group | No |
| Animation support | Yes (action entries) | Via `EventTrigger` actions |
| Best for | Interaction feedback, adaptive layout | Data-driven property changes |

## Related Topics

- [Styling](styling.md)
- [Animations](animations.md)
- [Triggers](triggers.md)
