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
- Registers the page with the hot-reload handler — **always**, via a weak reference (a no-op in production where no update ever arrives, and never extends the page's lifetime). When the runtime applies a code update, `Build()` is re-invoked **on the main thread** for every registered page (default `RebuildAllOnUpdate = true` — the only reliable mode, because pages compose helper classes/styles whose edits must refresh them, and the Mono runtime on iOS/Android often reports an empty updated-type list).
- Every update logs a diagnostic line to the debug output: `FmgLib.MauiMarkup hot reload: update received (types: …) — rebuilding N registered target(s).` — if you see this line, the pipeline is working end-to-end.
- A `Build()` that throws during a reload **never crashes the app**: the failure is logged (`Trace`) and surfaced through the `FmgLibHotReloadHandler.ReloadFailed` event, so you can fix the edit and save again.

### Memory safety

Hot-reload registration uses **weak references** (`FmgLibHotReloadHandler.Register`): registering a page never extends its lifetime. When a page is popped from navigation and released, it is garbage-collected normally and its registration is pruned automatically — leak detectors (e.g. Nalu's) will not report pages pinned by hot reload. Registering the same instance twice is a no-op, and an explicit `FmgLibHotReloadHandler.Unregister(page)` exists if you ever want to opt a live page out of rebuilds.

> If you subscribe to the raw `FmgLibHotReloadHandler.UpdateApplicationEvent` yourself, note that it is an ordinary .NET event: subscribers are held strongly and must unsubscribe themselves. Prefer `Register`.

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

Takes the view model in the constructor, assigns it to `BindingContext` **before the first `Build()` runs** (so `Build()` can safely read the view model), and — the nice part — **re-types the `BindingContext` property** so you access your VM without casting:

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

## IDE / Channel Support Matrix

FmgLib's handler sits on top of **.NET Hot Reload** (`MetadataUpdateHandler`), so it works wherever the .NET tooling can deliver code updates to the running process:

| Channel | Windows | macOS | Notes |
|---|---|---|---|
| Visual Studio (F5 debug) | ✅ | — (retired) | Full support, updates delivered with type list |
| VS Code + C# Dev Kit / .NET MAUI extension | ✅ | ✅ | **Requires** `"csharp.experimental.debug.hotReload": true` in settings when debugging — without it F5 delivers no updates at all |
| `dotnet watch run` (CLI, any editor) | ✅ | ✅ | No debugger needed — the most IDE-independent, most reliable path |
| Rider (debugger) | ❌ | ❌ | Rider's debugger does not deliver .NET Hot Reload for MAUI — use a `dotnet watch` run configuration inside Rider instead |
| Plain `dotnet run` / Release build | ❌ | ❌ | No update channel exists — by design, zero overhead |

> **Self-diagnosis:** if a debugger is attached but the process cannot receive updates, the library logs a one-time warning to the debug output (`FmgLib.MauiMarkup hot reload: … MetadataUpdater.IsSupported = false …`) telling you exactly which setting/channel to fix. If you see that message, edits will never apply in the current session regardless of what you change.

Platform notes:

- **Android emulator/device (debug)**: supported through all channels above.
- **iOS / Mac Catalyst (debug)**: .NET Hot Reload requires the Mono **interpreter**, which MAUI enables by default in Debug configuration (`UseInterpreter`). If you disabled it, re-enable it for Debug.
- **What decides support at runtime** is `System.Reflection.Metadata.MetadataUpdater.IsSupported` (the tooling sets `DOTNET_MODIFIABLE_ASSEMBLIES=debug` when launching). You can check `FmgLibHotReloadHandler.IsSupported` yourself, e.g. to show a dev-only banner.

## Handler Options & Diagnostics

```csharp
// Default is TRUE: every registered page rebuilds on any update. Opt into targeted
// rebuilds (only pages whose own/base type changed) if you prefer:
FmgLibHotReloadHandler.RebuildAllOnUpdate = false;

// Observe reload failures (already logged via Trace; never crash the app):
FmgLibHotReloadHandler.ReloadFailed += (target, ex) =>
    Console.WriteLine($"Hot reload failed for {target.GetType().Name}: {ex.Message}");

// Rescue hatch: force-rebuild every registered page NOW — for tooling that applies
// code updates without notifying the runtime handlers. Wire it to a debug-only
// gesture/button:
FmgLibHotReloadHandler.RebuildAll();
```

Example — a debug-only refresh gesture on a page:

```csharp
#if DEBUG
this.GestureRecognizers(
    new TapGestureRecognizer()
        .NumberOfTapsRequired(3)
        .OnTapped((s, e) => FmgLibHotReloadHandler.RebuildAll()));
#endif
```

Targeted-mode matching rules: an update rebuilds a registered target when the runtime reports its exact type, **any of its base types** (editing a shared base page refreshes all derived pages), or an unknown/empty type list.

> Only types implementing `IFmgLibHotReload` (and calling `InitializeHotReload`, or deriving from `FmgLibContentPage`) rebuild themselves. If an edit to your `AppShell` or a plain `ContentView` doesn't appear, give that type the same treatment.

## One-Keypress Dev Loop in VS Code (recommended)

Projects created from the `fmglib-mauimarkup-app` template ship a ready `.vscode/tasks.json`. For existing projects, add:

```jsonc
// .vscode/tasks.json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "🔥 Hot Reload: iOS Simulator",
            "type": "shell",
            "command": "dotnet",
            "args": [ "watch", "run", "-f", "net10.0-ios" ],
            "isBackground": true,
            "problemMatcher": []
        },
        {
            "label": "🔥 Hot Reload: Android",
            "type": "shell",
            "command": "dotnet",
            "args": [ "watch", "run", "-f", "net10.0-android" ],
            "isBackground": true,
            "problemMatcher": []
        }
    ]
}
```

Run the task (`Terminal → Run Task…`), edit `Build()`, save — the change is on the device in seconds, and the terminal shows both `dotnet watch 🔥 … applied` and the library's `update received … rebuilding N target(s)` confirmation. Use F5 debugging separately when you need breakpoints.

## Troubleshooting

| Symptom | Cause / fix |
|---|---|
| Edits don't appear (VS Code, debugging) | First check the Debug Console for `FmgLib.MauiMarkup hot reload: update received …`. **If the line is absent**, the extension applied the changes without notifying the app's metadata update handlers (a known gap of the debug-launch channel on Mono/mobile targets — `Hot Reload result: {"result":0, …}` with all-empty arrays is a tell). Fix on your side is not possible from the library: iterate with `dotnet watch run` (fully supported), or trigger `FmgLibHotReloadHandler.RebuildAll()` from a debug-only gesture to render already-applied edits. Also ensure `"csharp.experimental.debug.hotReload": true` is set. |
| Edits don't appear (Rider, debugging) | Rider's debugger cannot apply .NET Hot Reload to MAUI apps. Create a `dotnet watch run -f <tfm>` run configuration (works with the emulator/simulator) — our handler supports watch without a debugger. |
| Edits don't appear (other) | Run through a hot-reload channel: IDE debug/run with hot reload, or `dotnet watch run -f net10.0-…`. Plain `dotnet run` has no update channel. Verify with `FmgLibHotReloadHandler.IsSupported`. |
| Edits to a helper `ContentView`/method don't refresh the page using it | Either implement `IFmgLibHotReload` on the component (it rebuilds itself), or set `FmgLibHotReloadHandler.RebuildAllOnUpdate = true`. |
| UI resets state on edit | State lives in `Build()`; move it to fields/constructor. |
| Duplicate event firing after edits | A handler was attached to a long-lived object inside `Build()`; move it to the constructor. |
| "Rude edit" messages from the IDE | Some code changes (e.g. changing method signatures, adding fields to certain types) exceed .NET Hot Reload's capability — restart the session. This is a runtime/tooling limit, not a library one. |
| App used to crash mid-session after a bad edit | No longer: exceptions thrown by `Build()` during a reload are caught, logged and reported via `ReloadFailed`. |
| iOS device edits ignored | Ensure Debug uses the Mono interpreter (MAUI default) and the session was started by the IDE/watch, not a plain install. |

## Related Topics

- [Getting Started](getting-started.md)
- [Assign & References](assign-and-references.md)
- [Event Handlers](event-handlers.md)
