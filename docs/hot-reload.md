# .NET Hot Reload Support

Because FmgLib.MauiMarkup UIs are plain C#, .NET's built-in Hot Reload applies to them — the library adds a small handler that **re-runs your UI construction when code changes**, so edits appear on the running app without restarting.

## The Pattern: `IFmgLibHotReload` + `Build()`

1. Implement `IFmgLibHotReload` on the page (it declares a single `void Build()` method).
2. Call `this.InitializeHotReload()` in the constructor.
3. Put all UI construction in `Build()`.

```csharp
public partial class ExamplePage : ContentPage, IFmgLibHotReload
{
    public ExamplePage()
    {
        this.InitializeHotReload();
    }

    public void Build()
    {
        this
        .Content(
            new Label()
            .Text("FmgLib.MauiMarkup")
            .CharacterSpacing(2)
            .FontSize(30)
            .FontAttributes(FontAttributes.Italic)
            .TextColor(Colors.Green)
            .TextCenter()
        );
    }
}
```

### What `InitializeHotReload()` does

- Calls `Build()` once immediately (initial construction).
- **Only when a debugger is attached**, subscribes to the hot-reload notification; when the runtime applies a code update, `Build()` is re-invoked **on the main thread** — for the edited page type, or for all pages when the update's affected types are unknown.
- In release builds / without a debugger, it is just a single `Build()` call with zero overhead.

Edit `Build()` (or anything it calls) while debugging, save/apply, and the page redraws.

## Ready-Made Base Classes

The library ships base pages that wire this up for you.

### `FmgLibContentPage`

```csharp
public class HomePage : FmgLibContentPage
{
    public override void Build() =>
        this.Content(
            new Label().Text("Hello!").Center()
        );
}
```

No constructor plumbing — the base class subscribes to hot reload (debugger-only) and calls `Build()`.

### `FmgLibContentPage<TViewModel>` — MVVM base

Takes the view model in the constructor, assigns it to `BindingContext`, and — the nice part — **re-types the `BindingContext` property** so you access your VM without casting:

```csharp
public class ProfilePage : FmgLibContentPage<ProfileViewModel>
{
    public ProfilePage(ProfileViewModel vm) : base(vm) { }

    public override void Build() =>
        this.Content(
            new VerticalStackLayout()
            .Padding(20)
            .Children(
                new Label().Text(e => e.Getter(static (ProfileViewModel v) => v.UserName)),

                new Button()
                    .Text("Refresh")
                    .Command(BindingContext.RefreshCommand)   // typed! no cast
            )
        );
}
```

Pairs naturally with dependency injection:

```csharp
// MauiProgram.cs
builder.Services.AddTransient<ProfileViewModel>();
builder.Services.AddTransient<ProfilePage>();
```

## Rules for a Reload-Safe `Build()`

`Build()` may run **many times** during a debug session. Structure it accordingly:

**✅ Do**

- Make `Build()` idempotent: it should fully describe the UI from scratch each time (setting `Content` replaces the old tree, so this is the natural style).
- Keep state (counters, view models, service references) in **fields initialized in the constructor**, not in `Build()`:

  ```csharp
  private readonly MainPageViewModel viewModel;

  public MainPage()
  {
      viewModel = new MainPageViewModel();   // survives reloads
      this.InitializeHotReload();
  }

  public void Build() => this.BindingContext(viewModel).Content(/* ... */);
  ```

- Capture control references with `Assign` into locals inside `Build()` — they are recreated consistently on each run.

**❌ Avoid**

- Creating the view model inside `Build()` — you'd reset app state on every reload.
- Subscribing to **long-lived/static events** inside `Build()` (e.g. `Application.Current.RequestedThemeChanged += …`) — subscriptions stack up on each rebuild. Do that in the constructor.
- Kicking off animations or network calls directly in `Build()`; use `OnLoaded`/`OnAppearing` [event handlers](event-handlers.md).

## Works for Any View, Not Just Pages

`IFmgLibHotReload` + `InitializeHotReload()` is equally valid on a `ContentView`:

```csharp
public class ProductCard : ContentView, IFmgLibHotReload
{
    public ProductCard() => this.InitializeHotReload();

    public void Build() => this.Content(/* ... */);
}
```

## Troubleshooting

| Symptom | Cause / fix |
|---|---|
| Edits don't appear | Hot reload requires a **debug session with the debugger attached** (`Debugger.IsAttached` gate). Plain `dotnet run` without a debugger won't trigger rebuilds. |
| UI resets state on edit | State lives in `Build()`; move it to fields/constructor. |
| Duplicate event firing after edits | A handler was attached to a long-lived object inside `Build()`; move it to the constructor. |
| "Rude edit" messages from the IDE | Some code changes (e.g. changing method signatures) exceed hot reload's capability — restart the session. |

## Related Topics

- [Getting Started](getting-started.md)
- [Assign & References](assign-and-references.md)
- [Event Handlers](event-handlers.md)
