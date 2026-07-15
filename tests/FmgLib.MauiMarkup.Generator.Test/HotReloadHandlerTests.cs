using System.Runtime.CompilerServices;
using FmgLib.MauiMarkup;

namespace FmgLib.MauiMarkup.Generator.Test;

/// <summary>
/// Locks down the hot reload pipeline semantics:
/// - a metadata update must rebuild registered pages regardless of how the runtime reports the
///   updated types (CoreCLR sends a type list; Mono on iOS/Android often sends an empty array),
/// - registration must be weak (pages stay collectible — the original leak bug),
/// - duplicate registration must be a no-op,
/// - a Build() that throws must never propagate (a bad edit must not kill the session).
/// </summary>
[TestFixture]
[NonParallelizable]
public class HotReloadHandlerTests
{
    private sealed class FakePage : IFmgLibHotReload
    {
        public int BuildCount;
        public Action? OnBuild;

        public void Build()
        {
            BuildCount++;
            OnBuild?.Invoke();
        }
    }

    private static readonly List<IFmgLibHotReload> registered = new();

    [SetUp]
    public void SetUp()
    {
        // Run rebuilds synchronously instead of on the MAUI main thread.
        FmgLibHotReloadHandler.DispatchOverride = action => action();
        FmgLibHotReloadHandler.RebuildAllOnUpdate = true;
    }

    [TearDown]
    public void TearDown()
    {
        foreach (var target in registered)
        {
            FmgLibHotReloadHandler.Unregister(target);
        }

        registered.Clear();
        FmgLibHotReloadHandler.DispatchOverride = null;
        FmgLibHotReloadHandler.RebuildAllOnUpdate = true;
    }

    private static FakePage RegisterPage()
    {
        var page = new FakePage();
        FmgLibHotReloadHandler.Register(page);
        registered.Add(page);
        return page;
    }

    private static void Update(Type[]? types) =>
        typeof(FmgLibHotReloadHandler)
            .GetMethod("UpdateApplication", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, new object?[] { types });

    [Test]
    public void Update_WithNullTypes_RebuildsRegisteredPage()
    {
        var page = RegisterPage();
        Update(null);
        Assert.That(page.BuildCount, Is.EqualTo(1));
    }

    [Test]
    public void Update_WithEmptyTypes_RebuildsRegisteredPage_MonoBehavior()
    {
        // Mono (iOS/Android) frequently invokes the handler with an empty type list.
        var page = RegisterPage();
        Update(Type.EmptyTypes);
        Assert.That(page.BuildCount, Is.EqualTo(1));
    }

    [Test]
    public void Update_WithUnrelatedTypes_StillRebuilds_ByDefault()
    {
        // Pages compose helper classes; editing a helper must refresh the page (default mode).
        var page = RegisterPage();
        Update(new[] { typeof(string) });
        Assert.That(page.BuildCount, Is.EqualTo(1));
    }

    [Test]
    public void TargetedMode_RebuildsOnlyMatchingAndBaseTypes()
    {
        FmgLibHotReloadHandler.RebuildAllOnUpdate = false;
        var page = RegisterPage();

        Update(new[] { typeof(string) });
        Assert.That(page.BuildCount, Is.Zero, "unrelated type must not rebuild in targeted mode");

        Update(new[] { typeof(FakePage) });
        Assert.That(page.BuildCount, Is.EqualTo(1), "own type must rebuild");

        Update(new[] { typeof(object) });
        Assert.That(page.BuildCount, Is.EqualTo(2), "base type must rebuild derived targets");

        Update(Type.EmptyTypes);
        Assert.That(page.BuildCount, Is.EqualTo(3), "empty list means 'unknown' and must rebuild");
    }

    [Test]
    public void Register_SameInstanceTwice_RebuildsOnce()
    {
        var page = RegisterPage();
        FmgLibHotReloadHandler.Register(page);

        Update(null);
        Assert.That(page.BuildCount, Is.EqualTo(1));
    }

    [Test]
    public void RebuildAll_RebuildsEveryRegisteredTarget_EvenInTargetedMode()
    {
        FmgLibHotReloadHandler.RebuildAllOnUpdate = false;
        var first = RegisterPage();
        var second = RegisterPage();

        FmgLibHotReloadHandler.RebuildAll();

        Assert.That(first.BuildCount, Is.EqualTo(1));
        Assert.That(second.BuildCount, Is.EqualTo(1));
    }

    [Test]
    public void ThrowingBuild_IsContained_AndReportedViaReloadFailed()
    {
        var page = RegisterPage();
        page.OnBuild = () => throw new InvalidOperationException("bad edit");

        (IFmgLibHotReload Target, Exception Error)? failure = null;
        void OnFailed(IFmgLibHotReload t, Exception e) => failure = (t, e);
        FmgLibHotReloadHandler.ReloadFailed += OnFailed;

        try
        {
            Assert.DoesNotThrow(() => Update(null));
            Assert.That(failure, Is.Not.Null);
            Assert.That(failure!.Value.Target, Is.SameAs(page));
            Assert.That(failure.Value.Error, Is.TypeOf<InvalidOperationException>());
        }
        finally
        {
            FmgLibHotReloadHandler.ReloadFailed -= OnFailed;
        }
    }

    [Test]
    public void Registration_IsWeak_PageStaysCollectible()
    {
        var before = FmgLibHotReloadHandler.LiveSubscriberCount;

        CreateCollectablePage();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.That(FmgLibHotReloadHandler.LiveSubscriberCount, Is.EqualTo(before),
            "a registered page must not be kept alive by the hot reload handler");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CreateCollectablePage()
    {
        var page = new FakePage();
        FmgLibHotReloadHandler.Register(page);
    }
}
