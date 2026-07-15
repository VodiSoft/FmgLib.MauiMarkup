[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(FmgLib.MauiMarkup.FmgLibHotReloadHandler))]
namespace FmgLib.MauiMarkup;

using System.Diagnostics;
using System.Reflection.Metadata;

public static class FmgLibHotReloadHandler
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    /// <summary>
    /// Raised when the runtime applies a hot reload update.
    /// Prefer <see cref="Register"/> for page rebuilds: subscribers of this event are held
    /// with strong references and are responsible for unsubscribing themselves.
    /// </summary>
    public static event Action<Type[]?>? UpdateApplicationEvent;

    /// <summary>
    /// Raised when a registered target's <see cref="IFmgLibHotReload.Build"/> throws during a
    /// hot reload rebuild. The failure is logged and swallowed so a bad edit never crashes the
    /// running debug session; hook this event for custom diagnostics.
    /// </summary>
    public static event Action<IFmgLibHotReload, Exception>? ReloadFailed;

    /// <summary>
    /// Whether the current process can receive .NET hot reload updates.
    /// <see cref="MetadataUpdater.IsSupported"/> is <see langword="true"/> for every official
    /// delivery channel — Visual Studio, VS Code (C# Dev Kit), <c>dotnet watch</c> —
    /// with or without a debugger. The debugger check is kept as a conservative fallback.
    /// </summary>
    public static bool IsSupported => MetadataUpdater.IsSupported || Debugger.IsAttached;

    /// <summary>
    /// When <see langword="true"/> (the default), every registered target is rebuilt on any hot
    /// reload update, regardless of which types the runtime reports as changed. This is the only
    /// reliable mode in practice: pages compose helper classes, static UI methods, styles and
    /// converters whose edits must refresh the pages using them, and on the Mono runtime
    /// (iOS/Android) the update handler frequently receives an empty type list. Set to
    /// <see langword="false"/> to rebuild only targets whose own (or base) type was updated.
    /// </summary>
    public static bool RebuildAllOnUpdate { get; set; } = true;

    private static readonly List<WeakReference<IFmgLibHotReload>> subscribers = new();
    private static readonly object gate = new();
    private static bool warnedUpdatesUnavailable;

    /// <summary>
    /// Test hook: replaces the main-thread dispatcher used to run rebuilds.
    /// </summary>
    internal static Action<Action>? DispatchOverride;

    /// <summary>
    /// Test/diagnostic hook: number of live registered targets (dead entries pruned).
    /// </summary>
    internal static int LiveSubscriberCount
    {
        get
        {
            lock (gate)
            {
                subscribers.RemoveAll(w => !w.TryGetTarget(out _));
                return subscribers.Count;
            }
        }
    }

    /// <summary>
    /// Emits a one-time diagnostic when a debugger is attached but the runtime cannot receive
    /// .NET hot reload updates — the most common "hot reload silently does nothing" scenario
    /// (VS Code without the hot reload flag, Rider's debugger, plain launches).
    /// </summary>
    private static void WarnIfUpdatesUnavailable()
    {
        if (warnedUpdatesUnavailable || MetadataUpdater.IsSupported || !Debugger.IsAttached)
        {
            return;
        }

        warnedUpdatesUnavailable = true;
        Trace.WriteLine(
            "FmgLib.MauiMarkup hot reload: pages are registered, but this process cannot receive " +
            ".NET hot reload updates (MetadataUpdater.IsSupported = false — the launcher did not set " +
            "DOTNET_MODIFIABLE_ASSEMBLIES=debug), so Build() will never re-run on edits. " +
            "VS Code: set \"csharp.experimental.debug.hotReload\": true in settings and restart debugging. " +
            "Rider: its debugger does not deliver .NET Hot Reload for MAUI — use a 'dotnet watch' run configuration. " +
            "CLI (works with any editor): dotnet watch run -f <target-framework>.");
    }

    /// <summary>
    /// Registers a page/view so its <see cref="IFmgLibHotReload.Build"/> method is re-invoked
    /// (on the main thread) when a hot reload update arrives.
    /// The target is tracked with a weak reference: registration never keeps the page alive,
    /// so navigated-away pages remain eligible for garbage collection.
    /// Registering the same instance twice is a no-op.
    /// </summary>
    /// <param name="target">The page or view to rebuild on hot reload.</param>
    public static void Register(IFmgLibHotReload target)
    {
        if (target is null)
        {
            return;
        }

        WarnIfUpdatesUnavailable();

        lock (gate)
        {
            for (var i = subscribers.Count - 1; i >= 0; i--)
            {
                if (!subscribers[i].TryGetTarget(out var existing))
                {
                    subscribers.RemoveAt(i);
                }
                else if (ReferenceEquals(existing, target))
                {
                    return;
                }
            }

            subscribers.Add(new WeakReference<IFmgLibHotReload>(target));
        }
    }

    /// <summary>
    /// Rebuilds every registered target immediately (on the main thread), exactly as if a hot
    /// reload update had arrived. Rescue hatch for tooling channels that apply code updates to
    /// the process but never invoke the metadata update handlers: wire it to a debug-only
    /// gesture or toolbar button to force the UI to reflect already-applied edits.
    /// </summary>
    public static void RebuildAll()
    {
        Trace.WriteLine("FmgLib.MauiMarkup hot reload: manual RebuildAll() requested.");
        RebuildTargets(CollectTargets(types: null));
    }

    /// <summary>
    /// Removes a previously registered target. Optional: dead targets are pruned automatically.
    /// </summary>
    /// <param name="target">The page or view to stop rebuilding on hot reload.</param>
    public static void Unregister(IFmgLibHotReload target)
    {
        if (target is null)
        {
            return;
        }

        lock (gate)
        {
            for (var i = subscribers.Count - 1; i >= 0; i--)
            {
                if (!subscribers[i].TryGetTarget(out var existing) || ReferenceEquals(existing, target))
                {
                    subscribers.RemoveAt(i);
                }
            }
        }
    }

    /// <summary>
    /// Executes the <c>ClearCache</c> operation.
    /// </summary>
    /// <param name="types">The value used for <paramref name="types"/>.</param>
    internal static void ClearCache(Type[]? types) { }

    /// <summary>
    /// Executes the <c>UpdateApplication</c> operation.
    /// </summary>
    /// <param name="types">The value used for <paramref name="types"/>.</param>
    internal static void UpdateApplication(Type[]? types)
    {
        try
        {
            UpdateApplicationEvent?.Invoke(types);
        }
        catch (Exception exception)
        {
            Trace.TraceError($"FmgLib.MauiMarkup hot reload: an UpdateApplicationEvent subscriber threw: {exception}");
        }

        var targets = CollectTargets(types);

        var typeSummary = types == null ? "unknown" : types.Length == 0 ? "empty" : string.Join(", ", types.Select(t => t.Name));
        Trace.WriteLine($"FmgLib.MauiMarkup hot reload: update received (types: {typeSummary}) — rebuilding {targets?.Count ?? 0} registered target(s).");

        RebuildTargets(targets);
    }

    /// <summary>
    /// Snapshots the live registered targets affected by the given update (pruning dead entries),
    /// in registration order.
    /// </summary>
    /// <param name="types">The types the runtime reports as updated; <see langword="null"/> means all.</param>
    /// <returns>The targets to rebuild, or <see langword="null"/> when there are none.</returns>
    private static List<IFmgLibHotReload>? CollectTargets(Type[]? types)
    {
        List<IFmgLibHotReload>? targets = null;

        lock (gate)
        {
            for (var i = subscribers.Count - 1; i >= 0; i--)
            {
                if (!subscribers[i].TryGetTarget(out var target))
                {
                    subscribers.RemoveAt(i);
                    continue;
                }

                if (ShouldRebuild(target, types))
                {
                    (targets ??= new List<IFmgLibHotReload>()).Insert(0, target);
                }
            }
        }

        return targets;
    }

    /// <summary>
    /// Dispatches <see cref="IFmgLibHotReload.Build"/> for each target to the main thread,
    /// containing per-target failures so one broken rebuild never affects the others or the app.
    /// </summary>
    /// <param name="targets">The targets to rebuild; may be <see langword="null"/>.</param>
    private static void RebuildTargets(List<IFmgLibHotReload>? targets)
    {
        if (targets == null)
        {
            return;
        }

        var dispatch = DispatchOverride ?? (action => MainThread.BeginInvokeOnMainThread(action));

        foreach (var target in targets)
        {
            var current = target;

            try
            {
                dispatch(() =>
                {
                    try
                    {
                        current.Build();
                    }
                    catch (Exception exception)
                    {
                        // A rude/incompatible edit must never take the whole debug session down:
                        // report it and keep the app alive so the developer can fix and re-save.
                        Trace.TraceError($"FmgLib.MauiMarkup hot reload: rebuilding '{current.GetType()}' failed: {exception}");
                        ReloadFailed?.Invoke(current, exception);
                    }
                });
            }
            catch (Exception exception)
            {
                // Dispatcher not ready (update applied before the app fully started).
                Trace.TraceError($"FmgLib.MauiMarkup hot reload: could not dispatch rebuild of '{current.GetType()}': {exception}");
            }
        }
    }

    /// <summary>
    /// Decides whether a registered target is affected by the updated types. Everything rebuilds
    /// when <see cref="RebuildAllOnUpdate"/> is on (default) or when the update list is unknown or
    /// empty (the Mono runtime frequently reports no types). In targeted mode a target rebuilds
    /// when an updated type is its own type or one of its base types (editing a shared base page
    /// refreshes every page derived from it).
    /// </summary>
    /// <param name="target">The registered page or view.</param>
    /// <param name="types">The types the runtime reports as updated.</param>
    /// <returns><see langword="true"/> when the target should rebuild.</returns>
    private static bool ShouldRebuild(IFmgLibHotReload target, Type[]? types)
    {
        if (RebuildAllOnUpdate || types == null || types.Length == 0)
        {
            return true;
        }

        var targetType = target.GetType();

        foreach (var type in types)
        {
            if (type.IsAssignableFrom(targetType))
            {
                return true;
            }
        }

        return false;
    }
#pragma warning restore CS8632
}
