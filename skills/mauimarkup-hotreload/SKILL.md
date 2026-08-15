---
name: mauimarkup-hotreload
description: Set up and debug the FmgLib.MauiMarkup hot reload loop — IFmgLibHotReload, InitializeHotReload(), FmgLibContentPage, FmgLibHotReloadHandler options, RebuildAll, ReloadFailed, dotnet watch vs IDE channels, VS Code and Rider limits, and writing a reload-safe Build(). Use when UI edits don't appear in the running app, when state resets or handlers fire twice after an edit, or when configuring the dev loop for a C# markup MAUI project.
license: MIT
---

# Hot Reload

Requires the `mauimarkup` core skill.

Because the UI is plain C#, .NET Hot Reload applies to it. The library adds a `MetadataUpdateHandler`
that **re-runs your UI construction** when code changes, so edits appear without restarting the app.

## The pattern

```csharp
public partial class ExamplePage : ContentPage, IFmgLibHotReload
{
    public ExamplePage() => this.InitializeHotReload();

    public void Build() => this.Content(/* … */);
}
```

`InitializeHotReload()`:

- calls `Build()` once immediately (initial construction);
- registers the page with the handler through a **weak reference** — registration never extends the
  page's lifetime, popped pages are collected normally, and leak detectors (e.g. Nalu's) stay quiet.
  Registering twice is a no-op; `FmgLibHotReloadHandler.Unregister(page)` opts a live page out;
- on an update, re-invokes `Build()` **on the main thread** for every registered target;
- logs one diagnostic line per update:
  `FmgLib.MauiMarkup hot reload: update received (types: …) — rebuilding N registered target(s).`
  **If you see that line, the pipeline works end to end** — start every diagnosis there.

A `Build()` that throws during a reload never crashes the app: the failure is logged via `Trace` and
surfaced through `FmgLibHotReloadHandler.ReloadFailed`.

Ready-made bases: `FmgLibContentPage` (override `Build()`) and `FmgLibContentPage<TViewModel>` (typed
`BindingContext`, VM assigned before the first `Build()`). It works on **any view**, not just pages —
give a `ContentView` the same treatment and it rebuilds itself.

## Writing a reload-safe `Build()`

`Build()` runs many times per session.

| Do | Don't |
|---|---|
| Describe the UI from scratch each call (setting `Content` replaces the tree, so this is natural) | Mutate the previous tree incrementally |
| Keep view models, counters and services in **fields set in the constructor** | `new MyViewModel()` inside `Build()` — every edit resets app state |
| `Assign(out var x)` into locals declared inside `Build()` | Depend on `Assign`ed **fields** surviving a rebuild |
| Subscribe to long-lived/static events in the constructor | `Application.Current.RequestedThemeChanged += …` in `Build()` — subscriptions stack per rebuild |
| Start animations/network calls from `OnLoaded` / `OnAppearing` | Start them directly in `Build()` |

```csharp
private readonly MainPageViewModel viewModel;

public MainPage()
{
    viewModel = new MainPageViewModel();     // survives every reload
    this.InitializeHotReload();
}

public void Build() => this.BindingContext(viewModel).Content(/* … */);
```

Handlers attached with `On<Event>` to controls created inside `Build()` are safe — the old controls are
discarded with their subscriptions.

## Channel support

The handler sits on .NET Hot Reload, so it works wherever the tooling delivers updates to the process:

| Channel | Windows | macOS | Notes |
|---|---|---|---|
| Visual Studio (F5) | ✅ | — | full support, updates carry a type list |
| VS Code + C# Dev Kit / .NET MAUI ext. | ✅ | ✅ | **requires** `"csharp.experimental.debug.hotReload": true`; without it F5 delivers nothing |
| `dotnet watch run` (any editor) | ✅ | ✅ | no debugger needed — **the most reliable path** |
| Rider (debugger) | ❌ | ❌ | Rider's debugger does not deliver .NET Hot Reload for MAUI — use a `dotnet watch` run configuration |
| plain `dotnet run` / Release | ❌ | ❌ | no update channel exists, by design |

Platform notes:

