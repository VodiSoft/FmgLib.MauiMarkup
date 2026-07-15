# Nesne Referansları — `Assign`, `InvokeOnElement`, `RegisterName`

XAML'de öğeleri `x:Name` ile adlandırıp code-behind'dan erişirsiniz. FmgLib.MauiMarkup'ta referansları **ağacı kurarken, satır içinde** yakalarsınız — fluent zinciri bozmadan.

## `Assign(out var …)` — `x:Name`'in karşılığı

`Assign`, geçerli kontrolü bir değişkende saklar ve kontrolü döndürür; zincir devam eder:

```csharp
public static T Assign<T>(this T self, out T obj) where T : BindableObject;
```

### Yerel değişkene yakalama

```csharp
new Label().Assign(out var label);
new Entry().Assign(out var entry);
```

### Alana (field) yakalama

```csharp
Button btnOk;

new Button()
    .Text("Tamam")
    .Assign(out btnOk);
```

### Tipik kullanım — bir kontrolün diğerine referans vermesi

En yaygın desen: ağaçta daha önce tanımlanan bir kontrol, sonraki bir kontrolün binding'i veya olay işleyicisi tarafından kullanılır.

```csharp
this.Content(
    new StackLayout()
    .Children(
        new Slider()
            .Assign(out var slider)
            .Minimum(1)
            .Maximum(20),

        new Label()
            // bu label'ın Text'ini yukarıda yakalanan slider'a bağla
            .Text(e => e.Path("Value").Source(slider).StringFormat("Slider value: {0}"))
            .FontSize(28)
    )
);
```

Ya da bir kardeşi değiştiren olay işleyicileri:

```csharp
new StackLayout()
.Children(
    new Label().Text("Görsele iki kez dokunun").Assign(out var label),

    new Image()
        .Source("dotnet_bot.png")
        .SizeRequest(100, 100)
        .GestureRecognizers(
            new TapGestureRecognizer()
                .NumberOfTapsRequired(2)
                .OnTapped((s, e) => label.Text = "2 kez dokundunuz!")
        )
)
```

### Sıralama uyarısı

`out var` değişkenleri C#'ta *kullanımdan önce bildirilmelidir*. Ağaçta önce gelen bir kontrolün sonra gelen bir kontrole referansı gerekiyorsa değişkeni baştan bildirin:

```csharp
Button submit = null!;

this.Content(
    new VerticalStackLayout()
    .Children(
        new Entry()
            .Placeholder("Ad")
            .OnTextChanged((s, e) => submit.IsEnabled = !string.IsNullOrEmpty(e.NewTextValue)),

        new Button()
            .Text("Gönder")
            .IsEnabled(false)
            .Assign(out submit)      // önceden bildirilen değişkene atar
    )
);
```

> **Hot reload ipucu:** sayfa `Build()` ile yeniden kurulduğunda ([Hot Reload](hot-reload.md)), `Assign` ile yakalanan alanlar her kurulumda yeniden atanır — `Build()` içindeki yerel değişkenler en güvenli seçimdir.

## `InvokeOnElement` — zincirin ortasında rastgele kod

`InvokeOnElement`, kontrole karşı herhangi bir aksiyonu çalıştırır ve zinciri sürdürür. Fluent metodu olmayan nadir API'ler için kullanın (bindable olmayan özellikler, metot çağrıları, özel mantıklı olay abonelikleri):

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

Ayrıca satır içi **koşullu yapılandırmanın** standart yoludur:

```csharp
new Button()
    .Text("Satın Al")
    .InvokeOnElement(b =>
    {
        if (user.IsPremium)
            b.BackgroundColor = Colors.Gold;
    })
```

## `RegisterName` — birebir `x:Name` karşılığı

MAUI'nin name scope'una ihtiyaç duyan ileri senaryolarda (bazı animasyonlar veya `FindByName` interop'u) `RegisterName`, kontrolü verilen kök öğenin name scope'una kaydeder:

```csharp
this.Content(
    new StackLayout()
        .Assign(out var root)
        .Children(
            new Label()
                .Text("Adlandırılmış öğe")
                .RegisterName("myLabel", root)
        )
);

// daha sonra
var label = (Label)((INameScope)NameScope.GetNameScope(root)).FindByName("myLabel");
```

Günlük kodda buna ihtiyacınız olmaz — `Assign` derleme zamanı güvenliğiyle vakaların %99'unu kapsar.

## Hangisini Ne Zaman?

| İhtiyaç | Kullan |
|---|---|
| Aynı sayfanın başka yerinde bir kontrole referans | `Assign(out var x)` |
| Kontrolü sonradan kullanmak için alanda sakla | `Assign(out _field)` |
| Satır içinde metot çağır / bindable olmayan özellik ayarla | `InvokeOnElement(x => …)` |
| Zincir içinde koşullu kurulum | `InvokeOnElement` |
| MAUI name-scope interop'u | `RegisterName` |

## İlgili Konular

- [Özellik Bağlama](data-binding.md) — `Source(control)`, `Assign` ile yakalanan referansları kullanır
- [Hot Reload](hot-reload.md) — yakalanan referanslar için yaşam döngüsü etkileri
