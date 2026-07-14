# SwipeView — Swipe Actions

`SwipeView` wraps content and reveals action buttons when swiped — the standard "swipe to delete/archive" pattern for list rows. All of it is fluent in FmgLib.MauiMarkup.

## Anatomy

```csharp
new SwipeView()
    .LeftItems(...)      // revealed when swiping right
    .RightItems(...)     // revealed when swiping left
    .TopItems(...)
    .BottomItems(...)
    .Threshold(120)      // swipe distance to trigger
    .Content(view)       // the visible row
```

Each direction takes a `SwipeItems` collection containing `SwipeItem`s (text/icon buttons) or `SwipeItemView`s (arbitrary views).

## Delete / Favorite Row Example

```csharp
new SwipeView()
.RightItems(
    new SwipeItems
    {
        new SwipeItem()
            .Text("Delete")
            .BackgroundColor(Colors.Red)
            .IconImageSource("trash.png")
            .OnInvoked((s, e) => vm.DeleteCommand.Execute(item)),

        new SwipeItem()
            .Text("Favorite")
            .BackgroundColor(Colors.Orange)
            .IconImageSource("star.png")
            .Command(vm.ToggleFavoriteCommand)
    }
)
.Content(
    new Grid()
    .Padding(16)
    .BackgroundColor(Colors.White)
    .Children(
        new Label().Text(e => e.Path("Title")).CenterVertical()
    )
)
```

`SwipeItem` derives from `MenuItem`, so the fluent surface includes `.Text(...)`, `.IconImageSource(...)`, `.BackgroundColor(...)`, `.IsVisible(...)`, `.IsDestructive(...)`, `.Command(...)`/`.CommandParameter(...)` and `.OnInvoked(...)` / `.OnClicked(...)`.

## SwipeItems Behavior

Configure the container fluently too:

```csharp
new SwipeItems
{
    new SwipeItem().Text("Archive").BackgroundColor(Colors.SteelBlue)
}
.Mode(SwipeMode.Execute)              // Execute = run immediately on full swipe
                                      // Reveal (default) = show buttons, tap to run
.SwipeBehaviorOnInvoked(SwipeBehaviorOnInvoked.Close)
```

## Custom Swipe Content — `SwipeItemView`

For fully custom revealed UI:

```csharp
new SwipeItems
{
    new SwipeItemView()
        .Command(vm.ReplyCommand)
        .Content(
            new VerticalStackLayout()
            .Padding(12)
            .BackgroundColor(Colors.MediumSeaGreen)
            .Children(
                new Image().Source("reply.png").SizeRequest(24, 24).CenterHorizontal(),
                new Label().Text("Reply").TextColor(Colors.White).FontSize(12)
            )
        )
}
```

## Inside a CollectionView

The typical placement — as the item template root:

```csharp
new CollectionView()
.ItemsSource(e => e.Path("Messages"))
.ItemTemplate(() =>
    new SwipeView()
    .RightItems(
        new SwipeItems(new[]
        {
            (ISwipeItem)new SwipeItem()
                .Text("Delete")
                .BackgroundColor(Colors.Red)
                .Command(vm.DeleteCommand)
                .CommandParameter(e => e.Path("."))   // the swiped item
        })
    )
    .Content(
        new Grid().Padding(16).Children(
            new Label().Text(e => e.Path("Subject"))
        )
    )
)
```

> Give the swipe **content an opaque `BackgroundColor`** — with transparent content the revealed items show through and the row looks broken.

## Events

```csharp
new SwipeView()
    .OnSwipeStarted((s, e) => Console.WriteLine(e.SwipeDirection))
    .OnSwipeChanging((s, e) => Console.WriteLine(e.Offset))
    .OnSwipeEnded((s, e) => Console.WriteLine(e.IsOpen))
    .OnOpenRequested((s, e) => { })
    .OnCloseRequested((s, e) => { })
```

Programmatic open/close via a captured reference:

```csharp
new SwipeView().Assign(out var swipe) /* ... */;
swipe.Open(OpenSwipeItem.RightItems);
swipe.Close();
```

## SwipeView vs. SwipeGestureRecognizer

| | `SwipeView` | [`SwipeGestureRecognizer`](gesture-recognizers.md#swipe) |
|---|---|---|
| Reveals action UI | ✅ | ❌ (event only) |
| Follows the finger | ✅ | ❌ |
| Use for | List row actions | Simple directional gestures (dismiss, navigate) |

## Related Topics

- [Collections & Templates](collections-and-templates.md)
- [Gesture Recognizers](gesture-recognizers.md)
- [Menus](menus.md) — `MenuItem` base of `SwipeItem`
