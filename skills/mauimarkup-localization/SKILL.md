---
name: mauimarkup-localization
description: Localize FmgLib.MauiMarkup apps with live language switching — UseMauiMarkupLocalization options, JSON language files as MauiAsset, RESX with TranslatorResx, e.Translate / TranslateFormat bindings, culture fallback, missing-key policies, RTL FlowDirection and persisting the user's choice. Use when adding multi-language support, translating an existing C# markup MAUI UI, or debugging texts that don't update when the language changes.
license: MIT
---

# Localization (JSON & RESX)

Requires the `mauimarkup` core skill.

Both backends support **live switching**: bound texts update instantly when the culture changes, with
no page reload. Pick JSON for a single file editable by anyone; pick RESX when the team already has a
resource workflow or wants generated `nameof`-able keys.

## JSON setup

```csharp
// MauiProgram.cs — the options overload is the recommended form
builder.UseMauiMarkupLocalization(o => o
    .UseFiles("Common.json", "Checkout.json")   // merged in order; later files win on duplicate keys
    .UseDefaultCulture("en-US")
    .UseFallbackCulture("en-US"));
```

Shorter forms exist, but **watch the first argument** — in
`UseMauiMarkupLocalization(defaultLang, params filePaths)` the culture comes first, so
`UseMauiMarkupLocalization("Common.json")` passes a file name as the culture. That is rejected at
startup with a message naming the fix. Use `filePaths:` or the options overload.

```csharp
.UseMauiMarkupLocalization()                                        // default: Localization.json
.UseMauiMarkupLocalization(defaultLang: "en-US")
.UseMauiMarkupLocalization(defaultLang: "en-US", "Loc1.json", "Loc2.json")
.UseMauiMarkupLocalization(filePaths: new[] { "Localization1.json", "/Languages/Temp1.json" })
```

Loading is synchronous and throws: a missing or malformed file fails startup instead of leaving every
label showing a raw key.

### The file

```json
{
  "Hello": { "en-US": "Hello World!", "tr-TR": "Merhaba Dünya!" },
  "WelcomeUser": { "en-US": "Welcome, {0}!", "tr-TR": "Hoş geldin, {0}!" }
}
```

Structure is `{ "key": { "culture": "translation" } }`. Keys are free-form; standard culture names
(`en-US`, `tr-TR`) are recommended because they align with `CultureInfo`.

> **The build action must be `MauiAsset`** — the file is read through
> `FileSystem.OpenAppPackageFileAsync`. This is the #1 cause of "it throws at startup":
>
> ```xml
> <ItemGroup>
>   <MauiAsset Include="Localization.json" />
> </ItemGroup>
> ```

## Binding texts

```csharp
new Label().Text(e => e.Translate("Hello"))
new Entry().Placeholder(e => e.Translate("EnterEmail"))
this.Title(e => e.Translate("SettingsTitle"))
new Label().SemanticDescription(e => e.Translate("Msg"))
```

`Translate` works on **any string property**, not just `Text`. RESX uses `TranslateResx` identically.

### Texts containing values

```csharp
new Label().Text(e => e.TranslateFormat("WelcomeUser", nameof(vm.UserName)))
new Label().Text(e => e.TranslateFormat("CartSummary", nameof(vm.ItemCount), nameof(vm.Total)))
```

Argument paths resolve against the element's `BindingContext`, and the label re-renders when the
**language** changes *and* when any **argument** changes. Placeholders are formatted with the selected
culture, so `{1:C}` renders `$1,234.50` in `en-US` and `1.234,50 ₺` in `tr-TR`. A translation that lost
its `{0}` falls back to the raw pattern instead of throwing. RESX equivalent:
`TranslateResxFormat`.

## Switching at runtime

```csharp
Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));   // RESX: TranslatorResx.Instance
```

Every property bound with `Translate` updates immediately. Safe to call from a background thread — the
notification is marshalled to the main thread.

A culture change also sets `CultureInfo.DefaultThreadCurrentCulture` and `DefaultThreadCurrentUICulture`
by default, so dates, numbers and currency follow. Narrow it if the app formats persisted values with
the ambient culture:

```csharp
o.SyncCulture(CultureSyncMode.UICultureOnly)   // formatting untouched
o.SyncCulture(CultureSyncMode.None)            // translator only
```

Language selector:

```csharp
new VerticalStackLayout().Center().Children(
    new RadioButton()
        .Content("English")
        .IsChecked(Translator.Instance.CurrentCulture.Name == "en-US")
        .OnCheckedChanged((s, e) =>
        {
            if (e.Value) Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));
        }),
    new RadioButton()
        .Content("Türkçe")
        .IsChecked(Translator.Instance.CurrentCulture.Name == "tr-TR")
        .OnCheckedChanged((s, e) =>
        {
            if (e.Value) Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));
        }))
```

The library does not persist the choice — combine with `Preferences`:

```csharp
Preferences.Set("lang", "tr-TR");                                   // on change
var saved = Preferences.Get("lang", "en-US");                       // at startup
Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo(saved));
```

## Strings in code (alerts, logs)

```csharp
string title = Translator.Instance["Hello"];
string hello = "Hello".ToTranslate();              // current culture
string tr     = "Hello".ToTranslate("tr-TR");      // explicit culture
```

> **These return a snapshot.** `new Label().Text("Hello".ToTranslate())` compiles and shows the right
> text but will **never** update on a language change — there is no binding behind it. Anything on
> screen must use `.Text(e => e.Translate("Hello"))`. This is the most common localization bug in
> practice; check for it during review.

## Right-to-left

```csharp
this.FlowDirection(e => e.FromCulture())
```

One call per page mirrors the whole layout for Arabic/Hebrew. `Translator.Instance.IsRightToLeft` and
`.FlowDirection` are available for code paths.

## Missing keys

```csharp
builder.UseMauiMarkupLocalization(o => o
    .UseFiles("Localization.json")
    .OnMissingTranslation(MissingTranslationBehavior.Marker));
```

| Behaviour | Result for a missing `Hello` |
|---|---|
| `ReturnKey` *(default)* | `Hello` |
| `ReturnEmpty` | *(empty)* |
| `Marker` | `⟦Hello⟧` |
| `Throw` | `KeyNotFoundException` |

RESX honours the same setting. `Marker` in Debug builds makes gaps impossible to miss on screen.

## Organizing larger apps

- **Split by feature**: `o.UseFiles("Common.json", "Checkout.json", "Settings.json")`. Files merge into
  one dictionary; on duplicate keys later files win **per language**, so a feature file can override one
  language of a key without repeating the others.
- **Name keys meaningfully**: `Login_InvalidPassword`, not `Msg3`.
- **Culture fallback** walks `tr-TR` → `tr` → the configured `FallbackCulture`. Writing shared keys
  under the neutral language covers every regional variant at once.
- **Validate in CI** — a malformed file throws `FileLoadException` at startup, so a smoke test that
  boots the translator catches broken translations before release.

## JSON vs. RESX

| | JSON | RESX |
|---|---|---|
| Files | one file, all languages | one `.resx` per language |
| Tooling | any text editor | VS resource editor, enterprise workflows |
| Keys | strings | strings + generated typed class (`nameof`) |
| Runtime | `Translator.Instance` | `TranslatorResx.Instance` |
| Binding | `e.Translate("Key")` | `e.TranslateResx("Key")` |
| Formatted | `e.TranslateFormat(...)` | `e.TranslateResxFormat(...)` |
| Fallback | `tr-TR` → `tr` → `FallbackCulture` | `ResourceManager` chain → neutral `.resx` |
