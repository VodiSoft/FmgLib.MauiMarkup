[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(FmgLib.MauiMarkup.FmgLibHotReloadHandler))]
namespace FmgLib.MauiMarkup;

public static class FmgLibHotReloadHandler
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public static event Action<Type[]?>? UpdateApplicationEvent;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

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
        UpdateApplicationEvent?.Invoke(types);
    }
}
