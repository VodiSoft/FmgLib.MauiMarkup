# Tips & Troubleshooting

Practical answers to the questions that come up most when working with FmgLib.MauiMarkup.

## Naming & Discoverability

**"How do I guess a method name?"**

- Property `Foo` → method `.Foo(value)`. Always.
- Event `Bar` → `.OnBar(handler)` or `.OnBar(sender => …)`.
- Attached property `Owner.Prop` → `.OwnerProp(...)` (`Shell.TitleColor` → `ShellTitleColor`), except Grid placement which drops the prefix (`Grid.Row` → `Row`). Full table: [Attached Properties](attached-properties.md).
- Property hidden with `new` in a subclass → `.PropNameNew(...)` ([details](third-party-controls.md#naming-collision-rule-propertyname--new)).

**"A method I expect doesn't exist."**

1. Check the `using FmgLib.MauiMarkup;` (or global using) is present.
2. Is the property a real `BindableProperty`? Plain CLR properties don't get fluent methods — use [`InvokeOnElement`](assign-and-references.md) or a [custom extension](custom-extension-methods.md).
3. Third-party control? It needs `[MauiMarkup(typeof(...))]` or automatic generator mode ([Third-Party Controls](third-party-controls.md)).
4. Check for the `New` suffix (hidden properties).

## Common Compile Errors

**"Ambiguous call between FmgLib method and another markup library"** — remove other fluent-markup packages (e.g. `CommunityToolkit.Maui.Markup`) from the same file's usings, or fully qualify. Mixing both libraries in one project works, but avoid mixing in one file.

**"Cannot convert lambda expression…" on a property method** — the builder lambda must **return** the builder chain: `e => e.Path("X")` (expression), not `e => { e.Path("X"); }` (statement returning void).

**Collection initializer syntax fails on `MenuFlyout` / `Style<T>` / `VisualState<T>`** — fluent calls must come **after** the `{ … }` initializer block:

```csharp
new MenuFlyoutSubItem() { /* items */ }.Text("Submenu")   // ✅
```

## Runtime Gotchas

**Binding silently does nothing**

- `Path` string typo — prefer [compiled bindings](compiled-bindings.md) (`e.Getter(...)`) to make these compile errors.
- Wrong `BindingContext` — remember templates rebind to the item; use `.Source(...)` to escape ([Property Bindings](data-binding.md)).
- The bound object doesn't raise `INotifyPropertyChanged`.

**`Center()` doesn't center my text** — `Center()` positions the control in its parent; `TextCenter()` aligns text inside the control. See [Layout Options](layout-options.md) vs [Text Alignment](text-alignment.md).

**Theme values don't update on OS theme change** — make sure you used the builder (`.TextColor(e => e.OnLight(...).OnDark(...))`), not a ternary evaluated once (`.TextColor(isDark ? … : …)`).

**Hot reload doesn't refresh the page** — the handler only activates when a **debugger is attached**, and only pages implementing `IFmgLibHotReload` + calling `InitializeHotReload()` rebuild. Full checklist: [Hot Reload](hot-reload.md#troubleshooting).

**State resets while editing with hot reload** — move state out of `Build()` into fields/constructor.

**Duplicate event handlers after hot reload** — handlers attached to long-lived objects inside `Build()` accumulate; attach them in the constructor.

## Architecture Recommendations

- **One page = one class**, UI in `Build()`, state in fields, logic in a view model. The [`FmgLibContentPage<TViewModel>`](hot-reload.md#fmglibcontentpagetviewmodel--mvvm-base) base gives you a typed `BindingContext`.
- **Extract repeated subtrees** into private methods or `ContentView` components — this is the biggest readability win over XAML ([Complete Examples](complete-examples.md#4-reusable-component--custom-contentview-with-bindable-properties)).
- **Centralize design tokens**: a static `AppColors` / `AppStyles` class plus app-level `ResourceDictionary` ([Styling](styling.md)).
- **Prefer compiled bindings** for all view-model paths; keep string paths only for quick control-to-control wiring.
- **Choose the right reaction tool**: constant → direct value; VM-driven → binding; state-driven visuals → [visual states](visual-states.md); condition-driven properties → [triggers](triggers.md); reusable control logic → [behaviors](behaviors.md); one-off → [event handler](event-handlers.md).

## Performance Notes

- Direct values (`.FontSize(14)`) are plain `SetValue` calls — zero binding overhead. Don't bind what never changes.
- [Compiled bindings](compiled-bindings.md) avoid reflection; use them in item templates especially.
- `CollectionView` virtualizes; [`BindableLayout`](collections-and-templates.md#bindablelayout--templated-items-in-any-layout) does not — keep it small.
- Animate transforms (`Translation`, `Scale`, `Opacity`) rather than layout properties when possible ([Animations](animations.md#performance-tips)).
- In automatic generator mode, generation covers every referenced control — switch to explicit `[MauiMarkup]` attributes if compile times grow.

## FAQ

**Can I mix XAML and FmgLib pages?** Yes — per page. Migrate incrementally.

**Does it work with Shell/Navigation/DI/Essentials?** Yes; the library only changes *how you construct views*, not app architecture. See [Shell Applications](shell-navigation.md).

**Do CommunityToolkit.Mvvm, `ObservableObject`, `RelayCommand` work?** Fully — bind with `e.Path(...)`/`e.Getter(...)` and `.Command(...)` as usual.

**How do I set a property the library doesn't cover?** [`InvokeOnElement`](assign-and-references.md#invokeonelement--arbitrary-code-mid-chain), or plain C# on the captured reference.

**Where are the real-world samples?** In the repo's [`sample/`](../sample) directory — complete apps including games and shop-style UIs.

**Which .NET versions are supported?** The current package line targets .NET 10 (MAUI 10); the [project template](getting-started.md) can scaffold .NET 9 or 10 via `--netMajor`.

## Related Topics

- [Getting Started](getting-started.md)
- [Complete Examples](complete-examples.md)
