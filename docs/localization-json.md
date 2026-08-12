# Localization with JSON Files

FmgLib.MauiMarkup includes a lightweight localization system fed by JSON files, with **live language switching** — bound texts update instantly when the culture changes, no page reload required.

## 1. Register in `MauiProgram.cs`

```csharp
builder
    .UseMauiApp<App>()
    .UseMauiMarkupLocalization();
```

The recommended form is the **options overload** — it cannot confuse a file name with a culture name, and it is where the fallback culture, missing-key policy and culture-sync mode live:

```csharp
builder.UseMauiMarkupLocalization(o => o
    .UseFiles("Common.json", "Checkout.json")   // merged in order; later files win on duplicate keys
    .UseDefaultCulture("en-US")                 // startup language
    .UseFallbackCulture("en-US"));              // used when the current culture yields nothing
```

Shorter forms:

```csharp
// default: looks for "Localization.json" in the app package
.UseMauiMarkupLocalization()

// set the startup language
.UseMauiMarkupLocalization(defaultLang: "en-US")

// startup language + custom files
.UseMauiMarkupLocalization(defaultLang: "en-US", "Loc1.json", "Loc2.json")

// files only — the argument MUST be named, because the first positional parameter is the culture
.UseMauiMarkupLocalization(filePaths: new[] { "Localization1.json", "/Languages/Temp1.json" })
```

> **Watch the first argument.** In `UseMauiMarkupLocalization(defaultLang, params filePaths)` the culture comes first, so `UseMauiMarkupLocalization("Common.json", "Checkout.json")` passes a *file name* as the culture. That is now rejected at startup with a message naming the fix — use `filePaths:` or the options overload.

Loading is **synchronous and throws**: a missing or malformed language file fails the app at startup rather than leaving every label showing its raw key.

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

Every property bound with `Translate` updates immediately (the translator implements `INotifyPropertyChanged` and the bindings listen to it). `ChangeCulture` is safe to call from a background thread — the notification is marshalled to the main thread for you.

By default a culture change also sets `CultureInfo.DefaultThreadCurrentCulture` and `DefaultThreadCurrentUICulture`, so dates, numbers and currency follow the selected language too. Narrow or disable it if your app formats persisted values with the ambient culture:

```csharp
builder.UseMauiMarkupLocalization(o => o.SyncCulture(CultureSyncMode.UICultureOnly)); // formatting untouched
builder.UseMauiMarkupLocalization(o => o.SyncCulture(CultureSyncMode.None));          // translator only
```

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

> **These return a snapshot.** `new Label().Text("Hello".ToTranslate())` compiles and shows the right text, but it will **not** update when the language changes — there is no binding behind it. For anything on screen use `.Text(e => e.Translate("Hello"))`.

## Texts with Values in Them — `TranslateFormat`

A translated sentence usually carries a runtime value. `TranslateFormat` binds the translation *and* the arguments, so the label re-renders when the **language** changes and when any **argument** changes:

```json
{
  "WelcomeUser": { "en-US": "Welcome, {0}!",     "tr-TR": "Hoş geldin, {0}!" },
  "CartSummary": { "en-US": "{0} items — {1:C}", "tr-TR": "{0} ürün — {1:C}" }
}
```

```csharp
new Label().Text(e => e.TranslateFormat("WelcomeUser", nameof(vm.UserName)))
new Label().Text(e => e.TranslateFormat("CartSummary", nameof(vm.ItemCount), nameof(vm.Total)))
```

Argument paths resolve against the element's `BindingContext`. Placeholders are formatted with the **selected** culture, so `{1:C}` renders `$1,234.50` in `en-US` and `1.234,50 ₺` in `tr-TR`. If a translation loses its `{0}` the label falls back to the raw pattern instead of throwing.

## Right-to-Left Languages

Translating an Arabic or Hebrew UI without mirroring it leaves the layout wrong. Bind `FlowDirection` to the culture once, on the page:

```csharp
this.FlowDirection(e => e.FromCulture())
```

`Translator.Instance.IsRightToLeft` and `.FlowDirection` are also available for code paths.

## Missing Keys

By default a key with no translation renders as the key itself. Pick a different policy when that is not what you want:

```csharp
builder.UseMauiMarkupLocalization(o => o
    .UseFiles("Localization.json")
    .OnMissingTranslation(MissingTranslationBehavior.Marker));   // renders ⟦Key⟧ — impossible to miss
```

| Behaviour | Result for a missing `Hello` |
|---|---|
| `ReturnKey` *(default)* | `Hello` |
| `ReturnEmpty` | *(empty)* |
| `Marker` | `⟦Hello⟧` |
| `Throw` | `KeyNotFoundException` |

The RESX translator honours the same setting, so switching backend does not switch behaviour.

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

- **Split by feature:** `UseMauiMarkupLocalization(o => o.UseFiles("Common.json", "Checkout.json", "Settings.json"))`. Files are merged into one dictionary; on duplicate keys, later files override earlier ones — per language, so a feature file can override one language of a key without repeating the others.
- **Missing keys:** prefer meaningful key names (`"Login_InvalidPassword"`), and consider `MissingTranslationBehavior.Marker` in Debug builds so gaps are visible on screen.
- **Culture fallback:** a lookup walks `tr-TR` → `tr` → the configured `FallbackCulture`. Writing shared keys under the neutral language (`"tr"`, `"en"`) covers every regional variant at once.
- If a language file is missing or malformed, startup throws a `FileLoadException` describing the expected format — validate files as part of CI.

## JSON vs. RESX

| | JSON (this page) | [RESX](localization-resx.md) |
|---|---|---|
| File format | Single file for all languages | One `.resx` per language |
| Tooling | Any text editor | Visual Studio resource editor, existing enterprise workflows |
| Key access | String keys | String keys + generated strongly-typed class (`nameof` support) |
| Runtime switch | `Translator.Instance` | `TranslatorResx.Instance` |
| Binding method | `e.Translate("Key")` | `e.TranslateResx("Key")` |
| Formatted text | `e.TranslateFormat("Key", paths…)` | `e.TranslateResxFormat("Key", paths…)` |
| Culture fallback | `tr-TR` → `tr` → `FallbackCulture` | `ResourceManager` chain → neutral `.resx` |

Both support live switching; pick whichever fits your translation workflow.

## Related Topics

- [Localization (RESX)](localization-resx.md)
- [Fluent Properties](fluent-properties.md)
