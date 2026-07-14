# Triggers

Triggers change properties declaratively in response to conditions or events — no event-handler code needed. FmgLib.MauiMarkup gives fluent builders for all MAUI trigger types:

| Trigger | Fires when |
|---|---|
| `Trigger` (property trigger) | A property on the same control reaches a value |
| `DataTrigger` | A **binding** reaches a value |
| `EventTrigger` | An event fires (runs `TriggerAction`s) |
| `MultiTrigger` | Several conditions are all true |
| State triggers (`AdaptiveTrigger`, `CompareStateTrigger`, `DeviceStateTrigger`, `OrientationStateTrigger`, `StateTrigger`) | Used inside [Visual States](visual-states.md) |

Triggers can be attached to a control via `.Triggers(...)`, or placed inside a [`Style<T>`](styling.md).

## Property Triggers

React to the control's own property. Example: highlight an `Entry` while it is focused — defined once in a style, applied to every `Entry` on the page:

```csharp
using FmgLib.MauiMarkup;

public class PropertyTriggerPage : ContentPage
{
    public PropertyTriggerPage()
    {
        this
        .Resources(
            new ResourceDictionary
            {
                new Style<Entry>
                {
                    Entry.BackgroundColorProperty.Set(Colors.Black),
                    Entry.TextColorProperty.Set(Colors.White),

                    new Trigger(typeof(Entry))
                        .Property(Entry.IsFocusedProperty)
                        .Value(true)
                        .Setters(
                            new Setters<Entry>(e => e
                                .BackgroundColor(Colors.Yellow)
                                .TextColor(Colors.Black))
                        ),
                }
            }
        )
        .Content(
            new StackLayout()
            .Children(
                new Entry().Placeholder("Enter name"),
                new Entry().Placeholder("Enter password"),
                new Entry().Placeholder("Enter address")
            )
        );
    }
}
```

Pieces worth noting:

- `SomeProperty.Set(value)` — an extension on `BindableProperty` that creates a `Setter`; used for the style's base values.
- `new Trigger(typeof(Entry)).Property(...).Value(...)` — the trigger condition.
- `Setters(new Setters<Entry>(e => e...))` — the strongly-typed setter builder; **the same fluent property methods work inside it**.

When the property leaves the trigger value, the original style values are restored automatically (standard MAUI trigger semantics).

## Data Triggers

React to a **binding** — another control, or the view model. Example: disable the Save button while the entry is empty:

```csharp
this.Content(
    new StackLayout()
    .Children(
        new Entry().Assign(out var entry).Placeholder("Enter text..."),

        new Button()
            .Text("Save")
            .Triggers(
                new DataTrigger(typeof(Button))
                    .Binding(e => e.Path("Text.Length").Source(entry))
                    .Value(0)
                    .Setters(new Setters<Button>(e => e.IsEnabled(false)))
            )
    )
);
```

`Binding(...)` accepts the full [property-builder syntax](data-binding.md), so view-model conditions work too:

```csharp
new Label()
    .Text("Offline mode")
    .IsVisible(false)
    .Triggers(
        new DataTrigger(typeof(Label))
            .Binding(e => e.Path("IsConnected"))
            .Value(false)
            .Setters(new Setters<Label>(e => e.IsVisible(true)))
    )
```

## Event Triggers

Run a `TriggerAction` when an event fires. Example: validate numeric input on every keystroke:

```csharp
this.Content(
    new StackLayout()
    .Children(
        new Entry()
            .Placeholder("Enter a number...")
            .Triggers(
                new EventTrigger()
                    .Event("TextChanged")
                    .Actions(new NumericValidationTriggerAction())
            )
    )
);
```

The action class:

```csharp
public class NumericValidationTriggerAction : TriggerAction<Entry>
{
    protected override void Invoke(Entry entry)
    {
        bool isValid = double.TryParse(entry.Text, out _);
        entry.TextColor = isValid ? Colors.Black : Colors.Red;
    }
}
```

> Event triggers don't revert anything automatically — the action decides what to do each time the event fires.

## Multi Triggers

All conditions must hold. Conditions are built with `PropertyCondition` / `BindingCondition`:

```csharp
new Entry().Assign(out var email),
new Entry().Assign(out var phone),

new Button()
    .Text("Submit")
    .IsEnabled(false)
    .Triggers(
        new MultiTrigger(typeof(Button))
            .Conditions(
                new BindingCondition()
                    .Binding(e => e.Path("Text.Length").Source(email))
                    .Value(0),
                new BindingCondition()
                    .Binding(e => e.Path("Text.Length").Source(phone))
                    .Value(0)
            )
            .Setters(new Setters<Button>(e => e.IsEnabled(true)))
    )
```

*(This example enables the button only when both entries are empty — invert values to match your validation logic.)*

## Triggers in Styles vs. on Controls

- **On a control** (`.Triggers(...)`): affects that instance only.
- **In a `Style<T>`** (added into the style's collection initializer, as in the first example): applies to every control the style targets — the DRY option for app-wide behavior. See [Styling](styling.md).

## Triggers vs. Alternatives

| Scenario | Recommended |
|---|---|
| Visual response to focus/press/disabled state across the app | Property trigger in a style, or [Visual States](visual-states.md) |
| Enable/disable driven by view-model state | `DataTrigger`, or bind `IsEnabled` with a [converter](binding-converters.md) |
| Complex validation logic | [Behavior](behaviors.md) or view-model logic |
| One-off event response | [`On<Event>`](event-handlers.md) |

## Related Topics

- [Visual States](visual-states.md) — state triggers, `AdaptiveTrigger` for responsive layouts
- [Styling](styling.md)
- [Property Bindings](data-binding.md)
