# Jest Tanıyıcılar (Gesture Recognizers)

Her `View`, `GestureRecognizers(...)` metoduyla jest tanıyıcılar eklenerek dokunma ve pointer girdisine tepki verebilir. Tüm tanıyıcı tipleri tam fluent desteklidir — özellikler ve `On…` olay metotları dahil.

Mevcut tanıyıcılar (hepsi MAUI'den, hepsi fluent):

- `TapGestureRecognizer`
- `PanGestureRecognizer`
- `PointerGestureRecognizer`
- `SwipeGestureRecognizer`
- `PinchGestureRecognizer`
- `DragGestureRecognizer` / `DropGestureRecognizer`

## Tanıyıcı Ekleme

`GestureRecognizers` bir veya daha çok tanıyıcı kabul eder:

```csharp
new Image()
    .Source("dotnet_bot.png")
    .GestureRecognizers(
        new TapGestureRecognizer().OnTapped((s, e) => Console.WriteLine("tap"))
    )
```

## Tap (Dokunma)

`TapGestureRecognizer` dokunmaları algılar; `NumberOfTapsRequired` tek/çift dokunmayı seçer.

```csharp
new StackLayout()
.Children(
    new Label()
        .Text("Görsele 2 kez dokunun")
        .Assign(out var label),

    new Image()
        .Source("dotnet_bot.png")
        .SizeRequest(100, 100)
        .GestureRecognizers(
            new TapGestureRecognizer()
                .NumberOfTapsRequired(2)
                .OnTapped((s, e) => label.Text = "2 kez dokundunuz")
        )
)
```

MVVM için komut stili (herhangi bir view'ı "tıklanabilir" yapmanın yolu budur):

```csharp
new Label()
    .Text("Tümünü gör")
    .TextDecorations(TextDecorations.Underline)
    .GestureRecognizers(
        new TapGestureRecognizer()
            .Command(BindingContext.GotoAllProductsCommand)
    )
```

Destekleyen MAUI sürümlerinde `Buttons(ButtonsMask.Secondary)` dokunuşu sağ tık/ikincil girdiye sınırlar — masaüstü bağlam aksiyonları için kullanışlıdır.

## Pan (sürükleyerek taşıma)

`PanGestureRecognizer`, parmak/fare hareket ederken kayma deltalarını akıtır. Klasik "görseli sürükle" uygulaması:

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

`args.StatusType`, `Started → Running → Completed` (veya `Canceled`) döngüsündedir; `TotalX`/`TotalY` jestin başından itibaren kümülatiftir — bu yüzden tamamlanınca geçerli kayma saklanır.

## Pointer (hover / hareket — masaüstü ve pointer cihazlar)

`PointerGestureRecognizer` giriş/çıkış/hareket bildirir; Windows/macOS'ta hover efektleri için idealdir:

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
                            .OnPointerEntered((s, e) => stateLabel.Text = "Girdi")
                            .OnPointerExited((s, e) => stateLabel.Text = "Çıktı")
                            .OnPointerMoved((s, e) =>
                            {
                                var pos = e.GetPosition(relativeTo: image)!.Value;
                                posLabel.Text = $"nokta: {pos.X:F0}, {pos.Y:F0}";
                            })
                    )
            )
        );
    }
}
```

Basit hover vurgusu:

```csharp
new Border()
    .Assign(out var card)
    .GestureRecognizers(
        new PointerGestureRecognizer()
            .OnPointerEntered((s, e) => card.BackgroundColor = Colors.LightGray)
            .OnPointerExited((s, e) => card.BackgroundColor = Colors.White)
    )
```

## Swipe (Kaydırma)

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

> Liste satırları için ham swipe jesti yerine `SwipeView` kullanın (açığa çıkan aksiyon butonlarıyla tam bir kontrol) — bkz. [SwipeView](swipeview.md).

## Pinch (Yakınlaştırma)

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

## Sürükle ve Bırak (Drag & Drop)

```csharp
// Kaynak
new Label()
    .Text("Beni sürükle")
    .GestureRecognizers(
        new DragGestureRecognizer()
            .CanDrag(true)
            .OnDragStarting((s, e) => e.Data.Text = "payload")
    ),

// Hedef
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

## İpuçları

- Tanıyıcılar **her view'da** çalışır — `Label`, `Image`, `Border`, hatta bütün layout'lar. Bir konteynere eklemek tüm alt ağacı dokunulabilir yapar.
- Birden çok tanıyıcıyı tek `GestureRecognizers(...)` çağrısında birleştirin (`params` dostudur, swipe örneğindeki gibi).
- Şablonlardaki basit "tıklanabilir yap" durumlarında `TapGestureRecognizer().Command(...)` + `CommandParameter(e => e.Path("."))`, dokunulan öğeyi view model'e geçirir.

## İlgili Konular

- [Olay İşleyiciler](event-handlers.md)
- [SwipeView](swipeview.md)
- [Assign](assign-and-references.md) — jestlerin değiştirdiği view'ları yakalama
