---
name: mauimarkup-review
description: Audit a FmgLib.MauiMarkup codebase for correctness, performance, accessibility and maintainability — Build() safety, binding type-safety, theming bugs, list virtualization, design-system consistency and leftover XAML. Use when the user asks to review, audit, clean up, modernize or improve a C# markup MAUI project, or before shipping a MauiMarkup app.
license: MIT
---

# Reviewing a FmgLib.MauiMarkup Codebase

Requires the `mauimarkup` core skill.

Report findings **most severe first**, each with the file, the concrete failure it causes, and the fix.
Don't report style preferences as defects, and don't rewrite working code that merely reads
differently from your preference.

## Severity model

| Severity | Meaning |
|---|---|
| **Bug** | Wrong behavior at runtime today — silent bindings, theme values that never update, stacked event handlers, state loss |
| **Risk** | Correct today, fragile tomorrow — string paths, magic state names, unvirtualized lists that will grow |
| **Cleanup** | Duplication, inconsistency, missed shorthands |

## Pass 1 — `Build()` safety (highest yield)

`Build()` re-runs on every hot reload. Every violation below produces a real, reproducible bug during
development.

```bash
# view models constructed inside Build()
rg -n 'public (override )?void Build' -A 40 --type cs | rg 'new \w*(ViewModel|VM)\('

# subscriptions to long-lived objects inside Build()
rg -n 'Application\.Current.*\+=|Connectivity\..*\+=|Messenger\.|WeakReferenceMessenger' --type cs
```

Flag:
- a view model, service, timer or `CancellationTokenSource` created inside `Build()` → **Bug**: app
  state resets on every edit. Move it to a constructor field.
- `+=` on `Application.Current`, a static service, a messenger or a singleton VM inside `Build()` →
  **Bug**: handlers stack up, events fire N times. Move to the constructor.
- animations or `await`ed loads started directly in `Build()` → **Bug**: they restart on every reload.
  Move to `OnLoaded` / `OnAppearing`.
- `Assign(out _field)` where a local would do → **Risk**: stale references across rebuilds.

## Pass 2 — bindings

```bash
rg -n '\.Path\("' --type cs        # string paths
rg -n 'BindingMode\.TwoWay' --type cs
```

- `Path("…")` against a view model → **Risk**: a typo or rename fails silently. Convert to
  `Getter(static (VM vm) => vm.X)`. String paths remain fine for quick control-to-control wiring.
- `TwoWay` on a compiled binding with no `.Setter(...)` → **Bug**: edits never reach the source.
- A binding to a constant (`.Text(e => e.Path("Title"))` where `Title` never changes) → **Cleanup**:
  assign the value.
- A source that doesn't implement `INotifyPropertyChanged` → **Bug**: the UI never updates.
- Converters doing domain logic → **Cleanup**: a computed VM property is simpler and testable.
- Item templates using string paths → **Risk + perf**: templates are the hottest binding path.

## Pass 3 — theming

```bash
rg -n 'RequestedTheme|UserAppTheme' --type cs
```

- A ternary on the current theme (`isDark ? a : b`) → **Bug**: evaluated once, never follows a theme
  change. Use `.P(e => e.OnLight(a).OnDark(b))`.
- A nested builder inside a theme branch (`.OnDark(l => l.DynamicResource("X"))`) → **Risk**: resolves
  once at build time. Use plain values in both branches where the theme switches live.
- Hex literals scattered inline → **Cleanup**: centralize in `AppColors`.

## Pass 4 — visual states & styles

- A `VisualStateGroup` with no `Normal` state → **Bug**: properties don't restore when leaving a state.
- State names as string literals → **Risk**: use `VisualStates.Button.Pressed` etc.
- The same visual configuration repeated across controls → **Cleanup**: a `Style<T>` in a
  `ResourceDictionary`.
- Deep `BasedOn` chains, or styles defined inline per page → **Cleanup**: one `AppStyles` class.

## Pass 5 — lists

- `BindableLayout` bound to a collection that can grow beyond a few dozen → **Risk/Bug**: no
  virtualization; every item view is created immediately. Use `CollectionView`.
- A `CollectionView` inside a `ScrollView`, or in an `Auto` grid row → **Bug**: virtualization is lost
  and the list may not size. Give it a `Star` row.
- No `EmptyView` where the list can be empty → **Cleanup**.
- Deeply nested row templates → **Risk**: the cost multiplies per realized cell.
- `Frame` used for cards → **Cleanup**: `Border` is lighter and current.

## Pass 6 — structure

```bash
fd -e xaml            # leftover XAML in a markup project
rg -n 'InitializeComponent' --type cs
```

- Leftover `.xaml` / `.xaml.cs` / `InitializeComponent()` in a migrated project → **Cleanup** (or a
  deliberate hybrid — confirm before flagging).
- A `Build()` longer than ~120 lines → **Cleanup**: extract private `View`-returning methods or
  `ContentView` components. This is the largest readability gain the library offers.
- Repeated subtrees copy-pasted between pages → **Cleanup**: a shared `ContentView`.
- Pages with a view model but not deriving from `FmgLibContentPage<TViewModel>` → **Cleanup**: free
  typed `BindingContext`.
- Missing `global using FmgLib.MauiMarkup;` with the using repeated in every file → **Cleanup**.

## Pass 7 — accessibility

```bash
rg -n 'new (Image|ImageButton)\(\)' --type cs
```

- Meaningful images/icon-only buttons with no `SemanticDescription`, `AutomationName` or
  `ToolTipPropertiesText` → **Risk**: unusable with a screen reader.
- Headings without `SemanticHeadingLevel` → **Cleanup**.
- Tap targets below 44×44 → **Risk**: set `MinimumHeightRequest` / `MinimumWidthRequest`.
- Color used as the only signal for state (error, selection) → **Risk**.

## Pass 8 — localization (if present)

```bash
rg -n 'ToTranslate\(\)|Translator\.Instance\[' --type cs
```

- `"Key".ToTranslate()` or `Translator.Instance["Key"]` assigned to a **UI property** → **Bug**: a
  snapshot, so the text never updates on a language change. Use `.Text(e => e.Translate("Key"))`.
  (In `DisplayAlert` and log strings it is correct.)
- No `.FlowDirection(e => e.FromCulture())` while shipping an RTL language → **Bug**.
- JSON language file without `MauiAsset` build action → **Bug**: throws at startup.

## Pass 9 — generator hygiene

- `<MauiMarkupSourceGenerator>true</MauiMarkupSourceGenerator>` in a large solution → note the
  compile-time trade-off; explicit `[MauiMarkup]` attributes scope generation to what's used.
- Hand-written extensions duplicating what the generator would emit for a `BindableProperty` →
  **Cleanup**: annotate the type instead.
- Custom extensions writing the CLR property instead of `SetValue(XProperty, …)` → **Bug**: styles,
  triggers and bindings stop working on that property.
- Non-generic custom extensions returning a base type → **Risk**: breaks chains for derived controls.

## Output format

```
### Bug — MainPage.cs:42 — view model created inside Build()
`new MainPageViewModel()` runs on every hot reload, so all app state is lost on each edit.
Fix: assign it to a readonly field in the constructor and reference the field in Build().
```

Group by severity, keep each finding to a few lines, and lead the summary with the count per severity
so the user can triage at a glance.
