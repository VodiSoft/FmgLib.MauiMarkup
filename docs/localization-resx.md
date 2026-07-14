# Localization with RESX Files

If your team already translates via `.resx` resource files (the classic .NET workflow), FmgLib.MauiMarkup plugs into them directly — same live-switching behavior as the [JSON variant](localization-json.md), driven by your `ResourceManager`.

## 1. Create the RESX Resources

Add resource files to your project, e.g. under `Resources/Languages`:

```
AppResources.resx          (default language)
AppResources.tr-TR.resx
AppResources.fr-FR.resx
```

Each file contains the same keys with translated values (`Hello` → "Hello World!" / "Merhaba Dünya!"). Visual Studio generates the `AppResources` class with a static `ResourceManager`.

## 2. Register in `MauiProgram.cs`

```csharp
builder
    .UseMauiApp<App>()
    .UseMauiMarkupLocalizationWithResx(AppResources.ResourceManager);

// or with an explicit startup language:
// .UseMauiMarkupLocalizationWithResx(AppResources.ResourceManager, "en-US");
```

## 3. Bind Texts with `TranslateResx`

```csharp
new Label()
    .Text(e => e.TranslateResx("Hello"))
    .FontSize(32)
    .CenterHorizontal()
    .SemanticHeadingLevel(SemanticHeadingLevel.Level1),

new Label()
    .Text(e => e.TranslateResx(nameof(AppResources.Msg)))   // strongly-typed key!
    .FontSize(18)
    .CenterHorizontal()
    .SemanticDescription(e => e.TranslateResx("Msg"))
```

The `nameof(AppResources.Msg)` form is the recommended one — renaming a resource key becomes a compile-time-checked refactoring.

As with the JSON variant, any string property can be translated:

```csharp
new Entry().Placeholder(e => e.TranslateResx(nameof(AppResources.EnterEmail)))
this.Title(e => e.TranslateResx(nameof(AppResources.SettingsTitle)))
```

## 4. Switch Languages at Runtime

```csharp
TranslatorResx.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));
```

All `TranslateResx`-bound properties update immediately.

Language selector example:

```csharp
new VerticalStackLayout()
.Center()
.Children(
    new RadioButton()
        .IsChecked(TranslatorResx.Instance.CurrentCulture.Name == "tr-TR")
        .Content("tr-TR")
        .OnCheckedChanged((sender, e) =>
        {
            if (e.Value)
                TranslatorResx.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));
        }),

    new RadioButton()
        .IsChecked(TranslatorResx.Instance.CurrentCulture.Name == "en-US")
        .Content("en-US")
        .OnCheckedChanged((sender, e) =>
        {
            if (e.Value)
                TranslatorResx.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));
        })
)
```

## Reading Translations in Code

```csharp
string msg = TranslatorResx.Instance[nameof(AppResources.Msg)];
await DisplayAlert("Info", msg, "OK");

// string extension equivalents:
string hello   = "Hello".ToTranslateResx();            // current culture
string helloTr = "Hello".ToTranslateResx("tr-TR");     // explicit culture
```

## Notes & Tips

- **Culture fallback** follows standard `ResourceManager` rules: `tr-TR` → `tr` → default resources. Keep the neutral `.resx` complete.
- **Persist the selection** with `Preferences` and re-apply it at startup (see the pattern in [Localization (JSON)](localization-json.md#persisting-the-choice)).
- JSON and RESX systems are independent (`Translator` vs. `TranslatorResx`); you *can* use both in one app, but standardizing on one keeps things simple.
- Formatted strings: store the pattern in resources (`"WelcomeUser" = "Welcome, {0}!"`) and combine with a [converter](binding-converters.md) or `string.Format` in code.

## Related Topics

- [Localization (JSON)](localization-json.md) — includes a JSON-vs-RESX comparison table
- [Fluent Properties](fluent-properties.md)
