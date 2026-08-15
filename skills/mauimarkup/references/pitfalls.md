# FmgLib.MauiMarkup — Pitfalls & Review Checklist

## Compile errors

| Error | Cause / fix |
|---|---|
| "Cannot convert lambda expression…" on a property method | The builder lambda must **return** the chain: `e => e.Path("X")`, not `e => { e.Path("X"); }` |
| Ambiguous call between two markup libraries | `CommunityToolkit.Maui.Markup` and FmgLib in the same file. Both can live in one project — just not one file. Drop one `using` or fully qualify |
| CS0121 ambiguity on a third-party control property | The property is served by a base-class extension; don't add your own duplicate |
| Collection-initializer syntax fails on `MenuFlyout` / `Style<T>` / `VisualState<T>` | Fluent calls must follow the `{ … }` block: `new MenuFlyoutSubItem() { … }.Text("Submenu")` |
| `VisualStateGroup` rejects `{ … }` | It holds states in a `States` property. Use `VisualStateGroupList` or `.States(...)` |
| Method not found on a third-party control | Needs `[MauiMarkup(typeof(X))]` or `<MauiMarkupSourceGenerator>true</MauiMarkupSourceGenerator>` |

## Silent runtime failures

**A binding does nothing.**
1. `Path` string typo — switch to `e.Getter(...)` and the compiler catches it.
2. Wrong `BindingContext` — inside a template the context is the *item*; use `.Source(...)` to escape.
3. The source doesn't raise `INotifyPropertyChanged`.

**`Center()` doesn't center my text.** `Center()` positions the control in its parent;
`TextCenter()` aligns text inside the control. Full-width centered text needs
`.FillHorizontal().TextCenter()`.

**Theme values don't follow the OS theme.** A ternary is evaluated once. Use the builder:
`.TextColor(e => e.OnLight(a).OnDark(b))`.

**A nested theme builder stopped following the theme.** `.OnDark(l => l.DynamicResource("X"))` cannot
be carried by an `AppThemeBinding`; it resolves once at build time. Use plain values in both branches.

**Translations don't update on language change.** `"Key".ToTranslate()` and
`Translator.Instance["Key"]` return a snapshot. On-screen text needs `.Text(e => e.Translate("Key"))`.

**State resets while editing.** State lives in `Build()`; move it to fields set in the constructor.

**Handlers fire two, three, four times after edits.** A handler was attached to a long-lived object
(`Application.Current`, a static service, a singleton view model) inside `Build()` and stacked up over
rebuilds. Attach those in the constructor.

**A `VisualState` doesn't restore the previous look.** No `Normal` state defined.

## Hot reload

The handler activates only for pages implementing `IFmgLibHotReload` + calling
`InitializeHotReload()` (or deriving from `FmgLibContentPage`), and only where the runtime can deliver
metadata updates. `dotnet watch run -f <tfm>` is the most reliable channel; Rider's debugger cannot
deliver .NET Hot Reload to MAUI; VS Code needs `"csharp.experimental.debug.hotReload": true`. Full
matrix and diagnostics: `mauimarkup-hotreload`.

## Performance

- Direct values are plain `SetValue` — zero binding overhead. Don't bind constants.
- Compiled bindings (`Getter`) avoid reflection; use them in item templates especially.
- `CollectionView` virtualizes; `BindableLayout` creates every item view immediately — keep it to
  dozens of items.
- Animate transforms (`TranslationX/Y`, `Scale`, `Rotation`, `Opacity`), not layout properties.
  `Animate…RequestTo` relayouts each frame.
- `Auto` grid rows measure content every pass — don't wrap long lists in one.
- Automatic generator mode (`<MauiMarkupSourceGenerator>true</...>`) generates for every referenced
  control. If compile times grow, switch to explicit `[MauiMarkup]` attributes.
- Share stateless `IValueConverter` instances via `static readonly` fields.

## Review checklist for FmgLib.MauiMarkup code

Structure
- [ ] One page = one `.cs` file; no leftover `.xaml`/`.xaml.cs`/`InitializeComponent()`.
- [ ] UI in `Build()`, state in fields set in the constructor, logic in a view model.
- [ ] Repeated subtrees extracted into private methods or `ContentView` components.
- [ ] Design tokens centralized (`AppColors`, `AppStyles`) rather than hex literals scattered inline.

`Build()` safety
- [ ] No view-model construction inside `Build()`.
- [ ] No subscriptions to long-lived/static events inside `Build()`.
- [ ] No animations or network calls started directly in `Build()` — use `OnLoaded`/`OnAppearing`.
- [ ] `Assign` targets are locals declared inside `Build()`, not fields, wherever possible.

Bindings
- [ ] View-model paths use `Getter` (compiled), not `Path` strings.
- [ ] Two-way compiled bindings have a matching `Setter`.
- [ ] Constants are assigned, not bound.
- [ ] Converter logic that is really view-model logic lives on the view model.

Styling
- [ ] Theme-dependent values use `OnLight`/`OnDark`, not ternaries on the current theme.
- [ ] Visual state groups define `Normal`.
- [ ] State names come from `VisualStates.*`, not string literals.
- [ ] Shared visuals live in a `Style<T>` in a `ResourceDictionary`, not repeated per control.

Lists
- [ ] Long/dynamic lists use `CollectionView`, not `BindableLayout`.
- [ ] Item templates use compiled bindings.
- [ ] An `EmptyView` is provided where the list can be empty.

Accessibility
- [ ] Meaningful images have `SemanticDescription`; decorative ones are excluded.
- [ ] Headings carry `SemanticHeadingLevel`.
- [ ] Icon-only buttons have `AutomationName` or `ToolTipPropertiesText`.

## FAQ

**Can XAML and FmgLib pages coexist?** Yes, per page — migrate incrementally.

**Does it work with Shell, DI, Essentials, CommunityToolkit.Mvvm?** Yes. The library changes *how you
construct views*, nothing else. `ObservableObject` / `RelayCommand` bind normally.

**How do I set something the library doesn't cover?** `InvokeOnElement(x => …)`, or plain C# on a
captured reference — views are ordinary objects.

**Which .NET versions?** The current package line targets .NET 10 (MAUI 10); the template scaffolds
.NET 9 or 10 via `--netMajor`.
