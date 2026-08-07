#nullable enable

using System.Globalization;

namespace FmgLib.MauiMarkup;

/// <summary>
/// Identifies the direction of a binding in which a fluent converter delegate failed.
/// </summary>
public enum ConverterDirection
{
    /// <summary>Source to target (reading).</summary>
    Convert,

    /// <summary>Target to source (writing).</summary>
    ConvertBack
}

/// <summary>
/// Thrown when a fluent converter delegate (<c>Convert</c>, <c>ConvertBack</c>, <c>MultiConvert</c>,
/// <c>MultiConvertBack</c>) is declared with a parameter type the binding never produces.
/// <para>
/// Nothing in the binding pipeline catches this, so the text has to stand alone in a crash report: it names
/// the bound property, the path of the sub binding, and — inside a multi binding — which of the values it
/// was.
/// </para>
/// </summary>
public sealed class MauiMarkupConverterException : InvalidOperationException
{
    internal MauiMarkupConverterException(
        BindingSite? site,
        Type expectedType,
        Type? actualType,
        ConverterDirection direction,
        int? valueIndex)
        : base(Describe(site, expectedType, actualType, direction, valueIndex))
    {
        TargetType = site?.OwnerType;
        TargetProperty = site?.PropertyName;
        BindingPath = site?.PathAt(valueIndex);
        ExpectedType = expectedType;
        ActualType = actualType;
        Direction = direction;
        ValueIndex = valueIndex;
    }

    /// <summary>Type declaring the bound property, when known.</summary>
    public Type? TargetType { get; }

    /// <summary>Name of the bound property, when known.</summary>
    public string? TargetProperty { get; }

    /// <summary>Path of the sub binding that produced the value, when known.</summary>
    public string? BindingPath { get; }

    /// <summary>Parameter type declared on the converter delegate.</summary>
    public Type ExpectedType { get; }

    /// <summary>Runtime type of the value the binding supplied, or <see langword="null"/>.</summary>
    public Type? ActualType { get; }

    /// <summary>Direction that failed.</summary>
    public ConverterDirection Direction { get; }

    /// <summary>Zero based position inside a multi binding, or <see langword="null"/> for a single binding.</summary>
    public int? ValueIndex { get; }

    private static string Describe(
        BindingSite? site, Type expectedType, Type? actualType, ConverterDirection direction, int? valueIndex)
    {
        var target = site?.PropertyName is null
            ? "a bound property"
            : site.OwnerType is null ? site.PropertyName : $"{site.OwnerType.Name}.{site.PropertyName}";

        var path = site?.PathAt(valueIndex) is { } p ? $" (path \"{p}\")" : string.Empty;
        var slot = valueIndex is null ? string.Empty : $", value #{valueIndex}";
        var hint = direction == ConverterDirection.Convert
            ? "Give the parameter the type this source really produces."
            : "Give the parameter the type of the property being written back from.";

        return $"FmgLib.MauiMarkup: the {direction} delegate of {target}{path}{slot} was declared as " +
               $"'{expectedType.FullName}', but the binding supplied '{actualType?.FullName ?? "null"}'. {hint}";
    }
}

/// <summary>
/// Snapshot of the bound property, captured when a converter is created so that a failing delegate can
/// describe where it sits. Kept separate from the builder on purpose: a converter outlives the builder and
/// capturing the builder itself would keep the whole entry list alive for the lifetime of the binding.
/// </summary>
internal sealed class BindingSite
{
    public Type? OwnerType;

    public string? PropertyName;

    public Type? PropertyType;

    /// <summary>Paths of the sub bindings, in declaration order. Filled in once the builder is built.</summary>
    public string?[]? Paths;

    public string? PathAt(int? index)
    {
        if (Paths is null || Paths.Length == 0)
            return null;

        if (index is null)
            return Paths[0];

        return index.Value >= 0 && index.Value < Paths.Length ? Paths[index.Value] : null;
    }
}

/// <summary>
/// Unboxing helpers shared by every fluent converter.
/// <para>
/// Delegate parameters are written in terms of the type the developer has in mind, while the binding hands
/// over a boxed <see cref="object"/> whose runtime type comes from the source property. Where the two differ
/// only in width — a <c>byte</c> counter read as an <c>int</c>, an <c>int</c> read as a <c>double</c> — the
/// value is converted; where they differ in kind, the mistake is turned into
/// <see cref="MauiMarkupConverterException"/> rather than an unexplained cast failure.
/// </para>
/// </summary>
internal static class BindingValues
{
    public static bool TryUnbox<TValue>(object? value, out TValue result)
    {
        if (value is TValue typed)
        {
            result = typed;
            return true;
        }

        if (value is null)
        {
            result = default!;
            return default(TValue) is null;
        }

        if (value is IConvertible && typeof(IConvertible).IsAssignableFrom(typeof(TValue)))
        {
            try
            {
                result = (TValue)System.Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue), CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException)
            {
            }
        }

        result = default!;
        return false;
    }

    public static TValue Unbox<TValue>(
        object? value, BindingSite? site, ConverterDirection direction, int? valueIndex = null)
    {
        if (TryUnbox<TValue>(value, out var result))
            return result;

        throw new MauiMarkupConverterException(site, typeof(TValue), value?.GetType(), direction, valueIndex);
    }

    /// <summary>
    /// <see langword="true"/> when <paramref name="value"/> cannot be handed to a delegate parameter of
    /// <paramref name="expected"/> because the parameter is a non nullable value type and the source has
    /// nothing to give yet. Used to hold the target property at its current value instead of crashing.
    /// </summary>
    public static bool IsMissing(object? value, Type? expected)
    {
        if (ReferenceEquals(value, BindableProperty.UnsetValue))
            return true;

        if (value is not null)
            return false;

        return expected is not null && expected.IsValueType && Nullable.GetUnderlyingType(expected) is null;
    }
}
