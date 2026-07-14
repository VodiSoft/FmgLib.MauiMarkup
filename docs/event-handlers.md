# Event Handlers

For **every event** on every control, FmgLib.MauiMarkup generates a fluent `On<EventName>` method — the C# equivalent of XAML's `Clicked="OnCounterClicked"` attributes, but attached inline, with two convenient shapes.

## The Two Shapes

Using `Button.Clicked` as the example, the generator emits:

```csharp
// 1. Classic event-handler signature
public static T OnClicked<T>(this T self, EventHandler handler) where T : Button;

// 2. Simplified action receiving the (typed!) sender only
public static T OnClicked<T>(this T self, Action<T> action) where T : Button;
```

Shape 2 is the everyday one: no unused `object sender, EventArgs e` boilerplate, and the parameter is already the concrete control type.

## Method-Group Style

```csharp
using FmgLib.MauiMarkup;

public class ExamPage : ContentPage
{
    int count = 0;

    public ExamPage()
    {
        this
        .Content(
            new VerticalStackLayout()
            .Children(
                new Button()
                    .Text("Click me")
                    .OnClicked(OnCounterClicked)
            )
        );
    }

    private void OnCounterClicked(Button sender)
    {
        count++;
        sender.Text = $"Clicked {count} ";
        sender.Text += count == 1 ? "time" : "times";
    }
}
```

## Inline Lambda Style

```csharp
new Button()
    .Text("Click me")
    .OnClicked(button =>
    {
        count++;
        button.Text = $"Clicked {count} ";
        button.Text += count == 1 ? "time" : "times";
    })
```

## When You Need the Event Args

Use the classic shape — the full `EventArgs` are available:

```csharp
new Entry()
    .OnTextChanged((sender, e) =>
    {
        Console.WriteLine($"'{e.OldTextValue}' → '{e.NewTextValue}'");
    })

new CollectionView()
    .OnSelectionChanged((sender, e) =>
    {
        var selected = e.CurrentSelection.FirstOrDefault();
        if (selected is Product p) ShowDetail(p);
    })
```

## Common Events Cheat Sheet

| Control | Fluent method | Typical use |
|---|---|---|
| `Button` | `OnClicked`, `OnPressed`, `OnReleased` | Actions |
| `Entry` / `Editor` | `OnTextChanged`, `OnCompleted`, `OnFocused`, `OnUnfocused` | Validation, search-as-you-type |
| `CheckBox` | `OnCheckedChanged` | Toggles |
| `Switch` | `OnToggled` | Settings |
| `Slider` | `OnValueChanged`, `OnDragCompleted` | Ranges |
| `Picker` | `OnSelectedIndexChanged` | Selection |
| `CollectionView` | `OnSelectionChanged`, `OnScrolled`, `OnRemainingItemsThresholdReached` | Lists, infinite scroll |
| `RefreshView` | `OnRefreshing` | Pull to refresh |
| `ContentPage` | `OnAppearing`, `OnDisappearing`, `OnLoaded`, `OnUnloaded`, `OnNavigatedTo` | Lifecycle |
| `WebView` | `OnNavigating`, `OnNavigated` | Web content |

(Any event not listed follows the same `On<EventName>` naming.)

## Page Lifecycle Inline

Because pages are `BindableObject`s too, lifecycle wiring can live in the same fluent chain:

```csharp
public void Build() =>
    this
    .OnAppearing(async page => await ViewModel.RefreshAsync())
    .Content(/* ... */);
```

## Events vs. Commands

Both work; pick by architecture:

```csharp
// Event style — page-local logic
new Button().Text("Save").OnClicked(async b => await SaveAsync())

// Command style — MVVM
new Button()
    .Text("Save")
    .Command(e => e.Path("SaveCommand"))
    .CommandParameter(e => e.Path("."))
```

A pragmatic middle ground when you hold a typed view-model reference (e.g. with [`FmgLibContentPage<TViewModel>`](hot-reload.md)):

```csharp
new Button().Text("Save").Command(BindingContext.SaveCommand)
```

## Unsubscription & Hot Reload

`On<Event>` subscribes with `+=`. Two consequences:

- **Rebuilding the UI creates new controls**, so old subscriptions die with the old controls — no leak in the typical [hot reload](hot-reload.md) `Build()` flow.
- If you attach handlers to **long-lived objects** (e.g. `Application.Current`, a static service) inside `Build()`, you *will* stack up subscriptions on every hot reload. Attach those in the constructor instead, or unsubscribe first via [`InvokeOnElement`](assign-and-references.md).

## Custom / Third-Party Events

The source generator produces the same two `On…` shapes for events on [third-party controls](third-party-controls.md):

```csharp
[MauiMarkup(typeof(ZXing.Net.Maui.Controls.CameraBarcodeReaderView))]
class Markup { }

// generated: OnBarcodesDetected(handler) and OnBarcodesDetected(Action<T>)
new CameraBarcodeReaderView()
    .OnBarcodesDetected((s, e) => Process(e.Results))
```

## Related Topics

- [Gesture Recognizers](gesture-recognizers.md) — tap/pan/pointer events on arbitrary views
- [Triggers](triggers.md) — declarative reactions without code
- [Hot Reload](hot-reload.md)
