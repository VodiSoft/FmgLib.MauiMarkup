# Trigger'lar

Trigger'lar, koşullara veya olaylara yanıt olarak özellikleri bildirimsel şekilde değiştirir — olay işleyici kodu gerekmez. FmgLib.MauiMarkup tüm MAUI trigger tipleri için fluent builder'lar sunar:

| Trigger | Tetiklenme koşulu |
|---|---|
| `Trigger` (property trigger) | Aynı kontroldeki bir özellik belirli değere ulaştığında |
| `DataTrigger` | Bir **binding** belirli değere ulaştığında |
| `EventTrigger` | Bir olay ateşlendiğinde (`TriggerAction` çalıştırır) |
| `MultiTrigger` | Birden çok koşulun tamamı sağlandığında |
| State trigger'lar (`AdaptiveTrigger`, `CompareStateTrigger`, `DeviceStateTrigger`, `OrientationStateTrigger`, `StateTrigger`) | [Visual State'ler](visual-states.md) içinde kullanılır |

Trigger'lar bir kontrole `.Triggers(...)` ile bağlanabilir veya bir [`Style<T>`](styling.md) içine konabilir.

## Property Trigger'lar

Kontrolün kendi özelliğine tepki verir. Örnek: odaklanınca `Entry`'yi vurgula — bir kez stilde tanımlanır, sayfadaki tüm `Entry`'lere uygulanır:

```csharp
using FmgLib.MauiMarkup;

public class PropertyTriggerPage : ContentPage
{
    public PropertyTriggerPage()
    {
        this
        .Resources(
            new ResourceDictionary
            {
                new Style<Entry>
                {
                    Entry.BackgroundColorProperty.Set(Colors.Black),
                    Entry.TextColorProperty.Set(Colors.White),

                    new Trigger(typeof(Entry))
                        .Property(Entry.IsFocusedProperty)
                        .Value(true)
                        .Setters(
                            new Setters<Entry>(e => e
                                .BackgroundColor(Colors.Yellow)
                                .TextColor(Colors.Black))
                        ),
                }
            }
        )
        .Content(
            new StackLayout()
            .Children(
                new Entry().Placeholder("Ad girin"),
                new Entry().Placeholder("Şifre girin"),
                new Entry().Placeholder("Adres girin")
            )
        );
    }
}
```

Dikkat edilecek parçalar:

- `SomeProperty.Set(value)` — `BindableProperty` üzerinde `Setter` üreten genişletme; stilin taban değerleri için kullanılır.
- `new Trigger(typeof(Entry)).Property(...).Value(...)` — trigger koşulu.
- `Setters(new Setters<Entry>(e => e...))` — güçlü tipli setter builder; **içinde aynı fluent özellik metotları çalışır**.

Özellik trigger değerinden çıktığında orijinal stil değerleri otomatik geri yüklenir (standart MAUI trigger semantiği).

## Data Trigger'lar

Bir **binding**'e tepki verir — başka bir kontrol veya view model. Örnek: entry boşken Kaydet butonunu devre dışı bırak:

```csharp
this.Content(
    new StackLayout()
    .Children(
        new Entry().Assign(out var entry).Placeholder("Metin girin..."),

        new Button()
            .Text("Kaydet")
            .Triggers(
                new DataTrigger(typeof(Button))
                    .Binding(e => e.Path("Text.Length").Source(entry))
                    .Value(0)
                    .Setters(new Setters<Button>(e => e.IsEnabled(false)))
            )
    )
);
```

`Binding(...)` tam [property-builder sözdizimini](data-binding.md) kabul eder; view-model koşulları da çalışır:

```csharp
new Label()
    .Text("Çevrimdışı mod")
    .IsVisible(false)
    .Triggers(
        new DataTrigger(typeof(Label))
            .Binding(e => e.Path("IsConnected"))
            .Value(false)
            .Setters(new Setters<Label>(e => e.IsVisible(true)))
    )
```

## Event Trigger'lar

Bir olay ateşlendiğinde `TriggerAction` çalıştırır. Örnek: her tuş vuruşunda sayısal giriş doğrulaması:

```csharp
this.Content(
    new StackLayout()
    .Children(
        new Entry()
            .Placeholder("Bir sayı girin...")
            .Triggers(
                new EventTrigger()
                    .Event("TextChanged")
                    .Actions(new NumericValidationTriggerAction())
            )
    )
);
```

Aksiyon sınıfı:

```csharp
public class NumericValidationTriggerAction : TriggerAction<Entry>
{
    protected override void Invoke(Entry entry)
    {
        bool isValid = double.TryParse(entry.Text, out _);
        entry.TextColor = isValid ? Colors.Black : Colors.Red;
    }
}
```

> Event trigger'lar hiçbir şeyi otomatik geri almaz — olay her ateşlendiğinde ne yapılacağına aksiyon karar verir.

## Multi Trigger'lar

Tüm koşullar sağlanmalıdır. Koşullar `PropertyCondition` / `BindingCondition` ile kurulur:

```csharp
new Entry().Assign(out var email),
new Entry().Assign(out var phone),

new Button()
    .Text("Gönder")
    .IsEnabled(false)
    .Triggers(
        new MultiTrigger(typeof(Button))
            .Conditions(
                new BindingCondition()
                    .Binding(e => e.Path("Text.Length").Source(email))
                    .Value(0),
                new BindingCondition()
                    .Binding(e => e.Path("Text.Length").Source(phone))
                    .Value(0)
            )
            .Setters(new Setters<Button>(e => e.IsEnabled(true)))
    )
```

*(Bu örnek butonu yalnızca iki giriş de boşken etkinleştirir — doğrulama mantığınıza göre değerleri tersleyin.)*

## Stilde mi, Kontrolde mi?

- **Kontrolde** (`.Triggers(...)`): yalnızca o örneği etkiler.
- **`Style<T>` içinde** (ilk örnekteki gibi stilin koleksiyon başlatıcısına eklenir): stilin hedeflediği her kontrole uygulanır — uygulama geneli davranış için DRY seçenek. Bkz. [Stiller](styling.md).

## Trigger mı, Alternatifler mi?

| Senaryo | Önerilen |
|---|---|
| Uygulama genelinde odak/basma/devre dışı görsel tepkisi | Stilde property trigger veya [Visual State'ler](visual-states.md) |
| View-model durumuna bağlı etkin/pasif | `DataTrigger` veya [converter'lı](binding-converters.md) `IsEnabled` binding'i |
| Karmaşık doğrulama mantığı | [Behavior](behaviors.md) veya view-model mantığı |
| Tek seferlik olay tepkisi | [`On<Event>`](event-handlers.md) |

## İlgili Konular

- [Visual State'ler](visual-states.md) — state trigger'lar, duyarlı yerleşim için `AdaptiveTrigger`
- [Stiller](styling.md)
- [Özellik Bağlama](data-binding.md)
