# Behavior'lar

Behavior'lar, kontrollere **alt sınıf oluşturmadan** yeniden kullanılabilir işlevsellik ekler. FmgLib.MauiMarkup onları her `VisualElement`'te bulunan `Behaviors(...)` metoduyla fluent olarak bağlar.

## Behavior Bağlama

```csharp
new Entry()
    .Text("Click Item")
    .Behaviors(new YourCustomBehavior());
```

Tek çağrıda birden çok behavior:

```csharp
new Entry()
    .Placeholder("E-posta")
    .Behaviors(
        new EmailValidationBehavior(),
        new MaxLengthBehavior(64)
    );
```

## Özel Behavior Yazma

Bir behavior `Behavior<T>`'den türer ve bağlanma/ayrılma kancalarını override eder:

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

Kullanım:

```csharp
new Entry()
    .Placeholder("Tutar")
    .Keyboard(Keyboard.Numeric)
    .Behaviors(new NumericValidationBehavior());
```

## Bindable Yapılandırmalı Behavior

Behavior'lar `BindableObject` olduğundan bindable özellikler sunabilirler — ve [source generator](third-party-controls.md) *herhangi bir* `BindableObject` için fluent metot üretebildiğinden, behavior'ınıza kendi fluent API'sini bile verebilirsiniz:

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

// behavior'ı fluent üretimine dahil edin:
[MauiMarkup(typeof(MinLengthBehavior))]
public class MarkupTargets { }

// sonra:
new Entry()
    .Behaviors(new MinLengthBehavior().MinLength(8));
```

## Community Toolkit Behavior'ları

`CommunityToolkit.Maui` birçok hazır behavior içerir; aynı şekilde bağlanırlar:

```csharp
new Entry()
    .Placeholder("E-posta")
    .Behaviors(
        new CommunityToolkit.Maui.Behaviors.EmailValidationBehavior
        {
            InvalidStyle = invalidStyle,
            ValidStyle = validStyle
        }
    );
```

(Toolkit behavior'ları için de fluent metot isterseniz `[MauiMarkup(typeof(EmailValidationBehavior))]` ile kaydedin — bkz. [Üçüncü Parti Kontroller](third-party-controls.md).)

## Behavior mu, Alternatifler mi?

| İhtiyaç | En iyi araç |
|---|---|
| Yeniden kullanılabilir, kendi kendine yeten kontrol mantığı (doğrulama, maskeleme, throttling) | **Behavior** |
| Tek kontrolde tek seferlik olay tepkisi | [`On<Event>`](event-handlers.md) lambda |
| Koşula bağlı bildirimsel özellik değişimi | [Trigger'lar](triggers.md) |
| Kontrol durumlarına görsel tepki (odak, devre dışı…) | [Visual State'ler](visual-states.md) |

## Hot Reload Notu

Behavior'lar `Build()` içinde yaratılan kontrollere bağlanır; [hot reload](hot-reload.md)'da kontroller yeniden kurulur ve behavior'lar doğal olarak yeniden bağlanır. Behavior'ları durumsuz tutun veya durumu `OnAttachedTo`'da sıfırlayın ki yeniden kurulumlar güvenli kalsın.

## İlgili Konular

- [Olay İşleyiciler](event-handlers.md)
- [Trigger'lar](triggers.md)
- [Üçüncü Parti Kontroller](third-party-controls.md)
