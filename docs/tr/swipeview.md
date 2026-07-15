# SwipeView — Kaydırma Aksiyonları

`SwipeView`, içeriği sarar ve kaydırıldığında aksiyon butonlarını açığa çıkarır — liste satırları için standart "kaydırıp sil/arşivle" deseni. FmgLib.MauiMarkup'ta tamamı fluent'tir.

## Anatomi

```csharp
new SwipeView()
    .LeftItems(...)      // sağa kaydırınca açılır
    .RightItems(...)     // sola kaydırınca açılır
    .TopItems(...)
    .BottomItems(...)
    .Threshold(120)      // tetikleme için kaydırma mesafesi
    .Content(view)       // görünen satır
```

Her yön, `SwipeItem` (metin/ikon butonları) veya `SwipeItemView` (rastgele view) içeren bir `SwipeItems` koleksiyonu alır.

## Sil / Favori Satır Örneği

```csharp
new SwipeView()
.RightItems(
    new SwipeItems
    {
        new SwipeItem()
            .Text("Sil")
            .BackgroundColor(Colors.Red)
            .IconImageSource("trash.png")
            .OnInvoked((s, e) => vm.DeleteCommand.Execute(item)),

        new SwipeItem()
            .Text("Favori")
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

`SwipeItem`, `MenuItem`'dan türediğinden fluent yüzeyi şunları içerir: `.Text(...)`, `.IconImageSource(...)`, `.BackgroundColor(...)`, `.IsVisible(...)`, `.IsDestructive(...)`, `.Command(...)`/`.CommandParameter(...)` ve `.OnInvoked(...)` / `.OnClicked(...)`.

## SwipeItems Davranışı

Konteyneri de fluent yapılandırın:

```csharp
new SwipeItems
{
    new SwipeItem().Text("Arşivle").BackgroundColor(Colors.SteelBlue)
}
.Mode(SwipeMode.Execute)              // Execute = tam kaydırmada hemen çalıştır
                                      // Reveal (varsayılan) = butonları göster, dokununca çalıştır
.SwipeBehaviorOnInvoked(SwipeBehaviorOnInvoked.Close)
```

## Özel Swipe İçeriği — `SwipeItemView`

Tamamen özel açığa çıkan UI için:

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
                new Label().Text("Yanıtla").TextColor(Colors.White).FontSize(12)
            )
        )
}
```

## CollectionView İçinde

Tipik yerleşim — öğe şablonunun kökü olarak:

```csharp
new CollectionView()
.ItemsSource(e => e.Path("Messages"))
.ItemTemplate(() =>
    new SwipeView()
    .RightItems(
        new SwipeItems(new[]
        {
            (ISwipeItem)new SwipeItem()
                .Text("Sil")
                .BackgroundColor(Colors.Red)
                .Command(vm.DeleteCommand)
                .CommandParameter(e => e.Path("."))   // kaydırılan öğe
        })
    )
    .Content(
        new Grid().Padding(16).Children(
            new Label().Text(e => e.Path("Subject"))
        )
    )
)
```

> Swipe **içeriğine opak bir `BackgroundColor`** verin — şeffaf içerikte açılan öğeler alttan görünür ve satır bozuk görünür.

## Olaylar

```csharp
new SwipeView()
    .OnSwipeStarted((s, e) => Console.WriteLine(e.SwipeDirection))
    .OnSwipeChanging((s, e) => Console.WriteLine(e.Offset))
    .OnSwipeEnded((s, e) => Console.WriteLine(e.IsOpen))
    .OnOpenRequested((s, e) => { })
    .OnCloseRequested((s, e) => { })
```

Yakalanan referansla programatik aç/kapa:

```csharp
new SwipeView().Assign(out var swipe) /* ... */;
swipe.Open(OpenSwipeItem.RightItems);
swipe.Close();
```

## SwipeView mi, SwipeGestureRecognizer mı?

| | `SwipeView` | [`SwipeGestureRecognizer`](gesture-recognizers.md#swipe-kaydırma) |
|---|---|---|
| Aksiyon UI'si açar | ✅ | ❌ (yalnızca olay) |
| Parmağı takip eder | ✅ | ❌ |
| Kullanım | Liste satırı aksiyonları | Basit yönlü jestler (kapatma, gezinme) |

## İlgili Konular

- [Koleksiyonlar ve Şablonlar](collections-and-templates.md)
- [Jest Tanıyıcılar](gesture-recognizers.md)
- [Menüler](menus.md) — `SwipeItem`'ın tabanı `MenuItem`
