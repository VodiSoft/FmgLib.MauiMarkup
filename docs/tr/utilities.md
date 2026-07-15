# Yardımcı Genişletmeler

Tek bir kontrole ait olmayan ama FmgLib.MauiMarkup kodunun her yerinde görünen küçük yardımcılar.

## Renk Yardımcıları — `string` → `Color`

Hex string'ler doğrudan dönüşür, `Color.FromArgb` gürültüsü olmadan:

```csharp
"#FF3366".ToColor()          // otomatik algılar; #RGB, #RRGGBB, #AARRGGBB ile çalışır
"#804C3BCF".ToColorFromArgb()  // açık ARGB kanal sırası
"#4C3BCF80".ToColorFromRgba()  // açık RGBA kanal sırası
```

```csharp
new Border().BackgroundColor("#1E1E2E".ToColor())
```

## `Assign` / `InvokeOnElement`

[Nesne Referansları](assign-and-references.md) sayfasında derinlemesine anlatılır; bütünlük için burada listelenmiştir:

```csharp
new Entry().Assign(out var entry)                       // referans yakala
new Entry().InvokeOnElement(e => e.Focus())             // zincir ortasında rastgele kod
```

## Koleksiyon Yardımcıları

### `Add(configure)` — koleksiyon başlatıcıları içinde fluent zincirler

Layout'lar ve diğer `IEnumerable` bindable nesneler **koleksiyon başlatıcılarında lambda** kabul eder; başlatıcı sözdizimini fluent yapılandırmayla karıştırmanızı sağlar:

```csharp
new VerticalStackLayout
{
    new Label().Text("Satır 1"),
    l => l.Spacing(12).Padding(16),      // Action<T> — layout'un kendisini yapılandırır
    new Label().Text("Satır 2"),
}
```

### `AddRangeMarkup` — herhangi bir koleksiyon özelliğine toplu ekleme

Koleksiyon tipli bir özelliğe öğe ekler (koleksiyon `null` ise oluşturur) ve zincirleme için sahibi döndürür:

```csharp
grid.AddRangeMarkup(g => g.Children, view1, view2, view3);
picker.AddRangeMarkup(p => p.Items, "Küçük", "Orta", "Büyük");
```

Mevcut ağaçları yeniden kurmak yerine onlara ekleyen yardımcı metotlarda kullanışlıdır.

## Çeviri String Genişletmeleri

Kod tarafı (binding olmayan) çeviriler — bkz. [Yerelleştirme (JSON)](localization-json.md) / [RESX](localization-resx.md):

```csharp
"Hello".ToTranslate()             // JSON translator, geçerli kültür
"Hello".ToTranslate("tr-TR")      // açık kültür
"Hello".ToTranslateResx()         // RESX translator
"Hello".ToTranslateResx("tr-TR")
```

## Grid Tanım Koleksiyonları

`Auto` / `Star` / `Absolute` builder'ları hem lambda formunda hem koleksiyon genişletmesi olarak vardır — bkz. [Grid](grid.md):

```csharp
new Grid().ColumnDefinitions(new ColumnDefinitionCollection().Absolute(100).Star())
```

## Stil ve VisualState Interop Yardımcıları

FmgLib stillerini ham MAUI API'siyle karıştıran ileri senaryolar için:

```csharp
Style style = myFmgStyle;                              // Style<T> → Style (örtük)
var groups = style.GetVisualStateGroupList();          // VSM gruplarına eriş/genişlet
var common = groups.GetCommonStatesVisualStateGroup(); // "CommonStates" grubu
groups.Add(new VisualState<Button>("Custom", e => e.Opacity(0.5)));  // tek state ekle
```

`BindableProperty.Set(value)` bir `Setter` oluşturur — stil/trigger başlatıcılarında kullanılır ([Trigger'lar](triggers.md)):

```csharp
Entry.TextColorProperty.Set(Colors.White)
```

## `RegisterName` — MAUI name scope'ları

```csharp
new Label().RegisterName("title", pageRoot)
```

Ayrıntılar: [Nesne Referansları](assign-and-references.md#registername--birebir-xname-karşılığı).

## İlgili Konular

- [Assign ve Referanslar](assign-and-references.md)
- [Grid](grid.md)
- [Stiller](styling.md)
- [Yerelleştirme (JSON)](localization-json.md)
