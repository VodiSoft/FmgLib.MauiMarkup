# Behaviors

Behaviors add reusable functionality to controls **without subclassing** them. FmgLib.MauiMarkup attaches them fluently through the `Behaviors(...)` method available on every `VisualElement`.

## Attaching a Behavior

```csharp
new Entry()
    .Text("Click Item")
    .Behaviors(new YourCustomBehavior());
```

Multiple behaviors in one call:

```csharp
new Entry()
    .Placeholder("E-mail")
    .Behaviors(
        new EmailValidationBehavior(),
        new MaxLengthBehavior(64)
    );
```

## Writing a Custom Behavior

A behavior derives from `Behavior<T>` and overrides attach/detach hooks:

```csharp
public class NumericValidationBehavior : Behavior<Entry>
{
    protected override void OnAttachedTo(Entry entry)
    {
        entry.TextChanged += OnTextChanged;
        base.OnAttachedTo(entry);
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        entry.TextChanged -= OnTextChanged;
        base.OnDetachingFrom(entry);
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        bool isValid = double.TryParse(e.NewTextValue, out _);
        ((Entry)sender!).TextColor = isValid ? Colors.Black : Colors.Red;
    }
}
```

Usage:

```csharp
new Entry()
    .Placeholder("Amount")
    .Keyboard(Keyboard.Numeric)
    .Behaviors(new NumericValidationBehavior());
```

## Behavior with Bindable Configuration

Behaviors are `BindableObject`s, so they can expose bindable properties — and since the [source generator](third-party-controls.md) can create fluent methods for *any* `BindableObject`, you can even give your behavior its own fluent API:

```csharp
public class MinLengthBehavior : Behavior<Entry>
{
    public static readonly BindableProperty MinLengthProperty =
        BindableProperty.Create(nameof(MinLength), typeof(int), typeof(MinLengthBehavior), 6);

    public int MinLength
    {
        get => (int)GetValue(MinLengthProperty);
        set => SetValue(MinLengthProperty, value);
    }

    protected override void OnAttachedTo(Entry entry) =>
        entry.TextChanged += (s, e) =>
            entry.BackgroundColor = (e.NewTextValue?.Length ?? 0) >= MinLength
                ? Colors.Transparent
                : Colors.MistyRose;
}

// opt the behavior into fluent generation:
[MauiMarkup(typeof(MinLengthBehavior))]
public class MarkupTargets { }

// then:
new Entry()
    .Behaviors(new MinLengthBehavior().MinLength(8));
```

## Community Toolkit Behaviors

`CommunityToolkit.Maui` ships many ready-made behaviors; they attach exactly the same way:

```csharp
new Entry()
    .Placeholder("E-mail")
    .Behaviors(
        new CommunityToolkit.Maui.Behaviors.EmailValidationBehavior
        {
            InvalidStyle = invalidStyle,
            ValidStyle = validStyle
        }
    );
```

(If you want fluent methods for toolkit behaviors too, register them with `[MauiMarkup(typeof(EmailValidationBehavior))]` — see [Third-Party Controls](third-party-controls.md).)

## Behavior vs. Alternatives

| Need | Best tool |
|---|---|
| Reusable, self-contained control logic (validation, masking, throttling) | **Behavior** |
| One-off event reaction on a single control | [`On<Event>`](event-handlers.md) lambda |
| Declarative property change on a condition | [Triggers](triggers.md) |
| Visual reaction to control states (focused, disabled…) | [Visual States](visual-states.md) |

## Hot Reload Note

Behaviors attach to the controls created inside `Build()`; on [hot reload](hot-reload.md) the controls are rebuilt and behaviors re-attach naturally. Keep behaviors stateless or reinitialize state in `OnAttachedTo` so rebuilds stay safe.

## Related Topics

- [Event Handlers](event-handlers.md)
- [Triggers](triggers.md)
- [Third-Party Controls](third-party-controls.md)
