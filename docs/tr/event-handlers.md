# Olay İşleyiciler (Event Handlers)

FmgLib.MauiMarkup, her kontrolün **her olayı** için fluent bir `On<EventName>` metodu üretir — XAML'deki `Clicked="OnCounterClicked"` özniteliklerinin C# karşılığı, ama satır içinde ve iki pratik biçimde.

## İki Biçim

`Button.Clicked` örneğinde generator şunları üretir:

```csharp
// 1. Klasik event-handler imzası
public static T OnClicked<T>(this T self, EventHandler handler) where T : Button;

// 2. Yalnızca (tipli!) sender alan sadeleştirilmiş aksiyon
public static T OnClicked<T>(this T self, Action<T> action) where T : Button;
```

Günlük kullanım 2. biçimdir: kullanılmayan `object sender, EventArgs e` kalabalığı yok ve parametre zaten somut kontrol tipindedir.

## Method-Group Stili

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

## Satır İçi Lambda Stili

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

## Event Args Gerektiğinde

Klasik biçimi kullanın — tam `EventArgs` erişilebilir:

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

## Yaygın Olaylar Kopya Kağıdı

| Kontrol | Fluent metot | Tipik kullanım |
|---|---|---|
| `Button` | `OnClicked`, `OnPressed`, `OnReleased` | Aksiyonlar |
| `Entry` / `Editor` | `OnTextChanged`, `OnCompleted`, `OnFocused`, `OnUnfocused` | Doğrulama, yazarken arama |
| `CheckBox` | `OnCheckedChanged` | Anahtarlar |
| `Switch` | `OnToggled` | Ayarlar |
| `Slider` | `OnValueChanged`, `OnDragCompleted` | Aralıklar |
| `Picker` | `OnSelectedIndexChanged` | Seçim |
| `CollectionView` | `OnSelectionChanged`, `OnScrolled`, `OnRemainingItemsThresholdReached` | Listeler, sonsuz kaydırma |
| `RefreshView` | `OnRefreshing` | Çekerek yenileme |
| `ContentPage` | `OnAppearing`, `OnDisappearing`, `OnLoaded`, `OnUnloaded`, `OnNavigatedTo` | Yaşam döngüsü |
| `WebView` | `OnNavigating`, `OnNavigated` | Web içeriği |

(Listede olmayan her olay aynı `On<EventName>` kuralını izler.)

## Sayfa Yaşam Döngüsü Satır İçi

Sayfalar da `BindableObject` olduğundan yaşam döngüsü bağlama aynı fluent zincirde yaşayabilir:

```csharp
public void Build() =>
    this
    .OnAppearing(async page => await ViewModel.RefreshAsync())
    .Content(/* ... */);
```

## Olaylar mı Komutlar mı?

İkisi de çalışır; mimariye göre seçin:

```csharp
// Olay stili — sayfaya özgü mantık
new Button().Text("Kaydet").OnClicked(async b => await SaveAsync())

// Komut stili — MVVM
new Button()
    .Text("Kaydet")
    .Command(e => e.Path("SaveCommand"))
    .CommandParameter(e => e.Path("."))
```

Tipli bir view-model referansınız varsa (örn. [`FmgLibContentPage<TViewModel>`](hot-reload.md) ile) pragmatik ara yol:

```csharp
new Button().Text("Kaydet").Command(BindingContext.SaveCommand)
```

## Abonelik İptali ve Hot Reload

`On<Event>` `+=` ile abone olur. İki sonucu vardır:

- **UI yeniden kurulduğunda yeni kontroller yaratılır**; eski abonelikler eski kontrollerle ölür — tipik [hot reload](hot-reload.md) `Build()` akışında sızıntı olmaz.
- `Build()` içinde **uzun ömürlü nesnelere** (örn. `Application.Current`, statik bir servis) handler bağlarsanız her hot reload'da abonelik birikir. Bunları constructor'da bağlayın veya önce [`InvokeOnElement`](assign-and-references.md) ile aboneliği kaldırın.

## Özel / Üçüncü Parti Olaylar

Source generator, [üçüncü parti kontrollerin](third-party-controls.md) olayları için de aynı iki `On…` biçimini üretir:

```csharp
[MauiMarkup(typeof(ZXing.Net.Maui.Controls.CameraBarcodeReaderView))]
class Markup { }

// üretilen: OnBarcodesDetected(handler) ve OnBarcodesDetected(Action<T>)
new CameraBarcodeReaderView()
    .OnBarcodesDetected((s, e) => Process(e.Results))
```

## İlgili Konular

- [Jest Tanıyıcılar](gesture-recognizers.md) — rastgele view'larda tap/pan/pointer olayları
- [Trigger'lar](triggers.md) — kodsuz bildirimsel tepkiler
- [Hot Reload](hot-reload.md)
