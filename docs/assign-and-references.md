# Object References — `Assign`, `InvokeOnElement`, `RegisterName`

In XAML you name elements with `x:Name` and reach them from code-behind. In FmgLib.MauiMarkup you capture references **inline, while building the tree**, without breaking the fluent chain.

## `Assign(out var …)` — the `x:Name` replacement

`Assign` stores the current control in a variable and returns the control, so the chain continues:

```csharp
public static T Assign<T>(this T self, out T obj) where T : BindableObject;
```

### Capture into a local variable

```csharp
new Label().Assign(out var label);
new Entry().Assign(out var entry);
```

### Capture into a field

```csharp
Button btnOk;

new Button()
    .Text("OK")
    .Assign(out btnOk);
```

### Typical use — one control referencing another

The most common pattern: a control declared earlier in the tree is used by a binding or an event handler of a later control.

```csharp
this.Content(
    new StackLayout()
    .Children(
        new Slider()
            .Assign(out var slider)
            .Minimum(1)
            .Maximum(20),

        new Label()
            // bind this label's Text to the slider captured above
            .Text(e => e.Path("Value").Source(slider).StringFormat("Slider value: {0}"))
            .FontSize(28)
    )
);
```

Or event handlers mutating a sibling:

```csharp
new StackLayout()
.Children(
    new Label().Text("Tap the image twice").Assign(out var label),

    new Image()
        .Source("dotnet_bot.png")
        .SizeRequest(100, 100)
        .GestureRecognizers(
            new TapGestureRecognizer()
                .NumberOfTapsRequired(2)
                .OnTapped((s, e) => label.Text = "You tapped 2 times!")
        )
)
```

### Ordering caveat

`out var` variables must be *declared before use* in C#. If a control earlier in the tree needs a reference to a control that appears later, declare the variable up front:

```csharp
Button submit = null!;

this.Content(
    new VerticalStackLayout()
    .Children(
        new Entry()
            .Placeholder("Name")
            .OnTextChanged((s, e) => submit.IsEnabled = !string.IsNullOrEmpty(e.NewTextValue)),

        new Button()
            .Text("Submit")
            .IsEnabled(false)
            .Assign(out submit)      // assigns to the pre-declared variable
    )
);
```

> **Hot reload tip:** if a page rebuilds via `Build()` ([Hot Reload](hot-reload.md)), fields captured with `Assign` are reassigned on every rebuild — locals declared inside `Build()` are the safest choice.

## `InvokeOnElement` — arbitrary code mid-chain

`InvokeOnElement` runs any action against the control and continues the chain. Use it for the rare API that has no fluent method (non-bindable properties, method calls, event subscriptions with custom logic):

```csharp
new ActivityIndicator()
    .IsRunning(true)
    .SizeRequest(70, 70)
    .Center()
    .InvokeOnElement(ai => ai.Loaded += (s, e) => CheckLogin())
```

```csharp
new CollectionView()
    .ItemsSource(items)
    .InvokeOnElement(cv => cv.ScrollTo(items.Count - 1, position: ScrollToPosition.End))
```

It is also the standard way to run **conditional configuration** inline:

```csharp
new Button()
    .Text("Buy")
    .InvokeOnElement(b =>
    {
        if (user.IsPremium)
            b.BackgroundColor = Colors.Gold;
    })
```

## `RegisterName` — the literal `x:Name` equivalent

For advanced scenarios that need MAUI's name scope (e.g. certain animations or `FindByName` interop), `RegisterName` registers the control in the name scope of a given root element:

```csharp
this.Content(
    new StackLayout()
        .Assign(out var root)
        .Children(
            new Label()
                .Text("Named element")
                .RegisterName("myLabel", root)
        )
);

// later
var label = (Label)((INameScope)NameScope.GetNameScope(root)).FindByName("myLabel");
```

In everyday code you will not need this — `Assign` covers 99% of cases with compile-time safety.

## Choosing Between Them

| Need | Use |
|---|---|
| Reference a control elsewhere in the same page | `Assign(out var x)` |
| Store a control in a field for later use | `Assign(out _field)` |
| Call a method / set a non-bindable property inline | `InvokeOnElement(x => …)` |
| Conditional setup inside the chain | `InvokeOnElement` |
| MAUI name-scope interop | `RegisterName` |

## Related Topics

- [Property Bindings](data-binding.md) — `Source(control)` uses `Assign`ed references
- [Hot Reload](hot-reload.md) — lifecycle implications for captured references