- **iOS / Mac Catalyst (debug)** need the Mono **interpreter**, which MAUI enables by default in Debug
  (`UseInterpreter`). If it was disabled, re-enable it for Debug.
- What decides support at runtime is `MetadataUpdater.IsSupported`; check
  `FmgLibHotReloadHandler.IsSupported` yourself (e.g. a dev-only banner). If a debugger is attached but
  updates can't arrive, the library logs a one-time warning naming the setting to fix — when you see it,
  edits will never apply in that session regardless of what else you change.

## The recommended dev loop (VS Code)

Template projects ship `.vscode/tasks.json`. For an existing project:

```jsonc
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "🔥 Hot Reload: iOS Simulator",
            "type": "shell",
            "command": "dotnet",
            "args": ["watch", "run", "-f", "net10.0-ios"],
            "isBackground": true,
            "problemMatcher": []
        },
        {
            "label": "🔥 Hot Reload: Android",
            "type": "shell",
            "command": "dotnet",
            "args": ["watch", "run", "-f", "net10.0-android"],
            "isBackground": true,
            "problemMatcher": []
        }
    ]
}
```

Run the task, edit `Build()`, save — the terminal shows both `dotnet watch 🔥 … applied` and the
library's `update received … rebuilding N target(s)`. Use F5 separately when you need breakpoints.

## Handler options

```csharp
// Default TRUE: every registered target rebuilds on any update. This is the reliable mode, because
// pages compose helper classes and styles whose edits must refresh them, and Mono on iOS/Android
// often reports an empty updated-type list.
FmgLibHotReloadHandler.RebuildAllOnUpdate = false;   // opt into targeted rebuilds

FmgLibHotReloadHandler.ReloadFailed += (target, ex) =>
    Console.WriteLine($"Hot reload failed for {target.GetType().Name}: {ex.Message}");

FmgLibHotReloadHandler.RebuildAll();                 // force every registered target to rebuild now
```

Targeted mode rebuilds a target when the runtime reports its exact type, **any base type** (so editing a
shared base page refreshes all derived pages), or an unknown/empty type list.

A debug-only rescue gesture:

```csharp
#if DEBUG
this.GestureRecognizers(
    new TapGestureRecognizer().NumberOfTapsRequired(3)
        .OnTapped((s, e) => FmgLibHotReloadHandler.RebuildAll()));
#endif
```

> If you subscribe to the raw `FmgLibHotReloadHandler.UpdateApplicationEvent`, note it is an ordinary
> .NET event — subscribers are held strongly and must unsubscribe. Prefer `Register`.

## Troubleshooting

| Symptom | Cause / fix |
|---|---|
| Edits don't appear (VS Code, debugging) | Check the Debug Console for `update received …`. **If absent**, the extension applied changes without notifying metadata update handlers (a known gap of the debug-launch channel on Mono/mobile; `Hot Reload result: {"result":0, …}` with all-empty arrays is the tell). Not fixable from the library: use `dotnet watch run`, or wire `RebuildAll()` to a debug gesture. Also set `"csharp.experimental.debug.hotReload": true` |
| Edits don't appear (Rider, debugging) | Rider's debugger can't apply .NET Hot Reload to MAUI. Create a `dotnet watch run -f <tfm>` run configuration |
| Edits don't appear (other) | Verify the channel and `FmgLibHotReloadHandler.IsSupported`. Plain `dotnet run` has no update channel |
| A helper `ContentView` or method edit doesn't refresh the page | Implement `IFmgLibHotReload` on the component, or keep `RebuildAllOnUpdate = true` |
| `AppShell` edits ignored | Give `AppShell` the same `IFmgLibHotReload` + `InitializeHotReload()` treatment |
| UI resets state on edit | State lives in `Build()` — move it to fields/constructor |
| Duplicate event firing after edits | A handler was attached to a long-lived object inside `Build()` — move it to the constructor |
| "Rude edit" from the IDE | Some changes (method signatures, adding fields to certain types) exceed .NET Hot Reload's capability — restart the session. Runtime limit, not a library one |
| iOS device edits ignored | Ensure Debug uses the Mono interpreter and the session was started by the IDE/watch, not a plain install |
