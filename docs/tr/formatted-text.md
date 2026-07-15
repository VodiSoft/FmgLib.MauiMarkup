# Biçimli Metin — `FormattedString` ve `Span`

**Tek bir `Label` içinde** farklı font, renk, boyut veya dokunma aksiyonlarını karıştırmak için `FormattedText` ile `Span`'ları kullanın — XAML'deki `<Label.FormattedText>` bloğunun C# karşılığı, tamamen fluent.

## Temel Kullanım

```csharp
new Label()
    .FormattedText(
        new FormattedString()
        .Spans(
            new Span().Text("Toplam: "),
            new Span()
                .Text("₺49,90")
                .FontAttributes(FontAttributes.Bold)
                .TextColor(Colors.SeaGreen),
            new Span()
                .Text("  (KDV dahil)")
                .FontSize(11)
                .TextColor(Colors.Gray)
        )
    )
```

`FormattedString.Spans(...)` span'ları `params` olarak alır; her `Span` tam fluent özellik setini destekler:

| Span metodu | Amaç |
|---|---|
| `.Text(string)` | Segment metni (bindable — builder lambda çalışır) |
| `.TextColor(Color)` / `.BackgroundColor(Color)` | Renkler |
| `.FontSize(double)` / `.FontFamily(string)` / `.FontAttributes(...)` | Tipografi |
| `.TextDecorations(...)` | Altı çizili / üstü çizili |
| `.CharacterSpacing(double)` / `.LineHeight(double)` | Boşluk |
| `.TextTransform(...)` | Harf durumu |
| `.Style(Style)` | Span hedefli stil |

## Span İçinde Binding'ler

Span'lar `BindableObject`'tir — property builder her zamanki gibi çalışır:

```csharp
new Label()
    .FormattedText(
        new FormattedString()
        .Spans(
            new Span().Text("Merhaba, "),
            new Span()
                .Text(e => e.Path("UserName"))
                .FontAttributes(FontAttributes.Bold),
            new Span().Text("!")
        )
    )
```

## Tıklanabilir Span'lar (satır içi bağlantılar)

`Span` bir `GestureElement`'tir; jest tanıyıcılar tek tek segmentlere takılır — standart "kullanım şartları" deseni:

```csharp
new Label()
    .FormattedText(
        new FormattedString()
        .Spans(
            new Span().Text("Şunları kabul ediyorum: "),
            new Span()
                .Text("kullanım şartları")
                .TextColor(Colors.Blue)
                .TextDecorations(TextDecorations.Underline)
                .GestureRecognizers(
                    new TapGestureRecognizer()
                        .OnTapped((s, e) => Browser.OpenAsync("https://example.com/tos"))
                ),
            new Span().Text(".")
        )
    )
```

## Yeniden Kullanılabilir Yardımcılar

Her şey C# olduğundan yaygın kalıpları sarın:

```csharp
static Span Bold(string text) => new Span().Text(text).FontAttributes(FontAttributes.Bold);
static Span Link(string text, Action onTap) =>
    new Span()
        .Text(text)
        .TextColor(Colors.Blue)
        .TextDecorations(TextDecorations.Underline)
        .GestureRecognizers(new TapGestureRecognizer().OnTapped((s, e) => onTap()));

new Label().FormattedText(new FormattedString().Spans(
    new Span().Text("Şunu okuyun: "),
    Link("dokümanlar", () => OpenDocs()),
    new Span().Text(" veya "),
    Bold("örnekler"),
    new Span().Text(".")
));
```

## Ne Zaman Ne Kullanmalı?

| İhtiyaç | Araç |
|---|---|
| Tek biçimli metin | `Label.Text` |
| Tek paragrafta karışık stil / satır içi bağlantı | `FormattedText` + `Span`'lar (bu sayfa) |
| Her parça için ayrı yerleşim | `HorizontalStackLayout` içinde birden çok `Label` |
| Büyük zengin/HTML içerik | `Label.TextType(TextType.Html)` veya `WebView` |

## İlgili Konular

- [Fluent Özellikler](fluent-properties.md)
- [Jest Tanıyıcılar](gesture-recognizers.md)
- [Metin Hizalama](text-alignment.md)
