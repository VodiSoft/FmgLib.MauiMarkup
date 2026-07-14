# Formatted Text — `FormattedString` & `Span`

To mix fonts, colors, sizes or tap actions **within a single `Label`**, use `FormattedText` with `Span`s — the C# equivalent of XAML's `<Label.FormattedText>` block, fully fluent.

## Basic Usage

```csharp
new Label()
    .FormattedText(
        new FormattedString()
        .Spans(
            new Span().Text("Total: "),
            new Span()
                .Text("$49.90")
                .FontAttributes(FontAttributes.Bold)
                .TextColor(Colors.SeaGreen),
            new Span()
                .Text("  (incl. VAT)")
                .FontSize(11)
                .TextColor(Colors.Gray)
        )
    )
```

`FormattedString.Spans(...)` accepts the spans as `params`; each `Span` supports the full fluent property set:

| Span method | Purpose |
|---|---|
| `.Text(string)` | The segment text (bindable — builder lambda works) |
| `.TextColor(Color)` / `.BackgroundColor(Color)` | Colors |
| `.FontSize(double)` / `.FontFamily(string)` / `.FontAttributes(...)` | Typography |
| `.TextDecorations(...)` | Underline / strikethrough |
| `.CharacterSpacing(double)` / `.LineHeight(double)` | Spacing |
| `.TextTransform(...)` | Casing |
| `.Style(Style)` | Span-targeted style |

## Bindings Inside Spans

Spans are `BindableObject`s — the property builder works as usual:

```csharp
new Label()
    .FormattedText(
        new FormattedString()
        .Spans(
            new Span().Text("Hello, "),
            new Span()
                .Text(e => e.Path("UserName"))
                .FontAttributes(FontAttributes.Bold),
            new Span().Text("!")
        )
    )
```

## Tappable Spans (inline links)

`Span` is a `GestureElement`, so gesture recognizers attach to individual segments — the standard "terms and conditions" pattern:

```csharp
new Label()
    .FormattedText(
        new FormattedString()
        .Spans(
            new Span().Text("I agree to the "),
            new Span()
                .Text("terms of service")
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

## Reusable Helpers

Since it's all C#, wrap common patterns:

```csharp
static Span Bold(string text) => new Span().Text(text).FontAttributes(FontAttributes.Bold);
static Span Link(string text, Action onTap) =>
    new Span()
        .Text(text)
        .TextColor(Colors.Blue)
        .TextDecorations(TextDecorations.Underline)
        .GestureRecognizers(new TapGestureRecognizer().OnTapped((s, e) => onTap()));

new Label().FormattedText(new FormattedString().Spans(
    new Span().Text("Read the "),
    Link("docs", () => OpenDocs()),
    new Span().Text(" or the "),
    Bold("samples"),
    new Span().Text(".")
));
```

## When to Use What

| Need | Tool |
|---|---|
| Uniform text | `Label.Text` |
| Mixed styling / inline links in one paragraph | `FormattedText` + `Span`s (this page) |
| Separate layout per fragment | Multiple `Label`s in a `HorizontalStackLayout` |
| Large rich/HTML content | `Label.TextType(TextType.Html)` or `WebView` |

## Related Topics

- [Fluent Properties](fluent-properties.md)
- [Gesture Recognizers](gesture-recognizers.md)
- [Text Alignment](text-alignment.md)
