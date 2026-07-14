# Localization with JSON Files

FmgLib.MauiMarkup includes a lightweight localization system fed by JSON files, with **live language switching** — bound texts update instantly when the culture changes, no page reload required.

## 1. Register in `MauiProgram.cs`

```csharp
builder
    .UseMauiApp<App>()
    .UseMauiMarkupLocalization();
```

Overloads:

```csharp
// default: looks for "Localization.json" in the app package
.UseMauiMarkupLocalization()

// set the startup language
.UseMauiMarkupLocalization(defaultLang: "en-US")

// custom file(s) — multiple files are merged (later files win on duplicate keys)
.UseMauiMarkupLocalization(defaultLang: "en-US", "Loc1.json", "Loc2.json")
.UseMauiMarkupLocalization(filePaths: new[] { "Localization1.json", "Localization2.json", "/Languages/Temp1.json" })
```

## 2. Create the JSON Language File

Structure: `{ "key": { "languageCode": "translation", ... }, ... }`

```json
{
  "Hello": {
    "tr-TR": "Merhaba Dünya!",
    "en-US": "Hello World!"
  },
  "Msg": {
    "tr-TR": "Deneme amaçlı yapılmıştır.",
    "en-US": "It was made for testing purposes."
  }
}
```

- Keys can be any word or phrase — no regex/naming restrictions.
- Language keys are free-form too, but standard culture names (`en-US`, `tr-TR`, `fr-FR`) are recommended because they align with `CultureInfo`.

> **Critical:** the JSON file's **Build Action must be `MauiAsset`** (it is read via `FileSystem.OpenAppPackageFileAsync`). In the `.csproj`:
>
> ```xml
> <ItemGroup>
>   <MauiAsset Include="Localization.json" />
> </ItemGroup>
> ```

## 3. Bind Texts with `Translate`

Anywhere a property builder is accepted:

```csharp
new Label()
    .Text(e => e.Translate("Hello"))
    .FontSize(32)
    .CenterHorizontal()
    .SemanticHeadingLevel(SemanticHeadingLevel.Level1),

new Label()
    .Text(e => e.Translate("Msg"))
    .FontSize(18)
    .CenterHorizontal()
    .SemanticDescription(e => e.Translate("Msg"))
```

`Translate` works on **any string property**, not just `Text` — placeholders, titles, tooltips:

```csharp
new Entry().Placeholder(e => e.Translate("EnterEmail"))
this.Title(e => e.Translate("SettingsTitle"))
```

## 4. Switch Languages at Runtime

```csharp
Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));
```

Every property bound with `Translate` updates immediately (the translator implements `INotifyPropertyChanged` and the bindings listen to it).

A complete language selector:

```csharp
new VerticalStackLayout()
.Center()
.Children(
    new RadioButton()
        .IsChecked(Translator.Instance.CurrentCulture.Name == "tr-TR")
        .Content("tr-TR")
        .OnCheckedChanged((sender, e) =>
        {
            if (e.Value)
                Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));
        }),

    new RadioButton()
        .IsChecked(Translator.Instance.CurrentCulture.Name == "en-US")
        .Content("en-US")
        .OnCheckedChanged((sender, e) =>
        {
            if (e.Value)
                Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));
        })
)
```

## Reading Translations in Code

For non-UI strings (alerts, logs), index the translator directly — or use the `ToTranslate` string extension:

```csharp
string title = Translator.Instance["Hello"];
await DisplayAlert(Translator.Instance["Hello"], Translator.Instance["Msg"], "OK");

// string extension equivalents:
string hello  = "Hello".ToTranslate();            // current culture
string helloTr = "Hello".ToTranslate("tr-TR");    // explicit culture
```

## Persisting the Choice

The library does not persist the selected culture; combine with `Preferences`:

```csharp
// on change
Preferences.Set("lang", "tr-TR");

// at startup (e.g. in App constructor)
var saved = Preferences.Get("lang", "en-US");
Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo(saved));
```

## Organizing Larger Apps

- **Split by feature:** pass multiple files — `UseMauiMarkupLocalization("Common.json", "Checkout.json", "Settings.json")`. Files are merged into one dictionary; on duplicate keys, later files override earlier ones.
- **Missing keys:** prefer meaningful key names (`"Login_InvalidPassword"`) so untranslated keys are visible during testing.
- If the JSON is malformed, startup throws a `FileLoadException` describing the expected format — validate files as part of CI.

## JSON vs. RESX

| | JSON (this page) | [RESX](localization-resx.md) |
|---|---|---|
| File format | Single file for all languages | One `.resx` per language |
| Tooling | Any text editor | Visual Studio resource editor, existing enterprise workflows |
| Key access | String keys | String keys + generated strongly-typed class (`nameof` support) |
| Runtime switch | `Translator.Instance` | `TranslatorResx.Instance` |
| Binding method | `e.Translate("Key")` | `e.TranslateResx("Key")` |

Both support live switching; pick whichever fits your translation workflow.

## Related Topics

- [Localization (RESX)](localization-resx.md)
- [Fluent Properties](fluent-properties.md)
