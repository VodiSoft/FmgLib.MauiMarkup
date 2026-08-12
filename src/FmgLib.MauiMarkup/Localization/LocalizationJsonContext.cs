#nullable enable

using System.Text.Json.Serialization;

namespace FmgLib.MauiMarkup;

/// <summary>
/// Source-generated <c>System.Text.Json</c> metadata for the localization file shape.
/// </summary>
/// <remarks>
/// Reflection-based <c>JsonSerializer.Deserialize&lt;T&gt;</c> is not trim- or AOT-safe: it produces
/// IL2026/IL3050 warnings in any consumer that enables trim analysis, and throws
/// <see cref="NotSupportedException"/> outright when an app sets
/// <c>JsonSerializerIsReflectionEnabledByDefault=false</c>. MAUI iOS Release builds trim by default, so
/// the localization loader is the last place that should depend on runtime reflection.
/// </remarks>
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = false, AllowTrailingCommas = true, ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip)]
[JsonSerializable(typeof(Dictionary<string, Dictionary<string, string>>), TypeInfoPropertyName = "LocalizationMap")]
internal sealed partial class LocalizationJsonContext : JsonSerializerContext
{
}
