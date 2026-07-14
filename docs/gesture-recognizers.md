# Gesture Recognizers

Any `View` can react to touch and pointer input by adding gesture recognizers with the `GestureRecognizers(...)` method. Every recognizer type has full fluent support — properties and `On…` event methods alike.

Available recognizers (all from MAUI, all fluent-enabled):

- `TapGestureRecognizer`
- `PanGestureRecognizer`
- `PointerGestureRecognizer`
- `SwipeGestureRecognizer`
- `PinchGestureRecognizer`
- `DragGestureRecognizer` / `DropGestureRecognizer`
- `ClickGestureRecognizer`

## Adding Recognizers

`GestureRecognizers` accepts one or more recognizers:

```csharp
new Image()
    .Source("dotnet_bot.png")
    .GestureRecognizers(
        new TapGestureRecognizer().OnTapped((s, e) => Console.WriteLine("tap"))
    )
```

## Tap

`TapGestureRecognizer` detects taps; `NumberOfTapsRequired` selects single/double tap.

```csharp
new StackLayout()
.Children(
    new Label()
        .Text("Tap 2 times on the image")
        .Assign(out var label),

    new Image()
        .Source("dotnet_bot.png")
        .SizeRequest(100, 100)
        .GestureRecognizers(
            new TapGestureRecognizer()
                .NumberOfTapsRequired(2)
                .OnTapped((s, e) => label.Text = "You tapped 2 times")
        )
)
```

Command-style for MVVM (this is how you make any view "clickable"):

```csharp
new Label()
    .Text("See all")
    .TextDecorations(TextDecorations.Underline)
    .GestureRecognizers(
        new TapGestureRecognizer()
            .Command(BindingContext.GotoAllProductsCommand)
    )
```

On MAUI versions supporting it, `Buttons(ButtonsMask.Secondary)` restricts the tap to right-click/secondary input — handy for desktop context actions.

## Pan (drag to move)

`PanGestureRecognizer` streams translation deltas while the finger/mouse moves. Classic "drag the image around" implementation:

```csharp
public class PanGesturePage : ContentPage
{
    double x, y;

    public PanGesturePage()
    {
        this
        .Content(
            new Grid()
            .Children(
                new Image()
                    .Source("dotnet_bot.png")
                    .Assign(out var image)
                    .SizeRequest(100, 100)
                    .GestureRecognizers(
                        new PanGestureRecognizer()
                            .OnPanUpdated((s, args) =>
                            {
                                switch (args.StatusType)
                                {
                                    case GestureStatus.Running:
                                        image.TranslationX = x + args.TotalX;
                                        image.TranslationY = y + args.TotalY;
                                        break;

                                    case GestureStatus.Completed:
                                        x = image.TranslationX;
                                        y = image.TranslationY;
                                        break;
                                }
                            })
                    )
            )
        );
    }
}
```

`args.StatusType` cycles `Started → Running → Completed` (or `Canceled`); `TotalX`/`TotalY` are cumulative from the gesture start, which is why the current translation is cached on completion.

## Pointer (hover / move — desktop & pointer devices)

`PointerGestureRecognizer` reports enter/exit/move, ideal for hover effects on Windows/macOS:

```csharp
public class PointerGesturePage : ContentPage
{
    public PointerGesturePage()
    {
        this
        .Content(
            new StackLayout()
            .Center()
            .Children(
                new Label().Assign(out var posLabel).FontSize(20),
                new Label().Assign(out var stateLabel).FontSize(20).TextColor(Colors.Blue),

                new Image()
                    .Source("dotnet_bot.png")
                    .Assign(out var image)
                    .SizeRequest(300, 300)
                    .GestureRecognizers(
                        new PointerGestureRecognizer()
                            .OnPointerEntered((s, e) => stateLabel.Text = "Entered")
                            .OnPointerExited((s, e) => stateLabel.Text = "Exited")
                            .OnPointerMoved((s, e) =>
                            {
                                var pos = e.GetPosition(relativeTo: image)!.Value;
                                posLabel.Text = $"point: {pos.X:F0}, {pos.Y:F0}";
                            })
                    )
            )
        );
    }
}
```

Simple hover highlight:

```csharp
new Border()
    .Assign(out var card)
    .GestureRecognizers(
        new PointerGestureRecognizer()
            .OnPointerEntered((s, e) => card.BackgroundColor = Colors.LightGray)
            .OnPointerExited((s, e) => card.BackgroundColor = Colors.White)
    )
```

## Swipe

```csharp
new Frame()
    .GestureRecognizers(
        new SwipeGestureRecognizer()
            .Direction(SwipeDirection.Left)
            .Threshold(80)
            .OnSwiped((s, e) => DeleteItem()),
        new SwipeGestureRecognizer()
            .Direction(SwipeDirection.Right)
            .OnSwiped((s, e) => ArchiveItem())
    )
```

> For list rows, prefer `SwipeView` (a full control with revealed action buttons) over raw swipe gestures.

## Pinch (zoom)

```csharp
double currentScale = 1, startScale = 1;

new Image()
    .Source("photo.png")
    .Assign(out var photo)
    .GestureRecognizers(
        new PinchGestureRecognizer()
            .OnPinchUpdated((s, e) =>
            {
                if (e.Status == GestureStatus.Started)
                    startScale = photo.Scale;
                else if (e.Status == GestureStatus.Running)
                {
                    currentScale = Math.Clamp(startScale * e.Scale, 1, 4);
                    photo.Scale = currentScale;
                }
            })
    )
```

## Drag & Drop

```csharp
// Source
new Label()
    .Text("Drag me")
    .GestureRecognizers(
        new DragGestureRecognizer()
            .CanDrag(true)
            .OnDragStarting((s, e) => e.Data.Text = "payload")
    ),

// Target
new Border()
    .GestureRecognizers(
        new DropGestureRecognizer()
            .AllowDrop(true)
            .OnDrop(async (s, e) =>
            {
                var text = await e.Data.GetTextAsync();
                Handle(text);
            })
    )
```

## Tips

- Recognizers work on **any view** — `Label`, `Image`, `Border`, whole layouts. Attaching one to a container makes the entire subtree tappable.
- Combine multiple recognizers in a single `GestureRecognizers(...)` call (it is `params`-friendly, as shown in the swipe example).
- For simple "make it clickable" cases in templates, `TapGestureRecognizer().Command(...)` with `CommandParameter(e => e.Path("."))` passes the tapped item to the view model.

## Related Topics

- [Event Handlers](event-handlers.md)
- [Assign](assign-and-references.md) — capturing views mutated by gestures
